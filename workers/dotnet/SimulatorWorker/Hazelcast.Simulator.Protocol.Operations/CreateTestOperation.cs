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
    /// <summary>
    /// Creates a Simulator Test based on an index, a testId and a property map.
    //</summary>
    public class CreateTestOperation : AbstractWorkerOperation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CreateTestOperation));

        [JsonProperty("testIndex")]
        public int TestIndex { get; }

        [JsonProperty("testId")]
        public string TestId { get; }

        [JsonProperty("properties")]
        public IDictionary<string, string> Properties { get; }

//        [JsonIgnore]
//        private WorkerConnector connector;


        public CreateTestOperation()
        {
        }

//        public CreateTestOperation(int testIndex, TestCase testCase)
//        {
//            this.TestIndex = testIndex;
//            this.TestId = testCase.TestId;
//            this.Properties = testCase.Properties;
//            this.testCase = new TestCase(this.TestId, this.Properties);
//        }

        public async Task<Response.Part> Run(OperationContext ctx)
        {
            var testCase = new TestCase(this.TestId, this.Properties);
            Logger.Info($"Initializing test {testCase}");

            var testContext= new TestContext(this.TestId, ctx.HazelcastInstance, this.Connector);
            var testContainer = new TestContainer(testContext, testCase, this.Connector.WorkerAddress.GetChild(TestIndex));
            if (!ctx.Tests.TryAdd(this.TestIndex, testContainer))
            {
                throw new InvalidOperationException($"Can't init {testCase}, another test with testId {this.TestId} already exists");
            }
            return new Response.Part(this.TargetAddress, ResponseType.Success, null);
        }

    }
}