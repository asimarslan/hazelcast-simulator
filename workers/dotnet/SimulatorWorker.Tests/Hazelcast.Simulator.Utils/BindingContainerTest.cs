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

using System.Collections.Generic;
using Hazelcast.Core;
using Hazelcast.Simulator.Probe;
using Hazelcast.Simulator.Test;
using NUnit.Framework;
using static Hazelcast.Simulator.Utils.DependencyInjectionUtil;
using TestContext = Hazelcast.Simulator.Test.TestContext;
using Moq;

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
            this.hzClient = new Mock<IHazelcastInstance>();
            var testContext = new TestContext(TestId, this.hzClient.Object);
            var dict = new Dictionary<string, string>();
            dict.Add("testStrField","Value0");
            dict.Add("testLongField","99");
            dict.Add("TestIntProperty","90");
            var testCase = new TestCase(TestId, dict);
            this.bindingContainer = new BindingContainer(testContext, testCase);
            this.testInstance = new Dependent();
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void TestBindingContainerInit()
        {
            this.bindingContainer.Bind(this.testInstance);
            Assert.IsNotNull(this.bindingContainer);
        }

        [Test]
        public void TestBindingHazelcast()
        {
            this.bindingContainer.Bind(this.testInstance);
            Assert.IsNotNull(this.testInstance);
            Assert.IsNotNull(this.testInstance.hazelcastClient);
        }

        [Test]
        public void TestBindingTestContext()
        {
            this.bindingContainer.Bind(this.testInstance);
            Assert.IsNotNull(this.testInstance);
            Assert.IsNotNull(this.testInstance.testContext);
        }

        [Test]
        public void TestBindingProbes()
        {
            this.bindingContainer.Bind(this.testInstance);
            Assert.IsNotNull(this.testInstance.probe1);
            Assert.IsNotNull(this.testInstance.probe2);
            Assert.IsNotNull(this.testInstance.probe3);
            Assert.AreEqual(this.bindingContainer.GetProbes().Count, 2);
            Assert.AreEqual(this.testInstance.probe1.GetType(), typeof(HdrProbe));
            Assert.AreEqual(this.testInstance.probe2.GetType(), typeof(HdrProbe));
        }

    }
}