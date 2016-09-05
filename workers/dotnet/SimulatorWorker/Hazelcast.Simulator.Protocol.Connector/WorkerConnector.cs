using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Processors;
using Hazelcast.Core;
using Hazelcast.Simulator.Protocol.Operations;
using Hazelcast.Simulator.Utils;
using Hazelcast.Simulator.Worker;
using log4net;

using Hazelcast.Simulator.Protocol.Handler;

namespace Hazelcast.Simulator.Protocol.Connector
{
    public class WorkerConnector
    {
        const int DEFAULT_SHUTDOWN_QUIET_PERIOD = 0;
        const int DEFAULT_SHUTDOWN_TIMEOUT = 15;

        internal readonly ILog Logger = LogManager.GetLogger(typeof(WorkerConnector));

        public IOperationProcessor Processor { get; private set; }

        private readonly SimulatorAddress localAddress;
        private readonly int addressIndex;

        private readonly ConnectionManager connectionManager;
        private readonly TestProcessorManager testProcessorManager;

        private readonly AtomicBoolean isStarted = new AtomicBoolean();
        private readonly int port;

        private readonly IEventLoopGroup eventLoopGroup = new MultithreadEventLoopGroup();

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private readonly BlockingCollection<SimulatorMessage> messageQueue = new BlockingCollection<SimulatorMessage>();

        private IChannel channel;

        public void SetChannel(IChannel channel)
        {
            this.channel = channel;
        }

        public WorkerConnector(SimulatorAddress localAddress, int port, IHazelcastInstance hazelcastInstance,
            ClientWorker worker)
        {
            this.localAddress = localAddress;
        }

        public async Task Start()
        {
            if (!isStarted.CompareAndSet(false, true))
            {
                throw new SimulatorProtocolException("ServerConnector cannot be started twice or after shutdown!");
            }
            var bootstrap = new ServerBootstrap();
            bootstrap
                .Group(eventLoopGroup)
                .Channel<TcpServerSocketChannel>()
                .LocalAddress(port)
                .ChildHandler(new ActionChannelInitializer<ISocketChannel>(ConfigureServerPipeline));

            var boundChannel = await bootstrap.BindAsync();
            Logger.Info($"WorkerConnector {localAddress} listens on {boundChannel.LocalAddress}");

            try
            {
                await ProcessMessageQueue(cancellationTokenSource.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                await
                    eventLoopGroup.ShutdownGracefullyAsync(TimeSpan.FromSeconds(DEFAULT_SHUTDOWN_QUIET_PERIOD),
                        TimeSpan.FromSeconds(DEFAULT_SHUTDOWN_TIMEOUT));
            }
        }

        private void ConfigureServerPipeline(ISocketChannel socketChannel)
        {
            var pipeline = socketChannel.Pipeline;
            pipeline.AddLast("connectionValidationHandler", new ConnectionValidationHandler(SetChannel) );
//            pipeline.AddLast("connectionValidationHandler", new ChannelInboundHandlerAdapter() {} ConnectionValidationHandler())};
//            pipeline.AddLast("connectionListenerHandler", new ConnectionListenerHandler(connectionManager));
            pipeline.AddLast("responseEncoder", new ResponseEncoder(localAddress));
//            pipeline.AddLast("messageEncoder", new MessageEncoder(localAddress, localAddress.getParent()));
//            pipeline.AddLast("frameDecoder", new SimulatorFrameDecoder());
//            pipeline.AddLast("protocolDecoder", new SimulatorProtocolDecoder(localAddress));
//            pipeline.AddLast("messageConsumeHandler", new MessageConsumeHandler(localAddress, processor, getScheduledExecutor()));
//            pipeline.AddLast("testProtocolDecoder", new SimulatorProtocolDecoder(localAddress.getChild(0)));
//            pipeline.AddLast("testMessageConsumeHandler", new MessageTestConsumeHandler(testProcessorManager, localAddress, getScheduledExecutor()));
//            pipeline.AddLast("responseHandler", new ResponseHandler(localAddress, localAddress.getParent(), futureMap, addressIndex));
//            pipeline.AddLast("exceptionHandler", new ExceptionHandler(serverConnector));
        }


        public void Shutdown()
        {
            cancellationTokenSource.Cancel();
        }

        public async Task<Response> Submit(SimulatorAddress destination, ISimulatorOperation operation)
        {
            return null;

        }

        private async Task ProcessMessageQueue(CancellationToken token)
        {
            while (!token.IsCancellationRequested && !messageQueue.IsAddingCompleted)
            {
            }
            token.ThrowIfCancellationRequested();
        }

    }




}