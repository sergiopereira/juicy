using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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
        public void ShouldReturnTheServerRootUrl()
        {
            _server.PortNumber = 8123;
            //_server.Start();

            string url = "http://localhost:8123/";  
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

        private static string GetResponseBodyFromUrl(string url)
        {
            var request = WebRequest.Create(url);
            using (var response = request.GetResponse())
            using (var sr = new StreamReader(response.GetResponseStream())) {
                return sr.ReadToEnd();
            }

        }
		private static HttpStatusCode GetResponseStatusCode(string url)
		{
			var request = WebRequest.Create(url);
			using (var response = (HttpWebResponse) request.GetResponse())
			{
				//using (var sr = new StreamReader(response.GetResponseStream())) {
				return response.StatusCode;
			}
		}
    }
}
