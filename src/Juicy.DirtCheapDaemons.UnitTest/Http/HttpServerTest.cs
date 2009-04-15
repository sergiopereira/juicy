using System;
using System.IO;
using System.Net;
using Juicy.DirtCheapDaemons.Http;
using NUnit.Framework;

namespace Juicy.DirtCheapDaemons.UnitTest.Http
{
	[TestFixture]
	public class HttpServerTest
	{
		private HttpServer _server;

		[SetUp]
		public void Setup()
		{
			_server = new HttpServer();
		}

		[TearDown]
		public void Teardown()
		{
			_server.UnmountAll();
			_server.Shutdown();
		}

		[Test]
		public void ShouldInitializeMountPointsOnNonDefaultCtor_BUG()
		{
			_server = new HttpServer(8181);
			Assert.IsNotNull(_server.MountPoints);
		}

		[Test]
		public void ShouldReturnTheServerRootUrl()
		{
			_server.PortNumber = 8123;
			const string url = "http://localhost:8123/";
			Assert.AreEqual(url, _server.RootUrl);
		}

		[Test]
		public void ShouldRespondWithDynamicContent()
		{
			_server.Start();
			const string text = "response here";
			_server.Mount("/testdir", (i, o) => o.Output.Write(text));
			string url = _server.RootUrl + "testdir";
			Assert.AreEqual(text, GetResponseBodyFromUrl(url));
		}

		[Test]
		public void ShouldUnmountVDir()
		{
			_server.Mount("/testdir", (i, o) => o.Output.Write(""));
			Assert.AreEqual(1, _server.MountPoints.Count);
			_server.Unmount("/testdir");
			Assert.AreEqual(0, _server.MountPoints.Count);
		}

		[Test, ExpectedException(typeof(InvalidOperationException))]
		public void ShouldNoAllowPortChangingAfterStart()
		{
			_server.Start();
			_server.PortNumber = 1234;
		}

		[Test]
		public void ShouldReturn404ForUnhandledPath()
		{
			//_server.Mount("/test", (req, resp) => resp.Output.WriteLine("not important"));
			_server.Start();

			var code = GetResponseStatusCode(_server.RootUrl + "some/crazy/path");
			Assert.AreEqual(HttpStatusCode.NotFound, code);
		}

		[Test]
		public void ShouldParseQueryString()
		{
			_server.Start();
			string p1 = null, p2 = null;
			int count = 0;
			_server.Mount("/testdir", (req, resp) =>
			{
				//I tried using Asserts here but it would lock on failures... 
				//  maybe a threading issue? Did not investigate yet.
				p1 = req.QueryString["p1"];
				p2 = req.QueryString["p2"];
				count = req.QueryString.Count;
			});

			var body = GetResponseBodyFromUrl(_server.RootUrl + "testdir?p1=val1&p2=val2");

			Assert.AreEqual("val1", p1);
			Assert.AreEqual("val2", p2);
			Assert.AreEqual(2, count);
		}



		private static string GetResponseBodyFromUrl(string url)
		{
			var request = WebRequest.Create(url);
			using (var response = request.GetResponse())
			using (var sr = new StreamReader(response.GetResponseStream()))
			{
				return sr.ReadToEnd();
			}

		}
		private static HttpStatusCode GetResponseStatusCode(string url)
		{
			var request = WebRequest.Create(url);
			try
			{
				using (var response = (HttpWebResponse)request.GetResponse())
				{
					return response.StatusCode;
				}
			}
			catch (WebException ex)
			{
				return ((HttpWebResponse)ex.Response).StatusCode;
			}
		}
	}
}
