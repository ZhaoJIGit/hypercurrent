using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.Identity.Attribute;
using JieNor.Megi.Identity.Go.HtmlHelper;
using JieNor.Megi.ServiceContract.PA;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.PA.Controllers
{
	public class SalaryPaymentController : GoControllerBase
	{
		protected IPASalaryPayment _salaryPayment = null;

		public SalaryPaymentController(IPASalaryPayment salaryPayment)
		{
			_salaryPayment = salaryPayment;
		}

		[Permission("PayRun", "View", "")]
		public ActionResult PayRunList()
		{
			DateTime dateNow = base.MContext.DateNow;
			ViewBag.CurrentDate = dateNow.ToString("yyyy-MM");
			ViewBag.SalaryPaymentSummary = _salaryPayment.GetSalaryPaymentSummaryModelByStatus(null).ResultData;
			ViewBag.PieList = _salaryPayment.GetChartPieDictionary(dateNow, dateNow.AddMonths(-6), null).ResultData;
			return View();
		}

		public ActionResult EmployeeAdd(string id)
		{
			ViewBag.RunId = id;
			return View();
		}

		[Permission("PayRun", "View", "")]
		public ActionResult SalaryPaymentList(string id, bool? isCopy, int? status)
		{
			PAPayRunModel pAPayRunModel = string.IsNullOrWhiteSpace(id) ? null : _salaryPayment.GetPayRunModel(id, null).ResultData;
			if (pAPayRunModel == null)
			{
				pAPayRunModel = new PAPayRunModel();
			}
			ViewData["PayRunModel"] = pAPayRunModel;
			ViewBag.IsCopy = (isCopy.HasValue && isCopy.Value) ? "true" : "false";
			ViewBag.Status = status.HasValue ? status.Value : 0;

			PASalaryPaymentSummaryModel resultData = _salaryPayment.GetSalaryPaymentSummaryModel(id, null).ResultData;
			ViewData["SalaryPaymentSummary"] = resultData;
			return View();
		}

		[Permission("PayRun", "View", "")]
		public ActionResult GetSalaryPaymentListByVerificationId(string id)
		{
			string resultData = _salaryPayment.GetSalaryPaymentListByVerificationId(id, null).ResultData;
			PAPayRunModel pAPayRunModel = string.IsNullOrWhiteSpace(resultData) ? null : _salaryPayment.GetPayRunModel(resultData, null).ResultData;
			if (pAPayRunModel == null)
			{
				pAPayRunModel = new PAPayRunModel();
			}
			ViewData["PayRunModel"] = pAPayRunModel;
			ViewBag.IsCopy = false;
			ViewBag.Status = 4;

			PASalaryPaymentSummaryModel resultData2 = _salaryPayment.GetSalaryPaymentSummaryModel(resultData, null).ResultData;
			ViewData["SalaryPaymentSummary"] = resultData2;
			return View("SalaryPaymentList");
		}

		[Permission("PayRun", "View", "")]
		public JsonResult GetSalaryPaymentSummaryModel(string id)
		{
			return base.Json(_salaryPayment.GetSalaryPaymentSummaryModel(id, null));
		}

		[Permission("PayRun", "View", "")]
		public ActionResult SalaryPaymentEdit(string id)
		{
			PASalaryPaymentModel resultData = _salaryPayment.GetSalaryPaymentEditModel(id, null).ResultData;
			bool flag = HtmlSECMenu.HavePermission("PayRun", "Change", "");
			base.ViewData["SalaryPayID"] = id;
			ViewDataDictionary viewData = base.ViewData;
			int num;
			if ((resultData == null || resultData.MStatus < 3) && flag)
			{
				num = 1;
				goto IL_0059;
			}
			num = 0;
			goto IL_0059;
			IL_0059:
			viewData["IsEdit"] = ((byte)num != 0);
			base.ViewData["TaxAmt"] = (resultData?.MTaxSalary ?? decimal.Zero);
			base.ViewData["model"] = resultData;
			return base.View();
		}

		[Permission("PayRun", "View", "")]
		public JsonResult GetPayRunListPage(PAPayRunListFilterModel param)
		{
			MActionResult<DataGridJson<PAPayRunListModel>> payRunListPage = _salaryPayment.GetPayRunListPage(param, null);
			return base.Json(payRunListPage);
		}

		[Permission("PayRun", "View", "")]
		public JsonResult GetPayRunChartData(string payRunListData)
		{
			MActionResult<string> chartStackedDictionary = _salaryPayment.GetChartStackedDictionary(payRunListData, null);
			return base.Json(chartStackedDictionary);
		}

		[Permission("PayRun", "View", "")]
		public JsonResult GetChartPieDictionary(DateTime startDate, DateTime endDate)
		{
			MActionResult<List<ChartPie2DModel>> chartPieDictionary = _salaryPayment.GetChartPieDictionary(startDate, endDate, null);
			return base.Json(chartPieDictionary);
		}

		[Permission("PayRun", "Change", "")]
		public JsonResult GetSalaryPaymentEditModel(string id)
		{
			return base.Json(_salaryPayment.GetSalaryPaymentEditModel(id, null));
		}

		[Permission("PayRun", "View", "")]
		public JsonResult GetSalaryPaymentPersonDetails(string salaryPayId)
		{
			return base.Json(_salaryPayment.GetSalaryPaymentPersonDetails(salaryPayId, null));
		}

		[Permission("PayRun", "View", "")]
		public JsonResult MergePaySalary(List<IVMakePaymentModel> list)
		{
			return base.Json("");
		}

		[Permission("PayRun", "Change", "")]
		public JsonResult SalaryPaymentUpdate(PASalaryPaymentModel spModel)
		{
			return base.Json(_salaryPayment.SalaryPaymentUpdate(spModel, null));
		}

		public JsonResult ValidatePayRunAction(string yearMonth, PayRunSourceEnum source)
		{
			return base.Json(_salaryPayment.ValidatePayRunAction(yearMonth, source, null));
		}

		public JsonResult PayRunNew(string yearMonth)
		{
			return base.Json(_salaryPayment.PayRunNew(yearMonth, null));
		}

		public JsonResult PayRunCopy(string yearMonth)
		{
			return base.Json(_salaryPayment.PayRunCopy(yearMonth, null));
		}

		[Permission("PayRun", "View", "")]
		public JsonResult GetSalaryPaymentList(PASalaryPaymentListFilterModel filter)
		{
			MActionResult<DataGridJson<PASalaryPaymentListModel>> salaryPaymentList = _salaryPayment.GetSalaryPaymentList(filter, null);
			return base.Json(salaryPaymentList);
		}

		public JsonResult SavePayRun(PAPayRunModel model)
		{
			List<string> list = new List<string>();
			list.Add("MStatus");
			MActionResult<OperationResult> data = _salaryPayment.PayRunUpdate(model, list, null);
			return base.Json(data);
		}

		[Permission("PayRun", "Change", "")]
		public JsonResult SalaryPaymentListUpdate(string runId, string empIds)
		{
			return base.Json(_salaryPayment.SalaryPaymentListUpdate(runId, empIds, null));
		}

		[Permission("PayRun", "View", "")]
		public JsonResult GetUnPayEmployeeList(string runId)
		{
			return base.Json(_salaryPayment.GetUnPayEmployeeList(runId, null));
		}

		[Permission("PayRun", "Change", "")]
		public JsonResult SalaryPaymentDelete(ParamBase param)
		{
			return base.Json(_salaryPayment.SalaryPaymentDelete(param.KeyIDs, null));
		}

		[Permission("PayRun", "Change", "")]
		public JsonResult UnApproveSalaryPayment(string ids)
		{
			MActionResult<OperationResult> data = _salaryPayment.UnApproveSalaryPayment(ids, null);
			return base.Json(data);
		}

		[Permission("PayRun", "Change", "")]
		public JsonResult AddSalaryPayment(IVMakePaymentModel model)
		{
			MActionResult<OperationResult> data = _salaryPayment.PaySalary(model, null);
			return base.Json(data);
		}
	}
}
