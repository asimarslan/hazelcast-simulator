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

using System.Threading.Tasks;
using Hazelcast.Core;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Operations;
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

        public Task<ResponseResult> SubmitAsync(SimulatorMessage simulatorMessage)
        {
            return Task.Run(async () => { return await this.ProcessMessage(simulatorMessage); });
        }

        private async Task<ResponseResult> ProcessMessage(SimulatorMessage msg)
        {
            var simulatorOperation = JsonConvert.DeserializeObject(msg.OperationData, msg.OperationType.GetClassType())
                as ISimulatorOperation;
            (simulatorOperation as ISimulatorMessageAware)?.SetSimulatorMessage(msg);
            return await this.ExecuteOperation(simulatorOperation);
        }

        private async Task<ResponseResult> ExecuteOperation(ISimulatorOperation simulatorOperation)
        {
            return await simulatorOperation.Run(this.operationContext);
        }
    }
}