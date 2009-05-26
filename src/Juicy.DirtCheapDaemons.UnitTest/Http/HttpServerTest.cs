using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Juicy.DirtCheapDaemons.Http;
using NUnit.Framework;
using System.Threading;

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

		[Test]
		public void ShouldAcceptPostRequestsWithUrlEncodedValues()
		{
			_server.Start();
			const string text = "response here";
			string key1 = null, key2 = null;

			_server.Mount("/testdir", (i, o) =>
				{
					//key1 = i.Form["key1"];
					//key2 = i.Form["key2"];
					o.Output.Write(text);
				});

			string url = _server.RootUrl + "testdir";
			
			Assert.AreEqual(text, GetResponseBodyFromUrlViaPost(url, 
				"key1=val1&key2=val2", 
				"application/x-www-form-urlencoded"));
			
			Assert.AreEqual("val1", key1);
			Assert.AreEqual("val2", key2);

		}

		[Test]
		public void ShouldAcceptPostRequestsWithSimplePostBody()
		{
			_server.Start();
			//const string text = "response here";
			_server.Mount("/testdir", (i, o) => o.Output.Write(i.PostBody));

			string url = _server.RootUrl + "testdir";
			string body = "line1\r\nline2\r\nline3";
			
			Assert.AreEqual(body, GetResponseBodyFromUrlViaPost(url, body, null));

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

		private static string GetResponseBodyFromUrlViaPost(string url, string postBody, string contentType)
		{
			byte[] buffer = Encoding.UTF8.GetBytes(postBody);
			var request = WebRequest.Create(url);
			request.Method = "POST";
			//ServicePointManager.Expect100Continue = false;

			if(!string.IsNullOrEmpty(contentType))
			{
				request.ContentType = contentType;
			}

			request.ContentLength = buffer.Length;

			var stream = request.GetRequestStream();
			
			stream.Write(buffer, 0, buffer.Length);
			stream.Flush();
			stream.Close();
		

			using (var response = request.GetResponse())
			using (var sr = new StreamReader(response.GetResponseStream()))
			{
				return sr.ReadToEnd();
			}
		}

		private static HttpStatusCode GetResponseStatusCodeViaPost(string url, string postBody, string contentType)
		{
			var request = WebRequest.Create(url);
			request.Method = "POST";
			request.ContentType = contentType;
			request.ContentLength = postBody.Length;
			byte[] buffer = Encoding.ASCII.GetBytes(postBody);
			request.GetRequestStream().Write(buffer, 0, buffer.Length);

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
