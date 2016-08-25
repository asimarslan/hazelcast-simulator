using System;

namespace DotNettyDemo
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");


			var task = ServerConnector.RunServerAsync();

			Console.WriteLine(task.Status);
			ClientConnector.RunClientAsync().Wait();
		}
	}
}
