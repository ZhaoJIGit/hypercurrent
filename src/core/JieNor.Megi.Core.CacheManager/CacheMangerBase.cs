using JieNor.Megi.Common.Cache;

namespace JieNor.Megi.Core.CacheManager
{
	public abstract class CacheMangerBase<T>
	{
		protected readonly ICacheProvider _Provider;

		public CacheMangerBase(ICacheProvider provider)
		{
			_Provider = provider;
		}

		protected abstract string GetTypePrefix();

		public virtual T GetData(string key)
		{
			return _Provider.GetData<T>(GetTypePrefix() + key);
		}

		public void SaveData(string key, T data)
		{
			_Provider.SaveData<T>(GetTypePrefix() + key, data);
		}

		public virtual void RemoveData(string key)
		{
			_Provider.RemoveData(GetTypePrefix() + key);
		}
	}
}
