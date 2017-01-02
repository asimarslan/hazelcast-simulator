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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Handler;
using Hazelcast.Simulator.Protocol.Operations;
using Hazelcast.Simulator.Test;
using Hazelcast.Simulator.Utils;
using NUnit.Framework;

namespace Hazelcast.Simulator.Worker
{
    public class RemoteConnector
    {
        private static readonly BlockingCollection<SimulatorMessage> RecievedMessages = new BlockingCollection<SimulatorMessage>();
        private static readonly ConcurrentDictionary<TestPhase, AutoResetEvent> PhasesLocks = new ConcurrentDictionary<TestPhase, AutoResetEvent>();

        static RemoteConnector()
        {
            foreach (TestPhase testPhase in Enum.GetValues(typeof(TestPhase)))
            {
                PhasesLocks.TryAdd(testPhase, new AutoResetEvent(false));
            }
        }

        private readonly string address;
        private readonly int port;
        private IChannel channel;
        private readonly AtomicBoolean isStarted = new AtomicBoolean();
        private readonly IEventLoopGroup eventLoopGroup = new MultithreadEventLoopGroup();

        private readonly ConcurrentDictionary<string, TaskCompletionSource<Response>> responseCompletionSources =
            new ConcurrentDictionary<string, TaskCompletionSource<Response>>();

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
            channel?.CloseAsync().Wait(1000);
            eventLoopGroup.ShutdownGracefullyAsync().Wait(1000);
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
                if (msg.OperationType == OperationType.PhaseCompleted)
                {
                    var op = (PhaseCompletedOperation)msg.ToOperation();
                    PhasesLocks[op.TestPhase].Set();
                }
                else
                {
                    RecievedMessages.Add(msg);
                }
                var response = new Response(msg.MessageId, msg.Source);
                response.AddPart(new Response.Part(msg.Source, ResponseType.Success));
                ctx.WriteAndFlushAsync(response);
            }
        }

        public void WaitPhaseComplete(TestPhase testPhase) => PhasesLocks[testPhase].WaitOne();

        public static void AssertResponse(Response response, SimulatorAddress address, ResponseType result = ResponseType.Success)
        {
            Assert.True(response.Parts.ContainsKey(address));
            Assert.AreEqual(result, response.Parts[address].ResponseType);
        }

        public SimulatorMessage GetFirstLogMessage()
        {
            var msg = RecievedMessages.Take();
            return msg.OperationType == OperationType.Log ? msg : null;
        }
    }
}