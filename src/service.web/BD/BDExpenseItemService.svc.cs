using JieNor.Megi.BusinessContract.BD;
using JieNor.Megi.BusinessService.BD;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.BD;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.BD
{
	public class BDExpenseItemService : ServiceT<BDExpenseItemModel>, IBDExpenseItem
	{
		private IBDExpenseItemBusiness biz = new BDExpenseItemBusiness();

		public MActionResult<List<BDExpenseItemModel>> GetList(bool isActive, string accessToken = null)
		{
			IBDExpenseItemBusiness iBDExpenseItemBusiness = biz;
			return base.RunFunc(iBDExpenseItemBusiness.GetList, isActive, accessToken);
		}

		public MActionResult<List<BDExpenseItemModel>> GetListByTier(bool includeDisable = false, bool isAddChildren = false, string accessToken = null)
		{
			IBDExpenseItemBusiness iBDExpenseItemBusiness = biz;
			return base.RunFunc(iBDExpenseItemBusiness.GetListByTier, includeDisable, isAddChildren, accessToken);
		}

		public MActionResult<List<BDExpenseItemModel>> GetNoParentItemList(string accessToken = null)
		{
			IBDExpenseItemBusiness iBDExpenseItemBusiness = biz;
			return base.RunFunc(iBDExpenseItemBusiness.GetNoParentList, accessToken);
		}

		public MActionResult<DataGridJson<BDExpenseItemModel>> GetPageList(BDExpenseItemListFilterModel filter, string accessToken = null)
		{
			IBDExpenseItemBusiness iBDExpenseItemBusiness = biz;
			return base.RunFunc(iBDExpenseItemBusiness.GetPageList, filter, accessToken);
		}

		public MActionResult<BDExpenseItemModel> GetEditInfo(BDExpenseItemModel model, string accessToken = null)
		{
			IBDExpenseItemBusiness iBDExpenseItemBusiness = biz;
			return base.RunFunc(iBDExpenseItemBusiness.GetEditInfo, model, accessToken);
		}

		public MActionResult<OperationResult> Update(BDExpenseItemModel model, string accessToken = null)
		{
			IBDExpenseItemBusiness iBDExpenseItemBusiness = biz;
			return base.RunFunc(iBDExpenseItemBusiness.Update, model, accessToken);
		}

		public MActionResult<BDIsCanDeleteModel> IsCanDeleteOrInactive(ParamBase param, string accessToken = null)
		{
			IBDExpenseItemBusiness iBDExpenseItemBusiness = biz;
			return base.RunFunc(iBDExpenseItemBusiness.IsCanDeleteOrInactive, param, accessToken);
		}

		public MActionResult<OperationResult> DeleteExpItem(ParamBase param, string accessToken = null)
		{
			IBDExpenseItemBusiness iBDExpenseItemBusiness = biz;
			return base.RunFunc(iBDExpenseItemBusiness.DeleteExpItem, param, accessToken);
		}

		public MActionResult<List<BDExpenseItemModel>> GetParentItemList(string itemId = null, string accessToken = null)
		{
			IBDExpenseItemBusiness iBDExpenseItemBusiness = biz;
			return base.RunFunc(iBDExpenseItemBusiness.GetParentList, itemId, null);
		}

		public MActionResult<OperationResult> ArchiveItem(string itemIds, bool isRestore = false, string accessToken = null)
		{
			IBDExpenseItemBusiness iBDExpenseItemBusiness = biz;
			return base.RunFunc(iBDExpenseItemBusiness.ArchiveItem, itemIds, isRestore, null);
		}

		public MActionResult<OperationResult> ImportExpenseItemsList(List<BDExpenseItemModel> list, string accessToken = null)
		{
			IBDExpenseItemBusiness iBDExpenseItemBusiness = biz;
			return base.RunFunc(iBDExpenseItemBusiness.ImportExpenseItemsList, list, accessToken);
		}

		public MActionResult<ImportTemplateModel> GetImportTemplateModel(string accessToken = null)
		{
			IBDExpenseItemBusiness iBDExpenseItemBusiness = biz;
			return base.RunFunc(iBDExpenseItemBusiness.GetImportTemplateModel, accessToken);
		}
	}
}
