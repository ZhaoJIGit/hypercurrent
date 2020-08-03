using JieNor.Megi.Common.Context;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IV;
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
	public class IVInvoiceBaseController : IVBaseController
	{
		protected IIVInvoice _invoice = null;

		protected IBDTrack _track = null;

		protected IIVVerification _verification = null;

		protected IBDContacts _bdContact = null;

		public DateTime StartDate => DateTime.MinValue;

		public DateTime EndDate => DateTime.MaxValue;

		public IVInvoiceBaseController(IIVInvoice invoice)
		{
			_invoice = invoice;
		}

		public IVInvoiceBaseController(IBASOrganisation org, IIVInvoice invoice, IBDTrack track, IIVVerification verification, IBDContacts bdContact, IBDEmailTemplate emailTmpl, IGLSettlement settle)
			: base(org, settle)
		{
			_invoice = invoice;
			_track = track;
			_verification = verification;
			_bdContact = bdContact;
		}

		public JsonResult GetRepeatInvoiceList(IVInvoiceListFilterModel param)
		{
			if ((param.MStartDate == (DateTime?)DateTime.MinValue || !param.MStartDate.HasValue) && (param.MEndDate == (DateTime?)DateTime.MinValue || !param.MEndDate.HasValue))
			{
				param.MStartDate = StartDate;
				param.MEndDate = EndDate;
				param.MSearchWithin = IVInvoiceSearchWithinEnum.NextInvoiceDate;
			}
			MActionResult<DataGridJson<IVRepeatInvoiceListModel>> repeatInvoiceList = _invoice.GetRepeatInvoiceList(param, null);
			return base.Json(repeatInvoiceList);
		}

		public JsonResult UpdateRepeatInvoice(IVRepeatInvoiceModel model)
		{
			model.MOrgID = base.MContext.MOrgID;
			MActionResult<OperationResult> data = _invoice.UpdateRepeatInvoice(model, null);
			return base.Json(data);
		}

		public JsonResult UpdateRepeatInvoiceStatus(ParamBase param)
		{
			OperationResult resultData = _invoice.UpdateRepeatInvoiceStatus(param, null).ResultData;
			return base.Json(resultData);
		}

		public JsonResult DeleteRepeatInvoiceList(ParamBase param)
		{
			OperationResult resultData = _invoice.DeleteRepeatInvoiceList(param, null).ResultData;
			return base.Json(resultData);
		}

		protected IVRepeatInvoiceModel SetVDRepeatInvoice(string invoiceId, string billType)
		{
			IVRepeatInvoiceModel iVRepeatInvoiceModel = null;
			string text = base.Request.QueryString["copyInvoiceId"];
			if (!string.IsNullOrWhiteSpace(text))
			{
				IVInvoiceModel resultData = _invoice.GetInvoiceEditModel(text, billType, null).ResultData;
				iVRepeatInvoiceModel = InvoiceToRepeatInvoice(resultData);
			}
			else
			{
				iVRepeatInvoiceModel = _invoice.GetRepeatInvoiceEditModel(invoiceId, null).ResultData;
			}
			RemoveInactiveBD(iVRepeatInvoiceModel);
			base.ViewData["RepeatInvoiceModel"] = iVRepeatInvoiceModel;
			base.ViewData["MCurrencyID"] = iVRepeatInvoiceModel.MCyID;
			base.ViewData["InvoiceID"] = invoiceId;
			return iVRepeatInvoiceModel;
		}

		private IVRepeatInvoiceModel InvoiceToRepeatInvoice(IVInvoiceModel iviModel)
		{
			IVRepeatInvoiceModel iVRepeatInvoiceModel = new IVRepeatInvoiceModel();
			if (iviModel == null)
			{
				return iVRepeatInvoiceModel;
			}
			iVRepeatInvoiceModel.MOrgID = iviModel.MOrgID;
			iVRepeatInvoiceModel.MType = iviModel.MType;
			iVRepeatInvoiceModel.MContactID = iviModel.MContactID;
			iVRepeatInvoiceModel.MBizDate = iviModel.MBizDate;
			iVRepeatInvoiceModel.MExpectedDate = iviModel.MExpectedDate;
			iVRepeatInvoiceModel.MBranding = iviModel.MBranding;
			iVRepeatInvoiceModel.MAttachCount = iviModel.MAttachCount;
			iVRepeatInvoiceModel.MTaxID = iviModel.MTaxID;
			iVRepeatInvoiceModel.MCyID = iviModel.MCyID;
			iVRepeatInvoiceModel.MExchangeRate = iviModel.MExchangeRate;
			iVRepeatInvoiceModel.MTotalAmtFor = iviModel.MTotalAmtFor;
			iVRepeatInvoiceModel.MTotalAmt = iviModel.MTotalAmt;
			iVRepeatInvoiceModel.MTaxTotalAmtFor = iviModel.MTaxTotalAmtFor;
			iVRepeatInvoiceModel.MTaxTotalAmt = iviModel.MTaxTotalAmt;
			iVRepeatInvoiceModel.MReference = iviModel.MReference;
			iVRepeatInvoiceModel.MDesc = iviModel.MDesc;
			List<IVRepeatInvoiceEntryModel> list = new List<IVRepeatInvoiceEntryModel>();
			foreach (IVInvoiceEntryModel item in iviModel.InvoiceEntry)
			{
				IVRepeatInvoiceEntryModel iVRepeatInvoiceEntryModel = new IVRepeatInvoiceEntryModel();
				iVRepeatInvoiceEntryModel.MSeq = item.MSeq;
				iVRepeatInvoiceEntryModel.MItemID = item.MItemID;
				iVRepeatInvoiceEntryModel.MAcctID = item.MAcctID;
				iVRepeatInvoiceEntryModel.MTaxID = item.MTaxID;
				iVRepeatInvoiceEntryModel.MTrackItem1 = item.MTrackItem1;
				iVRepeatInvoiceEntryModel.MTrackItem2 = item.MTrackItem2;
				iVRepeatInvoiceEntryModel.MTrackItem3 = item.MTrackItem3;
				iVRepeatInvoiceEntryModel.MTrackItem4 = item.MTrackItem4;
				iVRepeatInvoiceEntryModel.MTrackItem5 = item.MTrackItem5;
				iVRepeatInvoiceEntryModel.MQty = item.MQty;
				iVRepeatInvoiceEntryModel.MPrice = item.MPrice;
				iVRepeatInvoiceEntryModel.MDiscount = item.MDiscount;
				iVRepeatInvoiceEntryModel.MAmountFor = item.MAmountFor;
				iVRepeatInvoiceEntryModel.MAmount = item.MAmount;
				iVRepeatInvoiceEntryModel.MTaxAmountFor = item.MTaxAmountFor;
				iVRepeatInvoiceEntryModel.MTaxAmount = item.MTaxAmount;
				iVRepeatInvoiceEntryModel.MTaxAmtFor = item.MTaxAmtFor;
				iVRepeatInvoiceEntryModel.MTaxAmt = item.MTaxAmt;
				iVRepeatInvoiceEntryModel.MDesc = item.MDesc;
				list.Add(iVRepeatInvoiceEntryModel);
			}
			iVRepeatInvoiceModel.RepeatInvoiceEntry = list;
			return iVRepeatInvoiceModel;
		}

		protected void RemoveInactiveBD(IVRepeatInvoiceModel model)
		{
			List<BDCheckInactiveModel> resultData = _invoice.GetInactiveList(null).ResultData;
			if (model != null)
			{
				if (resultData.Any((BDCheckInactiveModel m) => m.MItemID == model.MContactID && m.ObjectType == "Contact"))
				{
					model.MContactID = "";
				}
				List<IVRepeatInvoiceEntryModel> mEntryList = model.MEntryList;
				foreach (IVRepeatInvoiceEntryModel item in mEntryList)
				{
					if (resultData.Any((BDCheckInactiveModel m) => m.MItemID == item.MItemID && m.ObjectType == "Item"))
					{
						item.MItemID = "";
					}
					if (resultData.Any((BDCheckInactiveModel m) => m.MItemID == item.MTrackItem1 && m.ObjectType == "Track"))
					{
						item.MTrackItem1 = "";
					}
					if (resultData.Any((BDCheckInactiveModel m) => m.MItemID == item.MTrackItem2 && m.ObjectType == "Track"))
					{
						item.MTrackItem2 = "";
					}
					if (resultData.Any((BDCheckInactiveModel m) => m.MItemID == item.MTrackItem3 && m.ObjectType == "Track"))
					{
						item.MTrackItem3 = "";
					}
					if (resultData.Any((BDCheckInactiveModel m) => m.MItemID == item.MTrackItem4 && m.ObjectType == "Track"))
					{
						item.MTrackItem4 = "";
					}
					if (resultData.Any((BDCheckInactiveModel m) => m.MItemID == item.MTrackItem5 && m.ObjectType == "Track"))
					{
						item.MTrackItem5 = "";
					}
				}
			}
		}

		protected bool SetCanEditOrVoidOrDelete(string id)
		{
			bool resultData = _verification.CheckIsCanEditOrVoidOrDelete("Invoice", id, null).ResultData;
			base.ViewData["IsCanEditOrVoidOrDelete"] = resultData;
			return resultData;
		}

		public JsonResult AddInvoiceNoteLog(IVInvoiceModel model)
		{
			model.MOrgID = base.MContext.MOrgID;
			return base.Json(_invoice.AddInvoiceNoteLog(model, null));
		}

		protected void SetVDSummary(string type)
		{
			IVInvoiceSummaryModel resultData = _invoice.GetInvoiceSummaryModel(type, StartDate, EndDate, null).ResultData;
			base.ViewData["InvoiceSummary"] = resultData;
		}

		public JsonResult GetSummaryModel(string type)
		{
			return base.Json(_invoice.GetInvoiceSummaryModel(type, StartDate, EndDate, null));
		}

		protected void SetVDVerification(string invoiceId)
		{
			List<IVVerificationInforModel> resultData = _verification.GetCustomerWaitForVerificationInforByBillId(invoiceId, "Invoice", null).ResultData;
			base.ViewData["VerificationList"] = resultData;
			base.ViewData["IsShowVerif"] = (base.Request["IsShowVerif"] != null && resultData != null && resultData.Count != 0 && Convert.ToBoolean(base.Request["IsShowVerif"]));
		}

		protected IVInvoiceModel SetVDInvoice(string invoiceId, string billType)
		{
			base.ViewData["InvoiceID"] = invoiceId;
			IVInvoiceModel iVInvoiceModel = null;
			string text = base.Request["cpyId"];
			bool flag = base.Request["isCopyCredit"] != null && Convert.ToBoolean(base.Request["isCopyCredit"]);
			if (!string.IsNullOrEmpty(text))
			{
				iVInvoiceModel = _invoice.GetInvoiceCopyModel(text, flag, null).ResultData;
				base.ViewData["InvCopyID"] = text;
			}
			else
			{
				iVInvoiceModel = _invoice.GetInvoiceEditModel(invoiceId, billType, null).ResultData;
				base.ViewData["BillType"] = billType;
			}
			base.ViewData["InvoiceModel"] = iVInvoiceModel;
			base.ViewData["MCurrencyID"] = iVInvoiceModel.MCyID;
			base.ViewData["isCopyCredit"] = flag;
			base.ViewData["InvoiceType"] = iVInvoiceModel.MType;
			return iVInvoiceModel;
		}

		public JsonResult GetInvoiceList(IVInvoiceListFilterModel param)
		{
			if ((param.MStartDate == (DateTime?)DateTime.MinValue || !param.MStartDate.HasValue) && (param.MEndDate == (DateTime?)DateTime.MinValue || !param.MEndDate.HasValue))
			{
				param.MStartDate = StartDate;
				param.MEndDate = EndDate;
				param.MSearchWithin = IVInvoiceSearchWithinEnum.TransactionDate;
			}
			MActionResult<DataGridJson<IVInvoiceListModel>> invoiceList = _invoice.GetInvoiceList(param, null);
			return base.Json(invoiceList);
		}

		public JsonResult GetFPInvoiceSummary(IVInvoiceListFilterModel param)
		{
			return base.Json(_invoice.GetFPInvoiceSummary(param, null));
		}

		public JsonResult GetInvoiceListByFilter(IVInvoiceListFilterModel param)
		{
			if ((param.MStartDate == (DateTime?)DateTime.MinValue || !param.MStartDate.HasValue) && (param.MEndDate == (DateTime?)DateTime.MinValue || !param.MEndDate.HasValue))
			{
				param.MStartDate = StartDate;
				param.MEndDate = EndDate;
				param.MSearchWithin = IVInvoiceSearchWithinEnum.TransactionDate;
			}
			MActionResult<DataGridJson<IVInvoiceListModel>> invoiceList = _invoice.GetInvoiceList(param, null);
			return base.Json(invoiceList);
		}

		public JsonResult UpdateInvoiceExpectedInfo(IVInvoiceModel model)
		{
			MActionResult<OperationResult> data = _invoice.UpdateInvoiceExpectedInfo(model, null);
			return base.Json(data);
		}

		public JsonResult DeleteInvoiceList(ParamBase param)
		{
			MActionResult<OperationResult> data = _invoice.DeleteInvoiceList(param, null);
			return base.Json(data);
		}

		public JsonResult GetContactInfo(string contactId)
		{
			MActionResult<BDContactsInfoModel> contactViewData = _bdContact.GetContactViewData(contactId, null);
			return base.Json(contactViewData);
		}

		public JsonResult UpdateInvoiceStatus(ParamBase param)
		{
			MActionResult<OperationResult> data = _invoice.UpdateInvoiceStatus(param, null);
			return base.Json(data);
		}

		public JsonResult UnApproveInvoice(string invoiceId)
		{
			MActionResult<OperationResult> data = _invoice.UnApproveInvoice(invoiceId, null);
			return base.Json(data);
		}

		public JsonResult ApproveInvoice(ParamBase param)
		{
			MActionResult<OperationResult> data = _invoice.ApproveInvoice(param, null);
			return base.Json(data);
		}

		public JsonResult UpdateInvoice(IVInvoiceModel model)
		{
			model.MOrgID = base.MContext.MOrgID;
			OperationResult operationResult = new OperationResult();
			if (model.MBizDate < ContextHelper.MContext.MBeginDate && ContextHelper.MContext.MInitBalanceOver)
			{
				operationResult.Success = false;
				operationResult.Message = HtmlLang.GetText(LangModule.BD, "InitBalanceIsOver", "The initial balance has been completed and is not allowed to initialize the document operation!");
				return base.Json(operationResult);
			}
			MActionResult<OperationResult> data = _invoice.UpdateInvoice(model, null);
			return base.Json(data);
		}

		public JsonResult GetVerificationById(string invoiceId)
		{
			return base.Json(_verification.GetCustomerWaitForVerificationInforByBillId(invoiceId, "Invoice", null));
		}

		public JsonResult GetInvEditModel(string invId)
		{
			return base.Json(_invoice.GetInvoiceEditModel(invId, "Invoice_Sale", null));
		}
	}
}
