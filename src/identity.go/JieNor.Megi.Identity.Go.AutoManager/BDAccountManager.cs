using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.ServiceContract.BD;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.Identity.Go.AutoManager
{
	public class BDAccountManager
	{
		public static List<BDAccountTypeListModel> GetBDAccountTypeList(string filterString)
		{
			IBDAccount sysService = ServiceHostManager.GetSysService<IBDAccount>();
			using (sysService as IDisposable)
			{
				return sysService.GetBDAccountTypeList(filterString, null).ResultData;
			}
		}

		public List<BDAccountListModel> GetBDAccountList(string filterString)
		{
			IBDAccount sysService = ServiceHostManager.GetSysService<IBDAccount>();
			using (sysService as IDisposable)
			{
				return sysService.GetBDAccountList(filterString, null).ResultData;
			}
		}

		public static List<BDBankAccountEditModel> GetBDBankAccountList()
		{
			IBDBankAccount sysService = ServiceHostManager.GetSysService<IBDBankAccount>();
			using (sysService as IDisposable)
			{
				return sysService.GetBankAccountList(null).ResultData;
			}
		}

		public static List<BDAccountGroupEditModel> GetAccountGroupList()
		{
			IBDAccount sysService = ServiceHostManager.GetSysService<IBDAccount>();
			using (sysService as IDisposable)
			{
				return sysService.GetBDAccountGroupList("", null).ResultData;
			}
		}
	}
}
