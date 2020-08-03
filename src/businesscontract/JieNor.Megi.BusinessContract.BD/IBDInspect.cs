using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BD.RI;
using JieNor.Megi.DataModel.RI;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.BD
{
	public interface IBDInspect
	{
		List<BDInspectItemTreeModel> GetInspectItemTreeList(MContext ctx, BDInspectItemFilterModel filter);

		List<BDInspectItemModel> GetInspectItemList(MContext ctx, BDInspectItemFilterModel filter);

		OperationResult InitInspectItem(MContext ctx);

		OperationResult SaveInspectSetting(MContext ctx, List<RICategorySettingModel> inspectSetingItemList);
	}
}
