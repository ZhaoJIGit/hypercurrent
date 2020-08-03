using JieNor.Megi.Common.Redis;
using System;

namespace JieNor.Megi.Core.DisLock
{
	public class RedisLock : IDisLock
	{
		public void LockRelease(string lockKey, string lockValue)
		{
			RedisClientManager.Instance.LockRelease(lockKey, lockValue);
		}

		public bool LockTake(string lockKey, string lockValue, TimeSpan timeSpan)
		{
			return RedisClientManager.Instance.LockTake(lockKey, lockValue, timeSpan);
		}
	}
}
