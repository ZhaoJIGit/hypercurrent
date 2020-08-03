using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.ImportAndExport.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.EntityModel.BD.ExpenseItem;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.Identity;
using JieNor.Megi.Identity.Attribute;
using JieNor.Megi.Identity.HtmlHelper;
using JieNor.Megi.ServiceContract.BD;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.BD.Controllers
{
	public class ExpenseItemController : GoControllerBase
	{
		private IBDExpenseItem _expItem = null;

		public ExpenseItemController(IBDExpenseItem expItem)
		{
			_expItem = expItem;
		}

		[Permission("Setting", "View", "")]
		public ActionResult ExpenseItemList()
		{
			base.SetTitleAndCrumb(LangHelper.GetText(LangModule.BD, "ExpenseItem", "Expense Items"), "<a href='/Setting/'>" + LangHelper.GetText(LangModule.BD, "GeneralSettings", "General Settings") + " > </a>");
			return base.View();
		}

		[Permission("Setting", "View", "")]
		public ActionResult ExpenseItemEdit(string id)
		{
			ViewBag.ItemID = id;
			ViewBag.IsEnableGL = ContextHelper.MContext.MRegProgress >= 12 && true;

			return base.View();
		}

		public JsonResult GetEditInfo(BDExpenseItemModel model)
		{
			return base.Json(_expItem.GetEditInfo(model, null));
		}

		public JsonResult UpdateExpenseItem(BDExpenseItemModel expenseitem)
		{
			MActionResult<OperationResult> data = _expItem.Update(expenseitem, null);
			return base.Json(data);
		}

		public JsonResult GetExpenseItemList(bool isActive = true)
		{
			return base.Json(_expItem.GetList(isActive, null));
		}

		public JsonResult GetExpenseItemListByTier(bool includeDisable = false)
		{
			return base.Json(_expItem.GetListByTier(includeDisable, false, null));
		}

		public JsonResult GetParentExpenseItemList(string itemId = null)
		{
			return base.Json(_expItem.GetParentItemList(itemId, null));
		}

		public JsonResult GetChildrenExpenseItemList()
		{
			return base.Json(_expItem.GetNoParentItemList(null));
		}

		public JsonResult GetPageExpenseItemList(BDExpenseItemListFilterModel param)
		{
			DataGridJson<BDExpenseItemModel> resultData = _expItem.GetPageList(param, null).ResultData;
			DataGridJson<ExpenseItemViewModel> dataGridJson = new DataGridJson<ExpenseItemViewModel>();
			dataGridJson.total = resultData.total;
			dataGridJson.rows = new List<ExpenseItemViewModel>();
			if (resultData.rows != null && resultData.rows.Count > 0)
			{
				IEnumerable<BDExpenseItemModel> enumerable = from x in resultData.rows
				where x.MParentItemID == "0"
				select x;
				if (enumerable == null)
				{
					return base.Json(resultData);
				}
				List<BDExpenseItemModel> list = enumerable.ToList();
				foreach (BDExpenseItemModel item in list)
				{
					IEnumerable<BDExpenseItemModel> enumerable2 = from x in resultData.rows
					where x.MParentItemID == item.MItemID
					select x;
					ExpenseItemViewModel expenseItemViewModel = new ExpenseItemViewModel();
					if (enumerable2 == null || enumerable2.Count() == 0)
					{
						expenseItemViewModel = ModelSwitch(item, null);
						dataGridJson.rows.Add(expenseItemViewModel);
					}
					else
					{
						List<BDExpenseItemModel> list2 = enumerable2.ToList();
						foreach (BDExpenseItemModel item2 in list2)
						{
							ExpenseItemViewModel expenseItemViewModel2 = new ExpenseItemViewModel();
							expenseItemViewModel2 = ModelSwitch(item2, item);
							dataGridJson.rows.Add(expenseItemViewModel2);
						}
					}
				}
			}
			return base.Json(dataGridJson);
		}

		private ExpenseItemViewModel ModelSwitch(BDExpenseItemModel item, BDExpenseItemModel parentModel = null)
		{
			ExpenseItemViewModel expenseItemViewModel = new ExpenseItemViewModel();
			expenseItemViewModel.MItemID = item.MItemID;
			expenseItemViewModel.MParentItemID = item.MParentItemID;
			expenseItemViewModel.MDesc = item.MDesc;
			if (parentModel == null)
			{
				expenseItemViewModel.MParentName = item.MName;
			}
			else
			{
				expenseItemViewModel.MParentName = parentModel.MName;
				expenseItemViewModel.MName = item.MName;
			}
			expenseItemViewModel.MModifyDate = item.MModifyDate;
			return expenseItemViewModel;
		}

		[Permission("Setting", "Change", "")]
		public JsonResult IsCanDeleteOrInactive(ParamBase param)
		{
			return base.Json(_expItem.IsCanDeleteOrInactive(param, null));
		}

		[Permission("Setting", "Change", "")]
		public JsonResult DeleteExpItem(ParamBase param)
		{
			param.OrgID = base.MContext.MOrgID;
			MActionResult<OperationResult> data = _expItem.DeleteExpItem(param, null);
			return base.Json(data);
		}

		public JsonResult IsParentExpenseItem(string itemId)
		{
			OperationResult operationResult = new OperationResult();
			operationResult.Success = false;
			List<BDExpenseItemModel> resultData = _expItem.GetNoParentItemList(null).ResultData;
			if (resultData != null)
			{
				BDExpenseItemModel bDExpenseItemModel = (from x in resultData
				where x.MParentItemID == itemId
				select x).FirstOrDefault();
				if (bDExpenseItemModel != null)
				{
					operationResult.Success = true;
				}
			}
			return base.Json(operationResult);
		}

		public JsonResult AchiveExpenseItem(string itemIds, bool isRestore = false)
		{
			MActionResult<OperationResult> data = _expItem.ArchiveItem(itemIds, isRestore, null);
			return base.Json(data);
		}

		public FileResult Export()
		{
			Stream stream = ExportToImportHelper.CreateExportObjStream(BizReportType.ExpenseItem, null);
			string exportName = string.Format("{0} - {1}.xls", ContextHelper.MContext.MOrgName, HtmlLang.GetText(LangModule.BD, "ExpenseItem", "费用项目"));
			return base.ExportReport(stream, exportName);
		}
	}
}
