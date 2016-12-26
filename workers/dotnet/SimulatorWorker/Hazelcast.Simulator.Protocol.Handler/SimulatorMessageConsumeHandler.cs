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
using System.Linq;
using DotNetty.Transport.Channels;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Processors;
using log4net;

namespace Hazelcast.Simulator.Protocol.Handler
{
    public class SimulatorMessageConsumeHandler : SimpleChannelInboundHandler<SimulatorMessage>
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SimulatorMessageConsumeHandler));

        private readonly SimulatorAddress localAddress;

        private readonly OperationProcessor operationProcessor;

        public SimulatorMessageConsumeHandler(SimulatorAddress localAddress, OperationProcessor operationProcessor)
        {
            this.localAddress = localAddress;
            this.operationProcessor = operationProcessor;
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, SimulatorMessage msg)
        {
            if (msg.Destination.AddressLevel == AddressLevel.WORKER
                || msg.Destination.AddressLevel == AddressLevel.TEST)
            {
                HandleSimulatorMessage(ctx, msg);
            }
            else
            {
                //TODO WRONG ADDRESS LEVEL HOW TO HANDLE IT???
                Logger.Error($"A wrong AddressLevel: {localAddress.AddressLevel} is received by .Net Worker: {localAddress} - {msg}");
            }
        }

        private void HandleSimulatorMessage(IChannelHandlerContext ctx, SimulatorMessage msg)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"[{msg.MessageId}] SimulatorMessageConsumeHandler -- {localAddress} - " +
                    $"{localAddress.AddressLevel} - {msg}");
            }

            operationProcessor.SubmitAsync(msg).ContinueWith(task =>
            {
                var response = new Response(msg.MessageId, msg.Source);
                if (!task.IsFaulted)
                {
                    foreach (Response.Part part in task.Result)
                    {
                        response.AddPart(part);
                    }
                }
                else
                {
                    Exception ex = task.Exception.Flatten().InnerExceptions.First();
                    Logger.Error($"Exception during operation excecution {ex?.Message}", ex);
                    response.AddPart(localAddress, ResponseType.ExceptionDuringOperationExecution, ex?.Message);
                }
                ctx.WriteAndFlushAsync(response);
                Console.WriteLine($"Test count={operationProcessor.operationContext.Tests.Count}");
            });
        }
    }
}