using System.Collections.Generic;
using System.Collections.Specialized;

namespace Juicy.DirtCheapDaemons.Http
{
	public class Request : IRequest
	{

		public Request()
		{
			Headers = new Dictionary<string, string>();
			QueryString = new NameValueCollection();
		}

		public Dictionary<string, string> Headers { get; private set; }
		public string this[string headerName] { get { return Headers[headerName]; } set { Headers[headerName] = value; } }
		public MountPoint MountPoint { get; set; }
		public string VirtualPath { get; set; }
		public NameValueCollection QueryString { get; set; }
	}
}
