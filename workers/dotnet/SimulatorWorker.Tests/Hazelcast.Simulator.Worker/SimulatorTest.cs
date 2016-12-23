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
using Hazelcast.Simulator.Test;

namespace Hazelcast.Simulator.Worker
{
    [Named("Custom.Simulator.Name.SimulatorTest")]
    public class SimulatorTest
    {
        [Inject]
        public ITestContext Context;

        public IDictionary<TestPhase, int> InvokeCounts;

        public SimulatorTest()
        {
            InvokeCounts = new Dictionary<TestPhase, int>();
            foreach (TestPhase testPhase in Enum.GetValues(typeof(TestPhase)))
            {
                InvokeCounts.Add(testPhase, 0);
            }
        }

        [Setup]
        public void Setup1() => IncrementPhaseInvokeCount(TestPhase.Setup);

        [Setup]
        public void Setup2() => IncrementPhaseInvokeCount(TestPhase.Setup);

        [Teardown]
        public void LocalTearDown() => IncrementPhaseInvokeCount(TestPhase.LocalTeardown);

        [Teardown(true)]
        public void GlobalTearDown() => IncrementPhaseInvokeCount(TestPhase.GlobalTeardown);

        [Prepare]
        public void LocalPrepare() => IncrementPhaseInvokeCount(TestPhase.LocalPrepare);

        [Prepare(true)]
        public void GlobalPrepare() => IncrementPhaseInvokeCount(TestPhase.GlobalPrepare);

        [AfterWarmup]
        public void LocalAfterWarmup() => IncrementPhaseInvokeCount(TestPhase.LocalAfterWarmup);

        [AfterWarmup(true)]
        public void GlobalAfterWarmup() => IncrementPhaseInvokeCount(TestPhase.GlobalAfterWarmup);

        [Verify]
        public void LocalVerify() => IncrementPhaseInvokeCount(TestPhase.LocalVerify);

        [Verify(true)]
        public void GlobalVerify() => IncrementPhaseInvokeCount(TestPhase.GlobalVerify);

        [Run]
        public void Run() => IncrementPhaseInvokeCount(TestPhase.Run);

        private void IncrementPhaseInvokeCount(TestPhase testPhase) => InvokeCounts[testPhase] += 1;
    }
}