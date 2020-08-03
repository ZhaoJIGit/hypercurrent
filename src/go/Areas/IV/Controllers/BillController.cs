using JieNor.Megi.Common.Context;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.Identity.Attribute;
using JieNor.Megi.Identity.Go.HtmlHelper;
using JieNor.Megi.ServiceContract.BAS;
using JieNor.Megi.ServiceContract.BD;
using JieNor.Megi.ServiceContract.GL;
using JieNor.Megi.ServiceContract.IV;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.IV.Controllers
{
	public class BillController : IVInvoiceBaseController
	{
		public BillController(IBASOrganisation org, IIVInvoice invoice, IBDTrack track, IIVVerification verification, IBDContacts bdContact, IBDEmailTemplate emailTmpl, IGLSettlement settle)
			: base(org, invoice, track, verification, bdContact, emailTmpl, settle)
		{
		}

		[Permission("Invoice_Purchases", "View", "")]
		public JsonResult GetRepeatBillList(IVInvoiceListFilterModel param)
		{
			return base.GetRepeatInvoiceList(param);
		}

		[Permission("Invoice_Purchases", "Change", "")]
		public JsonResult UpdateRepeatBill(IVRepeatInvoiceModel model)
		{
			return base.UpdateRepeatInvoice(model);
		}

		[Permission("Invoice_Purchases", "Change", "")]
		public JsonResult UpdateRepeatBillStatus(ParamBase param)
		{
			return base.UpdateRepeatInvoiceStatus(param);
		}

		[Permission("Invoice_Purchases", "Change", "")]
		public JsonResult DeleteRepeatBillList(ParamBase param)
		{
			return base.DeleteRepeatInvoiceList(param);
		}

		private ActionResult GetRepeatInvoiceWithTrackingAction(string id)
		{
			base.SetVDRepeatInvoice(id, "Invoice_Purchases");
			return base.View();
		}

		[Permission("Invoice_Purchases", "View", "")]
		public ActionResult RepeatBillEdit(string id)
		{
			if (!HtmlSECMenu.HavePermission("Invoice_Purchases", "Change", ""))
			{
				return Redirect(string.Format("/IV/Bill/RepeatBillView/{0}?tabIndex={1}", id, base.Request.QueryString["tabIndex"]));
			}
			base.SetDefaultBizDate(false);
			return GetRepeatInvoiceWithTrackingAction(id);
		}

		[Permission("Invoice_Purchases", "View", "")]
		public ActionResult RepeatBillView(string id)
		{
			if (HtmlSECMenu.HavePermission("Invoice_Purchases", "Change", ""))
			{
				return Redirect(string.Format("/IV/Bill/RepeatBillEdit/{0}?tabIndex={1}", id, base.Request.QueryString["tabIndex"]));
			}
			base.SetDefaultBizDate(false);
			return GetRepeatInvoiceWithTrackingAction(id);
		}

		[Permission("Invoice_Purchases", "Change", "")]
		public JsonResult AddBillNoteLog(IVInvoiceModel model)
		{
			return base.AddInvoiceNoteLog(model);
		}

		[Permission("Invoice_Purchases", "View", "")]
		public ActionResult BillList(int? id, string pieListType)
		{
			if (!id.HasValue || id < 0 || id > 6)
			{
				id = 0;
			}
			ViewBag.InvoiceType = id;

			base.ViewData["TypeName"] = "Bills";
			base.ViewData["ListLink"] = "/IV/Bill/BillList";
			base.SetVDSummary("Invoice_Purchase");
			List<ChartPie2DModel> resultData = base._invoice.GetChartPieDictionary("'Invoice_Purchase','Invoice_Purchase_Red'", base.StartDate, base.EndDate, null).ResultData;
			base.ViewData["PieList"] = resultData;
			if (string.IsNullOrWhiteSpace(pieListType))
			{
				pieListType = "limit";
			}
			base.ViewData["pieListType"] = pieListType;
			return base.View();
		}

		[Permission("Invoice_Purchases", "View", "")]
		public JsonResult GetInComingChartData(string Type, string contactId = null)
		{
			MActionResult<string> chartStackedDictionary = base._invoice.GetChartStackedDictionary(Type, base.StartDate, base.EndDate, contactId, null);
			return base.Json(chartStackedDictionary);
		}

		private ActionResult GetBillAction(string id)
		{
			base.SetVDInvoice(id, "Invoice_Purchase");
			base.SetVDVerification(id);
			return base.View();
		}

		[Permission("Invoice_Purchases", "View", "")]
		public ActionResult BillEdit(string id)
		{
			base.ViewData["isEdit"] = true;
			base.ViewData["From"] = "Edit";
			base.SetDefaultBizDate(false);
			ActionResult billAction = GetBillAction(id);
			IVInvoiceModel iVInvoiceModel = base.ViewData["InvoiceModel"] as IVInvoiceModel;
			if (iVInvoiceModel != null && !string.IsNullOrWhiteSpace(iVInvoiceModel.MID) && iVInvoiceModel.MBizDate < ContextHelper.MContext.MBeginDate)
			{
				return BillInitEdit(id);
			}
			return billAction;
		}

		[Permission("Setting", "View", "")]
		public ActionResult BillInitEdit(string id)
		{
			base.ViewData["isEdit"] = true;
			base.ViewData["IsInit"] = "true";
			base.ViewData["BizDateValidType"] = "maxDate";
			base.SetDefaultBizDate(true);
			return GetBillAction(id);
		}

		[Permission("Invoice_Purchases", "View", "")]
		public ActionResult BillView(string id)
		{
			base.ViewData["isEdit"] = false;
			base.SetDefaultBizDate(false);
			ActionResult billAction = GetBillAction(id);
			IVInvoiceModel iVInvoiceModel = base.ViewData["InvoiceModel"] as IVInvoiceModel;
			if (iVInvoiceModel?.MActionPermission.MIsCanEdit ?? false)
			{
				return Redirect($"/IV/Bill/BillEdit/{id}");
			}
			return billAction;
		}

		private ActionResult GetCreditNoteAction(string id)
		{
			base.SetVDInvoice(id, "Invoice_Purchase_Red");
			base.SetVDVerification(id);
			return base.View();
		}

		[Permission("Invoice_Purchases", "View", "")]
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
			base.ViewData["isEdit"] = true;
			base.ViewData["IsInit"] = "true";
			base.ViewData["BizDateValidType"] = "maxDate";
			base.SetDefaultBizDate(true);
			return GetCreditNoteAction(id);
		}

		[Permission("Invoice_Purchases", "View", "")]
		public ActionResult CreditNoteView(string id)
		{
			base.ViewData["isEdit"] = false;
			base.SetDefaultBizDate(false);
			ActionResult creditNoteAction = GetCreditNoteAction(id);
			IVInvoiceModel iVInvoiceModel = base.ViewData["InvoiceModel"] as IVInvoiceModel;
			if (iVInvoiceModel?.MActionPermission.MIsCanEdit ?? false)
			{
				return Redirect($"/IV/Bill/CreditNoteEdit/{id}");
			}
			return creditNoteAction;
		}
	}
}
