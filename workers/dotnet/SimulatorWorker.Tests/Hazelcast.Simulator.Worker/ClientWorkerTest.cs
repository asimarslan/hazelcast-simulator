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
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Buffers;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Operations;
using Hazelcast.Simulator.Test;
using log4net.Config;
using NUnit.Framework;

namespace Hazelcast.Simulator.Worker
{
    [TestFixture]
    public class ClientWorkerTest
    {
        private ClientWorker clientWorker;
        private RemoteConnector rc;

        private static readonly SimulatorAddress TestAddress = new SimulatorAddress(AddressLevel.TEST, 1, 1, 1);
        private static readonly SimulatorAddress WorkerAddress = new SimulatorAddress(AddressLevel.WORKER, 1, 1, 0);
        private static readonly SimulatorAddress CoordinatorAddress = new SimulatorAddress(AddressLevel.COORDINATOR, 0, 0, 0);

        private Task startTask;

        [OneTimeSetUp]
        public void Init()
        {
            BasicConfigurator.Configure();

            DirectoryInfo tmpFolder = TestEnvironmentUtils.SetupFakeUserDir();
            Environment.SetEnvironmentVariable("WORKER_HOME", tmpFolder.FullName);
            clientWorker = new ClientWorker("dotnetclient", "127.0.0.1:5701", 1, 1, 9002, null, false, 0);
            startTask = clientWorker.Start();
            while (!clientWorker.Ready)
            {
                Thread.Sleep(10);
            }

            rc = new RemoteConnector("127.0.0.1", 9002, WorkerAddress);
            rc.Start().Wait();
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            rc.Shutdown();
            clientWorker.Shutdown();
            startTask.Wait();
            TestEnvironmentUtils.TeardownFakeUserDir();
        }

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

        //        [Test]
        //        public void TestWorkerAddressFileExists()
        //        {
        //            Assert.True(File.Exists(Environment.GetEnvironmentVariable("WORKER_HOME") + "/worker.address"));
        //        }
        //
        //        [Test]
        //        public void TestCreateTest()
        //        {
        //            var testAddress = new SimulatorAddress(AddressLevel.TEST, 1, 1, 1);
        //            var workerAddress = new SimulatorAddress(AddressLevel.WORKER, 1, 1, 0);
        //            var coordinatorAddress = new SimulatorAddress(AddressLevel.COORDINATOR, 0, 0, 0);
        //
        //            Response decodeResponse = CreateTest(workerAddress, coordinatorAddress);
        //
        //            Assert.AreEqual(messageId, decodeResponse.MessageId);
        //            Assert.AreEqual(coordinatorAddress, decodeResponse.Destination);
        //            Assert.AreEqual(1, decodeResponse.Size());
        //            AssertResponse(decodeResponse, workerAddress);
        //
        //            //delete test
        //            Response finalResponse = SendMessage(testAddress, coordinatorAddress, OperationType.StartTestPhase, "{'testPhase':'LOCAL_TEARDOWN'}");
        //            AssertResponse(finalResponse, testAddress);
        //        }
        //
        //        [Test]
        //        public void TestStartStopTest()
        //        {
        //            var testAddress = new SimulatorAddress(AddressLevel.TEST, 1, 1, 1);
        //            var workerAddress = new SimulatorAddress(AddressLevel.WORKER, 1, 1, 0);
        //            var coordinatorAddress = new SimulatorAddress(AddressLevel.COORDINATOR, 0, 0, 0);
        //
        //            Response createResponse = CreateTest(workerAddress, coordinatorAddress);
        //            AssertResponse(createResponse, workerAddress);
        //
        //            Response startResponse = SendMessage(testAddress, coordinatorAddress, OperationType.StartTest, "{ 'targetType':'CLIENT','targetWorkers':[]}");
        //            AssertResponse(startResponse, testAddress);
        //
        //            Response stopResponse = SendMessage(testAddress, coordinatorAddress, OperationType.StopTest, "{}");
        //
        //            Assert.AreEqual(messageId, stopResponse.MessageId);
        //            Assert.AreEqual(coordinatorAddress, stopResponse.Destination);
        //            Assert.AreEqual(1, stopResponse.Size());
        //            AssertResponse(stopResponse, testAddress);
        //
        //            //delete test
        //            SendMessage(testAddress, coordinatorAddress, OperationType.StartTestPhase, "{'testPhase':'LOCAL_TEARDOWN'}");
        //        }
        //
        [Test]
        public void TestPhaseTest()
        {
            Response createResponse = rc.Send(CoordinatorAddress, WorkerAddress, OperationType.CreateTest,
                @"{'testIndex':1,'testId':'SimulatorTest','properties':{'threadCount':'1','class':'Custom.Simulator.Name.SimulatorTest'}}").Result;
            AssertResponse(createResponse, WorkerAddress);

            var phaseList = new[]
            {
                TestPhase.Setup,
                TestPhase.LocalPrepare,
                TestPhase.GlobalPrepare,
                TestPhase.Warmup,
                TestPhase.LocalAfterWarmup,
                TestPhase.GlobalAfterWarmup,
                TestPhase.Run,
                TestPhase.GlobalVerify,
                TestPhase.LocalVerify,
                TestPhase.GlobalTeardown
            };

            foreach (TestPhase testPhase in phaseList)
            {
                Response phaseResponse = rc.Send(CoordinatorAddress, TestAddress, OperationType.StartTestPhase, $"{{'testPhase':'{testPhase.GetName()}'}}").Result;
                AssertResponse(phaseResponse, TestAddress);
            }

            var testInstance = clientWorker.operationProcessor.operationContext.Tests.Values.First().TestInstance as SimulatorTest;

            Assert.NotNull(testInstance);
            foreach (TestPhase testPhase in phaseList)
            {
                switch (testPhase)
                {
                    case TestPhase.Setup:
                        Assert.AreEqual(2, testInstance.InvokeCounts[TestPhase.Setup]);
                        break;
                    case TestPhase.Warmup:
                        Assert.AreEqual(0, testInstance.InvokeCounts[TestPhase.Warmup]);
                        break;
                    default:
                        Assert.AreEqual(1, testInstance.InvokeCounts[testPhase], $"{testPhase} not invoked!");
                        break;
                }
            }
        }

//        private Response SendMessage(SimulatorAddress destination, SimulatorAddress source, OperationType operationType, string payload)
//        {
//            var message = new SimulatorMessage(destination, source, 0, operationType, payload);
//
//            IByteBuffer buffer = ByteBufferUtil.DefaultAllocator.Buffer(1000);
//            message.EncodeByteBuf(buffer);
//
//            var tcpClient = new TcpClient("127.0.0.1", 9002);
//            if (!tcpClient.Connected)
//            {
//                Assert.Fail("Socket not connected!!!");
//            }
//            NetworkStream networkStream = tcpClient.GetStream();
//
//            byte[] bufArray = buffer.ToArray();
//            Array.Resize(ref bufArray, buffer.ReadableBytes);
//
//            networkStream.Write(bufArray, 0, bufArray.Length);
//
//            var response = new byte[500];
//            int rcvSize = networkStream.Read(response, 0, response.Length);
//
//            IByteBuffer responseBuffer = ByteBufferUtil.DefaultAllocator.Buffer(rcvSize);
//            responseBuffer.WriteBytes(response);
//
//            Response decodeResponse = responseBuffer.DecodeResponse();
//            Console.WriteLine(decodeResponse);
//            networkStream.Dispose();
//            tcpClient.Close();
//            return decodeResponse;
//        }

        private void AssertResponse(Response response, SimulatorAddress address)
        {
            //            Assert.True(response.Parts.ContainsKey(address));
            Assert.AreEqual(ResponseType.Success, response.Parts[address].ResponseType);
        }
    }
}