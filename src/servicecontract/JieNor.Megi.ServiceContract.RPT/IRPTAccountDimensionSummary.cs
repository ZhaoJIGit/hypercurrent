using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.RPT.GL;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.RPT
{
	[ServiceContract]
	public interface IRPTAccountDimensionSummary : IRPTBizReport<RPTAccountDemensionSummaryFilterModel>
	{
		[OperationContract]
		MActionResult<OperationResult> InsertOrUpdateFilterScheme(RPTFilterSchemeModel scheme, string accessToken = null);

		[OperationContract]
		MActionResult<List<RPTFilterSchemeModel>> GetFilterSchemeList(RPTFilterSchemeFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<RPTFilterSchemeModel> GetFilterScheme(string id, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> DeleteFilterScheme(string id, string accessToken = null);
	}
}
