
namespace Juicy.DirtCheapDaemons.Http
{
	public class PingHandler : IMountPointHandler
	{
		public static bool IsPing(string requestText)
		{
			return requestText.Contains("PING");
		}


		public void Respond(IRequest request, IResponse response)
		{
			response.Output.Write("PONG");
		}

	}
}
