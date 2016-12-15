using System;
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
            if ("js:java.lang.System.exit(0);" == this.command)
            {
                Environment.Exit(0);
            }
            return ResponseType.Success;
        }
    }
}