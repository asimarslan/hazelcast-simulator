using System.Collections.Concurrent;
using Hazelcast.Core;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Test;

namespace Hazelcast.Simulator.Protocol.Processors
{
    public class OperationContext
    {
        public IHazelcastInstance HazelcastInstance { get;}
        public SimulatorAddress WorkerAddress { get; }
        public ConcurrentDictionary<string, TestContainer> Tests { get; }

        public OperationContext(IHazelcastInstance hazelcastInstance, SimulatorAddress workerAddress)
        {
            this.HazelcastInstance = hazelcastInstance;
            this.WorkerAddress = workerAddress;
            this.Tests = new ConcurrentDictionary<string, TestContainer>();
        }
    }
}