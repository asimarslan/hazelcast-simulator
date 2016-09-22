using System.Threading.Tasks;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Processors;

namespace Hazelcast.Simulator.Protocol.Operations
{
	/**
	 * Creates traffic on the wire, so the WorkerProcessFailureMonitor
	 * on the Agent can see, that the Worker is still responsive.
	 *
	 * This is needed for long running test phases, which lead to a radio silence on the wire.
	 */
	public class PingOperation : ISimulatorOperation
	{
	    public Task Run(OperationContext operationContext, SimulatorMessage msg)
	    {
	        throw new System.NotImplementedException();
	    }
	}
}