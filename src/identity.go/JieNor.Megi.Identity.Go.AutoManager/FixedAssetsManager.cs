using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.ServiceContract.FA;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.Identity.Go.AutoManager
{
	public class FixedAssetsManager
	{
		public static List<DateTime> GetFAPeriodFromBeginDate(bool isDepreciationReport = false)
		{
			IFAFixAssets sysService = ServiceHostManager.GetSysService<IFAFixAssets>();
			using (sysService as IDisposable)
			{
				return sysService.GetFAPeriodFromBeginDate(isDepreciationReport, null).ResultData;
			}
		}
	}
}
