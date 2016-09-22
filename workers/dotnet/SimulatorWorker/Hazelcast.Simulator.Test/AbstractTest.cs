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

using Hazelcast.Core;
using log4net;

namespace Hazelcast.Simulator.Test
{
    public abstract class AbstractTest
    {
        protected ILog Logger;

        public string name;

        protected IHazelcastInstance hzClient;

        protected ITestContext testContext;

        protected AbstractTest()
        {
            this.Logger = LogManager.GetLogger(this.GetType());
        }
    }
}