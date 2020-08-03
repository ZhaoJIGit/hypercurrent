using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.API;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.DataRepository.API
{
	public class APILogRepository
	{
		public string Add(MContext ctx, APILogModel model)
		{
			List<CommandInfo> insertOrUpdateCmd = ModelInfoManager.GetInsertOrUpdateCmd<APILogModel>(ctx, model, null, true);
			DbHelperMySQL.ExecuteSqlTran(ctx, insertOrUpdateCmd);
			return model.MID;
		}
	}
}
