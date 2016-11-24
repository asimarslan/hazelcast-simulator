using System.Collections.Concurrent;
using Hazelcast.Core;
using Hazelcast.Simulator.Protocol.Connector;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Test;

namespace Hazelcast.Simulator.Protocol.Processors
{
    /// <summary>
    /// A stateful context for executing simulator operations.
    /// </summary>
    public class OperationContext
    {
        public IHazelcastInstance HazelcastInstance { get;}
        public SimulatorAddress WorkerAddress { get; }

        public WorkerConnector Connector { get; set; }

        public ConcurrentDictionary<int, TestContainer> Tests { get; }

        public OperationContext(IHazelcastInstance hazelcastInstance, SimulatorAddress workerAddress, WorkerConnector connector)
        {
            this.HazelcastInstance = hazelcastInstance;
            this.WorkerAddress = workerAddress;
            this.Connector = connector;
            this.Tests = new ConcurrentDictionary<int, TestContainer>();
        }
    }
}