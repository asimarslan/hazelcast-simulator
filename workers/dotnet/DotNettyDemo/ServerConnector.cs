using System;
using System.Threading.Tasks;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System.Text;
using DotNetty.Buffers;

namespace DotNettyDemo
{
    public class ServerConnector
    {
        public ServerConnector()
        {
        }

        public static IChannel boundChannel;

        public static async Task RunServerAsync()
        {
            var message = Unpooled.Buffer(256);
			byte[] messageBytes = Encoding.UTF8.GetBytes("Hello world-X");
			message.WriteBytes(messageBytes);

            //var bossGroup = new MultithreadEventLoopGroup(1);
            //var workerGroup = new MultithreadEventLoopGroup();

            var group = new MultithreadEventLoopGroup();
            try
            {
                var bootstrap = new ServerBootstrap();
                bootstrap
                    //.Group(bossGroup, workerGroup)
                    .Group(group)
                    .Channel<TcpServerSocketChannel>()
                    .Option(ChannelOption.SoBacklog, 100)
                    //.Handler(new LoggingHandler("SRV-LSTN"))
                    .ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
                    {
                        IChannelPipeline pipeline = channel.Pipeline;

                        //pipeline.AddLast(new LoggingHandler("SRV-CONN"));
                        pipeline.AddLast("framing-enc", new LengthFieldPrepender(2));
                        pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(ushort.MaxValue, 0, 2, 0, 2));

                        pipeline.AddLast("echo", new EchoServerHandler());
                    }));

                boundChannel = await bootstrap.BindAsync(9999);

//                await boundChannel.WriteAndFlushAsync(message);

                Console.ReadLine();

                await boundChannel.CloseAsync();
            }
            finally
            {
                await group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
                //await Task.WhenAll(
                //	bossGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)),
                //	workerGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)));
            }
        }
    }

    public class EchoServerHandler : ChannelHandlerAdapter
    {
        private int i;

        public override void ChannelActive(IChannelHandlerContext context)
        {
			var message = Unpooled.Buffer(256);
			byte[] messageBytes = Encoding.UTF8.GetBytes("Hello world-"+i++);
			message.WriteBytes(messageBytes);

            ServerConnector.boundChannel = context.Channel;
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
//            var buffer = message as IByteBuffer;
//            if (buffer != null)
//            {
//                Console.WriteLine("Received from client: " + buffer.ToString(Encoding.UTF8));
//            }
//            context.WriteAsync(message);
        }

        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Console.WriteLine("Exception: " + exception);
            context.CloseAsync();
        }
    }
}