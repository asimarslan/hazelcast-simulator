using System;
using System.Threading.Tasks;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Processors;
using Hazelcast.Simulator.Test;
using Newtonsoft.Json;

namespace Hazelcast.Simulator.Protocol.Operations
{
	public class StartTestPhaseOperation:ISimulatorOperation
	{
	    [JsonProperty("testPhase")]
	    private readonly string testPhaseStr;

	    public StartTestPhaseOperation(TestPhase testPhase)
	    {
	        this.testPhaseStr = testPhase.GetName();
	    }

	    public TestPhase GetTestPhase() => this.testPhaseStr.ToTestPhase();

	    public Task<ResponseResult> Run(OperationContext operationContext)
	    {
	        throw new System.NotImplementedException();
	    }
	}
}

