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

using Hazelcast.Simulator.Utils;

namespace Hazelcast.Simulator.Test
{
    public enum TestPhase
    {
        [Value("setup", false)]
        Setup,

        [Value("local prepare", false)]
        LocalPrepare,

        [Value("global prepare", true)]
        GlobalPrepare,

        [Value("warmup", false)]
        Warmup,

        [Value("local after warmup", false)]
        LocalAfterWarmup,

        [Value("global after warmup", true)]
        GlobalAfterWarmup,

        [Value("run", false)]
        Run,

        [Value("global verify", true)]
        GlobalVerify,

        [Value("local verify", false)]
        LocalVerify,

        [Value("global tear down", true)]
        GlobalTeardown,

        [Value("local tear down", false)]
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
    }
}