using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.Encrypt;
using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.EntityModel.SEC;
using JieNor.Megi.Identity;
using JieNor.Megi.ServiceContract.SEC;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace JieNor.Megi.Tools
{
	public static class LoginHelper
	{
		private static Dictionary<string, int> _loginError = new Dictionary<string, int>();

		private static object _lock = new object();

		private static ValidateCodeUtility ValidateCodeCreator;

		public static SECLoginResultModel SignIn(SECLoginModel model, ISECUserAccount userAccount, ISECPermission permission, ISECUserLoginLog loginLogService, HttpContextBase httpContext)
		{
			SECLoginResultModel result = new SECLoginResultModel
			{
				IsSuccess = false
			};
			int loginErrorCount = GetLoginErrorCount(model.Email);
			int allowLoginErrorCount = GetAllowLoginErrorCount();
			if (loginErrorCount > allowLoginErrorCount && string.IsNullOrEmpty(model.ValidateCode))
			{
				return result;
			}
			if (!JudgeValidateCode(httpContext, model.Email, model.ValidateCode).IsSuccess)
			{
				return result;
			}
			model.Password = MD5Service.MD5Encrypt(model.Password);
			model.MLCID = (string.IsNullOrWhiteSpace(model.MLCID) ? CookieHelper.GetCookieValue(ContextHelper.MLocaleIDCookie, null) : model.MLCID);
			model.UserIp = httpContext.Request.UserHostAddress;
			result = userAccount.Login(model, null).ResultData;
			if (result.IsSuccess)
			{
				result.MIsUserInvite = permission.LoginForAcceptInvite(model.UserId, model.OrgId, model.SendLinkID, result.MUserID, model.Email, null).ResultData;
				UserLoginRedirectEnum userLoginRedirectEnum;
				if (result.MIsUserInvite)
				{
					SECLoginResultModel sECLoginResultModel = result;
					userLoginRedirectEnum = UserLoginRedirectEnum.Go;
					sECLoginResultModel.MWhenILogIn = userLoginRedirectEnum.ToString();
				}
				if (!string.IsNullOrWhiteSpace(result.MOrgID))
				{
					if (result.MWhenILogIn == 2.ToString())
					{
						goto IL_013d;
					}
					string mWhenILogIn = result.MWhenILogIn;
					userLoginRedirectEnum = UserLoginRedirectEnum.Go;
					if (mWhenILogIn == userLoginRedirectEnum.ToString())
					{
						goto IL_013d;
					}
				}
				ContextHelper.SaveLocaleIDToCookie(model.MLCID, httpContext);
				goto IL_01b4;
			}
			int num2 = result.MLoginErrorCount = AddLoginErrorCount(model.Email, 1);
			goto IL_021e;
			IL_01b4:
			ContextHelper.SaveAccessTokenToCookie(result.MAccessToken, httpContext);
			loginLogService.InsertLoginLog(null, null);
			AddLoginErrorCount(model.Email, 0);
			SetUserEmailCookie(model.Email);
			if (httpContext.Session["VerifyCode"] != null)
			{
				httpContext.Session.Remove("VerifyCode");
			}
			goto IL_021e;
			IL_013d:
			if (ContextHelper.MLocaleID != result.MLocaleID)
			{
				ContextHelper.SaveLocaleIDToCookie(result.MLocaleID, httpContext);
			}
			else if (result.MActiveLocaleIDS != null && result.MActiveLocaleIDS.Count != 0 && !result.MActiveLocaleIDS.Contains(model.MLCID))
			{
				model.MLCID = result.MActiveLocaleIDS[0];
				ContextHelper.SaveLocaleIDToCookie(model.MLCID, httpContext);
			}
			goto IL_01b4;
			IL_021e:
			return result;
		}

		private static void SetUserEmailCookie(string email)
		{
			string mUserEmailCookie = ContextHelper.MUserEmailCookie;
			DateTime now = DateTime.Now;
			CookieHelper.SetCookie(mUserEmailCookie, email, now.AddYears(1), ContextHelper.Domain, null);
			string cookieValue = CookieHelper.GetCookieValue(ContextHelper.MHistoryEmailCookie, null);
			List<string> list = new List<string>();
			if (string.IsNullOrWhiteSpace(cookieValue))
			{
				list.Add(email);
			}
			else
			{
				list = cookieValue.Split(',').ToList();
				list.Add(email);
			}
			list.ForEach(delegate(string x)
			{
				x = MText.Decode(x);
			});
			list = list.Distinct().ToList();
			string mHistoryEmailCookie = ContextHelper.MHistoryEmailCookie;
			string cookieValue2 = string.Join(",", list);
			now = DateTime.Now;
			CookieHelper.SetCookie(mHistoryEmailCookie, cookieValue2, now.AddYears(1), ContextHelper.Domain, null);
		}

		private static int AddLoginErrorCount(string email, int count)
		{
			if (string.IsNullOrEmpty(email))
			{
				return 0;
			}
			string text = email.ToLower();
			lock (_lock)
			{
				if (_loginError == null)
				{
					_loginError = new Dictionary<string, int>();
					return 0;
				}
				if (_loginError.Keys.Contains(text))
				{
					if (count == 0)
					{
						_loginError.Remove(text);
						return 0;
					}
					int num = _loginError[text] + count;
					_loginError[text] = num;
					return num;
				}
				if (count > 0)
				{
					_loginError.Add(text, count);
					return count;
				}
				return 0;
			}
		}

		public static int GetAllowLoginErrorCount()
		{
			int result = 3;
			string text = ConfigurationManager.AppSettings["AllowLoginErrorCount"];
			if (!string.IsNullOrEmpty(text))
			{
				return int.Parse(text);
			}
			return result;
		}

		public static int GetLoginErrorCount(string email)
		{
			if (string.IsNullOrEmpty(email))
			{
				return 0;
			}
			string text = email.ToLower();
			if (_loginError != null && _loginError.Count != 0 && _loginError.Keys.Contains(text))
			{
				return _loginError[text];
			}
			return 0;
		}

		public static SECLoginResultModel LoginBoxSignIn(SECLoginModel model, HttpContextBase httpContext)
		{
			SECLoginResultModel sECLoginResultModel = null;
			model.Password = MD5Service.MD5Encrypt(model.Password);
			model.MLCID = (string.IsNullOrWhiteSpace(model.MLCID) ? CookieHelper.GetCookieValue(ContextHelper.MLocaleIDCookie, null) : model.MLCID);
			ISECUserAccount sysService = ServiceHostManager.GetSysService<ISECUserAccount>();
			using (sysService as IDisposable)
			{
				model.UserIp = httpContext.Request.UserHostAddress;
				sECLoginResultModel = sysService.Login(model, null).ResultData;
				if (sECLoginResultModel.IsSuccess)
				{
					ContextHelper.SaveAccessTokenToCookie(sECLoginResultModel.MAccessToken, httpContext);
					ISECUserLoginLog sysService2 = ServiceHostManager.GetSysService<ISECUserLoginLog>();
					using (sysService2 as IDisposable)
					{
						sysService2.InsertLoginLog(null, null);
					}
					string mUserEmailCookie = ContextHelper.MUserEmailCookie;
					string email = model.Email;
					DateTime now = DateTime.Now;
					CookieHelper.SetCookie(mUserEmailCookie, email, now.AddYears(1), ContextHelper.Domain, null);
					string mLocaleIDCookie = ContextHelper.MLocaleIDCookie;
					string mLCID = model.MLCID;
					now = DateTime.Now;
					CookieHelper.SetCookie(mLocaleIDCookie, mLCID, now.AddYears(1), ContextHelper.Domain, null);
					if (httpContext.Session["VerifyCode"] != null)
					{
						httpContext.Session.Remove("VerifyCode");
						return sECLoginResultModel;
					}
					return sECLoginResultModel;
				}
				return sECLoginResultModel;
			}
		}

		public static byte[] CreateValidateCodeImage(HttpContextBase httpContext)
		{
			ValidateCodeCreator = new ValidateCodeUtility();
			string text = ValidateCodeCreator.CreateVerifyCode(0);
			httpContext.Session["VerifyCode"] = text;
			return ValidateCodeCreator.CreateImageCodeByByte(text);
		}

		public static SECLoginResultModel JudgeValidateCode(HttpContextBase httpContext, string email, string code)
		{
			object obj = httpContext.Session["VerifyCode"];
			if (string.IsNullOrEmpty(code) && (obj == null || string.IsNullOrEmpty(obj.ToString())))
			{
				return new SECLoginResultModel
				{
					IsSuccess = true,
					MLoginErrorCount = GetLoginErrorCount(email)
				};
			}
			SECLoginResultModel sECLoginResultModel = new SECLoginResultModel
			{
				IsSuccess = false,
				Message = LangHelper.GetText(LangModule.Login, "validateCodeIncorrent", "validate code is incorrent!")
			};
			if (!string.IsNullOrEmpty(code) && obj != null && code.ToLower() == obj.ToString().ToLower())
			{
				sECLoginResultModel.IsSuccess = true;
				sECLoginResultModel.Message = string.Empty;
				sECLoginResultModel.MLoginErrorCount = GetLoginErrorCount(email);
			}
			return sECLoginResultModel;
		}

		public static void SignOut(HttpContextBase httpContext)
		{
			string domain = ConfigurationManager.AppSettings["Domain"];
			MContext mContext = ContextHelper.MContext;
			if (mContext != null)
			{
				mContext.MLogout = true;
				ContextHelper.MContext = mContext;
			}
			httpContext.Session["LoginErrorCount"] = null;
			CookieHelper.ClearCookie(ContextHelper.MAccessTokenCookie, domain, null);
		}
	}
}
