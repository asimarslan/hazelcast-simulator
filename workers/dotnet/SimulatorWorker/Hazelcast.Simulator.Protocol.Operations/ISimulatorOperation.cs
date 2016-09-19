using System.Threading.Tasks;
using Hazelcast.Simulator.Protocol.Processors;

namespace Hazelcast.Simulator.Protocol.Operations
{
	public interface ISimulatorOperation
	{
	    Task Run(OperationContext operationContext);
	}
}

