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
using System.Threading.Tasks;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Operations;
using Hazelcast.Simulator.Protocol.Processors;
using Hazelcast.Simulator.Test;
using Hazelcast.Simulator.Utils;
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
            rc = new RemoteConnector("127.0.0.1", 9002, WorkerAddress);
            rc.Start().Wait();
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            rc.Shutdown();
            clientWorker.Shutdown();
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

        [Test]
        public void TestWorkerAddressFileExists()
        {
            Assert.True(File.Exists(Environment.GetEnvironmentVariable("WORKER_HOME") + "/worker.address"));
        }

        [Test]
        public void TestCreateTest()
        {
            Response createResponse = rc.Send(CoordinatorAddress, WorkerAddress, OperationType.CreateTest,
                @"{'testIndex':1,'testId':'SimulatorTest','properties':{'threadCount':'1','class':'Custom.Simulator.Name.SimulatorTest'}}").Result;

            AssertResponse(createResponse, WorkerAddress);

            Assert.AreEqual(rc.LastMessageId, createResponse.MessageId);
            Assert.AreEqual(CoordinatorAddress, createResponse.Destination);
            Assert.AreEqual(1, createResponse.Size());
            AssertResponse(createResponse, WorkerAddress);

            //delete test
            Response finalResponse = rc.Send(CoordinatorAddress, TestAddress, OperationType.StartTestPhase, "{'testPhase':'LOCAL_TEARDOWN'}").Result;
            rc.WaitPhaseComplete(TestPhase.LocalTeardown);
            AssertResponse(finalResponse, TestAddress);
        }

        [Test]
        public void TestStartStopTest()
        {
            Response createResponse = rc.Send(CoordinatorAddress, WorkerAddress, OperationType.CreateTest,
                @"{'testIndex':1,'testId':'SimulatorTest','properties':{'threadCount':'1','class':'Custom.Simulator.Name.SimulatorTest'}}").Result;

            AssertResponse(createResponse, WorkerAddress);

            Response startResponse = rc.Send(CoordinatorAddress, TestAddress, OperationType.StartTest, "{ 'targetType':'CLIENT','targetWorkers':[]}").Result;
            AssertResponse(startResponse, TestAddress);

            Response stopResponse = rc.Send(CoordinatorAddress, TestAddress, OperationType.StopTest, "{}").Result;

            Assert.AreEqual(rc.LastMessageId, stopResponse.MessageId);
            Assert.AreEqual(CoordinatorAddress, stopResponse.Destination);
            Assert.AreEqual(1, stopResponse.Size());
            AssertResponse(stopResponse, TestAddress);

            //delete test
            Response finalResponse = rc.Send(CoordinatorAddress, TestAddress, OperationType.StartTestPhase, "{'testPhase':'LOCAL_TEARDOWN'}").Result;
            rc.WaitPhaseComplete(TestPhase.LocalTeardown);
            AssertResponse(finalResponse, TestAddress);
        }

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
                rc.WaitPhaseComplete(testPhase);
                AssertResponse(phaseResponse, TestAddress);
            }
            var operationContext = ReflectionUtil.ReadInstanceFieldValue<OperationContext>(clientWorker.operationProcessor, typeof(OperationProcessor), "operationContext");
            var testInstance = operationContext.Tests.Values.First().TestInstance as SimulatorTest;

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

            //delete test
            Response finalResponse = rc.Send(CoordinatorAddress, TestAddress, OperationType.StartTestPhase, "{'testPhase':'LOCAL_TEARDOWN'}").Result;
            rc.WaitPhaseComplete(TestPhase.LocalTeardown);
            AssertResponse(finalResponse, TestAddress);
        }

        private void AssertResponse(Response response, SimulatorAddress address)
        {
            //            Assert.True(response.Parts.ContainsKey(address));
            Assert.AreEqual(ResponseType.Success, response.Parts[address].ResponseType);
        }
    }
}