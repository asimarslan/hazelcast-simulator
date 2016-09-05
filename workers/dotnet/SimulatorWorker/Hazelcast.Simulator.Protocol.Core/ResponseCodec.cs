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
        private const int HEADER_SIZE = INT_SIZE + LONG_SIZE + ADDRESS_SIZE;
        private const int DATA_ENTRY_SIZE = ADDRESS_SIZE + INT_SIZE;

        public static void EncodeByteBuf(this Response response, IByteBuffer buffer)
        {
            buffer.WriteInt(HEADER_SIZE + response.Size() * DATA_ENTRY_SIZE);
            buffer.WriteInt(MAGIC_BYTES);

            buffer.WriteLong(response.MessageId);
            SimulatorAddressCodec.encodeByteBuf(response.getDestination(), buffer);

            for (Map.Entry < SimulatorAddress, ResponseType > entry : response.entrySet())
            {
                SimulatorAddressCodec.encodeByteBuf(entry.getKey(), buffer);
                buffer.writeInt(entry.getValue().toInt());
            }
        }

        public static Response decodeResponse(IByteBuffer buffer)
        {
            int frameLength = buffer.readInt();
            int dataLength = (frameLength - HEADER_SIZE) / DATA_ENTRY_SIZE;

            if (buffer.readInt() != MAGIC_BYTES)
            {
                throw new IllegalArgumentException("Invalid magic bytes for Response");
            }

            long messageId = buffer.readLong();
            SimulatorAddress destination = decodeSimulatorAddress(buffer);
            Response response = new Response(messageId, destination);

            for (int i = 0; i < dataLength; i++)
            {
                SimulatorAddress source = decodeSimulatorAddress(buffer);
                ResponseType responseType = ResponseType.fromInt(buffer.readInt());
                response.addResponse(source, responseType);
            }

            return response;
        }

        public static boolean isResponse(IByteBuffer buffer)
        {
            return (in.
            getInt(OFFSET_MAGIC_BYTES) == MAGIC_BYTES)
            ;
        }

        public static long getMessageId(IByteBuffer buffer)
        {
            return in.
            getLong(OFFSET_MESSAGE_ID);
        }

        public static int getDestinationAddressLevel(IByteBuffer buffer)
        {
            return in.
            getInt(OFFSET_DST_ADDRESS);
        }

        public static int getChildAddressIndex(IByteBuffer buffer, int addressLevelValue)
        {
            return in.
            getInt(OFFSET_DST_ADDRESS + ((addressLevelValue + 1) * INT_SIZE));
        }
    }
}