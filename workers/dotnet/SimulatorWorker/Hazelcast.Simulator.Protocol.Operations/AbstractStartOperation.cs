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
using System.Threading.Tasks;
using Hazelcast.Simulator.Protocol.Connector;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Processors;
using Hazelcast.Simulator.Test;
using Hazelcast.Simulator.Utils;
using log4net;
using Newtonsoft.Json;

namespace Hazelcast.Simulator.Protocol.Operations
{
    public abstract class AbstractStartOperation: ISimulatorOperation, ISimulatorMessageAware, IConnectorAware
    {
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(AbstractStartOperation));

        [JsonIgnore]
        protected SimulatorMessage msg;

        [JsonIgnore]
        protected WorkerConnector connector;

        public void SetSimulatorMessage(SimulatorMessage simulatorMessage) => this.msg = simulatorMessage;

        public void SetConnector(WorkerConnector connector) => this.connector = connector;

        public async Task<ResponseType> Run(OperationContext operationContext)
        {
            TestContainer testContainer;
            if (!operationContext.Tests.TryGetValue(this.msg.Destination.TestIndex, out testContainer))
            {
                throw new InvalidOperationException($"Test not created yet with testIndex:{msg.Destination.TestIndex}");
            }
            TestPhase testPhase = this.GetTestPhase();
            try
            {
                try
                {
                    return await this.RunInternal(operationContext, testContainer);
                }
                finally
                {
                    if (testPhase == TestPhases.GetLastTestPhase())
                    {
                        operationContext.Tests.TryRemove(this.msg.Destination.TestIndex, out testContainer);
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
                await this.SendPhaseCompletedOperation(testPhase);
            }
            return null;
        }

        protected async Task SendPhaseCompletedOperation(TestPhase testPhase)
        {
            var operation = new PhaseCompletedOperation(testPhase);
            await this.connector.Submit(SimulatorAddress.COORDINATOR, operation);
        }

        protected abstract Task<ResponseType> RunInternal(OperationContext operationContext, TestContainer testContainer);

        protected abstract TestPhase GetTestPhase();
    }
}