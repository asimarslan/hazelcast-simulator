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

namespace Hazelcast.Simulator.Protocol.Core
{
    public static class SimulatorAddressCodec
    {
        public static void EncodeByteBuf(this SimulatorAddress address, IByteBuffer buffer)
        {
            buffer.WriteInt((int)address.AddressLevel)
                .WriteInt(address.AgentIndex)
                .WriteInt(address.WorkerIndex)
                .WriteInt(address.TestIndex);
        }

        public static SimulatorAddress DecodeSimulatorAddress(this IByteBuffer buffer)
        {
            return new SimulatorAddress(
                (AddressLevel)buffer.ReadInt(),
                buffer.ReadInt(),
                buffer.ReadInt(),
                buffer.ReadInt()
            );
        }
    }
}