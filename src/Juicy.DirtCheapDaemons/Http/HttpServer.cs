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
			MountPoints = new Dictionary<string, IMountPointHandler>(StringComparer.OrdinalIgnoreCase);
		}

		public HttpServer(int portNumber)
		{
			PortNumber = portNumber;
		}

		public int PortNumber { get; set; }
		public Dictionary<string, IMountPointHandler> MountPoints { get; set; }

		public void Mount(string virtualDir, IMountPointHandler handler)
		{
			MountPoints.Add(virtualDir, handler);
		}

		public void Mount(string virtualDir, Action<IRequest,IResponse> handler)
		{
			MountPoints.Add(virtualDir, new InlineHandler(handler));
		}

		public void Start()
		{
			Shutdown();
			var host = Dns.GetHostEntry("localhost");
			var localIP = host.AddressList[0];
			_listener = new TcpListener(localIP, PortNumber);
			_listener.Start();

			_serverThread = new Thread(new ThreadStart(WaitForConnection));
			_serverThread.Start();

			Console.WriteLine("Juicy Web Server on port " + PortNumber + ". CTRL+C stops it.");
			
		}

		public void Shutdown()
		{
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
				string response = null;

				while (string.IsNullOrEmpty(response) && elapsed.TotalMilliseconds < 200)
				{
					Thread.Sleep(10);
					elapsed = DateTime.Now - startTime;
					response = rd.ReadToEnd();
				}

				bool result = response.StartsWith("PONG");
				Console.WriteLine("Server at localhost:{0} is {1}", PortNumber, result ? "Online" : "Offline");
				
				return result;

			}
		}

		public void WaitForConnection()
		{

			//int iStartPos = 0;
			//String sRequest;
			//String sDirName;
			//String sRequestedFile;
			//String sErrorMessage;
			//String sLocalDir;
			//String sMyWebServerRoot = "C:\\MyWebServerRoot\\";
			//String sPhysicalFilePath = "";
			//String sFormattedMessage = "";
			//String sResponse = "";


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
						IMountPointHandler handler;
						if (PingHandler.IsPing(reqText))
						{
							//this is not HTTP.. just our custom ping request
							handler = new PingHandler();
						}
						else
						{
							//this must be an HTTP request
							if (!ValidateHttpVerb(/*socket,*/ reqText))
							{
								return;
							}

							// Look for HTTP request
							int iStartPos = reqText.IndexOf("HTTP", 1);
							// Get the HTTP text and version e.g. it will return "HTTP/1.1"
							string sHttpVersion = reqText.Substring(iStartPos, 8);

							// Extract the Requested Type and Requested file/directory
							string sRequest = reqText.Substring(0, iStartPos - 1);

							//Replace backslash with Forward Slash, if Any
							sRequest.Replace("\\", "/");

							//If file name is not supplied add forward slash to indicate 
							//that it is a directory and then we will look for the 
							//default file name..
							if ((sRequest.IndexOf(".") < 1) && (!sRequest.EndsWith("/")))
							{
								sRequest = sRequest + "/";
							}

							//Extract the requested file name
							iStartPos = sRequest.LastIndexOf("/") + 1;
							string sRequestedFile = sRequest.Substring(iStartPos);

							//Extract The directory Name
							string sDirName = sRequest.Substring(sRequest.IndexOf("/"), sRequest.LastIndexOf("/") - 3);

							handler = FindHandler(sDirName);
						}

						ForwardToHandler(handler, socket);

						///////////////////////////////////////////////////////////////////////
						//// Identify the Physical Directory
						///////////////////////////////////////////////////////////////////////
						//if (sDirName == "/")
						//    sLocalDir = sMyWebServerRoot;
						//else
						//    //Get the Virtual Directory
						//    sLocalDir = GetLocalPath(sMyWebServerRoot, sDirName);
						//Console.WriteLine("Directory Requested : " + sLocalDir);
						////If the physical directory does not exists then
						//// dispaly the error message
						//if (sLocalDir.Length == 0)
						//{
						//    sErrorMessage = "<H2>Error!! Requested Directory does not exists</H2><Br>";
						//    //sErrorMessage = sErrorMessage + "Please check data\\Vdirs.Dat";
						//    //Format The Message
						//    SendHeader(sHttpVersion, "", sErrorMessage.Length, " 404 Not Found", ref socket);
						//    //Send to the browser
						//    SendToBrowser(sErrorMessage, ref socket);
						//    socket.Close();
						//    continue;
						//}
						///////////////////////////////////////////////////////////////////////
						//// Identify the File Name
						///////////////////////////////////////////////////////////////////////
						////If The file name is not supplied then look in the default file list
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
						///////////////////////////////////////////////////////////////////////
						//// Get TheMime Type
						///////////////////////////////////////////////////////////////////////
						//String sMimeType = GetMimeType(sRequestedFile);
						////Build the physical path
						//sPhysicalFilePath = sLocalDir + sRequestedFile;
						//Console.WriteLine("File Requested : " + sPhysicalFilePath);
						//if (File.Exists(sPhysicalFilePath) == false)
						//{
						//    sErrorMessage = "<H2>404 Error! File Does Not Exists...</H2>";
						//    SendHeader(sHttpVersion, "", sErrorMessage.Length, " 404 Not Found", ref socket);
						//    SendToBrowser(sErrorMessage, ref socket);
						//    Console.WriteLine(sFormattedMessage);
						//}
						//else
						//{
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
						//    //mySocket.Send(bytes, bytes.Length,0);
						//}
						socket.Close();
					}
				}
				Thread.Sleep(50);
			}
		}
		private static string GetRequestText(Socket socket)
		{
			byte[] bytes = new byte[1024];
			socket.Receive(bytes, bytes.Length, 0);
			return Encoding.ASCII.GetString(bytes);
		}

		private static bool ValidateHttpVerb(/*Socket socket, */string sBuffer)
		{
			//At present we will only deal with GET type
			if (sBuffer.Substring(0, 3) != "GET")
			{
				Console.WriteLine("Requested HTTP verb not supported.");
				//socket.Close();				
				return false;
			}

			return true;
		}

		private IMountPointHandler FindHandler(string requestedVirtualDir)
		{
			return MountPoints[requestedVirtualDir];
		}

		private void ForwardToHandler(IMountPointHandler handler, Socket socket)
		{
			var request = new Request();
			var response = new Response();
			handler.Respond(request, response);
			SendResponse(response, socket);
		}

		private void SendResponse(Response response, Socket socket)
		{
			//TODO: send headers
			string body = response.GetResponseBodyText();
			socket.Send(Encoding.ASCII.GetBytes(body));
			//var wr = new StreamWriter(socket.)
		}
	}
}
