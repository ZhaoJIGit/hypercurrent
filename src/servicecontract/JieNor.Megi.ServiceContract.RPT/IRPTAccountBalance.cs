using JieNor.Megi.Core;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.EntityModel.Context;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.RPT
{
	[ServiceContract]
	public interface IRPTAccountBalance : IRPTBizReport<RPTAccountBalanceFilterModel>
	{
		[OperationContract]
		[ServiceKnownType(typeof(BalanceBizReportRowModel))]
		MActionResult<BizReportModel> GetReportModel(RPTAccountBalanceFilterModel filter, string accessToken = null);
	}
}
