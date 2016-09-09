using System.Collections.Generic;
using System.Threading.Tasks;
using Hazelcast.Simulator.Protocol.Processors;
using Hazelcast.Simulator.Test;
using Newtonsoft.Json;

namespace Hazelcast.Simulator.Protocol.Operations
{

    /// <summary>
    /// Creates a Simulator Test based on an index, a testId and a property map.
    //</summary>
    public class CreateTestOperation : ISimulatorOperation
    {
        [JsonProperty("testIndex")]
        private readonly int testIndex;

        [JsonProperty("testId")]
        private readonly string testId;

        [JsonProperty("properties")]
        private readonly IDictionary<string, string> properties;

        public CreateTestOperation(int testIndex, TestCase testCase)
        {
            this.testIndex = testIndex;
            this.testId = testCase.Id;
            this.properties = testCase.Properties;
        }

        public Task Run(OperationContext operationContext, ISimulatorOperation simulatorOperation)
        {
            throw new System.NotImplementedException();
        }
    }
}