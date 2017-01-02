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
    public class BaseTestOperation
    {
        public static readonly SimulatorAddress TestAddress = new SimulatorAddress(AddressLevel.TEST, 1, 1, 1);
        public static readonly SimulatorAddress WorkerAddress = new SimulatorAddress(AddressLevel.WORKER, 1, 1, 0);
        public static readonly SimulatorAddress CoordinatorAddress = new SimulatorAddress(AddressLevel.COORDINATOR, 0, 0, 0);
        public const string PUBLIC_ADDRESS = "127.0.0.1:5701";

        protected ClientWorker clientWorker;
        protected RemoteConnector rc;

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

        [Setup]
        public void Setup()
        {
            Response createResponse = rc.Send(CoordinatorAddress, WorkerAddress, OperationType.CreateTest,
                @"{'testIndex':1,'testId':'SimulatorTest','properties':{'threadCount':'1','class':'Custom.Simulator.Name.SimulatorTest'}}").Result;
            AssertResponse(createResponse, WorkerAddress);
        }

        [TearDown]
        public void TearDown()
        {
            //delete test
            Response finalResponse = rc.Send(CoordinatorAddress, TestAddress, OperationType.StartTestPhase, "{'testPhase':'LOCAL_TEARDOWN'}").Result;
            rc.WaitPhaseComplete(TestPhase.LocalTeardown);
            AssertResponse(finalResponse, TestAddress);
        }
    }
}