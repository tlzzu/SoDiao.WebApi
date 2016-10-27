using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoDiao.App.WebApi
{
    public class FileCacheHelper
    {

        /// <summary>
        /// 获取当前应用程序指定CacheKey的Cache对象值
        /// </summary>
        /// <param name="CacheKey">索引键值</param>
        /// <returns>返回缓存对象</returns>
        public static T GetCache<T>(string CacheKey)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            var obj = objCache[CacheKey];
            return obj == null ? default(T) : (T)obj;
        }

        /// <summary>
        /// 设置以缓存依赖的方式缓存数据
        /// </summary>
        /// <param name="CacheKey">索引键值</param>
        /// <param name="objObject">缓存对象</param>
        /// <param name="cacheDepen">依赖对象</param>
        public static void SetCache(string CacheKey, object objObject, string filePath)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(
                CacheKey,
                objObject,
                new System.Web.Caching.CacheDependency(filePath),
                System.Web.Caching.Cache.NoAbsoluteExpiration, //从不过期
                System.Web.Caching.Cache.NoSlidingExpiration, //禁用可调过期
                System.Web.Caching.CacheItemPriority.Default,
                null);
        }
    }
}