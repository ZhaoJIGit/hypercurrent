using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.FA;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.FA
{
	public interface IFAFixAssetsChangeBusiness : IDataContract<FAFixAssetsChangeModel>
	{
		List<FAFixAssetsChangeModel> GetFixAssetsChangeLog(MContext ctx, FAFixAssetsChangeFilterModel filter = null);
	}
}
