using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.IV.Expense;
using JieNor.Megi.DataModel.IV.Invoice;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Identity.HtmlHelper;
using JieNor.Megi.ServiceContract.BAS;
using JieNor.Megi.ServiceContract.BD;
using JieNor.Megi.ServiceContract.GL;
using JieNor.Megi.ServiceContract.IV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.IV.Controllers
{
	public class UCController : IVInvoiceBaseController
	{
		private IIVExpense _expense = null;

		public UCController(IBASOrganisation org, IIVInvoice invoice, IIVExpense expense, IBDTrack track, IIVVerification verification, IBDContacts bdContact, IBDEmailTemplate emailTmpl, IGLSettlement settle)
			: base(org, invoice, track, verification, bdContact, emailTmpl, settle)
		{
			_expense = expense;
		}

		public ActionResult MakePayment(string id)
		{
			base.SetDefaultBizDate(false);
			base.SetVDInvoice(id, "Invoice_Sale");
			return base.MView();
		}

		public ActionResult PreviewPlaceholders(string billType)
		{
			base.ViewData["BillType"] = billType;
			return base.View();
		}

		public JsonResult FormatPlaceholders(List<IVRepeatPreviewPlaceholderModel> param)
		{
			string oldValue = "[" + HtmlLang.Write(LangModule.IV, "Week", "Week") + "]";
			string text = "[" + HtmlLang.Write(LangModule.IV, "Month", "Month") + "]";
			string text2 = "[" + HtmlLang.Write(LangModule.IV, "Year", "Year") + "]";
			string oldValue2 = "[" + HtmlLang.Write(LangModule.IV, "WeekYear", "Week Year") + "]";
			string oldValue3 = "[" + HtmlLang.Write(LangModule.IV, "MonthYear", "Month Year") + "]";
			DateTime dateNow = ContextHelper.MContext.DateNow;
			int num = DateUtility.GetWeekOfYear(dateNow);
			string text3 = num.ToString();
			foreach (IVRepeatPreviewPlaceholderModel item in param)
			{
				if (!string.IsNullOrWhiteSpace(item.Content))
				{
					item.Content = item.Content.Replace(oldValue, text3);
					IVRepeatPreviewPlaceholderModel iVRepeatPreviewPlaceholderModel = item;
					string content = item.Content;
					string oldValue4 = text;
					num = dateNow.Month;
					iVRepeatPreviewPlaceholderModel.Content = content.Replace(oldValue4, num.ToString());
					IVRepeatPreviewPlaceholderModel iVRepeatPreviewPlaceholderModel2 = item;
					string content2 = item.Content;
					string oldValue5 = text2;
					num = dateNow.Year;
					iVRepeatPreviewPlaceholderModel2.Content = content2.Replace(oldValue5, num.ToString());
					item.Content = item.Content.Replace(oldValue2, text3 + " " + dateNow.Year);
					item.Content = item.Content.Replace(oldValue3, dateNow.Month + " " + dateNow.Year);
				}
			}
			return base.Json(param);
		}

		public ActionResult BatchPayment(string selectIds, string obj, bool isMergePay = false, string rundId = "")
		{
			MActionResult<List<string>> settledPeriod = base._settle.GetSettledPeriod(null);
			string openDate = base.GetOpenDate(settledPeriod.ResultData);
			string pkID = selectIds.Split(',').FirstOrDefault();
			string value = "";
			if (obj == "PayRun")
			{
				value = base.MContext.MBasCurrencyID;
			}
			else if (obj == "Expense")
			{
				IVExpenseModel resultData = _expense.GetExpenseEditModel(pkID, null).ResultData;
				if (resultData != null)
				{
					value = resultData.MCyID;
				}
			}
			else
			{
				IVInvoiceModel resultData2 = base._invoice.GetInvoiceEditModel(pkID, "Invoice_Sale", null).ResultData;
				value = resultData2.MCyID;
			}
			base.ViewData["MCurrencyID"] = value;
			base.ViewData["selectIds"] = selectIds;
			base.ViewData["selectObj"] = obj;
			base.ViewData["isMergePay"] = isMergePay;
			base.ViewData["rundId"] = rundId;
			base.ViewData["openDate"] = openDate;
			return base.View();
		}

		public JsonResult GetBatchPaymentList(ParamBase para, string selectObj, bool isMergePay = false)
		{
			MActionResult<List<IVBatchPaymentModel>> batchPaymentList = base._verification.GetBatchPaymentList(para, selectObj, isMergePay, null);
			return base.Json(batchPaymentList);
		}

		public JsonResult IsSuccessBatch(ParamBase para, string selectObj)
		{
			return base.Json(base._verification.IsSuccessBatch(para, selectObj, null));
		}

		public JsonResult BatchPaymentUpdate(IVBatchPayHeadModel headModel)
		{
			MActionResult<OperationResult> data = base._verification.BatchPaymentUpdate(headModel, null);
			return base.Json(data);
		}
	}
}
