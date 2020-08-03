using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.Identity;
using JieNor.Megi.Identity.AutoManager;
using JieNor.Megi.Identity.HtmlHelper;
using JieNor.Megi.My.Web.Controllers;
using JieNor.Megi.ServiceContract.BAS;
using JieNor.Megi.ServiceContract.REG;
using JieNor.Megi.ServiceContract.SEC;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Mvc;

namespace JieNor.Megi.My.Web.Areas.FW.Controllers
{
	public class FWHomeController : MyControllerBase
	{
		private IBASMyHome _org;

		private IBASOrganisation OrgService;

		private readonly ISECUserAccount _userAccount;

		private readonly ISECUserLoginLog _loginLogService;

		private readonly ISECUserLoginLog LoginLogService;

		private readonly IREGGlobalization GlobalService;

		private ISECUser _userService;

		public FWHomeController(ISECUserAccount uacct, ISECUserLoginLog logService, IBASMyHome org, IBASOrganisation orgService, ISECUserLoginLog loginLogService, IREGGlobalization globalService, ISECUser userService)
		{
			_org = org;
			OrgService = orgService;
			LoginLogService = loginLogService;
			_userAccount = uacct;
			_loginLogService = logService;
			GlobalService = globalService;
			_userService = userService;
		}

		public ActionResult Index(string RedirectUrl, string IframeUrl, bool? redirectOnload)
		{
			MContext mContext = ContextHelper.MContext;
			if (mContext != null)
			{
				string mLastLoginOrgName = mContext.MLastLoginOrgName;
				string value = (mContext.MLastLoginOrgId == null) ? "" : mContext.MLastLoginOrgId;
				base.ViewData["lastLoginOrgName"] = mLastLoginOrgName;
				base.ViewData["lastLoginOrgId"] = value;
				DateTime mLastAccessTime = mContext.MLastAccessTime;
				string value2 = mContext.MLastAccessTime.ToLangDate();
				base.ViewData["lastAccessTime"] = value2;
				base.ViewData["UserCreateDate"] = mContext.MUserCreateDate;
				base.ViewData["MLocaleID"] = mContext.MLCID;
			}
			else
			{
				mContext = new MContext();
			}
			mContext.MServerType = 2;
			ContextHelper.MContext = mContext;
			base.ViewData["RedirectUrl"] = HttpUtility.UrlDecode(RedirectUrl);
			base.ViewData["IframeUrl"] = HttpUtility.UrlDecode(IframeUrl);
			base.ViewData["redirectOnload"] = (redirectOnload.HasValue ? redirectOnload.Value.ToString().ToLower() : "false");
			if (redirectOnload.HasValue && redirectOnload.Value && !string.IsNullOrWhiteSpace(RedirectUrl))
			{
				base.ViewData["IframeUrl"] = base.ViewData["RedirectUrl"];
			}
			base.ViewData["OrgListShowType"] = mContext.MOrgListShowType;
			BASOrganisationModel resultData = OrgService.GetDemoOrg(null).ResultData;
			if (resultData != null)
			{
				base.ViewData["DemoOrgID"] = resultData.MItemID;
			}
			else
			{
				base.ViewData["DemoOrgID"] = "";
			}
			return base.View();
		}

		[HttpPost]
		public ActionResult GetOrgList()
		{
			MActionResult<List<BASMyHomeModel>> orgInfoListByUserID = _org.GetOrgInfoListByUserID(null);
			MContext mContext = ContextHelper.MContext;
			return base.Json(new
			{
				Data = orgInfoListByUserID.ResultData,
				Context = mContext
			});
		}

		public ActionResult GetOrgListByPage(BDOrganistationListFilter param)
		{
			param = (param ?? new BDOrganistationListFilter());
			param.PageSize = 0;
			MActionResult<DataGridJson<BASMyHomeModel>> orgInfoPageListByUserID = _org.GetOrgInfoPageListByUserID(param, null);
			return base.Json(orgInfoPageListByUserID);
		}

		public ActionResult OrgSelect(string MOrgID, string RedirectUrl, bool isBetaOrg = false)
		{
			bool resultData = OrgService.ExistsByFilter(new SqlWhere().Equal("MItemID", MOrgID).Equal("MIsDelete", 0), null).ResultData;
			string arg = base.HttpContext.Request.QueryString["bti"];
			string arg2 = base.HttpContext.Request.QueryString["lang"];
			string url = (ServerHelper.MyServer.IndexOf('?') > 0) ? $"{ServerHelper.MyServer}&bti={arg}&lang={arg2}" : $"{ServerHelper.MyServer}?bti={arg}&lang={arg2}";
			if (!string.IsNullOrWhiteSpace(RedirectUrl) && (!resultData || string.IsNullOrEmpty(MOrgID)))
			{
				return new RedirectResult(url);
			}
			MContext mContext = ContextHelper.MContext;
			mContext.MExistsOrg = resultData;
			ContextHelper.MContext = mContext;
			BASOrganisationModel resultData2 = OrgService.GetModelById(MOrgID, null).ResultData;
			if (!_userService.ValidateCreateOrgAuth(resultData2.MVersionID, null).ResultData.Success)
			{
				return new RedirectResult(url);
			}
			BASOrganisationManager.ChangeOrgId(MOrgID);
			REGGlobalizationModel resultData3 = GlobalService.GetOrgGlobalizationDetail(MOrgID, null).ResultData;
			if (resultData3 == null || resultData3.MSystemLanguage.IndexOf(LangIndentity.CurrentLangID) < 0)
			{
				try
				{
					string cookieValue = resultData3.MSystemLanguage.Split(',')[0];
					CookieHelper.SetCookie(ContextHelper.MLocaleIDCookie, cookieValue, DateTime.MaxValue);
				}
				catch (Exception)
				{
					CookieHelper.SetCookie(ContextHelper.MLocaleIDCookie, LangCodeEnum.EN_US, DateTime.MaxValue);
				}
			}
			LoginLogService.InsertLoginLog(null, null);
			return new RedirectResult(HttpUtility.UrlDecode((RedirectUrl.IndexOf('?') > 0) ? $"{RedirectUrl}&bti={arg}&lang={arg2}" : $"{RedirectUrl}?bti={arg}&lang={arg2}"));
		}

		public ActionResult DeleteOrgById(string orgId)
		{
			if (string.IsNullOrEmpty(orgId))
			{
				return base.Json(0);
			}
			if (_org.DeleteOrgById(orgId, null).ResultData > 0)
			{
				return base.Json(1);
			}
			return base.Json(0);
		}

		public ActionResult CheckToken(string _eme = null, string _pmp = null, string _tmt = null, string _omo = null, string _lml = null)
		{
			return null;
		}

		public ActionResult LoginBoxSignIn(string _eme = null, string _pmp = null, string _tmt = null, string _omo = null, string _lml = null)
		{
			return null;
		}

		public string GetUserName()
		{
			MContext mContext = ContextHelper.MContext;
			if (mContext.MLCID == LangCodeEnum.EN_US)
			{
				return $"{mContext.MFirstName} {mContext.MLastName}";
			}
			return $"{mContext.MLastName}{mContext.MFirstName}";
		}

		public ActionResult UpdateOrgListShowType(int type)
		{
			MActionResult<OperationResult> data = _userService.UpdateOrgListShowType(type, null);
			return base.Json(data);
		}

		public ActionResult ValidateCreateOrgAuth(int type, bool checkBeta = false)
		{
			MActionResult<OperationResult> data = _userService.ValidateCreateOrgAuth(type, null);
			return base.Json(data);
		}

		public ActionResult ValidateJumpOrg(string id)
		{
			BASOrganisationModel resultData = OrgService.GetModelById(id, null).ResultData;
			MActionResult<OperationResult> data = _userService.ValidateCreateOrgAuth(resultData.MVersionID, null);
			return base.Json(data);
		}

		public ActionResult ValidateBeta(bool isBeta)
		{
			bool flag = Convert.ToBoolean(ConfigurationManager.AppSettings["IsBetaSite"]);
			if (flag && !isBeta)
			{
				goto IL_0023;
			}
			if (!flag & isBeta)
			{
				goto IL_0023;
			}
			return base.Json(false);
			IL_0023:
			return base.Json(true);
		}

		public ActionResult ValidateOrgIsDelete(string orgId)
		{
			bool resultData = OrgService.ExistsByFilter(new SqlWhere().Equal("MItemID", orgId).Equal("MIsDelete", 0), null).ResultData;
			return base.Json(!resultData);
		}
	}
}
