using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace JieNor.Megi.DataRepository.BAS
{
	public class BASAccountTypeRepository : DataServiceT<BASAccountTypeModel>
	{
		public List<BASAccountTypeModel> GetAccountTypeList(MContext ctx, bool ignoreLocale = false)
		{
			string sql = string.Format("select a.*,b.MName from t_bas_accounttype a \r\n                left join t_bas_accounttype_l b on a.MItemID=b.MParentID {0} and b.MIsDelete=0\r\n                where a.MAccountTableID=@MAccountTableID and a.MIsDelete=0", ignoreLocale ? string.Empty : "and b.MLocaleID=@MLocaleID");
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@MLocaleID", ctx.MLCID),
				new MySqlParameter("@MAccountTableID", ctx.MAccountTableID)
			};
			return ModelInfoManager.GetDataModelBySql<BASAccountTypeModel>(ctx, sql, cmdParms);
		}
	}
}
