using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.IV.Expense;
using JieNor.Megi.DataModel.IV.Verification;
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.DataRepository.Log;
using JieNor.Megi.DataRepository.PA;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace JieNor.Megi.DataRepository.IV
{
	public static class IVVerificationRepository
	{
		public static List<IVVerificationListModel> GetVerificationList(MContext ctx, IVVerificationListFilterModel filter)
		{
			filter.MBillID = null;
			List<IVVerificationListModel> result = null;
			switch (filter.MBizBillType)
			{
			case "Invoice":
				result = GetInvoiceVerificationList(ctx, filter);
				break;
			case "Payment":
				result = GetPaymentVerificationList(ctx, filter);
				break;
			case "Receive":
				result = GetReceiveVerificationList(ctx, filter);
				break;
			case "Expense":
				result = GetExpenseVerificationList(ctx, filter);
				break;
			case "PayRun":
				result = GetSalaryPaymentVerificationList(ctx, filter);
				break;
			}
			return result;
		}

		public static decimal GetHaveVerificationTotalAmt(MContext ctx, string billID)
		{
			string sql = "SELECT SUM(MAmt) from t_iv_verification where (MSourceBillID=@BillID or MTargetBillID=@BillID) AND MOrgID=@MOrgID AND MIsDelete=0 ";
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@BillID", billID)
			};
			object single = dynamicDbHelperMySQL.GetSingle(sql, cmdParms);
			return Convert.ToDecimal(single);
		}

		public static void UpdateVerification(MContext ctx, IVVerificationModel model)
		{
			UpdateVerificationList(ctx, new List<IVVerificationModel>
			{
				model
			});
		}

		public static List<CommandInfo> GetNewVerificationCmd(MContext ctx, IVVerificationModel model)
		{
			return GetNewVerificationListCmd(ctx, new List<IVVerificationModel>
			{
				model
			});
		}

		public static List<CommandInfo> GetNewVerificationListCmd(MContext ctx, List<IVVerificationModel> list)
		{
			decimal num = list.Sum((IVVerificationModel t) => t.MAmount);
			IVVerificationModel iVVerificationModel = list[0];
			List<CommandInfo> list2 = new List<CommandInfo>();
			foreach (IVVerificationModel item in list)
			{
				list2.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<IVVerificationModel>(ctx, item, null, true));
				list2.AddRange(GetCalculateBillVerificationAmountCmd(ctx, item, false));
			}
			return list2;
		}

		public static OperationResult UpdateVerificationList(MContext ctx, List<IVVerificationModel> list)
		{
			OperationResult operationResult = new OperationResult();
			if (list == null || list.Count == 0)
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "NoData", "NoData");
				return operationResult;
			}
			foreach (IVVerificationModel item in list)
			{
				if (!CheckDbVerifyAmt(ctx, item))
				{
					operationResult.Success = false;
					operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "DataHadBeenUpdate", "Data had been updated.");
					return operationResult;
				}
			}
			list = (from t in list
			orderby t.MAmount
			select t).ToList();
			decimal d = list.Sum((IVVerificationModel t) => t.MAmount);
			IVVerificationModel ivfModel = list[0];
			decimal one = decimal.One;
			decimal value = default(decimal);
			decimal d2 = default(decimal);
			decimal noVerifyAmt = GetNoVerifyAmt(ctx, ivfModel, out one, out value, out d2);
			if (d > noVerifyAmt)
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "DataHadBeenUpdate", "Data had been updated.");
				return operationResult;
			}
			foreach (IVVerificationModel item2 in list)
			{
				item2.MAmtFor = item2.MAmount;
				item2.MAmt = Math.Round(item2.MAmount * one, 2);
			}
			if (d == noVerifyAmt)
			{
				decimal num = Math.Abs(value) - d2 - Math.Abs(list.Sum((IVVerificationModel t) => t.MAmt));
				if (num != decimal.Zero)
				{
					list[list.Count - 1].MAmt = list[list.Count - 1].MAmt + num;
				}
			}
			List<CommandInfo> list2 = new List<CommandInfo>();
			foreach (IVVerificationModel item3 in list)
			{
				item3.MCreateDate = ctx.DateNow;
				list2.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<IVVerificationModel>(ctx, item3, null, true));
				if (item3.MSourceBillType == "Invoice")
				{
					IVInvoiceModel invoiceEditModel = IVInvoiceRepository.GetInvoiceEditModel(ctx, item3.MSourceBillID);
					list2.AddRange(IVInvoiceLogHelper.GetVerificationLogCmd(ctx, invoiceEditModel, item3.MAmount));
				}
				if (item3.MTargetBillType == "Invoice")
				{
					IVInvoiceModel invoiceEditModel2 = IVInvoiceRepository.GetInvoiceEditModel(ctx, item3.MTargetBillID);
					list2.AddRange(IVInvoiceLogHelper.GetVerificationLogCmd(ctx, invoiceEditModel2, item3.MAmount));
				}
				if (item3.MSourceBillType == "Expense")
				{
					IVExpenseModel expenseModel = IVExpenseRepository.GetExpenseModel(ctx, item3.MSourceBillID);
					list2.AddRange(IVExpenseLogHelper.GetVerificationLogCmd(ctx, expenseModel, item3.MAmount));
				}
				if (item3.MTargetBillType == "Expense")
				{
					IVExpenseModel expenseModel2 = IVExpenseRepository.GetExpenseModel(ctx, item3.MTargetBillID);
					list2.AddRange(IVExpenseLogHelper.GetVerificationLogCmd(ctx, expenseModel2, item3.MAmount));
				}
			}
			foreach (IVVerificationModel item4 in list)
			{
				list2.AddRange(GetCalculateBillVerificationAmountCmd(ctx, item4, false));
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num2 = dynamicDbHelperMySQL.ExecuteSqlTran(list2);
			operationResult.Success = (num2 > 0);
			return operationResult;
		}

		private static decimal GetNoVerifyAmt(MContext ctx, IVVerificationModel ivfModel, out decimal exchangeRate, out decimal totalAmt, out decimal verifyAmt)
		{
			exchangeRate = decimal.One;
			totalAmt = default(decimal);
			verifyAmt = default(decimal);
			decimal result = default(decimal);
			switch (ivfModel.MSourceBillType)
			{
			case "Invoice":
			{
				IVInvoiceRepository iVInvoiceRepository = new IVInvoiceRepository();
				IVInvoiceModel dataModel = iVInvoiceRepository.GetDataModel(ctx, ivfModel.MSourceBillID, false);
				if (dataModel != null)
				{
					dataModel.InvoiceEntry.Clear();
					result = Math.Abs(dataModel.MTaxTotalAmtFor) - Math.Abs(dataModel.MVerificationAmt);
					verifyAmt = Math.Abs(dataModel.MVerifyAmt);
					exchangeRate = dataModel.MOToLRate;
					totalAmt = dataModel.MTaxTotalAmt;
				}
				break;
			}
			case "Payment":
			{
				IVPaymentModel paymentEditModel = IVPaymentRepository.GetPaymentEditModel(ctx, ivfModel.MSourceBillID);
				if (paymentEditModel != null)
				{
					paymentEditModel.PaymentEntry.Clear();
					result = Math.Abs(paymentEditModel.MTaxTotalAmtFor) - Math.Abs(paymentEditModel.MVerificationAmt);
					verifyAmt = Math.Abs(paymentEditModel.MVerifyAmt);
					exchangeRate = paymentEditModel.MOToLRate;
					totalAmt = paymentEditModel.MTaxTotalAmt;
				}
				break;
			}
			case "Receive":
			{
				IVReceiveModel receiveEditModel = IVReceiveRepository.GetReceiveEditModel(ctx, ivfModel.MSourceBillID);
				if (receiveEditModel != null)
				{
					receiveEditModel.ReceiveEntry.Clear();
					result = Math.Abs(receiveEditModel.MTaxTotalAmtFor) - Math.Abs(receiveEditModel.MVerificationAmt);
					verifyAmt = Math.Abs(receiveEditModel.MVerifyAmt);
					exchangeRate = receiveEditModel.MOToLRate;
					totalAmt = receiveEditModel.MTaxTotalAmt;
				}
				break;
			}
			case "Expense":
			{
				IVExpenseModel expenseModel = IVExpenseRepository.GetExpenseModel(ctx, ivfModel.MSourceBillID);
				if (expenseModel != null)
				{
					expenseModel.ExpenseEntry.Clear();
					result = Math.Abs(expenseModel.MTaxTotalAmtFor) - Math.Abs(expenseModel.MVerificationAmt);
					verifyAmt = Math.Abs(expenseModel.MVerifyAmt);
					exchangeRate = expenseModel.MOToLRate;
					totalAmt = expenseModel.MTaxTotalAmt;
				}
				break;
			}
			case "PayRun":
			{
				PASalaryPaymentModel salaryPaymentEditModel = PASalaryPaymentRepository.GetSalaryPaymentEditModel(ctx, ivfModel.MSourceBillID);
				if (salaryPaymentEditModel != null)
				{
					salaryPaymentEditModel.SalaryPaymentEntry.Clear();
					result = Math.Abs(salaryPaymentEditModel.MNetSalary) - Math.Abs(salaryPaymentEditModel.MVerificationAmt);
					verifyAmt = Math.Abs(salaryPaymentEditModel.MVerificationAmt);
					exchangeRate = decimal.One;
					totalAmt = Math.Abs(salaryPaymentEditModel.MNetSalary);
				}
				break;
			}
			}
			return result;
		}

		private static bool CheckDbVerifyAmt(MContext ctx, IVVerificationModel ivfModel)
		{
			decimal num = default(decimal);
			switch (ivfModel.MTargetBillType)
			{
			case "Invoice":
			{
				IVInvoiceRepository iVInvoiceRepository = new IVInvoiceRepository();
				IVInvoiceModel dataModel = iVInvoiceRepository.GetDataModel(ctx, ivfModel.MTargetBillID, false);
				if (dataModel != null && dataModel.MStatus == 3)
				{
					dataModel.InvoiceEntry.Clear();
					num = Math.Abs(dataModel.MTaxTotalAmtFor) - Math.Abs(dataModel.MVerificationAmt);
					return num >= ivfModel.MAmount;
				}
				goto default;
			}
			case "Payment":
			{
				IVPaymentModel paymentEditModel = IVPaymentRepository.GetPaymentEditModel(ctx, ivfModel.MTargetBillID);
				if (paymentEditModel != null)
				{
					paymentEditModel.PaymentEntry.Clear();
					num = Math.Abs(paymentEditModel.MTaxTotalAmtFor) - Math.Abs(paymentEditModel.MVerificationAmt);
					return num >= ivfModel.MAmount;
				}
				goto default;
			}
			case "Receive":
			{
				IVReceiveModel receiveEditModel = IVReceiveRepository.GetReceiveEditModel(ctx, ivfModel.MTargetBillID);
				if (receiveEditModel != null)
				{
					receiveEditModel.ReceiveEntry.Clear();
					num = Math.Abs(receiveEditModel.MTaxTotalAmtFor) - Math.Abs(receiveEditModel.MVerificationAmt);
					return num >= ivfModel.MAmount;
				}
				goto default;
			}
			case "Expense":
			{
				IVExpenseModel expenseModel = IVExpenseRepository.GetExpenseModel(ctx, ivfModel.MTargetBillID);
				if (expenseModel != null && expenseModel.MStatus == 3)
				{
					expenseModel.ExpenseEntry.Clear();
					num = Math.Abs(expenseModel.MTaxTotalAmtFor) - Math.Abs(expenseModel.MVerificationAmt);
					return num >= ivfModel.MAmount;
				}
				goto default;
			}
			case "PayRun":
			{
				PASalaryPaymentModel salaryPaymentEditModel = PASalaryPaymentRepository.GetSalaryPaymentEditModel(ctx, ivfModel.MTargetBillID);
				if (salaryPaymentEditModel != null)
				{
					salaryPaymentEditModel.SalaryPaymentEntry.Clear();
					num = Math.Abs(salaryPaymentEditModel.MNetSalary) - Math.Abs(salaryPaymentEditModel.MVerificationAmt);
					return num >= ivfModel.MAmount;
				}
				goto default;
			}
			default:
				return false;
			}
		}

		public static OperationResult DeleteVerification(MContext ctx, IVVerificationModel model)
		{
			OperationResult operationResult = new OperationResult();
			List<CommandInfo> list = new List<CommandInfo>();
			if (model != null && !string.IsNullOrWhiteSpace(model.MID))
			{
				list.Add(GetVerificationDeleteSql(ctx, model));
			}
			list.AddRange(GetCalculateBillVerificationAmountCmd(ctx, model, true));
			if (model.MSourceBillType == "Invoice")
			{
				IVInvoiceModel invoiceEditModel = IVInvoiceRepository.GetInvoiceEditModel(ctx, model.MSourceBillID);
				list.AddRange(IVInvoiceLogHelper.GetDeleteVerificationLogCmd(ctx, invoiceEditModel, model.MAmount));
			}
			if (model.MTargetBillType == "Invoice")
			{
				IVInvoiceModel invoiceEditModel2 = IVInvoiceRepository.GetInvoiceEditModel(ctx, model.MTargetBillID);
				list.AddRange(IVInvoiceLogHelper.GetDeleteVerificationLogCmd(ctx, invoiceEditModel2, model.MAmount));
			}
			if (model.MSourceBillType == "Expense")
			{
				IVExpenseModel expenseModel = IVExpenseRepository.GetExpenseModel(ctx, model.MSourceBillID);
				list.AddRange(IVExpenseLogHelper.GetDeleteVerificationLogCmd(ctx, expenseModel, model.MAmount));
			}
			if (model.MTargetBillType == "Expense")
			{
				IVExpenseModel expenseModel2 = IVExpenseRepository.GetExpenseModel(ctx, model.MTargetBillID);
				list.AddRange(IVExpenseLogHelper.GetDeleteVerificationLogCmd(ctx, expenseModel2, model.MAmount));
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			dynamicDbHelperMySQL.ExecuteSqlTran(list);
			operationResult.Success = true;
			return operationResult;
		}

		public static List<CommandInfo> GetDeleteVerificationCmdList(MContext ctx, List<IVVerificationModel> modelList)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			if (modelList == null || modelList.Count() == 0)
			{
				return list;
			}
			foreach (IVVerificationModel model in modelList)
			{
				if (model != null && !string.IsNullOrWhiteSpace(model.MID))
				{
					list.Add(GetVerificationDeleteSql(ctx, model));
				}
			}
			return list;
		}

		public static OperationResult DeleteMergePayVerification(MContext ctx, IVVerificationModel model)
		{
			OperationResult operationResult = new OperationResult();
			List<CommandInfo> list = new List<CommandInfo>();
			if (model != null && !string.IsNullOrWhiteSpace(model.MID))
			{
				list.Add(GetVerificationDeleteSql(ctx, model));
				List<PASalaryToIVPayEntryModel> salaryToIVPayEntryLists = GetSalaryToIVPayEntryLists(ctx, model);
				foreach (PASalaryToIVPayEntryModel item in salaryToIVPayEntryLists)
				{
					list.Add(GetSalaryPaymentUpdateSql(ctx, item.MSalaryPaymentID));
				}
				list.Add(GetSalarytoIVPayDeleteSql(ctx, model));
				list.Add(GetSalaryToIVPayEntryDeleteSql(ctx, model));
				list.AddRange(GetSalaryMergePaymentVerificationDeleteCmds(ctx, model));
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			dynamicDbHelperMySQL.ExecuteSqlTran(list);
			operationResult.Success = true;
			return operationResult;
		}

		private static List<CommandInfo> GetSalaryMergePaymentVerificationDeleteCmds(MContext ctx, IVVerificationModel model)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			if (model == null || string.IsNullOrWhiteSpace(model.MID))
			{
				return list;
			}
			string sqlText = "UPDATE T_IV_Payment SET MVerificationAmt=0,MVerifyAmtFor=0,MVerifyAmt=0 WHERE MID = @BillID and MOrgID = @MOrgID and MIsDelete = 0";
			MySqlParameter[] para = new MySqlParameter[2]
			{
				new MySqlParameter("@BillID", model.MSourceBillID),
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			list.Add(new CommandInfo(sqlText, para));
			return list;
		}

		private static CommandInfo GetSalaryPaymentUpdateSql(MContext ctx, string salaryPaymentId)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(" UPDATE T_PA_SalaryPayment t1\r\n\t                                    JOIN T_PA_PayRun t2 on t2.MID=t1.MRunID and t2.MOrgID=t1.MOrgID and t2.MIsDelete=0\r\n                                        JOIN t_pa_salarytoivpayentry t3 on t3.MOrgID=t1.MOrgID and t3.MSalaryPaymentID=t1.MID and t3.MIsDelete=0\t\r\n                                        SET t1.MStatus={0},t1.MVerificationAmt=t1.MVerificationAmt-t3.MAmount,t1.MVerifyAmt=t1.MVerifyAmt-t3.MAmount,t1.MVerifyAmtFor=t1.MVerifyAmtFor-t3.MAmount,t2.MStatus={0} \r\n                                    WHERE t1.MID = @MID and t1.MOrgID = @MOrgID and t1.MIsDelete = 0 ", Convert.ToInt32(PASalaryPaymentStatusEnum.WaitingPayment));
			MySqlParameter[] para = new MySqlParameter[2]
			{
				new MySqlParameter("@MID", salaryPaymentId),
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			return new CommandInfo(stringBuilder.ToString(), para);
		}

		private static CommandInfo GetSalarytoIVPayDeleteSql(MContext ctx, IVVerificationModel model)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("UPDATE t_pa_salarytoivpay SET MIsDelete=1 where MID = @MID and MOrgID = @MOrgID and MIsDelete = 0 ");
			MySqlParameter[] array = new MySqlParameter[2]
			{
				new MySqlParameter("@MID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			array[0].Value = model.MID;
			return new CommandInfo(stringBuilder.ToString(), array);
		}

		private static List<PASalaryToIVPayEntryModel> GetSalaryToIVPayEntryLists(MContext ctx, IVVerificationModel model)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(" SELECT * FROM t_pa_salarytoivpayentry WHERE MOrgID=@MOrgID AND MID=@MID AND MIsDelete = 0 ");
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@MID", model.MTargetBillID),
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			return ModelInfoManager.GetDataModelBySql<PASalaryToIVPayEntryModel>(ctx, stringBuilder.ToString(), cmdParms);
		}

		private static CommandInfo GetSalaryToIVPayEntryDeleteSql(MContext ctx, IVVerificationModel model)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("UPDATE t_pa_salarytoivpayentry SET MIsDelete=1 where MID = @MID and MOrgID = @MOrgID and MIsDelete = 0 ");
			MySqlParameter[] para = new MySqlParameter[2]
			{
				new MySqlParameter("@MID", model.MTargetBillID),
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			return new CommandInfo(stringBuilder.ToString(), para);
		}

		public static OperationResult DeleteVerification(MContext ctx, string pkID, bool isMergePay = false)
		{
			IVVerificationModel verificationEditModel = GetVerificationEditModel(ctx, pkID);
			if (verificationEditModel == null)
			{
				return new OperationResult
				{
					Success = false,
					Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "DataHasBeenChanged", "Data has benn changed！")
				};
			}
			if (isMergePay)
			{
				return DeleteMergePayVerification(ctx, verificationEditModel);
			}
			return DeleteVerification(ctx, verificationEditModel);
		}

		public static IVVerificationModel GetVerificationEditModel(MContext ctx, string pkID)
		{
			return ModelInfoManager.GetDataEditModel<IVVerificationModel>(ctx, pkID, false, true);
		}

		public static List<IVVerificationListModel> GetHaveVerifData(MContext ctx, IVVerificationListFilterModel filter)
		{
			string haveVerifDataView = GetHaveVerifDataView(filter);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(GetHaveVerifDataSql(filter, haveVerifDataView, "Invoice"));
			stringBuilder.AppendLine(" Union All");
			stringBuilder.AppendLine(GetHaveVerifDataSql(filter, haveVerifDataView, "Payment"));
			stringBuilder.AppendLine(" Union All");
			stringBuilder.AppendLine(GetHaveVerifDataSql(filter, haveVerifDataView, "Receive"));
			MySqlParameter[] array = new MySqlParameter[8]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MBizType", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MContactID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MCurrencyID", MySqlDbType.VarChar, 6),
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MDueDate", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MBillID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MBizBillType", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = filter.MBizType;
			array[2].Value = filter.MContactID;
			array[3].Value = filter.MCurrencyID;
			array[4].Value = ctx.MLCID;
			array[5].Value = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
			array[6].Value = filter.MBillID;
			array[7].Value = filter.MBizBillType;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array);
			return ModelInfoManager.DataTableToList<IVVerificationListModel>(dataSet.Tables[0]);
		}

		public static IVVerificationModel GetMergeSalaryPaymentVerifData(MContext ctx, IVVerificationListFilterModel filter)
		{
			string sql = " SELECT t2.*, t3.MBizDate FROM t_pa_salarytoivpayentry t1 \r\n                                INNER JOIN   t_iv_verification t2 ON t1.MID = t2.MTargetBillID AND t1.MOrgID = t2.MOrgID AND t2.MIsDelete = 0 \r\n                                LEFT JOIN t_iv_payment t3 ON t3.MID = t2.MSourceBillID AND t3.MOrgID = t1.MOrgID AND t3.MIsDelete = 0 \r\n                            WHERE t1.MSalaryPaymentID =@MSalaryPaymentID AND t1.MOrgID =@MOrgID AND t1.MIsDelete= 0 ";
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MSalaryPaymentID", filter.MBillID)
			};
			return ModelInfoManager.GetDataModel<IVVerificationModel>(ctx, sql, cmdParms);
		}

		public static IVVerificationModel GetSalaryMergePaymentVerifData(MContext ctx, IVVerificationListFilterModel filter)
		{
			string sql = " SELECT t2.*,t1.MNumber FROM t_iv_payment t1 \r\n                                INNER JOIN   t_iv_verification t2 ON t1.MID = t2.MSourceBillID AND t1.MOrgID = t2.MOrgID AND t2.MIsDelete = 0 \r\n                                INNER JOIN t_pa_salarytoivpay t3 ON t3.MOrgID=t1.MOrgID AND t3.MID=t2.MTargetBillID AND t3.MIsDelete=0\r\n                            WHERE t1.MID =@MID AND t1.MOrgID =@MOrgID AND t1.MIsDelete= 0 ";
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MID", filter.MBillID)
			};
			return ModelInfoManager.GetDataModel<IVVerificationModel>(ctx, sql, cmdParms);
		}

		public static List<IVVerificationListModel> GetHistoryVerifData(MContext ctx, IVVerificationListFilterModel filter)
		{
			string haveVerifDataView = GetHaveVerifDataView(filter);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(GetHistoryVerifDataSql(filter, haveVerifDataView, "Invoice"));
			stringBuilder.AppendLine(" Union All");
			stringBuilder.AppendLine(GetHistoryVerifDataSql(filter, haveVerifDataView, "Payment"));
			stringBuilder.AppendLine(" Union All");
			stringBuilder.AppendLine(GetHistoryVerifDataSql(filter, haveVerifDataView, "Receive"));
			stringBuilder.AppendLine(" Union All");
			stringBuilder.AppendLine(GetHistoryVerifDataSql(filter, haveVerifDataView, "Expense"));
			stringBuilder.AppendLine(" Union All");
			stringBuilder.AppendLine(GetHistoryVerifDataSql(filter, haveVerifDataView, "PayRun"));
			MySqlParameter[] array = new MySqlParameter[9]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MBizType", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MContactID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MCurrencyID", MySqlDbType.VarChar, 6),
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MDueDate", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MBillID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MBizBillType", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MCyID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = filter.MBizType;
			array[2].Value = filter.MContactID;
			array[3].Value = filter.MCurrencyID;
			array[4].Value = ctx.MLCID;
			array[5].Value = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
			array[6].Value = filter.MBillID;
			array[7].Value = filter.MBizBillType;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array);
			List<IVVerificationListModel> list = ModelInfoManager.DataTableToList<IVVerificationListModel>(dataSet.Tables[0]);
			if (list != null && list.Count > 0)
			{
				return (from t in list
				orderby t.MCreateDate
				select t).ToList();
			}
			return list;
		}

		public static bool CheckIsCanEditOrDelete(MContext ctx, string billType, string pkID)
		{
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			StringBuilder stringBuilder = new StringBuilder();
			if (billType == "Invoice")
			{
				string strSql = "SELECT 1 FROM T_IV_Invoice WHERE MID=@PKID AND MBizDate < @ConversionData and MOrgID= @MOrgID and MIsDelete = 0 ";
				MySqlParameter[] cmdParms = new MySqlParameter[3]
				{
					new MySqlParameter("@PKID", MySqlDbType.VarChar, 36)
					{
						Value = pkID
					},
					new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
					{
						Value = ctx.MOrgID
					},
					new MySqlParameter("@ConversionData", MySqlDbType.DateTime)
					{
						Value = (object)ctx.MBeginDate
					}
				};
				if (!dynamicDbHelperMySQL.Exists(strSql, cmdParms))
				{
					stringBuilder.Append("SELECT 1 FROM T_IV_Invoice WHERE MID=@PKID AND MStatus=@MStatus and MOrgID= @MOrgID and MIsDelete = 0  Union ALL ");
				}
			}
			else if (billType == "Expense")
			{
				string strSql2 = "SELECT 1 FROM T_IV_Expense WHERE MID=@PKID AND MBizDate < @ConversionData and MOrgID= @MOrgID and MIsDelete = 0 ";
				MySqlParameter[] cmdParms2 = new MySqlParameter[3]
				{
					new MySqlParameter("@PKID", MySqlDbType.VarChar, 36)
					{
						Value = pkID
					},
					new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
					{
						Value = ctx.MOrgID
					},
					new MySqlParameter("@ConversionData", MySqlDbType.DateTime)
					{
						Value = (object)ctx.MBeginDate
					}
				};
				if (!dynamicDbHelperMySQL.Exists(strSql2, cmdParms2))
				{
					stringBuilder.Append("SELECT 1 FROM T_IV_Expense WHERE MID=@PKID AND MStatus=@MStatus and MOrgID= @MOrgID and MIsDelete = 0 Union ALL ");
				}
			}
			else if (billType == "Payment" || billType == "Receive" || billType == "Transfer")
			{
				stringBuilder.Append("SELECT 1 FROM T_IV_BankBillReconcileEntry WHERE MTargetBillType =@BillType AND  MTargetBillID=@PKID and MOrgID = @MOrgID and MIsDelete=0  Union ALL ");
			}
			stringBuilder.Append("select 1 from T_IV_Verification where ((MSourceBillType=@BillType and MSourceBillID=@PKID) or (MTargetBillType=@BillType and MTargetBillID=@PKID)) and MOrgID = @MOrgID and MIsDelete=0 ");
			MySqlParameter[] cmdParms3 = new MySqlParameter[4]
			{
				new MySqlParameter("@BillType", MySqlDbType.VarChar, 100)
				{
					Value = billType
				},
				new MySqlParameter("@PKID", MySqlDbType.VarChar, 36)
				{
					Value = pkID
				},
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
				{
					Value = ctx.MOrgID
				},
				new MySqlParameter("@MStatus", MySqlDbType.Int32)
				{
					Value = (object)Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment)
				}
			};
			bool flag = dynamicDbHelperMySQL.Exists(stringBuilder.ToString(), cmdParms3);
			bool result = true;
			if (flag)
			{
				result = false;
			}
			return result;
		}

		public static List<string> CheckIsCanEditOrDelete(MContext ctx, string billType, List<string> pkIDList)
		{
			List<string> list = new List<string>();
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			StringBuilder stringBuilder = new StringBuilder();
			List<MySqlParameter> list2 = new List<MySqlParameter>
			{
				new MySqlParameter("@BillType", billType),
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MStatus", Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment)),
				new MySqlParameter("@ConversionDate", ctx.MBeginDate)
			};
			int num = ctx.MInitBalanceOver ? 1 : 0;
			string tableNameByBillType = GetTableNameByBillType(billType);
			string inFilterQuery = GLUtility.GetInFilterQuery(pkIDList, ref list2, "M_ID");
			string text = $"(MStatus <=@MStatus AND MBizDate < @ConversionDate AND {num} = 0)";
			if (billType == "Payment" || billType == "Receive" || billType == "Transfer")
			{
				string value = "SELECT \n                        MID\n                    FROM\n                        " + tableNameByBillType + " t\n                    WHERE\n                        MID " + inFilterQuery + " AND MOrgID = @MOrgID\n                            AND MIsDelete = 0\n                            AND \n                            NOT EXISTS( SELECT \n                                1\n                            FROM\n                                T_IV_BankBillReconcileEntry ir\n                            WHERE\n                                ir.MTargetBillType = @BillType\n                                    AND ir.MTargetBillID " + inFilterQuery + "\n                                    AND ir.MOrgID = @MOrgID\n                                    AND ir.MIsDelete = 0\n                                    AND t.MID = ir.MTargetBillID)\n                            AND NOT EXISTS( SELECT \n                                1\n                            FROM\n                                T_IV_Verification iv\n                            WHERE\n                                ((iv.MSourceBillType = @BillType\n                                    AND iv.MSourceBillID " + inFilterQuery + ")\n                                    OR (iv.MTargetBillType = @BillType\n                                    AND iv.MTargetBillID " + inFilterQuery + "))\n                                    AND iv.MOrgID = @MOrgID\n                                    AND iv.MIsDelete = 0\n                                    AND ((t.MID = iv.MSourceBillID AND iv.MSourceBillType = @BillType) \r\n                                    OR (t.MID = iv.MTargetBillID AND iv.MTargetBillType=@BillType))\r\n                                    )";
				stringBuilder.Append(value);
			}
			else
			{
				string value2 = "\r\n                        SELECT \n                            MID\n                        FROM " + tableNameByBillType + "\n                        WHERE\n                        MID " + inFilterQuery + " AND MOrgID = @MOrgID AND MIsDelete = 0 AND (" + text + " OR(\n                        MBizDate >= @ConversionDate AND MStatus < @MStatus))";
				stringBuilder.Append(value2);
			}
			DataSet dataSet = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), list2.ToArray());
			if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
			{
				foreach (DataRow row in dataSet.Tables[0].Rows)
				{
					string item = row["MID"].ToString();
					list.Add(item);
				}
			}
			return list;
		}

		private static string GetTableNameByBillType(string billType)
		{
			string result = "";
			if (billType == "Invoice")
			{
				result = "T_IV_Invoice";
			}
			else if (billType == "Expense")
			{
				result = "T_IV_Expense";
			}
			else if (billType == "Payment")
			{
				result = "T_IV_Payment";
			}
			else if (billType == "Receive")
			{
				result = "T_IV_Receive";
			}
			else if (billType == "Transfer")
			{
				result = "T_IV_Transfer";
			}
			return result;
		}

		private static void CalculateBillVerificationAmount(MContext ctx, IVVerificationModel model)
		{
			List<CommandInfo> calculateBillVerificationAmountCmd = GetCalculateBillVerificationAmountCmd(ctx, model, false);
			if (calculateBillVerificationAmountCmd != null && calculateBillVerificationAmountCmd.Count > 0)
			{
				DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
				dynamicDbHelperMySQL.ExecuteSqlTran(calculateBillVerificationAmountCmd);
			}
		}

		public static List<CommandInfo> GetCalculateBillVerificationAmountCmd(MContext ctx, IVVerificationModel model, bool isFromDelete = false)
		{
			if (model == null || string.IsNullOrWhiteSpace(model.MID))
			{
				return null;
			}
			List<CommandInfo> list = new List<CommandInfo>();
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			string text = " select IFNULL(sum(MAmt),0) as MAmount,IFNULL(sum(MAmtFor),0) as MAmountFor from T_IV_Verification where MIsDelete=0 and MOrgID = @MOrgID and ((MSourceBillType =@BillType And  MSourceBillID =@BillID) OR (MTargetBillType=@BillType AND MTargetBillID=@BillID)) ";
			if (model.MSourceBillType == "PayRun")
			{
				text = $"select sum(MAmount) as MAmount, sum(MAmountFor) as MAmountFor from (\r\n                    {text} \r\n                        union all \r\n                    select IFNULL(v3.MAmount, 0) as MAmount, IFNULL(v3.MAmount, 0) as MAmountFor\r\n                        from T_IV_Verification v1\r\n                        join t_pa_salarytoivpay v2 on v2.MOrgID = v1.MOrgID and v2.MID = v1.MTargetBillID and v2.MIsDelete = 0\r\n                        join t_pa_salarytoivpayentry v3 on v3.MOrgID = v1.MOrgID and v3.MID = v2.MID and v3.MIsDelete = 0 and v3.MSalaryPaymentID = @BillID\r\n                        where v1.MIsDelete = 0 and v1.MOrgID = @MOrgID) u";
			}
			string updateVerificationAmountSql = GetUpdateVerificationAmountSql(ctx, model.MSourceBillType, model.MSourceBillID, text, isFromDelete);
			if (!string.IsNullOrWhiteSpace(updateVerificationAmountSql))
			{
				MySqlParameter[] para = new MySqlParameter[3]
				{
					new MySqlParameter("@BillType", model.MSourceBillType),
					new MySqlParameter("@BillID", model.MSourceBillID),
					new MySqlParameter("@MOrgID", ctx.MOrgID)
				};
				list.Add(new CommandInfo(updateVerificationAmountSql, para));
			}
			updateVerificationAmountSql = GetUpdateVerificationAmountSql(ctx, model.MTargetBillType, model.MTargetBillID, text, isFromDelete);
			if (!string.IsNullOrWhiteSpace(updateVerificationAmountSql))
			{
				MySqlParameter[] para2 = new MySqlParameter[3]
				{
					new MySqlParameter("@BillType", model.MTargetBillType),
					new MySqlParameter("@BillID", model.MTargetBillID),
					new MySqlParameter("@MOrgID", ctx.MOrgID)
				};
				list.Add(new CommandInfo(updateVerificationAmountSql, para2));
			}
			return list;
		}

		public static List<IVVerifiactionDocMapModel> GetVerificationList(MContext ctx, IVVerificationFilterModel filter)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(" SELECT * FROM t_iv_verification where MOrgID=@OrgID and MIsDelete=0 ");
			if (filter.StartDate != DateTime.MinValue)
			{
				stringBuilder.AppendLine(" and MCreateDate>=@StartDate ");
			}
			if (filter.EndDate != DateTime.MinValue)
			{
				stringBuilder.AppendLine(" and MCreateDate<@EndDate ");
			}
			if (!string.IsNullOrWhiteSpace(filter.MSourceBillType))
			{
				stringBuilder.AppendLine(" and MSourceBillType=@SourceBillType");
			}
			if (!string.IsNullOrWhiteSpace(filter.MTargetBillType))
			{
				stringBuilder.AppendLine(" and MTargetBillType=@TargetBillType");
			}
			string text = "";
			if (filter.BillIdList != null && filter.BillIdList.Count() > 0)
			{
				text = "'" + string.Join("','", filter.BillIdList) + "'";
				stringBuilder.AppendFormat(" and  (MSourceBillID in ({0}) or MTargetBillID in ({0}))", text);
			}
			MySqlParameter[] cmdParms = new MySqlParameter[6]
			{
				new MySqlParameter("@OrgID", ctx.MOrgID),
				new MySqlParameter("@StartDate", filter.StartDate),
				new MySqlParameter("@EndDate", filter.EndDate),
				new MySqlParameter("@MSourceBillType", filter.MSourceBillType),
				new MySqlParameter("@MTargetBillType", filter.MTargetBillType),
				new MySqlParameter("@BillIds", text)
			};
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), cmdParms);
			return ModelInfoManager.DataTableToList<IVVerifiactionDocMapModel>(dataSet.Tables[0]);
		}

		public static List<BDBankBasicModel> GetPaymentBankList(MContext ctx, string id)
		{
			string text = "select distinct t3.MItemID AS MBankID, t3.MBankNo, t3.MCyID, t4.MName AS MBankName from T_IV_Verification t1 \r\n                        INNER JOIN T_IV_Payment t2 ON ((t1.MSourceBillID=t2.MID AND MSourceBillType='Payment') OR (t1.MTargetBillID=t2.MID AND MTargetBillType='Payment')) \r\n\t\t\t\t\t\t\t\t                        AND t1.MOrgID=t2.MorgID AND t2.MIsDelete=0\r\n                        INNER JOIN T_BD_BankAccount t3 ON t3.MItemID=t2.MBankID AND t3.MOrgID=t2.MorgID AND t3.MIsDelete=0\r\n                        INNER JOIN T_BD_BankAccount_L t4 ON t4.MParentID=t3.MItemID AND t4.MLocaleID=@MLocaleID  AND t4.MOrgID=t3.MorgID AND t4.MIsDelete=0\r\n                        WHERE (MSourceBillID=@MID OR  MTargetBillID = @MID) AND t1.MOrgID=@MOrgID AND t1.MIsDelete=0\r\n                        ";
			MySqlParameter[] cmdParms = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MLocaleID", ctx.MLCID),
				new MySqlParameter("@MID", id)
			};
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet ds = dynamicDbHelperMySQL.Query(text.ToString(), cmdParms);
			return ModelInfoManager.DataTableToList<BDBankBasicModel>(ds);
		}

		private static string GetUpdateVerificationAmountSql(MContext ctx, string billType, string pkID, string calAmtSql, bool isFromDelete = false)
		{
			switch (billType)
			{
			case "Invoice":
				return $"UPDATE T_IV_Invoice cross join ({calAmtSql})aa\r\n                                        SET MVerificationAmt=CASE WHEN MType='Invoice_Sale_Red' OR MType='Invoice_Purchase_Red' THEN -aa.MAmountFor ELSE aa.MAmountFor END,\r\n                                        MVerifyAmtFor=CASE WHEN MType='Invoice_Sale_Red' OR MType='Invoice_Purchase_Red' THEN -aa.MAmountFor ELSE aa.MAmountFor END,\r\n                                        MVerifyAmt=CASE WHEN MType='Invoice_Sale_Red' OR MType='Invoice_Purchase_Red' THEN -aa.MAmount ELSE aa.MAmount END,\r\n                                        MStatus= CASE WHEN ((MType='Invoice_Sale_Red' OR MType='Invoice_Purchase_Red') AND MTaxTotalAmtFor=-aa.MAmountFor) OR MTaxTotalAmtFor=aa.MAmountFor THEN {Convert.ToInt32(IVInvoiceStatusEnum.Paid)} ELSE {Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment)} END\r\n                                        WHERE MID='{pkID}' and MOrgID = '{ctx.MOrgID}' and MIsDelete = 0 ";
			case "Payment":
				return $"UPDATE T_IV_Payment cross join ({calAmtSql})aa\r\n                                           SET MVerificationAmt=CASE WHEN MType='Pay_PurReturn' OR MType='Pay_OtherReturn' THEN -aa.MAmountFor ELSE aa.MAmountFor END,\r\n                                           MVerifyAmtFor=CASE WHEN MType='Pay_PurReturn' OR MType='Pay_OtherReturn' THEN -aa.MAmountFor ELSE aa.MAmountFor END,\r\n                                           MVerifyAmt=CASE WHEN MType='Pay_PurReturn' OR MType='Pay_OtherReturn' THEN -aa.MAmount ELSE aa.MAmount END   \r\n                                           WHERE MID='{pkID}'  and MOrgID = '{ctx.MOrgID}' and MIsDelete = 0 ";
			case "Receive":
				return $"UPDATE T_IV_Receive cross join ({calAmtSql})aa\r\n                                        SET MVerificationAmt=CASE WHEN MType='Receive_SaleReturn' OR MType='Receive_OtherReturn' THEN -aa.MAmountFor ELSE aa.MAmountFor END ,\r\n                                        MVerifyAmtFor=CASE WHEN MType='Receive_SaleReturn' OR MType='Receive_OtherReturn' THEN  -aa.MAmountFor ELSE aa.MAmountFor END ,\r\n                                        MVerifyAmt=CASE WHEN MType='Receive_SaleReturn' OR MType='Receive_OtherReturn' THEN  -aa.MAmount ELSE  aa.MAmount END \r\n                                        WHERE MID='{pkID}'  and MOrgID = '{ctx.MOrgID}' and MIsDelete = 0 ";
			case "Expense":
				return $"Update T_IV_Expense cross join ({calAmtSql})aa\r\n                                            Set MVerificationAmt =aa.MAmountFor,\r\n                                           MVerifyAmtFor =aa.MAmountFor,\r\n                                           MVerifyAmt =aa.MAmount,\r\n                                           MStatus= CASE WHEN MTaxTotalAmtFor=aa.MAmountFor THEN {Convert.ToInt32(IVInvoiceStatusEnum.Paid)} ELSE {Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment)} END\r\n                                           Where MID='{pkID}' and MOrgID = '{ctx.MOrgID}' and MIsDelete = 0  ";
			case "PayRun":
				return string.Format(" Update T_PA_SalaryPayment t1 cross join ({0}) aa\r\n\t                                            Set t1.MVerificationAmt =aa.MAmountFor,t1.MVerifyAmtFor =aa.MAmountFor,t1.MVerifyAmt =aa.MAmountFor,t1.MStatus= CASE WHEN t1.MNetSalary=aa.MAmountFor and {4} THEN {1} ELSE {2} END\r\n                                            Where t1.MID='{3}'  and t1.MOrgID = '{5}' and t1.MIsDelete = 0;\r\n                                            Update T_PA_PayRun t1 \n                                                    join\n                                                T_PA_SalaryPayment t2 ON t2.MOrgID = t1.MOrgID\n                                                    and t2.MRunID = t1.MID\n                                                    and t2.MID = '{3}'\n                                                    and t2.MIsDelete = 0\n                                                    left join\n                                                (select \n                                                    MRunID, Count(*) as UnPaidCount\n                                                from\n                                                    T_PA_SalaryPayment\n                                                where\n                                                    MOrgID = '{5}'\n                                                    and MID <> '{3}'\n                                                    and MStatus < 4\n                                                    and MIsDelete = 0\n                                                group by MRunID) t3 ON t3.MRunID = t1.MID \n                                            cross join ({0})aa\n                                            set \n                                                t1.MStatus = CASE WHEN t2.MNetSalary=aa.MAmountFor and {4} THEN CASE WHEN ifnull(t3.UnPaidCount, 0) = 0 THEN {1} else t1.MStatus END\n                                                    ELSE CASE WHEN ifnull(t3.UnPaidCount, 0) = 0 THEN {2} else t1.MStatus END \n                                                END\n                                            where t1.MOrgID = '{5}' and t1.MIsDelete = 0;", calAmtSql, Convert.ToInt32(PASalaryPaymentStatusEnum.Paid), Convert.ToInt32(PASalaryPaymentStatusEnum.WaitingPayment), pkID, isFromDelete ? "false" : "true", ctx.MOrgID);
			default:
				return string.Empty;
			}
		}

		private static string GetVerificationBizType(string pkId, MContext ctx)
		{
			string sql = "SELECT MType FROM T_IV_Invoice WHERE  MID=@MID and MOrgID = @MOrgID and MIsDelete = 0 ";
			MySqlParameter[] array = new MySqlParameter[2]
			{
				new MySqlParameter("@MID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			array[0].Value = pkId;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			object single = dynamicDbHelperMySQL.GetSingle(sql, array);
			return (single == null || single == DBNull.Value) ? "" : single.ToString();
		}

		private static CommandInfo GetVerificationDeleteSql(MContext ctx, IVVerificationModel model)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("UPDATE T_IV_Verification SET MIsDelete=1 where MID = @MID and MOrgID = @MOrgID and MIsDelete = 0 ");
			MySqlParameter[] array = new MySqlParameter[2]
			{
				new MySqlParameter("@MID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			array[0].Value = model.MID;
			return new CommandInfo(stringBuilder.ToString(), array);
		}

		public static List<IVVerificationListModel> GetVerificationList(MContext ctx, List<string> billIdList)
		{
			List<IVVerificationListModel> result = new List<IVVerificationListModel>();
			if (billIdList == null || billIdList.Count() == 0)
			{
				return result;
			}
			string str = "select MSourceBillID , MTargetBillID from t_iv_verification where MOrgID=@MOrgID and MIsDelete=0 ";
			List<MySqlParameter> list = new List<MySqlParameter>();
			string inSql = BDRepository.GetInSql(ctx, billIdList, out list);
			str += string.Format(" and (MSourceBillID in {0} or MTargetBillID in {0}) ", inSql, list);
			list.Add(new MySqlParameter
			{
				ParameterName = "@MOrgID",
				Value = ctx.MOrgID
			});
			return ModelInfoManager.GetDataModelBySql<IVVerificationListModel>(ctx, str, list.ToArray());
		}

		public static List<IVVerificationModel> GetVerificationRecordList(MContext ctx, List<string> billIdList)
		{
			List<IVVerificationModel> result = new List<IVVerificationModel>();
			if (billIdList == null || billIdList.Count() == 0)
			{
				return result;
			}
			string str = "select MID , MSourceBillID , MTargetBillID , MSourceBillType , MTargetBillType from t_iv_verification where MOrgID=@MOrgID and MIsDelete=0 ";
			List<MySqlParameter> list = new List<MySqlParameter>();
			string inSql = BDRepository.GetInSql(ctx, billIdList, out list);
			str += string.Format(" and (MSourceBillID in {0} or MTargetBillID in {0}) ", inSql, list);
			list.Add(new MySqlParameter
			{
				ParameterName = "@MOrgID",
				Value = ctx.MOrgID
			});
			return ModelInfoManager.GetDataModelBySql<IVVerificationModel>(ctx, str, list.ToArray());
		}

		public static List<IVVerificationInforModel> GetCustomerWaitForVerificationInfor(MContext ctx, string contactID, string currencyID, string bizBillType, string bizType, string contactType)
		{
			List<IVVerificationInforModel> list = new List<IVVerificationInforModel>();
			switch (bizBillType)
			{
			case "Invoice":
				list.AddRange(GetInvoiceWaitForVerif(ctx, contactID, currencyID, bizType));
				break;
			case "Payment":
				list.AddRange(GetPaymentWaitForVerif(ctx, contactID, currencyID, bizType, contactType, bizBillType));
				break;
			case "Receive":
				list.AddRange(GetReceiveWaitForVerif(ctx, contactID, currencyID, bizType, contactType));
				break;
			case "Expense":
				list.AddRange(GetExpenseWaitForVerif(ctx, contactID, currencyID, bizType));
				break;
			case "PayRun":
				list.AddRange(GetSalaryPaymentWaitForVerif(ctx, contactID, currencyID, bizType));
				break;
			}
			return list;
		}

		private static List<IVVerificationInforModel> GetReceiveWaitForVerif(MContext ctx, string contactID, string currencyID, string bizType, string contactType)
		{
			List<IVVerificationInforModel> list = new List<IVVerificationInforModel>();
			switch (bizType)
			{
			case "Receive_Sale":
				list.AddRange(GetSaleReceiveWaitForVerif(ctx, contactID, currencyID));
				break;
			case "Receive_Prepare":
				list.AddRange(GetPreReceiveWaitForVerif(ctx, contactID, currencyID));
				break;
			case "Receive_Other":
				list.AddRange(GetOtherReceiveWaitForVerif(ctx, contactID, currencyID));
				break;
			case "Receive_SaleReturn":
				list.AddRange(GetSaleReturnReceiveWaitForVerif(ctx, contactID, currencyID));
				break;
			case "Receive_OtherReturn":
				list.AddRange(GetOtherReturnReceiveWaitForVerif(ctx, contactID, currencyID, contactType));
				break;
			}
			return list;
		}

		private static List<IVVerificationInforModel> GetOtherReturnReceiveWaitForVerif(MContext ctx, string contactID, string currencyID, string contactType)
		{
			List<IVVerificationInforModel> list = new List<IVVerificationInforModel>();
			if (contactType == "Employees")
			{
				GetPaymentWaitForVerif(ctx, list, contactID, currencyID, "", contactType);
			}
			return list;
		}

		private static List<IVVerificationInforModel> GetSaleReturnReceiveWaitForVerif(MContext ctx, string contactID, string currencyID)
		{
			List<IVVerificationInforModel> list = new List<IVVerificationInforModel>();
			GetInvoiceWaitForVerif(ctx, list, contactID, currencyID, "Invoice_Purchase_Red");
			GetPaymentWaitForVerif(ctx, list, contactID, currencyID, "Pay_Purchase", "");
			return list;
		}

		private static List<IVVerificationInforModel> GetOtherReceiveWaitForVerif(MContext ctx, string contactID, string currencyID)
		{
			return new List<IVVerificationInforModel>();
		}

		private static List<IVVerificationInforModel> GetPreReceiveWaitForVerif(MContext ctx, string contactID, string currencyID)
		{
			return new List<IVVerificationInforModel>();
		}

		private static List<IVVerificationInforModel> GetSaleReceiveWaitForVerif(MContext ctx, string contactID, string currencyID)
		{
			List<IVVerificationInforModel> list = new List<IVVerificationInforModel>();
			GetInvoiceWaitForVerif(ctx, list, contactID, currencyID, "Invoice_Sale");
			GetPaymentWaitForVerif(ctx, list, contactID, currencyID, "Pay_PurReturn", "");
			return list;
		}

		private static void GetReceiveWaitForVerif(MContext ctx, List<IVVerificationInforModel> lst, string contactID, string currencyID, string bizType, string contactType = "")
		{
			decimal receiveUnVerificationAmount = GetReceiveUnVerificationAmount(ctx, contactID, currencyID, bizType);
			if (receiveUnVerificationAmount != decimal.Zero)
			{
				IVVerificationInforModel iVVerificationInforModel = new IVVerificationInforModel();
				iVVerificationInforModel.Amount = receiveUnVerificationAmount;
				iVVerificationInforModel.MBizBillType = "Receive";
				iVVerificationInforModel.MBizType = bizType;
				iVVerificationInforModel.MContactID = contactID;
				iVVerificationInforModel.MCurrencyID = currencyID;
				iVVerificationInforModel.MContactType = contactType;
				lst.Add(iVVerificationInforModel);
			}
		}

		private static decimal GetReceiveUnVerificationAmount(MContext ctx, string contactID, string currencyID, string bizType)
		{
			decimal result = default(decimal);
			string str = " select sum(abs(MTaxTotalAmtFor)-abs(IFNULL(MVerificationAmt,0))) as MAmount  from T_IV_Receive Where abs(IFNULL(MVerificationAmt,0)) < abs(MTaxTotalAmtFor) ";
			str = ((!string.IsNullOrWhiteSpace(bizType)) ? (str + " And MIsActive=1 AND MIsDelete=0 And MOrgID=@MOrgID And MContactID= @MContactID And MType=@MType  And MCyID=@MCyID ") : (str + " And MIsActive=1 AND MIsDelete=0 And MOrgID=@MOrgID And MContactID= @MContactID  And MCyID=@MCyID "));
			MySqlParameter[] array = new MySqlParameter[4]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MContactID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MCyID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MType", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = contactID;
			array[2].Value = currencyID;
			array[3].Value = bizType;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			object single = dynamicDbHelperMySQL.GetSingle(str, array);
			if (single != null)
			{
				decimal.TryParse(single.ToString(), out result);
			}
			return result;
		}

		private static List<IVVerificationInforModel> GetPaymentWaitForVerif(MContext ctx, string contactID, string currencyID, string bizType, string contactType, string bizBillType = null)
		{
			List<IVVerificationInforModel> list = new List<IVVerificationInforModel>();
			switch (bizType)
			{
			case "Pay_Purchase":
				list.AddRange(GetPurPaymentWaitForVerif(ctx, contactID, currencyID));
				break;
			case "Pay_Prepare":
				list.AddRange(GetPrePaymentWaitForVerif(ctx, contactID, currencyID));
				break;
			case "Pay_Other":
				list.AddRange(GetOtherPaymentWaitForVerif(ctx, contactID, currencyID, contactType, bizBillType));
				break;
			case "Pay_PurReturn":
				list.AddRange(GetPurReturnPaymentWaitForVerif(ctx, contactID, currencyID));
				break;
			case "Pay_OtherReturn":
				list.AddRange(GetOtherReturnPaymentWaitForVerif(ctx, contactID, currencyID));
				break;
			}
			return list;
		}

		private static List<IVVerificationInforModel> GetOtherReturnPaymentWaitForVerif(MContext ctx, string contactID, string currencyID)
		{
			return new List<IVVerificationInforModel>();
		}

		private static List<IVVerificationInforModel> GetPurReturnPaymentWaitForVerif(MContext ctx, string contactID, string currencyID)
		{
			List<IVVerificationInforModel> list = new List<IVVerificationInforModel>();
			GetInvoiceWaitForVerif(ctx, list, contactID, currencyID, "Invoice_Sale_Red");
			GetReceiveWaitForVerif(ctx, list, contactID, currencyID, "Receive_Sale", "");
			return list;
		}

		private static List<IVVerificationInforModel> GetPurPaymentWaitForVerif(MContext ctx, string contactID, string currencyID)
		{
			List<IVVerificationInforModel> list = new List<IVVerificationInforModel>();
			GetInvoiceWaitForVerif(ctx, list, contactID, currencyID, "Invoice_Purchase");
			GetReceiveWaitForVerif(ctx, list, contactID, currencyID, "Receive_SaleReturn", "");
			return list;
		}

		private static List<IVVerificationInforModel> GetPrePaymentWaitForVerif(MContext ctx, string contactID, string currencyID)
		{
			return new List<IVVerificationInforModel>();
		}

		private static List<IVVerificationInforModel> GetOtherPaymentWaitForVerif(MContext ctx, string contactID, string currencyID, string contactType, string bizBillType)
		{
			List<IVVerificationInforModel> list = new List<IVVerificationInforModel>();
			if (contactType == "Employees")
			{
				GetExpenseWaitForVerif(ctx, list, contactID, currencyID, "Expense_Claims");
				GetSalaryPaymentWaitForVerif(ctx, list, contactID, currencyID, "Pay_Salary");
				GetReceiveWaitForVerif(ctx, list, contactID, currencyID, "Receive_OtherReturn", contactType);
			}
			return list;
		}

		private static void GetPaymentWaitForVerif(MContext ctx, List<IVVerificationInforModel> lst, string contactID, string currencyID, string bizType, string contactType = "")
		{
			decimal paymentUnVerificationAmount = GetPaymentUnVerificationAmount(ctx, contactID, currencyID, bizType, contactType);
			if (paymentUnVerificationAmount != decimal.Zero)
			{
				IVVerificationInforModel iVVerificationInforModel = new IVVerificationInforModel();
				iVVerificationInforModel.Amount = paymentUnVerificationAmount;
				iVVerificationInforModel.MBizBillType = "Payment";
				iVVerificationInforModel.MBizType = bizType;
				iVVerificationInforModel.MContactID = contactID;
				iVVerificationInforModel.MCurrencyID = currencyID;
				iVVerificationInforModel.MContactType = contactType;
				lst.Add(iVVerificationInforModel);
			}
		}

		private static decimal GetPaymentUnVerificationAmount(MContext ctx, string contactID, string currencyID, string bizType, string contactType = "")
		{
			contactID = ((contactID == null) ? "" : contactID);
			decimal result = default(decimal);
			string str = " select sum(abs(MTaxTotalAmtFor)-abs(IFNULL(MVerificationAmt,0))) as MAmount  from T_IV_Payment Where abs(IFNULL(MVerificationAmt,0)) < abs(MTaxTotalAmtFor) ";
			str += " And MIsActive=1 AND MIsDelete=0 AND MOrgID=@MOrgID And IFNULL(MContactID,'')= @MContactID  And MCyID=@MCyID ";
			if (!string.IsNullOrWhiteSpace(bizType))
			{
				str += "  And MType=@MType ";
			}
			if (!string.IsNullOrEmpty(contactType))
			{
				str += "  And MContactType=@MContactType ";
			}
			MySqlParameter[] array = new MySqlParameter[5]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MContactID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MCyID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MType", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MContactType", MySqlDbType.VarChar, 50)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = contactID;
			array[2].Value = currencyID;
			array[3].Value = bizType;
			array[4].Value = contactType;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			object single = dynamicDbHelperMySQL.GetSingle(str, array);
			if (single != null)
			{
				decimal.TryParse(single.ToString(), out result);
			}
			return result;
		}

		private static List<IVVerificationInforModel> GetExpenseWaitForVerif(MContext ctx, string contactID, string currencyID, string bizType)
		{
			List<IVVerificationInforModel> list = new List<IVVerificationInforModel>();
			GetPaymentWaitForVerif(ctx, list, contactID, currencyID, "", "Employees");
			return list;
		}

		private static void GetExpenseWaitForVerif(MContext ctx, List<IVVerificationInforModel> lst, string contactID, string currencyID, string bizType)
		{
			decimal expenseUnVerificationAmount = GetExpenseUnVerificationAmount(ctx, contactID, currencyID, bizType);
			if (expenseUnVerificationAmount != decimal.Zero)
			{
				IVVerificationInforModel iVVerificationInforModel = new IVVerificationInforModel();
				iVVerificationInforModel.Amount = expenseUnVerificationAmount;
				iVVerificationInforModel.MBizBillType = "Expense";
				iVVerificationInforModel.MBizType = bizType;
				iVVerificationInforModel.MContactID = contactID;
				iVVerificationInforModel.MCurrencyID = currencyID;
				lst.Add(iVVerificationInforModel);
			}
		}

		private static decimal GetExpenseUnVerificationAmount(MContext ctx, string contactID, string currencyID, string bizType)
		{
			decimal result = default(decimal);
			contactID = ((contactID == null) ? "" : contactID);
			string str = " select sum(abs(MTaxTotalAmtFor)-abs(IFNULL(MVerificationAmt,0))) as MAmount  from T_IV_Expense Where abs(IFNULL(MVerificationAmt,0)) < abs(MTaxTotalAmtFor) AND  MStatus=@MStatus  ";
			str = ((!string.IsNullOrWhiteSpace(bizType)) ? (str + "  And MIsActive=1 AND MIsDelete=0 And MOrgID=@MOrgID And IFNULL(MContactID,'')= @MContactID And MType=@MType  And MCyID=@MCyID ") : (str + " And MIsActive=1 AND MIsDelete=0 AND MOrgID=@MOrgID And IFNULL(MContactID,'')= @MContactID  And MCyID=@MCyID "));
			MySqlParameter[] array = new MySqlParameter[5]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MContactID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MCyID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MType", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MStatus", MySqlDbType.Int32)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = contactID;
			array[2].Value = currencyID;
			array[3].Value = bizType;
			array[4].Value = Convert.ToInt32(IVExpenseStatusEnum.WaitingPayment);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			object single = dynamicDbHelperMySQL.GetSingle(str, array);
			if (single != null)
			{
				decimal.TryParse(single.ToString(), out result);
			}
			return result;
		}

		private static List<IVVerificationInforModel> GetSalaryPaymentWaitForVerif(MContext ctx, string contactID, string currencyID, string bizType)
		{
			List<IVVerificationInforModel> list = new List<IVVerificationInforModel>();
			GetPaymentWaitForVerif(ctx, list, contactID, currencyID, "Pay_Other", "Employees");
			return list;
		}

		private static void GetSalaryPaymentWaitForVerif(MContext ctx, List<IVVerificationInforModel> lst, string contactID, string currencyID, string bizType)
		{
			decimal salaryPaymentUnVerificationAmount = GetSalaryPaymentUnVerificationAmount(ctx, contactID);
			if (salaryPaymentUnVerificationAmount != decimal.Zero)
			{
				IVVerificationInforModel iVVerificationInforModel = new IVVerificationInforModel();
				iVVerificationInforModel.Amount = salaryPaymentUnVerificationAmount;
				iVVerificationInforModel.MBizBillType = "PayRun";
				iVVerificationInforModel.MBizType = bizType;
				iVVerificationInforModel.MContactID = contactID;
				iVVerificationInforModel.MCurrencyID = currencyID;
				lst.Add(iVVerificationInforModel);
			}
		}

		private static decimal GetSalaryPaymentUnVerificationAmount(MContext ctx, string contactID)
		{
			decimal result = default(decimal);
			string sql = " select sum(abs(a.MNetSalary)-abs(IFNULL(a.MVerificationAmt,0))) as MAmount  \r\n                        from T_PA_SalaryPayment a \r\n                        left join T_PA_PayRun b on a.MRunID=b.MID \r\n                        Where abs(IFNULL(a.MVerificationAmt,0)) < abs(a.MNetSalary) AND  a.MStatus=@MStatus  \r\n                        And a.MIsActive=1 AND a.MIsDelete=0 AND b.MOrgID=@MOrgID And a.MEmployeeID= @MEmployeeID ";
			MySqlParameter[] array = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MEmployeeID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MStatus", MySqlDbType.Int32)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = contactID;
			array[2].Value = Convert.ToInt32(PASalaryPaymentStatusEnum.WaitingPayment);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			object single = dynamicDbHelperMySQL.GetSingle(sql, array);
			if (single != null)
			{
				decimal.TryParse(single.ToString(), out result);
			}
			return result;
		}

		private static List<IVVerificationInforModel> GetInvoiceWaitForVerif(MContext ctx, string contactID, string currencyID, string bizType)
		{
			List<IVVerificationInforModel> list = new List<IVVerificationInforModel>();
			switch (bizType)
			{
			case "Invoice_Purchase":
				list.AddRange(GetPurInvoiceWaitForVerif(ctx, contactID, currencyID));
				break;
			case "Invoice_Purchase_Red":
				list.AddRange(GetPurInvoiceWaitForVerif_Red(ctx, contactID, currencyID));
				break;
			case "Invoice_Sale":
				list.AddRange(GetSaleInvoiceWaitForVerif(ctx, contactID, currencyID));
				break;
			case "Invoice_Sale_Red":
				list.AddRange(GetSaleInvoiceWaitForVerif_Red(ctx, contactID, currencyID));
				break;
			}
			return list;
		}

		private static List<IVVerificationInforModel> GetSaleInvoiceWaitForVerif(MContext ctx, string contactID, string currencyID)
		{
			List<IVVerificationInforModel> list = new List<IVVerificationInforModel>();
			GetReceiveWaitForVerif(ctx, list, contactID, currencyID, "Receive_Sale", "");
			GetInvoiceWaitForVerif(ctx, list, contactID, currencyID, "Invoice_Sale_Red");
			GetInvoiceWaitForVerif(ctx, list, contactID, currencyID, "Invoice_Purchase");
			return list;
		}

		private static List<IVVerificationInforModel> GetSaleInvoiceWaitForVerif_Red(MContext ctx, string contactID, string currencyID)
		{
			List<IVVerificationInforModel> list = new List<IVVerificationInforModel>();
			GetPaymentWaitForVerif(ctx, list, contactID, currencyID, "Pay_PurReturn", "");
			GetInvoiceWaitForVerif(ctx, list, contactID, currencyID, "Invoice_Sale");
			return list;
		}

		private static List<IVVerificationInforModel> GetPurInvoiceWaitForVerif(MContext ctx, string contactID, string currencyID)
		{
			List<IVVerificationInforModel> list = new List<IVVerificationInforModel>();
			GetPaymentWaitForVerif(ctx, list, contactID, currencyID, "Pay_Purchase", "");
			GetInvoiceWaitForVerif(ctx, list, contactID, currencyID, "Invoice_Purchase_Red");
			GetInvoiceWaitForVerif(ctx, list, contactID, currencyID, "Invoice_Sale");
			return list;
		}

		private static List<IVVerificationInforModel> GetPurInvoiceWaitForVerif_Red(MContext ctx, string contactID, string currencyID)
		{
			List<IVVerificationInforModel> list = new List<IVVerificationInforModel>();
			GetReceiveWaitForVerif(ctx, list, contactID, currencyID, "Receive_SaleReturn", "");
			GetInvoiceWaitForVerif(ctx, list, contactID, currencyID, "Invoice_Purchase");
			return list;
		}

		private static void GetInvoiceWaitForVerif(MContext ctx, List<IVVerificationInforModel> lst, string contactID, string currencyID, string bizType)
		{
			decimal invoiceUnVerificationAmount = GetInvoiceUnVerificationAmount(ctx, contactID, currencyID, bizType);
			if (invoiceUnVerificationAmount != decimal.Zero)
			{
				IVVerificationInforModel iVVerificationInforModel = new IVVerificationInforModel();
				iVVerificationInforModel.Amount = invoiceUnVerificationAmount;
				iVVerificationInforModel.MBizBillType = "Invoice";
				iVVerificationInforModel.MBizType = bizType;
				iVVerificationInforModel.MContactID = contactID;
				iVVerificationInforModel.MCurrencyID = currencyID;
				lst.Add(iVVerificationInforModel);
			}
		}

		private static decimal GetInvoiceUnVerificationAmount(MContext ctx, string contactID, string currencyID, string bizType)
		{
			decimal result = default(decimal);
			string str = " select sum(ABS(MTaxTotalAmtFor)-ABS(IFNULL(MVerificationAmt,0))) as MAmount  from T_IV_Invoice Where MStatus=@MStatus ";
			str = ((!string.IsNullOrWhiteSpace(bizType)) ? (str + " And MIsActive=1 AND MIsDelete=0 And MOrgID=@MOrgID And MContactID= @MContactID And MType=@MType  And MCyID=@MCyID ") : (str + " And MIsActive=1 AND MIsDelete=0 And MOrgID=@MOrgID And MContactID= @MContactID And MCyID=@MCyID "));
			MySqlParameter[] array = new MySqlParameter[5]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MContactID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MCyID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MType", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MStatus", MySqlDbType.Int32)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = contactID;
			array[2].Value = currencyID;
			array[3].Value = bizType;
			array[4].Value = Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			object single = dynamicDbHelperMySQL.GetSingle(str, array);
			if (single != null)
			{
				decimal.TryParse(single.ToString(), out result);
			}
			return result;
		}

		private static List<IVVerificationListModel> GetReceiveVerificationList(MContext ctx, IVVerificationListFilterModel filter)
		{
			List<IVVerificationListModel> list = null;
			StringBuilder stringBuilder = new StringBuilder();
			if (filter.MContactType == "Employees")
			{
				stringBuilder.AppendLine("Select 'Receive' as MBizBillType,a.MType as MBizType,");
				stringBuilder.AppendLine("a.MID as MBillID,'' as MBillNo,");
				stringBuilder.AppendLine("a.MBizDate,a.MContactID,F_GetUserName(MFirstName,MLastName) as MContactName,a.MCyID as MCurrencyID,");
				stringBuilder.AppendLine("c.MName as MCurrencyName,a.MExchangeRate as MRate,a.MReference,");
				stringBuilder.AppendLine("a.MTaxTotalAmt as MAmountTotal,a.MTaxTotalAmtFor as MAmountTotalFor,");
				stringBuilder.AppendLine("abs(a.MVerifyAmt) as MHaveVerificationAmt,abs(a.MVerifyAmtFor)  as MHaveVerificationAmtFor,");
				stringBuilder.AppendLine("ABS(a.MTaxTotalAmt)-abs(a.MVerifyAmt) as MNoVerificationAmt,ABS(a.MTaxTotalAmtFor)-abs(a.MVerifyAmtFor) as MNoVerificationAmtFor");
				stringBuilder.AppendLine("From T_IV_Receive a ");
				stringBuilder.AppendLine("INNER Join T_BD_Employees_L b On a.MContactID=b.MParentID and b.MLocaleID=@MLocaleID  and b.MIsDelete=0 ");
				stringBuilder.AppendLine("Left Join T_Bas_Currency_L c On a.MCyID=c.MParentID and c.MLocaleID=@MLocaleID  and c.MIsDelete=0 ");
				stringBuilder.AppendLine("Where a.MIsDelete=0 AND a.MOrgID=@MOrgID and ABS(a.MTaxTotalAmtFor-a.MVerifyAmtFor)>0 ");
			}
			else
			{
				stringBuilder.AppendLine("Select 'Receive' as MBizBillType,a.MType as MBizType,");
				stringBuilder.AppendLine("a.MID as MBillID,'' as MBillNo,");
				stringBuilder.AppendFormat("a.MBizDate,a.MContactID,convert(AES_DECRYPT(b.MName,'{0}') using utf8) as MContactName,a.MCyID as MCurrencyID,", "JieNor-001");
				stringBuilder.AppendLine("c.MName as MCurrencyName,a.MExchangeRate as MRate,a.MReference,");
				stringBuilder.AppendLine("a.MTaxTotalAmt as MAmountTotal,a.MTaxTotalAmtFor as MAmountTotalFor,");
				stringBuilder.AppendLine("abs(a.MVerifyAmt) as MHaveVerificationAmt,abs(a.MVerifyAmtFor)  as MHaveVerificationAmtFor,");
				stringBuilder.AppendLine("ABS(a.MTaxTotalAmt)-abs(a.MVerifyAmt) as MNoVerificationAmt,ABS(a.MTaxTotalAmtFor)-abs(a.MVerifyAmtFor) as MNoVerificationAmtFor");
				stringBuilder.AppendLine("From T_IV_Receive a ");
				stringBuilder.AppendLine("INNER Join T_BD_Contacts_l b On a.MContactID=b.MParentID and b.MLocaleID=@MLocaleID  and b.MIsDelete=0 ");
				stringBuilder.AppendLine("Left Join T_Bas_Currency_L c On a.MCyID=c.MParentID and c.MLocaleID=@MLocaleID  and c.MIsDelete=0 ");
				stringBuilder.AppendLine("Where a.MIsDelete=0 AND a.MOrgID=@MOrgID and ABS(a.MTaxTotalAmtFor-a.MVerifyAmtFor)>0 ");
			}
			if (!string.IsNullOrEmpty(filter.MContactType))
			{
				stringBuilder.AppendLine(" AND a.MContactType=@MContactType");
			}
			GetFilterString(filter, stringBuilder);
			MySqlParameter[] array = new MySqlParameter[10]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MBizType", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MContactID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MCurrencyID", MySqlDbType.VarChar, 6),
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MDueDate", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MBillID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MKeyword", MySqlDbType.VarChar, 300),
				new MySqlParameter("@MAmount", MySqlDbType.Decimal),
				new MySqlParameter("@MContactType", MySqlDbType.VarChar, 50)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = filter.MBizType;
			array[2].Value = filter.MContactID;
			array[3].Value = filter.MCurrencyID;
			array[4].Value = ctx.MLCID;
			array[5].Value = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
			array[6].Value = filter.MBillID;
			array[7].Value = filter.MKeyword;
			array[8].Value = filter.MAmount;
			array[9].Value = filter.MContactType;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array);
			list = ModelInfoManager.DataTableToList<IVVerificationListModel>(dataSet.Tables[0]);
			GetVerifData(ctx, filter, list);
			return list;
		}

		private static List<IVVerificationListModel> GetPaymentVerificationList(MContext ctx, IVVerificationListFilterModel filter)
		{
			List<IVVerificationListModel> list = null;
			StringBuilder stringBuilder = new StringBuilder();
			if (filter.MContactType == "Employees")
			{
				stringBuilder.AppendLine("Select 'Payment' as MBizBillType,a.MType as MBizType,");
				stringBuilder.AppendLine("a.MID as MBillID,'' as MBillNo,");
				stringBuilder.AppendFormat("a.MBizDate,a.MContactID,{0} as MContactName,a.MCyID as MCurrencyID,", (ctx.MLCID == "0x0009") ? string.Format("concat(MFirstName,' ',MLastName)") : string.Format("concat(MLastName,MFirstName)"));
				stringBuilder.AppendLine("c.MName as MCurrencyName,a.MExchangeRate as MRate,a.MReference,");
				stringBuilder.AppendLine("a.MTaxTotalAmt as MAmountTotal,a.MTaxTotalAmtFor as MAmountTotalFor,");
				stringBuilder.AppendLine("abs(a.MVerifyAmt) as MHaveVerificationAmt,abs(a.MVerifyAmtFor)  as MHaveVerificationAmtFor,");
				stringBuilder.AppendLine("ABS(a.MTaxTotalAmt)-abs(a.MVerifyAmt) as MNoVerificationAmt,ABS(a.MTaxTotalAmtFor)-abs(a.MVerifyAmtFor) as MNoVerificationAmtFor");
				stringBuilder.AppendLine("From T_IV_Payment a ");
				stringBuilder.AppendLine("INNER Join T_BD_Employees_L b On a.MContactID=b.MParentID and b.MLocaleID=@MLocaleID  and b.MIsDelete=0 ");
				stringBuilder.AppendLine("Left Join T_Bas_Currency_L c On a.MCyID=c.MParentID and c.MLocaleID=@MLocaleID  and c.MIsDelete=0 ");
				stringBuilder.AppendLine("Where  a.MIsDelete=0 AND a.MOrgID=@MOrgID and ABS(a.MTaxTotalAmtFor-a.MVerifyAmtFor)>0 ");
			}
			else
			{
				stringBuilder.AppendLine("Select 'Payment' as MBizBillType,a.MType as MBizType,");
				stringBuilder.AppendLine("a.MID as MBillID,'' as MBillNo,");
				stringBuilder.AppendFormat("a.MBizDate,a.MContactID,convert(AES_DECRYPT(b.MName,'{0}') using utf8) as MContactName,a.MCyID as MCurrencyID,", "JieNor-001");
				stringBuilder.AppendLine("c.MName as MCurrencyName,a.MExchangeRate as MRate,a.MReference,");
				stringBuilder.AppendLine("a.MTaxTotalAmt as MAmountTotal,a.MTaxTotalAmtFor as MAmountTotalFor,");
				stringBuilder.AppendLine("abs(a.MVerifyAmt) as MHaveVerificationAmt,abs(a.MVerifyAmtFor)  as MHaveVerificationAmtFor,");
				stringBuilder.AppendLine("ABS(a.MTaxTotalAmt)-abs(a.MVerifyAmt) as MNoVerificationAmt,ABS(a.MTaxTotalAmtFor)-abs(a.MVerifyAmtFor) as MNoVerificationAmtFor");
				stringBuilder.AppendLine("From T_IV_Payment a ");
				stringBuilder.AppendLine("INNER Join T_BD_Contacts_l b On a.MContactID=b.MParentID and b.MLocaleID=@MLocaleID   and b.MIsDelete=0 ");
				stringBuilder.AppendLine("Left Join T_Bas_Currency_L c On a.MCyID=c.MParentID and c.MLocaleID=@MLocaleID  and c.MIsDelete=0 ");
				stringBuilder.AppendLine("Where  a.MIsDelete=0 AND a.MOrgID=@MOrgID and ABS(a.MTaxTotalAmtFor-a.MVerifyAmtFor)>0 ");
			}
			if (!string.IsNullOrEmpty(filter.MContactType))
			{
				stringBuilder.AppendLine(" AND a.MContactType=@MContactType");
			}
			GetFilterString(filter, stringBuilder);
			MySqlParameter[] array = new MySqlParameter[10]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MBizType", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MContactID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MCurrencyID", MySqlDbType.VarChar, 6),
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MDueDate", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MBillID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MKeyword", MySqlDbType.VarChar, 300),
				new MySqlParameter("@MAmount", MySqlDbType.Decimal),
				new MySqlParameter("@MContactType", MySqlDbType.VarChar, 50)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = filter.MBizType;
			array[2].Value = filter.MContactID;
			array[3].Value = filter.MCurrencyID;
			array[4].Value = ctx.MLCID;
			array[5].Value = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
			array[6].Value = filter.MBillID;
			array[7].Value = filter.MKeyword;
			array[8].Value = filter.MAmount;
			array[9].Value = filter.MContactType;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array);
			list = ModelInfoManager.DataTableToList<IVVerificationListModel>(dataSet.Tables[0]);
			GetVerifData(ctx, filter, list);
			return list;
		}

		private static List<IVVerificationListModel> GetExpenseVerificationList(MContext ctx, IVVerificationListFilterModel filter)
		{
			List<IVVerificationListModel> list = null;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Select 'Expense' as MBizBillType,a.MType as MBizType,");
			stringBuilder.AppendLine("a.MID as MBillID,'' as MBillNo,");
			stringBuilder.AppendFormat("a.MBizDate,a.MContactID, {0} as MContactName,a.MCyID as MCurrencyID,", (ctx.MLCID == "0x0009") ? string.Format("concat(MFirstName,' ',MLastName)") : string.Format("concat(MLastName,MFirstName)"));
			stringBuilder.AppendLine("c.MName as MCurrencyName,a.MExchangeRate as MRate,a.MReference,");
			stringBuilder.AppendLine("a.MTaxTotalAmt as MAmountTotal,a.MTaxTotalAmtFor as MAmountTotalFor,");
			stringBuilder.AppendLine("abs(a.MVerifyAmt) as MHaveVerificationAmt,abs(a.MVerifyAmtFor)  as MHaveVerificationAmtFor,");
			stringBuilder.AppendLine("ABS(a.MTaxTotalAmt)-abs(a.MVerifyAmt) as MNoVerificationAmt,ABS(a.MTaxTotalAmtFor)-abs(a.MVerifyAmtFor) as MNoVerificationAmtFor");
			stringBuilder.AppendLine("From T_IV_Expense a ");
			stringBuilder.AppendLine("INNER Join T_BD_Employees_L b On a.MContactID=b.MParentID and b.MLocaleID=@MLocaleID  and b.MIsDelete=0 ");
			stringBuilder.AppendLine("Left Join T_Bas_Currency_L c On a.MCyID=c.MParentID and c.MLocaleID=@MLocaleID  and c.MIsDelete=0 ");
			stringBuilder.AppendLine("Where a.MIsDelete=0 AND a.MOrgID=@MOrgID and ABS(a.MTaxTotalAmtFor-a.MVerifyAmtFor)>0  and a.MStatus=@MStatus");
			GetFilterString(filter, stringBuilder);
			MySqlParameter[] array = new MySqlParameter[10]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MBizType", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MContactID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MCurrencyID", MySqlDbType.VarChar, 6),
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MDueDate", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MBillID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MKeyword", MySqlDbType.VarChar, 300),
				new MySqlParameter("@MAmount", MySqlDbType.Decimal),
				new MySqlParameter("@MStatus", MySqlDbType.Int32)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = filter.MBizType;
			array[2].Value = filter.MContactID;
			array[3].Value = filter.MCurrencyID;
			array[4].Value = ctx.MLCID;
			array[5].Value = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
			array[6].Value = filter.MBillID;
			array[7].Value = filter.MKeyword;
			array[8].Value = filter.MAmount;
			array[9].Value = Convert.ToInt32(IVExpenseStatusEnum.WaitingPayment);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array);
			list = ModelInfoManager.DataTableToList<IVVerificationListModel>(dataSet.Tables[0]);
			GetVerifData(ctx, filter, list);
			return list;
		}

		private static List<IVVerificationListModel> GetSalaryPaymentVerificationList(MContext ctx, IVVerificationListFilterModel filter)
		{
			List<IVVerificationListModel> list = null;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Select 'PayRun' as MBizBillType,'Pay_Salary' as MBizType,");
			stringBuilder.AppendLine("a.MID as MBillID,date_format(a1.MDate, '%Y-%m') as MBillNo,");
			stringBuilder.AppendFormat("a.MEmployeeID, {0} as MContactName,@MCurrencyID as MCurrencyID,", (ctx.MLCID == "0x0009") ? string.Format("concat(MFirstName,' ',MLastName)") : string.Format("concat(MLastName,MFirstName)"));
			stringBuilder.AppendLine("c.MName as MCurrencyName,1 as MRate,'' as MReference,");
			stringBuilder.AppendLine("a.MNetSalary as MAmountTotal,a.MNetSalary as MAmountTotalFor,");
			stringBuilder.AppendLine("ABS(IFNULL(a.MVerificationAmt,0) * 1) as MHaveVerificationAmt,IFNULL(a.MVerificationAmt,0) as MHaveVerificationAmtFor,");
			stringBuilder.AppendLine("ABS(a.MNetSalary-IFNULL(a.MVerificationAmt,0) * 1) as MNoVerificationAmt,ABS(a.MNetSalary-IFNULL(a.MVerificationAmt,0)) as MNoVerificationAmtFor");
			stringBuilder.AppendLine("From T_PA_SalaryPayment a ");
			stringBuilder.AppendLine("left join T_PA_PayRun a1 on a.MRunID=a1.MID  and a1.MIsDelete=0 ");
			stringBuilder.AppendLine("Left Join T_BD_Employees_L b On a.MEmployeeID=b.MParentID and b.MLocaleID=@MLocaleID and b.MIsDelete=0 ");
			stringBuilder.AppendLine("Left Join T_Bas_Currency_L c On @MCurrencyID=c.MParentID and c.MLocaleID=@MLocaleID  and c.MIsDelete=0 ");
			stringBuilder.AppendLine("Where  a.MIsDelete=0 AND a1.MOrgID=@MOrgID and (a.MNetSalary-IFNULL(a.MVerificationAmt,0))>0  and a.MStatus=@MStatus");
			GetFilterString(filter, stringBuilder);
			MySqlParameter[] array = new MySqlParameter[10]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MBizType", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MContactID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MCurrencyID", MySqlDbType.VarChar, 6),
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MDueDate", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MBillID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MKeyword", MySqlDbType.VarChar, 300),
				new MySqlParameter("@MAmount", MySqlDbType.Decimal),
				new MySqlParameter("@MStatus", MySqlDbType.Int32)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = filter.MBizType;
			array[2].Value = filter.MContactID;
			array[3].Value = filter.MCurrencyID;
			array[4].Value = ctx.MLCID;
			array[5].Value = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
			array[6].Value = filter.MBillID;
			array[7].Value = filter.MKeyword;
			array[8].Value = filter.MAmount;
			array[9].Value = Convert.ToInt32(PASalaryPaymentStatusEnum.WaitingPayment);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array);
			list = ModelInfoManager.DataTableToList<IVVerificationListModel>(dataSet.Tables[0]);
			GetVerifData(ctx, filter, list);
			return list;
		}

		private static List<IVVerificationListModel> GetInvoiceVerificationList(MContext ctx, IVVerificationListFilterModel filter)
		{
			List<IVVerificationListModel> list = null;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Select 'Invoice' as MBizBillType,a.MType as MBizType,");
			stringBuilder.AppendLine("a.MID as MBillID,a.MNumber as MBillNo,");
			stringBuilder.AppendFormat("a.MBizDate,a.MContactID,convert(AES_DECRYPT(b.MName,'{0}') using utf8) as MContactName,a.MCyID as MCurrencyID,", "JieNor-001");
			stringBuilder.AppendLine("c.MName as MCurrencyName,a.MExchangeRate as MRate,a.MReference,");
			stringBuilder.AppendLine("a.MTaxTotalAmt as MAmountTotal,a.MTaxTotalAmtFor as MAmountTotalFor,");
			stringBuilder.AppendLine("abs(a.MVerifyAmt) as MHaveVerificationAmt,abs(a.MVerifyAmtFor)  as MHaveVerificationAmtFor,");
			stringBuilder.AppendLine("ABS(a.MTaxTotalAmt)-abs(a.MVerifyAmt) as MNoVerificationAmt,ABS(a.MTaxTotalAmtFor)-abs(a.MVerifyAmtFor) as MNoVerificationAmtFor");
			stringBuilder.AppendLine("From T_IV_Invoice a ");
			stringBuilder.AppendLine("INNER Join T_BD_Contacts_l b On a.MContactID=b.MParentID and b.MLocaleID=@MLocaleID   and b.MIsDelete=0 ");
			stringBuilder.AppendLine("Left Join T_Bas_Currency_L c On a.MCyID=c.MParentID and c.MLocaleID=@MLocaleID  and c.MIsDelete=0 ");
			stringBuilder.AppendLine("Where a.MIsDelete=0 AND a.MOrgID=@MOrgID  and ABS(a.MTaxTotalAmtFor-a.MVerifyAmtFor)>0 and a.MStatus=@MStatus");
			GetFilterString(filter, stringBuilder);
			MySqlParameter[] array = new MySqlParameter[10]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MBizType", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MContactID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MCurrencyID", MySqlDbType.VarChar, 6),
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MDueDate", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MBillID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MKeyword", MySqlDbType.VarChar, 300),
				new MySqlParameter("@MAmount", MySqlDbType.Decimal),
				new MySqlParameter("@MStatus", MySqlDbType.Int32)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = filter.MBizType;
			array[2].Value = filter.MContactID;
			array[3].Value = filter.MCurrencyID;
			array[4].Value = ctx.MLCID;
			array[5].Value = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
			array[6].Value = filter.MBillID;
			array[7].Value = filter.MKeyword;
			array[8].Value = filter.MAmount;
			array[9].Value = Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array);
			list = ModelInfoManager.DataTableToList<IVVerificationListModel>(dataSet.Tables[0]);
			GetVerifData(ctx, filter, list);
			return list;
		}

		private static string GetVerifScope(IVVerificationInforViewScopeEnum verificationInforViewScope, string tableAs)
		{
			switch (verificationInforViewScope)
			{
			case IVVerificationInforViewScopeEnum.All:
				return "";
			case IVVerificationInforViewScopeEnum.DueAndUnFinished:
				return string.Format(" IFNULL({0}.MVerificationAmt,0) < {0}.MTaxTotalAmtFor And {0}.MDueDate <=@MDueDate", tableAs);
			case IVVerificationInforViewScopeEnum.Finish:
				return string.Format(" IFNULL({0}.MVerificationAmt,0) >= {0}.MTaxTotalAmtFor ", tableAs);
			case IVVerificationInforViewScopeEnum.UnFinished:
				return string.Format(" IFNULL({0}.MVerificationAmt,0) < {0}.MTaxTotalAmtFor ", tableAs);
			default:
				return "";
			}
		}

		private static void GetVerifData(MContext ctx, IVVerificationListFilterModel filter, List<IVVerificationListModel> verifLst)
		{
			if (filter.MViewVerif)
			{
				GetHaveVerifData(ctx, filter, verifLst);
			}
			if (filter.MViewCanVerif)
			{
				GetCanVerifData(ctx, filter, verifLst);
			}
		}

		private static void GetCanVerifData(MContext ctx, IVVerificationListFilterModel filter, List<IVVerificationListModel> verifLst)
		{
			List<IVVerificationListModel> list = new List<IVVerificationListModel>();
			switch (filter.MBizBillType)
			{
			case "Invoice":
				list = GetInvoiceCanVerifData(ctx, filter);
				break;
			case "Payment":
				list = GetPaymentCanVerifData(ctx, filter);
				break;
			case "Receive":
				list = GetReceiveCanVerifData(ctx, filter);
				break;
			}
			if (list != null && list.Count != 0)
			{
				foreach (IVVerificationListModel item in verifLst)
				{
					if (!(item.MHaveVerificationAmtFor >= item.MAmountTotalFor))
					{
						List<IVVerificationListModel> canVerificationBillList = (from f in list
						where f.MContactID == item.MContactID && f.MCurrencyID == item.MCurrencyID
						select f).ToList();
						item.CanVerificationBillList = canVerificationBillList;
					}
				}
			}
		}

		private static List<IVVerificationListModel> GetReceiveCanVerifData(MContext ctx, IVVerificationListFilterModel filter)
		{
			IVVerificationListFilterModel iVVerificationListFilterModel = null;
			List<IVVerificationListModel> list = new List<IVVerificationListModel>();
			switch (filter.MBizType)
			{
			case "Receive_Sale":
				iVVerificationListFilterModel = GetCanVerifFilter(filter, "Invoice", "Invoice_Sale");
				list = GetInvoiceVerificationList(ctx, iVVerificationListFilterModel);
				iVVerificationListFilterModel = GetCanVerifFilter(filter, "Receive", "Receive_SaleReturn");
				list.AddRange(GetReceiveVerificationList(ctx, iVVerificationListFilterModel));
				break;
			case "Receive_SaleReturn":
				iVVerificationListFilterModel = GetCanVerifFilter(filter, "Invoice", "Invoice_Sale_Red");
				list = GetInvoiceVerificationList(ctx, iVVerificationListFilterModel);
				break;
			}
			return list;
		}

		private static IVVerificationListFilterModel GetCanVerifFilter(IVVerificationListFilterModel filter, string bizBillType, string bizType)
		{
			IVVerificationListFilterModel iVVerificationListFilterModel = new IVVerificationListFilterModel();
			iVVerificationListFilterModel.MContactID = filter.MContactID;
			iVVerificationListFilterModel.MCurrencyID = filter.MCurrencyID;
			iVVerificationListFilterModel.MViewCanVerif = false;
			iVVerificationListFilterModel.MViewDataType = IVVerificationInforViewScopeEnum.UnFinished;
			iVVerificationListFilterModel.MViewVerif = false;
			iVVerificationListFilterModel.MBizBillType = bizBillType;
			iVVerificationListFilterModel.MBizType = bizType;
			return iVVerificationListFilterModel;
		}

		private static List<IVVerificationListModel> GetPaymentCanVerifData(MContext ctx, IVVerificationListFilterModel filter)
		{
			IVVerificationListFilterModel iVVerificationListFilterModel = null;
			List<IVVerificationListModel> result = new List<IVVerificationListModel>();
			switch (filter.MBizType)
			{
			case "Pay_Purchase":
				iVVerificationListFilterModel = GetCanVerifFilter(filter, "Invoice", "Invoice_Purchase");
				result = GetInvoiceVerificationList(ctx, iVVerificationListFilterModel);
				iVVerificationListFilterModel = GetCanVerifFilter(filter, "Payment", "Pay_PurReturn");
				result = GetInvoiceVerificationList(ctx, iVVerificationListFilterModel);
				break;
			case "Pay_PurReturn":
				iVVerificationListFilterModel = GetCanVerifFilter(filter, "Invoice", "Invoice_Purchase_Red");
				result = GetInvoiceVerificationList(ctx, iVVerificationListFilterModel);
				break;
			}
			return result;
		}

		private static List<IVVerificationListModel> GetInvoiceCanVerifData(MContext ctx, IVVerificationListFilterModel filter)
		{
			IVVerificationListFilterModel iVVerificationListFilterModel = null;
			List<IVVerificationListModel> result = new List<IVVerificationListModel>();
			switch (filter.MBizType)
			{
			case "Invoice_Purchase":
				iVVerificationListFilterModel = GetCanVerifFilter(filter, "Payment", "Pay_Purchase");
				result = GetInvoiceVerificationList(ctx, iVVerificationListFilterModel);
				iVVerificationListFilterModel = GetCanVerifFilter(filter, "Invoice", "Invoice_Purchase_Red");
				result = GetInvoiceVerificationList(ctx, iVVerificationListFilterModel);
				iVVerificationListFilterModel = GetCanVerifFilter(filter, "Invoice", "Invoice_Sale");
				result = GetInvoiceVerificationList(ctx, iVVerificationListFilterModel);
				break;
			case "Invoice_Purchase_Red":
				iVVerificationListFilterModel = GetCanVerifFilter(filter, "Payment", "Pay_PurReturn");
				result = GetInvoiceVerificationList(ctx, iVVerificationListFilterModel);
				iVVerificationListFilterModel = GetCanVerifFilter(filter, "Invoice", "Invoice_Purchase");
				result = GetInvoiceVerificationList(ctx, iVVerificationListFilterModel);
				break;
			case "Invoice_Sale":
				iVVerificationListFilterModel = GetCanVerifFilter(filter, "Receive", "Receive_Sale");
				result = GetInvoiceVerificationList(ctx, iVVerificationListFilterModel);
				iVVerificationListFilterModel = GetCanVerifFilter(filter, "Invoice", "Invoice_Sale_Red");
				result = GetInvoiceVerificationList(ctx, iVVerificationListFilterModel);
				iVVerificationListFilterModel = GetCanVerifFilter(filter, "Invoice", "Invoice_Purchase_Red");
				result = GetInvoiceVerificationList(ctx, iVVerificationListFilterModel);
				break;
			case "Invoice_Sale_Red":
				iVVerificationListFilterModel = GetCanVerifFilter(filter, "Receive", "Receive_SaleReturn");
				result = GetInvoiceVerificationList(ctx, iVVerificationListFilterModel);
				iVVerificationListFilterModel = GetCanVerifFilter(filter, "Invoice", "Invoice_Sale");
				result = GetInvoiceVerificationList(ctx, iVVerificationListFilterModel);
				break;
			}
			return result;
		}

		private static void GetHaveVerifData(MContext ctx, IVVerificationListFilterModel filter, List<IVVerificationListModel> verifLst)
		{
			List<IVVerificationListModel> haveVerifData = GetHaveVerifData(ctx, filter);
			if (haveVerifData != null && haveVerifData.Count != 0)
			{
				foreach (IVVerificationListModel item in verifLst)
				{
					if (!(item.MHaveVerificationAmtFor == decimal.Zero))
					{
						List<IVVerificationListModel> haveVerificationBillList = (from f in haveVerifData
						where f.MSourceBillID == item.MBillID
						select f).ToList();
						item.HaveVerificationBillList = haveVerificationBillList;
					}
				}
			}
		}

		private static string GetHaveVerifDataView(IVVerificationListFilterModel filter)
		{
			string str = "T_IV_Invoice";
			if (filter.MBizBillType == "Payment")
			{
				str = "T_IV_Payment";
			}
			else if (filter.MBizBillType == "Receive")
			{
				str = "T_IV_Receive";
			}
			else if (filter.MBizBillType == "Expense")
			{
				str = "T_IV_Expense";
			}
			else if (filter.MBizBillType == "PayRun")
			{
				str = "T_PA_SalaryPayment";
			}
			StringBuilder stringBuilder = new StringBuilder();
			string arg = "a";
			stringBuilder.AppendLine("Select v.MID AS VerificationID,v.MSourceBillType,v.MSourceBillID, v.MTargetBillType,v.MTargetBillID,v.MAmount,v.MCreateDate ");
			stringBuilder.AppendLine("From " + str + " a ");
			if (filter.MBizBillType == "PayRun")
			{
				arg = "a1";
				stringBuilder.AppendLine("left join T_PA_PayRun a1 on a.MRunID=a1.MID  and a1.MIsActive=1 and a1.MIsDelete=0 ");
			}
			stringBuilder.AppendLine("inner Join T_IV_Verification v On v.MSourceBillType=@MBizBillType And a.MID=v.MSourceBillID  and v.MIsActive=1 and v.MIsDelete=0");
			stringBuilder.AppendFormat("Where {0}.MOrgID=@MOrgID  and a.MIsActive=1 and a.MIsDelete=0 ", arg);
			GetFilterString(filter, stringBuilder);
			stringBuilder.AppendLine("UNION ALL ");
			stringBuilder.AppendLine("Select v.MID AS VerificationID, v.MTargetBillType as MSourceBillType,v.MTargetBillID as MSourceBillID,v.MSourceBillType as MTargetBillType,v.MSourceBillID as MTargetBillID,v.MAmount,v.MCreateDate ");
			stringBuilder.AppendLine("From " + str + " a ");
			if (filter.MBizBillType == "PayRun")
			{
				stringBuilder.AppendLine("left join T_PA_PayRun a1 on a.MRunID=a1.MID and a1.MIsActive=1 and a1.MIsDelete=0  ");
			}
			stringBuilder.AppendLine("inner Join T_IV_Verification v On v.MTargetBillType=@MBizBillType And a.MID=v.MTargetBillID and v.MIsActive=1 and v.MIsDelete=0  ");
			stringBuilder.AppendFormat("Where {0}.MOrgID=@MOrgID  and a.MIsActive=1 and a.MIsDelete=0 ", arg);
			GetFilterString(filter, stringBuilder);
			return stringBuilder.ToString();
		}

		private static string GetHaveVerifDataSql(IVVerificationListFilterModel filter, string sqlVerifi, string bizBillType)
		{
			string str = "T_IV_Invoice";
			if (bizBillType == "Payment")
			{
				str = "T_IV_Payment";
			}
			else if (bizBillType == "Receive")
			{
				str = "T_IV_Receive";
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Select v.VerificationID, v.MSourceBillType,v.MSourceBillID,'Invoice' as MBizBillType,a.MType as MBizType,");
			stringBuilder.AppendLine("a.MID as MBillID,a.MNumber as MBillNo,");
			stringBuilder.AppendFormat("a.MBizDate,a.MContactID,convert(AES_DECRYPT(b.MName,'{0}') using utf8) as MContactName,a.MCyID as MCurrencyID,", "JieNor-001");
			stringBuilder.AppendLine("c.MName as MCurrencyName,a.MExchangeRate as MRate,a.MReference,");
			stringBuilder.AppendLine("a.MTaxTotalAmt as MAmountTotal,a.MTaxTotalAmtFor as MAmountTotalFor,");
			stringBuilder.AppendLine("a.MTaxTotalAmt as MHaveVerificationAmt,IFNULL(a.MVerificationAmt,0) as MHaveVerificationAmtFor,");
			stringBuilder.AppendLine("IFNULL(a.MVerificationAmt,0) * a.MExchangeRate as MNoVerificationAmt,IFNULL(a.MVerificationAmt,0) as MNoVerificationAmtFor");
			stringBuilder.AppendLine("From " + str + " a ");
			stringBuilder.AppendLine("Left Join T_BD_Contacts_l b On a.MContactID=b.MParentID and b.MLocaleID=@MLocaleID  and b.MIsActive=1 and b.MIsDelete=0 ");
			stringBuilder.AppendLine("Left Join T_Bas_Currency_L c On a.MCyID=c.MParentID and c.MLocaleID=@MLocaleID  and c.MIsActive=1 and c.MIsDelete=0 ");
			stringBuilder.AppendLine("inner Join (" + sqlVerifi + ") v On a.MID=v.MTargetBillID and v.MTargetBillType= '" + bizBillType + "' ");
			stringBuilder.AppendLine("Where a.MOrgID=@MOrgID  and a.MIsActive=1 and a.MIsDelete=0  ");
			if (!string.IsNullOrWhiteSpace(filter.MContactID))
			{
				stringBuilder.AppendLine(" And a.MContactID=@MContactID ");
			}
			if (!string.IsNullOrWhiteSpace(filter.MCurrencyID))
			{
				stringBuilder.AppendLine(" And a.MCyID=@MCurrencyID ");
			}
			if (!string.IsNullOrWhiteSpace(filter.MBillID))
			{
				stringBuilder.AppendLine(" And v.MSourceBillID=@MBillID ");
			}
			return stringBuilder.ToString();
		}

		private static string GetHistoryVerifDataSql(IVVerificationListFilterModel filter, string sqlVerifi, string bizBillType)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string str = "T_IV_Invoice";
			if (bizBillType == "Payment")
			{
				str = "T_IV_Payment";
			}
			else if (bizBillType == "Receive")
			{
				str = "T_IV_Receive";
			}
			else if (bizBillType == "Expense")
			{
				str = "T_IV_Expense";
			}
			else if (bizBillType == "PayRun")
			{
				stringBuilder.AppendLine("Select v.VerificationID, v.MSourceBillType,v.MSourceBillID,'" + bizBillType + "' as MBizBillType,'Pay_Salary' as MBizType,");
				stringBuilder.AppendLine("a.MID as MBillID,DATE_FORMAT(a1.MDate, '%Y-%m') as MBillNo,");
				stringBuilder.AppendLine("'' as MBizDate,a.MEmployeeID as MContactID,(case @MLocaleID when '0x0009' then concat(b.MFirstName, ' ', b.MLastName) else concat(b.MLastName, b.MFirstName) end) as MContactName,@MCurrencyID as MCurrencyID,");
				stringBuilder.AppendLine("c.MName as MCurrencyName,1 as MRate,'' as MReference,");
				stringBuilder.AppendLine("a.MNetSalary as MAmountTotal,a.MNetSalary as MAmountTotalFor,");
				stringBuilder.AppendLine("a.MNetSalary as MHaveVerificationAmt,v.MAmount as MHaveVerificationAmtFor,");
				stringBuilder.AppendLine("IFNULL(a.MVerificationAmt,0) * 1 as MNoVerificationAmt,IFNULL(a.MVerificationAmt,0) as MNoVerificationAmtFor,v.MCreateDate");
				stringBuilder.AppendLine("From T_PA_SalaryPayment a ");
				stringBuilder.AppendLine("Left Join T_PA_PayRun a1 on a.MRunID=a1.MID and a1.MIsActive=1 and a1.MIsDelete=0 ");
				stringBuilder.AppendLine("Left Join T_BD_Employees_l b On a.MEmployeeID=b.MParentID and b.MLocaleID=@MLocaleID  and b.MIsActive=1 and b.MIsDelete=0  ");
				stringBuilder.AppendLine("Left Join T_Bas_Currency_L c On c.MParentID=@MCurrencyID and c.MLocaleID=@MLocaleID  and c.MIsActive=1 and c.MIsDelete=0 ");
				stringBuilder.AppendLine("inner Join (" + sqlVerifi + ") v On a.MID=v.MTargetBillID and v.MTargetBillType= '" + bizBillType + "' ");
				stringBuilder.AppendLine("Where a.MOrgID=@MOrgID and a.MIsActive=1 and a.MIsDelete=0 ");
				if (!string.IsNullOrWhiteSpace(filter.MContactID))
				{
					stringBuilder.AppendLine(" And a.MEmployeeID=@MContactID ");
				}
			}
			if (bizBillType != "PayRun")
			{
				stringBuilder.AppendLine("Select v.VerificationID, v.MSourceBillType,v.MSourceBillID,'" + bizBillType + "' as MBizBillType,a.MType as MBizType,");
				stringBuilder.AppendLine("a.MID as MBillID,a.MNumber as MBillNo,");
				stringBuilder.AppendFormat("a.MBizDate,a.MContactID,convert(AES_DECRYPT(b.MName,'{0}') using utf8) as MContactName,a.MCyID as MCurrencyID,", "JieNor-001");
				stringBuilder.AppendLine("c.MName as MCurrencyName,a.MExchangeRate as MRate,a.MReference,");
				stringBuilder.AppendLine("a.MTaxTotalAmt as MAmountTotal,a.MTaxTotalAmtFor as MAmountTotalFor,");
				stringBuilder.AppendLine("a.MTaxTotalAmt as MHaveVerificationAmt,v.MAmount as MHaveVerificationAmtFor,");
				stringBuilder.AppendLine("IFNULL(a.MVerificationAmt,0) * a.MExchangeRate as MNoVerificationAmt,IFNULL(a.MVerificationAmt,0) as MNoVerificationAmtFor,v.MCreateDate");
				stringBuilder.AppendLine("From " + str + " a ");
				stringBuilder.AppendLine("Left Join T_BD_Contacts_l b On a.MContactID=b.MParentID and b.MLocaleID=@MLocaleID  and b.MIsActive=1 and b.MIsDelete=0 ");
				stringBuilder.AppendLine("Left Join T_Bas_Currency_L c On a.MCyID=c.MParentID and c.MLocaleID=@MLocaleID  and b.MIsActive=1 and b.MIsDelete=0 ");
				stringBuilder.AppendLine("inner Join (" + sqlVerifi + ") v On a.MID=v.MTargetBillID and v.MTargetBillType= '" + bizBillType + "' ");
				stringBuilder.AppendLine("Where a.MOrgID=@MOrgID  and a.MIsActive=1 and a.MIsDelete=0 ");
				if (!string.IsNullOrWhiteSpace(filter.MContactID))
				{
					stringBuilder.AppendLine(" And a.MContactID=@MContactID ");
				}
				if (!string.IsNullOrWhiteSpace(filter.MCurrencyID))
				{
					stringBuilder.AppendLine(" And a.MCyID=@MCurrencyID ");
				}
			}
			if (!string.IsNullOrWhiteSpace(filter.MBillID))
			{
				stringBuilder.AppendFormat(" And v.MSourceBillID in ('{0}') ", string.Join("','", filter.MBillID.Split(',')));
			}
			return stringBuilder.ToString();
		}

		private static void GetFilterString(IVVerificationListFilterModel filter, StringBuilder sqlVerifi)
		{
			if (filter.MBizBillType != "PayRun" && !string.IsNullOrWhiteSpace(filter.MBizType))
			{
				sqlVerifi.AppendLine(" And a.MType=@MBizType ");
			}
			if (!string.IsNullOrWhiteSpace(filter.MContactID))
			{
				if (filter.MBizBillType != "PayRun")
				{
					sqlVerifi.AppendLine(" And a.MContactID=@MContactID ");
				}
				else
				{
					sqlVerifi.AppendLine(" And a.MEmployeeID=@MContactID ");
				}
			}
			if (filter.MBizBillType != "PayRun" && !string.IsNullOrWhiteSpace(filter.MCurrencyID))
			{
				sqlVerifi.AppendLine(" And a.MCyID=@MCurrencyID ");
			}
			if (!string.IsNullOrWhiteSpace(filter.MBillID))
			{
				sqlVerifi.AppendFormat(" And a.MID in ('{0}') ", string.Join("','", filter.MBillID.Split(',')));
			}
			string verifScope = GetVerifScope(filter.MViewDataType, " a");
			if (!string.IsNullOrWhiteSpace(verifScope))
			{
				sqlVerifi.AppendLine(" And " + verifScope);
			}
			if (filter.MBizBillType != "PayRun" && !string.IsNullOrEmpty(filter.MKeyword))
			{
				sqlVerifi.AppendLine(" AND (a.MReference like concat('%', @MKeyword, '%') OR a.MNumber like concat('%', @MKeyword, '%'))");
			}
			if (filter.MAmount > decimal.Zero)
			{
				string arg = (filter.MBizBillType != "PayRun") ? "MTaxTotalAmtFor" : "MNetSalary";
				sqlVerifi.AppendFormat(" AND ABS(a.{0}) - ABS(IFNULL(a.MVerificationAmt,0))=@MAmount", arg);
			}
		}

		public static List<IVBatchPaymentModel> GetBatchPaymentList(MContext ctx, ParamBase para, string selectObj)
		{
			StringBuilder stringBuilder = new StringBuilder();
			switch (selectObj)
			{
			case "Invoice_Sales":
			case "Invoice_Purchases":
				stringBuilder.Append(GetInvoiceBatchSql(ctx, para));
				break;
			case "Expense":
				stringBuilder.Append(GetExpenseBatchSql(ctx, para));
				break;
			case "PayRun":
				stringBuilder.Append(GetSalaryBatchSql(ctx, para));
				break;
			}
			MySqlParameter[] array = new MySqlParameter[1]
			{
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 6)
			};
			array[0].Value = ctx.MLCID;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return ModelInfoManager.DataTableToList<IVBatchPaymentModel>(dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array).Tables[0]);
		}

		public static OperationResult IsSuccessBatch(MContext ctx, ParamBase para, string selectObj)
		{
			OperationResult operationResult = new OperationResult();
			if (selectObj == "Invoice_Sales" || selectObj == "Invoice_Purchases")
			{
				List<IVInvoiceModel> dataModelList = ModelInfoManager.GetDataModelList<IVInvoiceModel>(ctx, new SqlWhere().AddFilter("MID", SqlOperators.In, para.KeyIDSWithSingleQuote.Replace("'", "").Split(',').ToList()), false, false);
				if ((from g in dataModelList
				group g by g.MCyID).ToList().Count > 1)
				{
					operationResult.VerificationInfor.Add(new BizVerificationInfor
					{
						Level = AlertEnum.Error,
						Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "CurrMustSame", "the currency must be the same.")
					});
				}
			}
			return operationResult;
		}

		private static string GetInvoiceBatchSql(MContext ctx, ParamBase para)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("select t1.MID, t1.MCyID, '{0}' AS MOrgCyID,t1.MNumber,convert(AES_DECRYPT(t2.MName,'{1}') using utf8) as MContactName,t1.MDueDate,\r\n                              abs(ifnull(t1.MTaxTotalAmtFor,0)-ifnull(t1.MVerifyAmtFor,0)) as MNoVerifyAmtFor,\r\n                              abs(ifnull(t1.MTaxTotalAmt,0)-ifnull(t1.MVerifyAmt,0)) as MNoVerifyAmt,\r\n                              abs(ifnull(t1.MTaxTotalAmtFor,0)-ifnull(t1.MVerifyAmtFor,0)) as MPayAmount,\r\n                              '' MReference  ", ctx.MBasCurrencyID, "JieNor-001");
			stringBuilder.AppendLine("from T_IV_Invoice t1 ");
			stringBuilder.AppendLine("left join T_BD_Contacts_l t2 on t1.MContactID=t2.MParentID AND t2.MLocaleID=@MLocaleID AND t2.MIsDelete = 0 ");
			stringBuilder.AppendFormat("where t1.MID in ({0}) ", para.KeyIDSWithSingleQuote);
			return stringBuilder.ToString();
		}

		private static string GetExpenseBatchSql(MContext ctx, ParamBase para)
		{
			return $"select t1.MID, t1.MCyID,'{ctx.MBasCurrencyID}' AS MOrgCyID,t1.MNumber, t2.MFirstName,t2.MLastName, t1.MDueDate,\r\n                                abs(ifnull(t1.MTaxTotalAmtFor,0)-ifnull(t1.MVerifyAmtFor,0)) as MNoVerifyAmtFor,\r\n                                abs(ifnull(t1.MTaxTotalAmt,0)-ifnull(t1.MVerifyAmt,0)) as MNoVerifyAmt,\r\n                                abs(ifnull(t1.MTaxTotalAmtFor,0)-ifnull(t1.MVerifyAmtFor,0)) as MPayAmount,\r\n                                t1.MReference\r\n                                from T_IV_Expense t1\r\n                                left JOIN t_bd_employees_l t2 ON t1.MEmployee=t2.MParentID AND t2.MLocaleID=@MLocaleID AND t2.MIsDelete = 0 \r\n                                WHERE t1.MID in ({para.KeyIDSWithSingleQuote})";
		}

		private static string GetSalaryBatchSql(MContext ctx, ParamBase para)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("select t1.MID, '{0}' AS MCyID, '{0}' AS MOrgCyID, DATE_FORMAT(t3.MDate, '%Y-%m') as MNumber,t2.MFirstName,t2.MLastName,\r\n                              abs(ifnull(t1.MNetSalary,0)-ifnull(t1.MVerificationAmt,0)) as MNoVerifyAmtFor,\r\n                              abs(ifnull(t1.MNetSalary,0)-ifnull(t1.MVerificationAmt,0)) as MNoVerifyAmt,\r\n                              abs(ifnull(t1.MNetSalary,0)-ifnull(t1.MVerificationAmt,0)) as MPayAmount,\r\n                              '' as MReference  ", ctx.MBasCurrencyID);
			stringBuilder.AppendLine("from T_PA_SalaryPayment t1 ");
			stringBuilder.AppendLine("left join t_bd_employees_l t2 on t1.MEmployeeID=t2.MParentID AND t2.MLocaleID=@MLocaleID AND t2.MIsDelete = 0 ");
			stringBuilder.AppendLine("left join T_PA_PayRun t3 on t1.MRunID=t3.MID ");
			stringBuilder.AppendFormat("where t1.MID in ({0}) ", para.KeyIDSWithSingleQuote);
			return stringBuilder.ToString();
		}

		public static OperationResult BatchPaymentUpdate(MContext ctx, IVBatchPayHeadModel headModel)
		{
			OperationResult operationResult = new OperationResult();
			List<IVMakePaymentModel> list = new List<IVMakePaymentModel>();
			foreach (IVBatchPaymentModel item in headModel.PaymentEntry)
			{
				IVMakePaymentModel iVMakePaymentModel = new IVMakePaymentModel();
				iVMakePaymentModel.MBankID = headModel.MPayBank;
				iVMakePaymentModel.MPaidDate = headModel.MPayDate;
				iVMakePaymentModel.MObjectID = item.MID;
				iVMakePaymentModel.MPaidAmount = item.MPayAmount;
				iVMakePaymentModel.MRef = item.MReference;
				iVMakePaymentModel.IsPayRun = headModel.IsPayRun;
				iVMakePaymentModel.SalaryPaymentIDLists = headModel.SalaryPaymentIDLists;
				iVMakePaymentModel.PayRunID = headModel.PayRunID;
				list.Add(iVMakePaymentModel);
			}
			IVMakePaymentAbstractRepository iVMakePaymentAbstractRepository = IVMakePaymentAbstractRepository.CreateInstance(headModel.SelectObj);
			return iVMakePaymentAbstractRepository.ToPay(ctx, list);
		}

		public static List<CommandInfo> GetBatchPaymentCmdList(MContext ctx, List<IVBatchPayHeadModel> headModelList, ref string errorMsg)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			if (headModelList == null || headModelList.Count == 0)
			{
				return list;
			}
			IVMakePaymentAbstractRepository iVMakePaymentAbstractRepository = IVMakePaymentAbstractRepository.CreateInstance(headModelList[0].SelectObj);
			foreach (IVBatchPayHeadModel headModel in headModelList)
			{
				List<IVMakePaymentModel> list2 = new List<IVMakePaymentModel>();
				foreach (IVBatchPaymentModel item in headModel.PaymentEntry)
				{
					IVMakePaymentModel iVMakePaymentModel = new IVMakePaymentModel();
					iVMakePaymentModel.MBankID = headModel.MPayBank;
					iVMakePaymentModel.MPaidDate = headModel.MPayDate;
					iVMakePaymentModel.MObjectID = item.MID;
					iVMakePaymentModel.MPaidAmount = item.MPayAmount;
					iVMakePaymentModel.MRef = item.MReference;
					iVMakePaymentModel.MObject = item.MObject;
					list2.Add(iVMakePaymentModel);
				}
				list.AddRange(iVMakePaymentAbstractRepository.GetToPayCmdList(ctx, list2, ref errorMsg));
			}
			return list;
		}
	}
}
