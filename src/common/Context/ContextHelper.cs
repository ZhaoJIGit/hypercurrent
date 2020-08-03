using JieNor.Megi.Common.Logger;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web;

namespace JieNor.Megi.Common.Context
{
	public class ContextHelper
	{
		private static string _mContentKey = "MegiMContext";

		public static string Domain = ConfigurationManager.AppSettings["Domain"];

		public static string ExpireTime = ConfigurationManager.AppSettings["ExpireTime"];

		public static string DefaultExpireTime = "60";

		public static string MegiChinaNamespace
		{
			get
			{
				string name = ConfigurationManager.AppSettings["MegiChinaNamespace"];
				return string.IsNullOrEmpty(name) ? "MegiChinaNamespace" : name;
			}
			private set
			{
			}
		}

		public static string MOrgID
		{
			get
			{
				string name = ConfigurationManager.AppSettings["MOrgID"];
				return string.IsNullOrEmpty(name) ? "MOrgID" : name;
			}
			private set
			{
			}
		}

		public static string MOrgName
		{
			get
			{
				string name = ConfigurationManager.AppSettings["MOrgName"];
				return string.IsNullOrEmpty(name) ? "MOrgName" : name;
			}
			private set
			{
			}
		}

		public static string MAccessTokenCookie
		{
			get
			{
				string name = ConfigurationManager.AppSettings["MAccessTokenCookie"];
				return string.IsNullOrEmpty(name) ? "MAccessToken" : name;
			}
			private set
			{
			}
		}

		public static string MAppSourceName => "AppSource";

		public static string MAppSource
		{
			get
			{
				string app = ConfigurationManager.AppSettings[MAppSourceName];
				return string.IsNullOrWhiteSpace(app) ? "System" : app;
			}
		}

		public static string MLocaleIDCookie
		{
			get
			{
				string name = ConfigurationManager.AppSettings["MLocaleIDCookie"];
				return string.IsNullOrEmpty(name) ? "MLocaleID" : name;
			}
			private set
			{
			}
		}

		public static string MContentKey => _mContentKey;

		public static string MUserEmailCookie
		{
			get
			{
				string name = ConfigurationManager.AppSettings["MUserEmailCookie"];
				return string.IsNullOrEmpty(name) ? "MUserEmail" : name;
			}
			private set
			{
			}
		}

		public static string MHistoryEmailCookie
		{
			get
			{
				string name = ConfigurationManager.AppSettings["MHistoryEmailCookie"];
				return string.IsNullOrEmpty(name) ? "MHistoryEmailCookie" : name;
			}
			private set
			{
			}
		}

		public static string MOrgIDCookie
		{
			get
			{
				string name = ConfigurationManager.AppSettings["MOrgIDCookieCookie"];
				return string.IsNullOrEmpty(name) ? "MOrgIDCookie" : name;
			}
			private set
			{
			}
		}

		public static string MCheckTokenUrl
		{
			get
			{
				string name = ConfigurationManager.AppSettings["MCheckTokenUrl"];
				return string.IsNullOrEmpty(name) ? "/fw/fwhome/checktoken" : name;
			}
			private set
			{
			}
		}

		public static string MLoginBoxSignInUrl
		{
			get
			{
				string name = ConfigurationManager.AppSettings["MLoginBoxSignInUrl"];
				return string.IsNullOrEmpty(name) ? "/fw/fwhome/loginboxsignin" : name;
			}
			private set
			{
			}
		}

		public static string MLoginSite
		{
			get
			{
				string name = ConfigurationManager.AppSettings["LoginServer"];
				return string.IsNullOrEmpty(name) ? "" : name;
			}
			private set
			{
			}
		}

		public static MContext MContext
		{
			get
			{
				if (HttpContext.Current != null && HttpContext.Current.Items[MContentKey] != null)
				{
					return HttpContext.Current.Items[MContentKey] as MContext;
				}
				string token = CookieHelper.GetCookieValue(MAccessTokenCookie, null);
				if (!string.IsNullOrEmpty(token))
				{
					string app = "System";
					if (OperationContext.Current != null)
					{
						app = GetValueFromHeaderByKey(OperationContext.Current, MAppSourceName);
					}
					MContext ctx = MContextManager.GetMContextByAccessToken(token, app);
					if (ctx != null)
					{
						ctx.MAppID = "1";
						ctx.MVoucherNumberFilledChar = ((ctx.MVoucherNumberLength < 3) ? "0" : ctx.MVoucherNumberFilledChar);
						ctx.MVoucherNumberLength = ((ctx.MVoucherNumberLength < 3) ? 3 : ctx.MVoucherNumberLength);
						HttpContext.Current.Items[MContentKey] = ctx;
					}
					return ctx;
				}
				return null;
			}
			set
			{
				value.MLastAccessTime = value.DateNow;
				string app = "System";
				if (OperationContext.Current != null)
				{
					app = GetValueFromHeaderByKey(OperationContext.Current, MAppSourceName);
				}
				value.AppSource = app;
				MContextManager.SaveMContext(value, true, app);
				HttpContext.Current.Items[MContentKey] = value;
			}
		}

		public static string MAccessToken
		{
			get
			{
				return CookieHelper.GetCookieValue(MAccessTokenCookie, null) ?? string.Empty;
			}
			private set
			{
			}
		}

		public static string MLocaleID
		{
			get
			{
				string localeId = string.Empty;
				HttpContext httpContext = HttpContext.Current;
				if (httpContext != null)
				{
					string urlLoaclId = httpContext.Request.QueryString["lang"];
					localeId = (string.IsNullOrWhiteSpace(urlLoaclId) ? CookieHelper.GetCookieValue(MLocaleIDCookie, null) : urlLoaclId);
				}
				return localeId;
			}
			private set
			{
			}
		}

		public static string MAccessTokenApp
		{
			get;
			set;
		}

		public static string MUserEmail
		{
			get
			{
				return CookieHelper.GetCookieValue(MUserEmailCookie, null) ?? string.Empty;
			}
			private set
			{
			}
		}

		public static List<string> AccessWhiteList
		{
			get
			{
				List<string> list = new List<string>();
				string listStr = ServerHelper.AccessWhiteList;
				if (!string.IsNullOrEmpty(listStr))
				{
					list.AddRange(listStr.Split(';'));
				}
				return list;
			}
			private set
			{
			}
		}

		public static void UpdateMContextByKeyField(string keyName, string keyValue, string fieldName, object fieldValue, bool syn)
		{
			MContextManager.UpdateMContextByKeyField(keyName, keyValue, fieldName, fieldValue, syn, "System");
		}

		public static void RemoveMContext(Dictionary<string, string> queryDictionary)
		{
			MContextManager.RemoveMContext(queryDictionary, "System");
		}

		public static void SaveAccessTokenToCookie(string accessToken, HttpContextBase httpContext = null)
		{
			CookieHelper.ClearCookie(MAccessTokenCookie, Domain, httpContext);
			CookieHelper.SetTempCookie(MAccessTokenCookie, accessToken, Domain, httpContext);
		}

		public static void SaveOrgInfoToCookie(string orgId, string orgName, HttpContextBase httpContext = null)
		{
			CookieHelper.SetTempCookie(MOrgID, orgId, Domain, httpContext);
			CookieHelper.SetTempCookie(MOrgName, HttpUtility.UrlEncode(orgName), Domain, httpContext);
		}

		public static void SaveLocaleIDToCookie(string LCID, HttpContextBase httpContext = null)
		{
			CookieHelper.ClearCookie(MLocaleIDCookie, Domain, httpContext);
			CookieHelper.SetCookie(MLocaleIDCookie, LCID, DateTime.MaxValue);
		}

		public static List<string> GetRedirectUrl(MContext mContext = null)
		{
			mContext = (mContext ?? MContext);
			List<string> result = new List<string>();
			string myServer = string.IsNullOrWhiteSpace(ServerHelper.FinalMyServer) ? ServerHelper.MyServer : ServerHelper.FinalMyServer;
			string loginServer = string.IsNullOrWhiteSpace(ServerHelper.FinalLoginServer) ? ServerHelper.LoginServer : ServerHelper.FinalLoginServer;
			if (mContext?.IsExpired ?? true)
			{
				result.Add(loginServer);
			}
			else if (!mContext.MExistsOrg)
			{
				result.AddRange(new List<string>
				{
					myServer,
					ServerHelper.LoginServer
				});
			}
			else if (string.IsNullOrEmpty(mContext.MOrgID) || (!string.IsNullOrWhiteSpace(mContext.MOrgID) && mContext.MExpireDate.ToDayLastSecond() < mContext.DateNow))
			{
				result.AddRange(new List<string>
				{
					myServer,
					ServerHelper.MainServer,
					loginServer
				});
			}
			else
			{
				bool isBeta = mContext.MIsBeta;
				bool isBetaSite = Convert.ToBoolean(ConfigurationManager.AppSettings["IsBetaSite"]);
				if (mContext.LoginRedirect == 1)
				{
					result.AddRange(new List<string>
					{
						myServer,
						ServerHelper.GoServer,
						ServerHelper.MainServer,
						loginServer
					});
				}
				else if (mContext.LoginRedirect == 2)
				{
					string goServer = (isBeta && !string.IsNullOrWhiteSpace(ServerHelper.BetaGoServer)) ? ServerHelper.BetaGoServer : ServerHelper.GoServer;
					result.AddRange(new List<string>
					{
						goServer,
						ServerHelper.MainServer,
						myServer,
						loginServer
					});
				}
				else
				{
					result.AddRange(new List<string>
					{
						myServer,
						ServerHelper.GoServer,
						ServerHelper.MainServer,
						loginServer
					});
				}
			}
			return result;
		}

		public static void ChangeLang(string langId, string token = null)
		{
			if (string.IsNullOrWhiteSpace(token))
			{
				token = CookieHelper.GetCookieValue(MAccessTokenCookie, null);
			}
			HttpContext current = HttpContext.Current;
			if (current != null && !string.IsNullOrWhiteSpace(current.Request.Url.AbsoluteUri))
			{
				string tempUrl = current.Request.Url.AbsoluteUri.GetAbsoluteUri(current);
				if (tempUrl.ToLower().IndexOf(ServerHelper.LoginServer.ToLower()) >= 0)
				{
					return;
				}
			}
			if (!string.IsNullOrEmpty(token))
			{
				MContext mContext = MContext;
				if (mContext != null)
				{
					mContext.MLCID = langId;
					MContext = mContext;
				}
			}
		}

		public static MContext ClearContextOrgInfo(MContext ctx = null, bool save = false)
		{
			MContext mContext = ctx ?? MContext;
			mContext.MDefaultLocaleID = string.Empty;
			mContext.MMaster = string.Empty;
			mContext.MUsedStatusID = string.Empty;
			mContext.MExpireDate = mContext.DateNow;
			mContext.MOrgName = string.Empty;
			mContext.MZoneFormat = string.Empty;
			mContext.MDateFormat = string.Empty;
			mContext.MTimeFormat = string.Empty;
			mContext.MDigitGrpFormat = string.Empty;
			mContext.MIsShowCSymbol = false;
			if (save)
			{
				MContext = mContext;
			}
			return mContext;
		}

		public static MContext GetContextByUserId(string userId)
		{
			return MContextManager.GetMContextByUserId(userId, "System");
		}

		public static MContext GetContextByAccessCode(string accessCode)
		{
			return MContextManager.GetMContextByAccessCode(accessCode, "System");
		}

		public static MContext GetContextByEmail(string email)
		{
			return MContextManager.GetMContextByEmail(email, "System");
		}

		public static string GetMEmailByUserIp(string userIp)
		{
			return MContextManager.GetMEmailByUserIp(userIp, "System");
		}

		public static LoginStateEnum ValidateAccessToken(MContext ctx = null, string accessToken = null, string email = null, string orgid = null, string locale = null)
		{
			LoginStateEnum result = LoginStateEnum.Unlogin;
			accessToken = (string.IsNullOrWhiteSpace(accessToken) ? MAccessToken : accessToken);
			MContext mContext = ctx ?? MContextManager.GetMContextByAccessToken(accessToken, MAppSource);
			email = (string.IsNullOrWhiteSpace(email) ? MUserEmail : email);
			if (mContext == null)
			{
				if (!string.IsNullOrEmpty(email))
				{
					MContext eContext = GetContextByEmail(email);
					if (eContext != null && !eContext.IsExpired)
					{
						result = LoginStateEnum.ForceLogout;
					}
				}
			}
			else if (mContext.IsExpired || (!string.IsNullOrEmpty(orgid) && orgid != mContext.MOrgID))
			{
				result = LoginStateEnum.Expired;
			}
			else if (!string.IsNullOrEmpty(orgid) && !string.IsNullOrWhiteSpace(mContext.MOrgID) && mContext.MExpireDate.ToDayLastSecond() < mContext.DateNow)
			{
				result = LoginStateEnum.OrgExpired;
			}
			else if (!string.IsNullOrEmpty(orgid) && !string.IsNullOrWhiteSpace(locale) && mContext.MActiveLocaleIDS.Any() && !mContext.MActiveLocaleIDS.Contains(locale))
			{
				result = LoginStateEnum.LangUnMatch;
				mContext.MLCID = mContext.MActiveLocaleIDS[0];
				MContext = mContext;
				CookieHelper.SetCookie(MLocaleIDCookie, mContext.MLCID, DateTime.MaxValue);
			}
			else
			{
				result = LoginStateEnum.Valid;
			}
			if (string.IsNullOrWhiteSpace(locale) && mContext != null)
			{
				CookieHelper.SetCookie(MLocaleIDCookie, mContext.MLCID, DateTime.MaxValue);
			}
			return result;
		}

		public static bool ValidateAccessToken<T>(string accessToken, out MContext oContext, ref MActionResult<T> oResult)
		{
			oContext = null;
			bool needSaveFlag = false;
			OperationContext context = OperationContext.Current;
			MessageProperties properties = context.IncomingMessageProperties;
			RemoteEndpointMessageProperty endpoint = properties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
			string requestIP = endpoint.Address;
			accessToken = (string.IsNullOrWhiteSpace(accessToken) ? GetAccessTokenFromHeader(context) : accessToken);
			string localeID = GetValueFromHeaderByKey(context, MLocaleIDCookie);
			string app = GetValueFromHeaderByKey(context, MAppSourceName);
			if (oContext == null && string.IsNullOrEmpty(accessToken))
			{
				oResult.ResultCode.Add(MActionResultCodeEnum.AccessTokenMissing);
				MLogger.Log("当前请求里面没有获取到Token的值:" + context.IncomingMessageHeaders.UnderstoodHeaders, (MContext)null);
				throw new MActionException
				{
					Codes = new List<MActionResultCodeEnum>
					{
						MActionResultCodeEnum.MLoginInfoLost
					}
				};
			}
			if (oContext == null)
			{
				oContext = MContextManager.GetMContextByAccessToken(accessToken, app);
			}
			if (oContext == null)
			{
				oResult.ResultCode.Add(MActionResultCodeEnum.ContextMissing);
				return false;
			}
			if (string.IsNullOrEmpty(requestIP))
			{
				oResult.ResultCode.Add(MActionResultCodeEnum.UnTracableAddress);
				return false;
			}
			if (oContext.IsExpired)
			{
				oResult.ResultCode.Add(MActionResultCodeEnum.ContextExpired);
				return false;
			}
			if (!string.IsNullOrWhiteSpace(oContext.MOrgID) && !oContext.IsExpired && oContext.MExpireDate.ToDayLastSecond() < oContext.DateNow)
			{
				oResult.ResultCode.Add(MActionResultCodeEnum.OrgExpired);
				oContext.MOrgID = string.Empty;
				oContext.MExpireDate = default(DateTime);
				oContext.MOrgName = string.Empty;
				oContext.MOrgTaxType = 0;
				needSaveFlag = true;
			}
			else
			{
				oResult.ResultCode.Add(MActionResultCodeEnum.Success);
			}
			if (!string.IsNullOrWhiteSpace(localeID) && (string.IsNullOrWhiteSpace(oContext.MLCID) || oContext.MLCID != localeID))
			{
				oContext.MLCID = localeID;
				needSaveFlag = true;
			}
			if (needSaveFlag)
			{
				oContext.AppSource = app;
				MContext = oContext;
			}
			return true;
		}

		private static string GetAccessTokenFromHeader(OperationContext context)
		{
			try
			{
				return context.IncomingMessageHeaders.GetHeader<string>(MAccessTokenCookie, MegiChinaNamespace);
			}
			catch (Exception)
			{
				throw;
			}
		}

		private static string GetValueFromHeaderByKey(OperationContext context, string key)
		{
			try
			{
				return context.IncomingMessageHeaders.GetHeader<string>(key, MegiChinaNamespace);
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static int Check(HttpContextBase httpContext, string t = null, string e = null, string o = null, string l = null)
		{
			int result = 0;
			string requestHost = httpContext.Request.Url.DnsSafeHost;
			if (!CheckHostInWhiteList(requestHost))
			{
				MContext ctx = string.IsNullOrWhiteSpace(t) ? MContext : null;
				result = (int)ValidateAccessToken(ctx, t, e, o, l);
			}
			return result;
		}

		public static bool CheckHostInWhiteList(string host)
		{
			return AccessWhiteList.Contains(host.TrimEnd('/').ToLower());
		}
	}
}
