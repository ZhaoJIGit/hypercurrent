using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.IV.Expense;
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.BAS;
using JieNor.Megi.ServiceContract.BD;
using JieNor.Megi.ServiceContract.GL;
using JieNor.Megi.ServiceContract.IV;
using JieNor.Megi.ServiceContract.PA;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.IV.Controllers
{
	public class VerificationController : IVInvoiceBaseController
	{
		private IIVVerification _vfc = null;

		protected IIVPayment _payment = null;

		private IIVReceive _receive = null;

		private IIVExpense _expense = null;

		private IPASalaryPayment _salaryPayment = null;

		public VerificationController(IBASOrganisation org, IIVInvoice invoice, IIVPayment payment, IIVReceive receive, IIVExpense expense, IBDTrack track, IIVVerification vfc, IBDContacts bdContact, IBDEmailTemplate emailTmpl, IGLSettlement settle, IPASalaryPayment salaryPayment)
			: base(org, invoice, track, vfc, bdContact, emailTmpl, settle)
		{
			_payment = payment;
			_vfc = vfc;
			base._bdContact = bdContact;
			_receive = receive;
			_expense = expense;
			_salaryPayment = salaryPayment;
		}

		public ActionResult Verification(string id)
		{
			base.ViewData["InvoiceID"] = id;
			string text = base.Request["srcBizBillType"];
			base.ViewData["SrcBizBillType"] = text;
			List<IVVerificationInforModel> resultData = _vfc.GetCustomerWaitForVerificationInforByBillId(id, text, null).ResultData;
			foreach (IVVerificationInforModel item in resultData)
			{
				BDContactsInfoModel resultData2 = base._bdContact.GetContactViewData(item.MContactID, null).ResultData;
				if (resultData2 != null)
				{
					item.MContactName = resultData2.MName;
				}
			}
			base.ViewData["VerificationList"] = resultData;
			return base.View();
		}

		public ActionResult VerificationEdit(string id)
		{
			string text = base.Request["srcBizBillType"];
			decimal num = default(decimal);
			string value = "";
			string value2 = "";
			string value3 = "";
			string value4 = "";
			switch (text)
			{
			case "Invoice":
			{
				IVInvoiceModel resultData2 = base._invoice.GetInvoiceEditModel(id, "Invoice_Sale", null).ResultData;
				if (resultData2 != null)
				{
					num = Math.Abs(resultData2.MTaxTotalAmtFor) - Math.Abs(resultData2.MVerificationAmt);
					value = resultData2.MReference;
					value2 = resultData2.MContactID;
					value3 = resultData2.MCyID;
					value4 = resultData2.MType;
				}
				break;
			}
			case "Payment":
			{
				IVPaymentModel resultData4 = _payment.GetPaymentEditModel(id, null).ResultData;
				if (resultData4 != null)
				{
					num = Math.Abs(resultData4.MTaxTotalAmtFor) - Math.Abs(resultData4.MVerificationAmt);
					value = resultData4.MReference;
					value2 = resultData4.MContactID;
					value3 = resultData4.MCyID;
					value4 = resultData4.MType;
				}
				break;
			}
			case "Receive":
			{
				IVReceiveModel resultData5 = _receive.GetReceiveEditModel(id, null).ResultData;
				if (resultData5 != null)
				{
					num = Math.Abs(resultData5.MTaxTotalAmtFor) - Math.Abs(resultData5.MVerificationAmt);
					value = resultData5.MReference;
					value2 = resultData5.MContactID;
					value3 = resultData5.MCyID;
					value4 = resultData5.MType;
				}
				break;
			}
			case "Expense":
			{
				IVExpenseModel resultData3 = _expense.GetExpenseEditModel(id, null).ResultData;
				if (resultData3 != null)
				{
					num = Math.Abs(resultData3.MTaxTotalAmtFor) - Math.Abs(resultData3.MVerificationAmt);
					value = resultData3.MReference;
					value2 = resultData3.MContactID;
					value3 = resultData3.MCyID;
					value4 = resultData3.MType;
				}
				break;
			}
			case "PayRun":
			{
				PASalaryPaymentModel resultData = _salaryPayment.GetSalaryPaymentEditModel(id, null).ResultData;
				if (resultData != null)
				{
					num = Math.Abs(resultData.MNetSalary) - Math.Abs(resultData.MVerificationAmt);
					value = resultData.MReference;
					value2 = resultData.MEmployeeID;
					value3 = base.MContext.MBasCurrencyID;
					value4 = "Pay_Salary";
				}
				break;
			}
			}
			base.ViewData["Amount"] = num;
			base.ViewData["Ref"] = value;
			base.ViewData["MBillID"] = id;
			base.ViewData["MContactID"] = value2;
			base.ViewData["MCurrencyID"] = value3;
			base.ViewData["MSrcBizBillType"] = text;
			base.ViewData["MSrcBizType"] = value4;
			base.ViewData["MBizBillType"] = base.Request["MBizBillType"];
			base.ViewData["MBizType"] = base.Request["MBizType"];
			base.ViewData["MAmount"] = base.Request["MAmount"];
			return base.View();
		}

		public JsonResult GetVerificationList(IVVerificationListFilterModel filter)
		{
			MActionResult<List<IVVerificationListModel>> verificationList = _vfc.GetVerificationList(filter, null);
			return base.Json(verificationList);
		}

		public ActionResult UpdateVerificationList(List<IVVerificationModel> list)
		{
			MActionResult<OperationResult> data = _vfc.UpdateVerificationList(list, null);
			return base.Json(data);
		}

		public ActionResult DeleteVerification(string verificationId, bool isMergePay = false)
		{
			MActionResult<OperationResult> data = _vfc.DeleteVerificationByPKID(verificationId, isMergePay, null);
			return base.Json(data);
		}
	}
}
