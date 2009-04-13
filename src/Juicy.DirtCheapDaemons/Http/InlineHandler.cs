using System;
using System.Collections.Generic;

namespace Juicy.DirtCheapDaemons.Http
{
	public class InlineHandler : IMountPointHandler
	{
		public InlineHandler(Action<IRequest, IResponse> handlerMethod)
		{
			HandlerMethod = handlerMethod;
		}
		public Action<IRequest, IResponse> HandlerMethod { get; private set; }

		public void Respond(IRequest request, IResponse response)
		{
			HandlerMethod(request, response);
		}
	}
}
