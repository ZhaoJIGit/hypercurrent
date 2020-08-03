using JieNor.Megi.DataModel.IV;
using JieNor.Megi.EntityModel.Context;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.RPT
{
	[ServiceContract]
	public interface IRPTBankBillRecSummary : IRPTBizReport<RPTBankBillRecSummaryFilterModel>
	{
		[OperationContract]
		MActionResult<string> AddReport(string accessToken = null);
	}
}
