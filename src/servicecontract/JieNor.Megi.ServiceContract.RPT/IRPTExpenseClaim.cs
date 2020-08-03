using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.EntityModel.Context;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.RPT
{
	[ServiceContract]
	public interface IRPTExpenseClaim : IRPTBizReport<RPTExpenseClaimFilterModel>
	{
		[OperationContract]
		MActionResult<string> GetBizSubReportJson(RPTExpenseClaimDeatailFilterModel filter, string accessToken = null);
	}
}
