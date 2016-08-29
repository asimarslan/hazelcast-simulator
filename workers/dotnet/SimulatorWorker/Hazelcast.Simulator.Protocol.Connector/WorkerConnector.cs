using System;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Processors;
using Hazelcast.Core;
using Hazelcast.Simulator.Worker;

namespace Hazelcast.Simulator.Protocol.Connector
{
	public class WorkerConnector
	{

		public IOperationProcessor Processor { get; private set; }

		private readonly SimulatorAddress localAddress;
		private readonly int addressIndex;

		private readonly ConnectionManager connectionManager;
		private readonly TestProcessorManager testProcessorManager;

		public WorkerConnector(SimulatorAddress localAddress, int port, IHazelcastInstance hazelcastInstance, ClientWorker worker)
		{
			this.localAddress = localAddress;
		}
	}
}

