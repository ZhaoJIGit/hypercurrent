using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.MultiLanguage;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace JieNor.Megi.DataRepository.SEC
{
	public static class SECPermissionRepository
	{
		private static Dictionary<string, List<SECUserPermissionModel>> userPermission = new Dictionary<string, List<SECUserPermissionModel>>();

		private static object objLock = new object();

		public static bool HavePermission(MContext ctx, string bizObjectKey, string permissionItem, string inputOrgId = "")
		{
			string orgId = ctx.MOrgID;
			if (!string.IsNullOrWhiteSpace(inputOrgId))
			{
				orgId = inputOrgId;
			}
			if (ctx == null || string.IsNullOrWhiteSpace(ctx.MUserID) || string.IsNullOrWhiteSpace(bizObjectKey) || string.IsNullOrWhiteSpace(permissionItem))
			{
				return false;
			}
			List<SECUserPermissionModel> list = null;
			KeyValuePair<string, List<SECUserPermissionModel>> keyValuePair;
			if (userPermission == null || userPermission.Count == 0 || !userPermission.ContainsKey(ctx.MUserID))
			{
				GetUserPermission(ctx, "");
			}
			else
			{
				lock (objLock)
				{
					keyValuePair = userPermission.FirstOrDefault((KeyValuePair<string, List<SECUserPermissionModel>> f) => f.Key.Equals(ctx.MUserID, StringComparison.OrdinalIgnoreCase));
					list = keyValuePair.Value;
				}
				if (!list.Exists((SECUserPermissionModel e) => e.MOrgID.Equals(orgId, StringComparison.OrdinalIgnoreCase)))
				{
					GetUserPermission(ctx, "");
					lock (objLock)
					{
						keyValuePair = userPermission.FirstOrDefault((KeyValuePair<string, List<SECUserPermissionModel>> f) => f.Key.Equals(ctx.MUserID, StringComparison.OrdinalIgnoreCase));
						list = keyValuePair.Value;
					}
					if (list == null || list.Count <= 0 || !list.Exists((SECUserPermissionModel e) => e.MOrgID.Equals(orgId, StringComparison.OrdinalIgnoreCase)))
					{
						return false;
					}
				}
			}
			if (userPermission == null || userPermission.Count == 0)
			{
				return false;
			}
			if (list == null)
			{
				lock (objLock)
				{
					keyValuePair = userPermission.FirstOrDefault((KeyValuePair<string, List<SECUserPermissionModel>> f) => f.Key.Equals(ctx.MUserID, StringComparison.OrdinalIgnoreCase));
					list = keyValuePair.Value;
				}
			}
			SECUserPermissionModel sECUserPermissionModel = list.FirstOrDefault((SECUserPermissionModel f) => f.MOrgID.Equals(orgId, StringComparison.OrdinalIgnoreCase) && f.MBizObjectID.Equals(bizObjectKey, StringComparison.OrdinalIgnoreCase) && f.MPermissionItemNumber.Equals(permissionItem, StringComparison.OrdinalIgnoreCase) && f.MIsRefuset);
			if (sECUserPermissionModel != null)
			{
				return false;
			}
			SECUserPermissionModel sECUserPermissionModel2 = list.FirstOrDefault((SECUserPermissionModel f) => f.MOrgID.Equals(orgId, StringComparison.OrdinalIgnoreCase) && f.MBizObjectID.Equals(bizObjectKey, StringComparison.OrdinalIgnoreCase) && f.MPermissionItemNumber.Equals(permissionItem, StringComparison.OrdinalIgnoreCase) && f.MIsGrant);
			if (sECUserPermissionModel2 != null)
			{
				return true;
			}
			return false;
		}

		public static bool HavePermissionNoByCache(MContext ctx, string bizObjectKey, string permissionItem, List<SECUserPermissionModel> permissionModels, string inputOrgId = "")
		{
			string orgId = ctx.MOrgID;
			if (!string.IsNullOrWhiteSpace(inputOrgId))
			{
				orgId = inputOrgId;
			}
			if (ctx == null || string.IsNullOrWhiteSpace(ctx.MUserID) || string.IsNullOrWhiteSpace(bizObjectKey) || string.IsNullOrWhiteSpace(permissionItem))
			{
				return false;
			}
			if (permissionModels == null || permissionModels.Count == 0)
			{
				return false;
			}
			SECUserPermissionModel sECUserPermissionModel = permissionModels.FirstOrDefault((SECUserPermissionModel f) => f.MOrgID.Equals(orgId, StringComparison.OrdinalIgnoreCase) && f.MBizObjectID.Equals(bizObjectKey, StringComparison.OrdinalIgnoreCase) && f.MPermissionItemNumber.Equals(permissionItem, StringComparison.OrdinalIgnoreCase) && f.MIsRefuset);
			if (sECUserPermissionModel != null)
			{
				return false;
			}
			SECUserPermissionModel sECUserPermissionModel2 = permissionModels.FirstOrDefault((SECUserPermissionModel f) => f.MOrgID.Equals(orgId, StringComparison.OrdinalIgnoreCase) && f.MBizObjectID.Equals(bizObjectKey, StringComparison.OrdinalIgnoreCase) && f.MPermissionItemNumber.Equals(permissionItem, StringComparison.OrdinalIgnoreCase) && f.MIsGrant);
			if (sECUserPermissionModel2 != null)
			{
				return true;
			}
			return false;
		}

		public static List<SECUserPermissionModel> GetUserPermissionNoByCache(MContext ctx, string setUserID = "")
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(" select g.MOrgID,c.MItemID as MBizObjectID,d.MNumber as MPermissionItemNumber,");
			stringBuilder.AppendLine(" a.MIsGrant,a.MIsRefuset ");
			stringBuilder.AppendLine(" from T_Sec_RolePermission a ");
			stringBuilder.AppendLine(" inner join T_Sec_Role b on a.MRoleID=b.MItemID and a.MIsDelete=0 and b.MIsDelete=0 ");
			stringBuilder.AppendLine(" inner join T_Sec_ObjectPermission c on a.MBizObjectID=c.MItemID and c.MIsDelete=0");
			stringBuilder.AppendLine(" inner join T_Sec_PermisionItem d on a.MPermItemID=d.MItemID and d.MIsDelete=0 ");
			stringBuilder.AppendLine(" inner join T_Sec_RoleUser e on e.MRoleID= b.MItemID and e.MIsDelete=0 ");
			stringBuilder.AppendLine(" inner join T_Sec_User f on e.MUserID=f.MItemID and f.MIsDelete=0");
			stringBuilder.AppendLine(" inner join T_Sec_OrgUser g on f.MItemID=g.MUserID and b.MOrgID=g.MOrgID and g.MIsDelete=0");
			stringBuilder.AppendLine(" left join T_Sec_Role_L bl on b.MNumber=bl.MParentID  and bl.MLocaleID=@MLocaleID and bl.MIsDelete=0");
			stringBuilder.AppendLine(" left join T_Sec_ObjectPermission_L cl on c.MItemID=cl.MParentID  and cl.MLocaleID=@MLocaleID and cl.MIsDelete=0");
			stringBuilder.AppendLine(" left join T_Sec_PermisionItem_L dl on d.MItemID=dl.MParentID  and dl.MLocaleID=@MLocaleID and dl.MIsDelete=0 ");
			stringBuilder.AppendLine(" Where f.MItemID=@MUserID ");
			stringBuilder.AppendLine(" order by f.MEmailAddress,bl.MName,cl.MName,dl.MName ");
			MySqlParameter[] array = new MySqlParameter[2]
			{
				new MySqlParameter("@MUserID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 6)
			};
			if (string.IsNullOrWhiteSpace(setUserID))
			{
				setUserID = ctx.MUserID;
			}
			array[0].Value = setUserID;
			array[1].Value = ctx.MLCID;
			return ModelInfoManager.DataTableToList<SECUserPermissionModel>(DbHelperMySQL.Query(stringBuilder.ToString(), array).Tables[0]);
		}

		public static void GetUserPermission(MContext ctx, string setUserID = "")
		{
			if (string.IsNullOrEmpty(setUserID))
			{
				setUserID = ctx.MUserID;
			}
			List<SECUserPermissionModel> userPermissionNoByCache = GetUserPermissionNoByCache(ctx, setUserID);
			lock (objLock)
			{
				if (userPermission.ContainsKey(setUserID))
				{
					userPermission[setUserID] = userPermissionNoByCache;
				}
				else
				{
					userPermission.Add(setUserID, userPermissionNoByCache);
				}
			}
		}

		public static OperationResult UserPermissionUpd(MContext ctx, SECInviteUserInfoModel model)
		{
			OperationResult operationResult = new OperationResult();
			List<CommandInfo> list = new List<CommandInfo>();
			List<CommandInfo> list2 = new List<CommandInfo>();
			list.Add(new CommandInfo
			{
				CommandText = " SET SQL_SAFE_UPDATES = 0;"
			});
			list2.Add(new CommandInfo
			{
				CommandText = " SET SQL_SAFE_UPDATES = 0;"
			});
			string mItemID = model.MItemID;
			if (ExistsOrgUserByEmail(ctx.MOrgID, model.MEmail, mItemID))
			{
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "OrgUserExists", "A user with that email address already exists for this organisation,or user is actived,please open edit page again.")
				});
				return operationResult;
			}
			if (string.IsNullOrWhiteSpace(mItemID))
			{
				string empty = string.Empty;
				list.AddRange(GetSecOrgUserAddCmd(ctx, model, null, out mItemID, out empty, false));
				model.MItemID = mItemID;
				string empty2 = string.Empty;
				list2.AddRange(GetSecOrgUserAddCmd(ctx, model, empty, out mItemID, out empty2, true));
				operationResult.ObjectID = mItemID;
			}
			else
			{
				bool flag = true;
				List<SECPermisionGrpOperateModel> list3 = (from w in model.MGrpOperateList
				where w.GroupID.Equals("System_Settings") && w.Change
				select w).ToList();
				bool needCheck = (list3 == null || list3.Count == 0) && true;
				List<CommandInfo> collection = DeleteInfoByUser(ctx, mItemID, false, needCheck, out flag);
				list.AddRange(collection);
				list2.AddRange(collection);
				if (!flag)
				{
					operationResult.VerificationInfor.Add(new BizVerificationInfor
					{
						Level = AlertEnum.Error,
						Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "AtLeastOneSystemManager", "every organisation must have at least one system manager.")
					});
					return operationResult;
				}
				List<CommandInfo> secOrgUserUpdCmd = GetSecOrgUserUpdCmd(ctx, model);
				list.AddRange(secOrgUserUpdCmd);
				list2.AddRange(secOrgUserUpdCmd);
				List<CommandInfo> updateCreateOrgAuthCmds = GetUpdateCreateOrgAuthCmds(ctx, mItemID, ctx.MIsHadAddOrgAuth);
				list.AddRange(updateCreateOrgAuthCmds);
				list2.AddRange(updateCreateOrgAuthCmds);
			}
			string empty3 = string.Empty;
			List<CommandInfo> secRoleUserAddCmd = GetSecRoleUserAddCmd(ctx, mItemID, out empty3);
			list.AddRange(secRoleUserAddCmd);
			list2.AddRange(secRoleUserAddCmd);
			List<SECPermisionGrpOperateModel> grpOperateList = (from w in model.MGrpOperateList
			where w.View || w.Change || w.Approve || w.Export
			select w).ToList();
			List<CommandInfo> collection2 = AddRoleGrpLink(ctx, empty3, grpOperateList);
			list.AddRange(collection2);
			list2.AddRange(collection2);
			List<CommandInfo> collection3 = AddRolePermission(ctx, empty3, grpOperateList);
			list.AddRange(collection3);
			list2.AddRange(collection3);
			operationResult.Success = DbHelperMySQL.ExecuteSqlTran(ctx, new MultiDBCommand[2]
			{
				new MultiDBCommand(ctx)
				{
					DBType = SysOrBas.Sys,
					CommandList = list
				},
				new MultiDBCommand(ctx)
				{
					DBType = SysOrBas.Bas,
					CommandList = list2
				}
			});
			if (operationResult.Success)
			{
				if (model.MPermStatus == "Active")
				{
					GetUserPermission(ctx, mItemID);
				}
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "SaveSuccess", "user has been saved.");
			}
			else
			{
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "SaveFail", "user save fail.");
			}
			return operationResult;
		}

		private static List<CommandInfo> GetUpdateCreateOrgAuthCmds(MContext ctx, string userId, bool isHasAuth)
		{
			List<CommandInfo> result = new List<CommandInfo>();
			SECUserRepository sECUserRepository = new SECUserRepository();
			SECUserModel userModel = sECUserRepository.GetUserModel(ctx, userId, true);
			if (userModel == null)
			{
				return result;
			}
			userModel.MIsHadAddOrgAuth = (userModel.MIsHadAddOrgAuth ? userModel.MIsHadAddOrgAuth : isHasAuth);
			List<string> fields = new List<string>
			{
				"MIsHadAddOrgAuth"
			};
			return ModelInfoManager.GetInsertOrUpdateCmd<SECUserModel>(ctx, userModel, fields, true);
		}

		private static List<CommandInfo> GetSecOrgUserAddCmd(MContext ctx, SECInviteUserInfoModel model, string orgUserId, out string userid, out string outOrgUserId, bool isBDBase = false)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			outOrgUserId = "";
			List<SECUserlModel> list2 = new List<SECUserlModel>();
			SECUserRepository sECUserRepository = new SECUserRepository();
			SECUserModel userModel = isBDBase ? sECUserRepository.GetBDBaseModelByEmail(ctx, model.MEmail) : sECUserRepository.GetModelByEmail(model.MEmail);
			if (isBDBase && userModel == null)
			{
				userModel = sECUserRepository.GetModelByEmail(model.MEmail);
				if (userModel != null)
				{
					userModel.IsNew = true;
					list2 = sECUserRepository.GetUserLModelById(userModel.MItemID);
				}
			}
			if (userModel == null || string.IsNullOrEmpty(userModel.MItemID))
			{
				userModel = ModelInfoManager.GetEmptyDataEditModel<SECUserModel>(ctx);
				userModel.MEmailAddress = model.MEmail;
				userModel.MIsTemp = true;
				userModel.IsNew = true;
				userModel.MIsHadAddOrgAuth = ctx.MIsHadAddOrgAuth;
				SECUserRepository.setMultiField(userModel, ctx.MLCID, "MFristName", model.MFirstName);
				SECUserRepository.setMultiField(userModel, ctx.MLCID, "MLastName", model.MLastName);
			}
			else
			{
				userModel.MIsHadAddOrgAuth = (userModel.MIsHadAddOrgAuth ? userModel.MIsHadAddOrgAuth : ctx.MIsHadAddOrgAuth);
			}
			if (isBDBase)
			{
				userModel.MItemID = model.MItemID;
			}
			if (string.IsNullOrWhiteSpace(orgUserId))
			{
				outOrgUserId = UUIDHelper.GetGuid();
				orgUserId = outOrgUserId;
			}
			new SECUserRepository().HandleUserModelMultiLanguage(userModel);
			list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<SECUserModel>(ctx, userModel, null, true));
			if (list2?.Any() ?? false)
			{
				list2.ForEach(delegate(SECUserlModel x)
				{
					x.MParentID = userModel.MItemID;
					x.IsNew = true;
				});
				list.AddRange(ModelInfoManager.GetInsertOrUpdateCmds(ctx, list2, null, true));
			}
			SECOrgUserModel sECOrgUserModel = new SECOrgUserModel
			{
				MUserID = userModel.MItemID,
				MOrgID = ctx.MOrgID,
				MUserIsActive = false,
				MPosition = model.MPosition,
				MRole = model.MRole,
				IsSelfData = model.IsSelfData
			};
			sECOrgUserModel.MItemID = orgUserId;
			sECOrgUserModel.IsNew = true;
			list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<SECOrgUserModel>(ctx, sECOrgUserModel, null, true));
			userid = userModel.MItemID;
			return list;
		}

		private static List<CommandInfo> GetSecRoleUserAddCmd(MContext ctx, string userId, out string roleId)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			SECRoleModel sECRoleModel = new SECRoleModel();
			sECRoleModel.MOrgID = ctx.MOrgID;
			sECRoleModel.MAppID = "1";
			sECRoleModel.MNumber = "3";
			list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<SECRoleModel>(ctx, sECRoleModel, null, true));
			list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<SECRoleUserModel>(ctx, new SECRoleUserModel
			{
				MUserID = userId,
				MRoleID = sECRoleModel.MItemID,
				MOrgID = ctx.MOrgID
			}, null, true));
			roleId = sECRoleModel.MItemID;
			return list;
		}

		private static List<CommandInfo> AddRoleGrpLink(MContext ctx, string roleId, List<SECPermisionGrpOperateModel> GrpOperateList)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			List<BaseModel> list2 = new List<BaseModel>();
			foreach (SECPermisionGrpOperateModel GrpOperate in GrpOperateList)
			{
				SECRolePermisionGroupModel sECRolePermisionGroupModel = new SECRolePermisionGroupModel();
				sECRolePermisionGroupModel.MRoleID = roleId;
				sECRolePermisionGroupModel.MPermGrpID = GrpOperate.GroupID;
				sECRolePermisionGroupModel.MView = GrpOperate.View;
				sECRolePermisionGroupModel.MChange = GrpOperate.Change;
				sECRolePermisionGroupModel.MApprove = GrpOperate.Approve;
				sECRolePermisionGroupModel.MExport = GrpOperate.Export;
				list2.Add(sECRolePermisionGroupModel);
			}
			return ModelInfoManager.GetInsertOrUpdateCmd<SECRolePermisionGroupModel>(ctx, list2, null);
		}

		private static List<CommandInfo> AddRolePermission(MContext ctx, string roleId, List<SECPermisionGrpOperateModel> GrpOperateList)
		{
			List<SECPermisionItemGrpModel> finalItemGrpModelList = GetFinalItemGrpModelList(ctx, GrpOperateList);
			List<BaseModel> list = new List<BaseModel>();
			foreach (SECPermisionItemGrpModel item in finalItemGrpModelList)
			{
				SECRolePermissionModel sECRolePermissionModel = new SECRolePermissionModel();
				sECRolePermissionModel.MRoleID = roleId;
				sECRolePermissionModel.MBizObjectID = item.MBizObjID;
				sECRolePermissionModel.MPermItemID = item.MPermItemID;
				sECRolePermissionModel.MIsGrant = true;
				sECRolePermissionModel.MIsRefuset = false;
				list.Add(sECRolePermissionModel);
			}
			return ModelInfoManager.GetInsertOrUpdateCmd<SECRolePermissionModel>(ctx, list, null);
		}

		public static List<CommandInfo> AddAllPermissionToRole(MContext ctx, string orgUserId, string roleId, SqlWhere sqlWhere, ModuleEnum module = ModuleEnum.Sales)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			if (module == ModuleEnum.Sales || ctx.MOrgVersionID == 1)
			{
				list.AddRange(SetOrgUserPositionAndRole(ctx, new SECOrgUserModel
				{
					MItemID = orgUserId,
					MPosition = "SysManage",
					MRole = "Admin"
				}));
			}
			List<SECPermisionGrpOperateModel> list2 = new List<SECPermisionGrpOperateModel>();
			List<SECPermisionGroupModel> modelList = new SECPermisionGroupRepository().GetModelList(ctx, sqlWhere, false);
			foreach (SECPermisionGroupModel item in modelList)
			{
				SECPermisionGrpOperateModel sECPermisionGrpOperateModel = new SECPermisionGrpOperateModel();
				sECPermisionGrpOperateModel.GroupID = item.MItemID;
				sECPermisionGrpOperateModel.View = true;
				sECPermisionGrpOperateModel.Change = true;
				sECPermisionGrpOperateModel.Approve = true;
				sECPermisionGrpOperateModel.Export = true;
				list2.Add(sECPermisionGrpOperateModel);
			}
			list.AddRange(AddRoleGrpLink(ctx, roleId, list2));
			list.AddRange(AddRolePermission(ctx, roleId, list2));
			return list;
		}

		private static List<CommandInfo> SetOrgUserPositionAndRole(MContext ctx, SECOrgUserModel model)
		{
			return ModelInfoManager.GetInsertOrUpdateCmd<SECOrgUserModel>(ctx, model, new List<string>
			{
				"MPosition",
				"MRole"
			}, true);
		}

		public static SECUserPermissionListModel GetUserPermissionInfo(MContext ctx, string userId)
		{
			SECUserPermissionListModel result = new SECUserPermissionListModel();
			string sql = "select t0.MUserID as MItemID,t0.MPosition,t0.MRole,t0.MUserIsActive ,t0.MIsArchive\r\n                            from T_Sec_OrgUser t0 \r\n                            where t0.MIsDelete=0 and t0.MOrgID=@MOrgID and t0.MUserID=@MUserID";
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MUserID", userId)
			};
			List<SECUserPermissionListModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<SECUserPermissionListModel>(ctx, sql, cmdParms);
			if (dataModelBySql.Any())
			{
				result = dataModelBySql[0];
			}
			return result;
		}

		public static DataGridJson<SECUserPermissionListModel> GetUserPermissionPageList(MContext ctx, SECUserPermissionListFilterModel filter)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("select t1.MItemID,t1L.MFristName AS MFirstName,t1L.MLastName,t1.MEmailAddress,t0.MPosition,t0.MRole,t0.MUserIsActive ,t0.MIsArchive");
			stringBuilder.AppendLine("from T_Sec_OrgUser t0 ");
			stringBuilder.AppendLine("join T_Sec_User t1 on t0.MUserID=t1.MItemID and t1.MIsDelete=0 and t0.MIsDelete=0 ");
			stringBuilder.AppendLine("join( ");
			stringBuilder.AppendLine(GetUserMultiSql());
			stringBuilder.AppendLine(") t1L on t1.MItemID=t1L.MParentID ");
			stringBuilder.AppendLine("where t0.MIsDelete=0 and t0.MOrgID=@MOrgID ");
			SqlQuery sqlQuery = new SqlQuery();
			sqlQuery.SqlWhere = filter;
			sqlQuery.SelectString = stringBuilder.ToString();
			sqlQuery.AddParameter(new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
			{
				Value = ctx.MOrgID
			});
			sqlQuery.AddParameter(new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 6)
			{
				Value = ctx.MLCID
			});
			if (string.IsNullOrEmpty(filter.Sort))
			{
				sqlQuery.OrderBy("t1.MCreateDate DESC");
			}
			else if (filter.Sort == "MEmail")
			{
				sqlQuery.OrderBy($"t1.MEmailAddress {filter.Order}");
			}
			else if (filter.Sort == "MFullName")
			{
				sqlQuery.OrderBy($"t1L.MFristName {filter.Order}");
			}
			else if (filter.Sort == "MPermStatus")
			{
				sqlQuery.OrderBy($"t0.MUserIsActive {filter.Order}");
			}
			else
			{
				sqlQuery.OrderBy($"t0.{filter.Sort} {filter.Order}");
			}
			DataGridJson<SECUserPermissionListModel> dataGridJson = new DataGridJson<SECUserPermissionListModel>();
			DataSet dataSet = DbHelperMySQL.Query(sqlQuery.Sql, sqlQuery.Parameters);
			if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
			{
				dataGridJson.rows = DataTableToList(ctx, dataSet.Tables[0]);
			}
			dataSet = DbHelperMySQL.Query(sqlQuery.CountSqlString, sqlQuery.Parameters);
			if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
			{
				dataGridJson.total = Convert.ToInt32(dataSet.Tables[0].Rows[0][0]);
			}
			return dataGridJson;
		}

		private static DataSet GetExeSqlResult(MContext ctx, string strUserId)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("SELECT t1.MItemID,t1L.MFristName,t1L.MLastName,t1.MEmailAddress,t0.MUserIsActive,t0.MIsArchive,t0.MPosition,t0.MRole,t0.IsSelfData, ");
			stringBuilder.AppendLine("t4.MPermGrpID,t4.MView,t4.MChange,t4.MApprove,t4.MExport ");
			stringBuilder.AppendLine("FROM T_Sec_OrgUser t0 ");
			stringBuilder.AppendLine("join T_Sec_User t1 on t0.MUserID=t1.MItemID AND t1.MIsDelete=0 ");
			stringBuilder.AppendLine("join( ");
			stringBuilder.AppendLine(GetUserMultiSql());
			stringBuilder.AppendLine(") t1L on t1.MItemID=t1L.MParentID ");
			stringBuilder.AppendLine("join T_Sec_RoleUser t2 on t1.MItemID=t2.MUserID AND t2.MIsDelete=0 ");
			stringBuilder.AppendLine("join T_Sec_Role t3 on t2.MRoleID=t3.MItemID and t3.MOrgID=@MOrgID AND t3.MIsDelete=0 ");
			stringBuilder.AppendLine("left join T_Sec_RoleObjectGroup t4 on t3.MItemID=t4.MRoleID AND t4.MIsDelete=0 ");
			stringBuilder.AppendLine("where t0.MIsDelete=0 and t0.MOrgID=@MOrgID ");
			stringBuilder.AppendLine("and t1.MItemID=@MItemID ");
			stringBuilder.AppendLine("order by MFristName");
			MySqlParameter[] array = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MItemID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 6)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = strUserId;
			array[2].Value = ctx.MLCID;
			return DbHelperMySQL.Query(stringBuilder.ToString(), array);
		}

		public static string GetUserMultiSql()
		{
			return "select MParentID, MFristName,MLastName from T_Sec_User_L \r\n                    where IFNULL(MFristName,'') <>'' and IFNULL(MLastName,'')<>'' and MIsDelete=0 \r\n                    group by MParentID";
		}

		private static List<SECUserPermissionListModel> DataTableToList(MContext ctx, DataTable dt)
		{
			List<SECUserPermissionListModel> list = new List<SECUserPermissionListModel>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					SECUserPermissionListModel sECUserPermissionListModel = new SECUserPermissionListModel();
					sECUserPermissionListModel.MItemID = dt.Rows[i]["MItemID"].ToString();
					sECUserPermissionListModel.MFullName = GlobalFormat.GetUserName(dt.Rows[i]["MFirstName"].ToString(), dt.Rows[i]["MLastName"].ToString(), ctx);
					sECUserPermissionListModel.MEmail = dt.Rows[i]["MEmailAddress"].ToString();
					sECUserPermissionListModel.MPosition = dt.Rows[i]["MPosition"].ToString();
					sECUserPermissionListModel.MRole = dt.Rows[i]["MRole"].ToString();
					sECUserPermissionListModel.MPermStatus = GetPermStatusByFormmat(dt.Rows[i]["MUserIsActive"].ToString());
					sECUserPermissionListModel.MIsArchive = dt.Rows[i]["MIsArchive"].ToString();
					list.Add(sECUserPermissionListModel);
				}
			}
			return list;
		}

		public static bool DeleteUserLinkInfo(MContext ctx, string userId, bool isDelete = true)
		{
			bool flag = true;
			List<CommandInfo> commandList = DeleteInfoByUser(ctx, userId, isDelete, true, out flag);
			if (!flag)
			{
				return false;
			}
			bool flag2 = DbHelperMySQL.ExecuteSqlTran(ctx, new MultiDBCommand[2]
			{
				new MultiDBCommand(ctx)
				{
					DBType = SysOrBas.Sys,
					CommandList = commandList
				},
				new MultiDBCommand(ctx)
				{
					DBType = SysOrBas.Bas,
					CommandList = commandList
				}
			});
			if (flag2)
			{
				GetUserPermission(ctx, userId);
			}
			return flag2;
		}

		private static List<CommandInfo> DeleteInfoByUser(MContext ctx, string userId, bool isDelete, bool needCheck, out bool canDelete)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			canDelete = true;
			if (needCheck)
			{
				canDelete = HaveMoreSysManager(ctx, userId);
			}
			if (!canDelete)
			{
				return list;
			}
			string sql = "select ru.MRoleID from t_sec_roleuser ru \r\n                            inner join t_sec_role r on (ru.MRoleID=r.MitemID) and ru.MIsDelete=0 and r.MIsDelete=0  \r\n                            where r.MOrgID=@OrgID and ru.MUserID=@UserID";
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@OrgID", MySqlDbType.VarChar, 36)
				{
					Value = ctx.MOrgID
				},
				new MySqlParameter("@UserID", MySqlDbType.VarChar, 36)
				{
					Value = userId
				}
			};
			List<string> list2 = (from ru in ModelInfoManager.GetDataModelBySql<SECRoleUserModel>(ctx, sql, cmdParms)
			select ru.MRoleID).ToList();
			MySqlParameter[] para = new MySqlParameter[3]
			{
				new MySqlParameter("@OrgID", MySqlDbType.VarChar, 36)
				{
					Value = ctx.MOrgID
				},
				new MySqlParameter("@UserID", MySqlDbType.VarChar, 36)
				{
					Value = userId
				},
				new MySqlParameter("@RoleID", MySqlDbType.VarChar, 36)
				{
					Value = ((list2.Count > 0) ? list2[0] : "")
				}
			};
			if (isDelete)
			{
				list.Add(new CommandInfo("update t_sec_orguser set MIsDelete=1 where MUserID=@UserID AND MOrgID=@OrgID;", para));
			}
			if (list2.Count > 0)
			{
				list.Add(new CommandInfo("update T_Sec_RoleUser set MIsDelete=1 where MUserID=@UserID AND MRoleID=@RoleID;", para));
				list.Add(new CommandInfo("update T_Sec_Role set MIsDelete=1 where MItemID=@RoleID;", para));
				list.Add(new CommandInfo("update T_Sec_RoleObjectGroup set MIsDelete=1 Where MRoleID=@RoleID;", para));
				list.Add(new CommandInfo("update T_Sec_RolePermission set MIsDelete=1 Where MRoleID=@RoleID;", para));
			}
			return list;
		}

		public static SECInviteUserInfoModel GetUserEditInfo(MContext ctx, SECInviteUserInfoModel model)
		{
			SECInviteUserInfoModel sECInviteUserInfoModel = new SECInviteUserInfoModel();
			DataTable dataTable = GetExeSqlResult(ctx, model.MItemID).Tables[0];
			int count = dataTable.Rows.Count;
			if (count > 0)
			{
				DataRow dataRow = dataTable.Rows[0];
				sECInviteUserInfoModel.MItemID = model.MItemID;
				sECInviteUserInfoModel.MFirstName = dataRow["MFristName"].ToString();
				sECInviteUserInfoModel.MLastName = dataRow["MLastName"].ToString();
				sECInviteUserInfoModel.MEmail = dataRow["MEmailAddress"].ToString();
				sECInviteUserInfoModel.MPosition = dataRow["MPosition"].ToString();
				sECInviteUserInfoModel.MRole = dataRow["MRole"].ToString();
				sECInviteUserInfoModel.MPermStatus = GetPermStatusByFormmat(dataRow["MUserIsActive"].ToString());
				sECInviteUserInfoModel.MIsArchive = dataRow["MIsArchive"].ToString();
				sECInviteUserInfoModel.IsSelfData = Convert.ToBoolean(dataRow["IsSelfData"]);
				string[] source = new string[2]
				{
					"General_Ledger",
					"Other_Reports"
				};
				for (int i = 0; i < count; i++)
				{
					SECPermisionGrpOperateModel sECPermisionGrpOperateModel = new SECPermisionGrpOperateModel();
					sECPermisionGrpOperateModel.GroupID = dataTable.Rows[i]["MPermGrpID"].ToString();
					if (ctx.MEnabledModules.Contains(ModuleEnum.GL) || !source.Contains(sECPermisionGrpOperateModel.GroupID))
					{
						sECPermisionGrpOperateModel.View = Convert.ToBoolean(dataTable.Rows[i]["MView"]);
						sECPermisionGrpOperateModel.Change = Convert.ToBoolean(dataTable.Rows[i]["MChange"]);
						sECPermisionGrpOperateModel.Approve = Convert.ToBoolean(dataTable.Rows[i]["MApprove"]);
						sECPermisionGrpOperateModel.Export = Convert.ToBoolean(dataTable.Rows[i]["MExport"]);
						sECInviteUserInfoModel.MGrpOperateList.Add(sECPermisionGrpOperateModel);
					}
				}
			}
			return sECInviteUserInfoModel;
		}

		public static SECInviteUserInfoModel GetUserInviteInfo(MContext ctx, string userId)
		{
			SECInviteUserInfoModel sECInviteUserInfoModel = new SECInviteUserInfoModel();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("SELECT t1.MItemID,t1L.MFristName,t1L.MLastName,t1.MEmailAddress,t1.MIsTemp ");
			stringBuilder.AppendLine("FROM T_Sec_User t1 ");
			stringBuilder.AppendLine("join( ");
			stringBuilder.AppendLine(GetUserMultiSql());
			stringBuilder.AppendLine(") t1L on t1.MItemID=t1L.MParentID ");
			stringBuilder.AppendLine("where t1.MItemID=@MItemID and t1.MIsDelete=0 ");
			MySqlParameter[] array = new MySqlParameter[2]
			{
				new MySqlParameter("@MItemID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 6)
			};
			array[0].Value = userId;
			array[1].Value = ctx.MLCID;
			DataSet dataSet = DbHelperMySQL.Query(stringBuilder.ToString(), array);
			if (dataSet.Tables[0].Rows.Count > 0)
			{
				DataRow dataRow = dataSet.Tables[0].Rows[0];
				sECInviteUserInfoModel.MItemID = userId;
				sECInviteUserInfoModel.MFirstName = dataRow["MFristName"].ToString();
				sECInviteUserInfoModel.MLastName = dataRow["MLastName"].ToString();
				sECInviteUserInfoModel.MEmail = dataRow["MEmailAddress"].ToString();
				sECInviteUserInfoModel.MIsTemp = Convert.ToBoolean(dataRow["MIsTemp"]);
			}
			return sECInviteUserInfoModel;
		}

		private static bool ExistsOrgUserByEmail(string OrgId, string Email, string UserId)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(" select count(1) from T_Sec_OrgUser t1");
			stringBuilder.Append(" join T_Sec_User t2 on t1.MUserID=t2.MItemID and t2.MIsDelete=0");
			stringBuilder.Append(" where t1.MOrgID=@MOrgID and t2.MItemID<>@UserId and t2.MEmailAddress=@Email and t1.MIsDelete=0 ");
			MySqlParameter[] array = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@UserId", MySqlDbType.VarChar, 36),
				new MySqlParameter("@Email", MySqlDbType.VarChar, 36)
			};
			array[0].Value = OrgId;
			array[1].Value = (string.IsNullOrWhiteSpace(UserId) ? "" : UserId);
			array[2].Value = Email;
			return DbHelperMySQL.Exists(stringBuilder.ToString(), array);
		}

		public static bool ExistsOrgUser(string OrgId, string UserId)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(" select count(1) from T_Sec_OrgUser t1");
			stringBuilder.Append(" join T_Sec_User t2 on t1.MUserID=t2.MItemID ");
			stringBuilder.Append(" where t1.MOrgID=@MOrgID and t2.MItemID=@UserId and t1.MIsDelete=0 and t2.MIsDelete=0");
			MySqlParameter[] array = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@UserId", MySqlDbType.VarChar, 36)
			};
			array[0].Value = OrgId;
			array[1].Value = UserId;
			return DbHelperMySQL.Exists(stringBuilder.ToString(), array);
		}

		private static bool ExistsTempUserById(string UserId)
		{
			return ModelInfoManager.ExistsByFilter<SECUserModel>(new MContext
			{
				IsSys = true
			}, new SqlWhere().Equal("MItemID", UserId).Equal("MIsTemp", 1));
		}

		private static bool ExistsUserByEmail(string Email)
		{
			return ModelInfoManager.ExistsByFilter<SECUserModel>(new MContext
			{
				IsSys = true
			}, new SqlWhere().AddDeleteFilter("MIsDelete", SqlOperators.Equal, false).Equal("MEmailAddress", Email).Equal("MIsTemp", 0));
		}

		private static bool ActiveOfOrgUser(string OrgId, string UserId)
		{
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.Equal("MOrgID", OrgId);
			sqlWhere.Equal("MUserID", UserId);
			sqlWhere.Equal("MUserIsActive", 1);
			sqlWhere.Equal("MIsDelete", 0);
			return ModelInfoManager.ExistsByFilter<SECOrgUserModel>(new MContext
			{
				IsSys = true
			}, sqlWhere);
		}

		private static string GetPermStatusByFormmat(string isActive)
		{
			if (isActive == "1" || isActive.ToLower() == "true")
			{
				return "Active";
			}
			return "Pending";
		}

		private static List<CommandInfo> GetSecOrgUserUpdCmd(MContext ctx, SECInviteUserInfoModel model)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			SECUserModel dataEditModel = ModelInfoManager.GetDataEditModel<SECUserModel>(ctx, model.MItemID, false, true);
			dataEditModel.MEmailAddress = model.MEmail;
			SECUserRepository.setMultiField(dataEditModel, ctx.MLCID, "MFristName", model.MFirstName);
			SECUserRepository.setMultiField(dataEditModel, ctx.MLCID, "MLastName", model.MLastName);
			new SECUserRepository().HandleUserModelMultiLanguage(dataEditModel);
			list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<SECUserModel>(ctx, dataEditModel, new List<string>
			{
				"MEmailAddress",
				"MFristName",
				"MLastName",
				"MModifyDate"
			}, true));
			SECOrgUserModel sECOrgUserModel = ModelInfoManager.GetDataModelList<SECOrgUserModel>(ctx, new SqlWhere().AddDeleteFilter("MIsDelete", SqlOperators.Equal, false).Equal("MOrgID", ctx.MOrgID).Equal("MUserID", model.MItemID), false, false).FirstOrDefault();
			sECOrgUserModel.MPosition = model.MPosition;
			sECOrgUserModel.MRole = model.MRole;
			sECOrgUserModel.IsSelfData = model.IsSelfData;
			if (sECOrgUserModel.MUserIsActive)
			{
				model.MPermStatus = "Active";
			}
			list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<SECOrgUserModel>(ctx, sECOrgUserModel, new List<string>
			{
				"MPosition",
				"MRole",
				"IsSelfData"
			}, true));
			return list;
		}

		private static List<CommandInfo> GetAcceptSecUserUpdCmd(MContext ctx, SECInviteUserInfoModel model)
		{
			SECUserModel dataEditModel = ModelInfoManager.GetDataEditModel<SECUserModel>(ctx, model.MItemID, false, true);
			dataEditModel.MEmailAddress = model.MEmail;
			dataEditModel.MPassWord = model.MPassword;
			dataEditModel.MIsTemp = false;
			SECUserRepository.setMultiField(dataEditModel, ctx.MLCID, "MFristName", model.MFirstName);
			SECUserRepository.setMultiField(dataEditModel, ctx.MLCID, "MLastName", model.MLastName);
			new SECUserRepository().HandleUserModelMultiLanguage(dataEditModel);
			return ModelInfoManager.GetInsertOrUpdateCmd<SECUserModel>(ctx, dataEditModel, new List<string>
			{
				"MEmailAddress",
				"MPassWord",
				"MIsTemp",
				"MFristName",
				"MLastName"
			}, true);
		}

		private static List<SECPermisionItemGrpModel> GetFinalItemGrpModelList(MContext ctx, List<SECPermisionGrpOperateModel> GrpOperateList)
		{
			List<SECPermisionItemGrpModel> list = new List<SECPermisionItemGrpModel>();
			SECPermisionItemGrpRepository sECPermisionItemGrpRepository = new SECPermisionItemGrpRepository();
			SqlWhere filter = new SqlWhere().AddFilter("MGroupID", SqlOperators.In, (from s in GrpOperateList
			select s.GroupID).ToList());
			List<SECPermisionItemGrpModel> modelList = sECPermisionItemGrpRepository.GetModelList(ctx, filter, false);
			foreach (SECPermisionGrpOperateModel GrpOperate in GrpOperateList)
			{
				List<string> list2 = new List<string>();
				if (GrpOperate.View)
				{
					list2.Add("View");
				}
				if (GrpOperate.Change)
				{
					list2.Add("Change");
				}
				if (GrpOperate.Approve)
				{
					list2.Add("Approve");
				}
				if (GrpOperate.Export)
				{
					list2.Add("Export");
				}
				List<SECPermisionItemGrpModel> list3 = (from w in modelList
				where w.MGroupID == GrpOperate.GroupID
				select w).ToList();
				foreach (SECPermisionItemGrpModel item in list3)
				{
					foreach (string item2 in list2)
					{
						SECPermisionItemGrpModel sECPermisionItemGrpModel = new SECPermisionItemGrpModel();
						sECPermisionItemGrpModel.MGroupID = GrpOperate.GroupID;
						sECPermisionItemGrpModel.MBizObjID = item.MBizObjID;
						sECPermisionItemGrpModel.MPermItemID = item2;
						list.Add(sECPermisionItemGrpModel);
					}
				}
			}
			return list;
		}

		private static bool HaveMoreSysManager(MContext ctx, string userId)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("select count(1) from T_Sec_RoleObjectGroup t1");
			stringBuilder.AppendLine("join T_Sec_Role t2 on t1.MRoleID=t2.MItemID and t2.MOrgID=@MOrgID and t1.MIsDelete=0 and t2.MIsDelete=0");
			stringBuilder.AppendLine("join T_Sec_RoleUser t3 on t2.MItemID=t3.MRoleID");
			stringBuilder.AppendLine("join T_Sec_OrgUser t4 on t3.MUserID=t4.MUserID and t4.MUserIsActive=1");
			stringBuilder.AppendLine("where t4.MOrgID=@MOrgID and t4.MUserID<>@MUserID");
			stringBuilder.AppendLine("and t1.MPermGrpID='System_Settings' and t1.MChange=1 ");
			stringBuilder.AppendLine("and t1.MIsDelete=0 and t2.MIsDelete=0 and t3.MIsDelete=0 and t4.MIsDelete=0");
			MySqlParameter[] array = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MUserID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = userId;
			return DbHelperMySQL.Exists(stringBuilder.ToString(), array);
		}

		public static OperationResult AcceptInvite(MContext ctx, SECInviteUserInfoModel model)
		{
			OperationResult operationResult = new OperationResult();
			ctx = GetNewContext();
			ctx.MOrgID = model.MOrgID;
			if (ActiveOfOrgUser(model.MOrgID, model.MItemID))
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "InviteLinkActived", "this invite link has been actived.");
				return operationResult;
			}
			if (!ExistsTempUserById(model.MItemID))
			{
				operationResult.Success = false;
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "InviteLinkInvalid", "this invite link is invalid.")
				});
				return operationResult;
			}
			if (ExistsUserByEmail(model.MEmail))
			{
				operationResult.Success = false;
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "EmailRegistered", "this Email is already registered.")
				});
				return operationResult;
			}
			List<CommandInfo> list = new List<CommandInfo>();
			list.AddRange(GetAcceptSecUserUpdCmd(ctx, model));
			List<string> list2 = (from s in ModelInfoManager.GetDataModelList<SECOrgUserModel>(ctx, new SqlWhere().Equal("MOrgID", model.MOrgID).Equal("MUserID", model.MItemID), false, false)
			select s.MItemID).ToList();
			foreach (string item in list2)
			{
				list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<SECOrgUserModel>(ctx, new SECOrgUserModel
				{
					MItemID = item,
					MUserIsActive = true
				}, new List<string>
				{
					"MUserIsActive"
				}, true));
			}
			if (DbHelperMySQL.ExecuteSqlTran(ctx, list) > 0)
			{
				DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
				int num = dynamicDbHelperMySQL.ExecuteSqlTran(list);
				if (num > 0)
				{
					SECSendLinkInfoRepository.DeleteLink(model.SendLinkID);
					return operationResult;
				}
			}
			operationResult.VerificationInfor.Add(new BizVerificationInfor
			{
				Level = AlertEnum.Error,
				Message = COMMultiLangRepository.GetText(model.MContext.MLCID, LangModule.User, "ErrorInvite", "error for invite.")
			});
			return operationResult;
		}

		public static bool LoginForAcceptInvite(MContext context, string userId, string orgId, string sendLinkID, string newUserId, string loginEmail)
		{
			if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(orgId))
			{
				return false;
			}
			SECUserRepository sECUserRepository = new SECUserRepository();
			SECUserModel dataModel = sECUserRepository.GetDataModel(context, userId, false);
			if (dataModel.MEmailAddress != loginEmail)
			{
				return false;
			}
			MContext newContext = GetNewContext();
			newContext.MOrgID = orgId;
			if (ActiveOfOrgUser(orgId, newUserId))
			{
				SECSendLinkInfoRepository.DeleteLink(sendLinkID);
				return true;
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(newContext);
			List<CommandInfo> list = new List<CommandInfo>();
			List<string> list2 = (from s in ModelInfoManager.GetDataModelList<SECOrgUserModel>(newContext, new SqlWhere().Equal("MOrgID", orgId).Equal("MUserID", userId), false, false)
			select s.MItemID).ToList();
			foreach (string item in list2)
			{
				list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<SECOrgUserModel>(newContext, new SECOrgUserModel
				{
					MItemID = item,
					MUserID = newUserId,
					MUserIsActive = true
				}, new List<string>
				{
					"MUserID",
					"MUserIsActive"
				}, true));
			}
			string sql = "select ru.MitemID from t_sec_roleuser ru \r\n                            inner join t_sec_role r on (ru.MRoleID=r.MitemID) \r\n                            where r.MOrgID=@OrgID and ru.MUserID=@UserID and ru.MIsDelete=0 and r.MIsDelete=0";
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@OrgID", MySqlDbType.VarChar, 36)
				{
					Value = orgId
				},
				new MySqlParameter("@UserID", MySqlDbType.VarChar, 36)
				{
					Value = userId
				}
			};
			List<string> list3 = (from ru in ModelInfoManager.GetDataModelBySql<SECRoleUserModel>(newContext, sql, cmdParms)
			select ru.MItemID).ToList();
			foreach (string item2 in list3)
			{
				list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<SECRoleUserModel>(newContext, new SECRoleUserModel
				{
					MItemID = item2,
					MUserID = newUserId,
					MOrgID = newContext.MOrgID
				}, new List<string>
				{
					"MUserID"
				}, true));
			}
			MultiDBCommand[] array = new MultiDBCommand[2]
			{
				new MultiDBCommand(newContext)
				{
					DBType = SysOrBas.Sys,
					CommandList = new List<CommandInfo>()
				},
				null
			};
			array[0].CommandList.AddRange(list);
			array[0].CommandList.Add(SECSendLinkInfoRepository.GetDeleteJoinLink(sendLinkID));
			array[1] = new MultiDBCommand(newContext)
			{
				DBType = SysOrBas.Bas,
				CommandList = new List<CommandInfo>()
			};
			array[1].CommandList.AddRange(list);
			newContext.MUserID = newUserId;
			List<CommandInfo> collection = UserCopyToDBServer(newContext);
			array[1].CommandList.AddRange(collection);
			return DbHelperMySQL.ExecuteSqlTran(context, array);
		}

		public static List<SECUserRoleInitModel> GetSmartUserRoleInitModel(MContext ctx)
		{
			List<SECUserRoleInitModel> list = new List<SECUserRoleInitModel>
			{
				new SECUserRoleInitModel
				{
					Id = "Bank",
					MPosition = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "Bank", "Bank"),
					MView = false,
					MChange = false,
					MApprove = false,
					MExport = false,
					children = new List<SECUserRoleInitModel>
					{
						new SECUserRoleInitModel
						{
							Id = "Bank_Reconciliation",
							MPosition = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "BankFeedsOperation", "对帐单操作"),
							MView = false,
							MChange = false,
							MApprove = false,
							MExport = false
						},
						new SECUserRoleInitModel
						{
							Id = "Bank_ExceptReconciliation",
							MPosition = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "BankFeedsOperationOther", "银行(除对帐单操作)"),
							MView = false,
							MChange = false,
							MApprove = false,
							MExport = false
						}
					}
				},
				new SECUserRoleInitModel
				{
					Id = "Contacts",
					MPosition = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "Contacts", "Contacts"),
					MView = false,
					MChange = false,
					MApprove = false,
					MExport = false
				},
				new SECUserRoleInitModel
				{
					Id = "Attachment",
					MPosition = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "Attachment", "Attachment"),
					MView = false,
					MChange = false,
					MApprove = false,
					MExport = false
				},
				new SECUserRoleInitModel
				{
					Id = "Fixed_Assets",
					MPosition = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "FixedAssets", "Fixed Assets"),
					MView = false,
					MChange = false,
					MApprove = false,
					MExport = false
				},
				new SECUserRoleInitModel
				{
					Id = "General_Ledger",
					MPosition = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "General Ledger", "General Ledger"),
					MView = false,
					MChange = false,
					MApprove = false,
					MExport = false
				},
				new SECUserRoleInitModel
				{
					Id = "Fapiao",
					MPosition = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "Fapiao", "发票"),
					MView = false,
					MChange = false,
					MApprove = false,
					MExport = false,
					children = new List<SECUserRoleInitModel>
					{
						new SECUserRoleInitModel
						{
							Id = "Sales_Fapiao",
							MPosition = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "SalesFapiao", "销售发票"),
							MView = false,
							MChange = false,
							MApprove = false,
							MExport = false
						},
						new SECUserRoleInitModel
						{
							Id = "Purchases_Fapiao",
							MPosition = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "PurchasesFapiao", "采购发票"),
							MView = false,
							MChange = false,
							MApprove = false,
							MExport = false
						}
					}
				},
				new SECUserRoleInitModel
				{
					Id = "Reports",
					MPosition = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "Reports", "Reports"),
					MView = false,
					MChange = false,
					MApprove = false,
					MExport = false,
					children = new List<SECUserRoleInitModel>
					{
						new SECUserRoleInitModel
						{
							Id = "Other_Reports",
							MPosition = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "GeneralReports", "General Reports"),
							MView = false,
							MChange = false,
							MApprove = false,
							MExport = false
						},
						new SECUserRoleInitModel
						{
							Id = "Fixed_Assets_Reports",
							MPosition = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "FixedAssetsReports", "固定资产报表"),
							MView = false,
							MChange = false,
							MApprove = false,
							MExport = false
						}
					}
				},
				new SECUserRoleInitModel
				{
					Id = "Edit_Settings",
					MPosition = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "EditSettings", "Edit Settings"),
					MView = false,
					MChange = false,
					MApprove = false,
					MExport = false
				},
				new SECUserRoleInitModel
				{
					Id = "System_Settings",
					MPosition = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "SystemSettings", "System Settings"),
					MView = false,
					MChange = false,
					MApprove = false,
					MExport = false
				},
				new SECUserRoleInitModel
				{
					Id = "Excel_Plus_Download",
					MPosition = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "ExcelPlusDown", "Excel 工具下载"),
					MView = false,
					MChange = false,
					MApprove = false,
					MExport = false
				},
				new SECUserRoleInitModel
				{
					Id = "Migration_Tool_Download",
					MPosition = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "MigrationToolDown", "迁移工具下载"),
					MView = false,
					MChange = false,
					MApprove = false,
					MExport = false
				}
			};
			if (ctx.MFABeginDate == DateTime.MinValue)
			{
				list.Remove(list.SingleOrDefault((SECUserRoleInitModel f) => f.Id == "Fixed_Assets"));
				List<SECUserRoleInitModel> children = list.SingleOrDefault((SECUserRoleInitModel f) => f.Id == "Reports").children;
				children.Remove(children.SingleOrDefault((SECUserRoleInitModel f) => f.Id == "Fixed_Assets_Reports"));
			}
			return list;
		}

		public static List<SECUserRoleInitModel> GetBasicUserRoleInitModel(MContext ctx)
		{
			List<SECUserRoleInitModel> list = new List<SECUserRoleInitModel>
			{
				new SECUserRoleInitModel
				{
					Id = "Bank",
					MPosition = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "Bank", "Bank"),
					MView = false,
					MChange = false,
					MApprove = false,
					MExport = false,
					children = new List<SECUserRoleInitModel>
					{
						new SECUserRoleInitModel
						{
							Id = "Bank_Reconciliation",
							MPosition = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "BankReconciliation", "Bank reconciliation"),
							MView = false,
							MChange = false,
							MApprove = false,
							MExport = false
						},
						new SECUserRoleInitModel
						{
							Id = "Bank_ExceptReconciliation",
							MPosition = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "BankExceptReconciliation)", "Bank(except reconciliation)"),
							MView = false,
							MChange = false,
							MApprove = false,
							MExport = false
						}
					}
				},
				new SECUserRoleInitModel
				{
					Id = "Sales",
					MPosition = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "Sales", "Sales"),
					MView = false,
					MChange = false,
					MApprove = false,
					MExport = false
				},
				new SECUserRoleInitModel
				{
					Id = "Purchases",
					MPosition = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "Purchases", "Purchases"),
					MView = false,
					MChange = false,
					MApprove = false,
					MExport = false
				},
				new SECUserRoleInitModel
				{
					Id = "Expense_Claims",
					MPosition = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "ExpenseClaims", "Expense Claims"),
					MView = false,
					MChange = false,
					MApprove = false,
					MExport = false
				},
				new SECUserRoleInitModel
				{
					Id = "Contacts",
					MPosition = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "Contacts", "Contacts"),
					MView = false,
					MChange = false,
					MApprove = false,
					MExport = false
				},
				new SECUserRoleInitModel
				{
					Id = "Attachment",
					MPosition = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "Attachment", "Attachment"),
					MView = false,
					MChange = false,
					MApprove = false,
					MExport = false
				},
				new SECUserRoleInitModel
				{
					Id = "Fixed_Assets",
					MPosition = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "FixedAssets", "Fixed Assets"),
					MView = false,
					MChange = false,
					MApprove = false,
					MExport = false
				},
				new SECUserRoleInitModel
				{
					Id = "Payroll_Admin",
					MPosition = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "PayrollAdmin", "Payroll Admin"),
					MView = false,
					MChange = false,
					MApprove = false,
					MExport = false
				},
				new SECUserRoleInitModel
				{
					Id = "General_Ledger",
					MPosition = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "General Ledger", "General Ledger"),
					MView = false,
					MChange = false,
					MApprove = false,
					MExport = false
				},
				new SECUserRoleInitModel
				{
					Id = "Fapiao",
					MPosition = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "Fapiao", "发票"),
					MView = false,
					MChange = false,
					MApprove = false,
					MExport = false,
					children = new List<SECUserRoleInitModel>
					{
						new SECUserRoleInitModel
						{
							Id = "Sales_Fapiao",
							MPosition = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "SalesFapiao", "销售发票"),
							MView = false,
							MChange = false,
							MApprove = false,
							MExport = false
						},
						new SECUserRoleInitModel
						{
							Id = "Purchases_Fapiao",
							MPosition = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "PurchasesFapiao", "采购发票"),
							MView = false,
							MChange = false,
							MApprove = false,
							MExport = false
						}
					}
				},
				new SECUserRoleInitModel
				{
					Id = "Reports",
					MPosition = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "Reports", "Reports"),
					MView = false,
					MChange = false,
					MApprove = false,
					MExport = false,
					children = new List<SECUserRoleInitModel>
					{
						new SECUserRoleInitModel
						{
							Id = "Sale_Reports",
							MPosition = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "SalesReports", "Sales Reports"),
							MView = false,
							MChange = false,
							MApprove = false,
							MExport = false
						},
						new SECUserRoleInitModel
						{
							Id = "Purchase_Reports",
							MPosition = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "PurchasesReports", "Purchases Reports"),
							MView = false,
							MChange = false,
							MApprove = false,
							MExport = false
						},
						new SECUserRoleInitModel
						{
							Id = "Expense_Reports",
							MPosition = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "ExpenseReports", "Expense Reports"),
							MView = false,
							MChange = false,
							MApprove = false,
							MExport = false
						},
						new SECUserRoleInitModel
						{
							Id = "Bank_Reports",
							MPosition = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "BankReports", "Bank Reports"),
							MView = false,
							MChange = false,
							MApprove = false,
							MExport = false
						},
						new SECUserRoleInitModel
						{
							Id = "PayRun_Reports",
							MPosition = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "PayRunReports", "Pay Run Reports"),
							MView = false,
							MChange = false,
							MApprove = false,
							MExport = false
						},
						new SECUserRoleInitModel
						{
							Id = "Other_Reports",
							MPosition = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "GeneralReports", "General Reports"),
							MView = false,
							MChange = false,
							MApprove = false,
							MExport = false
						},
						new SECUserRoleInitModel
						{
							Id = "Fixed_Assets_Reports",
							MPosition = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "FixedAssetsReports", "固定资产报表"),
							MView = false,
							MChange = false,
							MApprove = false,
							MExport = false
						}
					}
				},
				new SECUserRoleInitModel
				{
					Id = "Edit_Settings",
					MPosition = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "EditSettings", "Edit Settings"),
					MView = false,
					MChange = false,
					MApprove = false,
					MExport = false
				},
				new SECUserRoleInitModel
				{
					Id = "System_Settings",
					MPosition = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.User, "SystemSettings", "System Settings"),
					MView = false,
					MChange = false,
					MApprove = false,
					MExport = false
				},
				new SECUserRoleInitModel
				{
					Id = "Excel_Plus_Download",
					MPosition = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "ExcelPlusDown", "Excel 工具下载"),
					MView = false,
					MChange = false,
					MApprove = false,
					MExport = false
				},
				new SECUserRoleInitModel
				{
					Id = "Migration_Tool_Download",
					MPosition = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "MigrationToolDown", "迁移工具下载"),
					MView = false,
					MChange = false,
					MApprove = false,
					MExport = false
				}
			};
			if (!ctx.MEnabledModules.Contains(ModuleEnum.GL))
			{
				list.Remove(list.SingleOrDefault((SECUserRoleInitModel f) => f.Id == "General_Ledger"));
				List<SECUserRoleInitModel> children = list.SingleOrDefault((SECUserRoleInitModel f) => f.Id == "Reports").children;
				children.Remove(children.SingleOrDefault((SECUserRoleInitModel f) => f.Id == "Other_Reports"));
			}
			if (ctx.MFABeginDate == DateTime.MinValue)
			{
				list.Remove(list.SingleOrDefault((SECUserRoleInitModel f) => f.Id == "Fixed_Assets"));
				List<SECUserRoleInitModel> children2 = list.SingleOrDefault((SECUserRoleInitModel f) => f.Id == "Reports").children;
				children2.Remove(children2.SingleOrDefault((SECUserRoleInitModel f) => f.Id == "Fixed_Assets_Reports"));
			}
			return list;
		}

		public static List<SECUserRoleInitModel> GetUserRoleInitModel(MContext ctx)
		{
			if (ctx.MOrgVersionID == 1)
			{
				return GetSmartUserRoleInitModel(ctx);
			}
			return GetBasicUserRoleInitModel(ctx);
		}

		public static bool ContainPermGrp(MContext ctx, string grpKey, string permItem = "")
		{
			return false;
		}

		public static bool IsOnlySelfData(MContext ctx)
		{
			return true;
		}

		public static List<CommandInfo> UserCopyToDBServer(MContext ctx)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			if (ctx.MActiveLocaleIDS == null || ctx.MActiveLocaleIDS.Count != COMMultiLangRepository.MegiLangTypes.Count())
			{
				ctx.MActiveLocaleIDS = COMMultiLangRepository.MegiLangTypes.ToList();
			}
			ctx.IsSys = true;
			SECUserModel dataEditModel = ModelInfoManager.GetDataEditModel<SECUserModel>(ctx, ctx.MUserID, false, true);
			ctx.IsSys = false;
			new SECUserRepository().HandleUserModelMultiLanguage(dataEditModel);
			return ModelInfoManager.GetInsertOrUpdateCmd<SECUserModel>(ctx, dataEditModel, null, true);
		}

		public static List<CommandInfo> UserCopyToDBServer(MContext ctx, string connectionString)
		{
			List<CommandInfo> result = new List<CommandInfo>();
			if (ctx.MActiveLocaleIDS == null || ctx.MActiveLocaleIDS.Count != COMMultiLangRepository.MegiLangTypes.Count())
			{
				ctx.MActiveLocaleIDS = COMMultiLangRepository.MegiLangTypes.ToList();
			}
			ctx.IsSys = true;
			SECUserModel dataEditModel = ModelInfoManager.GetDataEditModel<SECUserModel>(ctx, ctx.MUserID, false, true);
			ctx.IsSys = false;
			if (!ModelInfoManager.ExistsByKey<SECUserModel>(ctx, connectionString, ctx.MUserID, false))
			{
				dataEditModel.IsNew = true;
				new SECUserRepository().HandleUserModelMultiLanguage(dataEditModel);
				result = ModelInfoManager.GetInsertOrUpdateCmd<SECUserModel>(ctx, dataEditModel, null, true);
			}
			return result;
		}

		private static MContext GetNewContext()
		{
			MContext mContext = new MContext();
			mContext.MLCID = "0x0009";
			if (mContext.MActiveLocaleIDS == null || mContext.MActiveLocaleIDS.Count != COMMultiLangRepository.MegiLangTypes.Count())
			{
				mContext.MActiveLocaleIDS = COMMultiLangRepository.MegiLangTypes.ToList();
			}
			return mContext;
		}

		public static List<SECMenuPermissionModel> GetGrantMenuPermissionList(MContext ctx)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(" select distinct * ");
			stringBuilder.AppendLine(" from( ");
			stringBuilder.AppendLine(" select t4.MBizObjectID,t4.MPermItemID ");
			stringBuilder.AppendLine(" from T_Sec_User t1 ");
			stringBuilder.AppendLine(" join T_Sec_OrgUser t2 on t1.MItemID=t2.MUserID and t2.MOrgID=@MOrgID ");
			stringBuilder.AppendLine(" join T_Sec_RoleUser t3 on t1.MItemID=t3.MUserID ");
			stringBuilder.AppendLine(" join T_Sec_RolePermission t4 on t3.MRoleID=t4.MRoleID and t4.MIsGrant=1 and t4.MIsRefuset=0 ");
			stringBuilder.AppendLine(" where t1.MIsDelete=0 and t1.MIsTemp=0 and t1.MItemID=@MUserID and t2.MIsDelete=0 and t3.MIsDelete=0 and t4.MIsDelete=0");
			stringBuilder.AppendLine(" union all ");
			stringBuilder.AppendLine(" select t1.MBizObjID,t1.MPermItemID ");
			stringBuilder.AppendLine(" from T_Bas_Menu t1 ");
			stringBuilder.AppendLine(" join T_Bas_Module t2 on t1.MModuleID=t2.MID and t2.MIsFree=1 ");
			stringBuilder.AppendLine(" where t1.MIsDelete=0 and t2.MIsDelete=0");
			stringBuilder.AppendLine(" ) t ");
			MySqlParameter[] array = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MUserID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = ctx.MUserID;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return ModelInfoManager.DataTableToList<SECMenuPermissionModel>(dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array).Tables[0]);
		}

		public static SECPermissionEditModel GetRolePermissionEditModel(MContext ctx, string roleID)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(" Select a.MItemID as MRoleID,b.MName as MRoleName ");
			stringBuilder.AppendLine(" From T_Sec_Role a ");
			stringBuilder.AppendLine(" Left Join T_Sec_Role_l b on a.MNumber=b.MParentID And b.MLocaleID=@MLocaleID and a.MIsDelete=0 and b.MIsDelete=0");
			MySqlParameter mySqlParameter = new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 6);
			mySqlParameter.Value = ctx.MLCID;
			List<SECPermissionEditModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<SECPermissionEditModel>(ctx, stringBuilder.ToString(), new MySqlParameter[1]
			{
				mySqlParameter
			});
			if (dataModelBySql == null || dataModelBySql.Count == 0)
			{
				throw new Exception("角色不存在");
			}
			SECPermissionEditModel sECPermissionEditModel = dataModelBySql[0];
			stringBuilder.Clear();
			stringBuilder.AppendLine(" Select a.MItemID as MBizObjectID,b.MName as MBizObjectName, ");
			stringBuilder.AppendLine(" d.MItemID as MPermissionItemID,d.MNumber as MPermissionItemNumber,e.MName as MPermissionItemName ");
			stringBuilder.AppendLine(" From T_Sec_ObjectPermission a ");
			stringBuilder.AppendLine(" Left Join T_Sec_ObjectPermission_l b on a.MItemID=b.MParentID And b.MLocaleID=@MLocaleID and a.MIsDelete=0 and b.MIsDelete=0 ");
			stringBuilder.AppendLine(" inner Join T_Sec_ObjectPermEntry c on a.MItemID=c.MItemID and c.MIsDelete=0 ");
			stringBuilder.AppendLine(" inner Join T_Sec_PermisionItem d on c.MItemID=d.MItemID and d.MIsDelete=0");
			stringBuilder.AppendLine(" Left Join T_Sec_PermisionItem_l e on d.MItemID=e.MParentID And e.MLocaleID=@MLocaleID and e.MIsDelete=0 ");
			stringBuilder.AppendLine(" Order By b.MName,d.MNumber ");
			sECPermissionEditModel.BizObjects = ModelInfoManager.GetDataModelBySql<ObjectPermissionModel>(ctx, stringBuilder.ToString(), new MySqlParameter[1]
			{
				mySqlParameter
			});
			stringBuilder.Clear();
			stringBuilder.AppendLine(" select MBizObjectID,MPermItemID,MIsGrant,MIsRefuset from T_Sec_RolePermission a ");
			stringBuilder.AppendLine(" Where a.MRoleID='" + roleID + "' and MIsDelete=0 ");
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataTable dataTable = dynamicDbHelperMySQL.Query(stringBuilder.ToString()).Tables[0];
			foreach (DataRow row in dataTable.Rows)
			{
				ObjectPermissionModel objectPermissionModel = sECPermissionEditModel.BizObjects.FirstOrDefault((ObjectPermissionModel f) => f.MBizObjectID.Equals(row["MBizObjectID"].ToString(), StringComparison.OrdinalIgnoreCase) && f.MPermissionItemID.Equals(row["MPermItemID"].ToString(), StringComparison.OrdinalIgnoreCase));
				if (objectPermissionModel != null)
				{
					objectPermissionModel.MIsGrant = (bool)row["MIsGrant"];
					objectPermissionModel.MIsRefuset = (bool)row["MIsRefuset"];
				}
			}
			return sECPermissionEditModel;
		}

		public static bool PermissionGrant(MContext ctx, SECPermissionEditModel model)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			CommandInfo commandInfo = new CommandInfo();
			commandInfo.CommandText = $"update t_sec_rolepermission set MIsDelete=1 and MRoleID='{model.MRoleID}'";
			commandInfo.Parameters = null;
			list.Add(commandInfo);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("insert into T_Sec_RolePermission(");
			stringBuilder.Append("MItemID,MRoleID,MBizObjectID,MPermItemID,MIsGrant,MIsRefuset");
			stringBuilder.Append(") values (");
			stringBuilder.Append("@MItemID,@MRoleID,@MBizObjectID,@MPermItemID,@MIsGrant,@MIsRefuset");
			stringBuilder.Append(") ");
			foreach (ObjectPermissionModel bizObject in model.BizObjects)
			{
				MySqlParameter[] array = new MySqlParameter[6]
				{
					new MySqlParameter("@MItemID", MySqlDbType.VarChar, 36),
					new MySqlParameter("@MRoleID", MySqlDbType.VarChar, 36),
					new MySqlParameter("@MBizObjectID", MySqlDbType.VarChar, 36),
					new MySqlParameter("@MPermItemID", MySqlDbType.VarChar, 36),
					new MySqlParameter("@MIsGrant", MySqlDbType.Bit),
					new MySqlParameter("@MIsRefuset", MySqlDbType.Bit)
				};
				array[0].Value = UUIDHelper.GetGuid();
				array[1].Value = model.MRoleID;
				array[2].Value = bizObject.MBizObjectID;
				array[3].Value = bizObject.MPermissionItemID;
				array[4].Value = bizObject.MIsGrant;
				array[5].Value = bizObject.MIsRefuset;
				CommandInfo item = new CommandInfo(stringBuilder.ToString(), array);
				list.Add(item);
			}
			int num = DbHelperMySQL.ExecuteSqlTran(ctx, list);
			return num > 0;
		}

		public static OperationResult ArchiveUser(MContext ctx, string userId, int status)
		{
			string text = "update t_sec_orguser set MIsArchive = @MIsArchive where MUserID=@MUserID and MOrgID=@MOrgID";
			MySqlParameter[] array = new MySqlParameter[3]
			{
				new MySqlParameter("@MIsArchive", MySqlDbType.Int32),
				new MySqlParameter("@MUserID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = status;
			array[1].Value = userId;
			array[2].Value = ctx.MOrgID;
			int num = DbHelperMySQL.ExecuteSql(ctx, text, array);
			if (num > 0)
			{
				DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
				num = dynamicDbHelperMySQL.ExecuteSql(text, array);
			}
			OperationResult operationResult = new OperationResult();
			operationResult.Success = (num > 0);
			return operationResult;
		}

		public static bool UserIsArchiveInOrg(MContext ctx, string userId)
		{
			string sQLString = "select * from t_sec_orguser where MIsArchive=@MIsArchive and MUserID=@MUserID and MOrgID=@MOrgID and MIsDelete=0 ";
			MySqlParameter[] array = new MySqlParameter[3]
			{
				new MySqlParameter("@MIsArchive", MySqlDbType.Int32),
				new MySqlParameter("@MUserID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = 1;
			array[1].Value = userId;
			array[2].Value = ctx.MOrgID;
			DataSet dataSet = DbHelperMySQL.Query(sQLString, array);
			if (dataSet != null && dataSet.Tables[0] != null && dataSet.Tables[0].Rows.Count > 0)
			{
				return true;
			}
			return false;
		}

		public static SECOrgUserModel GetUserOrgModel(MContext ctx, string userId)
		{
			SECOrgUserModel result = null;
			string sQLString = "SELECT * FROM t_sec_orguser where MOrgID=@MOrgID and MUserID=@MUserID and MIsDelete=0";
			MySqlParameter[] array = new MySqlParameter[3]
			{
				new MySqlParameter("@MIsArchive", MySqlDbType.Int32),
				new MySqlParameter("@MUserID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = 1;
			array[1].Value = userId;
			array[2].Value = ctx.MOrgID;
			DataSet dataSet = DbHelperMySQL.Query(sQLString, array);
			if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0] != null && dataSet.Tables[0].Rows.Count > 0)
			{
				result = ModelInfoManager.DataTableToList<SECOrgUserModel>(dataSet).FirstOrDefault();
			}
			return result;
		}

		public static List<SECUserPermissionListModel> GetUserPermissionList(MContext ctx)
		{
			return new List<SECUserPermissionListModel>();
		}
	}
}
