using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.ImportAndExport;
using JieNor.Megi.Common.ImportAndExport.DataModel;
using JieNor.Megi.Common.ImportAndExport.Export;
using JieNor.Megi.Common.ImportAndExport.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IO.Export;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.Identity;
using JieNor.Megi.Identity.Attribute;
using JieNor.Megi.Identity.HtmlHelper;
using JieNor.Megi.ServiceContract.BAS;
using JieNor.Megi.ServiceContract.BD;
using JieNor.Megi.ServiceContract.REG;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.BD.Controllers
{
	public class ItemController : GoControllerBase
	{
		private IBDItem _item = null;

		private IBDAccount _account = null;

		private IREGTaxRate _taxRate = null;

		private IBASMyHome _myHome = null;

		private IREGCurrency CurrencyService = null;

		public ItemController()
		{
		}

		public ItemController(IBDItem item, IBDAccount account, IREGTaxRate taxRate, IBASMyHome myHome, IREGCurrency currencyService)
		{
			_item = item;
			_account = account;
			_taxRate = taxRate;
			_myHome = myHome;
			CurrencyService = currencyService;
		}

		[Permission("Setting", "View", "")]
		public ActionResult ItemList()
		{
			base.SetTitleAndCrumb(LangHelper.GetText(LangModule.BD, "InventoryItems", "Inventory Items"), "<a href='/Setting/'>" + LangHelper.GetText(LangModule.BD, "GeneralSettings", "General Settings") + " > </a>");
			return base.View();
		}

		[Permission("Setting", "View", "")]
		public ActionResult ItemEdit(string id)
		{
			//if (_003C_003Eo__8._003C_003Ep__0 == null)
			//{
			//	_003C_003Eo__8._003C_003Ep__0 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "ItemID", typeof(ItemController), new CSharpArgumentInfo[2]
			//	{
			//		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
			//		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
			//	}));
			//}
			//_003C_003Eo__8._003C_003Ep__0.Target(_003C_003Eo__8._003C_003Ep__0, base.get_ViewBag(), id);
			ViewBag.ItemID = id;

			BASCurrencyViewModel resultData = CurrencyService.GetBaseCurrency(null).ResultData;
			//if (_003C_003Eo__8._003C_003Ep__1 == null)
			//{
			//	_003C_003Eo__8._003C_003Ep__1 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "BaseCurrency", typeof(ItemController), new CSharpArgumentInfo[2]
			//	{
			//		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
			//		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
			//	}));
			//}
			//_003C_003Eo__8._003C_003Ep__1.Target(_003C_003Eo__8._003C_003Ep__1, base.get_ViewBag(), resultData.MCurrencyID);
			ViewBag.BaseCurrency = resultData.MCurrencyID;


			//if (_003C_003Eo__8._003C_003Ep__2 == null)
			//{
			//	_003C_003Eo__8._003C_003Ep__2 = CallSite<Func<CallSite, object, bool, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "IsEnableGL", typeof(ItemController), new CSharpArgumentInfo[2]
			//	{
			//		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
			//		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
			//	}));
			//}
			//_003C_003Eo__8._003C_003Ep__2.Target(_003C_003Eo__8._003C_003Ep__2, base.get_ViewBag(), ContextHelper.MContext.MRegProgress >= 12 && true);
			ViewBag.IsEnableGL = ContextHelper.MContext.MRegProgress >= 12 && true;

			return View();
		}

		[Permission("Setting", "Export", "")]
		public FileResult Export(string id)
		{
			ExportFileType fileType = (ExportFileType)Enum.Parse(typeof(ExportFileType), id);
			ReportModel reportModel = ReportStorageHelper.CreateReportModel(BizReportType.InventoryItems, string.Empty, CreateReportModelSource.Export, null, id, null);
			Stream stream = ExportHelper.CreateRptExportFile(reportModel, fileType);
			string exportName = string.Format("{0} - {1}.{2}", reportModel.OrgName, HtmlLang.GetText(LangModule.BD, "InventoryItems", "Inventory Items"), id.ToLower());
			return base.ExportReport(stream, exportName);
		}

		public JsonResult GetEditInfo(BDItemModel model)
		{
			return base.Json(_item.GetEditInfo(model, null));
		}

		public JsonResult GetItemList(BDItemListFilterModel filter)
		{
			MActionResult<List<BDItemModel>> itemList = _item.GetItemList(filter, null);
			return base.Json(itemList);
		}

		public JsonResult GetPageItemList(BDItemListFilterModel param)
		{
			MActionResult<DataGridJson<BDItemModel>> pageList = _item.GetPageList(param, null);
			return base.Json(pageList);
		}

		public JsonResult ItemInfoUpd(BDItemModel item)
		{
			MActionResult<OperationResult> data = _item.ItemInfoUpd(item, null);
			return base.Json(data);
		}

		[Permission("Setting", "Change", "")]
		public JsonResult IsCanDeleteOrInactive(ParamBase param)
		{
			return base.Json(_item.IsCanDeleteOrInactive(param, null));
		}

		[Permission("Setting", "Change", "")]
		public JsonResult DeleteItemList(ParamBase param)
		{
			return base.Json(_item.DeleteItemList(param, null));
		}

		public JsonResult IsItemCodeExists(string id, BDItemModel model)
		{
			MActionResult<bool> data = _item.IsItemCodeExists(id, model, null);
			return base.Json(data);
		}

		[Permission("Setting", "Change", "")]
		public JsonResult ArchiveItem(ParamBase param, bool isRestore = false)
		{
			MActionResult<OperationResult> data = _item.ArchiveItem(param.KeyIDs, isRestore, null);
			return base.Json(data);
		}
	}
}
