using System;
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
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"[{msg.MessageId}] SimulatorMessageConsumeHandler -- {this.localAddress} - {this.localAddress.AddressLevel} - {msg}");
            }
            this.operationProcessor.SubmitAsync(msg).ContinueWith(task =>
            {
                var response = new Response(msg.MessageId, msg.Source);
                if (task.Exception == null)
                {
                    ResponseResult responseResult = task.Result;
                    response.AddPart(this.localAddress, responseResult.ResponseType, responseResult.Payload);
                }
                else
                {
                    response.AddPart(this.localAddress, ResponseType.ExceptionDuringOperationExecution, task.Exception.Message);
                }
                ctx.WriteAndFlushAsync(response);
            });
        }
    }
}