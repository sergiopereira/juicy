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
