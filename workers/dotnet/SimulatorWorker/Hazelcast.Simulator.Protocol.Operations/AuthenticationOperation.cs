using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Processors;

namespace Hazelcast.Simulator.Protocol.Operations
{
    public class AuthenticationOperation : AbstractWorkerOperation
    {
        public override async Task<ResponseType> RunInternal(OperationContext operationContext, SimulatorAddress targetAddress) => ResponseType.Success;
    }
}
