using System.Collections.Generic;
using System.Threading.Tasks;
using Hazelcast.Simulator.Protocol.Processors;
using Hazelcast.Simulator.Test;
using Newtonsoft.Json;

namespace Hazelcast.Simulator.Protocol.Operations
{

    ///
    /// Creates a Simulator Test based on an index, a testId and a property map.
    ///
    public class CreateTestOperation : ISimulatorOperation
    {
        [JsonProperty("testIndex")]
        public readonly int TestIndex;

        [JsonProperty("testId")]
        public readonly string TestId;

        [JsonIgnore]
        public readonly IDictionary<string, string> Properties;

        public CreateTestOperation(int testIndex, TestCase testCase)
        {
            this.TestIndex = testIndex;
            this.TestId = testCase.Id;
            this.Properties = testCase.Properties;
        }

        public Task Run(OperationContext operationContext, ISimulatorOperation simulatorOperation)
        {
            throw new System.NotImplementedException();
        }
    }
}