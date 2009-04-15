using System;
using NUnit.Framework;
using Juicy.DirtCheapDaemons.Http;

namespace Juicy.DirtCheapDaemons.UnitTest.Http
{
	[TestFixture]
	public class InlineHandlerTests
	{
		[Test]
		public void ShouldCallLambdaWithReqResp()
		{
			IRequest req = new Request();
			IResponse resp = new Response();
			bool called = false;

			var h = new InlineHandler((request, response) =>
				{
					Assert.AreSame(req, request);
					Assert.AreSame(resp, response);
					called = true;
				});

			h.Respond(req, resp);
			Assert.IsTrue(called);
		}
	}
}
