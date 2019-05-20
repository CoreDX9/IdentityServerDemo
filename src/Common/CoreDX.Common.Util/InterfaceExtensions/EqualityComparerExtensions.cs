using System;
using System.Collections.Generic;

namespace CoreDX.Common.Util.InterfaceExtensions
{
    class EqualityComparerFactory
    {
        public static IEqualityComparer<T> Create<T>(Func<T, T, bool> comparer, Func<T, int> hashCodeGenerator = null)
        {
            return new InternalEqualityComparer<T>(comparer, hashCodeGenerator);
        }

        private class InternalEqualityComparer<T> : IEqualityComparer<T>
        {
            private readonly Func<T, T, bool> _comparer;
            private readonly Func<T, int> _hashCodeGenerator;

            public InternalEqualityComparer(Func<T, T, bool> comparer) =>
                _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));

            public InternalEqualityComparer(Func<T, T, bool> comparer, Func<T, int> hashCodeGenerator)
                : this(comparer) => _hashCodeGenerator = hashCodeGenerator;

            public bool Equals(T x, T y) => _comparer(x, y);

            public int GetHashCode(T obj) => _hashCodeGenerator?.Invoke(obj) ?? 1;
        }
    }
}
