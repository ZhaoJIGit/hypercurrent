using JieNor.Megi.Core;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.RPT
{
	[ServiceContract]
	public interface IRPTSubsidiaryLedger : IRPTBizReport<RPTSubsidiaryLedgerFilterModel>
	{
		[OperationContract]
		MActionResult<BizReportModel> GetBizReportModel(RPTSubsidiaryLedgerFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<string> GetBizReportModelJson(RPTSubsidiaryLedgerFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<List<BizReportModel>> GetBatchReportList(RPTSubsidiaryLedgerFilterModel filter, string accessToken = null);
	}
}
