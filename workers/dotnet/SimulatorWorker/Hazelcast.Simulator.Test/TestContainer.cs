using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Common.Utilities;
using Hazelcast.Simulator.Utils;
using static Hazelcast.Simulator.Utils.DependencyInjectionUtil;

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

        private IDictionary<TestPhase, Delegate> phaseDelegates = new Dictionary<TestPhase, Delegate>();

        private object testInstance;

        public TestContainer(TestContext testContext, TestCase testCase)
        {
            this.testContext = testContext;
            this.testCase = testCase;

            this.testInstance = ReflectionUtil.CreateInstanceOfType(testCase.GetClassname());

            this.bindingContainer = new BindingContainer(testCase, testContext);

            this.InjectDependencies();

            this.RegisterPhaseDelegates();
        }

        private void InjectDependencies()
        {
            this.testInstance.GetType().GetMembers();

            foreach (KeyValuePair<string, string> pair in this.testCase.Properties)
            {
                Inject(this.testInstance, pair.Key, pair.Value);
            }
            throw new NotImplementedException();
        }

        public async Task Invoke(TestPhase testPhase)
        {
            if (!this.running.CompareAndSet(false, true))
            {
                throw new InvalidOperationException($"Test:{this.testContext.GetTestId()} is still running phase:{this.currentPhase}");
            }
            this.currentPhase = testPhase;

            if (testPhase == TestPhase.Warmup)
            {
                this.testContext.BeforeWarmup();
            }
            else if (testPhase == TestPhase.LocalAfterWarmup)
            {
                this.testContext.AfterWarmup();
            }
        }

        private void RegisterPhaseDelegates()
        {
            throw new NotImplementedException();
        }
    }
}