using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.SEC
{
	[ServiceContract]
	public interface ISECPermission
	{
		[OperationContract]
		[FaultContract(typeof(DivideByZeroException))]
		MActionResult<SECPermissionEditModel> GetRolePermissionEditModel(string roleID, string accessToken = null);

		[OperationContract]
		MActionResult<bool> PermissionGrant(SECPermissionEditModel model, string accessToken = null);

		[OperationContract]
		MActionResult<bool> HavePermission(string bizObjectKey, string permissionItem, string orgId = "", string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> UserPermissionUpd(SECInviteUserInfoModel model, string accessToken = null);

		[OperationContract]
		MActionResult<List<SECUserPermissionListModel>> GetUserPermissionList(string accessToken = null);

		[OperationContract]
		MActionResult<bool> IsAdminUser(string userId, string accessToken = null);

		[OperationContract]
		MActionResult<DataGridJson<SECUserPermissionListModel>> GetUserPermissionPageList(SECUserPermissionListFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<SECInviteUserInfoModel> GetUserEditInfo(SECInviteUserInfoModel model, string accessToken = null);

		[OperationContract]
		MActionResult<bool> DeleteUserLinkInfo(string userId, string accessToken = null);

		[OperationContract]
		MActionResult<SECInviteUserInfoModel> GetUserInviteInfo(string userId, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> AcceptInvite(SECInviteUserInfoModel model, string accessToken = null);

		[OperationContract]
		MActionResult<bool> LoginForAcceptInvite(string userId, string orgId, string sendLinkID, string newUserId, string loginEmail, string accessToken = null);

		[OperationContract]
		MActionResult<List<SECMenuPermissionModel>> GetGrantMenuPermissionList(string accessToken = null);

		[OperationContract]
		MActionResult<bool> ContainPermGrp(string grpKey, string accessToken = null);

		[OperationContract]
		MActionResult<List<SECUserRoleInitModel>> GetUserRoleInitModel(string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> ArchiveUser(string Ids, string accessToken = null);
	}
}
