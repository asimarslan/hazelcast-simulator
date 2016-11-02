using System;
using DotNetty.Transport.Channels;
using Hazelcast.Simulator.Protocol.Connector;
using log4net;

namespace Hazelcast.Simulator.Protocol.Handler
{
    public class ConnectionValidationHandler : ChannelHandlerAdapter
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ConnectionValidationHandler));

//        private const int MinimumByteBufferSize = 8;
        private readonly Action<IChannel> setChannel;

//        private Delegate ChannelInit;

        public ConnectionValidationHandler(Action<IChannel> setChannel)
        {
            this.setChannel = setChannel;
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            context.Channel.CloseCompletion.ContinueWith(task => { setChannel(null); });
            setChannel(context.Channel);
        }

        //TODO Do we really need MAGIC_BYTE Validation as each message is validated later on
//        public override void ChannelRead(IChannelHandlerContext context, object obj)
//        {
//            var buf = obj as IByteBuffer;
//            if (buf == null || buf.ReadableBytes < MinimumByteBufferSize)
//            {
//                return;
//            }
//
//            if (!buf.IsSimulatorMessage() && !buf.IsResponse())
//            {
//                Logger.Warn($"Invalid connection from {context.Channel.RemoteAddress} (no magic bytes found)");
//                context.Close();
//                return;
//            }
//
//            // the connection is valid so we remove this handler and forward the buffer to the pipeline
//            LOGGER.info(format("Valid connection from %s (magic bytes found)", ctx.channel().remoteAddress()));
//            ctx.pipeline().remove(this);
//            ctx.fireChannelRead(obj);
//        }
    }
}