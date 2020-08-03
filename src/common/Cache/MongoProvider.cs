using JieNor.Megi.Common.Mongo.Factory;

namespace JieNor.Megi.Common.Cache
{
	public class MongoProvider : ICacheProvider
	{
		public T GetData<T>(string key)
		{
			return DataQueryingCacheFactory.BuilderCacheProvider().GetData<T>(key);
		}

		public void RemoveByPrefix(string prefix)
		{
			DataQueryingCacheFactory.BuilderCacheProvider().RemoveByPrefix(prefix);
		}

		public void RemoveData(string key)
		{
			DataQueryingCacheFactory.BuilderCacheProvider().RemoveData(key);
		}

		public void SaveData<T>(string key, T data)
		{
			DataQueryingCacheFactory.BuilderCacheProvider().SaveData(key, data);
		}
	}
}
