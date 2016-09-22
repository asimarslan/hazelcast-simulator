using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Processors;
using Hazelcast.Simulator.Test;
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

        public async Task Run(OperationContext operationContext, SimulatorMessage msg)
        {
            TestContainer testContainer;
            if (!operationContext.Tests.TryGetValue(msg.Destination.TestIndex, out testContainer))
            {
                throw new InvalidOperationException($"Test not created yet with testIndex:{msg.Destination.TestIndex}");
            }
            TestPhase testPhase = this.warmup ? TestPhase.Warmup : TestPhase.Run;

            if (this.SkipRunPhase())
            {
                this.SendPhaseCompletedOperation(testPhase);
                return;
            }

            await testContainer.Invoke(testPhase);
        }

        private bool SkipRunPhase()
        {
            throw new NotImplementedException();
        }

        private void SendPhaseCompletedOperation(TestPhase tp)
        {
            throw new NotImplementedException();
        }
    }
}