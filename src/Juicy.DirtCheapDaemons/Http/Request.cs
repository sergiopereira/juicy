using System.Collections.Generic;
using System.Collections.Specialized;

namespace Juicy.DirtCheapDaemons.Http
{
	public class Request : IRequest
	{

		public Request()
		{
			Headers = new Dictionary<string, string>();
			QueryString = new Dictionary<string, string>();
			Form = new Dictionary<string, string>();
		}

		public string this[string headerName] { get { return Headers[headerName]; } set { Headers[headerName] = value; } }
		public MountPoint MountPoint { get; set; }
		public string VirtualPath { get; set; }
		public string PostBody { get; set; }

		public IDictionary<string, string> Headers { get; private set; }
		public IDictionary<string, string> QueryString { get; private set; }
		public IDictionary<string, string> Form { get; private set; }
	}
}
