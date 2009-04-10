using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;

namespace Juicy.DirtCheapDaemons.Http
{
    public class Request : IRequest
	{
		public Request()
		{
		}

		public string VirtualPath { get; set; }
		public string FullPath { get; set; }
		public Uri Uri { get; set; }

	}


}
