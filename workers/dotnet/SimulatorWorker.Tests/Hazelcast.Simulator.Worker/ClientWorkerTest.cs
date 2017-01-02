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

using System;
using System.IO;
using System.Linq;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Operations;
using Hazelcast.Simulator.Protocol.Processors;
using Hazelcast.Simulator.Test;
using Hazelcast.Simulator.Utils;
using NUnit.Framework;
using static Hazelcast.Simulator.Worker.RemoteConnector;

namespace Hazelcast.Simulator.Worker
{
    [TestFixture]
    public class ClientWorkerTest : BaseTestOperation
    {
        [Test]
        public void TestClientWorker()
        {
            Assert.NotNull(clientWorker);
        }

        [Test]
        public void TestPing()
        {
            Response decodeResponse = rc.Send(CoordinatorAddress, WorkerAddress, OperationType.Ping, "{}").Result;

            Assert.AreEqual(rc.LastMessageId, decodeResponse.MessageId);
            Assert.AreEqual(CoordinatorAddress, decodeResponse.Destination);
            Assert.AreEqual(1, decodeResponse.Size());
            AssertResponse(decodeResponse, WorkerAddress);
        }

        [Test]
        public void TestWorkerAddressFileExists()
        {
            Assert.True(File.Exists(Environment.GetEnvironmentVariable("WORKER_HOME") + "/worker.address"));
        }

        [Test]
        public void TestStartWorkerTwice()
        {
            clientWorker.Start().ContinueWith(t =>
            {
                Assert.True(t.IsFaulted);
                Assert.AreEqual(t.Exception?.Flatten().InnerExceptions.First().GetType(), typeof(SimulatorProtocolException));
            }).Wait();
        }

        [Test]
        public void TestPublicIpAddress()
        {
            Assert.AreEqual(clientWorker.PublicIpAddress, PUBLIC_ADDRESS);
            Assert.AreEqual(clientWorker.Connector.PublicIpAddress, PUBLIC_ADDRESS);
        }

        [Test]
        public void TestLogOperation()
        {
            Response logResponse = rc.Send(CoordinatorAddress, WorkerAddress, OperationType.Log, "{'message':'Test Log message','level':'INFO'}").Result;

            Assert.AreEqual(rc.LastMessageId, logResponse.MessageId);
            Assert.AreEqual(CoordinatorAddress, logResponse.Destination);
            Assert.AreEqual(1, logResponse.Size());
            AssertResponse(logResponse, WorkerAddress);
        }

        [Test]
        public void TestAuthOperation()
        {
            Response response = rc.Send(CoordinatorAddress, WorkerAddress, OperationType.Auth, "{}").Result;

            Assert.AreEqual(rc.LastMessageId, response.MessageId);
            Assert.AreEqual(CoordinatorAddress, response.Destination);
            Assert.AreEqual(1, response.Size());
            AssertResponse(response, WorkerAddress);
        }
    }
}