using JieNor.Megi.Common.Redis;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace JieNor.Megi.Core.DisLock
{
	public class DisLockManager
	{
		private static DisLockConfig disLockConfig;

		private static DateTime TimeOutTick = DateTime.MinValue;

		private static IDisLock disLock = new RedisLock();

		public static void Register(DisLockConfig _disLockConfig)
		{
			disLockConfig = _disLockConfig;
		}

		public static void GetLock(MContext ctx, MethodInfo method)
		{
			if (disLockConfig != null && NeedLock(ctx, method) && !LockTake(ctx.MOrgID, ctx.GetHashCode().ToString()))
			{
				throw new MActionException
				{
					Codes = new List<MActionResultCodeEnum>
					{
						MActionResultCodeEnum.SyncBusy
					}
				};
			}
		}

		public static void ReleaseLock(MContext ctx, MethodInfo method)
		{
			if (disLockConfig != null && NeedLock(ctx, method))
			{
				LockRelease(ctx.MOrgID, ctx.GetHashCode().ToString());
			}
		}

		private static bool NeedLock(MContext ctx, MethodInfo methodName)
		{
			if (ctx == null || string.IsNullOrEmpty(ctx.MOrgID))
			{
				return false;
			}
			if (!disLockConfig.SyncLockEnable)
			{
				return false;
			}
			if (disLockConfig.LockOperations == null || disLockConfig.LockOperations.Count <= 0)
			{
				return false;
			}
			string fullname = methodName.DeclaringType.FullName + "." + methodName.Name;
			return disLockConfig.LockOperations.Any((LockOperation t) => t.MFunctionName == fullname && t.MIsLock);
		}

		private static bool LockTake(string lockKey, string lockValue, TimeSpan timeSpan)
		{
			if (TimeOutTick.AddMinutes((double)disLockConfig.HystrixInterval).CompareTo(DateTime.Now) > 0)
			{
				return true;
			}
			bool result = true;
			try
			{
				result = disLock.LockTake(lockKey, lockValue, timeSpan);
			}
			catch (MRedisTimeoutException)
			{
				TimeOutTick = DateTime.Now;
			}
			catch
			{
			}
			return result;
		}

		private static bool LockTake(string lockKey, string lockValue)
		{
			for (int num = disLockConfig.RetryCount; num > 0; num--)
			{
				if (LockTake(lockKey, lockValue, disLockConfig.DefaultRelease))
				{
					return true;
				}
				Thread.Sleep(disLockConfig.RetryInterval);
			}
			return false;
		}

		private static void LockRelease(string lockKey, string lockValue)
		{
			if (TimeOutTick.AddMinutes((double)disLockConfig.HystrixInterval).CompareTo(DateTime.Now) <= 0)
			{
				try
				{
					disLock.LockRelease(lockKey, lockValue);
				}
				catch
				{
				}
			}
		}
	}
}
