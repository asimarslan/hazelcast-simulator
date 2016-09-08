using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Hazelcast.Simulator.Protocol.Core;
using log4net;

namespace Hazelcast.Simulator.Protocol.Handler
{
    public class ResponseEncoder : MessageToByteEncoder<Response>
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ResponseEncoder));

        private readonly SimulatorAddress localAddress;

        public ResponseEncoder(SimulatorAddress localAddress)
        {
            this.localAddress = localAddress;
        }

        protected override void Encode(IChannelHandlerContext ctx, Response response, IByteBuffer buf)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"[{response.MessageId}] ResponseEncoder.encode() {this.localAddress} - {response}");
            }
            response.EncodeByteBuf(buf);
        }
    }
}