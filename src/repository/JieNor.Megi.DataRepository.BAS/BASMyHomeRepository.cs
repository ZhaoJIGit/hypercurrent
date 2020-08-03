using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.DataModel.SYS;
using JieNor.Megi.DataRepository.SEC;
using JieNor.Megi.DataRepository.SYS;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace JieNor.Megi.DataRepository.BAS
{
	public class BASMyHomeRepository
	{
		public List<BASMyHomeModel> GetOrgInfoListByUserID(MContext ctx)
		{
			List<BASMyHomeModel> list = new List<BASMyHomeModel>();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(" select t1.MItemID as MOrgID, t1.MName as MOrgName, t1.MRegProgress,t1.MDefaulLocaleID,t1.MVersionID,t1.MIsBeta,t1.MIsPaid, ");
			stringBuilder.AppendLine("     t1p.MUsedStatusID,  ");
			stringBuilder.AppendLine("     t1p.MLastViewUserID , t1p.MLastViewDate, t1p.MMasterID, ");
			stringBuilder.AppendLine("     F_GetUserName(t3l.MFristName,t3l.MLastName) as MLastViewUserName,t3.MItemID as MUserID,group_concat(t5.MItemID) as MRoleID, t2.MRole as MRoleName");
			stringBuilder.AppendLine(" FROM  ( SELECT MItemID,MName,MRegProgress,MDefaulLocaleID,MIsDelete,MIsActive,MExpiredDate,MIsPaid,MVersionID,MIsBeta ");
			stringBuilder.AppendLine(" FROM T_Bas_Organisation  ");
			stringBuilder.AppendLine(" WHERE ");
			stringBuilder.AppendLine(" MIsDelete = 0  AND DATE_ADD( date(MExpiredDate),INTERVAL 1 DAY) >= @MExpiredDate");
			stringBuilder.AppendLine(" ) t1  ");
			stringBuilder.AppendLine(" left join T_Sec_OrgUser t2 on t1.MItemID = t2.MOrgID and t2.MIsDelete=0 and t2.MUserIsActive=1 and t2.MIsArchive<>@MIsArchive and t2.MIsActive = 1");
			stringBuilder.AppendLine(" INNER join T_Sec_User t3 on t2.MUserID = t3.MItemID and t3.MIsDelete=0 and t3.MIsActive = 1  AND t3.MItemID =@UserID");
			stringBuilder.AppendLine(" left join T_SYS_OrgApp t1p on t1.MItemID = t1p.MOrgID and t1p.MIsDelete=0 and t1p.MIsActive = 1");
			stringBuilder.AppendLine(" left join (select MParentID, MFristName,MLastName , MIsDelete,MIsActive from T_Sec_User_L \r\n                    where IFNULL(MFristName,'') <>'' and IFNULL(MLastName,'')<>'' and MIsDelete=0 and MIsActive=1 group by MParentID) t3l on t3.MItemID = t3l.MParentID and t3l.MIsDelete = 0 and t3l.MIsActive = 1");
			stringBuilder.AppendLine(" left join T_Sec_RoleUser t4 on t3.MItemID=t4.MUserID and t4.MIsDelete=0 and t4.MIsActive = 1 ");
			stringBuilder.AppendLine(" left join T_Sec_Role t5 on t4.MRoleID=t5.MItemID and t5.MIsDelete=0 and t5.MOrgID=t1.MItemID and t5.MIsActive = 1 ");
			stringBuilder.AppendLine(" left join T_Sec_Role_l t5l on t5.MNumber = t5l.MParentID and t5l.MLocaleID=@MLocaleID and t5l.MIsDelete = 0 and t5l.MIsActive = 1");
			stringBuilder.AppendLine(" group by MOrgID, MOrgName,MRegProgress,MDefaulLocaleID,");
			stringBuilder.AppendLine(" MUsedStatusID,MLastViewUserID,MLastViewDate,MMasterID,MLastViewUserName,MUserID");
			stringBuilder.AppendLine(" order by MIsPaid DESC,MOrgName ");
			MySqlParameter[] array = new MySqlParameter[4]
			{
				new MySqlParameter("@UserID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 6),
				new MySqlParameter("@MIsArchive", MySqlDbType.Int32),
				new MySqlParameter("@MExpiredDate", MySqlDbType.DateTime)
			};
			array[0].Value = ctx.MUserID;
			array[1].Value = ctx.MLCID;
			array[2].Value = 1;
			array[3].Value = ctx.DateNow;
			DataTable dt = DbHelperMySQL.Query(stringBuilder.ToString(), array).Tables[0];
			return ModelInfoManager.DataTableToList<BASMyHomeModel>(dt);
		}

		public DataGridJson<BASMyHomeModel> GetOrgInfoListByPage(MContext ctx, BDOrganistationListFilter param)
		{
			string str = "select * from ( \r\n                            SELECT DISTINCT\r\n                                    t1.MItemID AS MOrgID,\r\n                                    t1.MName AS MOrgName,\r\n                                    t3.MitemID as userid,\r\n                                    t1.MVersionID,\r\n                                    t1.MCreateDate,\r\n                                    t1.MRegProgress,\r\n                                    t1.MExpiredDate,\r\n                                    t1.MIsPaid,\r\n                                    t1.MIsBetaUser,\r\n                                    t1.MIsBeta,\r\n                                    t9.MLastViewDate,\r\n                                    F_GETUSERNAME(t5.MFristName, t5.MLastName) AS MLastViewUserName,\r\n                                    t3.MRole AS MRoleName,\r\n                                    t7.MItemID as MRoleID,\r\n                                    case when t8.MIsGrant = 1 then 1 else 0 end AS HasChangePermission\r\n                                FROM\r\n                                    T_Bas_Organisation t1\r\n                                        INNER JOIN\r\n                                    T_Sec_OrgUser t3 ON t1.MItemID = t3.MOrgID\r\n                                        AND t3.MIsDelete = t1.MIsDelete\r\n                                        AND t3.MIsActive = t1.MIsActive\r\n                                        AND t3.MUserIsActive = 1\r\n                                        AND t3.MIsArchive = 0\r\n                                        LEFT JOIN\r\n                                    T_SYS_OrgApp t2 ON t1.MItemID = t2.MOrgID\r\n                                        AND t2.MIsDelete = t1.MIsDelete\r\n                                        AND t2.MIsActive = t1.MIsActive\r\n                                        LEFT JOIN\r\n                                    T_Sec_User_L t5 ON t5.MParentID = t2.MLastViewUserID\r\n                                        AND t5.MIsDelete = t1.MIsDelete\r\n                                        AND t5.MIsActive = t1.MIsActive\r\n                                        AND t5.MLocaleID = @MLocaleID\r\n                                        INNER JOIN\r\n                                    T_Sec_RoleUser t6 ON t6.MUserID = t3.MUserID\r\n\t\t                                and t6.MOrgID = t1.MItemID\r\n                                        AND t6.MIsDelete = t1.MIsDelete\r\n                                        AND t6.MIsActive = t1.MIsActive\r\n                                        INNER JOIN\r\n                                    T_Sec_Role t7 ON t6.MRoleID = t7.MItemID\r\n                                        AND t7.MIsDelete = t1.MIsDelete\r\n                                        AND t7.MIsActive = t1.MIsActive\r\n                                        left JOIN\r\n                                    T_Sec_RolePermission t8 ON t8.MRoleID = t7.MItemID\r\n                                        AND t8.MIsDelete = t1.MIsDelete\r\n                                        AND t8.MIsActive = t1.MIsActive\r\n                                        AND t8.MBizObjectID = @OrgBizObject\r\n                                        AND t8.MPermItemID = @PermissionItem\r\n                                        LEFT JOIN\r\n                                    (SELECT \r\n                                        Muserid, MOrgID, MAX(MloginDate) AS MLastViewDate\r\n                                    FROM\r\n                                        T_User_LoginLog\r\n                                    WHERE\r\n                                        MUserID = @UserID\r\n                                    GROUP BY Muserid , MOrgID) t9 ON t9.MOrgID = t1.MItemID\r\n                                WHERE\r\n                                    t3.MUserID = @UserID\r\n                                        AND ((t1.MIsDemo = 1\r\n                                        AND t1.MMasterID <> @UserID)\r\n                                        OR t1.MIsDemo = 0)\r\n                                        AND t1.MIsDelete = 0\r\n                            ) t";
			MySqlParameter[] cmdParms = new MySqlParameter[4]
			{
				new MySqlParameter("@UserID", ctx.MUserID),
				new MySqlParameter("@OrgBizObject", "Org"),
				new MySqlParameter("@PermissionItem", "Change"),
				new MySqlParameter("@MLocaleID", ctx.MLCID)
			};
			str = ((!string.IsNullOrEmpty(param.Sort)) ? ((!(param.Sort == "MVersionType")) ? (str + $" order by t.{param.Sort} {param.Order}") : (str + $" order by case when date(t.MExpiredDate) < date(NOW()) \r\n                                            then case when t.MIsPaid then 5 else 1 end \r\n                                        when t.MIsPaid then 3 when t.MIsBetaUser then 4 else 2 end {param.Order} ")) : (str + " order by t.MIsPaid DESC, t.MOrgName "));
			List<BASMyHomeModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<BASMyHomeModel>(ctx, str, cmdParms);
			return new DataGridJson<BASMyHomeModel>
			{
				rows = dataModelBySql,
				total = dataModelBySql.Count
			};
		}

		public bool Register(MContext ctx, SYSOrgAddModel model)
		{
			try
			{
				SYSStorageRepository sYSStorageRepository = new SYSStorageRepository();
				SYSStorageModel activeStorageModel = sYSStorageRepository.GetActiveStorageModel();
				List<CommandInfo> list = new List<CommandInfo>();
				List<CommandInfo> list2 = new List<CommandInfo>();
				if (string.IsNullOrEmpty(model.OrgModel.MLegalTradingName))
				{
					model.OrgModel.MLegalTradingName = model.OrgModel.MName;
				}
				ctx.IsSys = true;
				List<CommandInfo> insertOrUpdateCmd = ModelInfoManager.GetInsertOrUpdateCmd<BASOrganisationModel>(ctx, model.OrgModel, null, true);
				list.AddRange(insertOrUpdateCmd);
				list2.AddRange(insertOrUpdateCmd);
				model.OrgContactModel.MOrgID = model.OrgModel.MItemID;
				List<CommandInfo> insertOrUpdateCmd2 = ModelInfoManager.GetInsertOrUpdateCmd<BASOrgContactModel>(ctx, model.OrgContactModel, null, true);
				list.AddRange(insertOrUpdateCmd2);
				list2.AddRange(insertOrUpdateCmd2);
				model.OrgAddressPhysicalModel.MOrgID = model.OrgModel.MItemID;
				List<CommandInfo> insertOrUpdateCmd3 = ModelInfoManager.GetInsertOrUpdateCmd<BASOrgAddressModel>(ctx, model.OrgAddressPhysicalModel, null, true);
				list.AddRange(insertOrUpdateCmd3);
				list2.AddRange(insertOrUpdateCmd3);
				model.OrgAddressPostalModel.MOrgID = model.OrgModel.MItemID;
				List<CommandInfo> insertOrUpdateCmd4 = ModelInfoManager.GetInsertOrUpdateCmd<BASOrgAddressModel>(ctx, model.OrgAddressPostalModel, null, true);
				list.AddRange(insertOrUpdateCmd4);
				list2.AddRange(insertOrUpdateCmd4);
				model.OrgAppModel.MOrgID = model.OrgModel.MItemID;
				list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<SYSOrgAppModel>(ctx, model.OrgAppModel, null, true));
				model.OrgAppStorageModel.MOrgAppID = model.OrgAppModel.MEntryID;
				model.OrgAppStorageModel.MStorageID = activeStorageModel.MItemID;
				list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<SYSOrgAppStorageModel>(ctx, model.OrgAppStorageModel, null, true));
				model.SecRoleModel.MOrgID = model.OrgModel.MItemID;
				List<CommandInfo> insertOrUpdateCmd5 = ModelInfoManager.GetInsertOrUpdateCmd<SECRoleModel>(ctx, model.SecRoleModel, null, true);
				list.AddRange(insertOrUpdateCmd5);
				list2.AddRange(insertOrUpdateCmd5);
				model.SecRoleUserModel.MOrgID = model.OrgModel.MItemID;
				model.SecRoleUserModel.MRoleID = model.SecRoleModel.MItemID;
				List<CommandInfo> insertOrUpdateCmd6 = ModelInfoManager.GetInsertOrUpdateCmd<SECRoleUserModel>(ctx, model.SecRoleUserModel, null, true);
				list.AddRange(insertOrUpdateCmd6);
				list2.AddRange(insertOrUpdateCmd6);
				model.SecOrgUserModel.MOrgID = model.OrgModel.MItemID;
				List<CommandInfo> insertOrUpdateCmd7 = ModelInfoManager.GetInsertOrUpdateCmd<SECOrgUserModel>(ctx, model.SecOrgUserModel, null, true);
				list.AddRange(insertOrUpdateCmd7);
				list2.AddRange(insertOrUpdateCmd7);
				SqlWhere setupInitModuleFilter = GetSetupInitModuleFilter(ctx);
				List<CommandInfo> collection = SECPermissionRepository.AddAllPermissionToRole(ctx, model.SecOrgUserModel.MItemID, model.SecRoleModel.MItemID, setupInitModuleFilter, ModuleEnum.Sales);
				list.AddRange(collection);
				list2.AddRange(collection);
				List<CommandInfo> orgModuleInfoCmd = GetOrgModuleInfoCmd(ctx, model.OrgModel);
				list.AddRange(orgModuleInfoCmd);
				list2.AddRange(orgModuleInfoCmd);
				list.Add(sYSStorageRepository.GetUpdateOrgCountCmdInfo(model.OrgAppStorageModel.MStorageID));
				string serverConnectionString = sYSStorageRepository.GetServerConnectionString(activeStorageModel.MServerID, activeStorageModel.MBDName);
				list2.AddRange(SECPermissionRepository.UserCopyToDBServer(ctx, serverConnectionString));
				MultiDBCommand[] cmdArray = new MultiDBCommand[2]
				{
					new MultiDBCommand(ctx)
					{
						CommandList = list,
						DBType = SysOrBas.Sys
					},
					new MultiDBCommand(ctx)
					{
						CommandList = list2,
						ConnectionString = serverConnectionString
					}
				};
				return DbHelperMySQL.ExecuteSqlTran(ctx, cmdArray);
			}
			catch(Exception exception)
			{
				return false;
			}
		}

		private SqlWhere GetSetupInitModuleFilter(MContext ctx)
		{
			SqlWhere sqlWhere = new SqlWhere();
			if (ctx.MOrgVersionID == 1)
			{
				List<SECUserRoleInitModel> userRoleInitModel = SECPermissionRepository.GetUserRoleInitModel(ctx);
				List<string> list = new List<string>();
				foreach (SECUserRoleInitModel item in userRoleInitModel)
				{
					list.Add(item.Id);
					if (item.children != null)
					{
						foreach (SECUserRoleInitModel child in item.children)
						{
							list.Add(child.Id);
						}
					}
				}
				sqlWhere.In("MItemID", list.ToArray());
			}
			else
			{
				sqlWhere.NotIn("MItemID", new string[2]
				{
					"Fixed_Assets",
					"Fixed_Assets_Reports"
				});
			}
			return sqlWhere;
		}

		public OperationResult OrgRegisterForTry(MContext ctx, BASOrganisationModel model)
		{
			SYSOrgAddModel sYSOrgAddModel = new SYSOrgAddModel();
			model.MIsPaid = false;
			DateTime dateTime = ctx.DateNow;
			dateTime = dateTime.Date;
			dateTime = dateTime.AddDays(30.0);
			model.MExpiredDate = dateTime.AddSeconds(-1.0);
			sYSOrgAddModel.OrgModel = model;
			sYSOrgAddModel.OrgAddressPhysicalModel = SetAddrPhysicalModel(ctx, model);
			sYSOrgAddModel.OrgAddressPostalModel = SetAddrPostalModel(ctx, model);
			sYSOrgAddModel.OrgContactModel = SetOrgContactModel(ctx, model);
			sYSOrgAddModel.OrgAppModel = SetOrgAppModel(ctx, model);
			sYSOrgAddModel.OrgAppStorageModel = SetOrgAppStorageModel(ctx, sYSOrgAddModel.OrgAppModel);
			sYSOrgAddModel.SecRoleModel = SetSecRoleModel(ctx, model);
			sYSOrgAddModel.SecRoleUserModel = SetSecRoleUserModel(ctx, sYSOrgAddModel.SecRoleModel);
			sYSOrgAddModel.SecOrgUserModel = SetSecOrgUserModel(ctx, model);
			ctx.MOrgID = sYSOrgAddModel.OrgModel.MItemID;
			bool success = Register(ctx, sYSOrgAddModel);
			return new OperationResult
			{
				Success = success,
				ObjectID = sYSOrgAddModel.OrgModel.MItemID
			};
		}

		private BASOrgAddressModel SetAddrPostalModel(MContext ctx, BASOrganisationModel model)
		{
			BASOrgAddressModel bASOrgAddressModel = new BASOrgAddressModel();
			bASOrgAddressModel.MAddressType = 1.ToString();
			bASOrgAddressModel.MOrgID = model.MItemID;
			return bASOrgAddressModel;
		}

		private BASOrgAddressModel SetAddrPhysicalModel(MContext ctx, BASOrganisationModel model)
		{
			BASOrgAddressModel bASOrgAddressModel = new BASOrgAddressModel();
			bASOrgAddressModel.MAddressType = 2.ToString();
			bASOrgAddressModel.MOrgID = model.MItemID;
			return bASOrgAddressModel;
		}

		private BASOrgContactModel SetOrgContactModel(MContext ctx, BASOrganisationModel model)
		{
			BASOrgContactModel bASOrgContactModel = new BASOrgContactModel();
			bASOrgContactModel.MOrgID = model.MItemID;
			return bASOrgContactModel;
		}

		private SYSOrgAppModel SetOrgAppModel(MContext ctx, BASOrganisationModel model)
		{
			SYSOrgAppModel sYSOrgAppModel = new SYSOrgAppModel();
			sYSOrgAppModel.MOrgID = model.MItemID;
			sYSOrgAppModel.MAppID = "1";
			sYSOrgAppModel.MMasterID = ctx.MUserID;
			sYSOrgAppModel.MUsedStatusID = ctx.MUsedStatusID;
			sYSOrgAppModel.MLastViewUserID = ctx.MUserID;
			sYSOrgAppModel.MLastViewDate = DateTime.UtcNow;
			sYSOrgAppModel.MExpireDate = DateTime.UtcNow.AddMonths(1);
			return sYSOrgAppModel;
		}

		private SYSOrgAppStorageModel SetOrgAppStorageModel(MContext ctx, SYSOrgAppModel model)
		{
			SYSOrgAppStorageModel sYSOrgAppStorageModel = new SYSOrgAppStorageModel();
			sYSOrgAppStorageModel.MOrgAppID = model.MEntryID;
			return sYSOrgAppStorageModel;
		}

		private SECRoleModel SetSecRoleModel(MContext ctx, BASOrganisationModel model)
		{
			SECRoleModel sECRoleModel = new SECRoleModel();
			sECRoleModel.MOrgID = model.MItemID;
			sECRoleModel.MAppID = "1";
			sECRoleModel.MNumber = "5";
			sECRoleModel.MShowIndex = 1;
			sECRoleModel.MCreatorID = ctx.MUserID;
			sECRoleModel.MModifierID = ctx.MUserID;
			return sECRoleModel;
		}

		private SECRoleUserModel SetSecRoleUserModel(MContext ctx, SECRoleModel model)
		{
			SECRoleUserModel sECRoleUserModel = new SECRoleUserModel();
			sECRoleUserModel.MRoleID = model.MItemID;
			sECRoleUserModel.MUserID = ctx.MUserID;
			sECRoleUserModel.MOrgID = ctx.MOrgID;
			sECRoleUserModel.MCreatorID = ctx.MUserID;
			sECRoleUserModel.MModifierID = ctx.MUserID;
			return sECRoleUserModel;
		}

		private SECOrgUserModel SetSecOrgUserModel(MContext ctx, BASOrganisationModel model)
		{
			SECOrgUserModel sECOrgUserModel = new SECOrgUserModel();
			sECOrgUserModel.MOrgID = model.MItemID;
			sECOrgUserModel.MUserID = ctx.MUserID;
			sECOrgUserModel.MUserIsActive = true;
			sECOrgUserModel.MCreatorID = ctx.MUserID;
			sECOrgUserModel.MModifierID = ctx.MUserID;
			return sECOrgUserModel;
		}

		private List<CommandInfo> GetOrgModuleInfoCmd(MContext ctx, BASOrganisationModel orgModel)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			BASOrgModuleModel bASOrgModuleModel = new BASOrgModuleModel();
			bASOrgModuleModel.MOrgID = orgModel.MItemID;
			int value = (ctx.MOrgVersionID == 0) ? 1 : 2;
			bASOrgModuleModel.MModuleID = Convert.ToString(value);
			BASOrgModuleModel bASOrgModuleModel2 = bASOrgModuleModel;
			DateTime dateTime = ctx.DateNow;
			dateTime = dateTime.Date;
			dateTime = dateTime.AddDays(30.0);
			bASOrgModuleModel2.MExpiredDate = dateTime.AddSeconds(-1.0);
			list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BASOrgModuleModel>(ctx, bASOrgModuleModel, null, true));
			return list;
		}

		public int DeleteOrgById(MContext ctx, string orgId)
		{
			string sqlText = "update T_Bas_Organisation set MIsDelete=1 , MModifierID=@MModifierID , MModifyDate = now() where MItemID=@MItemID and MIsDelete=0 ";
			MySqlParameter[] array = new MySqlParameter[2]
			{
				new MySqlParameter("@MModifierID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MItemID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ctx.MUserID;
			array[1].Value = orgId;
			bool isSys = ctx.IsSys;
			try
			{
				ctx.IsSys = true;
				List<CommandInfo> list = new List<CommandInfo>();
				list.Add(new CommandInfo(sqlText, array));
				DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
				return dynamicDbHelperMySQL.ExecuteSqlTran(list);
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				ctx.IsSys = isSys;
			}
		}
	}
}
