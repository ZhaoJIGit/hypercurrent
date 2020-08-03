using JieNor.Megi.Common.Context;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.BD.InitDocument;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.ServiceContract.BD;
using JieNor.Megi.ServiceContract.GL;
using JieNor.Megi.ServiceContract.IV;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.BD.Controllers
{
	public class InitDocumentController : GoControllerBase
	{
		private IBDInitDocument InitDocumentService;

		private IIVInvoice _invoice;

		private IIVReceive _receive;

		private IIVPayment _payment;

		private IIVExpense _expense;

		private IGLCheckType _checkType;

		public InitDocumentController(IBDInitDocument initDocumentService, IIVInvoice invoice, IIVReceive receive, IIVPayment payment, IIVExpense expense, IGLCheckType checkType)
		{
			InitDocumentService = initDocumentService;
			_invoice = invoice;
			_receive = receive;
			_payment = payment;
			_expense = expense;
			_checkType = checkType;
		}

		public ActionResult InitDocumentIndex(string accountCode)
		{
			DateTime mBeginDate = ContextHelper.MContext.MBeginDate;
			DateTime mGLBeginDate = ContextHelper.MContext.MGLBeginDate;
			base.ViewData["beginDate"] = mBeginDate;
			base.ViewData["glBeginDate"] = mGLBeginDate;
			base.ViewData["accountCode"] = accountCode;
			return base.View();
		}

		public ActionResult GetInitDocument(BDInitDocumentFilterModel query)
		{
			MActionResult<BDInitDocumentViewModel> initDocumentModel = InitDocumentService.GetInitDocumentModel(query, null);
			return base.Json(initDocumentModel);
		}

		public ActionResult SaveInitDocument(BDInitDocumentViewModel model)
		{
			MActionResult<OperationResult> data = InitDocumentService.SaveInitDocumentModel(model, null);
			return base.Json(data);
		}

		public ActionResult GetInitDocumentData(int type = 0)
		{
			MActionResult<List<NameValueModel>> initDocumentData = InitDocumentService.GetInitDocumentData(type, null);
			return base.Json(initDocumentData);
		}

		public ActionResult RemoveInitBill(string MID, string MType)
		{
			MActionResult<OperationResult> data = new MActionResult<OperationResult>();
			switch (MType)
			{
			case "Invoice_Sale":
			case "Invoice_Sale_Red":
			case "Invoice_Purchase":
			case "Invoice_Purchase_Red":
				data = _invoice.DeleteInvoiceList(new ParamBase
				{
					KeyIDs = MID,
					MIsInit = true
				}, null);
				break;
			case "Receive_Sale":
			case "Receive_Other":
			case "Receive_SaleReturn":
			case "Receive_Adjustment":
			case "Receive_BankFee":
			case "Receive_OtherReturn":
			case "Receive_Prepare":
				data = _receive.DeleteReceiveList(new ParamBase
				{
					KeyIDs = MID,
					MIsInit = true
				}, null);
				break;
			case "Pay_Purchase":
			case "Pay_Other":
			case "Pay_PurReturn":
			case "Pay_BankFee":
			case "Pay_OtherReturn":
			case "Pay_Adjustment":
			case "Pay_Prepare":
				data = _payment.DeletePaymentList(new ParamBase
				{
					KeyIDs = MID,
					MIsInit = true
				}, null);
				break;
			case "Expense_Claims":
				data = _expense.DeleteExpenseList(new ParamBase
				{
					KeyIDs = MID,
					MIsInit = true
				}, null);
				break;
			}
			return base.Json(data);
		}

		public ActionResult UpdateDocCurrentAccountCode(string docType, string docId, string accountCode)
		{
			MActionResult<OperationResult> data = InitDocumentService.UpdateDocCurrentAccountCode(docType, docId, accountCode, null);
			return base.Json(data);
		}

		public OperationResult CheckIsExistInitBill()
		{
			return new OperationResult();
		}
	}
}
