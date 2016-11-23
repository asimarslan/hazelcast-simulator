using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Hazelcast.Core;
using Hazelcast.Simulator.Protocol.Connector;
using Hazelcast.Simulator.Protocol.Core;
using log4net;
using log4net.Config;
using static Hazelcast.Simulator.Utils.HazelcastUtils;
using static Hazelcast.Simulator.Utils.FileUtils;

namespace Hazelcast.Simulator.Worker
{
    public class ClientWorker
    {
        static readonly ILog log = LogManager.GetLogger(typeof(ClientWorker));

        private readonly string workerType;

        public string PublicIpAddress { get; }

        private readonly int agentIndex;
        private readonly int workerIndex;
        private readonly int workerPort;
        private readonly string hzConfigFile;
        private readonly bool autoCreateHzInstance;
        private readonly int workerPerformanceMonitorIntervalSeconds;
        private readonly IHazelcastInstance hazelcastInstance;

        public WorkerConnector Connector { get; }

        public ClientWorker()
        {
        }

        private ClientWorker(string workerType, string publicIpAddress, int agentIndex, int workerIndex, int workerPort,
            string hzConfigFile, bool autoCreateHzInstance, int workerPerformanceMonitorIntervalSeconds)
        {
            this.workerType = workerType;
            this.PublicIpAddress = publicIpAddress;
            this.agentIndex = agentIndex;
            this.workerIndex = workerIndex;
            this.workerPort = workerPort;
            this.hzConfigFile = hzConfigFile;
            this.autoCreateHzInstance = autoCreateHzInstance;
            this.workerPerformanceMonitorIntervalSeconds = workerPerformanceMonitorIntervalSeconds;

            this.hazelcastInstance = this.GetHazelcastInstance();

            this.Connector = new WorkerConnector(agentIndex, workerIndex, this.workerPort, this.hazelcastInstance, this);
            this.SignalStartToAgent();
        }

        private IHazelcastInstance GetHazelcastInstance()
        {
            IHazelcastInstance instance = null;
            if (this.autoCreateHzInstance)
            {
                log.Info($"Creating {this.workerType} HazelcastInstance");
                //                instance = createClientHazelcastInstance(hzConfigFile);
                //                logHeader("Successfully created " + type + " HazelcastInstance");
                //                warmupPartitions(instance);
            }
            return instance;
        }

        public static async Task StartWorker()
        {
            log.Info($"Starting .Net Worker pid:{Process.GetCurrentProcess().Id}");

//            string workerId = Environment.GetEnvironmentVariable("workerId");
            string workerType = Environment.GetEnvironmentVariable("workerType");

            string publicAddress = Environment.GetEnvironmentVariable("publicAddress");

            int agentIndex;
            int.TryParse(Environment.GetEnvironmentVariable("agentIndex"), out agentIndex);

            int workerIndex;
            int.TryParse(Environment.GetEnvironmentVariable("workerIndex"), out workerIndex);

            int workerPort;
            int.TryParse(Environment.GetEnvironmentVariable("workerPort"), out workerPort);

            string hzConfigFile = Environment.GetEnvironmentVariable("hzConfigFile");

            bool autoCreateHzInstance;
            bool.TryParse(Environment.GetEnvironmentVariable("autoCreateHzInstance"), out autoCreateHzInstance);
            int workerPerformanceMonitorIntervalSeconds = int.Parse(Environment.GetEnvironmentVariable("workerPerformanceMonitorIntervalSeconds"));

            ClientWorker worker = new ClientWorker(workerType, publicAddress, agentIndex, workerIndex, workerPort, hzConfigFile,
                autoCreateHzInstance, workerPerformanceMonitorIntervalSeconds);

            await worker.Start();
        }

        private void SignalStartToAgent()
        {
            string address = GetHazelcastAddress(this.workerType, this.PublicIpAddress, this.hazelcastInstance);
            var path = $"{GetUserDirectoryPath()}/worker.address";
            Console.WriteLine($"worker.address: {path}");
            File.WriteAllText(path, address);
        }

        public async Task Start()
        {
            await this.Connector.Start();
        }

        public void Shutdown()
        {
            if (this.hazelcastInstance != null) {
                log.Info("Stopping HazelcastInstance...");
                this.hazelcastInstance.Shutdown();
            }

            //TODO PERf monitor
//            if (performanceMonitor != null) {
//                log.Info("Shutting down WorkerPerformanceMonitor");
//                performanceMonitor.shutdown();
//            }

            if (this.Connector != null) {
                log.Info("Stopping WorkerConnector...");
                this.Connector.Shutdown();
            }

//            OperationTypeCounter.printStatistics(Level.DEBUG);
            Environment.Exit(0);
        }

        public static void Main(string[] args)
        {
            BasicConfigurator.Configure();
            log.Info($"Starting .Net Worker pid:{Process.GetCurrentProcess().Id}");
            Task.Run(async () => await StartWorker());
        }
    }
}