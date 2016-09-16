using System.Collections.Generic;
using System.Threading.Tasks;
using Hazelcast.Simulator.Protocol.Connector;
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
        private readonly int testIndex;

        [JsonProperty("testId")]
        private readonly string testId;

        [JsonProperty("properties")]
        private readonly IDictionary<string, string> properties;

        [JsonIgnore]
        private readonly TestCase testCase;

        public CreateTestOperation(int testIndex, TestCase testCase)
        {
            this.testIndex = testIndex;
            this.testId = testCase.Id;
            this.properties = testCase.Properties;
            this.testCase = new TestCase(this.testId, this.properties);
        }

        public Task Run(OperationContext opCtx, ISimulatorOperation simulatorOperation)
        {
            Logger.Info(this.testCase.ToString());

            opCtx.
        }
    }
}