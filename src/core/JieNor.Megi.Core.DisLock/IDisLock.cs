using System;

namespace JieNor.Megi.Core.DisLock
{
	public interface IDisLock
	{
		bool LockTake(string lockKey, string lockValue, TimeSpan timeSpan);

		void LockRelease(string lockKey, string lockValue);
	}
}
