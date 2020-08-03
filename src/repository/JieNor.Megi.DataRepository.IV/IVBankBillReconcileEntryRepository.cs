using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace JieNor.Megi.DataRepository.IV
{
	public static class IVBankBillReconcileEntryRepository
	{
		public static List<IVBankBillReconcileEntryModel> GetIVBankBillReconcileEntryModelList(MContext ctx, string billType, string billId)
		{
			string sql = string.Format("\n             SELECT bbre.MEntryID,bbre.MID,bbre.MTargetBillType,bbre.MTargetBillID,bbre.MSpentAmtFor,bbre.MReceiveAmtFor,bbre.MCreatorID,t1.MBizDate AS MCreateDate, t1.MReference AS MDesc FROM T_IV_BankBillReconcileEntry bbre \n             INNER JOIN T_IV_Payment t1 ON bbre.MTargetBillID=t1.MID AND bbre.MTargetBillType='Payment' AND t1.MIsDelete=0 and bbre.MOrgID = t1.MOrgID\n             where bbre.MOrgID=@MOrgID AND bbre.MID in (select MID from T_IV_BankBillReconcileEntry where MTargetBillID = @MTargetBillID  AND MIsDelete=0)\n             UNION ALL \n             SELECT bbre.MEntryID,bbre.MID,bbre.MTargetBillType,bbre.MTargetBillID,bbre.MSpentAmtFor,bbre.MReceiveAmtFor,bbre.MCreatorID,t1.MBizDate AS MCreateDate, t1.MReference AS MDesc FROM T_IV_BankBillReconcileEntry bbre \n             INNER JOIN  T_IV_Receive t1 ON bbre.MTargetBillID=t1.MID AND bbre.MTargetBillType='Receive'  AND t1.MIsDelete=0  and bbre.MOrgID = t1.MOrgID\n             where bbre.MOrgID=@MOrgID AND bbre.MID in (select MID from T_IV_BankBillReconcileEntry where MTargetBillID = @MTargetBillID AND MIsDelete=0) \n            UNION ALL \n             SELECT bbre.MEntryID,bbre.MID,bbre.MTargetBillType,bbre.MTargetBillID,bbre.MSpentAmtFor,bbre.MReceiveAmtFor,bbre.MCreatorID,t1.MBizDate AS MCreateDate, t1.MReference AS MDesc FROM T_IV_BankBillReconcileEntry bbre \n             INNER JOIN  T_IV_Transfer t1 ON bbre.MTargetBillID=t1.MID AND bbre.MTargetBillType='Transfer' AND t1.MIsDelete=0  and bbre.MOrgID = t1.MOrgID\n            where bbre.MOrgID=@MOrgID and bbre.MIsDelete = 0 AND bbre.MID in (select MID from T_IV_BankBillReconcileEntry where MTargetBillID = @MTargetBillID AND MIsDelete=0)", "JieNor-001");
			MySqlParameter[] cmdParms = new MySqlParameter[3]
			{
				new MySqlParameter("@MLocaleID", ctx.MLCID),
				new MySqlParameter("@MTargetBillID", billId),
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			return ModelInfoManager.GetDataModelBySql<IVBankBillReconcileEntryModel>(ctx, sql, cmdParms);
		}

		public static bool CheckIsExistsBankBillReconcile(MContext ctx, string billType, string billId)
		{
			string strSql = "select count(1) from T_IV_BankBillReconcileEntry where MOrgID=@MOrgID AND MTargetBillID=@MTargetBillID AND MIsDelete=0 ";
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@MTargetBillID", billId),
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return dynamicDbHelperMySQL.Exists(strSql, cmdParms);
		}

		private static string GetDeleteBankBillRecIds(MContext ctx, string billId)
		{
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MTargetBillID", billId)
			};
			string sql = "SELECT * from T_IV_BankBillReconcileEntry where MTargetBillID = @MTargetBillID AND MOrgID=@MOrgID AND MIsDelete=0 ";
			List<IVBankBillReconcileEntryModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<IVBankBillReconcileEntryModel>(ctx, sql, cmdParms);
			if (dataModelBySql == null || dataModelBySql.Count == 0)
			{
				return "";
			}
			List<string> list = (from t in dataModelBySql
			select t.MID).Distinct().ToList();
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string item in list)
			{
				stringBuilder.AppendFormat("'{0}',", item);
			}
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
			return stringBuilder.ToString();
		}

		private static List<IVBankBillReconcileEntryModel> GetDeleteBankBill(MContext ctx, string billId)
		{
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MTargetBillID", billId)
			};
			string sql = "SELECT * from T_IV_BankBillReconcileEntry where mid IN(\r\n                    SELECT mid from T_IV_BankBillReconcileEntry where MTargetBillID = @MTargetBillID AND MOrgID=@MOrgID AND MIsDelete=0 \r\n                    ) AND MOrgID=@MOrgID AND MIsDelete=0";
			return ModelInfoManager.GetDataModelBySql<IVBankBillReconcileEntryModel>(ctx, sql, cmdParms);
		}

		public static OperationResult DeleteBankBillReconcile(MContext ctx, string billType, string billId)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			MySqlParameter[] parameters = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MTargetBillID", billId)
			};
			string deleteBankBillRecIds = GetDeleteBankBillRecIds(ctx, billId);
			if (string.IsNullOrEmpty(deleteBankBillRecIds))
			{
				return new OperationResult
				{
					Success = false
				};
			}
			CommandInfo commandInfo = new CommandInfo();
			commandInfo.CommandText = $"UPDATE T_IV_BankBillReconcile SET MIsDelete=1 where MIsDelete = 0 and  MOrgID=@MOrgID AND MID IN ({deleteBankBillRecIds})";
			DbParameter[] array = commandInfo.Parameters = parameters;
			list.Add(commandInfo);
			CommandInfo commandInfo2 = new CommandInfo();
			commandInfo2.CommandText = $"UPDATE T_IV_BankBillReconcileEntry SET MIsDelete=1  where MIsDelete = 0 and  MOrgID=@MOrgID AND  MID IN ({deleteBankBillRecIds})";
			array = (commandInfo2.Parameters = parameters);
			list.Add(commandInfo2);
			MySqlParameter[] source = new MySqlParameter[1]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			List<IVBankBillReconcileEntryModel> deleteBankBill = GetDeleteBankBill(ctx, billId);
			if (deleteBankBill == null || !deleteBankBill.Any())
			{
				return new OperationResult
				{
					Success = false
				};
			}
			var enumerable = from a in deleteBankBill
			group a by new
			{
				a.MTargetBillID,
				a.MTargetBillType
			};
			DateTime mDate = deleteBankBill.FirstOrDefault().MDate;
			foreach (var item in enumerable)
			{
				CommandInfo commandInfo3 = new CommandInfo();
				string mTargetBillType = item.Key.MTargetBillType;
				if (mTargetBillType.ToLower() == "payment")
				{
					commandInfo3.CommandText = "UPDATE t_iv_payment pay \n                                    LEFT JOIN(\n                                    SELECT MTargetBillID,SUM(MSpentAmtFor) AS MSpentAmtFor \n                                    FROM t_iv_bankbillreconcileentry\n                                    WHERE MOrgID = @MOrgID AND MIsdelete = 0 AND MTargetBillID=@MID AND MTargetBillType=@MTargetBillType \n                                    GROUP BY MTargetBillID)t \n                                    ON pay.mid = t.MTargetBillID \n                                    SET pay.MReconcileAmtFor =IFNULL(t.MSpentAmtFor,0),pay.MReconcileAmt= ROUND(IFNULL(t.MSpentAmtFor,0) * pay.MEXCHANGERATE,2),\n                                    pay.MReconcileStatu =(CASE WHEN t.MSpentAmtFor > 0 THEN @MReconcileStatuPartly ELSE @MReconcileStatuNone END) \n                                    WHERE pay.MOrgID = @MOrgID AND pay.misdelete = 0 AND pay.mid = @MID";
					List<MySqlParameter> list2 = source.ToList();
					list2.Add(new MySqlParameter("@MID", item.Key.MTargetBillID));
					list2.Add(new MySqlParameter("@MTargetBillType", mTargetBillType));
					list2.Add(new MySqlParameter("@MReconcileStatuNone", Convert.ToInt32(IVReconcileStatus.None)));
					list2.Add(new MySqlParameter("@MReconcileStatuPartly", Convert.ToInt32(IVReconcileStatus.Partly)));
					array = (commandInfo3.Parameters = list2.ToArray());
					list.Add(commandInfo3);
				}
				else if (mTargetBillType.ToLower() == "receive")
				{
					commandInfo3.CommandText = "UPDATE t_iv_receive receive\n                                     LEFT JOIN(\n                                     SELECT MTargetBillID,SUM(MReceiveAmtFor) AS MReceiveAmtFor\r\n                                     FROM t_iv_bankbillreconcileentry  \r\n                                     WHERE MOrgID = @MOrgID AND misdelete = 0 AND MTargetBillID =@MID AND MTargetBillType=@MTargetBillType \n                                     GROUP BY MTargetBillID)t \n                                     ON receive.mid = t.MTargetBillID \n                                     SET receive.MReconcileAmtFor =IFNULL(t.MReceiveAmtFor,0),receive.MReconcileAmt=ROUND(IFNULL(t.MReceiveAmtFor,0) * receive.MEXCHANGERATE,2),\n                                     receive.MReconcileStatu =(CASE WHEN t.MReceiveAmtFor > 0 THEN @MReconcileStatuPartly ELSE @MReconcileStatuNone END) \n                                     WHERE receive.MOrgID = @MOrgID AND receive.misdelete = 0 AND receive.mid = @MID";
					List<MySqlParameter> list3 = source.ToList();
					list3.Add(new MySqlParameter("@MID", item.Key.MTargetBillID));
					list3.Add(new MySqlParameter("@MTargetBillType", mTargetBillType));
					list3.Add(new MySqlParameter("@MReconcileStatuNone", Convert.ToInt32(IVReconcileStatus.None)));
					list3.Add(new MySqlParameter("@MReconcileStatuPartly", Convert.ToInt32(IVReconcileStatus.Partly)));
					array = (commandInfo3.Parameters = list3.ToArray());
					list.Add(commandInfo3);
				}
				else if (mTargetBillType.ToLower() == "transfer")
				{
					commandInfo3.CommandText = "UPDATE t_iv_transfer transfer \n                                     LEFT JOIN(\n                                     SELECT MTargetBillID,SUM(MReceiveAmtFor) AS MReceiveAmtFor,SUM(MSpentAmtFor) AS MSpentAmtFor\n                                     FROM t_iv_bankbillreconcileentry  \n                                     WHERE MOrgID =@MOrgID AND misdelete = 0 AND MTargetBillID =@MID AND MTargetBillType=@MTargetBillType \n                                     GROUP BY MTargetBillID)t\n                                     ON transfer.mid = t.MTargetBillID \n                                     SET transfer.MFromReconcileAmtFor =IFNULL(t.MSpentAmtFor,0),transfer.MFromReconcileAmt=ROUND(IFNULL(t.MSpentAmtFor,0) * ROUND(transfer.MFromTotalAmt/transfer.MFromTotalAmtFor,6),2),\n\t\t\t\t\t\t\t\t\t transfer.MToReconcileAmtFor =IFNULL(t.MReceiveAmtFor,0),transfer.MToReconcileAmt=ROUND(IFNULL(t.MReceiveAmtFor,0) * ROUND(transfer.MToTotalAmt/transfer.MToTotalAmtFor,6),2),\n                                     transfer.MFromReconcileStatu =(CASE WHEN t.MSpentAmtFor > 0 AND transfer.MFromTotalAmtFor= t.MSpentAmtFor THEN  @MReconcileStatuCompletely WHEN t.MSpentAmtFor > 0 THEN @MReconcileStatuPartly ELSE @MReconcileStatuNone END),\n                                     transfer.MToReconcileStatu =(CASE WHEN t.MReceiveAmtFor > 0 AND transfer.MToTotalAmtFor= t.MReceiveAmtFor THEN @MReconcileStatuCompletely WHEN t.MReceiveAmtFor > 0  THEN @MReconcileStatuPartly ELSE @MReconcileStatuNone END) \n                                     WHERE transfer.MOrgID = @MOrgID AND transfer.misdelete = 0 AND transfer.mid =@MID";
					List<MySqlParameter> list4 = source.ToList();
					list4.Add(new MySqlParameter("@MID", item.Key.MTargetBillID));
					list4.Add(new MySqlParameter("@MTargetBillType", mTargetBillType));
					list4.Add(new MySqlParameter("@MReconcileStatuNone", Convert.ToInt32(IVReconcileStatus.None)));
					list4.Add(new MySqlParameter("@MReconcileStatuPartly", Convert.ToInt32(IVReconcileStatus.Partly)));
					list4.Add(new MySqlParameter("@MReconcileStatuCompletely", Convert.ToInt32(IVReconcileStatus.Completely)));
					array = (commandInfo3.Parameters = list4.ToArray());
					list.Add(commandInfo3);
				}
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			dynamicDbHelperMySQL.ExecuteSqlTranWithIndentity(list);
			return new OperationResult
			{
				Success = true
			};
		}
	}
}
