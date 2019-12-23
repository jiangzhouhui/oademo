using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Caching;

namespace OABordrinCommon
{
    public class MemoryCacheUtils
    {
        private static Dictionary<string, MemoryCache> cacheCollection = new Dictionary<string, MemoryCache>();

        public static void Set(string key, object value, string cacheName = "Default")
        {
            if (value == null)
            {
                return;
            }
            MemoryCache memoryCache;
            if (!cacheCollection.ContainsKey(cacheName))
            {
                if (cacheName.Equals("Default"))
                {
                    cacheCollection.Add(cacheName, MemoryCache.Default);
                }
                else
                {
                    cacheCollection.Add(cacheName, new MemoryCache(cacheName));
                }
            }
            memoryCache = cacheCollection[cacheName];
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = new DateTimeOffset(DateTime.Now.Add(memoryCache.PollingInterval));
            memoryCache.Set(key, value, policy);
        }

        public static void Set(string key, object value, CacheItemPolicy policy, string cacheName = "Default")
        {
            if (value == null)
            {
                return;
            }
            MemoryCache memoryCache;
            if (!cacheCollection.ContainsKey(cacheName))
            {
                if (cacheName.Equals("Default"))
                {
                    cacheCollection.Add(cacheName, MemoryCache.Default);
                }
                else
                {
                    cacheCollection.Add(cacheName, new MemoryCache(cacheName));
                }
            }
            memoryCache = cacheCollection[cacheName];
            memoryCache.Set(key, value, policy);
        }

        public static object Get(string key, string cacheName = "Default")
        {
            if (!cacheCollection.ContainsKey(cacheName))
            {
                if (cacheName.Equals("Default"))
                {
                    cacheCollection.Add(cacheName, MemoryCache.Default);
                }
                else
                {
                    cacheCollection.Add(cacheName, new MemoryCache(cacheName));
                }
            }
            return cacheCollection[cacheName].Get(key);
        }

        public static void Clear(string key, string cacheName = "Default")
        {
            cacheCollection[cacheName].Remove(key);
        }

        public static void ClearAll()
        {
            foreach (var element in cacheCollection.SelectMany(cache => cache.Value))
            {
                MemoryCache.Default.Remove(element.Key);
            }
        }
    }
}