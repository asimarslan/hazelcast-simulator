using System.Threading.Tasks;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Processors;

namespace Hazelcast.Simulator.Protocol.Operations
{
    /// <summary>
    /// Initiates the shutdown process of the Worker.
    /// </summary>
	public class TerminateWorkerOperation : ISimulatorOperation
	{
	    public Task<ResponseResult> Run(OperationContext operationContext)
	    {
	        throw new System.NotImplementedException();

	    }
	}
}