using System;
using Hazelcast.Core;
using Hazelcast.Simulator.Protocol.Connector;
using Hazelcast.Simulator.Protocol.Operations;
using static Hazelcast.Simulator.Protocol.Core.SimulatorAddress;

namespace Hazelcast.Simulator.Test
{
    public class TestContext : ITestContext
    {
        public const string LOCALHOST = "127.0.0.1";

        internal IHazelcastInstance TargetInstance { get; private set; }

        private readonly string testId;
        private readonly string publicIpAddress;
        private readonly WorkerConnector connector;

        private volatile bool stopped;
        private bool warmingUp;

        public TestContext(string testId, IHazelcastInstance targetInstance = null, string publicIpAddress = LOCALHOST,
            WorkerConnector connector = null)
        {
            this.testId = testId;
            this.TargetInstance = targetInstance;
            this.publicIpAddress = publicIpAddress;
            this.connector = connector;
        }

        public bool IsWarmingUp() => this.warmingUp;

        public string GetTestId() => this.testId;

        public string GetPublicIpAddress() => this.publicIpAddress;

        public bool IsStopped() => this.stopped;

        public void Stop() => this.stopped = true;

        public void EchoCoordinator(string msg) => this.connector.Submit(COORDINATOR, new LogOperation(msg));

        public void BeforeWarmup() => this.warmingUp = true;

        public void AfterWarmup() => this.warmingUp = this.stopped = false;
    }
}