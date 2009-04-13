using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;

namespace Juicy.DirtCheapDaemons.Http
{
	public class StaticFileHandler : IMountPointHandler
	{
		public StaticFileHandler(string physicalDirectory)
		{
			PhysicalDirectory = physicalDirectory;
		}
		public string PhysicalDirectory { get; set; }

		public void Respond(IRequest request, IResponse response)
		{
		    var path = FindRequestedPhysicalPath(request);
			using (var file = new StreamReader(path))
			{
				response.Output.Write(file.ReadToEnd());
			}
		}

        public string FindRequestedPhysicalPath(IRequest request)
        {
            var vpath = request.VirtualPath.TrimEnd('/');
            var vdir = request.MountPoint.VirtualPath;

            if (vpath.Equals( vdir, StringComparison.OrdinalIgnoreCase))
                return PhysicalDirectory.TrimEnd('\\');
                
            if(vpath.StartsWith(vdir, StringComparison.OrdinalIgnoreCase))
            {
                var subPath = vpath.Substring(vdir.Length).TrimStart('/');
                var subPhysPath = subPath.Replace("/", @"\");
                return Path.Combine(PhysicalDirectory, subPhysPath);
            }

            throw new ArgumentOutOfRangeException("request", "How the heck did was this handler invoked? Bug.");
        }
	}
}