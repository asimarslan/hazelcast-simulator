using System.Threading.Tasks;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Processors;
using log4net;

namespace Hazelcast.Simulator.Protocol.Operations
{
    /// <summary>
    /// Creates traffic on the wire, so the WorkerProcessFailureMonitor
    /// on the Agent can see, that the Worker is still responsive.
    /// This is needed for long running test phases, which lead to a radio silence on the wire.
    /// </summary>
    public class PingOperation : AbstractWorkerOperation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(PingOperation));

        public override async Task<ResponseType> RunInternal(OperationContext ctx, SimulatorAddress targetAddress)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"Pinged by {this.sourceAddress} (queue size: {ctx.Connector.GetMessageQueueSize()})...");
            }
            return ResponseType.Success;
        }
    }
}