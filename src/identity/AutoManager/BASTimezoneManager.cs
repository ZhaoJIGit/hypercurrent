using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.ServiceContract.BAS;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.Identity.AutoManager
{
	public class BASTimezoneManager
	{
		public static List<BASTimezoneModel> GetList()
		{
			IBASTimezone sysService = ServiceHostManager.GetSysService<IBASTimezone>();
			using (sysService as IDisposable)
			{
				return sysService.GetList(null).ResultData;
			}
		}
	}
}
