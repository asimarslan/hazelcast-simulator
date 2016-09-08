using System.Collections.Concurrent;
using Hazelcast.Core;
using Hazelcast.Simulator.Test;

namespace Hazelcast.Simulator.Protocol.Processors
{
    public class OperationContext
    {
        public IHazelcastInstance HazelcastInstance { get;}

        ConcurrentDictionary<string, TestContainer> tests = new ConcurrentDictionary<string, TestContainer>();
    }
}