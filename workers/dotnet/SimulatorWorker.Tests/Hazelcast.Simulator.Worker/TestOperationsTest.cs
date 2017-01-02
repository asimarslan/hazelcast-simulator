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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Operations;
using Hazelcast.Simulator.Protocol.Processors;
using Hazelcast.Simulator.Test;
using Hazelcast.Simulator.Utils;
using log4net.Config;
using Newtonsoft.Json;
using NUnit.Framework;

using static Hazelcast.Simulator.Worker.RemoteConnector;

namespace Hazelcast.Simulator.Worker
{
    [TestFixture]
    public class TestOperationsTest : BaseTestOperation
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
        public void TestStartStopTest()
        {
            Response startResponse = rc.Send(CoordinatorAddress, TestAddress, OperationType.StartTest, "{ 'targetType':'CLIENT','targetWorkers':[]}").Result;
            Response stopResponse = rc.Send(CoordinatorAddress, TestAddress, OperationType.StopTest, "{}").Result;

            var operationContext = ReflectionUtil.ReadInstanceFieldValue<OperationContext>(clientWorker.operationProcessor, typeof(OperationProcessor), "operationContext");
            var testInstance = operationContext.Tests.Values.First().TestInstance as SimulatorTest;

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
            Response startResponse = rc.Send(CoordinatorAddress, TestAddress, OperationType.StartTest, "{ 'targetType':'MEMBER','targetWorkers':[]}").Result;
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
        }

        [Test]
        public void TestStartTestTwice()
        {
            Response createResponse = rc.Send(CoordinatorAddress, WorkerAddress, OperationType.CreateTest,
                @"{'testIndex':1,'testId':'SimulatorTest','properties':{'threadCount':'1','class':'Custom.Simulator.Name.SimulatorTest'}}").Result;

            AssertResponse(createResponse, WorkerAddress);

            rc.Send(CoordinatorAddress, TestAddress, OperationType.StartTest, "{ 'targetType':'CLIENT','targetWorkers':[]}");

            Response startResponse2 = rc.Send(CoordinatorAddress, TestAddress, OperationType.StartTest, "{ 'targetType':'CLIENT','targetWorkers':[]}").Result;
            AssertResponse(startResponse2, TestAddress);

            //delete test
            Response finalResponse = rc.Send(CoordinatorAddress, TestAddress, OperationType.StartTestPhase, "{'testPhase':'LOCAL_TEARDOWN'}").Result;
            rc.WaitPhaseComplete(TestPhase.LocalTeardown);
            AssertResponse(finalResponse, TestAddress);

        }
//
//        [Test]
//        public void TestStartWorkerTwice()
//        {
//            clientWorker.Start().ContinueWith(t =>
//            {
//                Assert.True(t.IsFaulted);
//                Assert.AreEqual(t.Exception?.Flatten().InnerExceptions.First().GetType(), typeof(SimulatorProtocolException));
//            }).Wait();
//        }
//
//        [Test]
//        public void TestPublicIpAddress()
//        {
//            Assert.AreEqual(clientWorker.PublicIpAddress, PUBLIC_ADDRESS);
//            Assert.AreEqual(clientWorker.Connector.PublicIpAddress, PUBLIC_ADDRESS);
//        }
//
//
//        [Test]
//        public void TestLogOperation()
//        {
//            Response logResponse= rc.Send(CoordinatorAddress, WorkerAddress, OperationType.Log, "{'message':'Test Log message','level':'INFO'}").Result;
//
//            AssertResponse(logResponse, WorkerAddress);
//
//            Assert.AreEqual(rc.LastMessageId, logResponse.MessageId);
//            Assert.AreEqual(CoordinatorAddress, logResponse.Destination);
//            Assert.AreEqual(1, logResponse.Size());
//            AssertResponse(logResponse, WorkerAddress);
//
//        }


    }
}