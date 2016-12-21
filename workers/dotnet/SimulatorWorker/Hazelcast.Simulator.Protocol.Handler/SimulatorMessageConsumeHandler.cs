using System;
using System.Linq;
using System.Threading.Tasks;
using Hazelcast.Simulator.Protocol.Processors;
using DotNetty.Transport.Channels;
using Hazelcast.Simulator.Protocol.Core;
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
                this.HandleSimulatorMessage(ctx, msg);
            }
            else
            {
                //TODO WRONG ADDRESS LEVEL HOW TO HANDLE IT???
                Logger.Error($"A wrong AddressLevel: {this.localAddress.AddressLevel} is received by .Net Worker: {this.localAddress} - {msg}");
            }
        }

        private void HandleSimulatorMessage(IChannelHandlerContext ctx, SimulatorMessage msg)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"[{msg.MessageId}] SimulatorMessageConsumeHandler -- {this.localAddress} - " +
                    $"{this.localAddress.AddressLevel} - {msg}");
            }

            this.operationProcessor.SubmitAsync(msg).ContinueWith(task =>
            {
                var response = new Response(msg.MessageId, msg.Source);
                if (!task.IsFaulted)
                {
                    foreach (var part in task.Result)
                    {
                        response.AddPart(part);
                    }
                }
                else
                {
                    var ex = task.Exception.Flatten().InnerExceptions.First();
                    Logger.Error($"Exception during operation excecution {ex?.Message}", ex);
                    response.AddPart(this.localAddress, ResponseType.ExceptionDuringOperationExecution, ex?.Message);
                }
                ctx.WriteAndFlushAsync(response).Wait();
                Console.WriteLine($"Test count={operationProcessor.operationContext.Tests.Count}");
            });
        }
    }
}