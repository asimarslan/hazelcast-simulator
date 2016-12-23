// Copyright (c) 2008-2017, Hazelcast, Inc. All Rights Reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hazelcast.Core;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Operations;
using Hazelcast.Simulator.Test;
using Hazelcast.Simulator.Worker;
using log4net;

namespace Hazelcast.Simulator.Protocol.Processors
{
    public class OperationProcessor
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(OperationProcessor));
        public readonly OperationContext operationContext;
        private readonly ClientWorker worker;

        public OperationProcessor(IHazelcastInstance hazelcastInstance, SimulatorAddress workerAddress, ClientWorker worker)
        {
            operationContext = new OperationContext(hazelcastInstance, workerAddress, worker.Connector);
            this.worker = worker;
        }

        public Task<Response.Part[]> SubmitAsync(SimulatorMessage simulatorMessage)
            => Task.Run(async () => await ProcessMessage(simulatorMessage));

        private async Task<Response.Part[]> ProcessMessage(SimulatorMessage msg)
        {
            Logger.Debug($"Processing simulator message:{msg}");
            if (msg.OperationType == OperationType.TerminateWorker)
            {
                //SHUTDOWN requested
                worker.Shutdown();
            }
            if (msg.Destination.AddressLevel == AddressLevel.WORKER)
            {
                return await RunMessageAtWorkerAddressLevel(msg);
            }
            if (msg.Destination.AddressLevel == AddressLevel.TEST)
            {
                return await RunMessageAtTestAddressLevel(msg);
            }
            throw new NotSupportedException($"Not supported address level at .Net worker {msg.Destination.AddressLevel}");
        }

        private async Task<Response.Part[]> RunMessageAtTestAddressLevel(SimulatorMessage msg)
        {
            ISimulatorOperation simulatorOperation = msg.ToOperation();
            if (msg.Destination.TestIndex == 0)
            {
                //process On All Tests
                var taskList = new List<Task<Response.Part>>();
                foreach (TestContainer container in operationContext.Tests.Values)
                {
                    taskList.Add(simulatorOperation.Run(operationContext, container.TestAddress));
                }
                return await Task.WhenAll(taskList);
            }
            else
            {
                TestContainer tc;
                if (operationContext.Tests.TryGetValue(msg.Destination.TestIndex, out tc))
                {
                    //process On single Test
                    return new[] { await simulatorOperation.Run(operationContext, tc.TestAddress) };
                }
                else
                {
                    if (msg.OperationType == OperationType.StopTest)
                    {
                        return new[] { new Response.Part(worker.Connector.WorkerAddress, ResponseType.Success, null) };
                    }
                    else
                    {
                        return new[] { new Response.Part(worker.Connector.WorkerAddress, ResponseType.FailureTestNotFound, null) };
                    }
                }
            }
        }

        private async Task<Response.Part[]> RunMessageAtWorkerAddressLevel(SimulatorMessage msg)
        {
            ISimulatorOperation simulatorOperation = msg.ToOperation();
            Response.Part part = await simulatorOperation.Run(operationContext, worker.Connector.WorkerAddress);
            return new[] { part };
        }
    }
}