using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreDX.Common.Util.InterfaceExtensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<TSource> Distinct<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            return source.GroupBy(keySelector).Select(_ => _.FirstOrDefault());
        }

        public static IEnumerable<TSource> Distinct<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector, Func<IGrouping<TKey, TSource>, TSource> selector)
        {
            return source.GroupBy(keySelector).Select(selector);
        }

        public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source,
            Func<TSource, TSource, bool> comparer, Func<TSource, int> hashCodeGenerator = null)
        {
            return source.Distinct(EqualityComparerHelper.Create(comparer, hashCodeGenerator));
        }
    }
}
