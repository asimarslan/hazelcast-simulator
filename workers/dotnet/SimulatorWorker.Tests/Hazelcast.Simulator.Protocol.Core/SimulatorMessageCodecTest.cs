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

using DotNetty.Buffers;
using Hazelcast.Simulator.Protocol.Operations;
using NUnit.Framework;

namespace Hazelcast.Simulator.Protocol.Core
{
    [TestFixture]
    public class SimulatorMessageCodecTest
    {
        [SetUp]
        public void Setup() {}

        [TearDown]
        public void TearDown() {}

        [Test]
        public void TestClientWorker() {}

        [Test]
        public void TestSimulatorMessagePingEncode()
        {
            var destination = new SimulatorAddress(AddressLevel.WORKER, 1, 1, 0);
            var source = new SimulatorAddress(AddressLevel.COORDINATOR, 0, 0, 0);
            var message = new SimulatorMessage(destination, source, 1, OperationType.Ping, "{}");

            IByteBuffer buffer = ByteBufferUtil.DefaultAllocator.Buffer(100);
            message.EncodeByteBuf(buffer);
            Assert.AreEqual(buffer.ReadableBytes, 54);
        }
    }
}