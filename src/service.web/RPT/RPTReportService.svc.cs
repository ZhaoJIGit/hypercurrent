using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.BusinessService.RPT;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.RPT;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.RPT
{
	public class RPTReportService : ServiceT<BaseModel>, IRPTReport
	{
		private readonly IRPTReportBusiness biz = new RPTReportBusiness();

		public MActionResult<OperationResult> UpdateReport(RPTReportModel model, string accessToken = null)
		{
			IRPTReportBusiness iRPTReportBusiness = biz;
			return base.RunFunc(iRPTReportBusiness.UpdateReport, model, accessToken);
		}

		public MActionResult<string> AddEmptyReport(string mainReportId, string accessToken = null)
		{
			IRPTReportBusiness iRPTReportBusiness = biz;
			return base.RunFunc(iRPTReportBusiness.AddEmptyReport, mainReportId, accessToken);
		}

		public MActionResult<OperationResult> UpdateReportByBizReport(string reportId, string content, string accessToken = null)
		{
			IRPTReportBusiness iRPTReportBusiness = biz;
			return base.RunFunc(iRPTReportBusiness.UpdateReportByBizReport, reportId, content, accessToken);
		}

		public MActionResult<List<RPTReportSheetModel>> GetReportSheetList(string reportId, string accessToken = null)
		{
			IRPTReportBusiness iRPTReportBusiness = biz;
			return base.RunFunc(iRPTReportBusiness.GetReportSheetList, reportId, accessToken);
		}

		public MActionResult<bool> UpdateReportContent(RPTReportModel model, string accessToken = null)
		{
			IRPTReportBusiness iRPTReportBusiness = biz;
			return base.RunFunc(iRPTReportBusiness.UpdateReportContent, model, accessToken);
		}

		public MActionResult<string> GetBizReportJson(ReportFilterBase filter, Func<BizReportModel> getReport, string accessToken = null)
		{
			IRPTReportBusiness iRPTReportBusiness = biz;
			return base.RunFunc(iRPTReportBusiness.GetBizReportJson, filter, getReport, accessToken);
		}

		public MActionResult<BizReportModel> GetBizReportModel(string reportId, string accessToken = null)
		{
			IRPTReportBusiness iRPTReportBusiness = biz;
			return base.RunFunc(iRPTReportBusiness.GetBizReportModel, reportId, accessToken);
		}

		public MActionResult<RPTReportModel> GetReportModel(string reportId, string accessToken = null)
		{
			IRPTReportBusiness iRPTReportBusiness = biz;
			return base.RunFunc(iRPTReportBusiness.GetReportModel, reportId, accessToken);
		}

		public MActionResult<List<RPTReportModel>> GetReportList(RPTReportQueryParam param, string accessToken = null)
		{
			IRPTReportBusiness iRPTReportBusiness = biz;
			return base.RunFunc(iRPTReportBusiness.GetReportList, param, accessToken);
		}

		public MActionResult<List<RPTReportModel>> GetDraftReportList(RPTReportQueryParam param, string accessToken = null)
		{
			IRPTReportBusiness iRPTReportBusiness = biz;
			return base.RunFunc(iRPTReportBusiness.GetDraftReportList, param, accessToken);
		}

		public MActionResult<RPTReportModel> GetMainReportModel(string reportId, string accessToken = null)
		{
			IRPTReportBusiness iRPTReportBusiness = biz;
			return base.RunFunc(iRPTReportBusiness.GetMainReportModel, reportId, accessToken);
		}

		public MActionResult<RPTReportModel> GetCurrentReportModel(string reportId, string accessToken = null)
		{
			IRPTReportBusiness iRPTReportBusiness = biz;
			return base.RunFunc(iRPTReportBusiness.GetCurrentReportModel, reportId, accessToken);
		}

		public MActionResult<RPTReportLayoutModel> GetReportLayoutModel(BizReportType reportType, string printSettingID, string accessToken = null)
		{
			IRPTReportBusiness iRPTReportBusiness = biz;
			return base.RunFunc(iRPTReportBusiness.GetReportLayoutModel, reportType, printSettingID, accessToken);
		}

		public MActionResult<OperationResult> UpdateReportLayout(RPTReportLayoutModel model, string accessToken = null)
		{
			IRPTReportBusiness iRPTReportBusiness = biz;
			return base.RunFunc(iRPTReportBusiness.UpdateReportLayout, model, accessToken);
		}

		public MActionResult<OperationResult> RestoreReport(string mid, string accessToken = null)
		{
			IRPTReportBusiness iRPTReportBusiness = biz;
			return base.RunFunc(iRPTReportBusiness.RestoreReport, mid, accessToken);
		}

		public MActionResult<OperationResult> DeleteReport(List<string> mids, string accessToke = null)
		{
			IRPTReportBusiness iRPTReportBusiness = biz;
			return base.RunFunc(iRPTReportBusiness.DeleteReport, mids, accessToke);
		}
	}
}
