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
        private const int DefaultShutdownQuietPeriod = 0;
        private const int DefaultShutdownTimeout = 15;

        private static readonly ILog Logger = LogManager.GetLogger(typeof(WorkerConnector));

        private readonly SimulatorAddress workerAddress;
        private readonly int addressIndex;

        private readonly AtomicBoolean isStarted = new AtomicBoolean();
        private readonly int port;
        private readonly IHazelcastInstance hazelcastInstance;
        private readonly ClientWorker worker;

        private readonly IEventLoopGroup eventLoopGroup = new MultithreadEventLoopGroup();

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private readonly BlockingCollection<SimulatorMessage> messageQueue = new BlockingCollection<SimulatorMessage>();
        private readonly ConcurrentDictionary<long, TaskCompletionSource<Response>> responseCompletionSources = new ConcurrentDictionary<long, TaskCompletionSource<Response>>();

        private IChannel channel;

        public void SetChannel(IChannel channel) => this.channel = channel;

        public WorkerConnector(SimulatorAddress workerAddress, int port, IHazelcastInstance hazelcastInstance, ClientWorker worker)
        {
            this.workerAddress = workerAddress;
            this.port = port;
            this.hazelcastInstance = hazelcastInstance;
            this.worker = worker;
        }

        public WorkerConnector(int agentIndex, int workerIndex, int port, IHazelcastInstance hazelcastInstance, ClientWorker worker)
            : this(new SimulatorAddress(AddressLevel.WORKER, agentIndex, workerIndex, 0), port, hazelcastInstance, worker)
        {}

        public async Task Start()
        {
            if (!this.isStarted.CompareAndSet(false, true))
            {
                throw new SimulatorProtocolException("ServerConnector cannot be started twice or after shutdown!");
            }
            var bootstrap = new ServerBootstrap();
            bootstrap
                .Group(this.eventLoopGroup)
                .Channel<TcpServerSocketChannel>()
                .LocalAddress(this.port)
                .ChildHandler(new ActionChannelInitializer<ISocketChannel>(this.ConfigureServerPipeline));

            var boundChannel = await bootstrap.BindAsync();
            Logger.Info($"WorkerConnector {this.workerAddress} listens on {boundChannel.LocalAddress}");

            try
            {
                await this.ProcessMessageQueue(this.cancellationTokenSource.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                await this.eventLoopGroup.ShutdownGracefullyAsync(TimeSpan.FromSeconds(DefaultShutdownQuietPeriod),
                        TimeSpan.FromSeconds(DefaultShutdownTimeout));
            }
        }

        private void ConfigureServerPipeline(ISocketChannel socketChannel)
        {
            var pipeline = socketChannel.Pipeline;
            pipeline.AddLast("connectionValidationHandler", new ConnectionValidationHandler(this.SetChannel));
            //            pipeline.AddLast("connectionListenerHandler", new ConnectionListenerHandler(connectionManager));

            pipeline.AddLast("responseEncoder", new ResponseEncoder(this.workerAddress));
            pipeline.AddLast("messageEncoder", new SimulatorMessageEncoder(this.workerAddress, this.workerAddress.GetParent()));

            pipeline.AddLast("frameDecoder", new SimulatorFrameDecoder());
            pipeline.AddLast("protocolDecoder", new SimulatorProtocolDecoder(this.workerAddress));

            var op = new OperationProcessor(this.hazelcastInstance, this.workerAddress, this.worker);
            pipeline.AddLast("messageConsumeHandler", new SimulatorMessageConsumeHandler(this.workerAddress, op));

            pipeline.AddLast("responseHandler", new ResponseHandler(this.workerAddress, this.workerAddress.GetParent(), this.SetReponse));
            pipeline.AddLast("exceptionHandler", new ExceptionHandler());
        }

        public void Shutdown()
        {
            this.cancellationTokenSource.Cancel();
        }

        public Task<Response> Submit(SimulatorAddress destination, ISimulatorOperation operation)
        {
            //TODO FIXME
            TaskCompletionSource<Response> tcs = new TaskCompletionSource<Response>();
            long key = -1;
            this.responseCompletionSources.TryAdd(key, tcs);
            return tcs.Task;
        }

        private async Task ProcessMessageQueue(CancellationToken token)
        {
            while (!token.IsCancellationRequested && !messageQueue.IsAddingCompleted)
            {
            }
            token.ThrowIfCancellationRequested();
        }

        private void SetReponse(Response response)
        {
            TaskCompletionSource<Response> resCompletionSource;
            if (this.responseCompletionSources.TryRemove(response.MessageId, out resCompletionSource))
            {
                resCompletionSource.SetResult(response);
            }
        }

        public int GetMessageQueueSize() => this.messageQueue.Count;

//        private ResponseFuture writeAsyncToParents(SimulatorMessage message) {
//            long messageId = message.getMessageId();
//            String futureKey = createFutureKey(message.getSource(), messageId, addressIndex);
//            ResponseFuture future = createInstance(futureMap, futureKey);
//            if (LOGGER.isTraceEnabled()) {
//                LOGGER.trace(format("[%d] %s created ResponseFuture %s", messageId, localAddress, futureKey));
//            }
//            OperationTypeCounter.sent(message.getOperationType());
//            getChannelGroup().writeAndFlush(message);
//
//            return future;
//        }


    }
}