using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.BusinessService.PT;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.PT;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.DataRepository.RPT;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace JieNor.Megi.BusinessService.RPT
{
	public class RPTReportBusiness : PTBaseBusiness, IRPTReportBusiness
	{
		public OperationResult UpdateReport(MContext ctx, RPTReportModel model)
		{
			OperationResult operationResult = new OperationResult();
			RPTReportRepository.UpdateReportInfo(ctx, model);
			operationResult.Success = true;
			operationResult.ObjectID = model.MID;
			return operationResult;
		}

		public string AddEmptyReport(MContext ctx, string mainReportId)
		{
			RPTReportModel rPTReportModel = new RPTReportModel();
			rPTReportModel.MParentID = mainReportId;
			rPTReportModel.MType = 0;
			rPTReportModel.MOrgID = ctx.MOrgID;
			rPTReportModel.MStatus = Convert.ToInt32(RPTReportStatus.Draft);
			rPTReportModel.MTitle = string.Empty;
			rPTReportModel.MSubtitle = string.Empty;
			rPTReportModel.MAuthor = string.Empty;
			rPTReportModel.MSheetName = string.Empty;
			rPTReportModel.MIsActive = false;
			rPTReportModel.MReportDate = ctx.DateNow;
			RPTReportRepository.AddReport(ctx, rPTReportModel);
			return rPTReportModel.MID;
		}

		public OperationResult UpdateReportByBizReport(MContext ctx, string reportId, string content)
		{
			OperationResult operationResult = new OperationResult();
			RPTReportModel rPTReportModel = new RPTReportModel();
			rPTReportModel.MOrgID = ctx.MOrgID;
			rPTReportModel.MID = reportId;
			rPTReportModel.MContent = content;
			RPTReportRepository.UpdateReportContent(ctx, rPTReportModel, true);
			operationResult.Success = true;
			operationResult.ObjectID = rPTReportModel.MID;
			return operationResult;
		}

		public List<RPTReportSheetModel> GetReportSheetList(MContext ctx, string reportId)
		{
			return RPTReportRepository.GetReportSheetList(reportId, ctx);
		}

		public bool UpdateReportContent(MContext ctx, RPTReportModel model)
		{
			return RPTReportRepository.UpdateReportContent(ctx, model, true);
		}

		public string GetBizReportJson(MContext ctx, ReportFilterBase filter, Func<BizReportModel> getReport)
		{
			if (string.IsNullOrEmpty(filter.MReportID))
			{
				return null;
			}
			RPTReportModel reportModel = RPTReportRepository.GetReportModel(filter.MReportID, ctx);
			if (reportModel == null)
			{
				return null;
			}
			JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
			javaScriptSerializer.MaxJsonLength = 2147483647;
			BizReportModel bizReportModel = null;
			if (string.IsNullOrEmpty(reportModel.MContent))
			{
				bizReportModel = getReport();
				bizReportModel.Filter = filter;
				bizReportModel.IsActive = reportModel.MIsActive;
				bizReportModel.LCID = ctx.MLCID;
				reportModel.MID = filter.MReportID;
				reportModel.MType = bizReportModel.Type;
				reportModel.MStatus = Convert.ToInt32(RPTReportStatus.Draft);
				reportModel.MTitle = bizReportModel.Title1;
				reportModel.MSubtitle = bizReportModel.Title2;
				reportModel.MAuthor = "";
				reportModel.MSheetName = bizReportModel.Title1;
				List<BizReportRowModel> rows = bizReportModel.Rows;
				if (!CheckIsSaveReportDataInView(ctx, bizReportModel.Type))
				{
					bizReportModel.Rows = new List<BizReportRowModel>();
				}
				reportModel.MContent = javaScriptSerializer.Serialize(bizReportModel);
				RPTReportRepository.UpdateInActiveReport(ctx, reportModel);
				bizReportModel.Rows = rows;
				reportModel.MContent = javaScriptSerializer.Serialize(bizReportModel);
			}
			else if (filter.IsReload)
			{
				bizReportModel = getReport();
				bizReportModel.Filter = filter;
				bizReportModel.LCID = ctx.MLCID;
				reportModel.MTitle = bizReportModel.Title1;
				reportModel.MSubtitle = bizReportModel.Title2;
				List<BizReportRowModel> rows2 = bizReportModel.Rows;
				if (!CheckIsSaveReportDataInView(ctx, bizReportModel.Type))
				{
					bizReportModel.Rows = new List<BizReportRowModel>();
				}
				reportModel.MContent = javaScriptSerializer.Serialize(bizReportModel);
				RPTReportRepository.UpdateReportContent(ctx, reportModel, false);
				bizReportModel.Rows = rows2;
				reportModel.MContent = javaScriptSerializer.Serialize(bizReportModel);
			}
			else
			{
				bizReportModel = javaScriptSerializer.Deserialize<BizReportModel>(reportModel.MContent);
				if (bizReportModel.LCID != ctx.MLCID || bizReportModel.Rows == null || bizReportModel.Rows.Count() == 0)
				{
					bizReportModel = getReport();
					bizReportModel.Filter = ((bizReportModel.Filter == null) ? filter : bizReportModel.Filter);
					bizReportModel.LCID = ctx.MLCID;
					List<BizReportRowModel> rows3 = bizReportModel.Rows;
					if (!CheckIsSaveReportDataInView(ctx, bizReportModel.Type))
					{
						bizReportModel.Rows = new List<BizReportRowModel>();
					}
					reportModel.MContent = javaScriptSerializer.Serialize(bizReportModel);
					RPTReportRepository.UpdateReportContent(ctx, reportModel, false);
					bizReportModel.Rows = rows3;
					reportModel.MContent = javaScriptSerializer.Serialize(bizReportModel);
				}
			}
			if (reportModel.MStatus == Convert.ToInt32(RPTReportStatus.Published))
			{
				reportModel.MContent = GetPublishReportContent(reportModel.MContent);
			}
			return HtmlHelper.Decode(reportModel.MContent);
		}

		private string GetPublishReportContent(string content)
		{
			if (string.IsNullOrEmpty(content))
			{
				return content;
			}
			JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
			javaScriptSerializer.MaxJsonLength = 2147483647;
			BizReportModel bizReportModel = javaScriptSerializer.Deserialize<BizReportModel>(content);
			bizReportModel.ReadOnly = true;
			content = javaScriptSerializer.Serialize(bizReportModel);
			return content;
		}

		private void ResetNote(string dataSource, ref BizReportModel tgtModel)
		{
			if (tgtModel.Rows.Count != 0)
			{
				dataSource = HtmlHelper.Decode(dataSource);
				JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
				javaScriptSerializer.MaxJsonLength = 2147483647;
				BizReportModel bizReportModel = javaScriptSerializer.Deserialize<BizReportModel>(dataSource);
				if (bizReportModel.Rows.Count != 0)
				{
					foreach (BizReportRowModel row in tgtModel.Rows)
					{
						ResetRowNote(row, bizReportModel);
					}
				}
			}
		}

		private void ResetRowNote(BizReportRowModel tgtRow, BizReportModel srcModel)
		{
			if (tgtRow.RowType == BizReportRowType.Item && !string.IsNullOrEmpty(tgtRow.UniqueValue))
			{
				foreach (BizReportRowModel row in srcModel.Rows)
				{
					ResetCellNote(tgtRow, row);
				}
			}
		}

		private void ResetCellNote(BizReportRowModel tgtRow, BizReportRowModel srcRow)
		{
			if (!string.IsNullOrEmpty(srcRow.UniqueValue) && tgtRow.UniqueValue.Equals(srcRow.UniqueValue) && tgtRow.Cells.Count == srcRow.Cells.Count)
			{
				int num = 0;
				foreach (BizReportCellModel cell in tgtRow.Cells)
				{
					cell.Notes = srcRow.Cells[num].Notes;
					num++;
				}
			}
		}

		public BizReportModel GetBizReportModel(MContext ctx, string reportId)
		{
			BizReportModel bizReportModel = null;
			RPTReportModel reportModel = RPTReportRepository.GetReportModel(reportId, ctx);
			if (reportModel != null)
			{
				JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
				javaScriptSerializer.MaxJsonLength = 2147483647;
				bizReportModel = javaScriptSerializer.Deserialize<BizReportModel>(reportModel.MContent);
				bizReportModel.ReportID = reportModel.MID;
				if (reportModel.MStatus == Convert.ToInt32(RPTReportStatus.Published))
				{
					bizReportModel.ReadOnly = true;
				}
			}
			return bizReportModel;
		}

		public RPTReportModel GetReportModel(MContext ctx, string reportId)
		{
			return RPTReportRepository.GetReportModel(reportId, ctx);
		}

		public List<RPTReportModel> GetReportList(MContext ctx, RPTReportQueryParam param)
		{
			return RPTReportRepository.GetReportList(ctx, param);
		}

		public List<RPTReportModel> GetDraftReportList(MContext ctx, RPTReportQueryParam param)
		{
			return RPTReportRepository.GetDraftReportList(ctx, param);
		}

		public RPTReportModel GetMainReportModel(MContext ctx, string reportId)
		{
			return RPTReportRepository.GetMainReportModel(reportId, ctx);
		}

		public RPTReportModel GetCurrentReportModel(MContext ctx, string reportId)
		{
			return RPTReportRepository.GetCurrentReportModel(reportId, ctx);
		}

		public RPTReportLayoutModel GetReportLayoutModel(MContext ctx, BizReportType reportType, string printSettingID)
		{
			List<RPTReportLayoutModel> list = null;
			string key = $"{ctx.MOrgID}_PrtLayout";
			if (((ConcurrentDictionary<string, object>)PTBaseBusiness.cacheList).ContainsKey(key))
			{
				list = (((ConcurrentDictionary<string, object>)PTBaseBusiness.cacheList)[key] as List<RPTReportLayoutModel>);
			}
			else
			{
				list = RPTReportRepository.GetReportLayoutList(ctx, null);
				((ConcurrentDictionary<string, object>)PTBaseBusiness.cacheList).TryAdd(key, (object)list);
			}
			IEnumerable<RPTReportLayoutModel> source = from f in list
			where f.MReportType == reportType.ToString()
			select f;
			if (!string.IsNullOrWhiteSpace(printSettingID))
			{
				return source.FirstOrDefault((RPTReportLayoutModel f) => f.MPrintSettingID == printSettingID);
			}
			return source.FirstOrDefault((RPTReportLayoutModel f) => string.IsNullOrWhiteSpace(f.MPrintSettingID));
		}

		public OperationResult UpdateReportLayout(MContext ctx, RPTReportLayoutModel model)
		{
			OperationResult result = RPTReportRepository.UpdateReportLayout(ctx, model);
			UpdateReportLayoutCache(ctx);
			return result;
		}

		public OperationResult RestoreReport(MContext ctx, string mid)
		{
			RPTReportLayoutModel dataEditModel = ModelInfoManager.GetDataEditModel<RPTReportLayoutModel>(ctx, mid, false, true);
			OperationResult result = RPTReportRepository.RestoreReport(mid, ctx);
			if (dataEditModel != null && dataEditModel.MBizObject == "General_Ledger")
			{
				PTVoucherModel dataEditModel2 = ModelInfoManager.GetDataEditModel<PTVoucherModel>(ctx, dataEditModel.MPrintSettingID, false, true);
				if (dataEditModel2 != null && dataEditModel2.MEntryCount == 0)
				{
					dataEditModel2.MEntryCount = 5;
					ModelInfoManager.InsertOrUpdate<PTVoucherModel>(ctx, dataEditModel2, new List<string>
					{
						"MEntryCount"
					});
					base.GetKeyValueList(ctx, "Voucher", true);
				}
			}
			UpdateReportLayoutCache(ctx);
			return result;
		}

		public OperationResult DeleteReport(MContext ctx, List<string> mids)
		{
			return RPTReportRepository.DeleteReport(ctx, mids);
		}

		public void UpdateReportLayoutCache(MContext ctx)
		{
			string key = $"{ctx.MOrgID}_PrtLayout";
			if (((ConcurrentDictionary<string, object>)PTBaseBusiness.cacheList).ContainsKey(key))
			{
				((ConcurrentDictionary<string, object>)PTBaseBusiness.cacheList)[key] = RPTReportRepository.GetReportLayoutList(ctx, null);
			}
			else
			{
				((ConcurrentDictionary<string, object>)PTBaseBusiness.cacheList).TryAdd(key, (object)RPTReportRepository.GetReportLayoutList(ctx, null));
			}
		}

		private bool CheckIsSaveReportDataInView(MContext ctx, int reportType)
		{
			bool result = false;
			switch (reportType)
			{
			case 6:
			case 9:
			case 13:
			case 19:
			case 21:
			case 38:
				result = true;
				break;
			}
			return result;
		}
	}
}
