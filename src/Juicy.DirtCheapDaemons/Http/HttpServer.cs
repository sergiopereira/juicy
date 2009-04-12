using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace Juicy.DirtCheapDaemons.Http
{

	public class HttpServer
	{
		public const int DefaultPortNumber = 8081;
		private TcpListener _listener;
		private Thread _serverThread;

		public HttpServer()
			: this(DefaultPortNumber)
		{
			MountPoints = new List<MountPoint>();
		}

		public HttpServer(int portNumber)
		{
			PortNumber = portNumber;
		}

		public int PortNumber { get; set; }
		public IList<MountPoint> MountPoints { get; private set; }

		public void Mount(string virtualDir, string physicalDirectory)
		{
			Mount(virtualDir, new StaticFileHandler(physicalDirectory) );
		}
	
		public void Mount(string virtualDir, Action<IRequest,IResponse> handler)
		{
			Mount(virtualDir, new InlineHandler(handler));
		}

        public void Mount(string virtualDir, IMountPointHandler handler)
		{
            MountPoints.Add(new MountPoint {VirtualPath = virtualDir, Handler = handler});
		}


		public void Start()
		{
			Shutdown();
			var host = Dns.GetHostEntry("localhost");
			var localIP = host.AddressList[0];
			_listener = new TcpListener(localIP, PortNumber);
			_listener.Start();

			_serverThread = new Thread(WaitForConnection);
			_serverThread.Start();

			Console.WriteLine("Juicy Web Server ready on port " + PortNumber + ". ");
			
		}

		public void Shutdown()
		{
            if (_listener != null)
			{
			    _listener.Stop();
                _listener = null;
			}

			if (_serverThread != null)
			{
				_serverThread.Abort();
				_serverThread = null;
			}
		}

		public bool Ping()
		{
			using (var cli = new TcpClient())//"localhost", PortNumber))
			{
				cli.Connect("localhost", PortNumber);
				var startTime = DateTime.Now;
				var elapsed = TimeSpan.Zero;
				while (!cli.Connected && elapsed.TotalMilliseconds < 200)
				{
					Thread.Sleep(10);
					elapsed = DateTime.Now - startTime;
				}
				if (!cli.Connected)
				{
					Console.WriteLine("Timeout trying to connect for Ping.");
					return false;
				}
				var wr = new StreamWriter(cli.GetStream());
				wr.Write("PING");
				wr.Flush();
				//wr.Close();

				var rd = new StreamReader(cli.GetStream());
				startTime = DateTime.Now;
				elapsed = TimeSpan.Zero;
				string response = "";

				while (string.IsNullOrEmpty(response) && elapsed.TotalMilliseconds < 200)
				{
					Thread.Sleep(10);
					elapsed = DateTime.Now - startTime;
					response += rd.ReadToEnd();
				}

				bool result = response.StartsWith("PONG");
				Console.WriteLine("Server at localhost:{0} is {1}", PortNumber, result ? "Online" : "Offline");
				
				return result;

			}
		}

		public void WaitForConnection()
		{

			while (true)
			{
				//Accept a new connection
				using (var socket = _listener.AcceptSocket())
				{
					//Console.WriteLine("Socket Type " + mySocket.SocketType);
					if (socket.Connected)
					{
						Console.WriteLine("\nRequest from IP {0}\n", socket.RemoteEndPoint);
						string reqText = GetRequestText(socket);
                        if(string.IsNullOrEmpty(reqText))
                        {
                            socket.Close();
                            continue;
                        }
					    string[] lines = reqText.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);

                        //(starting n the next line is what a GET request looks like, line break = \r\n                                                
                        //GET /some/path/in/the/server.html HTTP/1.1
                        //Host: localhost:8081
                        //User-Agent: Mozilla/5.0 (blah blah blah)
                        //Accept: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8
                        //Accept-Language: en-us,en;q=0.5
                        //Accept-Encoding: gzip,deflate
                        //Accept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.7
                        //Keep-Alive: 300
                        //Connection: keep-alive
                        //Cookie: cookie1=val1; cookie2=val2;

					    MountPoint mount = null;
						IMountPointHandler handler;
					    string vpath = null;

						if (PingHandler.IsPing(reqText))
						{
							//this is not HTTP.. just our custom ping request
							handler = new PingHandler();
						}
						else
						{
							//so this must be an HTTP request
						    string[] httpCommand = lines[0].Split(' ');
						    var httpVerb = httpCommand[0];
						    vpath = httpCommand[1];

                            if (!ValidateHttpVerb(httpVerb))
							{
								continue;
                                //TODO: return an error code here
							}

							mount = FindHandler(vpath);
						    handler = mount.Handler;
						}

					    var request = CreateRequest(lines, mount, vpath);
                        var response = CreateResponse(200, "OK");
					    handler.Respond(request, response);
                        SendResponse(response, socket);

						socket.Close();

					    #region old code


					    //if (sRequestedFile.Length == 0)
					    //{
					    //    // Get the default filename
					    //    sRequestedFile = GetTheDefaultFileName(sLocalDir);
					    //    if (sRequestedFile == "")
					    //    {
					    //        sErrorMessage = "<H2>Error!! No Default File Name Specified</H2>";
					    //        SendHeader(sHttpVersion, "", sErrorMessage.Length, " 404 Not Found", ref socket);
					    //        SendToBrowser(sErrorMessage, ref socket);
					    //        socket.Close();
					    //        return;
					    //    }
					    //}
						
                        
					    //    int iTotBytes = 0;
					    //    sResponse = "";
					    //    FileStream fs = new FileStream(sPhysicalFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
					    //    // Create a reader that can read bytes from the FileStream.
					    //    BinaryReader reader = new BinaryReader(fs);
					    //    byte[] bytes = new byte[fs.Length];
					    //    int read;
					    //    while ((read = reader.Read(bytes, 0, bytes.Length)) != 0)
					    //    {
					    //        // Read from the file and write the data to the network
					    //        sResponse = sResponse + Encoding.ASCII.GetString(bytes, 0, read);
					    //        iTotBytes = iTotBytes + read;
					    //    }
					    //    reader.Close();
					    //    fs.Close();
					    //    SendHeader(sHttpVersion, sMimeType, iTotBytes, " 200 OK", ref socket);
					    //    SendToBrowser(bytes, ref socket);


					    #endregion

					}
				}
				Thread.Sleep(50);
			}
		}

	    private static Response CreateResponse(int statusCode, string statusMessage)
        {
            var response = new Response
                {
                    StatusCode = statusCode,
                    StatusMessage = statusMessage
                };
            //add some standard headers that can be replaced by 
            // the handler if needed

            response.Headers["Cache-Control"] = "private";
	        response.Headers["Content-Type"] = "text/html; charset=utf-8";
	        response.Headers["Server"] = "Juicy/1.0";
	        response.Headers["Date"] = DateTime.UtcNow.ToString("ddd, d MMM yyyy HH:mm:ss 'GMT'");
            
            return response;
        }



        private static Request CreateRequest(IEnumerable<string> requestLines, MountPoint mount, string vpath)
        {
            var request = new Request {MountPoint = mount, VirtualPath = vpath};
            //copy the headers to the request object
            requestLines.Skip(1).ToList().ForEach(line => {
                    if (!string.IsNullOrEmpty(line)) {
                        int pos = line.IndexOf(":");
                        if (pos > 0) {
                            request.Headers.Add(line.Substring(0, pos), line.Substring(pos + 1));
                        }
                    }
                });
            return request;
        }

		private static string GetRequestText(Socket socket)
		{
			byte[] bytes = new byte[1024];
			socket.Receive(bytes, bytes.Length, 0);
			return Encoding.ASCII.GetString(bytes).TrimEnd('\0');
		}

        private static bool ValidateHttpVerb(string httpVerb)
        {
            //At present we will only deal with GET type
            if (!httpVerb.Equals("GET", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Requested HTTP verb not supported.");
                return false;
            }

            return true;
        }

        private MountPoint FindHandler(string requestedVirtualDir)
		{
	        var mounts = from point in MountPoints
                               orderby point.VirtualPath descending 
	                           select point;
	        var mountedPoint =
	            mounts.FirstOrDefault(p => requestedVirtualDir.StartsWith(p.VirtualPath, StringComparison.OrdinalIgnoreCase));
            //TODO: if path not mounted, return error code
            return mountedPoint;
		}

		private void SendResponse(Response response, Socket socket)
		{
			//TODO: send headers
            //TODO: take care of text encoding... assuming ASCII is no good
            socket.Send(Encoding.UTF8.GetBytes(
                string.Format("HTTP/1.1 {0} {1}\r\n", response.StatusCode, response.StatusMessage)
                )); //end of headers
            
            
            string body = response.GetResponseBodyText();
		    var buffer = Encoding.UTF8.GetBytes(body);
            
            AddHeaders(response, socket);
            SendHeader("Content-Length", buffer.Length.ToString(), socket);
		    
            socket.Send(Encoding.UTF8.GetBytes("\r\n")); //end of headers

		    socket.Send(buffer);
		}

        private static void AddHeaders(IResponse response, Socket socket)
        {
            /*
            Cache-Control	private
            Content-Type	text/html; charset=utf-8
            Content-Encoding	gzip
            Expires	Sat, 11 Apr 2009 16:06:57 GMT
            Vary	Accept-Encoding
            Server	Microsoft-IIS/7.0
            Date	Sat, 11 Apr 2009 16:06:57 GMT
            Content-Length	12772
             */

            foreach (var h in response.Headers) 
            {
                SendHeader(h.Key, h.Value, socket);
            }
        }

        private static void SendHeader(string name, string value, Socket socket)
        {
            socket.Send(Encoding.UTF8.GetBytes(string.Format("{0}: {1}\r\n", name, value)));
        }
	}
}
