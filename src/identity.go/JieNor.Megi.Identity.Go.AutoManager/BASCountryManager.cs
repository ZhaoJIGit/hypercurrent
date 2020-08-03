using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.ServiceContract.BAS;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.Identity.Go.AutoManager
{
	public class BASCountryManager
	{
		public static List<BASCountryModel> GetCountryList()
		{
			IBASCountry sysService = ServiceHostManager.GetSysService<IBASCountry>();
			using (sysService as IDisposable)
			{
				return sysService.GetCountryList(null).ResultData;
			}
		}

		public static List<BASProvinceModel> GetProvinceList(string countryId)
		{
			IBASCountry sysService = ServiceHostManager.GetSysService<IBASCountry>();
			using (sysService as IDisposable)
			{
				return sysService.GetProvinceList(countryId, null).ResultData;
			}
		}
	}
}
