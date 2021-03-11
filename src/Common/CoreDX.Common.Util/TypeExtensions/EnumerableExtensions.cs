using System.Collections.Generic;

namespace CoreDX.Common.Util.TypeExtensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source)
        {
            int index = 0;
            foreach (var item in source)
            {
                yield return (item, index);
                index++;
            }
        }

        public static IEnumerable<(T item, long index)> WithLongIndex<T>(this IEnumerable<T> source)
        {
            long index = 0L;
            foreach (var item in source)
            {
                yield return (item, index);
                index++;
            }
        }

        public static async IAsyncEnumerable<(T item, int index)> WithIndex<T>(this IAsyncEnumerable<T> source)
        {
            int index = 0;
            await foreach (var item in source)
            {
                yield return (item, index);
                index++;
            }
        }

        public static async IAsyncEnumerable<(T item, long index)> WithLongIndex<T>(this IAsyncEnumerable<T> source)
        {
            long index = 0L;
            await foreach (var item in source)
            {
                yield return (item, index);
                index++;
            }
        }
    }
}
