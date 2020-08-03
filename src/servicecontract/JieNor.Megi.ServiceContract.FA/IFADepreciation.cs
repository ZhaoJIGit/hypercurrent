using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.FA;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.FA
{
	[ServiceContract]
	public interface IFADepreciation
	{
		[OperationContract]
		MActionResult<DataGridJson<FADepreciationModel>> GetSummaryDepreciationPageList(FAFixAssetsFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> DepreciatePeriod(FAFixAssetsFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<List<FADepreciationModel>> GetSummaryDepreciationList(FAFixAssetsFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<List<FADepreciationModel>> GetDetailDepreciationList(FAFixAssetsFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> SaveDepreciationList(FAFixAssetsFilterModel filter, string accessToken = null);
	}
}
