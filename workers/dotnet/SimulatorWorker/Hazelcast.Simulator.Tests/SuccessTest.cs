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
using System.Threading;
using Hazelcast.Simulator.Test;

namespace Hazelcast.Simulator.Tests
{
    public class SuccessTest
    {
        private ITestContext context;

        [Setup]
        public void Setup(ITestContext testContext) => this.context = testContext;

        [Teardown(Global = false)]
        public void LocalTearDown() {}

        [Teardown(Global = true)]
        public void GlobalTearDown() {}

        [Prepare(Global = false)]
        public void LocalPrepare() => Thread.Sleep(1);

        [Prepare(Global = true)]
        public void GlobalPrepare() => Thread.Sleep(1);

        [Verify(Global = false)]
        public void LocalVerify() {}

        [Verify(Global = true)]
        public void GlobalVerify() {}

        [Run]
        public void Run()
        {
            while (!this.context.IsStopped())
            {
                Thread.Sleep(1);
                Console.WriteLine("testSuccess running");
            }
            Console.WriteLine("testSuccess completed");
        }

    }
}