using JieNor.Megi.BusinessContract.BAS;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataRepository.BAS;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessService.BAS
{
	public class BASOrgPrefixSettingBusiness : IBASOrgPrefixSettingBusiness, IDataContract<BASOrgPrefixSettingModel>
	{
		private readonly BASOrgPrefixSettingRepository dal = new BASOrgPrefixSettingRepository();

		public OperationResult UpdateOrgPrefixSettingModel(MContext ctx, BASOrgPrefixSettingModel model)
		{
			return dal.UpdateOrgPrefixSettingModel(ctx, model);
		}

		public BASOrgPrefixSettingModel GetOrgPrefixSettingModel(MContext ctx, string module)
		{
			return dal.GetOrgPrefixSettingModel(ctx, module);
		}

		public bool Exists(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.Exists(ctx, pkID, includeDelete);
		}

		public bool ExistsByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.ExistsByFilter(ctx, filter);
		}

		public OperationResult InsertOrUpdate(MContext ctx, BASOrgPrefixSettingModel modelData, string fields = null)
		{
			return dal.InsertOrUpdate(ctx, modelData, fields);
		}

		public OperationResult InsertOrUpdateModels(MContext ctx, List<BASOrgPrefixSettingModel> modelData, string fields = null)
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

		public BASOrgPrefixSettingModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.GetDataModel(ctx, pkID, includeDelete);
		}

		public BASOrgPrefixSettingModel GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.GetDataModelByFilter(ctx, filter);
		}

		public List<BASOrgPrefixSettingModel> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelList(ctx, filter, includeDelete);
		}

		public DataGridJson<BASOrgPrefixSettingModel> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelPageList(ctx, filter, includeDelete);
		}
	}
}
