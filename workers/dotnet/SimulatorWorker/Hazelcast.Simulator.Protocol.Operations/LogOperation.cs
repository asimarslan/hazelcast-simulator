using System.Threading.Tasks;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Processors;
using Newtonsoft.Json;

namespace Hazelcast.Simulator.Protocol.Operations
{
    /// <summary>
    /// Writes the message with the requested log level to the local logging framework.
    /// </summary>
    public class LogOperation :ISimulatorOperation
    {

        /// <summary>
        /// Defines the message which should be logged.
        /// </summary>
        [JsonProperty("message")]
        private readonly string message;

        /// <summary>
        /// Defines the desired log level of the message.
        /// </summary>
        [JsonProperty("level")]
        private readonly string level;


        public Task Run(OperationContext operationContext, SimulatorMessage msg)
        {
            throw new System.NotImplementedException();
        }
    }
}