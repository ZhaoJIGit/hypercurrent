using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Helper;
using JieNor.Megi.Core.MultiLanguage;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IO.Export.DataRowModel;
using JieNor.Megi.DataModel.IO.Import.PA;
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace JieNor.Megi.DataRepository.PA
{
	public class PASalaryPaymentRepository
	{
		private static Dictionary<PayrollItemEnum, string> DicTypeFieldMapping
		{
			get
			{
				Dictionary<PayrollItemEnum, string> dictionary = new Dictionary<PayrollItemEnum, string>();
				dictionary.Add(PayrollItemEnum.Allowance, "MShowAllowance");
				dictionary.Add(PayrollItemEnum.Commission, "MShowCommission");
				dictionary.Add(PayrollItemEnum.Bonus, "MShowBonus");
				dictionary.Add(PayrollItemEnum.OverTime, "MShowOverTime");
				dictionary.Add(PayrollItemEnum.TaxAdjustment, "MShowTaxAdjustment");
				dictionary.Add(PayrollItemEnum.Attendance, "MShowAttendance");
				dictionary.Add(PayrollItemEnum.Other, "MShowOther");
				dictionary.Add(PayrollItemEnum.EmployeeSocialSecurity, "MShowEmployeeSocialSecurity");
				dictionary.Add(PayrollItemEnum.EmployeeHousingProvidentFund, "MShowEmployeeHousingProvidentFund");
				dictionary.Add(PayrollItemEnum.RetirementSecurity, "MShowPension");
				dictionary.Add(PayrollItemEnum.MedicalInsurance, "MShowMedicalInsurance");
				dictionary.Add(PayrollItemEnum.UnemploymentInsurance, "MShowUmemploymentInsurance");
				dictionary.Add(PayrollItemEnum.MaternityInsurance, "MShowMeternityInsurance");
				dictionary.Add(PayrollItemEnum.IndustrialInjury, "MShowIndustrialInjury");
				dictionary.Add(PayrollItemEnum.SeriousIllnessMedicalTreatment, "MShowSeriousMedical");
				dictionary.Add(PayrollItemEnum.HousingProvidentFund, "MShowHousingProvidentFund");
				dictionary.Add(PayrollItemEnum.AdditionHousingProvidentFund, "MShowHousingProvidentFundAdition");
				dictionary.Add(PayrollItemEnum.SocialSecurityOther, "MShowSocialSecurityOther");
				return dictionary;
			}
		}

		public static PayrollItemEnum[] IncreaseItemTypes => new PayrollItemEnum[6]
		{
			PayrollItemEnum.Allowance,
			PayrollItemEnum.Bonus,
			PayrollItemEnum.Commission,
			PayrollItemEnum.OverTime,
			PayrollItemEnum.TaxAdjustment,
			PayrollItemEnum.UserAddItem
		};

		public static List<PAPayRunListModel> GetPayRunList(MContext ctx, PAPayRunListFilterModel filter)
		{
			filter.PageSize = 2147483647;
			return GetPayRunListPage(ctx, filter).rows;
		}

		public static DataGridJson<PAPayRunListModel> GetPayRunListPage(MContext ctx, PAPayRunListFilterModel filter)
		{
			SqlQuery sqlQuery = new SqlQuery();
			DateTime dateTime = default(DateTime);
			DateTime dateTime2 = default(DateTime);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(" SELECT a.MID,a.MDate,MIN(b.MStatus) AS MStatus, SUM(b.MTaxSalary) AS PIT,SUM(b.MNetSalary) AS SalaryAfterPIT\r\n                FROM T_PA_PayRun a\r\n                LEFT JOIN T_PA_SalaryPayment b ON a.MID=b.MRunID AND b.MIsDelete=0 \r\n                WHERE a.MOrgID=@MOrgID AND a.MIsDelete=0  ");
			if (filter.Status > 0)
			{
				stringBuilder.AppendLine(" AND a.MStatus = @MStatus ");
			}
			if (!string.IsNullOrEmpty(filter.StartDate))
			{
				DateTime.TryParse(filter.StartDate + "-1", out dateTime);
				stringBuilder.AppendLine(" AND a.MDate >= @StartDate ");
			}
			DateTime dateTime3;
			if (!string.IsNullOrEmpty(filter.EndDate))
			{
				DateTime.TryParse(filter.EndDate + "-1", out dateTime2);
				dateTime3 = dateTime2.AddMonths(1);
				dateTime2 = dateTime3.AddMilliseconds(-1.0);
				stringBuilder.AppendLine(" AND a.MDate <= @EndDate ");
			}
			stringBuilder.AppendLine(" GROUP BY a.MID  ");
			MySqlParameter[] array = new MySqlParameter[4]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MStatus", filter.Status),
				new MySqlParameter("@StartDate", dateTime),
				new MySqlParameter("@EndDate", dateTime2)
			};
			sqlQuery.SqlWhere = filter;
			sqlQuery.SelectString = stringBuilder.ToString();
			sqlQuery.AddOrderBy("MDate", SqlOrderDir.Desc);
			MySqlParameter[] array2 = array;
			foreach (MySqlParameter para in array2)
			{
				sqlQuery.AddParameter(para);
			}
			DataGridJson<PAPayRunListModel> pageDataModelListBySql = ModelInfoManager.GetPageDataModelListBySql<PAPayRunListModel>(ctx, sqlQuery);
			List<PAPayRunListModel> rows = pageDataModelListBySql.rows;
			if (!rows.Any())
			{
				return pageDataModelListBySql;
			}
			List<PASalaryPaymentModel> dataModelList = ModelInfoManager.GetDataModelList<PASalaryPaymentModel>(ctx, new SqlWhere().In("MRunID", (from f in rows
			select f.MID).ToList()), false, false);
			string sql = string.Format(" SELECT a.MID,b.MItemType as ItemType,MAmount as Amount,b.MItemID as MPayItemID,b.MCoefficient FROM T_PA_SalaryPaymentEntry a\r\n                    inner join T_PA_PayItemGroup b on a.MPayItemID=b.MItemID and b.MIsDelete=0 \r\n                    where a.MOrgID=@MOrgID and a.MIsDelete=0 and MID in ('{0}') ", string.Join("','", (from f in dataModelList
			select f.MID).ToList()));
			MySqlParameter[] cmdParms = new MySqlParameter[1]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			List<PAPayItemGroupAmtModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<PAPayItemGroupAmtModel>(ctx, sql, cmdParms);
			List<PAPayItemModel> disableItemList = GetDisableItemList(ctx);
			PayrollItemEnum[] increaseItemTypes = IncreaseItemTypes;
			List<PAPayItemGroupAmtModel> userPayItemList = PAPayItemGroupRepository.GetUserPayItemList(ctx);
			foreach (PAPayRunListModel item in rows)
			{
				PAPayRunListModel pAPayRunListModel = item;
				PAPayRunListModel pAPayRunListModel2 = pAPayRunListModel;
				dateTime3 = pAPayRunListModel.MDate;
				object arg = dateTime3.Year;
				dateTime3 = pAPayRunListModel.MDate;
				pAPayRunListModel2.MFormatDate = $"{arg}-{dateTime3.Month}";
				item.DisableItemList = disableItemList;
				IEnumerable<PASalaryPaymentModel> source = from f in dataModelList
				where f.MRunID == item.MID
				select f;
				IEnumerable<string> runSPMIDList = from f in source
				select f.MID;
				List<PAPayItemGroupAmtModel> amtList = (from f in dataModelBySql
				where runSPMIDList.Contains(f.MID)
				select f).ToList();
				SetListSalaryItemAmount(amtList, item, userPayItemList, increaseItemTypes);
			}
			rows.Sort(filter.Sort, filter.Order);
			pageDataModelListBySql.rows = rows;
			return pageDataModelListBySql;
		}

		public static PASalaryPaymentSummaryModel GetSalaryPaymentSummaryModelByStatus(MContext ctx)
		{
			PASalaryPaymentSummaryModel pASalaryPaymentSummaryModel = new PASalaryPaymentSummaryModel();
			string sql = " SELECT SUM(1) AS AllCount,SUM(CASE WHEN MStatus = 1 THEN 1 ELSE 0 END ) AS DraftCount,\r\n                                SUM(CASE WHEN MStatus = 3 THEN 1 ELSE 0 END ) AS WaitingPaymentCount,SUM(CASE WHEN MStatus = 4 THEN 1 ELSE 0 END) AS PaidCount\r\n                            FROM t_pa_payrun WHERE MOrgID = @MOrgID AND MIsDelete = 0\r\n                            UNION\r\n                            SELECT SUM(ifnull(t1.MNetSalary, 0)) AS AllAmount,\r\n\t                            SUM(CASE WHEN t2.MStatus = 1 THEN ifnull(t1.MNetSalary, 0) ELSE 0 END ) AS DraftAmount,\r\n\t                            SUM(CASE WHEN t2.MStatus = 3 THEN ifnull(t1.MNetSalary, 0) ELSE 0 END ) AS AwaitingAmount,\r\n\t                            SUM(CASE WHEN t2.MStatus = 4 THEN ifnull(t1.MNetSalary, 0) ELSE 0 END ) AS PaidAmount\r\n\t                        FROM t_pa_salarypayment t1 inner join t_pa_payrun t2 on t2.MOrgID=t1.MOrgID and t2.MID=t1.MRunID and t2.MIsDelete=0 \r\n\t\t                        WHERE t1.MOrgID = @MOrgID AND t1.MIsDelete = 0;";
			MySqlParameter[] array = new MySqlParameter[1]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ctx.MOrgID;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataTable dataTable = dynamicDbHelperMySQL.Query(sql, array).Tables[0];
			if (dataTable == null || dataTable.Rows.Count <= 1)
			{
				return pASalaryPaymentSummaryModel;
			}
			DataRow dataRow = dataTable.Rows[0];
			pASalaryPaymentSummaryModel.AllCount = dataRow["AllCount"].ToMInt32();
			pASalaryPaymentSummaryModel.DraftCount = dataRow["DraftCount"].ToMInt32();
			pASalaryPaymentSummaryModel.WaitingPaymentCount = dataRow["WaitingPaymentCount"].ToMInt32();
			pASalaryPaymentSummaryModel.PaidCount = dataRow["PaidCount"].ToMInt32();
			DataRow dataRow2 = dataTable.Rows[1];
			pASalaryPaymentSummaryModel.AllAmount = dataRow2["AllCount"].ToMDecimal();
			pASalaryPaymentSummaryModel.DraftAmount = dataRow2["DraftCount"].ToMDecimal();
			pASalaryPaymentSummaryModel.WaitingPaymentAmount = dataRow2["WaitingPaymentCount"].ToMDecimal();
			pASalaryPaymentSummaryModel.PaidAmount = dataRow2["PaidCount"].ToMDecimal();
			return pASalaryPaymentSummaryModel;
		}

		public static PASalaryPaymentSummaryModel GetSalaryPaymentSummaryModel(MContext ctx, string runId)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("SELECT SUM(1) as AllCount, \r\n\t                    SUM(CASE WHEN a.MStatus = 1 THEN 1 ELSE 0 END) AS DraftCount,SUM(CASE WHEN a.MStatus = 1 THEN ifnull(a.MNetSalary,0)-ifnull(a.MVerificationAmt,0)/1 ELSE 0 END) AS DraftAmount,\r\n\t                    SUM(CASE WHEN a.MStatus = 2 THEN 1 ELSE 0 END) AS WaitingApprovalCount,SUM(CASE WHEN a.MStatus = 2 THEN ifnull(a.MNetSalary,0)-ifnull(a.MVerificationAmt,0)/1 ELSE 0 END) AS WaitingApprovalAmount,\r\n\t                    SUM(CASE WHEN a.MStatus = 3 THEN 1 ELSE 0 END) AS WaitingPaymentCount,SUM(CASE WHEN a.MStatus = 3 THEN ifnull(a.MNetSalary,0)-ifnull(a.MVerificationAmt,0)/1 ELSE 0 END) AS WaitingPaymentAmount,\r\n                        SUM(CASE WHEN a.MStatus = 4 THEN 1 ELSE 0 END) AS PaidCount,SUM(CASE WHEN a.MStatus = 4 THEN ifnull(a.MVerificationAmt,0)/1 ELSE 0 END) AS PaidAmount\r\n                        FROM T_PA_SalaryPayment a\r\n                        left join T_PA_PayRun b on a.MRunID=b.MID and  b.MIsDelete = 0\r\n                        WHERE a.MIsDelete = 0 AND b.MOrgID=@MOrgID and a.MRunID=@MRunID ");
			if (ctx.IsSelfData)
			{
				stringBuilder.Append(" AND MCreatorID=@MCreatorID ");
			}
			MySqlParameter[] array = new MySqlParameter[4]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MCreatorID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MNow", MySqlDbType.DateTime),
				new MySqlParameter("@MRunID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = ctx.MUserID;
			MySqlParameter obj = array[2];
			DateTime dateTime = ctx.DateNow;
			dateTime = dateTime.Date;
			dateTime = dateTime.AddDays(1.0);
			obj.Value = dateTime.AddSeconds(-1.0);
			array[3].Value = runId;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataTable dataTable = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array).Tables[0];
			PASalaryPaymentSummaryModel pASalaryPaymentSummaryModel = new PASalaryPaymentSummaryModel();
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				return pASalaryPaymentSummaryModel;
			}
			DataRow dataRow = dataTable.Rows[0];
			pASalaryPaymentSummaryModel.AllCount = dataRow["AllCount"].ToMInt32();
			pASalaryPaymentSummaryModel.DraftCount = dataRow["DraftCount"].ToMInt32();
			pASalaryPaymentSummaryModel.DraftAmount = dataRow["DraftAmount"].ToMDecimal();
			pASalaryPaymentSummaryModel.WaitingApprovalCount = dataRow["WaitingApprovalCount"].ToMInt32();
			pASalaryPaymentSummaryModel.WaitingApprovalAmount = dataRow["WaitingApprovalAmount"].ToMDecimal();
			pASalaryPaymentSummaryModel.WaitingPaymentCount = dataRow["WaitingPaymentCount"].ToMInt32();
			pASalaryPaymentSummaryModel.WaitingPaymentAmount = dataRow["WaitingPaymentAmount"].ToMDecimal();
			pASalaryPaymentSummaryModel.PaidCount = dataRow["PaidCount"].ToMInt32();
			pASalaryPaymentSummaryModel.PaidAmount = dataRow["PaidAmount"].ToMDecimal();
			pASalaryPaymentSummaryModel.AllAmount = pASalaryPaymentSummaryModel.DraftAmount + pASalaryPaymentSummaryModel.WaitingApprovalAmount + pASalaryPaymentSummaryModel.WaitingPaymentAmount + pASalaryPaymentSummaryModel.PaidAmount;
			return pASalaryPaymentSummaryModel;
		}

		public static List<PASalaryPaymentModel> GetEmployeeSalaryPaymentList(MContext ctx, string employeeId, int status)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("select t1.*, t2.MDate\r\n                                    from T_PA_SalaryPayment t1\r\n                                    join t_pa_payrun t2 on t2.MOrgID=t1.MOrgID and t2.MID=t1.MRunID and t2.MIsActive=1 and t2.MIsDelete=0\r\n                                where t1.MOrgID=@MOrgID and t1.MIsActive=1 and t1.MIsDelete=0 ");
			if (!string.IsNullOrWhiteSpace(employeeId))
			{
				stringBuilder.Append(" and t1.MEmployeeID=@MEmployeeID");
			}
			if (status > 0)
			{
				stringBuilder.Append(" and t1.MStatus=@MStatus");
			}
			MySqlParameter[] cmdParms = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MEmployeeID", employeeId),
				new MySqlParameter("@MStatus", status)
			};
			return ModelInfoManager.GetDataModelBySql<PASalaryPaymentModel>(ctx, stringBuilder.ToString(), cmdParms);
		}

		public static DataGridJson<PASalaryPaymentListModel> GetSalaryPaymentList(MContext ctx, PASalaryPaymentListFilterModel filter)
		{
			List<PASalaryPaymentListModel> list = null;
			DataGridJson<PASalaryPaymentListModel> dataGridJson = null;
			if (!string.IsNullOrWhiteSpace(filter.PayRunID))
			{
				StringBuilder stringBuilder = new StringBuilder();
				string arg = (filter.MStatus > 0) ? " and a.MStatus=@MStatus" : string.Empty;
				stringBuilder.AppendFormat("select a.MID, a.MEmployeeID, a.MIsSent, l.MFirstName, l.MLastName, \r\n                                a.MTaxSalary as PIT, a.MNetSalary as SalaryAfterPIT, a.MVerificationAmt, a.MStatus, a.MSeq\r\n                                from T_PA_SalaryPayment a\r\n                                left join T_BD_Employees_l l on a.MEmployeeID=l.MParentID and l.MLocaleID=@MLocaleID and l.MIsDelete=0\r\n                                where a.MOrgID=@MOrgID and a.MRunID=@MRunID and a.MIsDelete=0 {0}\r\n                                group by a.MID \r\n                                ", arg);
				MySqlParameter[] array = new MySqlParameter[4]
				{
					new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
					new MySqlParameter("@MRunID", MySqlDbType.VarChar, 36),
					new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 6),
					new MySqlParameter("@MStatus", MySqlDbType.Int32)
				};
				array[0].Value = ctx.MOrgID;
				array[1].Value = filter.PayRunID;
				array[2].Value = ctx.MLCID;
				array[3].Value = filter.MStatus;
				SqlQuery sqlQuery = new SqlQuery();
				sqlQuery.SqlWhere = filter;
				sqlQuery.SelectString = stringBuilder.ToString();
				string employeeOrderFields = GetEmployeeOrderFields(ctx, "l");
				sqlQuery.AddOrderBy(employeeOrderFields, SqlOrderDir.Asc);
				MySqlParameter[] array2 = array;
				foreach (MySqlParameter para in array2)
				{
					sqlQuery.AddParameter(para);
				}
				dataGridJson = ModelInfoManager.GetPageDataModelListBySql<PASalaryPaymentListModel>(ctx, sqlQuery);
				list = dataGridJson.rows;
				if (!list.Any())
				{
					return dataGridJson;
				}
				string sql = string.Format("select a.MID,b.MItemType as ItemType,a.MAmount as Amount,b.MItemID as MPayItemID,b.MCoefficient from T_PA_SalaryPaymentEntry a\r\n                        inner join T_PA_PayItemGroup b on a.MPayItemID=b.MItemID and b.MIsDelete=0 \r\n                        where a.MOrgID=@MOrgID and a.MIsDelete=0 and MID in ('{0}')", string.Join("','", (from f in list
				select f.MID).ToList()));
				MySqlParameter[] cmdParms = new MySqlParameter[1]
				{
					new MySqlParameter("@MOrgID", ctx.MOrgID)
				};
				List<PAPayItemGroupAmtModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<PAPayItemGroupAmtModel>(ctx, sql, cmdParms);
				List<PAPayItemModel> disableItemList = GetDisableItemList(ctx);
				List<PAPayItemGroupAmtModel> userPayItemList = PAPayItemGroupRepository.GetUserPayItemList(ctx);
				int num = 1;
				foreach (PASalaryPaymentListModel item in list)
				{
					item.MRowNo = filter.PageSize * (filter.PageIndex - 1) + num;
					item.DisableItemList = disableItemList;
					List<PAPayItemGroupAmtModel> amtList = (from f in dataModelBySql
					where f.MID == item.MID
					select f).ToList();
					SetListSalaryItemAmount(amtList, item, userPayItemList, null);
					item.MEmployeeName = GlobalFormat.GetUserName(item.MFirstName, item.MLastName, ctx);
					num++;
				}
				dataGridJson.rows = list;
			}
			return dataGridJson;
		}

		public static List<PASalaryListModel> GetSalaryListForPrint(MContext ctx, PASalaryListFilterModel filter)
		{
			List<PASalaryListModel> list = new List<PASalaryListModel>();
			if (!string.IsNullOrWhiteSpace(filter.ObjectIds))
			{
				MySqlParameter[] cmdParms = new MySqlParameter[3]
				{
					new MySqlParameter("@MLocaleID1", ctx.MLCID),
					new MySqlParameter("@MLocaleID2", (ctx.MLCID == "0x0009") ? "0x7804" : "0x0009"),
					new MySqlParameter("@MOrgID", ctx.MOrgID)
				};
				StringBuilder stringBuilder = new StringBuilder();
				string arg = string.Join("','", filter.ObjectIds.Split(','));
				stringBuilder.AppendFormat("select a.MID,b.MEmployeeID, b.MJoinTime as JoinTime, b.MIDNumber as IDNumber, \r\n                                (case @MLocaleID1 when '0x0009' then concat(l1.MFirstName, ' ', l1.MLastName) else concat(l1.MLastName, l1.MFirstName) end) as EmployeeName1,\r\n                                (case @MLocaleID2 when '0x0009' then concat(l2.MFirstName, ' ', l2.MLastName) else concat(l2.MLastName, l2.MFirstName) end) as EmployeeName2,\r\n                                a.MTaxSalary as PITAmount, a.MNetSalary as NetSalaryAmount, (a.MNetSalary+a.MTaxSalary) as SalaryBeforePITAmount,b.MSocialSecurityBase,b.MHosingProvidentFundBase\r\n                                from T_PA_SalaryPayment a\r\n                                left join t_bd_emppayrollbasicset b on a.MEmployeeID=b.MEmployeeID and b.MIsDelete=0\r\n                                left join T_BD_Employees_l l1 on a.MEmployeeID=l1.MParentID and l1.MLocaleID=@MLocaleID1 and l1.MIsDelete=0 \r\n                                left join T_BD_Employees_l l2 on a.MEmployeeID=l2.MParentID and l2.MLocaleID=@MLocaleID2 and l2.MIsDelete=0\r\n                                where a.MOrgID=@MOrgID and a.MIsDelete=0 and a.MID in ('{0}') \r\n                                group by a.MID\r\n                                order by a.MSeq", arg);
				list = ModelInfoManager.GetDataModelBySql<PASalaryListModel>(ctx, stringBuilder.ToString(), cmdParms);
				string sql = string.Format("select b.MID,a.MItemID as MPayItemID,a.MItemType as ItemType,b.MAmount as Amount,b.MDesc,a.MCoefficient,b1.MName,a.MCreateDate from T_PA_PayItemGroup a\r\n                            left join T_PA_SalaryPaymentEntry b on b.MOrgID=a.MOrgID and b.MPayItemID=a.MItemID and b.MIsDelete=0 and b.MID in ('{0}')\r\n                            left join T_PA_PayItemGroup_l b1 on b1.MOrgID=a.MOrgID and b1.MParentID=a.MItemID and b1.MLocaleID=@MLocaleID and b1.MIsDelete=0 \r\n                            where a.MOrgID=@MOrgID and a.MIsDelete=0 \r\n                        union all\r\n                            select a.MID,b.MItemID as MPayItemID,b.MItemType as ItemType,a.MAmount as Amount,a.MDesc,b.MCoefficient,b1.MName,b.MCreateDate from T_PA_SalaryPaymentEntry a\r\n                            inner join T_PA_PayItem b on a.MPayItemID=b.MItemID and b.MIsDelete=0 \r\n                            left join T_PA_PayItem_l b1 on b.MItemID=b1.MParentID and MLocaleID=@MLocaleID and b1.MIsDelete=0 \r\n                            where a.MOrgID=@MOrgID and a.MIsDelete=0 and MID in ('{0}')", arg);
				MySqlParameter[] cmdParms2 = new MySqlParameter[2]
				{
					new MySqlParameter("@MOrgID", ctx.MOrgID),
					new MySqlParameter("@MLocaleID", ctx.MLCID)
				};
				List<PAPayItemGroupAmtModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<PAPayItemGroupAmtModel>(ctx, sql, cmdParms2);
				List<BDPayrollDetailModel> dataModelList = ModelInfoManager.GetDataModelList<BDPayrollDetailModel>(ctx, new SqlWhere().In("MEmployeeID", (from f in list
				select f.MEmployeeID).ToList()), false, false);
				PAPaySettingModel paySettingModel = GetPaySettingModel(ctx);
				List<NameValueModel> multLangGroupNameList = PAPayItemGroupRepository.GetMultLangGroupNameList(ctx);
				foreach (PASalaryListModel item in list)
				{
					PASalaryListModel pASalaryListModel = item;
					pASalaryListModel.PITAmount *= decimal.MinusOne;
					List<PAPayItemGroupAmtModel> list2 = (from f in dataModelBySql
					where f.MID == item.MID || f.ItemType == 1023 || f.ItemType == 1067
					select f).ToList();
					if (item.SalaryItemModels == null)
					{
						item.SalaryItemModels = new List<SalaryItemModel>();
					}
					if (item.SSAndHFModels == null)
					{
						item.SSAndHFModels = new List<SSAndHFModel>();
					}
					SetSalaryListTitle(ctx, item, filter.PrintSettingModel);
					PASalaryListModel pASalaryListModel2 = item;
					DateTime salaryMonth;
					object period;
					if (!(ctx.MLCID == "0x0009"))
					{
						salaryMonth = filter.SalaryMonth;
						period = salaryMonth.ToString("yyyy年MM月");
					}
					else
					{
						salaryMonth = filter.SalaryMonth;
						period = salaryMonth.ToString("yyyy-MM");
					}
					pASalaryListModel2.Period = (string)period;
					PAPayItemGroupAmtModel pAPayItemGroupAmtModel = list2.FirstOrDefault((PAPayItemGroupAmtModel f) => f.ItemType == 1000);
					if (pAPayItemGroupAmtModel != null)
					{
						item.BaseSalaryAmount = pAPayItemGroupAmtModel.Amount;
					}
					PAPayItemGroupAmtModel pAPayItemGroupAmtModel2 = list2.FirstOrDefault((PAPayItemGroupAmtModel f) => f.ItemType == 2015);
					PAPayItemGroupAmtModel pAPayItemGroupAmtModel3 = list2.FirstOrDefault((PAPayItemGroupAmtModel f) => f.ItemType == 1035);
					item.SSAndHFModels.Add(new SSAndHFModel
					{
						Name = string.Format("  {0}", COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "SocialSecurity", "社保")),
						EmployerAmount = (pAPayItemGroupAmtModel2?.Amount ?? decimal.Zero),
						EmployeeAmount = (pAPayItemGroupAmtModel3?.Amount ?? decimal.Zero),
						Type = 4000
					});
					item.TotalSalaryAmount = item.SalaryBeforePITAmount + (from f in list2
					where f.ItemType == 1035 || f.ItemType == 1055 || f.ItemType == 2015 || f.ItemType == 2000
					select f).Sum((PAPayItemGroupAmtModel f) => f.Amount);
					AddPayItemByPrintSetting(ctx, filter, item, list2, multLangGroupNameList);
					bool flag = false;
					foreach (PAPayItemGroupAmtModel item2 in list2)
					{
						PayrollItemEnum itemType = (PayrollItemEnum)item2.ItemType;
						switch (itemType)
						{
						case PayrollItemEnum.PBasicRetirementSecurity:
						case PayrollItemEnum.PBasicMedicalInsurance:
						case PayrollItemEnum.PBasicUnemploymentInsurance:
						case PayrollItemEnum.PHousingProvidentFund:
						case PayrollItemEnum.PAdditionHousingProvidentFund:
							AddSSAndHFModel(ctx, itemType, item2.MName, item2.Amount, item, filter.PrintSettingModel);
							break;
						case PayrollItemEnum.CHousingProvidentFund:
						case PayrollItemEnum.CAdditionHousingProvidentFund:
						case PayrollItemEnum.CBasicRetirementSecurity:
						case PayrollItemEnum.CBasicMedicalInsurance:
						case PayrollItemEnum.CMaternityInsurance:
						case PayrollItemEnum.CBasicUnemploymentInsurance:
						case PayrollItemEnum.CIndustrialInjury:
						case PayrollItemEnum.CSeriousIllnessMedicalTreatment:
						case PayrollItemEnum.CSocialSecurityOther:
							flag = true;
							AddSSAndHFModel(ctx, itemType, item2.MName, item2.Amount, item, filter.PrintSettingModel);
							break;
						}
					}
					PAPayItemGroupAmtModel pAPayItemGroupAmtModel4 = list2.FirstOrDefault((PAPayItemGroupAmtModel f) => f.ItemType == 2015);
					PAPayItemGroupAmtModel pAPayItemGroupAmtModel5 = list2.FirstOrDefault((PAPayItemGroupAmtModel f) => f.ItemType == 2000);
					bool flag2 = pAPayItemGroupAmtModel4 != null && pAPayItemGroupAmtModel4.Amount > decimal.Zero;
					bool flag3 = pAPayItemGroupAmtModel5 != null && pAPayItemGroupAmtModel5.Amount > decimal.Zero;
					if (!flag && (flag2 | flag3))
					{
						AddEmployerItem(ctx, paySettingModel, dataModelList.FirstOrDefault((BDPayrollDetailModel f) => f.MEmployeeID == item.MEmployeeID), item, filter.PrintSettingModel, flag2, flag3);
					}
					item.SalaryItemModels = (from f in item.SalaryItemModels
					orderby f.Type
					select f).ToList();
					item.SSAndHFModels = (from f in item.SSAndHFModels
					orderby f.Type
					select f).ToList();
				}
			}
			return list;
		}

		private static bool IsUserItem(int itemType)
		{
			return itemType == 1023 || itemType == 1067;
		}

		private static void AddPayItemByPrintSetting(MContext ctx, PASalaryListFilterModel filter, PASalaryListModel salary, List<PAPayItemGroupAmtModel> entryList, List<NameValueModel> multLangGroupNameList)
		{
			foreach (PAPrintSettingEntryModel mEntry in filter.PrintSettingModel.MEntryList)
			{
				if (mEntry.MIsShow)
				{
					PAPayItemGroupAmtModel foundEntry = entryList.FirstOrDefault((PAPayItemGroupAmtModel f) => f.MPayItemID == mEntry.MPayItemID && (f.MID == salary.MID || (string.IsNullOrWhiteSpace(f.MID) && IsUserItem(f.ItemType))));
					NameValueModel multLangGroupNameModel = multLangGroupNameList.FirstOrDefault((NameValueModel f) => f.MName == mEntry.MPayItemType.ToString() && f.MTag == ctx.MLCID);
					SalaryItemModel showPayItem = GetShowPayItem(ctx, salary, (PayrollItemEnum)mEntry.MPayItemType, foundEntry, multLangGroupNameModel);
					salary.SalaryItemModels.Add(showPayItem);
				}
			}
			List<PayrollItemEnum> list = new List<PayrollItemEnum>
			{
				PayrollItemEnum.EmployeeSocialSecurity,
				PayrollItemEnum.EmployeeHousingProvidentFund
			};
			foreach (PayrollItemEnum item in list)
			{
				if (Convert.ToBoolean(ModelHelper.GetModelValue(filter.PrintSettingModel, DicTypeFieldMapping[item])))
				{
					PAPayItemGroupAmtModel foundEntry = entryList.FirstOrDefault((PAPayItemGroupAmtModel f) => f.ItemType == (int)item);
					salary.SalaryItemModels.Add(GetShowPayItem(ctx, salary, item, foundEntry, null));
				}
			}
			salary.SalaryItemModels = (from item in salary.SalaryItemModels
			where item.Type != 1000
			orderby item.Type, item.MCreateDate
			select item).ToList();
		}

		private static SalaryItemModel GetShowPayItem(MContext ctx, PASalaryListModel salary, PayrollItemEnum item, PAPayItemGroupAmtModel foundEntry, NameValueModel multLangGroupNameModel)
		{
			SalaryItemModel salaryItemModel = new SalaryItemModel
			{
				Type = (int)item,
				Name = ((multLangGroupNameModel != null) ? multLangGroupNameModel.MValue : GetSalaryLabel(ctx, item))
			};
			if (foundEntry != null)
			{
				salaryItemModel.Name = foundEntry.MName;
				salaryItemModel.MAmount = foundEntry.Amount * (decimal)foundEntry.MCoefficient;
				salaryItemModel.MDesc = foundEntry.MDesc;
				salaryItemModel.MCreateDate = foundEntry.MCreateDate;
			}
			return salaryItemModel;
		}

		private static void AddEmployerItem(MContext ctx, PAPaySettingModel setting, BDPayrollDetailModel empSetting, PASalaryListModel model, PAPrintSettingModel printSettingModel, bool hasSS, bool hasHF)
		{
			decimal d = empSetting.MSocialSecurityBase / 100m;
			decimal d2 = empSetting.MHosingProvidentFundBase / 100m;
			Dictionary<PayrollItemEnum, decimal> dictionary = new Dictionary<PayrollItemEnum, decimal>();
			if (hasSS)
			{
				dictionary.Add(PayrollItemEnum.CBasicRetirementSecurity, d * setting.MRetirementSecurityPer);
				dictionary.Add(PayrollItemEnum.CBasicMedicalInsurance, d * setting.MMedicalInsurancePer);
				dictionary.Add(PayrollItemEnum.CBasicUnemploymentInsurance, d * setting.MUmemploymentInsurancePer);
				dictionary.Add(PayrollItemEnum.CMaternityInsurance, d * setting.MMaternityInsurancePer);
				dictionary.Add(PayrollItemEnum.CIndustrialInjury, d * setting.MIndustrialInjuryPer);
				dictionary.Add(PayrollItemEnum.CSeriousIllnessMedicalTreatment, d * setting.MSeriousIiinessInjuryPer);
				dictionary.Add(PayrollItemEnum.CSocialSecurityOther, d * setting.MOtherPer);
			}
			if (hasHF)
			{
				dictionary.Add(PayrollItemEnum.CHousingProvidentFund, d2 * setting.MProvidentFundPer);
				dictionary.Add(PayrollItemEnum.CAdditionHousingProvidentFund, d2 * setting.MAddProvidentFundPer);
			}
			foreach (PayrollItemEnum key in dictionary.Keys)
			{
				AddSSAndHFModel(ctx, key, GetSalaryLabel(ctx, key), dictionary[key], model, printSettingModel);
			}
		}

		private static void AddSSAndHFModel(MContext ctx, PayrollItemEnum itemEnum, string name, decimal amount, PASalaryListModel model, PAPrintSettingModel printSettingModel)
		{
			PayrollItemEnum mappingEnum = PayrollItemEnum.None;
			switch (itemEnum)
			{
			case PayrollItemEnum.PBasicRetirementSecurity:
			case PayrollItemEnum.CBasicRetirementSecurity:
				mappingEnum = PayrollItemEnum.RetirementSecurity;
				break;
			case PayrollItemEnum.PBasicMedicalInsurance:
			case PayrollItemEnum.CBasicMedicalInsurance:
				mappingEnum = PayrollItemEnum.MedicalInsurance;
				break;
			case PayrollItemEnum.PBasicUnemploymentInsurance:
			case PayrollItemEnum.CBasicUnemploymentInsurance:
				mappingEnum = PayrollItemEnum.UnemploymentInsurance;
				break;
			case PayrollItemEnum.CMaternityInsurance:
				mappingEnum = PayrollItemEnum.MaternityInsurance;
				break;
			case PayrollItemEnum.CIndustrialInjury:
				mappingEnum = PayrollItemEnum.IndustrialInjury;
				break;
			case PayrollItemEnum.CSeriousIllnessMedicalTreatment:
				mappingEnum = PayrollItemEnum.SeriousIllnessMedicalTreatment;
				break;
			case PayrollItemEnum.PHousingProvidentFund:
			case PayrollItemEnum.CHousingProvidentFund:
				mappingEnum = PayrollItemEnum.HousingProvidentFund;
				break;
			case PayrollItemEnum.PAdditionHousingProvidentFund:
			case PayrollItemEnum.CAdditionHousingProvidentFund:
				mappingEnum = PayrollItemEnum.AdditionHousingProvidentFund;
				break;
			case PayrollItemEnum.CSocialSecurityOther:
				mappingEnum = PayrollItemEnum.SocialSecurityOther;
				break;
			}
			if (Convert.ToBoolean(ModelHelper.GetModelValue(printSettingModel, DicTypeFieldMapping[mappingEnum])))
			{
				SSAndHFModel sSAndHFModel = model.SSAndHFModels.FirstOrDefault((SSAndHFModel f) => f.Type == (int)mappingEnum);
				bool flag = sSAndHFModel == null;
				if (flag)
				{
					sSAndHFModel = new SSAndHFModel();
					string format = (mappingEnum == PayrollItemEnum.HousingProvidentFund) ? "  {0}" : "    {0}";
					sSAndHFModel.Name = string.Format(format, string.IsNullOrWhiteSpace(name) ? GetSalaryLabel(ctx, itemEnum) : name);
					sSAndHFModel.Type = (int)mappingEnum;
				}
				if (itemEnum.ToString().StartsWith("P"))
				{
					sSAndHFModel.EmployeeAmount = amount;
				}
				else
				{
					sSAndHFModel.EmployerAmount = amount;
				}
				if (flag)
				{
					model.SSAndHFModels.Add(sSAndHFModel);
				}
			}
		}

		private static void SetSalaryListTitle(MContext ctx, PASalaryListModel model, PAPrintSettingModel printSetting)
		{
			model.Title = printSetting.MTitle;
			model.AdditionalInfoTitle = printSetting.MAdditionalInfo;
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "EnglishName", "英文名:");
			string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "ChineseName", "中文名:");
			if (ctx.MLCID == "0x0009")
			{
				model.EmployeeNameTitle1 = text;
				model.EmployeeNameTitle2 = text2;
			}
			else
			{
				model.EmployeeNameTitle1 = text2;
				model.EmployeeNameTitle2 = text;
			}
			model.JoinTimeTitle = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "JoinTime", "Join Time:");
			model.IDNumberTitle = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "IDNumber", "ID Number:");
			model.BasicSalaryTitle = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "BaseSalary", "Base Salary");
			model.SalaryBeforePITTitle = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "SalaryBeforeTax", "Total Salary before Tax");
			model.PITTitle = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "PIT", "P-I-T");
			model.NetSalaryTitle = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "NetSalary", "Net Salary");
			model.ItemTitle = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "Item", "项目");
			model.PersonTitle = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "Person", "个人");
			model.EmployerTitle = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "Employer", "公司");
			model.TotalSalaryTitle = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "SalaryTotal", "工资合计");
		}

		private static string GetSalaryLabel(MContext ctx, PayrollItemEnum itemType)
		{
			string key = string.Empty;
			LangModule module = LangModule.PA;
			switch (itemType)
			{
			case PayrollItemEnum.PBasicRetirementSecurity:
			case PayrollItemEnum.CBasicRetirementSecurity:
				module = LangModule.BD;
				key = "BaiscRetirementSecurity";
				break;
			case PayrollItemEnum.PBasicMedicalInsurance:
			case PayrollItemEnum.CBasicMedicalInsurance:
				module = LangModule.BD;
				key = "BasicMedicalInsurance";
				break;
			case PayrollItemEnum.PBasicUnemploymentInsurance:
			case PayrollItemEnum.CBasicUnemploymentInsurance:
				module = LangModule.BD;
				key = "UmemploymentInsurance";
				break;
			case PayrollItemEnum.CMaternityInsurance:
				module = LangModule.BD;
				key = "MeternityInsurance";
				break;
			case PayrollItemEnum.CIndustrialInjury:
				module = LangModule.BD;
				key = "IndustrialInjury";
				break;
			case PayrollItemEnum.CSeriousIllnessMedicalTreatment:
				module = LangModule.BD;
				key = "SeriousIlinessMedicalTreatment";
				break;
			case PayrollItemEnum.CSocialSecurityOther:
				module = LangModule.BD;
				key = "Other";
				break;
			case PayrollItemEnum.Allowance:
			case PayrollItemEnum.Bonus:
			case PayrollItemEnum.Commission:
			case PayrollItemEnum.OverTime:
			case PayrollItemEnum.TaxAdjustment:
			case PayrollItemEnum.Attendance:
			case PayrollItemEnum.Other:
				key = itemType.ToString();
				break;
			case PayrollItemEnum.EmployeeSocialSecurity:
				module = LangModule.Contact;
				key = "SocialSecurityEmployer";
				break;
			case PayrollItemEnum.EmployeeHousingProvidentFund:
				module = LangModule.Contact;
				key = "HousingProvidentFundEmployee";
				break;
			case PayrollItemEnum.PHousingProvidentFund:
			case PayrollItemEnum.CHousingProvidentFund:
				module = LangModule.PA;
				key = "HousingProvidentFund";
				break;
			case PayrollItemEnum.PAdditionHousingProvidentFund:
			case PayrollItemEnum.CAdditionHousingProvidentFund:
				module = LangModule.PA;
				key = "HousingProvidentFundAddition";
				break;
			}
			return COMMultiLangRepository.GetText(module, ctx.MLCID, key);
		}

		public static OperationResult PayRunNew(MContext ctx, string yearMonth)
		{
			OperationResult operationResult = new OperationResult();
			string empty = string.Empty;
			try
			{
				DateTime salaryDate = GetSalaryDate(yearMonth);
				operationResult = CreatePayRun(ctx, salaryDate, null);
				if (!operationResult.Success)
				{
					return operationResult;
				}
				empty = operationResult.ObjectID;
				operationResult = CreateSalaryPayment(ctx, operationResult.ObjectID, salaryDate, null);
				operationResult.ObjectID = empty;
			}
			catch (Exception ex)
			{
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = ex.Message
				});
			}
			return operationResult;
		}

		public static OperationResult PayRunCopy(MContext ctx, string yearMonth)
		{
			OperationResult operationResult = new OperationResult();
			string empty = string.Empty;
			string empty2 = string.Empty;
			List<CommandInfo> list = new List<CommandInfo>();
			try
			{
				DateTime salaryDate = GetSalaryDate(yearMonth);
				PAPayRunModel payRunModel = GetPayRunModel(ctx, salaryDate, true);
				if (payRunModel == null)
				{
					operationResult.VerificationInfor.Add(new BizVerificationInfor
					{
						Level = AlertEnum.Error,
						Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "NoPayRunExist", "No Pay Run exist.")
					});
					return operationResult;
				}
				empty2 = payRunModel.MID;
				list.AddRange(GetCreatePayRunCmdList(ctx, salaryDate, payRunModel));
				empty = payRunModel.MID;
				list.AddRange(GetCopySalaryPaymentCmdList(ctx, empty, empty2, salaryDate));
				DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
				operationResult.Success = (dynamicDbHelperMySQL.ExecuteSqlTran(list) > 0);
				operationResult.ObjectID = empty;
			}
			catch (Exception ex)
			{
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = ex.Message
				});
			}
			return operationResult;
		}

		public static OperationResult PayRunUpdate(MContext ctx, PAPayRunModel model, List<string> updateFields = null)
		{
			OperationResult operationResult = new OperationResult();
			try
			{
				SqlWhere sqlWhere = new SqlWhere();
				sqlWhere.Equal("MRunID", model.MID);
				List<PASalaryPaymentModel> dataModelList = ModelInfoManager.GetDataModelList<PASalaryPaymentModel>(ctx, sqlWhere, false, false);
				DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
				List<CommandInfo> list = new List<CommandInfo>();
				MySqlParameter[] source = new MySqlParameter[3]
				{
					new MySqlParameter("@MOrgID", ctx.MOrgID),
					new MySqlParameter("@MStatus", model.MStatus),
					new MySqlParameter("@MRunID", model.MID)
				};
				List<MySqlParameter> list2 = source.ToList();
				List<string> list3 = new List<string>();
				string[] arrIds = model.MSelectIds.Split(',');
				int i;
				for (i = 0; i < arrIds.Length; i++)
				{
					list3.Add("@MID" + i);
					list2.Add(new MySqlParameter("@MID" + i, arrIds[i]));
					dataModelList.Remove(dataModelList.FirstOrDefault((PASalaryPaymentModel m) => m.MID == arrIds[i]));
				}
				list.Add(new CommandInfo(string.Format("update t_pa_salarypayment set MStatus=@MStatus where MOrgID=@MOrgID and MRunID=@MRunID and MID in ({0})", string.Join(",", list3)), list2.ToArray()));
				if (!dataModelList.Any((PASalaryPaymentModel m) => m.MStatus < model.MStatus))
				{
					list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<PAPayRunModel>(ctx, model, updateFields, true));
				}
				dynamicDbHelperMySQL.ExecuteSqlTran(list);
			}
			catch (Exception ex)
			{
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = ex.Message
				});
			}
			return operationResult;
		}

		public static string GetChartStackedDictionary(MContext ctx, string payRunListData = null)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			double num = 0.0;
			List<PAPayRunListModel> list = null;
			JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
			list = (string.IsNullOrWhiteSpace(payRunListData) ? GetPayRunList(ctx, new PAPayRunListFilterModel()) : javaScriptSerializer.Deserialize<List<PAPayRunListModel>>(payRunListData));
			DateTime dateNow = ctx.DateNow;
			DateTime dateTime = new DateTime(dateNow.Year, dateNow.Month, 1);
			List<PAPayRunListModel> list2 = new List<PAPayRunListModel>();
			for (int num2 = 6; num2 > 0; num2--)
			{
				DateTime tmpDate = dateTime.AddMonths(-num2);
				PAPayRunListModel pAPayRunListModel = list.FirstOrDefault((PAPayRunListModel f) => f.MDate == tmpDate);
				list2.Add(pAPayRunListModel ?? new PAPayRunListModel
				{
					MDate = tmpDate
				});
			}
			dictionary["data"] = GetPayRunChartList(ctx, list2);
			List<DateTime> monthList = (from f in list2
			select f.MDate).ToList();
			dictionary["labels"] = GetLables(monthList);
			dictionary["scalSpace"] = Math.Ceiling(num / 30.0) * 10.0;
			return javaScriptSerializer.Serialize(dictionary);
		}

		public static List<ChartPie2DModel> GetChartPieDictionary(MContext ctx, DateTime startDate, DateTime endDate)
		{
			List<PAPayRunListModel> payRunList = GetPayRunList(ctx, new PAPayRunListFilterModel());
			List<ChartPie2DModel> list = new List<ChartPie2DModel>();
			DateTime dateNow = ctx.DateNow;
			DateTime endDt = new DateTime(dateNow.Year, dateNow.Month, 1);
			payRunList = (from f in payRunList
			where f.MDate >= endDt.AddMonths(-6) && f.MDate < endDt
			select f).ToList();
			if (payRunList.Any())
			{
				foreach (PAPayRunListModel item in payRunList)
				{
					ChartPie2DModel chartPie2DModel = new ChartPie2DModel();
					chartPie2DModel.name = item.MDate.ToString("yyyy-MM");
					chartPie2DModel.value = item.TotalSalary;
					chartPie2DModel.MContactID = item.MID;
					list.Add(chartPie2DModel);
				}
			}
			return list;
		}

		public static OperationResult SalaryPaymentListUpdate(MContext ctx, string runId, string employeeIds)
		{
			PAPayRunModel dataEditModel = ModelInfoManager.GetDataEditModel<PAPayRunModel>(ctx, runId, false, true);
			return CreateSalaryPayment(ctx, runId, dataEditModel.MDate, employeeIds);
		}

		public static List<PAEmployeesListModel> GetUnPayEmployeeList(MContext ctx, string runId)
		{
			string employeeOrderFields = GetEmployeeOrderFields(ctx, "l");
			string sql = $"select a.MItemID,l.MLastName,l.MFirstName\r\n                            from T_BD_Employees a \r\n                            left join T_BD_EmpPayrollBasicSet b on a.MItemID=b.MEmployeeID and b.MIsDelete=0\r\n                            left join T_BD_Employees_l l on a.MItemID=l.MParentID and l.MLocaleID=@MLocaleID and l.MIsDelete=0\r\n                            where a.MIsActive=1 and a.MIsDelete=0 AND ifnull(a.MStatus, '')<>'Leave' AND a.MOrgID=@MOrgID \r\n                            AND a.MItemID not in (select MEmployeeID from T_PA_SalaryPayment where MRunID=@MRunID and MOrgID=@MOrgID and MIsDelete=0)\r\n                            AND (b.MJoinTime is null OR b.MJoinTime<(select DATE_ADD(MDate,INTERVAL 1 MONTH) from T_PA_PayRun where MID=@MRunID and MOrgID=@MOrgID and MIsDelete=0))\r\n                            group by MItemID\r\n                            order by {employeeOrderFields}";
			MySqlParameter[] array = new MySqlParameter[3]
			{
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 6),
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MRunID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ctx.MLCID;
			array[1].Value = ctx.MOrgID;
			array[2].Value = runId;
			List<PAEmployeesListModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<PAEmployeesListModel>(ctx, sql, array);
			foreach (PAEmployeesListModel item in dataModelBySql)
			{
				item.MName = GlobalFormat.GetUserName(item.MFirstName, item.MLastName, ctx);
			}
			return dataModelBySql;
		}

		public static OperationResult SalaryPaymentDelete(MContext ctx, string salaryPaymentIds)
		{
			OperationResult operationResult = new OperationResult();
			try
			{
				DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
				List<CommandInfo> list = new List<CommandInfo>();
				string[] array = salaryPaymentIds.Split(',');
				string sql = "select * from T_PA_SalaryPayment where MRunID = (select MRunID from T_PA_SalaryPayment where MID=@MID) and MOrgID=@MOrgID and MIsDelete=0";
				MySqlParameter[] cmdParms = new MySqlParameter[2]
				{
					new MySqlParameter("@MOrgID", ctx.MOrgID),
					new MySqlParameter("@MID", array[0])
				};
				List<PASalaryPaymentModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<PASalaryPaymentModel>(ctx, sql, cmdParms);
				if (dataModelBySql.Count == array.Length)
				{
					list.AddRange(ModelInfoManager.GetDeleteCmd<PAPayRunModel>(ctx, dataModelBySql[0].MRunID));
				}
				List<PASalaryPaymentEntryModel> dataModelList = ModelInfoManager.GetDataModelList<PASalaryPaymentEntryModel>(ctx, new SqlWhere().In("MID", array), false, false);
				if (dataModelList.Any())
				{
					list.AddRange(ModelInfoManager.GetDeleteCmd<PASalaryPaymentEntryModel>(ctx, (from f in dataModelList
					select f.MEntryID).ToList()));
				}
				list.AddRange(ModelInfoManager.GetDeleteCmd<PASalaryPaymentModel>(ctx, array.ToList()));
				dynamicDbHelperMySQL.ExecuteSqlTran(list);
			}
			catch (Exception ex)
			{
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = ex.Message
				});
			}
			return operationResult;
		}

		public static OperationResult ValidatePayRunAction(MContext ctx, string yearMonth, PayRunSourceEnum source)
		{
			OperationResult operationResult = new OperationResult();
			try
			{
				DateTime salaryDate = GetSalaryDate(yearMonth);
				if ((uint)(source - 1) <= 1u)
				{
					CheckEmployeeExist(ctx, salaryDate);
				}
				string arg = string.Empty;
				bool flag = IsPayRunExist(ctx, yearMonth, source, ref arg);
				if (source == PayRunSourceEnum.Copy)
				{
					if (flag)
					{
						flag = IsPayRunExist(ctx, yearMonth, PayRunSourceEnum.New);
						if (flag)
						{
							arg = yearMonth;
						}
					}
					else
					{
						operationResult.VerificationInfor.Add(new BizVerificationInfor
						{
							Level = AlertEnum.Error,
							Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "NoHistoryPayRunToCopy", "Not exist history Pay Run to copy!")
						});
					}
				}
				if (flag)
				{
					operationResult.Tag = "Exist";
					string message = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "ExistPayRun", "The Pay Run:{0} already exist, want to overwrite?"), arg);
					operationResult.VerificationInfor.Add(new BizVerificationInfor
					{
						Level = AlertEnum.Error,
						Message = message
					});
				}
				if (salaryDate < new DateTime(2011, 9, 1))
				{
					operationResult.Tag = "AlertAndContinue";
					string message2 = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "SalaryDateConfirm", "系统不支持2011年9月之前月份的个税计算，个税将会按照起征点3500的准则进行计算 ，如有需要请手动进行修改。"), arg);
					operationResult.VerificationInfor.Add(new BizVerificationInfor
					{
						Level = AlertEnum.Error,
						Message = message2
					});
				}
			}
			catch (Exception ex)
			{
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = ex.Message
				});
			}
			return operationResult;
		}

		private static string GetEmployeeOrderFields(MContext ctx, string aliasName)
		{
			return string.Format("convert(F_GetUserName({0}.MFirstName,{0}.MLastName) using gbk)", aliasName);
		}

		public static ImportSalaryModel GetEmployeeSalaryList(MContext ctx, List<PAPayItemModel> disabledItems, DateTime period)
		{
			ImportSalaryModel importSalaryModel = new ImportSalaryModel();
			string sql = "select * from t_pa_paysetting where MOrgID=@MOrgID and MIsDelete=0";
			MySqlParameter[] cmdParms = new MySqlParameter[1]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			importSalaryModel.PaySetting = ModelInfoManager.GetDataModel<ImportPaySettingModel>(ctx, sql, cmdParms);
			importSalaryModel.PaySetting.MEmpProvidentFundPer = importSalaryModel.PaySetting.MProvidentFundPer;
			importSalaryModel.PaySetting.MEmpAddProvidentFundPer = importSalaryModel.PaySetting.MAddProvidentFundPer;
			string employeeOrderFields = GetEmployeeOrderFields(ctx, "t2");
			string sql2 = $"select t1.MItemID as MEmployeeID,t2.MFirstName,t2.MLastName,t5.MRetirementSecurityPercentage,t5.MMedicalInsurancePercentage,t5.MUmemploymentPercentage,t5.MProvidentPercentage,t5.MProvidentAdditionalPercentage\r\n                                    ,t5.MBaseSalary,t5.MSocialSecurityBase,t5.MHosingProvidentFundBase,t5.MRetirementSecurityAmount,t5.MMedicalInsuranceAmount,t5.MUmemploymentAmount,t5.MProvidentAmount,t5.MProvidentAdditionalAmount\r\n                                    ,(case ifnull(t5.MSocialSecurityBase, 0) when 0 then t5.MBaseSalary else t5.MSocialSecurityBase end) as MSocialSecurityBase  \r\n                                    ,(case ifnull(t5.MHosingProvidentFundBase, 0) when 0 then t5.MBaseSalary else t5.MHosingProvidentFundBase end) as MHosingProvidentFundBase   \r\n                                from T_BD_Employees t1 \r\n                                left join T_BD_Employees_l t2 ON t2.MParentID=t1.MItemID AND t2.MLocaleID=@MLocaleID and t2.MIsDelete=0\r\n                                left join t_bd_emppayrollbasicset t5 on t1.MItemID=t5.MEmployeeID and t5.MIsDelete=0\r\n                                where t1.MIsActive=1 and t1.MIsDelete=0 and t1.MOrgID=@MOrgID AND ifnull(t1.MStatus, '')<>'Leave' order by {employeeOrderFields}";
			MySqlParameter[] cmdParms2 = new MySqlParameter[2]
			{
				new MySqlParameter("@MLocaleID", ctx.MLCID),
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			importSalaryModel.SalaryList = ModelInfoManager.GetDataModelBySql<ImportSalaryListModel>(ctx, sql2, cmdParms2);
			PAPITCalculateUtility pAPITCalculateUtility = new PAPITCalculateUtility(ctx, period, true, null);
			decimal mIncomeTaxThreshold = default(decimal);
			foreach (ImportSalaryListModel salary in importSalaryModel.SalaryList)
			{
				decimal d = salary.MRetirementSecurityAmount + salary.MUmemploymentAmount + salary.MMedicalInsuranceAmount + salary.MProvidentAmount + salary.MProvidentAdditionalAmount;
				salary.MSalaryBeforeTax = salary.MBaseSalary + salary.MAllowance + salary.MCommission + salary.MBonus + salary.MOverTime + salary.MTaxAdjustment + salary.MAttendance + salary.MOther - d;
				salary.MTaxSalary = pAPITCalculateUtility.CalculateSalaryPIT(salary.MEmployeeID, salary.MSalaryBeforeTax, out mIncomeTaxThreshold);
				salary.MIncomeTaxThreshold = mIncomeTaxThreshold;
				salary.MNetSalary = Math.Round(salary.MSalaryBeforeTax - salary.MTaxSalary, 2, MidpointRounding.AwayFromZero);
				SetSSAndHFAmount(salary, importSalaryModel, disabledItems);
				salary.MTotalSalary = salary.MSalaryBeforeTax + d + salary.MRetirementSecurityAmountC + salary.MMedicalInsuranceAmountC + salary.MUmemploymentInsuranceAmountC + salary.MMaternityInsuranceAmountC + salary.MIndustrialInjuryAmountC + salary.MSeriousIiinessInjuryAmountC + salary.MSocialSecurityOtherAmountC + salary.MProvidentFundAmountC + salary.MAddProvidentFundAmountC;
			}
			return importSalaryModel;
		}

		private static void SetSSAndHFAmount(ImportSalaryListModel salary, ImportSalaryModel model, List<PAPayItemModel> disabledItems)
		{
			if (disabledItems.Any((PAPayItemModel f) => f.MItemType == 1040))
			{
				salary.MRetirementSecurityAmount = decimal.Zero;
			}
			if (disabledItems.Any((PAPayItemModel f) => f.MItemType == 1045))
			{
				salary.MMedicalInsuranceAmount = decimal.Zero;
			}
			if (disabledItems.Any((PAPayItemModel f) => f.MItemType == 1050))
			{
				salary.MUmemploymentAmount = decimal.Zero;
			}
			if (disabledItems.Any((PAPayItemModel f) => f.MItemType == 1060))
			{
				salary.MProvidentAmount = decimal.Zero;
			}
			if (disabledItems.Any((PAPayItemModel f) => f.MItemType == 1065))
			{
				salary.MProvidentAdditionalAmount = decimal.Zero;
			}
			if (!disabledItems.Any((PAPayItemModel f) => f.MItemType == 2020))
			{
				salary.MRetirementSecurityAmountC = Math.Round(salary.MSocialSecurityBase * model.PaySetting.MRetirementSecurityPer / 100m, 2, MidpointRounding.AwayFromZero);
			}
			if (!disabledItems.Any((PAPayItemModel f) => f.MItemType == 2025))
			{
				salary.MMedicalInsuranceAmountC += Math.Round(salary.MSocialSecurityBase * model.PaySetting.MMedicalInsurancePer / 100m, 2, MidpointRounding.AwayFromZero);
			}
			if (!disabledItems.Any((PAPayItemModel f) => f.MItemType == 2035))
			{
				salary.MUmemploymentInsuranceAmountC = Math.Round(salary.MSocialSecurityBase * model.PaySetting.MUmemploymentInsurancePer / 100m, 2, MidpointRounding.AwayFromZero);
			}
			if (!disabledItems.Any((PAPayItemModel f) => f.MItemType == 2030))
			{
				salary.MMaternityInsuranceAmountC = Math.Round(salary.MSocialSecurityBase * model.PaySetting.MMaternityInsurancePer / 100m, 2, MidpointRounding.AwayFromZero);
			}
			if (!disabledItems.Any((PAPayItemModel f) => f.MItemType == 2040))
			{
				salary.MIndustrialInjuryAmountC = Math.Round(salary.MSocialSecurityBase * model.PaySetting.MIndustrialInjuryPer / 100m, 2, MidpointRounding.AwayFromZero);
			}
			if (!disabledItems.Any((PAPayItemModel f) => f.MItemType == 2045))
			{
				salary.MSeriousIiinessInjuryAmountC = Math.Round(salary.MSocialSecurityBase * model.PaySetting.MSeriousIiinessInjuryPer / 100m, 2, MidpointRounding.AwayFromZero);
			}
			if (!disabledItems.Any((PAPayItemModel f) => f.MItemType == 2050))
			{
				salary.MSocialSecurityOtherAmountC = Math.Round(salary.MSocialSecurityBase * model.PaySetting.MOtherPer / 100m, 2, MidpointRounding.AwayFromZero);
			}
			if (!disabledItems.Any((PAPayItemModel f) => f.MItemType == 2005))
			{
				salary.MProvidentFundAmountC = Math.Round(salary.MHosingProvidentFundBase * model.PaySetting.MProvidentFundPer / 100m, 2, MidpointRounding.AwayFromZero);
			}
			if (!disabledItems.Any((PAPayItemModel f) => f.MItemType == 2010))
			{
				salary.MAddProvidentFundAmountC = Math.Round(salary.MHosingProvidentFundBase * model.PaySetting.MAddProvidentFundPer / 100m, 2, MidpointRounding.AwayFromZero);
			}
		}

		private static bool IsPayRunExist(MContext ctx, string yearMonth, PayRunSourceEnum source, ref string existPeriod)
		{
			DateTime salaryDate = GetSalaryDate(yearMonth);
			SqlWhere sqlWhere = new SqlWhere().Equal("MOrgID", ctx.MOrgID);
			switch (source)
			{
			case PayRunSourceEnum.New:
				sqlWhere = sqlWhere.Equal("MDate", salaryDate);
				break;
			case PayRunSourceEnum.Import:
			{
				List<string> list = new List<string>();
				string[] array = yearMonth.Split(',');
				if (array.Length != 0)
				{
					string[] array2 = array;
					foreach (string yearMonth2 in array2)
					{
						list.Add(GetSalaryDate(yearMonth2).ToString("yyyy-MM-dd"));
					}
				}
				sqlWhere = sqlWhere.In("MDate", list);
				break;
			}
			case PayRunSourceEnum.Copy:
				sqlWhere = sqlWhere.LessThen("MDate", salaryDate);
				break;
			}
			List<PAPayRunModel> dataModelList = ModelInfoManager.GetDataModelList<PAPayRunModel>(ctx, sqlWhere, false, false);
			if (dataModelList.Any() && source != PayRunSourceEnum.Copy)
			{
				string[] values = (from f in dataModelList
				select f.MID).ToArray();
				List<PASalaryPaymentModel> dataModelList2 = ModelInfoManager.GetDataModelList<PASalaryPaymentModel>(ctx, new SqlWhere().In("MRunID", values), false, false);
				IEnumerable<PASalaryPaymentModel> source2 = from f in dataModelList2
				where f.MStatus >= 3
				select f;
				if (source2.Any())
				{
					IEnumerable<string> approvedRunIds = (from f in source2
					select f.MRunID).Distinct();
					IEnumerable<PAPayRunModel> source3 = from f in dataModelList
					where approvedRunIds.Contains(f.MID)
					select f;
					string arg = string.Join(",", from f in source3
					select f.MDate.ToString("yyyy-MM"));
					string format = (source2.Count() == dataModelList2.Count()) ? COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "PayRunHasApproved", "The Pay Run:{0} have approved Salary List!") : COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "PayRunHasPartiallyApproved", "The Pay Run:{0} have approved Salary List!");
					throw new Exception(string.Format(format, arg));
				}
			}
			existPeriod = string.Join(",", from f in dataModelList
			select f.MDate.ToString("yyyy-MM"));
			return dataModelList.Any();
		}

		private static bool IsPayRunExist(MContext ctx, string yearMonth, PayRunSourceEnum source)
		{
			string empty = string.Empty;
			return IsPayRunExist(ctx, yearMonth, source, ref empty);
		}

		public static DateTime GetSalaryDate(string yearMonth)
		{
			if (yearMonth.IndexOf(",") != -1)
			{
				yearMonth = yearMonth.Split(',')[0];
			}
			yearMonth = Regex.Replace(yearMonth, "[^\\d]", "");
			string value = yearMonth.Substring(0, 4);
			string value2 = yearMonth.Substring(4, yearMonth.Length - 4);
			return new DateTime(Convert.ToInt32(value), Convert.ToInt32(value2), 1);
		}

		public static void CheckEmployeeExist(MContext ctx, DateTime salaryDate)
		{
			List<BDEmployeesModel> list = (from f in GetActiveEmployeeList(ctx)
			where f.MJoinTime < salaryDate.AddMonths(1)
			select f).ToList();
			if (list.Count != 0)
			{
				return;
			}
			throw new Exception(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "NoEmployeeExist", "No employee exist, please add employee first!"));
		}

		public static List<BDEmployeesModel> GetActiveEmployeeList(MContext ctx)
		{
			string sql = "select a.*,F_GetUserName(c.MFirstName,c.MLastName) as MFullName,b.MJoinTime from T_BD_Employees a \r\n                            left join T_BD_EmpPayrollBasicSet b on b.MOrgID=a.MOrgID and a.MItemID=b.MEmployeeID and b.MIsDelete=0\r\n                            left join t_bd_employees_l c on c.MParentID=a.MItemID and c.MOrgID=a.MOrgID and c.MIsDelete=0 and c.MLocaleID=@MLocaleID\r\n                            where a.MIsActive=1 and a.MIsDelete=0 AND ifnull(a.MStatus, '')<>'Leave' AND a.MOrgID=@MOrgID ";
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MLocaleID", ctx.MLCID)
			};
			return ModelInfoManager.GetDataModelBySql<BDEmployeesModel>(ctx, sql, cmdParms);
		}

		public static List<PAPayItemModel> GetDisableItemList(MContext ctx)
		{
			List<PAPayItemModel> payItemList = PAPayItemRepository.GetPayItemList(ctx, null);
			List<PAPayItemModel> list = (from f in payItemList
			where !f.MIsActive
			select f).ToList();
			if (list.Any((PAPayItemModel f) => f.MItemType == 1035) && list.Any((PAPayItemModel f) => f.MItemType == 1055))
			{
				list.Add(new PAPayItemModel
				{
					MItemType = 3105
				});
			}
			if (list.Any((PAPayItemModel f) => f.MItemType == 2015) && list.Any((PAPayItemModel f) => f.MItemType == 2000))
			{
				list.Add(new PAPayItemModel
				{
					MItemType = 3110
				});
			}
			if (list.Any((PAPayItemModel f) => f.MItemType == 3105) && list.Any((PAPayItemModel f) => f.MItemType == 3110))
			{
				list.Add(new PAPayItemModel
				{
					MItemType = 3115
				});
			}
			if (list.Any((PAPayItemModel f) => f.MItemType == 1005) && list.Any((PAPayItemModel f) => f.MItemType == 1010) && list.Any((PAPayItemModel f) => f.MItemType == 1015) && list.Any((PAPayItemModel f) => f.MItemType == 1020) && list.Any((PAPayItemModel f) => f.MItemType == 1025) && list.Count((PAPayItemModel f) => f.MItemType == 1023) == payItemList.Count((PAPayItemModel f) => f.MItemType == 1023))
			{
				list.Add(new PAPayItemModel
				{
					MItemType = 2001
				});
			}
			foreach (PAPayItemModel item in list)
			{
				item.MItemTypeName = PAPayItemGroupRepository.GetItemTypeName((PayrollItemEnum)item.MItemType, item.MItemID);
			}
			return list;
		}

		public static PASalaryPaymentModel GetSalaryPaymentEditModel(MContext ctx, string pkID)
		{
			string sql = "select a.*,b.MDate,F_GetUserName(c.MFirstName,c.MLastName)  as MEmployeeName\r\n                                ,case when e.MIsActive=0 and a.MCreateDate>e.MModifyDate then 0 else 1 end as IsCalculatePIT \r\n                            from T_PA_SalaryPayment a \r\n                            join T_PA_PayRun b on b.MOrgID=a.MOrgID and a.MRunID=b.MID and b.MIsDelete=0\r\n                            left join T_BD_Employees_L c on c.MOrgID=a.MOrgID and a.MEmployeeID=c.MParentID and c.MIsDelete=0 AND c.MLocaleID=@MLocaleID\r\n                            left join t_pa_payitemgroup e on e.MOrgID=a.MOrgID and e.MItemType=@MItemType and e.MIsDelete=0\r\n                            where a.MID=@MID and a.MOrgID=@MOrgID and a.MIsDelete=0";
			MySqlParameter[] cmdParms = new MySqlParameter[4]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MLocaleID", ctx.MLCID),
				new MySqlParameter("@MID", pkID),
				new MySqlParameter("@MItemType", 3010)
			};
			PASalaryPaymentModel pASalaryPaymentModel = ModelInfoManager.GetDataModel<PASalaryPaymentModel>(ctx, sql, cmdParms);
			if (pASalaryPaymentModel == null)
			{
				pASalaryPaymentModel = new PASalaryPaymentModel();
			}
			pASalaryPaymentModel.MReference = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "PeriodSalaryList", "{0} Salary List"), pASalaryPaymentModel.MDateFormat);
			return pASalaryPaymentModel;
		}

		public static OperationResult UnApproveSalaryPayment(MContext ctx, PASalaryPaymentModel model)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			List<CommandInfo> list2 = list;
			CommandInfo commandInfo = new CommandInfo
			{
				CommandText = "UPDATE T_PA_SalaryPayment t1\r\n\t                                JOIN T_PA_PayRun t2 on t2.MID=t1.MRunID and t2.MOrgID=t1.MOrgID and t2.MIsDelete=0 \r\n\t                                SET t1.MStatus=@MStatus, t2.MStatus=@MStatus\r\n                                WHERE t1.MOrgID=@MOrgID and t1.MID=@MID"
			};
			DbParameter[] array = commandInfo.Parameters = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MStatus", Convert.ToInt32(PASalaryPaymentStatusEnum.Draft)),
				new MySqlParameter("@MID", model.MID)
			};
			list2.Add(commandInfo);
			RecordStatus status = RecordStatus.Draft;
			OperationResult operationResult = GLInterfaceRepository.TransferBillCreatedVouchersByStatus(ctx, model.MID, status);
			if (!operationResult.Success)
			{
				return operationResult;
			}
			list.AddRange(operationResult.OperationCommands);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num = dynamicDbHelperMySQL.ExecuteSqlTran(list);
			if (num > 0)
			{
				PASalaryPaymentLogRepository.AddSalaryPaymentUnApproveLog(ctx, model);
			}
			return new OperationResult
			{
				Success = (num > 0),
				ObjectID = model.MID
			};
		}

		private static PAPayRunModel GetPayRunModel(MContext ctx, DateTime dt, bool isCopy = false)
		{
			SqlWhere sqlWhere = new SqlWhere();
			if (isCopy)
			{
				sqlWhere = sqlWhere.LessThen("MDate", dt);
				sqlWhere.OrderBy("MDate DESC");
			}
			else
			{
				sqlWhere = sqlWhere.Equal("MDate", dt);
			}
			List<PAPayRunModel> dataModelList = ModelInfoManager.GetDataModelList<PAPayRunModel>(ctx, sqlWhere, false, false);
			return dataModelList.Any() ? dataModelList[0] : null;
		}

		private static string GetChartColor(int k)
		{
			string[] array = "#f69679,#F5A58D,#F5AF9A,#F4B9A8,#F4BFAF,#F4C8BA,#F4D2C9,#F3D6CD,#F3DFD9,#F9EEEB,#FBF3F1,#FDFAF9,#FFFFFF".Split(',');
			if (k < array.Length)
			{
				return array[k];
			}
			return array[array.Length - 1];
		}

		private static string GetSalaryItemLabel(MContext ctx, string name)
		{
			string result = string.Empty;
			switch (name)
			{
			case "BaseSalary":
				result = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "BaseSalary", "Base Salary");
				break;
			case "IncreaseItem":
				result = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "IncreaseItem", "增加项");
				break;
			case "SSWithHFEmployer":
				result = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "SSHFEmployer", "社保和公积金（公司）");
				break;
			}
			return result;
		}

		private static List<PayRunChartModel> GetPayRunChartList(MContext ctx, List<PAPayRunListModel> payRunList)
		{
			List<PayRunChartModel> list = new List<PayRunChartModel>();
			if (payRunList.Any())
			{
				int num = 0;
				PropertyInfo[] properties = typeof(PASalaryListBaseModel).GetProperties();
				string[] obj = new string[3];
				PayrollItemEnum payrollItemEnum = PayrollItemEnum.BaseSalary;
				obj[0] = payrollItemEnum.ToString();
				payrollItemEnum = PayrollItemEnum.IncreaseItem;
				obj[1] = payrollItemEnum.ToString();
				payrollItemEnum = PayrollItemEnum.SSWithHFEmployer;
				obj[2] = payrollItemEnum.ToString();
				string[] array = obj;
				string[] array2 = array;
				foreach (string text in array2)
				{
					PayRunChartModel payRunChartModel = new PayRunChartModel();
					payRunChartModel.value = new double[6];
					payRunChartModel.field = text;
					payRunChartModel.name = GetSalaryItemLabel(ctx, text);
					payRunChartModel.color = GetChartColor(num);
					list.Add(payRunChartModel);
					num++;
				}
				int num2 = 0;
				foreach (PAPayRunListModel payRun in payRunList)
				{
					int num3 = 0;
					foreach (PayRunChartModel item in list)
					{
						PropertyInfo propertyInfo = properties.FirstOrDefault((PropertyInfo f) => f.Name == item.field);
						list[num3].value[num2] = Convert.ToDouble(propertyInfo.GetValue(payRun));
						num3++;
					}
					num2++;
				}
			}
			return list;
		}

		private static void SetListSalaryItemAmount(IEnumerable<PAPayItemGroupAmtModel> amtList, PASalaryListBaseModel listModel, List<PAPayItemGroupAmtModel> userPayItemList, PayrollItemEnum[] increaseItemTypes = null)
		{
			if (amtList.Any())
			{
				List<PAPayItemGroupAmtModel> list = (from f in userPayItemList
				where !amtList.Any((PAPayItemGroupAmtModel e) => e.MPayItemID == f.MPayItemID)
				select f).ToList();
				IEnumerable<IGrouping<string, PAPayItemGroupAmtModel>> enumerable = from f in amtList
				group f by f.MPayItemID;
				decimal d = default(decimal);
				foreach (IGrouping<string, PAPayItemGroupAmtModel> item in enumerable)
				{
					List<PAPayItemGroupAmtModel> groupList = item.ToList();
					PayrollItemEnum itemType = (PayrollItemEnum)groupList[0].ItemType;
					decimal num = groupList.Sum((PAPayItemGroupAmtModel f) => f.Amount * (decimal)((f.MCoefficient == 0) ? 1 : f.MCoefficient));
					string propName = itemType.ToString();
					if (itemType == PayrollItemEnum.UserAddItem || itemType == PayrollItemEnum.UserSubtractItem)
					{
						PAPayItemGroupAmtModel pAPayItemGroupAmtModel = userPayItemList.FirstOrDefault((PAPayItemGroupAmtModel f) => f.MPayItemID == groupList[0].MPayItemID)?.Clone() as PAPayItemGroupAmtModel;
						pAPayItemGroupAmtModel.Amount = num;
						list.Add(pAPayItemGroupAmtModel);
						decimal modelValueD = ModelHelper.GetModelValueD(listModel, propName);
						num = modelValueD + num;
					}
					ModelHelper.SetModelValue(listModel, propName, num, null);
					if (increaseItemTypes != null && increaseItemTypes.Contains(itemType) && itemType != PayrollItemEnum.UserAddItem)
					{
						d += num;
					}
				}
				listModel.UserPayItemList = (from item in list
				orderby item.ItemType, item.MCreateDate
				select item).ToList();
				d += listModel.UserAddItem;
				ModelHelper.SetModelValue(listModel, 2001.ToString(), d, null);
				listModel.SSWithHFEmployee = listModel.EmployeeSocialSecurity + listModel.EmployeeHousingProvidentFund;
				listModel.SSWithHFEmployer = listModel.EmployerSocialSecurity + listModel.EmployerHousingProvidentFund;
				listModel.SSWithHFTotal = Math.Abs(listModel.SSWithHFEmployee) + listModel.SSWithHFEmployer;
				listModel.SalaryBeforePIT = listModel.BaseSalary + listModel.Allowance + listModel.Bonus + listModel.Commission + listModel.OverTime + listModel.UserAddItem + listModel.TaxAdjustment + listModel.Attendance + listModel.SSWithHFEmployee + listModel.UserSubtractItem + listModel.Other;
				listModel.SalaryAfterPIT = listModel.SalaryBeforePIT - listModel.PIT;
				listModel.TotalSalary = listModel.SalaryBeforePIT + Math.Abs(listModel.SSWithHFEmployee) + listModel.SSWithHFEmployer;
			}
		}

		public static PAPaySettingModel GetPaySettingModel(MContext ctx)
		{
			List<PAPaySettingModel> dataModelList = ModelInfoManager.GetDataModelList<PAPaySettingModel>(ctx, new SqlWhere().Equal("MOrgID", ctx.MOrgID), false, false);
			return dataModelList.Any() ? dataModelList[0] : new PAPaySettingModel
			{
				MOrgID = ctx.MOrgID
			};
		}

		public static List<CommandInfo> GetImportSalaryCommandList(MContext ctx, string period, ref string runId)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			DateTime salaryDate = GetSalaryDate(period);
			list.AddRange(GetDelExistPayRunCommandList(ctx, salaryDate));
			PAPayRunModel newOrCopyPayRunModel = GetNewOrCopyPayRunModel(ctx, salaryDate, null);
			list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<PAPayRunModel>(ctx, newOrCopyPayRunModel, null, true));
			runId = newOrCopyPayRunModel.MID;
			return list;
		}

		public static PAPayRunModel GetNewOrCopyPayRunModel(MContext ctx, DateTime salaryMonth, PAPayRunModel model = null)
		{
			if (model == null)
			{
				model = new PAPayRunModel();
				model.MOrgID = ctx.MOrgID;
				model.MID = UUIDHelper.GetGuid();
			}
			else
			{
				model.MID = UUIDHelper.GetGuid();
				model.MCreatorID = ctx.MUserID;
				model.MModifierID = ctx.MUserID;
				model.MCreateDate = ctx.DateNow;
				model.MModifyDate = ctx.DateNow;
			}
			model.MStatus = 1;
			model.MDate = salaryMonth;
			model.IsNew = true;
			model.MNumber = GetPayRunAutoNumber(ctx);
			return model;
		}

		private static OperationResult CreatePayRun(MContext ctx, DateTime salaryMonth, PAPayRunModel model = null)
		{
			OperationResult operationResult = new OperationResult();
			model = GetNewOrCopyPayRunModel(ctx, salaryMonth, model);
			try
			{
				DelExistPayRunData(ctx, model.MDate);
			}
			catch (Exception ex)
			{
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = ex.Message
				});
				return operationResult;
			}
			return ModelInfoManager.InsertOrUpdate<PAPayRunModel>(ctx, model, null);
		}

		private static List<CommandInfo> GetCreatePayRunCmdList(MContext ctx, DateTime salaryMonth, PAPayRunModel model = null)
		{
			OperationResult operationResult = new OperationResult();
			model = GetNewOrCopyPayRunModel(ctx, salaryMonth, model);
			List<CommandInfo> list = new List<CommandInfo>();
			list.AddRange(GetDelExistPayRunCommandList(ctx, salaryMonth));
			list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<PAPayRunModel>(ctx, model, null, true));
			string mID = model.MID;
			return list;
		}

		public static void DelExistPayRunData(MContext ctx, string yearMonth)
		{
			DelExistPayRunData(ctx, GetSalaryDate(yearMonth));
		}

		public static List<CommandInfo> GetDelExistPayRunCommandList(MContext ctx, DateTime yearMonth)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			PAPayRunModel payRunModel = GetPayRunModel(ctx, yearMonth, false);
			if (payRunModel != null)
			{
				DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
				List<PASalaryPaymentModel> dataModelList = ModelInfoManager.GetDataModelList<PASalaryPaymentModel>(ctx, new SqlWhere().Equal("MRunID", payRunModel.MID), false, false);
				list.AddRange(ModelInfoManager.GetDeleteCmd<PAPayRunModel>(ctx, payRunModel.MID));
				if (dataModelList.Any())
				{
					foreach (PASalaryPaymentModel item in dataModelList)
					{
						List<PASalaryPaymentEntryModel> dataModelList2 = ModelInfoManager.GetDataModelList<PASalaryPaymentEntryModel>(ctx, new SqlWhere().Equal("MID", item.MID), false, false);
						if (dataModelList2.Any())
						{
							list.AddRange(ModelInfoManager.GetDeleteCmd<PASalaryPaymentEntryModel>(ctx, (from f in dataModelList2
							select f.MEntryID).ToList()));
						}
					}
					list.AddRange(ModelInfoManager.GetDeleteCmd<PASalaryPaymentModel>(ctx, (from f in dataModelList
					select f.MID).ToList()));
				}
			}
			return list;
		}

		public static void DelExistPayRunData(MContext ctx, DateTime yearMonth)
		{
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			List<CommandInfo> delExistPayRunCommandList = GetDelExistPayRunCommandList(ctx, yearMonth);
			dynamicDbHelperMySQL.ExecuteSqlTran(delExistPayRunCommandList);
		}

		public List<CommandInfo> GetDeleteExistPayRunDataCmdList(MContext ctx, DateTime yearMonth)
		{
			return GetDelExistPayRunCommandList(ctx, yearMonth);
		}

		private static OperationResult CreateSalaryPayment(MContext ctx, string payRunId, DateTime salaryDate, string employeeIds = null)
		{
			OperationResult operationResult = new OperationResult();
			try
			{
				DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
				List<PASalaryPaymentListModel> newSalaryPaymentList = GetNewSalaryPaymentList(ctx, salaryDate, employeeIds);
				List<CommandInfo> list = new List<CommandInfo>();
				List<PAPayItemModel> disableItemList = GetDisableItemList(ctx);
				List<PANewSalaryPaymentEntryModel> payItemList = GetPayItemList(ctx, false);
				PAPaySettingModel paySettingModel = GetPaySettingModel(ctx);
				List<BDPayrollDetailModel> dataModelList = ModelInfoManager.GetDataModelList<BDPayrollDetailModel>(ctx, new SqlWhere().In("MEmployeeID", (from f in newSalaryPaymentList
				select f.MEmployeeID).ToList()), false, false);
				foreach (PASalaryPaymentListModel item in newSalaryPaymentList)
				{
					PASalaryPaymentModel pASalaryPaymentModel = new PASalaryPaymentModel();
					pASalaryPaymentModel.MOrgID = ctx.MOrgID;
					pASalaryPaymentModel.MID = UUIDHelper.GetGuid();
					pASalaryPaymentModel.IsNew = true;
					pASalaryPaymentModel.MStatus = 1;
					pASalaryPaymentModel.MRunID = payRunId;
					pASalaryPaymentModel.MEmployeeID = item.MEmployeeID;
					if (!disableItemList.Any((PAPayItemModel f) => f.MItemType == 1000))
					{
						if (!disableItemList.Any((PAPayItemModel f) => f.MItemType == 3010))
						{
							pASalaryPaymentModel.MTaxSalary = Math.Round(item.PIT, 2, MidpointRounding.AwayFromZero);
						}
						if (!disableItemList.Any((PAPayItemModel f) => f.MItemType == 3005))
						{
							pASalaryPaymentModel.MNetSalary = Math.Round(item.SalaryAfterPIT, 2, MidpointRounding.AwayFromZero);
						}
					}
					BDPayrollDetailModel empPaySetting = dataModelList.FirstOrDefault((BDPayrollDetailModel f) => f.MEmployeeID == item.MEmployeeID);
					List<PASalaryPaymentEntryModel> list2 = new List<PASalaryPaymentEntryModel>();
					foreach (PANewSalaryPaymentEntryModel item2 in payItemList)
					{
						if (!disableItemList.Any((PAPayItemModel f) => f.MItemType == (int)item2.MItemType))
						{
							PASalaryPaymentEntryModel pASalaryPaymentEntryModel = new PASalaryPaymentEntryModel();
							pASalaryPaymentEntryModel.MItemType = item2.MItemType;
							pASalaryPaymentEntryModel.MID = pASalaryPaymentModel.MID;
							pASalaryPaymentEntryModel.MPayItemID = item2.MPayItemID;
							pASalaryPaymentEntryModel.MParentPayItemID = item2.MParentPayItemID;
							pASalaryPaymentEntryModel.MAmount = GetSPEntryItemAmount(item2.MItemType, item, empPaySetting, paySettingModel);
							list2.Add(pASalaryPaymentEntryModel);
						}
					}
					SetSPEntryGroupAmount(list2);
					pASalaryPaymentModel.SalaryPaymentEntry = list2;
					List<CommandInfo> insertOrUpdateCmd = ModelInfoManager.GetInsertOrUpdateCmd<PASalaryPaymentModel>(ctx, pASalaryPaymentModel, null, true);
					list.AddRange(insertOrUpdateCmd);
				}
				dynamicDbHelperMySQL.ExecuteSqlTran(list);
			}
			catch (Exception ex)
			{
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = ex.Message
				});
			}
			return operationResult;
		}

		private static void SetSPEntryGroupAmount(List<PASalaryPaymentEntryModel> entryList)
		{
			PayrollItemEnum[] array = new PayrollItemEnum[4]
			{
				PayrollItemEnum.EmployerSocialSecurity,
				PayrollItemEnum.EmployerHousingProvidentFund,
				PayrollItemEnum.EmployeeSocialSecurity,
				PayrollItemEnum.EmployeeHousingProvidentFund
			};
			IEnumerable<PASalaryPaymentEntryModel> source = new List<PASalaryPaymentEntryModel>();
			PayrollItemEnum[] array2 = array;
			foreach (PayrollItemEnum itemType in array2)
			{
				PASalaryPaymentEntryModel pASalaryPaymentEntryModel = entryList.FirstOrDefault((PASalaryPaymentEntryModel f) => f.MItemType == itemType);
				if (pASalaryPaymentEntryModel != null)
				{
					switch (itemType)
					{
					case PayrollItemEnum.EmployerSocialSecurity:
						source = from f in entryList
						where f.MItemType == PayrollItemEnum.CBasicMedicalInsurance || f.MItemType == PayrollItemEnum.CBasicRetirementSecurity || f.MItemType == PayrollItemEnum.CBasicUnemploymentInsurance || f.MItemType == PayrollItemEnum.CIndustrialInjury || f.MItemType == PayrollItemEnum.CMaternityInsurance || f.MItemType == PayrollItemEnum.CSeriousIllnessMedicalTreatment || f.MItemType == PayrollItemEnum.CSocialSecurityOther
						select f;
						break;
					case PayrollItemEnum.EmployerHousingProvidentFund:
						source = from f in entryList
						where f.MItemType == PayrollItemEnum.CHousingProvidentFund || f.MItemType == PayrollItemEnum.CAdditionHousingProvidentFund
						select f;
						break;
					case PayrollItemEnum.EmployeeSocialSecurity:
						source = from f in entryList
						where f.MItemType == PayrollItemEnum.PBasicMedicalInsurance || f.MItemType == PayrollItemEnum.PBasicRetirementSecurity || f.MItemType == PayrollItemEnum.PBasicUnemploymentInsurance
						select f;
						break;
					case PayrollItemEnum.EmployeeHousingProvidentFund:
						source = from f in entryList
						where f.MItemType == PayrollItemEnum.PHousingProvidentFund || f.MItemType == PayrollItemEnum.PAdditionHousingProvidentFund
						select f;
						break;
					}
					if (source.Any())
					{
						pASalaryPaymentEntryModel.MAmount = source.Sum((PASalaryPaymentEntryModel f) => f.MAmount);
					}
				}
			}
		}

		private static decimal GetSPEntryItemAmount(PayrollItemEnum itemType, PASalaryPaymentListModel spItem, BDPayrollDetailModel empPaySetting, PAPaySettingModel orgSetting)
		{
			decimal d = default(decimal);
			switch (itemType)
			{
			case PayrollItemEnum.BaseSalary:
				d = spItem.BaseSalary;
				break;
			case PayrollItemEnum.PBasicRetirementSecurity:
				d = spItem.MRetirementSecurityAmount;
				break;
			case PayrollItemEnum.PBasicMedicalInsurance:
				d = spItem.MMedicalInsuranceAmount;
				break;
			case PayrollItemEnum.PBasicUnemploymentInsurance:
				d = spItem.MUmemploymentAmount;
				break;
			case PayrollItemEnum.PHousingProvidentFund:
				d = spItem.MProvidentAmount;
				break;
			case PayrollItemEnum.PAdditionHousingProvidentFund:
				d = spItem.MProvidentAdditionalAmount;
				break;
			case PayrollItemEnum.CBasicMedicalInsurance:
				d = ((empPaySetting == null || orgSetting == null) ? decimal.Zero : (empPaySetting.MSocialSecurityBase * orgSetting.MMedicalInsurancePer / 100m));
				break;
			case PayrollItemEnum.CBasicRetirementSecurity:
				d = ((empPaySetting == null || orgSetting == null) ? decimal.Zero : (empPaySetting.MSocialSecurityBase * orgSetting.MRetirementSecurityPer / 100m));
				break;
			case PayrollItemEnum.CBasicUnemploymentInsurance:
				d = ((empPaySetting == null || orgSetting == null) ? decimal.Zero : (empPaySetting.MSocialSecurityBase * orgSetting.MUmemploymentInsurancePer / 100m));
				break;
			case PayrollItemEnum.CIndustrialInjury:
				d = ((empPaySetting == null || orgSetting == null) ? decimal.Zero : (empPaySetting.MSocialSecurityBase * orgSetting.MIndustrialInjuryPer / 100m));
				break;
			case PayrollItemEnum.CMaternityInsurance:
				d = ((empPaySetting == null || orgSetting == null) ? decimal.Zero : (empPaySetting.MSocialSecurityBase * orgSetting.MMaternityInsurancePer / 100m));
				break;
			case PayrollItemEnum.CSeriousIllnessMedicalTreatment:
				d = ((empPaySetting == null || orgSetting == null) ? decimal.Zero : (empPaySetting.MSocialSecurityBase * orgSetting.MSeriousIiinessInjuryPer / 100m));
				break;
			case PayrollItemEnum.CSocialSecurityOther:
				d = ((empPaySetting == null || orgSetting == null) ? decimal.Zero : (empPaySetting.MSocialSecurityBase * orgSetting.MOtherPer / 100m));
				break;
			case PayrollItemEnum.CHousingProvidentFund:
				d = ((empPaySetting == null || orgSetting == null) ? decimal.Zero : (empPaySetting.MHosingProvidentFundBase * orgSetting.MProvidentFundPer / 100m));
				break;
			case PayrollItemEnum.CAdditionHousingProvidentFund:
				d = ((empPaySetting == null || orgSetting == null) ? decimal.Zero : (empPaySetting.MHosingProvidentFundBase * orgSetting.MAddProvidentFundPer / 100m));
				break;
			}
			return Math.Round(d, 2, MidpointRounding.AwayFromZero);
		}

		public static List<PANewSalaryPaymentEntryModel> GetPayItemList(MContext ctx, bool includeDisabled = false)
		{
			string arg = string.Empty;
			if (!includeDisabled)
			{
				arg = " and MIsActive=1";
			}
			string sql = string.Format("SELECT MItemType, MItemID as MPayItemID, 0 as MParentPayItemID FROM t_pa_payitemgroup\r\n                                                where MOrgID=@MOrgID and MItemType <= {0} {1} and MIsDelete=0\r\n                                            union all\r\n                                            SELECT MItemType, MItemID as MPayItemID, MGroupID as MParentPayItemID FROM t_pa_payitem\r\n                                                where MOrgID=@MOrgID {1} and MIsDelete=0", 2015, arg);
			MySqlParameter[] cmdParms = new MySqlParameter[1]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			return ModelInfoManager.GetDataModelBySql<PANewSalaryPaymentEntryModel>(ctx, sql, cmdParms);
		}

		public static List<PASalaryPaymentEntryModel> GetSalaryEntryList(MContext ctx, List<string> midList)
		{
			string sql = string.Format("SELECT c.MName as MItemName, b.MItemType, a.MEntryID,a.MID,a.MPayItemID,b.MCoefficient,a.MParentPayItemID,Round(a.MAmount, 2) as MAmount,a.MDesc,a.MOrgID,a.MIsActive,a.MIsDelete,a.MCreatorID,a.MCreateDate,a.MModifierID,a.MModifyDate \r\n                                FROM t_pa_salarypaymententry a \r\n                            left join t_pa_payitemgroup b on b.MOrgID=a.MOrgID and b.MItemID=a.MPayItemID and b.MIsDelete=0\r\n                            left join t_pa_payitemgroup_l c on c.MOrgID=a.MOrgID and c.MParentID=a.MPayItemID and c.MLocaleID=@MLocaleID and c.MIsDelete=0\r\n                        where a.MOrgID=@MOrgID and a.MID in ('{0}') and a.MParentPayItemID='0' and a.MIsDelete=0\r\n                        union all\r\n                        SELECT c.MName as MItemName, b.MItemType, a.MEntryID,a.MID,a.MPayItemID,b.MCoefficient,a.MParentPayItemID,Round(a.MAmount, 2) as MAmount,a.MDesc,a.MOrgID,a.MIsActive,a.MIsDelete,a.MCreatorID,a.MCreateDate,a.MModifierID,a.MModifyDate \r\n                        FROM t_pa_salarypaymententry a \r\n                            left join t_pa_payitem b on b.MOrgID=a.MOrgID and b.MItemID=a.MPayItemID and b.MIsDelete=0\r\n                            left join t_pa_payitem_l c on c.MOrgID=a.MOrgID and c.MParentID=a.MPayItemID and c.MLocaleID=@MLocaleID and c.MIsDelete=0\r\n                        where a.MOrgID=@MOrgID and a.MID in ('{0}') and a.MParentPayItemID<>'0' and a.MIsDelete=0;", string.Join("','", midList));
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MLocaleID", ctx.MLCID)
			};
			return ModelInfoManager.GetDataModelBySql<PASalaryPaymentEntryModel>(ctx, sql, cmdParms);
		}

		public static List<CommandInfo> GetCopySalaryPaymentCmdList(MContext ctx, string newRunId, string copyRunId, DateTime salaryDate)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			List<PAPayItemModel> payItemList = PAPayItemRepository.GetPayItemList(ctx, false);
			List<PASalaryPaymentModel> dataModelList = ModelInfoManager.GetDataModelList<PASalaryPaymentModel>(ctx, new SqlWhere().Equal("MRunID", copyRunId), false, false);
			if (dataModelList.Any())
			{
				List<PASalaryPaymentEntryModel> salaryEntryList = GetSalaryEntryList(ctx, (from f in dataModelList
				select f.MID).ToList());
				List<BDPayrollDetailModel> dataModelList2 = ModelInfoManager.GetDataModelList<BDPayrollDetailModel>(ctx, new SqlWhere().In("MEmployeeID", (from f in dataModelList
				select f.MEmployeeID).ToList()), false, false);
				List<BDEmployeesModel> dataModelList3 = ModelInfoManager.GetDataModelList<BDEmployeesModel>(ctx, new SqlWhere(), false, false);
				PAPITCalculateUtility pAPITCalculateUtility = new PAPITCalculateUtility(ctx, salaryDate, true, null);
				foreach (PASalaryPaymentModel item in dataModelList)
				{
					if (!dataModelList3.Exists((BDEmployeesModel f) => f.MItemID == item.MEmployeeID && ("Leave".EqualsIgnoreCase(f.MStatus) || !f.MIsActive)))
					{
						List<PASalaryPaymentEntryModel> list2 = (from f in salaryEntryList
						where f.MID == item.MID
						select f).ToList();
						string guid = UUIDHelper.GetGuid();
						item.IsNew = true;
						Dictionary<PayrollItemEnum, int> dictionary = new Dictionary<PayrollItemEnum, int>();
						dictionary.Add(PayrollItemEnum.EmployeeSocialSecurity, 0);
						dictionary.Add(PayrollItemEnum.EmployeeHousingProvidentFund, 0);
						dictionary.Add(PayrollItemEnum.EmployerSocialSecurity, 0);
						dictionary.Add(PayrollItemEnum.EmployerHousingProvidentFund, 0);
						foreach (PASalaryPaymentEntryModel item2 in list2)
						{
							item2.MEntryID = string.Empty;
							item2.MID = guid;
							if (payItemList.Any((PAPayItemModel f) => f.MItemID == item2.MPayItemID))
							{
								PASalaryPaymentEntryModel pASalaryPaymentEntryModel = null;
								switch (item2.MItemType)
								{
								case PayrollItemEnum.PBasicRetirementSecurity:
								case PayrollItemEnum.PBasicMedicalInsurance:
								case PayrollItemEnum.PBasicUnemploymentInsurance:
									dictionary[PayrollItemEnum.EmployeeSocialSecurity]++;
									pASalaryPaymentEntryModel = list2.FirstOrDefault((PASalaryPaymentEntryModel f) => f.MItemType == PayrollItemEnum.EmployeeSocialSecurity);
									if (pASalaryPaymentEntryModel != null)
									{
										PASalaryPaymentEntryModel pASalaryPaymentEntryModel5 = pASalaryPaymentEntryModel;
										pASalaryPaymentEntryModel5.MAmount -= item2.MAmount;
										pASalaryPaymentEntryModel.MIsAvailable = (dictionary[PayrollItemEnum.EmployeeSocialSecurity] < 3);
									}
									break;
								case PayrollItemEnum.PHousingProvidentFund:
								case PayrollItemEnum.PAdditionHousingProvidentFund:
									dictionary[PayrollItemEnum.EmployeeHousingProvidentFund]++;
									pASalaryPaymentEntryModel = list2.FirstOrDefault((PASalaryPaymentEntryModel f) => f.MItemType == PayrollItemEnum.EmployeeHousingProvidentFund);
									if (pASalaryPaymentEntryModel != null)
									{
										PASalaryPaymentEntryModel pASalaryPaymentEntryModel3 = pASalaryPaymentEntryModel;
										pASalaryPaymentEntryModel3.MAmount -= item2.MAmount;
										pASalaryPaymentEntryModel.MIsAvailable = (dictionary[PayrollItemEnum.EmployeeHousingProvidentFund] < 2);
									}
									break;
								case PayrollItemEnum.CBasicRetirementSecurity:
								case PayrollItemEnum.CBasicMedicalInsurance:
								case PayrollItemEnum.CMaternityInsurance:
								case PayrollItemEnum.CBasicUnemploymentInsurance:
								case PayrollItemEnum.CIndustrialInjury:
								case PayrollItemEnum.CSeriousIllnessMedicalTreatment:
								case PayrollItemEnum.CSocialSecurityOther:
									dictionary[PayrollItemEnum.EmployerSocialSecurity]++;
									pASalaryPaymentEntryModel = list2.FirstOrDefault((PASalaryPaymentEntryModel f) => f.MItemType == PayrollItemEnum.EmployerSocialSecurity);
									if (pASalaryPaymentEntryModel != null)
									{
										PASalaryPaymentEntryModel pASalaryPaymentEntryModel4 = pASalaryPaymentEntryModel;
										pASalaryPaymentEntryModel4.MAmount -= item2.MAmount;
										pASalaryPaymentEntryModel.MIsAvailable = (dictionary[PayrollItemEnum.EmployerSocialSecurity] < 7);
									}
									break;
								case PayrollItemEnum.CHousingProvidentFund:
								case PayrollItemEnum.CAdditionHousingProvidentFund:
									dictionary[PayrollItemEnum.EmployerHousingProvidentFund]++;
									pASalaryPaymentEntryModel = list2.FirstOrDefault((PASalaryPaymentEntryModel f) => f.MItemType == PayrollItemEnum.EmployerHousingProvidentFund);
									if (pASalaryPaymentEntryModel != null)
									{
										PASalaryPaymentEntryModel pASalaryPaymentEntryModel2 = pASalaryPaymentEntryModel;
										pASalaryPaymentEntryModel2.MAmount -= item2.MAmount;
										pASalaryPaymentEntryModel.MIsAvailable = (dictionary[PayrollItemEnum.EmployerHousingProvidentFund] < 2);
									}
									break;
								}
							}
							else
							{
								item2.MIsAvailable = true;
							}
						}
						List<PASalaryPaymentEntryModel> list3 = (from f in list2
						where f.MIsAvailable
						select f).ToList();
						if (list3.Any())
						{
							list.AddRange(ModelInfoManager.GetInsertOrUpdateCmds(ctx, list3, null, true));
						}
						decimal payItemAmount = GetPayItemAmount(list3, new List<PayrollItemEnum>
						{
							PayrollItemEnum.PBasicMedicalInsurance,
							PayrollItemEnum.PBasicRetirementSecurity,
							PayrollItemEnum.PBasicUnemploymentInsurance,
							PayrollItemEnum.PHousingProvidentFund,
							PayrollItemEnum.PAdditionHousingProvidentFund
						});
						decimal payItemAmount2 = GetPayItemAmount(list3, new List<PayrollItemEnum>
						{
							PayrollItemEnum.BaseSalary,
							PayrollItemEnum.Allowance,
							PayrollItemEnum.Commission,
							PayrollItemEnum.Bonus,
							PayrollItemEnum.OverTime,
							PayrollItemEnum.TaxAdjustment,
							PayrollItemEnum.UserAddItem
						});
						decimal payItemAmount3 = GetPayItemAmount(list3, new List<PayrollItemEnum>
						{
							PayrollItemEnum.Attendance,
							PayrollItemEnum.Other,
							PayrollItemEnum.UserSubtractItem
						});
						decimal num = payItemAmount2 + payItemAmount3 + payItemAmount;
						item.MTaxSalary = pAPITCalculateUtility.CalculateSalaryPIT(item.MEmployeeID, num);
						item.MNetSalary = Math.Round(num - item.MTaxSalary, 2, MidpointRounding.AwayFromZero);
						item.MID = guid;
						item.MOrgID = ctx.MOrgID;
						item.MRunID = newRunId;
						item.MVerificationAmt = decimal.Zero;
						item.MVerifyAmt = decimal.Zero;
						item.MVerifyAmtFor = decimal.Zero;
						item.MCreatorID = ctx.MUserID;
						item.MModifierID = ctx.MUserID;
						item.MCreateDate = ctx.DateNow;
						item.MModifyDate = ctx.DateNow;
						item.MStatus = 1;
						item.MIsSent = false;
						list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<PASalaryPaymentModel>(ctx, item, null, true));
					}
				}
			}
			return list;
		}

		private static decimal GetPayItemAmount(List<PASalaryPaymentEntryModel> entryList, List<PayrollItemEnum> itemTypeList)
		{
			return (from f in entryList
			where itemTypeList.Contains(f.MItemType)
			select f).Sum((PASalaryPaymentEntryModel f) => f.MAmount * (decimal)((f.MCoefficient == 0) ? 1 : f.MCoefficient));
		}

		private static List<PASalaryPaymentListModel> GetNewSalaryPaymentList(MContext ctx, DateTime salaryDate, string employeeIds = null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("select a.MItemID as MEmployeeID,b.MBaseSalary as BaseSalary, ");
			stringBuilder.AppendLine(" b.MRetirementSecurityAmount,b.MMedicalInsuranceAmount,b.MUmemploymentAmount,b.MProvidentAmount,b.MProvidentAdditionalAmount,");
			stringBuilder.AppendLine(" (b.MBaseSalary-b.MRetirementSecurityAmount-b.MMedicalInsuranceAmount-b.MUmemploymentAmount-b.MProvidentAmount-b.MProvidentAdditionalAmount) SalaryBeforePIT ");
			stringBuilder.AppendLine(" from T_BD_Employees a ");
			stringBuilder.AppendLine(" left join T_BD_EmpPayrollBasicSet b on a.MItemID=b.MEmployeeID and b.MIsDelete=0");
			stringBuilder.AppendLine(" left join T_PA_PaySetting s on a.MOrgID=s.MOrgID and s.MIsActive=1 and s.MIsDelete=0 ");
			stringBuilder.AppendLine(" WHERE a.MOrgID=@MOrgID and a.MIsActive=1 and a.MIsDelete=0 AND ifnull(a.MStatus, '')<>'Leave' AND ifnull(b.MJoinTime, '0001/1/1')<@SalaryDate ");
			if (!string.IsNullOrWhiteSpace(employeeIds))
			{
				stringBuilder.AppendFormat(" AND a.MItemID in ('{0}') ", string.Join("','", employeeIds.Split(',')));
			}
			stringBuilder.AppendLine(" GROUP BY a.MItemID");
			MySqlParameter[] array = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLocalID", MySqlDbType.VarChar, 6),
				new MySqlParameter("@SalaryDate", MySqlDbType.DateTime)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = ctx.MLCID;
			array[2].Value = salaryDate.AddMonths(1);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataTable dt = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array).Tables[0];
			List<PASalaryPaymentListModel> list = ModelInfoManager.DataTableToList<PASalaryPaymentListModel>(dt);
			PAPITCalculateUtility pAPITCalculateUtility = new PAPITCalculateUtility(ctx, salaryDate, true, null);
			foreach (PASalaryPaymentListModel item in list)
			{
				item.PIT = pAPITCalculateUtility.CalculateSalaryPIT(item.MEmployeeID, item.SalaryBeforePIT);
				item.SalaryAfterPIT = Math.Round(item.SalaryBeforePIT - item.PIT, 2, MidpointRounding.AwayFromZero);
			}
			return list;
		}

		private static string[] GetLables(List<DateTime> monthList)
		{
			List<string> list = new List<string>();
			int count = monthList.Count;
			string text = "{0}-{1}";
			for (int i = 0; i < count; i++)
			{
				List<string> list2 = list;
				string format = text;
				DateTime dateTime = monthList[i];
				object arg = dateTime.Year;
				dateTime = monthList[i];
				list2.Add(string.Format(format, arg, dateTime.Month));
			}
			return list.ToArray();
		}

		private static string GetPayRunAutoNumber(MContext ctx)
		{
			string arg = "PR-";
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("select RIGHT(MNumber,4) AS MNo from T_PA_PayRun ");
			stringBuilder.AppendFormat("WHERE MOrgID=@MOrgID and MIsDelete=0 AND MNumber REGEXP '{0}[0-9]{{4}}' ", arg);
			stringBuilder.Append("ORDER BY MNumber DESC limit 1");
			MySqlParameter[] array = new MySqlParameter[1]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ctx.MOrgID;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			object single = dynamicDbHelperMySQL.GetSingle(stringBuilder.ToString(), array);
			if (single != null && single != DBNull.Value)
			{
				return $"{arg}{(Convert.ToInt32(single) + 1).ToString().PadLeft(4, '0')}";
			}
			return $"{arg}0001";
		}

		public static List<PASalaryPaymentTreeModel> GetSalaryPaymentPersonDetails(MContext ctx, string salaryPayId)
		{
			PASalaryPaymentModel salaryPaymentEditModel = GetSalaryPaymentEditModel(ctx, salaryPayId);
			BDPayrollDetailModel modelByEmpId = GetModelByEmpId(ctx, salaryPaymentEditModel.MEmployeeID);
			PAPITThresholdModel pITThresholdModel = PAPITRepository.GetPITThresholdModel(ctx, salaryPaymentEditModel.MDate, salaryPaymentEditModel.MEmployeeID);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("select t1.MPayItemID,t1.MParentPayItemID,t1.MPayItemName,t1.MQZDAmount,t1.MCoefficient,t1.MItemType,t1.MCreateDate,t2.MSalTaxAmount,abs(t2.MAmount) MAmount,ifnull(t2.MDesc,'') MDesc");
			stringBuilder.AppendLine("from (");
			stringBuilder.AppendLine("select t1.MItemID as MPayItemID,'0' as MParentPayItemID,t2.MName as MPayItemName,@MQZDAmount as MQZDAmount,t1.MCoefficient,t1.MItemType,t1.MCreateDate");
			stringBuilder.AppendLine("from T_PA_PayItemGroup t1");
			stringBuilder.AppendLine("join T_PA_PayItemGroup_L t2 on t1.MItemID=t2.MParentID and t2.MLocaleID=@MLocaleID and t2.MIsDelete=0 ");
			stringBuilder.AppendFormat("where t1.MOrgID=@MOrgID and t1.MIsDelete=0 and t1.MItemType<{0} ", 2000);
			stringBuilder.AppendLine("union all");
			stringBuilder.AppendLine("select t1.MItemID as MPayItemID,t1.MGroupID as MParentPayItemID,t2.MName as MPayItemName,@MQZDAmount as MQZDAmount,t3.MCoefficient,t1.MItemType,t1.MCreateDate");
			stringBuilder.AppendLine("from T_PA_PayItem t1");
			stringBuilder.AppendLine("join T_PA_PayItem_L t2 on t1.MItemID=t2.MParentID and t2.MLocaleID=@MLocaleID and t2.MIsDelete=0 ");
			stringBuilder.AppendLine("join T_PA_PayItemGroup t3 on t1.MGroupID=t3.MItemID ");
			stringBuilder.AppendFormat("where t1.MOrgID=@MOrgID and t1.MIsDelete=0 and t1.MItemType<{0} ", 2000);
			stringBuilder.AppendLine(") t1");
			stringBuilder.AppendLine("right join (");
			stringBuilder.AppendLine("select t1.MTaxSalary as MSalTaxAmount,t2.MPayItemID,ifnull(t2.MAmount,0) as MAmount,ifnull(t2.MDesc,'') MDesc");
			stringBuilder.AppendLine("from T_PA_SalaryPayment t1");
			stringBuilder.AppendLine("join T_PA_SalaryPaymentEntry t2 on t1.MID=t2.MID and t2.MIsDelete=0 ");
			stringBuilder.AppendLine("where t1.MOrgID=@MOrgID and t1.MID=@MSalaryPayID and t1.MIsDelete=0 and t2.MIsDelete=0");
			stringBuilder.AppendLine(") t2 on t1.MPayItemID=t2.MPayItemID");
			stringBuilder.AppendLine("where ifnull(t1.MPayItemID, '')<>''");
			stringBuilder.AppendLine("union all");
			stringBuilder.AppendLine("select t3.MPayItemID,t3.MParentPayItemID,t3.MPayItemName,t3.MQZDAmount,t3.MCoefficient,t3.MItemType,t3.MCreateDate,0 as MSalTaxAmount,0 as  MAmount,'' as MDesc");
			stringBuilder.AppendLine("from (");
			stringBuilder.AppendLine("select t1.MItemID as MPayItemID,'0' as MParentPayItemID,t2.MName as MPayItemName,@MQZDAmount as MQZDAmount,t1.MCoefficient,t1.MItemType,t1.MCreateDate");
			stringBuilder.AppendLine("from T_PA_PayItemGroup t1");
			stringBuilder.AppendLine("join T_PA_PayItemGroup_L t2 on t1.MItemID=t2.MParentID and t2.MLocaleID=@MLocaleID and t2.MIsDelete=0 ");
			stringBuilder.AppendFormat("where t1.MOrgID=@MOrgID and t1.MIsDelete=0 and t1.MItemType<{0} and t1.MIsActive=1 ", 2000);
			stringBuilder.AppendLine("union all");
			stringBuilder.AppendLine("select t1.MItemID as MPayItemID,t1.MGroupID as MParentPayItemID,t2.MName as MPayItemName,@MQZDAmount as MQZDAmount,t3.MCoefficient,t1.MItemType,t1.MCreateDate");
			stringBuilder.AppendLine("from T_PA_PayItem t1");
			stringBuilder.AppendLine("join T_PA_PayItem_L t2 on t1.MItemID=t2.MParentID and t2.MLocaleID=@MLocaleID and t2.MIsDelete=0 ");
			stringBuilder.AppendLine("join T_PA_PayItemGroup t3 on t1.MGroupID=t3.MItemID ");
			stringBuilder.AppendFormat("where t1.MOrgID=@MOrgID and t1.MIsDelete=0 and t1.MItemType<{0} and t1.MIsActive=1 ", 2000);
			stringBuilder.AppendLine(") t3");
			stringBuilder.AppendLine("where MPayItemID not in (select MPayItemID from T_PA_SalaryPaymentEntry where MOrgID=@MOrgID and MID=@MSalaryPayID and MIsDelete=0)");
			stringBuilder.AppendLine("order by convert(MItemType,signed), MCreateDate");
			MySqlParameter[] array = new MySqlParameter[12]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 6),
				new MySqlParameter("@MQZDAmount", MySqlDbType.Decimal, 23),
				new MySqlParameter("@MBaseSalary", MySqlDbType.Decimal, 23),
				new MySqlParameter("@MProvidentAmount", MySqlDbType.Decimal, 23),
				new MySqlParameter("@MProvidentAdditionalAmount", MySqlDbType.Decimal, 23),
				new MySqlParameter("@MRetirementSecurityAmount", MySqlDbType.Decimal, 23),
				new MySqlParameter("@MMedicalInsuranceAmount", MySqlDbType.Decimal, 23),
				new MySqlParameter("@MUmemploymentAmount", MySqlDbType.Decimal, 23),
				new MySqlParameter("@MSocialSecurityAmt", MySqlDbType.Decimal, 23),
				new MySqlParameter("@MProvidentAllAmount", MySqlDbType.Decimal, 23),
				new MySqlParameter("@MSalaryPayID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = ctx.MLCID;
			array[2].Value = pITThresholdModel.MAmount;
			array[3].Value = modelByEmpId.MBaseSalary;
			array[4].Value = modelByEmpId.MProvidentAmount;
			array[5].Value = modelByEmpId.MProvidentAdditionalAmount;
			array[6].Value = modelByEmpId.MRetirementSecurityAmount;
			array[7].Value = modelByEmpId.MMedicalInsuranceAmount;
			array[8].Value = modelByEmpId.MUmemploymentAmount;
			array[9].Value = modelByEmpId.MRetirementSecurityAmount + modelByEmpId.MMedicalInsuranceAmount + modelByEmpId.MUmemploymentAmount;
			array[10].Value = modelByEmpId.MProvidentAmount + modelByEmpId.MProvidentAdditionalAmount;
			array[11].Value = salaryPayId;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataTable dt = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array).Tables[0];
			return ModelInfoManager.DataTableToList<PASalaryPaymentTreeModel>(dt);
		}

		public static OperationResult SalaryPaymentUpdate(MContext ctx, PASalaryPaymentModel spModel)
		{
			OperationResult operationResult = new OperationResult();
			List<CommandInfo> list = new List<CommandInfo>();
			PASalaryPaymentModel dataEditModel = ModelInfoManager.GetDataEditModel<PASalaryPaymentModel>(ctx, spModel.MID, false, true);
			if (dataEditModel == null)
			{
				return operationResult;
			}
			IEnumerable<string> comSalaryItemIdList = from f in PAPayItemRepository.GetCompanySalaryItemList(ctx)
			select f.MItemID;
			List<string> list2 = (from f in dataEditModel.SalaryPaymentEntry
			where !comSalaryItemIdList.Contains(f.MPayItemID)
			select f into s
			select s.MEntryID).ToList();
			if (list2.Count > 0)
			{
				list.AddRange(ModelInfoManager.GetDeleteCmd<PASalaryPaymentEntryModel>(ctx, list2));
			}
			List<PASalaryPaymentEntryModel> list3 = (from f in dataEditModel.SalaryPaymentEntry
			where comSalaryItemIdList.Contains(f.MPayItemID)
			select f).ToList();
			list3.ForEach(delegate(PASalaryPaymentEntryModel f)
			{
				f.MEntryID = string.Empty;
			});
			list.AddRange(ModelInfoManager.GetInsertOrUpdateCmds(ctx, list3, null, true));
			dataEditModel.MStatus = 1;
			dataEditModel.MNetSalary = Math.Round(spModel.MNetSalary, 2, MidpointRounding.AwayFromZero);
			dataEditModel.MTaxSalary = Math.Round(spModel.MTaxSalary, 2, MidpointRounding.AwayFromZero);
			spModel.MEmployeeID = dataEditModel.MEmployeeID;
			dataEditModel.SalaryPaymentEntry = spModel.SalaryPaymentEntry;
			list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<PASalaryPaymentModel>(ctx, dataEditModel, null, true));
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num = dynamicDbHelperMySQL.ExecuteSqlTran(list);
			if (num <= 0)
			{
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "salaryPaymentFailed", "salary payment failed.")
				});
			}
			return operationResult;
		}

		private static BDPayrollDetailModel GetModelByEmpId(MContext ctx, string empId)
		{
			List<BDPayrollDetailModel> dataModelList = ModelInfoManager.GetDataModelList<BDPayrollDetailModel>(ctx, new SqlWhere().Equal(" MEmployeeID ", empId), false, false);
			return (dataModelList.Count > 0) ? dataModelList[0] : ModelInfoManager.GetEmptyDataEditModel<BDPayrollDetailModel>(ctx);
		}

		public static PASalaryToIVPayEntryModel GetSalaryToIvPayEntryModel(MContext ctx, string salaryPayMentId)
		{
			string sql = " SELECT * FROM t_pa_salarytoivpayentry WHERE MSalaryPaymentID=@MSalaryPaymentID AND MOrgID=@MOrgID AND MIsDelete=0 ";
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MSalaryPaymentID", salaryPayMentId)
			};
			return ModelInfoManager.GetDataModel<PASalaryToIVPayEntryModel>(ctx, sql, cmdParms);
		}

		public static string GetSalaryPaymentListByBillId(MContext ctx, string billId)
		{
			string sql = " SELECT t3.MRunID FROM t_iv_verification t1\r\n                            LEFT JOIN t_pa_salarytoivpayentry t2 \r\n                            ON t1.MTargetBillID = t2.MID AND t2.MOrgID = @MOrgID AND t2.MIsDelete = 0\r\n                            LEFT JOIN t_pa_salarypayment t3 \r\n                            ON t2.MSalaryPaymentID = t3.MID AND t3.MOrgID = @MOrgID AND t3.MIsDelete = 0\r\n                            WHERE t1.MSourceBillID = @MSourceBillID AND t1.MOrgID = @MOrgID AND t1.MIsDelete = 0 ";
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MSourceBillID", billId)
			};
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			object single = dynamicDbHelperMySQL.GetSingle(sql, cmdParms);
			return (single == null) ? "" : single.ToString();
		}
	}
}
