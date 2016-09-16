using System.Collections.Generic;
using System.Threading.Tasks;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Processors;
using Newtonsoft.Json;

namespace Hazelcast.Simulator.Protocol.Operations
{
    /**
     * Starts the TestPhase.RUN phase of a Simulator Test.
     */

    public class StartTestOperation : ISimulatorOperation
    {
        [JsonProperty("targetType")]
        private readonly string targetType;

        [JsonProperty("targetWorkers")]
        private readonly List<string> targetWorkers;

        [JsonProperty("warmup")]
        private readonly bool warmup;

        public bool MatchesTargetWorkers(SimulatorAddress workerAddress) => this.targetWorkers.Count == 0 || this.targetWorkers.Contains(workerAddress.ToString());

        public Task Run(OperationContext operationContext, ISimulatorOperation simulatorOperation)
        {
            throw new System.NotImplementedException();
        }
    }
}