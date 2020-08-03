using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IO.Export.DataRowModel;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.BD
{
	[ServiceContract]
	public interface IBDItem
	{
		[OperationContract]
		MActionResult<List<BDItemModel>> GetListByWhere(string filterString, string accessToken = null);

		[OperationContract]
		MActionResult<DataGridJson<BDItemModel>> GetPageList(BDItemListFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<List<ItemRowModel>> GetReportList(string accessToken = null);

		[OperationContract]
		MActionResult<BDItemModel> GetEditInfo(BDItemModel info, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> ItemInfoUpd(BDItemModel item, string accessToken = null);

		[OperationContract]
		MActionResult<BDIsCanDeleteModel> IsCanDeleteOrInactive(ParamBase param, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> DeleteItemList(ParamBase param, string accessToken = null);

		[OperationContract]
		MActionResult<bool> IsItemCodeExists(string id, BDItemModel model, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> IsImportItemsCodeExist(List<BDItemModel> list, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> ImportItemList(List<BDItemModel> list, string accessToken = null);

		[OperationContract]
		MActionResult<ImportTemplateModel> GetImportTemplateModel(string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> ArchiveItem(string itemIds, bool isRestore = false, string accessToken = null);

		[OperationContract]
		MActionResult<List<BDItemModel>> GetItemList(BDItemListFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<List<BDItemModel>> GetModelList(SqlWhere filter, bool includeDelete = false, string accessToken = null);
	}
}
