using log4net;
using log4net.Config;

namespace Hazelcast.Simulator.Worker
{
	public class ClientWorker
	{
		static readonly ILog log = LogManager.GetLogger(typeof(ClientWorker));

		public static void Main(string[] args)
		{

			BasicConfigurator.Configure();
			log.Info("Hello World!");
		}
	}
}
