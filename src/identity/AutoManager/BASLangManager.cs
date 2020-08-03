using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.ServiceContract.BAS;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.Identity.AutoManager
{
	public class BASLangManager
	{
		public static List<BASLangModel> GetOrgLangList()
		{
			IBASLang sysService = ServiceHostManager.GetSysService<IBASLang>();
			using (sysService as IDisposable)
			{
				return sysService.GetOrgLangList(null).ResultData;
			}
		}

		public static List<BASLangModel> GetSysLangList()
		{
			IBASLang sysService = ServiceHostManager.GetSysService<IBASLang>();
			using (sysService as IDisposable)
			{
				return sysService.GetSysLangList(null).ResultData;
			}
		}

		public static string GetClientGlobalInfo()
		{
			IBASLang sysService = ServiceHostManager.GetSysService<IBASLang>();
			using (sysService as IDisposable)
			{
				return sysService.GetClientGlobalInfo(null).ResultData;
			}
		}
	}
}
