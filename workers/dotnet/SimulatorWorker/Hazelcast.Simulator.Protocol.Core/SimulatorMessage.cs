using Hazelcast.Simulator.Protocol.Operations;

namespace Hazelcast.Simulator.Protocol.Core
{
	public class SimulatorMessage
	{
		public SimulatorAddress Destination { get; }
		public SimulatorAddress Source { get; }
		public long MessageId { get; }

		public OperationType operationType { get; }
		public string operationData { get; }

		public override string ToString()
		{
			return string.Format("[SimulatorMessage: Destination={0}, Source={1}, MessageId={2}, operationType={3}, operationData={4}]", 
			                     Destination, Source, MessageId, operationType, operationData);
		}
	}
}

