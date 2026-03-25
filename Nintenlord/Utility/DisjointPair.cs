using System;

namespace Nintenlord.Utility
{
    public sealed class DisjointPair<T1, T2>
    {
        private readonly bool hasFirst;
        private readonly T1 first;
        private readonly T2 second;

        public bool HasFirst => hasFirst;

        public bool HasSecond => !hasFirst;

        public T1 First
        {
            get
            {
                if (!hasFirst)
                    throw new InvalidOperationException();

                return first;
            }
        }
        public T2 Second
        {
            get
            {
                if (hasFirst)
                    throw new InvalidOperationException();

                return second;
            }
        }

        public DisjointPair(T1 item)
        {
            hasFirst = true;
            first = item;
            second = default(T2);
        }

        public DisjointPair(T2 item)
        {
            hasFirst = false;
            first = default(T1);
            second = item;
        }

        public static implicit operator DisjointPair<T1, T2>(T1 item) => new(item);

        public static implicit operator DisjointPair<T1, T2>(T2 item) => new(item);

        public static explicit operator T1(DisjointPair<T1, T2> item) => item.First;

        public static explicit operator T2(DisjointPair<T1, T2> item) => item.Second;

        public void Apply(Action<T1> first, Action<T2> second)
        {
            if (hasFirst)
            {
                first(this.first);
            }
            else
            {
                second(this.second);
            }
        }

        public T Apply<T>(Func<T1, T> first, Func<T2, T> second) => hasFirst ? first(this.first) : second(this.second);
    }
}
