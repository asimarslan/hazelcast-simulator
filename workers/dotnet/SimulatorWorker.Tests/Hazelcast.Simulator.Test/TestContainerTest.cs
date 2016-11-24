// Copyright (c) 2008-2016, Hazelcast, Inc. All Rights Reserved.
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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hazelcast.Core;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Utils;
using Moq;
using NUnit.Framework;

namespace Hazelcast.Simulator.Test
{
    [TestFixture]
    public class TestContainerTest
    {
        private const string TestId = "The-Test-Id";

        private BindingContainer bindingContainer;
        private TestSample testInstance;
        private Mock<IHazelcastInstance> hzClient;

        private TestContainer testContainer;

        [SetUp]
        public void Setup()
        {
            this.hzClient = new Mock<IHazelcastInstance>();
            var testContext = new TestContext(TestId, this.hzClient.Object);
            var dict = new Dictionary<string, string>();
            var testCase = new TestCase(TestId, dict);
            var testAddress = new SimulatorAddress(AddressLevel.TEST, 1, 1, 1);

            this.testInstance = new TestSample();
            this.testContainer = new TestContainer(testContext, testCase, testAddress, this.testInstance);
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void TestSetupPhase()
        {
            Assert.IsNotNull(this.testContainer);
            Assert.IsNotNull(this.testContainer.TestInstance);
        }

        [Test]
        public void TestSetupPhaseInvoke()
        {
            this.testContainer.Invoke(TestPhase.Setup);
            Assert.AreEqual(2, this.testInstance.invokeCounts[TestPhase.Setup]);
        }

        [Test]
        public void TestPhaseInvoke()
        {
            var taskList = new List<Task>();
            foreach (TestPhase testPhase in Enum.GetValues(typeof(TestPhase)))
            {
                taskList.Add(this.testContainer.Invoke(testPhase));
            }

            Task.WaitAll(taskList.ToArray());

            foreach (TestPhase testPhase in Enum.GetValues(typeof(TestPhase)))
            {
                switch (testPhase)
                {
                    case TestPhase.Setup:
                        Assert.AreEqual(2, this.testInstance.invokeCounts[TestPhase.Setup]);
                        break;
                    case TestPhase.Warmup:
                        Assert.AreEqual(0, this.testInstance.invokeCounts[TestPhase.Warmup]);
                        break;
                    default:
                        Assert.AreEqual(1, this.testInstance.invokeCounts[testPhase], $"{testPhase} not invoked!");
                        break;
                }
            }
        }
    }
}