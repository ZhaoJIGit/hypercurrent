using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.ServiceContract.BAS;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.Identity.AutoManager
{
	public class BASOrganisationManager
	{
		public static List<BASMyHomeModel> GetOrgInfoListByUserID()
		{
			IBASMyHome sysService = ServiceHostManager.GetSysService<IBASMyHome>();
			using (sysService as IDisposable)
			{
				return sysService.GetOrgInfoListByUserID(null).ResultData;
			}
		}

		public static void ChangeOrgId(string orgId)
		{
			IBASOrganisation sysService = ServiceHostManager.GetSysService<IBASOrganisation>();
			using (sysService as IDisposable)
			{
				sysService.ChangeOrgById(orgId, null);
			}
		}
	}
}
