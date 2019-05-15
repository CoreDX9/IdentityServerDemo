using System;
using System.Collections.Generic;
using System.Linq;

namespace Util.InterfaceExtensions
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
    }
}
