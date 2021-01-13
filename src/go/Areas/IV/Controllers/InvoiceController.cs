using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.ImportAndExport;
using JieNor.Megi.Common.ImportAndExport.Export;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Identity;
using JieNor.Megi.Identity.Attribute;
using JieNor.Megi.Identity.Go.HtmlHelper;
using JieNor.Megi.Identity.HtmlHelper;
using JieNor.Megi.ServiceContract.BAS;
using JieNor.Megi.ServiceContract.BD;
using JieNor.Megi.ServiceContract.GL;
using JieNor.Megi.ServiceContract.IV;
using JieNor.Megi.ServiceContract.SEC;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.IV.Controllers
{
	public class InvoiceController : IVInvoiceBaseController
	{
		private ISECPermission _perm = null;

		private IBDContacts _contact = null;

		private IBDEmailTemplate _emailTmpl = null;

		public InvoiceController(IBASOrganisation org, ISECPermission perm, IIVInvoice invoice, IBDTrack track, IIVVerification verification, IBDContacts bdContact, IBDEmailTemplate emailTmpl, IGLSettlement settle)
			: base(org, invoice, track, verification, bdContact, emailTmpl, settle)
		{
			base._org = org;
			_perm = perm;
			_contact = bdContact;
			_emailTmpl = emailTmpl;
		}

		[Permission("Invoice_Sales", "View", "")]
		public JsonResult GetRepeatIVList(IVInvoiceListFilterModel param)
		{
			return base.GetRepeatInvoiceList(param);
		}

		[Permission("Invoice_Sales", "Change", "")]
		public JsonResult UpdateRepeatIV(IVRepeatInvoiceModel model)
		{
			return base.UpdateRepeatInvoice(model);
		}

		[Permission("Invoice_Sales", "Change", "")]
		public JsonResult UpdateRepeatIVStatus(ParamBase param)
		{
			return base.UpdateRepeatInvoiceStatus(param);
		}

		[Permission("Invoice_Sales", "Change", "")]
		public JsonResult DeleteRepeatIVList(ParamBase param)
		{
			return base.DeleteRepeatInvoiceList(param);
		}

		private ActionResult GetRepeatInvoiceWithTrackingAction(string id)
		{
			base.SetVDRepeatInvoice(id, "Invoice_Sales");
			return base.View();
		}

		[Permission("Invoice_Sales", "View", "")]
		public ActionResult RepeatInvoiceEditMessage(string ids)
		{
			base.ViewData["InvoiceIds"] = ids;
			SECInviteUserInfoModel resultData = _perm.GetUserInviteInfo(base.MContext.MUserID, null).ResultData;
			base.ViewData["UserName"] = resultData.MFirstName + " " + resultData.MLastName;
			base.ViewData["UserEmail"] = resultData.MEmail;
			return base.View();
		}

		[Permission("Invoice_Sales", "Change", "")]
		public JsonResult UpdateRepeatInvoiceMessage(IVInvoiceEmailSendModel model)
		{
			return base.Json(base._invoice.UpdateRepeatInvoiceMessage(model, null));
		}

		[Permission("Invoice_Sales", "View", "")]
		public ActionResult RepeatInvoiceEdit(string id)
		{
			if (!HtmlSECMenu.HavePermission("Invoice_Sales", "Change", ""))
			{
				return Redirect(string.Format("/IV/Invoice/RepeatInvoiceView/{0}?tabIndex={1}", id, base.Request.QueryString["tabIndex"]));
			}
			base.SetDefaultBizDate(false);
			return GetRepeatInvoiceWithTrackingAction(id);
		}

		[Permission("Invoice_Sales", "View", "")]
		public ActionResult RepeatInvoiceView(string id)
		{
			if (HtmlSECMenu.HavePermission("Invoice_Sales", "Change", ""))
			{
				return Redirect(string.Format("/IV/Invoice/RepeatInvoiceEdit/{0}?tabIndex={1}", id, base.Request.QueryString["tabIndex"]));
			}
			base.SetDefaultBizDate(false);
			return GetRepeatInvoiceWithTrackingAction(id);
		}

		[Permission("Invoice_Sales", "Change", "")]
		public JsonResult AddSaleNoteLog(IVInvoiceModel model)
		{
			return base.AddInvoiceNoteLog(model);
		}

		[Permission("Invoice_Sales", "View", "")]
		public ActionResult InvoiceList(int? id)
		{
			if (!id.HasValue || id < 0 || id > 6)
			{
				id = 0;
			}
			ViewBag.InvoiceType = id;

			base.SetVDSummary("Invoice_Sale");
			List<ChartPie2DModel> resultData = base._invoice.GetChartPieDictionary("'Invoice_Sale','Invoice_Sale_Red'", base.StartDate, base.EndDate, null).ResultData;
			base.ViewData["PieList"] = resultData;
			return base.View();
		}

		private ActionResult GetInvoiceAction(string id)
		{
			base.SetVDInvoice(id, "Invoice_Sale");
			base.SetVDVerification(id);
			return base.View();
		}

		[Permission("Invoice_Sales", "View", "")]
		public ActionResult InvoiceEdit(string id)
		{
			base.ViewData["isEdit"] = true;
			base.ViewData["From"] = "Edit";
			base.SetDefaultBizDate(false);
			ActionResult invoiceAction = GetInvoiceAction(id);
			IVInvoiceModel iVInvoiceModel = base.ViewData["InvoiceModel"] as IVInvoiceModel;
			if (iVInvoiceModel != null && !string.IsNullOrWhiteSpace(iVInvoiceModel.MID) && iVInvoiceModel.MBizDate < ContextHelper.MContext.MBeginDate)
			{
				return InvoiceInitEdit(id);
			}
			return invoiceAction;
		}

		public ActionResult IssueFapiaoInfo(string id)
		{
			IVInvoiceModel resultData = base._invoice.GetInvoiceEditModel(id, string.Empty, null).ResultData;
			base.ViewData["InvoiceModel"] = resultData;
			return base.PartialView();
		}

		[Permission("Setting", "View", "")]
		public ActionResult InvoiceInitEdit(string id)
		{
			base.ViewData["IsInit"] = "true";
			base.ViewData["isEdit"] = true;
			base.ViewData["BizDateValidType"] = "maxDate";
			base.SetDefaultBizDate(true);
			return GetInvoiceAction(id);
		}

		[Permission("Invoice_Sales", "View", "")]
		public ActionResult InvoiceView(string id)
		{
			base.ViewData["isEdit"] = false;
			base.SetDefaultBizDate(false);
			ActionResult invoiceAction = GetInvoiceAction(id);
			IVInvoiceModel iVInvoiceModel = base.ViewData["InvoiceModel"] as IVInvoiceModel;
			if (iVInvoiceModel?.MActionPermission.MIsCanEdit ?? false)
			{
				return Redirect($"/IV/Invoice/InvoiceEdit/{id}");
			}
			return invoiceAction;
		}

		private ActionResult GetCreditNoteAction(string id)
		{
			base.SetVDInvoice(id, "Invoice_Sale_Red");
			base.SetVDVerification(id);
			return base.View();
		}

		[Permission("Invoice_Sales", "View", "")]
		public ActionResult CreditNoteEdit(string id)
		{
			base.ViewData["isEdit"] = true;
			base.ViewData["From"] = "Edit";
			base.SetDefaultBizDate(false);
			ActionResult creditNoteAction = GetCreditNoteAction(id);
			IVInvoiceModel iVInvoiceModel = base.ViewData["InvoiceModel"] as IVInvoiceModel;
			if (iVInvoiceModel != null && !string.IsNullOrWhiteSpace(iVInvoiceModel.MID) && iVInvoiceModel.MBizDate < ContextHelper.MContext.MBeginDate)
			{
				return CreditNoteInitEdit(id);
			}
			return creditNoteAction;
		}

		[Permission("Setting", "View", "")]
		public ActionResult CreditNoteInitEdit(string id)
		{
			base.ViewData["IsInit"] = "true";
			base.ViewData["isEdit"] = true;
			base.ViewData["BizDateValidType"] = "maxDate";
			base.SetDefaultBizDate(true);
			return GetCreditNoteAction(id);
		}

		[Permission("Invoice_Sales", "View", "")]
		public ActionResult CreditNoteView(string id)
		{
			base.ViewData["isEdit"] = false;
			base.SetDefaultBizDate(false);
			ActionResult creditNoteAction = GetCreditNoteAction(id);
			IVInvoiceModel iVInvoiceModel = base.ViewData["InvoiceModel"] as IVInvoiceModel;
			if (iVInvoiceModel?.MActionPermission.MIsCanEdit ?? false)
			{
				return Redirect($"/IV/Invoice/CreditNoteEdit/{id}");
			}
			return creditNoteAction;
		}

		[Permission("Invoice_Sales", "View", "")]
		public ActionResult SendInvoice(string selectIds, string status, string type, string printSettingId, int sendType = 1, string beginDate = "", string endDate = "", string reportType = "", string rptJsonParam = "", string contactId = "", string totalAmount = "0.00")
		{
			base.ViewData["selIds"] = selectIds;
			base.ViewData["sendType"] = sendType;
			base.ViewData["contactId"] = contactId;
			base.ViewData["totalAmount"] = totalAmount;
			if (sendType == 1 || sendType == 3)
			{
				base.ViewData["status"] = status;
				base.ViewData["MType"] = type;
			}
			else if (sendType == 2)
			{
				base.ViewData["beginDate"] = beginDate;
				base.ViewData["endDate"] = endDate;
			}
			if (sendType == 1 || sendType == 2 || sendType == 4)
			{
				if (string.IsNullOrWhiteSpace(reportType))
				{
					reportType = type;
				}
				Dictionary<string, string> value = ExportHelper.CreateRptAttachesForEmail(reportType, printSettingId, rptJsonParam, ExportFileType.Pdf, false);
				base.ViewData["filePath"] = value;
			}
			SECInviteUserInfoModel resultData = _perm.GetUserInviteInfo(base.MContext.MUserID, null).ResultData;
			base.ViewData["userName"] = resultData.MFirstName + " " + resultData.MLastName;
			base.ViewData["userEmail"] = resultData.MEmail;
			List<KeyValuePair<string, string>> emailTmplVariableList = GetEmailTmplVariableList((EmailSendTypeEnum)sendType);
			base.ViewData["EmailTmplVariableList"] = emailTmplVariableList;
			base.ViewData["EmailTmplVariableJson"] = JsonConvert.SerializeObject(emailTmplVariableList);

			
			return base.View();
		}

		public ActionResult SelectPrintSetting(string type, string param)
		{
			string arg = string.Empty;
			switch (type)
			{
			case "Invoices":
			case "Invoice":
			{
				arg = HtmlLang.GetText(LangModule.Report, "Invoice", "Invoice");
				string[] array = HttpUtility.UrlDecode(param).TrimStart('?').Split('&');
				string[] array2 = array;
						foreach (string text in array2)
						{
							if (text.Split('=')[0] == "reportType")
							{

								ViewBag.ReportType = text.Split('=')[1];
								break;
							}
						}
				break;
			}
			case "Statements":
			case "Statement":
				arg = HtmlLang.GetText(LangModule.IV, "statement", "statement");
				break;
			}

			ViewBag.Title = string.Format(HtmlLang.GetText(LangModule.IV, "ApplyBranding", "What branding would you like to apply to the {0}?"), arg);
			ViewBag.Type = type;
			ViewBag.Param = param;
			ViewBag.BizObject = (type == 220.ToString()) ? "PayRun" : string.Empty;

			return View();
		}

		[Permission("Invoice_Sales", "View", "")]
		public ActionResult Statements(string endDate = "")
		{
			DateTime dateNow = base.MContext.DateNow;
			base.ViewData["FirstDayOfMonth"] = dateNow.AddDays((double)(1 - dateNow.Day));
			ViewDataDictionary viewData = base.ViewData;
			DateTime dateTime = dateNow.AddDays((double)(1 - dateNow.Day));
			dateTime = dateTime.AddMonths(1);
			viewData["LastDayOfMonth"] = dateTime.AddDays(-1.0);
			return base.View();
		}

		[Permission("Invoice_Sales", "View", "")]
		public ActionResult ViewStatement(string statementType, string statementContactID, string startDate, string endDate)
		{
			DateTime dateNow = base.MContext.DateNow;
			DateTime beginDate = startDate.ToDateTime(Convert.ToDateTime(dateNow.AddDays((double)(1 - dateNow.Day)).ToString("yyyy-MM-dd")));
			DateTime endDate2 = endDate.ToDateTime(Convert.ToDateTime(dateNow.ToString("yyyy-MM-dd")));
			base.ViewData["statementType"] = statementType;
			base.ViewData["startDate"] = beginDate.ToString("yyyy-MM-dd");
			base.ViewData["endDate"] = endDate2.ToString("yyyy-MM-dd");
			base.ViewData["statementContactID"] = statementContactID;
			base.ViewData["contactModel"] = _contact.GetStatementContData(statementContactID, null).ResultData;
			base.ViewData["viewPreData"] = base._invoice.GetViewStatementOpeningBalanceDate(statementType, statementContactID, beginDate, null).ResultData;
			base.ViewData["viewStateData"] = base._invoice.GetViewStatementData(statementContactID, statementType, beginDate, endDate2, null).ResultData;
			return base.View();
		}

		[Permission("Invoice_Sales", "View", "")]
		public ActionResult EditContactAddr(string id, string number)
		{
			base.ViewData["contactID"] = id;
			base.ViewData["viewFrom"] = number;
			return base.View();
		}

		[Permission("Invoice_Sales", "View", "")]
		public JsonResult GetSendInvoiceList(IVInvoiceSendParam param)
		{
			return base.Json(base._invoice.GetSendInvoiceList(param, null));
		}

		[Permission("Invoice_Sales", "Change", "")]
		public JsonResult SendInvoiceList(IVInvoiceEmailSendModel model)
		{
			//if(null != model.SendEntryList && model.SendEntryList.Count > 0)
   //         {
			//	var entry = model.SendEntryList.First();
			//	// 超级账本POC
			//	var invoice = _invoice.GetInvoiceEditModel(entry.MID, string.Empty).ResultData;
			//	var item = invoice.MEntryList.FirstOrDefault();
			//	if (null != item)
			//	{
			//		// 调用http://siritech.com.cn:8080/create?number=INVOICE1&item=Laptop&desc=DELL&quantity=1&price=5000&total=5000
			//		var number = int.Parse(invoice.MNumber.Substring(invoice.MNumber.LastIndexOf('-') + 1));
			//		var client = new WebClient();
			//		client.DownloadString($"http://siritech.com.cn:8080/create?number=INVOICE{number}&item={item.MItemID}&desc={item.MDesc}&quantity={item.MQty}&price={item.MPrice}&total={invoice.MTotalAmt}");
			//	}
			//}

			return base.Json(base._invoice.SendInvoiceList(model, null));
		}

		[Permission("Invoice_Sales", "View", "")]
		private List<KeyValuePair<string, string>> GetEmailTmplVariableList(EmailSendTypeEnum sendType)
		{
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			switch (sendType)
			{
			case EmailSendTypeEnum.Invoice:
			case EmailSendTypeEnum.RepeatingInvoice:
				list.Add(new KeyValuePair<string, string>("[Current Month]", LangHelper.GetText(LangModule.BD, "CurrentMonth", "Current Month")));
				list.Add(new KeyValuePair<string, string>("[Contact Name]", LangHelper.GetText(LangModule.Common, "ContactName", "Contact Name")));
				list.Add(new KeyValuePair<string, string>("[Contact Primary Person]", LangHelper.GetText(LangModule.Common, "ContactPrimaryPerson", "Contact Primary Person")));
				list.Add(new KeyValuePair<string, string>("[Invoice Number]", LangHelper.GetText(LangModule.IV, "InvoiceNumber", "Invoice Number")));
				list.Add(new KeyValuePair<string, string>("[Amount Due]", LangHelper.GetText(LangModule.IV, "AmountDue", "Amount Due")));
				list.Add(new KeyValuePair<string, string>("[Due Date]", LangHelper.GetText(LangModule.IV, "DueDate", "Due date")));
				list.Add(new KeyValuePair<string, string>("[Invoice Total]", LangHelper.GetText(LangModule.IV, "InvoiceTotal", "Invoice Total")));
				list.Add(new KeyValuePair<string, string>("[Organization Name]", LangHelper.GetText(LangModule.Common, "OrganizationName", "Organization Name")));
				list.Add(new KeyValuePair<string, string>("[Trading Name]", LangHelper.GetText(LangModule.Common, "TradingName", "Trading Name")));
				break;
			case EmailSendTypeEnum.Statement:
				list.Add(new KeyValuePair<string, string>("[Contact Name]", LangHelper.GetText(LangModule.Common, "ContactName", "Contact Name")));
				list.Add(new KeyValuePair<string, string>("[Contact Primary Person]", LangHelper.GetText(LangModule.Common, "ContactPrimaryPerson", "Contact Primary Person")));
				list.Add(new KeyValuePair<string, string>("[Statement Date]", LangHelper.GetText(LangModule.IV, "StatementDate", "Statement Date")));
				list.Add(new KeyValuePair<string, string>("[Statement Begin Date]", LangHelper.GetText(LangModule.IV, "StatementBeginDate", "Statement Begin Date")));
				list.Add(new KeyValuePair<string, string>("[Statement End Date]", LangHelper.GetText(LangModule.IV, "StatementEndDate", "Statement End Date")));
				list.Add(new KeyValuePair<string, string>("[Organization Name]", LangHelper.GetText(LangModule.Common, "OrganizationName", "Organization Name")));
				break;
			case EmailSendTypeEnum.Payslip:
				list.Add(new KeyValuePair<string, string>("[Employee Name]", LangHelper.GetText(LangModule.Common, "EmployeeName", "Employee Name")));
				list.Add(new KeyValuePair<string, string>("[Salary Period]", LangHelper.GetText(LangModule.PA, "SalaryPeriod", "Salary Period")));
				list.Add(new KeyValuePair<string, string>("[Organization Name]", LangHelper.GetText(LangModule.Common, "OrganizationName", "Organization Name")));
				break;
			}
			return list;
		}

		public JsonResult PreviewEmailTmpl(IVInvoiceEmailSendModel model)
		{
			return base.Json(base._invoice.PreviewEmailTmpl(model, null));
		}

		[Permission("BankAccount", "Change", "")]
		public JsonResult AddInvoiceReceive(IVMakePaymentModel model)
		{
			MActionResult<OperationResult> data = base._invoice.PayToInvoice(model, null);
			return base.Json(data);
		}

		[Permission("BankAccount", "Change", "")]
		public JsonResult AddInvoicePayment(IVMakePaymentModel model)
		{
			MActionResult<OperationResult> data = base._invoice.PayToInvoice(model, null);
			return base.Json(data);
		}

		[Permission("Invoice_Sales", "View", "")]
		public JsonResult GetInComingChartData(string Type, string contactId = null)
		{
			MActionResult<string> chartStackedDictionary = base._invoice.GetChartStackedDictionary(Type, base.StartDate, base.EndDate, contactId, null);
			return base.Json(chartStackedDictionary);
		}

		[Permission("Invoice_Sales", "View", "")]
		public JsonResult GetOwingMostChartData(string Type)
		{
			MActionResult<List<ChartPie2DModel>> chartPieDictionary = base._invoice.GetChartPieDictionary(Type, base.StartDate, base.EndDate, null);
			return base.Json(chartPieDictionary);
		}

		[Permission("Invoice_Sales", "View", "")]
		public JsonResult GetContactAddr(BDContactsInfoModel model)
		{
			BDContactsInfoModel data = null;
			if (model.MItemID == null)
			{
				return base.Json(data);
			}
			BDContactsInfoModel resultData = _contact.GetContactEditInfo(model, null).ResultData;
			if (resultData != null)
			{
				data = resultData;
			}
			return base.Json(data);
		}

		[Permission("Invoice_Sales", "Change", "")]
		public JsonResult UpdateStateContactAddr(BDContactsInfoModel model)
		{
			List<string> fields = new List<string>
			{
				"MEmail",
				"MPAttention",
				"MPStreet",
				"MPCityID",
				"MPRegion",
				"MPPostalNo",
				"MPCountryID"
			};
			BDContactsInfoModel resultData = _contact.GetStatementContData(model.MItemID, null).ResultData;
			model.MIsCustomer = resultData.MIsCustomer;
			model.MIsSupplier = resultData.MIsSupplier;
			model.MIsOther = resultData.MIsOther;
			return base.Json(_contact.ContactsUpdate(model, fields, null).ResultData);
		}

		[Permission("Invoice_Sales", "View", "")]
		public JsonResult GetStatementData(IVStatementListFilterModel param)
		{
			MActionResult<List<IVStatementsModel>> statementData = base._invoice.GetStatementData(param, null);
			return base.Json(statementData);
		}

		[Permission("Invoice_Sales", "View", "")]
		public JsonResult GetEmailTmplList(EmailSendTypeEnum sendType)
		{
			MActionResult<List<BDEmailTemplateModel>> list = _emailTmpl.GetList(sendType, null);
			return base.Json(list);
		}

		[Permission("Invoice_Sales", "Change", "")]
		public JsonResult EmailTmplAdd(BDEmailTemplateModel model)
		{
			MActionResult<OperationResult> data = _emailTmpl.InsertOrUpdate(model, null, null);
			return base.Json(data);
		}

		[Permission("Invoice_Sales", "Change", "")]
		public JsonResult DeleteEmailTmpl(string id)
		{
			MActionResult<OperationResult> data = _emailTmpl.Delete(id, null);
			return base.Json(data);
		}
	}
}
