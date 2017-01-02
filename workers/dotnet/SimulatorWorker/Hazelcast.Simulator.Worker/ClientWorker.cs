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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Hazelcast.Core;
using Hazelcast.Simulator.Protocol.Connector;
using Hazelcast.Simulator.Protocol.Core;
using Hazelcast.Simulator.Protocol.Processors;
using log4net;
using log4net.Config;
using log4net.Repository;
using static Hazelcast.Simulator.Utils.HazelcastUtils;
using static Hazelcast.Simulator.Utils.FileUtils;

namespace Hazelcast.Simulator.Worker
{
    public class ClientWorker
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ClientWorker));

        private readonly string workerType;

        public string PublicIpAddress { get; }

        private readonly int agentIndex;
        private readonly int workerIndex;
        private readonly int workerPort;
        private readonly string hzConfigFile;
        private readonly bool autoCreateHzInstance;
        private readonly int workerPerformanceMonitorIntervalSeconds;
        private readonly IHazelcastInstance hazelcastInstance;
        public readonly OperationProcessor operationProcessor;

        public WorkerConnector Connector { get; }

        public ClientWorker(string workerType, string publicIpAddress, int agentIndex, int workerIndex, int workerPort,
            string hzConfigFile, bool autoCreateHzInstance, int workerPerformanceMonitorIntervalSeconds)
        {
            this.workerType = workerType;
            PublicIpAddress = publicIpAddress;
            this.agentIndex = agentIndex;
            this.workerIndex = workerIndex;
            this.workerPort = workerPort;
            this.hzConfigFile = hzConfigFile;
            this.autoCreateHzInstance = autoCreateHzInstance;
            this.workerPerformanceMonitorIntervalSeconds = workerPerformanceMonitorIntervalSeconds;

            hazelcastInstance = GetHazelcastInstance();
            var workerAddress = new SimulatorAddress(AddressLevel.WORKER, agentIndex, workerIndex, 0);
            Connector = new WorkerConnector(agentIndex, workerIndex, this.workerPort, publicIpAddress);
            operationProcessor = new OperationProcessor(new OperationContext(hazelcastInstance, workerAddress, Connector), this);
            Connector.GetOperationProcessor = () => operationProcessor;
            SignalStartToAgent();
        }

        private IHazelcastInstance GetHazelcastInstance()
        {
            IHazelcastInstance instance = null;
            if (autoCreateHzInstance)
            {
                Logger.Info($"Creating {workerType} HazelcastInstance with config:{hzConfigFile}");
                instance = CreateClientHazelcastInstance(hzConfigFile);
                Logger.Info($"HazelcastInstance IsRunning:{instance.GetLifecycleService().IsRunning()}");
            }
            return instance;
        }

        public static async Task StartWorker()
        {
            Dictionary<string, string> workerParams = GetParameterDictionary();
            InitLog(workerParams);

            Logger.Info("Parameters:");
            foreach (KeyValuePair<string, string> param in workerParams)
            {
                Logger.Info($"{param.Key}={param.Value}");
            }

            Logger.Info($"Starting .Net Worker pid:{Process.GetCurrentProcess().Id}");

            string workerType = workerParams["workerType"];
            string publicAddress = workerParams["publicAddress"];

            int agentIndex;
            int.TryParse(workerParams["agentIndex"], out agentIndex);

            int workerIndex;
            int.TryParse(workerParams["workerIndex"], out workerIndex);

            int workerPort;
            int.TryParse(workerParams["workerPort"], out workerPort);

//            string workerHome = workerParams["workerHome"];
            string hzConfigFile = workerParams["hzConfigFile"];

            bool autoCreateHzInstance;
            bool.TryParse(workerParams["autoCreateHzInstance"], out autoCreateHzInstance);

            int workerPerformanceMonitorIntervalSeconds;
            int.TryParse(workerParams["workerPerformanceMonitorIntervalSeconds"], out workerPerformanceMonitorIntervalSeconds);

            ClientWorker worker = null;
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

        public static void InitLog(Dictionary<string, string> workerParams)
        {
            try
            {
                if (workerParams.ContainsKey("log4netConfig"))
                {
                    using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(workerParams["log4netConfig"])))
                    {
                        XmlConfigurator.Configure(stream);
                    }
                }
                else if (workerParams.ContainsKey("log4netConfigFile"))
                {
                    var fileInfo = new FileInfo(workerParams["log4netConfigFile"]);
                    if (fileInfo.Exists)
                    {
                        XmlConfigurator.Configure(fileInfo);
                    }
                }
                else
                {
                    BasicConfigurator.Configure();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception occured during configuring Log4net: {e}");
            }
        }

        private void SignalStartToAgent()
        {
            string address = GetHazelcastAddress(workerType, PublicIpAddress, hazelcastInstance);
            string path = $"{GetUserDirectoryPath()}/worker.address";
            Console.WriteLine($"worker.address: {path}");
            File.WriteAllText(path, address);
        }

        public async Task Start() => await Connector.Start();

        public void Shutdown()
        {
            if (hazelcastInstance != null)
            {
                Logger.Info("Stopping HazelcastInstance...");
                hazelcastInstance.Shutdown();
            }

            //TODO PERf monitor
            //            if (performanceMonitor != null) {
            //                log.Info("Shutting down WorkerPerformanceMonitor");
            //                performanceMonitor.shutdown();
            //            }

            if (Connector != null)
            {
                Logger.Info("Stopping WorkerConnector...");
                try
                {
                    Connector.Shutdown();
                }
                catch (Exception e)
                {
                    Logger.Debug(e);
                }
            }
        }

        public static void Main(string[] args)
        {
            //init cmd params
            GetParameterDictionary(args);

            //start worker
            StartWorker().Wait();
            Logger.Debug($"Stopping .Net Worker pid:{Process.GetCurrentProcess().Id}");
        }
    }
}