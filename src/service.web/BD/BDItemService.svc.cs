using JieNor.Megi.BusinessContract.BD;
using JieNor.Megi.BusinessService.BD;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IO.Export.DataRowModel;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.BD;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.BD
{
	public class BDItemService : ServiceT<BDItemModel>, IBDItem
	{
		private readonly IBDItemBusiness biz = new BDItemBusiness();

		public MActionResult<List<BDItemModel>> GetListByWhere(string filterString, string accessToken = null)
		{
			IBDItemBusiness iBDItemBusiness = biz;
			return base.RunFunc(iBDItemBusiness.GetListByWhere, filterString, accessToken);
		}

		public MActionResult<DataGridJson<BDItemModel>> GetPageList(BDItemListFilterModel filter, string accessToken = null)
		{
			IBDItemBusiness iBDItemBusiness = biz;
			return base.RunFunc(iBDItemBusiness.GetPageList, filter, accessToken);
		}

		public MActionResult<BDItemModel> GetEditInfo(BDItemModel info, string accessToken = null)
		{
			IBDItemBusiness iBDItemBusiness = biz;
			return base.RunFunc(iBDItemBusiness.GetEditInfo, info, accessToken);
		}

		public MActionResult<OperationResult> ItemInfoUpd(BDItemModel item, string accessToken = null)
		{
			IBDItemBusiness iBDItemBusiness = biz;
			return base.RunFunc(iBDItemBusiness.ItemInfoUpd, item, accessToken);
		}

		public MActionResult<BDIsCanDeleteModel> IsCanDeleteOrInactive(ParamBase param, string accessToken = null)
		{
			IBDItemBusiness iBDItemBusiness = biz;
			return base.RunFunc(iBDItemBusiness.IsCanDeleteOrInactive, param, accessToken);
		}

		public MActionResult<OperationResult> DeleteItemList(ParamBase param, string accessToken = null)
		{
			IBDItemBusiness iBDItemBusiness = biz;
			return base.RunFunc(iBDItemBusiness.DeleteItemList, param, accessToken);
		}

		public MActionResult<bool> IsItemCodeExists(string id, BDItemModel model, string accessToken = null)
		{
			IBDItemBusiness iBDItemBusiness = biz;
			return base.RunFunc(iBDItemBusiness.IsItemCodeExists, id, model, accessToken);
		}

		public MActionResult<OperationResult> IsImportItemsCodeExist(List<BDItemModel> list, string accessToken = null)
		{
			IBDItemBusiness iBDItemBusiness = biz;
			return base.RunFunc(iBDItemBusiness.IsImportItemsCodeExist, list, accessToken);
		}

		public MActionResult<OperationResult> ImportItemList(List<BDItemModel> list, string accessToken = null)
		{
			IBDItemBusiness iBDItemBusiness = biz;
			return base.RunFunc(iBDItemBusiness.ImportItemList, list, accessToken);
		}

		public MActionResult<ImportTemplateModel> GetImportTemplateModel(string accessToken = null)
		{
			IBDItemBusiness iBDItemBusiness = biz;
			return base.RunFunc(iBDItemBusiness.GetImportTemplateModel, accessToken);
		}

		public MActionResult<List<ItemRowModel>> GetReportList(string accessToken = null)
		{
			IBDItemBusiness iBDItemBusiness = biz;
			return base.RunFunc(iBDItemBusiness.GetReportList, accessToken);
		}

		public MActionResult<OperationResult> ArchiveItem(string itemIds, bool isRestore = false, string accessToken = null)
		{
			IBDItemBusiness iBDItemBusiness = biz;
			return base.RunFunc(iBDItemBusiness.ArchiveItem, itemIds, isRestore, null);
		}

		public MActionResult<List<BDItemModel>> GetItemList(BDItemListFilterModel filter, string accessToken = null)
		{
			IBDItemBusiness iBDItemBusiness = biz;
			return base.RunFunc(iBDItemBusiness.GetItemList, filter, null);
		}

		public MActionResult<List<BDItemModel>> GetModelList(SqlWhere filter, bool includeDelete = false, string accessToken = null)
		{
			IBDItemBusiness iBDItemBusiness = biz;
			return base.GetModelList(iBDItemBusiness.GetModelList, filter, includeDelete, accessToken);
		}
	}
}
