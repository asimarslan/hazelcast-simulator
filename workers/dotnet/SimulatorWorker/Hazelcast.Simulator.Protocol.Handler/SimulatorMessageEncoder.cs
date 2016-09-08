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
                Logger.Debug($"[{msg.MessageId}] SimulatorMessageEncoder.encode() {this.localAddress} -> {this.targetAddress} - {msg}");
            }
            msg.EncodeByteBuf(buf);
        }
    }
}