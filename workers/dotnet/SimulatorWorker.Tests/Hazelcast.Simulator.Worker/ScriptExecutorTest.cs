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

using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Operations;
using Newtonsoft.Json;
using NUnit.Framework;
using static Hazelcast.Simulator.Worker.RemoteConnector;

namespace Hazelcast.Simulator.Worker
{
    [TestFixture]
    public class ScriptExecutorTest : BaseTestOperation
    {
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