using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace JieNor.Megi.DataRepository.BAS
{
	public class BASCountryRepository
	{
		public List<BASCountryModel> GetCountryList(MContext context)
		{
			string sql = "select a.MItemID, b.MName as MCountryName from T_Bas_Country a \r\n                                left join T_Bas_Country_L b on a.MItemID=b.MParentID and b.MIsDelete = 0 \r\n                                where b.MLocaleID=@MLocaleID and a.MIsDelete = 0\r\n                                order by convert(b.MName using gbk) ";
			MySqlParameter[] array = new MySqlParameter[1]
			{
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = context.MLCID;
			return ModelInfoManager.GetDataModelBySql<BASCountryModel>(context, sql, array);
		}

		public List<BASProvinceModel> GetProvinceList(MContext context, string countryId)
		{
			string sql = "select a.MItemID, b.MName from T_Bas_Province a \r\n                                left join T_Bas_Province_L b on a.MItemID=b.MParentID and b.MIsDelete = 0 \r\n                                where a.MParentID=@MParentID and b.MLocaleID=@MLocaleID and a.MIsDelete = 0 \r\n                                order by a.MName";
			MySqlParameter[] array = new MySqlParameter[2]
			{
				new MySqlParameter("@MParentID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = countryId;
			array[1].Value = context.MLCID;
			return ModelInfoManager.GetDataModelBySql<BASProvinceModel>(context, sql, array);
		}
	}
}
