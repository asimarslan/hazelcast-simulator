﻿// Copyright (c) 2008-2016, Hazelcast, Inc. All Rights Reserved.
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
using System.Linq;
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
        private readonly ISet<String> unusedProperties = new HashSet<string>();

        private readonly TestContext testContext;
        private readonly TestCase testCase;
        private readonly MetronomeFactory metronomeFactory;

        public BindingContainer(TestContext testContext, TestCase testCase)
        {
            //defaults
            this.testContext = testContext;
            this.testCase = testCase;
            this.metronomeFactory = new MetronomeFactory(testCase.Properties);
            this.testCase.Properties.Keys.All(key => this.unusedProperties.Add(key));
        }

        public void EnsureNoUnusedProperties()
        {
            if (this.unusedProperties.Count > 0)
            {
                throw new BindingException($"Some of the properities({string.Join(", ",this.unusedProperties)}) are not used on {this.testCase.GetClassname()}");
            }
        }

        public void Bind(object testInstance)
        {
            MemberInfo[] memberInfos = testInstance.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance);
            foreach (MemberInfo memberInfo in memberInfos)
            {
                if (memberInfo.MemberType != MemberTypes.Field && memberInfo.MemberType != MemberTypes.Property)
                {
                    continue;
                }
                if (memberInfo.IsDefined(typeof(InjectAttribute), true))
                {
                    this.Inject(testInstance, memberInfo);
                }
            }

            ISet<string> injectProperties = InjectProperties(testInstance, this.testCase.Properties);
            injectProperties.All(key => this.unusedProperties.Remove(key));
        }

        private void Inject(object testInstance, MemberInfo memberInfo)
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
                IMetronome metronome = this.metronomeFactory.CreateMetronome();
                SetValue(testInstance, memberInfo, metronome);
            }
        }

        private IProbe GetOrCreateProbe(string probeName, bool isPartOfTotalThoughput)
        {
            return this.probes.GetOrAdd(probeName, name => new HdrProbe(isPartOfTotalThoughput));
        }

        public ConcurrentDictionary<string, IProbe> GetProbes() => this.probes;

    }
}