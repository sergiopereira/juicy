using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;

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
		public IDictionary<string,string> Headers { get; private set; }
		public int StatusCode { get; set; }
		public string StatusMessage { get; set; }
		public string GetResponseBodyText()
		{
			return ((StringWriter)Output).GetStringBuilder().ToString();
		}

	}
}
