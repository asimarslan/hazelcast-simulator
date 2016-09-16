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
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Operations;
using Newtonsoft.Json;

namespace Hazelcast.Simulator.Protocol.Processors
{
    public class OperationProcessor
    {
        private OperationContext operationContext;


        public void SubmitAsync(SimulatorMessage simulatorMessage)
        {
            Task.Run(async () => await this.ProcessMessage(simulatorMessage));
        }

        protected async Task ProcessMessage(SimulatorMessage msg)
        {
            var simulatorOperation = JsonConvert.DeserializeObject(msg.OperationData, msg.OperationType.GetClassType())
                as ISimulatorOperation;

            await this.ExecuteOperation(simulatorOperation);
        }

        private async Task ExecuteOperation(ISimulatorOperation simulatorOperation)
        {
//            InitOperation(simulatorOperation);
            await simulatorOperation.Run(this.operationContext, simulatorOperation);
        }

//        private void InitOperation(ISimulatorOperation simulatorOperation)
//        {
//            simulatorOperation.Init();

//        }
    }
}