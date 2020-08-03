using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.ServiceContract.BAS;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.Identity.AutoManager
{
	public class BASCurrencyManager
	{
		public static List<BASCurrencyModel> GetList()
		{
			IBASCurrency sysService = ServiceHostManager.GetSysService<IBASCurrency>();
			using (sysService as IDisposable)
			{
				return sysService.GetList(null).ResultData;
			}
		}

		public static List<BASCurrencyViewModel> GetViewList()
		{
			IBASCurrency sysService = ServiceHostManager.GetSysService<IBASCurrency>();
			using (sysService as IDisposable)
			{
				return sysService.GetViewList(true, null).ResultData;
			}
		}
	}
}
