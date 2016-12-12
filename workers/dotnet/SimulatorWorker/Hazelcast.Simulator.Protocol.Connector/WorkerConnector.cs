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
using Newtonsoft.Json;

namespace Hazelcast.Simulator.Protocol.Connector
{
    public class WorkerConnector
    {
        private const int DefaultShutdownQuietPeriod = 0;
        private const int DefaultShutdownTimeout = 15;

        private static readonly ILog Logger = LogManager.GetLogger(typeof(WorkerConnector));

        public SimulatorAddress WorkerAddress;
        private readonly int addressIndex;
        private long currentMessageId = 1;

        private readonly AtomicBoolean isStarted = new AtomicBoolean();
        private readonly int port;
        private readonly IHazelcastInstance hazelcastInstance;
        private readonly ClientWorker worker;

        private readonly IEventLoopGroup eventLoopGroup = new MultithreadEventLoopGroup();

//        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

//        private readonly BlockingCollection<SimulatorMessage> messageQueue = new BlockingCollection<SimulatorMessage>();
        private readonly ConcurrentDictionary<string, TaskCompletionSource<Response>> responseCompletionSources =
            new ConcurrentDictionary<string, TaskCompletionSource<Response>>();

        private IChannel channel;

        public string PublicIpAddress => this.worker.PublicIpAddress;

        public void SetChannel(IChannel channel) => this.channel = channel;

        public WorkerConnector(SimulatorAddress workerAddress, int port, IHazelcastInstance hazelcastInstance, ClientWorker worker)
        {
            this.WorkerAddress = workerAddress;
            this.port = port;
            this.hazelcastInstance = hazelcastInstance;
            this.worker = worker;
        }

        public WorkerConnector(int agentIndex, int workerIndex, int port, IHazelcastInstance hazelcastInstance, ClientWorker worker)
            : this(new SimulatorAddress(AddressLevel.WORKER, agentIndex, workerIndex, 0), port, hazelcastInstance, worker)
        {
        }

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
            Logger.Info($"WorkerConnector {this.WorkerAddress} listens on {boundChannel.LocalAddress}");

//            try
//            {
//                await this.ProcessMessageQueue(this.cancellationTokenSource.Token).ConfigureAwait(false);
//            }
//            catch (OperationCanceledException)
//            {
//                await this.eventLoopGroup.ShutdownGracefullyAsync(TimeSpan.FromSeconds(DefaultShutdownQuietPeriod),
//                    TimeSpan.FromSeconds(DefaultShutdownTimeout));
//            }
        }

        private void ConfigureServerPipeline(ISocketChannel socketChannel)
        {
            var pipeline = socketChannel.Pipeline;
            pipeline.AddLast("connectionValidationHandler", new ConnectionValidationHandler(this.SetChannel));
            //            pipeline.AddLast("connectionListenerHandler", new ConnectionListenerHandler(connectionManager));

            pipeline.AddLast("responseEncoder", new ResponseEncoder(this.WorkerAddress));
            pipeline.AddLast("messageEncoder", new SimulatorMessageEncoder(this.WorkerAddress,
                this.WorkerAddress.GetParent()));

            pipeline.AddLast("frameDecoder", new SimulatorFrameDecoder());
            pipeline.AddLast("protocolDecoder", new SimulatorProtocolDecoder(this.WorkerAddress));

            pipeline.AddLast("messageConsumeHandler", new SimulatorMessageConsumeHandler(this.WorkerAddress,
                new OperationProcessor(this.hazelcastInstance, this.WorkerAddress, this.worker)));

            pipeline.AddLast("responseHandler", new ResponseHandler(this.HandleReponse));
            pipeline.AddLast("exceptionHandler", new ExceptionHandler());
        }

        public void Shutdown()
        {
            if (!this.isStarted.CompareAndSet(true, false)) {
                throw new SimulatorProtocolException("ServerConnector cannot be shutdown twice or if not been started!");
            }

            this.channel?.CloseAsync().Wait();

            this.eventLoopGroup.ShutdownGracefullyAsync(TimeSpan.FromSeconds(DefaultShutdownQuietPeriod),
                TimeSpan.FromSeconds(DefaultShutdownTimeout)).Wait();
        }

        public Task<Response> Submit(SimulatorAddress source, SimulatorAddress destination, ISimulatorOperation operation)
        {
            //TODO FIXME
            TaskCompletionSource<Response> tcs = new TaskCompletionSource<Response>();
            long messageId = Interlocked.Increment(ref this.currentMessageId);
            string json = JsonConvert.SerializeObject(operation);
            var message = new SimulatorMessage(destination, source, messageId, operation.GetOperationType(), json);
            string key = source.CreateResponseKey(messageId, 0);
            if (this.responseCompletionSources.TryAdd(key, tcs))
            {
                this.channel.WriteAndFlushAsync(message);
            }
            return tcs.Task;
        }

//        private async Task ProcessMessageQueue(CancellationToken token)
//        {
//            while (!token.IsCancellationRequested && !messageQueue.IsAddingCompleted)
//            {
//            }
//            token.ThrowIfCancellationRequested();
//        }

        private void HandleReponse(Response response)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"[{response.MessageId}] ResponseHandler -- {this.WorkerAddress} <- " +
                    $"{this.WorkerAddress.GetParent()} received {response}");
            }

            if (response.MessageId == 0)
            {
                return;
            }
            TaskCompletionSource<Response> responseCompletionSource;
            string key = response.Destination.CreateResponseKey(response.MessageId, this.addressIndex);
            if (this.responseCompletionSources.TryRemove(key, out responseCompletionSource))
            {
                responseCompletionSource.SetResult(response);
            }
            else
            {
                Logger.Error($"[{response.MessageId}] ResponseHandler -- {this.WorkerAddress} <- " +
                    $"{this.WorkerAddress.GetParent()} , No corresponding request found for received {response},");
            }
        }

//        public int GetMessageQueueSize() => this.messageQueue.Count;
    }
}