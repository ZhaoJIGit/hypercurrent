using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.MultiLanguage;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.FC;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace JieNor.Megi.DataRepository.FC
{
	public class FCVoucherModuleRepository : DataServiceT<FCVoucherModuleModel>
	{
		private readonly GLUtility utility = new GLUtility();

		public List<FCVoucherModuleModel> GetVoucherModuleListWithNoEntry(MContext ctx)
		{
			List<FCVoucherModuleModel> list = new List<FCVoucherModuleModel>();
			string sql = "select distinct t1.* from t_fc_vouchermodule t1 \r\n                         inner join t_fc_vouchermoduleentry t2 on t1.MItemID = t2.MID and t1.MOrgID = t2.MOrgID and t2.MIsDelete = 0  \r\n                        where t1.MOrgID = @MOrgID and t1.MLCID = @MLCID and t1.MIsDelete = 0  order by t1.MFastCode asc";
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter
				{
					ParameterName = "MOrgID",
					Value = ctx.MOrgID
				},
				new MySqlParameter
				{
					ParameterName = "@MLCID",
					Value = ctx.MLCID
				}
			};
			DataSet ds = new DynamicDbHelperMySQL(ctx).Query(sql, cmdParms);
			return ModelInfoManager.DataTableToList<FCVoucherModuleModel>(ds);
		}

		public FCVoucherModuleModel GetVoucherModelWithEntry(MContext ctx, string PKID)
		{
			return GetDataModel(ctx, PKID, false);
		}

		public FCVoucherModuleModel UpdateVoucherModuleModel(MContext ctx, FCVoucherModuleModel model)
		{
			model.MOrgID = ctx.MOrgID;
			model.MLCID = ctx.MLCID;
			GetInsertCheckGroupValue(ctx, new List<FCVoucherModuleModel>
			{
				model
			});
			List<CommandInfo> insertOrUpdateCmd = ModelInfoManager.GetInsertOrUpdateCmd<FCVoucherModuleModel>(ctx, model, null, true);
			insertOrUpdateCmd.AddRange(new GLVoucherReferenceRepository().GetInsertReferenceCmds(ctx, (from x in model.MVoucherModuleEntrys
			where !string.IsNullOrWhiteSpace(x.MExplanation)
			select x.MExplanation).ToList()));
			int num = new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(insertOrUpdateCmd);
			if (num <= 0)
			{
				model.MItemID = string.Empty;
			}
			return model;
		}

		private void GetInsertCheckGroupValue(MContext ctx, List<FCVoucherModuleModel> vouchers)
		{
			List<FCVoucherModuleEntryModel> list = utility.Union((from x in vouchers
			select x.MVoucherModuleEntrys).ToList());
			for (int i = 0; i < list.Count; i++)
			{
				list[i].MCheckGroupValueModel.MItemID = utility.GetCheckGroupValueModel(ctx, list[i].MCheckGroupValueModel).MItemID;
				list[i].MCheckGroupValueID = list[i].MCheckGroupValueModel.MItemID;
			}
		}

		public FCVoucherModuleModel GetVoucherModuleByFastCode(MContext ctx, string fastCode)
		{
			string sql = "select * from  t_fc_vouchermodule t where t.MFastCode = @MFastCode and t.MOrgID = @MOrgID and MLCID = @MLCID and t.MIsDelete = 0 ";
			MySqlParameter[] cmdParms = new MySqlParameter[3]
			{
				new MySqlParameter
				{
					ParameterName = "@MOrgID",
					Value = ctx.MOrgID
				},
				new MySqlParameter
				{
					ParameterName = "MFastCode",
					Value = fastCode
				},
				new MySqlParameter
				{
					ParameterName = "@MLCID",
					Value = ctx.MLCID
				}
			};
			DataSet ds = new DynamicDbHelperMySQL(ctx).Query(sql, cmdParms);
			return ModelInfoManager.DataTableToList<FCVoucherModuleModel>(ds).FirstOrDefault();
		}

		public FCVoucherModuleModel GetVoucherModule(MContext ctx, string pkID = null)
		{
			if (!string.IsNullOrWhiteSpace(pkID))
			{
				return GetVoucherModuleModel(ctx, pkID);
			}
			return GetNewVoucherModule(ctx);
		}

		private FCVoucherModuleModel GetNewVoucherModule(MContext ctx)
		{
			FCVoucherModuleModel fCVoucherModuleModel = new FCVoucherModuleModel();
			fCVoucherModuleModel.MCreatorName = GlobalFormat.GetUserName(ctx.MFirstName, ctx.MLastName, null);
			FCVoucherModuleEntryModel item = new FCVoucherModuleEntryModel
			{
				MAccountModel = new BDAccountModel
				{
					MCurrencyDataModel = new GLCurrencyDataModel(),
					MCheckGroupModel = new GLCheckGroupModel(),
					MCheckGroupValueModel = new GLCheckGroupValueModel()
				}
			};
			fCVoucherModuleModel.MVoucherModuleEntrys = new List<FCVoucherModuleEntryModel>
			{
				item,
				item,
				item,
				item,
				item
			};
			return fCVoucherModuleModel;
		}

		public OperationResult Delete(MContext ctx, List<string> pkIdList)
		{
			OperationResult operationResult = new OperationResult();
			List<CommandInfo> list = new List<CommandInfo>();
			List<MySqlParameter> list2 = new List<MySqlParameter>();
			string inSql = BDRepository.GetInSql(ctx, pkIdList, out list2);
			list2.Add(new MySqlParameter
			{
				ParameterName = "@MOrgID",
				Value = ctx.MOrgID
			});
			CommandInfo obj = new CommandInfo
			{
				CommandText = "UPDATE t_fc_vouchermodule SET MIsDelete = 1 WHERE MOrgID = @MOrgID AND mitemid IN " + inSql
			};
			DbParameter[] array = obj.Parameters = list2.ToArray();
			CommandInfo item = obj;
			list.Add(item);
			CommandInfo obj2 = new CommandInfo
			{
				CommandText = " UPDATE t_fc_vouchermoduleentry SET MIsDelete = 1 Where MOrgID = @MOrgID AND MID IN " + inSql
			};
			array = (obj2.Parameters = list2.ToArray());
			CommandInfo item2 = obj2;
			list.Add(item2);
			operationResult.Success = (BDRepository.ExecuteSqlTran(ctx, list) > 0);
			return operationResult;
		}

		public List<FCVoucherModuleModel> GetVoucherModuleModelList(MContext ctx, List<string> pkIDS)
		{
			List<FCVoucherModuleModel> result = new List<FCVoucherModuleModel>();
			GLVoucherListFilterModel gLVoucherListFilterModel = new GLVoucherListFilterModel();
			if (pkIDS != null && pkIDS.Count > 0)
			{
				gLVoucherListFilterModel.MItemID = "'" + string.Join("','", pkIDS) + "'";
			}
			List<MySqlParameter> parameterList = GetParameterList(ctx, gLVoucherListFilterModel);
			string voucherModuleSelectSql = GetVoucherModuleSelectSql(gLVoucherListFilterModel, false, false);
			voucherModuleSelectSql += " ORDER BY t1.MItemID,t1.MFastCode,t2.Mentryseq ";
			DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(voucherModuleSelectSql, parameterList.ToArray());
			if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
			{
				result = DataTable2VoucherModuleList(ctx, dataSet.Tables[0]);
			}
			return result;
		}

		public List<MySqlParameter> GetParameterList(MContext ctx, GLVoucherListFilterModel filter)
		{
			List<MySqlParameter> list = new List<MySqlParameter>();
			list.Add(new MySqlParameter
			{
				ParameterName = "@Keyword",
				Value = filter.KeyWord,
				MySqlDbType = MySqlDbType.VarChar
			});
			list.Add(new MySqlParameter
			{
				ParameterName = "@MLCID",
				Value = ctx.MLCID,
				MySqlDbType = MySqlDbType.VarChar
			});
			list.Add(new MySqlParameter
			{
				ParameterName = "@MOrgID",
				Value = ctx.MOrgID,
				MySqlDbType = MySqlDbType.VarChar
			});
			list.Add(new MySqlParameter
			{
				ParameterName = "@MFastCode",
				Value = filter.MFastCode,
				MySqlDbType = MySqlDbType.VarChar
			});
			list.Add(new MySqlParameter
			{
				ParameterName = "@MDescription",
				Value = filter.MDescription,
				MySqlDbType = MySqlDbType.VarChar
			});
			list.Add(new MySqlParameter
			{
				ParameterName = "@MIsMulti",
				Value = filter.MIsMulti,
				MySqlDbType = MySqlDbType.VarChar
			});
			return list;
		}

		public List<FCVoucherModuleModel> GetVoucherModulePageList(MContext ctx, GLVoucherListFilterModel filter, bool includeDraft = false)
		{
			List<FCVoucherModuleModel> result = new List<FCVoucherModuleModel>();
			List<MySqlParameter> parameterList = GetParameterList(ctx, filter);
			string voucherModuleSelectSql = GetVoucherModuleSelectSql(filter, false, true);
			DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(voucherModuleSelectSql, parameterList.ToArray());
			if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
			{
				result = DataTable2VoucherModuleList(ctx, dataSet.Tables[0]);
			}
			return result;
		}

		public string GetVoucherModuleSelectSql(GLVoucherListFilterModel filter, bool isCount = false, bool isPager = true)
		{
			string str = "";
			str = ((!isCount) ? (str + " \r\n                     SELECT t1.MItemID,\n                        t1.MOrgID,\n                        t1.MDescription,\n                        t1.MFastCode,\n                        t1.MIsMulti,\n                        t1.MLCID,\n                        t2.MEntryID,\n                        t2.MID,\n                        t2.MExplanation,\n                        t2.MAccountID,\n                        t2.MCheckGroupValueID,\n                        t2.MAmountFor,\n                        t2.MAmount,\n                        t2.MCurrencyID,\n                        t2.MExchangeRate,\n                        t2.MDC,\n                        t2.MDebit,\n                        t2.MCredit,\n                        t2.MEntrySeq,\n                        t4.MNumber AS MAccountNo,\n                        t3.MName AS MAccountNameOnly,\n                        t3.MFullName AS MAccountName,\n                        t4.MIsCheckForCurrency,\n                        t4.MCheckGroupID,\n                        t4.MCode AS MAccountCode,\n                        t4.MAccountTypeID,\n                        t5_0.MContactID AS MContact,\n                        t5_0.MEmployeeID AS MEmployee,\n                        t5_0.MMerItemID AS MMerItem,\n                        t5_0.MExpItemID AS MExpItem,\n                        t5_0.MPaItemID AS MPaItem,\n                        t5_0.MTrackItem1 AS MTrack1,\n                        t5_0.MTrackItem2 AS MTrack2,\n                        t5_0.MTrackItem3 AS MTrack3,\n                        t5_0.MTrackItem4 AS MTrack4,\n                        t5_0.MTrackItem5 AS MTrack5,\n                        t5.MContactID,\n                        t5.MEmployeeID,\n                        t5.MMerItemID,\n                        t5.MExpItemID,\n                        t5.MPaItemID,\n                        t5.MTrackItem1,\n                        t5.MTrackItem2,\n                        t5.MTrackItem3,\n                        t5.MTrackItem4,\n                        t5.MTrackItem5,\n                        CONVERT( AES_DECRYPT(t6.MName, '{0}') USING UTF8) AS MContactName,\n                        F_GETUSERNAME(t7.MFirstName, t7.MLastName) AS MEmployeeName,\n                        concat(t8_0.MNumber,':', t8.MDesc) AS MMerItemName,\n                        t9.MName AS MExpItemName,\n                        t10.MName AS MPaItemName,\n                        t10_0.MGroupID AS MPaItemGroupID,\n                        t10_1.MName AS MPaItemGroupName,\n                        t11.MName AS MTrackItem1Name,\n                        t11_2.MName AS MTrackItem1GroupName,\n                        t12.MName AS MTrackItem2Name,\n                        t12_2.MName AS MTrackItem2GroupName,\n                        t13.MName AS MTrackItem3Name,\n                        t13_2.MName AS MTrackItem3GroupName,\n                        t14.MName AS MTrackItem4Name,\n                        t14_2.MName AS MTrackItem4GroupName,\n                        t15.MName AS MTrackItem5Name,\r\n                        t15_2.MName AS MTrackItem5GroupName") : (str + " SELECT COUNT(*) AS total From (SELECT t1.MItemID "));
			str += " FROM ";
			str += "( SELECT *\r\n                FROM t_fc_vouchermodule\r\n                WHERE MOrgID = @MOrgID  AND MIsDelete = 0 and MLCID = @MLCID  ";
			if (!string.IsNullOrWhiteSpace(filter.MItemID))
			{
				str += $" AND MItemID in ({filter.MItemID}) ";
			}
			str += "  ) t1 \r\n                INNER JOIN\r\n                t_fc_vouchermoduleentry t2 \r\n                ON t1.MItemID = t2.MID AND t2.MOrgID = t1.MOrgID AND t2.MIsDelete = 0 \r\n                LEFT JOIN  \r\n                t_bd_account_l t3 \r\n                ON t3.MParentID = t2.MAccountID AND t3.MLocaleID =@MLCID AND t3.MOrgID = t1.MOrgID  And t3.MIsDelete = 0\r\n                LEFT JOIN \r\n                t_bd_account t4 \r\n                ON t4.MItemID = t2.MAccountID AND t4.MOrgID = t1.MOrgID  AND t4.MIsDelete = 0 \r\n                LEFT JOIN \r\n                t_gl_checkgroup t5_0 \r\n                ON t4.MCheckGroupID = t5_0.MItemID  AND t5_0.MIsDelete = 0 \r\n                LEFT JOIN \r\n                t_gl_checkgroupvalue t5 \r\n                ON t5.MItemID = t2.MCheckGroupValueID AND t5.MOrgID = t1.MOrgID AND t5.MIsDelete = t1.MIsDelete \r\n                LEFT JOIN \r\n                t_bd_contacts_l t6 \r\n                ON t6.MParentID = t5.MContactID AND t6.MLocaleID = t3.MLocaleID AND t6.MOrgID = t1.MOrgID AND t6.MIsDelete = 0\r\n                LEFT JOIN \r\n                t_bd_employees_l t7 \r\n                ON t7.MParentID = t5.MEmployeeID AND t7.MOrgID = t1.MOrgId AND t7.MIsDelete = t1.MIsDelete AND t7.MLocaleID = t3.MLocaleID\r\n                LEFT JOIN \r\n                t_bd_item t8_0 \r\n                ON t8_0.MItemID = t5.MMerItemID AND t8_0.MOrgID = t1.MOrgId AND t8_0.MIsDelete = t1.MIsDelete\r\n                LEFT JOIN \r\n                t_bd_item_l t8 \r\n                ON t8.MParentID = t5.MMerItemID AND t8.MOrgID = t1.MOrgId AND t8.MIsDelete = t1.MIsDelete AND t8.MLocaleID = t3.MLocaleID\r\n                LEFT JOIN \r\n                t_bd_expenseitem_l t9 \r\n                ON t9.MParentID = t5.MExpItemID AND t9.MOrgID = t1.MOrgId AND t9.MIsDelete = t1.MIsDelete AND t9.MLocaleID = t3.MLocaleID\r\n                LEFT JOIN \r\n                t_pa_payitem_l t10 \r\n                ON t10.MParentID = t5.MPaItemID AND t10.MOrgID = t1.MOrgId AND t10.MIsDelete = t1.MIsDelete AND t10.MLocaleID = t3.MLocaleID\r\n                LEFT JOIN\r\n                t_pa_payitem t10_0\r\n                ON t10_0.MItemID = t5.MPaItemID AND t10_0.MOrgID = t1.MOrgId AND t10_0.MIsDelete = t1.MIsDelete \r\n                LEFT JOIN \r\n\r\n                t_pa_payitemgroup_l t10_1 \r\n                ON t10_1.MParentID = t5.MPaItemID AND t10_1.MOrgID = t1.MOrgId AND t10_1.MIsDelete = t1.MIsDelete AND t10_1.MLocaleID = t3.MLocaleID\r\n\r\n\r\n                LEFT JOIN \r\n                t_bd_trackentry_l t11 \r\n                ON t11.MParentID = t5.MTrackItem1 AND t11.MLocaleID = t3.MLocaleID AND t11.MOrgID = t1.MOrgID  AND t11.MIsDelete = 0\r\n                LEFT JOIN \r\n                t_bd_trackentry t11_1 \r\n                ON t11_1.MEntryID = t5.MTrackItem1 AND t11_1.MOrgID = t1.MOrgID   AND t11_1.MIsDelete = 0\r\n                LEFT JOIN \r\n                t_bd_track_l t11_2 \r\n                ON t11_2.MParentID = t11_1.MItemID AND t11_2.MLocaleID = t3.MLocaleID AND t11_2.MOrgID = t1.MOrgID  AND t11_2.MIsDelete = 0\r\n\r\n                 LEFT JOIN \r\n                t_bd_trackentry_l t12 \r\n                ON t12.MParentID = t5.MTrackItem2 AND t12.MLocaleID = t3.MLocaleID AND t12.MOrgID = t1.MOrgID  AND t12.MIsDelete = 0\r\n                LEFT JOIN \r\n                t_bd_trackentry t12_1 \r\n                ON t12_1.MEntryID = t5.MTrackItem2 AND t12_1.MOrgID = t1.MOrgID  AND t12_1.MIsDelete = 0\r\n                LEFT JOIN \r\n                t_bd_track_l t12_2 \r\n                ON t12_2.MParentID = t12_1.MItemID AND t12_2.MLocaleID = t3.MLocaleID AND t12_2.MOrgID = t1.MOrgID  AND t12_2.MIsDelete = 0\r\n\r\n                LEFT JOIN \r\n                t_bd_trackentry_l t13 \r\n                ON t13.MParentID = t5.MTrackItem3 AND t13.MLocaleID = t3.MLocaleID AND t13.MOrgID = t1.MOrgID  AND t13.MIsDelete = 0\r\n                LEFT JOIN \r\n                t_bd_trackentry t13_1 \r\n                ON t13_1.MEntryID = t5.MTrackItem3 AND t13_1.MOrgID = t1.MOrgID  AND t13_1.MIsDelete = 0\r\n                LEFT JOIN \r\n                t_bd_track_l t13_2 \r\n                ON t13_2.MParentID = t13_1.MItemID AND t13_2.MLocaleID = t3.MLocaleID AND t13_2.MOrgID = t1.MOrgID  AND t13_2.MIsDelete = 0\r\n\r\n                LEFT JOIN \r\n                t_bd_trackentry_l t14 \r\n                ON t14.MParentID = t5.MTrackItem4 AND t14.MLocaleID = t3.MLocaleID AND t14.MOrgID = t1.MOrgID  AND t14.MIsDelete = 0\r\n                LEFT JOIN \r\n                t_bd_trackentry t14_1 \r\n                ON t14_1.MEntryID = t5.MTrackItem4 AND t14_1.MOrgID = t1.MOrgID  AND t14_1.MIsDelete = 0\r\n                LEFT JOIN \r\n                t_bd_track_l t14_2 \r\n                ON t14_2.MParentID = t14_1.MItemID AND t14_2.MLocaleID = t3.MLocaleID AND t14_2.MOrgID = t1.MOrgID AND t14_2.MIsDelete = 0\r\n\r\n                LEFT JOIN \r\n                t_bd_trackentry_l t15 \r\n                ON t15.MParentID = t5.MTrackItem5 AND t15.MLocaleID = t3.MLocaleID AND t15.MOrgID = t1.MOrgID   AND t15.MIsDelete = 0\r\n                LEFT JOIN \r\n                t_bd_trackentry t15_1 \r\n                ON t15_1.MEntryID = t5.MTrackItem5 AND t15_1.MOrgID = t1.MOrgID AND t15_1.MIsDelete = 0\r\n                LEFT JOIN \r\n                t_bd_track_l t15_2 \r\n                ON t15_2.MParentID = t15_1.MItemID AND t15_2.MLocaleID = t3.MLocaleID AND t15_2.MOrgID = t1.MOrgID  AND t15_2.MIsDelete = 0\r\n                ";
			if (isPager && !isCount)
			{
				str += " WHERE t1.MItemID IN ( SELECT * FROM (\r\n                SELECT t1.MItemID FROM t_fc_vouchermodule t1 \r\n                 INNER JOIN \r\n                t_fc_vouchermoduleentry t2 \r\n                ON t1.MItemID = t2.MID AND t2.MOrgID = t1.MOrgID AND t2.MIsDelete = 0 \r\n                LEFT JOIN \r\n                t_bd_account_l t3 \r\n                ON t3.MParentID = t2.MAccountID AND t3.MLocaleID =@MLCID AND t3.MOrgID = t1.MOrgID  And t3.MIsDelete = 0\r\n                 LEFT JOIN  \r\n               t_bd_account t4 \r\n                ON t4.MItemID = t2.MAccountID AND t4.MOrgID = t1.MOrgID  AND t4.MIsDelete = 0 \r\n                LEFT JOIN \r\n                t_gl_checkgroupvalue t5 \r\n                ON t5.MItemID = t2.MCheckGroupValueID AND t5.MOrgID = t1.MOrgID AND t5.MIsDelete = t1.MIsDelete  ";
				if (!string.IsNullOrWhiteSpace(filter.KeyWord))
				{
					str += " LEFT JOIN \r\n                     t_bd_contacts_l t6 \r\n                     ON t6.MParentID = t5.MContactID AND t6.MLocaleID = t3.MLocaleID AND t6.MOrgID = t1.MOrgID  AND t6.MIsDelete = 0\r\n                      LEFT JOIN \r\n                      t_bd_employees_l t7 \r\n                      ON t7.MParentID = t5.MEmployeeID AND t7.MOrgID = t1.MOrgId AND t7.MIsDelete = t1.MIsDelete AND t7.MLocaleID = t3.MLocaleID \r\n                      LEFT JOIN \r\n                      t_bd_item t8_0 \r\n                     ON t8_0.MItemID = t5.MMerItemID AND t8_0.MOrgID = t1.MOrgId AND t8_0.MIsDelete = t1.MIsDelete\r\n                     LEFT JOIN \r\n                      t_bd_item_l t8 \r\n                     ON t8.MParentID = t5.MMerItemID AND t8.MOrgID = t1.MOrgId AND t8.MIsDelete = t1.MIsDelete AND t8.MLocaleID = t3.MLocaleID \r\n                     LEFT JOIN \r\n                     t_bd_expenseitem_l t9 \r\n                     ON t9.MParentID = t5.MExpItemID AND t9.MOrgID = t1.MOrgId AND t9.MIsDelete = t1.MIsDelete AND t9.MLocaleID = t3.MLocaleID \r\n                     LEFT JOIN \r\n                     t_pa_payitem_l t10 \r\n                     ON t10.MParentID = t5.MPaItemID AND t10.MOrgID = t1.MOrgId AND t10.MIsDelete = t1.MIsDelete AND t10.MLocaleID = t3.MLocaleID \r\n                     \r\n                     LEFT JOIN \r\n                     t_pa_payitemgroup_l t10_1 \r\n                     ON t10_1.MParentID = t5.MPaItemID AND t10_1.MOrgID = t1.MOrgId AND t10_1.MIsDelete = t1.MIsDelete AND t10_1.MLocaleID = t3.MLocaleID\r\n\r\n\r\n                     LEFT JOIN \r\n                     t_bd_trackentry_l t11 \r\n                     ON t11.MParentID = t5.MTrackItem1 AND t11.MLocaleID = t3.MLocaleID AND t11.MOrgID = t1.MOrgID  AND t11.MIsDelete = 0\r\n                     LEFT JOIN \r\n                     t_bd_trackentry t11_1 \r\n                     ON t11_1.MEntryID = t5.MTrackItem1 AND t11_1.MOrgID = t1.MOrgID AND t11_1.MIsDelete = 0\r\n                     LEFT JOIN \r\n                     t_bd_track_l t11_2 \r\n                     ON t11_2.MParentID = t11_1.MItemID AND t11_2.MLocaleID = t3.MLocaleID AND t11_2.MOrgID = t1.MOrgID  AND t11_2.MIsDelete = 0\r\n\r\n                     LEFT JOIN \r\n                     t_bd_trackentry_l t12 \r\n                     ON t12.MParentID = t5.MTrackItem2 AND t12.MLocaleID = t3.MLocaleID AND t12.MOrgID = t1.MOrgID  AND t12.MIsDelete = 0\r\n                     LEFT JOIN \r\n                     t_bd_trackentry t12_1 \r\n                     ON t12_1.MEntryID = t5.MTrackItem2 AND t12_1.MOrgID = t1.MOrgID  AND t12_1.MIsDelete = 0\r\n                     LEFT JOIN \r\n                     t_bd_track_l t12_2 \r\n                     ON t12_2.MParentID = t12_1.MItemID AND t12_2.MLocaleID = t3.MLocaleID AND t12_2.MOrgID = t1.MOrgID  AND t12_2.MIsDelete = 0\r\n\r\n                     LEFT JOIN \r\n                     t_bd_trackentry_l t13 \r\n                     ON t13.MParentID = t5.MTrackItem3 AND t13.MLocaleID = t3.MLocaleID AND t13.MOrgID = t1.MOrgID  AND t13.MIsDelete = 0\r\n                     LEFT JOIN \r\n                     t_bd_trackentry t13_1 \r\n                     ON t13_1.MEntryID = t5.MTrackItem3 AND t13_1.MOrgID = t1.MOrgID  AND t13_1.MIsDelete = 0\r\n                     LEFT JOIN \r\n                     t_bd_track_l t13_2 \r\n                     ON t13_2.MParentID = t13_1.MItemID AND t13_2.MLocaleID = t3.MLocaleID AND t13_2.MOrgID = t1.MOrgID  AND t13_2.MIsDelete = 0\r\n\r\n                     LEFT JOIN \r\n                     t_bd_trackentry_l t14 \r\n                     ON t14.MParentID = t5.MTrackItem4 AND t14.MLocaleID = t3.MLocaleID AND t14.MOrgID = t1.MOrgID  AND t14.MIsDelete = 0\r\n                     LEFT JOIN \r\n                     t_bd_trackentry t14_1 \r\n                     ON t14_1.MEntryID = t5.MTrackItem4 AND t14_1.MOrgID = t1.MOrgID  AND t14_1.MIsDelete = 0\r\n                     LEFT JOIN \r\n                     t_bd_track_l t14_2 \r\n                     ON t14_2.MParentID = t14_1.MItemID AND t14_2.MLocaleID = t3.MLocaleID AND t14_2.MOrgID = t1.MOrgID  AND t14_2.MIsDelete = 0\r\n\r\n                     LEFT JOIN \r\n                     t_bd_trackentry_l t15 \r\n                     ON t15.MParentID = t5.MTrackItem5 AND t15.MLocaleID = t3.MLocaleID AND t15.MOrgID = t1.MOrgID  AND t15.MIsDelete = 0\r\n                     LEFT JOIN \r\n                     t_bd_trackentry t15_1 \r\n                     ON t15_1.MEntryID = t5.MTrackItem5 AND t15_1.MOrgID = t1.MOrgID  AND t15_1.MIsDelete = 0\r\n                     LEFT JOIN \r\n                     t_bd_track_l t15_2 \r\n                     ON t15_2.MParentID = t15_1.MItemID AND t15_2.MLocaleID = t3.MLocaleID AND t15_2.MOrgID = t1.MOrgID  AND t15_2.MIsDelete = 0\r\n                     ";
				}
				str += " WHERE t1.MOrgID =  @MOrgID  AND t1.MIsDelete = 0  ";
				if (!string.IsNullOrWhiteSpace(filter.KeyWord))
				{
					str += " AND ( t1.MFastCode LIKE concat('%',@Keyword,'%') \r\n                     OR t1.MDescription LIKE concat('%',@Keyword,'%') \r\n                     OR t2.MEXPLANATION LIKE concat('%',@Keyword,'%')\r\n                     OR t3.MFullName LIKE concat('%',@Keyword,'%')\r\n                     OR CONVERT( AES_DECRYPT(t6.MName, '{0}') USING UTF8) LIKE concat('%',@Keyword,'%')\r\n                     OR F_GETUSERNAME(t7.MFirstName, t7.MLastName) LIKE concat('%',@Keyword,'%')\r\n                     OR t8.MDesc LIKE concat('%',@Keyword,'%') \r\n                     OR t8_0.MNumber LIKE concat('%',@Keyword,'%') \r\n                     OR t9.MName LIKE concat('%',@Keyword,'%') \r\n                     OR t10.MName LIKE concat('%',@Keyword,'%') \r\n                     OR t10_1.MName LIKE concat('%',@Keyword,'%') \r\n                     OR t11.MName  LIKE concat('%',@Keyword,'%')\r\n                     OR t12.MName  LIKE concat('%',@Keyword,'%') \r\n                     OR t13.MName  LIKE concat('%',@Keyword,'%') \r\n                     OR t14.MName  LIKE concat('%',@Keyword,'%') \r\n                     OR t15.MName  LIKE concat('%',@Keyword,'%'))";
				}
				if (!string.IsNullOrWhiteSpace(filter.MFastCode))
				{
					str += " and t1.MFastCode = @MFastCode ";
				}
				if (!string.IsNullOrWhiteSpace(filter.MDescription))
				{
					str += " and t1.MDescription = @MDescription ";
				}
				if (!string.IsNullOrWhiteSpace(filter.MIsMulti))
				{
					str += " and t1.MIsMulti = @MIsMulti ";
				}
				if (!string.IsNullOrWhiteSpace(filter.AccountCode))
				{
					str += " and t4.MCode = @MCode ";
				}
				if (!string.IsNullOrWhiteSpace(filter.AccountTypeID))
				{
					str += " and t4.MAccountTypeID = @MAccountTypeID ";
				}
				str += " GROUP BY t1.MITEMID  ";
				str = str + " ORDER BY t1.MFastCode DESC LIMIT " + (filter.page - 1) * filter.rows + "," + filter.rows + " ) t20 ) ";
				str += " ORDER BY t1.MFastCode DESC,t2.Mentryseq ";
			}
			else
			{
				str += " WHERE t1.MOrgID =  @MOrgID  AND t1.MIsDelete = 0  ";
				if (!string.IsNullOrWhiteSpace(filter.KeyWord))
				{
					str += " AND ( t1.MFastCode LIKE concat('%',@Keyword,'%') \r\n                     OR t1.MDescription LIKE concat('%',@Keyword,'%') \r\n                     OR t2.MEXPLANATION LIKE concat('%',@Keyword,'%')\r\n                     OR t3.MFullName LIKE concat('%',@Keyword,'%')\r\n                     OR CONVERT( AES_DECRYPT(t6.MName, '{0}') USING UTF8) LIKE concat('%',@Keyword,'%')\r\n                     OR F_GETUSERNAME(t7.MFirstName, t7.MLastName) LIKE concat('%',@Keyword,'%')\r\n                     OR t8.MDesc LIKE concat('%',@Keyword,'%') \r\n                     OR t8_0.MNumber LIKE concat('%',@Keyword,'%') \r\n                     OR t9.MName LIKE concat('%',@Keyword,'%') \r\n                     OR t10.MName LIKE concat('%',@Keyword,'%') \r\n                     OR t10_1.MName LIKE concat('%',@Keyword,'%')\r\n                     OR t11.MName  LIKE concat('%',@Keyword,'%')\r\n                     OR t12.MName  LIKE concat('%',@Keyword,'%') \r\n                     OR t13.MName  LIKE concat('%',@Keyword,'%') \r\n                     OR t14.MName  LIKE concat('%',@Keyword,'%') \r\n                     OR t15.MName  LIKE concat('%',@Keyword,'%') )";
				}
				if (!string.IsNullOrWhiteSpace(filter.MFastCode))
				{
					str += " and t1.MFastCode = @MFastCode ";
				}
				if (!string.IsNullOrWhiteSpace(filter.MDescription))
				{
					str += " and t1.MDescription = @MDescription ";
				}
				if (!string.IsNullOrWhiteSpace(filter.MIsMulti))
				{
					str += " and t1.MIsMulti = @MIsMulti ";
				}
				if (!string.IsNullOrWhiteSpace(filter.AccountCode))
				{
					str += " and t4.MCode = @MCode ";
				}
				if (isCount)
				{
					str += " GROUP BY t1.MItemID ) t100  ";
				}
			}
			return string.Format(str, "JieNor-001");
		}

		public int GetVoucherModulePageListCount(MContext ctx, GLVoucherListFilterModel filter)
		{
			string voucherModuleSelectSql = GetVoucherModuleSelectSql(filter, true, true);
			List<MySqlParameter> parameterList = GetParameterList(ctx, filter);
			DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(voucherModuleSelectSql, parameterList.ToArray());
			if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
			{
				int result = 0;
				if (int.TryParse(dataSet.Tables[0].Rows[0]["total"].ToString(), out result))
				{
					return result;
				}
			}
			return 0;
		}

		public List<FCVoucherModuleModel> DataTable2VoucherModuleList(MContext ctx, DataTable dt)
		{
			List<FCVoucherModuleModel> list = new List<FCVoucherModuleModel>();
			FCVoucherModuleModel fCVoucherModuleModel = new FCVoucherModuleModel();
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				DataRow row = dt.Rows[i];
				string text = row.MField<string>("MItemID");
				if (text != fCVoucherModuleModel.MItemID)
				{
					if (!string.IsNullOrWhiteSpace(fCVoucherModuleModel.MItemID))
					{
						list.Add(fCVoucherModuleModel);
					}
					fCVoucherModuleModel = new FCVoucherModuleModel
					{
						MItemID = text,
						MOrgID = row.MField<string>("MOrgID"),
						MDescription = row.MField<string>("MDescription"),
						MLCID = row.MField<string>("MLCID"),
						MFastCode = row.MField<string>("MFastCode"),
						MIsMulti = row.MField<bool>("MIsMulti"),
						MVoucherModuleEntrys = new List<FCVoucherModuleEntryModel>()
					};
				}
				FCVoucherModuleEntryModel fCVoucherModuleEntryModel = new FCVoucherModuleEntryModel
				{
					MEntryID = row.MField<string>("MEntryID"),
					MID = row.MField<string>("MID"),
					MOrgID = row.MField<string>("MOrgID"),
					MExplanation = row.MField<string>("MExplanation"),
					MAccountID = row.MField<string>("MAccountID"),
					MAccountNo = row.MField<string>("MAccountNo"),
					MAccountNameOnly = row.MField<string>("MAccountNameOnly"),
					MAccountName = row.MField<string>("MAccountName"),
					MAccountCode = row.MField<string>("MAccountCode"),
					MCheckGroupValueID = row.MField<string>("MCheckGroupValueID"),
					MIsCheckForCurrency = row.MField<bool>("MIsCheckForCurrency"),
					MCurrencyID = row.MField<string>("MCurrencyID"),
					MExchangeRate = row.MField<decimal>("MExchangeRate"),
					MDC = row.MField<int>("MDC"),
					MAmount = row.MField<decimal>("MAmount"),
					MAmountFor = row.MField<decimal>("MAmountFor"),
					MCredit = row.MField<decimal>("MCredit"),
					MDebit = row.MField<decimal>("MDebit"),
					MEntrySeq = row.MField<int>("MEntrySeq")
				};
				BDAccountModel bDAccountModel = new BDAccountModel
				{
					MItemID = fCVoucherModuleEntryModel.MAccountID,
					MFullName = row.MField<string>("MAccountName"),
					MCheckGroupID = fCVoucherModuleEntryModel.MCheckGroupID,
					MIsCheckForCurrency = row.MField<bool>("MIsCheckForCurrency"),
					MDC = row.MField<int>("MDC")
				};
				GLCurrencyDataModel obj = new GLCurrencyDataModel
				{
					MCurrencyID = fCVoucherModuleEntryModel.MCurrencyID,
					MAmount = fCVoucherModuleEntryModel.MAmount,
					MAmountFor = fCVoucherModuleEntryModel.MAmountFor,
					MExchangeRate = fCVoucherModuleEntryModel.MExchangeRate
				};
				GLCurrencyDataModel gLCurrencyDataModel2 = bDAccountModel.MCurrencyDataModel = obj;
				GLCheckGroupModel obj2 = new GLCheckGroupModel
				{
					MItemID = row.MField<string>("MCheckGroupID"),
					MContactID = row.MField<int>("MContact"),
					MEmployeeID = row.MField<int>("MEmployee"),
					MMerItemID = row.MField<int>("MMerItem"),
					MExpItemID = row.MField<int>("MExpItem"),
					MPaItemID = row.MField<int>("MPaItem"),
					MTrackItem1 = row.MField<int>("MTrack1"),
					MTrackItem2 = row.MField<int>("MTrack2"),
					MTrackItem3 = row.MField<int>("MTrack3"),
					MTrackItem4 = row.MField<int>("MTrack4"),
					MTrackItem5 = row.MField<int>("MTrack5")
				};
				GLCheckGroupModel gLCheckGroupModel2 = bDAccountModel.MCheckGroupModel = obj2;
				fCVoucherModuleEntryModel.MCheckGroupValueModel = new GLCheckGroupValueModel
				{
					MItemID = row.MField<string>("MCheckGroupValueID"),
					MTrackItem1 = row.MField<string>("MTrackItem1"),
					MTrackItem2 = row.MField<string>("MTrackItem2"),
					MTrackItem3 = row.MField<string>("MTrackItem3"),
					MTrackItem4 = row.MField<string>("MTrackItem4"),
					MTrackItem5 = row.MField<string>("MTrackItem5"),
					MTrackItem1Name = row.MField<string>("MTrackItem1Name"),
					MTrackItem2Name = row.MField<string>("MTrackItem2Name"),
					MTrackItem3Name = row.MField<string>("MTrackItem3Name"),
					MTrackItem4Name = row.MField<string>("MTrackItem4Name"),
					MTrackItem5Name = row.MField<string>("MTrackItem5Name"),
					MTrackItem1GroupName = row.MField<string>("MTrackItem1GroupName"),
					MTrackItem2GroupName = row.MField<string>("MTrackItem2GroupName"),
					MTrackItem3GroupName = row.MField<string>("MTrackItem3GroupName"),
					MTrackItem4GroupName = row.MField<string>("MTrackItem4GroupName"),
					MTrackItem5GroupName = row.MField<string>("MTrackItem5GroupName"),
					MContactID = row.MField<string>("MContactID"),
					MEmployeeID = row.MField<string>("MEmployeeID"),
					MMerItemID = row.MField<string>("MMerItemID"),
					MExpItemID = row.MField<string>("MExpItemID"),
					MPaItemID = row.MField<string>("MPaItemID"),
					MPaItemGroupID = row.MField<string>("MPaItemGroupID"),
					MPaItemGroupName = row.MField<string>("MPaItemGroupName"),
					MContactName = row.MField<string>("MContactName"),
					MEmployeeName = row.MField<string>("MEmployeeName"),
					MMerItemName = row.MField<string>("MMerItemName"),
					MExpItemName = row.MField<string>("MExpItemName"),
					MPaItemName = row.MField<string>("MPaItemName")
				};
				if (!string.IsNullOrWhiteSpace(fCVoucherModuleEntryModel.MCheckGroupValueModel.MPaItemGroupName))
				{
					fCVoucherModuleEntryModel.MCheckGroupValueModel.MPaItemGroupID = fCVoucherModuleEntryModel.MCheckGroupValueModel.MPaItemID;
					fCVoucherModuleEntryModel.MCheckGroupValueModel.MPaItemName = fCVoucherModuleEntryModel.MCheckGroupValueModel.MPaItemGroupName;
				}
				bDAccountModel.MCheckGroupValueModel = fCVoucherModuleEntryModel.MCheckGroupValueModel;
				fCVoucherModuleEntryModel.MAccountModel = bDAccountModel;
				fCVoucherModuleModel.MVoucherModuleEntrys.Add(fCVoucherModuleEntryModel);
			}
			list.Add(fCVoucherModuleModel);
			return list;
		}

		public FCVoucherModuleModel GetVoucherModuleModel(MContext ctx, string itemID)
		{
			string format = "\r\n                SELECT \r\n                t1.MOrgID,\r\n                t1.MEntryID,\r\n                t1.MID,\r\n                t1.MExplanation,\r\n                t1.MAmount,\r\n                t1.MAmountFor,\r\n                t1.MAccountID,\r\n                t1.MCheckGroupValueID,\r\n                t1.MCurrencyID,\r\n                t1.MExchangeRate,\r\n                t1.MDC,\r\n                t1.MDebit,\r\n                t1.MCredit,\r\n                t1.MEntrySeq,\r\n                t2.MLCID,\r\n                t2.MDescription,\r\n                t2.MIsMulti,\r\n                t2.MFastCode,\r\n               (case when length(ifnull(t3_0.MItemID,'')) > 0 then '1' else '0' end) as MIsBankAccount,\r\n                t3.MIsCheckForCurrency,\r\n                t3.MCheckGroupID,\r\n                t3.MDC as MAccountDC,\r\n                t3.MNumber as MAccountNumber,\r\n                t3_1.MContactID as MCheckGroupContactID,\r\n                t3_1.MEmployeeID as MCheckGroupEmployeeID,\r\n                t3_1.MMerItemID as MCheckGroupMerItemID,\r\n                t3_1.MExpItemID as MCheckGroupExpItemID,\r\n                t3_1.MPaItemID as MCheckGroupPaItemID,\r\n                t3_1.MTrackItem1 as MCheckGroupTrackItem1,\r\n                t3_1.MTrackItem2 as MCheckGroupTrackItem2,\r\n                t3_1.MTrackItem3 as MCheckGroupTrackItem3,\r\n                t3_1.MTrackItem4 as MCheckGroupTrackItem4,\r\n                t3_1.MTrackItem5 as MCheckGroupTrackItem5,\r\n                t4.MFullName AS MAccountName,\r\n                t5.MContactID,\r\n                t5.MEmployeeID,\r\n                t5.MMerItemID,\r\n                t5.MExpItemID,\r\n                t5.MPaItemID,\r\n                t5.MTrackItem1,\r\n                t5.MTrackItem2,\r\n                t5.MTrackItem3,\r\n                t5.MTrackItem4,\r\n                t5.MTrackItem5,\r\n                CONVERT( AES_DECRYPT(t6.MName, '{0}') USING UTF8) AS MContactName,\r\n                F_GETUSERNAME(t7.MFirstName, t7.MLastName) AS MEmployeeName,\r\n                concat(t8_0.MNumber,':',t8.MDesc) AS MMerItemName,\r\n                t9.MName AS MExpItemName,\r\n                t10.MName AS MPaItemName,\r\n                t10_0.MGroupID AS MPaItemGroupID,\r\n                t10_1.MName as MPaItemGroupName,\r\n                t11.MItemID AS MTrackItem1GroupID,\r\n                t12.MName AS MTrackItem1GroupName,\r\n                t13.MName AS MTrackItem1Name,\r\n                t14.MItemID AS MTrackItem2GroupID,\r\n                t15.MName AS MTrackItem2GroupName,\r\n                t16.MName AS MTrackItem2Name,\r\n                t17.MItemID AS MTrackItem3GroupID,\r\n                t18.MName AS MTrackItem3GroupName,\r\n                t19.MName AS MTrackItem3Name,\r\n                t20.MItemID AS MTrackItem4GroupID,\r\n                t21.MName AS MTrackItem4GroupName,\r\n                t22.MName AS MTrackItem4Name,\r\n                t23.MItemID AS MTrackItem5GroupID,\r\n                t24.MName AS MTrackItem5GroupName,\r\n                t25.MName AS MTrackItem5Name,\r\n                F_GETUSERNAME(t27.MFristName, t27.MLastName) AS MCreatorName\r\n            FROM\r\n                t_fc_vouchermoduleentry t1\r\n                    INNER JOIN\r\n                t_fc_vouchermodule t2 ON t1.MID = t2.MItemID\r\n                    AND t2.MOrgID = t1.MOrgID\r\n                    AND t2.MIsDelete = t1.MIsDelete\r\n                    LEFT JOIN\r\n                t_bd_account t3 ON t3.MItemID = t1.MAccountID\r\n                    AND t3.MOrgID = t1.MOrgID\r\n                    AND t3.MIsDelete = t1.MIsDelete\r\n                    LEFT JOIN\r\n                t_bd_account_l t4 ON t4.MParentID = t3.MItemID\r\n                    AND t4.MOrgID = t1.MOrgID\r\n                    AND t4.MIsDelete = t1.MIsDelete\r\n                    AND t4.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_gl_checkgroupvalue t5 ON t5.MItemID = t1.MCheckGroupValueID\r\n                    AND t5.MOrgID = t1.MOrgID\r\n                    AND t5.MIsDelete = t1.MIsDelete\r\n                    LEFT JOIN\r\n                t_bd_bankaccount t3_0 on t3_0.MItemID = t1.MAccountID\r\n                    AND t3_0.MOrgID = t1.MOrgID\r\n                    AND t3_0.MIsDelete = t1.MIsDelete\r\n\t\t\t\t\tLEFT JOIN\r\n\t\t\t\tt_gl_checkgroup t3_1 on t3_1.MItemID = t3.MCheckGroupID\r\n\t\t\t\t\t AND t3_1.MIsDelete = t1.MIsDelete\r\n                    LEFT JOIN\r\n                t_bd_contacts_l t6 ON t6.MParentID = t5.MContactID\r\n                    AND t6.MOrgID = t1.MOrgId\r\n                    AND t6.MIsDelete = t1.MIsDelete\r\n                    AND t6.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_employees_l t7 ON t7.MParentID = t5.MEmployeeID\r\n                    AND t7.MOrgID = t1.MOrgId\r\n                    AND t7.MIsDelete = t1.MIsDelete\r\n                    AND t7.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_item t8_0 ON t8_0.MItemID = t5.MMerItemID\r\n                    AND t8_0.MOrgID = t1.MOrgID\r\n                    AND t8_0.MIsDelete = t1.MIsDelete\r\n                    LEFT JOIN\r\n                t_bd_item_l t8 ON t8.MParentID = t5.MMerItemID\r\n                    AND t8.MOrgID = t1.MOrgId\r\n                    AND t8.MIsDelete = t1.MIsDelete\r\n                    AND t8.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_expenseitem_l t9 ON t9.MParentID = t5.MExpItemID\r\n                    AND t9.MOrgID = t1.MOrgId\r\n                    AND t9.MIsDelete = t1.MIsDelete\r\n                    AND t9.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_pa_payitem_l t10 ON t10.MParentID = t5.MPaItemID\r\n                    AND t10.MOrgID = t1.MOrgId\r\n                    AND t10.MIsDelete = t1.MIsDelete\r\n                    AND t10.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_pa_payitem t10_0 ON t10_0.MItemID = t5.MPaItemID\r\n                    AND t10_0.MOrgID = t1.MOrgId\r\n                    AND t10_0.MIsDelete = t1.MIsDelete\r\n                    LEFT JOIN\r\n                t_pa_payitemgroup_l t10_1 ON t10_1.MParentID = t5.MPaItemID\r\n                    AND t10_1.MOrgID = t1.MOrgId\r\n                    AND t10_1.MIsDelete = t1.MIsDelete\r\n                    AND t10_1.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_trackentry t11 ON t11.MEntryID = t5.MTrackItem1\r\n                    AND t11.MOrgID = t1.MOrgID\r\n                    AND t11.MIsDelete = t1.MIsDelete\r\n                    LEFT JOIN\r\n                t_bd_track_l t12 ON t12.MParentID = t11.MItemID\r\n                    AND t12.MOrgID = t1.MOrgID\r\n                    AND t12.MIsDelete = t1.MIsDelete\r\n                    AND t12.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_trackentry_l t13 ON t13.MParentID = t5.MTrackItem1\r\n                    AND t13.MOrgID = t1.MOrgId\r\n                    AND t13.MIsDelete = t1.MIsDelete\r\n                    AND t13.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_trackentry t14 ON t14.MEntryID = t5.MTrackItem2\r\n                    AND t14.MOrgID = t1.MOrgID\r\n                    AND t14.MIsDelete = t1.MIsDelete\r\n                    LEFT JOIN\r\n                t_bd_track_l t15 ON t15.MParentID = t14.MItemID\r\n                    AND t15.MOrgID = t1.MOrgID\r\n                    AND t15.MIsDelete = t1.MIsDelete\r\n                    AND t15.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_trackentry_l t16 ON t16.MParentID = t5.MTrackItem2\r\n                    AND t16.MOrgID = t1.MOrgId\r\n                    AND t16.MIsDelete = t1.MIsDelete\r\n                    AND t16.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_trackentry t17 ON t17.MEntryID = t5.MTrackItem3\r\n                    AND t17.MOrgID = t1.MOrgID\r\n                    AND t17.MIsDelete = t1.MIsDelete\r\n                    LEFT JOIN\r\n                t_bd_track_l t18 ON t18.MParentID = t17.MItemID\r\n                    AND t18.MOrgID = t1.MOrgID\r\n                    AND t18.MIsDelete = t1.MIsDelete\r\n                    AND t18.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_trackentry_l t19 ON t19.MParentID = t5.MTrackItem3\r\n                    AND t19.MOrgID = t1.MOrgId\r\n                    AND t19.MIsDelete = t1.MIsDelete\r\n                    AND t19.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_trackentry t20 ON t20.MEntryID = t5.MTrackItem4\r\n                    AND t20.MOrgID = t1.MOrgID\r\n                    AND t20.MIsDelete = t1.MIsDelete\r\n                    LEFT JOIN\r\n                t_bd_track_l t21 ON t21.MParentID = t20.MItemID\r\n                    AND t21.MOrgID = t1.MOrgID\r\n                    AND t21.MIsDelete = t1.MIsDelete\r\n                    AND t21.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_trackentry_l t22 ON t22.MParentID = t5.MTrackItem4\r\n                    AND t22.MOrgID = t1.MOrgID\r\n                    AND t22.MIsDelete = t1.MIsDelete\r\n                    AND t22.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_trackentry t23 ON t23.MEntryID = t5.MTrackItem5\r\n                    AND t23.MOrgID = t1.MOrgID\r\n                    AND t23.MIsDelete = t1.MIsDelete\r\n                    LEFT JOIN\r\n                t_bd_track_l t24 ON t24.MParentID = t23.MItemID\r\n                    AND t24.MOrgID = t1.MOrgID\r\n                    AND t24.MIsDelete = t1.MIsDelete\r\n                    AND t24.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_trackentry_l t25 ON t25.MParentID = t5.MTrackItem5\r\n                    AND t25.MOrgID = t1.MOrgId\r\n                    AND t25.MIsDelete = t1.MIsDelete\r\n                    AND t25.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_sec_user_l t27 ON t1.MCreatorID = t27.MParentID\r\n                    AND t27.MIsDelete = t1.MIsDelete\r\n                    AND t27.MLocaleID = @MLocaleID\r\n                WHERE\r\n                    t1.MOrgID = @MOrgID\r\n                        AND t1.MID = @MItemID\r\n                        AND t1.MIsDelete = 0\r\n                        AND t2.MLCID = @MLocaleID\r\n                    order by t1.MEntrySeq asc\r\n\r\n            ";
			format = string.Format(format, "JieNor-001");
			DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(format, ctx.GetParameters("@MItemID", itemID));
			if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return null;
			}
			return BindDataset2VoucherModule(ctx, dataSet.Tables[0]);
		}

		public FCVoucherModuleModel BindDataset2VoucherModule(MContext ctx, DataTable dt)
		{
			List<FCVoucherModuleModel> list = new List<FCVoucherModuleModel>();
			List<FCVoucherModuleEntryModel> list2 = new List<FCVoucherModuleEntryModel>();
			DataRow row = dt.Rows[0];
			FCVoucherModuleModel fCVoucherModuleModel = new FCVoucherModuleModel
			{
				MItemID = row.MField<string>("MID"),
				MOrgID = row.MField<string>("MOrgID"),
				MFastCode = row.MField<string>("MFastCode"),
				MIsMulti = row.MField<bool>("MIsMulti"),
				MLCID = row.MField<string>("MLCID"),
				MDescription = row.MField<string>("MDescription"),
				MCreatorName = row.MField<string>("MCreatorName")
			};
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				row = dt.Rows[i];
				FCVoucherModuleEntryModel fCVoucherModuleEntryModel = new FCVoucherModuleEntryModel
				{
					MEntryID = row.MField<string>("MEntryID"),
					MID = row.MField<string>("MID"),
					MOrgID = row.MField<string>("MOrgID"),
					MExplanation = row.MField<string>("MExplanation"),
					MAccountID = row.MField<string>("MAccountID"),
					MAccountName = row.MField<string>("MAccountName"),
					MCurrencyID = (row.MField<string>("MCurrencyID") ?? ctx.MBasCurrencyID),
					MExchangeRate = row.MField<decimal>("MExchangeRate"),
					MCheckGroupValueID = row.MField<string>("MCheckGroupValueID"),
					MDC = row.MField<int>("MDC"),
					MCredit = row.MField<decimal>("MCredit"),
					MAmount = row.MField<decimal>("MAmount"),
					MAmountFor = row.MField<decimal>("MAmountFor"),
					MDebit = row.MField<decimal>("MDebit"),
					MEntrySeq = row.MField<int>("MEntrySeq")
				};
				BDAccountModel bDAccountModel = string.IsNullOrWhiteSpace(fCVoucherModuleEntryModel.MAccountID) ? new BDAccountModel() : new BDAccountModel
				{
					MItemID = row.MField<string>("MAccountID"),
					MIsCheckForCurrency = row.MField<bool>("MIsCheckForCurrency"),
					MDC = row.MField<int>("MAccountDC"),
					MIsBankAccount = row.MField<bool>("MIsBankAccount"),
					MFullName = row.MField<string>("MAccountName"),
					MNumber = row.MField<string>("MAccountNumber"),
					MCheckGroupID = row.MField<string>("MCheckGroupID")
				};
				GLCheckGroupModel obj = string.IsNullOrWhiteSpace(bDAccountModel.MItemID) ? new GLCheckGroupModel() : new GLCheckGroupModel
				{
					MItemID = row.MField<string>("MCheckGroupID"),
					MContactID = row.MField<int>("MCheckGroupContactID"),
					MEmployeeID = row.MField<int>("MCheckGroupEmployeeID"),
					MMerItemID = row.MField<int>("MCheckGroupMerItemID"),
					MExpItemID = row.MField<int>("MCheckGroupExpItemID"),
					MPaItemID = row.MField<int>("MCheckGroupPaItemID"),
					MTrackItem1 = row.MField<int>("MCheckGroupTrackItem1"),
					MTrackItem2 = row.MField<int>("MCheckGroupTrackItem2"),
					MTrackItem3 = row.MField<int>("MCheckGroupTrackItem3"),
					MTrackItem4 = row.MField<int>("MCheckGroupTrackItem4"),
					MTrackItem5 = row.MField<int>("MCheckGroupTrackItem5")
				};
				GLCheckGroupModel gLCheckGroupModel2 = bDAccountModel.MCheckGroupModel = obj;
				GLCurrencyDataModel obj2 = new GLCurrencyDataModel
				{
					MAmount = fCVoucherModuleEntryModel.MAmount,
					MAmountFor = fCVoucherModuleEntryModel.MAmountFor,
					MCurrencyID = fCVoucherModuleEntryModel.MCurrencyID,
					MExchangeRate = fCVoucherModuleEntryModel.MExchangeRate
				};
				GLCurrencyDataModel gLCurrencyDataModel2 = bDAccountModel.MCurrencyDataModel = obj2;
				GLCheckGroupValueModel gLCheckGroupValueModel = new GLCheckGroupValueModel
				{
					MItemID = row.MField<string>("MCheckGroupValueID"),
					MContactID = row.MField<string>("MContactID"),
					MContactName = row.MField<string>("MContactName"),
					MEmployeeID = row.MField<string>("MEmployeeID"),
					MEmployeeName = row.MField<string>("MEmployeeName"),
					MMerItemID = row.MField<string>("MMerItemID"),
					MMerItemName = row.MField<string>("MMerItemName"),
					MExpItemID = row.MField<string>("MExpItemID"),
					MExpItemName = row.MField<string>("MExpItemName"),
					MPaItemID = row.MField<string>("MPaItemID"),
					MPaItemGroupID = row.MField<string>("MPaItemGroupID"),
					MPaItemGroupName = row.MField<string>("MPaItemGroupName"),
					MPaItemName = row.MField<string>("MPaItemName"),
					MTrackItem1 = row.MField<string>("MTrackItem1"),
					MTrackItem2 = row.MField<string>("MTrackItem2"),
					MTrackItem3 = row.MField<string>("MTrackItem3"),
					MTrackItem4 = row.MField<string>("MTrackItem4"),
					MTrackItem5 = row.MField<string>("MTrackItem5"),
					MTrackItem1GroupID = row.MField<string>("MTrackItem1GroupID"),
					MTrackItem2GroupID = row.MField<string>("MTrackItem2GroupID"),
					MTrackItem3GroupID = row.MField<string>("MTrackItem3GroupID"),
					MTrackItem4GroupID = row.MField<string>("MTrackItem4GroupID"),
					MTrackItem5GroupID = row.MField<string>("MTrackItem5GroupID"),
					MTrackItem1Name = row.MField<string>("MTrackItem1Name"),
					MTrackItem2Name = row.MField<string>("MTrackItem2Name"),
					MTrackItem3Name = row.MField<string>("MTrackItem3Name"),
					MTrackItem4Name = row.MField<string>("MTrackItem4Name"),
					MTrackItem5Name = row.MField<string>("MTrackItem5Name"),
					MTrackItem1GroupName = row.MField<string>("MTrackItem1GroupName"),
					MTrackItem2GroupName = row.MField<string>("MTrackItem2GroupName"),
					MTrackItem3GroupName = row.MField<string>("MTrackItem3GroupName"),
					MTrackItem4GroupName = row.MField<string>("MTrackItem4GroupName"),
					MTrackItem5GroupName = row.MField<string>("MTrackItem5GroupName")
				};
				if (!string.IsNullOrWhiteSpace(gLCheckGroupValueModel.MPaItemGroupName))
				{
					gLCheckGroupValueModel.MPaItemGroupID = gLCheckGroupValueModel.MPaItemID;
					gLCheckGroupValueModel.MPaItemName = gLCheckGroupValueModel.MPaItemGroupName;
				}
				fCVoucherModuleEntryModel.MCheckGroupValueModel = gLCheckGroupValueModel;
				bDAccountModel.MCheckGroupValueModel = gLCheckGroupValueModel;
				fCVoucherModuleEntryModel.MAccountModel = bDAccountModel;
				list2.Add(fCVoucherModuleEntryModel);
			}
			fCVoucherModuleModel.MVoucherModuleEntrys = list2;
			return fCVoucherModuleModel;
		}
	}
}
