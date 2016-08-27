namespace Hazelcast.Simulator.Protocol.Operations
{
	public class StartTestPhaseOperation:ISimulatorOperation
	{
		public string TestPhase { get;}

		public StartTestPhaseOperation(string testPhase)
		{
			TestPhase = testPhase;
		}

	}
}

