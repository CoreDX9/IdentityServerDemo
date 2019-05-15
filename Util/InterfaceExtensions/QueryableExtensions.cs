using System;
using System.Linq;
using System.Linq.Expressions;

namespace Util.InterfaceExtensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<TSource> Distinct<TSource, TKey>(this IQueryable<TSource> source,
            Expression<Func<TSource, TKey>> keySelector)
        {
            return source.GroupBy(keySelector).Select(_ => _.FirstOrDefault());
        }

        public static IQueryable<TSource> Distinct<TSource, TKey>(this IQueryable<TSource> source,
            Expression<Func<TSource, TKey>> keySelector, Expression<Func<IGrouping<TKey, TSource>, TSource>> selector)
        {
            return source.GroupBy(keySelector).Select(selector);
        }
    }
}
