using JieNor.Megi.Common.Context;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.Identity.Attribute;
using JieNor.Megi.ServiceContract.BD;
using JieNor.Megi.ServiceContract.REG;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.BD.Controllers
{
	public class ExchangeRateController : GoControllerBase
	{
		private IBDExchangeRate _bdExchangeRate = null;

		private IREGCurrency _bdCurrency = null;

		public ExchangeRateController(IBDExchangeRate BDExchangeRage, IREGCurrency currency)
		{
			_bdExchangeRate = BDExchangeRage;
			_bdCurrency = currency;
		}

		public ActionResult GetBDExchangeRate(BDExchangeRateFilterModel filter)
		{
			decimal resultData = _bdExchangeRate.GetExchangeRate(filter.MSourceCurrencyID, filter.MRateDate, filter.MTargetCurrencyID, null).ResultData;
			return base.Json(resultData);
		}

		[Permission("Setting", "View", "")]
		public JsonResult GetBDExchangeRateList(BDExchangeRateFilterModel filter)
		{
			if (filter.MTargetCurrencyID == null || filter.MTargetCurrencyID.Length == 0)
			{
				return base.Json(new List<BDExchangeRateViewModel>());
			}
			MActionResult<DataGridJson<BDExchangeRateViewModel>> exchangeRateViewList = _bdExchangeRate.GetExchangeRateViewList(filter, null);
			return base.Json(exchangeRateViewList);
		}

		public JsonResult GetMonthlyExchangeRateList(DateTime date)
		{
			MActionResult<List<BDExchangeRateModel>> monthlyExchangeRateList = _bdExchangeRate.GetMonthlyExchangeRateList(date, null);
			return base.Json(monthlyExchangeRateList);
		}

		[Permission("Setting", "View", "")]
		public JsonResult AddBDExchangeRate(BDExchangeRateModel model, bool isUpdate = false)
		{
			model.MOrgID = base.MContext.MOrgID;
			DateTime mRateDate = model.MRateDate;
			model.MRateDate = ((model.MRateDate.Year == 1) ? base.MContext.DateNow : model.MRateDate);
			if ((model.MItemID != null && model.MItemID.Length > 0) | isUpdate)
			{
				return base.Json(_bdExchangeRate.UpdateExchangeRate(model, null));
			}
			return base.Json(_bdExchangeRate.InsertExchangeRate(model, null));
		}

		public JsonResult UpdateExchangeRateList(List<BDExchangeRateModel> list, DateTime date)
		{
			list.ForEach(delegate(BDExchangeRateModel x)
			{
				DateTime dateTime = new DateTime(date.Year, date.Month, 1);
				dateTime = dateTime.AddMonths(1);
				x.MRateDate = dateTime.AddDays(-1.0);
			});
			MActionResult<OperationResult> data = _bdExchangeRate.UpdateExchangeRateList(list, null);
			return base.Json(data);
		}

		[Permission("Setting", "View", "")]
		public JsonResult RemoveBDExchangeRate(BDExchangeRateModel model)
		{
			model.MOrgID = base.MContext.MOrgID;
			return base.Json(_bdExchangeRate.RemoveExchangeRate(model, null));
		}

		public ActionResult CheckExchangeRateIsExist(BDExchangeRateModel model)
		{
			model.MOrgID = base.MContext.MOrgID;
			DateTime mRateDate2 = model.MRateDate;
			DateTime mRateDate = model.MRateDate;
			model.MRateDate = ((mRateDate.Year == 1) ? base.MContext.DateNow : model.MRateDate);
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.Equal(" MOrgID ", ContextHelper.MContext.MOrgID);
			sqlWhere.Equal(" MTargetCurrencyID ", model.MTargetCurrencyID);
			sqlWhere.Equal(" MSourceCurrencyID ", model.MSourceCurrencyID);
			SqlWhere sqlWhere2 = sqlWhere;
			mRateDate = model.MRateDate;
			sqlWhere2.Equal(" DATE_FORMAT(MRateDate , '%Y-%m-%d') ", mRateDate.ToString("yyyy-MM-dd"));
			sqlWhere.Equal(" MIsDelete ", "0");
			bool resultData = _bdExchangeRate.ExistsByFilter(sqlWhere, null).ResultData;
			OperationResult operationResult = new OperationResult();
			operationResult.Success = resultData;
			return base.Json(operationResult);
		}

		public ActionResult GetExchangeRateById(string id)
		{
			MActionResult<BDExchangeRateModel> dataModel = _bdExchangeRate.GetDataModel(id, false, null);
			return base.Json(dataModel);
		}
	}
}
