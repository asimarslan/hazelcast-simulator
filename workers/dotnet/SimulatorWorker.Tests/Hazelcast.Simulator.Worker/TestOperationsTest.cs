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
using Newtonsoft.Json;
using NUnit.Framework;
using static Hazelcast.Simulator.Utils.FileUtils;
using static Hazelcast.Simulator.Worker.RemoteConnector;

namespace Hazelcast.Simulator.Worker
{
    [TestFixture]
    public class TestOperationsTest : BaseTestOperation
    {
        [SetUp]
        public void Setup() => CreateTest(1);

        [TearDown]
        public void TearDown() => DeleteTest(1);

        [Test]
        public void TestStartStopTest()
        {
            Response startResponse = rc.Send(CoordinatorAddress, TestAddress, OperationType.StartTest, START_CLIENT_PAYLOAD).Result;
            Response stopResponse = rc.Send(CoordinatorAddress, TestAddress, OperationType.StopTest, "{}").Result;

            var operationContext = ReflectionUtil.ReadInstanceFieldValue<OperationContext>(clientWorker.operationProcessor, typeof(OperationProcessor), "operationContext");
            var testInstance = operationContext.Tests.Values.First().TestInstance as SimulatorTest;

            var msg= rc.GetFirstLogMessage();
            var logOperation = msg?.ToOperation() as LogOperation;
            Assert.AreEqual(SimulatorTest.ECHO_TEXT, logOperation?.GetMessage());

            AssertResponse(startResponse, TestAddress);

            Assert.NotNull(testInstance);
            Assert.AreEqual(1, testInstance.InvokeCounts[TestPhase.Run]);

            Assert.AreEqual(rc.LastMessageId, stopResponse.MessageId);
            Assert.AreEqual(CoordinatorAddress, stopResponse.Destination);
            Assert.AreEqual(1, stopResponse.Size());
            AssertResponse(stopResponse, TestAddress);
        }

        [Test]
        public void TestStartSkipTest()
        {
            Response startResponse = rc.Send(CoordinatorAddress, TestAddress, OperationType.StartTest, START_MEMBER_PAYLOAD).Result;
            var operationContext = ReflectionUtil.ReadInstanceFieldValue<OperationContext>(clientWorker.operationProcessor, typeof(OperationProcessor), "operationContext");
            var testInstance = operationContext.Tests.Values.First().TestInstance as SimulatorTest;

            Assert.NotNull(testInstance);
            Assert.AreEqual(0, testInstance.InvokeCounts[TestPhase.Run]);

            Assert.AreEqual(rc.LastMessageId, startResponse.MessageId);
            Assert.AreEqual(CoordinatorAddress, startResponse.Destination);
            Assert.AreEqual(1, startResponse.Size());
            AssertResponse(startResponse, TestAddress);
        }

        [Test]
        public void TestPhaseTest()
        {
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
                var op = new StartTestPhaseOperation(testPhase);
                string json = JsonConvert.SerializeObject(op);

                Response phaseResponse = rc.Send(CoordinatorAddress, TestAddress, OperationType.StartTestPhase, json).Result;
                AssertResponse(phaseResponse, TestAddress);
                rc.WaitPhaseComplete(testPhase);
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
        }

        [Test]
        public void TestStartAllTests()
        {
            CreateTest(2);
            var testAddress = new SimulatorAddress(AddressLevel.TEST, 1, 1, 0);
            Response startResponse = rc.Send(CoordinatorAddress, testAddress, OperationType.StartTest, START_CLIENT_PAYLOAD).Result;
            AssertResponse(startResponse, TestAddress);
            AssertResponse(startResponse, TestAddress2);

            DeleteTest(2);
        }

        [Test]
        public void TestStopNotExistedTest()
        {
            Response stopResponse = rc.Send(CoordinatorAddress, TestAddress2, OperationType.StopTest, "{}").Result;

            Assert.AreEqual(rc.LastMessageId, stopResponse.MessageId);
            Assert.AreEqual(CoordinatorAddress, stopResponse.Destination);
            Assert.AreEqual(1, stopResponse.Size());
            AssertResponse(stopResponse, WorkerAddress);
        }

        [Test]
        public void TestStartNotExistedTest()
        {
            Response response = rc.Send(CoordinatorAddress, TestAddress2, OperationType.StartTest, START_CLIENT_PAYLOAD).Result;

            Assert.AreEqual(rc.LastMessageId, response.MessageId);
            Assert.AreEqual(CoordinatorAddress, response.Destination);
            Assert.AreEqual(1, response.Size());
            AssertResponse(response, WorkerAddress, ResponseType.FailureTestNotFound);
        }

        [Test]
        public void TestStartMatchingWorkerTest()
        {
            Response response = rc.Send(CoordinatorAddress, TestAddress, OperationType.StartTest, "{'targetType':'CLIENT','targetWorkers':['C_A1_W1']}").Result;

            var operationContext = ReflectionUtil.ReadInstanceFieldValue<OperationContext>(clientWorker.operationProcessor, typeof(OperationProcessor), "operationContext");
            var testInstance = operationContext.Tests.Values.First().TestInstance as SimulatorTest;

            Assert.NotNull(testInstance);
            Assert.AreEqual(1, testInstance.InvokeCounts[TestPhase.Run]);

            Assert.AreEqual(rc.LastMessageId, response.MessageId);
            Assert.AreEqual(CoordinatorAddress, response.Destination);
            Assert.AreEqual(1, response.Size());
            AssertResponse(response, TestAddress);
        }

        [Test]
        public void TestStartNonMatchingWorkerTest()
        {
            Response response = rc.Send(CoordinatorAddress, TestAddress, OperationType.StartTest, "{'targetType':'CLIENT','targetWorkers':['C_A1_W2']}").Result;

            var operationContext = ReflectionUtil.ReadInstanceFieldValue<OperationContext>(clientWorker.operationProcessor, typeof(OperationProcessor), "operationContext");
            var testInstance = operationContext.Tests.Values.First().TestInstance as SimulatorTest;

            Assert.NotNull(testInstance);
            Assert.AreEqual(0, testInstance.InvokeCounts[TestPhase.Run]);

            Assert.AreEqual(rc.LastMessageId, response.MessageId);
            Assert.AreEqual(CoordinatorAddress, response.Destination);
            Assert.AreEqual(1, response.Size());
            AssertResponse(response, TestAddress);
        }

        [Test]
        public void TestStartFailingTest()
        {
            try
            {
                Response createResponse = rc.Send(CoordinatorAddress, WorkerAddress, OperationType.CreateTest,
                    "{'testIndex':2,'testId':'FailingSimulatorTest','properties':{'threadCount':'1','class':'Custom.Simulator.Name.FailingSimulatorTest'}}").Result;
                AssertResponse(createResponse, WorkerAddress);

                Response response = rc.Send(CoordinatorAddress, TestAddress2, OperationType.StartTest, START_CLIENT_PAYLOAD).Result;
                AssertResponse(response, TestAddress2);
                rc.WaitPhaseComplete(TestPhase.Run);

                string userDirectoryPath = GetUserDirectoryPath();
                string exceptionFile = Path.Combine(userDirectoryPath, "1.exception");
                string report = File.ReadAllText(exceptionFile);

                Assert.True(File.Exists(exceptionFile));
                Assert.IsNotEmpty(report);
                Assert.True(report.StartsWith("FailingSimulatorTest"));

                Assert.AreEqual(rc.LastMessageId, response.MessageId);
                Assert.AreEqual(CoordinatorAddress, response.Destination);
                Assert.AreEqual(1, response.Size());

            }
            finally
            {
                DeleteTest(2);
            }
            
        }
    }
}