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
            Headers = new Dictionary<string, string>();
        }

        public IDictionary<string, string> Headers { get; private set; }
        public MountPoint MountPoint { get; set; }
		public string VirtualPath { get; set; }
		//public string FullPath { get; set; }
		//public Uri Uri { get; set; }
    }


}
