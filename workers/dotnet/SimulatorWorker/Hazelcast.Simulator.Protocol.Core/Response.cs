using System.Collections.Generic;

namespace Hazelcast.Simulator.Protocol.Core
{
    public class Response
    {
        public long MessageId { get; }
        public SimulatorAddress Destination { get; }

        public IDictionary<SimulatorAddress, Part> Parts { get; } = new Dictionary<SimulatorAddress, Part>();

        public Response(long messageId, SimulatorAddress destination)
        {
            MessageId = messageId;
            Destination = destination;
        }

        public int Size()
        {
            return Parts.Count;
        }

        public override string ToString()
        {
            return $"[Response: MessageId={MessageId}, Destination={Destination}]";
        }

        public Response AddPart(SimulatorAddress address, ResponseType responseType, string payload)
        {
            Parts.Add(address, new Part(responseType, payload));
            return this;
        }
    }

    public class Part
    {
        public ResponseType ResponseType { get; }
        public string Payload { get; }

        public Part(ResponseType responseType, string payload)
        {
            ResponseType = responseType;
            Payload = payload;
        }

        public override string ToString()
        {
            return $"Part[type={ResponseType}, payload={Payload}";
        }
    }
}