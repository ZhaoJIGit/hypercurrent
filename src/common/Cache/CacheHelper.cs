using System;
using System.Web;
using System.Web.Caching;

namespace JieNor.Megi.Common.Cache
{
	public static class CacheHelper
	{
		public static System.Web.Caching.Cache Cache
		{
			get
			{
				if (HttpContext.Current != null)
				{
					return HttpContext.Current.Cache;
				}
				return HttpRuntime.Cache;
			}
		}

		public static string AppPrefix => AppDomain.CurrentDomain.Id.ToString();

		public static object Get(string key)
		{
			return Cache.Get(AppPrefix + key);
		}

		public static void Set(string key, object value)
		{
			if (value != null)
			{
				Cache.Insert(AppPrefix + key, value);
			}
		}

		public static void Set(string key, object value, CacheDependency cacheDependency)
		{
			if (value != null)
			{
				Cache.Insert(AppPrefix + key, value, cacheDependency);
			}
		}

		public static void Set(string key, object value, CacheDependency cacheDependency, DateTime dt)
		{
			if (value != null)
			{
				Cache.Insert(AppPrefix + key, value, cacheDependency, dt, TimeSpan.Zero);
			}
		}

		public static void Set(string key, object value, CacheDependency cacheDependency, TimeSpan ts)
		{
			if (value != null)
			{
				Cache.Insert(AppPrefix + key, value, cacheDependency, System.Web.Caching.Cache.NoAbsoluteExpiration, ts);
			}
		}

		public static void Set(string key, object value, CacheDependency cacheDependency, DateTime dt, TimeSpan ts)
		{
			if (value != null)
			{
				Cache.Insert(AppPrefix + key, value, cacheDependency, dt, ts);
			}
		}

		public static void Set(string key, object value, CacheDependency cacheDependency, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemRemovedCallback onUpdate)
		{
			if (value != null)
			{
				Cache.Insert(AppPrefix + key, value, cacheDependency, absoluteExpiration, slidingExpiration, CacheItemPriority.Normal, onUpdate);
			}
		}

		public static void Set(string key, object value, CacheDependency cacheDependency, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority, CacheItemRemovedCallback onUpdate)
		{
			if (value != null)
			{
				Cache.Insert(AppPrefix + key, value, cacheDependency, absoluteExpiration, slidingExpiration, priority, onUpdate);
			}
		}

		public static void Remove(string key)
		{
			Cache.Remove(AppPrefix + key);
		}

		public static bool Contain(string key)
		{
			return Cache.Get(AppPrefix + key) != null;
		}
	}
}
