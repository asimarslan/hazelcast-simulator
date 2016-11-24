using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Processors;
using Hazelcast.Simulator.Test;
using log4net;
using Newtonsoft.Json;

namespace Hazelcast.Simulator.Protocol.Operations
{
    /// <summary>
    /// Creates a Simulator Test based on an index, a testId and a property map.
    //</summary>
    public class CreateTestOperation : AbstractWorkerOperation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CreateTestOperation));

        [JsonProperty("testIndex")]
        private readonly int testIndex;

        [JsonProperty("testId")]
        private readonly string testId;

        [JsonProperty("properties")]
        private readonly IDictionary<string, string> properties;

        public CreateTestOperation()
        {
        }

        public CreateTestOperation(int testIndex, TestCase testCase)
        {
            this.testIndex = testIndex;
            this.testId = testCase.TestId;
            this.properties = testCase.Properties;
        }

        public override async Task<ResponseType> RunInternal(OperationContext ctx, SimulatorAddress targetAddress)
        {
            var testCase = new TestCase(this.testId, this.properties);
            Logger.Info($"Initializing test {testCase}");

            var testContext = new TestContext(this.testId, ctx.HazelcastInstance, ctx.Connector);
            var testAddress = ctx.WorkerAddress.GetChild(this.testIndex);
            var testContainer = new TestContainer(testContext, testCase, testAddress);
            if (!ctx.Tests.TryAdd(this.testIndex, testContainer))
            {
                throw new InvalidOperationException(
                    $"Can't init {testCase}, another test with testId:{this.testId} already exists");
            }
            return ResponseType.Success;
        }
    }
}