using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Common.Utilities;
using Hazelcast.Simulator.Utils;
using static Hazelcast.Simulator.Utils.DependencyInjectionUtil;
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
        private readonly TestContext testContext;
        private readonly TestCase testCase;
        private BindingContainer bindingContainer;
        private TestPhase currentPhase;
        private readonly AtomicBoolean running = new AtomicBoolean(false);
        private IDictionary<TestPhase, Action> phaseDelegates = new Dictionary<TestPhase, Action>();

        public object TestInstance { get; }

        public TestContainer(TestContext testContext, TestCase testCase, object testInstance = null)
        {
            this.testContext = testContext;
            this.testCase = testCase;
            this.TestInstance = testInstance ?? ReflectionUtil.CreateInstanceOfType(testCase.GetClassname());
            this.bindingContainer = new BindingContainer(testContext, testCase);

            this.bindingContainer.Bind(this.TestInstance);

            this.RegisterPhaseDelegates();
            //this.testPerformanceTracker = new TestPerformanceTracker(this);
        }

        public async Task Invoke(TestPhase testPhase)
        {
//            if (!this.running.CompareAndSet(false, true))
//            {
//                throw new InvalidOperationException($"Test:{this.testContext.GetTestId()} is still running phase:{this.currentPhase}");
//            }
//            this.currentPhase = testPhase;

            if (testPhase == TestPhase.Warmup)
            {
                this.testContext.BeforeWarmup();
            }
            else if (testPhase == TestPhase.LocalAfterWarmup)
            {
                this.testContext.AfterWarmup();
            }

            Action phaseDelegate = this.phaseDelegates[testPhase];
            phaseDelegate?.Invoke();
        }

        private void RegisterPhaseDelegates()
        {
            Func<Attribute, bool> emptyFilter = attr => true;
            this.RegisterPhase(TestPhase.Setup, typeof(SetupAttribute), emptyFilter);

            this.RegisterPhase(TestPhase.LocalPrepare, typeof(PrepareAttribute), attr => !(attr as PrepareAttribute).Global);
            this.RegisterPhase(TestPhase.GlobalPrepare, typeof(PrepareAttribute), attr => (attr as PrepareAttribute).Global);

            //TODO WARMUP :we need this step in TimeStepRunner !!!
            this.phaseDelegates.Add(TestPhase.Warmup, () => {});

            this.RegisterPhase(TestPhase.LocalAfterWarmup, typeof(AfterWarmupAttribute), attr => !(attr as AfterWarmupAttribute).Global);
            this.RegisterPhase(TestPhase.GlobalAfterWarmup, typeof(AfterWarmupAttribute), attr => (attr as AfterWarmupAttribute).Global);
            //RUN
            this.RegisterPhase(TestPhase.Run, typeof(RunAttribute), emptyFilter);

            this.RegisterPhase(TestPhase.LocalVerify, typeof(VerifyAttribute), attr => !(attr as VerifyAttribute).Global);
            this.RegisterPhase(TestPhase.GlobalVerify, typeof(VerifyAttribute), attr => (attr as VerifyAttribute).Global);

            this.RegisterPhase(TestPhase.LocalTeardown, typeof(TeardownAttribute), attr => !(attr as TeardownAttribute).Global);
            this.RegisterPhase(TestPhase.GlobalTeardown, typeof(TeardownAttribute), attr => (attr as TeardownAttribute).Global);
        }

        private void RegisterPhase(TestPhase testPhase, Type attributeType, Func<Attribute, bool> filter, Action action=null)
        {
            IEnumerable<MemberInfo> methodInfos = GetMethodsWithAttribute(this.TestInstance.GetType(), attributeType);
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
            action = action??delegate
            {
                foreach (MethodInfo methodInfo in methods)
                {
                    methodInfo.Invoke(this.TestInstance, null);
                }
            };
            this.phaseDelegates.Add(testPhase, action);
        }
    }
}