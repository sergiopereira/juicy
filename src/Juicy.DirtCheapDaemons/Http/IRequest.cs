using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Juicy.DirtCheapDaemons.Http
{
	public interface IRequest
	{
        MountPoint MountPoint { get;} 
		string VirtualPath { get; }
        IDictionary<string, string> Headers { get; }
		NameValueCollection QueryString { get; }
	}
}
