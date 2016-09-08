using DotNetty.Codecs;
using static Hazelcast.Simulator.Utils.Constants;
namespace Hazelcast.Simulator.Protocol.Handler
{

    public class SimulatorFrameDecoder : LengthFieldBasedFrameDecoder
    {
        private const int MAX_FRAME_SIZE = int.MaxValue;
        private const int LENGTH_FIELD_OFFSET = 0;
        private const int LENGTH_FIELD_SIZE = INT_SIZE;

        public SimulatorFrameDecoder() : base(MAX_FRAME_SIZE, LENGTH_FIELD_OFFSET, LENGTH_FIELD_SIZE)
        {
        }
    }
}