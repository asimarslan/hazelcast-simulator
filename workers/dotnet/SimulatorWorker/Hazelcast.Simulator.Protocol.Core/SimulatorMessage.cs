using Hazelcast.Simulator.Protocol.Operations;

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
            Destination = destination;
            Source = source;
            MessageId = messageId;
            OperationType = operationType;
            OperationData = operationData;
        }

        public override string ToString() =>
            $"[SimulatorMessage: Destination={this.Destination}, Source={this.Source}, MessageId={this.MessageId}, " +
            $"operationType={this.OperationType}, operationData={this.OperationData}]";
    }
}