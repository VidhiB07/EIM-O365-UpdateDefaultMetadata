using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace EIM_O365_MetaDataTermSet.Data {
    public class MemoryCacheManager {
        /// <summary>
        /// Object used for null data to cache
        /// </summary>
        public const string NullDataObject = "IsNull|{AAB14E8F-5AA3-4368-8388-480735A72A67}";
        /// <summary>
        /// Memory Cache object
        /// </summary>
        public ObjectCache Cache;
        /// <summary>
        /// Default cache key
        /// </summary>
        public string CacheKey { get; set; }
        /// <summary>
        /// Memory Cache Manager
        /// </summary>
        /// <param name="cacheKey">default cache key</param>
        public MemoryCacheManager(string cacheKey) {
            Cache = MemoryCache.Default;
            CacheKey = cacheKey;
        }
        /// <summary>
        /// Gets from cache with default key
        /// </summary>
        /// <returns>object or null</returns>
        public object GetCacheObject() {
            return GetCacheObject(CacheKey);
        }
        /// <summary>
        /// Gets from cache using provided cache key
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <returns>object or null</returns>
        public object GetCacheObject(string cacheKey) {
            object cacheValue = Cache.Get(cacheKey);
            return cacheValue.ToString() == NullDataObject ? null : cacheValue;
        }
        /// <summary>
        /// If object with key is in cache using default key
        /// </summary>
        /// <returns>result as bool</returns>
        public bool IsObjectInCache() {
            return IsObjectInCache(CacheKey);
        }
        /// <summary>
        /// If object with key is in cache
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <returns>result as bool</returns>
        public bool IsObjectInCache(string cacheKey) {
            return Cache.Contains(cacheKey);
        }

        /// <summary>
        /// Sets to cache using default key
        /// </summary>
        /// <param name="obj">object to cache</param>
        /// <param name="expireHours">hours to expire in cache</param>
        /// <param name="expireMinutes">minutes to expire in cache</param>
        public void SetCache(object obj, int expireHours = 1, int expireMinutes = 0) {
            SetCache(obj, CacheKey, expireHours, expireMinutes);
        }


        /// <summary>
        /// Sets data to cache
        /// </summary>
        /// <param name="obj">object to cache</param>
        /// <param name="cacheKey">cachekey</param>
        /// <param name="expireHours">hours to expire in cache</param>
        /// <param name="expireMinutes">minutes to expire in cache</param>
        public void SetCache(object obj, string cacheKey, int expireHours = 1, int expireMinutes = 0) {
            if (expireHours < 0) expireHours = 0;
            if (expireMinutes < 1) expireHours = 1;

            if (obj == null) {
                obj = NullDataObject;
            }

            var expireDate = expireHours > 0 ? DateTime.Now.AddHours(expireHours) : DateTime.Now;
            expireDate = expireDate.AddMinutes(expireMinutes);
            Cache.Add(cacheKey, obj, expireDate);
        }
    }
}
