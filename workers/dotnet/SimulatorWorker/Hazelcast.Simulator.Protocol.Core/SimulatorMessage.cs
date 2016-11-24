using Hazelcast.Simulator.Protocol.Operations;
using Newtonsoft.Json;

namespace Hazelcast.Simulator.Protocol.Core
{
    public class SimulatorMessage
    {
        public SimulatorAddress Destination { get; }

        public SimulatorAddress Source { get; }

        public long MessageId { get; }

        public OperationType OperationType { get; }

        public string OperationData { get; }

        public SimulatorMessage(SimulatorAddress destination, SimulatorAddress source, long messageId,
            OperationType operationType, string operationData)
        {
            this.Destination = destination;
            this.Source = source;
            this.MessageId = messageId;
            this.OperationType = operationType;
            this.OperationData = operationData;
        }

        public ISimulatorOperation ToOperation()
        {
            var type = this.OperationType.GetClassType();
            var simulatorOperation = (ISimulatorOperation)JsonConvert.DeserializeObject(this.OperationData, type);
            simulatorOperation.SetSourceAddress(this.Source);
            return simulatorOperation;
        }

        public override string ToString() =>
            $"[SimulatorMessage: Destination={this.Destination}, Source={this.Source}, MessageId={this.MessageId}, " +
            $"operationType={this.OperationType}, operationData={this.OperationData}]";
    }
}