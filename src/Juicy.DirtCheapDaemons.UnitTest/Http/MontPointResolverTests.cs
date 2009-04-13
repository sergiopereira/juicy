using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Juicy.DirtCheapDaemons.Http;
using NUnit.Framework;

namespace Juicy.DirtCheapDaemons.UnitTest.Http
{
	[TestFixture]
	public class MontPointResolverTests
	{
		[Test]
		public void ShouldFindWithoutExtension()
		{
			//finds without the trailing slash
			Assert.IsTrue(MountPointResolver.MountContainsPath(
								new MountPoint { Handler = null, VirtualPath = "/dir/subdir" },
								"/dir/subdir"));

			//finds with the trailing slash
			Assert.IsTrue(MountPointResolver.MountContainsPath(
								new MountPoint { Handler = null, VirtualPath = "/dir/subdir" },
								"/dir/subdir/"));
		}

		[Test]
		public void ShouldFindForFileWithExtension()
		{
			Assert.IsTrue(MountPointResolver.MountContainsPath(
					new MountPoint { Handler = null, VirtualPath = "/dir/subdir" },
					"/dir/subdir/file.ext"));
		}

		[Test]
		public void ShouldNotFindForPartialDirectoryNameMatches()
		{
			Assert.IsFalse(MountPointResolver.MountContainsPath(
					new MountPoint { Handler = null, VirtualPath = "/dir/name" },
					"/dir/nameother"));
		}

		[Test]
		public void ShouldFindSubDirPath()
		{
			Assert.IsTrue(MountPointResolver.MountContainsPath(
							new MountPoint { Handler = null, VirtualPath = "/dir/name" },
							"/dir/name/child/file.ext"));
		}

		[Test]
		public void ShouldFindTheMostSpecificMatch()
		{
			var resolver = new MountPointResolver();
			var mounts = new List<MountPoint>();
			mounts.Add(new MountPoint { Handler = null, VirtualPath = "/dir" });
			mounts.Add(new MountPoint { Handler = null, VirtualPath = "/dir/subdir" });
			mounts.Add(new MountPoint { Handler = null, VirtualPath = "/dir/subdir/subsub" });
	
			MountPoint mount = resolver.Resolve(mounts, "/dir/subdir");
			Assert.IsNotNull(mount);
			Assert.AreEqual("/dir/subdir", mount.VirtualPath);

			mount = resolver.Resolve(mounts, "/dir/subdir/file.ext");
			Assert.IsNotNull(mount);
			Assert.AreEqual("/dir/subdir", mount.VirtualPath);
		}

		[Test]
		public void ShouldReturnNullWhenNotFound()
		{
			var resolver = new MountPointResolver();
			var mounts = new List<MountPoint>();
			mounts.Add(new MountPoint { Handler = null, VirtualPath = "/dir" });
			MountPoint mount = resolver.Resolve(mounts, "/non-existing-path");
				Assert.IsNull(mount);			
		}
	}
}
