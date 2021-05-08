using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.Logger;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Logger;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.DataModel.SYS;
using JieNor.Megi.DataRepository.API;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.SEC;
using JieNor.Megi.DataRepository.SYS;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace JieNor.Megi.DataRepository.BAS
{
    public class BASOrganisationRepository : DataServiceT<BASOrganisationModel>
    {
        private const string CHINA_COUNTRY_ID = "106";

        public static string CommonSelect = string.Format("SELECT \n                t1.MItemID as MOrganizationID, \n\t            t1.*,\n                t1.MCreateDate as MOrgCreateDate,\n\t            t2.MAccountingStandard,\n\t            DATE_FORMAT(t2.MConversionDate, '%Y-%m') as MConversionPeriod,\n\t            t3.MSystemLanguage as MLanguage,\n\t            t3.MSystemZone,\n                ifnull(t6.MCode, t1.MStateID) AS MStateName\n            FROM t_bas_organisation t1\n                INNER JOIN t_bas_orginitsetting t2 ON t2.MOrgID=t1.MItemID AND t2.MIsDelete=0\n                INNER JOIN t_reg_globalization t3 ON t3.MOrgID=t1.MItemID AND t3.MIsDelete=0\n                LEFT JOIN t_bas_country t5 ON t5.MItemID=t1.MCountryID AND t5.MIsDelete=0\n                LEFT JOIN t_bas_province t6 ON t6.MItemID=t1.MStateID AND t6.MIsDelete=0 and t5.MItemID='{0}'\n            WHERE 1=1", "106");

        public DataGridJson<BASOrganisationModel> Get(MContext ctx, GetParam param)
        {
            param.ElementID = ctx.MOrgID;
            return new APIDataRepository().Get<BASOrganisationModel>(ctx, param, CommonSelect, false, true, null);
        }

        public DataGridJson<BASOrganisationModel> GetMigrationOrgList(MContext ctx, GetParam param)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("SELECT t1.*,(select MConversionDate from t_bas_orginitsetting where MOrgID=t1.MItemID  and MIsDelete=0) as MGLConversionDate,\r\n                                ifnull(t4.MStatus,0) as MigrateProgress \r\n                            FROM T_Bas_Organisation t1\r\n                            left join t_mi_config t4 on t1.MItemID=t4.MOrgID  and t4.MIsDelete=0\r\n                            WHERE \r\n                                EXISTS(select 1 from T_Sec_OrgUser t2\r\n                                        INNER JOIN T_Sec_User t3 ON t2.MUserID=t3.MItemID AND t3.MIsDelete=0 AND t3.MIsActive=1 \r\n                                        WHERE t1.MItemID=t2.MOrgID AND MUserID=@MUserID AND t2.MIsDelete=0 AND t2.MIsActive=1)\r\n                                AND t1.MIsActive=1 AND t1.MIsDelete=0 \r\n                                AND t1.MRegProgress = 15 \n                                AND t1.MInitBalanceOver = 1\n                                AND DATE_ADD(date(t1.MExpiredDate),INTERVAL 1 DAY) >= @MExpiredDate\n                                AND t1.MItemID in (select \n                                    distinct g.MOrgID\n                                from\n                                    T_Sec_RolePermission a\n                                        inner join\n                                    T_Sec_Role b ON a.MRoleID = b.MItemID\n                                        AND a.MIsDelete = 0\n                                        AND a.MIsActive = 1\n                                        AND b.MIsDelete = 0\n                                        AND b.MIsActive = 1\n                                        inner join\n                                    T_Sec_RoleUser e ON e.MRoleID = b.MItemID\n                                        AND e.MIsDelete = 0\n                                        AND e.MIsActive = 1\n                                        inner join\n                                    T_Sec_OrgUser g ON e.MUserID = g.MUserID\n                                        AND b.MOrgID = g.MOrgID\n                                        AND g.MIsDelete = 0\n                                        AND g.MIsActive = 1\n                                Where\n                                    e.MUserID = @MUserID\n                                        AND a.MBizObjectID = 'General_Ledger'\n                                        AND a.MPermItemID = 'Approve')");
            if (!string.IsNullOrEmpty(param.ElementID))
            {
                stringBuilder.Append(" AND t1.MItemID=@MItemID ");
            }
            MySqlParameter[] parameters = new MySqlParameter[4]
            {
                new MySqlParameter("@MUserID", ctx.MUserID),
                new MySqlParameter("@MItemID", param.ElementID),
                new MySqlParameter("@MExpiredDate", ctx.DateNow),
                new MySqlParameter("@MLocaleID", ctx.MLCID)
            };
            return ModelInfoManager.GetPageDataModelListBySql<BASOrganisationModel>(ctx, stringBuilder.ToString(), parameters, param);
        }

        public DataGridJson<BASOrganisationModel> GetOrgListWithFPChangeAuth(MContext ctx, GetParam param)
        {
            if (string.IsNullOrWhiteSpace(param.Where))
            {
                param.Where = string.Format("{0},{1}", "Sales_Fapiao", "Purchases_Fapiao");
            }
            string sql = string.Format("select \n                                    t1.MItemID,\n                                    t1.MLegalTradingName,\n                                    t2.MViewBizObjectIDs,\n                                    t3.MChangeBizObjectIDs,\n                                    t1.MIsDelete,\n                                    (case when DATE_ADD(date(t1.MExpiredDate), INTERVAL 1 DAY) >= @MExpiredDate then 0 else 1 end) as MIsExpired\n                                from\n                                    t_bas_organisation t1\n                                left join (select\n                                            g.MOrgID,group_concat(a.MBizObjectID) as MViewBizObjectIDs\n                                        from\n                                            T_Sec_RolePermission a\n                                                inner join\n                                            T_Sec_Role b ON a.MRoleID = b.MItemID\n                                                AND a.MIsDelete = 0\n                                                AND a.MIsActive = 1\n                                                AND b.MIsDelete = 0\n                                                AND b.MIsActive = 1\n                                                inner join\n                                            T_Sec_RoleUser e ON e.MRoleID = b.MItemID\n                                                AND e.MIsDelete = 0\n                                                AND e.MIsActive = 1\n                                                inner join\n                                            T_Sec_OrgUser g ON e.MUserID = g.MUserID\n                                                AND b.MOrgID = g.MOrgID\n                                                AND g.MIsDelete = 0\n                                                AND g.MIsActive = 1\n                                                AND g.MUserIsActive = 1\n                                        Where\n                                            e.MUserID = @MUserID\n                                                AND a.MBizObjectID in ('{0}')\n                                                AND a.MPermItemID = 'View' group by g.MOrgID) t2\n                                    on t2.MOrgID=t1.MItemID\n\n                                left join (select\n                                            g.MOrgID,group_concat(a.MBizObjectID) as MChangeBizObjectIDs\n                                        from\n                                            T_Sec_RolePermission a\n                                                inner join\n                                            T_Sec_Role b ON a.MRoleID = b.MItemID\n                                                AND a.MIsDelete = 0\n                                                AND a.MIsActive = 1\n                                                AND b.MIsDelete = 0\n                                                AND b.MIsActive = 1\n                                                inner join\n                                            T_Sec_RoleUser e ON e.MRoleID = b.MItemID\n                                                AND e.MIsDelete = 0\n                                                AND e.MIsActive = 1\n                                                inner join\n                                            T_Sec_OrgUser g ON e.MUserID = g.MUserID\n                                                AND b.MOrgID = g.MOrgID\n                                                AND g.MIsDelete = 0\n                                                AND g.MIsActive = 1\n                                                AND g.MUserIsActive = 1\n                                        Where\n                                            e.MUserID = @MUserID\n                                                AND a.MBizObjectID in ('{0}')\n                                                AND a.MPermItemID = 'Change' group by g.MOrgID) t3\n                                    on t3.MOrgID=t1.MItemID\n                                where\n                                    t1.MIsDelete = 0\n                                    and t1.MRegProgress = 15\r\n                                    and EXISTS(select 1\n                                                from\n                                                    T_Sec_OrgUser t6\n                                                        INNER JOIN\n                                                    T_Sec_User t7 ON t6.MUserID = t7.MItemID\n                                                        AND t7.MIsDelete = 0\n                                                WHERE\n                                                        t6.MOrgID = t1.MItemID\n                                                        AND t6.MUserID = @MUserID\n                                                        AND t6.MIsDelete = 0\n                                                        AND t6.MUserIsActive = 1)", string.Join("','", param.Where.Split(',')));
            MySqlParameter[] parameters = new MySqlParameter[2]
            {
                new MySqlParameter("@MUserID", ctx.MUserID),
                new MySqlParameter("@MExpiredDate", ctx.DateNow)
            };
            ctx.IsSys = true;
            DataGridJson<BASOrganisationModel> pageDataModelListBySql = ModelInfoManager.GetPageDataModelListBySql<BASOrganisationModel>(ctx, sql, parameters, param);
            ctx.IsSys = false;
            return pageDataModelListBySql;
        }

        private static SECRoleModel GetRoleModel(MContext ctx, string orgId)
        {
            SECRoleModel sECRoleModel = new SECRoleModel();
            sECRoleModel.MOrgID = orgId;
            sECRoleModel.MAppID = "1";
            sECRoleModel.MNumber = "3";
            sECRoleModel.MShowIndex = 1;
            sECRoleModel.MCreatorID = ctx.MUserID;
            sECRoleModel.MModifierID = ctx.MUserID;
            return sECRoleModel;
        }

        private static SECRoleUserModel GetRoleUserModel(MContext ctx, string orgId, string roleId)
        {
            SECRoleUserModel sECRoleUserModel = new SECRoleUserModel();
            sECRoleUserModel.MOrgID = orgId;
            sECRoleUserModel.MRoleID = roleId;
            sECRoleUserModel.MUserID = ctx.MUserID;
            sECRoleUserModel.MCreatorID = ctx.MUserID;
            sECRoleUserModel.MModifierID = ctx.MUserID;
            return sECRoleUserModel;
        }

        private static SECOrgUserModel GetOrgUserModel(MContext ctx, string orgId)
        {
            SECOrgUserModel sECOrgUserModel = new SECOrgUserModel();
            sECOrgUserModel.MOrgID = orgId;
            sECOrgUserModel.MUserID = ctx.MUserID;
            sECOrgUserModel.MUserIsActive = true;
            sECOrgUserModel.MCreatorID = ctx.MUserID;
            sECOrgUserModel.MModifierID = ctx.MUserID;
            return sECOrgUserModel;
        }

        public static OperationResult UpdateRegProgress(BASOrgScheduleTypeEnum type, MContext context)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("update T_Bas_Organisation set ");
            stringBuilder.Append(" MRegProgress = IFNULL(@MRegProgress,MRegProgress) , ");
            stringBuilder.Append(" MModifierID = IFNULL(@MModifierID,MModifierID)  ");
            stringBuilder.Append(" where MItemID=@MItemID and MIsDelete = 0 and MIsActive = 1  AND IFNULL(MRegProgress,0) < @MRegProgress");
            MySqlParameter[] array = new MySqlParameter[3]
            {
                new MySqlParameter("@MItemID", MySqlDbType.VarChar, 36),
                new MySqlParameter("@MModifierID", MySqlDbType.VarChar, 36),
                new MySqlParameter("@MRegProgress", MySqlDbType.Int32, 11)
            };
            array[0].Value = context.MOrgID;
            array[1].Value = context.MUserID;
            array[2].Value = Convert.ToInt32(type);
            int num = DbHelperMySQL.ExecuteSql(new MContext(), stringBuilder.ToString(), array);
            OperationResult operationResult = new OperationResult();
            operationResult.Success = true;
            operationResult.Message = COMMultiLangRepository.GetText(context.MLCID, LangModule.Common, "Success", "Success");
            operationResult.SuccessModelID = new List<string>
            {
                context.MOrgID
            };
            return operationResult;
        }

        public static OperationResult UpdateRegSchedule(MContext ctx, BASOrganisationModel model)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("update T_Bas_Organisation set ");
            stringBuilder.Append(" MRegProgress = IFNULL(@MRegProgress,MRegProgress) , ");
            stringBuilder.Append(" MModifierID = @MModifierID  ");
            stringBuilder.Append(" where MItemID=@MItemID and MIsDelete = 0 and MIsActive = 1 AND IFNULL(MRegProgress,0) < @MRegProgress");
            MySqlParameter[] array = new MySqlParameter[3]
            {
                new MySqlParameter("@MItemID", MySqlDbType.VarChar, 36),
                new MySqlParameter("@MModifierID", MySqlDbType.VarChar, 36),
                new MySqlParameter("@MRegProgress", MySqlDbType.Int32, 11)
            };
            array[0].Value = model.MItemID;
            array[1].Value = ctx.MUserID;
            array[1].Value = model.MRegProgress;
            int num = DbHelperMySQL.ExecuteSql(new MContext(), stringBuilder.ToString(), array);
            OperationResult operationResult = new OperationResult();
            if (num > 0)
            {
                operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "Success", "Success");
                operationResult.SuccessModelID = new List<string>
                {
                    model.MItemID
                };
            }
            else
            {
                operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "Fail", "Fail");
                operationResult.FailModelID = new List<string>
                {
                    model.MItemID
                };
            }
            return operationResult;
        }

        public static OperationResult UpdateOrgInfo(MContext ctx, BASOrganisationModel orgModel, List<string> updateFields)
        {
            OperationResult operationResult = new OperationResult();
            List<CommandInfo> insertOrUpdateCmd = ModelInfoManager.GetInsertOrUpdateCmd<BASOrganisationModel>(ctx, orgModel, updateFields, true);
            MultiDBCommand[] cmdArray = new MultiDBCommand[2]
            {
                new MultiDBCommand(ctx)
                {
                    CommandList = insertOrUpdateCmd,
                    DBType = SysOrBas.Sys
                },
                new MultiDBCommand(ctx)
                {
                    CommandList = insertOrUpdateCmd,
                    DBType = SysOrBas.Bas
                }
            };
            operationResult.Success = DbHelperMySQL.ExecuteSqlTran(new MContext(), cmdArray);
            if (operationResult.Success && ctx.MOrgID == orgModel.MItemID)
            {
                ctx.MOrgName = orgModel.MName;
                ctx.MLastLoginOrgName = orgModel.MName;
                ctx.MLegalTradingName = orgModel.MLegalTradingName;
                ContextHelper.MContext = ctx;
            }
            return operationResult;
        }

        public static OperationResult Update(MContext ctx, BASOrganisationModel model)
        {
            string text = DynamicPubConstant.ConnectionString(ctx.MOrgID);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("update T_Bas_Organisation set ");
            stringBuilder.Append(" MItemID = IFNULL(@MItemID,MItemID) , ");
            stringBuilder.Append(" MRegionID = IFNULL(@MRegionID,MRegionID) , ");
            stringBuilder.Append(" MName = IFNULL(@MName,MName) , ");
            stringBuilder.Append(" MLegalTradingName = IFNULL(@MLegalTradingName,MLegalTradingName) , ");
            stringBuilder.Append(" MOrgTypeID = IFNULL(@MOrgTypeID,MOrgTypeID) , ");
            stringBuilder.Append(" MOrgBusiness = IFNULL(@MOrgBusiness,MOrgBusiness) , ");
            stringBuilder.Append(" MConversionDate = IFNULL(@MConversionDate,MConversionDate) , ");
            stringBuilder.Append(" MIsActive = IFNULL(@MIsActive,MIsActive) , ");
            stringBuilder.Append(" MIsDelete = IFNULL(@MIsDelete,MIsDelete) , ");
            stringBuilder.Append(" MModifierID = IFNULL(@MModifierID,MModifierID) , ");
            stringBuilder.Append(" MModifyDate = IFNULL(@MModifyDate,MModifyDate)  ");
            stringBuilder.Append(" where MItemID=@MItemID and MIsDelete = 0 and MIsActive = 1  ");
            MySqlParameter[] array = new MySqlParameter[11]
            {
                new MySqlParameter("@MItemID", MySqlDbType.VarChar, 36),
                new MySqlParameter("@MRegionID", MySqlDbType.VarChar, 3),
                new MySqlParameter("@MName", MySqlDbType.VarChar, 50),
                new MySqlParameter("@MLegalTradingName", MySqlDbType.VarChar, 200),
                new MySqlParameter("@MOrgTypeID", MySqlDbType.VarChar, 36),
                new MySqlParameter("@MOrgBusiness", MySqlDbType.VarChar, 200),
                new MySqlParameter("@MConversionDate", MySqlDbType.DateTime),
                new MySqlParameter("@MIsActive", MySqlDbType.Bit),
                new MySqlParameter("@MIsDelete", MySqlDbType.Bit),
                new MySqlParameter("@MModifierID", MySqlDbType.VarChar, 36),
                new MySqlParameter("@MModifyDate", MySqlDbType.DateTime)
            };
            array[0].Value = model.MItemID;
            array[1].Value = model.MRegionID;
            array[2].Value = model.MName;
            array[3].Value = model.MLegalTradingName;
            array[4].Value = model.MOrgTypeID;
            array[5].Value = model.MOrgBusiness;
            array[6].Value = model.MConversionDate.Date;
            array[8].Value = model.MIsDelete;
            array[9].Value = ctx.MUserID;
            array[10].Value = model.MModifyDate;
            CommandInfo commandInfo = new CommandInfo();
            commandInfo.CommandText = stringBuilder.ToString();
            DbParameter[] array2 = commandInfo.Parameters = array;
            List<CommandInfo> list = new List<CommandInfo>();
            list.Add(commandInfo);
            MultiDBCommand[] cmdArray = new MultiDBCommand[2]
            {
                new MultiDBCommand(ctx)
                {
                    CommandList = list,
                    DBType = SysOrBas.Sys
                },
                new MultiDBCommand(ctx)
                {
                    CommandList = list,
                    DBType = SysOrBas.Bas
                }
            };
            OperationResult operationResult = new OperationResult();
            operationResult.Success = DbHelperMySQL.ExecuteSqlTran(new MContext(), cmdArray);
            return operationResult;
        }
        public OperationResult UpdateStatus(MContext ctx, string mItmeId, int status)
        {
            OperationResult operationResult = new OperationResult();
            operationResult.Success = true;
            try
            {
                string sql = "update t_bas_organisation set MIsDelete=" + status + " where MItemID ='" + mItmeId + "';";
                //DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);

                int num = DbHelperMySQL.ExecuteSql(ctx, sql);
                //int result= dynamicDbHelperMySQL.ExecuteSql(sql);
                if (num <= 0)
                {
                    operationResult.Success = false;
                    operationResult.Message = "更新失败";

                }
            }
            catch (Exception ex)
            {
                operationResult.Success = false;
                operationResult.Message = ex.Message;

            }
            return operationResult;
            //return ModelInfoManager.DataTableToList<SECUserModel>((stringBuilder.ToString(), array).Tables[0]);
        }
        public OperationResult UpdateExpiredDate(MContext ctx, string mItmeId, DateTime expiredDate)
        {
            OperationResult operationResult = new OperationResult();
            operationResult.Success = true;
            try
            {
                string sql = "update t_bas_organisation set MExpiredDate='" + expiredDate + "' where MItemID ='" + mItmeId + "';";
                //DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);

                int num = DbHelperMySQL.ExecuteSql(ctx, sql);
                //int result= dynamicDbHelperMySQL.ExecuteSql(sql);
                if (num <= 0)
                {
                    operationResult.Success = false;
                    operationResult.Message = "更新失败";

                }
            }
            catch (Exception ex)
            {
                operationResult.Success = false;
                operationResult.Message = ex.Message;

            }
            return operationResult;
            //return ModelInfoManager.DataTableToList<SECUserModel>((stringBuilder.ToString(), array).Tables[0]);
        }

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

        public BASOrganisationModel GetOrgModel(MContext ctx)
        {
            return base.GetDataModel(ctx, ctx.MOrgID, false);
        }

        public BASOrganisationModel GetOrgModelInSysDB(MContext ctx, string orgId)
        {
            BASOrganisationModel result = null;
            string sQLString = "SELECT * FROM t_bas_organisation where MItemID=@MOrgID and MIsDelete=0 ";
            MySqlParameter[] cmdParms = new MySqlParameter[1]
            {
                new MySqlParameter("@MOrgID", orgId)
            };
            DataSet ds = DbHelperMySQL.Query(sQLString, cmdParms);
            List<BASOrganisationModel> list = ModelInfoManager.DataTableToList<BASOrganisationModel>(ds);
            if (list != null && list.Count > 0)
            {
                result = list.FirstOrDefault();
            }
            return result;
        }

        public BASOrganisationModel GetOrgModel(string orgId, string userId)
        {
            string sql = "SELECT a.* FROM t_bas_organisation a inner join t_sec_orguser b ON a.MItemID=b.MOrgID \r\n                          WHERE a.MItemID=@MOrgID AND b.MUserID=@MUserID AND a.MIsDelete=0 and a.MIsActive = 1 ";
            MySqlParameter[] array = new MySqlParameter[2]
            {
                new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
                new MySqlParameter("@MUserID", MySqlDbType.VarChar, 36)
            };
            array[0].Value = orgId;
            array[1].Value = userId;
            MContext mContext = new MContext();
            mContext.IsSys = true;
            List<BASOrganisationModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<BASOrganisationModel>(mContext, sql, array);
            if (dataModelBySql.Count > 0)
            {
                return dataModelBySql[0];
            }
            return null;
        }

        public static List<BASOrganisationModel> GetOrgListByName(MContext ctx, string displayName, string excludeId)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("select * from (select * from T_Bas_Organisation Where MMasterID=@MUserID and MIsDelete = 0 and MIsActive = 1\n                            union all\n                            select * from t_bas_organisation where MItemID in (\n                                select MOrgID from t_sec_orguser where MUserID=@MUserID and MIsDelete = 0 and MIsActive = 1)) u \r\n                            where MName=@MName and MIsDelete=0");
            if (!string.IsNullOrWhiteSpace(excludeId))
            {
                stringBuilder.AppendLine(" and MItemID<>@MItemID");
            }
            MySqlParameter[] cmdParms = new MySqlParameter[3]
            {
                new MySqlParameter("@MUserID", ctx.MUserID),
                new MySqlParameter("@MName", displayName),
                new MySqlParameter("@MItemID", excludeId)
            };
            return ModelInfoManager.GetDataModelBySql<BASOrganisationModel>(ctx, stringBuilder.ToString(), cmdParms);
        }

        public static BASOrganisationModel GetOrgBasicInfo(MContext ctx)
        {
            string sql = "SELECT a.MName,MLegalTradingName,MCountryID,MStateID,MCityID,MStreet,MPostalNo,b.MName as MCountryName,c.MName as MStateName\n                FROM t_bas_organisation a\n                left join t_bas_country_l b on a.MCountryID=b.MParentID and b.MLocaleID=@MLocaleID and b.MIsDelete = 0 and b.MIsActive = 1\n                left join t_bas_province_l c on a.MStateID=c.MParentID and c.MLocaleID=@MLocaleID and c.MIsDelete = 0 and c.MIsActive = 1\n                where MItemID=@MOrgID and a.MIsDelete = 0 and a.MIsActive = 1 ";
            MySqlParameter[] array = new MySqlParameter[2]
            {
                new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 36),
                new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
            };
            array[0].Value = ctx.MLCID;
            array[1].Value = ctx.MOrgID;
            List<BASOrganisationModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<BASOrganisationModel>(ctx, sql, array);
            if (dataModelBySql.Count > 0)
            {
                return dataModelBySql[0];
            }
            return null;
        }

        public static int GetHistoryOrgCount(string userId)
        {
            List<BASMyHomeModel> list = new List<BASMyHomeModel>();
            string sQLString = "select COUNT(*) from t_bas_organisation WHERE MMasterID=@UserID AND MItemID NOT LIKE 'dm0000000000%' and MIsDelete = 0 and MIsActive = 1";
            MySqlParameter[] array = new MySqlParameter[1]
            {
                new MySqlParameter("@UserID", MySqlDbType.VarChar, 36)
            };
            array[0].Value = userId;
            object single = DbHelperMySQL.GetSingle(sQLString, array);
            return Convert.ToInt32(single);
        }


        public static OperationResult CreateDemoCompany(MContext ctx)
        {
            string text = "dm000000000000000000000000000000";
            string storageId = "100000";
            OperationResult operationResult = new OperationResult();
            BASOrganisationModel demoOrg = GetDemoOrg(ctx);
            if (demoOrg != null)
            {
                operationResult.ObjectID = demoOrg.MItemID;
                operationResult.Success = true;
                return operationResult;
            }
            MySqlConnection mySqlConnection = new MySqlConnection(PubConstant.ConnectionString);
            string database = mySqlConnection.Database;
            SYSStorageRepository sYSStorageRepository = new SYSStorageRepository();
            string demoIDIndex = GetDemoIDIndex(ctx);
            string demoActiveStorageID = sYSStorageRepository.GetDemoActiveStorageID();
            string text2 = $"{text.Substring(0, 20)}{demoIDIndex}";
            SYSStorageModel storageModel = sYSStorageRepository.GetStorageModel(storageId);
            SYSStorageModel storageModel2 = sYSStorageRepository.GetStorageModel(demoActiveStorageID);
            MultiDBCommand[] array = new MultiDBCommand[4];
            MultiDBCommand[] array2 = array;
            MultiDBCommand multiDBCommand = new MultiDBCommand(ctx)
            {
                DBType = SysOrBas.Sys
            };
            MultiDBCommand multiDBCommand2 = multiDBCommand;
            List<CommandInfo> list = new List<CommandInfo>();
            List<CommandInfo> list2 = list;
            object[] obj = new object[7]
            {
                database,
                database,
                demoActiveStorageID,
                text,
                demoIDIndex,
                ctx.MUserID,
                null
            };
            DateTime dateNow = ctx.DateNow;
            obj[6] = dateNow.ToString("yyyy-MM-dd hh:mm:ss");
            list2.Add(new CommandInfo(string.Format("CALL P_SysDB_CreateDemo('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", obj), null));
            multiDBCommand2.CommandList = list;
            array2[0] = multiDBCommand;
            MultiDBCommand[] array3 = array;
            multiDBCommand = new MultiDBCommand(ctx)
            {
                ConnectionString = sYSStorageRepository.GetServerConnectionString(storageModel2.MServerID)
            };



            MultiDBCommand multiDBCommand3 = multiDBCommand;
            list = new List<CommandInfo>();
            List<CommandInfo> list3 = list;
            object[] obj2 = new object[6]
            {
                storageModel.MBDName,
                storageModel2.MBDName,
                text,
                demoIDIndex,
                ctx.MUserID,
                null
            };
            dateNow = ctx.DateNow;
            obj2[5] = dateNow.ToString("yyyy-MM-dd hh:mm:ss");
            list3.Add(new CommandInfo(string.Format("CALL P_BasDB_CreateDemo('{0}','{1}','{2}','{3}','{4}','{5}')", obj2), null));
            multiDBCommand3.CommandList = list;
            array3[1] = multiDBCommand;
            ctx.MOrgID = text2;
            string serverConnectionString = sYSStorageRepository.GetServerConnectionString(storageModel2.MServerID, storageModel2.MBDName);
            MLogger.Log("serverConnectionString=" + serverConnectionString);

            array[2] = new MultiDBCommand(ctx)
            {
                ConnectionString = serverConnectionString,
                CommandList = SECPermissionRepository.UserCopyToDBServer(ctx, serverConnectionString)
            };
            SYSStorageRepository sYSStorageRepository2 = new SYSStorageRepository();
            array[3] = new MultiDBCommand(ctx)
            {
                DBType = SysOrBas.Sys,
                CommandList = new List<CommandInfo>
                {
                    sYSStorageRepository2.GetUpdateOrgCountCmdInfo(demoActiveStorageID)
                }
            };


            foreach (var item in array)
            {
                //item.CommandList = item.CommandList.Where(i => !string.IsNullOrWhiteSpace(i.CommandText)).Select(i=>i).ToList();

                MLogger.Log(item.ConnectionString + "\n");
                MLogger.Log(string.Join("\n", item.CommandList.Select(x => x.CommandText)));
                MLogger.Log("=====================================================================================");
            }
            try
            {
                bool success = DbHelperMySQL.ExecuteSqlTran(ctx, array);
                operationResult.ObjectID = text2;
                operationResult.Success = success;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return operationResult;
        }

        public static BASOrganisationModel GetDemoOrg(MContext ctx)
        {
            string sQLString = "SELECT * FROM t_bas_organisation WHERE MIsDemo=1 AND MIsDelete=0 AND MMasterID=@MMasterID ";
            MySqlParameter[] cmdParms = new MySqlParameter[1]
            {
                new MySqlParameter("@MMasterID", ctx.MUserID)
            };
            DataSet dataSet = DbHelperMySQL.Query(sQLString, cmdParms);
            if (dataSet.Tables[0].Rows.Count > 0)
            {
                List<BASOrganisationModel> list = ModelInfoManager.DataTableToList<BASOrganisationModel>(dataSet.Tables[0]);
                return list[0];
            }
            return null;
        }

        private static BASOrganisationModel GetDemoOrg(string orgId)
        {
            string sQLString = "SELECT * FROM t_bas_organisation WHERE MItemID = @MItemID ";
            MySqlParameter[] cmdParms = new MySqlParameter[1]
            {
                new MySqlParameter("@MItemID", orgId)
            };
            DataSet dataSet = DbHelperMySQL.Query(sQLString, cmdParms);
            if (dataSet.Tables[0].Rows.Count > 0)
            {
                List<BASOrganisationModel> list = ModelInfoManager.DataTableToList<BASOrganisationModel>(dataSet.Tables[0]);
                return list[0];
            }
            return null;
        }

        private static string GetDemoIDIndex(MContext ctx)
        {
            string sQLString = "SELECT substring(MItemID,21,32)  \r\n                           FROM t_bas_organisation \r\n                           WHERE MItemID LIKE 'dm000000000000000000%' AND MMasterID=@MMasterID LIMIT 0,1;";
            MySqlParameter[] cmdParms = new MySqlParameter[1]
            {
                new MySqlParameter("@MMasterID", ctx.MUserID)
            };
            object single = DbHelperMySQL.GetSingle(sQLString, cmdParms);
            if (single == null || single == DBNull.Value)
            {
                sQLString = "SELECT LPAD(substring(MItemID,21,32)+1,12,0) \r\n                        FROM t_bas_organisation \r\n                        WHERE MItemID LIKE 'dm000000000000000000%' ORDER BY MItemID DESC LIMIT 0,1;";
                single = DbHelperMySQL.GetSingle(sQLString);
            }
            return single.ToString();
        }

        public int UpdateDemoData()
        {
            SYSStorageRepository sYSStorageRepository = new SYSStorageRepository();
            List<SYSStorageServerModel> demoStorageServerList = sYSStorageRepository.GetDemoStorageServerList();
            if (demoStorageServerList == null || demoStorageServerList.Count == 0)
            {
                return 0;
            }
            int num = 0;
            foreach (SYSStorageServerModel item in demoStorageServerList)
            {
                List<SYSStorageModel> demoStorageList = sYSStorageRepository.GetDemoStorageList(item.MItemID);
                if (demoStorageList != null && demoStorageList.Count != 0)
                {
                    string serverConnectionString = sYSStorageRepository.GetServerConnectionString(item);
                    foreach (SYSStorageModel item2 in demoStorageList)
                    {
                        if (string.IsNullOrEmpty(item2.MConOtherInfo))
                        {
                            item2.MConOtherInfo = "2015-06-24";
                        }
                        DateTime dateTime = Convert.ToDateTime(item2.MConOtherInfo);
                        DateTime date = dateTime.Date;
                        DateTime t = date;
                        dateTime = DateTime.Now;
                        if (!(t >= dateTime.Date))
                        {
                            if (UpdateDemoData(item2.MBDName, serverConnectionString, date, item2.MItemID))
                            {
                                UpdateOperateLog(item2.MBDName, serverConnectionString);
                            }
                            num++;
                        }
                    }
                }
            }
            return num;
        }

        public bool UpdateDemoData(string dbName, string connectionString, DateTime lastUpdateDate, string storageId)
        {
            string updateColumnSql = GetUpdateColumnSql(dbName, storageId);
            if (string.IsNullOrEmpty(updateColumnSql))
            {
                return true;
            }
            updateColumnSql = $"SET SQL_SAFE_UPDATES = 0;{updateColumnSql}";
            MultiDBCommand[] cmdArray = new MultiDBCommand[2]
            {
                new MultiDBCommand(null)
                {
                    CommandList = new List<CommandInfo>
                    {
                        new CommandInfo
                        {
                            CommandText = updateColumnSql
                        }
                    },
                    ConnectionString = connectionString
                },
                new MultiDBCommand(null)
                {
                    CommandList = new List<CommandInfo>
                    {
                        new CommandInfo
                        {
                            CommandText = string.Format("UPDATE T_Sys_Storage SET MConOtherInfo='{0}' WHERE MItemID='{1}'", lastUpdateDate.AddDays(1.0).ToString("yyyy-MM-dd"), storageId)
                        }
                    },
                    DBType = SysOrBas.Sys
                }
            };
            try
            {
                return DbHelperMySQL.ExecuteSqlTran(new MContext(), cmdArray);
            }
            catch
            {
                return false;
            }
        }

        private string GetUpdateColumnSql(string dbName, string storageId)
        {
            List<string> tableList = GetTableList(dbName);
            if (tableList == null || tableList.Count == 0)
            {
                return "";
            }
            List<DemoColumnUpdateModel> dateTimeColumnList = GetDateTimeColumnList(dbName);
            if (dateTimeColumnList == null || dateTimeColumnList.Count == 0)
            {
                return "";
            }
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string item in tableList)
            {
                List<DemoColumnUpdateModel> list = (from t in dateTimeColumnList
                                                    where t.TableName == item
                                                    select t).ToList();
                if (list != null && list.Count != 0)
                {
                    DemoColumnUpdateModel demoColumnUpdateModel = (from t in list
                                                                   where t.ColumnKey == "PRI"
                                                                   select t).FirstOrDefault();
                    if (demoColumnUpdateModel != null)
                    {
                        List<string> list2 = (from t in list
                                              where t.ColumnKey != "PRI"
                                              select t.ColumnName).ToList();
                        if (list2 != null && list2.Count != 0)
                        {
                            stringBuilder.AppendFormat("UPDATE {0}.{1} SET ", dbName, item);
                            foreach (string item2 in list2)
                            {
                                stringBuilder.AppendFormat(" {0}=DATE_ADD({0}, interval 1 day),", item2);
                            }
                            stringBuilder.Remove(stringBuilder.Length - 1, 1);
                            if (storageId == "100000")
                            {
                                stringBuilder.Append(";");
                            }
                            else
                            {
                                stringBuilder.AppendFormat(" WHERE {0} LIKE '%0000000%';", demoColumnUpdateModel.ColumnName);
                            }
                        }
                    }
                }
            }
            return stringBuilder.ToString();
        }

        private List<string> GetTableList(string dbName)
        {
            string sQLString = "select TABLE_NAME from information_schema.tables where TABLE_SCHEMA=@DBName";
            MySqlParameter[] cmdParms = new MySqlParameter[1]
            {
                new MySqlParameter("@DBName", dbName)
            };
            DataSet dataSet = DbHelperMySQL.Query(sQLString, cmdParms);
            if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
            {
                return null;
            }
            List<string> list = new List<string>();
            DataTable dataTable = dataSet.Tables[0];
            foreach (DataRow row in dataTable.Rows)
            {
                list.Add(row["TABLE_NAME"].ToString());
            }
            return list;
        }

        private List<DemoColumnUpdateModel> GetDateTimeColumnList(string dbName)
        {
            string sQLString = "select column_name AS ColumnName,table_name AS TableName,column_key AS ColumnKey from information_schema.columns \n\t\t                    where table_schema=@DBName AND (data_type='datetime' OR column_key='PRI')";
            MySqlParameter[] cmdParms = new MySqlParameter[1]
            {
                new MySqlParameter("@DBName", dbName)
            };
            DataSet ds = DbHelperMySQL.Query(sQLString, cmdParms);
            return ModelInfoManager.DataTableToList<DemoColumnUpdateModel>(ds);
        }

        private void UpdateOperateLog(string dbName, string connectionString)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(" SET SQL_SAFE_UPDATES = 0; ");
            stringBuilder.AppendFormat(" UPDATE {0}.t_log_operation ", dbName);
            stringBuilder.Append(" SET MValue1 = CONCAT('\"\\\\/Date(', UNIX_TIMESTAMP(DATE_ADD(FROM_UNIXTIME(REPLACE(REPLACE(MValue1,'\"\\\\/Date(',''),')\\\\/\"','')/1000),interval 1 day))*1000,')\\\\/\"')");
            stringBuilder.Append(" WHERE MPKID LIKE '%0000000000%' AND MValue1 like '%/Date(%'; ");
            stringBuilder.AppendFormat(" UPDATE {0}.t_log_operation ", dbName);
            stringBuilder.Append(" SET MValue2 = CONCAT('\"\\\\/Date(', UNIX_TIMESTAMP(DATE_ADD(FROM_UNIXTIME(REPLACE(REPLACE(MValue2,'\"\\\\/Date(',''),')\\\\/\"','')/1000),interval 1 day))*1000,')\\\\/\"')");
            stringBuilder.Append(" WHERE MPKID LIKE '%0000000000%' AND MValue2 like '%/Date(%'; ");
            DbHelperMySQL.ExecuteNonQuery(new MContext(), stringBuilder.ToString(), connectionString);
        }

        public List<BASOrganisationModel> GetOrgList(MContext ctx, SqlWhere filter)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("SELECT o.*,u.MEmailAddress FROM `t_bas_organisation` o inner join  t_sec_user u on o.MMasterID = u.MItemID");
            if (filter != null && !string.IsNullOrEmpty(filter.WhereSqlString))
            {
                stringBuilder.AppendLine(filter.WhereSqlString);
            }
            stringBuilder.AppendLine("order by o.MCreateDate desc");

            ArrayList arrayList = new ArrayList();
            arrayList.Add(new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 36));
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
            return ModelInfoManager.DataTableToList<BASOrganisationModel>(dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array).Tables[0]);

        }
    }
}
