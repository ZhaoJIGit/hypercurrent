using JieNor.Megi.Common.Context;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.Identity;
using JieNor.Megi.Identity.Attribute;
using JieNor.Megi.Identity.HtmlHelper;
using JieNor.Megi.ServiceContract.BAS;
using JieNor.Megi.ServiceContract.REG;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.BD.Controllers
{
	public class CurrencyController : GoControllerBase
	{
		private IREGCurrency _DBCurrency = null;

		private IBASCurrency BasCurrencyService = null;

		public CurrencyController(IREGCurrency DBCurrency, IBASCurrency basCurrency)
		{
			_DBCurrency = DBCurrency;
			BasCurrencyService = basCurrency;
			base.SetModule("setting");
		}

		[Permission("Setting", "View", "")]
		public ActionResult Currency()
		{
			base.SetTitleAndCrumb(LangHelper.GetText(LangModule.Acct, "Currencies", "Currencies"), "<a href='/Setting/'>" + LangHelper.GetText(LangModule.Acct, "GeneralSettings", "General Settings") + " > </a>");
			ViewBag.OrgCode = base.MContext.MOrgID;

			BASCurrencyViewModel resultData = _DBCurrency.GetBaseCurrency(null).ResultData;
			if (resultData != null)
			{
				ViewBag.Currency = resultData.MCurrencyID;
			}
			return base.View();
		}

		[Permission("Setting", "View", "")]
		public JsonResult GetBasCurrencyList(bool containFlag = true)
		{
			MActionResult<List<BASCurrencyViewModel>> viewList = BasCurrencyService.GetViewList(containFlag, null);
			return base.Json(viewList);
		}

		public JsonResult GetSystemCurrencyList()
		{
			MActionResult<List<BASCurrencyViewModel>> viewList = BasCurrencyService.GetViewList(true, null);
			return base.Json(viewList);
		}

		public JsonResult GetBDCurrencyList(REGCurrencyModel model, bool isIncludeBase = false)
		{
			MActionResult<List<REGCurrencyViewModel>> currencyViewList = _DBCurrency.GetCurrencyViewList(null, isIncludeBase, null);
			return base.Json(currencyViewList);
		}

		[Permission("Setting", "Change", "")]
		public ActionResult AddBDCurrency(REGCurrencyModel model)
		{
			model.MOrgID = ContextHelper.MContext.MOrgID;
			return base.Json(_DBCurrency.InsertCurrency(model, null));
		}

		[Permission("Setting", "Change", "")]
		public ActionResult RemoveBDCurrency(REGCurrencyModel model)
		{
			model.MOrgID = ContextHelper.MContext.MOrgID;
			return base.Json(_DBCurrency.RemoveCurrency(model, null));
		}

		[Permission("Setting", "Change", "")]
		public ActionResult AddCurrency(string currencyId, string exchangeRateId, int rowIndex = 0, string MItemID = null, DateTime? defaultDate = default(DateTime?))
		{
			ViewBag.OrgCode = MContext.MOrgID;

			BASCurrencyViewModel resultData = _DBCurrency.GetBaseCurrency(null).ResultData;
			if (resultData != null)
			{
				ViewBag.Currency = resultData.MCurrencyID;
			}
			if (!string.IsNullOrEmpty(currencyId))
			{
				ViewBag.EditCurrency = currencyId;
			}
			if (!string.IsNullOrEmpty(exchangeRateId))
			{
				ViewBag.ExchangeRateId = exchangeRateId;
			}
			if (!defaultDate.HasValue)
			{
				defaultDate = DateTime.Now;
			}
			ViewBag.RowIndex = rowIndex;
			ViewBag.MItemID = MItemID;
			ViewBag.DefaultDate = defaultDate.Value.Date;

			return base.View("UCCurrencyAdd");
		}

		public ActionResult ExchangeRateEdit(string currencyId, string exchangeRate)
		{
			BASCurrencyViewModel resultData = _DBCurrency.GetBaseCurrency(null).ResultData;
			if (resultData != null)
			{
				ViewBag.Currency = resultData.MCurrencyID;
			}
			if (!string.IsNullOrEmpty(currencyId))
			{
				ViewBag.EditCurrency = currencyId;
			}
			if (!string.IsNullOrEmpty(exchangeRate))
			{
				ViewBag.MExchangeRate = exchangeRate;
			}
			return base.View("ExchangeRateEdit");
		}

		public JsonResult GetCurrencyDataOpstion(DateTime? endDate)
		{
			string dataOptionsString = HtmlBDCurrency.GetDataOptionsString(endDate, false);
			return base.Json(dataOptionsString);
		}

		public JsonResult GetCurrencyName(string currencyId)
		{
			BASCurrencyViewModel resultData = BasCurrencyService.GetViewModel(new BASCurrencyModel
			{
				MItemID = currencyId
			}, null).ResultData;
			return base.Json((resultData == null) ? "" : resultData.MLocalName);
		}
	}
}
