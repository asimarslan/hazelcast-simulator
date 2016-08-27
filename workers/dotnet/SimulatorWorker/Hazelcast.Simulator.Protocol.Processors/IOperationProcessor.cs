using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Operations;

namespace Hazelcast.Simulator.Protocol.Processors
{
	public interface IOperationProcessor
	{
		ResponseType Process(ISimulatorOperation operation, SimulatorAddress sourceAddress);
	}
}

