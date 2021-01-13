using JieNor.Megi.Common.Encrypt;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.Mail;
using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Identity;
using JieNor.Megi.Identity.Controllers;
using JieNor.Megi.ServiceContract.BAS;
using JieNor.Megi.ServiceContract.SEC;
using System.Web.Mvc;

namespace JieNor.Megi.Login.Web.Controllers
{
	public class InviteController : FrameworkController
	{
		private ISECPermission _perm;

		private IBASOrganisation _org;

		private ISECSendLinkInfo _send;

		private ISECUser _user;

		public InviteController(ISECPermission perm, IBASOrganisation org, ISECSendLinkInfo send, ISECUser user)
		{
			_perm = perm;
			_org = org;
			_send = send;
			_user = user;
		}

		public ActionResult ConfirmInvite(string token)
		{
			string url = "/LinkExpire/Expire";
			if (string.IsNullOrEmpty(token))
			{
				return Redirect(url);
			}
			try
			{
				string[] str = DESEncrypt.Decrypt(token).Split('|');
				if (str.Length != 3)
				{
					return Redirect(url);
				}
				SECInviteUserInfoModel userModel = _perm.GetUserInviteInfo(str[0], null).ResultData;
				if (userModel != null && !userModel.MIsTemp)
				{
					return Redirect(string.Format("{0}?token={1}&isTemp={2}", ServerHelper.LoginServer, token, userModel.MIsTemp));
				}
				return Redirect(string.Format("{0}/Account/Register?token={1}", ServerHelper.MainServer, token));
			}
			catch
			{
				return Redirect(url);
			}
		}

		public JsonResult AcceptInvite(SECInviteUserInfoModel model)
		{
			model.MPassword = MD5Service.MD5Encrypt(model.MPassword);
			OperationResult result = _perm.AcceptInvite(model, null).ResultData;
			if (result.Success && !model.MEmail.EqualsIgnoreCase(model.DefaultEmail))
			{
				string strto = model.MEmail;
				string strSubject = LangHelper.GetText(LangIndentity.CurrentLangID, LangModule.Login, "MegiUserActivation", "Hypercurrent User Activation for") + " " + model.MEmail;
				string strBody = LangHelper.GetText(LangIndentity.CurrentLangID, LangModule.Login, "ThankyouForAccepting", "Thank you for accepting the invitation to Hypercurrent.<br/>") + LangHelper.GetText(LangIndentity.CurrentLangID, LangModule.Login, "ClickTheFollowingLink", "To activate your new Hypercurrent account, please click the following link:<br/>") + "<a href='" + ServerHelper.LoginServer + "' target= '_blank'>" + ServerHelper.LoginServer + "</a>";
				SendMail.SendSMTPEMail(strto, strSubject, strBody, "Megi", "");
			}
			return base.Json(result);
		}
	}
}
