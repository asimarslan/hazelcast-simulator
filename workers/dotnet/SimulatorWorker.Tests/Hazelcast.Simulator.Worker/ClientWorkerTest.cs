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
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Operations;
using log4net.Config;
using NUnit.Framework;
using DotNetty.Buffers;

namespace Hazelcast.Simulator.Worker
{
    [TestFixture]
    public class ClientWorkerTest
    {
        private ClientWorker clientWorker;

        [OneTimeSetUp]
        public void Init()
        {
            BasicConfigurator.Configure();

            var tmpFolder = TestEnvironmentUtils.SetupFakeUserDir();
            Environment.SetEnvironmentVariable("WORKER_HOME", tmpFolder.FullName);
            this.clientWorker = new ClientWorker("dotnetclient", "127.0.0.1:5701", 1, 1, 9002, null, false, 0);
            this.clientWorker.Start();
            while (!this.clientWorker.Ready)
            {
                Thread.Sleep(10);
            }
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            this.clientWorker.Shutdown();
            TestEnvironmentUtils.TeardownFakeUserDir();
        }

        [Test]
        public void TestClientWorker()
        {
            Assert.NotNull(this.clientWorker);
        }

        [Test]
        public void TestPing()
        {
            var workerAddress = new SimulatorAddress(AddressLevel.WORKER, 1, 1, 0);
            var coordinatorAddress = new SimulatorAddress(AddressLevel.COORDINATOR, 0, 0, 0);
            var message = new SimulatorMessage(workerAddress, coordinatorAddress, 1, OperationType.Ping, "{}");

            var buffer = ByteBufferUtil.DefaultAllocator.Buffer(100);
            message.EncodeByteBuf(buffer);

            TcpClient tcpClient = new TcpClient("127.0.0.1", 9002);
            if (!tcpClient.Connected)
            {
                Assert.Fail("Socket not connected!!!");
            }
            NetworkStream networkStream = tcpClient.GetStream();

            var bufArray = buffer.ToArray();
            Array.Resize(ref bufArray, buffer.ReadableBytes);

            networkStream.Write(bufArray, 0 , bufArray.Length);

            byte[] response= new byte[100];
            var rcvSize = networkStream.Read(response, 0, response.Length);

            IByteBuffer responseBuffer = ByteBufferUtil.DefaultAllocator.Buffer(rcvSize);
            responseBuffer.WriteBytes(response);
            Response decodeResponse = responseBuffer.DecodeResponse();

            Assert.AreEqual(1, decodeResponse.MessageId);
            Assert.AreEqual(coordinatorAddress, decodeResponse.Destination);
            Assert.AreEqual(1, decodeResponse.Size());
            Assert.AreEqual(ResponseType.Success, decodeResponse.Parts[workerAddress].ResponseType);
            networkStream.Dispose();
            tcpClient.Close();
        }

        [Test]
        public void TestWorkerAddressFileExists()
        {
            Assert.True(File.Exists(Environment.GetEnvironmentVariable("WORKER_HOME") + "/worker.address"));
        }

        [Test]
        public void TestCreateTest()
        {
            var workerAddress = new SimulatorAddress(AddressLevel.WORKER, 1, 1, 0);
            var coordinatorAddress = new SimulatorAddress(AddressLevel.COORDINATOR, 0, 0, 0);
            var message = new SimulatorMessage(workerAddress, coordinatorAddress, 1, OperationType.CreateTest,
                @"{'testIndex':1,'testId':'foo','properties':{'threadCount':'1','class':'com.hazelcast.simulator.tests.SuccessTest'}}");

            var buffer = ByteBufferUtil.DefaultAllocator.Buffer(1000);
            message.EncodeByteBuf(buffer);

            TcpClient tcpClient = new TcpClient("127.0.0.1", 9002);
            if (!tcpClient.Connected)
            {
                Assert.Fail("Socket not connected!!!");
            }
            NetworkStream networkStream = tcpClient.GetStream();

            var bufArray = buffer.ToArray();
            Array.Resize(ref bufArray, buffer.ReadableBytes);

            networkStream.Write(bufArray, 0 , bufArray.Length);

            byte[] response= new byte[500];
            var rcvSize = networkStream.Read(response, 0, response.Length);

            IByteBuffer responseBuffer = ByteBufferUtil.DefaultAllocator.Buffer(rcvSize);
            responseBuffer.WriteBytes(response);
            Response decodeResponse = responseBuffer.DecodeResponse();

            Console.WriteLine(decodeResponse);

            Assert.AreEqual(1, decodeResponse.MessageId);
            Assert.AreEqual(coordinatorAddress, decodeResponse.Destination);
            Assert.AreEqual(1, decodeResponse.Size());
            Assert.AreEqual(ResponseType.Success, decodeResponse.Parts[workerAddress].ResponseType);
            networkStream.Dispose();
            tcpClient.Close();
        }

    }
}