using JieNor.Megi.BusinessContract.PA;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.DataRepository.PA;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.BusinessService.PA
{
	public class PAPaySettingBussiness : IPAPaySettingBussiness
	{
		private PAPaySettingRepository dal = new PAPaySettingRepository();

		public OperationResult InsertOrUpdate(MContext ctx, PAPaySettingModel model)
		{
			model.MOrgID = ctx.MOrgID;
			return ModelInfoManager.InsertOrUpdate<PAPaySettingModel>(ctx, model, null);
		}

		public PAPaySettingModel GetModel(MContext ctx)
		{
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.Equal(" MOrgID ", ctx.MOrgID);
			return dal.GetDataModelByFilter(ctx, sqlWhere);
		}
	}
}
