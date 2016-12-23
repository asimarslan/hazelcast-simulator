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
using System.Threading.Tasks;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Processors;
using Hazelcast.Simulator.Test;
using log4net;
using Newtonsoft.Json;

namespace Hazelcast.Simulator.Protocol.Operations
{
    /// <summary>
    ///     Creates a Simulator Test based on an index, a testId and a property map.
    //</summary>
    public class CreateTestOperation : AbstractWorkerOperation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CreateTestOperation));

        [JsonProperty("testIndex")]
        private readonly int testIndex;

        [JsonProperty("testId")]
        private readonly string testId;

        [JsonProperty("properties")]
        private readonly IDictionary<string, string> properties;

        public CreateTestOperation() {}

        public CreateTestOperation(int testIndex, TestCase testCase)
        {
            this.testIndex = testIndex;
            testId = testCase.TestId;
            properties = testCase.Properties;
        }

        public override async Task<ResponseType> RunInternal(OperationContext ctx, SimulatorAddress targetAddress)
        {
            var testCase = new TestCase(testId, properties);
            Logger.Info($"Initializing test {testCase}");

            var testContext = new TestContext(testId, ctx.HazelcastInstance, ctx.Connector);
            SimulatorAddress testAddress = ctx.WorkerAddress.GetChild(testIndex);
            var testContainer = new TestContainer(testContext, testCase, testAddress);
            if (!ctx.Tests.TryAdd(testIndex, testContainer))
            {
                throw new InvalidOperationException(
                    $"Can't init {testCase}, another test with testId:{testId} already exists");
            }
            return ResponseType.Success;
        }
    }
}