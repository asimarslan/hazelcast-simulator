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

using Hazelcast.Simulator.Test;
using NUnit.Framework;
using static Hazelcast.Simulator.Utils.DependencyInjectionUtil;

namespace Hazelcast.Simulator.Utils
{
    [TestFixture]
    public class DependencyInjectionTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [TearDown]
        public void TearDown() { }

        [Test]
        public void TestInjectPublicNonStaticField()
        {
            const string strVal = "Value0";
            var obj = new Dependent();
            Inject(obj,"testStrField", strVal);
            Assert.AreEqual(obj.testStrField, strVal );
        }

        [Test]
        public void TestInjectPublicNonStaticProperty()
        {
            const int intVal = 99;
            var obj = new Dependent();
            Inject(obj,"TestIntProperty", intVal);
            Assert.AreEqual(obj.TestIntProperty, intVal );
        }

        [Test]
        public void TestNotInjectPrivateNonStaticProperty()
        {
            const int intVal = 99;
            var obj = new Dependent();
            Assert.False(Inject(obj,"privateField", intVal));
            Assert.AreEqual(obj.GetPrivateFieldValue(), 1 );
        }

        [Test]
        public void TestNotInjectPublicStaticField()
        {
            const int intVal = 99;
            var obj = new Dependent();
            Assert.False(Inject(obj,"staticField", intVal));
            Assert.AreEqual(Dependent.StaticField, 100 );
        }


    }

    public class Dependent
    {
        [Inject(Property = "testStrField")]
        public string testStrField;

        [Inject(Property = "TestIntProperty")]
        public int TestIntProperty { get; set; }

        [Inject(Property = "privateField")]
        private long privateField=1;

        [Inject(Property = "staticField")]
        public static long StaticField=100;

        public long GetPrivateFieldValue() => this.privateField;
    }
}