using JieNor.Megi.Identity.Go.AutoManager;

namespace JieNor.Megi.Identity.Go.HtmlHelper
{
	public static class HtmlSECMenu
	{
		public static bool HavePermission(string bizObjectKey, string permissionItem, string orgId = "")
		{
			return SECMenuPermissionManager.HavePermission(bizObjectKey, permissionItem, orgId);
		}
	}
}
