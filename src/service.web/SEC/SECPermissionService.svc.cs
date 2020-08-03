using JieNor.Megi.BusinessContract.SEC;
using JieNor.Megi.BusinessService.SEC;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.SEC;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.SEC
{
	public class SECPermissionService : ServiceT<SECPermissionEditModel>, ISECPermission
	{
		private readonly ISECPermissionBusiness biz = new SECPermissionBusiness();

		public MActionResult<SECPermissionEditModel> GetRolePermissionEditModel(string roleID, string accessToken = null)
		{
			ISECPermissionBusiness iSECPermissionBusiness = biz;
			return base.RunFunc(iSECPermissionBusiness.GetRolePermissionEditModel, roleID, accessToken);
		}

		public MActionResult<bool> PermissionGrant(SECPermissionEditModel model, string accessToken = null)
		{
			ISECPermissionBusiness iSECPermissionBusiness = biz;
			return base.RunFunc(iSECPermissionBusiness.PermissionGrant, model, accessToken);
		}

		public MActionResult<bool> HavePermission(string bizObjectKey, string permissionItem, string orgId = "", string accessToken = null)
		{
			ISECPermissionBusiness iSECPermissionBusiness = biz;
			return base.RunFunc(iSECPermissionBusiness.HavePermission, bizObjectKey, permissionItem, orgId, accessToken);
		}

		public MActionResult<OperationResult> UserPermissionUpd(SECInviteUserInfoModel model, string accessToken = null)
		{
			ISECPermissionBusiness iSECPermissionBusiness = biz;
			return base.RunFunc(iSECPermissionBusiness.UserPermissionUpd, model, accessToken);
		}

		public MActionResult<List<SECUserPermissionListModel>> GetUserPermissionList(string accessToken = null)
		{
			ISECPermissionBusiness iSECPermissionBusiness = biz;
			return base.RunFunc(iSECPermissionBusiness.GetUserPermissionList, accessToken);
		}

		public MActionResult<DataGridJson<SECUserPermissionListModel>> GetUserPermissionPageList(SECUserPermissionListFilterModel filter, string accessToken = null)
		{
			ISECPermissionBusiness iSECPermissionBusiness = biz;
			return base.RunFunc(iSECPermissionBusiness.GetUserPermissionPageList, filter, accessToken);
		}

		public MActionResult<bool> IsAdminUser(string userId, string accessToken = null)
		{
			ISECPermissionBusiness iSECPermissionBusiness = biz;
			return base.RunFunc(iSECPermissionBusiness.IsAdminUser, userId, accessToken);
		}

		public MActionResult<SECInviteUserInfoModel> GetUserEditInfo(SECInviteUserInfoModel model, string accessToken = null)
		{
			ISECPermissionBusiness iSECPermissionBusiness = biz;
			return base.RunFunc(iSECPermissionBusiness.GetUserEditInfo, model, accessToken);
		}

		public MActionResult<bool> DeleteUserLinkInfo(string userId, string accessToken = null)
		{
			ISECPermissionBusiness iSECPermissionBusiness = biz;
			return base.RunFunc(iSECPermissionBusiness.DeleteUserLinkInfo, userId, accessToken);
		}

		public MActionResult<SECInviteUserInfoModel> GetUserInviteInfo(string userId, string accessToken = null)
		{
			ISECPermissionBusiness iSECPermissionBusiness = biz;
			return base.RunFunc(iSECPermissionBusiness.GetUserInviteInfo, userId, accessToken);
		}

		public MActionResult<OperationResult> AcceptInvite(SECInviteUserInfoModel model, string accessToken = null)
		{
			ISECPermissionBusiness iSECPermissionBusiness = biz;
			return base.RunFunc(iSECPermissionBusiness.AcceptInvite, model, accessToken);
		}

		public MActionResult<bool> LoginForAcceptInvite(string userId, string orgId, string sendLinkID, string newUserId, string loginEmail, string accessToken = null)
		{
			ISECPermissionBusiness iSECPermissionBusiness = biz;
			return base.RunFunc(iSECPermissionBusiness.LoginForAcceptInvite, userId, orgId, sendLinkID, newUserId, loginEmail, accessToken);
		}

		public MActionResult<List<SECMenuPermissionModel>> GetGrantMenuPermissionList(string accessToken = null)
		{
			ISECPermissionBusiness iSECPermissionBusiness = biz;
			return base.RunFunc(iSECPermissionBusiness.GetGrantMenuPermissionList, accessToken);
		}

		public MActionResult<bool> ContainPermGrp(string grpKey, string accessToken = null)
		{
			ISECPermissionBusiness iSECPermissionBusiness = biz;
			return base.RunFunc(iSECPermissionBusiness.ContainPermGrp, grpKey, accessToken);
		}

		public MActionResult<List<SECUserRoleInitModel>> GetUserRoleInitModel(string accessToken = null)
		{
			ISECPermissionBusiness iSECPermissionBusiness = biz;
			return base.RunFunc(iSECPermissionBusiness.GetUserRoleInitModel, accessToken);
		}

		public MActionResult<OperationResult> ArchiveUser(string Ids, string accessToken = null)
		{
			ISECPermissionBusiness iSECPermissionBusiness = biz;
			return base.RunFunc(iSECPermissionBusiness.ArchiveUser, Ids, accessToken);
		}
	}
}
