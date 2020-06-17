using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreDX.Common.Util.InterfaceExtensions
{
    public static class EqualityComparerHelper
    {
        public static IEqualityComparer<T> Create<T>(Func<T, T, bool> comparer, Func<T, int> hashCodeGenerator = null)
        {
            return new EqualityComparer<T>(comparer, hashCodeGenerator);
        }
    }

    public class EqualityComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> _comparer;
        private readonly Func<T, int> _hashCodeGenerator;

        public EqualityComparer(Func<T, T, bool> comparer) : this(comparer, null) { }

        public EqualityComparer(Func<T, T, bool> comparer, Func<T, int> hashCodeGenerator)
        {
            _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            _hashCodeGenerator = hashCodeGenerator ?? (_ => 1);
        }

        public bool Equals(T x, T y) => _comparer(x, y);

        public int GetHashCode(T obj) => _hashCodeGenerator(obj);
    }
}
