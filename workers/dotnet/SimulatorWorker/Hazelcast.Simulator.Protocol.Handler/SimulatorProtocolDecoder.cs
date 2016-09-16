namespace Hazelcast.Simulator.Protocol.Handler
{
    using System.Collections.Generic;
    using System.IO;
    using DotNetty.Buffers;
    using DotNetty.Codecs;
    using DotNetty.Transport.Channels;
    using Hazelcast.Simulator.Protocol.Core;
    using log4net;

    public class SimulatorProtocolDecoder : ByteToMessageDecoder
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SimulatorProtocolDecoder));

        private readonly SimulatorAddress localAddress;

        public SimulatorProtocolDecoder(SimulatorAddress localAddress)
        {
            this.localAddress = localAddress;
        }

        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            if (Unpooled.Empty.Equals(input))
            {
                return;
            }
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"SimulatorProtocolDecoder.decode() {localAddress.AddressLevel}, {localAddress}");
            }
            if (input.IsSimulatorMessage())
            {
                output.Add(input.DecodeSimulatorMessage());
            }
            else if (input.IsResponse())
            {
                output.Add(input.DecodeResponse());
            }
            else
            {
                string logMsg = $"Invalid message magic bytes do not match. {input}";
                Logger.Error(logMsg);
                throw new InvalidDataException(logMsg);
            }
        }
    }
}