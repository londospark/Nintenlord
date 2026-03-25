using System;
using System.Diagnostics.CodeAnalysis;

namespace Nintenlord.Utility
{
    public static class LazyHelpers
    {
        public static Lazy<TResult> SelectWhere<TSource, TLazy, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TResult>(
            this Lazy<TSource> source,
            Func<TSource, Lazy<TLazy>> lazySelector,
            Func<TSource, TLazy, TResult> resultSelector) =>
            new(() => resultSelector(source.Value, lazySelector(source.Value).Value));

        public static Lazy<TResult> Select<TSource, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TResult>(this Lazy<TSource> source,
            Func<TSource, TResult> selector) =>
            new(() => selector(source.Value));
    }
}
