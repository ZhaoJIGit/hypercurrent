using JieNor.Megi.BusinessContract.BD;
using JieNor.Megi.BusinessService.RI;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BD.RI;
using JieNor.Megi.DataModel.RI;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.BusinessService.BD
{
	public class BDInspectBusiness : IBDInspect
	{
		private BDInspectRepository dal = new BDInspectRepository();

		public List<BDInspectItemTreeModel> GetInspectItemTreeList(MContext ctx, BDInspectItemFilterModel filter)
		{
			List<BDInspectItemModel> inspectItemList = GetInspectItemList(ctx, filter);
			List<BDInspectItemTreeModel> list = new List<BDInspectItemTreeModel>();
			if (inspectItemList == null || inspectItemList.Count() == 0)
			{
				return list;
			}
			List<BDInspectItemModel> list2 = (from x in inspectItemList
			where x.MParentID == "0"
			select x).ToList();
			foreach (BDInspectItemModel item in list2)
			{
				BDInspectItemTreeModel inspectItemTree = GetInspectItemTree(item, inspectItemList);
				list.Add(inspectItemTree);
				inspectItemTree.MEnable = (inspectItemTree.children != null && !inspectItemTree.children.Exists((BDInspectItemTreeModel x) => !x.MEnable));
			}
			return list;
		}

		private BDInspectItemTreeModel GetInspectItemTree(BDInspectItemModel inspectItem, List<BDInspectItemModel> itemList)
		{
			List<BDInspectItemModel> list = (from x in itemList
			where x.MParentID == inspectItem.MItemID
			select x).ToList();
			BDInspectItemTreeModel bDInspectItemTreeModel = ConvertInspectTreeModel(inspectItem);
			if (list == null || list.Count() == 0)
			{
				return bDInspectItemTreeModel;
			}
			foreach (BDInspectItemModel item in list)
			{
				BDInspectItemTreeModel inspectItemTree = GetInspectItemTree(item, itemList);
				bDInspectItemTreeModel.children = (bDInspectItemTreeModel.children ?? new List<BDInspectItemTreeModel>());
				bDInspectItemTreeModel.children.Add(inspectItemTree);
			}
			return bDInspectItemTreeModel;
		}

		private BDInspectItemTreeModel ConvertInspectTreeModel(BDInspectItemModel item)
		{
			BDInspectItemTreeModel bDInspectItemTreeModel = new BDInspectItemTreeModel();
			bDInspectItemTreeModel.id = item.MItemID;
			bDInspectItemTreeModel.text = item.MName;
			bDInspectItemTreeModel.MIndex = item.MIndex;
			bDInspectItemTreeModel.MEnable = item.MEnable;
			bDInspectItemTreeModel.MSettingID = item.MSettingID;
			bDInspectItemTreeModel.MSettingParamID = item.MSettingParamID;
			bDInspectItemTreeModel.MParameter = item.MParameter;
			bDInspectItemTreeModel.MRequirePass = item.MRequirePass;
			return bDInspectItemTreeModel;
		}

		public OperationResult InitInspectItem(MContext ctx)
		{
			OperationResult operationResult = new OperationResult();
			List<BDInspectItemModel> inspectItemList = GetInspectItemList(ctx, null);
			if (inspectItemList != null && inspectItemList.Count() > 0)
			{
				operationResult.Success = true;
				return operationResult;
			}
			return operationResult;
		}

		public OperationResult SaveInspectSetting(MContext ctx, List<RICategorySettingModel> inspectSetingItemList)
		{
			OperationResult operationResult = new OperationResult();
			if (inspectSetingItemList == null || inspectSetingItemList.Count() == 0)
			{
				operationResult.Success = true;
				return operationResult;
			}
			inspectSetingItemList = (from x in inspectSetingItemList
			where !string.IsNullOrWhiteSpace(x.MItemID)
			select x).ToList();
			List<CommandInfo> list = new List<CommandInfo>();
			List<RICategorySettingParamModel> list2 = (from x in inspectSetingItemList
			where x.MSettingParam != null
			select x.MSettingParam).ToList();
			if (list2 != null && list2.Count() > 0)
			{
				list.AddRange(ModelInfoManager.GetInsertOrUpdateCmds(ctx, list2, null, true));
			}
			List<BDInspectItemModel> inspectItemList = GetInspectItemList(ctx, null);
			List<BDInspectItemModel> list3 = (from x in inspectItemList
			where x.MParentID == "0"
			select x).ToList();
			foreach (BDInspectItemModel item in list3)
			{
				List<BDInspectItemModel> list4 = (from x in inspectItemList
				where x.MParentID == item.MItemID
				select x).ToList();
				if (list4 != null && list4.Count != 0)
				{
					List<string> settingIdList = (from x in list4
					select x.MSettingID).ToList();
					List<RICategorySettingModel> source = (from x in inspectSetingItemList
					where settingIdList.Contains(x.MItemID)
					select x).ToList();
					RICategorySettingModel rICategorySettingModel = inspectSetingItemList.FirstOrDefault((RICategorySettingModel x) => x.MItemID == item.MSettingID);
					rICategorySettingModel.MEnable = source.Any((RICategorySettingModel x) => x.MEnable);
				}
			}
			List<string> fields = new List<string>
			{
				"MEnable"
			};
			list.AddRange(ModelInfoManager.GetInsertOrUpdateCmds(ctx, inspectSetingItemList, fields, true));
			operationResult.Success = (BDRepository.ExecuteSqlTran(ctx, list) > 0);
			new RIInspectBusiness().UpdateCategory(ctx);
			return operationResult;
		}

		public List<BDInspectItemModel> GetInspectItemList(MContext ctx, BDInspectItemFilterModel filter)
		{
			List<BDInspectItemModel> list = new List<BDInspectItemModel>();
			List<RICategoryModel> categoryList = new RIInspectBusiness().GetCategoryList(ctx, true, 0, 0);
			if (categoryList != null && categoryList.Count() > 0)
			{
				foreach (RICategoryModel item2 in categoryList)
				{
					BDInspectItemModel item = CovertToItem(item2);
					list.Add(item);
				}
			}
			return list;
		}

		private BDInspectItemModel CovertToItem(RICategoryModel ri)
		{
			BDInspectItemModel bDInspectItemModel = new BDInspectItemModel();
			bDInspectItemModel.MItemID = ri.MItemID;
			bDInspectItemModel.MIndex = ri.MIndex;
			bDInspectItemModel.MParentID = ri.MParentID;
			bDInspectItemModel.MName = ri.MName;
			bDInspectItemModel.MEnable = (ri.MSetting != null && ri.MSetting.MEnable);
			bDInspectItemModel.MSettingID = ((ri.MSetting == null) ? "" : ri.MSetting.MItemID);
			if (ri.MSetting != null && ri.MSetting.MSettingParam != null)
			{
				bDInspectItemModel.MSettingParamID = ri.MSetting.MSettingParam.MItemID;
				bDInspectItemModel.MParameter = ri.MSetting.MSettingParam;
				bDInspectItemModel.MRequirePass = ri.MSetting.MRequirePass;
			}
			return bDInspectItemModel;
		}

		public OperationResult SaveInspectItemSetting(MContext ctx, List<RICategorySettingModel> inspectSetingItemList)
		{
			List<string> list = new List<string>();
			list.Add("MEnable");
			return ModelInfoManager.InsertOrUpdate(ctx, inspectSetingItemList, list);
		}
	}
}
