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
using System.IO;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Hazelcast.Simulator.Protocol.Core;
using log4net;

namespace Hazelcast.Simulator.Protocol.Handler
{
    public class SimulatorProtocolDecoder : ByteToMessageDecoder
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SimulatorProtocolDecoder));

        private readonly SimulatorAddress localAddress;

        public SimulatorProtocolDecoder(SimulatorAddress localAddress)
        {
            this.localAddress = localAddress;
        }

        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            if (Unpooled.Empty.Equals(input))
            {
                return;
            }
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"SimulatorProtocolDecoder.decode() {localAddress.AddressLevel}, {localAddress}");
            }
            if (input.IsSimulatorMessage())
            {
                output.Add(input.DecodeSimulatorMessage());
            }
            else if (input.IsResponse())
            {
                output.Add(input.DecodeResponse());
            }
            else
            {
                string logMsg = $"Invalid message magic bytes do not match. {input}";
                Logger.Error(logMsg);
                throw new InvalidDataException(logMsg);
            }
        }
    }
}