using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.EntityModel.BD;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.PA
{
	public interface IPAPayItemBussiness : IDataContract<PAPayItemModel>
	{
		List<PAPayItemModel> GetSalaryItemList(MContext ctx);

		OperationResult UpdateModel(MContext ctx, PAPayItemModel model);

		PAPayItemModel GetSalaryItemById(MContext ctx, string id);

		List<SalaryItemTreeModel> GetSalaryItemTreeList(MContext ctx, bool removeInActive);

		OperationResult ForbiddenSalaryItem(MContext ctx, string ids);

		BDIsCanDeleteModel IsCanDeleteOrInactive(MContext ctx, ParamBase param);

		List<PAPayItemModel> GetDisableItemList(MContext ctx);
	}
}
