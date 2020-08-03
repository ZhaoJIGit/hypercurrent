using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.MultiLanguage;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataRepository.IV;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.DataRepository.BD
{
	public class BDLayoutRepository
	{
		public static OperationResult UpdateBDLayout(MContext ctx, List<BDLayoutEditModel> modelList)
		{
			foreach (BDLayoutEditModel model in modelList)
			{
				model.MOrgID = ctx.MOrgID;
			}
			return ModelInfoManager.InsertOrUpdate(ctx, modelList, null);
		}

		public static List<BDLayoutListModel> GetBankLayoutList(MContext ctx, string[] accountIds, DateTime startDate, DateTime endDate, bool needSum)
		{
			List<BDLayoutListModel> bankLayoutInfoList = GetBankLayoutInfoList(ctx, accountIds);
			if (bankLayoutInfoList != null && bankLayoutInfoList.Count > 0)
			{
				UpdateBankInfo(ctx, bankLayoutInfoList, startDate, endDate, needSum);
			}
			return bankLayoutInfoList;
		}

		public static BDLayoutModel GetBDLayoutModel(MContext ctx, DateTime startDate, DateTime endDate)
		{
			BDLayoutModel bDLayoutModel = new BDLayoutModel();
			List<BDLayoutListModel> bDLayoutList = GetBDLayoutList(ctx);
			if (bDLayoutList != null && bDLayoutList.Count > 0)
			{
				int num = bDLayoutList.Count((BDLayoutListModel t) => t.MName.Equals("Bank"));
				if (num > 0)
				{
					bDLayoutList = (from t in bDLayoutList
					where !t.MName.Equals("BankNew")
					select t).ToList();
					UpdateBankInfo(ctx, bDLayoutList, startDate, endDate, false);
				}
				else
				{
					bDLayoutList = (from t in bDLayoutList
					where !t.MName.Equals("BankSummary")
					select t).ToList();
				}
				bDLayoutModel.Left = (from t in bDLayoutList
				where t.MPortal == "Left"
				orderby t.MSeq
				select t).ToList();
				bDLayoutModel.Right = (from t in bDLayoutList
				where t.MPortal == "Right"
				orderby t.MSeq
				select t).ToList();
			}
			return bDLayoutModel;
		}

		private static void UpdateBankInfo(MContext ctx, List<BDLayoutListModel> list, DateTime startDate, DateTime endDate, bool needSum)
		{
			List<NameValueModel> cashCodingGroupInfo = IVBankBillEntryRepository.GetCashCodingGroupInfo(ctx, startDate, endDate, null);
			List<NameValueModel> statementGroupInfo = IVBankBillEntryRepository.GetStatementGroupInfo(ctx, startDate, endDate);
			List<BDBankBalanceModel> bankBalanceList = BDBankAccountRepository.GetBankBalanceList(ctx, startDate, endDate, false, null);
			BDLayoutListModel bDLayoutListModel = new BDLayoutListModel
			{
				MBankRecCount = 0,
				MBankIsUse = false,
				MBankBalance = decimal.Zero,
				MBankChartInfo = null,
				MBankStatement = decimal.Zero
			};
			foreach (BDLayoutListModel item in list)
			{
				if (cashCodingGroupInfo != null && cashCodingGroupInfo.Count > 0)
				{
					item.MBankRecCount = GetLayoutRemark(cashCodingGroupInfo, item.MRefID).ToMInt32();
					bDLayoutListModel.MBankRecCount += item.MBankRecCount;
				}
				if (statementGroupInfo != null && statementGroupInfo.Count > 0)
				{
					decimal num = GetLayoutRemark(statementGroupInfo, item.MRefID).ToMDecimal();
					item.MBankStatement = num;
					BDLayoutListModel bDLayoutListModel2 = bDLayoutListModel;
					bDLayoutListModel2.MBankStatement += num;
				}
				if (bankBalanceList != null && bankBalanceList.Count > 0)
				{
					bankBalanceList.RemoveAt(0);
					List<BDBankBalanceModel> list2 = (from t in bankBalanceList
					where item.MRefID.Equals(t.MBankID)
					select t).ToList();
					if (list2 != null && list2.Count() > 0)
					{
						item.MBankIsUse = true;
						bDLayoutListModel.MBankIsUse = true;
						item.MBankBalance = list2.Sum((BDBankBalanceModel t) => t.MTotalAmtFor);
						BDLayoutListModel bDLayoutListModel3 = bDLayoutListModel;
						bDLayoutListModel3.MBankBalance += item.MBankBalance;
						item.MBankChartInfo = GetBankChartModel(ctx, list2, startDate, endDate);
					}
				}
			}
			if (bankBalanceList != null && bankBalanceList.Count > 0)
			{
				bDLayoutListModel.MBankChartInfo = GetBankChartModel(ctx, bankBalanceList, startDate, endDate);
			}
			if (needSum)
			{
				list.Insert(0, bDLayoutListModel);
			}
		}

		private static ChartModel GetBankChartModel(MContext ctx, List<BDBankBalanceModel> bankBalanceList, DateTime startDate, DateTime endDate)
		{
			ChartModel chartModel = new ChartModel();
			DateTime d = startDate;
			int days = (endDate - d).Days;
			string[] array = new string[days];
			string[] array2 = new string[days];
			object[] array3 = new object[days];
			for (int i = 0; i < days; i++)
			{
				DateTime dtTemp = d.AddDays((double)(i + 1));
				if (dtTemp.DayOfWeek == DayOfWeek.Sunday)
				{
					array[i] = dtTemp.ToOrgZoneDateString(ctx);
				}
				else
				{
					array[i] = "";
				}
				array2[i] = dtTemp.ToOrgZoneDateString(ctx);
				array3[i] = (from t in bankBalanceList
				where t.MBizDate < dtTemp
				select t).Sum((BDBankBalanceModel t) => t.MTotalAmtFor);
			}
			chartModel.MLabels = array;
			chartModel.MValue = array3;
			chartModel.MTipLabels = array2;
			double maxAmt = array3.Max().ToMDouble();
			double minAmt = array3.Min().ToMDouble();
			chartModel.MScale = ChartHelper.GetChartScaleModel(maxAmt, minAmt);
			return chartModel;
		}

		private static string GetLayoutRemark(List<NameValueModel> list, string key)
		{
			NameValueModel nameValueModel = list.FirstOrDefault((NameValueModel t) => key.Equals(t.MName));
			return (nameValueModel == null) ? string.Empty : nameValueModel.MValue;
		}

		private static List<BDLayoutListModel> GetBDLayoutList(MContext ctx)
		{
			string sql = "SELECT a.MItemID, MPortal,'' as MCurrencyID, MSource,MRefID,'Bank' AS MName, '~/Areas/Chart/Views/Bank/ChtBank.cshtml' AS MPath,MSeq, cast(a.MIsActive AS SIGNED) AS MIsActive,\r\n                            c.MName as MBankAcctName, b.MBankNo as MBankNo,b.MBankTypeID,d.MName as 'MBankTypeName', b.MIsCreditCard,b.MIsCash,b.MIsShowInHome,b.MModifyDate\r\n                            FROM T_BD_Layout a \r\n                            INNER JOIN T_BD_BankAccount b ON a.MRefID=b.MItemID and a.MSource='Bank' and b.MOrgID = a.MOrgID and b.MIsDelete = 0 \r\n                            INNER JOIN T_BD_BankAccount_L c ON b.MItemID=c.MParentID AND MLocaleID=@MLocaleID and c.MOrgID = a.MOrgID and c.MIsDelete = 0 \r\n                            INNER JOIN T_BD_BankType_l d on b.MBankTypeID = d.MParentID AND d.MLocaleID=@MLocaleID  and d.MOrgID = a.MOrgID and d.MIsDelete = 0 \r\n                            WHERE a.MOrgID=@MOrgID AND a.MIsDelete=0 AND a.MIsActive=1 AND b.MIsShowInHome = 1\r\n                            UNION\r\n                            \r\n                            SELECT '' AS MItemID, 'Left' AS MPortal, MCyID as MCurrencyID, 'Bank' AS MSource, MItemID AS MRefID,'Bank' AS MName,'~/Areas/Chart/Views/Bank/ChtBank.cshtml' AS MPath, 100 AS MSeq,1 AS MIsActive,\r\n                            b.MName as MBankAcctName, a.MBankNo as MBankNo,a.MBankTypeID,d.MName as 'MBankTypeName',a.MIsCreditCard,a.MIsCash,a.MIsShowInHome,a.MModifyDate\r\n                            FROM T_BD_BankAccount a\r\n                            INNER JOIN T_BD_BankAccount_L b ON a.MItemID=b.MParentID AND MLocaleID=@MLocaleID\r\n                             INNER JOIN T_BD_BankType_l d on a.MBankTypeID = d.MParentID AND d.MLocaleID=@MLocaleID\r\n                            WHERE a.MOrgID=@MOrgID AND a.MIsDelete=0 AND a.MIsActive=1 AND a.MIsShowInHome=1\r\n                            AND NOT exists(SELECT 1 FROM T_BD_Layout b WHERE b.MOrgID=@MOrgID\r\n                            AND MSource='Bank' AND b.MRefID=a.MItemID)\r\n                            \r\n                            UNION \r\n                            SELECT a.MItemID, a.MPortal,'' as MCurrencyID, MSource,MRefID,b.MName, b.MPath,a.MSeq, cast(MIsActive AS SIGNED) AS MIsActive,\r\n                            '' as MBankAcctName, '' as MBankNo,'' AS MBankTypeID,'' as 'MBankTypeName',NULL AS MIsCreditCard,NULL AS MIsCash, NULL AS MIsShowInHome,NULL as MModifyDate\r\n                            FROM T_BD_Layout a\r\n                            INNER JOIN T_BD_LayoutConfig b ON a.MRefID=b.MItemID AND MSource='LayountConfig'\r\n                            WHERE MOrgID=@MOrgID\r\n                            \r\n                            UNION\r\n                            SELECT '' AS MItemID, MPortal,'' as MCurrencyID, 'LayountConfig' AS MSource, MItemID AS MRefID, MName, MPath, MSeq, 1 AS MIsActive,\r\n                            '' as MBankAcctName, '' as MBankNo,'' AS MBankTypeID,'' as 'MBankTypeName',NULL AS MIsCreditCard,NULL AS MIsCash, NULL AS MIsShowInHome,NULL as MModifyDate\r\n                            FROM T_BD_LayoutConfig a\r\n                            WHERE NOT EXISTS(SELECT 1 FROM T_BD_Layout b WHERE MOrgID=@MOrgID\r\n\t\t\t\t                            AND a.MItemID= b.MRefID AND MSource='LayountConfig')";
			return ModelInfoManager.GetDataModelBySql<BDLayoutListModel>(ctx, sql, ctx.GetParameters((MySqlParameter)null));
		}

		private static List<BDLayoutListModel> GetBankLayoutInfoList(MContext ctx, string[] ids)
		{
			string text = string.Empty;
			foreach (string str in ids)
			{
				text = text + "'" + str + "',";
			}
			string str2 = (ids.Length != 0) ? (" AND  a.MItemID in(" + text.TrimEnd(',') + ") ") : "";
			string sql = "SELECT a.MItemID AS MRefID,  a.MItemID, MCyID as MCurrencyID, b.MName as MBankAcctName, a.MBankNo,a.MBankTypeID,c.MName as MBankTypeName, a.MIsCreditCard,a.MIsCash,a.MIsShowInHome,a.MModifyDate\r\n                            FROM T_BD_BankAccount a\r\n                            LEFT JOIN T_BD_BankAccount_L b ON a.MItemID=b.MParentID AND MLocaleID=@MLocaleID\r\n\t\t\t\t\t\t\tLEFT JOIN T_BD_BankType_l c on a.MBankTypeID = c.MParentID AND c.MLocaleID=@MLocaleID\r\n                            WHERE a.MOrgID=@MOrgID AND a.MIsDelete=0 " + str2 + " ORDER BY MCreateDate";
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MLocaleID", ctx.MLCID)
			};
			return ModelInfoManager.GetDataModelBySql<BDLayoutListModel>(ctx, sql, cmdParms);
		}
	}
}
