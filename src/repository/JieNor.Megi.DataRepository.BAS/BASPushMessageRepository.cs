using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace JieNor.Megi.DataRepository.BAS
{
	public class BASPushMessageRepository
	{
		public List<BASPushMessageModel> GetMessageResult(MContext ctx, PushMessageEnum messageType)
		{
			List<BASPushMessageModel> result = new List<BASPushMessageModel>();
			if (messageType == PushMessageEnum.SystemUpdate)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(" select * from T_Bas_PushMessage a");
				stringBuilder.Append(" inner join T_Bas_PushMessage_L l on l.MParentID = a.MItemID  and l.MIsDelete = 0 ");
				stringBuilder.Append("where a.MMessageType=@MessageType and a.MIsDelete = 0 ");
				MySqlParameter[] array = new MySqlParameter[1]
				{
					new MySqlParameter("@MessageType", MySqlDbType.VarChar, 2)
				};
				MySqlParameter obj = array[0];
				int num = (int)messageType;
				obj.Value = num.ToString();
				DataSet dataSet = DbHelperMySQL.Query(stringBuilder.ToString(), array);
				if (dataSet.Tables[0].Rows.Count > 0)
				{
					result = ModelInfoManager.DataTableToList<BASPushMessageModel>(dataSet.Tables[0]);
				}
			}
			return result;
		}

		public bool UpdateCompleteCount(BASPushMessageModel model)
		{
			//bool flag = false;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(" update T_Bas_PushMessage set MCompleteCount=@count, MLastPushDate=@MLastPushDate where MItemID=@MItemID and MIsDelete = 0 ");
			MySqlParameter[] array = new MySqlParameter[3]
			{
				new MySqlParameter("@count", MySqlDbType.Int32),
				new MySqlParameter("@MItemID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLastPushDate", MySqlDbType.DateTime)
			};
			array[0].Value = model.MCompleteCount;
			array[1].Value = model.MItemID;
			array[2].Value = model.MLastPushDate;
			int num = DbHelperMySQL.ExecuteSql(new MContext(), stringBuilder.ToString(), array);
			return num > 0 && true;
		}
	}
}
