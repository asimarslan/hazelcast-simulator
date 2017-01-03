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

using System.Net;
using Hazelcast.Core;
using Moq;
using NUnit.Framework;

namespace Hazelcast.Simulator.Utils
{
    [TestFixture]
    public class HazelcastUtilTest
    {
        private const string ADDRESS = "127.0.0.1";
        private Mock<IHazelcastInstance> hzClient;

        [SetUp]
        public void Setup()
        {
            hzClient = new Mock<IHazelcastInstance>();
            var iClient = new Mock<IEndpoint>();
            iClient.Setup(t => t.GetSocketAddress()).Returns(new IPEndPoint(IPAddress.Parse(ADDRESS), 5701));
            hzClient.Setup(client => client.GetLocalEndpoint()).Returns(iClient.Object);
        }

        [TearDown]
        public void TearDown() {}

        [Test]
        public void TestGetAddress_hzNull()
        {
            string theAddress = HazelcastUtils.GetHazelcastAddress("dotnetclient", ADDRESS, null);
            Assert.AreEqual($"client:{ADDRESS}", theAddress);
        }

        [Test]
        public void TestGetAddress_hzMock()
        {
            string theAddress = HazelcastUtils.GetHazelcastAddress("dotnetclient", ADDRESS, hzClient.Object);
            Assert.AreEqual("127.0.0.1:5701", theAddress);
        }
    }
}