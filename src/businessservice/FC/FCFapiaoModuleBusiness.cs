using JieNor.Megi.BusinessContract.FC;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.FC;
using JieNor.Megi.DataRepository.FC;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessService.FC
{
	public class FCFapiaoModuleBusiness : IFCFapiaoModuleBusiness, IDataContract<FCFapiaoModuleModel>
	{
		private FCFapiaoModuleRepository dal = new FCFapiaoModuleRepository();

		public OperationResult SaveFapiaoModule(MContext ctx, FCFapiaoModuleModel model)
		{
			return dal.SaveFapiaoModule(ctx, model);
		}

		public OperationResult DeleteFapiaoModules(MContext ctx, List<string> ids)
		{
			return dal.DeleteFapiaoModules(ctx, ids);
		}

		public DataGridJson<FCFapiaoModuleModel> GetFapiaoModulePageList(MContext ctx, FCFastCodeFilterModel filter)
		{
			List<FCFapiaoModuleModel> fapiaoModulePageList = dal.GetFapiaoModulePageList(ctx, filter);
			int fapiaoModulePageCount = dal.GetFapiaoModulePageCount(ctx, filter);
			return new DataGridJson<FCFapiaoModuleModel>
			{
				rows = fapiaoModulePageList,
				total = fapiaoModulePageCount
			};
		}

		public int GetFapiaoModulePageListCount(MContext ctx)
		{
			return dal.GetFapiaoModulePageCount(ctx, new FCFastCodeFilterModel());
		}

		public List<FCFapiaoModuleModel> GetFapiaoModuleList(MContext ctx, FCFastCodeFilterModel filter)
		{
			return dal.GetFapiaoModuleModelList(ctx, filter);
		}

		public OperationResult Delete(MContext ctx, string pkID)
		{
			return dal.Delete(ctx, pkID);
		}

		public OperationResult DeleteModels(MContext ctx, List<string> pkID)
		{
			return dal.DeleteModels(ctx, pkID);
		}

		public bool Exists(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.Exists(ctx, pkID, includeDelete);
		}

		public bool ExistsByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.ExistsByFilter(ctx, filter);
		}

		public FCFapiaoModuleModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.GetDataModel(ctx, pkID, includeDelete);
		}

		public FCFapiaoModuleModel GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.GetDataModelByFilter(ctx, filter);
		}

		public List<FCFapiaoModuleModel> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelList(ctx, filter, includeDelete);
		}

		public DataGridJson<FCFapiaoModuleModel> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelPageList(ctx, filter, includeDelete);
		}

		public OperationResult InsertOrUpdate(MContext ctx, FCFapiaoModuleModel modelData, string fields = null)
		{
			return dal.InsertOrUpdate(ctx, modelData, fields);
		}

		public OperationResult InsertOrUpdateModels(MContext ctx, List<FCFapiaoModuleModel> modelData, string fields = null)
		{
			return dal.InsertOrUpdateModels(ctx, modelData, fields);
		}
	}
}
