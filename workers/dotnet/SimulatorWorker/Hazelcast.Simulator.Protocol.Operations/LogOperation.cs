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
using log4net.Core;
using Newtonsoft.Json;

namespace Hazelcast.Simulator.Protocol.Operations
{
    /// <summary>
    ///     Writes the message with the requested log level to the local logging framework.
    /// </summary>
    public class LogOperation : AbstractWorkerOperation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LogOperation));

        /// <summary>
        ///     Defines the message which should be logged.
        /// </summary>
        [JsonProperty("message")]
        private readonly string message;

        /// <summary>
        ///     Defines the desired log level of the message.
        /// </summary>
        [JsonProperty("level")]
        private readonly string level;

        public LogOperation() {}

        public LogOperation(string message)
            : this(message, Level.Info) {}

        public LogOperation(string message, Level level)
        {
            this.message = message;
            this.level = level.ToString();
        }

        public override async Task<ResponseType> RunInternal(OperationContext operationContext, SimulatorAddress targetAddress)
        {
            Logger.Logger.Log(typeof(LogOperation), GetLevel(), $"[{SourceAddress}] {message}", null);
            return ResponseType.Success;
        }

        private Level GetLevel() => LogManager.GetRepository().LevelMap[level];

        public string GetMessage() => message;
    }
}