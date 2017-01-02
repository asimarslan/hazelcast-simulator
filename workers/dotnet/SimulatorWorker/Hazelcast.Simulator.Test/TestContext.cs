// Copyright (c) 2008-2017, Hazelcast, Inc. All Rights Reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Hazelcast.Core;
using Hazelcast.Simulator.Protocol.Connector;
using Hazelcast.Simulator.Protocol.Operations;
using static Hazelcast.Simulator.Protocol.Core.SimulatorAddress;

namespace Hazelcast.Simulator.Test
{
    public class TestContext : ITestContext
    {
        public const string LOCALHOST = "127.0.0.1";

        internal IHazelcastInstance HazelcastClient { get; private set; }

        private readonly string testId;
        private readonly WorkerConnector connector;

        private volatile bool stopped;
        private bool warmingUp;

        public TestContext(string testId, IHazelcastInstance hazelcastClient = null, WorkerConnector connector = null)
        {
            this.testId = testId;
            HazelcastClient = hazelcastClient;
            this.connector = connector;
        }

        public bool IsWarmingUp() => warmingUp;

        public string GetTestId() => testId;

        public string GetPublicIpAddress() => connector.PublicIpAddress;

        public bool IsStopped() => stopped;

        public void Stop() => stopped = true;

        public void EchoCoordinator(string msg) => connector.Submit(connector.WorkerAddress, COORDINATOR, new LogOperation(msg));

        public void BeforeWarmup() => warmingUp = true;

        public void AfterWarmup() => warmingUp = stopped = false;
    }
}