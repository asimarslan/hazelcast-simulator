using System;
using System.Threading.Tasks;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Operations;
using log4net;

namespace Hazelcast.Simulator.Protocol.Processors
{
    using Newtonsoft.Json;

    public class AbstractOperationProcessor //: IOperationProcessor
    {
        private static ILog Logger = LogManager.GetLogger(typeof(AbstractOperationProcessor));

        public Task<ResponseType> Process(SimulatorMessage msg)
        {
            var simulatorOperation = JsonConvert.DeserializeObject(msg.OperationData, msg.OperationType.GetClassType());
        }
    }
}