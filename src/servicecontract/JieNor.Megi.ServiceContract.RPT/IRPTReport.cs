using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.RPT
{
	[ServiceContract]
	public interface IRPTReport
	{
		[OperationContract]
		MActionResult<string> AddEmptyReport(string mainReportId, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> UpdateReport(RPTReportModel model, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> UpdateReportByBizReport(string reportId, string content, string accessToken = null);

		[OperationContract]
		MActionResult<bool> UpdateReportContent(RPTReportModel model, string accessToken = null);

		[OperationContract]
		MActionResult<string> GetBizReportJson(ReportFilterBase filter, Func<BizReportModel> getReport, string accessToken = null);

		[OperationContract]
		MActionResult<BizReportModel> GetBizReportModel(string reportId, string accessToken = null);

		[OperationContract]
		MActionResult<RPTReportModel> GetReportModel(string reportId, string accessToken = null);

		[OperationContract]
		MActionResult<List<RPTReportModel>> GetReportList(RPTReportQueryParam param, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> DeleteReport(List<string> mids, string accessToken = null);

		[OperationContract]
		MActionResult<List<RPTReportModel>> GetDraftReportList(RPTReportQueryParam param, string accessToken = null);

		[OperationContract]
		MActionResult<List<RPTReportSheetModel>> GetReportSheetList(string reportId, string accessToken = null);

		[OperationContract]
		MActionResult<RPTReportModel> GetMainReportModel(string reportId, string accessToken = null);

		[OperationContract]
		MActionResult<RPTReportModel> GetCurrentReportModel(string reportId, string accessToken = null);

		[OperationContract]
		MActionResult<RPTReportLayoutModel> GetReportLayoutModel(BizReportType reportType, string printSettingID, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> UpdateReportLayout(RPTReportLayoutModel model, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> RestoreReport(string mid, string accessToken = null);
	}
}
