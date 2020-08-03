using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.RPT.GL;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.RPT
{
	public interface IRPTFilterScheme
	{
		List<RPTFilterSchemeModel> GetFilterSchemeList(MContext ctx, RPTFilterSchemeFilterModel filter);

		OperationResult InsertOrUpateFilterScheme(MContext ctx, RPTFilterSchemeModel filterSchemeModel);

		RPTFilterSchemeModel GetFilterScheme(MContext ctx, string id);

		OperationResult DeleteFilterScheme(MContext ctx, string id);
	}
}
