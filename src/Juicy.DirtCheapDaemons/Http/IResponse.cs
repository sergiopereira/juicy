using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Juicy.DirtCheapDaemons.Http
{
	public interface IResponse
	{
		TextWriter Output { get;  }
		IDictionary<string, string> Headers { get; }
		HttpStatusCode StatusCode { get; set; }
		string StatusMessage { get; set; }
	}
}
