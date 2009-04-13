using System;
using System.Collections.Generic;
using NUnit.Framework;
using Juicy.DirtCheapDaemons.Http;

namespace Juicy.DirtCheapDaemons.UnitTest.Http
{
    [TestFixture]
    public class StaticFileHandlerTests
    {
        [Test]
        public void ShouldResolvePathForMountedDirectory()
        {
            var req = new Request();
            req.MountPoint = new MountPoint {VirtualPath = "/vdir"};
            req.VirtualPath = "/vdir";
            var handler = new StaticFileHandler(@"c:\webroot");
            Assert.AreEqual(@"c:\webroot", handler.FindRequestedPhysicalPath(req));
            //should be tolerant to trailing backslashes
            handler = new StaticFileHandler(@"c:\webroot\");
            Assert.AreEqual(@"c:\webroot", handler.FindRequestedPhysicalPath(req));

        }

        [Test]
        public void ShouldResolvePathForFileInChildDirectory()
        {
            var req = new Request();
            req.MountPoint = new MountPoint {VirtualPath = "/vdir"};
            req.VirtualPath = "/vdir/dir/file.html";
            var handler = new StaticFileHandler(@"c:\webroot");
            Assert.AreEqual(@"c:\webroot\dir\file.html", handler.FindRequestedPhysicalPath(req));
            //should be tolerant to trailing backslashes
            handler = new StaticFileHandler(@"c:\webroot\");
            Assert.AreEqual(@"c:\webroot\dir\file.html", handler.FindRequestedPhysicalPath(req));
        }

        

    }
}
