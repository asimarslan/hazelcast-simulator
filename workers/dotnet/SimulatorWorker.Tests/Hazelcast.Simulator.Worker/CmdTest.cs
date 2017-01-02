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

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Operations;
using NUnit.Framework;
using Properties;
using static Hazelcast.Simulator.Worker.BaseTestOperation;
using static Hazelcast.Simulator.Worker.RemoteConnector;

namespace Hazelcast.Simulator.Worker
{
    [TestFixture]
    public class CmdTest
    {
        private const string FILE_NAME = "\\SimulatorWorker.exe";

        private const string ARGS_BASE =
            @"publicAddress=127.0.0.1 agentIndex=1 workerType=dotnetclient workerId=C_A1_W1-127.0.0.1-dotnetclient workerIndex=1 workerPort=9002 workerPerformanceMonitorIntervalSeconds=0 autoCreateHzInstance=false hzConfigFile=hazelcast-client.xml workerHome=";

        private RemoteConnector rc;

        [OneTimeSetUp]
        public void Setup()
        {
            DirectoryInfo tmpFolder = TestEnvironmentUtils.SetupFakeUserDir();
            string[] args =
            {
                "workerType=dotnetclient", $"publicAddress={PUBLIC_ADDRESS}", "agentIndex=1", "workerIndex=1", "workerPort=9002",
                "autoCreateHzInstance=false", "workerPerformanceMonitorIntervalSeconds=0", "hzConfigFile=hazelcast-client.xml", $"workerHome={tmpFolder.FullName}",
                $" log4netConfig=\"{Resources.log4net}\""
            };

            Task.Run(() => ClientWorker.Main(args));
            Thread.Sleep(3000);
            rc = new RemoteConnector("127.0.0.1", 9002, WorkerAddress);
            rc.Start().Wait();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            rc.Send(CoordinatorAddress, WorkerAddress, OperationType.TerminateWorker, "{}");
            rc.Shutdown();
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
    }
}