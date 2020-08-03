using JieNor.Megi.Common.Context;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.Identity.Go.HtmlHelper;
using JieNor.Megi.ServiceContract.GL;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.BD.Controllers
{
	public class BDAccountBalanceController : GoControllerBase
	{
		private IGLInitBalance InitBalanceServie;

		public BDAccountBalanceController(IGLInitBalance initBalanceService)
		{
			InitBalanceServie = initBalanceService;
		}

		public ActionResult AccountBalances(string id)
		{
			MContext mContext = ContextHelper.MContext;
			ViewData["ForbidEdit"] = Convert.ToInt32(mContext.MInitBalanceOver);

			ViewBag.OrgCode = id;


			ViewDataDictionary viewData = base.ViewData;
			DateTime dateTime = mContext.DateNow;
			object arg = dateTime.Year;
			dateTime = mContext.DateNow;
			viewData["Period"] = arg + "-" + dateTime.Month;
			ViewDataDictionary viewData2 = base.ViewData;
			dateTime = mContext.MGLBeginDate;
			viewData2["GLBeginMonth"] = dateTime.Month;
			ViewData["BaseCurrency"] = mContext.MBasCurrencyID;
			ViewData["IsCanEditBankBalance"] = true;
			ViewData["IsGLPermission"] = HtmlSECMenu.HavePermission("General_Ledger", "Change", "");
			ViewData["AccountStandard"] = mContext.MAccountTableID;
			return View();
		}

		public ActionResult BalanceInput(string defaultId = null, bool isGuide = false)
		{
			ViewBag.DefaultId = defaultId;

			MContext mContext = ContextHelper.MContext;
			bool flag = mContext.MInitBalanceOver;
			if (!isGuide)
			{
				flag = (flag || !HtmlSECMenu.HavePermission("General_Ledger", "Change", ""));
			}
			ViewData["ForbidEdit"] = flag;
			ViewData["SysVersion"] = mContext.MOrgVersionID;
			ViewData["GLBeginMonth"] = mContext.MGLBeginDate.Month;

			ViewBag.BaseCurrencyId = ContextHelper.MContext.MBasCurrencyID;
			return View();
		}

		public ActionResult GetInitBalanceList(GLInitBalanceListFilterModel filter)
		{
			MActionResult<List<GLInitBalanceModel>> initBalanceList = InitBalanceServie.GetInitBalanceList(filter, null);
			return base.Json(initBalanceList);
		}

		public ActionResult SaveInitBalance(List<GLInitBalanceModel> initBalanceList)
		{
			MActionResult<OperationResult> data = InitBalanceServie.Save(initBalanceList, null);
			return base.Json(data);
		}

		public ActionResult DeleteInitBalance(string initBalanceId)
		{
			MActionResult<OperationResult> data = InitBalanceServie.ClearInitBalance(initBalanceId, null);
			return base.Json(data);
		}
	}
}
