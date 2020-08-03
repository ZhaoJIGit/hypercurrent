using JieNor.Megi.Common.Redis;
using StackExchange.Redis;
using System;

namespace JieNor.Megi.Common.Cache
{
	public class RedisProvider : ICacheProvider
	{
		public T GetData<T>(string key)
		{
			return RedisClientManager.Instance.StringGet<T>(key);
		}

		public void SaveData<T>(string key, T data)
		{
			RedisClientManager.Instance.StringSet(key, data, null);
		}

		public void RemoveData(string key)
		{
			RedisClientManager.Instance.KeyDelete(key);
		}

		public void RemoveByPrefix(string prefix)
		{
			RedisClientManager.Instance.KeyDeleteByPrefix(prefix);
		}

		public static long Publish<T>(string channel, T msg)
		{
			return RedisClientManager.Instance.Publish(channel, msg);
		}

		public static void Subscribe<T>(string channel, Action<T> handler = null)
		{
			RedisClientManager.Instance.Subscribe(channel, delegate(RedisChannel rdchannel, T t)
			{
				handler(t);
			});
		}
	}
}
