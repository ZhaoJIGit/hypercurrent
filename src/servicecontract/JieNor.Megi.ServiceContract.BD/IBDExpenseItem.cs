using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.BD
{
	[ServiceContract]
	public interface IBDExpenseItem
	{
		[OperationContract]
		MActionResult<List<BDExpenseItemModel>> GetList(bool isActive = true, string accessToken = null);

		[OperationContract]
		MActionResult<List<BDExpenseItemModel>> GetListByTier(bool includeDisable = false, bool isAddChildren = false, string accessToken = null);

		[OperationContract]
		MActionResult<List<BDExpenseItemModel>> GetNoParentItemList(string accessToken = null);

		[OperationContract]
		MActionResult<List<BDExpenseItemModel>> GetParentItemList(string itemId = null, string accessToken = null);

		[OperationContract]
		MActionResult<DataGridJson<BDExpenseItemModel>> GetPageList(BDExpenseItemListFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<BDExpenseItemModel> GetEditInfo(BDExpenseItemModel Expenseinfo, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> Update(BDExpenseItemModel Expenseitem, string accessToken = null);

		[OperationContract]
		MActionResult<BDIsCanDeleteModel> IsCanDeleteOrInactive(ParamBase param, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> DeleteExpItem(ParamBase param, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> ArchiveItem(string itemIds, bool isRestore = false, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> ImportExpenseItemsList(List<BDExpenseItemModel> list, string accessToken = null);

		[OperationContract]
		MActionResult<ImportTemplateModel> GetImportTemplateModel(string accessToken = null);
	}
}
