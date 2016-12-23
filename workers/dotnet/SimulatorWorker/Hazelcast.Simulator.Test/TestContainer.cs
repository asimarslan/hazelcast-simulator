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
using System.Reflection;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Utils;
using static Hazelcast.Simulator.Utils.ReflectionUtil;

namespace Hazelcast.Simulator.Test
{
    /**
     * Container for test instances.
     *
     * It is responsible for:
     * <ul>
     * <li>Creates the test class instance by its fully qualified class name.</li>
     * <li>Binding properties to the test class instance (test parameters).</li>
     * <li>Injects required objects to annotated fields.</li>
     * <li>Analyses the test class instance for annotated test phase methods.</li>
     * <li>Provides a method to invoke test methods.</li>
     * </ul>
     */

    public class TestContainer
    {
        private readonly BindingContainer bindingContainer;
        private TestPhase currentPhase;
        private readonly AtomicBoolean running = new AtomicBoolean(false);
        private readonly IDictionary<TestPhase, Action> phaseDelegates = new Dictionary<TestPhase, Action>();

        public TestContext TestContext { get; }

        public TestCase TestCase { get; }

        public object TestInstance { get; }

        public SimulatorAddress TestAddress { get; }

        public TestContainer(TestContext testContext, TestCase testCase, SimulatorAddress testAddress, object testInstance = null)
        {
            TestContext = testContext;
            TestCase = testCase;
            TestAddress = testAddress;

            Type testClass = SearchNamedType(testCase.GetClassname());
            TestInstance = testInstance ?? Activator.CreateInstance(testClass);
            bindingContainer = new BindingContainer(testContext, testCase);

            bindingContainer.Bind(TestInstance);

            RegisterPhaseDelegates();
            //this.testPerformanceTracker = new TestPerformanceTracker(this);
        }

        public void Invoke(TestPhase testPhase)
        {
            if (!running.CompareAndSet(false, true))
            {
                throw new InvalidOperationException($"Test:{TestContext.GetTestId()} is still running phase:{currentPhase}");
            }
            try
            {
                InvokeInternal(testPhase);
            }
            finally
            {
                running.GetAndSet(false);
            }
        }

        private void InvokeInternal(TestPhase testPhase)
        {
            currentPhase = testPhase;

            if (testPhase == TestPhase.Warmup)
            {
                TestContext.BeforeWarmup();
            }
            else if (testPhase == TestPhase.LocalAfterWarmup)
            {
                TestContext.AfterWarmup();
            }

            Action phaseDelegate = phaseDelegates[testPhase];
            phaseDelegate?.Invoke();
        }

        private void RegisterPhaseDelegates()
        {
            Func<Attribute, bool> emptyFilter = attr => true;
            RegisterPhase(TestPhase.Setup, typeof(SetupAttribute), emptyFilter);

            RegisterPhase(TestPhase.LocalPrepare, typeof(PrepareAttribute), attr => !(attr as PrepareAttribute).Global);
            RegisterPhase(TestPhase.GlobalPrepare, typeof(PrepareAttribute), attr => (attr as PrepareAttribute).Global);

            //TODO WARMUP :we need this step in TimeStepRunner !!!
            phaseDelegates.Add(TestPhase.Warmup, () =>
            {
                /*ignore*/
            });

            RegisterPhase(TestPhase.LocalAfterWarmup, typeof(AfterWarmupAttribute), attr => !(attr as AfterWarmupAttribute).Global);
            RegisterPhase(TestPhase.GlobalAfterWarmup, typeof(AfterWarmupAttribute), attr => (attr as AfterWarmupAttribute).Global);
            //RUN
            RegisterPhase(TestPhase.Run, typeof(RunAttribute), emptyFilter);

            RegisterPhase(TestPhase.LocalVerify, typeof(VerifyAttribute), attr => !(attr as VerifyAttribute).Global);
            RegisterPhase(TestPhase.GlobalVerify, typeof(VerifyAttribute), attr => (attr as VerifyAttribute).Global);

            RegisterPhase(TestPhase.LocalTeardown, typeof(TeardownAttribute), attr => !(attr as TeardownAttribute).Global);
            RegisterPhase(TestPhase.GlobalTeardown, typeof(TeardownAttribute), attr => (attr as TeardownAttribute).Global);
        }

        private void RegisterPhase(TestPhase testPhase, Type attributeType, Func<Attribute, bool> filter, Action action = null)
        {
            IEnumerable<MemberInfo> methodInfos = GetMethodsWithAttribute(TestInstance.GetType(), attributeType);
            var methods = new List<MethodInfo>();
            foreach (MethodInfo methodInfo in methodInfos)
            {
                if (methodInfo.GetParameters().Length > 0)
                {
                    throw new IllegalTestException($"TestPhase:{testPhase} method:{methodInfo.Name} cannot have parameters!");
                }
                Attribute customAttribute = methodInfo.GetCustomAttribute(attributeType);
                if (filter(customAttribute))
                {
                    methods.Add(methodInfo);
                }
            }
            action = action ?? delegate
            {
                foreach (MethodInfo methodInfo in methods)
                {
                    methodInfo.Invoke(TestInstance, null);
                }
            };
            phaseDelegates.Add(testPhase, action);
        }
    }
}