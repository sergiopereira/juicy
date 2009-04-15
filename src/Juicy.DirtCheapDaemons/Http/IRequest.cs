using System;
using System.Collections.Specialized;

namespace Juicy.DirtCheapDaemons.Http
{
	public interface IRequest
	{
		MountPoint MountPoint { get; }
		string VirtualPath { get; }
		string this[string headerName] { get; }
		NameValueCollection QueryString { get; }
	}
}
