using JieNor.Megi.Common.Context;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.Identity;
using JieNor.Megi.Identity.Attribute;
using JieNor.Megi.Identity.HtmlHelper;
using JieNor.Megi.ServiceContract.BAS;
using JieNor.Megi.ServiceContract.BD;
using JieNor.Megi.ServiceContract.IV;
using JieNor.Megi.ServiceContract.REG;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.BD.Controllers
{
	public class OrganisationController : GoControllerBase
	{
		private IBASOrganisation _org = null;

		private IREGGlobalization _glob = null;

		private IBDAccount _acct = null;

		private IIVInvoice _inv = null;

		private IBASCountry _country = null;

		public OrganisationController(IBASOrganisation org, IREGGlobalization glob, IBDAccount acct, IIVInvoice inv, IBASCountry country)
		{
			_org = org;
			_glob = glob;
			_acct = acct;
			_inv = inv;
			_country = country;
		}

		[Permission("Setting", "View", "")]
		public ActionResult OrganisationEdit()
		{
			base.SetTitleAndCrumb(LangHelper.GetText(LangModule.Org, "Organisation", "Organisation"), "<a href='/Setting/'>" + LangHelper.GetText(LangModule.Org, "GeneralSettings", "General Settings") + " > </a>");
			base.SetModule("setting");
			//if (_003C_003Eo__6._003C_003Ep__0 == null)
			//{
			//	_003C_003Eo__6._003C_003Ep__0 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "OrgCode", typeof(OrganisationController), new CSharpArgumentInfo[2]
			//	{
			//		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
			//		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
			//	}));
			//}
			//_003C_003Eo__6._003C_003Ep__0.Target(_003C_003Eo__6._003C_003Ep__0, base.get_ViewBag(), base.MContext.MOrgID);
			ViewBag.OrgCode = base.MContext.MOrgID;

			return base.View();
		}

		[Permission("Setting", "View", "")]
		public ActionResult GlobalizationSettings()
		{
			base.SetTitleAndCrumb(LangHelper.GetText(LangModule.Org, "Globalization", "Globalization"), "<a href='/Setting/'>" + LangHelper.GetText(LangModule.Org, "GeneralSettings", "General Settings") + " > </a>");
			base.SetModule("setting");
			//if (_003C_003Eo__7._003C_003Ep__0 == null)
			//{
			//	_003C_003Eo__7._003C_003Ep__0 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "OrgCode", typeof(OrganisationController), new CSharpArgumentInfo[2]
			//	{
			//		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
			//		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
			//	}));
			//}
			//_003C_003Eo__7._003C_003Ep__0.Target(_003C_003Eo__7._003C_003Ep__0, base.get_ViewBag(), base.MContext.MOrgID);

			ViewBag.OrgCode = base.MContext.MOrgID;
			return base.View();
		}

		[Permission("Setting", "View", "")]
		public ActionResult InitSettings(int? id)
		{
			if (!id.HasValue || id < 0 || id > 3)
			{
				id = 0;
			}
			//if (_003C_003Eo__8._003C_003Ep__0 == null)
			//{
			//	_003C_003Eo__8._003C_003Ep__0 = CallSite<Func<CallSite, object, int?, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "InvoiceType", typeof(OrganisationController), new CSharpArgumentInfo[2]
			//	{
			//		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
			//		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
			//	}));
			//}
			//_003C_003Eo__8._003C_003Ep__0.Target(_003C_003Eo__8._003C_003Ep__0, base.get_ViewBag(), id);
			ViewBag.InvoiceType = id;



			BASOrganisationModel resultData = _org.GetModel(null).ResultData;
			//if (_003C_003Eo__8._003C_003Ep__1 == null)
			//{
			//	_003C_003Eo__8._003C_003Ep__1 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "ConversionDate", typeof(OrganisationController), new CSharpArgumentInfo[2]
			//	{
			//		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
			//		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
			//	}));
			//}
			//_003C_003Eo__8._003C_003Ep__1.Target(_003C_003Eo__8._003C_003Ep__1, base.get_ViewBag(), resultData.MConversionDate.ToDateFormat());
			ViewBag.ConversionDate = resultData.MConversionDate.ToDateFormat();

			//if (_003C_003Eo__8._003C_003Ep__2 == null)
			//{
			//	_003C_003Eo__8._003C_003Ep__2 = CallSite<Func<CallSite, object, bool, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "IsInitBalanceOver", typeof(OrganisationController), new CSharpArgumentInfo[2]
			//	{
			//		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
			//		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
			//	}));
			//}
			//_003C_003Eo__8._003C_003Ep__2.Target(_003C_003Eo__8._003C_003Ep__2, base.get_ViewBag(), ContextHelper.MContext.MInitBalanceOver);
			ViewBag.IsInitBalanceOver = ContextHelper.MContext.MInitBalanceOver;

			return base.View();
		}

		[Permission("Setting", "View", "")]
		public ActionResult AccountBalances()
		{
			base.SetTitleAndCrumb(LangHelper.GetText(LangModule.BD, "AccountBalances", "Account Balances"), "<a href='/Setting/'>" + LangHelper.GetText(LangModule.Org, "GeneralSettings", "General Settings") + " > </a>");
			base.ViewData["bankAcctList"] = _acct.GetInitBalanceList(null, null).ResultData;
			base.SetModule("setting");
			return base.View();
		}

		public JsonResult GetOrgDetail()
		{
			ParamBase paramBase = new ParamBase();
			paramBase.KeyIDs = base.MContext.MOrgID;
			BASOrgInfoModel resultData = _org.GetOrgInfo(paramBase, null).ResultData;
			resultData.GlobalizationModel = _glob.GetOrgGlobalizationDetail(base.MContext.MOrgID, null).ResultData;
			base.Response.AddHeader("Access-Control-Allow-Origin", "*");
			return base.Json(resultData);
		}

		[Permission("Setting", "View", "")]
		public JsonResult UpdateOrgDetail(BASOrgInfoModel model)
		{
			MActionResult<OperationResult> data = _org.UpdateOrgInfo(model, null);
			return base.Json(data);
		}

		[Permission("Setting", "View", "")]
		public JsonResult UpdateBasicInfo(BASOrganisationModel model)
		{
			MActionResult<OperationResult> data = _org.Update(model, null);
			return base.Json(data);
		}

		[Permission("Setting", "View", "")]
		public JsonResult GetBasicInfo()
		{
			ParamBase paramBase = new ParamBase();
			paramBase.KeyIDs = base.MContext.MOrgID;
			MActionResult<BASOrganisationModel> model = _org.GetModel(null);
			return base.Json(model);
		}

		public JsonResult GetOrgGlobalizationDetail()
		{
			return base.Json(_glob.GetOrgGlobalizationDetail(base.MContext.MOrgID, null));
		}

		public JsonResult GlobalizationUpdate(REGGlobalizationModel model)
		{
			MActionResult<OperationResult> mActionResult = _glob.GlobalizationUpdate(model, null);
			if (mActionResult != null && mActionResult.ResultData != null && mActionResult.ResultData.Success && mActionResult.ResultData.ObjectID == "1")
			{
				MContext mContext = ContextHelper.MContext;
				ContextHelper.SaveLocaleIDToCookie(mContext.MLCID, null);
			}
			return base.Json(mActionResult);
		}

		public JsonResult GetProvinceList(string countryId)
		{
			return base.Json("[{MItemID: '54', MName: 'Algeria'},{MItemID: '12', MName: 'Angola'}]");
		}

		public JsonResult IsOrgExist(string displayName, string excludeId)
		{
			MActionResult<bool> mActionResult = _org.IsOrgExist(displayName, excludeId, null);
			return base.Json(mActionResult.ResultData ? "true" : "false");
		}
	}
}
