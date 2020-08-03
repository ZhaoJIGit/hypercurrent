using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.FA;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace JieNor.Megi.DataRepository.RPT
{
	public class RPTFABaseRepository : RPTBaseREpository
	{
		public static List<FAFixAssetsChangeModel> GetFAChangeList(MContext context)
		{
			string fAChangeSql = GetFAChangeSql();
			MySqlParameter[] array = new MySqlParameter[1]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = context.MOrgID;
			return ModelInfoManager.GetDataModelBySql<FAFixAssetsChangeModel>(context, fAChangeSql, array);
		}

		private static string GetFAChangeSql()
		{
			return " SELECT * FROM  t_fa_fixassetschange WHERE MOrgID = @MOrgID AND MIsDelete = 0 ";
		}
	}
}
