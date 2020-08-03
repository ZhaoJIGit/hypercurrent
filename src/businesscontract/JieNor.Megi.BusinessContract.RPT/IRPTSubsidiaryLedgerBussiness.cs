using JieNor.Megi.Core;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.RPT
{
	public interface IRPTSubsidiaryLedgerBussiness : IRPTBizReportBusiness<RPTSubsidiaryLedgerFilterModel>
	{
		BizReportModel GetBizReportModel(MContext ctx, RPTSubsidiaryLedgerFilterModel filter);

		string GetBizReportModelJson(MContext ctx, RPTSubsidiaryLedgerFilterModel filter);

		List<BizReportModel> GetBatchReportList(MContext ctx, RPTSubsidiaryLedgerFilterModel filter);
	}
}
