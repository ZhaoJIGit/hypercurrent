using JieNor.Megi.Core;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.EntityModel.Context;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.RPT
{
	[ServiceContract]
	public interface IRPTGeneralLedger : IRPTBizReport<RPTGeneralLedgerFilterModel>
	{
		[OperationContract]
		MActionResult<BizReportModel> GetBalanceReportModel(RPTGeneralLedgerFilterModel filter, string accessToken);
	}
}
