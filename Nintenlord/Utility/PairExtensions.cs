using System;
using System.Collections.Generic;

namespace Nintenlord.Utility
{
    public static class PairExtensions
    {
        public static TResult Apply<T1, T2, TResult>(this KeyValuePair<T1, T2> item,
            Func<T1, T2, TResult> f) =>
            f(item.Key, item.Value);

        public static bool Equals<T1, T2>(this KeyValuePair<T1, T2> item, T2 toCompare) => EqualityComparer<T2>.Default.Equals(toCompare, item.Value);

        public static bool Equals<T1, T2>(this KeyValuePair<T1, T2> item, T1 toCompare) => EqualityComparer<T1>.Default.Equals(toCompare, item.Key);
    }
}
