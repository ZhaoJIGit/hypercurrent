using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.SEC
{
	public interface ISECPermissionBusiness
	{
		SECPermissionEditModel GetRolePermissionEditModel(MContext ctx, string roleID);

		bool PermissionGrant(MContext ctx, SECPermissionEditModel model);

		bool HavePermission(MContext ctx, string bizObjectKey, string permissionItem, string orgId = "");

		OperationResult UserPermissionUpd(MContext ctx, SECInviteUserInfoModel model);

		List<SECUserPermissionListModel> GetUserPermissionList(MContext ctx);

		DataGridJson<SECUserPermissionListModel> GetUserPermissionPageList(MContext ctx, SECUserPermissionListFilterModel filter);

		bool IsAdminUser(MContext ctx, string userId);

		SECInviteUserInfoModel GetUserEditInfo(MContext ctx, SECInviteUserInfoModel model);

		bool DeleteUserLinkInfo(MContext ctx, string userId);

		SECInviteUserInfoModel GetUserInviteInfo(MContext ctx, string userId);

		OperationResult AcceptInvite(MContext ctx, SECInviteUserInfoModel model);

		bool LoginForAcceptInvite(MContext ctx, string userId, string orgId, string sendLinkID, string newUserId, string loginEmail);

		List<SECMenuPermissionModel> GetGrantMenuPermissionList(MContext ctx);

		bool ContainPermGrp(MContext ctx, string grpKey);

		List<SECUserRoleInitModel> GetUserRoleInitModel(MContext ctx);

		OperationResult ArchiveUser(MContext ctx, string Ids);
	}
}
