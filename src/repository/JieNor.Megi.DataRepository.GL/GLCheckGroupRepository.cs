using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace JieNor.Megi.DataRepository.GL
{
	public class GLCheckGroupRepository : DataServiceT<GLCheckGroupModel>
	{
		public List<GLCheckGroupModel> GetCheckGroupListByAcct(MContext ctx, List<string> acctIdList)
		{
			string sql = string.Format("select t1.*, t2.MItemID as MAccountID, t2.MNumber from t_gl_checkgroup t1 \r\n                            right join t_bd_account t2 on t1.MItemID=t2.MCheckGroupID and t2.MOrgID=@MOrgID and t2.MItemID in ('{0}')\r\n                            where t1.MIsDelete=0", string.Join("','", acctIdList));
			return ModelInfoManager.GetDataModelBySql<GLCheckGroupModel>(ctx, sql, ctx.GetParameters((MySqlParameter)null));
		}
	}
}
