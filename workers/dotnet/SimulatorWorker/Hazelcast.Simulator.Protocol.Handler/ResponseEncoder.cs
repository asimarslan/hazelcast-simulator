using DotNetty.Buffers;
using DotNetty.Codec;
using DotNetty.Transport.Channels;
using Hazelcast.Simulator.Protocol.Core;
using log4net;

namespace Hazelcast.Simulator.Protocol.Handler
{
    public class ResponseEncoder : MessageToByteEncoder<Response>
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ResponseEncoder));

        private readonly SimulatorAddress _localAddress;

        public ResponseEncoder(SimulatorAddress localAddress)
        {
            this._localAddress = localAddress;
        }

        protected override void Encode(IChannelHandlerContext ctx, Response response, IByteBuffer buf)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"[{response.MessageId}] ResponseEncoder.encode() {_localAddress} - {response}");
            }
            response.EncodeByteBuf(buf);
        }
    }
}