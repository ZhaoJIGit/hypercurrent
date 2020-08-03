using JieNor.Megi.Common.Mongo.IProvider;
using JieNor.Megi.Common.Mongo.Provider;
using System.Configuration;

namespace JieNor.Megi.Common.Mongo.Factory
{
	public class DataQueryingCacheFactory
	{
		private static MongoMContextProvider mContextProvider = null;

		private static MongoMContextProvider mExcelContextProvider = null;

		private static MongoMContextProvider mApiContextProvider = null;

		private static MongoMLogProvider mLogProvider = null;

		private static MongoMResourceProvider mResourceProvider = null;

		private static MongoMUserLogProvider mUserLogProvider = null;

		private static MongoCacheProvider cacheProvider = null;

		public static IMContextProvider BuilderMContextProvider()
		{
			if (mContextProvider == null)
			{
				mContextProvider = new MongoMContextProvider(ConfigurationManager.ConnectionStrings["MegiWebCacheConnection"].ConnectionString.TrimEnd('/') + "/megi");
			}
			return mContextProvider;
		}

		public static IMContextProvider BuilderExcelMContextProvider()
		{
			if (mExcelContextProvider == null)
			{
				mExcelContextProvider = new MongoMContextProvider(ConfigurationManager.ConnectionStrings["MegiWebCacheConnection"].ConnectionString.TrimEnd('/') + "/excel");
			}
			return mExcelContextProvider;
		}

		public static IMContextProvider BuilderApiMContextProvider()
		{
			if (mApiContextProvider == null)
			{
				mApiContextProvider = new MongoMContextProvider(ConfigurationManager.ConnectionStrings["MegiWebCacheConnection"].ConnectionString.TrimEnd('/') + "/api");
			}
			return mApiContextProvider;
		}

		public static IMLogProvider BuilderMLogProvider()
		{
			if (mLogProvider == null)
			{
				mLogProvider = new MongoMLogProvider(ConfigurationManager.ConnectionStrings["MegiLogCacheConnection"].ConnectionString);
			}
			return mLogProvider;
		}

		public static IMResourceProvider BuilderMResourceProvider()
		{
			if (mResourceProvider == null)
			{
				mResourceProvider = new MongoMResourceProvider(ConfigurationManager.ConnectionStrings["MegiWebCacheConnection"].ConnectionString.TrimEnd('/') + "/resource");
			}
			return mResourceProvider;
		}

		public static IMUserLogProvider BuilderMUserLogProvider()
		{
			if (mUserLogProvider == null)
			{
				mUserLogProvider = new MongoMUserLogProvider(ConfigurationManager.ConnectionStrings["MegiLogCacheConnection"].ConnectionString);
			}
			return mUserLogProvider;
		}

		public static ICacheProvider BuilderCacheProvider()
		{
			if (cacheProvider == null)
			{
				cacheProvider = new MongoCacheProvider(ConfigurationManager.ConnectionStrings["MegiWebCacheConnection"].ConnectionString.TrimEnd('/') + "/megi");
			}
			return cacheProvider;
		}
	}
}
