using JieNor.Megi.Common.Context;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Identity.Attribute;
using JieNor.Megi.Identity.HtmlHelper;
using JieNor.Megi.ServiceContract.BAS;
using JieNor.Megi.ServiceContract.BD;
using JieNor.Megi.ServiceContract.GL;
using JieNor.Megi.ServiceContract.IV;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.IV.Controllers
{
	public class ReceiptController : IVBaseController
	{
		private IBDBankAccount _bdBankAccount;

		protected IIVReceive _receive = null;

		protected IBDTrack _track = null;

		private IIVVerification _verification = null;

		private IIVBankBill _bankBill = null;

		public ReceiptController(IBASOrganisation org, IBDBankAccount bdAccount, IIVReceive receive, IBDTrack track, IIVVerification verification, IIVBankBill bankBill, IGLSettlement settle)
			: base(org, settle)
		{
			_bdBankAccount = bdAccount;
			_receive = receive;
			_track = track;
			_verification = verification;
			_bankBill = bankBill;
		}

		[Permission("Bank", "Change", "")]
		public ActionResult ReceiptEdit(string id, string acctid)
		{
			base.ViewData["IsInit"] = "false";
			base.ViewData["isEdit"] = true;
			base.ViewData["ShowEditButton"] = false;
			ActionResult receiveAction = GetReceiveAction(id, acctid, false);
			IVPaymentModel iVPaymentModel = base.ViewData["ReceiveModel"] as IVPaymentModel;
			if (iVPaymentModel != null && !string.IsNullOrWhiteSpace(iVPaymentModel.MID) && iVPaymentModel.MBizDate < ContextHelper.MContext.MBeginDate)
			{
				return ReceiptInitEdit(id, acctid);
			}
			return receiveAction;
		}

		[Permission("Bank", "Change", "")]
		public ActionResult ReceiptInitEdit(string id, string acctid)
		{
			base.ViewData["BizDateValidType"] = "maxDate";
			base.ViewData["HideRefund"] = "true";
			base.ViewData["IsInit"] = "true";
			base.ViewData["isEdit"] = true;
			return GetReceiveAction(id, acctid, true);
		}

		[Permission("Bank", "Change", "")]
		public ActionResult ReceiptCreateByBankBill(string id, string acctid)
		{
			base.ViewData["isEdit"] = true;
			return GetReceiveAction(id, acctid, false);
		}

		[Permission("Bank", "View", "")]
		public ActionResult ReceiptView(string id, string acctid)
		{
			base.ViewData["isEdit"] = false;
			return GetReceiveAction(id, acctid, false);
		}

		private ActionResult GetReceiveAction(string receiveId, string acctid, bool minConversionDate)
		{
			DateTime dateTime = base.SetDefaultBizDate(minConversionDate);
			string text = acctid;
			base.ViewData["ReceiptID"] = receiveId;
			IVReceiveModel resultData = _receive.GetReceiveEditModel(receiveId, null).ResultData;
			if (string.IsNullOrEmpty(text))
			{
				text = resultData.MBankID;
			}
			base.ViewData["AccountID"] = text;
			bool flag = !string.IsNullOrEmpty(base.Request["showBnkAcct"]) && true;
			if (string.IsNullOrEmpty(receiveId) && string.IsNullOrEmpty(text) && !flag)
			{
				return Redirect("/BD/BDBank/BDBankHome");
			}
			string text2 = base.Request.QueryString["paymentType"];
			if (!string.IsNullOrWhiteSpace(text2))
			{
				resultData.MType = text2;
			}
			base.ViewData["ReceiveModel"] = resultData;
			BDBankAccountEditModel resultData2 = _bdBankAccount.GetBDBankAccountEditModel(text, null).ResultData;
			if (!string.IsNullOrWhiteSpace(text) && string.IsNullOrEmpty(resultData2.MItemID))
			{
				return Redirect("/BD/BDBank/BDBankHome");
			}
			resultData2.MCyID = (string.IsNullOrEmpty(resultData.MCyID) ? resultData2.MCyID : resultData.MCyID);
			base.ViewData["BankModel"] = resultData2;
			SetVDVerification(receiveId);
			return base.View();
		}

		[Permission("Bank", "View", "")]
		public JsonResult GetReceiptEditModel(string id)
		{
			MActionResult<IVReceiveModel> receiveEditModel = _receive.GetReceiveEditModel(id, null);
			return base.Json(receiveEditModel);
		}

		[Permission("Bank", "View", "")]
		public JsonResult GetReceiptViewModel(string id)
		{
			MActionResult<IVReceiveViewModel> receiveViewModel = _receive.GetReceiveViewModel(id, null);
			return base.Json(receiveViewModel);
		}

		[Permission("Bank", "Change", "")]
		public JsonResult UpdateReceipt(IVReceiveModel model)
		{
			model.MOrgID = base.MContext.MOrgID;
			OperationResult operationResult = new OperationResult();
			if (model.MBizDate < ContextHelper.MContext.MBeginDate && ContextHelper.MContext.MInitBalanceOver)
			{
				operationResult.Success = false;
				operationResult.Message = HtmlLang.GetText(LangModule.BD, "InitBalanceIsOver", "The initial balance has been completed and is not allowed to initialize the document operation!");
				return base.Json(operationResult);
			}
			MActionResult<OperationResult> data = _receive.UpdateReceive(model, null);
			return base.Json(data);
		}

		[Permission("Bank", "View", "")]
		protected void SetVDVerification(string receiveId)
		{
			List<IVVerificationInforModel> resultData = _verification.GetCustomerWaitForVerificationInforByBillId(receiveId, "Receive", null).ResultData;
			base.ViewData["VerificationList"] = resultData;
			base.ViewData["IsShowVerif"] = (base.Request["IsShowVerif"] != null && resultData != null && resultData.Count != 0 && Convert.ToBoolean(base.Request["IsShowVerif"]));
		}

		[Permission("Receive", "View", "")]
		public JsonResult GetInitList()
		{
			return base.Json(_receive.GetInitList(null));
		}

		[Permission("BankAccount", "View", "")]
		public JsonResult GetInitReceiveListByPage(IVReceiveListFilterModel filter)
		{
			return base.Json(_receive.GetInitReceiveListByPage(filter, null));
		}

		[Permission("BankAccount", "Change", "")]
		public JsonResult DeleteReceiveList(ParamBase param)
		{
			MActionResult<OperationResult> data = _receive.DeleteReceiveList(param, null);
			return base.Json(data);
		}

		public JsonResult GetVerificationById(string receiveId)
		{
			return base.Json(_verification.GetCustomerWaitForVerificationInforByBillId(receiveId, "Receive", null).ResultData);
		}

		[Permission("Bank_Reconciliation", "Change", "")]
		public JsonResult UpdateReconcileStatu(string receiveId, IVReconcileStatus statu)
		{
			MActionResult<OperationResult> data = _receive.UpdateReconcileStatu(receiveId, statu, null);
			return base.Json(data);
		}

		[Permission("Bank_Reconciliation", "Change", "")]
		public JsonResult BatchUpdateReconcileStatu(ParamBase param, IVReconcileStatus statu)
		{
			MActionResult<OperationResult> data = _receive.BatchUpdateReconcileStatu(param, statu, null);
			return base.Json(data);
		}
	}
}
