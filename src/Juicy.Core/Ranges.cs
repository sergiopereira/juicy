using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Juicy
{
    public abstract class RangeBase<T> : IEnumerable<T> where T:IComparable
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

			while (true) {

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

        public Range(T start, T end, Func<T,T> getNext) : base(start, end)
        {
            GetNext = getNext;
        }

        protected override T GetNextItem(T currentItem)
        {
            return GetNext(currentItem);
        }
    }

}
