using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.Identity;
using JieNor.Megi.Identity.Attribute;
using JieNor.Megi.ServiceContract.IV;
using System;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.IV.Controllers
{
	public class IVTransferController : GoControllerBase
	{
		private IIVTransfer _trf = null;

		private IIVBankBill _bankBill = null;

		public IVTransferController(IIVTransfer trf, IIVBankBill bankBill)
		{
			_trf = trf;
			_bankBill = bankBill;
		}

		public ActionResult IVTransferHome(string MID, string acctId, string dialog)
		{
			base.ViewData["MID"] = MID;
			base.ViewData["acctId"] = acctId;
			base.ViewData["dialog"] = (dialog ?? string.Empty);
			IVTransferModel resultData = _trf.GetTransferEditModel(MID, null).ResultData;
			base.ViewData["TransferModel"] = resultData;
			base.ViewData["MFromCyID"] = resultData.MFromCyID;
			base.ViewData["MToCyID"] = resultData.MToCyID;
			bool resultData2 = _bankBill.CheckIsExistsBankBillReconcile("Transfer", MID, null).ResultData;
			base.ViewData["IsExistsBankBillReconcile"] = resultData2;
			return base.View();
		}

		public JsonResult GetTransferEditModel(string MID)
		{
			IVTransferModel resultData = _trf.GetTransferEditModel(MID, null).ResultData;
			resultData.MTransferAmt = resultData.MFromTotalAmtFor;
			return base.Json(resultData);
		}

		public JsonResult SubmitTransfer(IVTransferModel model)
		{
			MActionResult<OperationResult> data = _trf.UpdateTransfer(model, null);
			return base.Json(data);
		}

		public JsonResult deleteTransfer(IVTransferModel model)
		{
			MActionResult<OperationResult> data = _trf.DeleteTransfer(model, null);
			return base.Json(data);
		}

		public JsonResult GetAccountInfo(string acctid)
		{
			return null;
		}

		public ActionResult Index(string id)
		{
			string empty = string.Empty;
			if (string.IsNullOrWhiteSpace(id))
			{
				empty = LangHelper.GetText(LangModule.Bank, "TransferMoney", "Transfer Money");
			}
			else
			{
				empty = LangHelper.GetText(LangModule.Bank, "TransactionTransfer", "Transaction: Transfer");
			}
			base.ViewData["transferId"] = id;
			base.ViewData["AccountId"] = Convert.ToString(base.Request["acctid"]);
			return base.View();
		}

		public ActionResult TransferCreateByBankBill(string id)
		{
			base.ViewData["transferId"] = id;
			base.ViewData["AccountId"] = Convert.ToString(base.Request["acctid"]);
			return base.View();
		}

		[Permission("Bank_Reconciliation", "Change", "")]
		public JsonResult UpdateReconcileStatu(string transferId, IVReconcileStatus statu)
		{
			MActionResult<OperationResult> data = _trf.UpdateReconcileStatu(transferId, statu, null);
			return base.Json(data);
		}
	}
}
