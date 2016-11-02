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
    public class CreateTestOperation : ISimulatorOperation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CreateTestOperation));

        [JsonProperty("testIndex")]
        public int TestIndex { get; }

        [JsonProperty("testId")]
        public string TestId { get; }

        [JsonProperty("properties")]
        public IDictionary<string, string> Properties { get; }

        [JsonIgnore]
        public TestCase testCase { get; }

        [JsonIgnore]
        public string PublicIpAddress { get; set; }

        public CreateTestOperation()
        {
        }

        public CreateTestOperation(int testIndex, TestCase testCase)
        {
            this.TestIndex = testIndex;
            this.TestId = testCase.Id;
            this.Properties = testCase.Properties;
            this.testCase = new TestCase(this.TestId, this.Properties);
        }

        public async Task<ResponseResult> Run(OperationContext ctx)
        {
            Logger.Info($"Initializing test {this.testCase}");

            var testContext= new TestContext(this.TestId, ctx.HazelcastInstance, this.PublicIpAddress);
            if (!ctx.Tests.TryAdd(this.TestIndex, new TestContainer(testContext, this.testCase)))
            {
                throw new InvalidOperationException($"Can't init {this.testCase}, another test with testId {this.TestId} already exists");
            }
            return new ResponseResult(ResponseType.Success);
        }
    }
}