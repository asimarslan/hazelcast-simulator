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

using System;
using System.IO;
using System.Text;
using DotNetty.Buffers;
using Hazelcast.Simulator.Protocol.Operations;
using static Hazelcast.Simulator.Utils.Constants;

namespace Hazelcast.Simulator.Protocol.Core
{
    public static class SimulatorMessageCodec
    {
        private const uint MAGIC_BYTES = 0xA5E1CA57;
        private const int OFFSET_MAGIC_BYTES = INT_SIZE;
        private const int OFFSET_DST_ADDRESS = 2 * INT_SIZE;
        private const int OFFSET_SRC_ADDRESS = OFFSET_DST_ADDRESS + ADDRESS_SIZE;
        private const int OFFSET_MESSAGE_ID = OFFSET_SRC_ADDRESS + ADDRESS_SIZE;
        private const int HEADER_SIZE = 2 * INT_SIZE + LONG_SIZE + 2 * ADDRESS_SIZE;

        public static void EncodeByteBuf(this SimulatorMessage msg, IByteBuffer buffer)
        {
            byte[] data = Encoding.UTF8.GetBytes(msg.OperationData);

            buffer.WriteInt(HEADER_SIZE + data.Length);
            buffer.WriteUnsignedInt(MAGIC_BYTES);

            msg.Destination.EncodeByteBuf(buffer);
            msg.Source.EncodeByteBuf(buffer);

            buffer.WriteLong(msg.MessageId);
            buffer.WriteInt((int)msg.OperationType);

            buffer.WriteBytes(data);
        }

        public static SimulatorMessage DecodeSimulatorMessage(this IByteBuffer buffer)
        {
            int frameLength = buffer.ReadInt();
            int dataLength = frameLength - HEADER_SIZE;
            uint magicBytes = buffer.ReadUnsignedInt();
            if (magicBytes != MAGIC_BYTES)
            {
                throw new InvalidDataException("Invalid magic bytes for SimulatorMessage");
            }
            SimulatorAddress destination = buffer.DecodeSimulatorAddress();
            SimulatorAddress source = buffer.DecodeSimulatorAddress();
            long messageId = buffer.ReadLong();
            int operationTypeId = buffer.ReadInt();
            OperationType operationType;
            try
            {
                operationType = (OperationType)operationTypeId;
            }
            catch (Exception)
            {
                throw new InvalidDataException($"Invalid operation type id:{operationTypeId}");
            }
            string operationData = buffer.ReadSlice(dataLength).ToString(Encoding.UTF8);
            return new SimulatorMessage(destination, source, messageId, operationType, operationData);
        }

        public static bool IsSimulatorMessage(this IByteBuffer buffer)
        {
            return buffer.GetUnsignedInt(OFFSET_MAGIC_BYTES) == MAGIC_BYTES;
        }

        public static long GetSimulatorMessageId(this IByteBuffer buffer)
        {
            return buffer.GetLong(OFFSET_MESSAGE_ID);
        }

        public static int GetSimulatorMessageDestinationAddressLevel(this IByteBuffer buffer)
        {
            return buffer.GetInt(OFFSET_DST_ADDRESS);
        }

        public static int GetSimulatorMessageChildAddressIndex(this IByteBuffer buffer, int addressLevelValue)
        {
            return buffer.GetInt(OFFSET_DST_ADDRESS + (addressLevelValue + 1) * INT_SIZE);
        }

        public static SimulatorAddress GetSimulatorMessageSourceAddress(this IByteBuffer buffer)
        {
            return buffer.Slice(OFFSET_SRC_ADDRESS, ADDRESS_SIZE).DecodeSimulatorAddress();
        }
    }
}