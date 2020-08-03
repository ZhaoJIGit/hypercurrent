using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.BusinessService.RPT;
using JieNor.Megi.Core;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.RPT;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.RPT
{
	public class RPTSubsidiaryLedgerService : ServiceT<BaseModel>, IRPTSubsidiaryLedger, IRPTBizReport<RPTSubsidiaryLedgerFilterModel>
	{
		private readonly IRPTSubsidiaryLedgerBussiness biz = new RPTSubsidiaryLedgerBusiness();

		public MActionResult<string> GetBizReportJson(RPTSubsidiaryLedgerFilterModel filter, string accessToken = null)
		{
			IRPTSubsidiaryLedgerBussiness iRPTSubsidiaryLedgerBussiness = biz;
			return base.RunFunc(iRPTSubsidiaryLedgerBussiness.GetBizReportJson, filter, accessToken);
		}

		public MActionResult<BizReportModel> GetBizReportModel(RPTSubsidiaryLedgerFilterModel filter, string accessToken = null)
		{
			IRPTSubsidiaryLedgerBussiness iRPTSubsidiaryLedgerBussiness = biz;
			return base.RunFunc(iRPTSubsidiaryLedgerBussiness.GetBizReportModel, filter, accessToken);
		}

		public MActionResult<string> GetBizReportModelJson(RPTSubsidiaryLedgerFilterModel filter, string accessToken = null)
		{
			IRPTSubsidiaryLedgerBussiness iRPTSubsidiaryLedgerBussiness = biz;
			return base.RunFunc(iRPTSubsidiaryLedgerBussiness.GetBizReportModelJson, filter, accessToken);
		}

		public MActionResult<List<BizReportModel>> GetBatchReportList(RPTSubsidiaryLedgerFilterModel filter, string accessToken = null)
		{
			IRPTSubsidiaryLedgerBussiness iRPTSubsidiaryLedgerBussiness = biz;
			return base.RunFunc(iRPTSubsidiaryLedgerBussiness.GetBatchReportList, filter, accessToken);
		}
	}
}
