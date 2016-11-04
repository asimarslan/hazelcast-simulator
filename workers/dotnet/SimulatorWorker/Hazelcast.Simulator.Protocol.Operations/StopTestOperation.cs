using System.Threading.Tasks;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Processors;
using Hazelcast.Simulator.Test;

namespace Hazelcast.Simulator.Protocol.Operations
{
    /// <summary>
    /// Stops the <see cref="TestPhase.Run"/> phase of a Simulator Test.
    /// </summary>
	public class StopTestOperation : ISimulatorOperation
	{
	    public Task<ResponseResult> Run(OperationContext operationContext)
	    {
	        throw new System.NotImplementedException();

	    }
	}
}