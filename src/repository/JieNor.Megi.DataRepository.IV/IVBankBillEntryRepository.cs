using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.MultiLanguage;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.Core.Utility;
using JieNor.Megi.DataModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.DataRepository.REG;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace JieNor.Megi.DataRepository.IV
{
	public class IVBankBillEntryRepository : DataServiceT<IVBankBillEntryModel>
	{
		private static object _lock = new object();

		public static List<IVBankBillStatementModel> GetBankBillStatementList(MContext ctx, RPTBankStatementFilterModel filter)
		{
			if (filter.MFromDate > filter.MEndDate)
			{
				DateTime mFromDate = filter.MFromDate;
				filter.MFromDate = filter.MEndDate;
				filter.MEndDate = mFromDate;
			}
			DateTime mEndDate = filter.MEndDate;
			filter.MEndDate = mEndDate.AddDays(1.0);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("SELECT a.*,b.MImportDate from T_IV_BankBillEntry a\r\n                                   INNER JOIN T_IV_BankBill b ON a.MID=b.MID\r\n                                   WHERE a.MIsDelete=0 AND IFNULL(a.MParentID,'')='' AND  b.MBankID=@MBankID AND MDate BETWEEN @MFromDate AND @MEndDate ");
			if (filter.MIsReconciled)
			{
				stringBuilder.Append(" AND exists(SELECT 1 FROM T_IV_BankBillReconcile c \r\n                                                  WHERE c.MBankBillEntryID=a.MEntryID)");
			}
			stringBuilder.Append(" ORDER BY MDate, MTime");
			mEndDate = filter.MEndDate;
			int year = mEndDate.Year;
			mEndDate = filter.MEndDate;
			int month = mEndDate.Month;
			mEndDate = filter.MEndDate;
			filter.MEndDate = new DateTime(year, month, mEndDate.Day, 11, 59, 59);
			MySqlParameter[] cmdParms = new MySqlParameter[3]
			{
				new MySqlParameter("@MBankID", filter.MBankAccountID),
				new MySqlParameter("@MFromDate", filter.MFromDate),
				new MySqlParameter("@MEndDate", filter.MEndDate)
			};
			return ModelInfoManager.GetDataModelBySql<IVBankBillStatementModel>(ctx, stringBuilder.ToString(), cmdParms);
		}

		public static IVBankBillEntryModel GetIVBankBillEntryModelByBankBillReconcileMID(MContext ctx, string bankBillReconcileMID)
		{
			string sql = "select bbe.* from T_IV_BankBillEntry as bbe \r\n                            inner join T_IV_BankBillReconcile as bbr on bbr.MOrgID = bbe.MOrgID and bbe.MEntryID=bbr.MBankBillEntryID AND bbr.MIsDelete=0 \r\n                            where bbr.MID=@MID and bbe.MOrgID=@MOrgID AND bbe.MIsDelete=0";
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@MID", bankBillReconcileMID),
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			List<IVBankBillEntryModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<IVBankBillEntryModel>(ctx, sql, cmdParms);
			if (dataModelBySql != null && dataModelBySql.Count > 0)
			{
				return dataModelBySql[0];
			}
			return new IVBankBillEntryModel();
		}

		public static List<NameValueModel> GetStatementGroupInfo(MContext ctx, DateTime startDate, DateTime endDate)
		{
			string empty = string.Empty;
			empty = "select t2.MBankID as MName,t1.MSeq, t1.MBalance as MValue from t_iv_bankbillentry t1\r\n                inner join t_iv_bankbill t2\r\n                INNER JOIN (\r\n                (select t4.MBankID, max(concat(DATE_FORMAT(MDate,'%Y%m%d'),DATE_FORMAT(MEndDate,'%Y%m%d'),DATE_FORMAT(MStartDate,'%Y%m%d'),DATE_FORMAT(t4.MCreateDate,'%Y%m%d%H%i%s'), lpad(MSeq, 5, '0'))) AS MDate from t_iv_bankbillentry t3\r\n                inner join t_iv_bankbill t4 on t3.mid=t4.MID\r\n                and t4.MOrgID=@MOrgID  AND t4.MIsDelete=0\r\n                where   t3.MOrgID=@MOrgID  and t3.MIsDelete=0\r\n                and t3.MDate <= @MEndDate\r\n                and t3.mCheckState <> '2'\r\n                and IFNULL(t3.MParentID,'')=''\r\n                GROUP BY T4.MBankID)\r\n                ) t ON t2.MBankID=t.MBankID AND (concat(DATE_FORMAT(t1.MDate,'%Y%m%d'),DATE_FORMAT(MEndDate,'%Y%m%d'),DATE_FORMAT(MStartDate,'%Y%m%d'),DATE_FORMAT(t2.MCreateDate,'%Y%m%d%H%i%s'), lpad(MSeq, 5, '0')))=t.MDate\r\n                where t1.MID= t2.MID\r\n                and t2.MOrgID = @MOrgID\r\n                and t1.MOrgID = @MOrgID\r\n                and t1.MDate <= @MEndDate\r\n                and t1.MIsDelete=0\r\n                and t1.mCheckState <> '2'\r\n                and IFNULL(t1.MParentID,'')=''\r\n                order by t2.MBankID , t1.mdate desc,Mtime desc";
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MEndDate", endDate)
			};
			return ModelInfoManager.GetDataModelBySql<NameValueModel>(ctx, empty, cmdParms);
		}

		public static OperationResult UpdateBankBillEntryList(MContext ctx, List<IVBankBillEntryModel> entryList)
		{
			List<CommandInfo> updateBankBillEntryCmd = GetUpdateBankBillEntryCmd(ctx, entryList);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num = 0;
			lock (_lock)
			{
				num = dynamicDbHelperMySQL.ExecuteSqlTran(updateBankBillEntryCmd);
			}
			OperationResult operationResult = new OperationResult();
			operationResult.Success = (num > 0);
			return operationResult;
		}

		public static List<NameValueModel> GetCashCodingGroupInfo(MContext ctx, DateTime startDate, DateTime endDate, string[] accountIds = null)
		{
			string text = string.Empty;
			if (accountIds != null && accountIds.Length != 0)
			{
				foreach (string str in accountIds)
				{
					text = text + "'" + str + "',";
				}
			}
			string sql = "SELECT b.MBankID as MName,COUNT(*) as MValue from T_IV_BankBillEntry a\r\n                            INNER JOIN T_IV_BankBill b ON a.MOrgID = b.MOrgID and  a.MID=b.MID AND b.MIsDelete=0 \r\n                            WHERE a.MIsDelete=0 AND b.MOrgID=@MOrgID AND IFNULL(a.MParentID,'')=''\r\n                            AND a.MDate >=@StartDate AND a.MDate <= @EndDate AND a.MCheckState <> 2 " + ((text.Length > 0) ? (" AND b.MBankID in (" + text + ") ") : " ") + " AND NOT exists(SELECT 1 FROM T_IV_BankBillReconcile c \r\n                                            WHERE c.MOrgID = @MOrgID and  c.MBankBillEntryID=a.MEntryID AND c.MIsDelete=0  and b.MBankID is not null )\r\n                            GROUP BY b.MBankID";
			MySqlParameter[] cmdParms = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@StartDate", startDate),
				new MySqlParameter("@EndDate", endDate)
			};
			return ModelInfoManager.GetDataModelBySql<NameValueModel>(ctx, sql, cmdParms);
		}

		public static OperationResult UpdateCashCoding(MContext ctx, IVCashCodingEditModel model)
		{
			BDBankAccountRepository bDBankAccountRepository = new BDBankAccountRepository();
			BDBankAccountEditModel bDBankAccountEditModel = bDBankAccountRepository.GetBDBankAccountEditModel(ctx, model.MBankID);
			if (bDBankAccountEditModel == null || string.IsNullOrEmpty(bDBankAccountEditModel.MItemID))
			{
				return new OperationResult
				{
					Success = false
				};
			}
			List<ValidationError> list = new List<ValidationError>();
			int num = 0;
			int num2 = 0;
			List<CommandInfo> updateCashCodingSql = GetUpdateCashCodingSql(ctx, model, bDBankAccountEditModel, ref list, out num, out num2);
			int num3 = num - num2;
			OperationResult operationResult = new OperationResult();
			list.ForEach(delegate(ValidationError t)
			{
				t.Message = "- " + t.Message;
			});
			string arg = list.ToString("<br/>");
			if (num == num2)
			{
				operationResult.Success = false;
				operationResult.Message = string.Format("{0}<br/>{1}", COMMultiLangRepository.GetText(LangModule.Common, ctx.MLCID, "SaveFailture"), arg);
				return operationResult;
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num4 = dynamicDbHelperMySQL.ExecuteSqlTran(updateCashCodingSql, true);
			operationResult.Success = (num4 > 0);
			if (operationResult.Success)
			{
				if (num2 > 0)
				{
					operationResult.Message = COMMultiLangRepository.GetTextFormat(ctx.MLCID, LangModule.Common, "SaveResult", "{0} item(s) save successful!<br/>{1} item(s) save failure!<br/>{2}", num3, num2, "");
				}
				else
				{
					operationResult.Message = COMMultiLangRepository.GetText(LangModule.Common, ctx.MLCID, "SaveSuccessful");
				}
			}
			else
			{
				operationResult.Message = COMMultiLangRepository.GetText(LangModule.Common, ctx.MLCID, "SaveFailture");
			}
			return operationResult;
		}

		public static IVBankBillStatusCountModel GetBankbillVoucherStatusSummary(MContext ctx, int year, int period)
		{
			string sql = " SELECT\r\n                            SUM(\r\n                                CASE \r\n                                    WHEN T2.MVoucherStatus = 1 THEN 1 ELSE 0 \r\n                                END\r\n                               ) AS NonGeneratedCount,\r\n                            SUM(\r\n                                CASE\r\n                                    WHEN T2.MVoucherStatus = 2 THEN 1 ELSE 0\r\n                                END\r\n                                ) AS UnGenerateCount,\r\n                            SUM(\r\n                                CASE \r\n                                    WHEN T2.MVoucherStatus = 3 THEN 1 ELSE 0\r\n                                END\r\n                               ) AS GeneratedCount , t1.MBankID,t1.MBankTypeID\r\n                            FROM t_iv_bankbill t1 \r\n                            INNER JOIN t_iv_bankbillentry t2\r\n                                ON T1.MID = T2.MID\r\n                            WHERE\r\n                                IFNULL(MParentID, '') = '' AND T2.MIsDelete = 0 \r\n                                AND T2.MOrgID = @MOrgID AND YEAR (T2.MDate) = @MYear AND MONTH (T2.MDate) = @MPeriod ";
			MySqlParameter[] cmdParms = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MYear", year),
				new MySqlParameter("@MPeriod", period)
			};
			return ModelInfoManager.GetDataModel<IVBankBillStatusCountModel>(ctx, sql, cmdParms);
		}

		public static bool HasBankBillData(MContext ctx, string bankId)
		{
			string strSql = "SELECT 1 from T_IV_BankBillEntry a\r\n                                   INNER JOIN T_IV_BankBill b ON a.MOrgID = b.MOrgID and  a.MID=b.MID AND b.MIsDelete=0 \r\n                                   WHERE b.MBankID=@MBankID and a.MOrgID=@MOrgID and a.MIsDelete=0";
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("MBankID", bankId),
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return dynamicDbHelperMySQL.Exists(strSql, cmdParms);
		}

		public static List<IVBankBillEntryModel> GetCashCodingList(MContext ctx, List<string> bankBillEntryIdList)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("SELECT *,MEntryID AS MSrcEntryID,\r\n                                    (CASE WHEN IFNULL(MParentID,'')='' THEN MEntryID ELSE MParentID END) AS MRootID,\r\n                                    MSpentAmt AS MSrcSpentAmt,\r\n                                    MReceivedAmt AS MSrcReceivedAmt, \r\n                                    (CASE WHEN MSpentAmt>0 THEN 'Supplier' ELSE 'Customer' END) AS MContactType  FROM T_IV_BankBillEntry WHERE MOrgID=@MOrgID AND MIsDelete=0 AND MParentID IN (");
			List<MySqlParameter> list = new List<MySqlParameter>();
			list.Add(new MySqlParameter("@MOrgID", ctx.MOrgID));
			int num = 1;
			foreach (string bankBillEntryId in bankBillEntryIdList)
			{
				string text = $"@Param{num}";
				stringBuilder.AppendFormat("{0},", text);
				list.Add(new MySqlParameter(text, bankBillEntryId));
				num++;
			}
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
			stringBuilder.Append(")");
			return ModelInfoManager.GetDataModelBySql<IVBankBillEntryModel>(ctx, stringBuilder.ToString(), list.ToArray());
		}

		public static DataGridJson<IVBankBillEntryModel> GetCashCodingList(MContext ctx, IVBankBillEntryListFilterModel filter)
		{
			SqlQuery sqlQuery = new SqlQuery();
			sqlQuery.SqlWhere = filter;
			string str = "SELECT a.MEntryID,a.MParentID,a.MID,a.MSeq,a.MDate,a.MTime,a.MTransType,a.MTransNo,CONVERT(a.MTransAcctName USING gbk ) AS MTransAcctName,a.MTransAcctNo,a.MBillType,\r\n                                    MSpentSplitAmt as MSpentAmt,a.MReceivedSplitAmt AS MReceivedAmt,a.MBalance,a.MCheckBalance,a.MCheckState,a.MCheckerID,\r\n                                    a.MCheckDate,a.MRef,a.MUserRef,a.MDesc,a.MPrevState,a.MContactID,a.MTaxID,a.MTrackItem1,a.MTrackItem2,a.MTrackItem3,a.MTrackItem4,a.MTrackItem5,a.MAccountID,\r\n                                    a.MEntryID AS MSrcEntryID,\r\n                                    (CASE WHEN IFNULL(a.MParentID,'')='' THEN a.MEntryID ELSE a.MParentID END) AS MRootID,\r\n                                    a.MSpentAmt AS MSrcSpentAmt,\r\n                                    a.MReceivedAmt AS MSrcReceivedAmt, \r\n                                    (CASE WHEN a.MSpentAmt>0 THEN 'Supplier' ELSE 'Customer' END) AS MContactType \r\n                                    from T_IV_BankBillEntry a\r\n                                   INNER JOIN T_IV_BankBill b ON a.MOrgID = b.MOrgID and  a.MID=b.MID AND b.MIsDelete=0";
			str = (sqlQuery.SelectString = str + GetCashCodingListFilter(ctx, filter, ref sqlQuery));
			SqlOrderDir sqlOrderDir = string.IsNullOrWhiteSpace(filter.Order) ? SqlOrderDir.Desc : ((filter.Order.ToLower() == "desc") ? SqlOrderDir.Desc : SqlOrderDir.Asc);
			string text2 = "";
			text2 = ((!string.IsNullOrWhiteSpace(filter.Sort) && !(filter.Sort.ToLower() == "mdate")) ? ((!(filter.Sort.ToLower() == "mtransacctname")) ? $"{filter.Sort} {sqlOrderDir}" : string.Format("CONVERT(a.MTransAcctName USING gbk ) {0},a.MDate {0},a.MCreateDate {0}, a.MSeq {0}", sqlOrderDir)) : string.Format("a.MDate {0},a.MCreateDate {0}, a.MSeq {0}", sqlOrderDir));
			sqlQuery.SqlWhere.OrderBy(text2);
			return ModelInfoManager.GetPageDataModelListBySql<IVBankBillEntryModel>(ctx, sqlQuery);
		}

		private static string GetCashCodingListFilter(MContext ctx, IVBankBillEntryListFilterModel filter, ref SqlQuery query)
		{
			DateTime dateTime = filter.StartDate;
			DateTime date = dateTime.Date;
			dateTime = ctx.MBeginDate;
			if (date < dateTime.Date)
			{
				dateTime = ctx.MBeginDate;
				filter.StartDate = dateTime.Date;
			}
			dateTime = filter.EndDate;
			if (dateTime.Year <= 1900)
			{
				dateTime = DateTime.Now;
				filter.EndDate = new DateTime(dateTime.Year + 100, 11, 31);
			}
			string str = "";
			if (!string.IsNullOrEmpty(filter.TransAcctName))
			{
				str += " LEFT JOIN T_BD_Contacts c on c.MOrgID = a.MOrgID and  LEFT(a.MContactID,32)=c.MItemID AND c.MIsDelete=0 \r\n                                LEFT JOIN T_BD_Contacts_L d on c.MOrgID = d.MOrgID and c.MItemID=d.MParentID And d.MLocaleID=@MLocaleID AND d.MIsDelete=0 \r\n                                left join T_BD_Employees_L e on e.MOrgID = a.MOrgID and e.MParentID=LEFT(a.MContactID,32) and e.MLocaleID=@MLocaleID AND e.MIsDelete=0 ";
			}
			str += " WHERE a.MIsDelete=0 AND b.MBankID=@MBankID  AND MDate<=@EndDate AND MDate >=@StartDate AND a.MCheckState<>2 AND IFNULL(a.MParentID,'')=''\r\n                            AND NOT exists(SELECT 1 FROM T_IV_BankBillReconcile c WHERE c.MOrgID = @MOrgID and  c.MBankBillEntryID=a.MEntryID AND c.MIsDelete=0 ) ";
			filter.EndDate = filter.EndDate.ToDayLastSecond();
			query.AddParameter(new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
			{
				Value = ctx.MOrgID
			});
			query.AddParameter(new MySqlParameter("@StartDate", MySqlDbType.DateTime)
			{
				Value = (object)filter.StartDate
			});
			query.AddParameter(new MySqlParameter("@EndDate", MySqlDbType.DateTime)
			{
				Value = (object)filter.EndDate
			});
			query.AddParameter(new MySqlParameter("@MBankID", MySqlDbType.VarChar, 36)
			{
				Value = filter.MBankID
			});
			query.AddParameter(new MySqlParameter("@MLocaleID", ctx.MLCID));
			if (ctx.MOrgVersionID == Convert.ToInt32(1))
			{
				str += $" AND MVoucherStatus<>{Convert.ToInt32(IVBankBillVoucherStatus.UnGenerate)} ";
			}
			if (filter.ExactDate.HasValue)
			{
				str += " and date(a.MDate)=@ExactDate";
				SqlQuery obj = query;
				MySqlParameter mySqlParameter = new MySqlParameter("@ExactDate", MySqlDbType.DateTime);
				dateTime = filter.ExactDate.Value;
				mySqlParameter.Value = dateTime.Date;
				obj.AddParameter(mySqlParameter);
			}
			if (!string.IsNullOrEmpty(filter.TransAcctName))
			{
				str += string.Format(" and (a.MTransAcctName like concat('%', @TransAcctName, '%') \r\n                                or F_GetUserName(e.MFirstName,e.MLastName) like CONCAT('%',@TransAcctName, '%')\r\n                                or convert(AES_DECRYPT(d.MName,'{0}') using utf8) like CONCAT('%',@TransAcctName,'%')) ", "JieNor-001");
				query.AddParameter(new MySqlParameter("@TransAcctName", MySqlDbType.VarChar, 200)
				{
					Value = filter.TransAcctName
				});
			}
			if (!string.IsNullOrEmpty(filter.MDesc))
			{
				str += " and (a.MDesc like concat('%', @MDesc, '%') or a.MUserRef LIKE concat('%',@MDesc,'%')) ";
				query.AddParameter(new MySqlParameter("@MDesc", MySqlDbType.VarChar, 500)
				{
					Value = filter.MDesc
				});
			}
			if (filter.SrcFrom > 0)
			{
				if (filter.SrcFrom == 1)
				{
					str += " and a.MReceivedAmt>0 ";
				}
				else if (filter.SrcFrom == 2)
				{
					str += " and a.MSpentAmt>0 ";
				}
			}
			if (filter.IsExactAmount)
			{
				if (filter.AmountFrom.HasValue)
				{
					str += " and (a.MSpentAmt=@Amount or a.MReceivedAmt=@Amount)";
					query.AddParameter(new MySqlParameter("@Amount", MySqlDbType.Decimal)
					{
						Value = (object)filter.AmountFrom.Value
					});
				}
			}
			else
			{
				if (filter.AmountFrom.HasValue)
				{
					str += " and ((a.MSpentAmt>=@AmountFrom and a.MReceivedAmt=0) or (a.MReceivedAmt>=@AmountFrom and a.MSpentAmt=0)) ";
					query.AddParameter(new MySqlParameter("@AmountFrom", MySqlDbType.Decimal)
					{
						Value = (object)filter.AmountFrom.Value
					});
				}
				if (filter.AmountTo.HasValue)
				{
					str += " and ((a.MSpentAmt<=@AmountTo and a.MReceivedAmt=0) or (a.MReceivedAmt<=@AmountTo and a.MSpentAmt=0)) ";
					query.AddParameter(new MySqlParameter("@AmountTo", MySqlDbType.Decimal)
					{
						Value = (object)filter.AmountTo.Value
					});
				}
			}
			return str;
		}

		private static List<CommandInfo> GetUpdateCashCodingSql(MContext ctx, IVCashCodingEditModel model, BDBankAccountEditModel bankModel, ref List<ValidationError> validationErrors, out int totalCount, out int errorCount)
		{
			List<BDAccountModel> acctList = null;
			if (ctx.MRegProgress == 15)
			{
				BDAccountRepository bDAccountRepository = new BDAccountRepository();
				acctList = bDAccountRepository.GetAccountListWithCheckType(ctx, null, false, false);
			}
			List<CommandInfo> list = new List<CommandInfo>();
			model.MBankBillEntryList = (from a in model.MBankBillEntryList
			where !(a.MReceivedAmt == decimal.Zero) || !(a.MSpentAmt == decimal.Zero)
			select a).ToList();
			List<string> list2 = (from t in model.MBankBillEntryList
			select t.MRootID).Distinct().ToList();
			totalCount = list2.Count;
			errorCount = 0;
			string text = "";
			List<GLReceiveAndBankEntry> list3 = new List<GLReceiveAndBankEntry>();
			foreach (string item in list2)
			{
				List<IVBankBillEntryModel> list4 = (from t in model.MBankBillEntryList
				where t.MRootID == item
				select t).ToList();
				if (list4 == null || list4.Count == 0)
				{
					text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "ParameterError", "Parameter error");
					validationErrors.Add(new ValidationError(text));
					errorCount++;
					continue;
				}
				IVBankBillEntryModel iVBankBillEntryModel = (from t in list4
				where t.MEntryID == t.MRootID
				select t).FirstOrDefault();
				if (iVBankBillEntryModel == null)
				{
					text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "ParameterError", "Parameter error");
					validationErrors.Add(new ValidationError(text));
					errorCount++;
					continue;
				}
				decimal d = list4.Sum((IVBankBillEntryModel t) => t.MSpentAmt);
				decimal d2 = list4.Sum((IVBankBillEntryModel t) => t.MReceivedAmt);
				int num;
				if ((!(d == decimal.Zero) || !(d2 == decimal.Zero)) && !(d != iVBankBillEntryModel.MSrcSpentAmt))
				{
					num = ((d2 != iVBankBillEntryModel.MSrcReceivedAmt) ? 1 : 0);
					goto IL_0238;
				}
				num = 1;
				goto IL_0238;
				IL_0238:
				if (num != 0)
				{
					text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "ParameterError", "Parameter error");
					validationErrors.Add(new ValidationError(text));
					errorCount++;
				}
				else
				{
					OperationResult operationResult = GLInterfaceRepository.IsPeriodUnclosed(ctx, iVBankBillEntryModel.MDate);
					if (!operationResult.Success)
					{
						validationErrors.Add(new ValidationError(operationResult.Message));
						errorCount++;
					}
					else if (!IsCheckTypeOK(ctx, acctList, list4))
					{
						text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "CheckTypeMissing", "Check type missing");
						validationErrors.Add(new ValidationError(text));
						errorCount++;
					}
					else if (ctx.MOrgVersionID == Convert.ToInt32(1) && iVBankBillEntryModel.MVoucherStatus == Convert.ToInt32(IVBankBillVoucherStatus.UnGenerate))
					{
						errorCount++;
					}
					else
					{
						BDExchangeRateModel enableExchangeRate = BDExchangeRateRepository.GetEnableExchangeRate(ctx, iVBankBillEntryModel.MDate, bankModel.MCyID);
						if (enableExchangeRate == null)
						{
							text = COMMultiLangRepository.GetTextFormat(ctx.MLCID, LangModule.Common, "NoEffectiveExchangeRate", "{0} 没有有效汇率!", iVBankBillEntryModel.MDate.ToOrgZoneDateFormat(null));
							validationErrors.Add(new ValidationError(text));
							errorCount++;
						}
						else
						{
							model.MExchangeRate = enableExchangeRate.MRate;
							list.AddRange(GetUpdateCashCodingSql(ctx, model, bankModel, iVBankBillEntryModel, list4, ref validationErrors, ref list3));
						}
					}
				}
			}
			List<IVReceiveModel> list5 = (from a in list3
			where a.receive != null
			select a.receive).ToList();
			list.AddRange(ModelInfoManager.GetBatchInsertOrUpdateCmds(ctx, list5, null, "receives"));
			List<IVReceiveEntryModel> modelData = list5.SelectMany((IVReceiveModel a) => a.ReceiveEntry).ToList();
			list.AddRange(ModelInfoManager.GetBatchInsertOrUpdateCmds(ctx, modelData, null, "receiveEntrys"));
			List<IVPaymentModel> list6 = (from a in list3
			where a.payment != null
			select a.payment).ToList();
			list.AddRange(ModelInfoManager.GetBatchInsertOrUpdateCmds(ctx, list6, null, "payments"));
			List<IVPaymentEntryModel> modelData2 = list6.SelectMany((IVPaymentModel a) => a.PaymentEntry).ToList();
			list.AddRange(ModelInfoManager.GetBatchInsertOrUpdateCmds(ctx, modelData2, null, "paymentEntrys"));
			list.AddRange(GetGenerateVoucherCmds(ctx, list3));
			return list;
		}

		public static OperationResult UpdateBankBillVoucherStatus(MContext ctx, List<string> entryIdList, IVBankBillVoucherStatus status)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			List<IVBankBillEntryModel> dataModelList = ModelInfoManager.GetDataModelList<IVBankBillEntryModel>(ctx, entryIdList);
			if (dataModelList == null || dataModelList.Count == 0)
			{
				return new OperationResult
				{
					Success = false
				};
			}
			List<ValidationError> list2 = new List<ValidationError>();
			foreach (IVBankBillEntryModel item in dataModelList)
			{
				OperationResult operationResult = GLInterfaceRepository.IsPeriodUnclosed(ctx, item.MDate);
				if (!operationResult.Success)
				{
					list2.Add(new ValidationError(operationResult.Message));
				}
				item.MVoucherStatus = Convert.ToInt32(status);
				list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<IVBankBillEntryModel>(ctx, item, new List<string>
				{
					"MEntryID",
					"MVoucherStatus"
				}, true));
			}
			if (list2.Count > 0)
			{
				return new OperationResult
				{
					Success = false,
					Message = list2.ToString("<br/>")
				};
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num = dynamicDbHelperMySQL.ExecuteSqlTran(list);
			return new OperationResult
			{
				Success = true
			};
		}

		private static bool IsCheckTypeOK(MContext ctx, List<BDAccountModel> acctList, List<IVBankBillEntryModel> bankBillEntryList)
		{
			if (ctx.MRegProgress != 15 || acctList == null)
			{
				return true;
			}
			foreach (IVBankBillEntryModel bankBillEntry in bankBillEntryList)
			{
				if (string.IsNullOrEmpty(bankBillEntry.MAccountID))
				{
					return false;
				}
				BDAccountModel bDAccountModel = (from t in acctList
				where t.MItemID == bankBillEntry.MAccountID
				select t).FirstOrDefault();
				if (bDAccountModel == null)
				{
					return false;
				}
				if (bDAccountModel.MCheckGroupModel.MContactID == CheckTypeStatusEnum.Required && (string.IsNullOrEmpty(bankBillEntry.MContactID) || Convert.ToInt32(bankBillEntry.MContactID.Split('_')[1]) == 4))
				{
					return false;
				}
				if (bDAccountModel.MCheckGroupModel.MEmployeeID == CheckTypeStatusEnum.Required && (string.IsNullOrEmpty(bankBillEntry.MContactID) || Convert.ToInt32(bankBillEntry.MContactID.Split('_')[1]) != 4))
				{
					return false;
				}
				if (bDAccountModel.MCheckGroupModel.MTrackItem1 == CheckTypeStatusEnum.Required && string.IsNullOrEmpty(bankBillEntry.MTrackItem1))
				{
					return false;
				}
				if (bDAccountModel.MCheckGroupModel.MTrackItem2 == CheckTypeStatusEnum.Required && string.IsNullOrEmpty(bankBillEntry.MTrackItem2))
				{
					return false;
				}
				if (bDAccountModel.MCheckGroupModel.MTrackItem3 == CheckTypeStatusEnum.Required && string.IsNullOrEmpty(bankBillEntry.MTrackItem3))
				{
					return false;
				}
				if (bDAccountModel.MCheckGroupModel.MTrackItem4 == CheckTypeStatusEnum.Required && string.IsNullOrEmpty(bankBillEntry.MTrackItem4))
				{
					return false;
				}
				if (bDAccountModel.MCheckGroupModel.MTrackItem5 == CheckTypeStatusEnum.Required && string.IsNullOrEmpty(bankBillEntry.MTrackItem5))
				{
					return false;
				}
			}
			return true;
		}

		private static List<CommandInfo> GetUpdateCashCodingSql(MContext ctx, IVCashCodingEditModel model, BDBankAccountEditModel bankModel, IVBankBillEntryModel srcBankbillEntryModel, List<IVBankBillEntryModel> bankBillEntryList, ref List<ValidationError> validationErrors, ref List<GLReceiveAndBankEntry> receiveAndBankEntrys)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			List<CommandInfo> updateBankBillEntryCmd = GetUpdateBankBillEntryCmd(ctx, bankBillEntryList);
			list.AddRange(updateBankBillEntryCmd);
			IVBankBillReconcileModel iVBankBillReconcileModel = new IVBankBillReconcileModel();
			iVBankBillReconcileModel.MBankBillEntryID = srcBankbillEntryModel.MEntryID;
			if (srcBankbillEntryModel.MSpentAmt > decimal.Zero)
			{
				iVBankBillReconcileModel.MTargetBillType = "Payment";
			}
			else
			{
				iVBankBillReconcileModel.MTargetBillType = "Receive";
			}
			iVBankBillReconcileModel.MSpentAmtFor = srcBankbillEntryModel.MSpentAmt;
			iVBankBillReconcileModel.MReceiveAmtFor = srcBankbillEntryModel.MReceivedAmt;
			iVBankBillReconcileModel.MOrgID = ctx.MOrgID;
			List<CommandInfo> insertOrUpdateCmd = ModelInfoManager.GetInsertOrUpdateCmd<IVBankBillReconcileModel>(ctx, iVBankBillReconcileModel, null, true);
			list.AddRange(insertOrUpdateCmd);
			var list2 = (from p in bankBillEntryList
			group p by new
			{
				p.MContactID,
				p.MDesc
			} into g
			select new
			{
				MContactID = g.Key.MContactID,
				MDesc = g.Key.MDesc,
				MSpentAmt = g.Sum((IVBankBillEntryModel p) => p.MSpentAmt),
				MReceiveAmt = g.Sum((IVBankBillEntryModel p) => p.MReceivedAmt)
			}).ToList();
			foreach (var item in list2)
			{
				List<IVBankBillEntryModel> list3 = (from t in bankBillEntryList
				where t.MContactID == item.MContactID && t.MDesc == item.MDesc
				select t).ToList();
				if (list3 != null && list3.Count != 0)
				{
					IVBankBillEntryModel iVBankBillEntryModel = list3[0];
					string contactId = "";
					BDContactType contactType = BDContactType.Other;
					if (!string.IsNullOrEmpty(iVBankBillEntryModel.MContactID))
					{
						string[] array = iVBankBillEntryModel.MContactID.Split('_');
						if (array.Length != 0)
						{
							contactId = array[0];
							contactType = (BDContactType)Convert.ToInt32(array[1]);
						}
					}
					if (item.MSpentAmt > decimal.Zero)
					{
						list.AddRange(GetAddPaymentCmd(ctx, model, iVBankBillReconcileModel.MID, contactId, contactType, iVBankBillEntryModel, list3, bankModel, ref validationErrors, ref receiveAndBankEntrys));
					}
					if (item.MReceiveAmt > decimal.Zero)
					{
						list.AddRange(GetAddReceiveCmd(ctx, model, iVBankBillReconcileModel.MID, contactId, contactType, iVBankBillEntryModel, list3, bankModel, ref validationErrors, ref receiveAndBankEntrys));
					}
				}
			}
			return list;
		}

		private static List<CommandInfo> GetUpdateBankBillEntryCmd(MContext ctx, List<IVBankBillEntryModel> bankBillEntryList)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			List<MySqlParameter> list2 = new List<MySqlParameter>();
			StringBuilder stringBuilder = new StringBuilder();
			if (bankBillEntryList.Count > 0)
			{
				int num = 1;
				foreach (IVBankBillEntryModel bankBillEntry in bankBillEntryList)
				{
					if (string.IsNullOrEmpty(bankBillEntry.MSrcEntryID))
					{
						bankBillEntry.MEntryID = "";
					}
					string mRef = bankBillEntry.MRef;
					bankBillEntry.MRef = bankBillEntry.MDesc;
					bankBillEntry.MDesc = mRef;
					bankBillEntry.MUserRef = mRef;
					bankBillEntry.MSpentSplitAmt = bankBillEntry.MSpentAmt;
					bankBillEntry.MReceivedSplitAmt = bankBillEntry.MReceivedAmt;
					if (!(bankBillEntry.MSpentAmt == decimal.Zero) || !(bankBillEntry.MReceivedAmt == decimal.Zero))
					{
						List<string> list3 = new List<string>
						{
							"MParentID",
							"MContactID",
							"MSpentSplitAmt",
							"MReceivedSplitAmt",
							"MAccountID",
							"MRef",
							"MUserRef",
							"MTaxID",
							"MTrackItem1",
							"MTrackItem2",
							"MTrackItem3",
							"MTrackItem4",
							"MTrackItem5"
						};
						if (bankBillEntry.MEntryID == bankBillEntry.MRootID)
						{
							bankBillEntry.MParentID = "";
						}
						else
						{
							bankBillEntry.MParentID = bankBillEntry.MRootID;
							bankBillEntry.MSeq = num;
							bankBillEntry.MVoucherStatus = Convert.ToInt32(IVBankBillVoucherStatus.NonGenerated);
							list3.Add("MSeq");
							list3.Add("MSpentAmt");
							list3.Add("MReceivedAmt");
							list3.Add("MVoucherStatus");
							list3.Add("MDesc");
						}
						list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<IVBankBillEntryModel>(ctx, bankBillEntry, list3, true));
						if (!string.IsNullOrEmpty(bankBillEntry.MEntryID))
						{
							string text = $"@Param{num}";
							stringBuilder.AppendFormat("{0},", text);
							list2.Add(new MySqlParameter(text, bankBillEntry.MEntryID));
							num++;
						}
					}
				}
			}
			IVBankBillEntryModel iVBankBillEntryModel = (from t in bankBillEntryList
			where t.MEntryID == t.MRootID
			select t).FirstOrDefault();
			string text2 = "UPDATE T_IV_BankBillEntry SET MIsDelete=1 WHERE MOrgID=@MOrgID AND MParentID=@MParentID AND MIsDelete=0 ";
			list2.Add(new MySqlParameter("@MOrgID", ctx.MOrgID));
			list2.Add(new MySqlParameter("@MParentID", iVBankBillEntryModel.MEntryID));
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
				text2 = $"{text2} AND MEntryID NOT IN ({stringBuilder.ToString()})";
			}
			List<CommandInfo> list4 = list;
			CommandInfo obj = new CommandInfo
			{
				CommandText = text2
			};
			DbParameter[] array = obj.Parameters = list2.ToArray();
			list4.Add(obj);
			return list;
		}

		private static List<CommandInfo> GetAddPaymentCmd(MContext ctx, IVCashCodingEditModel cashCodingModel, string recId, string contactId, BDContactType contactType, IVBankBillEntryModel groupEntryModel, List<IVBankBillEntryModel> bankBillEntrylList, BDBankAccountEditModel bankModel, ref List<ValidationError> validationErrors, ref List<GLReceiveAndBankEntry> receiveAndBankEntrys)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			string text = "Tax_Inclusive";
			IVPaymentModel iVPaymentModel = new IVPaymentModel();
			iVPaymentModel.MID = UUIDHelper.GetGuid();
			iVPaymentModel.IsNew = true;
			if (string.IsNullOrEmpty(contactId))
			{
				iVPaymentModel.MContactType = "Other";
				iVPaymentModel.MType = "Pay_Other";
			}
			else
			{
				switch (contactType)
				{
				case BDContactType.Supplier:
					iVPaymentModel.MContactType = "Supplier";
					iVPaymentModel.MType = "Pay_Purchase";
					break;
				case BDContactType.Customer:
					iVPaymentModel.MContactType = "Customer";
					iVPaymentModel.MType = "Pay_PurReturn";
					break;
				case BDContactType.Employee:
					iVPaymentModel.MContactType = "Employees";
					iVPaymentModel.MType = "Pay_Other";
					text = "No_Tax";
					break;
				default:
					iVPaymentModel.MContactType = "Other";
					iVPaymentModel.MType = "Pay_Other";
					break;
				}
			}
			iVPaymentModel.MBankID = cashCodingModel.MBankID;
			iVPaymentModel.MCyID = bankModel.MCyID;
			iVPaymentModel.MContactID = contactId;
			iVPaymentModel.MReference = groupEntryModel.MDesc;
			iVPaymentModel.MBizDate = groupEntryModel.MDate;
			iVPaymentModel.MOToLRate = cashCodingModel.MExchangeRate;
			iVPaymentModel.MLToORate = Math.Round(decimal.One / cashCodingModel.MExchangeRate, 6);
			iVPaymentModel.MOrgID = ctx.MOrgID;
			iVPaymentModel.MTaxID = text;
			iVPaymentModel.MSource = Convert.ToInt32(BillSourceType.Reconcile);
			iVPaymentModel.MCreateFrom = Convert.ToInt32(CreateFromType.Cashcoding);
			iVPaymentModel.MReconcileStatu = Convert.ToInt32(IVReconcileStatus.Completely);
			List<IVPaymentEntryModel> list2 = new List<IVPaymentEntryModel>();
			foreach (IVBankBillEntryModel bankBillEntryl in bankBillEntrylList)
			{
				decimal num = Math.Round(bankBillEntryl.MSpentAmt, 2);
				decimal num2 = Math.Round(num * iVPaymentModel.MOToLRate, 2);
				decimal num3 = default(decimal);
				decimal mTaxAmt = default(decimal);
				if (text != "No_Tax")
				{
					num3 = Math.Round(num - num / (decimal.One + bankBillEntryl.MTaxRate), 2);
					mTaxAmt = Math.Round(num3 * cashCodingModel.MExchangeRate, 2);
				}
				decimal num4 = num;
				decimal num5 = num2;
				IVPaymentEntryModel iVPaymentEntryModel = new IVPaymentEntryModel();
				iVPaymentEntryModel.MOToLRate = iVPaymentModel.MOToLRate;
				iVPaymentEntryModel.MLToORate = iVPaymentModel.MLToORate;
				iVPaymentEntryModel.MID = iVPaymentModel.MID;
				iVPaymentEntryModel.MQty = decimal.One;
				iVPaymentEntryModel.MDesc = (string.IsNullOrEmpty(bankBillEntryl.MRef) ? bankBillEntryl.MDesc : bankBillEntryl.MRef);
				iVPaymentEntryModel.MTaxID = bankBillEntryl.MTaxID;
				iVPaymentEntryModel.MTrackItem1 = bankBillEntryl.MTrackItem1;
				iVPaymentEntryModel.MTrackItem2 = bankBillEntryl.MTrackItem2;
				iVPaymentEntryModel.MTrackItem3 = bankBillEntryl.MTrackItem3;
				iVPaymentEntryModel.MTrackItem4 = bankBillEntryl.MTrackItem4;
				iVPaymentEntryModel.MTrackItem5 = bankBillEntryl.MTrackItem5;
				iVPaymentEntryModel.MTaxAmount = num2;
				iVPaymentEntryModel.MTaxAmountFor = num;
				iVPaymentEntryModel.MAmount = num5;
				iVPaymentEntryModel.MAmountFor = num4;
				iVPaymentEntryModel.MTaxAmt = mTaxAmt;
				iVPaymentEntryModel.MTaxAmtFor = num3;
				iVPaymentEntryModel.MPrice = bankBillEntryl.MSpentAmt;
				iVPaymentEntryModel.MAcctID = bankBillEntryl.MAccountID;
				IVPaymentModel iVPaymentModel2 = iVPaymentModel;
				iVPaymentModel2.MTaxTotalAmt += num2;
				IVPaymentModel iVPaymentModel3 = iVPaymentModel;
				iVPaymentModel3.MTaxTotalAmtFor += num;
				IVPaymentModel iVPaymentModel4 = iVPaymentModel;
				iVPaymentModel4.MTotalAmt += num5;
				IVPaymentModel iVPaymentModel5 = iVPaymentModel;
				iVPaymentModel5.MTotalAmtFor += num4;
				IVPaymentModel iVPaymentModel6 = iVPaymentModel;
				iVPaymentModel6.MReconcileAmt += num2;
				IVPaymentModel iVPaymentModel7 = iVPaymentModel;
				iVPaymentModel7.MReconcileAmtFor += num;
				list2.Add(iVPaymentEntryModel);
			}
			iVPaymentModel.PaymentEntry = list2;
			receiveAndBankEntrys.Add(new GLReceiveAndBankEntry
			{
				payment = iVPaymentModel,
				billEntry = groupEntryModel,
				billEntrys = bankBillEntrylList
			});
			if (iVPaymentModel.ValidationErrors != null && iVPaymentModel.ValidationErrors.Any())
			{
				validationErrors.AddRange(iVPaymentModel.ValidationErrors);
			}
			IVBankBillReconcileEntryModel iVBankBillReconcileEntryModel = new IVBankBillReconcileEntryModel();
			iVBankBillReconcileEntryModel.MID = recId;
			iVBankBillReconcileEntryModel.MTargetBillType = "Payment";
			iVBankBillReconcileEntryModel.MSpentAmtFor = iVPaymentModel.MTaxTotalAmtFor;
			iVBankBillReconcileEntryModel.MTargetBillID = iVPaymentModel.MID;
			List<CommandInfo> insertOrUpdateCmd = ModelInfoManager.GetInsertOrUpdateCmd<IVBankBillReconcileModel>(ctx, iVBankBillReconcileEntryModel, null, true);
			list.AddRange(insertOrUpdateCmd);
			return list;
		}

		private static List<CommandInfo> GetAddReceiveCmd(MContext ctx, IVCashCodingEditModel cashCodingModel, string recId, string contactId, BDContactType contactType, IVBankBillEntryModel groupEntryModel, List<IVBankBillEntryModel> bankBillEntrylList, BDBankAccountEditModel bankModel, ref List<ValidationError> validationErrors, ref List<GLReceiveAndBankEntry> receiveAndBankEntrys)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			string text = "Tax_Inclusive";
			IVReceiveModel iVReceiveModel = new IVReceiveModel();
			iVReceiveModel.MID = UUIDHelper.GetGuid();
			iVReceiveModel.IsNew = true;
			if (string.IsNullOrEmpty(contactId))
			{
				iVReceiveModel.MContactType = "Other";
				iVReceiveModel.MType = "Receive_Other";
			}
			else
			{
				switch (contactType)
				{
				case BDContactType.Supplier:
					iVReceiveModel.MContactType = "Supplier";
					iVReceiveModel.MType = "Receive_SaleReturn";
					break;
				case BDContactType.Customer:
					iVReceiveModel.MContactType = "Customer";
					iVReceiveModel.MType = "Receive_Sale";
					break;
				case BDContactType.Employee:
					iVReceiveModel.MContactType = "Employees";
					iVReceiveModel.MType = "Receive_OtherReturn";
					text = "No_Tax";
					break;
				default:
					iVReceiveModel.MContactType = "Other";
					iVReceiveModel.MType = "Receive_Other";
					break;
				}
			}
			iVReceiveModel.MBankID = cashCodingModel.MBankID;
			iVReceiveModel.MCyID = bankModel.MCyID;
			iVReceiveModel.MContactID = contactId;
			iVReceiveModel.MReference = groupEntryModel.MDesc;
			iVReceiveModel.MBizDate = groupEntryModel.MDate;
			iVReceiveModel.MOToLRate = cashCodingModel.MExchangeRate;
			iVReceiveModel.MLToORate = Math.Round(decimal.One / cashCodingModel.MExchangeRate, 6);
			iVReceiveModel.MOrgID = ctx.MOrgID;
			iVReceiveModel.MTaxID = text;
			iVReceiveModel.MSource = Convert.ToInt32(BillSourceType.Reconcile);
			iVReceiveModel.MCreateFrom = Convert.ToInt32(CreateFromType.Cashcoding);
			iVReceiveModel.MReconcileStatu = Convert.ToInt32(IVReconcileStatus.Completely);
			List<IVReceiveEntryModel> list2 = new List<IVReceiveEntryModel>();
			foreach (IVBankBillEntryModel bankBillEntryl in bankBillEntrylList)
			{
				decimal num = Math.Round(bankBillEntryl.MReceivedAmt, 2);
				decimal num2 = Math.Round(num * cashCodingModel.MExchangeRate, 2);
				decimal num3 = default(decimal);
				decimal mTaxAmt = default(decimal);
				if (text != "No_Tax")
				{
					num3 = Math.Round(num - num / (decimal.One + bankBillEntryl.MTaxRate), 2);
					mTaxAmt = Math.Round(num3 * cashCodingModel.MExchangeRate, 2);
				}
				decimal num4 = num;
				decimal num5 = num2;
				IVReceiveEntryModel iVReceiveEntryModel = new IVReceiveEntryModel();
				iVReceiveEntryModel.MOToLRate = iVReceiveModel.MOToLRate;
				iVReceiveEntryModel.MLToORate = iVReceiveModel.MLToORate;
				iVReceiveEntryModel.MID = iVReceiveModel.MID;
				iVReceiveEntryModel.MQty = decimal.One;
				iVReceiveEntryModel.MDesc = (string.IsNullOrEmpty(bankBillEntryl.MRef) ? bankBillEntryl.MDesc : bankBillEntryl.MRef);
				iVReceiveEntryModel.MTaxID = bankBillEntryl.MTaxID;
				iVReceiveEntryModel.MTrackItem1 = bankBillEntryl.MTrackItem1;
				iVReceiveEntryModel.MTrackItem2 = bankBillEntryl.MTrackItem2;
				iVReceiveEntryModel.MTrackItem3 = bankBillEntryl.MTrackItem3;
				iVReceiveEntryModel.MTrackItem4 = bankBillEntryl.MTrackItem4;
				iVReceiveEntryModel.MTrackItem5 = bankBillEntryl.MTrackItem5;
				iVReceiveEntryModel.MTaxAmount = num2;
				iVReceiveEntryModel.MTaxAmountFor = num;
				iVReceiveEntryModel.MAmount = num5;
				iVReceiveEntryModel.MAmountFor = num4;
				iVReceiveEntryModel.MTaxAmt = mTaxAmt;
				iVReceiveEntryModel.MTaxAmtFor = num3;
				iVReceiveEntryModel.MPrice = bankBillEntryl.MReceivedAmt;
				iVReceiveEntryModel.MAcctID = bankBillEntryl.MAccountID;
				IVReceiveModel iVReceiveModel2 = iVReceiveModel;
				iVReceiveModel2.MTaxTotalAmt += num2;
				IVReceiveModel iVReceiveModel3 = iVReceiveModel;
				iVReceiveModel3.MTaxTotalAmtFor += num;
				IVReceiveModel iVReceiveModel4 = iVReceiveModel;
				iVReceiveModel4.MTotalAmt += num5;
				IVReceiveModel iVReceiveModel5 = iVReceiveModel;
				iVReceiveModel5.MTotalAmtFor += num4;
				IVReceiveModel iVReceiveModel6 = iVReceiveModel;
				iVReceiveModel6.MReconcileAmt += num2;
				IVReceiveModel iVReceiveModel7 = iVReceiveModel;
				iVReceiveModel7.MReconcileAmtFor += num;
				list2.Add(iVReceiveEntryModel);
			}
			iVReceiveModel.ReceiveEntry = list2;
			receiveAndBankEntrys.Add(new GLReceiveAndBankEntry
			{
				receive = iVReceiveModel,
				billEntry = groupEntryModel,
				billEntrys = bankBillEntrylList
			});
			if (iVReceiveModel.ValidationErrors != null && iVReceiveModel.ValidationErrors.Any())
			{
				validationErrors.AddRange(iVReceiveModel.ValidationErrors);
			}
			IVBankBillReconcileEntryModel iVBankBillReconcileEntryModel = new IVBankBillReconcileEntryModel();
			iVBankBillReconcileEntryModel.MID = recId;
			iVBankBillReconcileEntryModel.MTargetBillType = "Receive";
			iVBankBillReconcileEntryModel.MReceiveAmtFor = iVReceiveModel.MTaxTotalAmtFor;
			iVBankBillReconcileEntryModel.MTargetBillID = iVReceiveModel.MID;
			List<CommandInfo> insertOrUpdateCmd = ModelInfoManager.GetInsertOrUpdateCmd<IVBankBillReconcileModel>(ctx, iVBankBillReconcileEntryModel, null, true);
			list.AddRange(insertOrUpdateCmd);
			return list;
		}

		private static List<CommandInfo> GetGenerateVoucherCmd<T>(MContext ctx, T model, IVBankBillEntryModel groupEntryModel, List<IVBankBillEntryModel> bankBillEntrylList) where T : BizDataModel
		{
			List<CommandInfo> list = new List<CommandInfo>();
			if (groupEntryModel.MVoucherStatus == Convert.ToInt32(IVBankBillVoucherStatus.UnGenerate))
			{
				return list;
			}
			OperationResult operationResult = GLInterfaceRepository.GenerateVouchersByBill(ctx, model, null);
			if (operationResult.Success)
			{
				list.AddRange((IEnumerable<CommandInfo>)operationResult.OperationCommands);
			}
			if (string.IsNullOrEmpty(model.MVoucherID))
			{
				return list;
			}
			foreach (IVBankBillEntryModel bankBillEntryl in bankBillEntrylList)
			{
				IVBankBillEntryVoucher iVBankBillEntryVoucher = new IVBankBillEntryVoucher();
				iVBankBillEntryVoucher.MVoucherID = model.MVoucherID;
				iVBankBillEntryVoucher.MBankBillEntryID = bankBillEntryl.MEntryID;
				list.AddRange((IEnumerable<CommandInfo>)ModelInfoManager.GetInsertOrUpdateCmd<IVBankBillEntryVoucher>(ctx, iVBankBillEntryVoucher, null, true));
				List<CommandInfo> list2 = list;
				CommandInfo commandInfo = new CommandInfo
				{
					CommandText = "UPDATE T_IV_BankBillEntry SET MVoucherStatus=@MVoucherStatus WHERE MEntryID=@MEntryID AND MOrgID=@MOrgID"
				};
				DbParameter[] array = commandInfo.Parameters = new MySqlParameter[3]
				{
					new MySqlParameter("@MVoucherStatus", Convert.ToInt32(IVBankBillVoucherStatus.Generated)),
					new MySqlParameter("@MEntryID", bankBillEntryl.MEntryID),
					new MySqlParameter("@MOrgID", ctx.MOrgID)
				};
				list2.Add(commandInfo);
			}
			return list;
		}

		private static List<CommandInfo> GetGenerateVoucherCmds(MContext ctx, List<GLReceiveAndBankEntry> models)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			IEnumerable<GLReceiveAndBankEntry> enumerable = from a in models
			where a.billEntry.MVoucherStatus != Convert.ToInt32(IVBankBillVoucherStatus.UnGenerate)
			select a;
			if (!enumerable.Any())
			{
				return list;
			}
			List<IVReceiveModel> list2 = (from a in enumerable
			where a.receive != null
			select a.receive).ToList();
			List<IVPaymentModel> list3 = (from a in enumerable
			where a.payment != null
			select a.payment).ToList();
			if (list2.Any())
			{
				OperationResult operationResult = GLInterfaceRepository.GenerateVouchersByBills(ctx, list2, null);
				if (operationResult.Success)
				{
					list.AddRange(operationResult.OperationCommands);
				}
			}
			if (list3.Any())
			{
				OperationResult operationResult2 = GLInterfaceRepository.GenerateVouchersByBills(ctx, list3, null);
				if (operationResult2.Success)
				{
					list.AddRange(operationResult2.OperationCommands);
				}
			}
			foreach (GLReceiveAndBankEntry item in enumerable)
			{
				string empty = string.Empty;
				if (item.receive != null && !string.IsNullOrEmpty(item.receive.MVoucherID))
				{
					empty = item.receive.MVoucherID;
					goto IL_01e2;
				}
				if (item.payment != null && !string.IsNullOrEmpty(item.payment.MVoucherID))
				{
					empty = item.payment.MVoucherID;
					goto IL_01e2;
				}
				continue;
				IL_01e2:
				foreach (IVBankBillEntryModel billEntry in item.billEntrys)
				{
					IVBankBillEntryVoucher iVBankBillEntryVoucher = new IVBankBillEntryVoucher();
					iVBankBillEntryVoucher.MVoucherID = empty;
					iVBankBillEntryVoucher.MBankBillEntryID = billEntry.MEntryID;
					list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<IVBankBillEntryVoucher>(ctx, iVBankBillEntryVoucher, null, true));
					List<CommandInfo> list4 = list;
					CommandInfo commandInfo = new CommandInfo
					{
						CommandText = "UPDATE T_IV_BankBillEntry SET MVoucherStatus=@MVoucherStatus WHERE MEntryID=@MEntryID AND MOrgID=@MOrgID"
					};
					DbParameter[] array = commandInfo.Parameters = new MySqlParameter[3]
					{
						new MySqlParameter("@MVoucherStatus", Convert.ToInt32(IVBankBillVoucherStatus.Generated)),
						new MySqlParameter("@MEntryID", billEntry.MEntryID),
						new MySqlParameter("@MOrgID", ctx.MOrgID)
					};
					list4.Add(commandInfo);
				}
			}
			return list;
		}

		public static string GetReceiveIDsByBankBillEntry(MContext ctx, string bankBillEntryIds)
		{
			string sql = $"SELECT MTargetBillID from T_IV_BankbillReconcileEntry\r\n\t\t\t\t           WHERE MTargetBillType='Receive' AND MIsDelete=0\r\n                                AND MID in (select MID from T_IV_BankbillReconcile \r\n\t\t\t\t\t\t                     WHERE MIsDelete=0 AND MBankBillEntryID in ({bankBillEntryIds}))";
			return GetBankBillTargetIDs(ctx, sql);
		}

		public static string GetPaymentIDsByBankBillEntry(MContext ctx, string bankBillEntryIds)
		{
			string sql = $"SELECT MTargetBillID from T_IV_BankbillreconcileEntry\r\n\t\t\t\t           WHERE MTargetBillType='Payment' AND MIsDelete=0\r\n                                AND MID in (select MID from t_iv_bankbillreconcile \r\n\t\t\t\t\t\t                     WHERE MIsDelete=0 AND  MBankBillEntryID in ({bankBillEntryIds}))";
			return GetBankBillTargetIDs(ctx, sql);
		}

		public static string GetTransferIDsByBankBillEntry(MContext ctx, string bankBillEntryIds)
		{
			string sql = $"SELECT MTargetBillID from T_IV_BankbillreconcileEntry\r\n\t\t\t\t           WHERE MTargetBillType='Transfer' AND MIsDelete=0\r\n                                AND MID in (select MID from t_iv_bankbillreconcile \r\n\t\t\t\t\t\t                     WHERE MIsDelete=0 AND  MBankBillEntryID in ({bankBillEntryIds}))";
			return GetBankBillTargetIDs(ctx, sql);
		}

		public static string GetReceiveIDsByBankBill(MContext ctx, string bankBillIds)
		{
			string sql = $"SELECT MTargetBillID from T_IV_BankbillreconcileEntry\r\n\t\t\t\t           WHERE MTargetBillType='Receive' AND MIsDelete=0\r\n                                AND MID in (select MID from t_iv_bankbillreconcile \r\n\t\t\t\t\t\t                     where MIsDelete=0 AND  mbankbillentryid in  (select MEntryid from t_iv_bankbillentry \r\n\t\t\t\t\t\t\t                                                where MIsDelete=0 AND  mid in({bankBillIds})))";
			return GetBankBillTargetIDs(ctx, sql);
		}

		public static string GetPaymentIDsByBankBill(MContext ctx, string bankBillIds)
		{
			string sql = $"SELECT MTargetBillID from T_IV_BankbillreconcileEntry\r\n\t\t\t\t           WHERE MTargetBillType='Payment' AND MIsDelete=0\r\n                                AND MID in (select MID from t_iv_bankbillreconcile \r\n\t\t\t\t\t\t                     where MIsDelete=0 AND  mbankbillentryid in  (select MEntryid from t_iv_bankbillentry \r\n\t\t\t\t\t\t\t                                                where MIsDelete=0 AND  mid in({bankBillIds})))";
			return GetBankBillTargetIDs(ctx, sql);
		}

		public static string GetTransferIDsByBankBill(MContext ctx, string bankBillIds)
		{
			string sql = $"SELECT MTargetBillID from T_IV_BankbillreconcileEntry\r\n\t\t\t\t           WHERE MTargetBillType='Transfer' AND MIsDelete=0\r\n                                AND MID in (select MID from t_iv_bankbillreconcile \r\n\t\t\t\t\t\t                     where MIsDelete=0 AND  mbankbillentryid in  (select MEntryid from t_iv_bankbillentry \r\n\t\t\t\t\t\t\t                                                where MIsDelete=0 AND  mid in({bankBillIds})))";
			return GetBankBillTargetIDs(ctx, sql);
		}

		private static string GetBankBillTargetIDs(MContext ctx, string sql)
		{
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(sql);
			if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return string.Empty;
			}
			DataTable dataTable = dataSet.Tables[0];
			StringBuilder stringBuilder = new StringBuilder();
			foreach (DataRow row in dataTable.Rows)
			{
				stringBuilder.AppendFormat("'{0}',", row[0]);
			}
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
			return stringBuilder.ToString();
		}

		public static string GetAutoCreateReceiveIDs(MContext ctx, string bankBillIds)
		{
			return string.Empty;
		}

		public static string GetAutoCreatePaymentIDs(MContext ctx, string bankBillIds)
		{
			return string.Empty;
		}

		public static void UpdateReconcileAmt(MContext ctx, string paymentIds, string receiveIds, string transferIds, string autoCreatePaymentIds, string autoCreateReceiveIds)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("set sql_safe_updates = 0; ");
			if (!string.IsNullOrEmpty(paymentIds))
			{
				stringBuilder.AppendFormat("UPDATE T_IV_Payment a\r\n                                   LEFT JOIN (SELECT MTargetBillID,MTargetBillType,SUM(MSpentAmtFor) as MSpentAmtFor,SUM(MReceiveAmtFor) as MReceiveAmtFor\r\n                                   FROM T_IV_BankbillReconcileEntry where MIsDelete = 0 and MOrgID = @MOrgID\r\n                                   GROUP BY MTargetBillID,MTargetBillType) b ON a.MID=b.MTargetBillID AND MTargetBillType='Payment'\r\n                                   SET a.MReconcileAmt=round(ifnull(MSpentAmtFor,0)*a.MExchangeRate,2),a.MReconcileAmtFor=ifnull(MSpentAmtFor,0),\r\n                                    a.MReconcileStatu =  (CASE WHEN ifnull(MSpentAmtFor,0)<>0 AND ifnull(MSpentAmtFor,0)=MTaxTotalAmtFor THEN 203 \r\n                                    WHEN ifnull(MSpentAmtFor,0)<>0 AND ifnull(MSpentAmtFor,0)<>MTaxTotalAmtFor THEN 202\r\n                                    ELSE 201 END)\r\n                                   WHERE a.MID IN ({0}) and a.MIsDelete = 0 and a.MOrgID = @MOrgID ; ", paymentIds);
			}
			if (!string.IsNullOrEmpty(receiveIds))
			{
				stringBuilder.AppendFormat("UPDATE T_IV_Receive a\r\n                                   LEFT JOIN (SELECT MTargetBillID,MTargetBillType,SUM(MSpentAmtFor) as MSpentAmtFor,SUM(MReceiveAmtFor) as MReceiveAmtFor\r\n                                   FROM T_IV_BankbillReconcileEntry  where MIsDelete = 0 and MOrgID = @MOrgID\r\n                                   GROUP BY MTargetBillID,MTargetBillType) b ON a.MID=b.MTargetBillID AND MTargetBillType='Receive'\r\n                                   SET a.MReconcileAmt=round(ifnull(MReceiveAmtFor,0)*a.MExchangeRate,2),a.MReconcileAmtFor=ifnull(MReceiveAmtFor,0),\r\n                                   a.MReconcileStatu =  (CASE WHEN ifnull(MReceiveAmtFor,0)<>0 AND ifnull(MReceiveAmtFor,0)=MTaxTotalAmtFor THEN 203 \r\n                                    WHEN ifnull(MReceiveAmtFor,0)<>0 AND ifnull(MReceiveAmtFor,0)<>MTaxTotalAmtFor THEN 202\r\n                                    ELSE 201 END)\r\n                                   WHERE a.MID IN ({0})  and a.MIsDelete = 0 and a.MOrgID = @MOrgID ;", receiveIds);
			}
			if (!string.IsNullOrEmpty(transferIds))
			{
				stringBuilder.AppendFormat(" UPDATE T_IV_Transfer a\r\n                                   LEFT JOIN (SELECT MTargetBillID,MTargetBillType,SUM(MSpentAmtFor) as MSpentAmtFor,SUM(MReceiveAmtFor) as MReceiveAmtFor\r\n                                   FROM T_IV_BankbillReconcileEntry  where MIsDelete = 0 and MOrgID = @MOrgID\r\n                                   GROUP BY MTargetBillID,MTargetBillType) b ON a.MID=b.MTargetBillID AND MTargetBillType='Transfer'\r\n                                   SET a.MFromReconcileAmt=round(ifnull(MSpentAmtFor*a.MExchangeRate,0),2),a.MFromReconcileAmtFor=ifnull(MSpentAmtFor,0),\r\n                                   a.MToReconcileAmt=round(ifnull(MReceiveAmtFor*a.MExchangeRate,0),2),a.MToReconcileAmtFor=ifnull(MReceiveAmtFor,0),\r\n                                    MFromReconcileStatu =  (\r\n                                     CASE WHEN =ifnull(MSpentAmtFor,0)<>0 AND =ifnull(MSpentAmtFor,0)=MFromTotalAmtFor THEN 203 \r\n                                     WHEN =ifnull(MSpentAmtFor,0)<>0 AND =ifnull(MSpentAmtFor,0)<>MFromTotalAmtFor THEN 202\r\n                                     ELSE 201 END),MToReconcileStatu =  (\r\n                                     CASE WHEN ifnull(MReceiveAmtFor,0)<>0 AND ifnull(MReceiveAmtFor,0)=MToTotalAmtFor THEN 203 \r\n                                     WHEN ifnull(MReceiveAmtFor,0)<>0 AND ifnull(MReceiveAmtFor,0)<>MToTotalAmtFor THEN 202\r\n                                     ELSE 201 END)\r\n                                   WHERE a.MID IN ({0}) and a.MIsDelete = 0 and a.MOrgID = @MOrgID ;", transferIds);
			}
			if (!string.IsNullOrEmpty(autoCreatePaymentIds))
			{
				stringBuilder.AppendFormat(" UPDATE T_IV_Payment SET MIsDelete=1 WHERE MID IN ({0}) and MIsDelete = 0 and MOrgID = @MOrgID ;", autoCreatePaymentIds);
			}
			if (!string.IsNullOrEmpty(autoCreateReceiveIds))
			{
				stringBuilder.AppendFormat("UPDATE FROM T_IV_Receive SET MIsDelete=1 WHERE MID IN ({0}) and MIsDelete = 0 and MOrgID = @MOrgID ;", autoCreateReceiveIds);
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DynamicDbHelperMySQL dynamicDbHelperMySQL2 = dynamicDbHelperMySQL;
			List<CommandInfo> list = new List<CommandInfo>();
			CommandInfo obj = new CommandInfo
			{
				CommandText = stringBuilder.ToString()
			};
			DbParameter[] array = obj.Parameters = ctx.GetParameters((MySqlParameter)null);
			list.Add(obj);
			dynamicDbHelperMySQL2.ExecuteSqlTran(list);
		}

		public OperationResult UpdateBankBillRec(MContext ctx, List<IVBankBillReconcileModel> list)
		{
			IVBankBillEntryRepository iVBankBillEntryRepository = new IVBankBillEntryRepository();
			OperationResult operationResult = new OperationResult();
			BDBankAccountRepository bDBankAccountRepository = new BDBankAccountRepository();
			BDBankAccountEditModel bDBankAccountEditModel = bDBankAccountRepository.GetBDBankAccountEditModel(ctx, list[0].MBankID);
			if (bDBankAccountEditModel == null || string.IsNullOrEmpty(bDBankAccountEditModel.MItemID))
			{
				return new OperationResult
				{
					Success = false,
					Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "DataHasBeenChanged", "Data has benn changed！")
				};
			}
			List<CommandInfo> list2 = new List<CommandInfo>();
			foreach (IVBankBillReconcileModel item in list)
			{
				IVBankBillEntryModel dataModel = iVBankBillEntryRepository.GetDataModel(ctx, item.MBankBillEntryID, false);
				if (dataModel == null || dataModel.MCheckState == 2)
				{
					return new OperationResult
					{
						Success = false,
						Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "DataHasBeenChanged", "Data has benn changed！")
					};
				}
				if (dataModel.MReceivedAmt != item.RecEntryList.Sum((IVBankBillReconcileEntryModel t) => t.MReceiveAmtFor) || dataModel.MSpentAmt != item.RecEntryList.Sum((IVBankBillReconcileEntryModel t) => t.MSpentAmtFor))
				{
					return new OperationResult
					{
						Success = false,
						Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "ParamError", "参数错误！")
					};
				}
				decimal exchangeRate = REGCurrencyRepository.GetExchangeRate(ctx, bDBankAccountEditModel.MCyID, dataModel.MDate, out operationResult);
				item.MOrgID = ctx.MOrgID;
				foreach (IVBankBillReconcileEntryModel recEntry in item.RecEntryList)
				{
					bool flag = false;
					if (recEntry.MIsAdjustAmt)
					{
						if (!operationResult.Success)
						{
							return operationResult;
						}
						if (dataModel.MReceivedAmt > decimal.Zero && recEntry.MReceiveAmtFor < decimal.Zero)
						{
							recEntry.MSpentAmtFor = Math.Abs(recEntry.MReceiveAmtFor);
							recEntry.MReceiveAmtFor = decimal.Zero;
						}
						list2.AddRange(GetRecAdjustCmdInfo(ctx, item, bDBankAccountEditModel, recEntry, exchangeRate));
						flag = true;
					}
					else if (recEntry.MIsBankFeeAmt)
					{
						if (!operationResult.Success)
						{
							return operationResult;
						}
						if (dataModel.MReceivedAmt > decimal.Zero && recEntry.MReceiveAmtFor < decimal.Zero)
						{
							recEntry.MSpentAmtFor = Math.Abs(recEntry.MReceiveAmtFor);
							recEntry.MReceiveAmtFor = decimal.Zero;
						}
						list2.AddRange(GetRecBankFeeCmdInfo(ctx, item, bDBankAccountEditModel, recEntry, exchangeRate));
						flag = true;
					}
					else if (recEntry.MIsInterestAmt)
					{
						if (!operationResult.Success)
						{
							return operationResult;
						}
						list2.AddRange(GetRecInterestCmdInfo(ctx, item, bDBankAccountEditModel, recEntry, exchangeRate));
						flag = true;
					}
					else if (recEntry.MTargetBillType == "Invoice")
					{
						IVMakePaymentModel makePaymentModel = GetMakePaymentModel(ctx, recEntry, item.MBankID, "Invoice", dataModel.MDate);
						IVInvoiceMakePaymentRepository iVInvoiceMakePaymentRepository = new IVInvoiceMakePaymentRepository(BillSourceType.Reconcile, CreateFromType.Reconcile);
						MakePaymentResultModel toPayResult = iVInvoiceMakePaymentRepository.GetToPayResult(ctx, makePaymentModel, bDBankAccountEditModel);
						if (!toPayResult.MSuccess)
						{
							return new OperationResult
							{
								Success = false,
								Message = toPayResult.MMessage
							};
						}
						list2.AddRange(toPayResult.MCommand);
						flag = true;
						recEntry.MTargetBillID = toPayResult.MTargetBillID;
						recEntry.MTargetBillType = toPayResult.MTargetBillType;
					}
					else if (recEntry.MTargetBillType == "Expense")
					{
						IVMakePaymentModel makePaymentModel2 = GetMakePaymentModel(ctx, recEntry, item.MBankID, "Expense", dataModel.MDate);
						IVExpenseMakePaymentRepository iVExpenseMakePaymentRepository = new IVExpenseMakePaymentRepository(BillSourceType.Reconcile, CreateFromType.Reconcile);
						MakePaymentResultModel toPayResult2 = iVExpenseMakePaymentRepository.GetToPayResult(ctx, makePaymentModel2, bDBankAccountEditModel);
						if (!toPayResult2.MSuccess)
						{
							return new OperationResult
							{
								Success = false,
								Message = toPayResult2.MMessage
							};
						}
						list2.AddRange(toPayResult2.MCommand);
						flag = true;
						recEntry.MTargetBillID = toPayResult2.MTargetBillID;
						recEntry.MTargetBillType = toPayResult2.MTargetBillType;
					}
					else if (recEntry.MTargetBillType == "Transfer")
					{
						recEntry.MTargetBillType = "Transfer";
					}
					else if (recEntry.MSpentAmtFor > decimal.Zero)
					{
						recEntry.MTargetBillType = "Payment";
					}
					else
					{
						recEntry.MTargetBillType = "Receive";
					}
					if (!flag)
					{
						OperationResult operationResult2 = CheckCanReconcile(ctx, recEntry);
						if (!operationResult2.Success)
						{
							return operationResult2;
						}
					}
					OperationResult operationResult3 = ResultHelper.ToOperationResult(recEntry);
					if (!operationResult3.Success)
					{
						return operationResult3;
					}
				}
				list2.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<IVBankBillReconcileModel>(ctx, item, null, true));
				list2.AddRange(GetUpdateReconcileAmtCmd(ctx, item.RecEntryList));
				list2.AddRange(GetUpdateReconcileMatchCmd(ctx, item.MBankBillEntryID, item.RecEntryList[0].MTargetBillID));
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num = dynamicDbHelperMySQL.ExecuteSqlTran(list2);
			operationResult.Success = (num > 0 && true);
			return operationResult;
		}

		private static OperationResult CheckCanReconcile(MContext ctx, IVBankBillReconcileEntryModel item)
		{
			OperationResult operationResult = new OperationResult();
			string text2 = operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "DataHasBeenChanged", "Data has benn changed！");
			operationResult.Success = false;
			if (item.MTargetBillType == "Payment")
			{
				IVPaymentModel paymentEditModel = IVPaymentRepository.GetPaymentEditModel(ctx, item.MTargetBillID);
				if (string.IsNullOrEmpty(paymentEditModel.MID) || paymentEditModel.MReconcileStatu == Convert.ToInt32(IVReconcileStatus.Marked) || paymentEditModel.MReconcileStatu == Convert.ToInt32(IVReconcileStatus.Completely))
				{
					return operationResult;
				}
			}
			else if (item.MTargetBillType == "Receive")
			{
				IVReceiveModel receiveEditModel = IVReceiveRepository.GetReceiveEditModel(ctx, item.MTargetBillID);
				if (string.IsNullOrEmpty(receiveEditModel.MID) || receiveEditModel.MReconcileStatu == Convert.ToInt32(IVReconcileStatus.Marked) || receiveEditModel.MReconcileStatu == Convert.ToInt32(IVReconcileStatus.Completely))
				{
					return operationResult;
				}
			}
			else if (item.MTargetBillType == "Transfer")
			{
				IVTransferModel transferEditModel = IVTransferRepository.GetTransferEditModel(ctx, item.MTargetBillID);
				if (string.IsNullOrEmpty(transferEditModel.MID))
				{
					return operationResult;
				}
				if (item.MSpentAmtFor > decimal.Zero)
				{
					if (transferEditModel.MFromReconcileStatu == Convert.ToInt32(IVReconcileStatus.Marked) || transferEditModel.MFromReconcileStatu == Convert.ToInt32(IVReconcileStatus.Completely))
					{
						return operationResult;
					}
				}
				else if (transferEditModel.MToReconcileStatu == Convert.ToInt32(IVReconcileStatus.Marked) || transferEditModel.MToReconcileStatu == Convert.ToInt32(IVReconcileStatus.Completely))
				{
					return operationResult;
				}
			}
			operationResult.Success = true;
			operationResult.Message = "";
			return operationResult;
		}

		private static IVMakePaymentModel GetMakePaymentModel(MContext ctx, IVBankBillReconcileEntryModel item, string bankId, string objType, DateTime paidDate)
		{
			return new IVMakePaymentModel
			{
				MObjectID = item.MTargetBillID,
				MBankID = bankId,
				MPaidAmount = ((item.MSpentAmtFor > decimal.Zero) ? item.MSpentAmtFor : item.MReceiveAmtFor),
				MPaidDate = paidDate,
				MRefFromBill = true,
				MObjectType = objType
			};
		}

		private static List<CommandInfo> GetRecAdjustCmdInfo(MContext ctx, IVBankBillReconcileModel model, BDBankAccountEditModel bankModel, IVBankBillReconcileEntryModel item, decimal exchangeRate)
		{
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "ReconciliationAdjustment", "Reconciliation adjustment");
			List<CommandInfo> list = new List<CommandInfo>();
			if (item.MSpentAmtFor != decimal.Zero)
			{
				item.MTargetBillType = "Payment";
				string mTargetBillID = "";
				list.AddRange(GetRecPaymentCmdInfo(ctx, bankModel, "Pay_Adjustment", item.MDate, item.MSpentAmtFor, "", item.MRef, $"Reconciliation adjustment:{item.MDesc}", $"{text}:{item.MDesc}", "Other", exchangeRate, item, out mTargetBillID));
				item.MTargetBillID = mTargetBillID;
			}
			if (item.MReceiveAmtFor != decimal.Zero)
			{
				item.MTargetBillType = "Receive";
				string mTargetBillID2 = "";
				list.AddRange(GetRecReceiveCmdInfo(ctx, bankModel, "Receive_Adjustment", item.MDate, item.MReceiveAmtFor, "", item.MRef, $"Reconciliation adjustment:{item.MDesc}", $"{text}:{item.MDesc}", "Other", exchangeRate, item, out mTargetBillID2));
				item.MTargetBillID = mTargetBillID2;
			}
			return list;
		}

		private static List<CommandInfo> GetRecBankFeeCmdInfo(MContext ctx, IVBankBillReconcileModel model, BDBankAccountEditModel bankModel, IVBankBillReconcileEntryModel item, decimal exchangeRate)
		{
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "BankFees", "Bank charge");
			List<CommandInfo> list = new List<CommandInfo>();
			if (item.MSpentAmtFor > decimal.Zero)
			{
				item.MTargetBillType = "Payment";
				string mTargetBillID = "";
				list.AddRange(GetRecPaymentCmdInfo(ctx, bankModel, "Pay_BankFee", item.MDate, item.MSpentAmtFor, item.MContactID, item.MRef, $"Bank fee:{item.MDesc}", $"{text}:{item.MDesc}", "Other", exchangeRate, item, out mTargetBillID));
				item.MTargetBillID = mTargetBillID;
			}
			if (item.MReceiveAmtFor > decimal.Zero)
			{
				item.MTargetBillType = "Receive";
				string mTargetBillID2 = "";
				list.AddRange(GetRecReceiveCmdInfo(ctx, bankModel, "Receive_BankFee", item.MDate, item.MReceiveAmtFor, item.MContactID, item.MRef, $"Bank fee:{item.MDesc}", $"{text}:{item.MDesc}", "Other", exchangeRate, item, out mTargetBillID2));
				item.MTargetBillID = mTargetBillID2;
			}
			return list;
		}

		private static List<CommandInfo> GetRecInterestCmdInfo(MContext ctx, IVBankBillReconcileModel model, BDBankAccountEditModel bankModel, IVBankBillReconcileEntryModel item, decimal exchangeRate)
		{
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "Interest", "bank interest");
			List<CommandInfo> list = new List<CommandInfo>();
			if (item.MSpentAmtFor > decimal.Zero)
			{
				item.MTargetBillType = "Payment";
				string mTargetBillID = "";
				list.AddRange(GetRecPaymentCmdInfo(ctx, bankModel, "Pay_BankInterest", item.MDate, item.MSpentAmtFor, item.MContactID, item.MRef, $"Interest:{item.MDesc}", $"{text}:{item.MDesc}", "Other", exchangeRate, item, out mTargetBillID));
				item.MTargetBillID = mTargetBillID;
			}
			if (item.MReceiveAmtFor > decimal.Zero)
			{
				item.MTargetBillType = "Receive";
				string mTargetBillID2 = "";
				list.AddRange(GetRecReceiveCmdInfo(ctx, bankModel, "Receive_BankInterest", item.MDate, item.MReceiveAmtFor, item.MContactID, item.MRef, $"Interest:{item.MDesc}", $"{text}:{item.MDesc}", "Other", exchangeRate, item, out mTargetBillID2));
				item.MTargetBillID = mTargetBillID2;
			}
			return list;
		}

		private static List<CommandInfo> GetRecPaymentCmdInfo(MContext ctx, BDBankAccountEditModel bankModel, string bizType, DateTime bizDate, decimal spentAmtFor, string contactId, string reference, string description, string entryDescription, string contactType, decimal exchangeRate, IVBankBillReconcileEntryModel item, out string paymentId)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			decimal num = Math.Round(spentAmtFor * exchangeRate, 2);
			decimal num2 = num;
			IVPaymentModel iVPaymentModel = new IVPaymentModel();
			iVPaymentModel.MBankID = bankModel.MItemID;
			iVPaymentModel.MCyID = bankModel.MCyID;
			iVPaymentModel.MContactID = contactId;
			iVPaymentModel.MContactType = contactType;
			iVPaymentModel.MTaxID = "No_Tax";
			iVPaymentModel.MReference = reference;
			iVPaymentModel.MDesc = description;
			iVPaymentModel.MBizDate = bizDate;
			iVPaymentModel.MType = bizType;
			iVPaymentModel.MExchangeRate = exchangeRate;
			iVPaymentModel.MTaxTotalAmt = num;
			iVPaymentModel.MTaxTotalAmtFor = spentAmtFor;
			iVPaymentModel.MTotalAmt = num2;
			iVPaymentModel.MTotalAmtFor = spentAmtFor;
			iVPaymentModel.MOrgID = ctx.MOrgID;
			iVPaymentModel.MReconcileAmt = num;
			iVPaymentModel.MReconcileAmtFor = spentAmtFor;
			iVPaymentModel.MSource = Convert.ToInt32(BillSourceType.Reconcile);
			iVPaymentModel.MReconcileStatu = Convert.ToInt32(IVReconcileStatus.Completely);
			List<IVPaymentEntryModel> list2 = new List<IVPaymentEntryModel>();
			IVPaymentEntryModel iVPaymentEntryModel = new IVPaymentEntryModel();
			iVPaymentEntryModel.MID = iVPaymentModel.MID;
			iVPaymentEntryModel.MQty = decimal.One;
			iVPaymentEntryModel.MDesc = entryDescription;
			iVPaymentEntryModel.MTaxAmountFor = spentAmtFor;
			iVPaymentEntryModel.MTaxAmount = num;
			iVPaymentEntryModel.MAmount = num2;
			iVPaymentEntryModel.MAmountFor = spentAmtFor;
			iVPaymentEntryModel.MTaxAmt = decimal.Zero;
			iVPaymentEntryModel.MTaxAmtFor = decimal.Zero;
			iVPaymentEntryModel.MPrice = spentAmtFor;
			list2.Add(iVPaymentEntryModel);
			iVPaymentModel.PaymentEntry = list2;
			List<CommandInfo> insertOrUpdateCmd = ModelInfoManager.GetInsertOrUpdateCmd<IVPaymentModel>(ctx, iVPaymentModel, null, true);
			list.AddRange(insertOrUpdateCmd);
			paymentId = iVPaymentModel.MID;
			OperationResult operationResult = GLInterfaceRepository.GenerateVouchersByBill(ctx, iVPaymentModel, null);
			if (operationResult.Success)
			{
				list.AddRange(operationResult.OperationCommands);
			}
			return list;
		}

		private static List<CommandInfo> GetRecReceiveCmdInfo(MContext ctx, BDBankAccountEditModel bankModel, string bizType, DateTime bizDate, decimal receiveAmtFor, string contactId, string reference, string description, string entryDescription, string contactType, decimal exchangeRate, IVBankBillReconcileEntryModel item, out string receiveId)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			decimal num = Math.Round(receiveAmtFor * exchangeRate, 2);
			decimal num2 = num;
			IVReceiveModel iVReceiveModel = new IVReceiveModel();
			iVReceiveModel.MBankID = bankModel.MItemID;
			iVReceiveModel.MCyID = bankModel.MCyID;
			iVReceiveModel.MContactType = contactType;
			iVReceiveModel.MContactID = contactId;
			iVReceiveModel.MTaxID = "No_Tax";
			iVReceiveModel.MReference = reference;
			iVReceiveModel.MDesc = description;
			iVReceiveModel.MBizDate = bizDate;
			iVReceiveModel.MType = bizType;
			iVReceiveModel.MExchangeRate = exchangeRate;
			iVReceiveModel.MTaxTotalAmt = num;
			iVReceiveModel.MTaxTotalAmtFor = receiveAmtFor;
			iVReceiveModel.MTotalAmt = num2;
			iVReceiveModel.MTotalAmtFor = receiveAmtFor;
			iVReceiveModel.MOrgID = ctx.MOrgID;
			iVReceiveModel.MReconcileAmt = num;
			iVReceiveModel.MReconcileAmtFor = receiveAmtFor;
			iVReceiveModel.MSource = Convert.ToInt32(BillSourceType.Reconcile);
			iVReceiveModel.MReconcileStatu = Convert.ToInt32(IVReconcileStatus.Completely);
			List<IVReceiveEntryModel> list2 = new List<IVReceiveEntryModel>();
			IVReceiveEntryModel iVReceiveEntryModel = new IVReceiveEntryModel();
			iVReceiveEntryModel.MID = iVReceiveModel.MID;
			iVReceiveEntryModel.MQty = decimal.One;
			iVReceiveEntryModel.MDesc = entryDescription;
			iVReceiveEntryModel.MTaxAmountFor = receiveAmtFor;
			iVReceiveEntryModel.MTaxAmount = num;
			iVReceiveEntryModel.MAmount = num2;
			iVReceiveEntryModel.MAmountFor = receiveAmtFor;
			iVReceiveEntryModel.MTaxAmt = decimal.Zero;
			iVReceiveEntryModel.MTaxAmtFor = decimal.Zero;
			iVReceiveEntryModel.MPrice = receiveAmtFor;
			list2.Add(iVReceiveEntryModel);
			iVReceiveModel.ReceiveEntry = list2;
			List<CommandInfo> insertOrUpdateCmd = ModelInfoManager.GetInsertOrUpdateCmd<IVReceiveModel>(ctx, iVReceiveModel, null, true);
			list.AddRange(insertOrUpdateCmd);
			receiveId = iVReceiveEntryModel.MID;
			OperationResult operationResult = GLInterfaceRepository.GenerateVouchersByBill(ctx, iVReceiveModel, null);
			if (operationResult.Success)
			{
				list.AddRange(operationResult.OperationCommands);
			}
			else
			{
				item.ValidationErrors = (item.ValidationErrors ?? new List<ValidationError>());
				item.ValidationErrors.AddRange(iVReceiveModel.ValidationErrors);
			}
			return list;
		}

		public IVBankBillRecListModel GetBankBillRecByID(MContext ctx, string id, string bankId)
		{
			if (string.IsNullOrEmpty(id))
			{
				return null;
			}
			string sql = " Select a.* from T_IV_BankBillEntry a\r\n                                    INNER JOIN T_IV_BankBill b ON a.MID=b.MID\r\n                                    left join T_IV_BankBillReconcile c on a.mentryid=c.MBankBillEntryID\n                                     and c.MBankBillEntryID=a.MEntryID AND\n                                     c.MIsActive=1 and c.MIsDelete=0 and c.MOrgID=@MOrgID\r\n                                    WHERE a.MIsDelete=0 and a.MEntryID = @MEntryID AND b.MBankID=@MBankID \r\n                                    and c.mid is null  and a.MCheckState <> '2' AND IFNULL(a.MParentID,'')=''";
			MySqlParameter[] cmdParms = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MBankID", bankId),
				new MySqlParameter("@MEntryID", id)
			};
			IVBankBillRecListModel dataModel = ModelInfoManager.GetDataModel<IVBankBillRecListModel>(ctx, sql, cmdParms);
			List<IVBankBillRecListModel> list = new List<IVBankBillRecListModel>
			{
				dataModel
			};
			if (list != null && list.Count > 0)
			{
				ResetBankBillRecList(ctx, bankId, list);
			}
			return list[0];
		}

		public DataGridJson<IVBankBillRecListModel> GetBankBillRecList(MContext ctx, IVBankBillRecListFilterModel filter)
		{
			SqlQuery sqlQuery = new SqlQuery();
			DateTime dateTime = filter.StartDate;
			DateTime date = dateTime.Date;
			dateTime = ctx.MBeginDate;
			if (date < dateTime.Date)
			{
				dateTime = ctx.MBeginDate;
				filter.StartDate = dateTime.Date;
			}
			dateTime = filter.EndDate;
			if (dateTime.Year <= 1900)
			{
				filter.EndDate = DateTime.MaxValue;
			}
			sqlQuery.SqlWhere = filter;
			string text = "";
			if (filter.OnlyShowMatched)
			{
				text = (sqlQuery.SelectString = GetBankBillRecMatchedSql(ctx, filter, ref sqlQuery));
			}
			else
			{
				text = "Select a.* from T_IV_BankBillEntry a\r\n                            INNER JOIN T_IV_BankBill b ON a.MID=b.MID\r\n                            left join T_IV_BankBillReconcile c on a.mentryid=c.MBankBillEntryID\n                            and c.MBankBillEntryID=a.MEntryID AND\n                            c.MIsActive=1 and c.MIsDelete=0 and c.MOrgID=@MOrgID";
				text = (sqlQuery.SelectString = text + GetBankBillRecFilter(ctx, filter, ref sqlQuery));
			}
			if (filter.Sort == "Spent")
			{
				sqlQuery.AddOrderBy(" case when a.MSpentAmt>0 then 1 else 0 end ", SqlOrderDir.Desc);
			}
			else if (filter.Sort == "Recevied")
			{
				sqlQuery.AddOrderBy(" case when a.MReceivedAmt>0 then 1 else 0 end ", SqlOrderDir.Desc);
			}
			sqlQuery.AddOrderBy(" date_add(a.MDate, interval IFNULL(a.MSeq,1) second) ", (!(filter.Order == "asc")) ? SqlOrderDir.Desc : SqlOrderDir.Asc);
			DataGridJson<IVBankBillRecListModel> pageDataModelListBySql = ModelInfoManager.GetPageDataModelListBySql<IVBankBillRecListModel>(ctx, sqlQuery);
			List<IVBankBillRecListModel> rows = pageDataModelListBySql.rows;
			if (rows != null && rows.Count > 0)
			{
				ResetBankBillRecList(ctx, filter.MBankID, rows);
			}
			return pageDataModelListBySql;
		}

		private string GetBankBillRecFilter(MContext ctx, IVBankBillRecListFilterModel filter, ref SqlQuery query)
		{
			string str = "";
			str += " WHERE a.MIsDelete=0 and a.MDate<=@EndDate AND a.MDate >=@StartDate\r\n                            and c.mid is null  and a.MCheckState <> '2' AND IFNULL(a.MParentID,'')=''";
			filter.EndDate = filter.EndDate.ToDayLastSecond();
			MySqlParameter para = new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
			{
				Value = ctx.MOrgID
			};
			MySqlParameter para2 = new MySqlParameter("@StartDate", MySqlDbType.DateTime)
			{
				Value = (object)filter.StartDate
			};
			MySqlParameter para3 = new MySqlParameter("@EndDate", MySqlDbType.DateTime)
			{
				Value = (object)filter.EndDate
			};
			query.AddParameter(para);
			query.AddParameter(para2);
			query.AddParameter(para3);
			if (!string.IsNullOrEmpty(filter.MBankID))
			{
				str += " AND b.MBankID=@MBankID ";
				query.AddParameter(new MySqlParameter("@MBankID", MySqlDbType.VarChar, 36)
				{
					Value = filter.MBankID
				});
			}
			if (filter.ExactDate.HasValue)
			{
				str += " and date(a.MDate)=@ExactDate";
				query.AddParameter(new MySqlParameter("@ExactDate", MySqlDbType.DateTime)
				{
					Value = (object)filter.ExactDate.Value.Date
				});
			}
			if (!string.IsNullOrEmpty(filter.TransAcctName))
			{
				str += " and a.MTransAcctName like concat('%', @TransAcctName, '%')";
				query.AddParameter(new MySqlParameter("@TransAcctName", MySqlDbType.VarChar, 200)
				{
					Value = filter.TransAcctName
				});
			}
			if (!string.IsNullOrEmpty(filter.MDesc))
			{
				str += " and a.MDesc like concat('%', @MDesc, '%')";
				query.AddParameter(new MySqlParameter("@MDesc", MySqlDbType.VarChar, 500)
				{
					Value = filter.MDesc
				});
			}
			if (filter.SrcFrom > 0)
			{
				str += " and a.MBillType=@BillType";
				SqlQuery obj = query;
				MySqlParameter mySqlParameter = new MySqlParameter("@BillType", MySqlDbType.VarChar, 20);
				BillType billType;
				object value;
				if (filter.SrcFrom != 1)
				{
					billType = BillType.Spent;
					value = billType.ToString();
				}
				else
				{
					billType = BillType.Receive;
					value = billType.ToString();
				}
				mySqlParameter.Value = value;
				obj.AddParameter(mySqlParameter);
			}
			if (filter.IsExactAmount)
			{
				if (filter.AmountFrom.HasValue)
				{
					str += " and (a.MSpentAmt=@Amount or a.MReceivedAmt=@Amount)";
					query.AddParameter(new MySqlParameter("@Amount", MySqlDbType.Decimal)
					{
						Value = (object)filter.AmountFrom.Value
					});
				}
			}
			else
			{
				if (filter.AmountFrom.HasValue)
				{
					str += " and ((a.MSpentAmt>=@AmountFrom and a.MReceivedAmt=0) or (a.MReceivedAmt>=@AmountFrom and a.MSpentAmt=0))";
					query.AddParameter(new MySqlParameter("@AmountFrom", MySqlDbType.Decimal)
					{
						Value = (object)filter.AmountFrom.Value
					});
				}
				if (filter.AmountTo.HasValue)
				{
					str += " and ((a.MSpentAmt<=@AmountTo and a.MReceivedAmt=0) or (a.MReceivedAmt<=@AmountTo and a.MSpentAmt=0))";
					query.AddParameter(new MySqlParameter("@AmountTo", MySqlDbType.Decimal)
					{
						Value = (object)filter.AmountTo.Value
					});
				}
			}
			return str;
		}

		public string GetBankBillRecMatchedSql(MContext ctx, IVBankBillRecListFilterModel filter, ref SqlQuery query)
		{
			BDBankAccountRepository bDBankAccountRepository = new BDBankAccountRepository();
			BDBankAccountEditModel bDBankAccountEditModel = bDBankAccountRepository.GetBDBankAccountEditModel(ctx, filter.MBankID);
			BDBankRuleModel bDBankRuleModel = BDBankRuleRepository.GetBankRuleModel(filter.MBankID, ctx);
			if (bDBankRuleModel == null)
			{
				bDBankRuleModel = new BDBankRuleModel();
				bDBankRuleModel.MChkAmount = true;
				bDBankRuleModel.MChkPayee = true;
			}
			GLSettlementRepository gLSettlementRepository = new GLSettlementRepository();
			GLSettlementModel currentSettlement = gLSettlementRepository.GetCurrentSettlement(ctx);
			string str = "select a.MEntryID,a.MDate,a.MBillType,a.MSpentAmt,a.MReceivedAmt,a.MTransAcctName,a.MTransAcctNo,a.MTransNo,a.MDesc,a.MRef,a.Mseq,a.MMatchBillID\r\n                            from (Select a.MEntryID,a.MDate,a.MBillType,a.MSpentAmt,a.MReceivedAmt,a.MTransAcctName,a.MTransAcctNo,a.MTransNo,a.MDesc,a.MRef,a.Mseq,a.MMatchBillID\r\n                            from T_IV_BankBillEntry a INNER JOIN T_IV_BankBill b ON a.MID=b.MID\r\n                            left join T_IV_BankBillReconcile c on a.mentryid=c.MBankBillEntryID\n                            and c.MBankBillEntryID=a.MEntryID AND c.MIsActive=1 and c.MIsDelete=0 and c.MOrgID=@MOrgID";
			str += GetBankBillRecFilter(ctx, filter, ref query);
			str += ")a ";
			str += " inner join (";
			str += string.Format("SELECT a.MID AS MBillID, a.MType, a.MBizDate, a.MReference,a.MNumber, \r\n                            (a.MTaxTotalAmtFor-a.MReconcileAmtFor) AS MSpentAmtFor,0 AS MReceiveAmtFor, a.MDesc as MDescription, \r\n                            convert(AES_DECRYPT(b.MName,'{0}') using utf8) AS MBankAccountName,1 AS MSequence,\r\n                            '{1}' AS MTargetBillType from T_IV_Payment a \r\n                            LEFT JOIN T_BD_Contacts_L b ON a.MContactID=b.MParentID and b.MIsDelete=0 \r\n                            WHERE a.MOrgID=@MOrgID AND a.MBankID=@MBankID AND a.MIsActive=1 AND a.MIsDelete=0   AND MContactType<>'Employees' \r\n                            AND  MReconcileStatu<>@MCompletely AND MReconcileStatu<>@MMarked \r\n                            AND a.MBizDate >=@MInitDate ", "JieNor-001", "Payment");
			str += " UNION ALL ";
			str += string.Format("SELECT a.MID AS MBillID, a.MType, a.MBizDate, a.MReference,a.MNumber, (a.MTaxTotalAmtFor-a.MReconcileAmtFor) AS MSpentAmtFor, \r\n                                0 AS MReceiveAmtFor,a.MDesc as MDescription, \r\n                                F_GetUserName(be.MFirstName,be.MLastName) AS MBankAccountName,1 AS MSequence,\r\n                                '{1}' AS MTargetBillType from T_IV_Payment a \r\n                            LEFT JOIN T_BD_Employees_L be ON a.MContactID=be.MParentID and be.MIsDelete=0 \r\n                            WHERE a.MOrgID=@MOrgID AND a.MBankID=@MBankID AND a.MIsActive=1 AND a.MIsDelete=0  AND MContactType='Employees' \r\n                            AND  MReconcileStatu<>@MCompletely AND MReconcileStatu<>@MMarked AND a.MBizDate>=@MInitDate ", "JieNor-001", "Payment");
			str += " UNION ALL ";
			str += string.Format("SELECT a.MID AS MBillID,a.MType,a.MBizDate, a.MReference,a.MNumber,0 AS MSpentAmtFor,(a.MTaxTotalAmtFor-a.MReconcileAmtFor) AS MReceiveAmtFor, a.MDesc as MDescription ,\r\n                            convert(AES_DECRYPT(b.MName,'{0}') using utf8) AS MBankAccountName,1 AS MSequence,'{1}' AS MTargetBillType from T_IV_Receive a\r\n                            LEFT JOIN T_BD_Contacts_L b ON a.MContactID=b.MParentID and b.MIsDelete=0 \r\n                            WHERE a.MOrgID=@MOrgID AND a.MBankID=@MBankID AND a.MIsActive=1 AND a.MIsDelete=0\r\n                            AND  MReconcileStatu<>@MCompletely AND MReconcileStatu<>@MMarked   AND a.MBizDate>=@MInitDate ", "JieNor-001", "Receive");
			str += " UNION ALL ";
			str += string.Format("SELECT a.MID AS MBillID,a.MType,a.MBizDate, a.MReference,a.MNumber,\r\n                            case when MType='{2}' or MType='{3}' then abs(a.MTaxTotalAmtFor-IFNULL(MVerificationAmt,0)) else 0 end AS MSpentAmtFor,\r\n                            case when MType='{4}' or MType='{5}' then abs(a.MTaxTotalAmtFor-IFNULL(MVerificationAmt,0)) else 0 end AS MReceiveAmtFor, \r\n                            a.MDesc as MDescription,convert(AES_DECRYPT(b.MName,'{0}') using utf8) AS MBankAccountName,2 AS MSequence,\r\n                            '{1}' AS MTargetBillType  from T_IV_Invoice a \r\n                            LEFT JOIN T_BD_Contacts_L b ON a.MContactID=b.MParentID and b.MIsDelete=0 \r\n                            WHERE a.MOrgID=@MOrgID AND MCyID='{6}' AND MStatus='{7}' AND a.MIsActive=1 AND a.MIsDelete=0 ", "JieNor-001", "Invoice", "Invoice_Purchase", "Invoice_Sale_Red", "Invoice_Sale", "Invoice_Purchase_Red", bDBankAccountEditModel.MCyID, Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment));
			str += " UNION ALL ";
			str += string.Format("SELECT a.MID AS MBillID,a.MType,a.MBizDate, a.MReference,a.MNumber, (a.MTaxTotalAmtFor-IFNULL(MVerificationAmt,0)) AS MSpentAmtFor, \r\n                            0 AS MReceiveAmtFor,a.MDesc as MDescription ,F_GetUserName(b.MFirstName,b.MLastName) AS MBankAccountName,3 AS MSequence,\r\n                            '{0}' AS MTargetBillType  from T_IV_Expense a \r\n                            LEFT JOIN T_BD_Employees_L b ON a.MEmployee=b.MParentID and b.MIsDelete=0 \r\n                            WHERE a.MOrgID=@MOrgID AND MCyID='{1}' AND MStatus='{2}' AND a.MIsActive=1 AND a.MIsDelete=0 ", "Expense", bDBankAccountEditModel.MCyID, Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment));
			str += " UNION ALL ";
			str += string.Format("SELECT a.MID AS MBillID,'{0}' AS MType,a.MBizDate,a.MReference,'' AS MNumber,a.MFromTotalAmtFor-IFNULL(MFromReconcileAmtFor,0) AS MSpentAmtFor,\r\n                            a.MToTotalAmtFor - IFNULL(MToReconcileAmtFor,0) AS MReceiveAmtFor,c.MName as MDescription,c.MName AS MBankAccountName,\r\n                            4 AS MSequence,'{0}' AS MTargetBillType FROM T_IV_Transfer a \r\n                            INNER JOIN T_BD_BankAccount b on a.MToAcctID=b.MItemID \r\n                            INNER JOIN T_BD_BankAccount_L c on b.MItemID=c.MParentID And c.MLocaleID=@MLocaleID \r\n                            WHERE MFromAcctID=@MBankID AND  MFromReconcileStatu<>@MCompletely AND MFromReconcileStatu<>@MMarked  AND a.MIsActive=1 AND a.MIsDelete=0 ", "Transfer");
			str += " )sublist";
			str += " on a.MSpentAmt = sublist.MSpentAmtFor and a.MReceivedAmt = sublist.MReceiveAmtFor ";
			if (bDBankRuleModel.MChkPayee)
			{
				str += " and a.MTransAcctName = sublist.MBankAccountName ";
			}
			if (bDBankRuleModel.MChkTransDate)
			{
				str += " and a.MDate = sublist.MBizDate ";
			}
			if (bDBankRuleModel.MChkRef)
			{
				str += " and a.MRef = sublist.MReference ";
			}
			DateTime dateTime;
			if (currentSettlement != null)
			{
				str += string.Format(" where a.MDate>=@curPeriodDate or (a.MDate<@curPeriodDate and sublist.MType not in ('{0}','{1}','{2}','{3}','{4}'))", "Invoice_Sale", "Invoice_Sale_Red", "Invoice_Purchase", "Invoice_Purchase_Red", "Expense_Claims");
				SqlQuery obj = query;
				MySqlParameter mySqlParameter = new MySqlParameter("@curPeriodDate", MySqlDbType.DateTime);
				dateTime = new DateTime(currentSettlement.MYear, currentSettlement.MPeriod, 1);
				mySqlParameter.Value = dateTime.AddMonths(1);
				obj.AddParameter(mySqlParameter);
			}
			query.AddParameter(new MySqlParameter("@MLocaleID", ctx.MLCID));
			SqlQuery obj2 = query;
			dateTime = ctx.MBeginDate;
			obj2.AddParameter(new MySqlParameter("@MInitDate", dateTime.Date));
			query.AddParameter(new MySqlParameter("@MCompletely", Convert.ToInt32(IVReconcileStatus.Completely)));
			query.AddParameter(new MySqlParameter("@MMarked", Convert.ToInt32(IVReconcileStatus.Marked)));
			return str + " group by a.MEntryID,a.MDate,a.MSpentAmt,a.MReceivedAmt,a.MTransAcctName,a.MTransAcctNo,a.MTransNo,a.MDesc,a.MRef,a.MMatchBillID ";
		}

		private static void ResetBankBillRecList(MContext ctx, string bankId, List<IVBankBillRecListModel> list)
		{
			if (list != null && list.Count != 0)
			{
				BDBankRuleModel bDBankRuleModel = BDBankRuleRepository.GetBankRuleModel(bankId, ctx);
				if (bDBankRuleModel == null)
				{
					bDBankRuleModel = new BDBankRuleModel();
					bDBankRuleModel.MChkAmount = true;
					bDBankRuleModel.MChkPayee = true;
				}
				List<IVReconcileTranstionListModel> payementTransList = GetPayementTransList(ctx, list, bankId);
				List<IVReconcileTranstionListModel> receiveTransList = GetReceiveTransList(ctx, list, bankId);
				BDContactsRepository bDContactsRepository = new BDContactsRepository();
				List<NameValueModel> contactNameInfoList = bDContactsRepository.GetContactNameInfoList(ctx);
				BDEmployeesRepository bDEmployeesRepository = new BDEmployeesRepository();
				List<NameValueModel> employeeNameInfoList = bDEmployeesRepository.GetEmployeeNameInfoList(ctx, false);
				List<DateTime> gLCloseDate = GLInterfaceRepository.GetGLCloseDate(ctx);
				foreach (IVBankBillRecListModel item in list)
				{
					List<IVReconcileTranstionListModel> list2 = null;
					if (item.MSpentAmt > decimal.Zero)
					{
						list2 = (from t in payementTransList
						where t.MSpentAmtFor == item.MSpentAmt
						select t).ToList();
					}
					else if (item.MReceivedAmt > decimal.Zero)
					{
						list2 = (from t in receiveTransList
						where t.MReceiveAmtFor == item.MReceivedAmt
						select t).ToList();
					}
					if (list2 != null && list2.Count > 0)
					{
						list2 = ((!IsBankBillDateClose(gLCloseDate, item.MDate)) ? (from t in list2
						orderby t.MSequence, t.MBizDate
						select t).ToList() : (from t in list2
						where t.MType != "Invoice_Sale" && t.MType != "Invoice_Sale_Red" && t.MType != "Invoice_Purchase" && t.MType != "Invoice_Purchase_Red" && t.MType != "Expense_Claims"
						orderby t.MSequence, t.MBizDate
						select t).ToList());
					}
					item.MMatchList = GetMathTransList(bDBankRuleModel, item, list2, contactNameInfoList, employeeNameInfoList);
				}
			}
		}

		private static NameValueModel GetContactNameInfo(List<NameValueModel> contactNameList, string acctName)
		{
			if (string.IsNullOrEmpty(acctName) || contactNameList == null || contactNameList.Count == 0)
			{
				return null;
			}
			acctName = acctName.Trim().ToLower();
			return (from t in contactNameList
			where !string.IsNullOrEmpty(t.MName) && t.MName.Trim().ToLower() == acctName.Trim()
			select t).FirstOrDefault();
		}

		private static NameValueModel GetEmployeeNameInfo(List<NameValueModel> employeeNameList, string acctName)
		{
			if (string.IsNullOrEmpty(acctName) || employeeNameList == null || employeeNameList.Count == 0)
			{
				return null;
			}
			acctName = acctName.Replace(" ", "").ToLower();
			return (from t in employeeNameList
			where $"{t.MName}{t.MTag}".Replace(" ", "").ToLower() == acctName || $"{t.MTag}{t.MName}".Replace(" ", "").ToLower() == acctName
			select t).FirstOrDefault();
		}

		private static List<IVReconcileTranstionListModel> GetMathTransList(BDBankRuleModel ruleModel, IVBankBillRecListModel model, List<IVReconcileTranstionListModel> transList, List<NameValueModel> contactNameList, List<NameValueModel> employeeNameList)
		{
			if (transList == null)
			{
				return new List<IVReconcileTranstionListModel>();
			}
			if (ruleModel.MChkPayee)
			{
				NameValueModel nv = GetContactNameInfo(contactNameList, model.MTransAcctName);
				if (nv != null)
				{
					transList = (from t in transList
					where contactNameList.Exists((NameValueModel e) => e.MName.ToLower() == t.MBankAccountName.ToLower() && e.MValue == nv.MValue)
					select t).ToList();
					goto IL_0108;
				}
				nv = GetEmployeeNameInfo(employeeNameList, model.MTransAcctName);
				if (nv != null)
				{
					transList = (from q in transList
					where employeeNameList.Exists((NameValueModel t) => $"{t.MName}{t.MTag}".Replace(" ", "").ToLower() == q.MBankAccountName.Replace(" ", "").ToLower() || ($"{t.MTag}{t.MName}".Replace(" ", "").ToLower() == q.MBankAccountName.Replace(" ", "").ToLower() && t.MValue == nv.MValue))
					select q).ToList();
					goto IL_0108;
				}
				return new List<IVReconcileTranstionListModel>();
			}
			goto IL_0108;
			IL_0108:
			if (ruleModel.MChkRef)
			{
				transList = (from t in transList
				where !string.IsNullOrEmpty(t.MReference) && t.MReference.Equals(model.MRef, StringComparison.OrdinalIgnoreCase)
				select t).ToList();
			}
			if (ruleModel.MChkTransDate)
			{
				transList = (from t in transList
				where t.MBizDate == model.MDate
				select t).ToList();
			}
			return transList;
		}

		private static List<IVReconcileTranstionListModel> GetPayementTransList(MContext ctx, List<IVBankBillRecListModel> list, string bankId)
		{
			List<decimal> mAmountList = (from t in list
			select t.MSpentAmt).ToList();
			IVReconcileTranstionListFilterModel iVReconcileTranstionListFilterModel = new IVReconcileTranstionListFilterModel();
			iVReconcileTranstionListFilterModel.MAmountList = mAmountList;
			iVReconcileTranstionListFilterModel.MBankID = bankId;
			iVReconcileTranstionListFilterModel.BizObject = "Payment";
			return GetRecTranstionList(ctx, iVReconcileTranstionListFilterModel);
		}

		private static List<IVReconcileTranstionListModel> GetReceiveTransList(MContext ctx, List<IVBankBillRecListModel> list, string bankId)
		{
			List<decimal> mAmountList = (from t in list
			select t.MReceivedAmt).ToList();
			IVReconcileTranstionListFilterModel iVReconcileTranstionListFilterModel = new IVReconcileTranstionListFilterModel();
			iVReconcileTranstionListFilterModel.BizObject = "Receive";
			iVReconcileTranstionListFilterModel.MAmountList = mAmountList;
			iVReconcileTranstionListFilterModel.MBankID = bankId;
			return GetRecTranstionList(ctx, iVReconcileTranstionListFilterModel);
		}

		public static List<IVReconcileTranstionListModel> GetRecTranstionList(MContext ctx, IVReconcileTranstionListFilterModel filter)
		{
			List<string> list = new List<string>();
			BDBankAccountRepository bDBankAccountRepository = new BDBankAccountRepository();
			BDBankAccountEditModel bDBankAccountEditModel = bDBankAccountRepository.GetBDBankAccountEditModel(ctx, filter.MBankID);
			string bizObject = filter.BizObject;
			if (!(bizObject == "Payment"))
			{
				if (bizObject == "Receive")
				{
					list = GetReceiveRecTransListSql(ctx, filter, bDBankAccountEditModel.MCyID);
					return GetRecTranstionList(ctx, filter, list);
				}
				return null;
			}
			list = GetPaymentRecTransListSql(ctx, filter, bDBankAccountEditModel.MCyID);
			return GetRecTranstionList(ctx, filter, list);
		}

		public static List<IVReconcileTranstionListModel> GetRecTranstionList(MContext ctx, IVReconcileTranstionListFilterModel filter, List<string> sqlList)
		{
			List<IVReconcileTranstionListModel> list = new List<IVReconcileTranstionListModel>();
			foreach (string sql in sqlList)
			{
				MySqlParameter[] cmdParms = new MySqlParameter[9]
				{
					new MySqlParameter("@MOrgID", ctx.MOrgID),
					new MySqlParameter("@MLocaleID", ctx.MLCID),
					new MySqlParameter("@MTaxTotalAmtFor", filter.MAmount),
					new MySqlParameter("@MBankID", filter.MBankID),
					new MySqlParameter("@Keyword", filter.Keyword),
					new MySqlParameter("@MBankBillDate", filter.MBankBillDate),
					new MySqlParameter("@MInitDate", ctx.MBeginDate.Date),
					new MySqlParameter("@MCompletely", Convert.ToInt32(IVReconcileStatus.Completely)),
					new MySqlParameter("@MMarked", Convert.ToInt32(IVReconcileStatus.Marked))
				};
				DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
				DataTable dt = dynamicDbHelperMySQL.Query(sql, cmdParms).Tables[0];
				List<IVReconcileTranstionListModel> list2 = ModelInfoManager.DataTableToList<IVReconcileTranstionListModel>(dt);
				if (list2 != null && list2.Count > 0)
				{
					for (int i = 0; i < list2.Count; i++)
					{
						if (string.IsNullOrEmpty(list2[i].MBankAccountName))
						{
							if (!string.IsNullOrEmpty(list2[i].MFirstName) || !string.IsNullOrEmpty(list2[i].MLastName))
							{
								list2[i].MBankAccountName = GlobalFormat.GetUserName(list2[i].MFirstName, list2[i].MLastName, null);
							}
							else
							{
								list2[i].MBankAccountName = "";
							}
						}
						list2[i].MDescription = ((list2[i].MDescription == null) ? "" : list2[i].MDescription);
						list2[i].MReference = ((list2[i].MReference == null) ? "" : list2[i].MReference);
					}
				}
				list.AddRange(list2);
			}
			if (list == null || list.Count == 0)
			{
				return list;
			}
			if (string.IsNullOrEmpty(filter.Order) || filter.Order == "asc")
			{
				if (filter.Sort == "MBizDate")
				{
					return (from t in list
					orderby t.MBizDate
					select t).ToList();
				}
				if (filter.Sort == "MBankAccountName")
				{
					return (from t in list
					orderby t.MBankAccountName
					select t).ToList();
				}
				if (filter.Sort == "MReference")
				{
					return (from t in list
					orderby t.MReference
					select t).ToList();
				}
				if (filter.Sort == "MTargetBillType")
				{
					return (from t in list
					orderby t.MTargetBillType
					select t).ToList();
				}
				if (filter.Sort == "MSpentAmtFor")
				{
					return (from t in list
					orderby t.MSpentAmtFor
					select t).ToList();
				}
				if (filter.Sort == "MReceiveAmtFor")
				{
					return (from t in list
					orderby t.MReceiveAmtFor
					select t).ToList();
				}
			}
			else
			{
				if (filter.Sort == "MBizDate")
				{
					return (from t in list
					orderby t.MBizDate descending
					select t).ToList();
				}
				if (filter.Sort == "MBankAccountName")
				{
					return (from t in list
					orderby t.MBankAccountName descending
					select t).ToList();
				}
				if (filter.Sort == "MReference")
				{
					return (from t in list
					orderby t.MReference descending
					select t).ToList();
				}
				if (filter.Sort == "MTargetBillType")
				{
					return (from t in list
					orderby t.MTargetBillType descending
					select t).ToList();
				}
				if (filter.Sort == "MSpentAmtFor")
				{
					return (from t in list
					orderby t.MSpentAmtFor descending
					select t).ToList();
				}
				if (filter.Sort == "MReceiveAmtFor")
				{
					return (from t in list
					orderby t.MReceiveAmtFor descending
					select t).ToList();
				}
			}
			return (from t in list
			orderby t.MBizDate descending
			select t).ToList();
		}

		private static bool IsBankBillDateClose(List<DateTime> glCloseDate, DateTime bankBillDate)
		{
			if (glCloseDate == null || glCloseDate.Count == 0)
			{
				return false;
			}
			foreach (DateTime item in glCloseDate)
			{
				if (item.Year == bankBillDate.Year && item.Month == bankBillDate.Month)
				{
					return true;
				}
			}
			return false;
		}

		private static List<string> GetPaymentRecTransListSql(MContext ctx, IVReconcileTranstionListFilterModel filter, string cyId)
		{
			List<DateTime> gLCloseDate = GLInterfaceRepository.GetGLCloseDate(ctx);
			bool flag = IsBankBillDateClose(gLCloseDate, filter.MBankBillDate);
			List<string> list = new List<string>();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT a.MID AS MBillID, a.MType, a.MBizDate, a.MReference,a.MNumber, (a.MTaxTotalAmtFor-a.MReconcileAmtFor) AS MSpentAmtFor, \r\n                                a.MDesc as MDescription, \r\n                                convert(AES_DECRYPT(b.MName,'{0}') using utf8) AS MBankAccountName,a.MContactID, '' AS MFirstName,'' AS MLastName, 1 AS MSequence,\r\n                                '{1}' AS MTargetBillType from T_IV_Payment a ", "JieNor-001", "Payment");
			stringBuilder.Append("LEFT JOIN T_BD_Contacts_L b ON a.MContactID=b.MParentID AND b.MLocaleID=@MLocaleID and b.MIsDelete=0 ");
			stringBuilder.Append("WHERE a.MOrgID=@MOrgID AND a.MBankID=@MBankID AND a.MIsActive=1 AND a.MIsDelete=0   AND MContactType<>'Employees' ");
			stringBuilder.Append(" AND  MReconcileStatu<>@MCompletely AND MReconcileStatu<>@MMarked ");
			stringBuilder.Append(" AND a.MBizDate >=@MInitDate ");
			if (!string.IsNullOrWhiteSpace(filter.Keyword))
			{
				stringBuilder.AppendFormat(" AND (exists(SELECT 1 FROM T_BD_Contacts_L c where c.MParentID=a.MContactID AND c.MIsDelete=0 AND convert(AES_DECRYPT(c.MName,'{0}') using utf8)  LIKE concat('%', @Keyword, '%') ) \r\n                                                OR a.MReference LIKE concat('%', @Keyword, '%') \r\n                                                OR a.MDesc LIKE concat('%', @Keyword, '%')) ", "JieNor-001");
			}
			if (filter.MAmount > decimal.Zero)
			{
				stringBuilder.Append(" AND (a.MTaxTotalAmtFor- ifnull(a.MReconcileAmtFor,0))=@MTaxTotalAmtFor ");
			}
			if (filter.MAmountList != null && filter.MAmountList.Count > 0)
			{
				stringBuilder.AppendFormat(" AND (a.MTaxTotalAmtFor-a.MReconcileAmtFor) IN ({0}) ", string.Join(",", filter.MAmountList));
			}
			stringBuilder.Append(" UNION ALL ");
			stringBuilder.AppendFormat("SELECT a.MID AS MBillID, a.MType, a.MBizDate, a.MReference,a.MNumber, (a.MTaxTotalAmtFor-a.MReconcileAmtFor) AS MSpentAmtFor, \r\n                                a.MDesc as MDescription, \r\n                                '' AS MBankAccountName,a.MContactID,be.MFirstName AS MFirstName,be.MLastName AS MLastName, 1 AS MSequence,\r\n                                '{1}' AS MTargetBillType from T_IV_Payment a ", "JieNor-001", "Payment");
			stringBuilder.Append("LEFT JOIN T_BD_Employees_L be ON a.MContactID=be.MParentID AND be.MLocaleID=@MLocaleID  and be.MIsDelete=0 ");
			stringBuilder.Append("WHERE a.MOrgID=@MOrgID AND a.MBankID=@MBankID AND a.MIsActive=1 AND a.MIsDelete=0  AND MContactType='Employees' ");
			stringBuilder.Append(" AND  MReconcileStatu<>@MCompletely AND MReconcileStatu<>@MMarked ");
			stringBuilder.Append(" AND a.MBizDate>=@MInitDate ");
			if (!string.IsNullOrWhiteSpace(filter.Keyword))
			{
				stringBuilder.AppendFormat(" AND (exists(SELECT 1 FROM T_BD_Employees_L c where c.MParentID=a.MContactID \r\n                                                    AND (F_GetUserName(be.MFirstName,be.MLastName) like CONCAT('%',@Keyword, '%') \r\n                                          OR a.MReference LIKE concat('%', @Keyword, '%')))) ", "JieNor-001");
			}
			if (filter.MAmount > decimal.Zero)
			{
				stringBuilder.Append(" AND MTaxTotalAmtFor=@MTaxTotalAmtFor ");
			}
			if (filter.MAmountList != null && filter.MAmountList.Count > 0)
			{
				stringBuilder.AppendFormat(" AND (a.MTaxTotalAmtFor-a.MReconcileAmtFor) IN ({0}) ", string.Join(",", filter.MAmountList));
			}
			StringBuilder stringBuilder2 = new StringBuilder();
			stringBuilder2.AppendFormat("SELECT a.MID AS MBillID,a.MType,a.MBizDate, a.MReference,a.MNumber, (a.MTaxTotalAmtFor-IFNULL(MVerificationAmt,0)) AS MSpentAmtFor, a.MDesc as MDescription ,\r\n                                    convert(AES_DECRYPT(b.MName,'{0}') using utf8) AS MBankAccountName,a.MContactID,2 AS MSequence,\r\n                                    '{1}' AS MTargetBillType  from T_IV_Invoice a ", "JieNor-001", "Invoice");
			stringBuilder2.Append("LEFT JOIN T_BD_Contacts_L b ON a.MContactID=b.MParentID AND b.MLocaleID=@MLocaleID  and b.MIsDelete=0 ");
			stringBuilder2.AppendFormat("WHERE a.MOrgID=@MOrgID AND (MType='{0}' OR MType='{1}') AND MCyID='{2}' AND MStatus='{3}' AND a.MIsActive=1 AND a.MIsDelete=0 ", "Invoice_Purchase", "Invoice_Sale_Red", cyId, Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment));
			stringBuilder2.AppendFormat("AND MStatus='{0}' ", Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment));
			if (!string.IsNullOrWhiteSpace(filter.Keyword))
			{
				stringBuilder2.AppendFormat(" AND (exists(SELECT 1 FROM T_BD_Contacts_L c where c.MParentID=a.MContactID AND c.MIsDelete=0 AND convert(AES_DECRYPT(c.MName,'{0}') using utf8)  LIKE concat('%', @Keyword, '%') ) \r\n                                                OR concat('[ ',IFNULL(a.MNumber,''),' ] ',IFNULL(a.MReference,'')) LIKE concat('%', @Keyword, '%'))", "JieNor-001");
			}
			if (filter.MAmount > decimal.Zero)
			{
				stringBuilder2.Append(" AND (a.MTaxTotalAmtFor-IFNULL(MVerificationAmt,0))=@MTaxTotalAmtFor ");
			}
			if (filter.MAmountList != null && filter.MAmountList.Count > 0)
			{
				stringBuilder2.AppendFormat(" AND abs(a.MTaxTotalAmtFor-IFNULL(MVerificationAmt,0)) IN ({0}) ", string.Join(",", filter.MAmountList));
			}
			StringBuilder stringBuilder3 = new StringBuilder();
			stringBuilder3.AppendFormat("SELECT a.MID AS MBillID,a.MType,a.MBizDate, a.MReference,a.MNumber, (a.MTaxTotalAmtFor-IFNULL(MVerificationAmt,0)) AS MSpentAmtFor, \r\n                                        a.MDesc as MDescription ,3 AS MSequence,\r\n                                        '' AS MBankAccountName,a.MContactID,b.MFirstName AS MFirstName,b.MLastName AS MLastName, '{0}' AS MTargetBillType  from T_IV_Expense a ", "Expense");
			stringBuilder3.Append("LEFT JOIN T_BD_Employees_L b ON a.MEmployee=b.MParentID AND b.MLocaleID=@MLocaleID   and b.MIsDelete=0 ");
			stringBuilder3.AppendFormat("WHERE a.MOrgID=@MOrgID AND MCyID='{0}' AND MStatus='{1}' AND a.MIsActive=1 AND a.MIsDelete=0 ", cyId, Convert.ToInt32(IVExpenseStatusEnum.WaitingPayment));
			stringBuilder3.AppendFormat("AND MStatus='{0}' ", Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment));
			if (!string.IsNullOrWhiteSpace(filter.Keyword))
			{
				stringBuilder3.AppendFormat(" AND (exists(SELECT 1 FROM T_BD_Employees_L c where c.MParentID=a.MEmployee \r\n                                                                AND (F_GetUserName(b.MFirstName,b.MLastName) like CONCAT('%',@Keyword, '%') \r\n                                                OR a.MReference LIKE concat('%', @Keyword, '%')))) ", "JieNor-001");
			}
			if (filter.MAmount > decimal.Zero)
			{
				stringBuilder3.Append(" AND (a.MTaxTotalAmtFor-IFNULL(MVerificationAmt,0))=@MTaxTotalAmtFor ");
			}
			if (filter.MAmountList != null && filter.MAmountList.Count > 0)
			{
				stringBuilder3.AppendFormat(" AND (a.MTaxTotalAmtFor-IFNULL(MVerificationAmt,0)) IN ({0}) ", string.Join(",", filter.MAmountList));
			}
			StringBuilder stringBuilder4 = new StringBuilder();
			stringBuilder4.AppendFormat("SELECT a.MID AS MBillID,a.MBizDate,a.MReference,'' AS MNumber,a.MFromTotalAmtFor-IFNULL(MFromReconcileAmtFor,0) AS MSpentAmtFor,c.MName as MDescription,c.MName AS MBankAccountName,'{0}' AS MType,4 AS MSequence,'{0}' AS MTargetBillType FROM T_IV_Transfer a ", "Transfer");
			stringBuilder4.Append("INNER JOIN T_BD_BankAccount b on a.MToAcctID=b.MItemID ");
			stringBuilder4.Append("INNER JOIN T_BD_BankAccount_L c on b.MItemID=c.MParentID And c.MLocaleID=@MLocaleID ");
			stringBuilder4.Append("WHERE MFromAcctID=@MBankID AND  MFromReconcileStatu<>@MCompletely AND MFromReconcileStatu<>@MMarked  AND a.MIsActive=1 AND a.MIsDelete=0 ");
			if (!string.IsNullOrWhiteSpace(filter.Keyword))
			{
				stringBuilder4.Append(" AND c.MName LIKE concat('%', @Keyword, '%') ");
			}
			if (filter.MAmount > decimal.Zero)
			{
				stringBuilder4.Append(" AND a.MFromTotalAmtFor-IFNULL(MFromReconcileAmtFor,0)=@MTaxTotalAmtFor ");
			}
			if (filter.MAmountList != null && filter.MAmountList.Count > 0)
			{
				stringBuilder4.AppendFormat(" AND a.MFromTotalAmtFor-IFNULL(MFromReconcileAmtFor,0) IN ({0}) ", string.Join(",", filter.MAmountList));
			}
			list.Add(stringBuilder.ToString());
			if (!flag)
			{
				list.Add(stringBuilder2.ToString());
				list.Add(stringBuilder3.ToString());
			}
			list.Add(stringBuilder4.ToString());
			return list;
		}

		private static List<string> GetReceiveRecTransListSql(MContext ctx, IVReconcileTranstionListFilterModel filter, string cyId)
		{
			List<DateTime> gLCloseDate = GLInterfaceRepository.GetGLCloseDate(ctx);
			bool flag = IsBankBillDateClose(gLCloseDate, filter.MBankBillDate);
			List<string> list = new List<string>();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT a.MID AS MBillID,a.MType,a.MBizDate, a.MReference,a.MNumber, (a.MTaxTotalAmtFor-a.MReconcileAmtFor) AS MReceiveAmtFor, a.MDesc as MDescription ,1 AS MSequence,convert(AES_DECRYPT(b.MName,'{0}') using utf8) AS MBankAccountName,a.MContactID,'{1}' AS MTargetBillType\r\n                            from T_IV_Receive a\r\n                            LEFT JOIN T_BD_Contacts_L b ON a.MContactID=b.MParentID AND b.MLocaleID=@MLocaleID  and b.MIsDelete=0 \r\n                            WHERE a.MOrgID=@MOrgID AND a.MBankID=@MBankID AND a.MIsActive=1 AND a.MIsDelete=0\r\n                                 AND  MReconcileStatu<>@MCompletely AND MReconcileStatu<>@MMarked   AND a.MBizDate>=@MInitDate  ", "JieNor-001", "Receive");
			if (!string.IsNullOrWhiteSpace(filter.Keyword))
			{
				stringBuilder.AppendFormat(" AND (exists(SELECT 1 FROM T_BD_Contacts_L c where c.MParentID=a.MContactID AND c.MIsDelete=0 AND convert(AES_DECRYPT(c.MName,'{0}') using utf8)  LIKE concat('%', @Keyword, '%') ) \r\n                                                OR a.MReference LIKE concat('%', @Keyword, '%') \r\n                                                OR a.MDesc LIKE concat('%', @Keyword, '%')) ", "JieNor-001");
			}
			if (filter.MAmount > decimal.Zero)
			{
				stringBuilder.Append(" AND  (a.MTaxTotalAmtFor-a.MReconcileAmtFor)=@MTaxTotalAmtFor ");
			}
			if (filter.MAmountList != null && filter.MAmountList.Count > 0)
			{
				stringBuilder.AppendFormat(" AND (a.MTaxTotalAmtFor-a.MReconcileAmtFor) IN ({0}) ", string.Join(",", filter.MAmountList));
			}
			StringBuilder stringBuilder2 = new StringBuilder();
			stringBuilder2.AppendFormat("SELECT a.MID AS MBillID,a.MType,a.MBizDate, a.MReference,a.MNumber, (a.MTaxTotalAmtFor-IFNULL(MVerificationAmt,0)) AS MReceiveAmtFor, a.MDesc as MDescription ,convert(AES_DECRYPT(b.MName,'{0}') using utf8) AS MBankAccountName,a.MContactID,2 AS MSequence,2 AS MSequence,'{1}' AS MTargetBillType  from T_IV_Invoice a ", "JieNor-001", "Invoice");
			stringBuilder2.Append("LEFT JOIN T_BD_Contacts_L b ON a.MContactID=b.MParentID AND b.MLocaleID=@MLocaleID   and b.MIsDelete=0  ");
			stringBuilder2.AppendFormat("WHERE a.MOrgID=@MOrgID AND (MType='{0}' OR MType='{1}') AND MCyID='{2}' AND MStatus='{3}' AND a.MIsActive=1 AND a.MIsDelete=0 ", "Invoice_Sale", "Invoice_Purchase_Red", cyId, Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment));
			stringBuilder2.AppendFormat("AND MStatus='{0}' ", Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment));
			if (!string.IsNullOrWhiteSpace(filter.Keyword))
			{
				stringBuilder2.AppendFormat(" AND (exists(SELECT 1 FROM T_BD_Contacts_L c where c.MParentID=a.MContactID AND c.MIsDelete=0 AND convert(AES_DECRYPT(c.MName,'{0}') using utf8)  LIKE concat('%', @Keyword, '%') ) \r\n                                                OR concat('[ ',IFNULL(a.MNumber,''),' ] ',IFNULL(a.MReference,'')) LIKE concat('%', @Keyword, '%'))", "JieNor-001");
			}
			if (filter.MAmount > decimal.Zero)
			{
				stringBuilder2.Append(" AND (a.MTaxTotalAmtFor-IFNULL(MVerificationAmt,0))=@MTaxTotalAmtFor ");
			}
			if (filter.MAmountList != null && filter.MAmountList.Count > 0)
			{
				stringBuilder2.AppendFormat(" AND abs(a.MTaxTotalAmtFor-IFNULL(MVerificationAmt,0)) IN ({0}) ", string.Join(",", filter.MAmountList));
			}
			StringBuilder stringBuilder3 = new StringBuilder();
			stringBuilder3.AppendFormat("SELECT a.MID AS MBillID,a.MBizDate,a.MReference,'' AS MNumber,(a.MToTotalAmtFor - IFNULL(MToReconcileAmtFor,0)) AS MReceiveAmtFor,c.MName as MDescription,c.MName AS MBankAccountName,3 AS MSequence,'{0}' AS MType,'{0}' AS MTargetBillType\r\n                        FROM T_IV_Transfer a\r\n                        INNER JOIN T_BD_BankAccount b on a.MFromAcctID=b.MItemID\r\n                        INNER JOIN T_BD_BankAccount_L c on b.MItemID=c.MParentID And c.MLocaleID=@MLocaleID\r\n                        WHERE MToAcctID=@MBankID  AND a.MIsActive=1 AND a.MIsDelete=0 \r\n                        AND  MToReconcileStatu<>@MCompletely AND MToReconcileStatu<>@MMarked ", "Transfer");
			if (!string.IsNullOrWhiteSpace(filter.Keyword))
			{
				stringBuilder3.AppendLine(" AND c.MName LIKE concat('%', @Keyword, '%') ");
			}
			if (filter.MAmount > decimal.Zero)
			{
				stringBuilder3.Append(" AND (a.MToTotalAmtFor - IFNULL(MToReconcileAmtFor,0)) =@MTaxTotalAmtFor ");
			}
			if (filter.MAmountList != null && filter.MAmountList.Count > 0)
			{
				stringBuilder3.AppendFormat(" AND (a.MToTotalAmtFor - IFNULL(MToReconcileAmtFor,0)) IN ({0}) ", string.Join(",", filter.MAmountList));
			}
			list.Add(stringBuilder.ToString());
			list.Add(stringBuilder3.ToString());
			if (!flag)
			{
				list.Add(stringBuilder2.ToString());
			}
			return list;
		}

		private static void UpdateReconcileAmt(MContext ctx, List<IVBankBillReconcileEntryModel> list)
		{
			string text = "SELECT MTargetBillID,MTargetBillType,SUM(MSpentAmtFor) AS MSpentAmtFor,SUM(MReceiveAmtFor) AS MReceiveAmtFor FROM T_IV_BankBillReconcileEntry WHERE MIsActive=1 AND MIsDelete=0  GROUP BY MTargetBillID,MTargetBillType";
			List<CommandInfo> list2 = new List<CommandInfo>();
			foreach (IVBankBillReconcileEntryModel item in list)
			{
				string text2 = "";
				if (item.MTargetBillType == "Payment")
				{
					text2 = string.Format("UPDATE T_IV_Payment INNER JOIN ({0}) a ON a.MTargetBillID=T_IV_Payment.MID \n                            SET MReconcileAmt= CASE WHEN (MType='{1}' OR MType='{2}' OR MType='{3}' OR MType='{4}') THEN round(a.MSpentAmtFor*MExchangeRate,2) ELSE -round(a.MReceiveAmtFor*MExchangeRate,2)  END, \n                            MReconcileAmtFor = CASE WHEN (MType='{1}' OR MType='{2}' OR MType='{3}' OR MType='{4}') THEN a.MSpentAmtFor ELSE -a.MReceiveAmtFor END\n                            WHERE T_IV_Payment.MOrgID=@MOrgID AND MTargetBillType=@MBillType AND a.MTargetBillID=@MBillID", text, "Pay_Purchase", "Pay_Other", "Pay_Adjustment", "Pay_BankFee");
				}
				else if (item.MTargetBillType == "Receive")
				{
					text2 = string.Format("UPDATE T_IV_Receive INNER JOIN ({0}) a ON a.MTargetBillID=T_IV_Receive.MID \n                            SET MReconcileAmt= CASE WHEN (MType='{1}' OR MType='{2}' OR MType='{3}' OR MType='{4}') THEN round(a.MReceiveAmtFor*MExchangeRate,2) ELSE  -round(a.MSpentAmtFor*MExchangeRate,2) END,\n                            MReconcileAmtFor =  CASE WHEN (MType='{1}' OR MType='{2}' OR MType='{3}' OR MType='{4}') THEN  a.MReceiveAmtFor ELSE -a.MSpentAmtFor END\n                            WHERE T_IV_Receive.MOrgID=@MOrgID AND MTargetBillType=@MBillType AND a.MTargetBillID=@MBillID", text, "Receive_Sale", "Receive_Other", "Receive_Adjustment", "Receive_BankFee");
				}
				else if (item.MTargetBillType == "Transfer")
				{
					text2 = $"UPDATE T_IV_Transfer  INNER JOIN ({text}) a ON a.MTargetBillID=T_IV_Transfer.MID \r\n                                        SET MFromReconcileAmt = round(a.MSpentAmtFor*MExchangeRate,2), MFromReconcileAmtFor=a.MSpentAmtFor,\r\n                                        MToReconcileAmt = a.MReceiveAmtFor*MExchangeRate, MToReconcileAmtFor=a.MReceiveAmtFor\n                                        WHERE T_IV_Transfer.MOrgID=@MOrgID AND MTargetBillType=@MBillType AND a.MTargetBillID=@MBillID";
				}
				MySqlParameter[] parameters = new MySqlParameter[3]
				{
					new MySqlParameter("@MOrgID", ctx.MOrgID),
					new MySqlParameter("@MBillType", item.MTargetBillType),
					new MySqlParameter("@MBillID", item.MTargetBillID)
				};
				if (!string.IsNullOrEmpty(text2))
				{
					List<CommandInfo> list3 = list2;
					CommandInfo obj = new CommandInfo
					{
						CommandText = text2
					};
					DbParameter[] array = obj.Parameters = parameters;
					list3.Add(obj);
				}
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			dynamicDbHelperMySQL.ExecuteSqlTran(list2);
		}

		private List<CommandInfo> GetUpdateReconcileAmtCmd(MContext ctx, List<IVBankBillReconcileEntryModel> list)
		{
			string arg = "SELECT MTargetBillID,MTargetBillType,SUM(MSpentAmtFor) AS MSpentAmtFor,SUM(MReceiveAmtFor) AS MReceiveAmtFor FROM T_IV_BankBillReconcileEntry WHERE MIsActive=1 AND MIsDelete=0 GROUP BY MTargetBillID,MTargetBillType";
			List<CommandInfo> list2 = new List<CommandInfo>();
			foreach (IVBankBillReconcileEntryModel item in list)
			{
				string text = "";
				if (item.MTargetBillType == "Payment")
				{
					text = $"UPDATE T_IV_Payment b INNER JOIN ({arg}) a ON a.MTargetBillID=b.MID \n                            SET MReconcileAmt= round( a.MSpentAmtFor*MExchangeRate,2), \n                            MReconcileAmtFor = a.MSpentAmtFor,\n                            MReconcileStatu =(\n                             CASE WHEN a.MSpentAmtFor<>0 AND a.MSpentAmtFor=MTaxTotalAmtFor THEN @MCompletely \n                             WHEN a.MSpentAmtFor<>0 AND a.MSpentAmtFor<>MTaxTotalAmtFor THEN @MPartly\n                             ELSE @MNone END)\n                            WHERE b.MOrgID=@MOrgID AND MTargetBillType=@MBillType AND a.MTargetBillID=@MBillID";
				}
				else if (item.MTargetBillType == "Receive")
				{
					text = $"UPDATE T_IV_Receive b INNER JOIN ({arg}) a ON a.MTargetBillID=b.MID \n                            SET MReconcileAmt= round( a.MReceiveAmtFor*MExchangeRate,2),\n                            MReconcileAmtFor =  a.MReceiveAmtFor,\n                            MReconcileStatu =(\n                             CASE WHEN a.MReceiveAmtFor<>0 AND a.MReceiveAmtFor=MTaxTotalAmtFor THEN @MCompletely \n                             WHEN a.MReceiveAmtFor<>0 AND a.MReceiveAmtFor<>MTaxTotalAmtFor THEN @MPartly\n                             ELSE @MNone END)\n                            WHERE b.MOrgID=@MOrgID AND MTargetBillType=@MBillType AND a.MTargetBillID=@MBillID";
				}
				else if (item.MTargetBillType == "Transfer")
				{
					text = $"UPDATE T_IV_Transfer  INNER JOIN ({arg}) a ON a.MTargetBillID=T_IV_Transfer.MID \r\n                                        SET MFromReconcileAmt =round(a.MSpentAmtFor*(MFromTotalAmt/MFromTotalAmtFor),2), MFromReconcileAmtFor=a.MSpentAmtFor,\r\n                                        MToReconcileAmt = round(a.MReceiveAmtFor*(MToTotalAmt/MToTotalAmtFor),2), MToReconcileAmtFor=a.MReceiveAmtFor,\n                                        MFromReconcileStatu =(\n                                         CASE WHEN a.MSpentAmtFor<>0 AND a.MSpentAmtFor=MFromTotalAmtFor THEN @MCompletely \n                                         WHEN a.MSpentAmtFor<>0 AND a.MSpentAmtFor<>MFromTotalAmtFor THEN @MPartly\n                                         ELSE @MNone END),\n                                        MToReconcileStatu =(\n                                         CASE WHEN a.MReceiveAmtFor<>0 AND a.MReceiveAmtFor=MToTotalAmtFor THEN @MCompletely \n                                         WHEN a.MReceiveAmtFor<>0 AND a.MReceiveAmtFor<>MToTotalAmtFor THEN @MPartly\n                                         ELSE @MNone END)\n                                        WHERE T_IV_Transfer.MOrgID=@MOrgID AND MTargetBillType=@MBillType AND a.MTargetBillID=@MBillID";
				}
				MySqlParameter[] parameters = new MySqlParameter[6]
				{
					new MySqlParameter("@MOrgID", ctx.MOrgID),
					new MySqlParameter("@MBillType", item.MTargetBillType),
					new MySqlParameter("@MBillID", item.MTargetBillID),
					new MySqlParameter("@MCompletely", Convert.ToInt32(IVReconcileStatus.Completely)),
					new MySqlParameter("@MPartly", Convert.ToInt32(IVReconcileStatus.Partly)),
					new MySqlParameter("@MNone", Convert.ToInt32(IVReconcileStatus.None))
				};
				if (!string.IsNullOrEmpty(text))
				{
					List<CommandInfo> list3 = list2;
					CommandInfo obj = new CommandInfo
					{
						CommandText = text
					};
					DbParameter[] array = obj.Parameters = parameters;
					list3.Add(obj);
				}
			}
			return list2;
		}

		public List<CommandInfo> GetUpdateReconcileMatchCmd(MContext ctx, string entryID, string matchBillID)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			List<CommandInfo> list2 = list;
			CommandInfo commandInfo = new CommandInfo
			{
				CommandText = "update T_IV_BankBillEntry set MMatchBillID=null where MEntryID<>@MEntryID and MMatchBillID=@MMatchBillID and MOrgID=@MOrgID"
			};
			DbParameter[] array = commandInfo.Parameters = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MEntryID", entryID),
				new MySqlParameter("@MMatchBillID", matchBillID)
			};
			list2.Add(commandInfo);
			List<CommandInfo> list3 = list;
			commandInfo = new CommandInfo
			{
				CommandText = "update T_IV_BankBillEntry set MMatchBillID=@MMatchBillID where MEntryID=@MEntryID and MOrgID=@MOrgID"
			};
			array = (commandInfo.Parameters = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MEntryID", entryID),
				new MySqlParameter("@MMatchBillID", matchBillID)
			});
			list3.Add(commandInfo);
			return list;
		}
	}
}
