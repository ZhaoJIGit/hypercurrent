using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.FileUtility;
using JieNor.Megi.Core.MultiLanguage;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.IV.Param;
using JieNor.Megi.DataRepository.BD;
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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;

namespace JieNor.Megi.DataRepository.IV
{
	public class IVTransactionsRepository : IVBaseRepository<IVPaymentModel>
	{
		public static DataGridJson<IVAccountTransactionsModel> GetTransactionsList(MContext ctx, IVAccountTransactionsListFilterModel param)
		{
			StringBuilder stringBuilder = new StringBuilder();
			List<MySqlParameter> list = new List<MySqlParameter>();
			string bankIdInSql = GetBankIdInSql(param, list);
			bool flag = string.IsNullOrWhiteSpace(param.TransactionType);
			if ((param.TransactionType == "Payment" | flag) && param.SrcFrom != 1)
			{
				stringBuilder.AppendFormat(" SELECT 'Payment' as MTransType,t1.MModifyDate, t1.MID,t1.MBankID,t1.MBizDate,t1.MReference,  t1.MDesc AS MDescription , IFNULL(convert(AES_DECRYPT(t4.MName,'{0}') using utf8),IFNULL(F_GetUserName(t7.MFirstName,t7.MLastName),'')) AS MContactName,", "JieNor-001");
				stringBuilder.AppendLine(" t1.MTaxTotalAmtFor as MSpent,'' as MReceived,t1.MType,group_concat(DISTINCT t5.MAttachID) AS MAttachIDs, MReconcileStatu,ifnull(t1.MVerificationAmt,0) as MVerificationAmt  , MVerifyAmt ,MVerifyAmtFor,t1.MCreateDate, ");
				stringBuilder.AppendLine(" t6.RecDate,t6.RecTime,t6.RecSeq,t6.RecCreateDate");
				stringBuilder.AppendLine(" FROM T_IV_Payment t1");
				stringBuilder.AppendLine(" LEFT JOIN T_BD_Contacts t3 on t1.MOrgID = t3.MOrgID and  t1.MContactID=t3.MItemID AND t3.MIsDelete=0 ");
				stringBuilder.AppendLine(" LEFT JOIN T_BD_Contacts_L t4 on t1.MOrgID = t4.MOrgID and  t3.MItemID=t4.MParentID And t4.MLocaleID=@MLocaleID AND t4.MIsDelete=0 ");
				stringBuilder.AppendLine(" left join T_BD_Employees_L t7 on t1.MOrgID = t7.MOrgID and t7.MParentID=t1.MContactID and t7.MLocaleID=@MLocaleID AND t7.MIsDelete=0 ");
				stringBuilder.AppendLine(" LEFT JOIN T_IV_PaymentAttachment t5 ON t1.MID=t5.MParentID and t5.MOrgID = t1.MOrgID AND t5.MIsDelete=0 ");
				stringBuilder.AppendLine("LEFT JOIN \r\n                                    (\r\n                                        select t4.MTargetBillID, t1.MDate AS RecDate, t1.MTime AS RecTime, t1.MSeq AS RecSeq, t1.MCreateDate AS RecCreateDate from t_iv_bankbillentry t1\r\n                                            INNER JOIN t_iv_bankbill t2 ON t1.MID = t2.MID AND t2.MIsDelete = 0 AND t2.MOrgID=@MOrgID\r\n                                            INNER JOIN t_iv_bankbillreconcile t3 ON t1.MEntryID = t3.MBankBillEntryID AND t3.MIsDelete=0 AND t3.MOrgID=@MOrgID\r\n                                            INNER JOIN t_iv_bankbillreconcileentry t4 ON t3.MID = t4.MID AND t4.MIsDelete=0 AND t4.MOrgID=@MOrgID\r\n                                            WHERE t1.MIsDelete = 0 AND t1.MOrgID=@MOrgID AND t4.MTargetBillType='Payment'\r\n                                    ) t6 ON t1.MID=t6.MTargetBillID");
				stringBuilder.AppendFormat(" WHERE t1.MIsDelete=0 and t1.MBankID in ({0}) AND t1.MOrgID=@MOrgID", bankIdInSql);
				stringBuilder.AppendLine(" And t1.MBizDate >= @StartDate And t1.MBizDate<=@EndDate ");
				stringBuilder.AppendLine(GetTransactionsListFilter(ctx, param, GLDocTypeEnum.Payment));
				stringBuilder.AppendLine(" GROUP BY t1.MID");
			}
			if ((param.TransactionType == "Receive" | flag) && param.SrcFrom != 2)
			{
				if (!string.IsNullOrWhiteSpace(stringBuilder.ToString()))
				{
					stringBuilder.AppendLine(" UNION ALL");
				}
				stringBuilder.AppendFormat(" SELECT  'Receive' as MTransType,t1.MModifyDate, t1.MID,t1.MBankID,t1.MBizDate,t1.MReference,  t1.MDesc AS MDescription , IFNULL(convert(AES_DECRYPT(t4.MName,'{0}') using utf8),IFNULL(F_GetUserName(t7.MFirstName,t7.MLastName),'')) AS MContactName, ", "JieNor-001");
				stringBuilder.AppendLine(" '' as MSpent,t1.MTaxTotalAmtFor as MReceived,t1.MType,group_concat(DISTINCT t5.MAttachID) AS MAttachIDs,MReconcileStatu,ifnull(t1.MVerificationAmt,0) as MVerificationAmt , MVerifyAmt ,MVerifyAmtFor,t1.MCreateDate, ");
				stringBuilder.AppendLine(" t6.RecDate,t6.RecTime,t6.RecSeq,t6.RecCreateDate");
				stringBuilder.AppendLine(" FROM T_IV_Receive t1");
				stringBuilder.AppendLine(" LEFT JOIN T_BD_Contacts t3 on t1.MOrgID = t3.MOrgID and  t1.MContactID=t3.MItemID AND t3.MIsDelete=0 ");
				stringBuilder.AppendLine(" LEFT JOIN T_BD_Contacts_L t4 on t4.MOrgID = t1.MOrgID and t3.MItemID=t4.MParentID And t4.MLocaleID=@MLocaleID AND t4.MIsDelete=0 ");
				stringBuilder.AppendLine(" left join T_BD_Employees_L t7 on t7.MOrgID = t1.MOrgID and  t7.MParentID=t1.MContactID and t7.MLocaleID=@MLocaleID AND t7.MIsDelete=0 ");
				stringBuilder.AppendLine(" LEFT JOIN T_IV_ReceiveAttachment t5 ON t5.MOrgID = t1.MOrgID and  t1.MID=t5.MParentID AND t5.MIsDelete=0 ");
				stringBuilder.AppendLine("LEFT JOIN \r\n                                    (\r\n                                        select t4.MTargetBillID, t1.MDate AS RecDate, t1.MTime AS RecTime, t1.MSeq AS RecSeq, t1.MCreateDate AS RecCreateDate from t_iv_bankbillentry t1\r\n                                            INNER JOIN t_iv_bankbill t2 ON t1.MID = t2.MID AND t2.MIsDelete = 0 AND t2.MOrgID=@MOrgID\r\n                                            INNER JOIN t_iv_bankbillreconcile t3 ON t1.MEntryID = t3.MBankBillEntryID AND t3.MIsDelete=0 AND t3.MOrgID=@MOrgID\r\n                                            INNER JOIN t_iv_bankbillreconcileentry t4 ON t3.MID = t4.MID AND t4.MIsDelete=0 AND t4.MOrgID=@MOrgID\r\n                                            WHERE t1.MIsDelete = 0 AND t1.MOrgID=@MOrgID AND t4.MTargetBillType='Receive'\r\n                                    ) t6 ON t1.MID=t6.MTargetBillID");
				stringBuilder.AppendFormat(" WHERE t1.MIsDelete=0 And t1.MBankID in ({0}) AND t1.MOrgID=@MOrgID", bankIdInSql);
				stringBuilder.AppendLine(" And t1.MBizDate >= @StartDate And t1.MBizDate<=@EndDate ");
				stringBuilder.AppendLine(GetTransactionsListFilter(ctx, param, GLDocTypeEnum.Receive));
				stringBuilder.AppendLine(" GROUP BY t1.MID");
			}
			if ((param.TransactionType == "Payment" | flag) && param.SrcFrom != 1)
			{
				if (!string.IsNullOrWhiteSpace(stringBuilder.ToString()))
				{
					stringBuilder.AppendLine(" UNION ALL");
				}
				stringBuilder.AppendLine(" SELECT  'Transfer' as MTransType,t1.MModifyDate, t1.MID,t1.MFromAcctID as MBankID,t1.MBizDate,t1.MReference,");
				stringBuilder.AppendLine(" t3L.MName AS MDescription,'' AS MContactName, ");
				stringBuilder.AppendLine(" t1.MFromTotalAmtFor AS MSpent, '' AS MReceived ,'Transfer' as MType, '' AS MAttachIDs, MFromReconcileStatu AS MReconcileStatu,t1.MFromTotalAmtFor as MVerificationAmt ,t1.MFromTotalAmt as MVerifyAmt ,t1.MFromTotalAmtFor as MVerifyAmtFor,t1.MCreateDate,");
				stringBuilder.AppendLine(" '1900-01-01' AS RecDate,'00:00:00' AS RecTime,-1 AS RecSeq,'1900-01-01' AS RecCreateDate");
				stringBuilder.AppendLine(" FROM T_IV_Transfer t1");
				stringBuilder.AppendLine(" LEFT JOIN T_BD_BankAccount t3 on t1.MOrgID = t3.MOrgID and  t1.MToAcctID=t3.MItemID AND t3.MIsDelete=0 ");
				stringBuilder.AppendLine(" LEFT JOIN T_BD_BankAccount_L t3L on t1.MOrgID = t3L.MOrgID and  t3.MItemID=t3L.MParentID And t3L.MLocaleID=@MLocaleID AND t3L.MIsDelete=0 ");
				stringBuilder.AppendLine(" WHERE t1.MIsDelete=0 And t1.MOrgID=@MOrgID");
				stringBuilder.AppendLine(" And t1.MBizDate >= @StartDate And t1.MBizDate<=@EndDate ");
				stringBuilder.AppendFormat(" And t1.MFromAcctID in ({0}) ", bankIdInSql);
				stringBuilder.AppendLine(GetTransactionsListFilter(ctx, param, GLDocTypeEnum.Transfer));
			}
			if ((param.TransactionType == "Receive" | flag) && param.SrcFrom != 2)
			{
				if (!string.IsNullOrWhiteSpace(stringBuilder.ToString()))
				{
					stringBuilder.AppendLine(" UNION ALL");
				}
				stringBuilder.AppendLine(" SELECT  'Transfer' as MTransType,t1.MModifyDate, t1.MID,t1.MToAcctID as MBankID,t1.MBizDate,t1.MReference,");
				stringBuilder.AppendLine(" t3L.MName AS MDescription,'' AS MContactName, ");
				stringBuilder.AppendLine(" '' MSpent,t1.MToTotalAmtFor as MReceived,'Transfer' as MType, '' AS MAttachIDs,MToReconcileStatu AS MReconcileStatu ,t1.MToTotalAmtFor as MVerificationAmt ,t1.MToTotalAmt as MVerifyAmt ,t1.MToTotalAmtFor as MVerifyAmtFor,t1.MCreateDate, ");
				stringBuilder.AppendLine(" '1900-01-01' AS RecDate,'00:00:00'AS RecTime,-1 AS RecSeq,'1900-01-01' AS RecCreateDate");
				stringBuilder.AppendLine(" FROM T_IV_Transfer t1");
				stringBuilder.AppendLine(" LEFT JOIN T_BD_BankAccount t3 on t3.MOrgID = t1.MOrgID and t1.MFromAcctID=t3.MItemID AND t3.MIsDelete=0 ");
				stringBuilder.AppendLine(" LEFT JOIN T_BD_BankAccount_L t3L on t1.MOrgID = t3L.MOrgID and  t3.MItemID=t3L.MParentID And t3L.MLocaleID=@MLocaleID AND t3L.MIsDelete=0 ");
				stringBuilder.AppendLine(" WHERE t1.MIsDelete=0 And t1.MOrgID=@MOrgID");
				stringBuilder.AppendLine(" And t1.MBizDate >= @StartDate And t1.MBizDate<=@EndDate ");
				stringBuilder.AppendFormat(" And t1.MToAcctID in ({0}) ", bankIdInSql);
				stringBuilder.AppendLine(GetTransactionsListFilter(ctx, param, GLDocTypeEnum.Transfer));
			}
			DateTime dateTime;
			int num;
			if (param.EndDate.HasValue)
			{
				dateTime = param.EndDate.Value;
				num = ((dateTime.Year <= 1900) ? 1 : 0);
			}
			else
			{
				num = 1;
			}
			if (num != 0)
			{
				dateTime = DateTime.Now;
				param.EndDate = new DateTime(dateTime.Year + 100, 11, 31);
			}
			param.EndDate = param.EndDate.Value.ToDayLastSecond();
			SqlQuery sqlQuery = new SqlQuery();
			sqlQuery.SqlWhere = param;
			sqlQuery.SelectString = stringBuilder.ToString();
			sqlQuery.AddParameter(new MySqlParameter("@StartDate", MySqlDbType.DateTime)
			{
				Value = (object)param.StartDate
			});
			sqlQuery.AddParameter(new MySqlParameter("@EndDate", MySqlDbType.DateTime)
			{
				Value = (object)param.EndDate
			});
			sqlQuery.AddParameter(new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 36)
			{
				Value = ctx.MLCID
			});
			sqlQuery.AddParameter(new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
			{
				Value = ctx.MOrgID
			});
			foreach (MySqlParameter item in list)
			{
				sqlQuery.AddParameter(new MySqlParameter(item.ParameterName, MySqlDbType.VarChar, 36)
				{
					Value = item.Value
				});
			}
			if (param.ExactDate.HasValue)
			{
				SqlQuery sqlQuery2 = sqlQuery;
				MySqlParameter mySqlParameter = new MySqlParameter("@ExactDate", MySqlDbType.DateTime);
				dateTime = param.ExactDate.Value;
				mySqlParameter.Value = dateTime.Date;
				sqlQuery2.AddParameter(mySqlParameter);
			}
			if (!string.IsNullOrEmpty(param.TransAcctName))
			{
				sqlQuery.AddParameter(new MySqlParameter("@TransAcctName", MySqlDbType.VarChar, 200)
				{
					Value = param.TransAcctName
				});
			}
			if (!string.IsNullOrEmpty(param.MDesc))
			{
				sqlQuery.AddParameter(new MySqlParameter("@MDesc", MySqlDbType.VarChar, 500)
				{
					Value = param.MDesc
				});
			}
			if (param.IsExactAmount)
			{
				if (param.AmountFrom.HasValue)
				{
					sqlQuery.AddParameter(new MySqlParameter("@Amount", MySqlDbType.Decimal)
					{
						Value = (object)param.AmountFrom.Value
					});
				}
			}
			else
			{
				if (param.AmountFrom.HasValue)
				{
					sqlQuery.AddParameter(new MySqlParameter("@AmountFrom", MySqlDbType.Decimal)
					{
						Value = (object)param.AmountFrom.Value
					});
				}
				if (param.AmountTo.HasValue)
				{
					sqlQuery.AddParameter(new MySqlParameter("@AmountTo", MySqlDbType.Decimal)
					{
						Value = (object)param.AmountTo.Value
					});
				}
			}
			SqlOrderDir sqlOrderDir = string.IsNullOrWhiteSpace(param.Order) ? SqlOrderDir.Desc : ((param.Order.ToLower() == "desc") ? SqlOrderDir.Desc : SqlOrderDir.Asc);
			string text = "";
			text = ((!string.IsNullOrWhiteSpace(param.Sort) && !(param.Sort.ToLower() == "mbizdate")) ? $"{param.Sort} {sqlOrderDir}" : string.Format("MBizDate {0}, RecDate {0},RecCreateDate {0}, RecSeq {0},MCreateDate {0}", sqlOrderDir));
			sqlQuery.SqlWhere.OrderBy(text);
			return ModelInfoManager.GetPageDataModelListBySql<IVAccountTransactionsModel>(ctx, sqlQuery);
		}

		private static string GetTransactionsListFilter(MContext ctx, IVAccountTransactionsListFilterModel filter, GLDocTypeEnum type)
		{
			string text = "";
			if (filter.ExactDate.HasValue)
			{
				text += " and date(t1.MBizDate)=@ExactDate";
			}
			if (!string.IsNullOrEmpty(filter.TransAcctName))
			{
				if ((uint)(type - 3) > 1u)
				{
					if (type == GLDocTypeEnum.Transfer)
					{
						text += " and CONCAT('Bank Transfer to ',t3l.MName) like CONCAT('%',@TransAcctName, '%') ";
					}
				}
				else
				{
					text += string.Format(" and (F_GetUserName(t7.MFirstName,t7.MLastName) like CONCAT('%',@TransAcctName, '%')\r\n                                    or convert(AES_DECRYPT(t4.MName,'{0}') using utf8) like CONCAT('%',@TransAcctName,'%')) ", "JieNor-001");
				}
			}
			if (!string.IsNullOrEmpty(filter.MDesc))
			{
				text += " and t1.MReference like concat('%',@MDesc, '%') ";
			}
			if (filter.IsExactAmount)
			{
				if (filter.AmountFrom.HasValue)
				{
					if ((uint)(type - 3) > 1u)
					{
						if (type == GLDocTypeEnum.Transfer)
						{
							text += " and (t1.MToTotalAmtFor=@Amount or t1.MFromTotalAmtFor=@Amount) ";
						}
					}
					else
					{
						text += " and t1.MTaxTotalAmtFor=@Amount ";
					}
				}
			}
			else
			{
				if (filter.AmountFrom.HasValue)
				{
					if ((uint)(type - 3) > 1u)
					{
						if (type == GLDocTypeEnum.Transfer)
						{
							text += " and ((t1.MToTotalAmtFor>=@AmountFrom and t1.MFromTotalAmtFor=0) or (t1.MFromTotalAmtFor>=@AmountFrom and t1.MToTotalAmtFor=0)) ";
						}
					}
					else
					{
						text += " and t1.MTaxTotalAmtFor>=@AmountFrom ";
					}
				}
				if (filter.AmountTo.HasValue)
				{
					if ((uint)(type - 3) > 1u)
					{
						if (type == GLDocTypeEnum.Transfer)
						{
							text += " and ((t1.MToTotalAmtFor<=@AmountTo and t1.MFromTotalAmtFor=0) or (t1.MFromTotalAmtFor<=@AmountTo and t1.MToTotalAmtFor=0)) ";
						}
					}
					else
					{
						text += " and t1.MTaxTotalAmtFor<=@AmountTo ";
					}
				}
			}
			return text;
		}

		private static string GetBankIdInSql(IVAccountTransactionsListFilterModel param, List<MySqlParameter> paramList)
		{
			List<string> list = new List<string>();
			string[] array = param.MBankID.Split(',');
			string empty = string.Empty;
			int num = 1;
			string[] array2 = array;
			foreach (string value in array2)
			{
				empty = "@MBankID" + num;
				list.Add(empty);
				paramList.Add(new MySqlParameter(empty, value));
				num++;
			}
			return string.Join(",", list);
		}

		public static OperationResult UpdateReconcileStatu(MContext ctx, IVReconcileStatus statu, List<IVPaymentModel> paymentList, List<IVReceiveModel> receiveList, List<IVTransferModel> transferList)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			foreach (IVPaymentModel payment in paymentList)
			{
				list.Add(IVPaymentRepository.GetUpdateReconcileStatuSql(ctx, payment.MID, statu));
			}
			foreach (IVReceiveModel receive in receiveList)
			{
				list.Add(IVReceiveRepository.GetUpdateReconcileStatuSql(ctx, receive.MID, statu));
			}
			foreach (IVTransferModel transfer in transferList)
			{
				list.Add(IVTransferRepository.GetUpdateReconcileStatuSql(ctx, transfer.MID, statu));
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			dynamicDbHelperMySQL.ExecuteSqlTran(list);
			return new OperationResult
			{
				Success = true
			};
		}

		public static OperationResult DeteleTransactions(MContext ctx, List<IVAccountTransactionsModel> list)
		{
			OperationResult operationResult = new OperationResult
			{
				Success = true
			};
			List<CommandInfo> list2 = new List<CommandInfo>();
			int num = 0;
			operationResult = GLInterfaceRepository.IsDocCanOperate(ctx, (from x in list
			select x.MID).ToList());
			if (!operationResult.Success)
			{
				return operationResult;
			}
			IEnumerable<IGrouping<string, IVAccountTransactionsModel>> enumerable = from x in list
			group x by x.MTransType;
			foreach (IGrouping<string, IVAccountTransactionsModel> item in enumerable)
			{
				string key = item.Key;
				List<IVAccountTransactionsModel> source = item.ToList();
				List<string> pkIDList = (from x in source
				select x.MID).ToList();
				List<string> list3 = IVVerificationRepository.CheckIsCanEditOrDelete(ctx, key, pkIDList);
				if (list3.Count != 0)
				{
					List<CommandInfo> collection = new List<CommandInfo>();
					switch (key)
					{
					case "Transfer":
						collection = ModelInfoManager.GetDeleteFlagCmd<IVTransferModel>(ctx, list3);
						break;
					case "Payment":
						collection = ModelInfoManager.GetDeleteFlagCmd<IVPaymentModel>(ctx, list3);
						break;
					case "Receive":
						collection = ModelInfoManager.GetDeleteFlagCmd<IVReceiveModel>(ctx, list3);
						break;
					}
					List<CommandInfo> deleteVoucherByDocIDsCmds = GLInterfaceRepository.GetDeleteVoucherByDocIDsCmds(ctx, list3);
					list2.AddRange(collection);
					list2.AddRange(deleteVoucherByDocIDsCmds);
					num += list3.Count;
				}
			}
			if (list2.Count == 0)
			{
				return operationResult;
			}
			operationResult.Success = (new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(list2) > 0);
			operationResult.Message = num.ToString();
			return operationResult;
		}

		public static List<IVBankStatementsModel> GetBankStatementsList(MContext ctx, IVTransactionQueryParamModel filter)
		{
			DateTime dateTime = filter.EndDate;
			if (dateTime.Year == 1900)
			{
				dateTime = DateTime.Now;
				filter.EndDate = new DateTime(dateTime.Year + 100, 11, 31);
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(" SELECT distinct t1.MID,t1.MImportDate,t1.MStartDate,t1.MEndDate,t1.MStartBalance,t1.MEndBalance,t1.MCheckState,F_GetUserName(t2.MFristName,t2.MLastName) as MUser,t1.MFileName,t1.MBankTypeID,t3.MID AS MHasRecID ");
			stringBuilder.AppendLine(" FROM T_IV_BankBill t1");
			stringBuilder.AppendLine(" LEFT JOIN T_Sec_User_L t2 on t1.MCreatorID=t2.MParentID and t2.MLocaleID=@MLocaleID  AND t2.MIsDelete = 0 ");
			stringBuilder.AppendLine("LEFT JOIN (select DISTINCT c.MID from t_iv_bankbillreconcile a\r\n                                INNER JOIN t_iv_bankbillentry b ON b.MOrgID = a.MOrgID and a.MBankBillEntryID=b.MEntryID AND b.MIsDelete = 0 \r\n                                        AND IFNULL(b.MParentID,'')='' AND b.MCheckState<>2\r\n                                INNER JOIN t_iv_bankBill c ON b.MID=c.MID and c.MOrgID = a.MOrgID AND c.MIsDelete = 0\r\n                                WHERE  a.MIsDelete=0 AND c.MOrgID=@MOrgID AND c.MBankID=@MBankID) t3 ON t1.MID=t3.MID  ");
			stringBuilder.AppendLine(" WHERE t1.MIsDelete=0 AND MOrgID=@MOrgID AND MBankID=@MBankID and MStartDate >= @StartDate and MEndDate <= @EndDate");
			stringBuilder.AppendLine((!string.IsNullOrEmpty(filter.MID)) ? "AND t1.MID=@MID" : "");
			stringBuilder.AppendLine(" order by t1.MImportDate asc ");
			MySqlParameter[] array = new MySqlParameter[6]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MBankID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@StartDate", MySqlDbType.DateTime),
				new MySqlParameter("@EndDate", MySqlDbType.DateTime),
				new MySqlParameter("@MID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = ctx.MLCID;
			array[2].Value = filter.MBankID;
			array[3].Value = filter.StartDate;
			array[4].Value = filter.EndDate.ToDayLastSecond();
			array[5].Value = filter.MID;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return DataTableToStatementList(dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array).Tables[0], ctx, filter.MBankID);
		}

		private static List<NameValueModel> GetRecDetailList(MContext ctx, string bankId)
		{
			string sql = "SELECT distinct b.MID AS MName, (CASE WHEN c.MID IS NULL THEN '0' ELSE '1' END) AS MValue FROM t_iv_bankbillentry a \r\n                            INNER JOIN t_iv_bankBill b on a.MOrgID = b.MOrgID and a.MID=b.MID AND b.MIsDelete=0 AND IFNULL(a.MParentID,'')=''\r\n                            LEFT JOIN t_iv_bankbillreconcile c ON c.MOrgID = a.MOrgID and c.MBankBillEntryID=a.MEntryID AND c.MIsDelete=0 \r\n                            WHERE b.MOrgID=@MOrgID AND b.MBankID=@BankID AND a.MIsDelete=0 AND a.MCheckState<>2";
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@BankID", bankId)
			};
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return ModelInfoManager.DataTableToList<NameValueModel>(dynamicDbHelperMySQL.Query(sql, cmdParms));
		}

		private static List<IVBankStatementsModel> DataTableToStatementList(DataTable dt, MContext ctx, string bankId)
		{
			List<NameValueModel> list = GetRecDetailList(ctx, bankId);
			if (list == null)
			{
				list = new List<NameValueModel>();
			}
			List<IVBankStatementsModel> list2 = new List<IVBankStatementsModel>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				IVBankStatementsModel model;
				for (int i = 0; i < count; i++)
				{
					DataRow dataRow = dt.Rows[i];
					model = new IVBankStatementsModel();
					bool flag = Convert.ToBoolean(dataRow["MCheckState"]);
					model.MID = dataRow["MID"].ToString();
					List<string> list3 = (from t in list
					where t.MName == model.MID
					select t.MValue).ToList();
					if (list3.Count > 0)
					{
						if (list3.Contains("0") && list3.Contains("1"))
						{
							model.MStatus = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "PartlyReconciled", "Partly reconciled");
						}
						else if (list3.Contains("0") && !list3.Contains("1"))
						{
							model.MStatus = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "UnReconciled", "UnReconciled");
						}
						else
						{
							model.MStatus = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "Reconciled", "Reconciled");
						}
					}
					else
					{
						model.MStatus = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "UnReconciled", "UnReconciled");
					}
					if (dataRow["MImportDate"].ToString() != "" && dataRow["MImportDate"].ToString() != "0000/0/0 0:00:00")
					{
						model.MImportDate = Convert.ToDateTime(dataRow["MImportDate"]);
					}
					if (dataRow["MStartDate"].ToString() != "" && dataRow["MStartDate"].ToString() != "0000/0/0 0:00:00")
					{
						model.MStartDate = Convert.ToDateTime(dataRow["MStartDate"]);
					}
					if (dataRow["MEndDate"].ToString() != "" && dataRow["MEndDate"].ToString() != "0000/0/0 0:00:00")
					{
						model.MEndDate = Convert.ToDateTime(dataRow["MEndDate"]);
					}
					IVBankStatementsModel iVBankStatementsModel = model;
					decimal num;
					object mStartBalance;
					if (dataRow["MStartBalance"] != null && dataRow["MStartBalance"].ToString().Length != 0 && !(Convert.ToDecimal(dataRow["MStartBalance"]) == decimal.Zero))
					{
						num = Convert.ToDecimal(dataRow["MStartBalance"]);
						mStartBalance = num.ToString("0.00");
					}
					else
					{
						mStartBalance = "0.00";
					}
					iVBankStatementsModel.MStartBalance = (string)mStartBalance;
					IVBankStatementsModel iVBankStatementsModel2 = model;
					object mEndBalance;
					if (dataRow["MEndBalance"] != null && dataRow["MEndBalance"].ToString().Length != 0 && !(Convert.ToDecimal(dataRow["MEndBalance"]) == decimal.Zero))
					{
						num = Convert.ToDecimal(dataRow["MEndBalance"]);
						mEndBalance = num.ToString("0.00");
					}
					else
					{
						mEndBalance = "0.00";
					}
					iVBankStatementsModel2.MEndBalance = (string)mEndBalance;
					model.MUser = dataRow["MUser"].ToString();
					model.MFileName = dataRow["MFileName"].ToString();
					model.MBankTypeID = dataRow["MBankTypeID"].ToString();
					list2.Add(model);
				}
			}
			return list2;
		}

		public static IVBankStatementsModel GetBankStatementModel(MContext ctx, string MID)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(" SELECT MID,MImportDate,MStartDate,MEndDate,MStartBalance,MEndBalance,MCheckState,'' as MUser,'' as MFileName ");
			stringBuilder.AppendLine(" FROM T_IV_BankBill ");
			stringBuilder.AppendLine(" WHERE MID=@MID and MOrgID = @MOrgID AND MIsDelete=0 ");
			MySqlParameter[] array = new MySqlParameter[2]
			{
				new MySqlParameter("@MID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			array[0].Value = MID;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			IVBankBillModel dataEditModel = ModelInfoManager.GetDataEditModel<IVBankBillModel>(ctx, MID, false, true);
			string bankId = (dataEditModel == null) ? "" : dataEditModel.MBankID;
			return DataTableToStatementList(dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array).Tables[0], ctx, bankId)[0];
		}

		public static List<IVBankStatementViewModel> GetBankStatementView(MContext ctx, string statementID)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(" SELECT MID,MEntryID,MDate,MTransType,MTransAcctName,MTransAcctNo, ");
			stringBuilder.AppendLine(" MSpentAmt,MReceivedAmt,MBalance,case \r\n                when MCheckState = 0\r\n                then (case when (select count(*) from  t_iv_bankbillreconcile where MBankBillEntryId = MEntryID AND MIsDelete = 0) > 0 then 1 else 0 end)\r\n                else MCheckState end \r\n                as MCheckState,MDesc ");
			stringBuilder.AppendLine(" FROM T_IV_BankBillEntry ");
			stringBuilder.AppendLine(" WHERE MID=@MID and MOrgID=@MOrgID AND MIsDelete = 0 AND ifnull(MParentID,'')='' ");
			stringBuilder.AppendLine(" ORDER BY MDate asc, MSeq asc  ");
			MySqlParameter[] array = new MySqlParameter[2]
			{
				new MySqlParameter("@MID", statementID),
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			array[0].Value = statementID;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.SqlString = stringBuilder.ToString();
			return DataTableToStatementView(dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array).Tables[0]);
		}

		private static string GetBankStatementDetailSql()
		{
			return string.Format(" SELECT t1.MID,t1.MEntryID,T1.MParentID,t1.MDate,t1.MTransType,CONVERT(t1.MTransAcctName USING gbk) AS MTransAcctName,t1.MTransAcctNo, t1.MSpentSplitAmt AS MSpentAmt,t1.MReceivedSplitAmt AS MReceivedAmt,t1.MBalance, case \r\n                when t1.MCheckState = 0\r\n                then (case when (select count(*) from  t_iv_bankbillreconcile where MOrgId=@MOrgID AND MBankBillEntryId = t1.MEntryID AND MIsDelete = 0) > 0 then 1 else 0 end)\r\n                else t1.MCheckState end \r\n                as MCheckState,t1.MSeq,CONVERT(t1.MDesc USING gbk) AS MDesc,CONVERT(t1.MUserRef USING gbk) AS MUserRef,MVoucherStatus,t3.MVoucherNumber,t3.MVoucherID\r\n                FROM T_IV_BankBillEntry t1\r\n                INNER JOIN T_IV_BankBill t2 ON t1.MID=t2.MID AND t1.MOrgID=t2.MOrgID AND t2.MBankID=@MBankID AND t2.MIsDelete=0\r\n                LEFT JOIN  (\r\n\t\t\t\t\tSELECT t1.MBankBillEntryID,t2.MNumber AS MVoucherNumber,t2.MItemID AS MVoucherID  FROM  T_IV_BankBillEntryVoucher t1\r\n\t\t\t\t\tINNER JOIN T_GL_Voucher t2 on t1.MVoucherID=t2.MItemID AND t1.MOrgID=t2.MOrgID AND t2.MIsDelete=0\r\n\t\t\t\t\tWHERE t1.MOrgId=@MOrgID AND t1.MIsDelete=0\r\n                ) t3 ON t1.MEntryID=t3.MBankBillEntryID\r\n                WHERE t1.MOrgID=@MOrgID AND t1.MCheckState<>2 AND t1.MIsDelete = 0 ");
		}

		private static string GetBankStatementDetailListFilter(MContext ctx, IVBankStatementDetailFilter filter, ref SqlQuery query)
		{
			string text = "";
			if (filter.ExactDate.HasValue)
			{
				text += " and date(t1.MDate)=@ExactDate";
				query.AddParameter(new MySqlParameter("@ExactDate", MySqlDbType.DateTime)
				{
					Value = (object)filter.ExactDate.Value.Date
				});
			}
			if (!string.IsNullOrEmpty(filter.TransAcctName))
			{
				text += " and t1.MTransAcctName like CONCAT('%',@TransAcctName, '%') ";
				query.AddParameter(new MySqlParameter("@TransAcctName", MySqlDbType.VarChar, 200)
				{
					Value = filter.TransAcctName
				});
			}
			if (!string.IsNullOrEmpty(filter.MDesc))
			{
				text += " and t1.MUserRef like concat('%',@MDesc, '%') ";
				query.AddParameter(new MySqlParameter("@MDesc", MySqlDbType.VarChar, 500)
				{
					Value = filter.MDesc
				});
			}
			if (filter.SrcFrom > 0)
			{
				if (filter.SrcFrom == 1)
				{
					text += " and t1.MReceivedSplitAmt>0 ";
				}
				else if (filter.SrcFrom == 2)
				{
					text += " and t1.MSpentSplitAmt>0 ";
				}
			}
			if (filter.IsExactAmount)
			{
				if (filter.AmountFrom.HasValue)
				{
					text += " and (t1.MSpentSplitAmt=@Amount or t1.MReceivedSplitAmt=@Amount) ";
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
					text += " and ((t1.MSpentSplitAmt>=@AmountFrom and t1.MReceivedSplitAmt=0) or (t1.MReceivedSplitAmt>=@AmountFrom and t1.MSpentSplitAmt=0)) ";
					query.AddParameter(new MySqlParameter("@AmountFrom", MySqlDbType.Decimal)
					{
						Value = (object)filter.AmountFrom.Value
					});
				}
				if (filter.AmountTo.HasValue)
				{
					text += " and ((t1.MSpentSplitAmt<=@AmountTo and t1.MReceivedSplitAmt=0) or (t1.MReceivedSplitAmt<=@AmountTo and t1.MSpentSplitAmt=0)) ";
					query.AddParameter(new MySqlParameter("@AmountTo", MySqlDbType.Decimal)
					{
						Value = (object)filter.AmountTo.Value
					});
				}
			}
			return text;
		}

		public static List<IVBankStatementDetailModel> GetBankStatementDetailList(MContext ctx, string bankId, List<string> parentIdList)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("{0} AND t1.MParentID IN (", GetBankStatementDetailSql());
			List<MySqlParameter> list = new List<MySqlParameter>();
			list.Add(new MySqlParameter("@MOrgID", ctx.MOrgID));
			list.Add(new MySqlParameter("@MBankID", bankId));
			int num = 1;
			foreach (string parentId in parentIdList)
			{
				string text = $"@Param{num}";
				stringBuilder.AppendFormat("{0},", text);
				list.Add(new MySqlParameter(text, parentId));
				num++;
			}
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
			stringBuilder.Append(")");
			return ModelInfoManager.GetDataModelBySql<IVBankStatementDetailModel>(ctx, stringBuilder.ToString(), list.ToArray());
		}

		public static DataGridJson<IVBankStatementDetailModel> GetBankStatementDetailList(MContext ctx, IVBankStatementDetailFilter filter)
		{
			SqlQuery sqlQuery = new SqlQuery();
			sqlQuery.SqlWhere = filter;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("{0} {1} AND IFNULL(t1.MParentID,'')='' ", GetBankStatementDetailSql(), GetBankStatementDetailListFilter(ctx, filter, ref sqlQuery));
			stringBuilder.AppendFormat(" AND t1.MDate between @StartDate AND @EndDate ");
			sqlQuery.SelectString = stringBuilder.ToString();
			sqlQuery.AddParameter(new MySqlParameter("@MOrgID", ctx.MOrgID));
			sqlQuery.AddParameter(new MySqlParameter("@MBankID", filter.MBankID));
			sqlQuery.AddParameter(new MySqlParameter("@StartDate", filter.StartDate));
			sqlQuery.AddParameter(new MySqlParameter("@EndDate", filter.EndDate));
			SqlOrderDir sqlOrderDir = string.IsNullOrWhiteSpace(filter.Order) ? SqlOrderDir.Desc : ((filter.Order.ToLower() == "desc") ? SqlOrderDir.Desc : SqlOrderDir.Asc);
			string text = "";
			text = ((!string.IsNullOrWhiteSpace(filter.Sort) && !(filter.Sort.ToLower() == "mdate")) ? ((!(filter.Sort.ToLower() == "mtransacctname")) ? $"{filter.Sort} {sqlOrderDir}" : string.Format("CONVERT(t1.MTransAcctName USING gbk ) {0},t1.MDate {0},t1.MCreateDate {0},t1.MSeq {0}", sqlOrderDir)) : string.Format("t1.MDate {0},t1.MCreateDate {0}, t1.MSeq {0}", sqlOrderDir));
			sqlQuery.SqlWhere.OrderBy(text);
			return ModelInfoManager.GetPageDataModelListBySql<IVBankStatementDetailModel>(ctx, sqlQuery);
		}

		private static List<IVBankStatementViewModel> DataTableToStatementView(DataTable dt)
		{
			List<IVBankStatementViewModel> list = new List<IVBankStatementViewModel>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					DataRow dataRow = dt.Rows[i];
					IVBankStatementViewModel iVBankStatementViewModel = new IVBankStatementViewModel();
					iVBankStatementViewModel.MID = dataRow["MID"].ToString();
					iVBankStatementViewModel.MEntryID = dataRow["MEntryID"].ToString();
					iVBankStatementViewModel.MTransType = dataRow["MTransType"].ToString();
					iVBankStatementViewModel.MTransAcctName = dataRow["MTransAcctName"].ToString();
					iVBankStatementViewModel.MTransAcctNo = dataRow["MTransAcctNo"].ToString();
					iVBankStatementViewModel.MDesc = dataRow["MDesc"].ToString();
					iVBankStatementViewModel.MDate = dataRow["MDate"].ToMDateTime();
					iVBankStatementViewModel.MCheckState = dataRow["MCheckState"].ToString();
					IVBankStatementViewModel iVBankStatementViewModel2 = iVBankStatementViewModel;
					decimal num;
					object mSpentAmt;
					if (dataRow["MSpentAmt"] != null && dataRow["MSpentAmt"].ToString().Length != 0 && !(Convert.ToDecimal(dataRow["MSpentAmt"]) == decimal.Zero))
					{
						num = Convert.ToDecimal(dataRow["MSpentAmt"]);
						mSpentAmt = num.ToString("0.00");
					}
					else
					{
						mSpentAmt = "";
					}
					iVBankStatementViewModel2.MSpentAmt = (string)mSpentAmt;
					IVBankStatementViewModel iVBankStatementViewModel3 = iVBankStatementViewModel;
					object mReceivedAmt;
					if (dataRow["MReceivedAmt"] != null && dataRow["MReceivedAmt"].ToString().Length != 0 && !(Convert.ToDecimal(dataRow["MReceivedAmt"]) == decimal.Zero))
					{
						num = Convert.ToDecimal(dataRow["MReceivedAmt"]);
						mReceivedAmt = num.ToString("0.00");
					}
					else
					{
						mReceivedAmt = "";
					}
					iVBankStatementViewModel3.MReceivedAmt = (string)mReceivedAmt;
					IVBankStatementViewModel iVBankStatementViewModel4 = iVBankStatementViewModel;
					object mBalance;
					if (dataRow["MBalance"] != null && dataRow["MBalance"].ToString().Length != 0 && !(Convert.ToDecimal(dataRow["MBalance"]) == decimal.Zero))
					{
						num = Convert.ToDecimal(dataRow["MBalance"]);
						mBalance = num.ToString("0.00");
					}
					else
					{
						mBalance = "";
					}
					iVBankStatementViewModel4.MBalance = (string)mBalance;
					list.Add(iVBankStatementViewModel);
				}
			}
			return list;
		}

		private static string RemoveReconcileID(MContext ctx, string selectIds)
		{
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			string sql = $"SELECT MBankBillEntryID FROM T_IV_BankBillReconcile WHERE MBankBillEntryID in ({selectIds}) AND MIsDelete=0 ";
			DataSet dataSet = dynamicDbHelperMySQL.Query(sql);
			if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return selectIds;
			}
			List<string> temp = new List<string>();
			foreach (DataRow row in dataSet.Tables[0].Rows)
			{
				temp.Add(string.Format("'{0}'", row["MBankBillEntryID"].ToString()));
			}
			List<string> source = selectIds.Split(',').ToList();
			List<string> values = (from t in source
			where !temp.Contains(t)
			select t).ToList();
			return string.Join(",", values);
		}

		public static int StatementStatusUpdate(MContext ctx, string selectIds, int directType)
		{
			selectIds = RemoveReconcileID(ctx, selectIds);
			if (string.IsNullOrEmpty(selectIds))
			{
				return 1;
			}
			List<CommandInfo> list = new List<CommandInfo>();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(" update T_IV_BankBillEntry ");
			if (directType == 1)
			{
				stringBuilder.AppendLine(" set MPrevState=0, MCheckState=2 ");
				stringBuilder.AppendFormat(" where MCheckState<>2 and MEntryID in ({0}) and MOrgID ='{1}' and MIsDelete = 0 ", selectIds, ctx.MOrgID);
				string commandText = " update  t_iv_bankbillreconcileentry set MIsDelete=1 where mid in\r\n                (select mid from t_iv_bankbillreconcile where mbankbillentryid in\r\n                (" + selectIds + ") and MOrgID = '" + ctx.MOrgID + "' and MIsDelete = 0 ) and MOrgID = '" + ctx.MOrgID + "' and MIsDelete = 0 ";
				list.Add(new CommandInfo
				{
					CommandText = commandText
				});
				string commandText2 = " update t_iv_bankbillreconcile set MIsDelete=1\r\n                where mbankbillentryid in\r\n                (" + selectIds + ") and MOrgID = '" + ctx.MOrgID + "' and MIsDelete = 0 ";
				list.Add(new CommandInfo
				{
					CommandText = commandText2
				});
			}
			else
			{
				stringBuilder.AppendLine(" set MCheckState=0 ");
				stringBuilder.AppendFormat(" where MCheckState=2 and MEntryID in ({0}) and MOrgID = '{1}' and MIsDelete = 0 ", selectIds, ctx.MOrgID);
			}
			list.Add(new CommandInfo
			{
				CommandText = stringBuilder.ToString()
			});
			string paymentIDsByBankBillEntry = IVBankBillEntryRepository.GetPaymentIDsByBankBillEntry(ctx, selectIds);
			string receiveIDsByBankBillEntry = IVBankBillEntryRepository.GetReceiveIDsByBankBillEntry(ctx, selectIds);
			string transferIDsByBankBillEntry = IVBankBillEntryRepository.GetTransferIDsByBankBillEntry(ctx, selectIds);
			string autoCreatePaymentIDs = IVBankBillEntryRepository.GetAutoCreatePaymentIDs(ctx, selectIds);
			string autoCreateReceiveIDs = IVBankBillEntryRepository.GetAutoCreateReceiveIDs(ctx, selectIds);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int result = dynamicDbHelperMySQL.ExecuteSqlTran(list);
			IVBankBillEntryRepository.UpdateReconcileAmt(ctx, paymentIDsByBankBillEntry, receiveIDsByBankBillEntry, transferIDsByBankBillEntry, autoCreatePaymentIDs, autoCreateReceiveIDs);
			return result;
		}

		public static OperationResult CheckIsCanDeleteStatementStatus(MContext ctx, List<string> idList)
		{
			string format = "SELECT\r\n\t                            1\r\n                            FROM\r\n\t                            t_iv_bankbillreconcileentry\r\n                            WHERE\r\n\t                        mid IN (\r\n\t\t                        SELECT\r\n\t\t\t                        mid\r\n\t\t                        FROM\r\n\t\t\t                        t_iv_bankbillreconcile\r\n\t\t                        WHERE\r\n\t\t\t                        mbankbillentryid IN {0}\r\n\t\t                            AND MOrgID = @MOrgID\r\n\t\t                            AND MIsDelete = 0\r\n\t                            )\r\n                                AND MOrgID = @MOrgID\r\n                                AND MIsDelete = 0\r\n                            UNION\r\n\t                        SELECT 1 FROM\r\n\t\t                        t_iv_bankbillreconcile\r\n\t                        WHERE mbankbillentryid IN {0}\r\n\t                            AND MOrgID = @MOrgID\r\n\t                            AND MIsDelete = 0 ";
			List<MySqlParameter> list = new List<MySqlParameter>();
			format = string.Format(format, BDRepository.GetInSql(ctx, idList, out list));
			list.Add(new MySqlParameter
			{
				ParameterName = "@MOrgID",
				Value = ctx.MOrgID
			});
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(format, list.ToArray());
			OperationResult operationResult = new OperationResult();
			operationResult.Success = (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count == 0);
			return operationResult;
		}

		public static OperationResult StatementUpdate(MContext ctx, List<IVBankStatementViewModel> models)
		{
			OperationResult operationResult = new OperationResult();
			List<CommandInfo> list = new List<CommandInfo>();
			foreach (IVBankStatementViewModel model in models)
			{
				list.Add(GetStatementViewUpdateSql(ctx, model));
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num = dynamicDbHelperMySQL.ExecuteSqlTran(list);
			operationResult.Success = true;
			operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "UpdateBankStatementSuccess", "The Bank Statement was updated");
			return operationResult;
		}

		public static BDBankAccountModel GetBankAccountModel(MContext ctx, string bankNo)
		{
			SqlWhere sqlWhere = new SqlWhere();
			SqlFilter filter = new SqlFilter("MBankNo", SqlOperators.Equal, bankNo);
			sqlWhere.AddFilter(filter);
			sqlWhere.Top = 1;
			List<BDBankAccountModel> dataModelList = ModelInfoManager.GetDataModelList<BDBankAccountModel>(ctx, sqlWhere, false, false);
			return dataModelList.Any() ? dataModelList[0] : new BDBankAccountModel();
		}

		public static BDBankTypeModel GetBankTypeModel(MContext ctx, string id)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("SELECT a.MItemID,b.MName FROM T_BD_BankType a ");
			stringBuilder.AppendLine(" left join T_BD_BankType_l b on a.MItemID=b.MParentID  AND b.MIsDelete=0");
			stringBuilder.AppendLine(" WHERE a.MItemID=@MItemID and b.MLocaleID=@MLocaleID and a.MOrgID = @MOrgID  AND a.MIsDelete=0 ");
			MySqlParameter[] array = new MySqlParameter[3]
			{
				new MySqlParameter("@MItemID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			array[0].Value = id;
			array[1].Value = ctx.MLCID;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array);
			if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return new BDBankTypeModel();
			}
			return ModelInfoManager.DataTableToList<BDBankTypeModel>(dataSet.Tables[0])[0];
		}

		public static List<BankBillImportSolutionModel> GetBankBillImportSolutionList(MContext ctx, string bankTypeId)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("SELECT * from (");
			stringBuilder.Append("SELECT a.MItemID,a.MParentID,b.MName,a.MLastUsedTime,a.MModifyDate ");
			stringBuilder.AppendLine(" FROM T_BD_BankBillImportSolution a");
			stringBuilder.AppendLine(" Left Join T_BD_BankBillImportSolution_l b ON a.MOrgID = b.MOrgID and  a.MItemID=b.MParentID AND b.MLocaleID=@MLocaleID AND b.MIsDelete = 0 ");
			stringBuilder.AppendLine(" WHERE a.MIsDelete = 0 and a.MParentID=@MParentID and a.MOrgID=@MOrgID");
			stringBuilder.AppendLine(" union all ");
			stringBuilder.Append("SELECT a.MItemID,a.MParentID,b.MName,a.MLastUsedTime,a.MModifyDate ");
			stringBuilder.AppendLine(" FROM T_BD_BankBillImportSolution a");
			stringBuilder.AppendLine(" Left Join T_BD_BankBillImportSolution_l b ON ifnull(a.MOrgID, '') = ifnull(b.MOrgID, '') and  a.MItemID=b.MParentID AND b.MLocaleID=@MLocaleID AND b.MIsDelete = 0");
			stringBuilder.AppendLine(" WHERE a.MIsDefault=1 and a.MIsDelete = 0 and a.MParentID=@MParentID and ifnull(a.MOrgID, '') = '' ");
			stringBuilder.AppendLine(") u");
			stringBuilder.AppendLine(" order by MLastUsedTime desc,MModifyDate desc ");
			MySqlParameter[] array = new MySqlParameter[3]
			{
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MParentID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ctx.MLCID;
			array[1].Value = bankTypeId;
			array[2].Value = ctx.MOrgID;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return ModelInfoManager.GetDataModelBySql<BankBillImportSolutionModel>(ctx, stringBuilder.ToString(), array);
		}

		public static BankBillImportSolutionModel GetBankBillImportSolutionModel(MContext ctx, string MItemID)
		{
			return ModelInfoManager.GetDataEditModel<BankBillImportSolutionModel>(ctx, MItemID, false, false);
		}

		public static OperationResult SaveBankBillImportSolution(MContext ctx, BankBillImportSolutionModel model)
		{
			OperationResult operationResult = CheckImportSolutionNameExist(ctx, model.MParentID, model.MName, model.MItemID);
			if (!operationResult.Success)
			{
				operationResult.Tag = "Exist";
				return operationResult;
			}
			List<MultiLanguageField> mMultiLanguageField = model.MultiLanguage.SingleOrDefault((MultiLanguageFieldList f) => f.MFieldName == "MName").MMultiLanguageField;
			if (!string.IsNullOrWhiteSpace(model.MItemID) && (!model.MIsDefault || model.IsUpdate))
			{
				model.MOrgID = ctx.MOrgID;
				model.MLastUsedTime = ctx.DateNow;
				model.IsUpdate = true;
				operationResult = ModelInfoManager.InsertOrUpdate<BankBillImportSolutionModel>(ctx, model, null);
			}
			else
			{
				foreach (MultiLanguageField item in mMultiLanguageField)
				{
					item.MPKID = string.Empty;
				}
				model.MOrgID = ctx.MOrgID;
				model.MLastUsedTime = ctx.DateNow;
				model.IsNew = true;
				operationResult = ModelInfoManager.InsertOrUpdate<BankBillImportSolutionModel>(ctx, model, null);
			}
			if (model.MultiLanguage != null)
			{
				operationResult.Tag = mMultiLanguageField.SingleOrDefault((MultiLanguageField f) => f.MLocaleID == ctx.MLCID).MValue;
			}
			return operationResult;
		}

		public static OperationResult SaveImportBankBill(MContext ctx, IVBankBillModel model, DataTable importData)
		{
			if (importData == null)
			{
				Stream fileStream = new FileStream(model.MFileName, FileMode.Open, FileAccess.Read);
				importData = ExcelDataHelper.ReadExcelData(ctx, fileStream, model.MFileName);
				model.MFileName = Path.GetFileName(model.MFileName);
			}
			OperationResult operationResult = new OperationResult();
			model.IsNew = true;
			model.MOrgID = ctx.MOrgID;
			model.MImportDate = ctx.DateNow;
			BankBillImportSolutionModel solution = model.ImportSolutionModel;
			int mHeaderRowStart = solution.MHeaderRowStart;
			int mDataRowStart = solution.MDataRowStart;
			List<PropertyInfo> source = new List<PropertyInfo>(typeof(BankBillImportSolutionModel).GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public));
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			DataRow dataRow = importData.Rows[mHeaderRowStart - 1];
			DataRow drHeader2 = (mDataRowStart - mHeaderRowStart > 1) ? importData.Rows[mHeaderRowStart] : null;
			int num = -1;
			bool flag = false;
			var transAmtObj = new
			{
				SpendAmt = "MSpentAmt",
				ReceivedAmt = "MReceivedAmt"
			};
			Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
			string[] source2 = new string[4]
			{
				"MTransAcctNo",
				"MTransAcctName",
				"MTransAcctNo2",
				"MTransAcctName2"
			};
			List<string> list = new List<string>();
			JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
			int num2 = -1;
			int num3 = -1;
			try
			{
				int num5;
				for (int j = 0; j < importData.Columns.Count; j++)
				{
					string headerColumnVal = Convert.ToString(dataRow[j]);
					IEnumerable<PropertyInfo> enumerable = from f in source
					where !string.IsNullOrWhiteSpace(headerColumnVal) && GetPropertyValue(solution, f.Name).EqualsIgnoreCase(headerColumnVal)
					select f;
					if (enumerable.Any())
					{
						int num4 = enumerable.Count();
						if (num4 == 1)
						{
							PropertyInfo propertyInfo = enumerable.ElementAt(0);
							if (propertyInfo.Name == "MTransType")
							{
								num = j;
							}
							else if (source2.Contains(propertyInfo.Name))
							{
								dictionary2.Add(propertyInfo.Name, j);
							}
							if (dictionary.ContainsKey(headerColumnVal))
							{
								list.Add(headerColumnVal);
							}
							else
							{
								dictionary.Add(headerColumnVal, propertyInfo.Name);
							}
						}
						else
						{
							foreach (PropertyInfo item in enumerable)
							{
								if (!dictionary.ContainsKey(headerColumnVal))
								{
									dictionary.Add(headerColumnVal, item.Name);
								}
								else
								{
									Dictionary<string, string> dictionary3 = dictionary;
									string key = headerColumnVal;
									dictionary3[key] = dictionary3[key] + "," + item.Name;
								}
							}
						}
					}
					if (j == importData.Columns.Count - 1 && drHeader2 != null && !dictionary.Any((KeyValuePair<string, string> f) => !string.IsNullOrWhiteSpace(f.Value) && (f.Value.IndexOf(transAmtObj.SpendAmt) > -1 || f.Value.IndexOf(transAmtObj.ReceivedAmt) > -1)))
					{
						int i = 0;
						while (i < importData.Columns.Count)
						{
							enumerable = from f in source
							where GetPropertyValue(solution, f.Name).EqualsIgnoreCase(Convert.ToString(drHeader2[i]))
							select f;
							if (enumerable.Any())
							{
								PropertyInfo propertyInfo2 = enumerable.ElementAt(0);
								dictionary[Convert.ToString(drHeader2[i])] = propertyInfo2.Name;
							}
							num5 = ++i;
						}
					}
				}
				if (list.Any())
				{
					throw new Exception(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "ExcelColumnHeadingDuplicate", "导入文件有重复的标题：{0}，请检查标题开始行是否正确！"), string.Join(",", list)));
				}
				int num6 = 0;
				foreach (string key2 in dictionary.Keys)
				{
					if (!string.IsNullOrWhiteSpace(key2))
					{
						string[] array = dictionary[key2].Split(',');
						string[] array2 = array;
						foreach (string value in array2)
						{
							if (source2.Contains(value))
							{
								num6++;
							}
						}
					}
				}
				bool flag2 = num6 == source2.Count();
				List<IVBankBillEntryModel> list2 = new List<IVBankBillEntryModel>();
				var anon = new
				{
					Date = "MDate",
					Time = "MTime"
				};
				string propertyName = "MBillType";
				string value2 = string.Empty;
				for (int l = mDataRowStart - 1; l < importData.Rows.Count; l++)
				{
					num2 = l;
					IVBankBillEntryModel iVBankBillEntryModel = new IVBankBillEntryModel();
					iVBankBillEntryModel.MRowIndex = l + 1;
					for (int m = 0; m < importData.Columns.Count; m++)
					{
						num3 = m;
						string text = Convert.ToString(dataRow[m]);
						if (dictionary.ContainsKey(text))
						{
							string[] source3 = dictionary[text].Split(',');
							string text2 = Convert.ToString(importData.Rows[l][m]);
							if (!string.IsNullOrWhiteSpace(text2))
							{
								if (source3.Contains(transAmtObj.SpendAmt) && source3.Contains(transAmtObj.ReceivedAmt))
								{
									flag = true;
									string propertyName2 = string.Empty;
									string text3 = (num == -1) ? string.Empty : Convert.ToString(importData.Rows[l][num]);
									string[] source4 = new string[2]
									{
										"借",
										"贷"
									};
									BillType billType;
									if (!string.IsNullOrWhiteSpace(text3) && source4.Contains(text3))
									{
										string a = text3;
										if (!(a == "借"))
										{
											if (a == "贷")
											{
												propertyName2 = transAmtObj.SpendAmt;
												billType = BillType.Spent;
												value2 = billType.ToString();
											}
										}
										else
										{
											propertyName2 = transAmtObj.ReceivedAmt;
											billType = BillType.Receive;
											value2 = billType.ToString();
										}
									}
									else
									{
										decimal num7 = Convert.ToDecimal(text2);
										if (num7 > decimal.Zero)
										{
											propertyName2 = transAmtObj.ReceivedAmt;
											billType = BillType.Receive;
											value2 = billType.ToString();
											if (flag2)
											{
												if (dictionary2.ContainsKey("MTransAcctNo"))
												{
													iVBankBillEntryModel.MTransAcctNo = Convert.ToString(importData.Rows[l][dictionary2["MTransAcctNo"]]);
												}
												if (dictionary2.ContainsKey("MTransAcctName"))
												{
													iVBankBillEntryModel.MTransAcctName = Convert.ToString(importData.Rows[l][dictionary2["MTransAcctName"]]);
												}
											}
										}
										else
										{
											propertyName2 = transAmtObj.SpendAmt;
											billType = BillType.Spent;
											value2 = billType.ToString();
											num7 = Math.Abs(num7);
											if (flag2)
											{
												if (dictionary2.ContainsKey("MTransAcctNo2"))
												{
													iVBankBillEntryModel.MTransAcctNo = Convert.ToString(importData.Rows[l][dictionary2["MTransAcctNo2"]]);
												}
												if (dictionary2.ContainsKey("MTransAcctName2"))
												{
													iVBankBillEntryModel.MTransAcctName = Convert.ToString(importData.Rows[l][dictionary2["MTransAcctName2"]]);
												}
											}
										}
										text2 = num7.ToString();
									}
									SetPropertyValue(iVBankBillEntryModel, propertyName2, text2, null, null);
									SetPropertyValue(iVBankBillEntryModel, propertyName, value2, null, null);
								}
								else if (source3.Contains(anon.Date) && source3.Contains(anon.Time))
								{
									SetPropertyValue(iVBankBillEntryModel, anon.Date, text2, ctx, null);
									SetPropertyValue(iVBankBillEntryModel, anon.Time, text2, ctx, null);
								}
								else if (!(source2.Contains(dictionary[text]) & flag2))
								{
									SetBillType(iVBankBillEntryModel, dictionary, text2, text);
									string[] array3 = dictionary[text].Split(',');
									string[] array4 = array3;
									foreach (string propertyName3 in array4)
									{
										SetPropertyValue(iVBankBillEntryModel, propertyName3, text2, ctx, solution.MDateFormat);
									}
								}
								goto IL_0933;
							}
							continue;
						}
						goto IL_0933;
						IL_0933:
						if (m == importData.Columns.Count - 1 && !flag && drHeader2 != null)
						{
							for (int num8 = 0; num8 < importData.Columns.Count; num8++)
							{
								text = Convert.ToString(drHeader2[num8]);
								if (dictionary.ContainsKey(text))
								{
									string text4 = Convert.ToString(importData.Rows[l][num8]);
									SetBillType(iVBankBillEntryModel, dictionary, text4, text);
									SetPropertyValue(iVBankBillEntryModel, dictionary[text], text4, ctx, null);
								}
							}
						}
					}
					if (!(iVBankBillEntryModel.MDate == DateTime.MinValue) || !(iVBankBillEntryModel.MSpentAmt == decimal.Zero) || !(iVBankBillEntryModel.MReceivedAmt == decimal.Zero) || !(iVBankBillEntryModel.MBalance == decimal.Zero) || !string.IsNullOrWhiteSpace(iVBankBillEntryModel.MTransAcctNo) || !string.IsNullOrWhiteSpace(iVBankBillEntryModel.MTransAcctName))
					{
						iVBankBillEntryModel.MSpentSplitAmt = iVBankBillEntryModel.MSpentAmt;
						iVBankBillEntryModel.MReceivedSplitAmt = iVBankBillEntryModel.MReceivedSplitAmt;
						iVBankBillEntryModel.MDesc = iVBankBillEntryModel.MDesc.ReplaceMultiSpaceToSingle();
						iVBankBillEntryModel.MUserRef = iVBankBillEntryModel.MDesc;
						if (!(iVBankBillEntryModel.MSpentAmt == decimal.Zero) || !(iVBankBillEntryModel.MReceivedAmt == decimal.Zero))
						{
							list2.Add(iVBankBillEntryModel);
						}
					}
				}
				num2 = -1;
				num3 = -1;
				ValidateDate(ctx, operationResult, list2);
				if (!operationResult.Success)
				{
					return operationResult;
				}
				ValidateBankBill(ctx, operationResult, list2);
				if (!operationResult.Success)
				{
					list2.Reverse();
					OperationResult operationResult2 = new OperationResult();
					ValidateBankBill(ctx, operationResult2, list2);
					if (!operationResult2.Success)
					{
						return operationResult;
					}
					operationResult = new OperationResult();
				}
				IVBankBillEntryModel iVBankBillEntryModel2 = list2[0];
				IVBankBillEntryModel iVBankBillEntryModel3 = list2[list2.Count - 1];
				model.MStartDate = iVBankBillEntryModel2.MDate;
				if (model.MStartDate < ctx.MBeginDate)
				{
					throw new Exception(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "CannotImportStatementBeforeBeginDate", "You can't import Statement before begin date:{0}!"), ctx.MBeginDate.ToShortDateString()));
				}
				model.MEndDate = iVBankBillEntryModel3.MDate;
				model.MStartBalance = iVBankBillEntryModel2.MBalance + iVBankBillEntryModel2.MSpentAmt - iVBankBillEntryModel2.MReceivedAmt;
				model.MEndBalance = iVBankBillEntryModel3.MBalance;
				IVBankBillModel lastBankBillModel = GetLastBankBillModel(ctx, model);
				if (!ValidateAppendBankBill(ctx, iVBankBillEntryModel2, lastBankBillModel, operationResult))
				{
					return operationResult;
				}
				if (!ValidateBankBillField(ctx, model, list2, operationResult))
				{
					return operationResult;
				}
				operationResult = SaveBankBillByPartial(ctx, model, list2);
				if (operationResult.Success)
				{
					DateTime t = model.MStartDate;
					DateTime t2 = model.MEndDate;
					if (t > t2)
					{
						t = model.MEndDate;
						t2 = model.MStartDate;
					}
					OperationResult operationResult3 = operationResult;
					JavaScriptSerializer javaScriptSerializer2 = javaScriptSerializer;
					num5 = list2.Count;
					operationResult3.Tag = javaScriptSerializer2.Serialize(new
					{
						Total = num5.ToString(),
						StartDate = t.ToString("yyyy-MM-dd"),
						EndDate = t2.ToString("yyyy-MM-dd")
					});
				}
			}
			catch (Exception ex)
			{
				string str = string.Empty;
				if (num2 > -1 && num3 > -1)
				{
					string[] excelHeader = ExcelHelper.GetExcelHeader(importData.Columns.Count);
					str = $"（{excelHeader[num3]}{num2 + 1}）";
				}
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					CheckItem = "银行对账单导入",
					Level = AlertEnum.Error,
					Message = ex.Message + str
				});
				return operationResult;
			}
			return operationResult;
		}

		private static bool ValidateBankBillField(MContext ctx, IVBankBillModel model, List<IVBankBillEntryModel> bankBillEntryList, OperationResult result)
		{
			bool result2 = true;
			model.TableName = new IVBankBillModel().TableName;
			model.ImportSolutionModel.TableName = new BankBillImportSolutionModel().TableName;
			List<string> list = COMModelValidateHelper.ValidateModel(ctx, model);
			foreach (IVBankBillEntryModel bankBillEntry in bankBillEntryList)
			{
				bankBillEntry.TableName = new IVBankBillEntryModel().TableName;
				list.AddRange(COMModelValidateHelper.ValidateModel(ctx, bankBillEntry));
			}
			if (list.Any())
			{
				result2 = false;
				foreach (string item in list)
				{
					result.VerificationInfor.Add(new BizVerificationInfor
					{
						Level = AlertEnum.Error,
						Message = item
					});
				}
			}
			return result2;
		}

		private static OperationResult SaveBankBillByPartial(MContext ctx, IVBankBillModel model, List<IVBankBillEntryModel> entryList)
		{
			OperationResult operationResult = new OperationResult();
			int num = 1000;
			int num2 = (int)Math.Ceiling((double)entryList.Count / (double)num);
			List<MultiDBCommand> list = new List<MultiDBCommand>();
			List<CommandInfo> list2 = ModelInfoManager.GetInsertOrUpdateCmd<IVBankBillModel>(ctx, model, null, true);
			int num3 = 0;
			for (int i = 0; i < num2; i++)
			{
				if (i > 0)
				{
					list2 = new List<CommandInfo>();
				}
				num3 = i * num;
				List<IVBankBillEntryModel> entryList2 = entryList.Skip(num * i).Take(num).ToList();
				list2.AddRange(GetBankBillEntryInsertCmds(ctx, entryList2, model.MID, num3));
				list.Add(new MultiDBCommand(ctx)
				{
					CommandList = list2,
					DBType = SysOrBas.Bas
				});
			}
			operationResult.Success = DbHelperMySQL.ExecuteSqlTran(ctx, list.ToArray());
			return operationResult;
		}

		private static bool ValidateAppendBankBill(MContext ctx, IVBankBillEntryModel startBillEntry, IVBankBillModel lastBankBill, OperationResult result)
		{
			if (lastBankBill == null)
			{
				return true;
			}
			result.VerificationInfor = (result.VerificationInfor ?? new List<BizVerificationInfor>());
			decimal d = startBillEntry.MBalance + startBillEntry.MSpentAmt - startBillEntry.MReceivedAmt;
			if (startBillEntry.MDate < lastBankBill.MEndDate)
			{
				string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "BankStatementStartDateError", "导入文件中的最早日期必须晚于或等于系统内已导入的对账单最后一个日期");
				result.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = text
				});
			}
			else if (d != lastBankBill.MEndBalance)
			{
				string message = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "BankStatementStartBalanceError", "第{0}行数据的余额-第{0}行的数据的收+第{0}行数据的支 不等于 系统余额"), startBillEntry.MRowIndex);
				result.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = message
				});
			}
			return !result.VerificationInfor.Any();
		}

		private static void ValidateDate(MContext ctx, OperationResult result, List<IVBankBillEntryModel> bankBillEntryList)
		{
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "FieldCanNotBeEmpty", "{0} 不能有空值！");
			string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "TransDate", "Transaction Date");
			IEnumerable<IVBankBillEntryModel> source = from f in bankBillEntryList
			where f.MDate == DateTime.MinValue
			select f;
			if (source.Any())
			{
				result.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = string.Format(text, text2)
				});
			}
			else
			{
				string text3 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "TransDateNotInOrder", "Transaction dates are not in order!");
				DateTime mDate = bankBillEntryList[0].MDate;
				DateTime mDate2 = bankBillEntryList[bankBillEntryList.Count - 1].MDate;
				DateTime t = mDate;
				bool flag = mDate >= mDate2;
				bool flag2 = true;
				foreach (IVBankBillEntryModel bankBillEntry in bankBillEntryList)
				{
					if (flag)
					{
						if (t >= bankBillEntry.MDate)
						{
							t = bankBillEntry.MDate;
							continue;
						}
						flag2 = false;
					}
					else
					{
						if (t <= bankBillEntry.MDate)
						{
							t = bankBillEntry.MDate;
							continue;
						}
						flag2 = false;
					}
					break;
				}
				if (!flag2)
				{
					result.VerificationInfor.Add(new BizVerificationInfor
					{
						Level = AlertEnum.Error,
						Message = string.Format(text3)
					});
				}
			}
		}

		private static void ValidateBankBill(MContext ctx, OperationResult result, List<IVBankBillEntryModel> bankBillEntryList)
		{
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "TransactionAmountErrorMsg", "该交易日期（{0}）的交易金额不正确（{1}）！");
			int num = 0;
			IVBankBillEntryModel iVBankBillEntryModel = null;
			List<IVBankBillEntryModel> list = new List<IVBankBillEntryModel>();
			string empty = string.Empty;
			foreach (IVBankBillEntryModel bankBillEntry in bankBillEntryList)
			{
				if (list.Any())
				{
					iVBankBillEntryModel = list[list.Count() - 1];
					if (iVBankBillEntryModel.MBalance - bankBillEntry.MSpentAmt + bankBillEntry.MReceivedAmt != bankBillEntry.MBalance)
					{
						empty = ((!(bankBillEntry.MSpentAmt > decimal.Zero)) ? $"{iVBankBillEntryModel.MBalance} + {bankBillEntry.MReceivedAmt} != {bankBillEntry.MBalance}" : $"{iVBankBillEntryModel.MBalance} - {bankBillEntry.MSpentAmt} != {bankBillEntry.MBalance}");
						result.VerificationInfor.Add(new BizVerificationInfor
						{
							Level = AlertEnum.Error,
							Message = string.Format(text, bankBillEntry.MDate.ToOrgZoneDateString(null), empty)
						});
						break;
					}
				}
				list.Add(bankBillEntry);
				num++;
			}
		}

		private static List<CommandInfo> GetBankBillEntryInsertCmds(MContext ctx, List<IVBankBillEntryModel> entryList, string bankBillID, int startIndex = 0)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			string value = "INSERT INTO t_iv_bankbillentry(MEntryID,MID,MSeq,MDate,MTime,MTransType,MTransNo,MTransAcctName,MTransAcctNo,MBillType,MSpentAmt,MSpentSplitAmt,MReceivedAmt,MReceivedSplitAmt,MBalance,MDesc,MRef,MUserRef,MCheckState,MIsDelete,MCreatorID,MCreateDate,MModifierID,MModifyDate,MOrgID)";
			string format = " SELECT @MEntryID{0},@MID{0},@MSeq{0},@MDate{0},@MTime{0},@MTransType{0},@MTransNo{0},@MTransAcctName{0},@MTransAcctNo{0},@MBillType{0},@MSpentAmt{0},@MSpentAmt{0},@MReceivedAmt{0},@MReceivedAmt{0},@MBalance{0},@MDesc{0},@MRef{0},@MUserRef{0},0,0,@MCreatorID{0},@MCreateDate{0},@MModifierID{0},@MModifyDate{0},@MOrgID{0}";
			StringBuilder stringBuilder = new StringBuilder(56000);
			stringBuilder.Append(value);
			int num = 0;
			List<MySqlParameter> list2 = new List<MySqlParameter>();
			foreach (IVBankBillEntryModel entry in entryList)
			{
				if (num > 0)
				{
					stringBuilder.Append(" UNION ALL ");
				}
				list2.AddRange(GetBankBillEntryParams(ctx, startIndex + num, entry, bankBillID));
				stringBuilder.AppendFormat(format, startIndex + num);
				num++;
			}
			List<CommandInfo> list3 = list;
			CommandInfo obj = new CommandInfo
			{
				CommandText = stringBuilder.ToString()
			};
			DbParameter[] array = obj.Parameters = list2.ToArray();
			list3.Add(obj);
			return list;
		}

		private static List<MySqlParameter> GetBankBillEntryParams(MContext ctx, int i, IVBankBillEntryModel entry, string bankBillID)
		{
			return new List<MySqlParameter>
			{
				new MySqlParameter($"@MEntryID{i}", UUIDHelper.GetGuid()),
				new MySqlParameter($"@MID{i}", bankBillID),
				new MySqlParameter($"@MSeq{i}", i + 1),
				new MySqlParameter($"@MDate{i}", entry.MDate),
				new MySqlParameter($"@MTime{i}", entry.MTime),
				new MySqlParameter($"@MTransType{i}", entry.MTransType),
				new MySqlParameter($"@MTransNo{i}", entry.MTransNo),
				new MySqlParameter($"@MTransAcctName{i}", entry.MTransAcctName),
				new MySqlParameter($"@MTransAcctNo{i}", entry.MTransAcctNo),
				new MySqlParameter($"@MBillType{i}", entry.MBillType),
				new MySqlParameter($"@MSpentAmt{i}", entry.MSpentAmt),
				new MySqlParameter($"@MReceivedAmt{i}", entry.MReceivedAmt),
				new MySqlParameter($"@MBalance{i}", entry.MBalance),
				new MySqlParameter($"@MDesc{i}", entry.MDesc),
				new MySqlParameter($"@MRef{i}", entry.MRef),
				new MySqlParameter($"@MUserRef{i}", entry.MUserRef),
				new MySqlParameter($"@MCreatorID{i}", ctx.MUserID),
				new MySqlParameter($"@MCreateDate{i}", ctx.DateNow),
				new MySqlParameter($"@MModifierID{i}", ctx.MUserID),
				new MySqlParameter($"@MModifyDate{i}", ctx.DateNow),
				new MySqlParameter($"@MOrgID{i}", ctx.MOrgID)
			};
		}

		private static OperationResult CheckImportSolutionNameExist(MContext ctx, string type, string name, string id)
		{
			OperationResult operationResult = new OperationResult();
			string text = "select Count(*) from t_bd_bankbillimportsolution a \r\n                            left join t_bd_bankbillimportsolution_l b\r\n                            on a.MItemID=b.MParentID and MLocaleID=@MLocaleID AND  b.MIsDelete = 0 \r\n                            WHERE a.MIsDelete=0 and (a.MOrgID=@MOrgID or ifnull(a.MOrgID, '')='') AND a.MParentID=@MParentID AND b.MName=@MName";
			if (!string.IsNullOrEmpty(id))
			{
				text += " AND a.MItemID <> @MItemID ";
			}
			MySqlParameter[] cmdParms = new MySqlParameter[5]
			{
				new MySqlParameter("@MLocaleID", ctx.MLCID),
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MParentID", type),
				new MySqlParameter("@MName", name),
				new MySqlParameter("@MItemID", id)
			};
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			object single = dynamicDbHelperMySQL.GetSingle(text, cmdParms);
			if (Convert.ToInt32(single) > 0)
			{
				operationResult.Success = false;
				string message = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "ImportSolutionExist", "The import solution:{0} already exists!"), name);
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = message
				});
			}
			return operationResult;
		}

		private static void SetBillType(IVBankBillEntryModel model, Dictionary<string, string> dicMapping, string cellValue, string headerName)
		{
			decimal value = default(decimal);
			if (decimal.TryParse(cellValue, out value) && Math.Abs(value) > decimal.Zero)
			{
				BillType billType;
				if (dicMapping[headerName] == "MSpentAmt")
				{
					billType = BillType.Spent;
					SetPropertyValue(model, "MBillType", billType.ToString(), null, null);
				}
				else if (dicMapping[headerName] == "MReceivedAmt")
				{
					billType = BillType.Receive;
					SetPropertyValue(model, "MBillType", billType.ToString(), null, null);
				}
			}
		}

		private static IVBankBillModel GetLastBankBillModel(MContext ctx, IVBankBillModel model)
		{
			string sql = "SELECT * FROM T_IV_BankBill\r\n                             WHERE MOrgID=@MOrgID and MBankID=@MBankID AND MIsDelete=0  \r\n                            ORDER BY MImportDate DESC";
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MBankID", model.MBankID)
			};
			List<IVBankBillModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<IVBankBillModel>(ctx, sql, cmdParms);
			return dataModelBySql.FirstOrDefault();
		}

		private static CommandInfo GetStatementViewUpdateSql(MContext ctx, IVBankStatementViewModel entry)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("update T_IV_BankBillEntry set ");
			stringBuilder.Append(" MTransType = @MTransType , ");
			stringBuilder.Append(" MTransAcctName = @MTransAcctName,  ");
			stringBuilder.Append(" MTransAcctNo = @MTransAcctNo,  ");
			stringBuilder.Append(" MDesc = @MDesc  ");
			stringBuilder.Append(" where MEntryID=@MEntryID and MOrgID=@MOrgID");
			MySqlParameter[] array = new MySqlParameter[6]
			{
				new MySqlParameter("@MEntryID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MTransType", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MTransAcctName", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MTransAcctNo", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MDesc", MySqlDbType.VarChar, 500),
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = entry.MEntryID;
			array[1].Value = entry.MTransType;
			array[2].Value = entry.MTransAcctName;
			array[3].Value = entry.MTransAcctNo;
			array[4].Value = entry.MDesc;
			array[5].Value = ctx.MOrgID;
			return new CommandInfo(stringBuilder.ToString(), array);
		}

		private static string GetPropertyValue<T>(T model, string propertyName)
		{
			PropertyInfo property = model.GetType().GetProperty(propertyName);
			if (property != (PropertyInfo)null)
			{
				return Convert.ToString(property.GetValue(model, null));
			}
			return string.Empty;
		}

		private static void SetPropertyValue<T>(T model, string propertyName, string value, MContext ctx = null, string dateFormat = null)
		{
			PropertyInfo property = model.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
			if (property != (PropertyInfo)null && property.CanWrite)
			{
				if (property.PropertyType == typeof(bool))
				{
					property.SetValue(model, value == "1");
				}
				else if (property.PropertyType == typeof(DateTime))
				{
					if (!string.IsNullOrWhiteSpace(value))
					{
						try
						{
							if (string.IsNullOrWhiteSpace(dateFormat))
							{
								property.SetValue(model, Convert.ToDateTime(value.ToDateString()));
							}
							else
							{
								if (!DateTime.TryParseExact(value, dateFormat.Split(','), CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
								{
									dateTime = Convert.ToDateTime(value.ToDateString());
								}
								property.SetValue(model, dateTime);
							}
						}
						catch (Exception)
						{
							throw new Exception(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "DateConvertFail", "日期:{0}转换失败!"), value));
						}
					}
				}
				else if (property.PropertyType == typeof(decimal))
				{
					decimal num = default(decimal);
					if (decimal.TryParse(value, out num))
					{
						if (propertyName == "MSpentAmt" || propertyName == "MReceivedAmt")
						{
							num = Math.Round(Math.Abs(num), 2);
						}
						else if (propertyName == "MBalance")
						{
							num = Math.Round(num, 2);
						}
						property.SetValue(model, num);
					}
				}
				else if (property.PropertyType == typeof(int))
				{
					property.SetValue(model, Convert.ToInt32(value));
				}
				else if (property.Name.Equals("MTime"))
				{
					string message = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "TimeConvertFail", "时间:{0}转换失败!"), value);
					try
					{
						value = value.PadLeft(6, '0');
						string text = value.ToTimeString();
						if (!DateTime.TryParse(text, out DateTime _))
						{
							throw new Exception(message);
						}
						property.SetValue(model, text);
					}
					catch (Exception)
					{
						throw new Exception(message);
					}
				}
				else
				{
					property.SetValue(model, value);
				}
			}
		}
	}
}
