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
using Hazelcast.Simulator.Utils;

namespace Hazelcast.Simulator.Test
{
    public enum TestPhase
    {
        [Value("setup", false, "SETUP")]
        Setup,

        [Value("local prepare", false, "LOCAL_PREPARE")]
        LocalPrepare,

        [Value("global prepare", true, "GLOBAL_PREPARE")]
        GlobalPrepare,

        [Value("warmup", false, "WARMUP")]
        Warmup,

        [Value("local after warmup", false, "LOCAL_AFTER_WARMUP")]
        LocalAfterWarmup,

        [Value("global after warmup", true, "GLOBAL_AFTER_WARMUP")]
        GlobalAfterWarmup,

        [Value("run", false, "RUN")]
        Run,

        [Value("global verify", true, "GLOBAL_VERIFY")]
        GlobalVerify,

        [Value("local verify", false, "LOCAL_VERIFY")]
        LocalVerify,

        [Value("global tear down", true, "GLOBAL_TEARDOWN")]
        GlobalTeardown,

        [Value("local tear down", false, "LOCAL_TEARDOWN")]
        LocalTeardown
    }

    public static class TestPhases
    {
        public static string GetDescription(this TestPhase testPhase)
        {
            var attr = ValueAttribute.GetAttr(testPhase);
            return attr.Value as string;
        }

        public static bool IsGlobal(this TestPhase testPhase)
        {
            var attr = ValueAttribute.GetAttr(testPhase);
            return attr.Value2 != null && (bool)attr.Value2;
        }

        public static string GetName(this TestPhase testPhase)
        {
            var attr = ValueAttribute.GetAttr(testPhase);
            return attr.Value3 as string;
        }

        public static TestPhase ToTestPhase(this string testPhaseStr)
        {
            foreach (TestPhase testPhase in Enum.GetValues(typeof(TestPhase)))
            {
                if (testPhaseStr == GetName(testPhase))
                {
                    return testPhase;
                }
            }
            throw new ArgumentException($"Argument {testPhaseStr} cannot be converted to TestPhase.");
        }

        public static TestPhase GetLastTestPhase()
        {
            Array values = Enum.GetValues(typeof(TestPhase));
            return (TestPhase)values.GetValue(values.GetUpperBound(0));
        }
    }
}