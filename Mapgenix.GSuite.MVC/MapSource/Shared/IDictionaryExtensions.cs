using System;
using System.Collections.Generic;

namespace Mapgenix.GSuite.Mvc
{
    public static class IDictionaryExtensions
    {
        public static void Merge(this IDictionary<string, string> collection, string key, object value)
        {
            collection.Merge(key, value, false);
        }

        public static void Merge(this IDictionary<string, string> collection, string key, object value, bool replaceExisting)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Value cannot be null or empty.", "key");
            }

            if (replaceExisting || !collection.ContainsKey(key))
            {
                collection[key] = value.ToString();
            }
        }

        public static string GetValue(this IDictionary<string, string> collection, string key)
        {
            if (collection.ContainsKey(key))
            {
                return collection[key];
            }

            return null;
        }
    }
}
