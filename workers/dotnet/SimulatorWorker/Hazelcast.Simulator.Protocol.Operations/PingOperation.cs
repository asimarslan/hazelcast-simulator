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
using log4net;

namespace Hazelcast.Simulator.Protocol.Operations
{
    /// <summary>
    ///     Creates traffic on the wire, so the WorkerProcessFailureMonitor
    ///     on the Agent can see, that the Worker is still responsive.
    ///     This is needed for long running test phases, which lead to a radio silence on the wire.
    /// </summary>
    public class PingOperation : AbstractWorkerOperation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(PingOperation));

        public override async Task<ResponseType> RunInternal(OperationContext ctx, SimulatorAddress targetAddress)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"Pinged by {sourceAddress} ...");
            }
            return ResponseType.Success;
        }
    }
}