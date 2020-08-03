using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;

namespace JieNor.Megi.DataRepository.BAS
{
	public class BASTimezoneRepository
	{
		public static List<BASTimezoneModel> GetList(string languageCode)
		{
			string sQLString = "select tz.MitemID as MName,tzl.MName as MLocalName from t_bas_Timezones tz\r\n                        inner join t_bas_Timezones_l tzl ON tz.MitemID=tzl.MParentID and tzl.MIsDelete = 0  and tzl.MLocaleID = @LaunguageCode \r\n                        WHERE tz.MIsDelete = 0 ";
			MySqlParameter[] array = new MySqlParameter[1]
			{
				new MySqlParameter("@LaunguageCode", MySqlDbType.VarChar, 6)
			};
			array[0].Value = languageCode;
			DataSet dataSet = DbHelperMySQL.Query(sQLString, array);
			if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return null;
			}
			return ModelInfoManager.DataTableToList<BASTimezoneModel>(dataSet.Tables[0]);
		}
	}
}
