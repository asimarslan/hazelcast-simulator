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

using System.Collections.Generic;
using Hazelcast.Core;
using Hazelcast.Simulator.Probe;
using Hazelcast.Simulator.Test;
using Moq;
using NUnit.Framework;
using TestContext = Hazelcast.Simulator.Test.TestContext;

namespace Hazelcast.Simulator.Utils
{
    [TestFixture]
    public class BindingContainerTest
    {
        private const string TestId = "The-Test-Id";

        private BindingContainer bindingContainer;
        private Dependent testInstance;
        private Mock<IHazelcastInstance> hzClient;

        [SetUp]
        public void Setup()
        {
            hzClient = new Mock<IHazelcastInstance>();
            var testContext = new TestContext(TestId, hzClient.Object);
            var dict = new Dictionary<string, string>
            {
                { "testStrField", "Value0" },
                { "testLongField", "99" },
                { "TestIntProperty", "90" }
            };
            var testCase = new TestCase(TestId, dict);
            bindingContainer = new BindingContainer(testContext, testCase);
            testInstance = new Dependent();
        }

        [TearDown]
        public void TearDown() {}

        [Test]
        public void TestBindingContainerInit()
        {
            bindingContainer.Bind(testInstance);
            Assert.IsNotNull(bindingContainer);
        }

        [Test]
        public void TestBindingHazelcast()
        {
            bindingContainer.Bind(testInstance);
            Assert.IsNotNull(testInstance);
            Assert.IsNotNull(testInstance.hazelcastClient);
        }

        [Test]
        public void TestBindingTestContext()
        {
            bindingContainer.Bind(testInstance);
            Assert.IsNotNull(testInstance);
            Assert.IsNotNull(testInstance.testContext);
        }

        [Test]
        public void TestBindingProbes()
        {
            bindingContainer.Bind(testInstance);
            Assert.IsNotNull(testInstance.probe1);
            Assert.IsNotNull(testInstance.probe2);
            Assert.IsNotNull(testInstance.probe3);
            Assert.AreEqual(bindingContainer.GetProbes().Count, 2);
            Assert.AreEqual(testInstance.probe1.GetType(), typeof(HdrProbe));
            Assert.AreEqual(testInstance.probe2.GetType(), typeof(HdrProbe));
        }

        [Test]
        public void TestBindingUnusedProperty()
        {
            var testContext = new TestContext(TestId);
            var dict = new Dictionary<string, string>
            {
                { "testStrField", "Value0" },
                { "testLongField", "99" },
                { "TestIntProperty", "90" },
                { "UnusedPropert", "X" }
            };
            var testCase = new TestCase(TestId, dict);
            var container = new BindingContainer(testContext, testCase);
            var instance = new Dependent();
            Assert.Throws<BindingException>(() => container.Bind(instance));
        }
    }
}