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
			string fileName = VirtualPathUtility.GetFileName(request.VirtualPath);
			var path = Path.Combine(PhysicalDirectory, fileName);

			using (var file = new StreamReader(path))
			{
				response.Output.Write(file.ReadToEnd());
			}
		}

	}
}