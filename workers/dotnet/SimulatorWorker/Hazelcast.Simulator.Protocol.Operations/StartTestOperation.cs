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

using System.Collections.Generic;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Processors;
using Hazelcast.Simulator.Test;
using log4net;
using Newtonsoft.Json;

namespace Hazelcast.Simulator.Protocol.Operations
{
    /// <summary>
    ///     Starts the <see cref="TestPhase.Run" /> phase of a Simulator Test.
    /// </summary>
    public class StartTestOperation : AbstractStartOperation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CreateTestOperation));

        [JsonProperty("targetType")]
        private readonly string targetType;

        [JsonProperty("targetWorkers")]
        private readonly List<string> targetWorkers;

        [JsonProperty("warmup")]
        private readonly bool warmup;

        protected override void StartPhase(OperationContext operationContext, TestContainer testContainer)
        {
            TestPhase testPhase = GetTestPhase();
            if (SkipRunPhase(testContainer))
            {
                Logger.Info($"Skipping test {testContainer.TestCase.TestId}");
                SendPhaseCompletedOperation(operationContext.Connector, testPhase, testContainer.TestAddress);
            }
            else
            {
                Logger.Info($"Starting test {testContainer.TestCase.TestId}");
                testContainer.Invoke(testPhase);
            }
        }

        protected override TestPhase GetTestPhase() => warmup ? TestPhase.Warmup : TestPhase.Run;

        private bool SkipRunPhase(TestContainer testContainer)
        {
            if (!MatchesTargetType())
            {
                Logger.Info($"Skipping test ({targetType} Worker does not match .Net Client) {testContainer.TestCase.TestId}");
                return true;
            }
            if (!MatchesTargetWorkers(testContainer.TestAddress.GetParent()))
            {
                Logger.Info($"Skipping test (Worker is not on target list) {testContainer.TestCase.TestId}");
                return true;
            }
            return false;
        }

        private bool MatchesTargetWorkers(SimulatorAddress workerAddress)
            => targetWorkers.Count == 0 || targetWorkers.Contains(workerAddress.ToString());

        private bool MatchesTargetType() => targetType == "ALL" || targetType == "CLIENT";
    }
}