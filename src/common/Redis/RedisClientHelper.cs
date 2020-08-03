using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JieNor.Megi.Common.Redis
{
	public class RedisClientHelper
	{
		private readonly ConnectionMultiplexer _conn;

		private string KeyPrefix = "";

		private int DbNum
		{
			get;
		}

		public RedisClientHelper()
		{
		}

		public RedisClientHelper(int dbNum, ConfigurationOptions configOptions, string KeyPrefix)
		{
			DbNum = dbNum;
			this.KeyPrefix = KeyPrefix;
			_conn = ConnectionMultiplexer.Connect(configOptions, null);
		}

		public bool StringSet(string key, string value, TimeSpan? expiry = default(TimeSpan?))
		{
			key = AddSysCustomKey(key);
			return Do((IDatabase db) => db.StringSet(key, value, expiry, When.Always, CommandFlags.None));
		}

		public bool StringSet(List<KeyValuePair<RedisKey, RedisValue>> keyValues)
		{
			List<KeyValuePair<RedisKey, RedisValue>> newkeyValues = (from p in keyValues
			select new KeyValuePair<RedisKey, RedisValue>(AddSysCustomKey(p.Key), p.Value)).ToList();
			return Do((IDatabase db) => db.StringSet(newkeyValues.ToArray(), When.Always, CommandFlags.None));
		}

		public bool StringSet<T>(string key, T obj, TimeSpan? expiry = default(TimeSpan?))
		{
			key = AddSysCustomKey(key);
			string json = ConvertJson(obj);
			return Do((IDatabase db) => db.StringSet(key, json, expiry, When.Always, CommandFlags.None));
		}

		public string StringGet(string key)
		{
			key = AddSysCustomKey(key);
			return Do((IDatabase db) => db.StringGet(key, CommandFlags.None));
		}

		public RedisValue[] StringGet(List<string> listKey)
		{
			List<string> newKeys = listKey.Select(AddSysCustomKey).ToList();
			return Do((IDatabase db) => db.StringGet(ConvertRedisKeys(newKeys), CommandFlags.None));
		}

		public T StringGet<T>(string key)
		{
			key = AddSysCustomKey(key);
			return Do((IDatabase db) => this.ConvertObj<T>(db.StringGet(key, CommandFlags.None)));
		}

		public double StringIncrement(string key, double val = 1.0)
		{
			key = AddSysCustomKey(key);
			return Do((IDatabase db) => db.StringIncrement(key, val, CommandFlags.None));
		}

		public double StringDecrement(string key, double val = 1.0)
		{
			key = AddSysCustomKey(key);
			return Do((IDatabase db) => db.StringDecrement(key, val, CommandFlags.None));
		}

		public async Task<bool> StringSetAsync(string key, string value, TimeSpan? expiry = default(TimeSpan?))
		{
			key = AddSysCustomKey(key);
			return await Do((IDatabase db) => db.StringSetAsync(key, value, expiry, When.Always, CommandFlags.None));
		}

		public async Task<bool> StringSetAsync(List<KeyValuePair<RedisKey, RedisValue>> keyValues)
		{
			List<KeyValuePair<RedisKey, RedisValue>> newkeyValues = (from p in keyValues
			select new KeyValuePair<RedisKey, RedisValue>(AddSysCustomKey(p.Key), p.Value)).ToList();
			return await Do((IDatabase db) => db.StringSetAsync(newkeyValues.ToArray(), When.Always, CommandFlags.None));
		}

		public async Task<bool> StringSetAsync<T>(string key, T obj, TimeSpan? expiry = default(TimeSpan?))
		{
			key = AddSysCustomKey(key);
			string json = this.ConvertJson<T>(obj);
			return await this.Do<Task<bool>>((Func<IDatabase, Task<bool>>)((IDatabase db) => db.StringSetAsync(key, json, expiry, When.Always, CommandFlags.None)));
		}

		public async Task<string> StringGetAsync(string key)
		{
			key = AddSysCustomKey(key);
			return await Do((IDatabase db) => db.StringGetAsync(key, CommandFlags.None));
		}

		public async Task<RedisValue[]> StringGetAsync(List<string> listKey)
		{
			List<string> newKeys = listKey.Select(AddSysCustomKey).ToList();
			return await Do((IDatabase db) => db.StringGetAsync(ConvertRedisKeys(newKeys), CommandFlags.None));
		}

		public async Task<T> StringGetAsync<T>(string key)
		{
			key = AddSysCustomKey(key);
			string result = await this.Do<Task<RedisValue>>((Func<IDatabase, Task<RedisValue>>)((IDatabase db) => db.StringGetAsync(key, CommandFlags.None)));
			return this.ConvertObj<T>((RedisValue)result);
		}

		public async Task<double> StringIncrementAsync(string key, double val = 1.0)
		{
			key = AddSysCustomKey(key);
			return await Do((IDatabase db) => db.StringIncrementAsync(key, val, CommandFlags.None));
		}

		public async Task<double> StringDecrementAsync(string key, double val = 1.0)
		{
			key = AddSysCustomKey(key);
			return await Do((IDatabase db) => db.StringDecrementAsync(key, val, CommandFlags.None));
		}

		public void ListRemove<T>(string key, T value)
		{
			key = AddSysCustomKey(key);
			Do((IDatabase db) => db.ListRemove(key, this.ConvertJson<T>(value), 0L, CommandFlags.None));
		}

		public List<T> ListRange<T>(string key)
		{
			key = AddSysCustomKey(key);
			return Do(delegate(IDatabase redis)
			{
				RedisValue[] values = redis.ListRange(key, 0L, -1L, CommandFlags.None);
				return this.ConvetList<T>(values);
			});
		}

		public void ListRightPush<T>(string key, T value)
		{
			key = AddSysCustomKey(key);
			Do((IDatabase db) => db.ListRightPush(key, this.ConvertJson<T>(value), When.Always, CommandFlags.None));
		}

		public T ListRightPop<T>(string key)
		{
			key = AddSysCustomKey(key);
			return Do(delegate(IDatabase db)
			{
				RedisValue value = db.ListRightPop(key, CommandFlags.None);
				return this.ConvertObj<T>(value);
			});
		}

		public void ListLeftPush<T>(string key, T value)
		{
			key = AddSysCustomKey(key);
			Do((IDatabase db) => db.ListLeftPush(key, this.ConvertJson<T>(value), When.Always, CommandFlags.None));
		}

		public T ListLeftPop<T>(string key)
		{
			key = AddSysCustomKey(key);
			return Do(delegate(IDatabase db)
			{
				RedisValue value = db.ListLeftPop(key, CommandFlags.None);
				return this.ConvertObj<T>(value);
			});
		}

		public long ListLength(string key)
		{
			key = AddSysCustomKey(key);
			return Do((IDatabase redis) => redis.ListLength(key, CommandFlags.None));
		}

		public T ListRightPopLeftPush<T>(string source, string destination)
		{
			source = AddSysCustomKey(source);
			destination = AddSysCustomKey(destination);
			RedisValue value = Do((IDatabase db) => db.ListRightPopLeftPush(source, destination, CommandFlags.None));
			return ConvertObj<T>(value);
		}

		public async Task<long> ListRemoveAsync<T>(string key, T value)
		{
			key = AddSysCustomKey(key);
			return await this.Do<Task<long>>((Func<IDatabase, Task<long>>)((IDatabase db) => db.ListRemoveAsync(key, this.ConvertJson<T>(value), 0L, CommandFlags.None)));
		}

		public async Task<List<T>> ListRangeAsync<T>(string key)
		{
			key = AddSysCustomKey(key);
			return this.ConvetList<T>(await this.Do<Task<RedisValue[]>>((Func<IDatabase, Task<RedisValue[]>>)((IDatabase redis) => redis.ListRangeAsync(key, 0L, -1L, CommandFlags.None))));
		}

		public async Task<long> ListRightPushAsync<T>(string key, T value)
		{
			key = AddSysCustomKey(key);
			return await this.Do<Task<long>>((Func<IDatabase, Task<long>>)((IDatabase db) => db.ListRightPushAsync(key, this.ConvertJson<T>(value), When.Always, CommandFlags.None)));
		}

		public async Task<T> ListRightPopAsync<T>(string key)
		{
			key = AddSysCustomKey(key);
			return this.ConvertObj<T>(await this.Do<Task<RedisValue>>((Func<IDatabase, Task<RedisValue>>)((IDatabase db) => db.ListRightPopAsync(key, CommandFlags.None))));
		}

		public async Task<long> ListLeftPushAsync<T>(string key, T value)
		{
			key = AddSysCustomKey(key);
			return await this.Do<Task<long>>((Func<IDatabase, Task<long>>)((IDatabase db) => db.ListLeftPushAsync(key, this.ConvertJson<T>(value), When.Always, CommandFlags.None)));
		}

		public async Task<T> ListLeftPopAsync<T>(string key)
		{
			key = AddSysCustomKey(key);
			return this.ConvertObj<T>(await this.Do<Task<RedisValue>>((Func<IDatabase, Task<RedisValue>>)((IDatabase db) => db.ListLeftPopAsync(key, CommandFlags.None))));
		}

		public async Task<long> ListLengthAsync(string key)
		{
			key = AddSysCustomKey(key);
			return await Do((IDatabase redis) => redis.ListLengthAsync(key, CommandFlags.None));
		}

		public bool KeyDelete(string key)
		{
			key = AddSysCustomKey(key);
			return Do((IDatabase db) => db.KeyDelete(key, CommandFlags.None));
		}

		public long KeyDelete(List<string> keys)
		{
			List<string> newKeys = keys.Select(AddSysCustomKey).ToList();
			return Do((IDatabase db) => db.KeyDelete(ConvertRedisKeys(newKeys), CommandFlags.None));
		}

		public bool KeyExists(string key)
		{
			key = AddSysCustomKey(key);
			return Do((IDatabase db) => db.KeyExists(key, CommandFlags.None));
		}

		public bool KeyRename(string key, string newKey)
		{
			key = AddSysCustomKey(key);
			return Do((IDatabase db) => db.KeyRename(key, newKey, When.Always, CommandFlags.None));
		}

		public bool KeyExpire(string key, TimeSpan? expiry = default(TimeSpan?))
		{
			key = AddSysCustomKey(key);
			return Do((IDatabase db) => db.KeyExpire(key, expiry, CommandFlags.None));
		}

		public void KeyDeleteByPrefix(string keyPrefix)
		{
			Do(async delegate(IDatabase db)
			{
				keyPrefix = AddSysCustomKey(keyPrefix);
				RedisResult redisResult = await db.ScriptEvaluateAsync(LuaScript.Prepare(" local res = redis.call('KEYS', @keypattern)  return res "), new
				{
					keypattern = keyPrefix
				}, CommandFlags.None);
				if (!redisResult.IsNull)
				{
					db.KeyDelete((RedisKey[])redisResult, CommandFlags.None);
				}
				return true;
			});
		}

		public bool LockTake(string key, string lockValue, TimeSpan expiry)
		{
			key = AddSysCustomKey(key);
			return Do((IDatabase db) => db.LockTake(key, lockValue, expiry, CommandFlags.None));
		}

		public string LockQuery(string key)
		{
			key = AddSysCustomKey(key);
			return Do((IDatabase db) => db.LockQuery(key, CommandFlags.None));
		}

		public bool LockRelease(string key, string lockValue)
		{
			key = AddSysCustomKey(key);
			return Do((IDatabase db) => db.LockRelease(key, lockValue, CommandFlags.None));
		}

		public void Subscribe<T>(string subChannel, Action<RedisChannel, T> handler = null)
		{
			if (_conn != null)
			{
				ISubscriber sub = _conn.GetSubscriber(null);
				sub.Subscribe(subChannel, delegate(RedisChannel channel, RedisValue message)
				{
					if (handler != null)
					{
						T arg = this.ConvertObj<T>(message);
						handler(channel, arg);
					}
				}, CommandFlags.None);
			}
		}

		public void Subscribe(string subChannel, Action<RedisChannel, object> handler = null)
		{
			if (_conn != null)
			{
				ISubscriber sub = _conn.GetSubscriber(null);
				sub.Subscribe(subChannel, delegate(RedisChannel channel, RedisValue message)
				{
					handler?.Invoke(channel, message);
				}, CommandFlags.None);
			}
		}

		public long Publish<T>(string channel, T msg)
		{
			if (_conn != null)
			{
				ISubscriber sub = _conn.GetSubscriber(null);
				return sub.Publish(channel, ConvertJson(msg), CommandFlags.None);
			}
			return 0L;
		}

		public void Unsubscribe(string channel)
		{
			if (_conn != null)
			{
				ISubscriber sub = _conn.GetSubscriber(null);
				sub.Unsubscribe(channel, null, CommandFlags.None);
			}
		}

		public void UnsubscribeAll()
		{
			if (_conn != null)
			{
				ISubscriber sub = _conn.GetSubscriber(null);
				sub.UnsubscribeAll(CommandFlags.None);
			}
		}

		public ITransaction CreateTransaction()
		{
			return GetDatabase().CreateTransaction(null);
		}

		public IDatabase GetDatabase()
		{
			return _conn.GetDatabase(DbNum, null);
		}

		public IServer GetServer(string hostAndPort)
		{
			return _conn.GetServer(hostAndPort, null);
		}

		private string AddSysCustomKey(string oldKey)
		{
			string prefixKey = KeyPrefix;
			return prefixKey + oldKey;
		}

		private T Do<T>(Func<IDatabase, T> func)
		{
			try
			{
				if (_conn != null)
				{
					IDatabase database = _conn.GetDatabase(DbNum, null);
					return func(database);
				}
				return default(T);
			}
			catch (RedisConnectionException)
			{
				throw new MRedisTimeoutException();
			}
			catch (RedisTimeoutException)
			{
				throw new MRedisTimeoutException();
			}
			catch (Exception)
			{
				return default(T);
			}
		}

		private string ConvertJson<T>(T value)
		{
			return JsonConvert.SerializeObject(value);
		}

		private T ConvertObj<T>(RedisValue value)
		{
			if (value.IsNull)
			{
				return default(T);
			}
			string json = value.ToString();
			return JsonConvert.DeserializeObject<T>(json);
		}

		private List<T> ConvetList<T>(RedisValue[] values)
		{
			List<T> result = new List<T>();
			foreach (RedisValue item in values)
			{
				T model = ConvertObj<T>(item);
				result.Add(model);
			}
			return result;
		}

		private RedisKey[] ConvertRedisKeys(List<string> redisKeys)
		{
			return (from redisKey in (IEnumerable<string>)redisKeys
			select (RedisKey)(redisKey)).ToArray(); 
		}
	}
}
