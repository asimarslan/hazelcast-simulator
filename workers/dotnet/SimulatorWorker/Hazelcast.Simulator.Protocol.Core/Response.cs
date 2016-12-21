using System.Collections.Generic;
using System.Linq;

namespace Hazelcast.Simulator.Protocol.Core
{
    public class Response
    {
        public long MessageId { get; }
        public SimulatorAddress Destination { get; }

        public IDictionary<SimulatorAddress, Part> Parts { get; } = new Dictionary<SimulatorAddress, Part>();

        public Response(long messageId, SimulatorAddress destination)
        {
            this.MessageId = messageId;
            this.Destination = destination;
        }

        public int Size() => this.Parts.Count;

        public override string ToString() => $"[Response: MessageId={this.MessageId}, Destination={this.Destination}], Parts={string.Join(", ", Parts.Values.Select(x => x.ToString()))}";

        public Response AddPart(SimulatorAddress sourceAddress, ResponseType responseType, string payload)
        {
            this.AddPart(new Part(sourceAddress, responseType, payload));
            return this;
        }

        public Response AddPart(Part part)
        {
            this.Parts.Add(part.SourceAddress, part);
            return this;
        }

        public class Part
        {
            public SimulatorAddress SourceAddress { get; }
            public ResponseType ResponseType { get; }
            public string Payload { get; }

            public Part(SimulatorAddress sourceAddress, ResponseType responseType, string payload=null)
            {
                this.SourceAddress = sourceAddress;
                this.ResponseType = responseType;
                this.Payload = payload;
            }

            public override string ToString() => $"Part[type={this.ResponseType}, payload={this.Payload}";
        }
    }
}