using System.Threading.Tasks;
using Hazelcast.Simulator.Protocol.Connector;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Processors;
using log4net;
using Newtonsoft.Json;

namespace Hazelcast.Simulator.Protocol.Operations
{
    ///
    /// Creates traffic on the wire, so the WorkerProcessFailureMonitor
    /// on the Agent can see, that the Worker is still responsive.
    ///
    /// This is needed for long running test phases, which lead to a radio silence on the wire.
    ///
    public class PingOperation : ISimulatorOperation, ISimulatorMessageAware, IConnectorAware
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(PingOperation));

        [JsonIgnore]
        private SimulatorMessage msg;

        [JsonIgnore]
        private WorkerConnector connector;

        public async Task<ResponseResult> Run(OperationContext operationContext)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"Pinged by {this.msg.Source} (queue size: {this.connector.GetMessageQueueSize()})...");
            }
            return new ResponseResult(ResponseType.Success);
        }

        public void SetSimulatorMessage(SimulatorMessage simulatorMessage) => this.msg = simulatorMessage;

        public void SetConnector(WorkerConnector connector) => this.connector = connector;
    }
}