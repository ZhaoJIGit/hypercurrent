using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.BD
{
	public interface IBDExpenseItemBusiness
	{
		List<BDExpenseItemModel> GetList(MContext ctx, bool isActive = true);

		List<BDExpenseItemModel> GetListByTier(MContext ctx, bool includeDisable = false, bool isAddChildren = false);

		List<BDExpenseItemModel> GetNoParentList(MContext ctx);

		List<BDExpenseItemModel> GetParentList(MContext ctx, string itemId = null);

		DataGridJson<BDExpenseItemModel> GetPageList(MContext ctx, BDExpenseItemListFilterModel filter);

		BDExpenseItemModel GetEditInfo(MContext ctx, BDExpenseItemModel Expenseinfo);

		OperationResult Update(MContext ctx, BDExpenseItemModel Expenseitem);

		BDIsCanDeleteModel IsCanDeleteOrInactive(MContext ctx, ParamBase param);

		OperationResult DeleteExpItem(MContext ctx, ParamBase param);

		OperationResult ArchiveItem(MContext ctx, string itemIds, bool isRestore = false);

		OperationResult ImportExpenseItemsList(MContext ctx, List<BDExpenseItemModel> list);

		ImportTemplateModel GetImportTemplateModel(MContext ctx);
	}
}
