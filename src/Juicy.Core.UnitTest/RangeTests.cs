using System;
using System.Collections.Generic;
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

		[Test]
		public void ShouldNotEnumerateCollapsedRanges()
		{
			var ints = new IntRange(5, 4).ToList();
			Assert.AreEqual(0, ints.Count);
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
