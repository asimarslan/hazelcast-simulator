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
using Hazelcast.Core;
using Hazelcast.Simulator.Probe;
using Hazelcast.Simulator.Test;

namespace Hazelcast.Simulator.Utils
{
    public class Dependent
    {
        [Inject, Named("testStrField")]
        public string testStrField;

        [Inject, Named("testLongField")]
        public long testLongField;

        [Inject, Named("TestIntProperty")]
        public int TestIntProperty { get; set; }

        [Inject, Named("privateField")]
        private readonly long privateField = 1;

        [Inject, Named("staticField")]
        public static long StaticField = 100;

        [Named("child")]
        public Child child;

        [Inject]
        public IHazelcastInstance hazelcastClient;

        [Inject]
        public ITestContext testContext;

        [InjectProbe(true), Named("probe-1")]
        public IProbe probe1;

        [InjectProbe]
        public IProbe probe2;

        [InjectProbe, Named("probe-1")]
        public IProbe probe3;

        public long GetPrivateFieldValue() => privateField;

        public string SomeMethod() => null;
    }

    public class Child
    {
        [Inject, Named("childLongField")]
        public long ChildLongField;

        [Named("grandChild")]
        public Child child;
    }

    public class TestSample
    {
        public IDictionary<TestPhase, int> invokeCounts;

        public TestSample()
        {
            invokeCounts = new Dictionary<TestPhase, int>();
            foreach (TestPhase testPhase in Enum.GetValues(typeof(TestPhase)))
            {
                invokeCounts.Add(testPhase, 0);
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

        private void IncrementPhaseInvokeCount(TestPhase testPhase) => invokeCounts[testPhase] += 1;
    }
}