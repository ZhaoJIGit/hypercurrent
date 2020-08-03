using JieNor.Megi.BusinessContract.FC;
using JieNor.Megi.BusinessService.BD;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.FC;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.FC;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.BusinessService.FC
{
	public class FCCashCodingModuleBusiness : IFCCashCodingModuleBusiness, IDataContract<FCCashCodingModuleModel>
	{
		private FCCashCodingModuleRepository _cashCoding = new FCCashCodingModuleRepository();

		private void UpdateCashCodingModuleTrackInfo(MContext ctx, List<FCCashCodingModuleListModel> moduleList)
		{
			if (moduleList != null && moduleList.Count != 0)
			{
				BDTrackRepository bDTrackRepository = new BDTrackRepository();
				List<NameValueModel> trackBasicInfo = bDTrackRepository.GetTrackBasicInfo(ctx, null, false, true);
				BDContactsBusiness bDContactsBusiness = new BDContactsBusiness();
				List<BDContactItem> contactItemList = bDContactsBusiness.GetContactItemList(ctx, new BDContactsListFilter());
				foreach (FCCashCodingModuleListModel module in moduleList)
				{
					module.MTrackItem1Name = GetTrackItemName(module.MTrackItem1, 1, trackBasicInfo);
					module.MTrackItem2Name = GetTrackItemName(module.MTrackItem2, 2, trackBasicInfo);
					module.MTrackItem3Name = GetTrackItemName(module.MTrackItem3, 3, trackBasicInfo);
					module.MTrackItem4Name = GetTrackItemName(module.MTrackItem4, 4, trackBasicInfo);
					module.MTrackItem5Name = GetTrackItemName(module.MTrackItem5, 5, trackBasicInfo);
					if (!string.IsNullOrEmpty(module.MContactID))
					{
						BDContactItem bDContactItem = (from t in contactItemList
						where t.MItemID == module.MContactID
						select t).FirstOrDefault();
						if (bDContactItem != null)
						{
							module.MContactName = bDContactItem.MName;
						}
					}
				}
			}
		}

		private string GetTrackItemName(string entryId, int index, List<NameValueModel> trackList)
		{
			if (trackList == null || trackList.Count == 0 || string.IsNullOrEmpty(entryId) || index > trackList.Count)
			{
				return "";
			}
			return (from t in trackList[index - 1].MChildren
			where t.MValue == entryId
			select t.MName).FirstOrDefault();
		}

		public List<FCCashCodingModuleListModel> GetListByCode(MContext ctx, string code)
		{
			List<FCCashCodingModuleListModel> listByCode = _cashCoding.GetListByCode(ctx, code);
			UpdateCashCodingModuleTrackInfo(ctx, listByCode);
			return listByCode;
		}

		public OperationResult UpdateCashCodingModuleModel(MContext ctx, FCCashCodingModuleModel model)
		{
			return _cashCoding.UpdateCashCodingModuleModel(ctx, model);
		}

		public DataGridJson<FCCashCodingModuleListModel> GetCashCodingByPageList(MContext ctx, FCCashCodingModuleListFilter model)
		{
			BDContactsBusiness bDContactsBusiness = new BDContactsBusiness();
			List<BDContactItem> contactItemList = bDContactsBusiness.GetContactItemList(ctx, new BDContactsListFilter());
			DataGridJson<FCCashCodingModuleListModel> cashCodingByPageList = _cashCoding.GetCashCodingByPageList(ctx, model);
			foreach (FCCashCodingModuleListModel row in cashCodingByPageList.rows)
			{
				if (!string.IsNullOrEmpty(row.MContactID))
				{
					BDContactItem bDContactItem = contactItemList.FirstOrDefault((BDContactItem t) => t.MItemID == row.MContactID);
					if (bDContactItem != null)
					{
						row.MContactName = bDContactItem.MName;
					}
				}
			}
			return cashCodingByPageList;
		}

		public List<FCCashCodingModuleModel> GetCashCodingModuleListWithNoEntry(MContext ctx)
		{
			return _cashCoding.GetCashCodingModuleListWithNoEntry(ctx);
		}

		public OperationResult Delete(MContext ctx, string pkID)
		{
			return _cashCoding.DeleteCashCodingModule(ctx, new List<string>
			{
				pkID
			});
		}

		public OperationResult DeleteModels(MContext ctx, List<string> pkID)
		{
			return _cashCoding.DeleteCashCodingModule(ctx, pkID);
		}

		public bool Exists(MContext ctx, string pkID, bool includeDelete = false)
		{
			return _cashCoding.Exists(ctx, pkID, includeDelete);
		}

		public bool ExistsByFilter(MContext ctx, SqlWhere filter)
		{
			return _cashCoding.ExistsByFilter(ctx, filter);
		}

		public FCCashCodingModuleModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			return _cashCoding.GetDataModel(ctx, pkID, includeDelete);
		}

		public FCCashCodingModuleModel GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			return _cashCoding.GetDataModelByFilter(ctx, filter);
		}

		public List<FCCashCodingModuleModel> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return _cashCoding.GetModelList(ctx, filter, includeDelete);
		}

		public DataGridJson<FCCashCodingModuleModel> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return _cashCoding.GetModelPageList(ctx, filter, includeDelete);
		}

		public OperationResult InsertOrUpdate(MContext ctx, FCCashCodingModuleModel modelData, string fields = null)
		{
			return _cashCoding.InsertOrUpdate(ctx, modelData, fields);
		}

		public OperationResult InsertOrUpdateModels(MContext ctx, List<FCCashCodingModuleModel> modelData, string fields = null)
		{
			return _cashCoding.InsertOrUpdateModels(ctx, modelData, fields);
		}
	}
}
