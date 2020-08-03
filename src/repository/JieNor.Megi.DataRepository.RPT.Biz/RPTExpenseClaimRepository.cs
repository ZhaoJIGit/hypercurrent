using JieNor.Megi.Core;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace JieNor.Megi.DataRepository.RPT.Biz
{
	public class RPTExpenseClaimRepository
	{
		private List<string> DataColumns;

		private List<RPTExpenseClaimModel> DataList;

		private DateTime DataStartDate;

		private DateTime DataEndDate;

		private string BaseCurrencyID;

		private DataSet ExpenseClaimDetailDs;

		public RPTExpenseClaimRepository(string baseCurrencyID)
		{
			BaseCurrencyID = baseCurrencyID;
		}

		public BizReportModel ExpenseClaimList(MContext ctx, RPTExpenseClaimFilterModel filter)
		{
			BizReportModel bizReportModel = new BizReportModel();
			bizReportModel.Type = Convert.ToInt32(BizReportType.ExpenseClaims);
			SetCommonData(ctx, filter);
			SetTitle(bizReportModel, filter, ctx);
			SetRowHead(bizReportModel, filter, ctx);
			SetRowData(bizReportModel, filter, ctx);
			SetDataSummary(bizReportModel, filter, ctx);
			return bizReportModel;
		}

		private void SetCommonData(MContext context, RPTExpenseClaimFilterModel filter)
		{
			DataColumns = new List<string>();
			DataList = new List<RPTExpenseClaimModel>();
			if (filter.MPeriodCount < 6)
			{
				filter.MPeriodCount = 6;
			}
			SetDataColumns(context.MLCID, filter.MEndDate, filter.MPeriodCount, "asc", filter.MonthShowType);
			DataList = GetPerPeriodExpenseClaimData(context, filter);
		}

		private void SetDataColumns(string language, DateTime endDateTime, int periodCount, string order = "asc", string monthShowType = "1")
		{
			DateTime dateTime = new DateTime(endDateTime.Year, endDateTime.Month, 1);
			DataStartDate = dateTime.AddMonths(-periodCount + 1);
			DataEndDate = dateTime.AddMonths(1).AddDays(-1.0);
			DataColumns = new List<string>();
			if (order == "desc")
			{
				for (int num = periodCount - 1; num >= 0; num--)
				{
					DateTime date = dateTime.AddMonths(-num);
					string titlePeriod = GetTitlePeriod(language, monthShowType, date, num + 1);
					DataColumns.Add(titlePeriod);
				}
			}
			else
			{
				for (int i = 0; i < periodCount; i++)
				{
					DateTime date2 = dateTime.AddMonths(-i);
					string titlePeriod2 = GetTitlePeriod(language, monthShowType, date2, i);
					DataColumns.Add(titlePeriod2);
				}
			}
		}

		private string GetTitlePeriod(string language, string monthShowType, DateTime date, int period)
		{
			if (monthShowType != "2")
			{
				string text = date.Month.ToString();
				return COMMultiLangRepository.GetText(language, LangModule.BD, "Month_" + text, text);
			}
			switch (period)
			{
			case 1:
				return COMMultiLangRepository.GetText(language, LangModule.Report, "onemonthage", "1 month ago");
			case 2:
				return COMMultiLangRepository.GetText(language, LangModule.Report, "twomonthage", "2 month ago");
			case 3:
				return COMMultiLangRepository.GetText(language, LangModule.Report, "threemonthage", "3 month ago");
			case 4:
				return COMMultiLangRepository.GetText(language, LangModule.Report, "fourmonthage", "4 month ago");
			case 5:
				return COMMultiLangRepository.GetText(language, LangModule.Report, "fivemonthage", "5 month ago");
			default:
				return COMMultiLangRepository.GetText(language, LangModule.Report, "Current", "Current");
			}
		}

		private List<RPTExpenseClaimModel> GetPerPeriodExpenseClaimData(MContext ctx, RPTExpenseClaimFilterModel filter)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string text = "";
			if (filter.SortByValue != null && filter.SortByValue.Length != 0)
			{
				string[] sortByValue = filter.SortByValue;
				foreach (string text2 in sortByValue)
				{
					if (!string.IsNullOrEmpty(text2))
					{
						text = text + "'" + text2 + "',";
					}
				}
				if (!string.IsNullOrEmpty(text))
				{
					text = text.Substring(0, text.Length - 1);
				}
			}
			string text3 = "";
			if (filter.TrackIds != null && filter.TrackIds.Length != 0)
			{
				for (int j = 0; j < filter.TrackIds.Length; j++)
				{
					string text4 = " v.MTrackItem" + (j + 1);
					if (filter.TrackIds[j] != null)
					{
						string text5 = filter.TrackIds[j].Split('-')[1];
						string text6 = text5;
						if (!(text6 == "0") && (text6 == null || text6.Length != 0))
						{
							text3 = ((!(text6 == "1")) ? (text3 + text4 + "= '" + text5 + "' and ") : (text3 + text4 + " is null and "));
						}
					}
				}
				if (!string.IsNullOrEmpty(text3))
				{
					text3 = text3.Substring(0, text3.Length - 4);
				}
			}
			if (filter.SortBy == "1")
			{
				stringBuilder.AppendFormat("select sum(v1.MTaxTotalAmt) as MAmount , v1.departmentID as StatisticsFieldId , max(v1.DepartmentName) as StatisticsField , MBizDate from ( {0} ) v1 ", GetExpenseClaimViewSqlByDepartment(ctx, text3));
				stringBuilder.Append(" Where v1.MBizDate >= @startDate and v1.MBizDate <= @endDate ");
				if (!string.IsNullOrEmpty(text))
				{
					stringBuilder.AppendFormat(" and v1.departmentID in ({0})", text);
				}
				stringBuilder.Append(" GROUP BY v1.departmentID,v1.MBizDate");
			}
			if (filter.SortBy == "2")
			{
				stringBuilder.AppendFormat("select sum(v1.MTaxTotalAmt) as MAmount , v1.employeeID as StatisticsFieldId , IF(v1.MLocaleID ='0x0009',CONCAT(max(v1.MFirstName)  ,' ', max(v1.MLastName)),CONCAT( max(v1.MLastName) ,' ',max(v1.MFirstName)) ) as StatisticsField , MBizDate from ( {0} ) v1  ", GetExpenseClaimViewSqlByEmp(ctx, text3));
				stringBuilder.Append(" Where v1.MBizDate >= @startDate and v1.MBizDate <= @endDate ");
				if (!string.IsNullOrEmpty(text))
				{
					stringBuilder.AppendFormat(" and v1.employeeID in ({0})", text);
				}
				stringBuilder.Append(" GROUP BY v1.employeeID,v1.MBizDate");
			}
			if (filter.SortBy == "3")
			{
				stringBuilder.AppendFormat("select sum(v.MTotalAmt) as MAmount , v.ExpenseItemID as StatisticsFieldId , max(v.ExpenseItemName) as StatisticsField , max(v.ExpenseParentID) as ExpenseParentId, MBizDate from ( {0} ) v ", GetExpenseClaimViewSqlByExpense());
				stringBuilder.Append(" Where v.MBizDate >= @startDate and v.MBizDate <= @endDate ");
				if (!string.IsNullOrEmpty(text))
				{
					stringBuilder.AppendFormat(" and ( v.ExpenseItemID in ({0}) or v.ExpenseParentId in ({0}))", text);
				}
				if (!string.IsNullOrEmpty(text3))
				{
					stringBuilder.AppendFormat(" and {0}", text3);
				}
				stringBuilder.Append(" GROUP BY v.ExpenseItemID,v.MBizDate");
			}
			MySqlParameter[] array = new MySqlParameter[6]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLCID", MySqlDbType.VarChar, 6),
				new MySqlParameter("@MOrgCyID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@startDate", MySqlDbType.VarChar, 36),
				new MySqlParameter("@endDate", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MUserID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = ctx.MLCID;
			array[2].Value = BaseCurrencyID;
			array[3].Value = filter.MStartDate;
			array[4].Value = DataEndDate;
			array[5].Value = ctx.MUserID;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return ModelInfoManager.DataTableToList<RPTExpenseClaimModel>(dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array).Tables[0]);
		}

		private string GetExpenseClaimViewSqlByDepartment(MContext ctx, string trackItemFilter)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(" select * from ( ");
			stringBuilder.Append("select \r\n\r\n                            t1.MDepartment as departmentID ,t31.MName as DepartmentName,\r\n\t\t\t\t\t\t\tt6.MItemID as ExpenseItemID , t61.MName as ExpenseItemName,t6.MParentItemID as ExpenseParentID,\r\n\t\t\t\t\t\t\tt5.MTrackItem1 ,t5.MTrackItem2,t5.MTrackItem3,t5.MTrackItem4,t5.MTrackItem5,\r\n\t\t\t                t1.MID , t1.MOrgID , t1.MType, t1.MEmployee as MEmployee , t1.MDepartMent , \r\n                            t1.MContactID , t1.MNumber , t1.MBizDate, t1.MDueDate as MDueDate ,t1.MTaxID,t1.MCyID,\r\n\t\t\t                t1.MExchangeRate , if(t1.MStatus<>4,t1.MTotalAmt,t1.MVerificationAmt) as MTotalAmt ,t1.MTotalAmtFor , t1.MTaxTotalAmtFor,t1.MTaxTotalAmt, t1.MVerificationAmt,\r\n\t\t\t                t1.MStatus as MStatus ,t1.MReference , t1.MDesc ,t1.MIsDelete , t1.MCreatorID , t1.MCreateDate,t1.MModifierID,t1.MModifyDate\r\n                            from t_iv_expense t1\r\n                            \r\n                            inner Join t_bd_department_l t31 on  t1.MOrgID = t31.MOrgID and t31.MParentID = t1.MDepartment and t31.MLocaleID=@MLCID and t1.MStatus<>1 and t31.MIsDelete=0\r\n                            inner Join T_IV_ExpenseEntry t5 on  t1.MOrgID = t5.MOrgID and t5.MID = t1.MID and t5.MIsDelete=0\r\n                            left Join T_BD_ExpenseItem t6 on t1.MOrgID = t6.MOrgID and  t6.MItemID = t5.MItemID and t6.MIsDelete=0\r\n                            left Join T_BD_ExpenseItem_l t61 on t1.MOrgID = t61.MOrgID and  t6.MItemID = t61.MParentID and t61.MLocaleID=@MLCID and t61.MIsDelete=0\r\n                            WHERE t1.MOrgID=@MOrgID  and t1.MIsDelete=0 \r\n\t\t\t\t\t\t\t");
			if (ctx.IsSelfData)
			{
				stringBuilder.Append(" AND t1.MCreatorID=@MUserID ");
			}
			stringBuilder.Append(" Union all ");
			stringBuilder.Append("select \r\n\t\r\n                                    if(t1.MDepartment is null , 'other_departmentID' , t1.MDepartment) as departmentID , '' as DepartmentName ,\r\n\t\t\t\t\t\t\t        t6.MItemID as ExpenseItemID , t61.MName as ExpenseItemName,t6.MParentItemID as ExpenseParentID,\r\n\t\t\t\t\t\t\t        t5.MTrackItem1 ,t5.MTrackItem2,t5.MTrackItem3,t5.MTrackItem4,t5.MTrackItem5,\r\n\t\t\t                        t1.MID , t1.MOrgID , t1.MType, t1.MEmployee as MEmployee , t1.MDepartMent , \r\n                                    t1.MContactID , t1.MNumber , t1.MBizDate, t1.MDueDate as MDueDate ,t1.MTaxID,t1.MCyID,\r\n\t\t\t                        t1.MExchangeRate , if(t1.MStatus<>4,t1.MTotalAmt,t1.MVerificationAmt) as MTotalAmt ,t1.MTotalAmtFor , t1.MTaxTotalAmtFor,t1.MTaxTotalAmt, t1.MVerificationAmt,\r\n\t\t\t                        t1.MStatus as MStatus ,t1.MReference , t1.MDesc ,t1.MIsDelete , t1.MCreatorID , t1.MCreateDate,t1.MModifierID,t1.MModifyDate\r\n                                  from t_iv_expense t1\r\n                                  inner Join T_IV_ExpenseEntry t5 on  t1.MOrgID = t5.MOrgID and  t5.MID = t1.MID and  t1.MDepartment is null and t1.MStatus<>1 and t5.MIsDelete=0\r\n                                  left Join T_BD_ExpenseItem t6 on  t1.MOrgID = t6.MOrgID and t6.MItemID = t5.MItemID and t6.MIsDelete=0\r\n                                  left Join T_BD_ExpenseItem_l t61 on t1.MOrgID = t61.MOrgID and  t6.MItemID = t61.MParentID and t61.MLocaleID=@MLCID and t61.MIsDelete=0\r\n                                  WHERE t1.MOrgID=@MOrgID  and t1.MIsDelete=0 ");
			if (ctx.IsSelfData)
			{
				stringBuilder.Append(" AND t1.MCreatorID=@MUserID ");
			}
			stringBuilder.Append(" ) v ");
			if (!string.IsNullOrEmpty(trackItemFilter))
			{
				stringBuilder.AppendFormat(" where {0}", trackItemFilter);
			}
			stringBuilder.Append(" group by v.mid");
			return stringBuilder.ToString();
		}

		private string GetExpenseClaimViewSqlByEmp(MContext ctx, string trackItemFilter)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(" select * from ( ");
			stringBuilder.Append("select \r\n\t\t\t\t\t\t\tt1.Memployee as employeeID , t21.MFirstName , t21.MLastName ,\r\n                            \r\n\t\t\t\t\t\t\tt6.MItemID as ExpenseItemID , t61.MName as ExpenseItemName,t6.MParentItemID as ExpenseParentID,\r\n\t\t\t\t\t\t\tt5.MTrackItem1 ,t5.MTrackItem2,t5.MTrackItem3,t5.MTrackItem4,t5.MTrackItem5,\r\n\t\t\t                t1.MID , t1.MOrgID , t1.MType, t1.MEmployee as MEmployee , t1.MDepartMent , \r\n                            t1.MContactID , t1.MNumber , t1.MBizDate, t1.MDueDate as MDueDate ,t1.MTaxID,t1.MCyID,\r\n\t\t\t                t1.MExchangeRate ,  if(t1.MStatus<>4,t1.MTaxTotalAmt,t1.MVerificationAmt) as MTotalAmt ,t1.MTotalAmtFor , t1.MTaxTotalAmtFor,t1.MTaxTotalAmt, t1.MVerificationAmt,\r\n\t\t\t                t1.MStatus as MStatus ,t1.MReference , t1.MDesc ,t1.MIsDelete , t1.MCreatorID , t1.MCreateDate,t1.MModifierID,t1.MModifyDate,t21.MLocaleID\r\n                            from t_iv_expense t1\r\n                            inner join t_bd_employees_l t21 on  t1.MOrgID = t21.MOrgID and t21.MParentID = t1.Memployee and t21.MLocaleID=@MLCID  and t21.MIsDelete=0\r\n                            inner Join T_IV_ExpenseEntry t5 on  t1.MOrgID = t5.MOrgID and t5.MID = t1.MID  and t5.MIsDelete=0\r\n                            left Join T_BD_ExpenseItem t6 on t1.MOrgID = t6.MOrgID and  t6.MItemID = t5.MItemID  and t6.MIsDelete=0\r\n                            left Join T_BD_ExpenseItem_l t61 on  t1.MOrgID = t61.MOrgID and t6.MItemID = t61.MParentID and t61.MLocaleID=@MLCID and t61.MIsDelete=0\r\n                            WHERE t1.MOrgID=@MOrgID and t1.MIsDelete=0 and t1.MStatus<>1\r\n\t\t\t\t\t\t\t");
			if (ctx.IsSelfData)
			{
				stringBuilder.Append(" AND t1.MCreatorID=@MUserID ");
			}
			stringBuilder.Append(" Union all ");
			stringBuilder.Append("select \r\n\t\t\t\t\t\t\t        if(t1.MEmployee is null , 'other_employeeID' , t1.MEmployee) as employeeID , '' as MFirstName , ''  as MLastName ,\r\n                                   \r\n\t\t\t\t\t\t\t        t6.MItemID as ExpenseItemID , t61.MName as ExpenseItemName,t6.MParentItemID as ExpenseParentID,\r\n\t\t\t\t\t\t\t        t5.MTrackItem1 ,t5.MTrackItem2,t5.MTrackItem3,t5.MTrackItem4,t5.MTrackItem5,\r\n\t\t\t                        t1.MID , t1.MOrgID , t1.MType, t1.MEmployee as MEmployee , t1.MDepartMent , \r\n                                    t1.MContactID , t1.MNumber , t1.MBizDate, t1.MDueDate as MDueDate ,t1.MTaxID,t1.MCyID,\r\n\t\t\t                        t1.MExchangeRate ,  if(t1.MStatus<>4,t1.MTaxTotalAmt,t1.MVerificationAmt) as MTotalAmt ,t1.MTotalAmtFor , t1.MTaxTotalAmtFor,t1.MTaxTotalAmt, t1.MVerificationAmt,\r\n\t\t\t                        t1.MStatus as MStatus ,t1.MReference , t1.MDesc ,t1.MIsDelete , t1.MCreatorID , t1.MCreateDate,t1.MModifierID,t1.MModifyDate,'' as MLocaleID\r\n                                  from t_iv_expense t1\r\n                                  inner Join T_IV_ExpenseEntry t5 on t1.MOrgID = t5.MOrgID and  t1.Memployee is null and t5.MID = t1.MID and t5.MIsDelete=0\r\n                                  left Join T_BD_ExpenseItem t6 on t1.MOrgID = t6.MOrgID and  t6.MItemID = t5.MItemID and t6.MIsDelete=0\r\n                                  left Join T_BD_ExpenseItem_l t61 on t1.MOrgID = t61.MOrgID and  t6.MItemID = t61.MParentID and t61.MLocaleID=@MLCID and t61.MIsDelete=0\r\n                                  WHERE t1.MOrgID=@MOrgID and t1.MIsDelete=0 and t1.MStatus<>1\r\n                                  ");
			if (ctx.IsSelfData)
			{
				stringBuilder.Append(" AND t1.MCreatorID=@MUserID ");
			}
			stringBuilder.Append(" ) v ");
			if (!string.IsNullOrEmpty(trackItemFilter))
			{
				stringBuilder.AppendFormat(" where {0}", trackItemFilter);
			}
			stringBuilder.Append(" group by v.mid");
			return stringBuilder.ToString();
		}

		private string GetExpenseClaimViewSqlByExpense()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("select \r\n\t\t\t\t\t\t\t\r\n\t\t\t\t\t\t\tt6.MItemID as ExpenseItemID , t61.MName as ExpenseItemName,t6.MParentItemID as ExpenseParentID,\r\n\t\t\t\t\t\t\tt5.MTrackItem1 ,t5.MTrackItem2,t5.MTrackItem3,t5.MTrackItem4,t5.MTrackItem5,\r\n\t\t\t                t1.MID , t1.MOrgID , t1.MType, t1.MEmployee as MEmployee , t1.MDepartMent , \r\n                            t1.MContactID , t1.MNumber , t1.MBizDate, t1.MDueDate as MDueDate ,t1.MTaxID,t1.MCyID,\r\n\t\t\t                t1.MExchangeRate , t5.MAmount as MTotalAmt ,t5.MAmountFor as MTotalAmtFor  , t1.MTaxTotalAmtFor,t1.MTaxTotalAmt, t1.MVerificationAmt,\r\n\t\t\t                t1.MStatus as MStatus ,t1.MReference , t1.MDesc ,t1.MIsDelete , t1.MCreatorID , t1.MCreateDate,t1.MModifierID,t1.MModifyDate,t5.MTaxAmt , t5.MTaxAmtFor\r\n                            from t_iv_expense t1\r\n                           \r\n                            inner Join T_IV_ExpenseEntry t5 on  t5.MOrgID = t1.MOrgID and t5.MID = t1.MID and t5.MIsDelete=0\r\n                            left Join T_BD_ExpenseItem t6 on t6.MOrgID = t1.MOrgID and t6.MItemID = t5.MItemID and t6.MIsDelete=0\r\n                            left Join T_BD_ExpenseItem_l t61 on t61.MOrgID = t1.MOrgID and t6.MItemID = t61.MParentID and t61.MLocaleID=@MLCID and t61.MIsDelete=0\r\n                            WHERE t1.MOrgID=@MOrgID and t1.MIsDelete=0 and t1.MStatus<>1\r\n\t\t\t\t\t\t\t");
			stringBuilder.Append(" Union all ");
			stringBuilder.Append("select \r\n\t\t\t\t\t\t\t      \r\n\t\t\t\t\t\t\t        '' as ExpenseItemID , 'other_expense' as ExpenseItemName,'' as ExpenseParentID,\r\n\t\t\t\t\t\t\t        t5.MTrackItem1 ,t5.MTrackItem2,t5.MTrackItem3,t5.MTrackItem4,t5.MTrackItem5,\r\n\t\t\t                        t1.MID , t1.MOrgID , t1.MType, t1.MEmployee as MEmployee , t1.MDepartMent , \r\n                                    t1.MContactID , t1.MNumber , t1.MBizDate, t1.MDueDate as MDueDate ,t1.MTaxID,t1.MCyID,\r\n\t\t\t                        t1.MExchangeRate , t5.MAmount as MTotalAmt ,t5.MAmountFor as MTotalAmtFor  , t1.MTaxTotalAmtFor,t1.MTaxTotalAmt, t1.MVerificationAmt,\r\n\t\t\t                        t1.MStatus as MStatus ,t1.MReference , t1.MDesc ,t1.MIsDelete , t1.MCreatorID , t1.MCreateDate,t1.MModifierID,t1.MModifyDate,t5.MTaxAmt , t5.MTaxAmtFor\r\n                                  from t_iv_expense t1\r\n                                  inner Join T_IV_ExpenseEntry t5 on  t5.MOrgID = t1.MOrgID and  t5.MID = t1.MID and t5.MItemID is null and t5.MIsDelete=0\r\n                                  WHERE t1.MOrgID=@MOrgID and t1.MIsDelete=0 and t1.MStatus<>1");
			return stringBuilder.ToString();
		}

		private string GetExpenseClaimViewTableSql()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("select \r\n\t\t\t\t\t\t\tt1.Memployee as employeeID , t21.MFirstName , t21.MLastName ,\r\n                            t1.MDepartment as departmentID ,t31.MName as DepartmentName,\r\n\t\t\t\t\t\t\tt6.MItemID as ExpenseItemID , t61.MName as ExpenseItemName,t6.MParentItemID as ExpenseParentID,\r\n\t\t\t\t\t\t\tt5.MTrackItem1 ,t5.MTrackItem2,t5.MTrackItem3,t5.MTrackItem4,t5.MTrackItem5,\r\n\t\t\t                t1.MID , t1.MOrgID , t1.MType, t1.MEmployee as MEmployee , t1.MDepartMent , \r\n                            t1.MContactID , t1.MNumber , t1.MBizDate, t1.MDueDate as MDueDate ,t1.MTaxID,t1.MCyID,\r\n\t\t\t                t1.MExchangeRate , t5.MAmount as MTotalAmt ,t5.MAmountFor as MTotalAmtFor , t5.MTaxAmount ,t5.MTaxAmountFor , t5.MTaxAmt , t5.MTaxAmtFor , t1.MTaxTotalAmtFor,t1.MTaxTotalAmt, t1.MVerificationAmt,\r\n\t\t\t                t1.MStatus as MStatus ,t1.MReference , t1.MDesc ,t1.MIsDelete , t1.MCreatorID , t1.MCreateDate,t1.MModifierID,t1.MModifyDate\r\n                            from t_iv_expense t1\r\n                            inner Join T_IV_ExpenseEntry t5 on  t5.MOrgID = t1.MOrgID and t5.MID = t1.MID and t5.MIsDelete=0\r\n                            left Join T_BD_ExpenseItem t6 on  t6.MOrgID = t1.MOrgID and t6.MItemID = t5.MItemID and t6.MIsDelete=0\r\n                            left Join T_BD_ExpenseItem_l t61 on t61.MOrgID = t1.MOrgID and  t6.MItemID = t61.MParentID and t61.MLocaleID=@MLCID and t61.MIsDelete=0\r\n                            left join t_bd_employees_l t21 on t21.MOrgID = t1.MOrgID and  t21.MParentID = t1.Memployee and t21.MLocaleID=@MLCID and t21.MIsDelete=0\r\n                            left Join t_bd_department_l t31 on t31.MOrgID = t1.MOrgID and  t31.MParentID = t1.MDepartment and t31.MLocaleID=@MLCID and t31.MIsDelete=0\r\n                            where t1.MOrgID=@MOrgID and t1.MIsDelete=0 and t1.MStatus>1\r\n\t\t\t\t\t\t\t");
			return stringBuilder.ToString();
		}

		private void SetTitle(BizReportModel model, RPTExpenseClaimFilterModel filter, MContext context)
		{
			model.Title1 = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "ExpenseClaim", "Expense Claim");
			if (filter.SortBy == "3")
			{
				model.Title1 += COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "ExcludingVAT", "(不含增值税)");
			}
			model.Title2 = context.MOrgName;
			string[] obj = new string[7]
			{
				COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "ForThePeriod", "For the period "),
				" ",
				DataStartDate.ToString("D", CultureInfo.CreateSpecificCulture(RPTReportRepository.GetMultiLangKey(context))),
				" ",
				COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "To", "to"),
				" ",
				null
			};
			DateTime dateTime = filter.MEndDate;
			dateTime = dateTime.AddMonths(1);
			dateTime = dateTime.AddSeconds(-1.0);
			obj[6] = dateTime.ToString("D", CultureInfo.CreateSpecificCulture(RPTReportRepository.GetMultiLangKey(context)));
			model.Title3 = string.Concat(obj);
		}

		private void SetRowHead(BizReportModel model, RPTExpenseClaimFilterModel filter, MContext context)
		{
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = BizReportRowType.Header;
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "statisticsitems", "Statistics item"),
				CellType = BizReportCellType.Text
			});
			foreach (string dataColumn in DataColumns)
			{
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = dataColumn,
					CellType = BizReportCellType.Money
				});
			}
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Total", "Total"),
				CellType = BizReportCellType.Money
			});
			model.AddRow(bizReportRowModel);
		}

		private void SetRowData(BizReportModel model, RPTExpenseClaimFilterModel filter, MContext context)
		{
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			DateTime dateTime;
			if (filter.SortBy == "3")
			{
				ParentExpenseDataSupplement(context);
				IEnumerable<IGrouping<string, RPTExpenseClaimModel>> enumerable = from gr in DataList
				group gr by gr.StatisticsFieldId;
				foreach (IGrouping<string, RPTExpenseClaimModel> item in enumerable)
				{
					List<RPTExpenseClaimModel> list = item.ToList();
					string statisticsFieldId = list[0].StatisticsFieldId;
					string text = list[0].StatisticsField;
					string expenseParentId = list[0].ExpenseParentId;
					if (expenseParentId == "0")
					{
						IEnumerable<IGrouping<string, RPTExpenseClaimModel>> enumerable2 = from x in enumerable.ToList()
						where x.ToList()[0].ExpenseParentId == statisticsFieldId
						select x;
						bizReportRowModel = new BizReportRowModel();
						bizReportRowModel.RowType = BizReportRowType.Item;
						bizReportRowModel.SubRows = new List<BizReportRowModel>();
						if (string.IsNullOrEmpty(text) || text == " ")
						{
							text = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Other", "Other");
						}
						bizReportRowModel.AddCell(new BizReportCellModel
						{
							Value = text,
							CellType = BizReportCellType.Text,
							SubReport = GetBizSubRptModel(context, filter, item.ToList()[0], DataStartDate, DataEndDate)
						});
						decimal num = default(decimal);
						for (int i = 0; i < DataColumns.Count; i++)
						{
							dateTime = DataEndDate.AddDays(1.0);
							DateTime startDate = dateTime.AddMonths(-(i + 1));
							DateTime endDate = DataEndDate.AddMonths(-i);
							dateTime = endDate.AddDays((double)(1 - endDate.Day));
							dateTime = dateTime.AddMonths(1);
							endDate = dateTime.AddDays(-1.0);
							decimal num2 = (from w in item.ToList()
							where w.MBizDate >= startDate && w.MBizDate <= endDate
							select w).Sum((RPTExpenseClaimModel s) => s.MAmount);
							foreach (IGrouping<string, RPTExpenseClaimModel> item2 in enumerable2)
							{
								num2 += (from w in item2.ToList()
								where w.MBizDate >= startDate && w.MBizDate <= endDate
								select w).Sum((RPTExpenseClaimModel s) => s.MAmount);
							}
							bizReportRowModel.AddCell(new BizReportCellModel
							{
								Value = Convert.ToString(num2),
								CellType = BizReportCellType.Money
							});
							num += num2;
						}
						foreach (IGrouping<string, RPTExpenseClaimModel> item3 in enumerable2)
						{
							BizReportRowModel bizReportRowModel2 = new BizReportRowModel();
							bizReportRowModel2.RowType = BizReportRowType.SubItem;
							string statisticsFieldId2 = item3.ToList()[0].StatisticsFieldId;
							string text2 = item3.ToList()[0].StatisticsField;
							if (string.IsNullOrEmpty(text2) || text2 == " ")
							{
								text2 = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Other", "Other");
							}
							bizReportRowModel2.AddCell(new BizReportCellModel
							{
								Value = text2,
								CellType = BizReportCellType.Text,
								SubReport = GetBizSubRptModel(context, filter, item3.ToList()[0], DataStartDate, DataEndDate)
							});
							decimal num3 = default(decimal);
							for (int j = 0; j < DataColumns.Count; j++)
							{
								dateTime = DataEndDate.AddDays(1.0);
								DateTime subStartDate = dateTime.AddMonths(-(j + 1));
								DateTime subEndDate = DataEndDate.AddMonths(-j);
								decimal num4 = (from w in item3.ToList()
								where w.MBizDate >= subStartDate && w.MBizDate <= subEndDate
								select w).Sum((RPTExpenseClaimModel s) => s.MAmount);
								bizReportRowModel2.AddCell(new BizReportCellModel
								{
									Value = Convert.ToString(num4),
									CellType = BizReportCellType.Money
								});
								num3 += num4;
							}
							bizReportRowModel2.AddCell(new BizReportCellModel
							{
								Value = Convert.ToString(num3),
								CellType = BizReportCellType.Money
							});
							bizReportRowModel.SubRows.Add(bizReportRowModel2);
						}
						bizReportRowModel.AddCell(new BizReportCellModel
						{
							Value = Convert.ToString(num),
							CellType = BizReportCellType.Money
						});
						model.AddRow(bizReportRowModel);
					}
				}
			}
			else
			{
				IEnumerable<IGrouping<string, RPTExpenseClaimModel>> enumerable3 = from gr in DataList
				group gr by gr.StatisticsFieldId;
				foreach (IGrouping<string, RPTExpenseClaimModel> item4 in enumerable3)
				{
					bizReportRowModel = new BizReportRowModel();
					bizReportRowModel.RowType = BizReportRowType.Item;
					string statisticsFieldId3 = item4.ToList()[0].StatisticsFieldId;
					string text3 = item4.ToList()[0].StatisticsField;
					if (string.IsNullOrEmpty(text3) || text3 == " ")
					{
						text3 = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Other", "Other");
					}
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = text3,
						CellType = BizReportCellType.Text,
						SubReport = GetBizSubRptModel(context, filter, item4.ToList()[0], DataStartDate, DataEndDate)
					});
					decimal num5 = default(decimal);
					for (int k = 0; k < DataColumns.Count; k++)
					{
						dateTime = DataEndDate.AddDays(1.0);
						DateTime startDate2 = dateTime.AddMonths(-(k + 1));
						DateTime endDate2 = DataEndDate.AddMonths(-k);
						dateTime = endDate2.AddDays((double)(1 - endDate2.Day));
						dateTime = dateTime.AddMonths(1);
						endDate2 = dateTime.AddDays(-1.0);
						decimal num6 = (from w in item4.ToList()
						where w.MBizDate >= startDate2 && w.MBizDate <= endDate2
						select w).Sum((RPTExpenseClaimModel s) => s.MAmount);
						bizReportRowModel.AddCell(new BizReportCellModel
						{
							Value = Convert.ToString(num6),
							CellType = BizReportCellType.Money
						});
						num5 += num6;
					}
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToString(num5),
						CellType = BizReportCellType.Money
					});
					model.AddRow(bizReportRowModel);
				}
			}
		}

		private void ParentExpenseDataSupplement(MContext context)
		{
			BDExpenseItemRepository bDExpenseItemRepository = new BDExpenseItemRepository();
			List<BDExpenseItemModel> parentExpenseItemList = bDExpenseItemRepository.GetParentExpenseItemList(context, true, false, false);
			List<RPTExpenseClaimModel> list = new List<RPTExpenseClaimModel>();
			foreach (RPTExpenseClaimModel data in DataList)
			{
				if (!DataList.Exists((RPTExpenseClaimModel x) => x.StatisticsFieldId == data.ExpenseParentId) && !list.Exists((RPTExpenseClaimModel x) => x.StatisticsFieldId == data.ExpenseParentId))
				{
					RPTExpenseClaimModel rPTExpenseClaimModel = new RPTExpenseClaimModel();
					rPTExpenseClaimModel.StatisticsFieldId = data.ExpenseParentId;
					rPTExpenseClaimModel.MAmount = decimal.Zero;
					rPTExpenseClaimModel.MBizDate = data.MBizDate;
					BDExpenseItemModel bDExpenseItemModel = (from x in parentExpenseItemList
					where x.MItemID == data.ExpenseParentId
					select x).FirstOrDefault();
					if (bDExpenseItemModel != null)
					{
						rPTExpenseClaimModel.StatisticsField = bDExpenseItemModel.MName;
						rPTExpenseClaimModel.ExpenseParentId = "0";
						list.Add(rPTExpenseClaimModel);
					}
				}
			}
			DataList.AddRange(list);
			DataList = (from x in DataList
			orderby x.StatisticsFieldId
			select x).ToList();
		}

		private BizSubRptCreateModel GetBizSubRptModel(MContext ctx, RPTExpenseClaimFilterModel filter, RPTExpenseClaimModel model, DateTime startDate, DateTime endDate)
		{
			BizSubRptCreateModel bizSubRptCreateModel = new BizSubRptCreateModel();
			bizSubRptCreateModel.Text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Showtransactions", "Show transactions");
			bizSubRptCreateModel.ReportType = BizReportType.ExpenseClaimDetail;
			RPTExpenseClaimDeatailFilterModel rPTExpenseClaimDeatailFilterModel = new RPTExpenseClaimDeatailFilterModel();
			rPTExpenseClaimDeatailFilterModel.StatisticsType = filter.SortBy;
			rPTExpenseClaimDeatailFilterModel.StatisticsFieldId = model.StatisticsFieldId;
			rPTExpenseClaimDeatailFilterModel.StatisticsFieldOptionIds = new string[1]
			{
				model.StatisticsFieldId
			};
			rPTExpenseClaimDeatailFilterModel.StartDate = startDate;
			rPTExpenseClaimDeatailFilterModel.EndDate = endDate;
			rPTExpenseClaimDeatailFilterModel.TrackIds = filter.TrackIds;
			bizSubRptCreateModel.ReportFilter = rPTExpenseClaimDeatailFilterModel;
			return bizSubRptCreateModel;
		}

		private void SetDataSummary(BizReportModel model, RPTExpenseClaimFilterModel filter, MContext context)
		{
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = BizReportRowType.Total;
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Total", "Total"),
				CellType = BizReportCellType.Text
			});
			if (filter.SortBy == "3" && DataList != null)
			{
				DataList = (from x in DataList
				where !string.IsNullOrWhiteSpace(x.StatisticsField) && x.StatisticsField != "other_expense"
				select x).ToList();
			}
			decimal num = default(decimal);
			for (int i = 0; i < DataColumns.Count; i++)
			{
				DateTime dateTime = DataEndDate.AddDays(1.0);
				DateTime startDate = dateTime.AddMonths(-(i + 1));
				DateTime endDate = DataEndDate.AddMonths(-i);
				dateTime = endDate.AddDays((double)(1 - endDate.Day));
				dateTime = dateTime.AddMonths(1);
				endDate = dateTime.AddDays(-1.0);
				decimal num2 = (from w in DataList
				where w.MBizDate >= startDate && w.MBizDate <= endDate
				select w).Sum((RPTExpenseClaimModel s) => s.MAmount);
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = Convert.ToString(num2)
				});
				num += num2;
			}
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(num)
			});
			model.AddRow(bizReportRowModel);
		}

		public string GetChartStackedDictionary(MContext ctx, DateTime startDate, DateTime endDate, string statisticType = "1", int period = 6)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			double num = 0.0;
			SetDataColumns(ctx.MLCID, endDate, period, "desc", "1");
			List<ChartColumnStacked2DModel> list = (List<ChartColumnStacked2DModel>)(dictionary["data"] = GetChartColumnList(ctx, out num, statisticType));
			dictionary["labels"] = DataColumns;
			dictionary["scalSpace"] = Math.Ceiling(num / 30.0) * 10.0;
			JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
			return javaScriptSerializer.Serialize(dictionary);
		}

		private List<ChartColumnStacked2DModel> GetChartColumnList(MContext ctx, out double maxAmount, string statisticType)
		{
			maxAmount = 0.0;
			RPTExpenseClaimFilterModel rPTExpenseClaimFilterModel = new RPTExpenseClaimFilterModel();
			rPTExpenseClaimFilterModel.SortBy = statisticType;
			List<RPTExpenseClaimModel> perPeriodExpenseClaimData = GetPerPeriodExpenseClaimData(ctx, rPTExpenseClaimFilterModel);
			IEnumerable<IGrouping<string, RPTExpenseClaimModel>> enumerable = from gr in perPeriodExpenseClaimData
			group gr by gr.StatisticsFieldId;
			List<ChartColumnStacked2DModel> list = new List<ChartColumnStacked2DModel>();
			foreach (IGrouping<string, RPTExpenseClaimModel> item in enumerable)
			{
				string statisticsFieldId = item.ToList()[0].StatisticsFieldId;
				string text = item.ToList()[0].StatisticsField;
				if (string.IsNullOrEmpty(text) || text == " ")
				{
					text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Other", "Other");
				}
				ChartColumnStacked2DModel chartColumnStacked2DModel = new ChartColumnStacked2DModel();
				chartColumnStacked2DModel.value = new double[DataColumns.Count];
				DateTime dateTime = DataEndDate.AddDays(1.0);
				DateTime chartStartDate = dateTime.AddMonths(-DataColumns.Count);
				decimal value = (from w in item.ToList()
				where w.MBizDate < chartStartDate
				select w).Sum((RPTExpenseClaimModel s) => s.MAmount);
				chartColumnStacked2DModel.value[0] = (double)value;
				for (int num = DataColumns.Count; num > 0; num--)
				{
					dateTime = DataEndDate.AddDays(1.0);
					chartStartDate = dateTime.AddMonths(-num);
					dateTime = chartStartDate.AddMonths(1);
					DateTime chartEndDate = dateTime.AddSeconds(-1.0);
					value = (from w in item.ToList()
					where w.MBizDate >= chartStartDate && w.MBizDate <= chartEndDate
					select w).Sum((RPTExpenseClaimModel s) => s.MAmount);
					chartColumnStacked2DModel.value[DataColumns.Count - num] = (double)value;
					if ((double)value > maxAmount)
					{
						maxAmount = (double)value;
					}
				}
				var obj = new
				{
					name = text,
					MContactID = statisticsFieldId,
					MChartFirstName = "",
					MChartLastName = "",
					MChartDueOrOwing = ""
				};
				chartColumnStacked2DModel.name = new JavaScriptSerializer().Serialize(obj);
				list.Add(chartColumnStacked2DModel);
			}
			return list;
		}

		private string GetChartColor(int k)
		{
			switch (k)
			{
			case 0:
				return "#048fff";
			case 1:
				return "#1fb6e1";
			case 2:
				return "#51c5e7";
			case 3:
				return "#7ed4ee";
			case 4:
				return "#a8e1f2";
			case 5:
				return "#d3f0f9";
			case 6:
				return "#ffffff";
			case 7:
				return "#1fb6e1";
			default:
				return "#1fb6e1";
			}
		}

		public BizReportModel ExpenseClaimDetailList(MContext ctx, RPTExpenseClaimDeatailFilterModel filter)
		{
			BizReportModel bizReportModel = new BizReportModel();
			bizReportModel.Type = Convert.ToInt32(BizReportType.ExpenseClaimDetail);
			SetCommonDataForDetail(ctx, filter);
			SetTitleForDetail(bizReportModel, filter, ctx);
			SetRowHeadForDetail(bizReportModel, filter, ctx);
			decimal expenseAmount = default(decimal);
			decimal reviewAmount = default(decimal);
			SetRowDataForDetail(bizReportModel, filter, ctx, out expenseAmount, out reviewAmount);
			SetDataSummaryForDetail(bizReportModel, filter, ctx, expenseAmount, reviewAmount);
			return bizReportModel;
		}

		private void SetCommonDataForDetail(MContext ctx, RPTExpenseClaimDeatailFilterModel filter)
		{
			DataStartDate = filter.StartDate;
			DataEndDate = filter.EndDate;
		}

		private void SetTitleForDetail(BizReportModel reportModel, RPTExpenseClaimDeatailFilterModel filter, MContext ctx)
		{
			reportModel.Title1 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "ExpenseClaimTransactions", "Expense Claim Transactions");
			if (filter.StatisticsType == "3")
			{
				reportModel.Title1 += COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "ExcludingVAT", "(不含增值税)");
			}
			reportModel.Title2 = ctx.MOrgName;
			DateTime dateTime;
			if (!isNullDate(filter.StartDate))
			{
				string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "From", "From");
				dateTime = filter.StartDate;
				reportModel.Title3 = text + " " + dateTime.ToString("D", CultureInfo.CreateSpecificCulture(RPTReportRepository.GetMultiLangKey(ctx))) + " ";
			}
			if (!isNullDate(filter.EndDate))
			{
				string title = reportModel.Title3;
				string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "To", "to");
				dateTime = filter.EndDate;
				reportModel.Title3 = title + text2 + " " + dateTime.ToString("D", CultureInfo.CreateSpecificCulture(RPTReportRepository.GetMultiLangKey(ctx)));
			}
		}

		private bool isNullDate(DateTime date)
		{
			if (date.ToString("yyyy-MM-dd").Equals("0001-01-01", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			return false;
		}

		private void SetRowHeadForDetail(BizReportModel reportModel, RPTExpenseClaimDeatailFilterModel filter, MContext ctx)
		{
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = BizReportRowType.Header;
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Date", "Date"),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Type", "Type"),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Employee", "Employee"),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Reference", "Reference"),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Currency", "Currency"),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "ExpenseItem", "Expense Item"),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "AmountOriginalCurrency", "Amount(Original Currency)"),
				CellType = BizReportCellType.Money
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "AmountStandardCurrency", "Amount(Standard Currency)"),
				CellType = BizReportCellType.Money
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Total", "Total"),
				CellType = BizReportCellType.Money
			});
			reportModel.AddRow(bizReportRowModel);
		}

		private void SetRowDataForDetail(BizReportModel reportModel, RPTExpenseClaimDeatailFilterModel filter, MContext ctx, out decimal expenseAmount, out decimal reviewAmount)
		{
			expenseAmount = default(decimal);
			reviewAmount = default(decimal);
			ExpenseClaimDetailDs = GetExpenseClaimDetailDs(filter, ctx);
			if (ExpenseClaimDetailDs != null)
			{
				BizReportRowModel bizReportRowModel = new BizReportRowModel();
				foreach (DataRow row in ExpenseClaimDetailDs.Tables[0].Rows)
				{
					bizReportRowModel = new BizReportRowModel();
					bizReportRowModel.RowType = BizReportRowType.Item;
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToDateTime(row["MBizDate"]).ToString("yyyy-MM-dd"),
						CellType = BizReportCellType.Text
					});
					string empty = string.Empty;
					string text = Convert.ToString(row["MType"]);
					string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, text, text);
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = text2,
						CellType = BizReportCellType.Text,
						CellLink = GetCellLink(ctx, row)
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToString(row["EmployeeName"]),
						CellType = BizReportCellType.Text
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToString(row["MReference"]),
						CellType = BizReportCellType.Text
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToString(row["MCyID"]),
						CellType = BizReportCellType.Text
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToString(row["ExpenseItemName"]),
						CellType = BizReportCellType.Text
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToString(row["MTotalAmtFor"])
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToString(row["MTotalAmt"])
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToString(row["MTotalAmtFor"])
					});
					expenseAmount += Convert.ToDecimal(row["MTotalAmt"]);
					reviewAmount += Convert.ToDecimal(row["MTotalAmtFor"]);
					reportModel.AddRow(bizReportRowModel);
				}
			}
		}

		private DataSet GetExpenseClaimDetailDs(RPTExpenseClaimDeatailFilterModel filter, MContext ctx)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string text = "";
			if (filter.StatisticsFieldOptionIds != null && filter.StatisticsFieldOptionIds.Length != 0)
			{
				string[] statisticsFieldOptionIds = filter.StatisticsFieldOptionIds;
				foreach (string text2 in statisticsFieldOptionIds)
				{
					if (!string.IsNullOrEmpty(text2))
					{
						text = text + "'" + text2 + "',";
					}
				}
				if (!string.IsNullOrEmpty(text))
				{
					text = text.Substring(0, text.Length - 1);
				}
			}
			string text3 = "";
			if (filter.TrackIds != null && filter.TrackIds.Length != 0)
			{
				for (int j = 0; j < filter.TrackIds.Length; j++)
				{
					string text4 = "v.MTrackItem" + (j + 1);
					if (filter.TrackIds[j] != null)
					{
						string text5 = filter.TrackIds[j].Split('-')[1];
						string text6 = text5;
						if (!(text6 == "0") && (text6 == null || text6.Length != 0))
						{
							text3 = ((!(text6 == "1")) ? (text3 + text4 + "= '" + text5 + "' and ") : (text3 + text4 + " is null and "));
						}
					}
				}
				if (!string.IsNullOrEmpty(text3))
				{
					text3 = text3.Substring(0, text3.Length - 4);
				}
			}
			if (filter.StatisticsType == "1")
			{
				stringBuilder.AppendFormat("select v.MID , v.MBizDate , v.DepartmentName , F_GetUserName(v.MFirstName,v.MLastName) as EmployeeName,v.MReference,v.MCyID , v.ExpenseItemName,\r\n        v.MTotalAmt ,v.MTotalAmtFor , v.MType from ( {0} ) v", GetExpenseClaimViewTableSql());
				stringBuilder.Append(" Where v.MBizDate >= @startDate and v.MBizDate <= @endDate and v.MIsDelete = 0 and v.MOrgID = @MOrgID ");
				if (!string.IsNullOrEmpty(text))
				{
					stringBuilder.AppendFormat(" and v.departmentID in ({0})", text);
				}
				if (!string.IsNullOrEmpty(text3))
				{
					stringBuilder.AppendFormat(" and {0}", text3);
				}
				stringBuilder.Append(" Order BY v.departmentID,v.MBizDate");
			}
			if (filter.StatisticsType == "2")
			{
				stringBuilder.AppendFormat("select v.MID , v.MBizDate , v.DepartmentName , F_GetUserName(v.MFirstName,v.MLastName) as EmployeeName,v.MReference,v.MCyID , v.ExpenseItemName,\r\n       v.MTotalAmtFor as MTotalAmtFor ,v.MTotalAmt as MTotalAmt, v.MType from ( {0} ) v", GetExpenseClaimViewTableSql());
				stringBuilder.Append(" Where v.MBizDate >= @startDate and v.MBizDate <= @endDate and v.MIsDelete = 0 and v.MOrgID = @MOrgID ");
				if (!string.IsNullOrEmpty(text))
				{
					if (text == "'other_employeeID'")
					{
						stringBuilder.AppendFormat(" and IFNULL(v.employeeID,'')='' ", text);
					}
					else
					{
						stringBuilder.AppendFormat(" and v.employeeID in ({0})", text);
					}
				}
				if (!string.IsNullOrEmpty(text3))
				{
					stringBuilder.AppendFormat(" and {0}", text3);
				}
				stringBuilder.Append(" Order BY v.employeeID,v.MBizDate");
			}
			if (filter.StatisticsType == "3")
			{
				stringBuilder.AppendFormat("select v.MID , v.MBizDate , v.DepartmentName , F_GetUserName(v.MFirstName,v.MLastName) as EmployeeName,v.MReference,v.MCyID , v.ExpenseItemName,\r\n      v.MTaxAmountFor - v.MTaxAmtFor as MTotalAmtFor ,v.MTaxAmount - v.MTaxAmt as MTotalAmt, v.MType , v.ExpenseParentID from ( {0} ) v", GetExpenseClaimViewTableSql());
				stringBuilder.Append(" Where   v.MOrgID=@MOrgID and v.MBizDate >= @startDate and v.MBizDate <= @endDate and v.MIsDelete = 0 ");
				if (!string.IsNullOrEmpty(text))
				{
					stringBuilder.AppendFormat(" and (v.ExpenseItemID in ({0})  or v.ExpenseParentID in ({0}) )", text);
				}
				if (!string.IsNullOrEmpty(text3))
				{
					stringBuilder.AppendFormat(" and {0}", text3);
				}
				stringBuilder.Append(" order BY v.ExpenseItemID,v.MBizDate");
			}
			MySqlParameter[] array = new MySqlParameter[5]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLCID", MySqlDbType.VarChar, 6),
				new MySqlParameter("@MOrgCyID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@startDate", MySqlDbType.VarChar, 36),
				new MySqlParameter("@endDate", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = ctx.MLCID;
			array[2].Value = BaseCurrencyID;
			array[3].Value = DataStartDate;
			array[4].Value = DataEndDate;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array);
		}

		private BizReportCellLinkModel GetCellLink(MContext ctx, DataRow data)
		{
			BizReportCellLinkModel bizReportCellLinkModel = new BizReportCellLinkModel();
			bizReportCellLinkModel.Text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Viewdetails", "View Details");
			switch (Convert.ToString(data["MType"]))
			{
			case "Expense_Claims":
				bizReportCellLinkModel.Title = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "ViewExpense", "View Expense");
				bizReportCellLinkModel.Url = string.Format("/IV/Expense/ExpenseView/{0}?tabIndex=0", Convert.ToString(data["MID"]));
				break;
			case "Pay_Other":
			case "Pay_OtherReturn":
				bizReportCellLinkModel.Title = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "ViewPayment", "View Payment");
				bizReportCellLinkModel.Url = string.Format("/IV/Payment/PaymentView/{0}?acctid={1}", Convert.ToString(data["MID"]), "");
				break;
			}
			return bizReportCellLinkModel;
		}

		private void SetDataSummaryForDetail(BizReportModel model, RPTExpenseClaimDeatailFilterModel filter, MContext context, decimal expenseAmount, decimal reviewAmount)
		{
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = BizReportRowType.Total;
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Total", "Total"),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = " ",
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = " ",
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = " ",
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = " ",
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = " ",
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = " ",
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(expenseAmount)
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(reviewAmount)
			});
			model.AddRow(bizReportRowModel);
		}
	}
}
