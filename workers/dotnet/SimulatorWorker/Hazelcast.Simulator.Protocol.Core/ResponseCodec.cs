using System;
using System.Collections.Generic;
using System.Text;
using DotNetty.Buffers;
using static Hazelcast.Simulator.Utils.Constants;

namespace Hazelcast.Simulator.Protocol.Core
{
    public static class ResponseCodec
    {
        private const int MAGIC_BYTES = 0x3E5D0B5E;

        private const int OFFSET_MAGIC_BYTES = INT_SIZE;
        private const int OFFSET_MESSAGE_ID = OFFSET_MAGIC_BYTES + INT_SIZE;
        private const int OFFSET_DST_ADDRESS = OFFSET_MESSAGE_ID + LONG_SIZE;

        public static void EncodeByteBuf(this Response response, IByteBuffer buffer)
        {
            var parts = response.Parts;

            // write place holder for length. Eventually we'll overwrite it with the correct length.
            buffer.WriteInt(0);
            var start = buffer.WriterIndex;

            buffer.WriteUnsignedInt(MAGIC_BYTES);
            buffer.WriteLong(response.MessageId);
            response.Destination.EncodeByteBuf(buffer);

            buffer.WriteInt(parts.Count);
            foreach (KeyValuePair<SimulatorAddress, Response.Part> pair in parts)
            {
                pair.Key.EncodeByteBuf(buffer);
                var part = pair.Value;
                buffer.WriteInt((int) part.ResponseType);

                var payload = part.Payload;
                if (payload == null)
                {
                    buffer.WriteInt(-1);
                }
                else
                {
                    var data = Encoding.UTF8.GetBytes(payload);
                    buffer.WriteInt(data.Length);
                    buffer.WriteBytes(data);
                }
            }

            var length = buffer.WriterIndex - start;
            buffer.SetInt(start - INT_SIZE, length);
        }

        public static Response DecodeResponse(this IByteBuffer buffer)
        {
            buffer.ReadInt(); //frameLength

            if (buffer.ReadUnsignedInt() != MAGIC_BYTES)
            {
                throw new ArgumentException("Invalid magic bytes for Response");
            }

            var messageId = buffer.ReadLong();
            var destination = buffer.DecodeSimulatorAddress();
            var response = new Response(messageId, destination);

            var partCount = buffer.ReadInt();
            for (var i = 0; i < partCount; i++)
            {
                var source = buffer.DecodeSimulatorAddress();
                var responseType = (ResponseType) buffer.ReadInt();

                string payload = null;
                var size = buffer.ReadInt();
                if (size > -1)
                {
                    payload = buffer.ReadSlice(size).ToString(Encoding.UTF8);
                }

                response.AddPart(source, responseType, payload);
            }

            return response;
        }

        public static bool IsResponse(this IByteBuffer buffer)
        {
            return buffer.GetInt(OFFSET_MAGIC_BYTES) == MAGIC_BYTES;
        }

        public static long GetResponseMessageId(this IByteBuffer buffer)
        {
            return buffer.GetLong(OFFSET_MESSAGE_ID);
        }

        public static int GetResponseDestinationAddressLevel(this IByteBuffer buffer)
        {
            return buffer.GetInt(OFFSET_DST_ADDRESS);
        }

        public static int GetResponseChildAddressIndex(this IByteBuffer buffer, int addressLevelValue)
        {
            return buffer.GetInt(OFFSET_DST_ADDRESS + (addressLevelValue + 1) * INT_SIZE);
        }
    }
}