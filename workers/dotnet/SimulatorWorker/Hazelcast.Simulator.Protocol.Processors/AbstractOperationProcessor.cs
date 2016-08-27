using System;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Operations;
using log4net;

namespace Hazelcast.Simulator.Protocol.Processors
{
	public class AbstractOperationProcessor:IOperationProcessor
	{

		private static ILog Logger = LogManager.GetLogger(typeof(AbstractOperationProcessor));

		public ResponseType Process(ISimulatorOperation operation, SimulatorAddress sourceAddress)
		{
			throw new NotImplementedException();
		}
	}
}

