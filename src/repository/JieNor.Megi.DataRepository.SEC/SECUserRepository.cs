using JieNor.Megi.Common.Encrypt;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.DataRepository.API;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.EntityModel.SEC;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace JieNor.Megi.DataRepository.SEC
{
	public class SECUserRepository : DataServiceT<SECUserModel>
	{
		public string CommonSelect = $"SELECT \n                t1.MItemID,\n                t1.MItemID AS MUserID,\n                t1.MEmailAddress,\n                t1.MModifyDate,\n                t2.MFristName AS MFirstName,\n                t2.MLastName,\n                t3.MPosition as Position,\n                t3.MRole,\n                (case when t3.MUserIsActive=0 then 'PENDING' else (case when t3.MIsArchive=1 then 'DISABLED' else 'ACTIVE' end) end) as MStatus\n            FROM\n                t_sec_user t1\n                    JOIN\n                ({SECPermissionRepository.GetUserMultiSql()}) t2 ON t1.MItemID = t2.MParentID\n                    JOIN\n                T_Sec_OrgUser t3 ON t3.MUserID = t1.MItemID\n                    AND t3.MIsDelete = 0\n                    AND t3.MOrgID = @MOrgID\n            WHERE\n                t1.MIsDelete = 0";

		public DataGridJson<SECUserModel> Get(MContext ctx, GetParam param)
		{
			return new APIDataRepository().Get<SECUserModel>(ctx, param, CommonSelect, false, true, null);
		}

		public SECUserModel GetModel(string email, string password)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("select a.*, IFNULL(b.MFristName,substring(a.MEmailAddress,1, LOCATE('@',a.MEmailAddress)-1)) AS MFirstName,IFNULL(b.MLastName,'') AS MLastName,c.MName AS MLastLoginOrgName, c.MInitBalanceOver,a.MMobilePhone, ");
			stringBuilder.Append("       a.MProfileImage , b.MJobTitle , b.MBriefBio ");
			stringBuilder.Append("  from T_Sec_User a");
			stringBuilder.Append("  Inner JOIN T_Sec_User_L b ON a.MItemID = b.MParentID and (b.MFristName is not null || b.MLastName is not null)");
			stringBuilder.Append("  LEFT JOIN T_Bas_Organisation c ON a.MLastLoginOrgID = c.MItemID  ");
			stringBuilder.Append(" where  MEmailAddress =@MEmailAddress and MPassWord =@MPassWord and a.MIsTemp = 0 ");
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@MEmailAddress", email),
				new MySqlParameter("@MPassWord", password)
			};
			DataSet ds = DbHelperMySQL.Query(stringBuilder.ToString(), cmdParms);
			List<SECUserModel> list = ModelInfoManager.DataTableToList<SECUserModel>(ds);
			if (list == null || list.Count == 0)
			{
				return null;
			}
			return list[0];
		}

		public SECUserModel GetModel(MContext ctx, SECLoginModel loginModel)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("select a.*, IFNULL(b.MFristName,substring(a.MEmailAddress,1, LOCATE('@',a.MEmailAddress)-1)) AS MFirstName,IFNULL(b.MLastName,'') AS MLastName,c.MName AS MLastLoginOrgName, c.MInitBalanceOver,a.MMobilePhone, ");
			stringBuilder.Append("       a.MProfileImage , b.MJobTitle , b.MBriefBio ");
			stringBuilder.Append("  from T_Sec_User a");
			stringBuilder.Append("  Inner JOIN T_Sec_User_L b ON a.MItemID = b.MParentID and (b.MFristName is not null || b.MLastName is not null)");
			stringBuilder.Append("  LEFT JOIN T_Bas_Organisation c ON a.MLastLoginOrgID = c.MItemID  ");
			stringBuilder.Append(" where  MEmailAddress =@MEmailAddress and MPassWord =@MPassWord and a.MIsTemp = 0 and a.MIsActive=1 ");
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@MEmailAddress", loginModel.Email),
				new MySqlParameter("@MPassWord", loginModel.Password)
			};
			DataSet ds = DbHelperMySQL.Query(stringBuilder.ToString(), cmdParms);
			List<SECUserModel> list = ModelInfoManager.DataTableToList<SECUserModel>(ds);
			if (list == null || list.Count == 0)
			{
				return null;
			}
			return list[0];
		}

		public SECUserModel GetModelByEmail(MContext ctx, string email)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("select a.MPKID,a.MParentID,b.MMobilePhone, b.MPublicProfile, b.MProfileImage, a.MFristName , a.MLastName, a.MJobTitle, a.MBriefBio ");
			stringBuilder.Append("  from T_Sec_User_L a ");
			stringBuilder.Append(" inner join t_sec_user b on b.MItemID = a.MParentID and b.MEmailAddress=@MEmail and (a.MFristName is not null || a.MLastName is not null) and b.MIsTemp=0 ");
			MySqlParameter[] array = new MySqlParameter[1]
			{
				new MySqlParameter("@MEmail", MySqlDbType.VarChar, 36)
			};
			array[0].Value = email;
			SECUserModel sECUserModel = new SECUserModel();
			SECUserlModel sECUserlModel = new SECUserlModel();
			DataSet dataSet = DbHelperMySQL.Query(stringBuilder.ToString(), array);
			List<SECUserModel> list = ModelInfoManager.DataTableToList<SECUserModel>(dataSet.Tables[0]);
			if (list == null || list.Count() == 0)
			{
				return null;
			}
			List<SECUserlModel> list2 = ModelInfoManager.DataTableToList<SECUserlModel>(dataSet.Tables[0]);
			sECUserlModel = list2[0];
			sECUserModel = list[0];
			sECUserModel.SECUserLModel = sECUserlModel;
			sECUserModel.MFirstName = sECUserlModel.MFristName;
			return sECUserModel;
		}

		public List<SECUserlModel> GetUserLModelById(string userId)
		{
			string sQLString = " select * from t_sec_user_l where MParentID = @UserID and MIsDelete = 0";
			MySqlParameter[] cmdParms = new MySqlParameter[1]
			{
				new MySqlParameter("@UserID", userId)
			};
			DataSet ds = DbHelperMySQL.Query(sQLString, cmdParms);
			return ModelInfoManager.DataTableToList<SECUserlModel>(ds);
		}

		public SECUserModel GetModelByEmail(string email)
		{
			string sQLString = "select * from t_sec_user where MEmailAddress=@MEmailAddress ORDER BY MIsTemp";
			MySqlParameter[] cmdParms = new MySqlParameter[1]
			{
				new MySqlParameter("@MEmailAddress", email)
			};
			DataSet ds = DbHelperMySQL.Query(sQLString, cmdParms);
			return ModelInfoManager.GetFirstOrDefaultModel<SECUserModel>(ds);
		}

		public SECUserModel GetBDBaseModelByEmail(MContext ctx, string email)
		{
			string sql = "select * from t_sec_user where MEmailAddress=@MEmailAddress ORDER BY MIsTemp";
			MySqlParameter[] cmdParms = new MySqlParameter[1]
			{
				new MySqlParameter("@MEmailAddress", email)
			};
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet ds = dynamicDbHelperMySQL.Query(sql, cmdParms);
			return ModelInfoManager.GetFirstOrDefaultModel<SECUserModel>(ds);
		}

		public OperationResult UpdateImage(MContext ctx, SECUserModel model)
		{
			OperationResult operationResult = new OperationResult();
			string text = "update  t_sec_user set MProfileImage=@MProfileImage,MMobilePhone=@MMobilePhone  where MItemID=@MItemID";
			MySqlParameter[] array = new MySqlParameter[3]
			{
				new MySqlParameter("@MProfileImage", MySqlDbType.VarChar, 255),
				new MySqlParameter("@MMobilePhone", MySqlDbType.VarChar, 100),
				new MySqlParameter("@MItemID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = model.MProfileImage;
			array[1].Value = model.MMobilePhone;
			array[2].Value = model.MItemID;
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

		public OperationResult JoinToOrg(MContext ctx, SECUserModel model)
		{
			OperationResult operationResult = new OperationResult();
			operationResult.Success = false;
			if (string.IsNullOrEmpty(model.MOrgID))
			{
				return operationResult;
			}
			if (ctx.MActiveLocaleIDS == null || ctx.MActiveLocaleIDS.Count != COMMultiLangRepository.MegiLangTypes.Count())
			{
				ctx.MActiveLocaleIDS = COMMultiLangRepository.MegiLangTypes.ToList();
			}
			SECUserModel dataModel = base.GetDataModel(ctx, model.MItemID, false);
			if (dataModel == null)
			{
				return operationResult;
			}
			if (dataModel.MultiLanguage != null && dataModel.MultiLanguage.Count > 0)
			{
				foreach (MultiLanguageFieldList item in dataModel.MultiLanguage)
				{
					if (item.MFieldName == "MFristName")
					{
						item.MMultiLanguageField[0].MValue = model.MFirstName;
						item.MMultiLanguageField[1].MValue = model.MFirstName;
						item.MMultiLanguageField[2].MValue = model.MFirstName;
					}
					else if (item.MFieldName == "MLastName")
					{
						item.MMultiLanguageField[0].MValue = model.MLastName;
						item.MMultiLanguageField[1].MValue = model.MLastName;
						item.MMultiLanguageField[2].MValue = model.MLastName;
					}
				}
			}
			List<string> list = (from s in ModelInfoManager.GetDataModelList<SECOrgUserModel>(ctx, new SqlWhere().Equal("MOrgID", model.MOrgID).Equal("MUserID", model.MItemID), false, false)
			select s.MItemID).ToList();
			if (list == null || list.Count == 0)
			{
				return operationResult;
			}
			List<CommandInfo> list2 = new List<CommandInfo>();
			dataModel.MEmailAddress = model.MEmailAddress;
			dataModel.MIsTemp = false;
			dataModel.MPassWord = MD5Service.MD5Encrypt(model.MPassWord);
			dataModel.MMobilePhone = model.MMobilePhone;
			HandleUserModelMultiLanguage(dataModel);
			list2.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<SECUserModel>(ctx, dataModel, null, true));
			foreach (string item2 in list)
			{
				list2.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<SECOrgUserModel>(ctx, new SECOrgUserModel
				{
					MItemID = item2,
					MUserIsActive = true
				}, new List<string>
				{
					"MUserIsActive"
				}, true));
			}
			MultiDBCommand[] array = new MultiDBCommand[2]
			{
				new MultiDBCommand(ctx)
				{
					DBType = SysOrBas.Sys,
					CommandList = new List<CommandInfo>()
				},
				null
			};
			ctx.MOrgID = model.MOrgID;
			array[0].CommandList.AddRange(list2);
			array[0].CommandList.Add(SECSendLinkInfoRepository.GetDeleteJoinLink(model.SendLinkID));
			array[1] = new MultiDBCommand(ctx)
			{
				DBType = SysOrBas.Bas,
				CommandList = new List<CommandInfo>()
			};
			array[1].CommandList.AddRange(list2);
			operationResult.Success = DbHelperMySQL.ExecuteSqlTran(ctx, array);
			return operationResult;
		}

		public OperationResult SureRegister(MContext ctx, SECAccountModel model)
		{
			OperationResult operationResult = new OperationResult();
			try
			{
				SECUserModel sECUserModel = GetModelByEmail(model.MEmailAddress);
				if (sECUserModel != null)
				{
					sECUserModel.MIsTemp = false;
					sECUserModel.MPassWord = MD5Service.MD5Encrypt(model.MPassWord);
				}
				else
				{
					sECUserModel = ModelInfoManager.GetEmptyDataEditModel<SECUserModel>(ctx);
					sECUserModel.MEmailAddress = model.MEmailAddress;
					sECUserModel.MPassWord = MD5Service.MD5Encrypt(model.MPassWord);
					sECUserModel.MAppID = 1.ToString();
					sECUserModel.MMobilePhone = model.MMobilePhone;
					if (ctx.MActiveLocaleIDS == null || ctx.MActiveLocaleIDS.Count != COMMultiLangRepository.MegiLangTypes.Count())
					{
						ctx.MActiveLocaleIDS = COMMultiLangRepository.MegiLangTypes.ToList();
					}
					setMultiField(sECUserModel, ctx.MLCID, "MFristName", model.MFristName);
					setMultiField(sECUserModel, ctx.MLCID, "MLastName", model.MLastName);
				}
				HandleUserModelMultiLanguage(sECUserModel);
				List<CommandInfo> list = new List<CommandInfo>();
				list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<SECUserModel>(ctx, sECUserModel, null, true));
				list.Add(SECSendLinkInfoRepository.GetDeleteJoinLink(model.SendLinkID));
				operationResult.Success = (DbHelperMySQL.ExecuteSqlTran(ctx, list) > 0);
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				ctx.IsSys = false;
			}
			return operationResult;
		}

		public OperationResult PutNewPwd(MContext ctx, SECUserModel model)
		{
			OperationResult operationResult = new OperationResult();
			model.MPassWord = MD5Service.MD5Encrypt(model.MPassWord);
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.Equal(" MEmailAddress ", model.MEmailAddress);
			sqlWhere.Equal("MIsTemp", "0");
			SECUserModel dataModelByFilter = base.GetDataModelByFilter(ctx, sqlWhere);
			if (dataModelByFilter == null)
			{
				string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Login, "NotFoundEmail", "No account found with that email address!");
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = text
				});
				return operationResult;
			}
			dataModelByFilter.MPassWord = model.MPassWord;
			HandleUserModelMultiLanguage(dataModelByFilter);
			List<CommandInfo> insertOrUpdateCmd = ModelInfoManager.GetInsertOrUpdateCmd<SECUserModel>(ctx, dataModelByFilter, null, true);
			insertOrUpdateCmd.Add(SECSendLinkInfoRepository.GetDeleteJoinLink(model.SendLinkID));
			operationResult.Success = (DbHelperMySQL.ExecuteSqlTran(ctx, insertOrUpdateCmd) > 0);
			return operationResult;
		}

		public static void setMultiField(SECUserModel accModel, string localId, string multiKey, string multiValue)
		{
			if (string.IsNullOrWhiteSpace(localId))
			{
				localId = "0x0009";
			}
			MultiLanguageFieldList multiLanguageFieldList = accModel.MultiLanguage.Find((MultiLanguageFieldList p) => p.MFieldName.Equals(multiKey));
			if (multiLanguageFieldList != null)
			{
				if (multiLanguageFieldList.MMultiLanguageField.Find((MultiLanguageField p) => p.MLocaleID.Equals(localId, StringComparison.OrdinalIgnoreCase)) == null)
				{
					multiLanguageFieldList.MMultiLanguageField.Add(new MultiLanguageField
					{
						MLocaleID = localId,
						MValue = multiValue
					});
				}
				else
				{
					multiLanguageFieldList.MMultiLanguageField.Find((MultiLanguageField p) => p.MLocaleID.Equals(localId, StringComparison.OrdinalIgnoreCase)).MValue = multiValue;
				}
			}
		}

		public List<SECUserModel> GetUserList(MContext ctx, SqlWhere filter)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("select a.* , b.MLastName , b.MFristName as MFirstName from T_Sec_User a");
			stringBuilder.AppendLine("INNER JOIN t_sec_user_l b on a.MItemID=b.MParentID and  b.MFristName is not null and b.MFristName !=''  and b.MLastName is not null and b.MLastName !='' ");
			if (filter != null && !string.IsNullOrEmpty(filter.WhereSqlString))
			{
				stringBuilder.AppendLine(filter.WhereSqlString);
			}
			stringBuilder.AppendLine("Group by a.mitemid");
			ArrayList arrayList = new ArrayList();
			arrayList.Add(new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 6));
			if (filter != null && filter.Parameters.Length != 0)
			{
				MySqlParameter[] parameters = filter.Parameters;
				foreach (MySqlParameter value in parameters)
				{
					arrayList.Add(value);
				}
			}
			MySqlParameter[] array = (MySqlParameter[])arrayList.ToArray(typeof(MySqlParameter));
			array[0].Value = ctx.MLCID;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return ModelInfoManager.DataTableToList<SECUserModel>(dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array).Tables[0]);
		}

		public List<SECUserModel> GetAllUserByID(MContext ctx, string pkID)
		{
			string text = " select t1.MItemID,t1.MEmailAddress, t2.MFristName as MFirstName, t2.MLastName,t2.MLocaleID from t_sec_user t1 join t_sec_user_l t2 on t1.MItemID = t2.MParentID and t1.MItemID = '" + pkID + "'";
			return ModelInfoManager.DataTableToList<SECUserModel>(new DynamicDbHelperMySQL(ctx).Query(text.ToString()).Tables[0]);
		}

		public void HandleUserModelMultiLanguage(SECUserModel userModel)
		{
			try
			{
				if (userModel.MultiLanguage != null && userModel.MultiLanguage.Any())
				{
					List<MultiLanguageFieldList> list = (from x in userModel.MultiLanguage
					where x.MFieldName == "MFristName" || x.MFieldName == "MLastName"
					select x).ToList();
					for (int i = 0; i < list.Count; i++)
					{
						for (int j = 0; j < COMMultiLangRepository.MegiLangTypes.Length; j++)
						{
							if (list[i].MMultiLanguageField != null)
							{
								string localeId = COMMultiLangRepository.MegiLangTypes[j];
								MultiLanguageField multiLanguageField = list[i].MMultiLanguageField.FirstOrDefault((MultiLanguageField x) => !string.IsNullOrWhiteSpace(x.MValue));
								string text = (multiLanguageField == null) ? string.Empty : multiLanguageField.MValue;
								if (!string.IsNullOrWhiteSpace(text))
								{
									MultiLanguageField multiLanguageField2 = list[i].MMultiLanguageField.FirstOrDefault((MultiLanguageField x) => x.MLocaleID == localeId);
									if (multiLanguageField2 == null)
									{
										multiLanguageField2 = new MultiLanguageField
										{
											MLocaleID = localeId,
											MValue = text
										};
										list[i].MMultiLanguageField.Add(multiLanguageField2);
									}
									else if (string.IsNullOrWhiteSpace(multiLanguageField2.MValue))
									{
										multiLanguageField2.MValue = text;
									}
									continue;
								}
								return;
							}
						}
					}
				}
			}
			catch (Exception)
			{
			}
		}

		public SECUserModel GetUserModel(MContext ctx, string userId, bool onlyActive = true)
		{
			string text = "select * from t_sec_user where MItemID=@MItemID and MIsDelete=0  ";
			if (onlyActive)
			{
				text += " and MIsTemp=0 ";
			}
			MySqlParameter[] cmdParms = new MySqlParameter[1]
			{
				new MySqlParameter("@MItemID", userId)
			};
			DataSet ds = DbHelperMySQL.Query(text, cmdParms);
			List<SECUserModel> list = ModelInfoManager.DataTableToList<SECUserModel>(ds);
			if (list != null && list.Count() > 0)
			{
				return list.First();
			}
			return null;
		}
	}
}
