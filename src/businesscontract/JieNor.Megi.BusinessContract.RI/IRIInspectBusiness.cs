using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.RI;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.RI
{
	public interface IRIInspectBusiness : IDataContract<RICategoryModel>
	{
		RIInspectionResult Inspect(MContext ctx, string settingId, int year, int period);

		List<RICategoryModel> GetCategoryList(MContext ctx, bool includeSettingDisable = true, int year = 0, int period = 0);

		OperationResult ClearDataPool(MContext ctx);
	}
}
