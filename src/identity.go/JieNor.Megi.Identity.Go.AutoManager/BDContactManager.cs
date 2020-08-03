using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.ServiceContract.BD;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.Identity.Go.AutoManager
{
	public class BDContactManager
	{
		public static List<BDContactsInfoModel> GetContactsInfo()
		{
			IBDContacts sysService = ServiceHostManager.GetSysService<IBDContacts>();
			using (sysService as IDisposable)
			{
				return sysService.GetContactsInfo("", "", null).ResultData;
			}
		}

		public static BDContactsInfoModel GetContactViewData(string contactID)
		{
			IBDContacts sysService = ServiceHostManager.GetSysService<IBDContacts>();
			using (sysService as IDisposable)
			{
				return sysService.GetContactViewData(contactID, null).ResultData;
			}
		}
	}
}
