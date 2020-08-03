using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace JieNor.Megi.DataRepository.BAS
{
	public class BASOrgContactRepository : DataServiceT<BASOrgContactModel>
	{
		public static BASOrgContactModel GetModelByOrgID(string orgId)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("select MItemID, MOrgID, MTelephone, MFax, MMobile, MDDI, MEmail, MWebsite, MSkype, MTwitter, MLinkedIn, MFacebook, MGooglePlus, MTBShop, MQQ, MWeChat, MWeibo, MIsActive, MIsDelete, MCreatorID, MCreateDate, MModifierID, MModifyDate  ");
			stringBuilder.Append("  from T_Bas_OrgContact ");
			stringBuilder.Append(" where MOrgID=@MOrgID and MIsDelete = 0 ");
			MySqlParameter[] array = new MySqlParameter[1]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = orgId;
			BASOrgContactModel bASOrgContactModel = new BASOrgContactModel();
			DataSet dataSet = DbHelperMySQL.Query(stringBuilder.ToString(), array);
			if (dataSet.Tables[0].Rows.Count > 0)
			{
				List<BASOrgContactModel> list = ModelInfoManager.DataTableToList<BASOrgContactModel>(dataSet.Tables[0]);
				return list[0];
			}
			return null;
		}
	}
}
