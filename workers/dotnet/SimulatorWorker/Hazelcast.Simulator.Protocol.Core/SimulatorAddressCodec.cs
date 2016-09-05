using DotNetty.Buffers;

namespace Hazelcast.Simulator.Protocol.Core
{
    public static class SimulatorAddressCodec
    {
        public static void EncodeByteBuf(this SimulatorAddress address, IByteBuffer buffer)
        {
            buffer.WriteInt((int) address.AddressLevel)
                .WriteInt(address.AgentIndex)
                .WriteInt(address.WorkerIndex)
                .WriteInt(address.TestIndex);
        }

        public static SimulatorAddress DecodeSimulatorAddress(this IByteBuffer buffer)
        {
            return new SimulatorAddress(
                (AddressLevel) buffer.ReadInt(),
                buffer.ReadInt(),
                buffer.ReadInt(),
                buffer.ReadInt()
            );
        }
    }
}