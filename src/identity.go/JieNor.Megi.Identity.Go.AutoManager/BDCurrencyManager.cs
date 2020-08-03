using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.ServiceContract.REG;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.Identity.Go.AutoManager
{
	public class BDCurrencyManager
	{
		public static List<REGCurrencyModel> GetList()
		{
			IREGCurrency sysService = ServiceHostManager.GetSysService<IREGCurrency>();
			using (sysService as IDisposable)
			{
				return sysService.GetCurrencyList(null).ResultData;
			}
		}

		public static List<REGCurrencyModel> GetCurrencyList()
		{
			IREGCurrency sysService = ServiceHostManager.GetSysService<IREGCurrency>();
			using (sysService as IDisposable)
			{
				return sysService.GetCurrencyList(null).ResultData;
			}
		}

		public static List<REGCurrencyViewModel> GetCurrencyViewList(DateTime? endDate = default(DateTime?))
		{
			IREGCurrency sysService = ServiceHostManager.GetSysService<IREGCurrency>();
			using (sysService as IDisposable)
			{
				return sysService.GetCurrencyViewList(endDate, false, null).ResultData;
			}
		}

		public static List<REGCurrencyViewModel> GetBillCurrencyViewList(DateTime? endDate = default(DateTime?))
		{
			IREGCurrency sysService = ServiceHostManager.GetSysService<IREGCurrency>();
			using (sysService as IDisposable)
			{
				return sysService.GetBillCurrencyViewList(endDate, false, null).ResultData;
			}
		}

		public static List<REGCurrencyViewModel> GetAllCurrencyList(bool isIncludeBase = false, bool ignoreLocale = false)
		{
			IREGCurrency sysService = ServiceHostManager.GetSysService<IREGCurrency>();
			using (sysService as IDisposable)
			{
				return sysService.GetAllCurrencyList(isIncludeBase, ignoreLocale, null).ResultData;
			}
		}

		public static BASCurrencyViewModel GetBaseCurrency()
		{
			IREGCurrency sysService = ServiceHostManager.GetSysService<IREGCurrency>();
			using (sysService as IDisposable)
			{
				return sysService.GetBaseCurrency(null).ResultData;
			}
		}
	}
}
