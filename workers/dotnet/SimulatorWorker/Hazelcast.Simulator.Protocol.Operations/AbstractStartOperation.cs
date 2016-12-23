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
using System.Threading.Tasks;
using Hazelcast.Simulator.Protocol.Connector;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Processors;
using Hazelcast.Simulator.Test;
using Hazelcast.Simulator.Utils;
using log4net;

namespace Hazelcast.Simulator.Protocol.Operations
{
    public abstract class AbstractStartOperation : AbstractTestOperation
    {
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(AbstractStartOperation));

        public override async Task<ResponseType> RunInternal(OperationContext operationContext, SimulatorAddress targetAddress)
        {
            Task.Run(() =>
            {
                TestContainer testContainer;
                if (!operationContext.Tests.TryGetValue(targetAddress.TestIndex, out testContainer))
                {
                    throw new InvalidOperationException($"Test not created yet with testIndex:{targetAddress.TestIndex}");
                }
                TestPhase testPhase = GetTestPhase();
                try
                {
                    try
                    {
                        StartPhase(operationContext, testContainer);
                    }
                    finally
                    {
                        if (testPhase == TestPhases.GetLastTestPhase())
                        {
                            operationContext.Tests.TryRemove(targetAddress.TestIndex, out testContainer);
                        }
                    }
                }
                catch (Exception ex)
                {
                    string testId = testContainer.TestContext.GetTestId();
                    Logger.Error($"{testPhase.GetDescription()} of {testId} FAILDED.");
                    ExceptionReporter.Report(testId, ex);
                }
                finally
                {
                    SendPhaseCompletedOperation(operationContext.Connector, testPhase);
                }
            });
            return ResponseType.Success;
        }

        protected void SendPhaseCompletedOperation(WorkerConnector connector, TestPhase testPhase)
        {
            var operation = new PhaseCompletedOperation(testPhase);
            connector.Submit(sourceAddress, SimulatorAddress.COORDINATOR, operation);
        }

        protected abstract void StartPhase(OperationContext operationContext, TestContainer testContainer);

        protected abstract TestPhase GetTestPhase();
    }
}