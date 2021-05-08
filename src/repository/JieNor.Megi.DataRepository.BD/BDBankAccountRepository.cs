using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.MultiLanguage;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataRepository.API;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.IV;
using JieNor.Megi.DataRepository.REG;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
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
	public class BDBankAccountRepository : DataServiceT<BDBankAccountEditModel>
	{
		private string multLangFieldSql = "\r\n            ,t5.MName{0} as MBankType_MName{0}\r\n            ,t2.MName{0} ";

		private readonly string commonSql = "SELECT \n                        t1.MItemID,\n                        t1.MItemID AS MBankID,\n                        t1.MBankAccountType,\n                        convert(AES_DECRYPT(t1.MBankNo,'JieNor-001') using utf8) as MBankNo,\n                        t1.MCyID,    \n                        t1.MIsShowInHome,\n                        (case when {0} then 0 else convert(ifnull(t1.MIsNeedReconcile,0) using utf8) end) as MIsNeedReconcile,\n                        (case when {0} then convert(ifnull(t1.MIsNeedReconcile,0) using utf8) else 0 end) as MIsNeedImportBankStatement,\n                        t1.MModifyDate,\n                        t1.MIsActive,\n                        (case t1.MBankTypeID when 'Cash' then null else t1.MBankTypeID end) as MBankType_MBankTypeID,                   \n                        t5.MName as MBankType_MName,\n\t\t\t\t        t6.MIsSys as MBankType_MIsSys,\n                        t4.MNumber as MAccountNumber,\n                        t2.MName \n                        #_#lang_field0#_#               \n                    FROM\n                        T_BD_BankAccount t1\n                            JOIN\n                        @_@t_bd_bankaccount_l@_@ t2 ON t2.MParentID = t1.MItemID AND t2.MIsDelete = 0\n                            JOIN\n                        T_BD_Account t4 ON t4.MOrgID = t1.MOrgID AND t4.MItemID = t1.MItemID AND t4.MIsDelete = 0\n                            LEFT JOIN\n                        t_bd_banktype t6 on t6.MItemID = t1.MBankTypeID and t6.MIsDelete = 0 \n                            left JOIN\n                        @_@t_bd_banktype_l@_@ t5 on t5.MParentID = t6.MItemID and t5.MIsDelete = 0\n                   \n                    WHERE\n                        t1.MOrgID = @MOrgID AND t1.MIsDelete = 0";

		public DataGridJson<BDBankAccountEditModel> Get(MContext ctx, GetParam param)
		{
			return new APIDataRepository().Get<BDBankAccountEditModel>(ctx, param, string.Format(commonSql, ctx.MOrgVersionID), multLangFieldSql, false, true, null);
		}

		public List<BDBankAccountEditModel> GetBDBankDashboardData(MContext ctx, DateTime? startDate, DateTime? endDate)
		{
			DateTime dateTime;
			if (!startDate.HasValue || !startDate.Value.IsValidDateTime())
			{
				dateTime = startDate.Value.ToDayFirstSecond();
			}
			else
			{
				DateTime dateTime2 = ctx.DateNow;
				dateTime2 = dateTime2.AddMonths(-1);
				dateTime = dateTime2.AddDays(1.0).ToDayFirstSecond();
			}
			DateTime startDate2 = dateTime;
			DateTime endDate2 = (endDate.HasValue && endDate.Value.IsValidDateTime()) ? ctx.DateNow.ToDayLastSecond() : endDate.Value.ToDayLastSecond();
			List<NameValueModel> cashCodingGroupInfo = IVBankBillEntryRepository.GetCashCodingGroupInfo(ctx, ctx.MBeginDate, endDate2, null);
			List<BDBankAccountEditModel> bankAccountList = GetBankAccountList(ctx, null, false, null);
			List<NameValueModel> statementGroupInfo = IVBankBillEntryRepository.GetStatementGroupInfo(ctx, ctx.MBeginDate, endDate2);
			List<BDBankBalanceModel> bankBalanceList = GetBankBalanceList(ctx, ctx.MBeginDate, endDate2, false, null);
			List<BDBankBalanceModel> bankBalanceList2 = GetBankBalanceList(ctx, ctx.MBeginDate, endDate2, true, null);
			BDBankAccountEditModel bDBankAccountEditModel = new BDBankAccountEditModel();
			List<BDBankAccountEditModel> list = new List<BDBankAccountEditModel>();
			foreach (BDBankAccountEditModel item in bankAccountList)
			{
				if (cashCodingGroupInfo != null && cashCodingGroupInfo.Count > 0)
				{
					item.MReconcileCount = FindNameValueByName(cashCodingGroupInfo, item.MItemID).ToMInt32();
				}
				if (statementGroupInfo != null && statementGroupInfo.Count > 0 && list.Count < 4)
				{
					decimal mBankStatement = FindNameValueByName(statementGroupInfo, item.MItemID).ToMDecimal();
					item.MBankStatement = mBankStatement;
				}
				if (bankBalanceList != null && bankBalanceList.Count > 0 && list.Count < 4)
				{
					List<BDBankBalanceModel> list2 = (from t in bankBalanceList
					where item.MItemID != null && item.MItemID.Equals(t.MBankID)
					select t).ToList();
					if (list2 != null && list2.Count() > 0)
					{
						item.MBankIsUse = true;
						item.MMegiBalance = list2.Sum((BDBankBalanceModel t) => t.MTotalAmtFor);
					}
				}
				if (bankBalanceList2 != null && bankBalanceList2.Count > 0)
				{
					List<BDBankBalanceModel> list3 = (from t in bankBalanceList2
					where item.MItemID != null && item.MItemID.Equals(t.MBankID)
					select t).ToList();
					if (list3 != null && list3.Count() > 0)
					{
						item.MBankIsUse = true;
						BDBankAccountEditModel bDBankAccountEditModel2 = bDBankAccountEditModel;
						bDBankAccountEditModel2.MMegiBalance += list3.Sum((BDBankBalanceModel t) => t.MTotalAmtFor);
					}
				}
				if (item.MIsShowInHome && list.Count < 4)
				{
					list.Add(item);
				}
			}
			bDBankAccountEditModel.MBankChartInfo = GetBankChartModel(ctx, bankBalanceList2, startDate2, endDate2);
			list.Insert(0, bDBankAccountEditModel);
			return list;
		}

		public static List<BDBankAccountEditModel> GetBDBankAccountEditList(MContext ctx, DateTime startDate, DateTime endDate, string[] accountIds, bool useBase = false, bool needSum = false, bool needChart = false)
		{
			List<BDBankAccountEditModel> bankAccountList = GetBankAccountList(ctx, accountIds ?? new string[0], false, null);
			List<NameValueModel> cashCodingGroupInfo = IVBankBillEntryRepository.GetCashCodingGroupInfo(ctx, ctx.MBeginDate, endDate, null);
			List<NameValueModel> statementGroupInfo = IVBankBillEntryRepository.GetStatementGroupInfo(ctx, ctx.MBeginDate, endDate);
			List<BDBankBalanceModel> bankBalanceList = GetBankBalanceList(ctx, ctx.MBeginDate, endDate, useBase, null);
			List<BDBankAccountEditModel> bDBankAccountLastUpdateDate = GetBDBankAccountLastUpdateDate(ctx);
			BDBankAccountEditModel bDBankAccountEditModel = new BDBankAccountEditModel
			{
				MReconcileCount = 0,
				MBankIsUse = false,
				MBankStatement = decimal.Zero,
				MMegiBalance = decimal.Zero
			};
			foreach (BDBankAccountEditModel item in bankAccountList)
			{
				if (bDBankAccountLastUpdateDate != null && bDBankAccountLastUpdateDate.Count > 0)
				{
					item.MLastUpdateDate = bDBankAccountLastUpdateDate.Find((BDBankAccountEditModel x) => x.MItemID.Equals(item.MItemID)).MLastUpdateDate;
				}
				if (cashCodingGroupInfo != null && cashCodingGroupInfo.Count > 0)
				{
					item.MReconcileCount = FindNameValueByName(cashCodingGroupInfo, item.MItemID).ToMInt32();
					bDBankAccountEditModel.MReconcileCount += item.MReconcileCount;
				}
				if (statementGroupInfo != null && statementGroupInfo.Count > 0)
				{
					decimal num = FindNameValueByName(statementGroupInfo, item.MItemID).ToMDecimal();
					item.MBankStatement = num;
					BDBankAccountEditModel bDBankAccountEditModel2 = bDBankAccountEditModel;
					bDBankAccountEditModel2.MBankStatement += num;
				}
				if (bankBalanceList != null && bankBalanceList.Count > 0)
				{
					List<BDBankBalanceModel> list = (from t in bankBalanceList
					where item.MItemID.Equals(t.MBankID)
					select t).ToList();
					if (list != null && list.Count() > 0)
					{
						item.MBankIsUse = true;
						bDBankAccountEditModel.MBankIsUse = true;
						item.MMegiBalance = list.Sum((BDBankBalanceModel t) => t.MTotalAmtFor);
						item.MBankChartInfo = (needChart ? GetBankChartModel(ctx, list, startDate, endDate) : null);
						BDBankAccountEditModel bDBankAccountEditModel3 = bDBankAccountEditModel;
						bDBankAccountEditModel3.MMegiBalance += item.MMegiBalance;
					}
				}
			}
			bDBankAccountEditModel.MBankChartInfo = ((needChart && bankBalanceList != null && bankBalanceList.Count > 0) ? GetBankChartModel(ctx, bankBalanceList, startDate, endDate) : null);
			if (needSum)
			{
				bankAccountList.Insert(0, bDBankAccountEditModel);
			}
			return bankAccountList;
		}

		public OperationResult DeleteBankbill(MContext ctx, string[] mids)
		{
			OperationResult operationResult = new OperationResult();
			operationResult.Success = false;
			if (mids == null || mids.Length == 0)
			{
				operationResult.Message = "";
				return operationResult;
			}
			int num = mids.Length;
			string sqlInWhere = GetSqlInWhere(mids);
			List<string> bankBillIdList = GetReconciledBankbillIDList(ctx, sqlInWhere);
			if (mids.Length == bankBillIdList.Count)
			{
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "TransactionsHaveBeenGenerated", "Transactions have been generated to journal entries and can not be deleted!");
				return operationResult;
			}
			mids = (from t in mids
			where !bankBillIdList.Contains(t)
			select t).ToArray();
			sqlInWhere = GetSqlInWhere(mids);
			if (string.IsNullOrEmpty(sqlInWhere))
			{
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "TransactionsHaveBeenGenerated", "Transactions have been generated to journal entries and can not be deleted!");
				return operationResult;
			}
			List<CommandInfo> list = new List<CommandInfo>();
			string commandText = " update t_iv_bankbillreconcileentry set MIsDelete = 1 where MOrgID = @MOrgID and  mid in\r\n                (select mid from t_iv_bankbillreconcile where MOrgID = @MOrgID and MIsDelete = 0  AND mbankbillentryid in\r\n                (select mentryid from t_iv_bankbillentry where mid in(" + sqlInWhere + ") and MOrgID = @MOrgID and MIsDelete = 0  ))";
			List<CommandInfo> list2 = list;
			CommandInfo obj = new CommandInfo
			{
				CommandText = commandText
			};
			DbParameter[] array = obj.Parameters = ctx.GetParameters((MySqlParameter)null);
			list2.Add(obj);
			string commandText2 = " update t_iv_bankbillreconcile set MIsDelete = 1\r\n                where MOrgID = @MOrgID  and  mbankbillentryid in\r\n                (select mentryid from t_iv_bankbillentry where MOrgID = @MOrgID and MIsDelete = 0  and mid in(" + sqlInWhere + "))";
			List<CommandInfo> list3 = list;
			CommandInfo obj2 = new CommandInfo
			{
				CommandText = commandText2
			};
			array = (obj2.Parameters = ctx.GetParameters((MySqlParameter)null));
			list3.Add(obj2);
			string commandText3 = " update t_iv_bankbillentry set MIsDelete = 1 where MOrgID = @MOrgID  and  mid in(" + sqlInWhere + ")";
			List<CommandInfo> list4 = list;
			CommandInfo obj3 = new CommandInfo
			{
				CommandText = commandText3
			};
			array = (obj3.Parameters = ctx.GetParameters((MySqlParameter)null));
			list4.Add(obj3);
			string commandText4 = " update t_iv_bankbill set MIsDelete = 1 where MOrgID = @MOrgID and  mid in(" + sqlInWhere + ")";
			List<CommandInfo> list5 = list;
			CommandInfo obj4 = new CommandInfo
			{
				CommandText = commandText4
			};
			array = (obj4.Parameters = ctx.GetParameters((MySqlParameter)null));
			list5.Add(obj4);
			string paymentIDsByBankBill = IVBankBillEntryRepository.GetPaymentIDsByBankBill(ctx, sqlInWhere);
			string receiveIDsByBankBill = IVBankBillEntryRepository.GetReceiveIDsByBankBill(ctx, sqlInWhere);
			string transferIDsByBankBill = IVBankBillEntryRepository.GetTransferIDsByBankBill(ctx, sqlInWhere);
			string autoCreatePaymentIDs = IVBankBillEntryRepository.GetAutoCreatePaymentIDs(ctx, sqlInWhere);
			string autoCreateReceiveIDs = IVBankBillEntryRepository.GetAutoCreateReceiveIDs(ctx, sqlInWhere);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			dynamicDbHelperMySQL.ExecuteSqlTran(list);
			IVBankBillEntryRepository.UpdateReconcileAmt(ctx, paymentIDsByBankBill, receiveIDsByBankBill, transferIDsByBankBill, autoCreatePaymentIDs, autoCreateReceiveIDs);
			if (mids.Length != num)
			{
				operationResult.Message = COMMultiLangRepository.GetTextFormat(ctx.MLCID, LangModule.Common, "DeleteResult", "{0} items(s) have been delete successfully!<br/>{0} items(s) have been delete failure!", mids.Length, num - mids.Length);
			}
			else
			{
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangKey.DataDeleteSuccess);
			}
			operationResult.Success = true;
			return operationResult;
		}

		private static List<string> GetReconciledBankbillIDList(MContext ctx, string midstr)
		{
			List<string> list = new List<string>();
			string sql = $"select b.MID from t_iv_bankbillreconcile a\r\n                            INNER JOIN t_iv_bankbillentry b ON a.MBankBillEntryID=b.MEntryID and a.MOrgID = b.MOrgID and b.MIsDelete = 0 \r\n                            WHERE b.MID in ({midstr}) and a.MOrgID = @MOrgID and a.MIsDelete = 0  ";
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(sql, ctx.GetParameters((MySqlParameter)null));
			if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return list;
			}
			foreach (DataRow row in dataSet.Tables[0].Rows)
			{
				list.Add(row[0].ToString());
			}
			return list;
		}

		private string GetSqlInWhere(string[] mids)
		{
			string text = string.Empty;
			for (int i = 0; i < mids.Length; i++)
			{
				text = string.Join(",", text, "'" + mids[i] + "'");
			}
			return text.TrimStart(',');
		}

		public bool IsBankAccountUsed(MContext ctx, string bankId)
		{
			string strSql = "select 1 from t_iv_payment WHERE MOrgID=@MOrgID AND MBankID=@MBankID AND MIsDelete=0\r\n                            union all\r\n                            select 1 from t_iv_receive WHERE MOrgID=@MOrgID AND MBankID=@MBankID AND MIsDelete=0\r\n                            union all\r\n                            select 1 from t_iv_transfer WHERE (MFromAcctID=@MBankID OR MToAcctID=@MBankID) AND MOrgID=@MOrgID AND MIsDelete=0\r\n                            union all\r\n                            select 1 from t_iv_bankbill WHERE MOrgID=@MOrgID AND MBankID=@MBankID AND MIsDelete=0";
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MBankID", bankId)
			};
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return dynamicDbHelperMySQL.Exists(strSql, cmdParms);
		}

		public static List<NameValueModel> GetSimpleBankAccountList(MContext ctx, List<string> orgIdList, string connectionString)
		{
			List<NameValueModel> list = new List<NameValueModel>();
			string sQLString = string.Format("select a.MItemID as MTag, b.MName, c.MName as MValue from (select * from T_BD_BankAccount where MOrgID in ('{0}') AND MIsDelete=0 ) a\r\n                            left join T_BD_BankAccount_L b on a.MItemID=b.MParentID and a.MOrgID = b.MOrgID and b.MIsDelete = 0  and b.MLocaleId='{1}'\r\n                            left join t_bas_organisation c on a.MOrgID = c.MItemID and c.MIsDelete = 0 \r\n                            ORDER BY a.MCreateDate", string.Join("','", orgIdList), ctx.MLCID);
			DataSet dataSet = DbHelperMySQL.Query(connectionString, sQLString);
			if (dataSet.Tables[0].Rows.Count > 0)
			{
				foreach (DataRow row in dataSet.Tables[0].Rows)
				{
					list.Add(new NameValueModel
					{
						MTag = Convert.ToString(row["MTag"]),
						MName = Convert.ToString(row["MName"]),
						MValue = Convert.ToString(row["MValue"])
					});
				}
			}
			return list;
		}

		public static List<BDBankAccountEditModel> GetBankAccountList(MContext ctx, string[] accountIds, bool ignoreLocale = false, GetParam param = null)
		{
			string text = string.Empty;
			if (accountIds != null && accountIds.Length != 0)
			{
				foreach (string text2 in accountIds)
				{
					if (!string.IsNullOrEmpty(text2))
					{
						text = text + "'" + text2 + "',";
					}
				}
			}
			string format = ignoreLocale ? string.Empty : " AND {0}.MLocaleID=@MLocaleID";
			string text3 = (!string.IsNullOrEmpty(text)) ? (" AND  a.MItemID in(" + text.TrimEnd(',') + ") ") : "";
			MySqlParameter[] array = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MModifyDate", MySqlDbType.DateTime)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = ctx.MLCID;
			if (param != null && param.ModifiedSince > DateTime.MinValue)
			{
				text3 += " AND a.MModifyDate > @MModifyDate ";
				array[2].Value = param.ModifiedSince;
			}
			string sql = string.Format("SELECT  b.MPKID,a.MItemID,a.MItemID AS MAccountID ,s.MName as MCurrencyName,a.MCyID,b.MName as MBankName, convert(AES_DECRYPT(a.MBankNo ,'{0}') using utf8)  as MBankNo,a.MOpeningBank, a.MBankTypeID,c.MName as MBankTypeName,a.MBankAccountType, a.MIsShowInHome, a.MModifyDate,a.MIsNeedReconcile\r\n                            FROM T_BD_BankAccount a\r\n                            LEFT JOIN T_BD_BankAccount_L b ON  a.MItemID=b.MParentID and a.MOrgID = b.MOrgID and b.MIsDelete = 0  {1}\r\n\t\t\t\t\t\t\tLEFT JOIN T_BD_BankType_l c on a.MBankTypeID = c.MParentID and c.MIsDelete = 0  {2}\r\n                            INNER JOIN T_bas_currency_l  s  on  s.MParentID=a.MCyID  and s.MIsDelete = 0 \r\n                            WHERE \r\n                                a.MOrgID=@MOrgID  AND  s.MLocaleID='" + ctx.MLCID + "'AND a.MIsDelete= 0  " + text3 + " ORDER BY a.MCreateDate", "JieNor-001", string.Format(format, "b"), string.Format(format, "c"));
			return ModelInfoManager.GetDataModelBySql<BDBankAccountEditModel>(ctx, sql, array);
		}

		private static ChartModel GetBankChartModel(MContext ctx, List<BDBankBalanceModel> bankBalanceList, DateTime startDate, DateTime endDate)
		{
			ChartModel chartModel = new ChartModel();
			int num = (endDate.Year * 12 + endDate.Month - (startDate.Year * 12 + startDate.Month) <= 1) ? 1 : 7;
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			List<object> list3 = new List<object>();
			int num2 = 0;
			int num3 = 0;
			while (true)
			{
				DateTime dtTemp = startDate.AddDays((double)(num3 + 1));
				int num4;
				if (!(dtTemp > endDate))
				{
					int year = dtTemp.Year;
					DateTime dateNow = ctx.DateNow;
					if (year == dateNow.Year)
					{
						int month = dtTemp.Month;
						dateNow = ctx.DateNow;
						if (month == dateNow.Month)
						{
							int day = dtTemp.Day;
							dateNow = ctx.DateNow;
							num4 = ((day == dateNow.Day) ? 1 : 0);
							goto IL_00e0;
						}
					}
					num4 = 0;
					goto IL_00e0;
				}
				break;
				IL_00e0:
				bool flag = (byte)num4 != 0;
				if (dtTemp.DayOfWeek == DayOfWeek.Sunday | flag)
				{
					list2.Add(dtTemp.ToOrgZoneDateString(ctx));
					list3.Add((from t in bankBalanceList
					where t.MBizDate <= dtTemp
					select t).Sum((BDBankBalanceModel t) => t.MTotalAmtFor));
					num2++;
				}
				num3++;
			}
			chartModel.MLabels = list.ToArray();
			chartModel.MValue = list3.ToArray();
			chartModel.MTipLabels = list2.ToArray();
			double maxAmt = list3.Max().ToMDouble();
			double minAmt = list3.Min().ToMDouble();
			chartModel.MScale = ChartHelper.GetChartScaleModel(maxAmt, minAmt);
			return chartModel;
		}

		private static string FindNameValueByName(List<NameValueModel> list, string name)
		{
			NameValueModel nameValueModel = list.Find((NameValueModel x) => x.MName.Equals(name));
			return (nameValueModel == null) ? string.Empty : nameValueModel.MValue;
		}

		public static BDBankChartModel GetBankTotalChartModel(MContext ctx)
		{
			BDBankChartModel bDBankChartModel = new BDBankChartModel();
			List<BDBankBalanceModel> bankInList = GetBankInList(ctx);
			List<BDBankBalanceModel> bankOutList = GetBankOutList(ctx);
			bDBankChartModel.InChart = GetBankTotalChartModel(ctx, bankInList, GetBankInChartDataModel);
			bDBankChartModel.OutChart = GetBankTotalChartModel(ctx, bankOutList, GetBankOutChartDataModel);
			double num = (from t in bDBankChartModel.InChart.Data
			select t.value.ToMDouble()).Max();
			double num2 = (from t in bDBankChartModel.OutChart.Data
			select t.value.ToMDouble()).Min();
			if (num == 0.0)
			{
				num = Math.Abs(num2);
			}
			if (num2 == 0.0)
			{
				num2 = 0.0 - num;
			}
			ChartScaleModel chartScaleModel = ChartHelper.GetChartScaleModel(num, num2, 6);
			bDBankChartModel.InChart.MScale = chartScaleModel;
			bDBankChartModel.OutChart.MScale = chartScaleModel;
			return bDBankChartModel;
		}

		private static ChartModel GetBankTotalChartModel(MContext ctx, List<BDBankBalanceModel> balanceList, Func<MContext, DateTime, List<BDBankBalanceModel>, ChartDataModel> GetChartDataModel)
		{
			ChartModel chartModel = new ChartModel();
			List<ChartDataModel> list = new List<ChartDataModel>();
			DateTime date = ctx.DateNow.Date;
			DateTime dateTime = new DateTime(date.Year, date.Month, 1);
			for (int num = 4; num >= 0; num--)
			{
				DateTime arg = dateTime.AddMonths(-num);
				ChartDataModel item = GetChartDataModel(ctx, arg, balanceList);
				list.Add(item);
			}
			chartModel.Data = list;
			return chartModel;
		}

		private static ChartDataModel GetBankInChartDataModel(MContext ctx, DateTime dtTemp, List<BDBankBalanceModel> balanceList)
		{
			return new ChartDataModel
			{
				name = dtTemp.ToOrgZoneYearMonth(ctx),
				value = GetBankCashTotal(balanceList, dtTemp).To2Decimal(),
				color = "#c52120"
			};
		}

		private static ChartDataModel GetBankOutChartDataModel(MContext ctx, DateTime dtTemp, List<BDBankBalanceModel> balanceList)
		{
			return new ChartDataModel
			{
				name = string.Empty,
				value = GetBankCashTotal(balanceList, dtTemp).To2Decimal(),
				color = "#5f9d01"
			};
		}

		private static decimal GetBankCashTotal(List<BDBankBalanceModel> list, DateTime date)
		{
			if (list == null || list.Count == 0)
			{
				return decimal.Zero;
			}
			return (from t in list
			where t.MBizDate == date
			select t).Sum((BDBankBalanceModel t) => t.MTotalAmtFor);
		}

		private static List<BDBankBalanceModel> GetBankOutList(MContext ctx)
		{
			DateTime bankBalanceEndDate = GetBankBalanceEndDate(ctx);
			string sql = "SELECT MBankID, DATE_FORMAT(MBizDate,'%Y-%m-01') AS MBizDate, (0- MTotalAmt) AS MTotalAmtFor \r\n                            FROM T_IV_Payment  WHERE MOrgID=@MOrgID AND MBizDate<@MBizDate  AND  MIsDelete=0 \r\n                            UNION ALL\r\n                            SELECT MFromAcctID AS MBankID,DATE_FORMAT(MCreateDate,'%Y-%m-01') AS MBizDate,(0- MToTotalAmt) AS MTotalAmtFor\r\n                            FROM T_IV_Transfer   WHERE MOrgID=@MOrgID AND MCreateDate<@MBizDate AND  MIsDelete=0 ";
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MBizDate", bankBalanceEndDate)
			};
			return ModelInfoManager.GetDataModelBySql<BDBankBalanceModel>(ctx, sql, cmdParms);
		}

		private static List<BDBankBalanceModel> GetBankInList(MContext ctx)
		{
			DateTime bankBalanceEndDate = GetBankBalanceEndDate(ctx);
			string sql = "SELECT MBankID,DATE_FORMAT(MBizDate,'%Y-%m-01') AS MBizDate, MTotalAmt AS MTotalAmtFor \r\n                            FROM T_IV_Receive  WHERE MOrgID=@MOrgID AND MBizDate<@MBizDate AND  MIsDelete=0 \r\n                            UNION ALL\r\n                            SELECT MToAcctID AS MBankID,DATE_FORMAT(MCreateDate,'%Y-%m-01') AS MBizDate,MFromTotalAmt AS MTotalAmtFor\r\n                            FROM T_IV_Transfer   WHERE MOrgID=@MOrgID AND MCreateDate<@MBizDate AND  MIsDelete=0 ";
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MBizDate", bankBalanceEndDate)
			};
			return ModelInfoManager.GetDataModelBySql<BDBankBalanceModel>(ctx, sql, cmdParms);
		}

		public static decimal GetBankBalance(MContext ctx, DateTime startDate, DateTime endDate, string bankId)
		{
			if (string.IsNullOrEmpty(bankId))
			{
				return decimal.Zero;
			}
			List<BDBankAccountEditModel> bDBankAccountEditList = GetBDBankAccountEditList(ctx, startDate, endDate, new string[1]
			{
				bankId
			}, false, false, false);
			if (bDBankAccountEditList == null || bDBankAccountEditList.Count == 0)
			{
				return decimal.Zero;
			}
			return bDBankAccountEditList[0].MMegiBalance;
		}

		private static DateTime GetBankBalanceEndDate(MContext ctx)
		{
			DateTime dateTime = ctx.DateNow;
			dateTime = dateTime.Date;
			return dateTime.AddDays(1.0);
		}

		public static List<BDBankBalanceModel> GetBankBalanceList(MContext ctx, DateTime startDate, DateTime endDate, bool useBase = false, string bankId = null)
		{
			string text = "MTaxTotalAmtFor";
			string text2 = "MTaxFromTotalAmtFor";
			string text3 = "MTaxToTotalAmtFor";
			string text4 = "MFromTotalAmtFor";
			string text5 = "MToTotalAmtFor";
			string text6 = "MBeginBalanceFor";
			if (useBase)
			{
				text = "MTaxTotalAmt";
				text2 = "MTaxFromTotalAmt";
				text3 = "MTaxToTotalAmt";
				text4 = "MFromTotalAmt";
				text5 = "MToTotalAmt";
				text6 = "MBeginBalance";
			}
			string text7 = "";
			text7 = ((!string.IsNullOrWhiteSpace(bankId)) ? "SELECT MBankID ,MBizDate, SUM(MTaxTotalAmtFor) AS MTotalAmtFor\r\n                        FROM( {0} UNION ALL {1} UNION ALL {2} UNION ALL {3} UNION ALL {4}) t\r\n                        where MBankID = @BankID\r\n                        GROUP BY MBankID,MBizDate" : "SELECT MBankID ,MBizDate, SUM(MTaxTotalAmtFor) AS MTotalAmtFor\r\n                        FROM( {0} UNION ALL {1} UNION ALL {2} UNION ALL {3} UNION ALL {4}) t\r\n                        GROUP BY MBankID,MBizDate");
			string text8 = "SELECT MBankID, DATE_FORMAT(MBizDate,'%Y-%m-%d') AS MBizDate, (0- {0}) AS MTaxTotalAmtFor \r\n                            FROM T_IV_Payment  WHERE MBankID is not null AND MOrgID=@MOrgID AND MBizDate>=@StartDate AND MBizDate<=@EndDate AND  MIsDelete=0 ";
			string text9 = "SELECT MBankID,DATE_FORMAT(MBizDate,'%Y-%m-%d') AS MBizDate, {0} as MTaxTotalAmtFor \r\n                            FROM T_IV_Receive  WHERE MBankID is not null and MOrgID=@MOrgID AND MBizDate>=@StartDate AND MBizDate<=@EndDate AND  MIsDelete=0 ";
			string text10 = "SELECT MFromAcctID AS MBankID, MBizDate,(0- {3}) AS MTaxTotalAmtFor \r\n                            FROM T_IV_Transfer  WHERE MFromAcctID is not null and MOrgID=@MOrgID AND MBizDate>=@StartDate AND MBizDate<=@EndDate AND  MIsDelete=0 ";
			string text11 = "SELECT MToAcctID AS MBankID, MBizDate,({4}) AS MTaxTotalAmtFor\r\n                            FROM T_IV_Transfer  WHERE MToAcctID is not null and MOrgID=@MOrgID AND MBizDate>=@StartDate AND MBizDate<=@EndDate AND  MIsDelete=0 ";
			string text12 = "SELECT MAccountID AS MBankID, DATE_FORMAT(MDate,'%Y-%m-%d') AS MBizDate ,{5} AS MTaxTotalAmtFor \r\n                            FROM t_gl_initbankbalance WHERE MOrgID=@MOrgID and MAccountID is not null  AND  MIsDelete=0 ";
			text7 = string.Format(string.Format(text7, text8, text9, text10, text11, text12), text, text2, text3, text4, text5, text6);
			MySqlParameter[] cmdParms = new MySqlParameter[4]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@StartDate", startDate),
				new MySqlParameter("@EndDate", endDate),
				new MySqlParameter("@BankID", bankId)
			};
			return ModelInfoManager.GetDataModelBySql<BDBankBalanceModel>(ctx, text7, cmdParms);
		}

		public static BDBankBalanceModel GetBankBalanceModel(MContext ctx, DateTime startDate, DateTime endDate, string bankId)
		{
			string format = "SELECT MBankID ,MBizDate, SUM(MTaxTotalAmtFor) AS MTotalAmtFor , SUM(MTaxTotalAmt) AS MTotalAmt \r\n                        FROM( {0} UNION ALL {1} UNION ALL {2} UNION ALL {3} UNION ALL {4}) t\r\n                        where MBankID = @BankID\r\n                        GROUP BY MBankID,MBizDate";
			string text = "SELECT MBankID, DATE_FORMAT(MBizDate,'%Y-%m-%d') AS MBizDate, (0- MTaxTotalAmtFor) AS MTaxTotalAmtFor , (0-MTaxTotalAmt) as  MTaxTotalAmt\r\n                            FROM T_IV_Payment  WHERE MBankID is not null AND MOrgID=@MOrgID AND MBizDate>=@StartDate AND MBizDate<=@EndDate AND  MIsDelete=0 ";
			string text2 = "SELECT MBankID,DATE_FORMAT(MBizDate,'%Y-%m-%d') AS MBizDate, MTaxTotalAmtFor , MTaxTotalAmt\r\n                            FROM T_IV_Receive  WHERE MBankID is not null and MOrgID=@MOrgID AND MBizDate>=@StartDate AND MBizDate<=@EndDate AND  MIsDelete=0 ";
			string text3 = "SELECT MFromAcctID AS MBankID, MBizDate,(0- MFromTotalAmtFor) AS MTaxTotalAmtFor ,(0-MFromTotalAmt) as MTaxTotalAmt\r\n                            FROM T_IV_Transfer  WHERE MFromAcctID is not null and MOrgID=@MOrgID AND MBizDate>=@StartDate AND MBizDate<=@EndDate AND  MIsDelete=0 ";
			string text4 = "SELECT MToAcctID AS MBankID, MBizDate, MToTotalAmtFor AS MTaxTotalAmtFor , MToTotalAmt as MTaxTotalAmt\r\n                            FROM T_IV_Transfer  WHERE MToAcctID is not null and MOrgID=@MOrgID AND MBizDate>=@StartDate AND MBizDate<=@EndDate AND  MIsDelete=0 ";
			string text5 = "SELECT MAccountID AS MBankID, DATE_FORMAT(MDate,'%Y-%m-%d') AS MBizDate ,MBeginBalanceFor AS MTaxTotalAmtFor ,MBeginBalance as MTaxTotalAmt\r\n                            FROM t_gl_initbankbalance WHERE MOrgID=@MOrgID and MAccountID is not null  AND  MIsDelete=0 ";
			format = string.Format(format, text, text2, text3, text4, text5);
			MySqlParameter[] cmdParms = new MySqlParameter[4]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@StartDate", startDate),
				new MySqlParameter("@EndDate", endDate),
				new MySqlParameter("@BankID", bankId)
			};
			List<BDBankBalanceModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<BDBankBalanceModel>(ctx, format, cmdParms);
			if (dataModelBySql == null || dataModelBySql.Count() == 0)
			{
				return null;
			}
			return dataModelBySql.First();
		}

		public static List<BDBankBalanceModel> GetBankInitBalanceList(MContext ctx, DateTime startDate, DateTime endDate, string bankId = null)
		{
			string text = "MTaxTotalAmtFor";
			string text2 = "MTaxFromTotalAmtFor";
			string text3 = "MTaxToTotalAmtFor";
			string text4 = "MFromTotalAmtFor";
			string text5 = "MToTotalAmtFor";
			string format = "SELECT MBankID ,MBizDate, SUM(MTaxTotalAmtFor) AS MTotalAmtFor , SUM(MTaxTotalAmt) AS MTotalAmt \r\n                        FROM( {0} UNION ALL {1} UNION ALL {2} UNION ALL {3} UNION ALL {4}) t\r\n                        GROUP BY MBankID";
			string format2 = " {0} is not null ";
			if (!string.IsNullOrWhiteSpace(bankId))
			{
				format2 = "{0}='{1}'";
			}
			string text6 = "SELECT MBankID, DATE_FORMAT(MBizDate,'%Y-%m-%d') AS MBizDate, (0- {0}) AS MTaxTotalAmtFor ,(0-MTaxTotalAmt) as MTaxTotalAmt \r\n                            FROM T_IV_Payment  WHERE " + string.Format(format2, "MBankID", bankId) + " AND MOrgID=@MOrgID AND MBizDate>=@StartDate AND MBizDate<=@EndDate AND  MIsDelete=0 ";
			string text7 = "SELECT MBankID,DATE_FORMAT(MBizDate,'%Y-%m-%d') AS MBizDate, {0} as MTaxTotalAmtFor , MTaxTotalAmt as MTaxTotalAmt\r\n                            FROM T_IV_Receive  WHERE " + string.Format(format2, "MBankID", bankId) + " and MOrgID=@MOrgID AND MBizDate>=@StartDate AND MBizDate<=@EndDate AND  MIsDelete=0 ";
			string text8 = "SELECT MFromAcctID AS MBankID, MBizDate,(0- {3}) AS MTaxTotalAmtFor , (0-MToTotalAmt) as MTaxTotalAmt \r\n                            FROM T_IV_Transfer  WHERE " + string.Format(format2, "MFromAcctID", bankId) + "  and MOrgID=@MOrgID AND MBizDate>=@StartDate AND MBizDate<=@EndDate AND  MIsDelete=0";
			string text9 = "SELECT MToAcctID AS MBankID, MBizDate,({4}) AS MTaxTotalAmtFor, MFromTotalAmt AS MTaxTotalAmt\r\n                            FROM T_IV_Transfer  WHERE " + string.Format(format2, "MToAcctID", bankId) + " and MOrgID=@MOrgID AND MBizDate>=@StartDate AND MBizDate<=@EndDate AND  MIsDelete=0 ";
			string text10 = "SELECT MAccountID AS MBankID, DATE_FORMAT(MDate,'%Y-%m-%d') AS MBizDate ,MBeginBalanceFor AS MTaxTotalAmtFor , MBeginBalance as MTaxTotalAmt\r\n                            FROM t_gl_initbankbalance WHERE MOrgID=@MOrgID  AND  MIsDelete=0  and " + string.Format(format2, "MAccountID", bankId);
			format = string.Format(string.Format(format, text6, text7, text8, text9, text10), text, text2, text3, text4, text5);
			MySqlParameter[] cmdParms = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@StartDate", startDate),
				new MySqlParameter("@EndDate", endDate)
			};
			return ModelInfoManager.GetDataModelBySql<BDBankBalanceModel>(ctx, format, cmdParms);
		}

		private static List<BDBankAccountEditModel> GetBDBankAccountLastUpdateDate(MContext ctx)
		{
			string format = " select MItemID , max(MLastUpdateDate) as MLastUpdateDate from ({0} UNION ALL {1} Union ALL {2} Union ALL {3} Union ALL {4} Union ALL {5}) a group by MItemID ";
			string text = "select MItemID, MModifyDate as MLastUpdateDate from T_BD_Bankaccount where MOrgID=@MOrgID AND  MIsDelete=0 and MItemID is not null ";
			string text2 = "select MBankID as MItemID, MModifyDate as MLastUpdateDate from T_IV_Payment where MOrgID=@MOrgID AND  MIsDelete=0  and MBankID is not null ";
			string text3 = "select MBankID as MItemID, MModifyDate as MLastUpdateDate from T_IV_Receive where MOrgID=@MOrgID AND  MIsDelete=0 and MBankID is not null  ";
			string text4 = "select MFromAcctID as MItemID,   MModifyDate as MLastUpdateDate from T_IV_Transfer where MOrgID=@MOrgID AND  MIsDelete=0  and MFromAcctID is not null ";
			string text5 = "select MToAcctID as MItemID,   MModifyDate as MLastUpdateDate from T_IV_Transfer where MOrgID=@MOrgID AND  MIsDelete=0  and MToAcctID is not null";
			string text6 = "select MBankID as MItemID, MModifyDate as MLastUpdateDate from T_IV_Bankbill where MOrgID=@MOrgID AND  MIsDelete=0 and MBankID is not null ";
			format = string.Format(format, text, text2, text3, text4, text5, text6);
			return ModelInfoManager.GetDataModelBySql<BDBankAccountEditModel>(ctx, format, ctx.GetParameters((MySqlParameter)null));
		}

		public BDBankAccountEditModel GetBDBankAccountEditModel(MContext ctx, string pkID)
		{
			BDBankAccountEditModel bDBankAccountEditModel = ModelInfoManager.GetDataEditModel<BDBankAccountEditModel>(ctx, pkID, false, true);
			if (bDBankAccountEditModel == null)
			{
				bDBankAccountEditModel = new BDBankAccountEditModel();
			}
			else
			{
				BDAccountModel dataModel = new BDAccountRepository().GetDataModel(ctx, bDBankAccountEditModel.MAccountID, false);
				if (dataModel != null && !string.IsNullOrEmpty(dataModel.MNumber))
				{
					string[] array = dataModel.MNumber.Split('.');
					bDBankAccountEditModel.MNumber = ((array.Length > 1) ? array[array.Length - 1] : dataModel.MNumber);
				}
			}
			bDBankAccountEditModel.MBankName = bDBankAccountEditModel.GetMultiLanguageValue(ctx, "MName");
			return bDBankAccountEditModel;
		}

		public List<BDBankAccountListModel> GetBDBankAccountList(MContext ctx)
		{
			string strWhere = "  a.MIsBank = 1 and a.MIsDelete = 0 ";
			List<BDBankAccountListModel> bDBankAccountList = GetBDBankAccountList(ctx, strWhere);
			List<BDBankBalanceModel> balanceList = GetBankBalanceList(ctx, DateTime.Now.AddYears(-15), DateTime.Now, false, null);
			bDBankAccountList.ForEach(delegate(BDBankAccountListModel x)
			{
				x.MEndBalance = (from y in balanceList
				where y.MBankID == x.MItemID
				select y).Sum((BDBankBalanceModel t) => t.MTotalAmtFor);
			});
			return bDBankAccountList;
		}

		public List<BDBankAccountListModel> GetBDBankAccountList(MContext ctx, string strWhere)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("SELECT  f.MItemID,f.MAccountID,a.MNumber,b.MName,b.MDesc,MIsCheckForCurrency,a.MAccountGroupID, a.MIsSys,a.MParentID ,a.MDC,");
			stringBuilder.AppendLine("  c.MName as MAcctGroupName,a.MAccountTypeID,d.MName as MAcctTypeName,a.MTaxTypeID,e.MName as MTaxTypeName,a.MCyID as MCurrencyID,");
			stringBuilder.AppendLine("  g.MYDTDebitFor,g.MYDTDebit,g.MYDTCreditFor,g.MYDTCredit,0.00 MYTDFor,0.00 MYTD,g.MBeginBalanceFor,g.MBeginBalance,g.MEndBalanceFor,g.MEndBalance,f.MBankTypeID");
			stringBuilder.AppendLine(" FROM T_BD_Account  a");
			stringBuilder.AppendLine(" Left Join T_BD_Account_l b ON a.MItemID=b.MParentID  AND b.MLocaleID=@MLocaleID and b.MOrgID = a.MOrgID and b.MIsDelete = 0 ");
			stringBuilder.AppendLine(" Left Join T_BD_AcctGroup_l c ON a.MAccountGroupID=c.MParentID And c.MLocaleID=@MLocaleID and c.MOrgID = a.MOrgID and c.MIsDelete = 0  ");
			stringBuilder.AppendLine(" Left Join T_BD_AcctType_l d ON a.MAccountTypeID=d.MParentID And d.MLocaleID=@MLocaleID and d.MOrgID = a.MOrgID and d.MIsDelete = 0  ");
			stringBuilder.AppendLine(" Left Join T_REG_TaxRate_l e ON a.MTaxTypeID=e.MParentID And e.MLocaleID=@MLocaleID and e.MOrgID = a.MOrgIDand e.MIsDelete = 0  ");
			stringBuilder.AppendLine(" Left Join T_BD_BankAccount f ON a.MBankNo=f.MBankNo and f.MOrgID = @MOrgID and f.MIsDelete = 0  ");
			stringBuilder.AppendLine(" Left Join T_Gl_Balance g ON a.MItemID=g.MAccountID and g.MOrgID = @MOrgID and g.MIsDelete = 0  ");
			stringBuilder.AppendLine(" WHERE a.MIsDelete = 0 And a.MOrgID=@MOrgID a.IsActive = 1 ");
			if (strWhere.Trim() != "")
			{
				stringBuilder.AppendLine(" AND " + strWhere);
			}
			stringBuilder.AppendLine(" order by MNumber ");
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return ModelInfoManager.DataTableToList<BDBankAccountListModel>(dynamicDbHelperMySQL.Query(stringBuilder.ToString(), ctx.GetParameters((MySqlParameter)null)).Tables[0]);
		}

		public int UpdateBankAccountByTran(MContext ctx, List<CommandInfo> cmdInfos)
		{
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return dynamicDbHelperMySQL.ExecuteSqlTran(cmdInfos);
		}

		public OperationResult InsertDefaultCashAccount(MContext ctx)
		{
			OperationResult operationResult = new OperationResult();
			if (ModelInfoManager.ExistsByFilter<BDBankAccountEditModel>(ctx, new SqlWhere().Equal("MBankAccountType", 3)))
			{
				operationResult.Success = true;
				return operationResult;
			}
			BDBankAccountEditModel bDBankAccountEditModel = GetBDBankAccountEditModel(ctx, "");
			bDBankAccountEditModel.MOrgID = ctx.MOrgID;
			string key = "Cash";
			bDBankAccountEditModel.MBankAccountType = 3;
			bDBankAccountEditModel.IsNew = true;
			bDBankAccountEditModel.MCyID = new REGCurrencyRepository().GetBase(ctx, false, null, null).MCurrencyID;
			bDBankAccountEditModel.MAccountTypeID = "105";
			List<MultiLanguageFieldList> list = new List<MultiLanguageFieldList>();
			MultiLanguageFieldList multiLanguageFieldList = new MultiLanguageFieldList
			{
				MFieldName = "MName"
			};
			for (int i = 0; i < COMMultiLangRepository.MegiLangTypes.Length; i++)
			{
				string text = COMMultiLangRepository.MegiLangTypes[i];
				string text2 = COMMultiLangRepository.GetText(text, LangModule.Bank, key, "Cash");
				MultiLanguageField item = new MultiLanguageField
				{
					MLocaleID = text,
					MValue = text2
				};
				multiLanguageFieldList.MMultiLanguageField.Add(item);
				if (text == ctx.MDefaultLocaleID)
				{
					multiLanguageFieldList.MMultiLanguageValue = text2;
				}
			}
			list.Add(multiLanguageFieldList);
			bDBankAccountEditModel.MultiLanguage = list;
			bDBankAccountEditModel.MIsShowInHome = (ctx.MOrgVersionID == 0);
			List<CommandInfo> list2 = new List<CommandInfo>();
			list2.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDBankAccountEditModel>(ctx, bDBankAccountEditModel, null, true));
			int num = UpdateBankAccountByTran(ctx, list2);
			operationResult.Success = (num > 0);
			return operationResult;
		}

		public static BDAccountEditModel ToAccountEditModel(BDBankAccountEditModel bankModel)
		{
			BDAccountEditModel bDAccountEditModel = new BDAccountEditModel();
			bDAccountEditModel.MBankAccountType = bankModel.MBankAccountType;
			bDAccountEditModel.MNumber = bankModel.MNumber;
			bDAccountEditModel.MultiLanguage = bankModel.MultiLanguage;
			bDAccountEditModel.MOrgID = bankModel.MOrgID;
			return bDAccountEditModel;
		}

		public bool IsBankInfoExists(MContext ctx, BDBankAccountEditModel bankModel)
		{
			string text = string.Format("SELECT COUNT(8) FROM T_BD_BankAccount a \r\n                            WHERE MOrgID=@MOrgID AND MIsDelete=0 \r\n                            AND (IFNULL(@MBankID,'')='' OR MItemID <> @MBankID)\r\n                            AND MBankTypeID=@MBankTypeID AND convert(AES_DECRYPT(MBankNo,'{0}') using utf8) = @MBankNo\r\n                            AND MCyID=@MCyID\r\n                            ", "JieNor-001");
			List<MySqlParameter> list = new List<MySqlParameter>
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MBankID", bankModel.MItemID),
				new MySqlParameter("@MBankNo", bankModel.MBankNo),
				new MySqlParameter("@MBankTypeID", bankModel.MBankTypeID),
				new MySqlParameter("@MAccountName", bankModel.MBankTypeID),
				new MySqlParameter("@MCyID", bankModel.MCyID)
			};
			StringBuilder stringBuilder = new StringBuilder();
			MultiLanguageFieldList multiLanguageFieldList = (from t in bankModel.MultiLanguage
			where t.MFieldName == "MName"
			select t).FirstOrDefault();
			int num = 0;
			foreach (MultiLanguageField item in multiLanguageFieldList.MMultiLanguageField)
			{
				if (!string.IsNullOrEmpty(item.MValue) && item.MValue.Trim().Length != 0)
				{
					if (num > 0)
					{
						stringBuilder.Append(" OR ");
					}
					stringBuilder.AppendFormat(" (MName=@MName{0} AND MLocaleID=@MLocaleID{0}) ", num);
					list.Add(new MySqlParameter($"@MName{num}", item.MValue));
					list.Add(new MySqlParameter($"@MLocaleID{num}", item.MLocaleID));
					num++;
				}
			}
			if (stringBuilder.Length > 0)
			{
				text = $"{text}  AND EXISTS (SELECT * FROM T_BD_BankAccount_L b WHERE a.MItemID=b.MParentID AND  MOrgID=@MOrgID AND MIsDelete=0 \r\n                                        AND (IFNULL(@MBankID,'')='' OR MParentID <> @MBankID) AND ({stringBuilder}) )";
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			object single = dynamicDbHelperMySQL.GetSingle(text, list.ToArray());
			return Convert.ToInt32(single) > 0;
		}

		public static DataGridJson<BDBankInitBalanceModel> GetInitBalanceListByPage(MContext ctx, SqlWhere filter)
		{
			SqlQuery sqlQuery = new SqlQuery();
			StringBuilder bankInitBalanceSql = GetBankInitBalanceSql();
			filter.AddOrderBy("MCreateDate", SqlOrderDir.Asc);
			sqlQuery.SqlWhere = filter;
			sqlQuery.SelectString = bankInitBalanceSql.ToString();
			MySqlParameter[] parameters = ctx.GetParameters((MySqlParameter)null);
			foreach (MySqlParameter para in parameters)
			{
				sqlQuery.AddParameter(para);
			}
			return ModelInfoManager.GetPageDataModelListBySql<BDBankInitBalanceModel>(ctx, sqlQuery);
		}

		public static BDBankInitBalanceModel GetBankInitBalanceByBankId(MContext ctx, string bankId)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("select * from ({0})v where v.MBankID=@MBankID and MOrgID = @MOrgID and MIsDelete = 0 ", GetBankInitBalanceSql());
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet ds = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), ctx.GetParameters("@MBankID", bankId));
			return ModelInfoManager.DataTableToList<BDBankInitBalanceModel>(ds, 1, false).FirstOrDefault();
		}

		private static StringBuilder GetBankInitBalanceSql()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT a.MItemID as MBankID , a.MItemID as MAccountID , c.MID, al.MName as MBankName , convert(AES_DECRYPT(a.MBankNo,'{0}') using utf8) as MBankNo , a.MCyID , bl.MName as MBankTypeName , c.MBeginBalance,c.MBeginBalanceFor,a.MCreateDate , a.MOrgID , a.MIsActive , a.MIsDelete FROM t_bd_bankaccount a ", "JieNor-001");
			stringBuilder.AppendLine("INNER JOIN t_bd_bankaccount_l al on al.MParentID = a.MItemID and al.MLocaleID=@MLocaleID and al.MOrgID=a.MOrgID and al.MIsDelete = 0  ");
			stringBuilder.AppendLine("INNER JOIN t_bd_banktype_l bl on bl.MParentID = a.MBankTypeID and bl.MLocaleID=@MLocaleID and bl.MIsDelete = 0  ");
			stringBuilder.AppendLine("LEFT JOIN t_gl_initbankbalance c on c.MAccountID = a.MItemID  and c.MOrgID=a.MOrgID and c.MIsDelete = 0  where a.MOrgID = @MOrgID and a.MIsDelete = 0 ");
			stringBuilder.AppendLine(" UNION ");
			stringBuilder.AppendFormat("SELECT a.MItemID as MBankID , a.MItemID as MAccountID , c.MID, al.MName as MBankName , convert(AES_DECRYPT(a.MBankNo,'{0}') using utf8) as MBankNo , a.MCyID , '' as MBankTypeName , c.MBeginBalance,c.MBeginBalanceFor,a.MCreateDate,a.MOrgID , a.MIsActive , a.MIsDelete FROM t_bd_bankaccount a ", "JieNor-001");
			stringBuilder.AppendLine("INNER JOIN t_bd_bankaccount_l al on al.MParentID = a.MItemID and al.MLocaleID=@MLocaleID and al.MOrgID=a.MOrgID and al.MIsDelete = 0   and (a.MBankTypeID='' or a.MBankTypeID is null or a.MBankAccountType in ('3' ,'4' ,'5'))");
			stringBuilder.AppendLine("LEFT JOIN t_gl_initbankbalance c on a.mitemid=c.maccountid  and c.MOrgID=a.MOrgID and c.MIsDelete = 0  where a.MOrgID = @MOrgID and a.MIsDelete = 0 and a.MIsActive = 1 ");
			return stringBuilder;
		}

		public static CommandInfo GetUpdateBankInitBalanceCmd(MContext ctx, BDBankInitBalanceModel model)
		{
			CommandInfo commandInfo = new CommandInfo();
			if (string.IsNullOrWhiteSpace(model.MID))
			{
				commandInfo.CommandText = "insert into t_gl_initbankbalance(MID , MOrgID , MDate , MAccountID , MCyID , MBeginBalance,MBeginBalanceFor) \r\n                                        VALUES(@MID , @MOrgID , @MDate,@MAccountID,@MCyID,@MBeginBalance,@MBeginBalanceFor)";
				model.MID = UUIDHelper.GetGuid();
			}
			else
			{
				commandInfo.CommandText = "update t_gl_initbankbalance set MBeginBalance=@MBeginBalance , MBeginBalanceFor=@MBeginBalanceFor where MOrgID=@MOrgID and MID=@MID and MIsDelete = 0 ";
			}
			MySqlParameter[] array = new MySqlParameter[7]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MDate", MySqlDbType.DateTime),
				new MySqlParameter("@MAccountID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MBeginBalance", MySqlDbType.Decimal),
				new MySqlParameter("@MBeginBalanceFor", MySqlDbType.Decimal),
				new MySqlParameter("@MCyID", MySqlDbType.VarChar, 6)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = model.MID;
			array[2].Value = ctx.MBeginDate;
			array[3].Value = model.MAccountID;
			array[4].Value = model.MBeginBalance;
			array[5].Value = model.MBeginBalanceFor;
			array[6].Value = model.MCyID;
			DbParameter[] array2 = commandInfo.Parameters = array;
			return commandInfo;
		}

		public List<CommandInfo> GetDeleteBankInitBalanceCmds(MContext ctx, List<string> bankIdList)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			foreach (string bankId in bankIdList)
			{
				CommandInfo commandInfo = new CommandInfo();
				commandInfo.CommandText = "update t_gl_initbankbalance set MIsDelete=1 where MAccountID=@MAccountID and MOrgID = @MOrgID";
				MySqlParameter[] parameters = new MySqlParameter[2]
				{
					new MySqlParameter("@MOrgID", ctx.MOrgID),
					new MySqlParameter("@MAccountID", bankId)
				};
				DbParameter[] array = commandInfo.Parameters = parameters;
				list.Add(commandInfo);
			}
			return list;
		}
	}
}
