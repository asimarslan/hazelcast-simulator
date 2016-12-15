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

        //TODO Do we really need MAGIC_BYTE Validation as each message is validated later on
        public override void ChannelActive(IChannelHandlerContext context)
        {
//            if (Logger.IsDebugEnabled)
//            {
//            }
                Logger.Info($"Channel create from {context.Channel.RemoteAddress}");
            context.Channel.CloseCompletion.ContinueWith(task => { setChannel(null); });
            setChannel(context.Channel);
        }

    }
}