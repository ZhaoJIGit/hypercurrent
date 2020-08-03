using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;

namespace JieNor.Megi.DataRepository.BAS
{
	public class BASCurrencyRepository : DataServiceT<BASCurrencyModel>
	{
		public List<BASCurrencyModel> GetList(MContext context)
		{
			string sql = "select c.MItemID AS MItemID,cl.MName as MName,cl.MName AS MLocalName from t_bas_currency c\r\n                            inner join t_bas_currency_l cl ON c.MItemID=cl.MParentID and cl.MLocaleID = @LaunguageCode and cl.MIsDelete = 0 \r\n                            WHERE  c.MIsDelete = 0 \r\n                            order by convert(cl.MName using gbk)";
			MySqlParameter[] cmdParms = new MySqlParameter[1]
			{
				new MySqlParameter("@LaunguageCode", context.MLCID)
			};
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(context);
			DataSet dataSet = dynamicDbHelperMySQL.Query(sql, cmdParms);
			if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return null;
			}
			return ModelInfoManager.DataTableToList<BASCurrencyModel>(dataSet.Tables[0]);
		}

		public List<BASCurrencyViewModel> GetViewList(MContext context)
		{
			string sql = "select c.MItemID AS MCurrencyID,cl.MName AS MLocalName ,c.MSeq from t_bas_currency c\r\n                            inner join t_bas_currency_l cl ON c.MItemID=cl.MParentID and cl.MLocaleID = @LaunguageCode and cl.MIsDelete = 0 \r\n                            where c.MIsDelete = 0\r\n                            order by c.MSeq ,cl.MName";
			MySqlParameter[] array = new MySqlParameter[1]
			{
				new MySqlParameter("@LaunguageCode", MySqlDbType.VarChar, 6)
			};
			array[0].Value = context.MLCID;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(context);
			DataSet dataSet = dynamicDbHelperMySQL.Query(sql, array);
			if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return null;
			}
			return ModelInfoManager.DataTableToList<BASCurrencyViewModel>(dataSet.Tables[0]);
		}

		public BASCurrencyModel GetModel(BASCurrencyModel model)
		{
			return new BASCurrencyModel();
		}

		public BASCurrencyViewModel GetViewModel(MContext ctx, BASCurrencyModel model)
		{
			string sql = "select c.MItemID AS MCurrencyCode,cl.MName AS MLocalName,c.MName from t_bas_currency c\r\n                            inner join t_bas_currency_l cl ON c.MItemID=cl.MParentID and cl.MIsDelete = 0  and cl.MLocaleID = @LaunguageCode\r\n                            WHERE  c.MItemID = @CurrencyID and c.MIsDelete = 0 ";
			MySqlParameter[] array = new MySqlParameter[2]
			{
				new MySqlParameter("@LaunguageCode", MySqlDbType.VarChar, 6),
				new MySqlParameter("@CurrencyID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ctx.MLCID;
			array[1].Value = model.MItemID;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(sql, array);
			if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return null;
			}
			List<BASCurrencyViewModel> list = ModelInfoManager.DataTableToList<BASCurrencyViewModel>(dataSet.Tables[0]);
			return list[0];
		}
	}
}
