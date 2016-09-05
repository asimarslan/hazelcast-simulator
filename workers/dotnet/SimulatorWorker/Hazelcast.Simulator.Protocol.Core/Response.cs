using System.Collections.Generic;

namespace Hazelcast.Simulator.Protocol.Core
{
    public class Response
    {
        public long MessageId { get; }
        public SimulatorAddress Destination { get; }

        private IDictionary<SimulatorAddress, ResponseType> responses

        public Response(long messageId, SimulatorAddress destination)
        {
            MessageId = messageId;
            Destination = destination;
        }

        public override string ToString()
        {
            return $"[Response: MessageId={MessageId}, Destination={Destination}]";
        }
    }
}