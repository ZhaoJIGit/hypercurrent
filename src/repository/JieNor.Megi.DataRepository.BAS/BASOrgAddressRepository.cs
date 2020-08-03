using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace JieNor.Megi.DataRepository.BAS
{
	public class BASOrgAddressRepository : DataServiceT<BASOrgAddressModel>
	{
		public static BASOrgAddressModel GetModel(MContext ctx, string orgId, string addrType, ParamBase param)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("select MItemID, MOrgID, MAddressType, MStreet, MTownCity, MStateRegion, MCountry, MAttention, MZipcode, MIsActive, MIsDelete, MCreatorID, MCreateDate, MModifierID, MModifyDate  ");
			stringBuilder.Append("  from T_Bas_OrgAddress ");
			stringBuilder.Append(" where MOrgID=@MOrgID AND MAddressType=@MAddressType and MIsDelete = 0 ");
			MySqlParameter[] array = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MAddressType", MySqlDbType.VarChar, 36)
			};
			array[0].Value = orgId;
			array[1].Value = addrType;
			BASOrgAddressModel bASOrgAddressModel = new BASOrgAddressModel();
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array);
			if (dataSet.Tables[0].Rows.Count > 0)
			{
				List<BASOrgAddressModel> list = ModelInfoManager.DataTableToList<BASOrgAddressModel>(dataSet.Tables[0]);
				return list[0];
			}
			return null;
		}
	}
}
