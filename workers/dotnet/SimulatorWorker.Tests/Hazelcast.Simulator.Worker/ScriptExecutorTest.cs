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
    public class ScriptExecutorTest
    {
        public static readonly SimulatorAddress WorkerAddress = new SimulatorAddress(AddressLevel.WORKER, 1, 1, 0);
        public static readonly SimulatorAddress CoordinatorAddress = new SimulatorAddress(AddressLevel.COORDINATOR, 0, 0, 0);
        public const string PUBLIC_ADDRESS = "127.0.0.1:5701";

        private ClientWorker clientWorker;
        private RemoteConnector rc;

        [OneTimeSetUp]
        public void Init()
        {
            BasicConfigurator.Configure();

            DirectoryInfo tmpFolder = TestEnvironmentUtils.SetupFakeUserDir();
            Environment.SetEnvironmentVariable("WORKER_HOME", tmpFolder.FullName);
            var workerParams = new Dictionary<string, string>
            {
                { "log4netConfig", Properties.Resources.log4net }
            };

            ClientWorker.InitLog(workerParams);

            clientWorker = new ClientWorker("dotnetclient", PUBLIC_ADDRESS, 1, 1, 9002, null, false, 0);
            clientWorker.Start();
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
        public void TestExecuteJSScript_Success()
        {
            var op = new ExecuteScriptOperation("js:System.Console.WriteLine(\"console log from js\");", false);
            string json = JsonConvert.SerializeObject(op);

            Response response = rc.Send(CoordinatorAddress, WorkerAddress, OperationType.ExecuteScript, json).Result;

            Assert.AreEqual(rc.LastMessageId, response.MessageId);
            Assert.AreEqual(CoordinatorAddress, response.Destination);
            Assert.AreEqual(1, response.Size());
            AssertResponse(response, WorkerAddress);

        }

        [Test]
        public void TestExecuteJSScript_Success_FireAndForget()
        {
            var op = new ExecuteScriptOperation("js:System.Console.WriteLine(\"console log from js\");", true);
            string json = JsonConvert.SerializeObject(op);

            Response response = rc.Send(CoordinatorAddress, WorkerAddress, OperationType.ExecuteScript, json).Result;

            Assert.AreEqual(rc.LastMessageId, response.MessageId);
            Assert.AreEqual(CoordinatorAddress, response.Destination);
            Assert.AreEqual(1, response.Size());
            AssertResponse(response, WorkerAddress);

        }

        [Test]
        public void TestExecuteJSScript_InvalidScript()
        {
            var op = new ExecuteScriptOperation("js:MustFail!!!Script", false);
            string json = JsonConvert.SerializeObject(op);

            Response response = rc.Send(CoordinatorAddress, WorkerAddress, OperationType.ExecuteScript, json).Result;

            Assert.AreEqual(rc.LastMessageId, response.MessageId);
            Assert.AreEqual(CoordinatorAddress, response.Destination);
            Assert.AreEqual(1, response.Size());
            AssertResponse(response, WorkerAddress, ResponseType.ExceptionDuringOperationExecution);

        }

        [Test]
        public void TestExecuteJSScript_InvalidScript_FireAndForget()
        {
            var op = new ExecuteScriptOperation("js:MustFail!!!Script", true);
            string json = JsonConvert.SerializeObject(op);

            Response response = rc.Send(CoordinatorAddress, WorkerAddress, OperationType.ExecuteScript, json).Result;

            Assert.AreEqual(rc.LastMessageId, response.MessageId);
            Assert.AreEqual(CoordinatorAddress, response.Destination);
            Assert.AreEqual(1, response.Size());
            AssertResponse(response, WorkerAddress);

        }


    }
}