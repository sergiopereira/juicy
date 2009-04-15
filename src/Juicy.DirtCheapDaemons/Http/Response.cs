using System;
using System.Collections.Generic;
using System.Net;
using System.IO;

namespace Juicy.DirtCheapDaemons.Http
{
	public class Response : IResponse
	{

		public Response()
		{
			Output = new StringWriter();
			Headers = new Dictionary<string, string>();
		}

		public TextWriter Output { get; private set; }
		public IDictionary<string, string> Headers { get; private set; }
		public string this[string headerName] { get { return Headers[headerName]; } set { Headers[headerName] = value; } }
		public HttpStatusCode StatusCode { get; set; }
		public string StatusMessage { get; set; }
		public string GetResponseBodyText()
		{
			return ((StringWriter)Output).GetStringBuilder().ToString();
		}

	}
}
