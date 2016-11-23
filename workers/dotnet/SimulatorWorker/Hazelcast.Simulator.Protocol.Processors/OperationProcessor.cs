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

using System.Collections.Generic;
using System.Threading.Tasks;
using Hazelcast.Core;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Operations;
using Hazelcast.Simulator.Test;
using Hazelcast.Simulator.Worker;
using Newtonsoft.Json;

namespace Hazelcast.Simulator.Protocol.Processors
{
    public class OperationProcessor
    {
        private readonly OperationContext operationContext;
        private readonly ClientWorker worker;

        public OperationProcessor(IHazelcastInstance hazelcastInstance, SimulatorAddress workerAddress, ClientWorker worker)
        {
            this.operationContext = new OperationContext(hazelcastInstance, workerAddress);
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

            var simulatorOperation = JsonConvert.DeserializeObject(msg.OperationData, msg.OperationType.GetClassType());
            (simulatorOperation as ISimulatorMessageAware)?.SetSimulatorMessage(msg);
            (simulatorOperation as IConnectorAware)?.SetConnector(this.worker.Connector);

            if (msg.Destination.TestIndex == 0)
            {
                //process On All Tests
                var taskList = new List<Task<Response.Part>>();
                foreach (KeyValuePair<int,TestContainer> pair in this.operationContext.Tests)
                {
                    var testIndex = pair.Key;
                    var container = pair.Value;
                    taskList.Add(((ISimulatorOperation)simulatorOperation).Run(this.operationContext));

                }
                return await Task.WhenAll(taskList);
            }
            else
            {
                //process On single Test
                return await Task.WhenAll(this.RunOperation((ISimulatorOperation)simulatorOperation));
            }

        }

        private Task<Response.Part> RunOperation(ISimulatorOperation simulatorOperation)
        {
            var responseType = simulatorOperation.Run(this.operationContext);

        }

    }
}