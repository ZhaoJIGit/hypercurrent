using System;
using System.Collections;
using System.Web;
using System.Web.Caching;

namespace JieNor.Megi.Common.Cache
{
	public class Caches
	{
		public static object TryAddCaChe(string key, object value, ICacheDependency dependency, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority, CacheItemRemovedCallback onRemoveCallback)
		{
			if (HttpRuntime.Cache[key] == null && value != null)
			{
				if (dependency != null)
				{
					return HttpRuntime.Cache.Add(key, value, dependency.GetDependency(), absoluteExpiration, slidingExpiration, priority, onRemoveCallback);
				}
				return HttpRuntime.Cache.Add(key, value, null, absoluteExpiration, slidingExpiration, priority, onRemoveCallback);
			}
			return null;
		}

		public static object TryAddCaChe(string key, object value, ICacheDependency dependency, DateTime absoluteExpiration)
		{
			if (HttpRuntime.Cache[key] == null && value != null)
			{
				if (dependency != null)
				{
					return HttpRuntime.Cache.Add(key, value, dependency.GetDependency(), absoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);
				}
				return HttpRuntime.Cache.Add(key, value, null, absoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);
			}
			return null;
		}

		public static object TryRemoveCache(string key)
		{
			if (HttpRuntime.Cache[key] != null)
			{
				return HttpRuntime.Cache.Remove(key);
			}
			return null;
		}

		public static void RemoveMultiCache(string keyInclude)
		{
			IDictionaryEnumerator cacheEnum = HttpRuntime.Cache.GetEnumerator();
			while (cacheEnum.MoveNext())
			{
				if (cacheEnum.Key.ToString().IndexOf(keyInclude) >= 0)
				{
					HttpRuntime.Cache.Remove(cacheEnum.Key.ToString());
				}
			}
		}

		public static void RemovePrefixCache(string keyInclude)
		{
			IDictionaryEnumerator cacheEnum = HttpRuntime.Cache.GetEnumerator();
			while (cacheEnum.MoveNext())
			{
				if (cacheEnum.Key.ToString().StartsWith(keyInclude))
				{
					HttpRuntime.Cache.Remove(cacheEnum.Key.ToString());
				}
			}
		}

		public static void RemoveAllCache()
		{
			IDictionaryEnumerator cacheEnum = HttpRuntime.Cache.GetEnumerator();
			while (cacheEnum.MoveNext())
			{
				HttpRuntime.Cache.Remove(cacheEnum.Key.ToString());
			}
		}
	}
}
