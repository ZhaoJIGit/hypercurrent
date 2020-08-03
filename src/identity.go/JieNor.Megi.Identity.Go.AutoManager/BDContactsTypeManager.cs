using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.ServiceContract.BD;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.Identity.Go.AutoManager
{
	public class BDContactsTypeManager
	{
		public static List<BDContactsTypeLModel> GetTypeListByWhere(bool isAll)
		{
			IBDContacts sysService = ServiceHostManager.GetSysService<IBDContacts>();
			using (sysService as IDisposable)
			{
				return sysService.GetTypeListByWhere(isAll, null).ResultData;
			}
		}
	}
}
