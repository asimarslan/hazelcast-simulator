using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Hazelcast.Core;
using Hazelcast.Logging;
using Hazelcast.Simulator.Protocol.Connector;
using Hazelcast.Simulator.Utils;
using log4net;
using log4net.Config;
using static Hazelcast.Simulator.Utils.HazelcastUtils;
using static Hazelcast.Simulator.Utils.FileUtils;

namespace Hazelcast.Simulator.Worker
{
    public class ClientWorker
    {
        static readonly ILog Logger = LogManager.GetLogger(typeof(ClientWorker));

        private readonly string workerType;

        public string PublicIpAddress { get; }

        private readonly int agentIndex;
        private readonly int workerIndex;
        private readonly int workerPort;
        private readonly string hzConfigFile;
        private readonly bool autoCreateHzInstance;
        private readonly int workerPerformanceMonitorIntervalSeconds;
        private IHazelcastInstance hazelcastInstance;

        public WorkerConnector Connector { get; }

        public ClientWorker(string workerType, string publicIpAddress, int agentIndex, int workerIndex, int workerPort,
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
                Logger.Info($"Creating {this.workerType} HazelcastInstance with config:{this.hzConfigFile}");
                instance = HazelcastUtils.CreateClientHazelcastInstance(this.hzConfigFile);
                Logger.Info($"HazelcastInstance IsRunning:{instance.GetLifecycleService().IsRunning()}");
            }
            return instance;
        }

        public static async Task StartWorker()
        {
            Logger.Info($"Starting .Net Worker pid:{Process.GetCurrentProcess().Id}");

            var workerParams = FileUtils.GetParameterDictionary();
            Logger.Info("Parameters:");
            foreach (var param in workerParams)
            {
                Logger.Info($"{param.Key}={param.Value}");
            }

            string workerId = workerParams["workerId"];
            string workerType = workerParams["workerType"];

            string publicAddress = workerParams["publicAddress"];

            int agentIndex;
            int.TryParse(workerParams["agentIndex"], out agentIndex);

            int workerIndex;
            int.TryParse(workerParams["workerIndex"], out workerIndex);

            int workerPort;
            int.TryParse(workerParams["workerPort"], out workerPort);

            string workerHome = workerParams["workerHome"];
            string hzConfigFile = workerHome + "/" + workerParams["hzConfigFile"];

            bool autoCreateHzInstance;
            bool.TryParse(workerParams["autoCreateHzInstance"], out autoCreateHzInstance);
            int workerPerformanceMonitorIntervalSeconds;
            int.TryParse(workerParams["workerPerformanceMonitorIntervalSeconds"], out workerPerformanceMonitorIntervalSeconds);

            ClientWorker worker=null;
            try
            {
                worker = new ClientWorker(workerType, publicAddress, agentIndex, workerIndex, workerPort,
                    hzConfigFile, autoCreateHzInstance, workerPerformanceMonitorIntervalSeconds);

                await worker.Start();

            }
            catch (Exception e)
            {
                Logger.Warn("Unhandled exception", e);
            }
            finally
            {
                worker?.Shutdown();
                
            }
        }

        private void SignalStartToAgent()
        {
            string address = GetHazelcastAddress(this.workerType, this.PublicIpAddress, this.hazelcastInstance);
            var path = $"{GetUserDirectoryPath()}/worker.address";
            Console.WriteLine($"worker.address: {path}");
            File.WriteAllText(path, address);
        }

        public async Task Start() => await this.Connector.Start();

        public void Shutdown()
        {
            if (this.hazelcastInstance != null)
            {
                Logger.Info("Stopping HazelcastInstance...");
                this.hazelcastInstance.Shutdown();
            }

            //TODO PERf monitor
            //            if (performanceMonitor != null) {
            //                log.Info("Shutting down WorkerPerformanceMonitor");
            //                performanceMonitor.shutdown();
            //            }

            if (this.Connector != null)
            {
                Logger.Info("Stopping WorkerConnector...");
                try
                {
                    this.Connector.Shutdown();

                }
                catch (Exception e)
                {
                    Logger.Debug(e);
                }
            }

            //OperationTypeCounter.printStatistics(Level.DEBUG);
        }

        public bool Ready => this.Connector.Ready;

        public static void Main(string[] args)
        {
            XmlConfigurator.Configure(new System.IO.FileInfo("log4net.xml"));

            Logger.Debug($"Starting .Net Worker pid:{Process.GetCurrentProcess().Id}");
            StartWorker().Wait();

            Logger.Debug($"Stopping .Net Worker pid:{Process.GetCurrentProcess().Id}");
        }
    }
}