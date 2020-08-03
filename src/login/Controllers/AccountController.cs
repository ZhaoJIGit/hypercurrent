using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.Encrypt;
using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.EntityModel.SEC;
using JieNor.Megi.Identity.Controllers;
using JieNor.Megi.ServiceContract.BAS;
using JieNor.Megi.ServiceContract.SEC;
using JieNor.Megi.Tools;
using System;
using System.Web;
using System.Web.Mvc;

namespace JieNor.Megi.Login.Web.Controllers
{
	public class AccountController : FrameworkController
	{
		private ISECUser _user;

		private readonly ISECUserAccount _userAccount;

		private readonly ISECPermission _perm;

		private readonly ISECUserLoginLog loginLogService;

		private readonly ISECSendLinkInfo _send;

		private IBASOrganisation _org;

		public AccountController(ISECUserAccount uacct, ISECPermission perm, ISECUserLoginLog logService, ISECSendLinkInfo send, IBASOrganisation org, ISECUser user)
		{
			_userAccount = uacct;
			_perm = perm;
			loginLogService = logService;
			_send = send;
			_org = org;
			_user = user;
		}

		public ActionResult Index(string token, string RedirectUrl, string MUserEmail, string ForceToLogin, string Relogin, string cgtkn)
		{
			string linkExpireUrl = "/LinkExpire/Expire";
			bool isInvite = !string.IsNullOrEmpty(token);
			if (isInvite)
			{
				try
				{
					string[] str = DESEncrypt.Decrypt(token).Split('|');
					if (str.Length == 3)
					{
						if (!_send.IsValidLink(str[2], null).ResultData)
						{
							return Redirect(linkExpireUrl);
						}
						SECInviteUserInfoModel userInfo = _perm.GetUserInviteInfo(str[0], null).ResultData;
						BASOrganisationModel sysOrgModel = _org.GetModelById(str[1], null).ResultData;
						base.ViewData["userID"] = str[0];
						base.ViewData["orgId"] = str[1];
						base.ViewData["orgName"] = sysOrgModel.MName;
						base.ViewData["userInfo"] = userInfo;
						base.ViewData["SendLinkID"] = str[2];
						base.ViewData["InviteEmail"] = userInfo.MEmail;
					}
				}
				catch
				{
					return Redirect(linkExpireUrl);
				}
			}
			base.ViewData["RedirectUrl"] = HttpUtility.UrlDecode(RedirectUrl);
			base.ViewData["ForceToLogin"] = ForceToLogin;
			base.ViewData["Relogin"] = Relogin;
			base.ViewData["MUserEmail"] = MUserEmail;
			if (!isInvite)
			{
				SetViewDataEmail();
			}
			base.ViewData["LoginErrorCount"] = LoginHelper.GetLoginErrorCount(MUserEmail);
			base.ViewData["AllowLoginErrorCount"] = LoginHelper.GetAllowLoginErrorCount();
			if (!IsPassByEmailChange(cgtkn))
			{
				return Redirect("/LinkExpire/Expire");
			}
			return base.View();
		}

		public ActionResult CreateValidateCodeImage()
		{
			return base.File(LoginHelper.CreateValidateCodeImage(base.HttpContext), "image/png");
		}

		[HttpPost]
		public ActionResult ValidateCodeIsCorrect(string email, string code)
		{
			return base.Json(LoginHelper.JudgeValidateCode(base.HttpContext, email, code));
		}

		public ActionResult LoginBox(string orgid)
		{
			base.ViewData["email"] = ContextHelper.MUserEmail;
			base.ViewData["orgid"] = orgid;
			return base.View();
		}

		private void SetViewDataEmail()
		{
			string email = CookieHelper.GetCookieValue(ContextHelper.MUserEmailCookie, null);
			string historyEmail = CookieHelper.GetCookieValue(ContextHelper.MHistoryEmailCookie, null);
			string.IsNullOrEmpty(email);
			base.ViewData["Email"] = (email ?? string.Empty);
			base.ViewData["HE"] = (string.IsNullOrWhiteSpace(historyEmail) ? email : MText.Base64Encode(historyEmail));
		}

		[HttpPost]
		public ActionResult SignIn(SECLoginModel model)
		{
			try
			{
				SECLoginResultModel result = LoginHelper.SignIn(model, _userAccount, _perm, loginLogService, base.HttpContext);
				return Json(result);
			}
			catch (Exception)
			{
				throw;
			}
		}

		public ActionResult Logout()
		{
			LoginHelper.SignOut(base.HttpContext);
			SetViewDataEmail();
			base.ViewData["LoginErrorCount"] = 0;
			base.ViewData["AllowLoginErrorCount"] = LoginHelper.GetAllowLoginErrorCount();
			return base.View("Index");
		}

		private bool IsPassByEmailChange(string token)
		{
			ISECAccount _secAcc = ServiceHostManager.GetSysService<ISECAccount>();
			using (_secAcc as IDisposable)
			{
				if (string.IsNullOrWhiteSpace(token))
				{
					return true;
				}
				string[] str = DESEncrypt.Decrypt(token).Split(new char[4]
				{
					'|',
					'#',
					'|',
					'$'
				}, StringSplitOptions.RemoveEmptyEntries);
				if (!_send.IsValidLink(str[2], null).ResultData)
				{
					return false;
				}
				_secAcc.LoginForUpdateEmail(str, null);
				return true;
			}
		}

		public ActionResult GetLoginErrorCount(string email)
		{
			int count = LoginHelper.GetLoginErrorCount(email);
			int allowErrorCount = LoginHelper.GetAllowLoginErrorCount();
			if (count <= allowErrorCount)
			{
				base.HttpContext.Session.Remove("VerifyCode");
			}
			return base.Json(new
			{
				errorCount = count,
				allowErrorCount = allowErrorCount
			});
		}
	}
}
