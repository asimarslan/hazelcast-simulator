using System;
using System.Net;
using System.Threading.Tasks;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;


using System.Text;
using DotNetty.Buffers;

namespace DotNettyDemo
{
	public class ClientConnector
	{
		public ClientConnector()
		{
		}

		public static async Task RunClientAsync()
		{
			var group = new MultithreadEventLoopGroup();

			try
			{
				var bootstrap = new Bootstrap();
				bootstrap
					.Group(group)
					.Channel<TcpSocketChannel>()
					.Option(ChannelOption.TcpNodelay, true)
					.Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
					{
						IChannelPipeline pipeline = channel.Pipeline;

						//pipeline.AddLast(new LoggingHandler("CLIENT"));
						pipeline.AddLast("framing-enc", new LengthFieldPrepender(2));
						pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(ushort.MaxValue, 0, 2, 0, 2));

						pipeline.AddLast("echo", new ClientHandler());
					}));

				IChannel clientChannel = await bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999));

				Console.ReadLine();

				await clientChannel.CloseAsync();
			}
			finally
			{
				await group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
			}
		}
	}


    public class ClientHandler : ChannelHandlerAdapter
	{
		int i = 1;

		public ClientHandler()
		{
		}

		public override void ChannelActive(IChannelHandlerContext context)
		{
//			var message = Unpooled.Buffer(256);
//			byte[] messageBytes = Encoding.UTF8.GetBytes("Hello world-"+i++);
//			message.WriteBytes(messageBytes);
//
//			context.WriteAndFlushAsync(message);
		}
		public override void ChannelRead(IChannelHandlerContext context, object message)
		{
			var byteBuffer = message as IByteBuffer;
			if (byteBuffer != null)
			{
				Console.WriteLine("Received from server: " + byteBuffer.ToString(Encoding.UTF8));
			}

//			var message2 = Unpooled.Buffer(256);
//			byte[] messageBytes = Encoding.UTF8.GetBytes("Hello world-"+i++);
//			message2.WriteBytes(messageBytes);
//
//			context.WriteAsync(message2);
		}

		public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

		public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
		{
			Console.WriteLine("Exception: " + exception);
			context.CloseAsync();
		}
	}
}

