using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.Core;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.ServiceContract.BAS;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.Identity.AutoManager
{
	public class BASCountryRegionManager
	{
		public static List<BASCountryRegionModel> GetOrgInfoListByUserID(SqlWhere filter)
		{
			IBASCountryRegion sysService = ServiceHostManager.GetSysService<IBASCountryRegion>();
			using (sysService as IDisposable)
			{
				return sysService.GetModelList(filter, false, null).ResultData;
			}
		}

		public static List<BASCountryRegionModel> GetListByWhere(SqlWhere filter)
		{
			IBASCountryRegion sysService = ServiceHostManager.GetSysService<IBASCountryRegion>();
			using (sysService as IDisposable)
			{
				return sysService.GetModelList(filter, false, null).ResultData;
			}
		}
	}
}
