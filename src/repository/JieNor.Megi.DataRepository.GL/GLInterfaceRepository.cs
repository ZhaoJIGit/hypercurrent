using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.DataModel;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace JieNor.Megi.DataRepository.GL
{
	public class GLInterfaceRepository
	{
		private static readonly GLDocVoucherRepository _docVoucherDal = new GLDocVoucherRepository();

		private static readonly GLVoucherRepository _voucherDal = new GLVoucherRepository();

		public static OperationResult GenerateVouchersByBills<T>(MContext ctx, List<T> bills, List<T> dbBills = null) where T : BizDataModel
		{
			OperationResult operationResult = new OperationResult
			{
				Success = true
			};
			GLDocVoucherRepository gLDocVoucherRepository = new GLDocVoucherRepository();
			operationResult = gLDocVoucherRepository.GenerateVouchersByBills(ctx, bills, dbBills);
			List<T> list = (from x in bills
			where x.ValidationErrors != null && Enumerable.Any<ValidationError>((IEnumerable<ValidationError>)x.ValidationErrors)
			select x).ToList();
			if (list?.Any() ?? false)
			{
				operationResult.MessageList = (from x in new GLUtility().Union((from x in list
				select x.ValidationErrors).ToList())
				select x.Message.Trim()).Distinct().ToList();
				operationResult.Message = string.Join(";", operationResult.MessageList);
				operationResult.Success = false;
			}
			return operationResult;
		}

		public static OperationResult GenerateVouchersByBill<T>(MContext ctx, T bill, T dbBill = null) where T : BizDataModel
		{
			return GenerateVouchersByBills(ctx, new List<T>
			{
				bill
			}, (dbBill != null) ? new List<T>
			{
				dbBill
			} : null);
		}

		public static OperationResult IsPeriodUnclosed(MContext ctx, DateTime date)
		{
			OperationResult operationResult = new OperationResult
			{
				Success = true
			};
			GLDataPool instance = GLDataPool.GetInstance(ctx, false, 0, 0, 0);
			List<GLSettlementModel> closedPeriods = instance.ClosedPeriods;
			if (closedPeriods == null || !closedPeriods.Exists((GLSettlementModel x) => x.MYear * 100 + x.MPeriod == date.Year * 100 + date.Month))
			{
				return operationResult;
			}
			string arg = (ctx.MLCID == "0x0009") ? date.ToString("MMMMMMMMMMMMMMMM,yyyy", CultureInfo.CreateSpecificCulture("en-US")) : $"{date.Year}年{date.Month}月";
			operationResult.Success = false;
			operationResult.Message = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "GeneralLedgerHasBeenClosed", "The general ledger has been closed in {0} "), arg);
			return operationResult;
		}

		public static bool IsBillCreatedVoucher(MContext ctx, string billId)
		{
			return GLDocVoucherRepository.IsBillCreatedVoucher(ctx, billId);
		}

		public static OperationResult TransferVouchersDraft2SavedByBillIds(MContext ctx, List<string> billIds, bool create = true)
		{
			return new GLDocVoucherRepository().TransferVouchersDraft2SavedByBillIds(ctx, billIds, null, create);
		}

		public static OperationResult TransferVouchersSaved2DraftByBillIds(MContext ctx, List<string> billIds)
		{
			return new GLDocVoucherRepository().TransferVouchersSaved2DraftByBillIds(ctx, billIds);
		}

		public static OperationResult TransferBillsCreatedVouchersByStatus(MContext ctx, List<string> billIds, RecordStatus status)
		{
			OperationResult result = new OperationResult
			{
				Success = true
			};
			switch (status)
			{
			case RecordStatus.Draft:
				result = TransferVouchersSaved2DraftByBillIds(ctx, billIds);
				break;
			case RecordStatus.Saved:
				result = TransferVouchersDraft2SavedByBillIds(ctx, billIds, true);
				break;
			}
			return result;
		}

		public static OperationResult TransferBillCreatedVouchersByStatus(MContext ctx, string billId, RecordStatus status)
		{
			return TransferBillsCreatedVouchersByStatus(ctx, new List<string>
			{
				billId
			}, status);
		}

		public static List<DateTime> GetGLCloseDate(MContext ctx)
		{
			GLSettlementRepository gLSettlementRepository = new GLSettlementRepository();
			return gLSettlementRepository.GetSettledPeriodList(ctx);
		}

		public static OperationResult IsDocCanOperate(MContext ctx, string billId)
		{
			return IsDocCanOperate(ctx, new List<string>
			{
				billId
			});
		}

		public static OperationResult IsDocCanOperate(MContext ctx, List<string> billIds)
		{
			return _docVoucherDal.CanDocBeOperate(ctx, billIds);
		}

		public static List<CommandInfo> GetDeleteVoucherByDocIDCmds(MContext ctx, string billId)
		{
			return GetDeleteVoucherByDocIDsCmds(ctx, new List<string>
			{
				billId
			});
		}

		public static List<CommandInfo> GetDeleteVoucherByDocIDsCmds(MContext ctx, List<string> billIds)
		{
			List<CommandInfo> fullyDeleteDocsCreatedVoucherCmds = _docVoucherDal.GetFullyDeleteDocsCreatedVoucherCmds(ctx, billIds, null);
			return fullyDeleteDocsCreatedVoucherCmds ?? new List<CommandInfo>();
		}

		public static List<CommandInfo> DeleteBizDataByVoucher(MContext ctx, List<string> voucherIdList)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			if (voucherIdList == null || voucherIdList.Count == 0)
			{
				return list;
			}
			if (ctx.MOrgVersionID != Convert.ToInt32(1))
			{
				return list;
			}
			GLDocVoucherRepository gLDocVoucherRepository = new GLDocVoucherRepository();
			List<GLDocVoucherModel> docVoucherList = gLDocVoucherRepository.GetDocVoucherList(ctx, voucherIdList);
			if (docVoucherList == null || docVoucherList.Count == 0)
			{
				return list;
			}
			List<MySqlParameter> list2 = new List<MySqlParameter>
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			List<CommandInfo> deletBizDataCmd = GetDeletBizDataCmd(ctx, docVoucherList, GLDocTypeEnum.Payment);
			list.AddRange(deletBizDataCmd);
			List<CommandInfo> deletBizDataCmd2 = GetDeletBizDataCmd(ctx, docVoucherList, GLDocTypeEnum.Receive);
			list.AddRange(deletBizDataCmd2);
			return list;
		}

		private static List<CommandInfo> GetDeletBizDataCmd(MContext ctx, List<GLDocVoucherModel> docVoucherList, GLDocTypeEnum docType)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			List<string> list2 = (from t in docVoucherList
			where t.MDocType == Convert.ToInt32(docType)
			select t.MDocID).ToList();
			if (list2 == null || list2.Count == 0)
			{
				return list;
			}
			string text = "";
			StringBuilder stringBuilder;
			List<MySqlParameter> list3;
			List<CommandInfo> list4;
			CommandInfo obj;
			StringBuilder stringBuilder2;
			List<MySqlParameter> list5;
			List<CommandInfo> list6;
			CommandInfo obj2;
			StringBuilder stringBuilder3;
			List<MySqlParameter> list7;
			List<CommandInfo> list8;
			CommandInfo obj3;
			List<string> list9;
			DbParameter[] array;
			switch (docType)
			{
			case GLDocTypeEnum.Payment:
				text = "T_IV_Payment";
				goto IL_00a2;
			case GLDocTypeEnum.Receive:
				text = "T_IV_Receive";
				goto IL_00a2;
			default:
				{
					return list;
				}
				IL_00a2:
				list2 = GetBizDataBillIDList(ctx, text, list2);
				if (list2 == null || list2.Count == 0)
				{
					return list;
				}
				stringBuilder = new StringBuilder();
				list3 = new List<MySqlParameter>
				{
					new MySqlParameter("@MOrgID", ctx.MOrgID)
				};
				stringBuilder.AppendFormat("UPDATE {0} SET MIsDelete=0 WHERE MOrgID=@MOrgID AND MID IN (", text);
				stringBuilder.Append(GetIDSql(list2, ref list3));
				stringBuilder.Append(")");
				list4 = list;
				obj = new CommandInfo
				{
					CommandText = stringBuilder.ToString()
				};
				array = (obj.Parameters = list3.ToArray());
				list4.Add(obj);
				stringBuilder2 = new StringBuilder();
				list5 = new List<MySqlParameter>
				{
					new MySqlParameter("@MOrgID", ctx.MOrgID)
				};
				stringBuilder2.Append("UPDATE T_IV_BankBillReconcile SET MIsDelete=1 WHERE MOrgID=@MOrgID AND MIsDelete=0 AND MID IN (\r\n                                    SELECT MID FROM T_IV_BankBillReconcileEntry WHERE MOrgID=@MOrgID AND MIsDelete=0 AND MTargetBillID IN (");
				stringBuilder2.Append(GetIDSql(list2, ref list5));
				stringBuilder2.Append("))");
				list6 = list;
				obj2 = new CommandInfo
				{
					CommandText = stringBuilder2.ToString()
				};
				array = (obj2.Parameters = list5.ToArray());
				list6.Add(obj2);
				stringBuilder3 = new StringBuilder();
				list7 = new List<MySqlParameter>
				{
					new MySqlParameter("@MOrgID", ctx.MOrgID)
				};
				stringBuilder3.Append("UPDATE T_IV_BankBillReconcileEntry SET MIsDelete=1 WHERE MOrgID=@MOrgID AND MIsDelete=0 AND MTargetBillID IN (");
				stringBuilder3.Append(GetIDSql(list2, ref list7));
				stringBuilder3.Append(")");
				list8 = list;
				obj3 = new CommandInfo
				{
					CommandText = stringBuilder3.ToString()
				};
				array = (obj3.Parameters = list7.ToArray());
				list8.Add(obj3);
				list9 = (from t in docVoucherList
				where t.MDocType == Convert.ToInt32(docType)
				select t.MVoucherID).ToList();
				list.Add(GetUpdateBankBillVoucherStatusCmd(ctx, list9));
				if (list9.Count > 0)
				{
					StringBuilder stringBuilder4 = new StringBuilder();
					List<MySqlParameter> list10 = new List<MySqlParameter>
					{
						new MySqlParameter("@MOrgID", ctx.MOrgID)
					};
					stringBuilder4.Append("UPDATE T_IV_BankBillEntryVoucher SET MIsDelete=1 WHERE MOrgID=@MOrgID AND MIsDelete=0 AND MVoucherID IN (");
					stringBuilder4.Append(GetIDSql(list9, ref list10));
					stringBuilder4.Append(")");
					List<CommandInfo> list11 = list;
					CommandInfo obj4 = new CommandInfo
					{
						CommandText = stringBuilder4.ToString()
					};
					array = (obj4.Parameters = list10.ToArray());
					list11.Add(obj4);
				}
				return list;
			}
		}

		private static List<string> GetBizDataBillIDList(MContext ctx, string tableName, List<string> idList)
		{
			List<string> list = new List<string>();
			StringBuilder stringBuilder = new StringBuilder();
			List<MySqlParameter> list2 = new List<MySqlParameter>
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MCreateFrom", Convert.ToInt32(CreateFromType.Cashcoding))
			};
			stringBuilder.AppendFormat("SELECT MID FROM {0} WHERE MOrgID=@MOrgID AND MIsDelete=0 AND MCreateFrom=@MCreateFrom AND MVerifyAmtFor=0 AND MID IN ( ", tableName);
			stringBuilder.Append(GetIDSql(idList, ref list2));
			stringBuilder.Append(")");
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), list2.ToArray());
			if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return list;
			}
			DataTable dataTable = dataSet.Tables[0];
			foreach (DataRow row in dataTable.Rows)
			{
				list.Add(row["MID"].ToString());
			}
			return list;
		}

		public static CommandInfo GetUpdateBankBillVoucherStatusCmd(MContext ctx, List<string> voucherIdList)
		{
			CommandInfo commandInfo = new CommandInfo();
			StringBuilder stringBuilder = new StringBuilder();
			List<MySqlParameter> list = new List<MySqlParameter>
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			stringBuilder.AppendFormat("UPDATE T_IV_BankBillEntry SET MVoucherStatus={0}\r\n                                   WHERE MOrgID=@MOrgID AND MIsDelete=0 AND MVoucherStatus={1}\r\n                                   AND MEntryID IN (SELECT MBankBillEntryID FROM T_IV_BankBillEntryVoucher\n                                        WHERE MOrgID=@MOrgID AND MIsDelete=0 AND MVoucherID IN (", Convert.ToInt32(IVBankBillVoucherStatus.NonGenerated), Convert.ToInt32(IVBankBillVoucherStatus.Generated));
			stringBuilder.Append(GetIDSql(voucherIdList, ref list));
			stringBuilder.Append(") )");
			commandInfo.CommandText = stringBuilder.ToString();
			DbParameter[] array = commandInfo.Parameters = list.ToArray();
			return commandInfo;
		}

		public static string GetDeletedVoucherIDSql(MContext ctx, List<string> voucherIdList, ref List<MySqlParameter> paramList)
		{
			List<string> bankBillEntryIDListByVoucher = GetBankBillEntryIDListByVoucher(ctx, voucherIdList);
			if (bankBillEntryIDListByVoucher.Count == 0)
			{
				return string.Empty;
			}
			string iDSql = GetIDSql(bankBillEntryIDListByVoucher, ref paramList);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT t1.MVoucherID FROM T_IV_BankBillEntryVoucher t1\r\n                                INNER JOIN T_IV_BankBillEntry t2 ON t1.MBankBillEntryID=t2.MEntryID AND t1.MOrgID=t2.MOrgID AND t2.MIsDelete=0 \r\n                                WHERE t1.MOrgID=@MOrgID AND t1.MIsDelete=0 AND (t2.MEntryID IN (");
			stringBuilder.Append(iDSql);
			stringBuilder.Append(")");
			stringBuilder.Append("OR t2.MParentID IN (");
			stringBuilder.Append(iDSql);
			stringBuilder.Append(") ");
			stringBuilder.Append(" OR t2.MEntryID IN (SELECT MParentID FROM T_IV_BankBillEntry WHERE MOrgID=@MOrgID AND MIsDelete=0 AND IFNULL(MParentID,'')<>'' AND MEntryID IN(");
			stringBuilder.Append(iDSql);
			stringBuilder.Append(")) ");
			stringBuilder.Append(" OR t2.MParentID IN (SELECT MParentID FROM T_IV_BankBillEntry WHERE MOrgID=@MOrgID AND MIsDelete=0 AND IFNULL(MParentID,'')<>'' AND MEntryID IN(");
			stringBuilder.Append(iDSql);
			stringBuilder.Append(")))");
			return stringBuilder.ToString();
		}

		public static List<string> GetBankBillEntryIDListByVoucher(MContext ctx, List<string> voucherIdList)
		{
			List<string> list = new List<string>();
			List<MySqlParameter> list2 = new List<MySqlParameter>
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("SELECT MBankBillEntryID FROM T_IV_BankBillEntryVoucher WHERE MOrgID=@MOrgID AND MIsDelete=0 AND IFNULL(MBankBillEntryID,'')<>'' AND MVoucherID in (");
			stringBuilder.Append(GetIDSql(voucherIdList, ref list2));
			stringBuilder.Append(")");
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), list2.ToArray());
			if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return list;
			}
			DataTable dataTable = dataSet.Tables[0];
			foreach (DataRow row in dataTable.Rows)
			{
				list.Add(row["MBankBillEntryID"].ToString());
			}
			return list;
		}

		private static string GetIDSql(List<string> idList, ref List<MySqlParameter> paramList)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = 1;
			foreach (string id in idList)
			{
				string text = $"@IDParam{num}";
				stringBuilder.AppendFormat("{0},", text);
				paramList.Add(new MySqlParameter(text, id));
				num++;
			}
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
			return stringBuilder.ToString();
		}

		public static string GetActionExceptionMessageByCode(MContext ctx, MActionResultCodeEnum code)
		{
			string empty = string.Empty;
			switch (code)
			{
			case MActionResultCodeEnum.MContactInvalid:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "ContactInvalid", "联系人被删除或者禁用");
			case MActionResultCodeEnum.MEmployeeInvalid:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "EmployeeInvalid", "员工被删除或者禁用");
			case MActionResultCodeEnum.MMerItemInvalid:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "MerItemInvalid", "商品项目被删除或者禁用");
			case MActionResultCodeEnum.MExpItemInvalid:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "ExpItemInvalid", "费用项目被删除或者禁用");
			case MActionResultCodeEnum.MPaItemInvalid:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "PaItemInvalid", "工资项目被删除或者禁用");
			case MActionResultCodeEnum.MTrackItemInvalid:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "TrackItemInvalid", "跟踪项被删除或者禁用");
			case MActionResultCodeEnum.MNumberInvalid:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "NumberInvalid", "编号重复或者不合法");
			case MActionResultCodeEnum.MAccountInvalid:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "AccountInvalid", "科目被删除或者禁用");
			case MActionResultCodeEnum.MCurrencyInvalid:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "CurrencyInvalid", "币别被删除或者禁用");
			case MActionResultCodeEnum.MPeriodClosed:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "PeriodClosed", "选择的期间已经结账了");
			case MActionResultCodeEnum.MPeriodBeforeStart:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "PeriodBeforeStart", "选择的期在总账启用之前");
			case MActionResultCodeEnum.MVoucherApproved:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "VoucherApproved", "凭证已审核");
			case MActionResultCodeEnum.MVoucherUnapprove:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "VoucherUnapprove", "凭证未审核");
			case MActionResultCodeEnum.MPeriodTransferExists:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "PeriodTransferExists", "本期的期末结转类型已经存在了，请先删除");
			case MActionResultCodeEnum.MInitBalanceOver:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "InitBalanceOver", "总账未完成初始化");
			case MActionResultCodeEnum.MTrackGroupInvalid:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "TrackGroupInvalid", "跟踪项分组已经被禁用或者删除");
			case MActionResultCodeEnum.MAccountHasSub:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "AccountHasSub", "选择的科目含有子科目");
			case MActionResultCodeEnum.MExpItemHasSub:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "ExpItemHasSub", "选择的费用项目含有子项目");
			case MActionResultCodeEnum.MCheckGroupValueNotMatchWithAccount:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "CheckGroupValueNotMatchWithAccount", "核算维度的值与科目的核算维度不匹配");
			case MActionResultCodeEnum.MVoucherHasNotEntry:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "VoucherHasNotEntry", "凭证分录为空");
			case MActionResultCodeEnum.MCreditDebitImbalance:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "CreditDebitImbalance", "凭证分录借贷方不平衡");
			case MActionResultCodeEnum.MVoucherEntryHasNotAccountOrAccountNotMatchCheckGroup:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "VoucherEntry", "凭证分录没有选择科目或者科目与核算维度值不匹配");
			case MActionResultCodeEnum.MVoucherModuleFastCodeInvalid:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "VoucherModuleFastCodeInvalid", "凭证模板快速码重复了");
			case MActionResultCodeEnum.MCurrencyNotMatchAccount:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "CurrencyNotMatchAccount", "非外币核算的科目录入了外币");
			case MActionResultCodeEnum.MOneDocCreatedMoreThanOneVoucher:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "OneDocCreatedMoreThanOneVoucher", "一张业务单据生成多多张非法凭证");
			case MActionResultCodeEnum.MHasUnsettledPeriodBefore:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "HasUnsettledPeriodBefore", "本期之前有未结账的期");
			case MActionResultCodeEnum.MInvalidDocExsits:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "InvalidDocExsits", "存在不合法的业务单据");
			case MActionResultCodeEnum.MAccountNumberDuplicated:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "AccountNumberDuplicated", "科目编号重复了");
			case MActionResultCodeEnum.MAccountNameDuplicated:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "AccountNameDuplicated", "科目名称重复了");
			case MActionResultCodeEnum.MAccountParentInvalid:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "AccountParentInvalid", "科目父级科目不合法");
			case MActionResultCodeEnum.MAccountNewCheckGroupNotMathWithOldData:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "AccountNewCheckGroupNotMathWithOldData", "科目新的核算维度与历史数据不符");
			case MActionResultCodeEnum.MAccountDetailIncomplete:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "AccountDetailIncomplete", "科目基本信息不全");
			case MActionResultCodeEnum.MVoucherExplanationDuplicated:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "VoucherExplanationDuplicated", "凭证摘要重复");
			case MActionResultCodeEnum.MVoucherExplanationInvalid:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "VoucherExplanationInvalid", "凭证摘要不合法");
			case MActionResultCodeEnum.MVoucherInvalid:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "VoucherInvalid", "凭证不合法");
			case MActionResultCodeEnum.MBillHasAlreadyCreatedVoucher:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "BillHasAlreadyCreatedVoucher", "单据已经生成了凭证，不可重复生成!");
			case MActionResultCodeEnum.ExceptionExist:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "ExceptionExist", "系统出现异常");
			case MActionResultCodeEnum.MVoucherNumberNotMeetDemand:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "MVoucherNumberNotMeetDemand", "凭证编号的位数已达到系统设置的最大值，请【用户在设置-总账设置-凭证设置-凭证编号设置】进行调整!");
			default:
				return code.ToString();
			}
		}

		public static List<string> GetActionExceptionsMessagesByCodes(MContext ctx, List<MActionResultCodeEnum> codes)
		{
			List<string> list = new List<string>();
			if (codes == null || !codes.Any())
			{
				return list;
			}
			codes.ForEach(delegate(MActionResultCodeEnum x)
			{
				list.Add(GetActionExceptionMessageByCode(ctx, x));
			});
			return list;
		}

		public static void HandleActionException<T>(MContext ctx, T result, MActionException ex, bool filterHtml = false)
		{
			ImportResult importResult = result as ImportResult;
			if (importResult != null)
			{
				if (ex.Codes.Any())
				{
					importResult.MessageList.Add(GetActionExceptionMessageByCode(ctx, ex.Codes[0]));
				}
				foreach (string message2 in ex.Messages)
				{
					importResult.MessageList.Add(MText.Decode(Regex.Replace(message2, "<.*?>", "")).RemoveLineBreaks());
				}
			}
			else
			{
				OperationResult operationResult = result as OperationResult;
				if (operationResult != null)
				{
					if (ex.Codes.Any())
					{
						foreach (MActionResultCodeEnum code in ex.Codes)
						{
							operationResult.VerificationInfor.Add(new BizVerificationInfor
							{
								Level = AlertEnum.Error,
								Message = GetActionExceptionMessageByCode(ctx, code)
							});
						}
					}
					foreach (string message3 in ex.Messages)
					{
						string message = message3;
						if (filterHtml)
						{
							message = MText.Decode(Regex.Replace(message3, "<.*?>", "")).RemoveLineBreaks();
						}
						operationResult.VerificationInfor.Add(new BizVerificationInfor
						{
							Level = AlertEnum.Error,
							Message = message
						});
					}
				}
			}
		}
	}
}
