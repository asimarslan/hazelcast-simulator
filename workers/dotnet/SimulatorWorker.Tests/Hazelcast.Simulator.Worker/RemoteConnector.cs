using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Handler;
using Hazelcast.Simulator.Protocol.Operations;
using Hazelcast.Simulator.Utils;
using NUnit.Framework;

namespace Hazelcast.Simulator.Worker
{
    public class RemoteConnector
    {
        private readonly string address;
        private readonly int port;
        private IChannel channel;
        private readonly AtomicBoolean isStarted = new AtomicBoolean();
        private readonly IEventLoopGroup eventLoopGroup = new MultithreadEventLoopGroup();

        private readonly ConcurrentDictionary<string, TaskCompletionSource<Response>> responseCompletionSources =
            new ConcurrentDictionary<string, TaskCompletionSource<Response>>();

        private static readonly Queue<SimulatorMessage> RecievedMessages = new Queue<SimulatorMessage>();

        private readonly SimulatorAddress localAddress;
        private readonly SimulatorAddress remoteAddress;

        private long lastMessageId;

        public long LastMessageId => lastMessageId;

        public RemoteConnector(string address, int port, SimulatorAddress remoteAddress)
        {
            this.address = address;
            this.port = port;
            localAddress = SimulatorAddress.COORDINATOR;
            this.remoteAddress = remoteAddress;
        }

        public async Task Start()
        {
            if
                (!isStarted.CompareAndSet(false, true))
            {
                throw new SimulatorProtocolException("ServerConnector cannot be started twice or after shutdown!");
            }

            var bootstrap = new Bootstrap();
            bootstrap
                .Group(eventLoopGroup)
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Option(ChannelOption.ConnectTimeout, TimeSpan.FromMinutes(1))
                .Handler(new ActionChannelInitializer<ISocketChannel>(ConfigurePipeline));

            channel = await bootstrap.ConnectAsync(IPAddress.Parse(address), port);
        }

        public void Shutdown()
        {
            if (!isStarted.CompareAndSet(true, false))
            {
                return;
            }
            channel?.CloseAsync().Wait();
            eventLoopGroup.ShutdownGracefullyAsync().Wait();
        }

        private void ConfigurePipeline(ISocketChannel socketChannel)
        {
            IChannelPipeline pipeline = socketChannel.Pipeline;

            pipeline.AddLast("responseEncoder", new ResponseEncoder(localAddress));
            pipeline.AddLast("messageEncoder", new SimulatorMessageEncoder(localAddress, remoteAddress));

            pipeline.AddLast("frameDecoder", new SimulatorFrameDecoder());
            pipeline.AddLast("protocolDecoder", new SimulatorProtocolDecoder(localAddress));

            pipeline.AddLast("messageConsumeHandler", new MessageConsumeHandler());

            pipeline.AddLast("responseHandler", new ResponseHandler(HandleReponse));
            pipeline.AddLast("exceptionHandler", new ExceptionHandler());
        }

        private void HandleReponse(Response response)
        {
            if (response.MessageId == 0)
            {
                return;
            }
            TaskCompletionSource<Response> responseCompletionSource;
            string key = response.Destination.CreateResponseKey(response.MessageId, localAddress.WorkerIndex);
            if (responseCompletionSources.TryRemove(key, out responseCompletionSource))
            {
                responseCompletionSource.SetResult(response);
            }
        }

        public Task<Response> Send(SimulatorAddress source, SimulatorAddress destination, OperationType operationType, string payload)
        {
            var tcs = new TaskCompletionSource<Response>();
            long messageId = Interlocked.Increment(ref lastMessageId);
            var message = new SimulatorMessage(destination, source, messageId, operationType, payload);

            string key = source.CreateResponseKey(messageId, 0);
            if (responseCompletionSources.TryAdd(key, tcs))
            {
                channel.WriteAndFlushAsync(message);
            }
            return tcs.Task;
        }

        private class MessageConsumeHandler : SimpleChannelInboundHandler<SimulatorMessage>
        {
            protected override void ChannelRead0(IChannelHandlerContext ctx, SimulatorMessage msg)
            {
                RecievedMessages.Enqueue(msg);
                var response = new Response(msg.MessageId, msg.Source);
                response.AddPart(new Response.Part(msg.Source, ResponseType.Success));
                ctx.WriteAndFlushAsync(response).Wait();
            }
        }
    }
}