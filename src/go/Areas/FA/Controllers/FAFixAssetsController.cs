using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.ImportAndExport.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.FA;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.Identity.Attribute;
using JieNor.Megi.Identity.HtmlHelper;
using JieNor.Megi.ServiceContract.FA;
using JieNor.Megi.ServiceContract.GL;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.FA.Controllers
{
	public class FAFixAssetsController : GoControllerBase
	{
		private IFAFixAssets fixAssets = null;

		private IFADepreciation depreciation = null;

		private IFAFixAssetsChange fixAssetsChange = null;

		private IFAFixAssetsType fixAssetsType = null;

		private IGLVoucher voucher = null;

		public FAFixAssetsController(IFAFixAssets _fixAssets, IFADepreciation _depreciation, IFAFixAssetsChange _fixAssetsChange, IFAFixAssetsType _fixAssetsType, IGLVoucher _voucher)
		{
			fixAssets = _fixAssets;
			depreciation = _depreciation;
			fixAssetsChange = _fixAssetsChange;
			fixAssetsType = _fixAssetsType;
			voucher = _voucher;
		}

		[Permission("Fixed_Assets", "View", "")]
		public ActionResult FixAssetsHome()
		{
			base.ViewData["DashboardInfo"] = voucher.GetDashboardInfo(null).ResultData;
			return base.View();
		}

		[Permission("Fixed_Assets", "View", "")]
		public ActionResult FixAssetsTypeHome()
		{
			return base.View();
		}

		public ActionResult GetMergeCheckGroupValueModel(List<string> checkGroupValueIDs)
		{
			MActionResult<GLCheckGroupValueModel> mergeCheckGroupValueModel = fixAssets.GetMergeCheckGroupValueModel(checkGroupValueIDs, null);
			return base.Json(mergeCheckGroupValueModel);
		}

		[Permission("Fixed_Assets", "View", "")]
		public ActionResult GetFixAssetsTypeList(string itemID = null)
		{
			MActionResult<List<FAFixAssetsTypeModel>> fixAssetsTypeList = fixAssetsType.GetFixAssetsTypeList(itemID, null);
			return base.Json(fixAssetsTypeList);
		}

		[Permission("Fixed_Assets", "View", "")]
		public ActionResult GetFixAssetsTabInfo()
		{
			MActionResult<List<NameValueModel>> fixAssetsTabInfo = fixAssets.GetFixAssetsTabInfo(null);
			return base.Json(fixAssetsTabInfo);
		}

		[Permission("Fixed_Assets", "View", "")]
		public ActionResult GetFixAssetsModel(string itemID = null, bool isCopy = false)
		{
			MActionResult<FAFixAssetsModel> fixAssetsModel = fixAssets.GetFixAssetsModel(itemID, isCopy, null);
			return base.Json(fixAssetsModel);
		}

		[Permission("Fixed_Assets", "Change", "")]
		public ActionResult HandleFixAssets(List<string> itemIDs, int type)
		{
			MActionResult<OperationResult> data = fixAssets.HandleFixAssets(itemIDs, type, null);
			return base.Json(data);
		}

		[Permission("Fixed_Assets", "View", "")]
		public ActionResult GetFixAssetsTypeModel(string itemID = null)
		{
			MActionResult<FAFixAssetsTypeModel> fixAssetsTypeModel = fixAssetsType.GetFixAssetsTypeModel(itemID, null);
			return base.Json(fixAssetsTypeModel);
		}

		[Permission("Fixed_Assets", "Change", "")]
		public ActionResult EditFixAssetsType(string itemID)
		{
			base.ViewData["MItemID"] = itemID;
			return base.View();
		}

		[Permission("Fixed_Assets", "Change", "")]
		public ActionResult SaveFixAssetsType(FAFixAssetsTypeModel model)
		{
			return base.Json(fixAssetsType.SaveFixAssetsType(model, null));
		}

		[Permission("Fixed_Assets", "Change", "")]
		public ActionResult SetExpenseAccountDefault(bool check, string accountCode)
		{
			return base.Json(fixAssets.SetExpenseAccountDefault(check, accountCode, null));
		}

		[Permission("Fixed_Assets", "Change", "")]
		public ActionResult DeleteFixAssetsType(List<string> itemIDs)
		{
			MActionResult<OperationResult> data = fixAssetsType.DeleteFixAssetsType(itemIDs, null);
			return base.Json(data);
		}

		[Permission("Fixed_Assets", "View", "")]
		public ActionResult GetFixAssetsPageList(FAFixAssetsFilterModel filter)
		{
			return base.Json(fixAssets.GetFixAssetsPageList(filter, null));
		}

		[Permission("Fixed_Assets", "Change", "")]
		public ActionResult EditFixAssets(string itemID, bool isCopy = false)
		{
			base.ViewData["MItemID"] = itemID;
			base.ViewData["MIsCopy"] = (isCopy ? '1' : '0');
			base.ViewData["DashboardInfo"] = voucher.GetDashboardInfo(null).ResultData;
			return base.View();
		}

		[Permission("Fixed_Assets", "Change", "")]
		public ActionResult SaveFixAssets(FAFixAssetsModel model)
		{
			return base.Json(fixAssets.SaveFixAssets(model, null));
		}

		[Permission("Fixed_Assets", "Change", "")]
		public ActionResult DeleteFixAssets(List<string> itemIDs)
		{
			MActionResult<OperationResult> data = fixAssets.DeleteFixAssets(itemIDs, null);
			return base.Json(data);
		}

		[Permission("Fixed_Assets", "Change", "")]
		public ActionResult ChangeFixAssets(FAFixAssetsChangeModel model)
		{
			return base.Json(fixAssets.SaveFixAssets(model, null));
		}

		[Permission("Fixed_Assets", "View", "")]
		public ActionResult GetSummaryDepreciationPageList(FAFixAssetsFilterModel filter)
		{
			MActionResult<DataGridJson<FADepreciationModel>> summaryDepreciationPageList = depreciation.GetSummaryDepreciationPageList(filter, null);
			return base.Json(summaryDepreciationPageList);
		}

		[Permission("Fixed_Assets", "View", "")]
		public ActionResult GetDetailDepreciationList(FAFixAssetsFilterModel filter)
		{
			MActionResult<List<FADepreciationModel>> detailDepreciationList = depreciation.GetDetailDepreciationList(filter, null);
			return base.Json(detailDepreciationList);
		}

		[Permission("Fixed_Assets", "Approve", "")]
		public ActionResult DepreciatePeriod(FAFixAssetsFilterModel filter)
		{
			MActionResult<OperationResult> data = depreciation.DepreciatePeriod(filter, null);
			return base.Json(data);
		}

		[Permission("Fixed_Assets", "Change", "")]
		public ActionResult BatchSetupDepreciation(int year, int period, List<string> itemIDs = null)
		{
			base.ViewData["MItemIDs"] = string.Join(",", itemIDs ?? new List<string>());
			base.ViewData["MYear"] = year;
			base.ViewData["MPeriod"] = period;
			return base.View();
		}

		[Permission("Fixed_Assets", "View", "")]
		public ActionResult DepreciatePeriodEdit(int year, int period)
		{
			base.ViewData["MYear"] = year;
			base.ViewData["MPeriod"] = period;
			return base.View("BatchSetupDepreciation");
		}

		[Permission("Fixed_Assets", "Change", "")]
		public ActionResult SaveDepreciationList(FAFixAssetsFilterModel filter)
		{
			MActionResult<OperationResult> data = depreciation.SaveDepreciationList(filter, null);
			return base.Json(data);
		}

		[Permission("Fixed_Assets", "Export", "")]
		public FileResult Export(string jsonParam)
		{
			Stream stream = ExportToImportHelper.CreateExportObjStream(BizReportType.AssetCardList, jsonParam);
			string exportName = string.Format("{0} - {1}.xls", ContextHelper.MContext.MOrgName, HtmlLang.GetText(LangModule.FA, "FixedAssetCards", "固定资产卡片"));
			return base.ExportReport(stream, exportName);
		}
	}
}
