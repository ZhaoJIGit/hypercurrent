using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.ImportAndExport;
using JieNor.Megi.Common.ImportAndExport.DataModel;
using JieNor.Megi.Common.ImportAndExport.Export;
using JieNor.Megi.Common.ImportAndExport.Utility;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IO.Export;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.Identity;
using JieNor.Megi.Identity.Attribute;
using JieNor.Megi.Identity.Go.HtmlHelper;
using JieNor.Megi.Identity.HtmlHelper;
using JieNor.Megi.ServiceContract.BD;
using JieNor.Megi.ServiceContract.IV;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.BD.Controllers
{
	public class ContactsController : GoControllerBase
	{
		private IBDContacts _bdCont = null;

		private IBDTrack _track = null;

		private IIVInvoice _invoice = null;

		private IBDEmployees _bdEmp = null;

		public ContactsController()
		{
		}

		public ContactsController(IBDContacts bdContact, IBDTrack track, IIVInvoice invoice, IBDEmployees employee)
		{
			_bdCont = bdContact;
			_track = track;
			_invoice = invoice;
			_bdEmp = employee;
		}

		[Permission("Contact", "View", "")]
		public ActionResult ContactsList(string id = "0")
		{
			base.SetTitle(LangHelper.GetText(LangModule.Contact, "Contacts", "Contacts"));
			base.ViewData["ContactType"] = _bdCont.GetTypeListByWhere(true, null).ResultData;
			base.ViewData["tabKeySelected"] = id;
			return base.View();
		}

		[Permission("Contact", "View", "")]
		public ActionResult GetContactList()
		{
			List<BDContactsInfoModel> resultData = _bdCont.GetContactsInfo("", "", null).ResultData;
			resultData = (resultData ?? new List<BDContactsInfoModel>());
			return base.Json(resultData);
		}

		[Permission("Contact", "View", "")]
		public ActionResult GetContactBasicInfoList()
		{
			string data = HtmlBDContact.DataOptions();
			return base.Json(data);
		}

		[Permission("Contact", "View", "")]
		public JsonResult GetContactItemList(BDContactsListFilter filter)
		{
			filter.MaxCount = 0;
			MActionResult<List<BDContactItem>> contactItemList = _bdCont.GetContactItemList(filter, null);
			return base.Json(contactItemList);
		}

		[Permission("Contact", "View", "")]
		public string GetContactName(string itemId)
		{
			return _bdCont.GetContactName(itemId, null).ResultData;
		}

		[Permission("Contact", "View", "")]
		public ActionResult GetContactEmployeeList()
		{
			List<BDContactsInfoModel> customer = _bdCont.GetContactsListByContactType(1, null, null).ResultData ?? new List<BDContactsInfoModel>();
			List<BDContactsInfoModel> supplier = _bdCont.GetContactsListByContactType(2, null, null).ResultData ?? new List<BDContactsInfoModel>();
			List<BDContactsInfoModel> other = _bdCont.GetContactsListByContactType(4, null, null).ResultData ?? new List<BDContactsInfoModel>();
			List<BDEmployeesListModel> employee = _bdEmp.GetBDEmployeesList("", false, null).ResultData ?? new List<BDEmployeesListModel>();
			return base.Json(new
			{
				Customer = customer,
				Supplier = supplier,
				Employee = employee,
				Other = other
			});
		}

		[Permission("Contact", "View", "")]
		public ActionResult GetContactsEditModel(BDContactsEditModel model)
		{
			model.MOrgID = base.MContext.MOrgID;
			BDContactsEditModel resultData = _bdCont.GetContactsEditModel(model, null).ResultData;
			resultData.MLocaleID = base.MContext.MLCID;
			return base.Json(resultData);
		}

		[Permission("Contact", "Change", "")]
		public ActionResult ContactsEdit(string id, int? tabIndex, string tabTitle)
		{
			string text = base.Request["name"];
			if (!string.IsNullOrWhiteSpace(id))
			{
				BDContactsInfoModel resultData = _bdCont.GetStatementContData(id, null).ResultData;
				string str = (resultData == null) ? "" : resultData.MName;
				base.ViewData["IsQuote"] = (resultData.IsQuote ? "true" : "false");
				base.ViewData["ContactModel"] = resultData;
				if (resultData != null)
				{
					if (resultData.MIsOther && !resultData.MIsCustomer && !resultData.MIsSupplier)
					{
						base.ViewData["ContactType"] = "4";
					}
					else if (resultData.MIsCustomer && resultData.MIsSupplier && !resultData.MIsOther)
					{
						base.ViewData["ContactType"] = "3";
					}
					else if (!resultData.MIsCustomer && resultData.MIsSupplier && !resultData.MIsOther)
					{
						base.ViewData["ContactType"] = "2";
					}
					else if (resultData.MIsCustomer && !resultData.MIsSupplier && !resultData.MIsOther)
					{
						base.ViewData["ContactType"] = "1";
					}
					else if (resultData.MIsOther && resultData.MIsCustomer && resultData.MIsSupplier)
					{
						base.ViewData["ContactType"] = "5";
					}
				}
				else
				{
					base.ViewData["ContactType"] = "0";
				}
				base.SetTitleAndCrumb(LangHelper.GetText(LangModule.Contact, "Edit", "Edit") + " " + str, "<a href='/Contacts/ContactsList'>" + LangHelper.GetText(LangModule.Contact, "Contact", "Contact") + " > </a>");
			}
			else
			{
				base.ViewData["ContactType"] = base.Request.QueryString["contactType"];
				base.ViewData["IsQuote"] = false;
				base.SetTitleAndCrumb(LangHelper.GetText(LangModule.Contact, "AddContact", "Add Contact"), "<a href='/Contacts/ContactsList'>" + LangHelper.GetText(LangModule.Contact, "Contact", "Contact") + " > </a>");
			}
			base.ViewData["ItemId"] = id;
			base.ViewData["trackList"] = _track.GetList("", null).ResultData;
			SqlWhere sqlWhere = new SqlWhere();
			if (!string.IsNullOrWhiteSpace(id))
			{
				sqlWhere.AddFilter("MContactID", SqlOperators.Equal, id);
			}
			base.ViewData["consTrcLink"] = _bdCont.GetTrackLinkList(sqlWhere, null).ResultData;
			base.ViewData["tabIndex"] = tabIndex;
			base.ViewData["TabTitle"] = tabTitle;
			base.ViewData["ContactName"] = (string.IsNullOrEmpty(text) ? "" : text.Trim());

			ViewBag.IsEnableGL = ContextHelper.MContext.MRegProgress >= 12 && true;
			return base.View();
		}

		[Permission("Contact", "View", "")]
		public JsonResult GetContactBasicInfo(string contactId)
		{
			MActionResult<BDContactBasicInfoModel> contactTrackList = _bdCont.GetContactTrackList(contactId, null);
			return base.Json(contactTrackList);
		}

		[Permission("Contact", "Change", "")]
		public ActionResult ContactGroupEdit()
		{
			return base.View();
		}

		[Permission("Contact", "Change", "")]
		public ActionResult AddToGroup(string id)
		{
			base.ViewData["selectIds"] = id;
			return base.View();
		}

		[Permission("Contact", "Change", "")]
		public ActionResult MoveContacts(string id, string number, string title)
		{
			base.ViewData["selectIds"] = id;
			base.ViewData["typeId"] = number;
			base.ViewData["curTitle"] = (string.IsNullOrWhiteSpace(title) ? "" : title);
			return base.View();
		}

		[Permission("Contact", "View", "")]
		public ActionResult ContactView(string id)
		{
			DateTime startDate = DateTime.Now.AddYears(-15);
			DateTime now = DateTime.Now;
			BDContactsInfoModel resultData = _bdCont.GetStatementContData(id, null).ResultData;
			base.ViewData["IsCustomer"] = resultData.MIsCustomer;
			base.ViewData["IsSupplier"] = resultData.MIsSupplier;
			base.ViewData["MIsOther"] = resultData.MIsOther;
			base.ViewData["IsDisable"] = !resultData.MIsActive;
			string text = (resultData == null) ? "" : resultData.MName;
			base.SetTitleAndCrumb(text, "<a href='/Contacts/ContactsList'>" + LangHelper.GetText(LangModule.Contact, "Contact", "Contact") + " > </a>");
			List<ChartPie2DModel> resultData2 = _invoice.GetChartPieDictionary("'Invoice_Sale','Invoice_Sale_Red'", startDate, now, null).ResultData;
			List<ChartPie2DModel> resultData3 = _invoice.GetChartPieDictionary("'Invoice_Purchase','Invoice_Purchase_Red'", startDate, now, null).ResultData;
			base.ViewData["viewID"] = id;
			base.ViewData["viewName"] = JieNor.Megi.Common.Utility.HtmlHelper.Decode(text);
			base.ViewData["saleData"] = resultData2.FirstOrDefault((ChartPie2DModel w) => w.MContactID == id);
			base.ViewData["purData"] = resultData3.FirstOrDefault((ChartPie2DModel w) => w.MContactID == id);
			IVContactInvoiceSummaryModel resultData4 = _invoice.GetInvoiceSummaryModelByContact(id, null).ResultData;
			base.ViewData["InvoiceSummary"] = resultData4;
			return base.View();
		}

		[Permission("Contact", "View", "")]
		public JsonResult GetContactsList(string id, string number)
		{
			MActionResult<List<BDContactsInfoModel>> contactsInfo = _bdCont.GetContactsInfo(id, number, null);
			return base.Json(contactsInfo);
		}

		[Permission("Contact", "View", "")]
		public JsonResult GetPageContactsList(BDContactsInfoFilterModel param)
		{
			MActionResult<DataGridJson<BDContactsInfoModel>> contactPageList = _bdCont.GetContactPageList(param, null);
			return base.Json(contactPageList);
		}

		[Permission("Contact", "Change", "")]
		public JsonResult ContactsUpdate(BDContactsInfoModel model)
		{
			model.MOrgID = base.MContext.MOrgID;
			MActionResult<OperationResult> data = _bdCont.ContactsUpdate(model, null, null);
			return base.Json(data);
		}

		[Permission("Contact", "View", "")]
		public JsonResult GetContactEditInfo(BDContactsInfoModel model)
		{
			BDContactsInfoModel data = null;
			if (model.MItemID == null)
			{
				return base.Json(data);
			}
			BDContactsInfoModel resultData = _bdCont.GetContactEditInfo(model, null).ResultData;
			if (resultData != null)
			{
				data = resultData;
			}
			return base.Json(data);
		}

		[Permission("Contact", "Change", "")]
		public JsonResult ContactsGroupUpdate(BDContactsGroupModel model)
		{
			model.MOrgID = base.MContext.MOrgID;
			MActionResult<OperationResult> data = _bdCont.ContactsGroupUpdate(model, null);
			return base.Json(data);
		}

		[Permission("Contact", "Change", "")]
		public JsonResult ContactGroupMoveFromTo(BDContactsAddGroupModel model)
		{
			OperationResult resultData = _bdCont.ContactToGroup(model.ContactIds, model.GroupId, model.NewGroupMultiLangModel, model.IsGroupExist, model.MoveFromGroupId, null).ResultData;
			return base.Json(resultData);
		}

		[Permission("Contact", "Change", "")]
		public JsonResult ArchiveContact(string keyIDs, bool isActive)
		{
			List<string> contactIds = keyIDs.Split(',').ToList();
			MActionResult<OperationResult> data = _bdCont.ArchiveContact(contactIds, isActive, null);
			return base.Json(data);
		}

		[Permission("Contact", "Change", "")]
		public JsonResult ContactMoveOutGroup(string selIds, string moveFromTypeId)
		{
			_bdCont.ContactMoveOutGroup(selIds, moveFromTypeId, null);
			return base.Json(true);
		}

		[Permission("Contact", "Change", "")]
		public JsonResult DelGroupAndLink(string typeId)
		{
			MActionResult<OperationResult> data = _bdCont.DelGroupAndLink(typeId, null);
			return base.Json(data);
		}

		[Permission("Contact", "Change", "")]
		public JsonResult IsCanDeleteOrInactive(ParamBase param)
		{
			MActionResult<BDIsCanDeleteModel> data = _bdCont.IsCanDeleteOrInactive(param, null);
			return base.Json(data);
		}

		[Permission("Contact", "Change", "")]
		public JsonResult DeleteContact(ParamBase param)
		{
			MActionResult<OperationResult> data = _bdCont.DeleteContact(param, null);
			return base.Json(data);
		}

		[Permission("Contact", "View", "")]
		public JsonResult GetOverPastData(string contactID)
		{
			return base.Json(_invoice.GetOverPastDictionary(contactID, null));
		}

		public FileResult Export(string jsonParam)
		{
			ReportModel reportModel = ReportStorageHelper.CreateReportModel(BizReportType.ContactList, jsonParam, CreateReportModelSource.Export, null, null, null);
			Stream stream = ExportHelper.CreateRptExportFile(reportModel, ExportFileType.Xls);
			string exportName = string.Format("{0} - {1}.xls", reportModel.OrgName, HtmlLang.GetText(LangModule.Common, "Contacts", "Contacts"));
			return base.ExportReport(stream, exportName);
		}

		[Permission("Contact", "View", "")]
		public ActionResult GetContactsListByContactType(BDContactsListFilter filter)
		{
			MActionResult<List<BDContactsInfoModel>> contactsList = _bdCont.GetContactsList(filter, null);
			contactsList.ResultData = (contactsList.ResultData ?? new List<BDContactsInfoModel>());
			return base.Json(contactsList);
		}

		[Permission("Contact", "Change", "")]
		public JsonResult AddContactNoteLog(BDContactsModel model)
		{
			model.MOrgID = base.MContext.MOrgID;
			return base.Json(_bdCont.AddContactNoteLog(model, null));
		}
	}
}
