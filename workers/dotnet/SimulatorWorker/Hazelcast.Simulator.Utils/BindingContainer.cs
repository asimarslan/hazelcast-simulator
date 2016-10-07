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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using Hazelcast.Core;
using Hazelcast.Simulator.Metronome;
using Hazelcast.Simulator.Probe;
using Hazelcast.Simulator.Test;
using static Hazelcast.Simulator.Utils.DependencyInjectionUtil;
using static Hazelcast.Simulator.Utils.ReflectionUtil;

namespace Hazelcast.Simulator.Utils
{
    public class BindingContainer
    {
        private readonly ConcurrentDictionary<string, object> propertyScopeData = new ConcurrentDictionary<string, object>();
        private readonly ConcurrentDictionary<string, IProbe> probes = new ConcurrentDictionary<string, IProbe>();
//        private readonly ConcurrentDictionary<Type, object> typeScopeData = new ConcurrentDictionary<Type, object>();

        private readonly TestContext testContext;

        public BindingContainer(TestContext testContext, TestCase testCase)
        {
            //defaults
            this.testContext = testContext;
            //            this.typeScopeData.TryAdd(typeof(ITestContext), testContext);
            //            this.typeScopeData.TryAdd(typeof(IHazelcastInstance), testContext.TargetInstance);
            //
            //            foreach (KeyValuePair<string, string> pair in testCase.Properties)
            //            {
            //                this.propertyScopeData.TryAdd(pair.Key, pair.Value);
            //            }
        }

//        public void Bind(object testInstance)
//        {
//            MemberInfo[] memberInfos = testInstance.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance);
//            foreach (MemberInfo memberInfo in memberInfos)
//            {
//                if (memberInfo.MemberType != MemberTypes.Field || memberInfo.MemberType != MemberTypes.Property)
//                {
//                    continue;
//                }
//                if (memberInfo.IsDefined(typeof(InjectAttribute), true))
//                {
//                    var named = memberInfo.GetCustomAttribute<NamedAttribute>();
//                    string name = named != null ? named.Name : memberInfo.Name;
//                    object value;
//                    if (this.propertyScopeData.TryGetValue(name, out value))
//                    {
//                        SetValue(testInstance, memberInfo, value);
//                    }
//                    else
//                    {
//                        var type = GetFieldType(memberInfo);
//                        if (this.typeScopeData.TryGetValue(type, out value))
//                        {
//                            SetValue(testInstance, memberInfo, value);
//                        }
//                        else
//                        {
//                            value = Activator.CreateInstance(type);
//                        }
//                    }
//                }
//            }
//
//            //            Inject(testInstance, pair.Key, pair.Value);
//        }

        public void Bind(object testInstance)
        {
            MemberInfo[] memberInfos = testInstance.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance);
            foreach (MemberInfo memberInfo in memberInfos)
            {
                if (memberInfo.MemberType != MemberTypes.Field || memberInfo.MemberType != MemberTypes.Property)
                {
                    continue;
                }
                if (memberInfo.IsDefined(typeof(InjectAttribute), true))
                {
                    Inject11(testInstance, memberInfo);

                    //                    object value;
                    //                    var type = GetFieldType(memberInfo);
                    //                    if (this.typeScopeData.TryGetValue(type, out value))
                    //                    {
                    //                        SetValue(testInstance, memberInfo, value);
                    //                    }
                    //                    else
                    //                    {
                    //                        value = Activator.CreateInstance(type);
                    //                    }
                }
            }

            //            foreach (KeyValuePair<string, object> pair in this.propertyScopeData)
            //            {
            //                Inject(testInstance, pair.Key, pair.Value);
            //            }
        }

        private object Inject11(object testInstance, MemberInfo memberInfo)
        {
            Type type = GetFieldType(memberInfo);
            if (type == typeof(IHazelcastInstance))
            {
                SetValue(testInstance, memberInfo, this.testContext.TargetInstance);
            }
            else if (type == typeof(ITestContext))
            {
                SetValue(testInstance, memberInfo, this.testContext);
            }
            else if (type == typeof(IProbe))
            {
                IProbe probe = this.GetOrCreateProbe(GetProbeName(memberInfo), IsPartOfTotalThoughput(memberInfo));
                SetValue(testInstance, memberInfo, probe);
            }
            else if (type == typeof(IMetronome))
            {

            }
            return null;
        }

        private IProbe GetOrCreateProbe(string probeName, bool isPartOfTotalThoughput)
        {
            return this.probes.GetOrAdd(probeName, name => new HdrProbe(isPartOfTotalThoughput));
        }
    }
}