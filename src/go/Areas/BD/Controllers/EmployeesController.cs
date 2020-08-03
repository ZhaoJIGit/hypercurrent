using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.ImportAndExport;
using JieNor.Megi.Common.ImportAndExport.DataModel;
using JieNor.Megi.Common.ImportAndExport.Export;
using JieNor.Megi.Common.ImportAndExport.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IO.Export;
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.Identity;
using JieNor.Megi.Identity.Attribute;
using JieNor.Megi.Identity.HtmlHelper;
using JieNor.Megi.ServiceContract.BD;
using JieNor.Megi.ServiceContract.IV;
using JieNor.Megi.ServiceContract.PA;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.BD.Controllers
{
	public class EmployeesController : GoControllerBase
	{
		private IBDEmployees _bdEmpl = null;

		private IBDTrack _track = null;

		private IIVInvoice _invoice = null;

		private IPAPayrollBasic PayrollBasciService = null;

		private IPASalaryPayment _salaryPayment = null;

		public EmployeesController()
		{
		}

		public EmployeesController(IBDEmployees bdEmployee, IBDTrack track, IIVInvoice invoice, IPAPayrollBasic payrollBasci, IPASalaryPayment salaryPayment)
		{
			_bdEmpl = bdEmployee;
			_track = track;
			_invoice = invoice;
			PayrollBasciService = payrollBasci;
			_salaryPayment = salaryPayment;
		}

		[Permission("Contact", "View", "")]
		public ActionResult EmployeesList(string id = "0")
		{
			base.SetTitle(LangHelper.GetText(LangModule.Contact, "Employees", "Employees"));
			base.ViewData["tabKeySelected"] = id;
			return base.View();
		}

		public JsonResult GetEmployeesList(BDEmployeesListFilterModel param)
		{
			MActionResult<DataGridJson<BDEmployeesListModel>> bDEmployeesPageList = _bdEmpl.GetBDEmployeesPageList(param, null);
			return base.Json(bDEmployeesPageList);
		}

		public JsonResult GetEmployees(bool includeDisable)
		{
			MActionResult<List<BDEmployeesModel>> employeeList = _bdEmpl.GetEmployeeList(includeDisable, null);
			return base.Json(employeeList);
		}

		[Permission("Contact", "View", "")]
		public JsonResult GetBDEmployeesList(string filter = null)
		{
			return base.Json(_bdEmpl.GetBDEmployeesList(filter, false, null));
		}

		public JsonResult GetEmployeesEditData(BDEmployeesModel model)
		{
			MActionResult<BDEmployeesModel> employeesEditInfo = _bdEmpl.GetEmployeesEditInfo(model.MItemID, null);
			employeesEditInfo.ResultData = (employeesEditInfo.ResultData ?? new BDEmployeesModel());
			return base.Json(employeesEditInfo);
		}

		public JsonResult GetEmployeesInfo(BDEmployeesModel model)
		{
			MActionResult<BDEmployeesModel> employeeInfo = _bdEmpl.GetEmployeeInfo(model.MItemID, null);
			employeeInfo.ResultData = (employeeInfo.ResultData ?? new BDEmployeesModel());
			return base.Json(employeeInfo);
		}

		[Permission("Contact", "View", "")]
		public ActionResult EmployeesEdit(string id, int? tabIndex)
		{
			if (!string.IsNullOrWhiteSpace(id))
			{
				BDEmployeesModel resultData = _bdEmpl.GetEmployeesEditInfo(id, null).ResultData;
				string text = (resultData == null) ? "" : resultData.Name;
			}
			ViewData["ItemId"] = id;
			ViewData["tabIndex"] = tabIndex;
			ViewBag.IsEnableGL = ContextHelper.MContext.MRegProgress >= 12 && true;


			PAPITThresholdFilterModel filter = new PAPITThresholdFilterModel
			{
				EmployeeID = id,
				IsDefault = string.IsNullOrWhiteSpace(id)
			};
			ViewBag.PITThresholdList = _salaryPayment.GetPITThresholdList(filter, null).ResultData;

			return base.View();
		}

		[Permission("Contact", "Change", "")]
		public ActionResult EmployeeToArchived(string id)
		{
			base.ViewData["selectIds"] = id;
			return base.View();
		}

		[Permission("Contact", "View", "")]
		public ActionResult EmployeeView(string id)
		{
			BDEmployeesModel resultData = _bdEmpl.GetEmployeesEditInfo(id, null).ResultData;
			string text = (resultData == null) ? "" : resultData.Name;
			base.SetTitleAndCrumb(text, "<a href='/Employees/EmployeesList'>" + LangHelper.GetText(LangModule.Contact, "Employees", "Employees") + " > </a>");
			base.ViewData["viewID"] = id;
			base.ViewData["viewName"] = text;
			base.ViewData["viewdata"] = resultData;
			return base.View();
		}

		[Permission("Contact", "Change", "")]
		public JsonResult EmployeesUpdate(BDEmployeesModel model)
		{
			model.MOrgID = base.MContext.MOrgID;
			MActionResult<OperationResult> data = _bdEmpl.EmployeesUpdate(model, null);
			return base.Json(data);
		}

		public JsonResult EmployeeMoveToArchived(ParamBase param)
		{
			if (param.KeyIDs == null)
			{
				return base.Json(false);
			}
			param.OrgID = base.MContext.MOrgID;
			MActionResult<OperationResult> data = _bdEmpl.ArchiveEmployee(param, null);
			return base.Json(data);
		}

		public JsonResult RestoreEmployee(ParamBase param)
		{
			MActionResult<OperationResult> data = _bdEmpl.RestoreEmployee(param, null);
			return base.Json(data);
		}

		public JsonResult GetOverPastData(string EmployeeID)
		{
			return base.Json(_invoice.GetOverPastDictionary(EmployeeID, null));
		}

		[Permission("Contact", "Change", "")]
		public JsonResult IsCanDeleteOrInactive(ParamBase param)
		{
			MActionResult<BDIsCanDeleteModel> data = _bdEmpl.IsCanDeleteOrInactive(param, null);
			return base.Json(data);
		}

		[Permission("Contact", "Change", "")]
		public JsonResult DeleteEmployess(ParamBase param)
		{
			MActionResult<OperationResult> data = _bdEmpl.DeleteEmployee(param, null);
			return base.Json(data);
		}

		[Permission("Setting", "View", "")]
		public ActionResult GetEmpPayrollDetail(string employeeID)
		{
			MActionResult<BDPayrollDetailModel> empPayrollDetail = PayrollBasciService.GetEmpPayrollDetail(employeeID, null);
			return base.Json(empPayrollDetail);
		}

		public ActionResult UpdateEmpPayrollDetail(BDPayrollDetailModel model)
		{
			MActionResult<OperationResult> data = PayrollBasciService.UpdateEmpPayrollDetail(model, null);
			return base.Json(data);
		}

		public FileResult Export(string jsonParam)
		{
			ReportModel reportModel = ReportStorageHelper.CreateReportModel(BizReportType.EmployeeList, jsonParam, CreateReportModelSource.Export, null, null, null);
			Stream stream = ExportHelper.CreateRptExportFile(reportModel, ExportFileType.Xls);
			string exportName = string.Format("{0} - {1}.xls", reportModel.OrgName, HtmlLang.GetText(LangModule.BD, "Employees", "Employees"));
			return base.ExportReport(stream, exportName);
		}

		[Permission("Contact", "View", "")]
		public JsonResult GetEmployeeInfo(string employeeId)
		{
			MActionResult<BDEmployeesModel> employeesEditInfo = _bdEmpl.GetEmployeesEditInfo(employeeId, null);
			return base.Json(employeesEditInfo);
		}
	}
}
