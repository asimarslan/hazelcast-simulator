using System.Threading.Tasks;
using Hazelcast.Simulator.Protocol.Processors;
using Newtonsoft.Json;

namespace Hazelcast.Simulator.Protocol.Operations
{
    public class ExecuteScriptOperation :ISimulatorOperation
    {
        [JsonProperty("command")]
        public readonly string command;

        [JsonProperty("fireAndForget")]
        private readonly bool fireAndForget;

        public Task Run(OperationContext operationContext)
        {
            throw new System.NotImplementedException();
        }
    }
}