using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AngryTanks.Common.Extensions
{
    namespace DictionaryExtensions
    {
        public static class DictionaryExtensionsClass
        {
            public static void RemoveAll<TKey, TValue>(this IDictionary<TKey, TValue> dict, Func<KeyValuePair<TKey, TValue>, bool> predicate)
            {
                List<TKey> keysToRemove = new List<TKey>();

                foreach (KeyValuePair<TKey, TValue> kvp in dict.Where(predicate))
                    keysToRemove.Add(kvp.Key);

                foreach (TKey key in keysToRemove)
                    dict.Remove(key);
            }
        }
    }
}
