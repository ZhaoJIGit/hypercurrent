using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Data.Common;
using System.Text;

namespace JieNor.Megi.DataRepository.SEC
{
    public class SECSendLinkInfoRepository : DataServiceT<SECSendLinkInfoModel>
    {
        public static bool IsValidLink(string itemid)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("select 1 from T_Sec_SendLinkInfo");
            stringBuilder.Append(" where MItemID=@MItemID and MIsDelete=0");
            MySqlParameter[] array = new MySqlParameter[2]
            {
                new MySqlParameter("@MItemID", MySqlDbType.VarChar, 36),
                new MySqlParameter("@DateNow", MySqlDbType.DateTime)
            };
            array[0].Value = itemid;
            array[1].Value = DateTime.Now;
            return DbHelperMySQL.Exists(stringBuilder.ToString(), array);
        }

        public static void InsertLink(string itemid, string email, DateTime time, string phone, string firstName, string lastName, int linkType, string invitationEmail, string invitationOrgID, string planCode = "")
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("insert into T_Sec_SendLinkInfo(MItemID,MExpireDate , MEmail , MPhone , MFirstName,MLastName, MSendDate , MLinkType,MInvitationEmail,MInvitationOrgID)");
            stringBuilder.AppendLine("values (@MItemID,@MExpireDate,@MEmail  , @MPhone , @MFirstName,@MLastName ,@MSendDate,@MLinkType,@MInvitationEmail,@MInvitationOrgID)");
            MySqlParameter[] array = new MySqlParameter[10]
            {
                new MySqlParameter("@MItemID", MySqlDbType.VarChar, 36),
                new MySqlParameter("@MExpireDate", MySqlDbType.DateTime),
                new MySqlParameter("@MEmail", MySqlDbType.VarChar),
                new MySqlParameter("@MSendDate", MySqlDbType.DateTime),
                new MySqlParameter("@MPhone", MySqlDbType.VarChar, 36),
                new MySqlParameter("@MFirstName", MySqlDbType.VarChar, 36),
                new MySqlParameter("@MLastName", MySqlDbType.VarChar, 36),
                new MySqlParameter("@MLinkType", MySqlDbType.Int32),
                new MySqlParameter("@MInvitationEmail", MySqlDbType.VarChar, 100),
                new MySqlParameter("@MInvitationOrgID", MySqlDbType.VarChar, 36)
            };
            array[0].Value = itemid;
            array[1].Value = DateTime.Now.AddDays(1.0);
            array[2].Value = email;
            array[3].Value = time;
            array[4].Value = (string.IsNullOrWhiteSpace(phone) ? "" : phone);
            array[5].Value = (string.IsNullOrWhiteSpace(firstName) ? "" : firstName);
            array[6].Value = (string.IsNullOrWhiteSpace(lastName) ? "" : lastName);
            array[7].Value = linkType;
            array[8].Value = invitationEmail;
            array[9].Value = invitationOrgID;
            DbHelperMySQL.ExecuteSql(new MContext(), stringBuilder.ToString(), array);
            if (!string.IsNullOrWhiteSpace(planCode))
            {
                DataSet ds= DbHelperMySQL.Query("SELECT id FROM t_bas_plan where code='"+ planCode + "';");
                if (ds.Tables.Count>0) {
                    if (ds.Tables[0].Rows.Count>0) {
                        StringBuilder planBuilder = new StringBuilder();
                        planBuilder.AppendLine("insert into t_bas_planuser (planid,useremail)values(" +int.Parse( ds.Tables[0].Rows[0][0].ToString())+ ",'"+email+"');");
                        DbHelperMySQL.ExecuteSql(planBuilder.ToString());
                    }
                }
            }

        }

        public static void DeleteLink(string itemid)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(" update T_Sec_SendLinkInfo set MIsDelete=1 where MItemID=@MItemID");
            MySqlParameter[] array = new MySqlParameter[1]
            {
                new MySqlParameter("@MItemID", MySqlDbType.VarChar, 36)
            };
            array[0].Value = itemid;
            DbHelperMySQL.ExecuteSql(new MContext(), stringBuilder.ToString(), array);
        }

        public static CommandInfo GetDeleteJoinLink(string mitemId)
        {
            CommandInfo commandInfo = new CommandInfo();
            commandInfo.CommandText = "update T_Sec_SendLinkInfo set MIsDelete=1 where MItemID=@MItemID";
            MySqlParameter[] array = new MySqlParameter[1]
            {
                new MySqlParameter("@MItemID", MySqlDbType.VarChar, 36)
            };
            array[0].Value = mitemId;
            DbParameter[] array2 = commandInfo.Parameters = array;
            return commandInfo;
        }

        public static SECSendLinkInfoModel GetModel(string itemId)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("select * from T_Sec_SendLinkInfo where MItemID=@MItemID and MIsDelete=0");
            MySqlParameter[] array = new MySqlParameter[1]
            {
                new MySqlParameter("@MItemID", MySqlDbType.VarChar, 36)
            };
            array[0].Value = itemId;
            DataSet dataSet = DbHelperMySQL.Query(stringBuilder.ToString(), array);
            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
            {
                DataRow dataRow = dataSet.Tables[0].Rows[0];
                DataTable dataTable = dataSet.Tables[0];
                SECSendLinkInfoModel sECSendLinkInfoModel = new SECSendLinkInfoModel();
                sECSendLinkInfoModel.MItemID = Convert.ToString(dataRow["MItemID"]);
                sECSendLinkInfoModel.MEmail = Convert.ToString(dataRow["MEmail"]);
                sECSendLinkInfoModel.MPhone = Convert.ToString(dataRow["MPhone"]);
                sECSendLinkInfoModel.MFirstName = Convert.ToString(dataRow["MFirstName"]);
                sECSendLinkInfoModel.MLastName = Convert.ToString(dataRow["MLastName"]);
                sECSendLinkInfoModel.MInvitationOrgID = Convert.ToString(dataRow["MInvitationOrgID"]);
                sECSendLinkInfoModel.MInvitationEmail = Convert.ToString(dataRow["MInvitationEmail"]);
                sECSendLinkInfoModel.MSendDate = Convert.ToDateTime(dataRow["MSendDate"]);
                sECSendLinkInfoModel.MLinkType = Convert.ToInt32(dataRow["MLinkType"]);
                sECSendLinkInfoModel.MExpireDate = Convert.ToDateTime(dataRow["MExpireDate"]);
                return sECSendLinkInfoModel;
            }
            return null;
        }
    }
}
