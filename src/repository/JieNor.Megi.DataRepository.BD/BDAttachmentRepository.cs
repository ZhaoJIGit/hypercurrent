using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace JieNor.Megi.DataRepository.BD
{
	public class BDAttachmentRepository : DataServiceT<BDAttachmentModel>
	{
		private IDictionary<string, string> BizObjectTableMapping
		{
			get
			{
				IDictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("Invoice", "T_IV_InvoiceAttachment");
				dictionary.Add("Bill", "T_IV_InvoiceAttachment");
				dictionary.Add("Payment", "T_IV_PaymentAttachment");
				dictionary.Add("Receive", "T_IV_ReceiveAttachment");
				dictionary.Add("Expense", "T_IV_ExpenseAttachment");
				dictionary.Add("Voucher", "T_GL_VoucherAttachment");
				return dictionary;
			}
		}

		public List<BDAttachmentCategoryListModel> GetCategoryList(MContext ctx)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("SELECT T.MItemID,T.MCategoryName,Count(M.MCategoryID) AS AttachmentCount,T.MBizObject FROM T_BD_AttachmentCategory T\r\n\t\t                    LEFT JOIN (SELECT MCategoryID FROM T_BD_Attachment WHERE MIsDelete=0 AND MIsActive=1 and MOrgID = @MOrgID) M ON T.MItemID=M.MCategoryID\r\n                                WHERE T.MIsDelete=0  AND T.MOrgID=@MOrgID");
			if (ctx.MOrgVersionID == 1)
			{
				stringBuilder.Append(" and (T.MBizObject=@MBizObject  or  IFNULL(T.MBizObject,'')='') ");
			}
			stringBuilder.Append(" GROUP BY T.MItemID \r\n                        ORDER BY T.MCategoryName;");
			MySqlParameter[] array = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MBizObject", "Voucher")
			};
			array[0].Value = ctx.MOrgID;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array);
			if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return null;
			}
			return ModelInfoManager.DataTableToList<BDAttachmentCategoryListModel>(dataSet.Tables[0]);
		}

		public void CheckCategoryNameExist(MContext ctx, BDAttachmentCategoryModel model, OperationResult result)
		{
			string text = "select count(*) from T_BD_AttachmentCategory where MOrgID=@MOrgID  and MIsDelete=0 and MCategoryName=@MCategoryName";
			if (!string.IsNullOrWhiteSpace(model.MItemID))
			{
				text += " and MItemID<>@MItemID";
			}
			MySqlParameter[] cmdParms = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MCategoryName", model.MCategoryName),
				new MySqlParameter("@MItemID", model.MItemID)
			};
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			object single = dynamicDbHelperMySQL.GetSingle(text, cmdParms);
			int num = 0;
			if (int.TryParse(Convert.ToString(single), out num) && num > 0)
			{
				string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Docs, "CategoryNameExist", "The folder name already exists, please use a another one!");
				result.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = text2
				});
			}
		}

		public OperationResult UpdateCategoryModel(MContext ctx, BDAttachmentCategoryModel model)
		{
			OperationResult operationResult = new OperationResult();
			if (string.IsNullOrWhiteSpace(model.MItemID))
			{
				CheckCategoryNameExist(ctx, model, operationResult);
				if (!operationResult.Success)
				{
					return operationResult;
				}
				operationResult = ModelInfoManager.InsertOrUpdate<BDAttachmentCategoryModel>(ctx, model, null);
			}
			else
			{
				BDAttachmentCategoryModel dataEditModel = ModelInfoManager.GetDataEditModel<BDAttachmentCategoryModel>(ctx, model.MItemID, false, true);
				model.MBizObject = dataEditModel.MBizObject;
				if (model.IsDelete)
				{
					operationResult = BDRepository.IsCanDelete(ctx, "Attachment_Category", model.MItemID);
					if (operationResult.Success)
					{
						BDAttachmentListFilterModel bDAttachmentListFilterModel = new BDAttachmentListFilterModel();
						bDAttachmentListFilterModel.categoryId = model.MItemID;
						bDAttachmentListFilterModel.PageSize = 2147483647;
						List<BDAttachmentListModel> rows = GetAttachmentList(ctx, bDAttachmentListFilterModel).rows;
						if (rows?.Any() ?? false)
						{
							ParamBase paramBase = new ParamBase();
							paramBase.KeyIDs = string.Join(",", from f in rows
							select f.MItemID);
							operationResult = DeleteAttachmentList(ctx, paramBase);
							if (!operationResult.Success)
							{
								return operationResult;
							}
						}
						model.MIsDelete = true;
						ModelInfoManager.DeleteFlag<BDAttachmentCategoryModel>(ctx, model.PKFieldValue);
					}
				}
				else
				{
					CheckCategoryNameExist(ctx, model, operationResult);
					if (!operationResult.Success)
					{
						return operationResult;
					}
					operationResult = ModelInfoManager.InsertOrUpdate<BDAttachmentCategoryModel>(ctx, model, null);
				}
			}
			operationResult.ObjectID = model.MItemID;
			return operationResult;
		}

		public string GetAssociateTargetCategoryId(MContext ctx, string bizObject)
		{
			string empty = string.Empty;
			string sql = "SELECT * from T_BD_AttachmentCategory where MOrgID=@MOrgID AND MIsDelete=0 ";
			MySqlParameter[] array = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MBizObject", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = bizObject;
			List<BDAttachmentCategoryModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<BDAttachmentCategoryModel>(ctx, sql, array);
			BDAttachmentCategoryModel bDAttachmentCategoryModel = dataModelBySql.FirstOrDefault((BDAttachmentCategoryModel f) => f.MBizObject == bizObject);
			if (bDAttachmentCategoryModel == null)
			{
				bDAttachmentCategoryModel = dataModelBySql.FirstOrDefault((BDAttachmentCategoryModel f) => f.MCategoryName == bizObject);
				if (bDAttachmentCategoryModel != null)
				{
					bDAttachmentCategoryModel.MBizObject = bizObject;
				}
				else
				{
					bDAttachmentCategoryModel = new BDAttachmentCategoryModel
					{
						MOrgID = ctx.MOrgID,
						MBizObject = bizObject,
						MCategoryName = bizObject
					};
				}
				OperationResult operationResult = UpdateCategoryModel(ctx, bDAttachmentCategoryModel);
				return operationResult.ObjectID;
			}
			return bDAttachmentCategoryModel.MItemID;
		}

		public static DataGridJson<BDAttachmentListModel> GetAttachmentList(MContext ctx, BDAttachmentListFilterModel filter)
		{
			SqlQuery sqlQuery = new SqlQuery();
			sqlQuery.SqlWhere = filter;
			sqlQuery.SelectString = "SELECT T.MItemID,T.MOrgID,T.MCategoryID,T.MName,T.MSize,T.MPath,\r\n                                    T.MCreatorID,T.MCreateDate,\r\n                                    DATE_FORMAT(T.MCreateDate,'%Y/%c/%d %H:%i') as MCreateDateFormated,\r\n                                    F_GetUserName(P.MFristName,P.MLastName) AS MCreatorName \r\n                                    FROM T_BD_Attachment T\r\n                                    LEFT JOIN T_Sec_User_L P ON T.MCreatorID=P.MParentID and P.MIsDelete = 0 and P.MIsActive = 1";
			sqlQuery.SqlWhere.Equal("T.MIsDelete", 0);
			sqlQuery.SqlWhere.Equal("T.MOrgID", ctx.MOrgID);
			sqlQuery.SqlWhere.Equal("T.MCategoryID", filter.categoryId);
			sqlQuery.GroupBy("T.MItemID");
			if (string.IsNullOrEmpty(filter.Sort))
			{
				sqlQuery.OrderBy(" T.MCreateDate DESC");
			}
			else if (filter.Sort == "MCreateDateFormated")
			{
				sqlQuery.OrderBy("T.MCreateDate " + filter.Order);
			}
			else
			{
				sqlQuery.OrderBy($"{filter.Sort} {filter.Order}");
			}
			return ModelInfoManager.GetPageDataModelListBySql<BDAttachmentListModel>(ctx, sqlQuery);
		}

		public List<BDAttachmentListModel> GetRelatedAttachmentList(string bizObject, string bizObjectID, string attachIds, MContext ctx)
		{
			List<BDAttachmentListModel> result = new List<BDAttachmentListModel>();
			if (string.IsNullOrWhiteSpace(bizObjectID) && string.IsNullOrWhiteSpace(attachIds))
			{
				return result;
			}
			string arg = string.Empty;
			string value = string.Empty;
			MySqlParameter[] cmdParms = null;
			string arg2 = string.Empty;
			if (!string.IsNullOrWhiteSpace(attachIds))
			{
				arg2 = base.GetWhereInSql(attachIds, ref cmdParms, null);
			}
			if (!string.IsNullOrWhiteSpace(bizObjectID))
			{
				arg = ",R.MID as RelationID ";
				value = $" INNER JOIN (SELECT * FROM {BizObjectTableMapping[bizObject]} WHERE MParentID='{bizObjectID}' and MOrgID = '{ctx.MOrgID}' AND MIsActive=1 AND MIsDelete=0) R ON T.MItemID=R.MAttachID and T.MOrgID = R.MOrgID ";
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("SELECT T.MItemID,T.MOrgID,T.MCategoryID,T.MName,T.MSize,T.MPath,T.MCreatorID,T.MCreateDate,DATE_FORMAT(T.MCreateDate,'%Y/%c/%d %H:%i') as MCreateDateFormated,");
			stringBuilder.AppendFormat("F_GetUserName(P.MFristName,P.MLastName) AS MCreatorName{0} FROM T_BD_Attachment T ", arg);
			stringBuilder.Append(" INNER JOIN T_Sec_User_L P ON T.MCreatorID=P.MParentID and  P.MIsDelete = 0 and P.MIsActive = 1 ");
			stringBuilder.Append(value);
			stringBuilder.Append(" WHERE T.MIsDelete=0 and T.MOrgID = '" + ctx.MOrgID + "'");
			if (!string.IsNullOrWhiteSpace(attachIds))
			{
				stringBuilder.AppendFormat(" AND T.MItemID in ({0})", arg2);
			}
			stringBuilder.Append(" GROUP BY T.MItemID");
			stringBuilder.Append(" ORDER BY T.MCreateDate DESC");
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), cmdParms);
			if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return result;
			}
			return ModelInfoManager.DataTableToList<BDAttachmentListModel>(dataSet.Tables[0]);
		}

		public List<BDAttachmentListModel> GetAttachmentListByIds(string attachIds, MContext ctx)
		{
			MySqlParameter[] cmdParms = null;
			string whereInSql = base.GetWhereInSql(attachIds, ref cmdParms, null);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("SELECT T.MItemID,T.MOrgID,T.MCategoryID,T.MName,T.MSize,T.MPath,T.MCreatorID,T.MCreateDate,");
			stringBuilder.Append("DATE_FORMAT(T.MCreateDate,'%Y/%c/%d %H:%i') as MCreateDateFormated,F_GetUserName(P.MFristName,P.MLastName) AS MCreatorName FROM T_BD_Attachment T ");
			stringBuilder.Append(" INNER JOIN T_Sec_User_L P ON T.MCreatorID=P.MParentID and  P.MIsDelete = 0 and P.MIsActive = 1 ");
			stringBuilder.AppendFormat(" WHERE T.MOrgID = '" + ctx.MOrgID + "' AND T.MItemID in ({0}) and T.MIsDelete = 0  GROUP BY T.MItemID ORDER BY T.MCreateDate DESC", whereInSql);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), cmdParms);
			List<BDAttachmentListModel> result = new List<BDAttachmentListModel>();
			if (dataSet == null || dataSet.Tables.Count == 0)
			{
				return result;
			}
			DataTable dataTable = dataSet.Tables[0];
			if (dataTable.Rows.Count == 0)
			{
				return result;
			}
			return ModelInfoManager.DataTableToList<BDAttachmentListModel>(dataTable);
		}

		public BDAttachmentModel GetAttachmentModel(string attachId, MContext ctx, bool includeDelete = false)
		{
			return ModelInfoManager.GetDataEditModel<BDAttachmentModel>(ctx, attachId, includeDelete, true);
		}

		public BDAttachmentModel GetAttachmentModelById(string attachId, MContext ctx)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("select * from T_BD_Attachment where MItemID=@itemId and MOrgID = @MOrgID and MIsDelete = 0 ");
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@itemId", attachId),
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return ModelInfoManager.GetFirstOrDefaultModel<BDAttachmentModel>(dynamicDbHelperMySQL.Query(stringBuilder.ToString(), cmdParms));
		}

		public string UpdateAttachmentModel(MContext ctx, BDAttachmentModel model)
		{
			ModelInfoManager.InsertOrUpdate<BDAttachmentModel>(ctx, model, null);
			return model.MItemID;
		}

		public OperationResult DeleteAttachmentList(MContext ctx, ParamBase param)
		{
			OperationResult operationResult = new OperationResult();
			List<string> list = CheckDeletingAttachUsed(ctx, param.KeyIDs, null);
			if (list?.Any() ?? false)
			{
				List<BDAttachmentListModel> attachmentListByIds = GetAttachmentListByIds(string.Join(",", list), ctx);
				List<string> values = (from f in attachmentListByIds
				select f.MName).ToList();
				string message = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Docs, "DeletingFileHasUsed", "The files {0} has been used."), string.Join(",", values));
				operationResult.VerificationInfor = new List<BizVerificationInfor>();
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					CheckItem = "附件引用",
					Level = AlertEnum.Error,
					Message = message
				});
				return operationResult;
			}
			ModelInfoManager.DeleteFlag<BDAttachmentModel>(ctx, param.KeyIDs.Split(',').ToList());
			return operationResult;
		}

		public List<CommandInfo> GetDeleteAttachmentCmdList(MContext ctx, List<string> idList)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			if (idList == null || idList.Count() == 0)
			{
				return list;
			}
			list.AddRange(ModelInfoManager.GetDeleteFlagCmd<BDAttachmentModel>(ctx, idList));
			return list;
		}

		public bool MoveAttachmentListTo(MContext ctx, ParamBase param)
		{
			MySqlParameter[] cmdParms = new MySqlParameter[3]
			{
				new MySqlParameter("@MOperationID", param.MOperationID),
				new MySqlParameter("@MUserID", ctx.MUserID),
				new MySqlParameter("@MModifyDate", ctx.DateNow.ToString("yyyy-MM-dd HH:mm:ss"))
			};
			string whereInSql = base.GetWhereInSql(param.KeyIDSWithSingleQuote, ref cmdParms, null);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("update T_BD_Attachment ");
			stringBuilder.Append("set MCategoryID = @MOperationID, MModifierID =@MUserID, MModifyDate=@MModifyDate");
			stringBuilder.AppendFormat(" where MItemID in ({0}) and MOrgID = '{1}' ", whereInSql, ctx.MOrgID);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num = dynamicDbHelperMySQL.ExecuteSql(stringBuilder.ToString(), cmdParms);
			return num > 0;
		}

		public void CreateAttachmentAssociation(MContext ctx, string bizObject, ParamBase param)
		{
			if (!string.IsNullOrWhiteSpace(param.KeyIDs))
			{
				List<string> list = (from t in param.KeyIDSWithNoSingleQuote.Trim(',').Split(',')
				where !string.IsNullOrWhiteSpace(t)
				select t).ToList();
				if (list.Count > 0)
				{
					List<CommandInfo> list2 = new List<CommandInfo>();
					string sqlText = "INSERT INTO " + BizObjectTableMapping[bizObject] + "(MID,MOrgID,MParentID,MAttachID,MIsActive,MIsDelete,MCreatorID,MCreateDate,MModifierID,MModifyDate) values(@MID,@MOrgID,@MParentID,@MAttachID,1,0,@UserID,@DateNow,@UserID,@DateNow); ";
					foreach (string item2 in list)
					{
						MySqlParameter[] para = new MySqlParameter[6]
						{
							new MySqlParameter("@MID", UUIDHelper.GetGuid()),
							new MySqlParameter("@MOrgID", ctx.MOrgID),
							new MySqlParameter("@MParentID", param.MOperationID),
							new MySqlParameter("@MAttachID", item2),
							new MySqlParameter("@UserID", ctx.MUserID),
							new MySqlParameter("@DateNow", ctx.DateNow)
						};
						CommandInfo item = new CommandInfo(sqlText, para);
						list2.Add(item);
						if (!string.IsNullOrEmpty(param.MOperationID))
						{
							BDAttachmentModel attachmentModel = GetAttachmentModel(item2, ctx, false);
							if (attachmentModel != null)
							{
								list2.Add(CommonLogHelper.GetUploadAttachmentLogCmd(ctx, param.MOperationID, attachmentModel.MName));
							}
						}
					}
					DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
					dynamicDbHelperMySQL.ExecuteSqlTran(list2);
				}
			}
		}

		public OperationResult RemoveAttachmentAssociation(MContext ctx, string bizObject, ParamBase param)
		{
			string attachmentBillID = GetAttachmentBillID(ctx, param.MOperationID, bizObject);
			OperationResult operationResult = new OperationResult();
			bool flag = param.MType == "Delete";
			List<CommandInfo> list = new List<CommandInfo>();
			if (!string.IsNullOrWhiteSpace(param.KeyIDs))
			{
				CommandInfo commandInfo = new CommandInfo();
				commandInfo.CommandText = "update " + BizObjectTableMapping[bizObject] + " set MIsDelete = 1 WHERE MID=@MID and MOrgID = @MOrgID and MIsDelete = 0 ";
				DbParameter[] array = commandInfo.Parameters = new MySqlParameter[2]
				{
					new MySqlParameter("@MID", param.KeyIDSWithNoSingleQuote),
					new MySqlParameter("@MOrgID", ctx.MOrgID)
				};
				list.Add(commandInfo);
			}
			if (!string.IsNullOrWhiteSpace(param.MOperationID) & flag)
			{
				ParamBase paramBase = new ParamBase();
				paramBase.KeyIDs = param.MOperationID;
				List<string> list2 = CheckDeletingAttachUsed(ctx, paramBase.KeyIDs, param.KeyIDSWithNoSingleQuote);
				if (list2?.Any() ?? false)
				{
					List<BDAttachmentListModel> attachmentListByIds = GetAttachmentListByIds(string.Join(",", list2), ctx);
					List<string> values = (from f in attachmentListByIds
					select f.MName).ToList();
					string message = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Docs, "DeletingFileHasUsed", "The files {0} has been used."), string.Join(",", values));
					operationResult.VerificationInfor = new List<BizVerificationInfor>();
					operationResult.VerificationInfor.Add(new BizVerificationInfor
					{
						CheckItem = "附件引用",
						Level = AlertEnum.Error,
						Message = message
					});
					return operationResult;
				}
				List<string> idList = paramBase.KeyIDs.Split(',').ToList();
				list.AddRange(GetDeleteAttachmentCmdList(ctx, idList));
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num = dynamicDbHelperMySQL.ExecuteSqlTran(list);
			operationResult.Success = (num > 0);
			BDAttachmentModel attachmentModel = GetAttachmentModel(param.MOperationID, ctx, flag);
			if (attachmentModel != null && !string.IsNullOrEmpty(attachmentBillID))
			{
				if (flag)
				{
					CommonLogHelper.DeleteAttachmentLog(ctx, attachmentBillID, attachmentModel.MName);
				}
				else
				{
					CommonLogHelper.RemoveAttachmentLog(ctx, attachmentBillID, attachmentModel.MName);
				}
			}
			return operationResult;
		}

		private string GetAttachmentBillID(MContext ctx, string attachId, string bizObject)
		{
			string sql = $"SELECT MParentID FROM {BizObjectTableMapping[bizObject]} WHERE MAttachID = @MAttachID  and MOrgID = @MOrgID and MIsDelete = 0  ";
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MAttachID", attachId)
			};
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			object single = dynamicDbHelperMySQL.GetSingle(sql, cmdParms);
			if (single == null || single == DBNull.Value)
			{
				return string.Empty;
			}
			return single.ToString();
		}

		private List<string> CheckDeletingAttachUsed(MContext ctx, string deletingAttachIds, string relationMID = null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("SELECT MAttachID FROM T_IV_InvoiceAttachment where MOrgID = @MOrgID and MIsDelete = 0 ");
			if (!string.IsNullOrWhiteSpace(relationMID))
			{
				stringBuilder.AppendLine(" and  MID<>@MID  ");
			}
			stringBuilder.AppendLine(" UNION ALL SELECT MAttachID FROM T_IV_PaymentAttachment where MOrgID = @MOrgID and MIsDelete = 0 ");
			if (!string.IsNullOrWhiteSpace(relationMID))
			{
				stringBuilder.AppendLine(" and  MID<>@MID  ");
			}
			stringBuilder.AppendLine(" UNION ALL SELECT MAttachID FROM T_IV_ReceiveAttachment where MOrgID = @MOrgID and MIsDelete = 0 ");
			if (!string.IsNullOrWhiteSpace(relationMID))
			{
				stringBuilder.AppendLine(" and  MID<>@MID  ");
			}
			stringBuilder.AppendLine(" UNION ALL SELECT MAttachID FROM T_IV_ExpenseAttachment where MOrgID = @MOrgID and MIsDelete = 0 ");
			if (!string.IsNullOrWhiteSpace(relationMID))
			{
				stringBuilder.AppendLine(" and  MID<>@MID  ");
			}
			stringBuilder.AppendLine(" UNION ALL SELECT MAttachID FROM T_GL_VoucherAttachment where MOrgID = @MOrgID and MIsDelete = 0  ");
			if (!string.IsNullOrWhiteSpace(relationMID))
			{
				stringBuilder.AppendLine(" and  MID<>@MID  ");
			}
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MID", relationMID)
			};
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), cmdParms);
			if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return new List<string>();
			}
			List<IVInvoiceAttachmentModel> source = ModelInfoManager.DataTableToList<IVInvoiceAttachmentModel>(dataSet.Tables[0]);
			List<string> first = (from f in source
			select f.MAttachID).ToList();
			return first.Intersect(deletingAttachIds.Split(',').ToList()).ToList();
		}
	}
}
