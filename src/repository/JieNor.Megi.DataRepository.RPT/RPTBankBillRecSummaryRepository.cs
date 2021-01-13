using JieNor.Megi.Core;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Script.Serialization;

namespace JieNor.Megi.DataRepository.RPT
{
	public class RPTBankBillRecSummaryRepository
	{
		public static string AddReport(MContext ctx)
		{
			List<RPTReportModel> list = new List<RPTReportModel>();
			RPTReportModel rPTReportModel = new RPTReportModel();
			rPTReportModel.MType = Convert.ToInt32(BizReportType.BankReconciliationSummary);
			rPTReportModel.MSheetName = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "BankReconciliationSummary", "Bank Reconciliation Summary");
			list.Add(rPTReportModel);
			RPTReportModel rPTReportModel2 = new RPTReportModel();
			rPTReportModel2.MType = Convert.ToInt32(BizReportType.BankStatement);
			rPTReportModel2.MSheetName = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "BankStatement", "Bank Statement");
			rPTReportModel2.MIsShow = true;
			list.Add(rPTReportModel2);
			return RPTReportRepository.AddEmptyReport(ctx, list);
		}

		public static void UpdateReport(MContext ctx, RPTBankBillRecSummaryFilterModel recFilter)
		{
			JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
			javaScriptSerializer.MaxJsonLength = 2147483647;
			RPTReportModel reportModel = RPTReportRepository.GetReportModel(recFilter.MReportID, BizReportType.BankStatement, ctx);
			if (reportModel != null)
			{
				RPTBankStatementFilterModel rPTBankStatementFilterModel = new RPTBankStatementFilterModel();
				rPTBankStatementFilterModel.MFromDate = recFilter.MFromDate;
				rPTBankStatementFilterModel.MEndDate = recFilter.MToDate;
				rPTBankStatementFilterModel.MBankAccountID = recFilter.MBankAccountID;
				if (!string.IsNullOrEmpty(reportModel.MContent))
				{
					BizReportModel bizReportModel = javaScriptSerializer.Deserialize<BizReportModel>(reportModel.MContent);
					RPTBankStatementFilterModel rPTBankStatementFilterModel2 = bizReportModel.Filter as RPTBankStatementFilterModel;
					if (rPTBankStatementFilterModel2 != null)
					{
						rPTBankStatementFilterModel.MIsReconciled = rPTBankStatementFilterModel2.MIsReconciled;
					}
				}
				BizReportModel report = RPTBankStatementRepository.GetReport(ctx, rPTBankStatementFilterModel);
				report.Filter = rPTBankStatementFilterModel;
				reportModel.MContent = javaScriptSerializer.Serialize(report);
				RPTReportRepository.UpdateReportContent(ctx, reportModel, true);
			}
		}

		public static void UpdateReport(MContext ctx, RPTBankStatementFilterModel statementFilter)
		{
			JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
			DateTime mEndDate = statementFilter.MEndDate;
			RPTReportModel mainReportModel = RPTReportRepository.GetMainReportModel(statementFilter.MReportID, ctx);
			if (mainReportModel != null)
			{
				RPTBankBillRecSummaryFilterModel rPTBankBillRecSummaryFilterModel = new RPTBankBillRecSummaryFilterModel();
				rPTBankBillRecSummaryFilterModel.MToDate = mEndDate;
				rPTBankBillRecSummaryFilterModel.MFromDate = mEndDate.AddMonths(-1);
				rPTBankBillRecSummaryFilterModel.MBankAccountID = statementFilter.MBankAccountID;
				BizReportModel report = GetReport(ctx, rPTBankBillRecSummaryFilterModel);
				report.Filter = rPTBankBillRecSummaryFilterModel;
				mainReportModel.MContent = javaScriptSerializer.Serialize(report);
				RPTReportRepository.UpdateReportContent(ctx, mainReportModel, true);
			}
		}

		public static BizReportModel GetReport(MContext ctx, RPTBankBillRecSummaryFilterModel filter)
		{
			BizReportModel bizReportModel = new BizReportModel();
			bizReportModel.Type = Convert.ToInt32(BizReportType.BankReconciliationSummary);
			DateTime dateTime = filter.MToDate;
			dateTime = dateTime.AddDays(1.0);
			filter.MToDate = dateTime.AddSeconds(-1.0);
			BDBankAccountRepository bDBankAccountRepository = new BDBankAccountRepository();
			BDBankAccountEditModel dataModel = bDBankAccountRepository.GetDataModel(ctx, filter.MBankAccountID, false);
			if (dataModel == null)
			{
				return bizReportModel;
			}
			SetReportTitle(ctx, filter, bizReportModel, dataModel);
			SetReportHead(ctx, filter, bizReportModel);
			decimal d = SetMegiBalance(ctx, filter, bizReportModel);
			decimal d2 = SetUnRecPayment(ctx, filter, bizReportModel);
			decimal d3 = SetUnRecReceive(ctx, filter, bizReportModel);
			decimal d4 = SetUnRecBankBill(ctx, filter, bizReportModel, dataModel);
			decimal bankBalance = d + d2 - d3 + d4;
			SetMBankBalance(ctx, filter, bizReportModel, bankBalance, dataModel);
			return bizReportModel;
		}

		private static void SetReportTitle(MContext ctx, RPTBankBillRecSummaryFilterModel filter, BizReportModel model, BDBankAccountEditModel bankModel)
		{
			model.HeaderTitle = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Summary", "Summary");
			model.Title1 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "BankReconciliationSummary", "Bank Reconciliation Summary");
			MultiLanguageFieldList multiLanguageFieldList = (from t in bankModel.MultiLanguage
			where t.MFieldName == "MName"
			select t).FirstOrDefault();
			if (multiLanguageFieldList != null)
			{
				model.Title2 = multiLanguageFieldList.MMultiLanguageValue;
			}
			model.Title3 = ctx.MOrgName;
			model.Title4 = RPTBaseREpository.GetDateHeadTitle(ctx, filter.MFromDate, filter.MToDate);
		}

		private static void SetReportHead(MContext ctx, RPTBankBillRecSummaryFilterModel filter, BizReportModel model)
		{
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = BizReportRowType.Header;
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangKey.Date),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangKey.Reference),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangKey.Amount),
				CellType = BizReportCellType.Money
			});
			model.AddRow(bizReportRowModel);
		}

		private static decimal SetMegiBalance(MContext ctx, RPTBankBillRecSummaryFilterModel filter, BizReportModel model)
		{
			decimal bankBalance = BDBankAccountRepository.GetBankBalance(ctx, filter.MFromDate, filter.MToDate, filter.MBankAccountID);
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = BizReportRowType.SubTotal;
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = filter.MToDate.ToString(ctx.MDateFormat),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "BalanceInMegi", "Balance in Hypercurrent"),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = bankBalance.To2Decimal(),
				CellType = BizReportCellType.Money
			});
			model.AddRow(bizReportRowModel);
			return bankBalance;
		}

		private static void SetMBankBalance(MContext ctx, RPTBankBillRecSummaryFilterModel filter, BizReportModel model, decimal bankBalance, BDBankAccountEditModel bankModel)
		{
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = BizReportRowType.SubTotal;
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = filter.MToDate.ToString(ctx.MDateFormat),
				CellType = BizReportCellType.Text
			});
			if (bankModel.MBankAccountType == 1)
			{
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "BankBalance", "Bank Balance"),
					CellType = BizReportCellType.Text
				});
			}
			else
			{
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "StatementBalance", "Statement Balance"),
					CellType = BizReportCellType.Text
				});
			}
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = bankBalance.To2Decimal(),
				CellType = BizReportCellType.Money
			});
			model.AddRow(bizReportRowModel);
		}

		private static decimal SetUnRecPayment(MContext ctx, RPTBankBillRecSummaryFilterModel filter, BizReportModel model)
		{
			decimal result = default(decimal);
			List<IVPaymentModel> unRecPaymentList = GetUnRecPaymentList(ctx, filter);
			if (unRecPaymentList == null || unRecPaymentList.Count == 0)
			{
				return result;
			}
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "PlusOutstandingPayments", "Plus Outstanding Payments");
			string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "TotalOutstandingPayments", "Total Outstanding Payments");
			SetGroup(ctx, model, text);
			result = unRecPaymentList.Sum((IVPaymentModel t) => t.MTaxTotalAmtFor);
			foreach (IVPaymentModel item in unRecPaymentList)
			{
				BizReportRowModel bizReportRowModel = new BizReportRowModel();
				bizReportRowModel.UniqueValue = item.MID;
				bizReportRowModel.RowType = BizReportRowType.Item;
				string value = item.MBizDate.ToString(ctx.MDateFormat);
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = value,
					CellType = BizReportCellType.Text
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = item.MReference,
					CellType = BizReportCellType.Text
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = item.MTaxTotalAmtFor.To2Decimal(),
					CellType = BizReportCellType.Money
				});
				model.AddRow(bizReportRowModel);
			}
			SetSubTotal(ctx, model, text2, result);
			return result;
		}

		private static List<IVPaymentModel> GetUnRecPaymentList(MContext ctx, RPTBankBillRecSummaryFilterModel filter)
		{
			string sql = "SELECT MID, MBizDate,MReference,(MTaxTotalAmtFor-MReconcileAmtFor) AS MTaxTotalAmtFor FROM T_IV_Payment \r\n                           WHERE MBankID=@MBankID AND MOrgID=@MOrgID AND MIsDelete=0\r\n                           AND MBizDate>=@MStartDate AND MBizDate<=@MEndDate\r\n                           AND (MTaxTotalAmtFor-MReconcileAmtFor) > 0\r\n                           ORDER BY MBizDate";
			MySqlParameter[] cmdParms = new MySqlParameter[4]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MStartDate", filter.MFromDate),
				new MySqlParameter("@MEndDate", filter.MToDate),
				new MySqlParameter("@MBankID", filter.MBankAccountID)
			};
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet ds = dynamicDbHelperMySQL.Query(sql, cmdParms);
			return ModelInfoManager.DataTableToList<IVPaymentModel>(ds);
		}

		private static decimal SetUnRecReceive(MContext ctx, RPTBankBillRecSummaryFilterModel filter, BizReportModel model)
		{
			decimal result = default(decimal);
			List<IVReceiveModel> unRecReceiveList = GetUnRecReceiveList(ctx, filter);
			if (unRecReceiveList == null || unRecReceiveList.Count == 0)
			{
				return result;
			}
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "LessOutstandingReceipts", "Less Outstanding Receipts");
			string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "TotalOutstanding Receipts", "Total Outstanding Receipts");
			SetGroup(ctx, model, text);
			result = unRecReceiveList.Sum((IVReceiveModel t) => t.MTaxTotalAmtFor);
			foreach (IVReceiveModel item in unRecReceiveList)
			{
				BizReportRowModel bizReportRowModel = new BizReportRowModel();
				bizReportRowModel.UniqueValue = item.MID;
				bizReportRowModel.RowType = BizReportRowType.Item;
				string value = item.MBizDate.ToString(ctx.MDateFormat);
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = value,
					CellType = BizReportCellType.Text
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = item.MReference,
					CellType = BizReportCellType.Text
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = item.MTaxTotalAmtFor.To2Decimal(),
					CellType = BizReportCellType.Money
				});
				model.AddRow(bizReportRowModel);
			}
			SetSubTotal(ctx, model, text2, result);
			return result;
		}

		private static List<IVReceiveModel> GetUnRecReceiveList(MContext ctx, RPTBankBillRecSummaryFilterModel filter)
		{
			string sql = "SELECT MID, MBizDate,MReference,(MTaxTotalAmtFor-MReconcileAmtFor) AS MTaxTotalAmtFor FROM T_IV_Receive \r\n                           WHERE MBankID=@MBankID AND MOrgID=@MOrgID AND MIsDelete=0\r\n                           AND MBizDate>=@MStartDate AND MBizDate<=@MEndDate\r\n                           AND (MTaxTotalAmtFor-MReconcileAmtFor) > 0\r\n                           ORDER BY MBizDate";
			MySqlParameter[] cmdParms = new MySqlParameter[4]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MStartDate", filter.MFromDate),
				new MySqlParameter("@MEndDate", filter.MToDate),
				new MySqlParameter("@MBankID", filter.MBankAccountID)
			};
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet ds = dynamicDbHelperMySQL.Query(sql, cmdParms);
			return ModelInfoManager.DataTableToList<IVReceiveModel>(ds);
		}

		private static decimal SetUnRecBankBill(MContext ctx, RPTBankBillRecSummaryFilterModel filter, BizReportModel model, BDBankAccountEditModel bankModel)
		{
			decimal num = default(decimal);
			List<IVBankBillRecListModel> bankBillNoRecListByDate = GetBankBillNoRecListByDate(ctx, filter.MBankAccountID, filter.MToDate, filter.MFromDate);
			if (bankBillNoRecListByDate == null || bankBillNoRecListByDate.Count == 0)
			{
				return num;
			}
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "PlusUnReconciledBankStatementLines", "Plus Un-Reconciled Bank Statement Lines");
			string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "TotalUnReconciledBankStatementLines", "Total Un-Reconciled Bank Statement Lines");
			if (bankModel.MBankAccountType != 1)
			{
				text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "PlusUnReconciledStatementLines", "Plus Un-Reconciled Statement Lines");
				text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "TotalUnReconciledStatementLines", "Total Un-Reconciled Statement Lines");
			}
			SetGroup(ctx, model, text);
			List<IVBankBillRecListModel> list = (from t in bankBillNoRecListByDate
			orderby t.MDate
			select t).ToList();
			foreach (IVBankBillRecListModel item in bankBillNoRecListByDate)
			{
				decimal num2 = (Math.Abs(item.MSpentAmt) > decimal.Zero) ? (-Math.Abs(item.MSpentAmt)) : Math.Abs(item.MReceivedAmt);
				BizReportRowModel bizReportRowModel = new BizReportRowModel();
				bizReportRowModel.UniqueValue = item.MID;
				bizReportRowModel.RowType = BizReportRowType.Item;
				string value = item.MDate.ToString(ctx.MDateFormat);
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = value,
					CellType = BizReportCellType.Text
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = item.MDesc,
					CellType = BizReportCellType.Text
				});
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = num2.To2Decimal(),
					CellType = BizReportCellType.Money
				});
				model.AddRow(bizReportRowModel);
				num += num2;
			}
			SetSubTotal(ctx, model, text2, num);
			return num;
		}

		private static List<IVBankBillRecListModel> GetBankBillNoRecListByDate(MContext ctx, string bankId, DateTime endDate, DateTime? StartDate)
		{
			if (string.IsNullOrEmpty(bankId))
			{
				return new List<IVBankBillRecListModel>();
			}
			StartDate = (StartDate.HasValue ? StartDate.Value : new DateTime(1900, 1, 1));
			string sql = " Select a.MID,a.MSpentAmt,a.MReceivedAmt,a.MDesc,a.MDate from T_IV_BankBillEntry a\r\n                                    INNER JOIN T_IV_BankBill b ON a.MID=b.MID AND b.MIsDelete=0 AND IFNULL(a.MParentID,'')=''\r\n                                    WHERE a.MIsDelete=0 and b.MOrgId = @MOrgID AND b.MBankID=@MBankID and MDate<=@EndDate AND MDate >=@StartDate\r\n                                    AND not exists(SELECT 1 FROM T_IV_BankBillReconcile c \r\n                                                   WHERE c.MBankBillEntryID=a.MEntryID and c.MIsDelete=0)\r\n                                    and a.MCheckState <> '2'\r\n                                    ORDER BY a.MDate";
			MySqlParameter[] cmdParms = new MySqlParameter[4]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@StartDate", StartDate),
				new MySqlParameter("@EndDate", endDate),
				new MySqlParameter("@MBankID", bankId)
			};
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataTable dt = dynamicDbHelperMySQL.Query(sql, cmdParms).Tables[0];
			return ModelInfoManager.DataTableToList<IVBankBillRecListModel>(dt);
		}

		private static void SetGroup(MContext ctx, BizReportModel model, string groupName)
		{
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = BizReportRowType.Group;
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = groupName,
				CellType = BizReportCellType.Text
			});
			model.AddRow(bizReportRowModel);
		}

		private static void SetSubTotal(MContext ctx, BizReportModel model, string subTitleName, decimal subTotal)
		{
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = BizReportRowType.SubTotal;
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = subTitleName,
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = "",
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = subTotal.To2Decimal(),
				CellType = BizReportCellType.Money
			});
			model.AddRow(bizReportRowModel);
		}
	}
}
