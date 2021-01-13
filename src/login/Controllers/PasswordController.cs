using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.Encrypt;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.Mail;
using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Identity;
using JieNor.Megi.Identity.Controllers;
using JieNor.Megi.ServiceContract.SEC;
using System;
using System.Configuration;
using System.Text;
using System.Web.Mvc;

namespace JieNor.Megi.Login.Web.Controllers
{
	public class PasswordController : FrameworkController
	{
		private readonly ISECUser _acct;

		private readonly ISECSendLinkInfo _send;

		public PasswordController(ISECUser acct, ISECSendLinkInfo send)
		{
			_acct = acct;
			_send = send;
		}

		public ActionResult Create(string id)
		{
			string token = DESEncrypt.Decrypt(id);
			try
			{
				if (!_send.IsValidLink(token, null).ResultData)
				{
					return Redirect("/LinkExpire/Expire");
				}
			}
			catch
			{
				return Redirect("/LinkExpire/Expire");
			}
			SECSendLinkInfoModel linkModel = _send.GetModel(token.Trim(), null).ResultData;
			SECUserModel userModel = _acct.GetUserModel(linkModel.MEmail, null).ResultData;
			if (userModel != null && !userModel.MIsTemp)
			{
				ViewBag.Result = LangHelper.GetText(LangIndentity.CurrentLangID, LangModule.Login, "AccountHadActive", "Your account has been activated!");

				var detail = LangHelper.GetText(LangIndentity.CurrentLangID, LangModule.Login, "AccountHadActiveDetail", "Please click <a href='{0}'>Here</a> login Hypercurrent System");
				ViewBag.Detail = string.Format(detail, ServerHelper.LoginServer);

				ViewBag.Type = 0;

				return View("Success");
			}
			SECAccountModel info = new SECAccountModel();
			info.MEmailAddress = linkModel.MEmail;
			info.MFristName = JieNor.Megi.Common.Utility.HtmlHelper.Encode(linkModel.MFirstName);
			info.MLastName = JieNor.Megi.Common.Utility.HtmlHelper.Encode(linkModel.MLastName);
			info.MMobilePhone = (string.IsNullOrEmpty(linkModel.MPhone) ? "" : linkModel.MPhone);
			base.ViewData["info"] = info;
			base.ViewData["SendLinkID"] = token;
			return View();
		}

		public ActionResult CreateSuccess()
		{
			string email = System.Web.HttpContext.Current.Request["email"];
			if (!string.IsNullOrEmpty(email))
			{
				ServerHelper.DefaultUserEmail = email;
			}
			return View();
		}

		public ActionResult ForgetPwd()
		{
			return View();
		}

		public JsonResult ForgotPwdAndSendMail(SECUserModel model)
		{
			OperationResult result = new OperationResult();
			model.MEmailAddress = model.MEmailAddress.ToLower();
			model.MLCID = CookieHelper.GetCookieValue(ContextHelper.MLocaleIDCookie, null);
			if (_acct.IsExistEmail(model.MEmailAddress, null).ResultData)
			{
				string linkId = UUIDHelper.GetGuid();
				string strto = model.MEmailAddress;
				string strSubject = LangHelper.GetText(LangIndentity.CurrentLangID, LangModule.Login, "ForgottenPassword", "Reset your megi password");
				string token = DESEncrypt.Encrypt(model.MEmailAddress + "|" + linkId);
				StringBuilder strBody = new StringBuilder();
				strBody.Append("<i><span style='font-size: 20pt;font-family: 'Arial Unicode MS', sans-serif;'>");
				strBody.Append(LangHelper.GetText(LangIndentity.CurrentLangID, LangModule.Login, "ThereHasBeenARequestTo", "You've successfully requested to reset your password"));
				strBody.Append("</span></i><br><br>");
				strBody.Append(LangHelper.GetText(LangIndentity.CurrentLangID, LangModule.Login, "ThereHasBeenARequestTo2", "Click this link below to reset:"));
				strBody.Append("<br><br>");
				strBody.AppendFormat("<a href='{0}' target= '_blank'>{0}</a><br><br>", ServerHelper.LoginServer + "/Password/ResetPwd/" + token);
				strBody.AppendFormat(LangHelper.GetText(LangIndentity.CurrentLangID, LangModule.Login, "NotRequestToResetPassword", "If you did not request to change your password,please email <a href='mailto:{0}'>{0}</a>.") + "<br><br>", ConfigurationManager.AppSettings["ServiceEmail"]);
				strBody.Append("<i>" + LangHelper.GetText(LangIndentity.CurrentLangID, LangModule.Common, "TheMegiTeam", "The Hypercurrent Team") + "</i><br><br><br><br>");
				strBody.Append("<span style='font-size:12px'>" + LangHelper.GetText(LangIndentity.CurrentLangID, LangModule.Common, "NeedHelp", "Need help? Contact us at ") + "<a href='mailto:" + ConfigurationManager.AppSettings["ServiceEmail"] + "'>" + ConfigurationManager.AppSettings["ServiceEmail"] + "</a></span>");
				SendMail.SendSMTPEMail(strto, strSubject, strBody.ToString(), "Megi", "");
				SECSendLinkInfoModel linkModel = new SECSendLinkInfoModel();
				linkModel.MItemID = linkId;
				linkModel.MSendDate = DateTime.Now;
				linkModel.MLinkType = 4;
				linkModel.MEmail = model.MEmailAddress;
				linkModel.MFirstName = model.MFirstName;
				linkModel.MLastName = model.MLastName;
				linkModel.MPhone = model.MMobilePhone;
				_send.InsertLink(linkModel, null);
			}
			else
			{
				result.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = LangHelper.GetText(LangIndentity.CurrentLangID, LangModule.Login, "NotFoundEmail", "No account found with that email address!")
				});
			}
			return base.Json(result);
		}

		public ActionResult ResetPwd(string id)
		{
			string[] str = DESEncrypt.Decrypt(id).Split('|');
			if (!_send.IsValidLink(str[1], null).ResultData)
			{
				return Redirect("/LinkExpire/Expire");
			}
			ViewData["email"] = str[0];
			ViewData["SendLinkID"] = str[1];
			return base.View();
		}

		public JsonResult PutNewPwd(SECUserModel model)
		{
			return Json(_acct.PutNewPwd(model, null));
		}

		public JsonResult SureRegister(SECAccountModel model)
		{
			return Json(_acct.SureRegister(model, null));
		}

		public ActionResult OperateSuccess(int type = 1)
		{
			var email = System.Web.HttpContext.Current.Request["email"];
			if (!string.IsNullOrEmpty(email))
			{
				ServerHelper.DefaultUserEmail = email;
			}
			if (type == 1)
			{
				ViewBag.Result = LangHelper.GetText(LangIndentity.CurrentLangID, LangModule.Login, "CheckYourInbox", "Check your inbox!");

				ViewBag.Detail = LangHelper.GetText(LangIndentity.CurrentLangID, LangModule.Login, "CheckEmailToResetPassword", "We've sent you an email, please click on the link in the email to reset your password.");
			}
			if (type == 2)
			{
				ViewBag.Result = LangHelper.GetText(LangIndentity.CurrentLangID, LangModule.Login, "ResetPasswordSucess", "Password reset successfully!");

				var langString2 = LangHelper.GetText(LangIndentity.CurrentLangID, LangModule.Login, "ForgetResetSucess_x", "Please click <a href='{0}'>Here</a> login Hypercurrent System");
				ViewBag.Detail = string.Format(langString2, ServerHelper.LoginServer);
			}
			if (type == 3)
			{
				ViewBag.Result = LangHelper.GetText(LangIndentity.CurrentLangID, LangModule.Login, "CreatePasswordSuccess", "Congratulations, create password successfully!");

				var langString = LangHelper.GetText(LangIndentity.CurrentLangID, LangModule.Login, "AccountHadActiveDetail_x", "Please click <a href='{0}'>Here</a> login Hypercurrent System");
				ViewBag.Detail = string.Format(langString, ServerHelper.LoginServer);
			}
			return View("Success");
		}

		public ActionResult ResetPwdSuccess()
		{
			string email = System.Web.HttpContext.Current.Request["email"];
			if (!string.IsNullOrEmpty(email))
			{
				ServerHelper.DefaultUserEmail = email;
			}
			return base.View();
		}
	}
}
