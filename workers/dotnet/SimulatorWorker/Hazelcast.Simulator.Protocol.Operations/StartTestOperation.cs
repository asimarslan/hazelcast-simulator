using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hazelcast.Simulator.Protocol.Connector;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Processors;
using Hazelcast.Simulator.Test;
using log4net;
using Newtonsoft.Json;

namespace Hazelcast.Simulator.Protocol.Operations
{
    ///<summary>
    /// Starts the <see cref="TestPhase.Run"/> phase of a Simulator Test.
    ///</summary>
    public class StartTestOperation : AbstractStartOperation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CreateTestOperation));

        [JsonProperty("targetType")]
        private readonly string targetType;

        [JsonProperty("targetWorkers")]
        private readonly List<string> targetWorkers;

        [JsonProperty("warmup")]
        private readonly bool warmup;

        protected override async Task<ResponseType> StartPhase(OperationContext operationContext, TestContainer testContainer)
        {
            var testPhase = this.GetTestPhase();
            if (this.SkipRunPhase(testContainer))
            {
                Logger.Info($"Skipping test {testContainer.TestCase.TestId}");
                await this.SendPhaseCompletedOperation(operationContext.Connector, testPhase);
            }
            else
            {
                Logger.Info($"Starting test {testContainer.TestCase.TestId}");
                await testContainer.Invoke(testPhase);
            }
            return ResponseType.Success;
        }

        protected override TestPhase GetTestPhase() => this.warmup ? TestPhase.Warmup : TestPhase.Run;

        private bool SkipRunPhase(TestContainer testContainer)
        {
            if (!this.MatchesTargetType())
            {
                Logger.Info($"Skipping test ({this.targetType} Worker does not match .Net Client) {testContainer.TestCase.TestId}");
                return true;
            }
            if (!this.MatchesTargetWorkers(testContainer.TestAddress.GetParent()))
            {
                Logger.Info($"Skipping test (Worker is not on target list) {testContainer.TestCase.TestId}");
                return true;
            }
            return false;
        }

        private bool MatchesTargetWorkers(SimulatorAddress workerAddress)
            => this.targetWorkers.Count == 0 || this.targetWorkers.Contains(workerAddress.ToString());

        private bool MatchesTargetType() => this.targetType == "ALL" || this.targetType == "CLIENT";

    }
}