using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.FC;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace JieNor.Megi.DataRepository.FC
{
	public class FCCashCodingModuleRepository : DataServiceT<FCCashCodingModuleModel>
	{
		public List<FCCashCodingModuleListModel> GetListByCode(MContext ctx, string code)
		{
			string sql = string.Format("SELECT t1.MID, t1.MCode,t1.MName,t1.MDesc,t1.MRef,t1.MContactID, t1.MAccountID,concat(t3.MNumber,' ', t4.MName) AS MAccountName,t1.MTaxID, concat(t5.MName,'(',ROUND(t5.MTaxRate,2),'%)') AS MTaxName,\n                                t1.MTrackItem1,t1.MTrackItem2,t1.MTrackItem3,t1.MTrackItem4,t1.MTrackItem5 \n                            FROM T_FC_CashCodingModule t1\n                            LEFT JOIN T_BD_Account t3 ON t1.MAccountID = t3.MItemID AND t3.MIsDelete=0\n                            LEFT JOIN (\n\t                            SELECT a.MItemID,b.MName FROM T_BD_Account a\n                                INNER JOIN T_BD_Account_L b ON a.MItemID=b.MParentID AND  b.MLocaleID=@MLocaleID AND b.MIsDelete=0\n                                WHERE a.MOrgID=@MOrgID AND a.MIsDelete=0\n                            ) t4 ON t1.MAccountID=t4.MItemID\n                            LEFT JOIN \n                            (\n\t                            SELECT a.MItemID,b.MName,a.MEffectiveTaxRate AS MTaxRate FROM T_REG_TaxRate a\n                                INNER JOIN T_REG_TaxRate_L b ON a.MItemID=b.MParentID AND  b.MLocaleID=@MLocaleID AND b.MIsDelete=0\n                                WHERE a.MOrgID=@MOrgID AND a.MIsDelete=0\n                            ) t5\n                            ON t1.MTaxID=t5.MItemID\n                            WHERE t1.MOrgID=@MOrgID AND t1.MIsDelete=0 ", "JieNor-001");
			MySqlParameter[] parameters = ctx.GetParameters((MySqlParameter)null);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet ds = dynamicDbHelperMySQL.Query(sql, parameters);
			return ModelInfoManager.DataTableToList<FCCashCodingModuleListModel>(ds);
		}

		public DataGridJson<FCCashCodingModuleListModel> GetCashCodingByPageList(MContext ctx, FCCashCodingModuleListFilter filter)
		{
			SqlQuery sqlQuery = new SqlQuery();
			StringBuilder listSqlBuilder = GetListSqlBuilder(ctx, filter);
			MySqlParameter[] listParameters = GetListParameters(ctx, filter.KeyWord, filter.MCode);
			sqlQuery.SqlWhere = filter;
			if (!string.IsNullOrEmpty(filter.Sort))
			{
				sqlQuery.SqlWhere.OrderBy($" v1.{filter.Sort} {filter.Order}");
			}
			sqlQuery.SelectString = listSqlBuilder.ToString();
			MySqlParameter[] array = listParameters;
			foreach (MySqlParameter para in array)
			{
				sqlQuery.AddParameter(para);
			}
			return ModelInfoManager.GetPageDataModelListBySql<FCCashCodingModuleListModel>(ctx, sqlQuery);
		}

		private static StringBuilder GetListSqlBuilder(MContext ctx, FCCashCodingModuleListFilter filter)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("SELECT * FROM (");
			stringBuilder.AppendLine(" SELECT t1.MID, t1.MCode,t1.MRef,t1.MName,t1.MDesc,t1.MContactID,concat(t3.MNumber,' ', t4.MName) AS MAccountName,CONCAT(t5.MName,'(',ROUND(t5.MTaxRate,2),'%)') AS MTaxName,MTrackItem1,MTrackItem2,MTrackItem3,MTrackItem4,MTrackItem5  ");
			stringBuilder.AppendLine(" FROM (SELECT MID,MCode,MRef,MName,MDesc,MContactID,MAccountID,MTaxID,MTrackItem1,MTrackItem2,MTrackItem3,MTrackItem4,MTrackItem5 FROM T_FC_CashCodingModule WHERE MOrgID=@MOrgID AND MIsDelete=0  ");
			if (!string.IsNullOrWhiteSpace(filter.MCode))
			{
				stringBuilder.AppendLine(" AND MCode LIKE CONCAT('%',@MCode,'%') ");
			}
			if (!string.IsNullOrWhiteSpace(filter.KeyWord))
			{
				stringBuilder.AppendLine(" AND ( MRef LIKE CONCAT('%',@KeyWord,'%') ");
				stringBuilder.AppendLine(" OR  MName LIKE CONCAT('%',@KeyWord,'%')  ");
				stringBuilder.AppendLine(" OR  MDesc LIKE CONCAT('%',@KeyWord,'%') ) ");
			}
			stringBuilder.AppendLine(" ) t1 ");
			stringBuilder.AppendLine(" LEFT JOIN T_BD_Contacts_L t2 ON SUBSTRING(t1.MContactID,1, LOCATE('_', t1.MContactID)-1)=t2.MParentID  AND t2.MLocaleID=@MLocaleID AND t2.MIsDelete=0  ");
			stringBuilder.AppendLine(" LEFT JOIN T_BD_Account t3 ON t1.MAccountID = t3.MItemID AND t3.MIsDelete=0 ");
			stringBuilder.AppendLine(" LEFT JOIN ( ");
			stringBuilder.AppendLine(" SELECT a.MItemID,b.MName FROM T_BD_Account a ");
			stringBuilder.AppendLine(" INNER JOIN T_BD_Account_L b ON a.MItemID=b.MParentID AND  b.MLocaleID=@MLocaleID AND b.MIsDelete=0 ");
			stringBuilder.AppendLine(" WHERE a.MOrgID=@MOrgID AND a.MIsDelete=0 ");
			stringBuilder.AppendLine("  ) t4 ON t1.MAccountID=t4.MItemID ");
			stringBuilder.AppendLine(" LEFT JOIN ( ");
			stringBuilder.AppendLine(" SELECT a.MItemID,b.MName,a.MEffectiveTaxRate AS MTaxRate FROM T_REG_TaxRate a ");
			stringBuilder.AppendLine(" INNER JOIN T_REG_TaxRate_L b ON a.MItemID=b.MParentID AND  b.MLocaleID=@MLocaleID AND b.MIsDelete=0  ");
			stringBuilder.AppendLine(" WHERE a.MOrgID=@MOrgID AND a.MIsDelete=0 ");
			stringBuilder.AppendLine("  ) t5 ");
			stringBuilder.AppendLine(" ON t1.MTaxID=t5.MItemID ");
			stringBuilder.AppendLine(" ) v1 ");
			return stringBuilder;
		}

		private static MySqlParameter[] GetListParameters(MContext ctx, string keyword, string mCode)
		{
			MySqlParameter[] array = new MySqlParameter[4]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 6),
				new MySqlParameter("@KeyWord", MySqlDbType.VarChar, 50),
				new MySqlParameter("@MCode", MySqlDbType.VarChar, 50)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = ctx.MLCID;
			array[2].Value = keyword;
			array[3].Value = mCode;
			return array;
		}

		public FCCashCodingModuleModel GetVoucherModelWithEntry(MContext ctx, string PKID)
		{
			return GetDataModel(ctx, PKID, false);
		}

		public OperationResult UpdateCashCodingModuleModel(MContext ctx, FCCashCodingModuleModel model)
		{
			OperationResult operationResult = new OperationResult();
			operationResult.Success = true;
			FCCashCodingModuleModel cashCodingByFastCode = GetCashCodingByFastCode(ctx, model.MCode);
			if (cashCodingByFastCode != null && (string.IsNullOrWhiteSpace(model.MID) || (!string.IsNullOrWhiteSpace(model.MID) && cashCodingByFastCode.MID != model.MID)))
			{
				string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "VoucherModuleFastcodeBeenUsed", "模板快速码已经被使用!");
				operationResult.Success = false;
				operationResult.ErrorMessageDetail = text;
			}
			if (operationResult.Success)
			{
				List<CommandInfo> insertOrUpdateCmd = ModelInfoManager.GetInsertOrUpdateCmd<FCCashCodingModuleModel>(ctx, model, null, true);
				operationResult.Success = (new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(insertOrUpdateCmd) > 0);
			}
			operationResult.ObjectID = model.MID;
			return operationResult;
		}

		public OperationResult DeleteCashCodingModule(MContext ctx, List<string> mIds)
		{
			return ModelInfoManager.Delete<FCCashCodingModuleModel>(ctx, mIds);
		}

		public FCCashCodingModuleModel GetCashCodingByFastCode(MContext ctx, string fastCode)
		{
			string sql = "SELECT * FROM t_fc_cashcodingmodule t WHERE t.MCode = @MCode AND t.MOrgID = @MOrgID  AND t.MIsDelete = 0 ";
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter
				{
					ParameterName = "@MOrgID",
					Value = ctx.MOrgID
				},
				new MySqlParameter
				{
					ParameterName = "@MCode",
					Value = fastCode
				}
			};
			DataSet ds = new DynamicDbHelperMySQL(ctx).Query(sql, cmdParms);
			return ModelInfoManager.DataTableToList<FCCashCodingModuleModel>(ds).FirstOrDefault();
		}

		public List<FCCashCodingModuleModel> GetCashCodingModuleListWithNoEntry(MContext ctx)
		{
			List<FCCashCodingModuleModel> list = new List<FCCashCodingModuleModel>();
			string sql = " SELECT * FROM T_FC_CashCodingModule WHERE MOrgID=@MOrgID AND MIsDelete=0 ";
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter
				{
					ParameterName = "@MOrgID",
					Value = ctx.MOrgID
				},
				new MySqlParameter
				{
					ParameterName = "@MLCID",
					Value = ctx.MLCID
				}
			};
			DataSet ds = new DynamicDbHelperMySQL(ctx).Query(sql, cmdParms);
			return ModelInfoManager.DataTableToList<FCCashCodingModuleModel>(ds);
		}

		public static List<CommandInfo> UpdateAccountId(MContext ctx, string oldAccountId, string newAccountId)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			MySqlParameter[] parameters = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
				{
					Value = ctx.MOrgID
				},
				new MySqlParameter("@OldAccountId", MySqlDbType.VarChar, 36)
				{
					Value = oldAccountId
				},
				new MySqlParameter("@NewAccountId", MySqlDbType.VarChar, 36)
				{
					Value = newAccountId
				}
			};
			CommandInfo commandInfo = new CommandInfo();
			commandInfo.CommandText = "update T_FC_CashCodingModule set MAccountID=@NewAccountId where MAccountID=@OldAccountId and MOrgID=@MOrgID and MIsDelete=0 ";
			DbParameter[] array = commandInfo.Parameters = parameters;
			list.Add(commandInfo);
			return list;
		}
	}
}
