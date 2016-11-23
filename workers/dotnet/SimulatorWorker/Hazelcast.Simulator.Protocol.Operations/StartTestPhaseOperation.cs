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

        protected override async Task<ResponseType> RunInternal(OperationContext operationContext, TestContainer testContainer)
        {
            var testPhase = this.GetTestPhase();
            try
            {
                Logger.Info($"Starting test {testContainer.TestCase.TestId}");
                await testContainer.Invoke(testPhase);
            }
            finally
            {
                if (testPhase == TestPhases.GetLastTestPhase())
                {
                    operationContext.Tests.TryRemove(this.msg.Destination.TestIndex, out testContainer);
                }
            }
            return ResponseType.Success;
        }

        protected override TestPhase GetTestPhase() => this.testPhaseStr.ToTestPhase();

    }
}