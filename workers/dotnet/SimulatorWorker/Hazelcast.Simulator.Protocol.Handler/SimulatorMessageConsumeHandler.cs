using System;
using Hazelcast.Simulator.Protocol.Processors;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using Hazelcast.Simulator.Protocol.Connector;
using Hazelcast.Simulator.Protocol.Core;
using log4net;

namespace Hazelcast.Simulator.Protocol.Handler
{

    public class SimulatorMessageConsumeHandler : SimpleChannelInboundHandler<SimulatorMessage>
    {
        private readonly ILog Logger = LogManager.GetLogger(typeof(SimulatorMessageConsumeHandler));

        private readonly SimulatorAddress localAddress;

        private readonly OperationProcessor operationProcessor  = new OperationProcessor();

        public SimulatorMessageConsumeHandler(SimulatorAddress localAddress)
        {
            this.localAddress = localAddress;
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, SimulatorMessage msg)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"[{msg.MessageId}] SimulatorMessageConsumeHandler -- {this.localAddress} - {this.localAddress.AddressLevel} - {msg}");
            }
            this.operationProcessor.SubmitAsync(msg);
//            Task.Run(()=> this.operationProcessor.Submit(ctx, msg));

//
//            t.ContinueWith(task =>
//            {
//                if (task.IsFaulted)
//                {
//                    Response response = new Response(msg.MessageId, msg.Source).AddPart(localAddress, responseType, task.Exception.Message);
//                    ctx.WriteAndFlushAsync(response);
//                }
//            });

        }
    }
}