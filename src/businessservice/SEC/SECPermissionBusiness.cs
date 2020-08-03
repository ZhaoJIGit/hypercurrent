using JieNor.Megi.BusinessContract.SEC;
using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.MultiLanguage;
using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.SEC;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JieNor.Megi.BusinessService.SEC
{
	public class SECPermissionBusiness : ISECPermissionBusiness
	{
		public SECPermissionEditModel GetRolePermissionEditModel(MContext ctx, string roleID)
		{
			return SECPermissionRepository.GetRolePermissionEditModel(ctx, roleID);
		}

		public bool PermissionGrant(MContext ctx, SECPermissionEditModel model)
		{
			return SECPermissionRepository.PermissionGrant(ctx, model);
		}

		public bool HavePermission(MContext ctx, string bizObjectKey, string permissionItem, string orgId = "")
		{
			if (bizObjectKey.Equals("Report"))
			{
				return SECPermissionRepository.HavePermission(ctx, "Sale_Reports", permissionItem, "") || SECPermissionRepository.HavePermission(ctx, "Purchase_Reports", permissionItem, "") || SECPermissionRepository.HavePermission(ctx, "Expense_Reports", permissionItem, "") || SECPermissionRepository.HavePermission(ctx, "Bank_Reports", permissionItem, "") || SECPermissionRepository.HavePermission(ctx, "PayRun_Reports", permissionItem, "") || SECPermissionRepository.HavePermission(ctx, "Other_Reports", permissionItem, "") || SECPermissionRepository.HavePermission(ctx, "Fixed_Assets_Reports", permissionItem, "");
			}
			bool result = SECPermissionRepository.HavePermission(ctx, bizObjectKey, permissionItem, orgId);
			if (bizObjectKey.Equals("Bank"))
			{
				result = (SECPermissionRepository.HavePermission(ctx, "Bank_Reconciliation", permissionItem, "") || SECPermissionRepository.HavePermission(ctx, "BankAccount", permissionItem, ""));
			}
			return result;
		}

		public OperationResult UserPermissionUpd(MContext ctx, SECInviteUserInfoModel model)
		{
			OperationResult operationResult = new OperationResult();
			if (model != null && !string.IsNullOrWhiteSpace(model.MEmail))
			{
				model.MEmail = model.MEmail.Trim();
				if (!string.IsNullOrWhiteSpace(model.MItemID))
				{
					SECUserRepository sECUserRepository = new SECUserRepository();
					SECUserModel userModel = sECUserRepository.GetUserModel(ctx, model.MItemID, false);
					if (userModel != null && userModel.MEmailAddress.Trim().ToLower() != model.MEmail.Trim().ToLower())
					{
						operationResult.Success = false;
						operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "EamilNotEqual", "当前填写的邮箱与原账号的邮箱不一致，修改用户信息时，不允许修改邮箱！");
						return operationResult;
					}
				}
				return SECPermissionRepository.UserPermissionUpd(ctx, model);
			}
			operationResult.Success = false;
			return operationResult;
		}

		public List<SECUserPermissionListModel> GetUserPermissionList(MContext ctx)
		{
			return SECPermissionRepository.GetUserPermissionList(ctx);
		}

		public DataGridJson<SECUserPermissionListModel> GetUserPermissionPageList(MContext ctx, SECUserPermissionListFilterModel filter)
		{
			return SECPermissionRepository.GetUserPermissionPageList(ctx, filter);
		}

		public bool IsAdminUser(MContext ctx, string userId)
		{
			SECUserPermissionListModel userPermissionInfo = SECPermissionRepository.GetUserPermissionInfo(ctx, userId);
			return userPermissionInfo.MRole.EqualsIgnoreCase("Admin");
		}

		public SECInviteUserInfoModel GetUserEditInfo(MContext ctx, SECInviteUserInfoModel model)
		{
			return SECPermissionRepository.GetUserEditInfo(ctx, model);
		}

		public bool DeleteUserLinkInfo(MContext ctx, string userId)
		{
			bool flag = SECPermissionRepository.DeleteUserLinkInfo(ctx, userId, true);
			if (flag)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("_id", userId);
				dictionary.Add("MOrgID", ctx.MOrgID);
				ContextHelper.RemoveMContext(dictionary);
			}
			return flag;
		}

		[NoAuthorization]
		public SECInviteUserInfoModel GetUserInviteInfo(MContext ctx, string userId)
		{
			return SECPermissionRepository.GetUserInviteInfo(ctx, userId);
		}

		[NoAuthorization]
		public OperationResult AcceptInvite(MContext ctx, SECInviteUserInfoModel model)
		{
			return SECPermissionRepository.AcceptInvite(ctx, model);
		}

		[NoAuthorization]
		public bool LoginForAcceptInvite(MContext ctx, string userId, string orgId, string sendLinkID, string newUserId, string loginEmail)
		{
			bool flag = SECPermissionRepository.LoginForAcceptInvite(ctx, userId, orgId, sendLinkID, newUserId, loginEmail);
			if (flag && !ctx.MExistsOrg)
			{
				ContextHelper.UpdateMContextByKeyField("_id", newUserId, "MExistsOrg", true, true);
			}
			return flag;
		}

		public List<SECMenuPermissionModel> GetGrantMenuPermissionList(MContext ctx)
		{
			return SECPermissionRepository.GetGrantMenuPermissionList(ctx);
		}

		public bool ContainPermGrp(MContext ctx, string grpKey)
		{
			return SECPermissionRepository.ContainPermGrp(ctx, grpKey, "");
		}

		public List<SECUserRoleInitModel> GetUserRoleInitModel(MContext ctx)
		{
			return SECPermissionRepository.GetUserRoleInitModel(ctx);
		}

		public OperationResult ArchiveUser(MContext ctx, string ids)
		{
			OperationResult operationResult = new OperationResult();
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.In("MItemID", ids.Split(',').ToList());
			List<SECUserModel> userList = new SECUserRepository().GetUserList(ctx, sqlWhere);
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			List<SECUserModel> list = new List<SECUserModel>();
			foreach (SECUserModel item in userList)
			{
				string userName = GlobalFormat.GetUserName(item.MFirstName, item.MLastName, ctx);
				if (item.MEmailAddress == ctx.MEmail)
				{
					stringBuilder2.AppendLine(userName + ":" + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "CanNotArchiveYourSelf", "This user is yours，can not archive"));
				}
				else
				{
					SECInviteUserInfoModel sECInviteUserInfoModel = new SECInviteUserInfoModel();
					sECInviteUserInfoModel.MItemID = item.MItemID;
					sECInviteUserInfoModel = SECPermissionRepository.GetUserEditInfo(ctx, sECInviteUserInfoModel);
					if (sECInviteUserInfoModel != null && !string.IsNullOrEmpty(sECInviteUserInfoModel.MItemID))
					{
						if (sECInviteUserInfoModel.MPermStatus == "Pending")
						{
							stringBuilder2.AppendLine(userName + ":" + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "AccountIsNotActive", "User is not active"));
						}
						else
						{
							int status = (!SECPermissionRepository.UserIsArchiveInOrg(ctx, item.MItemID)) ? 1 : 0;
							operationResult = SECPermissionRepository.ArchiveUser(ctx, item.MItemID, status);
							stringBuilder.AppendLine(userName);
						}
					}
					else
					{
						stringBuilder2.AppendLine(userName + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "UserIsNotExist", "Can not find this user in Megi system"));
					}
				}
			}
			operationResult.Success = true;
			operationResult.Message = "";
			if (!string.IsNullOrWhiteSpace(stringBuilder.ToString()))
			{
				OperationResult operationResult2 = operationResult;
				operationResult2.Message += COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "UserEnabled/DisabledSuccessfully", "Successfully enable / disable the following account:</br>");
				OperationResult operationResult3 = operationResult;
				operationResult3.Message += stringBuilder.ToString();
			}
			if (!string.IsNullOrWhiteSpace(stringBuilder2.ToString()))
			{
				operationResult.Success = false;
				OperationResult operationResult4 = operationResult;
				operationResult4.Message += COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "UserEnabled/DisabledFail", "Enable / disable the following account failed:</br>");
				OperationResult operationResult5 = operationResult;
				operationResult5.Message += stringBuilder2.ToString();
			}
			return operationResult;
		}
	}
}
