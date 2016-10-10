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
        private long privateField=1;

        [Inject, Named("staticField")]
        public static long StaticField=100;

        [Named("child")]
        public Child child;

        [Inject]
        public IHazelcastInstance hazelcastClient;

        [Inject]
        public TestContext testContext;

        [InjectProbe(true), Named("probe-1")]
        public IProbe probe1;

        [InjectProbe]
        public IProbe probe2;

        [InjectProbe, Named("probe-1")]
        public IProbe probe3;

        public long GetPrivateFieldValue() => this.privateField;
    }

    public class Child
    {
        [Inject, Named("childLongField")]
        public long ChildLongField;

        [Named("grandChild")]
        public Child child;

    }
}