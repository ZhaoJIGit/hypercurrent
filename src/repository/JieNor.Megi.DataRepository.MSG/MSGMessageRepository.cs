using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Mail;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.MSG;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.DataRepository.MSG
{
	public class MSGMessageRepository : DataServiceT<MSGMessageModel>
	{
		public List<MSGMessageOrgUserModel> GetRelativeUserList(MContext ctx)
		{
			List<MSGMessageOrgUserModel> list = new List<MSGMessageOrgUserModel>();
			List<MSGMessageUserListModel> messageUserList = GetMessageUserList(ctx);
			if (messageUserList != null && messageUserList.Count > 0)
			{
				List<string> list2 = (from t in messageUserList
				select t.MOrgID).Distinct().ToList();
				foreach (string item in list2)
				{
					MSGMessageOrgUserModel mSGMessageOrgUserModel = new MSGMessageOrgUserModel();
					mSGMessageOrgUserModel.MOrgID = item;
					mSGMessageOrgUserModel.MOrgName = (from t in messageUserList
					where t.MOrgID == item
					select t.MOrgName).FirstOrDefault();
					mSGMessageOrgUserModel.MUserList = (from t in messageUserList
					where t.MOrgID == item
					select new MSGMessageUserModel
					{
						MUserID = t.MUserID,
						MUserName = MConverter.ToUserName(ctx.MLCID, t.MFirstName, t.MLastName),
						MEmail = t.MEmail
					}).ToList();
					list.Add(mSGMessageOrgUserModel);
				}
			}
			return list;
		}

		private List<MSGMessageUserListModel> GetMessageUserList(MContext ctx)
		{
			ctx.IsSys = true;
			string sql = "select o.MItemID AS MOrgID, o.MName AS MOrgName, u.MItemID as MUserID,u.MEmailAddress AS MEmail, ul.MFristName AS MFirstName,ul.MLastName \r\n                        from T_Bas_Organisation o\r\n                        inner join T_Sec_OrgUser ou on o.MItemID=ou.MOrgID and ou.MIsDelete = 0 and ou.MIsActive = 1\r\n                        inner join T_Sec_User u ON ou.MUserID=u.MItemID and u.MIsDelete = 0 and u.MIsActive = 1\r\n                        Left join T_Sec_User_L ul on u.MItemID = ul.MParentID and ul.MIsDelete = 0 and ul.MIsActive = 1\r\n                        where o.MIsDelete=0 and o.MIsActive=1 and u.MItemID<>@MUserID\r\n                        and exists(select 1 from T_Sec_OrgUser t where ou.MOrgID=t.MOrgID AND t.MUserID=@MUserID and t.MIsDelete = 0 )";
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@MLocaleID", ctx.MLCID),
				new MySqlParameter("@MUserID", ctx.MUserID)
			};
			return ModelInfoManager.GetDataModelBySql<MSGMessageUserListModel>(ctx, sql, cmdParms);
		}

		public MSGMessageViewModel GetMessageViewModel(string msgId, MContext ctx)
		{
			string sql = $"{GetMessageViewSql()}  WHERE msg.MID=@MID";
			MySqlParameter[] cmdParms = new MySqlParameter[1]
			{
				new MySqlParameter("@MID", msgId)
			};
			ctx.IsSys = true;
			List<MSGMessageViewModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<MSGMessageViewModel>(ctx, sql, cmdParms);
			if (dataModelBySql == null || dataModelBySql.Count == 0)
			{
				return null;
			}
			MSGMessageViewModel mSGMessageViewModel = dataModelBySql[0];
			mSGMessageViewModel.MGroupMessageList = GetGroupMessageList(msgId, ctx);
			return mSGMessageViewModel;
		}

		public List<MSGMessageListModel> GetGroupMessageList(string msgId, MContext ctx)
		{
			string msgGroupID = GetMsgGroupID(ctx, msgId);
			string sql = $"{GetMessageViewSql()} WHERE msg.MIsDelete=0 AND (msg.MID=@MGroupID OR MGroupID=@MGroupID) ORDER BY MCreateDate ";
			MySqlParameter[] cmdParms = new MySqlParameter[1]
			{
				new MySqlParameter("@MGroupID", msgGroupID)
			};
			List<MSGMessageListModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<MSGMessageListModel>(ctx, sql, cmdParms);
			if (dataModelBySql != null)
			{
				foreach (MSGMessageListModel item in dataModelBySql)
				{
					if (item.MSenderID.Equals(ctx.MUserID))
					{
						item.MIsSendByMe = true;
					}
				}
			}
			return dataModelBySql;
		}

		public OperationResult SendMessage(MContext ctx, MSGMessageEditModel msgModel)
		{
			string groupId = string.Empty;
			List<CommandInfo> list = new List<CommandInfo>();
			if (!string.IsNullOrEmpty(msgModel.MReplyID))
			{
				groupId = GetMsgGroupID(ctx, msgModel.MReplyID);
				list.AddRange(GetUpdateIsReplyCmd(ctx, msgModel.MReplyID));
			}
			list.AddRange(GetMessageInsertCmd(ctx, groupId, msgModel));
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num = dynamicDbHelperMySQL.ExecuteSqlTran(list);
			if (num > 0 && msgModel.MIsSendEmail)
			{
				List<string> emailList = (from t in msgModel.MReceiverList
				where !string.IsNullOrEmpty(t.MEmail)
				select t.MEmail).Distinct().ToList();
				SendMail.SendSMTPEMail(emailList, msgModel.MTitle, msgModel.MContent, "Megi", "");
			}
			OperationResult operationResult = new OperationResult();
			operationResult.Success = (num > 0);
			return operationResult;
		}

		private string GetMsgGroupID(MContext ctx, string msgId)
		{
			ctx.IsSys = true;
			string text = string.Empty;
			MSGMessageModel dataModel = base.GetDataModel(ctx, msgId, false);
			if (dataModel != null)
			{
				text = (string.IsNullOrEmpty(dataModel.MGroupID) ? dataModel.MID : dataModel.MGroupID);
			}
			return string.IsNullOrEmpty(text) ? msgId : text;
		}

		private List<CommandInfo> GetUpdateIsReplyCmd(MContext ctx, string msgId)
		{
			ctx.IsSys = true;
			MSGMessageEditModel mSGMessageEditModel = new MSGMessageEditModel();
			mSGMessageEditModel.MID = msgId;
			mSGMessageEditModel.MIsReply = true;
			return ModelInfoManager.GetInsertOrUpdateCmd<MSGMessageEditModel>(ctx, mSGMessageEditModel, new List<string>
			{
				"MReplyID"
			}, true);
		}

		private List<CommandInfo> GetMessageInsertCmd(MContext ctx, string groupId, MSGMessageEditModel msgModel)
		{
			ctx.IsSys = true;
			List<CommandInfo> list = new List<CommandInfo>();
			foreach (MSGMessageUserModel mReceiver in msgModel.MReceiverList)
			{
				MSGMessageModel mSGMessageModel = new MSGMessageModel();
				mSGMessageModel.MOrgID = ctx.MOrgID;
				mSGMessageModel.MGroupID = groupId;
				mSGMessageModel.MReceiverID = mReceiver.MUserID;
				mSGMessageModel.MSenderID = ctx.MUserID;
				mSGMessageModel.MTitle = msgModel.MTitle;
				mSGMessageModel.MContent = msgModel.MContent;
				List<CommandInfo> insertOrUpdateCmd = ModelInfoManager.GetInsertOrUpdateCmd<MSGMessageModel>(ctx, mSGMessageModel, null, true);
				list.AddRange(insertOrUpdateCmd);
			}
			return list;
		}

		public void ReadMessage(string msgId, MContext ctx)
		{
			ctx.IsSys = true;
			MSGMessageEditModel mSGMessageEditModel = new MSGMessageEditModel();
			mSGMessageEditModel.MID = msgId;
			mSGMessageEditModel.MIsRead = true;
			ModelInfoManager.InsertOrUpdate<MSGMessageEditModel>(ctx, mSGMessageEditModel, new List<string>
			{
				"MIsRead"
			});
		}

		public int GetReceiveMessageCount(MContext ctx)
		{
			string sQLString = "SELECT COUNT(*) FROM T_Msg_Message msg\r\n                        INNER JOIN T_Sec_User ru ON msg.MReceiverID=ru.MItemID and msg.MIsDelete = 0  WHERE msg.MIsDelete=0 AND msg.MIsRead=0 AND msg.MReceiverID=@MUserID";
			MySqlParameter[] cmdParms = new MySqlParameter[1]
			{
				new MySqlParameter("@MUserID", ctx.MUserID)
			};
			object single = DbHelperMySQL.GetSingle(sQLString, cmdParms);
			return Convert.ToInt32(single);
		}

		public DataGridJson<MSGMessageViewModel> GetReceiveMessageList(MContext ctx, MSGMessageListFilterModel filter)
		{
			string messageViewSql = GetMessageViewSql();
			filter.Equal("msg.MReceiverID", ctx.MUserID);
			return GetMessageList(ctx, filter, messageViewSql);
		}

		public DataGridJson<MSGMessageViewModel> GetSentMessageList(MContext ctx, MSGMessageListFilterModel filter)
		{
			string messageViewSql = GetMessageViewSql();
			filter.Equal("msg.MSenderID", ctx.MUserID);
			return GetMessageList(ctx, filter, messageViewSql);
		}

		private DataGridJson<MSGMessageViewModel> GetMessageList(MContext ctx, MSGMessageListFilterModel filter, string sql)
		{
			SqlQuery sqlQuery = new SqlQuery();
			sqlQuery.SelectString = sql;
			sqlQuery.SqlWhere = filter;
			sqlQuery.SqlWhere.Equal("msg.MIsDelete", 0);
			sqlQuery.OrderBy("MCreateDate Desc");
			DataGridJson<MSGMessageViewModel> pageDataModelListBySql = ModelInfoManager.GetPageDataModelListBySql<MSGMessageViewModel>(ctx, sqlQuery);
			if (pageDataModelListBySql == null)
			{
				return pageDataModelListBySql;
			}
			return pageDataModelListBySql;
		}

		private string GetMessageViewSql()
		{
			return "SELECT sul.MFristName AS MSenderFirstName,sul.MLastName as MSenderLastName,su.MEmailAddress AS MSenderEmail,\r\n                        rul.MFristName AS MReceiverFirstName,rul.MLastName as MReceiverLastName,ru.MEmailAddress AS MReceiverEmail,msg.* FROM T_Msg_Message msg\r\n                        INNER JOIN T_Sec_User su ON msg.MSenderID=su.MItemID and su.MIsDelete = 0 and su.MIsActive = 1\r\n                        INNER JOIN T_Sec_User ru ON msg.MReceiverID=ru.MItemID and ru.MIsDelete = 0 and ru.MIsActive = 1\r\n                        LEFT JOIN T_Sec_User_L sul ON su.MItemID=sul.MParentID and sul.MIsDelete = 0 and sul.MIsActive = 1\r\n                        LEFT JOIN T_Sec_User_L rul ON ru.MItemID=rul.MParentID and rul.MIsDelete = 0 and rul.MIsActive = 1 ";
		}
	}
}
