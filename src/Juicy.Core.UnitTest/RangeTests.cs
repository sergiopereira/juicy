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
			var ints = new EvenIntRangeMock(6, 10).ToList();
			Assert.AreEqual(3, ints.Count);
			Assert.AreEqual(6, ints[0]);
			Assert.AreEqual(8, ints[1]);
			Assert.AreEqual(10, ints[2]);
		}

		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void ShouldNotAllowInvertedRanges()
		{
			new EvenIntRangeMock(5, 4).ToList();
		}

		[Test]
		public void ShouldEnumerateRangeOfOne()
		{
			var ints = new EvenIntRangeMock(10, 10).ToList();
			Assert.AreEqual(1, ints.Count);
			Assert.AreEqual(10, ints[0]);
		}


		[Test]
		public void ShouldEnumerateWithLambda()
		{
			var rangeOfDates = new Range<DateTime>(
				DateTime.Today, 
				DateTime.Today.AddHours(1), d => d.AddMinutes(15));
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
			var range1 = new EvenIntRangeMock(5, 7);

			Assert.IsTrue(range1.Intersects(new EvenIntRangeMock(4, 6)));
			Assert.IsTrue(range1.Intersects(new EvenIntRangeMock(6, 8)));
			Assert.IsTrue(range1.Intersects(new EvenIntRangeMock(5, 7)));
			Assert.IsTrue(range1.Intersects(new EvenIntRangeMock(5, 5)));
			Assert.IsTrue(range1.Intersects(new EvenIntRangeMock(6, 6)));
			Assert.IsTrue(range1.Intersects(new EvenIntRangeMock(7, 7)));

			Assert.IsFalse(range1.Intersects(null));
			Assert.IsFalse(range1.Intersects(new EvenIntRangeMock(3, 4)));
			Assert.IsFalse(range1.Intersects(new EvenIntRangeMock(8, 9)));
		}

		[Test]
		public void ShouldDetectRangeContainment()
		{
			var range1 = new EvenIntRangeMock(10, 20);
			Assert.IsTrue(range1.Contains(new EvenIntRangeMock(10, 20)));
			Assert.IsTrue(range1.Contains(new EvenIntRangeMock(15, 16)));
			Assert.IsTrue(range1.Contains(new EvenIntRangeMock(10, 10)));
			Assert.IsTrue(range1.Contains(new EvenIntRangeMock(20, 20)));

			Assert.IsFalse(range1.Contains(null));
			Assert.IsFalse(range1.Contains(new EvenIntRangeMock(9, 11)));
			Assert.IsFalse(range1.Contains(new EvenIntRangeMock(19, 21)));
			Assert.IsFalse(range1.Contains(new EvenIntRangeMock(9, 21)));
		}

		[Test]
		public void ShouldDetectItemContainment()
		{
			var range1 = new EvenIntRangeMock(10, 20);
			Assert.IsTrue(range1.Contains(10));
			Assert.IsTrue(range1.Contains(15));
			Assert.IsTrue(range1.Contains(20));

			Assert.IsFalse(range1.Contains(9));
			Assert.IsFalse(range1.Contains(21));
		}

		[Test]
		public void NumberRangesShouldEnumerate()
		{
			CollectionAssert.AreEqual(new byte[]{5,6 ,7}, new ByteRange(5, 7).ToArray());
			CollectionAssert.AreEqual(new Int16[]{5,6 ,7}, new Int16Range(5, 7).ToArray());
			CollectionAssert.AreEqual(new Int32[]{5,6 ,7}, new Int32Range(5, 7).ToArray());
			CollectionAssert.AreEqual(new Int64[]{5,6 ,7}, new Int64Range(5, 7).ToArray());
		}

		[Test]
		public void DateRangesShouldEnumerate()
		{
			var d = DateTime.Now;

			CollectionAssert.AreEqual(
				new[]{d, d.AddSeconds(1), d.AddSeconds(2)}, 
				new SecondRange(d, d.AddSeconds(2)).ToArray());

			CollectionAssert.AreEqual(
				new[] { d, d.AddMinutes(1), d.AddMinutes(2) },
				new MinuteRange(d, d.AddMinutes(2)).ToArray());

			CollectionAssert.AreEqual(
				new[]{d, d.AddHours(1), d.AddHours(2)}, 
				new HourRange(d, d.AddHours(2)).ToArray());

			CollectionAssert.AreEqual(
				new[]{d, d.AddDays(1), d.AddDays(2)}, 
				new DayRange(d, d.AddDays(2)).ToArray());

			CollectionAssert.AreEqual(
				new[]{d, d.AddMonths(1), d.AddMonths(2)}, 
				new MonthRange(d, d.AddMonths(2)).ToArray());

			CollectionAssert.AreEqual(
				new[]{d, d.AddYears(1), d.AddYears(2)}, 
				new YearRange(d, d.AddYears(2)).ToArray());

			CollectionAssert.AreEqual(
				new[]{d, d.AddDays(7), d.AddDays(14)}, 
				new WeekRange(d, d.AddDays(14)).ToArray());
		}
	}



	public class EvenIntRangeMock : RangeBase<int>
	{
		public EvenIntRangeMock(int start, int end) : base(start, end) { }

		protected override int GetNextItem(int currentItem)
		{
			return currentItem + 2;
		}
	}

}
