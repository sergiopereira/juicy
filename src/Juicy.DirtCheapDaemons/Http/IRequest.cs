using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Juicy.DirtCheapDaemons.Http
{
	public interface IRequest
	{
		MountPoint MountPoint { get; }
		string VirtualPath { get; }
		string PostBody { get; set; }
		string this[string headerName] { get; set; }
		IDictionary<string, string> Headers { get; }
		IDictionary<string, string> QueryString { get; }
		IDictionary<string, string> Form { get; }
	}
}
