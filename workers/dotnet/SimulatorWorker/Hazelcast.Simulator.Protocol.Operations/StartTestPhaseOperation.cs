using System.Threading.Tasks;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Processors;
using Newtonsoft.Json;

namespace Hazelcast.Simulator.Protocol.Operations
{
	public class StartTestPhaseOperation:ISimulatorOperation
	{
	    [JsonProperty("testPhase")]
	    private readonly string testPhase;

	    public Task Run(OperationContext operationContext, SimulatorMessage msg)
	    {
	        throw new System.NotImplementedException();
	    }
	}
}

