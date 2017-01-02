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
using System.Linq;
using NUnit.Framework;

namespace Hazelcast.Simulator.Protocol.Operations
{
    [TestFixture]
    public class OperationRunTest
    {
        [SetUp]
        public void Setup() {}

        [TearDown]
        public void TearDown() {}

        [Test]
        public void TestIntegrationTestOperation()
        {
            var op = new IntegrationTestOperation();
            Assert.Throws<NotImplementedException>(() => { op.Run(null).Wait(); });
        }

        [Test]
        public void TestPhaseCompletedOperation()
        {
            var op = new PhaseCompletedOperation();

            Assert.Throws<NotImplementedException>(() =>
            {
                try
                {
                    op.Run(null, null).Wait();
                }
                catch (AggregateException ag)
                {
                    throw ag.Flatten().InnerExceptions.First();
                }
            });
        }
    }
}