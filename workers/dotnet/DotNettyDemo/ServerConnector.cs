using System;
using System.Threading.Tasks;
using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
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

		public static async Task RunServerAsync()
		{
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

				IChannel boundChannel = await bootstrap.BindAsync(9999);

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
		public override void ChannelRead(IChannelHandlerContext context, object message)
		{
			var buffer = message as IByteBuffer;
			if (buffer != null)
			{
				Console.WriteLine("Received from client: " + buffer.ToString(Encoding.UTF8));
			}
			context.WriteAsync(message);
		}

		public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

		public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
		{
			Console.WriteLine("Exception: " + exception);
			context.CloseAsync();
		}
	}
}

