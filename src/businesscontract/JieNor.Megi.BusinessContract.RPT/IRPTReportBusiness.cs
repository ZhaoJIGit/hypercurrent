using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.RPT
{
	public interface IRPTReportBusiness
	{
		string AddEmptyReport(MContext context, string mainReportId);

		OperationResult UpdateReport(MContext ctx, RPTReportModel model);

		OperationResult UpdateReportByBizReport(MContext context, string reportId, string content);

		bool UpdateReportContent(MContext ctx, RPTReportModel model);

		string GetBizReportJson(MContext context, ReportFilterBase filter, Func<BizReportModel> getReport);

		BizReportModel GetBizReportModel(MContext context, string reportId);

		RPTReportModel GetReportModel(MContext context, string reportId);

		List<RPTReportModel> GetReportList(MContext ctx, RPTReportQueryParam param);

		OperationResult DeleteReport(MContext ctx, List<string> mids);

		List<RPTReportModel> GetDraftReportList(MContext ctx, RPTReportQueryParam param);

		List<RPTReportSheetModel> GetReportSheetList(MContext context, string reportId);

		RPTReportModel GetMainReportModel(MContext context, string reportId);

		RPTReportModel GetCurrentReportModel(MContext context, string reportId);

		RPTReportLayoutModel GetReportLayoutModel(MContext context, BizReportType reportType, string printSettingID);

		OperationResult UpdateReportLayout(MContext ctx, RPTReportLayoutModel model);

		OperationResult RestoreReport(MContext context, string mid);
	}
}
