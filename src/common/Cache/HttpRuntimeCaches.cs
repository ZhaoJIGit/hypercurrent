using System;
using System.Web;
using System.Web.Caching;

namespace JieNor.Megi.Common.Cache
{
	public class HttpRuntimeCaches
	{
		public static object Get(string name)
		{
			return HttpRuntime.Cache.Get(name);
		}

		public static void Remove(string name)
		{
			if (HttpRuntime.Cache[name] != null)
			{
				HttpRuntime.Cache.Remove(name);
			}
		}

		public static void Set(string name, object value)
		{
			Set(name, value, null);
		}

		public static void Set(string name, object value, CacheDependency cacheDependency)
		{
			HttpRuntime.Cache.Insert(name, value, cacheDependency, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(20.0));
		}

		public static void Set(string name, object value, int minutes)
		{
			HttpRuntime.Cache.Insert(name, value, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes((double)minutes));
		}
	}
}
