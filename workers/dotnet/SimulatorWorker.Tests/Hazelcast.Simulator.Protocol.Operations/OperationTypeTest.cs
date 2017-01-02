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

using NUnit.Framework;

namespace Hazelcast.Simulator.Protocol.Operations
{
    [TestFixture]
    public class OperationTypeTest
    {
        [SetUp]
        public void Setup() {}

        [TearDown]
        public void TearDown() {}

        [Test]
        public void TestClassType()
        {
            Assert.AreEqual(typeof(IntegrationTestOperation), OperationType.IntegerationTest.GetClassType());
            Assert.AreEqual(typeof(LogOperation), OperationType.Log.GetClassType());
            Assert.AreEqual(typeof(PingOperation), OperationType.Ping.GetClassType());
            Assert.AreEqual(typeof(CreateTestOperation), OperationType.CreateTest.GetClassType());
            Assert.AreEqual(typeof(ExecuteScriptOperation), OperationType.ExecuteScript.GetClassType());
            Assert.AreEqual(typeof(StartTestPhaseOperation), OperationType.StartTestPhase.GetClassType());
            Assert.AreEqual(typeof(StartTestOperation), OperationType.StartTest.GetClassType());
            Assert.AreEqual(typeof(StopTestOperation), OperationType.StopTest.GetClassType());
        }

        [Test]
        public void TestImplicitConvert()
        {
            Assert.AreEqual(0, (int)OperationType.IntegerationTest);
            Assert.AreEqual(1, (int)OperationType.Auth);
            Assert.AreEqual(2, (int)OperationType.Log);
            Assert.AreEqual(4000, (int)OperationType.Ping);
            Assert.AreEqual(4001, (int)OperationType.TerminateWorker);
            Assert.AreEqual(4002, (int)OperationType.CreateTest);
            Assert.AreEqual(5001, (int)OperationType.StartTest);
            Assert.AreEqual(5000, (int)OperationType.StartTestPhase);
            Assert.AreEqual(5002, (int)OperationType.StopTest);
        }
    }
}