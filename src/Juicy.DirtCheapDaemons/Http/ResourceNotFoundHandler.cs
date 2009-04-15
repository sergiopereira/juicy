using System.Net;

namespace Juicy.DirtCheapDaemons.Http
{
	public class ResourceNotFoundHandler : IMountPointHandler
	{
		public void Respond(IRequest request, IResponse response)
		{
			response.StatusCode = HttpStatusCode.NotFound;
			response.StatusMessage = "Resource not found";
			response.Output.WriteLine("<h1>404: The resource <i>{0}</i> could not be found.</h1>",
									  request.VirtualPath);
		}
	}
}
