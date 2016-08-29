using System;
using Hazelcast.Core;
using Hazelcast.Simulator.Worker;
using Hazelcast.Simulator.Protocol.Core;
using System.Collections.Concurrent;
namespace Hazelcast.Simulator.Protocol.Processors
{
	public class WorkerOperationProcessor: AbstractOperationProcessor
	{
		readonly IHazelcastInstance hazelcastInstance;
		readonly ClientWorker worker;
		readonly SimulatorAddress simulatorAddress;

		ConcurrentDictionary<string, TestContainer> tests = new ConcurrentDictionary<string, TestContainer>();

		public WorkerOperationProcessor(IHazelcastInstance hazelcastInstance, ClientWorker worker, SimulatorAddress simulatorAddress)
		{
			this.simulatorAddress = simulatorAddress;
			this.worker = worker;
			this.hazelcastInstance = hazelcastInstance;
		}
	}
}

