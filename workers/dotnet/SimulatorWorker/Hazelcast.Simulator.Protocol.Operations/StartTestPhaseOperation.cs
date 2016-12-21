using System;
using System.Threading.Tasks;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Processors;
using Hazelcast.Simulator.Test;
using log4net;
using Newtonsoft.Json;

namespace Hazelcast.Simulator.Protocol.Operations
{
    /// <summary>
    /// Starts a <see cref="TestPhase"/> of the addressed Simulator Test.
    /// </summary>
    public class StartTestPhaseOperation : AbstractStartOperation
    {
        [JsonProperty("testPhase")]
        private readonly string testPhaseStr;

        public StartTestPhaseOperation(TestPhase testPhase)
        {
            this.testPhaseStr = testPhase.GetName();
        }

        protected override void StartPhase(OperationContext operationContext, TestContainer testContainer)
        {
            var testPhase = this.GetTestPhase();
            Logger.Info($"Starting  phase{testPhase.GetDescription()} for test:{testContainer.TestCase.TestId}");
            testContainer.Invoke(testPhase);
        }

        protected override TestPhase GetTestPhase() => this.testPhaseStr.ToTestPhase();

    }
}