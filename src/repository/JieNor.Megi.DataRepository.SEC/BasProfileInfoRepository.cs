using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace JieNor.Megi.DataRepository.SEC
{
	public class BasProfileInfoRepository : DataServiceT<SECUserlModel>
	{
		public SECUserlModel GetModelByKey(string MParentID, MContext ctx)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("select MPKID,MParentID, MPublicProfile, MProfileImage, MFristName, MLastName, MJobTitle, MBriefBio ");
			stringBuilder.Append("  from T_Sec_User_L ");
			stringBuilder.Append(" where MParentID=@MParentID ");
			MySqlParameter[] array = new MySqlParameter[1]
			{
				new MySqlParameter("@MParentID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = MParentID;
			SECUserlModel sECUserlModel = new SECUserlModel();
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array);
			if (dataSet.Tables[0].Rows.Count > 0)
			{
				List<SECUserlModel> list = ModelInfoManager.DataTableToList<SECUserlModel>(dataSet.Tables[0]);
				return list[0];
			}
			return null;
		}

		public SECUserlModel GetModelByEmail(MContext ctx, string email)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("select MPKID,MParentID, MPublicProfile, MProfileImage, MFristName, MLastName, MJobTitle, MBriefBio ");
			stringBuilder.Append("  from T_Sec_User_L a ");
			stringBuilder.Append(" inner join t_sec_user b on b.MItemID = a.MParentID and b.MEmailAddress=@MEmail and (a.MFristName is not null and a.MLastName is not null) and MIsTemp=0 ");
			MySqlParameter[] array = new MySqlParameter[1]
			{
				new MySqlParameter("@MEmail", MySqlDbType.VarChar, 36)
			};
			array[0].Value = email;
			SECUserlModel sECUserlModel = new SECUserlModel();
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array);
			List<SECUserlModel> list = ModelInfoManager.DataTableToList<SECUserlModel>(dataSet.Tables[0]);
			return list[0];
		}

		public List<SECUserlModel> GetList(string strWhere, MContext ctx)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("SELECT * ");
			stringBuilder.Append(" FROM T_Sec_User_L ");
			stringBuilder.Append("WHERE MIsDelete = 0 ");
			if (strWhere.Trim() != "")
			{
				stringBuilder.Append(" AND " + strWhere);
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return ModelInfoManager.DataTableToList<SECUserlModel>(dynamicDbHelperMySQL.Query(stringBuilder.ToString()).Tables[0]);
		}

		public DataSet GetList(int Top, string strWhere, string filedOrder, MContext ctx)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("select ");
			if (Top > 0)
			{
				stringBuilder.Append(" top " + Top.ToString());
			}
			stringBuilder.Append(" * ");
			stringBuilder.Append(" FROM T_Sec_User_L ");
			if (strWhere.Trim() != "")
			{
				stringBuilder.Append(" where " + strWhere);
			}
			stringBuilder.Append(" order by " + filedOrder);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return dynamicDbHelperMySQL.Query(stringBuilder.ToString());
		}

		public OperationResult Update(MContext ctx, SECUserlModel model)
		{
			OperationResult operationResult = new OperationResult();
			string text = "update  t_sec_user_l set MFristName=@MFirstName ,  MLastName=@MLastName , MJobTitle=@MJobTitle, MBriefBio=@MBriefBio  where MParentID=@MParentID";
			MySqlParameter[] array = new MySqlParameter[5]
			{
				new MySqlParameter("@MFirstName", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLastName", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MJobTitle", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MBriefBio", MySqlDbType.VarChar, 255),
				new MySqlParameter("@MParentID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = model.MFristName;
			array[1].Value = model.MLastName;
			array[2].Value = model.MJobTitle;
			array[3].Value = model.MBriefBio;
			array[4].Value = model.MParentID;
			int num = DbHelperMySQL.ExecuteSql(ctx, text, array);
			if (num > 0)
			{
				DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
				dynamicDbHelperMySQL.ExecuteSql(text, array);
				operationResult.Success = true;
			}
			else
			{
				operationResult.Success = false;
			}
			return operationResult;
		}
	}
}
