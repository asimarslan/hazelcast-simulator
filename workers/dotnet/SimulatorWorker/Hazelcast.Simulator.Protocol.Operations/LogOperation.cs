using System;
using System.Threading.Tasks;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Processors;
using log4net;
using log4net.Core;
using Newtonsoft.Json;

namespace Hazelcast.Simulator.Protocol.Operations
{
    /// <summary>
    /// Writes the message with the requested log level to the local logging framework.
    /// </summary>
    public class LogOperation :AbstractWorkerOperation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LogOperation));

        /// <summary>
        /// Defines the message which should be logged.
        /// </summary>
        [JsonProperty("message")]
        private readonly string message;

        /// <summary>
        /// Defines the desired log level of the message.
        /// </summary>
        [JsonProperty("level")]
        private readonly string level;

        public LogOperation(string message) :this(message, Level.Info)
        {
        }

        public LogOperation(string message, Level level)
        {
            this.message = message;
            this.level = level.ToString();
        }

        public override async Task<ResponseType> RunInternal(OperationContext operationContext, SimulatorAddress targetAddress)
        {
            Logger.Logger.Log(typeof(LogOperation), this.GetLevel(), $"[{this.sourceAddress}] {this.message}", null);
            return ResponseType.Success;
        }

        private Level GetLevel() => LogManager.GetRepository().LevelMap[this.level];
    }
}