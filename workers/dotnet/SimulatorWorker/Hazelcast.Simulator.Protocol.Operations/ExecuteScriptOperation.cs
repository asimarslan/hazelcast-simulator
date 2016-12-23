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

using System.Threading.Tasks;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Processors;
using Newtonsoft.Json;

namespace Hazelcast.Simulator.Protocol.Operations
{
    public class ExecuteScriptOperation : AbstractWorkerOperation
    {
        [JsonProperty("command")]
        public readonly string command;

        [JsonProperty("fireAndForget")]
        private readonly bool fireAndForget;

        public override async Task<ResponseType> RunInternal(OperationContext operationContext, SimulatorAddress targetAddress)
        {
            if ("js:java.lang.System.exit(0);" == command)
            {
                operationContext.Connector.Shutdown();
            }
            return ResponseType.Success;
        }
    }
}