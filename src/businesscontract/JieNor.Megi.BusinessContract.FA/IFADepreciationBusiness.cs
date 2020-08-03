using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.FA;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.FA
{
	public interface IFADepreciationBusiness : IDataContract<FADepreciationModel>
	{
		DataGridJson<FADepreciationModel> GetSummaryDepreciationPageList(MContext ctx, FAFixAssetsFilterModel filter);

		OperationResult DepreciatePeriod(MContext ctx, FAFixAssetsFilterModel filter);

		List<FADepreciationModel> GetSummaryDepreciationList(MContext ctx, FAFixAssetsFilterModel filter);

		List<FADepreciationModel> GetDetailDepreciationList(MContext ctx, FAFixAssetsFilterModel filter);

		OperationResult SaveDepreciationList(MContext ctx, FAFixAssetsFilterModel filter);
	}
}
