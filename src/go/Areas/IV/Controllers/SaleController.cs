using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Identity;
using JieNor.Megi.Identity.Attribute;
using JieNor.Megi.ServiceContract.BD;
using JieNor.Megi.ServiceContract.IV;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.IV.Controllers
{
	public class SaleController : IVInvoiceBaseController
	{
		private IBDContacts _Cont = null;

		public SaleController(IIVInvoice invoice, IBDContacts Cont)
			: base(invoice)
		{
			_Cont = Cont;
		}

		[Permission("Invoice_Sales", "View", "")]
		public ActionResult Index()
		{
			base.SetTitle(LangHelper.GetText(LangModule.IV, "Sales", "Sales"));
			base.ViewData["TypeName"] = "Sales";
			base.ViewData["ListLink"] = "/IV/Invoice/InvoiceList";
			base.SetVDSummary("Invoice_Sale");
			List<ChartPie2DModel> resultData = base._invoice.GetChartPieDictionary("'Invoice_Sale','Invoice_Sale_Red'", base.StartDate, base.EndDate, null).ResultData;
			base.ViewData["PieList"] = resultData;
			return base.MView();
		}

		public ActionResult Statements(string endDate = "")
		{
			ViewDataDictionary viewData = base.ViewData;
			DateTime dateTime = DateTime.Now;
			DateTime now = DateTime.Now;
			viewData["FirstDayOfMonth"] = dateTime.AddDays((double)(1 - now.Day));
			ViewDataDictionary viewData2 = base.ViewData;
			dateTime = DateTime.Now;
			now = DateTime.Now;
			dateTime = dateTime.AddDays((double)(1 - now.Day));
			dateTime = dateTime.AddMonths(1);
			viewData2["LastDayOfMonth"] = dateTime.AddDays(-1.0);
			base.SetTitleAndCrumb("Statements", "<a href='/IV/Sale/Index'>" + LangHelper.GetText(LangModule.IV, "Sales", "Sales") + " > </a>");
			return base.View();
		}

		public ActionResult EditContactAddr(string id, string number)
		{
			base.ViewData["contactID"] = id;
			base.ViewData["viewFrom"] = number;
			return base.View();
		}

		public JsonResult GetContactAddr(BDContactsInfoModel model)
		{
			BDContactsInfoModel data = null;
			if (model.MItemID == null)
			{
				return base.Json(data);
			}
			BDContactsInfoModel resultData = _Cont.GetContactEditInfo(model, null).ResultData;
			if (resultData != null)
			{
				data = resultData;
			}
			return base.Json(data);
		}

		public JsonResult UpdateStateContactAddr(BDContactsInfoModel model)
		{
			_Cont.ContactsUpdate(model, null, null);
			return base.Json(true);
		}

		public ActionResult ViewStatement(string statementType, string statementContactID, string startDate = "", string endDate = "")
		{
			DateTime now = DateTime.Now;
			BDContactsInfoModel resultData = _Cont.GetStatementContData(statementContactID, null).ResultData;
			string arg = string.Empty;
			if (resultData != null)
			{
				arg = resultData.MName;
			}
			base.SetTitleAndCrumb(string.Format("Statement for {0} as at {1}", arg, (endDate == "") ? now.ToString("MM/dd/yyyy") : endDate.Replace('-', '/')), "<a href='/IV/Sale/Index'>" + LangHelper.GetText(LangModule.IV, "Sales", "Sales") + " > </a><a href='/IV/Sale/Statements'>" + LangHelper.GetText(LangModule.IV, "Statements", "Statements") + " > </a>");
			base.ViewData["statementType"] = statementType;
			DateTime dateTime;
			if (!(startDate == ""))
			{
				dateTime = Convert.ToDateTime(startDate);
			}
			else
			{
				DateTime dateTime2 = now.AddDays((double)(1 - now.Day));
				dateTime2 = dateTime2.AddMonths(-1);
				dateTime = dateTime2.Date;
			}
			DateTime beginDate = dateTime;
			DateTime endDate2 = (endDate == "") ? now.Date : Convert.ToDateTime(endDate);
			base.ViewData["startDate"] = beginDate.ToShortDateString();
			base.ViewData["endDate"] = endDate2.ToShortDateString();
			base.ViewData["statementContactID"] = statementContactID;
			base.ViewData["contactModel"] = resultData;
			base.ViewData["viewStateData"] = base._invoice.GetViewStatementData(statementContactID, statementType, beginDate, endDate2, null).ResultData;
			base.ViewData["viewPreData"] = base._invoice.GetViewStatementOpeningBalanceDate(statementType, statementContactID, beginDate, null);
			return base.View();
		}

		public JsonResult GetStatementData(IVStatementListFilterModel param)
		{
			MActionResult<List<IVStatementsModel>> statementData = base._invoice.GetStatementData(param, null);
			return base.Json(statementData);
		}
	}
}
