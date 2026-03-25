using System;

namespace Nintenlord.Utility
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class LazyHelpers
    {
        public static Lazy<TResult> SelectWhere<TSource, TLazy, TResult>(
            this Lazy<TSource> source,
            Func<TSource, Lazy<TLazy>> lazySelector,
            Func<TSource, TLazy, TResult> resultSelector) =>
            new(() => resultSelector(source.Value, lazySelector(source.Value).Value));

        public static Lazy<TResult> Select<TSource, TResult>(this Lazy<TSource> source,
            Func<TSource, TResult> selector) =>
            new(() => selector(source.Value));
    }
}
