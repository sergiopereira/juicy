using System;
using System.IO;
using Juicy.DirtCheapDaemons.Http;
using System.Threading;

namespace DirtCheapHttpServer
{
	class Program
	{
		static void Main(string[] args)
		{
			//RunWebServer();

			Temp();
		    Console.WriteLine("Press enter");
			Console.ReadLine();
           
		}

		private static void Temp()
		{
			HttpServer server = new HttpServer { PortNumber = 2000 };
			server.Start();

			server.Mount("/getData",
				(req, resp) =>
				{
					resp.Output.Write("{result: 'ok', accountId: 123}");
					resp["Content-Type"] = "application/json";
					var delay = int.Parse(req.QueryString["delay"] ?? "0");
					Thread.Sleep(1000 * delay);
				}
			);

			Console.WriteLine("Press enter to stop server");
			Console.ReadLine();
			server.Shutdown();
		}

	    private static void RunWebServer()
	    {
	        HttpServer server = new HttpServer();
	        server.Start();

	        //this responds with content programmatically created
	        server.Mount(
	            "/", 
	            (req, resp) => resp.Output.Write(@"
					<h1>Welcome to the Juicy Web Server</h1>
					<a href=""test/page1.html"">Static Page</a><br>
					<a href=""test/subdir/page2.html"">Static Page in subdirectory</a><br>
					<a href=""test/notafile.aspx"">Dynamic page</a><br>")
	            );

	        //this will serve static files
	        var testWebRoot = Path.GetFullPath(@"..\..\webroot");
			server.Mount("/test", testWebRoot);

			//a "hidden" path
			server.Mount("/hidden", new ResourceNotFoundHandler());

	        //this path shows that more specific paths take precedence
	        server.Mount("/test/notafile.aspx", 
	                     (req, resp) => resp.Output.Write("<h1>notafile.aspx is dynamically created</h1><a href=\"/\">Home</a>"));

	        Console.WriteLine("Press enter to stop server");
	        Console.ReadLine();
	        server.Shutdown();
	    }


	    
	}
}
