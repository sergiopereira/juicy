using System;
using Juicy.DirtCheapDaemons.Http;

namespace DirtCheapHttpServer
{
	class Program
	{
		static void Main(string[] args)
		{
			HttpServer server = new HttpServer();
			server.Start();

			server.Mount(
					"/", 
					(req, resp) => resp.Output.Write("<h1>Welcome to the Juicy Web Server</h1>")
				);

			Console.WriteLine("Press enter to stop server");
			Console.ReadLine();
			server.Shutdown();
			Console.WriteLine("Press enter");
			Console.ReadLine();
		}
	}
}
