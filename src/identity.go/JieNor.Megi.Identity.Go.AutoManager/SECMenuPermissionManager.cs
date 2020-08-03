using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.ServiceContract.SEC;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.Identity.Go.AutoManager
{
	public class SECMenuPermissionManager
	{
		public static List<SECMenuPermissionModel> GetGrantMenuPermissionList()
		{
			ISECPermission sysService = ServiceHostManager.GetSysService<ISECPermission>();
			using (sysService as IDisposable)
			{
				return sysService.GetGrantMenuPermissionList(null).ResultData;
			}
		}

		public static bool HavePermission(string bizObjectKey, string permissionItem, string orgId = "")
		{
			ISECPermission sysService = ServiceHostManager.GetSysService<ISECPermission>();
			using (sysService as IDisposable)
			{
				return sysService.HavePermission(bizObjectKey, permissionItem, orgId, null).ResultData;
			}
		}
	}
}
