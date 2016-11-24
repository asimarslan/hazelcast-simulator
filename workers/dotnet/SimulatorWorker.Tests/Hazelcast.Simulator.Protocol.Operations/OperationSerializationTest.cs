// Copyright (c) 2008-2016, Hazelcast, Inc. All Rights Reserved.
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
using Hazelcast.Simulator.Test;
using Newtonsoft.Json;
using NUnit.Framework;
using static Hazelcast.Simulator.Utils.ReflectionUtil;

namespace Hazelcast.Simulator.Protocol.Operations
{
    [TestFixture]
    public class OperationSerializationTest
    {
        [Test]
        public void TestCreateTestOperation()
        {
            var properties = new Dictionary<string, string>();
            properties.Add("key0", "value0");
            properties.Add("key1", "value1");
            var op = new CreateTestOperation(99, new TestCase("testIdx", properties));
            string json = JsonConvert.SerializeObject(op);
            var operationType = OperationType.CreateTest;
            Type type = operationType.GetClassType();
            var sop = (CreateTestOperation)JsonConvert.DeserializeObject(json, type);

            Assert.AreEqual(ReadInstanceFieldValue<int>(sop, type, "testIndex"), 99);
            Assert.AreEqual(ReadInstanceFieldValue<string>(sop, type, "testId"), "testIdx");
            Assert.AreEqual(ReadInstanceFieldValue<IDictionary<string,string>>(sop, type, "properties"), properties);
        }
    }

}