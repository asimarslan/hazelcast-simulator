// Copyright (c) 2008-2016, Hazelcast, Inc. All Rights Reserved.
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

namespace Hazelcast.Simulator.Protocol.Processors
{
    public class OperationProcessor
    {
        private readonly OperationContext operationContext;
        private readonly ClientWorker worker;

        public OperationProcessor(IHazelcastInstance hazelcastInstance, SimulatorAddress workerAddress, ClientWorker worker)
        {
            this.operationContext = new OperationContext(hazelcastInstance, workerAddress, worker.Connector);
            this.worker = worker;
        }

        public Task<Response.Part[]> SubmitAsync(SimulatorMessage simulatorMessage)
            => Task.Run(async () => await this.ProcessMessage(simulatorMessage));

        private async Task<Response.Part[]> ProcessMessage(SimulatorMessage msg)
        {
            if (msg.OperationType == OperationType.TerminateWorker)
            {
                //SHUTDOWN requested
                this.worker.Shutdown();
            }
            if (msg.Destination.AddressLevel == AddressLevel.WORKER)
            {
                return await this.RunMessageAtWorkerAddressLevel(msg);
            }
            if (msg.Destination.AddressLevel == AddressLevel.TEST)
            {
                return await this.RunMessageAtTestAddressLevel(msg);
            }
            throw new NotSupportedException($"Not supported address level at .Net worker {msg.Destination.AddressLevel}");
        }

        private async Task<Response.Part[]> RunMessageAtTestAddressLevel(SimulatorMessage msg)
        {
            var simulatorOperation = msg.ToOperation();
            if (msg.Destination.TestIndex == 0)
            {
                //process On All Tests
                var taskList = new List<Task<Response.Part>>();
                foreach (var container in this.operationContext.Tests.Values)
                {
                    taskList.Add(simulatorOperation.Run(this.operationContext, container.TestAddress));
                }
                return await Task.WhenAll(taskList);
            }
            else
            {
                TestContainer tc;
                if (this.operationContext.Tests.TryGetValue(msg.Destination.TestIndex, out tc))
                {
                    //process On single Test
                    return new[] { await simulatorOperation.Run(this.operationContext, tc.TestAddress) };
                }
                else
                {
                    if (msg.OperationType == OperationType.StopTest)
                    {
                        return new[] { new Response.Part(this.worker.Connector.WorkerAddress, ResponseType.Success, null) };
                    }
                    else
                    {
                        return new[] { new Response.Part(this.worker.Connector.WorkerAddress, ResponseType.FailureTestNotFound, null) };
                    }
                }
            }
        }

        private async Task<Response.Part[]> RunMessageAtWorkerAddressLevel(SimulatorMessage msg)
        {
            var simulatorOperation = msg.ToOperation();
            Response.Part part = await simulatorOperation.Run(this.operationContext, this.worker.Connector.WorkerAddress);
            return new[] { part };
        }
    }
}