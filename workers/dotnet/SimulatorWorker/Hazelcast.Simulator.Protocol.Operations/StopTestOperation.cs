using System;
using System.Threading.Tasks;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Processors;
using Hazelcast.Simulator.Test;
using Newtonsoft.Json;

namespace Hazelcast.Simulator.Protocol.Operations
{
    /// <summary>
    /// Stops the <see cref="TestPhase.Run"/> phase of a Simulator Test.
    /// </summary>
	public class StopTestOperation : ISimulatorOperation
	{
	    [JsonIgnore]
	    private SimulatorMessage msg;

	    public void SetSimulatorMessage(SimulatorMessage simulatorMessage) => this.msg = simulatorMessage;

	    public async Task<ResponseType> Run(OperationContext operationContext)
	    {
	        TestContainer testContainer;
	        if (operationContext.Tests.TryGetValue(msg.Destination.TestIndex, out testContainer))
	        {
	            testContainer.TestContext.Stop();
//	            throw new InvalidOperationException($"Test not created yet with testIndex:{msg.Destination.TestIndex}");
	        }
	        return ResponseType.Success;
	    }
	}
}