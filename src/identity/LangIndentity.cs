using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.ServiceContract.BAS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace JieNor.Megi.Identity
{
	public class LangIndentity
	{
		public static string CurrentLangID
		{
			get
			{
				string text = HttpContext.Current.Request.QueryString["lang"];
				text = ((string.IsNullOrWhiteSpace(text) || !ServerHelper.MegiLangTypes.Contains(text)) ? CookieHelper.GetCookieValue(ContextHelper.MLocaleIDCookie, null) : text);
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
				MContext mContext = ContextHelper.MContext;
				if (mContext != null && !string.IsNullOrEmpty(mContext.MLCID))
				{
					return mContext.MLCID;
				}
				return SetCookieByBroswerLanguage();
			}
		}

		private static string SetCookieByBroswerLanguage()
		{
			string text = ConfigurationManager.AppSettings["DefaultWebsiteLanguage"];
			if (!string.IsNullOrEmpty(text) && text != "0")
			{
				CookieHelper.SetCookie(ContextHelper.MLocaleIDCookie, text, DateTime.MaxValue, ContextHelper.Domain, null);
				return text;
			}
			string text2 = string.Empty;
			string[] userLanguages = HttpContext.Current.Request.UserLanguages;
			if (userLanguages != null && userLanguages.Length != 0)
			{
				string text3 = userLanguages[0].ToLower();
				string a = text3;
				text2 = ((a == "zh-cn") ? LangCodeEnum.ZH_CN : ((!(a == "zh-tw")) ? LangCodeEnum.EN_US : LangCodeEnum.ZH_TW));
				CookieHelper.SetCookie(ContextHelper.MLocaleIDCookie, text2, DateTime.MaxValue, ContextHelper.Domain, null);
			}
			return text2;
		}

		public static void ChangeLang(string langId)
		{
			CookieHelper.SetCookie(ContextHelper.MLocaleIDCookie, langId, DateTime.MaxValue);
			ContextHelper.ChangeLang(langId, null);
		}

		public static List<BASLangModel> GetOrgLangList()
		{
			IBASLang sysService = ServiceHostManager.GetSysService<IBASLang>();
			using (sysService as IDisposable)
			{
				return sysService.GetOrgLangList(null).ResultData;
			}
		}

		public static List<BASLangModel> GetSysLangList()
		{
			IBASLang sysService = ServiceHostManager.GetSysService<IBASLang>();
			using (sysService as IDisposable)
			{
				return sysService.GetSysLangList(null).ResultData;
			}
		}

		public static string GetClientGlobalInfo()
		{
			IBASLang sysService = ServiceHostManager.GetSysService<IBASLang>();
			using (sysService as IDisposable)
			{
				return sysService.GetClientGlobalInfo(null).ResultData;
			}
		}
	}
}
