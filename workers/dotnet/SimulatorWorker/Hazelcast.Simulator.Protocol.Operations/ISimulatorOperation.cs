using System.Threading.Tasks;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Processors;

namespace Hazelcast.Simulator.Protocol.Operations
{
    /// <summary>
    /// Simulator operation that is converted from a <see cref="SimulatorMessage"/>
    /// <seealso cref="OperationTypes"/>
    /// </summary>
	public interface ISimulatorOperation
	{
	    /// <summary>
	    /// Called for operations that are run on worker.
	    /// </summary>
	    /// <param name="operationContext">operation context provide the worker resources that an operation need.</param>
	    /// <returns></returns>
	    Task<ResponseResult> Run(OperationContext operationContext);
	}
}

