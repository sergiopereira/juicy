using NUnit.Framework;
using Juicy.DirtCheapDaemons.Http;
using System;

namespace Juicy.DirtCheapDaemons.UnitTest.Http
{
	[TestFixture]
	public class RequestFactoryTests
	{
		[Test]
		public void ShouldReturnRequestInstance()
		{
			var req = new RequestFactory().Create(new string[0], null, "");
			Assert.IsNotNull(req);
		}

		[Test, ExpectedException(typeof(ArgumentNullException))]
		public void ShouldErrorOnMissingPath()
		{
			new RequestFactory().Create(new string[0], null, null);
		}

		[Test]
		public void ShouldParseHeaders()
		{
			var lines = new[]
			            	{
			            		"(doesn't matter)",
                                "header1: value1",
                                "header2: value2"
			            	};

			var req = new RequestFactory().Create(lines, null, "");
			Assert.AreEqual("value1", req["header1"]);
			Assert.AreEqual("value1", req["header1"]);
			Assert.AreEqual(2, req.Headers.Count);
		}

		[Test]
		public void ShouldParseQueryString()
		{
			var req = new RequestFactory().Create(new string[0], null, "/file.ext?key1=value+1&key2=value+2");
			Assert.AreEqual("value 1", req.QueryString["key1"]);
			Assert.AreEqual("value 2", req.QueryString["key2"]);
			Assert.AreEqual(2, req.QueryString.Count);
		}

		[Test]
		public void ShouldParseFormValues()
		{
			var lines = new[]
			            	{
                                "(not important, the HTTP command",
			            		"Content-Type: application/x-www-form-urlencoded",
                                "", //separate headers from posted values
                                "key1=value+1&key2=value+2"
			            	};

			var req = new RequestFactory().Create(lines, null, "");
			Assert.AreEqual("value 1", req.Form["key1"]);
			Assert.AreEqual("value 2", req.Form["key2"]);
			Assert.AreEqual(2, req.Form.Count);
		}

	}
}
