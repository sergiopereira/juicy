using System;
using System.Collections.Generic;
using System.Linq;

namespace Juicy
{
	public abstract class RangeBase<T> : IEnumerable<T> where T : IComparable
	{
		public T Start { get; set; }
		public T End { get; set; }

		protected RangeBase(T start, T end)
		{
			if (start.CompareTo(end) > 0)
			{
				throw new ArgumentOutOfRangeException("end", "The start value of the range must not be greater than its end value.");
			}

			Start = start;
			End = end;
		}

		protected abstract T GetNextItem(T currentItem);

		public IEnumerator<T> GetEnumerator()
		{
			T item = Start;

			while (true)
			{

				if (item.CompareTo(End) > 0)
					break;

				yield return item;

				item = GetNextItem(item);
			}
		}


		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public bool Intersects(RangeBase<T> otherRange)
		{
			if (otherRange == null)
			{
				return false;
			}

			return ((otherRange.Start.CompareTo(this.Start) >= 0) && (otherRange.Start.CompareTo(this.End) <= 0))
				|| ((otherRange.End.CompareTo(this.Start) >= 0) && (otherRange.End.CompareTo(this.End) <= 0));
		}

		public bool Contains(RangeBase<T> otherRange)
		{
			if (otherRange == null)
			{
				return false;
			}

			return (otherRange.Start.CompareTo(this.Start) >= 0) && (otherRange.End.CompareTo(this.End) <= 0);
		}

		public bool Contains(T element)
		{
			return (this.Start.CompareTo(element) <= 0) && (this.End.CompareTo(element) >= 0);
		}

	}

	public class Range<T> : RangeBase<T> where T : IComparable
	{
		public Func<T, T> GetNext { get; private set; }

		public Range(T start, T end, Func<T, T> getNext)
			: base(start, end)
		{
			GetNext = getNext;
		}

		protected override T GetNextItem(T currentItem)
		{
			return GetNext(currentItem);
		}
	}

	public class Int32Range : RangeBase<int>
	{
		public Int32Range(int start, int end) : base(start, end) { }
		protected override int GetNextItem(int currentItem) { return currentItem + 1; }
	}

	public class Int64Range : RangeBase<long>
	{
		public Int64Range(long start, long end) : base(start, end) { }
		protected override long GetNextItem(long currentItem) { return currentItem + 1; }
	}

	public class Int16Range : RangeBase<short>
	{
		public Int16Range(short start, short end) : base(start, end) { }
		protected override short GetNextItem(short currentItem) { return (short)(currentItem + 1); }
	}

	public class ByteRange : RangeBase<byte>
	{
		public ByteRange(byte start, byte end) : base(start, end) { }
		protected override byte GetNextItem(byte currentItem) { return (byte)(currentItem + 1); }
	}

	public class SecondRange : RangeBase<DateTime>
	{
		public SecondRange(DateTime start, DateTime end) : base(start, end) { }
		protected override DateTime GetNextItem(DateTime currentItem) { return currentItem.AddSeconds(1); }
	}

	public class MinuteRange : RangeBase<DateTime>
	{
		public MinuteRange(DateTime start, DateTime end) : base(start, end) { }
		protected override DateTime GetNextItem(DateTime currentItem) { return currentItem.AddMinutes(1); }
	}

	public class HourRange : RangeBase<DateTime>
	{
		public HourRange(DateTime start, DateTime end) : base(start, end) { }
		protected override DateTime GetNextItem(DateTime currentItem) { return currentItem.AddHours(1); }
	}

	public class DayRange : RangeBase<DateTime>
	{
		public DayRange(DateTime start, DateTime end) : base(start, end) { }
		protected override DateTime GetNextItem(DateTime currentItem) { return currentItem.AddDays(1); }
	}

	public class WeekRange : RangeBase<DateTime>
	{
		public WeekRange(DateTime start, DateTime end) : base(start, end) { }
		protected override DateTime GetNextItem(DateTime currentItem) { return currentItem.AddDays(7); }
	}

	public class MonthRange : RangeBase<DateTime>
	{
		public MonthRange(DateTime start, DateTime end) : base(start, end) { }
		protected override DateTime GetNextItem(DateTime currentItem) { return currentItem.AddMonths(1); }
	}

	public class YearRange : RangeBase<DateTime>
	{
		public YearRange(DateTime start, DateTime end) : base(start, end) { }
		protected override DateTime GetNextItem(DateTime currentItem) { return currentItem.AddYears(1); }
	}

	

}
