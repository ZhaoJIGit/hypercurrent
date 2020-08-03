using JieNor.Megi.BusinessContract.FA;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.FA;
using JieNor.Megi.DataRepository.FA;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessService.FA
{
	public class FAFixAssetsTypeBusiness : IFAFixAssetsTypeBusiness, IDataContract<FAFixAssetsTypeModel>
	{
		private readonly FAFixAssetsTypeRepository dal = new FAFixAssetsTypeRepository();

		public List<FAFixAssetsTypeModel> GetFixAssetsTypeList(MContext ctx, string itemID = null)
		{
			return dal.GetFixAssetsTypeList(ctx, itemID);
		}

		public FAFixAssetsTypeModel GetFixAssetsTypeModel(MContext ctx, string itemID = null)
		{
			if (string.IsNullOrWhiteSpace(itemID))
			{
				return new FAFixAssetsTypeModel();
			}
			GLUtility gLUtility = new GLUtility();
			FAFixAssetsTypeModel dataModel = GetDataModel(ctx, itemID, false);
			dataModel.MCheckGroupValueModel = gLUtility.MergeCheckGroupValueModelByIDs(ctx, new List<string>
			{
				dataModel.MFixCheckGroupValueID,
				dataModel.MDepCheckGroupValueID
			});
			return dataModel;
		}

		public OperationResult DeleteFixAssetsType(MContext ctx, List<string> itemIDs)
		{
			return dal.DeleteFixAssetsType(ctx, itemIDs);
		}

		public OperationResult SaveFixAssetsType(MContext ctx, FAFixAssetsTypeModel model)
		{
			if (model.MCheckGroupValueModel != null)
			{
				GLUtility gLUtility = new GLUtility();
				model.MFixCheckGroupValueID = gLUtility.GetCheckGroupValueID(ctx, model.MFixAccountCode, model.MCheckGroupValueModel);
				model.MDepCheckGroupValueID = gLUtility.GetCheckGroupValueID(ctx, model.MDepAccountCode, model.MCheckGroupValueModel);
			}
			return dal.SaveFixAssetsType(ctx, model);
		}

		public bool Exists(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.Exists(ctx, pkID, includeDelete);
		}

		public bool ExistsByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.ExistsByFilter(ctx, filter);
		}

		public OperationResult InsertOrUpdate(MContext ctx, FAFixAssetsTypeModel modelData, string fields = null)
		{
			return dal.InsertOrUpdate(ctx, modelData, fields);
		}

		public OperationResult InsertOrUpdateModels(MContext ctx, List<FAFixAssetsTypeModel> modelData, string fields = null)
		{
			return dal.InsertOrUpdateModels(ctx, modelData, fields);
		}

		public OperationResult Delete(MContext ctx, string pkID)
		{
			return dal.Delete(ctx, pkID);
		}

		public OperationResult DeleteModels(MContext ctx, List<string> pkID)
		{
			return dal.DeleteModels(ctx, pkID);
		}

		public FAFixAssetsTypeModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.GetDataModel(ctx, pkID, includeDelete);
		}

		public FAFixAssetsTypeModel GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.GetDataModelByFilter(ctx, filter);
		}

		public List<FAFixAssetsTypeModel> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelList(ctx, filter, includeDelete);
		}

		public DataGridJson<FAFixAssetsTypeModel> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelPageList(ctx, filter, includeDelete);
		}
	}
}
