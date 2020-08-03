using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IO.Export.DataRowModel;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.BD
{
	public interface IBDItemBusiness : IDataContract<BDItemModel>
	{
		List<BDItemModel> GetListByWhere(MContext ctx, string filterString);

		DataGridJson<BDItemModel> GetPageList(MContext ctx, BDItemListFilterModel filter);

		List<ItemRowModel> GetReportList(MContext ctx);

		BDItemModel GetEditInfo(MContext ctx, BDItemModel info);

		OperationResult ItemInfoUpd(MContext ctx, BDItemModel item);

		BDIsCanDeleteModel IsCanDeleteOrInactive(MContext ctx, ParamBase param);

		OperationResult DeleteItemList(MContext ctx, ParamBase param);

		bool IsItemCodeExists(MContext ctx, string id, BDItemModel model);

		OperationResult IsImportItemsCodeExist(MContext ctx, List<BDItemModel> list);

		OperationResult ImportItemList(MContext ctx, List<BDItemModel> list);

		ImportTemplateModel GetImportTemplateModel(MContext ctx);

		OperationResult ArchiveItem(MContext ctx, string itemIds, bool isRestore = false);

		List<BDItemModel> GetItemList(MContext ctx, BDItemListFilterModel filter);
	}
}
