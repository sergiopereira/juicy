using System;
using System.Collections.Generic;

namespace Juicy.DirtCheapDaemons.Http
{
	public interface IRequest
	{
        MountPoint MountPoint { get;} 
		string VirtualPath { get; }
        IDictionary<string, string> Headers { get; }
        //string FullPath { get; }
		//Uri Uri { get;  }
	}
}
