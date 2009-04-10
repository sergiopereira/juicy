using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;

namespace Juicy.DirtCheapDaemons.Http.Http
{
    public class MountPoint
	{
		public IMountPointHandler Handler { get; set; }
		public string VirtualPath { get; set; }
	}
}
