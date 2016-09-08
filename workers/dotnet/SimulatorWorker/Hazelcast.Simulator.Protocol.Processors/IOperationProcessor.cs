using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Operations;

namespace Hazelcast.Simulator.Protocol.Processors
{
    using System.Threading.Tasks;

    public interface IOperationProcessor
	{
		Task<ResponseType> Process(ISimulatorOperation operation, SimulatorAddress sourceAddress);
	}
}

