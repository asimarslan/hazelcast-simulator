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

using Hazelcast.Simulator.Protocol.Processors;
using Hazelcast.Simulator.Test;
using Newtonsoft.Json;

namespace Hazelcast.Simulator.Protocol.Operations
{
    /// <summary>
    ///     Starts a <see cref="TestPhase" /> of the addressed Simulator Test.
    /// </summary>
    public class StartTestPhaseOperation : AbstractStartOperation
    {
        [JsonProperty("testPhase")]
        private readonly string testPhaseStr;

        public StartTestPhaseOperation() {}

        public StartTestPhaseOperation(TestPhase testPhase)
        {
            testPhaseStr = testPhase.GetName();
        }

        protected override void StartPhase(OperationContext operationContext, TestContainer testContainer)
        {
            TestPhase testPhase = GetTestPhase();
            Logger.Info($"Starting  phase{testPhase.GetDescription()} for test:{testContainer.TestCase.TestId}");
            testContainer.Invoke(testPhase);
        }

        protected override TestPhase GetTestPhase() => testPhaseStr.ToTestPhase();
    }
}