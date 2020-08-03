using JieNor.Megi.Common.Encrypt;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.Mail;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.Identity;
using JieNor.Megi.Identity.Attribute;
using JieNor.Megi.Identity.HtmlHelper;
using JieNor.Megi.ServiceContract.BAS;
using JieNor.Megi.ServiceContract.SEC;
using System;
using System.Configuration;
using System.Text;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.BD.Controllers
{
	public class UserController : GoControllerBase
	{
		private ISECPermission _perm = null;

		private IBASOrganisation _org = null;

		private ISECSendLinkInfo _send = null;

		private ISECUserLoginLog UserLoginLogService = null;

		private ISECUser UserService = null;

		public UserController(ISECPermission perm, IBASOrganisation org, ISECSendLinkInfo send, ISECUserLoginLog userLoginLogService, ISECUser userService)
		{
			_perm = perm;
			_org = org;
			_send = send;
			UserLoginLogService = userLoginLogService;
			UserService = userService;
			base.SetModule("setting");
		}

		[Permission("User", "View", "")]
		public ActionResult UserList()
		{
			base.SetTitleAndCrumb(LangHelper.GetText(LangModule.User, "Users", "Users"), "<a href='/Setting/'>" + LangHelper.GetText(LangModule.User, "GeneralSettings", "General Settings") + " > </a>");
			return base.View();
		}

		[Permission("User", "Change", "")]
		public ActionResult UserEdit(string id)
		{
			if (!string.IsNullOrWhiteSpace(id))
			{
				base.ViewData["ItemId"] = id;
				base.ViewData["IsCurrentUser"] = ((id == HtmlUser.UserID) ? "1" : "0");
			}
			else
			{
				base.ViewData["IsCurrentUser"] = "0";
				string text = LangHelper.GetText(LangModule.User, "InviteAUser", "Invite a User");
				base.SetTitleAndCrumb(text, "<a href='/Setting/'>" + LangHelper.GetText(LangModule.User, "GeneralSettings", "General Settings") + " > </a><a href='/Setting/User/UserList'>" + LangHelper.GetText(LangModule.User, "UserList", "UserList") + " > </a>");
			}
			return base.View();
		}

		[Permission("User", "Change", "")]
		public ActionResult UserInvite(string id, bool isResent = false)
		{
			SECInviteUserInfoModel resultData = _perm.GetUserInviteInfo(id, null).ResultData;
			if (resultData != null)
			{
				base.ViewData["UserId"] = resultData.MItemID;
				base.ViewData["FirstName"] = resultData.MFirstName;
				base.ViewData["LastName"] = resultData.MLastName;
				base.ViewData["Email"] = resultData.MEmail;
				BASOrganisationModel resultData2 = _org.GetDataModel(base.MContext.MOrgID, false, null).ResultData;
				base.ViewData["OrgId"] = base.MContext.MOrgID;
				if (resultData2 != null)
				{
					base.ViewData["OrgName"] = resultData2.MName;
				}
				base.ViewData["IsResent"] = isResent;
			}
			return base.View();
		}

		[Permission("User", "Change", "")]
		public JsonResult UserPermissionUpd(SECInviteUserInfoModel model)
		{
			return base.Json(_perm.UserPermissionUpd(model, null));
		}

		[Permission("User", "View", "")]
		public JsonResult GetUserPermissionList(SECUserPermissionListFilterModel param)
		{
			MActionResult<DataGridJson<SECUserPermissionListModel>> userPermissionPageList = _perm.GetUserPermissionPageList(param, null);
			return base.Json(userPermissionPageList);
		}

		[Permission("User", "View", "")]
		public JsonResult GetUserEditInfo(SECInviteUserInfoModel model)
		{
			return base.Json(_perm.GetUserEditInfo(model, null));
		}

		[Permission("User", "Change", "")]
		public JsonResult UserLinkInfoDelete(SECInviteUserInfoModel model)
		{
			return base.Json(_perm.DeleteUserLinkInfo(model.MItemID, null));
		}

		[Permission("User", "Change", "")]
		public JsonResult UserInviteSendMeg(string UserId, string Email, string OrgId, string OrgName, string Message)
		{
			SECInviteUserInfoModel resultData = _perm.GetUserInviteInfo(base.MContext.MUserID, null).ResultData;
			string guid = UUIDHelper.GetGuid();
			string arg = DESEncrypt.Encrypt(UserId + "|" + OrgId + "|" + guid);
			string strSubject = LangHelper.GetText(LangModule.User, "InvitationToJoin", "Invitation to join ") + OrgName;
			Message = Message.Replace("\n", "<br>");
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("<i><span style='font-size: 20pt;font-family: 'Arial Unicode MS', sans-serif;'>{0}</span></i><br><br>", LangHelper.GetText(LangModule.User, "JoinYourColleagues", "Join Your Colleagues on Megi"));
			stringBuilder.Append(Message);
			stringBuilder.Append(LangHelper.GetText(LangModule.User, "ToAcceptThisInvitation", "To accept this invitation please use the following link:"));
			stringBuilder.Append("<br><br>");
			stringBuilder.AppendFormat("<a href='{0}/Invite/ConfirmInvite?token={1}' target= '_blank'>{0}/Invite/ConfirmInvite?token={1}</a><br><br>", ServerHelper.LoginServer, arg);
			stringBuilder.Append("<i>" + LangHelper.GetText(LangModule.Common, "TheMegiTeam", "The Megi Team") + "</i><br><br><br><br>");
			stringBuilder.Append("<span style='font-size:12px'>" + LangHelper.GetText(LangModule.Common, "NeedHelp", "Need help? Contact us at ") + "<a href='mailto:" + ConfigurationManager.AppSettings["ServiceEmail"] + "'>" + ConfigurationManager.AppSettings["ServiceEmail"] + "</a></span>");
			SendMail.SendSMTPEMail(Email, strSubject, stringBuilder.ToString(), resultData.MFirstName + " " + resultData.MLastName, "");
			SECSendLinkInfoModel sECSendLinkInfoModel = new SECSendLinkInfoModel();
			sECSendLinkInfoModel.MItemID = guid;
			sECSendLinkInfoModel.MSendDate = DateTime.Now;
			sECSendLinkInfoModel.MInvitationOrgID = OrgId;
			sECSendLinkInfoModel.MInvitationEmail = resultData.MEmail;
			sECSendLinkInfoModel.MLinkType = 2;
			sECSendLinkInfoModel.MEmail = Email;
			SECInviteUserInfoModel resultData2 = _perm.GetUserInviteInfo(UserId, null).ResultData;
			sECSendLinkInfoModel.MFirstName = ((resultData2 == null) ? "" : resultData2.MFirstName);
			sECSendLinkInfoModel.MLastName = ((resultData2 == null) ? "" : resultData2.MLastName);
			sECSendLinkInfoModel.MPhone = "";
			_send.InsertLink(sECSendLinkInfoModel, null);
			return base.Json(true);
		}

		[Permission("User", "View", "")]
		public JsonResult GetUserRoleList()
		{
			return base.Json(_perm.GetUserRoleInitModel(null));
		}

		[Permission("User", "View", "")]
		public JsonResult GetUserActivityList(string orgId, SECUserLoginLogListFilter param)
		{
			MActionResult<DataGridJson<SECUserLoginLogModel>> userLoginLogPageListByOrgId = UserLoginLogService.GetUserLoginLogPageListByOrgId(orgId, param, null);
			return base.Json(userLoginLogPageListByOrgId);
		}

		[Permission("User", "View", "")]
		public JsonResult ArchiveUser(string userId)
		{
			MActionResult<OperationResult> data = _perm.ArchiveUser(userId, null);
			return base.Json(data);
		}

		[Permission("User", "View", "")]
		public JsonResult GetUserInfo(string email)
		{
			MActionResult<SECUserModel> modelByEmail = UserService.GetModelByEmail(email, null);
			return base.Json(modelByEmail);
		}

		public ActionResult NotAllowAccess()
		{
			return base.View();
		}
	}
}
