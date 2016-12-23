// Copyright (c) 2008-2017, Hazelcast, Inc. All Rights Reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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
            MessageId = messageId;
            Destination = destination;
        }

        public int Size() => Parts.Count;

        public override string ToString() => $"[Response: MessageId={MessageId}, Destination={Destination}], Parts={string.Join(", ", Parts.Values.Select(x => x.ToString()))}";

        public Response AddPart(SimulatorAddress sourceAddress, ResponseType responseType, string payload)
        {
            AddPart(new Part(sourceAddress, responseType, payload));
            return this;
        }

        public Response AddPart(Part part)
        {
            Parts.Add(part.SourceAddress, part);
            return this;
        }

        public class Part
        {
            public SimulatorAddress SourceAddress { get; }

            public ResponseType ResponseType { get; }

            public string Payload { get; }

            public Part(SimulatorAddress sourceAddress, ResponseType responseType, string payload = null)
            {
                SourceAddress = sourceAddress;
                ResponseType = responseType;
                Payload = payload;
            }

            public override string ToString() => $"Part[type={ResponseType}, payload={Payload}";
        }
    }
}