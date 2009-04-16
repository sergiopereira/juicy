using System;
using System.Collections.Generic;
using System.Net;
using System.Web;

namespace Juicy.DirtCheapDaemons.Http
{
	public class EmptyHttpResponseHandler : IMountPointHandler
	{
		public HttpStatusCode StatusCode { get; set; }
		public string Message { get; set; }

		public EmptyHttpResponseHandler(HttpStatusCode statusCode, string message)
		{
			StatusCode = statusCode;
			Message = message;
		}

		public void Respond(IRequest request, IResponse response)
		{
			response.StatusCode = StatusCode;
			response.StatusMessage = Message;
			response.Output.WriteLine("<h1>{0}: {1}</h1>",
				(int)StatusCode, HttpUtility.HtmlEncode(Message));
		}
	}
}
