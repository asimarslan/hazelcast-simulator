using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Operations;
using NUnit.Framework;

using static Hazelcast.Simulator.Worker.ClientWorkerTest;
using static Hazelcast.Simulator.Worker.RemoteConnector;

namespace Hazelcast.Simulator.Worker
{
    [TestFixture]
    public class CmdTest
    {
        private const string FILE_NAME = "\\SimulatorWorker.exe"; 
        private const string ARGS_BASE = 
            @"publicAddress=127.0.0.1 agentIndex=1 workerType=dotnetclient workerId=C_A1_W1-127.0.0.1-dotnetclient workerIndex=1 workerPort=9002 workerPerformanceMonitorIntervalSeconds=0 autoCreateHzInstance=false hzConfigFile=hazelcast-client.xml workerHome=";

//        private Process workerProcess;
        private RemoteConnector rc;


        [SetUp]
        public void  Setup()
        {
            DirectoryInfo tmpFolder = TestEnvironmentUtils.SetupFakeUserDir();
            string[] args = { "workerType=dotnetclient", $"publicAddress={PUBLIC_ADDRESS}", "agentIndex=1", "workerIndex=1", "workerPort=9002",
                "autoCreateHzInstance=false", "workerPerformanceMonitorIntervalSeconds=0", "hzConfigFile=hazelcast-client.xml", $"workerHome={tmpFolder.FullName}" ,
                $" log4netConfig=\"{Properties.Resources.log4net}\""};

            Task.Run(()=>ClientWorker.Main(args));
            Thread.Sleep(3000);
            rc = new RemoteConnector("127.0.0.1", 9002, WorkerAddress);
            rc.Start().Wait();
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
