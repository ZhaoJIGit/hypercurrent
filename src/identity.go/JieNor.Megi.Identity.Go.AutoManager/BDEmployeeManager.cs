using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.ServiceContract.BD;
using System;

namespace JieNor.Megi.Identity.Go.AutoManager
{
	public class BDEmployeeManager
	{
		public static BDEmployeesModel GetEmployeeViewData(string id)
		{
			IBDEmployees sysService = ServiceHostManager.GetSysService<IBDEmployees>();
			using (sysService as IDisposable)
			{
				return sysService.GetEmployeesEditInfo(id, null).ResultData;
			}
		}
	}
}
