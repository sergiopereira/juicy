using System;
using System.IO;
using Juicy.DirtCheapDaemons.Http;
using System.Net;

namespace DirtCheapHttpServer
{
	class Program
	{
		static void Main(string[] args)
		{
			RunWebServer();
		    

		    Console.WriteLine("Press enter");
			Console.ReadLine();
           
		}

	    private static void RunWebServer()
	    {
	        HttpServer server = new HttpServer();
	        server.Start();

	        //this responds with content programmatically created
	        server.Mount(
	            "/", 
	            (req, resp) => resp.Output.Write("<h1>Welcome to the Juicy Web Server</h1>")
	            );

	        //this will serve static files
	        var testWebRoot = Path.GetFullPath(@"..\..\webroot");
	        server.Mount("/test", testWebRoot);

	        //this path shows that more specific paths take precedence
	        server.Mount("/test/notafile.aspx", 
	                     (req, resp) => resp.Output.Write("<h1>notafile.aspx is dynamically created</h1>"));

	        Console.WriteLine("Press enter to stop server");
	        Console.ReadLine();
	        server.Shutdown();
	    }


	    
	}
}
