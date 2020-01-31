using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CoreDX.Common.Util.EnumerableExtensions
{
    public static class QueryableExtensions
    {
        public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> query, bool condition, Func<T, bool> predicate)
        {
            return condition
                ? query.Where(predicate)
                : query;
        }

        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
        {
            return condition
                ? query.Where(predicate)
                : query;
        }

        public static IEnumerable<T> TakeIf<T>(this IEnumerable<T> query, bool condition, int limit)
        {
            return condition
                ? query.Take(limit)
                : query;
        }

        public static IQueryable<T> TakeIf<T>(this IQueryable<T> query, bool condition, int limit)
        {
            return condition
                ? query.Take(limit)
                : query;
        }
    }
}
