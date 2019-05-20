using System.Collections.Generic;

namespace CoreDX.Common.Util.TypeExtensions
{
    public static class  DictionaryExtensions
    {
        public static bool TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            try
            {
                dictionary.Add(key, value);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
