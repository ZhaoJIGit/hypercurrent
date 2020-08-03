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
	public class PaymentController : IVBaseController
	{
		private IBDBankAccount _bdBankAccount;

		protected IIVPayment _payment = null;

		protected IBDTrack _track = null;

		private IIVVerification _verification = null;

		private IIVBankBill _bankBill = null;

		public PaymentController(IBASOrganisation org, IBDBankAccount bdAccount, IIVPayment payment, IBDTrack track, IIVVerification verification, IIVBankBill bankBill, IGLSettlement settle)
			: base(org, settle)
		{
			_bdBankAccount = bdAccount;
			_payment = payment;
			_track = track;
			_verification = verification;
			_bankBill = bankBill;
		}

		[Permission("Bank", "Change", "")]
		public ActionResult PaymentEdit(string id, string acctid)
		{
			base.SetDefaultBizDate(false);
			base.ViewData["IsInit"] = "false";
			base.ViewData["isEdit"] = true;
			ActionResult paymentAction = GetPaymentAction(id, acctid, false);
			IVPaymentModel iVPaymentModel = base.ViewData["PaymentModel"] as IVPaymentModel;
			if (iVPaymentModel != null && !string.IsNullOrWhiteSpace(iVPaymentModel.MID) && iVPaymentModel.MBizDate < ContextHelper.MContext.MBeginDate)
			{
				return PaymentInitEdit(id, acctid);
			}
			return paymentAction;
		}

		[Permission("Bank", "Change", "")]
		public ActionResult PaymentInitEdit(string id, string acctid)
		{
			base.ViewData["BizDateValidType"] = "maxDate";
			base.ViewData["HideRefund"] = "true";
			base.ViewData["IsInit"] = "true";
			base.ViewData["isEdit"] = true;
			return GetPaymentAction(id, acctid, true);
		}

		[Permission("Bank", "Change", "")]
		public ActionResult PaymentCreateByBankBill(string id, string acctid)
		{
			base.SetDefaultBizDate(false);
			base.ViewData["isEdit"] = true;
			return GetPaymentAction(id, acctid, false);
		}

		[Permission("Bank", "View", "")]
		public ActionResult PaymentView(string id, string acctid)
		{
			base.ViewData["isEdit"] = false;
			return GetPaymentAction(id, acctid, false);
		}

		private ActionResult GetPaymentAction(string paymentId, string acctid, bool minConversionDate)
		{
			DateTime dateTime = base.SetDefaultBizDate(minConversionDate);
			string text = acctid;
			base.ViewData["PaymentID"] = paymentId;
			bool flag = !string.IsNullOrEmpty(base.Request["showBnkAcct"]) && true;
			IVPaymentModel resultData = _payment.GetPaymentEditModel(paymentId, null).ResultData;
			string text2 = base.Request.QueryString["paymentType"];
			if (!string.IsNullOrWhiteSpace(text2))
			{
				resultData.MType = text2;
			}
			base.ViewData["PaymentModel"] = resultData;
			if (string.IsNullOrEmpty(text))
			{
				text = resultData.MBankID;
			}
			base.ViewData["AccountID"] = text;
			BDBankAccountEditModel resultData2 = _bdBankAccount.GetBDBankAccountEditModel(text, null).ResultData;
			if (string.IsNullOrEmpty(paymentId) && string.IsNullOrEmpty(text) && !flag)
			{
				return Redirect("/BD/BDBank/BDBankHome");
			}
			resultData2.MCyID = (string.IsNullOrEmpty(resultData.MCyID) ? resultData2.MCyID : resultData.MCyID);
			base.ViewData["BankModel"] = resultData2;
			SetVDVerification(paymentId);
			return base.View();
		}

		[Permission("Bank", "View", "")]
		public JsonResult GetPaymentViewModel(string id)
		{
			MActionResult<IVPaymentViewModel> paymentViewModel = _payment.GetPaymentViewModel(id, null);
			return base.Json(paymentViewModel);
		}

		[Permission("Bank", "Change", "")]
		public JsonResult UpdatePayment(IVPaymentModel model)
		{
			model.MOrgID = base.MContext.MOrgID;
			OperationResult operationResult = new OperationResult();
			if (model.MBizDate < ContextHelper.MContext.MBeginDate && ContextHelper.MContext.MInitBalanceOver)
			{
				operationResult.Success = false;
				operationResult.Message = HtmlLang.GetText(LangModule.BD, "InitBalanceIsOver", "The initial balance has been completed and is not allowed to initialize the document operation!");
				return base.Json(operationResult);
			}
			MActionResult<OperationResult> data = _payment.UpdatePayment(model, null);
			return base.Json(data);
		}

		[Permission("Bank", "View", "")]
		protected void SetVDVerification(string paymentId)
		{
			List<IVVerificationInforModel> resultData = _verification.GetCustomerWaitForVerificationInforByBillId(paymentId, "Payment", null).ResultData;
			base.ViewData["VerificationList"] = resultData;
			base.ViewData["IsShowVerif"] = (base.Request["IsShowVerif"] != null && resultData != null && resultData.Count != 0 && Convert.ToBoolean(base.Request["IsShowVerif"]));
		}

		[Permission("Bank", "View", "")]
		public JsonResult GetInitList()
		{
			return base.Json(_payment.GetInitList(null).ResultData);
		}

		[Permission("Invoice_Purchases", "View", "")]
		public JsonResult GetInitPaymentListByPage(IVPaymentListFilterModel filter)
		{
			return base.Json(_payment.GetInitPaymentListByPage(filter, null));
		}

		[Permission("BankAccount", "Change", "")]
		public JsonResult DeletePaymentList(ParamBase param)
		{
			MActionResult<OperationResult> data = _payment.DeletePaymentList(param, null);
			return base.Json(data);
		}

		public JsonResult GetVerificationById(string paymentId)
		{
			return base.Json(_verification.GetCustomerWaitForVerificationInforByBillId(paymentId, "Payment", null).ResultData);
		}

		[Permission("Bank_Reconciliation", "Change", "")]
		public JsonResult UpdateReconcileStatu(string paymentId, IVReconcileStatus statu)
		{
			MActionResult<OperationResult> data = _payment.UpdateReconcileStatu(paymentId, statu, null);
			return base.Json(data);
		}

		[Permission("Bank_Reconciliation", "Change", "")]
		public JsonResult BatchUpdateReconcileStatu(ParamBase param, IVReconcileStatus statu)
		{
			MActionResult<OperationResult> data = _payment.BatchUpdateReconcileStatu(param, statu, null);
			return base.Json(data);
		}
	}
}
