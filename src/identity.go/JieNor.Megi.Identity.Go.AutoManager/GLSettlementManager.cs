using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.ServiceContract.GL;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.Identity.Go.AutoManager
{
	public class GLSettlementManager
	{
		public static string GetLastFinishedPeriod()
		{
			IGLSettlement sysService = ServiceHostManager.GetSysService<IGLSettlement>();
			using (sysService as IDisposable)
			{
				return sysService.GetLastFinishedPeriod(null).ResultData;
			}
		}

		/// <summary>
		/// 获取稳定时期
		/// </summary>
		/// <returns></returns>
		public static List<DateTime> GetSettledPeriodFromBeginDate()
		{
			IGLSettlement sysService = ServiceHostManager.GetSysService<IGLSettlement>();
			using (sysService as IDisposable)
			{
				return sysService.GetSettledPeriodFromBeginDate(true, null).ResultData;
			}
		}

		public static List<DateTime> GetFullPeriod()
		{
			IGLSettlement sysService = ServiceHostManager.GetSysService<IGLSettlement>();
			using (sysService as IDisposable)
			{
				return sysService.GetFullPeriod(null).ResultData;
			}
		}
	}
}
