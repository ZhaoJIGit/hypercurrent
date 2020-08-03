using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.Enum;
using JieNor.Megi.DataModel.FC;
using JieNor.Megi.DataModel.FP;
using JieNor.Megi.DataRepository.FP;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.DataRepository.FC
{
	public class FCFapiaoModuleRepository : DataServiceT<FCFapiaoModuleModel>
	{
		private readonly GLUtility utility = new GLUtility();

		public OperationResult SaveFapiaoModule(MContext ctx, FCFapiaoModuleModel model)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			list.AddRange(GetHandleFastCodeBaseData(ctx, model));
			model.MLCID = ctx.MLCID;
			ValidateFastCode(ctx, model);
			List<CommandInfo> insertOrUpdateCmd = ModelInfoManager.GetInsertOrUpdateCmd<FCFapiaoModuleModel>(ctx, model, null, true);
			list.AddRange(insertOrUpdateCmd);
			if (!string.IsNullOrWhiteSpace(model.MExplanation))
			{
				List<CommandInfo> insertReferenceCmds = new GLVoucherReferenceRepository().GetInsertReferenceCmds(ctx, new List<string>
				{
					model.MExplanation
				});
				list.AddRange(insertReferenceCmds);
			}
			int num = new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(list);
			return new OperationResult
			{
				Success = !string.IsNullOrWhiteSpace(model.MID),
				ObjectID = model.MID
			};
		}

		public List<CommandInfo> GetHandleFastCodeBaseData(MContext ctx, FCFapiaoModuleModel model)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			FPUtility fPUtility = new FPUtility();
			FPBaseDataModel baseData = fPUtility.GetBaseData(ctx);
			list.AddRange(fPUtility.GetInsertNewItemCmds(ctx, baseData.MMerItem, model));
			list.AddRange(fPUtility.GetInsertNewTrackOptionsCmds(ctx, baseData, model));
			return list;
		}

		public void ValidateFastCode(MContext ctx, FCFapiaoModuleModel fastCode)
		{
			GLUtility gLUtility = new GLUtility();
			ValidateQueryModel validateFapiaoModuleFastCodeSql = gLUtility.GetValidateFapiaoModuleFastCodeSql(new List<FCFapiaoModuleModel>
			{
				fastCode
			});
			List<string> ids = new List<string>
			{
				fastCode.MDebitAccount,
				fastCode.MCreditAccount,
				fastCode.MTaxAccount
			};
			ValidateQueryModel validateCommonModelSql = gLUtility.GetValidateCommonModelSql<BDAccountModel>(MActionResultCodeEnum.MAccountInvalid, ids, null, null);
			ValidateQueryModel validateAccountHasSubSql = gLUtility.GetValidateAccountHasSubSql(ids, false);
			List<string> ids2 = new List<string>
			{
				fastCode.MNewItem ? "" : fastCode.MMerItemID
			};
			ValidateQueryModel validateCommonModelSql2 = gLUtility.GetValidateCommonModelSql<BDItemModel>(MActionResultCodeEnum.MMerItemInvalid, ids2, null, null);
			List<string> ids3 = new List<string>
			{
				fastCode.MNewTrackItem1 ? "" : fastCode.MTrackItem1,
				fastCode.MNewTrackItem2 ? "" : fastCode.MTrackItem2,
				fastCode.MNewTrackItem3 ? "" : fastCode.MTrackItem3,
				fastCode.MNewTrackItem4 ? "" : fastCode.MTrackItem4,
				fastCode.MNewTrackItem5 ? "" : fastCode.MTrackItem5
			};
			ValidateQueryModel validateCommonModelSql3 = gLUtility.GetValidateCommonModelSql<BDTrackEntryModel>(MActionResultCodeEnum.MTrackItemInvalid, ids3, null, null);
			List<MActionResultCodeEnum> list = gLUtility.QueryValidateSql(ctx, true, validateFapiaoModuleFastCodeSql, validateCommonModelSql, validateAccountHasSubSql, validateCommonModelSql2, validateCommonModelSql3);
		}

		public OperationResult DeleteFapiaoModules(MContext ctx, List<string> ids)
		{
			List<CommandInfo> deleteCmd = ModelInfoManager.GetDeleteCmd<FCFapiaoModuleModel>(ctx, ids);
			int num = new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(deleteCmd);
			return new OperationResult
			{
				Success = (num > 0)
			};
		}

		public List<FCFapiaoModuleModel> GetFapiaoModuleModelList(MContext ctx, FCFastCodeFilterModel filter)
		{
			filter.rows = 0;
			return GetFapiaoModulePageList(ctx, filter);
		}

		public List<MySqlParameter> GetParameterList(MContext ctx, FCFastCodeFilterModel filter)
		{
			List<MySqlParameter> list = ctx.GetParameters((MySqlParameter)null).ToList();
			list.Add(new MySqlParameter
			{
				ParameterName = "@Keyword",
				Value = filter.KeyWord,
				MySqlDbType = MySqlDbType.VarChar
			});
			list.Add(new MySqlParameter
			{
				ParameterName = "@MFastCode",
				Value = filter.MFastCode,
				MySqlDbType = MySqlDbType.VarChar
			});
			return list;
		}

		public List<FCFapiaoModuleModel> GetFapiaoModulePageList(MContext ctx, FCFastCodeFilterModel filter)
		{
			List<FCFapiaoModuleModel> list = new List<FCFapiaoModuleModel>();
			List<MySqlParameter> parameterList = GetParameterList(ctx, filter);
			string str = " select t1.MID,\n                                t1.MFastCode,\n                                t1.MDescription,\n                                t1.MExplanation,\n                                t1.MMerItemID,\n                                concat(t2.MNumber, ':', t2_l.MDesc) as MMerItemIDName,\n                                t1.MDebitAccount,\n                                t1.MCreditAccount,\n                                t1.MTaxAccount,\n                                t1.MTrackItem1,\n                                t1.MTrackItem2,\n                                t1.MTrackItem3,\n                                t1.MTrackItem4,\n                                t1.MTrackItem5,\n                                t3.MFullName as MDebitAccountName,\n                                t4.MFullName as MCreditAccountName,\n                                t5.MFullName as MTaxAccountName,\n                                t6.MName as MTrackItem1Name,\n                                t7.MName as MTrackItem2Name,\n                                t8.MName as MTrackItem3Name,\n                                t9.MName as MTrackItem4Name,\n                                t10.MName as MTrackItem5Name " + GetFapiaoModuleSubQuerySql(ctx, filter);
			str += " order by t1.MFastCode desc ";
			str += ((filter.rows > 0) ? (" limit " + (filter.page - 1) * filter.rows + "," + filter.rows) : "");
			return ModelInfoManager.GetDataModelBySql<FCFapiaoModuleModel>(ctx, str, parameterList.ToArray());
		}

		public int GetFapiaoModulePageCount(MContext ctx, FCFastCodeFilterModel filter)
		{
			string sql = "SELECT \n                               count(*) as MCount " + GetFapiaoModuleSubQuerySql(ctx, filter);
			List<MySqlParameter> parameterList = GetParameterList(ctx, filter);
			string s = new DynamicDbHelperMySQL(ctx).GetSingle(sql, parameterList.ToArray()).ToString();
			return int.Parse(s);
		}

		public string GetFapiaoModuleSubQuerySql(MContext ctx, FCFastCodeFilterModel filter)
		{
			string text = " FROM\n                                t_fc_fapiaomodule t1\n                                    LEFT JOIN\n                                t_bd_item t2 on t2.MItemID = t1.MMerItemID\n                                    AND t1.MOrgID = t2.MOrgID\n                                    AND t1.MIsDelete = t2.MIsDelete\n                                    LEFT JOIN\n                                t_bd_item_l t2_l ON t1.MMerItemID = t2_l.MParentID\n                                    AND t2_l.MLocaleID = t1.MLCID\n                                    AND t1.MOrgID = t2_l.MOrgID\n                                    AND t1.MIsDelete = t2_l.MIsDelete\n                                    LEFT JOIN\n                                t_bd_account_l t3 ON t3.MParentID = t1.MDebitAccount\n                                    AND t3.MLocaleID = t1.MLCID\n                                    AND t3.MOrgID = t1.MOrgID\n                                    AND t3.MIsDelete = t1.MIsDelete\n                                    LEFT JOIN\n                                t_bd_account_l t4 ON t4.MParentID = t1.MCreditAccount\n                                    AND t4.MLocaleID = t1.MLCID\n                                    AND t4.MOrgID = t1.MOrgID\n                                    AND t4.MIsDelete = t1.MIsDelete\n                                    LEFT JOIN\n                                t_bd_account_l t5 ON t5.MParentID = t1.MTaxAccount\n                                    AND t5.MLocaleID = t1.MLCID\n                                    AND t5.MOrgID = t1.MOrgID\n                                    AND t5.MIsDelete = t1.MIsDelete\n                                    LEFT JOIN\n                                t_bd_trackentry_l t6 ON t6.MParentID = t1.MTrackItem1\n                                    AND t6.MLocaleID = t1.MLCID\n                                    AND t6.MOrgID = t1.MOrgID\n                                    AND t6.MIsDelete = t1.MIsDelete\n                                    LEFT JOIN\n                                t_bd_trackentry_l t7 ON t7.MParentID = t1.MTrackItem2\n                                    AND t7.MLocaleID = t1.MLCID\n                                    AND t7.MOrgID = t1.MOrgID\n                                    AND t7.MIsDelete = t1.MIsDelete\n                                    LEFT JOIN\n                                t_bd_trackentry_l t8 ON t8.MParentID = t1.MTrackItem3\n                                    AND t8.MLocaleID = t1.MLCID\n                                    AND t8.MOrgID = t1.MOrgID\n                                    AND t8.MIsDelete = t1.MIsDelete\n                                    LEFT JOIN\n                                t_bd_trackentry_l t9 ON t9.MParentID = t1.MTrackItem4\n                                    AND t9.MLocaleID = t1.MLCID\n                                    AND t9.MOrgID = t1.MOrgID\n                                    AND t9.MIsDelete = t1.MIsDelete\n                                    LEFT JOIN\n                                t_bd_trackentry_l t10 ON t10.MParentID = t1.MTrackItem5\n                                    AND t10.MLocaleID = t1.MLCID\n                                    AND t10.MOrgID = t1.MOrgID\n                                    AND t10.MIsDelete = t1.MIsDelete\n                            WHERE\n                                t1.MOrgID = @MOrgID AND t1.MIsDelete = 0\n                                    AND t1.MLCID = @MLocaleID ";
			if (!string.IsNullOrWhiteSpace(filter.MFastCode))
			{
				text += " and t1.MFastCode = @MFastCode";
			}
			if (!string.IsNullOrWhiteSpace(filter.KeyWord))
			{
				text += " AND ( t1.MFastCode LIKE concat('%',@Keyword,'%') \r\n                     OR t1.MDescription LIKE concat('%',@Keyword,'%') \r\n                     OR t1.MEXPLANATION LIKE concat('%',@Keyword,'%')\r\n                     OR t2.MNumber LIKE concat('%',@Keyword,'%')\r\n                     OR t2_l.MDesc LIKE concat('%',@Keyword,'%')\r\n                     OR t3.MFullName LIKE concat('%',@Keyword,'%')\r\n                     OR t4.MFullName LIKE concat('%',@Keyword,'%')\r\n                     OR t5.MFullName LIKE concat('%',@Keyword,'%') \r\n                     OR t6.MName  LIKE concat('%',@Keyword,'%')\r\n                     OR t7.MName  LIKE concat('%',@Keyword,'%') \r\n                     OR t8.MName  LIKE concat('%',@Keyword,'%') \r\n                     OR t9.MName  LIKE concat('%',@Keyword,'%') \r\n                     OR t10.MName  LIKE concat('%',@Keyword,'%') )";
			}
			return text;
		}
	}
}
