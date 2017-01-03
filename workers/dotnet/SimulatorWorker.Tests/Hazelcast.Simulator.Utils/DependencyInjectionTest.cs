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
using NUnit.Framework;
using static Hazelcast.Simulator.Utils.DependencyInjectionUtil;

namespace Hazelcast.Simulator.Utils
{
    [TestFixture]
    public class DependencyInjectionTest
    {
        [SetUp]
        public void Setup() {}

        [TearDown]
        public void TearDown() {}

        [Test]
        public void TestInjectPublicNonStaticField()
        {
            const string strVal = "Value0";
            var obj = new Dependent();
            InjectToPropertyPath(obj, "testStrField", strVal);
            Assert.AreEqual(obj.testStrField, strVal);
        }

        [Test]
        public void TestInjectPublicNonStaticFieldWithStringValue()
        {
            const string strVal = "90";
            var obj = new Dependent();
            InjectToPropertyPath(obj, "testLongField", strVal);
            Assert.AreEqual(obj.testLongField, long.Parse(strVal));
        }

        [Test]
        public void TestInjectPublicNonStaticProperty()
        {
            const string strVal = "90";
            var obj = new Dependent();
            InjectToPropertyPath(obj, "TestIntProperty", strVal);
            Assert.AreEqual(obj.TestIntProperty, int.Parse(strVal));
        }

        [Test]
        public void TestNotInjectPrivateNonStaticProperty()
        {
            const string strVal = "90";
            var obj = new Dependent();
            Assert.False(InjectToPropertyPath(obj, "privateField", strVal));
            Assert.AreEqual(obj.GetPrivateFieldValue(), 1);
        }

        [Test]
        public void TestNotInjectPublicStaticField()
        {
            const string strVal = "90";
            var obj = new Dependent();
            Assert.False(InjectToPropertyPath(obj, "staticField", strVal));
            Assert.AreEqual(Dependent.StaticField, 100);
        }

        [Test]
        public void TestInject_NullInstance()
        {
            const string strVal = "90";
            object obj = null;
            Assert.False(InjectToPropertyPath(obj, "testStrField", strVal));
        }

        [Test]
        public void TestInject_ToMethod()
        {
            const string strVal = "90";
            var obj = new Dependent();
            Assert.Throws<BindingException>(()=>InjectToPropertyPath(obj, "SomeMethod", strVal));
        }

        [Test]
        public void TestInjectProperties()
        {
            var dict = new Dictionary<string, string>();
            dict.Add("testStrField", "Value0");
            dict.Add("testLongField", "99");
            dict.Add("TestIntProperty", "90");
            var obj = new Dependent();
            InjectProperties(obj, dict);

            Assert.AreEqual(obj.testStrField, dict["testStrField"]);
            Assert.AreEqual(obj.testLongField, long.Parse(dict["testLongField"]));
            Assert.AreEqual(obj.TestIntProperty, int.Parse(dict["TestIntProperty"]));
        }

        [Test]
        public void TestInjectChildLongField()
        {
            const string strVal = "1000";
            var obj = new Dependent();
            InjectToPropertyPath(obj, "child.childLongField", strVal);
            Assert.AreEqual(obj.child.ChildLongField, long.Parse(strVal));
        }

        [Test]
        public void TestInjectNonExistChildField()
        {
            const string strVal = "1000";
            var obj = new Dependent
            {
                child = new Child()
            };
            
            Assert.Throws<BindingException>(() => InjectToPropertyPath(obj, "child.nonExistGrandChild.childLongField", strVal));
        }

        [Test]
        public void TestInjectNonExistChild()
        {
            const string strVal = "1000";
            var obj = new Dependent
            {
                child = new Child()
            };
            
            Assert.False(InjectToPropertyPath(obj, "NoSuchChild.childLongField", strVal));
        }

        [Test]
        public void TestInjectGrandChildLongField()
        {
            const string strVal = "1000";
            var obj = new Dependent();
            InjectToPropertyPath(obj, "child.grandChild.childLongField", strVal);
            Assert.AreEqual(obj.child.child.ChildLongField, long.Parse(strVal));
        }

        [Test]
        public void TestInjectGrandChildLongFieldMultiField()
        {
            const string strVal = "1000";
            var obj = new Dependent();
            InjectToPropertyPath(obj, "child.childLongField", strVal);
            InjectToPropertyPath(obj, "child.grandChild.childLongField", strVal);
            Assert.AreEqual(obj.child.ChildLongField, long.Parse(strVal));
            Assert.AreEqual(obj.child.child.ChildLongField, long.Parse(strVal));
        }
    }
}