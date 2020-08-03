using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.ServiceContract.BD;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.Identity.Go.AutoManager
{
	public class BDItemManager
	{
		public static List<BDItemModel> GetListByWhere(string strWhere)
		{
			IBDItem sysService = ServiceHostManager.GetSysService<IBDItem>();
			using (sysService as IDisposable)
			{
				return sysService.GetListByWhere(strWhere, null).ResultData;
			}
		}
	}
}
