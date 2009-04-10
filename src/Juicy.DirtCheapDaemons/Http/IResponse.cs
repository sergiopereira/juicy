using System.Collections.Generic;
using System.IO;

namespace Juicy.DirtCheapDaemons.Http
{
	public interface IResponse
	{
		TextWriter Output { get;  }
		IDictionary<string, string> Headers { get; }
		int StatusCode { get; set; }
		string StatusMessage { get; set; }
	}
}
