using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.MI;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.BusinessService.MI.Biz
{
	public class BankTypeMigrate : MigrateBase, IMigrate
	{
		public OperationResult SaveMigration(MContext ctx, MigrateDataModel model)
		{
			OperationResult operationResult = new OperationResult();
			List<CommandInfo> list = new List<CommandInfo>();
			List<BDBankTypeModel> modelData = base.DeSerializeObject<List<BDBankTypeModel>>(model.DataListJson);
			list.AddRange(ModelInfoManager.GetInsertOrUpdateCmds(ctx, modelData, null, true));
			list.AddRange(ModelInfoManager.GetInsertOrUpdateCmds(ctx, model.LogList, null, true));
			operationResult.Success = (DbHelperMySQL.ExecuteSqlTran(ctx, list) > 0);
			return operationResult;
		}

		public OperationResult DeleteMigration(MContext ctx, string migrationId)
		{
			OperationResult operationResult = new OperationResult();
			List<CommandInfo> list = new List<CommandInfo>();
			List<MigrateLogBaseModel> logList = base.GetLogList(ctx, migrationId);
			list.AddRange(ModelInfoManager.GetDeleteFlagCmd<MigrateLogBaseModel>(ctx, (from f in logList
			select f.MItemID).ToList()));
			list.AddRange(ModelInfoManager.GetDeleteFlagCmd<BDBankTypeModel>(ctx, (from f in logList
			select f.MMegiID).ToList()));
			operationResult.Success = (DbHelperMySQL.ExecuteSqlTran(ctx, list) > 0);
			return operationResult;
		}

		public string ViewMigration(MContext ctx, string migrationId)
		{
			return "";
		}

		public int GetlistCount(MContext ctx)
		{
			return 0;
		}
	}
}
