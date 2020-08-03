using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.ServiceContract.BD;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.Identity.Go.AutoManager
{
	public class BDOrgUserManager
	{
		public static List<BDEmployeesModel> GetOrgUserList()
		{
			IBDEmployees sysService = ServiceHostManager.GetSysService<IBDEmployees>();
			using (sysService as IDisposable)
			{
				return sysService.GetOrgUserList(null).ResultData;
			}
		}
	}
}
