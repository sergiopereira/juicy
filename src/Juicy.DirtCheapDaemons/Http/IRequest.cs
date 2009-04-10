using System;

namespace Juicy.DirtCheapDaemons.Http
{
	public interface IRequest
	{
		string VirtualPath { get; set; }
		string FullPath { get; set; }
		Uri Uri { get; set; }
	}
}
