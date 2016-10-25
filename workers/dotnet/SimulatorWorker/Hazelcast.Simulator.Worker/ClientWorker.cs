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
        private readonly string publicAddress;
        private readonly int agentIndex;
        private readonly int workerIndex;
        private readonly int workerPort;
        private readonly string hzConfigFile;
        private readonly bool autoCreateHzInstance;
        private readonly int workerPerformanceMonitorIntervalSeconds;
        private readonly IHazelcastInstance hazelcastInstance;


        private readonly WorkerConnector workerConnector;

        public ClientWorker()
        {
        }

        private ClientWorker(string workerType, string publicAddress, int agentIndex, int workerIndex, int workerPort,
            string hzConfigFile, bool autoCreateHzInstance, int workerPerformanceMonitorIntervalSeconds)
        {
            this.workerType = workerType;
            this.publicAddress = publicAddress;
            this.agentIndex = agentIndex;
            this.workerIndex = workerIndex;
            this.workerPort = workerPort;
            this.hzConfigFile = hzConfigFile;
            this.autoCreateHzInstance = autoCreateHzInstance;
            this.workerPerformanceMonitorIntervalSeconds = workerPerformanceMonitorIntervalSeconds;

            this.hazelcastInstance = this.GetHazelcastInstance();

            this.workerConnector = new WorkerConnector(agentIndex, workerIndex,this.workerPort,this.hazelcastInstance, this);
            this.SignalStartToAgent();
        }

        private IHazelcastInstance GetHazelcastInstance()
        {
            IHazelcastInstance instance = null;
            if (this.autoCreateHzInstance) {
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
            log.Info($"Starting .Net Worker pid:{Process.GetCurrentProcess().Id}");

            Environment.GetEnvironmentVariable("workerId");
            Environment.GetEnvironmentVariable("publicAddress");
            Environment.GetEnvironmentVariable("workerType");

            string workerId = Environment.GetEnvironmentVariable("workerId");
            string workerType = Environment.GetEnvironmentVariable("workerType");

            string publicAddress = Environment.GetEnvironmentVariable("publicAddress");
            int agentIndex = int.Parse(Environment.GetEnvironmentVariable("agentIndex"));
            int workerIndex = int.Parse(Environment.GetEnvironmentVariable("workerIndex"));
            int workerPort = int.Parse(Environment.GetEnvironmentVariable("workerPort"));
            string hzConfigFile = Environment.GetEnvironmentVariable("hzConfigFile");
            bool autoCreateHzInstance = true;
            try
            {
                autoCreateHzInstance = bool.Parse(Environment.GetEnvironmentVariable("autoCreateHzInstance"));
            }
            catch (Exception)
            {
            }
            int workerPerformanceMonitorIntervalSeconds = int.Parse(Environment.GetEnvironmentVariable("workerPerformanceMonitorIntervalSeconds"));

            ClientWorker worker = new ClientWorker(workerType, publicAddress, agentIndex, workerIndex, workerPort, hzConfigFile,
                autoCreateHzInstance, workerPerformanceMonitorIntervalSeconds);

            await worker.Start();
        }

        private void SignalStartToAgent()
        {
            string address = GetHazelcastAddress(this.workerType, publicAddress, hazelcastInstance);
            var path = $"{GetUserDirectoryPath()}/worker.address";
            Console.WriteLine($"worker.address: {path}");
            File.WriteAllText(path, address);
        }


        public async Task Start()
        {
            await this.workerConnector.Start();
        }


        public static void Main(string[] args)
        {
            BasicConfigurator.Configure();
            log.Info($"Starting .Net Worker pid:{Process.GetCurrentProcess().Id}");
            Task.Run(async () => await StartWorker());
        }

    }
}