using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hazelcast.Simulator.Protocol.Connector;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Processors;
using Hazelcast.Simulator.Test;
using log4net;
using Newtonsoft.Json;

namespace Hazelcast.Simulator.Protocol.Operations
{
    ///<summary>
    /// Starts the <see cref="TestPhase.Run"/> phase of a Simulator Test.
    ///</summary>
    public class StartTestOperation : ISimulatorOperation, ISimulatorMessageAware, IConnectorAware
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CreateTestOperation));

        [JsonProperty("targetType")]
        private readonly string targetType;

        [JsonProperty("targetWorkers")]
        private readonly List<string> targetWorkers;

        [JsonProperty("warmup")]
        private readonly bool warmup;

        [JsonIgnore]
        private SimulatorMessage msg;

        [JsonIgnore]
        private WorkerConnector connector;

        public void SetSimulatorMessage(SimulatorMessage simulatorMessage) => this.msg = simulatorMessage;

        public void SetConnector(WorkerConnector connector) => this.connector = connector;

        public bool MatchesTargetWorkers(SimulatorAddress workerAddress) => this.targetWorkers.Count == 0 || this.targetWorkers.Contains(workerAddress.ToString());

        public async Task<ResponseResult> Run(OperationContext operationContext)
        {
            TestContainer testContainer;
            if (!operationContext.Tests.TryGetValue(msg.Destination.TestIndex, out testContainer))
            {
                throw new InvalidOperationException($"Test not created yet with testIndex:{msg.Destination.TestIndex}");
            }
            TestPhase testPhase = this.warmup ? TestPhase.Warmup : TestPhase.Run;

            if (this.SkipRunPhase(testContainer))
            {
                Logger.Info($"Skipping test {testContainer.TestCase.Id}");
                await this.SendPhaseCompletedOperation(testPhase);
            }
            else
            {
                Logger.Info($"Starting test {testContainer.TestCase.Id}");
                await testContainer.Invoke(testPhase);
            }
            return ResponseResult.Success;
        }

        private bool SkipRunPhase(TestContainer testContainer)
        {
            //TODO: validate targetType
            //            if(! this.targetType in ())
            if (!this.MatchesTargetWorkers(this.msg.Destination.GetParent()))
            {
                Logger.Info($"Skipping test (Worker is not on target list) {testContainer.TestCase.Id}");
                return true;
            }
            return false;
        }

        private async Task SendPhaseCompletedOperation(TestPhase testPhase)
        {
            var operation = new PhaseCompletedOperation(testPhase);

            this.connector.Submit(SimulatorAddress.COORDINATOR, operation);
        }
    }
}