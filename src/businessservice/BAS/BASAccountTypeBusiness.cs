using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataRepository.BAS;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessService.BAS
{
	public class BASAccountTypeBusiness
	{
		private readonly BASAccountTypeRepository dal = new BASAccountTypeRepository();

		public List<BASAccountTypeModel> GetAccountTypeList(MContext ctx, bool ignoreLocale = false)
		{
			return dal.GetAccountTypeList(ctx, ignoreLocale);
		}

		public bool Exists(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.Exists(ctx, pkID, includeDelete);
		}

		public bool ExistsByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.ExistsByFilter(ctx, filter);
		}

		public OperationResult InsertOrUpdate(MContext ctx, BASAccountTypeModel modelData, string fields = null)
		{
			return dal.InsertOrUpdate(ctx, modelData, fields);
		}

		public OperationResult InsertOrUpdateModels(MContext ctx, List<BASAccountTypeModel> modelData, string fields = null)
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

		public BASAccountTypeModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.GetDataModel(ctx, pkID, includeDelete);
		}

		public BASAccountTypeModel GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.GetDataModelByFilter(ctx, filter);
		}

		public List<BASAccountTypeModel> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelList(ctx, filter, includeDelete);
		}

		public DataGridJson<BASAccountTypeModel> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelPageList(ctx, filter, includeDelete);
		}
	}
}
