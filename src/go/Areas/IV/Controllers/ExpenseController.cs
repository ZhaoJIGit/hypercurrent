using JieNor.Megi.Common.Context;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.IV.Expense;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Identity;
using JieNor.Megi.Identity.Attribute;
using JieNor.Megi.Identity.Go.HtmlHelper;
using JieNor.Megi.ServiceContract.BAS;
using JieNor.Megi.ServiceContract.BD;
using JieNor.Megi.ServiceContract.GL;
using JieNor.Megi.ServiceContract.IV;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.IV.Controllers
{
	public class ExpenseController : IVBaseController
	{
		private IIVExpense _exp = null;

		private IIVVerification _verification = null;

		private IIVInvoice _invoice = null;

		private IBDTrack _track = null;

		public DateTime StartDate => DateTime.MinValue;

		public DateTime EndDate => DateTime.MaxValue;

		public ExpenseController(IBASOrganisation org, IIVExpense exp, IIVInvoice invoice, IBDTrack track, IIVVerification verification, IBDContacts bdContact, IBDEmailTemplate emailTmpl, IGLSettlement settle)
			: base(org, settle)
		{
			_exp = exp;
			_verification = verification;
			_invoice = invoice;
			_track = track;
		}

		[Permission("Expense", "Change", "")]
		public JsonResult UpdateExpenseExpectedInfo(IVInvoiceModel model)
		{
			model.MType = "Expense_Claims";
			MActionResult<OperationResult> data = _invoice.UpdateInvoiceExpectedInfo(model, null);
			return base.Json(data);
		}

		[Permission("Expense", "Approve", "")]
		public JsonResult ExpenseUnAuditToDraft(string expenseId)
		{
			MActionResult<OperationResult> data = _exp.UnApproveExpense(expenseId, null);
			return base.Json(data);
		}

		[Permission("Expense", "Change", "")]
		public JsonResult AddExpenseNoteLog(IVExpenseModel model)
		{
			model.MOrgID = base.MContext.MOrgID;
			return base.Json(_exp.AddExpenseNoteLog(model, null));
		}

		[Permission("Expense", "View", "")]
		public ActionResult ExpenseList(int? id)
		{
			base.SetTitle(LangHelper.GetText(LangModule.IV, "ExpenseClaims", "Expense Claims"));
			if (!id.HasValue || id < 0 || id > 5)
			{
				id = 0;
			}

			ViewBag.ExpenseType = id;
			IVExpenseSummaryModel resultData = _exp.GetExpenseSummaryModel(StartDate, EndDate, null).ResultData;
			base.ViewData["ExpenseSummary"] = resultData;
			return base.View();
		}

		public JsonResult GetExpenseSummaryModel()
		{
			return base.Json(_exp.GetExpenseSummaryModel(StartDate, EndDate, null));
		}

		[Permission("Expense", "View", "")]
		public ActionResult ExpenseEdit(string id)
		{
			bool flag = HtmlSECMenu.HavePermission("Expense", "Change", "");
			bool resultData = _verification.CheckIsCanEditOrVoidOrDelete("Expense", id, null).ResultData;
			base.ViewData["IsCanEditOrVoidOrDelete"] = resultData;
			if (!flag || !resultData)
			{
				base.ViewData["isEdit"] = false;
				return Redirect(string.Format("/IV/Expense/ExpenseView/{0}?tabIndex={1}", id, base.Request.QueryString["tabIndex"]));
			}
			base.ViewData["isEdit"] = true;
			base.ViewData["ExpenseID"] = id;
			IVExpenseModel iVExpenseModel = null;
			string text = base.Request["cpyId"];
			iVExpenseModel = (string.IsNullOrEmpty(text) ? _exp.GetExpenseEditModel(id, null).ResultData : _exp.GetExpenseCopyModel(text, null).ResultData);
			base.ViewData["ExpenseModel"] = iVExpenseModel;
			base.ViewData["MCurrencyID"] = iVExpenseModel.MCyID;
			base.SetDefaultBizDate(false);
			if (iVExpenseModel != null && !string.IsNullOrWhiteSpace(iVExpenseModel.MID) && iVExpenseModel.MBizDate < ContextHelper.MContext.MBeginDate)
			{
				return ExpenseInitEdit(id);
			}
			return base.View();
		}

		[Permission("Expense", "View", "")]
		public ActionResult ExpenseInitEdit(string id)
		{
			base.ViewData["ExpenseID"] = id;
			base.ViewData["IsInit"] = 1;
			IVExpenseModel iVExpenseModel = null;
			string text = base.Request["cpyId"];
			iVExpenseModel = (string.IsNullOrEmpty(text) ? _exp.GetExpenseEditModel(id, null).ResultData : _exp.GetExpenseCopyModel(text, null).ResultData);
			base.ViewData["ExpenseModel"] = iVExpenseModel;
			base.ViewData["MCurrencyID"] = iVExpenseModel.MCyID;
			base.SetDefaultBizDate(true);
			return base.View();
		}

		[Permission("Expense", "View", "")]
		public ActionResult ExpenseView(string id)
		{
			bool flag = HtmlSECMenu.HavePermission("Expense", "Change", "");
			bool resultData = _verification.CheckIsCanEditOrVoidOrDelete("Expense", id, null).ResultData;
			base.ViewData["IsCanEditOrVoidOrDelete"] = resultData;
			base.ViewData["isEdit"] = false;
			base.ViewData["ExpenseID"] = id;
			IVExpenseModel resultData2 = _exp.GetExpenseEditModel(id, null).ResultData;
			if ((flag & resultData) && !resultData2.MIsInitBill)
			{
				if (resultData2.MIsInitBill)
				{
					return Redirect(string.Format("/IV/Expense/ExpenseInitEdit/{0}?tabIndex={1}", id, base.Request.QueryString["tabIndex"]));
				}
				return Redirect(string.Format("/IV/Expense/ExpenseEdit/{0}?tabIndex={1}", id, base.Request.QueryString["tabIndex"]));
			}
			base.ViewData["ExpenseModel"] = resultData2;
			base.ViewData["MCurrencyID"] = resultData2.MCyID;
			base.SetDefaultBizDate(false);
			return base.View();
		}

		[Permission("BankAccount", "Change", "")]
		public ActionResult ExpensesMerge(string expids, string payfrom)
		{
			List<IVExpenseModel> resultData = _exp.GetModelList(new ParamBase
			{
				KeyIDs = expids
			}, null).ResultData;
			if (resultData.Count > 0)
			{
				base.ViewData["MCurrencyID"] = resultData[0].MCyID;
				base.ViewData["MContactID"] = resultData[0].MEmployee;
				base.ViewData["MDepartment"] = resultData[0].MDepartment;
				base.ViewData["MTotAmount"] = resultData.Sum((IVExpenseModel s) => s.MTaxTotalAmtFor - s.MVerificationAmt);
			}
			base.SetDefaultBizDate(false);
			base.ViewData["expids"] = expids;
			base.ViewData["payfrom"] = payfrom;
			return base.View();
		}

		[Permission("Expense", "View", "")]
		public JsonResult GetExpenseList(IVExpenseListFilterModel param)
		{
			MActionResult<DataGridJson<IVExpenseListModel>> expenseList = _exp.GetExpenseList(param, null);
			return base.Json(expenseList);
		}

		[Permission("Expense", "View", "")]
		public JsonResult GetInitExpenseListByPage(IVExpenseListFilterModel param)
		{
			param.MEndDate = ContextHelper.MContext.MBeginDate.AddDays(-1.0);
			param.MSearchWithin = IVInvoiceSearchWithinEnum.TransactionDate;
			MActionResult<DataGridJson<IVExpenseListModel>> expenseList = _exp.GetExpenseList(param, null);
			return base.Json(expenseList);
		}

		[Permission("Expense", "Change", "")]
		public JsonResult UpdateExpense(IVExpenseModel model)
		{
			model.MOrgID = base.MContext.MOrgID;
			return base.Json(_exp.UpdateExpense(model, null));
		}

		[Permission("Expense", "Change", "")]
		public JsonResult DeleteExpenseList(ParamBase param)
		{
			return base.Json(_exp.DeleteExpenseList(param, null));
		}

		[Permission("Expense", "Change", "")]
		public JsonResult UpdateExpenseStatus(ParamBase param)
		{
			return base.Json(_exp.UpdateExpenseStatus(param, null));
		}

		[Permission("Expense", "Change", "")]
		public JsonResult GetVerificationById(string expenseId)
		{
			return base.Json(_verification.GetCustomerWaitForVerificationInforByBillId(expenseId, "Expense", null));
		}

		public JsonResult UnApproveExpense(string expenseId)
		{
			MActionResult<OperationResult> data = _exp.UnApproveExpense(expenseId, null);
			return base.Json(data);
		}

		public JsonResult ApproveExpense(ParamBase param)
		{
			MActionResult<OperationResult> data = _exp.ApproveExpense(param, null);
			return base.Json(data);
		}

		[Permission("Expense", "View", "")]
		public JsonResult IsSuccessMerge(string expids)
		{
			OperationResult operationResult = new OperationResult();
			List<IVExpenseModel> resultData = _exp.GetModelList(new ParamBase
			{
				KeyIDs = expids
			}, null).ResultData;
			if ((from g in resultData
			group g by g.MContactID + g.MCyID).ToList().Count > 1)
			{
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = LangHelper.GetText(base.MContext.MLCID, LangModule.IV, "Emp&CurrMustSame", "the employee & currency must be the same.")
				});
			}
			return base.Json(operationResult);
		}

		[Permission("BankAccount", "Change", "")]
		public JsonResult AddExpensePayment(List<IVMakePaymentModel> modelList)
		{
			return base.Json(_exp.AddExpensePayment(modelList, null));
		}
	}
}
