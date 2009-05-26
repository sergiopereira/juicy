using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Web;

namespace Juicy.DirtCheapDaemons.Http
{

	public class HttpServer
	{
		public const int DefaultPortNumber = 8081;
		private TcpListener _listener;
		private Thread _serverThread;
		private readonly MountPointResolver _resolver = new MountPointResolver();

		public HttpServer()
			: this(DefaultPortNumber)
		{
		}

		public HttpServer(int portNumber)
		{
			MountPoints = new List<MountPoint>();
			PortNumber = portNumber;
		}

		private bool IsRunning { get { return _serverThread != null; } }

		private int _portNumber;
		public int PortNumber
		{
			get { return _portNumber; }
			set
			{
				if (IsRunning)
				{
					throw new InvalidOperationException("Cannot change the port number after the server has been started.");
				}
				_portNumber = value;
			}
		}

		public IList<MountPoint> MountPoints { get; private set; }
		public string RootUrl { get { return string.Format("http://localhost:{0}/", PortNumber); } }

		public void Mount(string virtualDir, string physicalDirectory)
		{
			Mount(virtualDir, new StaticFileHandler(physicalDirectory));
		}

		public void Mount(string virtualDir, Action<IRequest, IResponse> handler)
		{
			Mount(virtualDir, new InlineHandler(handler));
		}

		public void Mount(string virtualDir, IMountPointHandler handler)
		{
			MountPoints.Add(new MountPoint { VirtualPath = virtualDir, Handler = handler });
		}

		public void Unmount(string virtualDir)
		{
			var mount = MountPoints.FirstOrDefault(m => m.VirtualPath.Equals(virtualDir, StringComparison.OrdinalIgnoreCase));
			if (mount != null)
			{
				MountPoints.Remove(mount);
			}
		}

		public void UnmountAll()
		{
			MountPoints.Clear();
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

			Console.WriteLine("Juicy Web Server ready at http://localhost:" + PortNumber + "/");

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


		public void WaitForConnection()
		{
			try
			{
				while (true)
				{
					//Accept a new connection
					using (var socket = _listener.AcceptSocket())
					{
						if(socket.Connected)
						{
							Console.WriteLine("\nRequest from IP {0}\n", socket.RemoteEndPoint);
							string reqText = GetRequestText(socket);

							if (string.IsNullOrEmpty(reqText))
							{
								Console.WriteLine("Empty request, canceling.");
								socket.Close();
								continue;
							}

							string[] lines = reqText.Split(new[] {"\r\n"}, StringSplitOptions.None);
							string firstLine = lines[0];

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

							string vpath = "/";
							//IMountPointHandler handler;
							MountPoint mount = null;


							if (CheckIfHttpRequest(firstLine))
							{
								string[] httpCommand = firstLine.Split(' ');
								//so this must be an HTTP request
								var httpVerb = httpCommand[0];

								//a vpath must have been given in the command
								vpath = httpCommand[1];
								Console.WriteLine("Requested path:" + vpath);

								if(ValidateHttpVerb(httpVerb))
								{
									mount = FindMount(vpath);
								}
							}

							if(mount == null)
							{
								mount = CreateUnacceptableMountPoint(vpath);
							}

							Console.WriteLine("Request being handled at vpath: {0}, by handler: {1}",
												  mount.VirtualPath, mount.Handler);

							var request = new RequestFactory().Create(lines, mount, vpath);
							var response = CreateResponse(HttpStatusCode.OK, "OK");

							//But... we can't accept all kinds of posts just yet.. it's gotta be 
							// simple form values or text body (no encoding)... no file uploads and stuff
							if (firstLine.StartsWith("POST ", StringComparison.OrdinalIgnoreCase))
							{
								if (request.Headers.ContainsKey("Content-Type") 
									&&
									!request.Headers["Content-Type"].Equals("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
								{
									mount = CreateUnacceptableMountPoint(vpath);
								}
							}

							mount.Handler.Respond(request, response);
							SendResponse(response, socket);

							socket.Close();
						}
					}
					Thread.Sleep(50);
				}
			}
			catch (SocketException)
			{

			}
		}

		private MountPoint CreateUnacceptableMountPoint(string vpath)
		{
			var handler = new EmptyHttpResponseHandler(HttpStatusCode.NotAcceptable, "Not acceptable");
			return new MountPoint { Handler = handler, VirtualPath = vpath };
		}

		private bool CheckIfHttpRequest(string line)
		{
			var cmd = line.Split(' ');
			return cmd.Length == 3 && cmd[2].StartsWith("HTTP", StringComparison.OrdinalIgnoreCase);
		}

		private static Response CreateResponse(HttpStatusCode statusCode, string statusMessage)
		{
			var response = new Response
				{
					StatusCode = statusCode,
					StatusMessage = statusMessage
				};

			//add some standard headers that can be replaced by 
			// the handler if needed

			response["Cache-Control"] = "private";
			response["Content-Type"] = "text/html; charset=utf-8";
			response["Server"] = "Juicy/1.0";
			response["Date"] = DateTime.UtcNow.ToString("ddd, d MMM yyyy HH:mm:ss 'GMT'");

			return response;
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
			if (
				!httpVerb.Equals("GET", StringComparison.OrdinalIgnoreCase)
				&&
				!httpVerb.Equals("POST", StringComparison.OrdinalIgnoreCase)
				)
			{
				Console.WriteLine("Requested HTTP verb '{0}' not supported.", httpVerb);
				return false;
			}

			return true;
		}

		private MountPoint FindMount(string requestedVirtualDir)
		{
			return _resolver.Resolve(MountPoints, requestedVirtualDir)
				??
				new MountPoint { VirtualPath = "/", Handler = new ResourceNotFoundHandler() };
		}

		private void SendResponse(Response response, Socket socket)
		{
			socket.Send(Encoding.UTF8.GetBytes(
				string.Format("HTTP/1.1 {0} {1}\r\n", (int)response.StatusCode, response.StatusMessage)
				));


			string body = response.GetResponseBodyText();
			var buffer = Encoding.UTF8.GetBytes(body);

			response["Content-Length"] = buffer.Length.ToString();
			SendAllHeaders(response, socket);

			socket.Send(Encoding.UTF8.GetBytes("\r\n")); //end of headers

			socket.Send(buffer);
		}

		private static void SendAllHeaders(IResponse response, Socket socket)
		{
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
