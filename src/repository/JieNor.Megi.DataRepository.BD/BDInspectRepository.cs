using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.DataModel.BD.RI;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data.Common;

namespace JieNor.Megi.DataRepository.BD
{
	public class BDInspectRepository : DataServiceT<BDInspectItemModel>
	{
		public OperationResult InitInspectItem(MContext ctx)
		{
			CommandInfo obj = new CommandInfo
			{
				CommandText = "update t_ri_categorysetting set MIsDelete = 1 where MOrgID = @MOrgID and MIsDelete = 0 "
			};
			DbParameter[] array = obj.Parameters = ctx.GetParameters((MySqlParameter)null);
			CommandInfo item = obj;
			CommandInfo obj2 = new CommandInfo
			{
				CommandText = "INSERT INTO t_ri_categorysetting(MItemID, MOrgID, MID, MEnable, MRequirePass, MIsActive, MIsDelete, MCreatorID , MCreateDate, MModifierID, MModifyDate) \r\n                           SELECT REPLACE(UUID(), '-', ''), @MOrgID, MItemID, MEnable , MRequirePass , MIsActive , MIsDelete ,@MUserID, @MCreateDate, @MUserID, @MCreateDate from t_ri_category"
			};
			array = (obj2.Parameters = ctx.GetParameters((MySqlParameter)null));
			CommandInfo item2 = obj2;
			List<CommandInfo> cmdList = new List<CommandInfo>
			{
				item,
				item2
			};
			int num = new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(cmdList);
			OperationResult operationResult = new OperationResult();
			operationResult.Success = (num > 0);
			return operationResult;
		}
	}
}
