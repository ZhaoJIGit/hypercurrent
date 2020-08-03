using JieNor.Megi.Core;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.BusinessContract.BAS
{
	public interface IBASSearchBusiness
	{
		DataGridJson<BASSearchModel> GetSearchResult(MContext ctx, BASSearchFilterModel filter);
	}
}
