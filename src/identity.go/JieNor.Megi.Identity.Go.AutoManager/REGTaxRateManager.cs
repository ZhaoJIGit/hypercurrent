using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.ServiceContract.REG;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.Identity.Go.AutoManager
{
	public class REGTaxRateManager
	{
		public static List<REGTaxRateModel> GetTaxRateList()
		{
			IREGTaxRate sysService = ServiceHostManager.GetSysService<IREGTaxRate>();
			using (sysService as IDisposable)
			{
				return sysService.GetTaxRateList(false, null).ResultData;
			}
		}
	}
}
