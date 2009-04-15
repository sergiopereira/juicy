using NUnit.Framework;

namespace Juicy.Core.UnitTest
{
	[TestFixture]
	public class ResourceFinderTests
	{
		[Test]
		public void ShouldFindResourceInResourcesNamespace()
		{
			var finder = new ResourceFinder(this.GetType().Assembly, this.GetType().Namespace + ".Resources");
			Assert.IsTrue(finder.Exists("TextFile.txt"));
			Assert.IsFalse(finder.Exists("Bogus.txt"));
		}

		[Test]
		public void ShouldFindResourceUsingTypeInSameNamespace()
		{
			var finder = new ResourceFinder(typeof(Resources.SomeClass));
			Assert.IsTrue(finder.Exists("TextFile.txt"));
			Assert.IsFalse(finder.Exists("Bogus.txt"));
		}

		[Test]
		public void ShouldGetResourceStream()
		{
			var finder = new ResourceFinder(typeof(Resources.SomeClass));
			using (var s = finder.GetStream("TextFile.txt"))
			{
				Assert.IsNotNull(s);
			}
			using (var s = finder.GetStream("Bogus.txt"))
			{
				Assert.IsNull(s);
			}
		}

		[Test]
		public void ShouldGetResourceText()
		{
			var finder = new ResourceFinder(typeof(Resources.SomeClass));
			Assert.AreEqual("test file", finder.GetText("TextFile.txt"));

		}

		[Test]
		public void ShouldGetResourceBytes()
		{
			var finder = new ResourceFinder(typeof(Resources.SomeClass));
			byte[] resBytes = finder.GetBytes("TextFile.txt");
			Assert.AreEqual(9, resBytes.Length);
			Assert.AreEqual((byte)'t', resBytes[0]);
			Assert.AreEqual((byte)'e', resBytes[1]);
			Assert.AreEqual((byte)'s', resBytes[2]);
			Assert.AreEqual((byte)'t', resBytes[3]);
			Assert.AreEqual((byte)' ', resBytes[4]);
			Assert.AreEqual((byte)'f', resBytes[5]);
			Assert.AreEqual((byte)'i', resBytes[6]);
			Assert.AreEqual((byte)'l', resBytes[7]);
			Assert.AreEqual((byte)'e', resBytes[8]);
		}
	}
}
