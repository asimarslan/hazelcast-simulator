// Copyright (c) 2008-2017, Hazelcast, Inc. All Rights Reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Handler;
using Hazelcast.Simulator.Protocol.Operations;
using Hazelcast.Simulator.Protocol.Processors;
using Hazelcast.Simulator.Utils;
using log4net;
using Newtonsoft.Json;

namespace Hazelcast.Simulator.Protocol.Connector
{
    public class WorkerConnector
    {
        private const int DefaultShutdownQuietPeriod = 1;
        private const int DefaultShutdownTimeout = 1;

        private static readonly ILog Logger = LogManager.GetLogger(typeof(WorkerConnector));

        public SimulatorAddress WorkerAddress;
        private long currentMessageId = 1;

        private readonly AtomicBoolean isStarted = new AtomicBoolean();
        private readonly int port;

        private readonly IEventLoopGroup eventLoopGroup = new MultithreadEventLoopGroup();

        private readonly ConcurrentDictionary<string, TaskCompletionSource<Response>> responseCompletionSources =
            new ConcurrentDictionary<string, TaskCompletionSource<Response>>();

        private IChannel channel;

        public string PublicIpAddress { get; }

        public void SetChannel(IChannel channel) => this.channel = channel;

        private readonly ManualResetEventSlim _startLock = new ManualResetEventSlim(false, 0);

        public volatile bool Ready;

        public Func<OperationProcessor> GetOperationProcessor { get; set; }

        public WorkerConnector(SimulatorAddress workerAddress, int port, string publicIpAddress)
        {
            WorkerAddress = workerAddress;
            this.port = port;
            PublicIpAddress = publicIpAddress;
        }

        public WorkerConnector(int agentIndex, int workerIndex, int port, string publicIpAddress)
            : this(new SimulatorAddress(AddressLevel.WORKER, agentIndex, workerIndex, 0), port, publicIpAddress) {}

        public async Task Start()
        {
            if (!isStarted.CompareAndSet(false, true))
            {
                throw new SimulatorProtocolException("ServerConnector cannot be started twice or after shutdown!");
            }
            try
            {
                var bootstrap = new ServerBootstrap();
                bootstrap
                    .Group(eventLoopGroup)
                    .Channel<TcpServerSocketChannel>()
                    .LocalAddress(port)
                    .ChildHandler(new ActionChannelInitializer<ISocketChannel>(ConfigureServerPipeline));

                IChannel boundChannel = await bootstrap.BindAsync();
                Logger.Info($"WorkerConnector {WorkerAddress} listens on {boundChannel.LocalAddress}");

                Ready = true;
                _startLock.Wait();

                await boundChannel.CloseAsync();
            }
            finally
            {
                await eventLoopGroup.ShutdownGracefullyAsync();
                _startLock.Reset();
            }
            Logger.Info("Start completed...");
        }

        private void ConfigureServerPipeline(ISocketChannel socketChannel)
        {
            IChannelPipeline pipeline = socketChannel.Pipeline;
            pipeline.AddLast("connectionValidationHandler", new ConnectionValidationHandler(SetChannel));
            //            pipeline.AddLast("connectionListenerHandler", new ConnectionListenerHandler(connectionManager));

            pipeline.AddLast("responseEncoder", new ResponseEncoder(WorkerAddress));
            pipeline.AddLast("messageEncoder", new SimulatorMessageEncoder(WorkerAddress,
                WorkerAddress.GetParent()));

            pipeline.AddLast("frameDecoder", new SimulatorFrameDecoder());
            pipeline.AddLast("protocolDecoder", new SimulatorProtocolDecoder(WorkerAddress));

            pipeline.AddLast("messageConsumeHandler", new SimulatorMessageConsumeHandler(WorkerAddress, GetOperationProcessor()));

            pipeline.AddLast("responseHandler", new ResponseHandler(HandleReponse));
            pipeline.AddLast("exceptionHandler", new ExceptionHandler());
        }

        public void Shutdown()
        {
            if (!isStarted.CompareAndSet(true, false))
            {
                return;
            }
            Logger.Debug("Shutting down worker connector");

            _startLock.Set();

            channel?.CloseAsync().Wait(1000);

            eventLoopGroup.ShutdownGracefullyAsync().Wait(1000);
        }

        public Task<Response> Submit(SimulatorAddress source, SimulatorAddress destination, ISimulatorOperation operation)
        {
            var tcs = new TaskCompletionSource<Response>();
            long messageId = Interlocked.Increment(ref currentMessageId);
            string json = JsonConvert.SerializeObject(operation);
            var message = new SimulatorMessage(destination, source, messageId, operation.GetOperationType(), json);
            string key = source.CreateResponseKey(messageId, 0);
            if (responseCompletionSources.TryAdd(key, tcs))
            {
                channel.WriteAndFlushAsync(message);
            }
            return tcs.Task;
        }

        private void HandleReponse(Response response)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"[{response.MessageId}] ResponseHandler -- {WorkerAddress} <- " +
                    $"{WorkerAddress.GetParent()} received {response}");
            }

            if (response.MessageId == 0)
            {
                return;
            }
            TaskCompletionSource<Response> responseCompletionSource;
            string key = response.Destination.CreateResponseKey(response.MessageId, WorkerAddress.WorkerIndex);
            if (responseCompletionSources.TryRemove(key, out responseCompletionSource))
            {
                responseCompletionSource.SetResult(response);
            }
            else
            {
                Logger.Error($"[{response.MessageId}] ResponseHandler -- {WorkerAddress} <- " +
                    $"{WorkerAddress.GetParent()} , No corresponding request found for received {response},");
            }
        }
    }
}