using System;
using System.Linq;
using NUnit.Framework;

namespace Juicy.Core.UnitTest
{
	[TestFixture]
	public class RangeTests
	{
		[Test]
		public void ShouldEnumerateTheRange()
		{
			var ints = new IntRange(5, 7).ToList();
			Assert.AreEqual(3, ints.Count);
			Assert.AreEqual(5, ints[0]);
			Assert.AreEqual(6, ints[1]);
			Assert.AreEqual(7, ints[2]);
		}

		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void ShouldNotAllowCollapsedRanges()
		{
			var ints = new IntRange(5, 4).ToList();
		}

		[Test]
		public void ShouldEnumerateRangeOfOne()
		{
			var ints = new IntRange(10, 10).ToList();
			Assert.AreEqual(1, ints.Count);
			Assert.AreEqual(10, ints[0]);
		}


		[Test]
		public void ShouldEnumerateWithLambda()
		{
			var rangeOfDates = new Range<DateTime>(DateTime.Today, DateTime.Today.AddHours(1), d => d.AddMinutes(15));
			var dates = rangeOfDates.ToList();

			Assert.AreEqual(5, dates.Count);
			Assert.AreEqual(DateTime.Today, dates[0]);
			Assert.AreEqual(DateTime.Today.AddMinutes(15), dates[1]);
			Assert.AreEqual(DateTime.Today.AddMinutes(30), dates[2]);
			Assert.AreEqual(DateTime.Today.AddMinutes(45), dates[3]);
			Assert.AreEqual(DateTime.Today.AddMinutes(60), dates[4]);
		}

		[Test]
		public void ShouldDetectIntersections()
		{
			var range1 = new IntRange(5, 7);

			Assert.IsTrue(range1.Intersects(new IntRange(4, 6)));
			Assert.IsTrue(range1.Intersects(new IntRange(6, 8)));
			Assert.IsTrue(range1.Intersects(new IntRange(5, 7)));
			Assert.IsTrue(range1.Intersects(new IntRange(5, 5)));
			Assert.IsTrue(range1.Intersects(new IntRange(6, 6)));
			Assert.IsTrue(range1.Intersects(new IntRange(7, 7)));

			Assert.IsFalse(range1.Intersects(null));
			Assert.IsFalse(range1.Intersects(new IntRange(3, 4)));
			Assert.IsFalse(range1.Intersects(new IntRange(8, 9)));
		}

		[Test]
		public void ShouldDetectRangeContainment()
		{
			var range1 = new IntRange(10, 20);
			Assert.IsTrue(range1.Contains(new IntRange(10, 20)));
			Assert.IsTrue(range1.Contains(new IntRange(15, 16)));
			Assert.IsTrue(range1.Contains(new IntRange(10, 10)));
			Assert.IsTrue(range1.Contains(new IntRange(20, 20)));

			Assert.IsFalse(range1.Contains(null));
			Assert.IsFalse(range1.Contains(new IntRange(9, 11)));
			Assert.IsFalse(range1.Contains(new IntRange(19, 21)));
			Assert.IsFalse(range1.Contains(new IntRange(9, 21)));
		}

		[Test]
		public void ShouldDetectItemContainment()
		{
			var range1 = new IntRange(10, 20);
			Assert.IsTrue(range1.Contains(10));
			Assert.IsTrue(range1.Contains(15));
			Assert.IsTrue(range1.Contains(20));

			Assert.IsFalse(range1.Contains(9));
			Assert.IsFalse(range1.Contains(21));
		}
	}



	public class IntRange : RangeBase<int>
	{
		public IntRange(int start, int end) : base(start, end) { }

		protected override int GetNextItem(int currentItem)
		{
			return currentItem + 1;
		}

	}

}
