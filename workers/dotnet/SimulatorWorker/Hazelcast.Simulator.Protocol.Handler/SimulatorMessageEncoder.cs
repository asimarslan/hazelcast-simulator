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

using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Hazelcast.Simulator.Protocol.Core;
using log4net;

namespace Hazelcast.Simulator.Protocol.Handler
{
    public class SimulatorMessageEncoder : MessageToByteEncoder<SimulatorMessage>
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SimulatorMessageEncoder));

        private readonly SimulatorAddress localAddress;
        private readonly SimulatorAddress targetAddress;

        public SimulatorMessageEncoder(SimulatorAddress localAddress, SimulatorAddress targetAddress)
        {
            this.localAddress = localAddress;
            this.targetAddress = targetAddress;
        }

        protected override void Encode(IChannelHandlerContext ctx, SimulatorMessage msg, IByteBuffer buf)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"[{msg.MessageId}] SimulatorMessageEncoder.encode() {localAddress} -> {targetAddress} - {msg}");
            }
            msg.EncodeByteBuf(buf);
        }
    }
}