using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Juicy.DirtCheapDaemons.Http
{
	public interface IMountPointHandler
	{
		void Respond(IRequest request, IResponse response);

	}
}
