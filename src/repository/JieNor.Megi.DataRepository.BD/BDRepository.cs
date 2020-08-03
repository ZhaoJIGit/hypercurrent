using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.MultiLanguage;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace JieNor.Megi.DataRepository.BD
{
	public static class BDRepository
	{
		public static OperationResult IsCanDelete(MContext ctx, string bizObject, string itemId)
		{
			OperationResult operationResult = new OperationResult();
			string empty = string.Empty;
			string empty2 = string.Empty;
			bool flag = CheckCanDeleteSql(ctx, bizObject, itemId, out empty, out empty2);
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangKey.DataHasReference);
			text = (string.IsNullOrWhiteSpace(empty) ? text : $"({empty}){text}");
			if (flag)
			{
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = text
				});
			}
			else
			{
				operationResult.ObjectID = itemId;
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Success,
					Message = COMMultiLangRepository.GetText(ctx.MLCID, LangKey.DataDeleteSuccess)
				});
			}
			return operationResult;
		}

		public static bool CheckCanDeleteSql(MContext ctx, string bizObject, string itemId, out string name, out string objectName)
		{
			string strSql = "";
			name = "";
			objectName = "";
			switch (bizObject)
			{
			case "Account":
			case "AccountEdit":
				strSql = GetCheckAccountDeleteSql(bizObject);
				name = GetModelMutiNameAndCodeById<BDAccountModel>(ctx, itemId, "MNumber", "MName");
				objectName = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "Account", "科目");
				break;
			case "BankAccount":
				strSql = GetCheckBankDeleteSql();
				name = GetModelMultiNameById<BDAccountModel>(ctx, itemId, "MName");
				objectName = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "BankAccounts", "银行科目");
				break;
			case "Contact":
				strSql = GetCheckContactDeleteSql();
				name = GetModelMultiNameById<BDContactsModel>(ctx, itemId, "MName");
				objectName = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "Contact", "联系人");
				break;
			case "Attachment":
				strSql = GetCheckAttachmentDeleteSql();
				name = GetAttachModelNameById(ctx, itemId);
				break;
			case "Attachment_Category":
				strSql = GetCheckAttachmentCategoryDeleteSql();
				name = GetAttachCategoryModelNameById(ctx, itemId);
				break;
			case "Department":
				strSql = GetCheckDepartmentDeleteSql();
				name = GetModelMultiNameById<BDDepartmentModel>(ctx, itemId, "MName");
				break;
			case "Employees":
				strSql = GetCheckEmployeeDeleteSql();
				name = GlobalFormat.GetUserName(GetModelMultiNameById<BDEmployeesModel>(ctx, itemId, "MFirstName"), GetModelMultiNameById<BDEmployeesModel>(ctx, itemId, "MLastName"), null);
				objectName = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "Employees", "员工");
				break;
			case "Item":
				strSql = GetCheckInventoryItemDeleteSql();
				name = GetModelMutiNameAndCodeById<BDItemModel>(ctx, itemId, "MNumber", "MDesc");
				objectName = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "MerItem", "商品项目");
				break;
			case "ExpenseItem":
				strSql = GetCheckExpenseItemDeleteSql();
				name = GetModelMultiNameById<BDExpenseItemModel>(ctx, itemId, "MName");
				objectName = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "ExpenseItem", "费用项目");
				break;
			case "TaxRate":
				strSql = GetCheckTaxRateDeleteSql();
				name = GetModelMultiNameById<REGTaxRateModel>(ctx, itemId, "MName");
				break;
			case "Currency":
				strSql = GetCheckCurrencyDeleteSql();
				name = GetModelMultiNameById<BASCurrencyModel>(ctx, itemId, "MName");
				break;
			case "ExchangeRate":
				strSql = GetCheckExchangeRateDeleteSql();
				name = GetModelMultiNameById<BDExchangeRateModel>(ctx, itemId, "MName");
				break;
			case "Track":
				strSql = GetCheckTrackDeleteSql(ctx, itemId);
				name = GetModelMultiNameById<BDTrackModel>(ctx, itemId, "MName");
				objectName = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "Tracking", "跟踪项");
				break;
			case "TrackEntry":
				strSql = GetCheckTrackEntryDeleteSql();
				name = GetModelMultiNameById<BDTrackEntryModel>(ctx, itemId, "MName");
				objectName = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "Tracking", "跟踪项");
				break;
			case "PayRun":
				strSql = GetPayItemExistsSql();
				name = GetModelMultiNameById<PAPayItemModel>(ctx, itemId, "MName");
				if (string.IsNullOrEmpty(name))
				{
					name = GetModelMultiNameById<PAPayItemGroupModel>(ctx, itemId, "MName");
				}
				break;
			}
			MySqlParameter[] array = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@ItemID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = itemId;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return dynamicDbHelperMySQL.Exists(strSql, array);
		}

		public static BDIsCanDeleteEntryModel IsCanDeleteOrInactive(MContext ctx, string bizObject, string itemId)
		{
			string empty = string.Empty;
			string empty2 = string.Empty;
			BDIsCanDeleteEntryModel bDIsCanDeleteEntryModel = new BDIsCanDeleteEntryModel();
			bool flag = CheckCanDeleteSql(ctx, bizObject, itemId, out empty, out empty2);
			bDIsCanDeleteEntryModel.Name = empty;
			bDIsCanDeleteEntryModel.ObjectName = empty2;
			bDIsCanDeleteEntryModel.IsCanDelete = !flag;
			bDIsCanDeleteEntryModel.Id = itemId;
			return bDIsCanDeleteEntryModel;
		}

		public static BDIsCanDeleteModel IsCanDeleteOrInactive(MContext ctx, string bizObject, List<string> itemIds, bool isDelete)
		{
			BDIsCanDeleteModel bDIsCanDeleteModel = new BDIsCanDeleteModel();
			List<BDIsCanDeleteEntryModel> list = new List<BDIsCanDeleteEntryModel>();
			foreach (string itemId in itemIds)
			{
				BDIsCanDeleteEntryModel item = IsCanDeleteOrInactive(ctx, bizObject, itemId);
				if (list.All((BDIsCanDeleteEntryModel m) => m.Id != itemId))
				{
					list.Add(item);
				}
			}
			bDIsCanDeleteModel.EntryList = list;
			bDIsCanDeleteModel.Success = true;
			bDIsCanDeleteModel.ObjectName = ((list.FirstOrDefault() == null) ? "" : list.FirstOrDefault().ObjectName);
			bDIsCanDeleteModel.IsDelete = isDelete;
			bDIsCanDeleteModel.BizObjectName = bizObject;
			if (list.All((BDIsCanDeleteEntryModel m) => m.IsCanDelete))
			{
				bDIsCanDeleteModel.AllSuccess = true;
			}
			else if (list.All((BDIsCanDeleteEntryModel m) => !m.IsCanDelete) & isDelete)
			{
				bDIsCanDeleteModel.AllSuccess = false;
				bDIsCanDeleteModel.Success = false;
			}
			else
			{
				bDIsCanDeleteModel.AllSuccess = false;
				if (bizObject == "Account")
				{
					bDIsCanDeleteModel.Success = false;
				}
			}
			return bDIsCanDeleteModel;
		}

		public static OperationResult IsCanDelete(MContext ctx, string bizObject, List<string> itemIds, out List<string> canDeleteIds)
		{
			OperationResult operationResult = new OperationResult();
			List<string> list = new List<string>();
			foreach (string itemId in itemIds)
			{
				OperationResult operationResult2 = IsCanDelete(ctx, bizObject, itemId);
				if (operationResult2.Success)
				{
					list.Add(operationResult2.ObjectID);
				}
				operationResult.VerificationInfor.AddRange(operationResult2.VerificationInfor);
			}
			canDeleteIds = list;
			return operationResult;
		}

		public static OperationResult IsCanDelete(MContext ctx, string bizObject, string itemIds, out List<string> canDeleteIds)
		{
			return IsCanDelete(ctx, bizObject, itemIds.Split(',').ToList(), out canDeleteIds);
		}

		private static string GetModelMutiNameAndCodeById<T>(MContext ctx, string pkId, string fieldCode = "MNumber", string fieldName = "MName") where T : BaseModel
		{
			string text = string.Empty;
			T dataEditModel = ModelInfoManager.GetDataEditModel<T>(ctx, pkId, false, true);
			if (dataEditModel == null || dataEditModel.MultiLanguage == null || dataEditModel.MultiLanguage.Count == 0)
			{
				return text;
			}
			PropertyInfo propertyInfo = typeof(T).GetProperties().FirstOrDefault((PropertyInfo m) => m.Name == fieldCode);
			string str = (propertyInfo == (PropertyInfo)null) ? "" : (propertyInfo.GetValue(dataEditModel) + ":");
			MultiLanguageFieldList multiLanguageFieldList = dataEditModel.MultiLanguage.SingleOrDefault((MultiLanguageFieldList f) => f.MFieldName == fieldName);
			if (multiLanguageFieldList != null)
			{
				MultiLanguageField multiLanguageField = multiLanguageFieldList.MMultiLanguageField.SingleOrDefault((MultiLanguageField f) => f.MLocaleID == ctx.MLCID);
				text = multiLanguageField.MValue;
			}
			return str + text;
		}

		private static string GetModelMultiNameById<T>(MContext ctx, string pkId, string fieldName = "MName") where T : BaseModel
		{
			string result = string.Empty;
			T dataEditModel = ModelInfoManager.GetDataEditModel<T>(ctx, pkId, false, true);
			if (dataEditModel == null || dataEditModel.MultiLanguage == null || dataEditModel.MultiLanguage.Count == 0)
			{
				return result;
			}
			MultiLanguageFieldList multiLanguageFieldList = dataEditModel.MultiLanguage.SingleOrDefault((MultiLanguageFieldList f) => f.MFieldName == fieldName);
			if (multiLanguageFieldList != null)
			{
				MultiLanguageField multiLanguageField = multiLanguageFieldList.MMultiLanguageField.SingleOrDefault((MultiLanguageField f) => f.MLocaleID == ctx.MLCID);
				result = multiLanguageField.MValue;
			}
			return result;
		}

		private static string GetAttachModelNameById(MContext ctx, string pkId)
		{
			BDAttachmentModel dataEditModel = ModelInfoManager.GetDataEditModel<BDAttachmentModel>(ctx, pkId, false, true);
			if (dataEditModel == null)
			{
				return string.Empty;
			}
			return dataEditModel.MName;
		}

		private static string GetAttachCategoryModelNameById(MContext ctx, string pkId)
		{
			BDAttachmentCategoryModel dataEditModel = ModelInfoManager.GetDataEditModel<BDAttachmentCategoryModel>(ctx, pkId, false, true);
			if (dataEditModel == null)
			{
				return string.Empty;
			}
			return dataEditModel.MCategoryName;
		}

		private static string GetCheckAccountDeleteSql(string bizObject)
		{
			string str = "select * from (\r\n               SELECT CASE WHEN (\r\n                 EXISTS(SELECT 1 FROM T_GL_InitBalance WHERE MOrgID=@MOrgID AND MACCOUNTID=@ItemID and (ABS(MInitBalanceFor) + ABS(MInitBalance)+ABS(MYtdDebit)+ABS(MYtdCredit))<>0 \r\n                         and MIsDelete = 0 )\r\n                 OR\r\n                 EXISTS(SELECT 1 FROM T_GL_Balance WHERE MOrgID=@MOrgID AND MACCOUNTID=@ItemID and (ABS(MBeginBalance) + ABS(MDebit)+ABS(MCredit)+ABS(MEndBalance))<>0 and MIsDelete = 0 )\r\n                 OR\r\n                 EXISTS(\r\n                    SELECT 1 from t_gl_voucher a\r\n                          INNER JOIN t_gl_voucherentry b on a.mitemid=b.mid and a.morgid=b.morgid and a.MIsDelete=b.MIsDelete\r\n                      WHERE a.MOrgID=@MOrgID and a.MIsDelete=0 and b.MAccountID=@ItemID  and a.MStatus>=0\r\n                 )\r\n                 OR\r\n                 EXISTS(SELECT 1 FROM t_fc_vouchermoduleentry WHERE MACCOUNTID=@ItemID and MOrgID=@MOrgID and MIsDelete = 0 )\r\n                 OR\r\n                 EXISTS(SELECT 1 from t_bd_contacts where MOrgID=@MOrgID and MIsDelete=0 and \r\n                 (MSCurrentAccountCode=(select MCode from t_bd_account where MOrgID=@MOrgID AND MItemID=@ItemID and MIsDelete = 0 ) \r\n                  or MCCurrentAccountCode=(select MCode from t_bd_account where MOrgID=@MOrgID AND MItemID=@ItemID and MIsDelete = 0 ) \r\n                  or MSaleIncomeAccountCode=(select MCode from t_bd_account where MOrgID=@MOrgID AND MItemID=@ItemID and MIsDelete = 0 )))\r\n                 OR\r\n                 EXISTS(SELECT 1 from t_bd_employees where MOrgID=@MOrgID and MIsDelete = 0  and \r\n                 (MCurrentAccountCode =(select MCode from t_bd_account where MOrgID=@MOrgID AND MItemID=@ItemID and MIsDelete = 0 ) \r\n                 or MExpenseAccountCode=(select MCode from t_bd_account where MOrgID=@MOrgID AND MItemID=@ItemID and MIsDelete = 0 )))\r\n                 OR\r\n                 EXISTS(SELECT 1 from t_bd_expenseitem where MOrgID=@MOrgID and MIsDelete = 0 and \r\n                 MAccountCode=(select MCode from t_bd_account where MOrgID=@MOrgID AND MItemID=@ItemID and MIsDelete = 0 ))\r\n                 OR \r\n                 EXISTS(SELECT 1 from t_bd_item where MOrgID=@MOrgID and MIsDelete = 0  and \r\n                 (MIncomeAccountCode=(select MCode from t_bd_account where MOrgID=@MOrgID AND MItemID=@ItemID and MIsDelete = 0 ) \r\n                   or MIncomeAccountCode=(select MCode from t_bd_account where MOrgID=@MOrgID AND MItemID=@ItemID and MIsDelete = 0 ) \r\n                   or MCostAccountCode=(select MCode from t_bd_account where MOrgID=@MOrgID AND MItemID=@ItemID and MIsDelete = 0 )))\r\n                 OR \r\n                 EXISTS(SELECT 1 from t_pa_payitem where MOrgID=@MOrgID and MIsDelete = 0  and \r\n                        MAccountCode=(select MCode from t_bd_account where MOrgID=@MOrgID AND MItemID=@ItemID and MIsDelete = 0 ))\r\n                 OR\r\n                 EXISTS( SELECT 1 FROM t_fa_fixassets WHERE MOrgID=@MOrgID and MIsDelete = 0  and \r\n                        (MFixAccountCode =(select MCode from t_bd_account where MOrgID=@MOrgID AND MItemID=@ItemID and MIsDelete = 0 ) \r\n                        or MDepAccountCode =(select MCode from t_bd_account where MOrgID=@MOrgID AND MItemID=@ItemID and MIsDelete = 0 ) \r\n                        or MExpAccountCode =(select MCode from t_bd_account where MOrgID=@MOrgID AND MItemID=@ItemID  and MIsDelete = 0)))\r\n                 OR \r\n                 EXISTS( \r\n                     SELECT 1 FROM t_fa_fixassetstype WHERE MOrgID=@MOrgID and MIsDelete = 0  and \r\n                        (MFixAccountCode =(select MCode from t_bd_account where MOrgID=@MOrgID AND MItemID=@ItemID and MIsDelete = 0 ) \r\n                        or MDepAccountCode =(select MCode from t_bd_account where MOrgID=@MOrgID AND MItemID=@ItemID and MIsDelete = 0 ) \r\n                        or MExpAccountCode =(select MCode from t_bd_account where MOrgID=@MOrgID AND MItemID=@ItemID  and MIsDelete = 0)))\r\n                 OR\r\n                 EXISTS( \r\n                     SELECT 1 FROM t_fa_fixassetschange WHERE MOrgID=@MOrgID and MIsDelete = 0  and \r\n                        (MFixAccountCode =(select MCode from t_bd_account where MOrgID=@MOrgID AND MItemID=@ItemID and MIsDelete = 0 ) \r\n                        or MDepAccountCode =(select MCode from t_bd_account where MOrgID=@MOrgID AND MItemID=@ItemID and MIsDelete = 0 ) \r\n                        or MExpAccountCode =(select MCode from t_bd_account where MOrgID=@MOrgID AND MItemID=@ItemID and MIsDelete = 0)))\r\n                OR\r\n                 EXISTS( \r\n                     SELECT 1 FROM t_fc_fapiaomodule WHERE MOrgID=@MOrgID and MIsDelete = 0  and \r\n                        (MDebitAccount =@ItemID \r\n                        or MCreditAccount =@ItemID\r\n                        or MTaxAccount =@ItemID ))\r\n                OR\r\n                EXISTS( SELECT 1 from t_fc_cashcodingmodule where MAccountID=@ItemID and MOrgID=@MOrgID and MIsDelete=0 )\r\n                OR\r\n                 EXISTS( \r\n                     SELECT 1 FROM t_fp_coding WHERE MOrgID=@MOrgID and MIsDelete = 0  and \r\n                        (MDebitAccount =@ItemID \r\n                        or MCreditAccount =@ItemID\r\n                        or MTaxAccount =@ItemID ))\r\n                 OR\r\n                 EXISTS( \r\n                   SELECT 1 FROM t_fa_depreciation WHERE MOrgID=@MOrgID and MIsDelete = 0  and \r\n                       (MFixAccountCode =(select MCode from t_bd_account where MOrgID=@MOrgID AND MItemID=@ItemID and MIsDelete = 0 ) \r\n                        or MDepAccountCode =(select MCode from t_bd_account where MOrgID=@MOrgID AND MItemID=@ItemID and MIsDelete = 0 ) \r\n                        or MExpAccountCode =(select MCode from t_bd_account where MOrgID=@MOrgID AND MItemID=@ItemID  and MIsDelete = 0)))\r\n                 OR\r\n                 EXISTS(SELECT 1 from t_pa_payitemgroup where MOrgID=@MOrgID and MIsDelete = 0  and \r\n                        MAccountCode=(select MCode from t_bd_account where MOrgID=@MOrgID AND MItemID=@ItemID and MIsDelete = 0 ))\r\n                 OR\r\n                 EXISTS(SELECT 1 from t_reg_taxrate where MOrgID=@MOrgID and MIsDelete = 0  and \r\n                        (MSaleTaxAccountCode=(select MCode from t_bd_account where MOrgID=@MOrgID AND MItemID=@ItemID and MIsDelete = 0 ) \r\n                        or MPurchaseAccountCode=(select MCode from t_bd_account where MOrgID=@MOrgID AND MItemID=@ItemID and MIsDelete = 0 ) \r\n                        or MPayDebitAccountCode=(select MCode from t_bd_account where MOrgID=@MOrgID AND MItemID=@ItemID and MIsDelete = 0 )))";
			return str + " ) THEN 1 ELSE 0 END AS Result \r\n             )  t where t.Result=1";
		}

		private static string GetCheckBankDeleteSql()
		{
			return "SELECT 1 FROM T_IV_Receive WHERE MOrgID=@MOrgID AND MBankID=@ItemID AND MIsDelete=0 \r\n                    UNION\r\n                     SELECT 1 FROM T_IV_Payment WHERE MOrgID=@MOrgID AND MBankID=@ItemID AND MIsDelete=0 \r\n                    UNION\r\n                     SELECT 1 FROM T_IV_Transfer WHERE MOrgID=@MOrgID AND (MFromAcctID=@ItemID OR MToAcctID=@ItemID) AND MIsDelete=0 \r\n                    UNION\r\n                     SELECT 1 FROM T_IV_BankBill WHERE MOrgID=@MOrgID AND MBankID=@ItemID AND MIsDelete=0 \r\n                    UNION\r\n                     select 1 from t_gl_initbankbalance where MOrgID=@MOrgID AND MAccountID=@ItemID AND MIsDelete=0  AND MBeginBalanceFor<>0 and MBeginBalance<>0\r\n                    UNION\r\n                     select 1 from t_gl_initbalance where MOrgID=@MOrgID AND MACCOUNTID=@ItemID AND MIsDelete=0  AND MACCOUNTID=@ItemID and (ABS(MInitBalanceFor) + ABS(MInitBalance)+ABS(MYtdDebit)+ABS(MYtdCredit))<>0 \r\n                    UNION                     \r\n                     SELECT 1 FROM T_GL_Balance WHERE MOrgID=@MOrgID AND MACCOUNTID=@ItemID AND MIsDelete=0 ";
		}

		private static string GetCheckContactDeleteSql()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("select * from (\r\n                SELECT CASE WHEN \r\n                (\r\n                    EXISTS(SELECT 1 FROM T_IV_Invoice WHERE MOrgID=@MOrgID AND MContactID=@ItemID AND MIsDelete=0  )\r\n                    OR EXISTS(SELECT 1 FROM T_IV_Expense WHERE MOrgID=@MOrgID AND MContactID=@ItemID AND MIsDelete=0 )\r\n                    OR EXISTS(SELECT 1 FROM T_IV_Receive WHERE MOrgID=@MOrgID AND MContactID=@ItemID AND MIsDelete=0 )\r\n                    OR EXISTS(SELECT 1 FROM T_IV_Payment WHERE MOrgID=@MOrgID AND MContactID=@ItemID AND MIsDelete=0 )\r\n                    OR EXISTS(SELECT 1 FROM T_FP_Fapiao WHERE MOrgID=@MOrgID AND MContactID=@ItemID AND MIsDelete=0 )\r\n                    OR EXISTS(SELECT 1 FROM T_FP_Coding WHERE MOrgID=@MOrgID AND MContactID=@ItemID AND MIsDelete=0 )\r\n                    OR EXISTS(SELECT 1 FROM T_FP_Table WHERE MOrgID=@MOrgID AND MContactID=@ItemID AND MIsDelete=0 )\r\n                    OR EXISTS(SELECT 1 FROM T_FP_Coding WHERE MOrgID=@MOrgID AND MContactID=@ItemID AND MIsDelete=0 )\r\n                    OR EXISTS(select 1 FROM t_GL_voucherentry  where MOrgID=@MOrgID  and MIsDelete=0 and MContactId=@ItemID)\r\n                    OR EXISTS(select 1 FROM t_fc_vouchermoduleentry  where MOrgID=@MOrgID  and MIsDelete=0 and MContactId=@ItemID)\r\n                    OR EXISTS(select 1 FROM t_iv_repeatinvoice  where MOrgID=@MOrgID  and MIsDelete=0 and MContactId=@ItemID)  \r\n                    OR EXISTS(SELECT 1 from t_fc_cashcodingmodule where substring(MContactID,1,length(MContactID)-2)=@ItemID and MOrgID=@MOrgID and MIsDelete=0)\r\n                    ");
			stringBuilder.AppendLine(GetBaseDataRelationCheckTyepValueExistsSql("MContactID"));
			stringBuilder.AppendLine(") THEN 1 ELSE 0 END AS Result \r\n                                )  t where t. Result=1\r\n                                ");
			return stringBuilder.ToString();
		}

		private static string GetCheckAttachmentDeleteSql()
		{
			return "SELECT 1 FROM T_IV_Invoice t1 JOIN T_IV_InvoiceAttachment t2 ON t1.MOrgID = t2.MOrgID and  t1.MID=t2.MParentID\r\n                     WHERE MOrgID=@MOrgID AND MIsDelete=0 AND MAttachID=@ItemID\r\n                    UNION\r\n                     SELECT 1 FROM T_IV_Expense t1 JOIN T_IV_ExpenseAttachment t2 ON t1.MOrgID = t2.MOrgID and t1.MID=t2.MParentID\r\n                     WHERE MOrgID=@MOrgID AND MIsDelete=0  AND MAttachID=@ItemID\r\n                    UNION\r\n                     SELECT 1 FROM T_IV_Receive t1 JOIN T_IV_ReceiveAttachment t2 ON t1.MOrgID = t2.MOrgID and t1.MID=t2.MParentID\r\n                     WHERE MOrgID=@MOrgID AND MIsDelete=0  AND MAttachID=@ItemID\r\n                    UNION\r\n                     SELECT 1 FROM T_IV_Payment t1 JOIN T_IV_PaymentAttachment t2 ON t1.MOrgID = t2.MOrgID and t1.MID=t2.MParentID\r\n                     WHERE MOrgID=@MOrgID AND MIsDelete=0  AND MAttachID=@ItemID\r\n                    UNION\r\n                     SELECT 1 FROM T_GL_Voucher t1 JOIN T_GL_VoucherAttachment t2 ON t1.MOrgID = t2.MOrgID and t1.MItemID=t2.MParentID\r\n                     WHERE MOrgID=@MOrgID AND MIsDelete=0 AND MAttachID=@ItemID";
		}

		private static string GetCheckAttachmentCategoryDeleteSql()
		{
			return "SELECT 1 FROM T_IV_Invoice t1 JOIN T_IV_InvoiceAttachment t2 ON t1.MOrgID = t2.MOrgID and t1.MID=t2.MParentID JOIN T_BD_Attachment t3 ON t2.MAttachID=t3.MItemID\r\n                     WHERE t1.MOrgID=@MOrgID AND t1.MIsDelete=0  AND MCategoryID=@ItemID \r\n                    UNION\r\n                     SELECT 1 FROM T_IV_Expense t1 JOIN T_IV_ExpenseAttachment t2 ON t1.MOrgID = t2.MOrgID and t1.MID=t2.MParentID JOIN T_BD_Attachment t3 ON t2.MAttachID=t3.MItemID\r\n                     WHERE t1.MOrgID=@MOrgID AND t1.MIsDelete=0  AND MCategoryID=@ItemID\r\n                    UNION\r\n                     SELECT 1 FROM T_IV_Receive t1 JOIN T_IV_ReceiveAttachment t2 ON t1.MOrgID = t2.MOrgID and t1.MID=t2.MParentID JOIN T_BD_Attachment t3 ON t2.MAttachID=t3.MItemID\r\n                     WHERE t1.MOrgID=@MOrgID AND t1.MIsDelete=0  AND MCategoryID=@ItemID\r\n                    UNION\r\n                     SELECT 1 FROM T_IV_Payment t1 JOIN T_IV_PaymentAttachment t2 ON t1.MOrgID = t2.MOrgID and t1.MID=t2.MParentID JOIN T_BD_Attachment t3 ON t2.MAttachID=t3.MItemID\r\n                     WHERE t1.MOrgID=@MOrgID AND t1.MIsDelete=0  AND MCategoryID=@ItemID\r\n                    UNION\r\n                     SELECT 1 FROM T_GL_Voucher t1 JOIN T_GL_VoucherAttachment t2 ON t1.MOrgID = t2.MOrgID and t1.MItemID=t2.MParentID JOIN T_BD_Attachment t3 ON t2.MAttachID=t3.MItemID\r\n                     WHERE t1.MOrgID=@MOrgID AND t1.MIsDelete=0  AND MCategoryID=@ItemID";
		}

		private static string GetCheckDepartmentDeleteSql()
		{
			return "SELECT 1 FROM T_IV_Expense WHERE MOrgID=@MOrgID AND MDepartment=@ItemID AND MIsDelete=0 \r\n                    UNION\r\n                     SELECT 1 FROM T_IV_Payment WHERE MOrgID=@MOrgID AND MDepartment=@ItemID AND MIsDelete=0 ";
		}

		private static string GetCheckEmployeeDeleteSql()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(" SELECT 1 FROM T_IV_Expense t1 WHERE t1.MOrgID=@MOrgID AND t1.MEmployee=@ItemID AND t1.MIsDelete=0  ");
			stringBuilder.AppendLine(" UNION ");
			stringBuilder.AppendLine(" SELECT 1 FROM T_PA_SalaryPayment t1 WHERE t1.MOrgID = @MOrgID and t1.MRunID in (SELECT t2.MID FROM T_PA_PayRun t2 WHERE t2.MOrgID=@MOrgID and MIsDelete = 0 ) AND t1.MEmployeeID=@ItemID AND t1.MIsDelete=0 ");
			stringBuilder.AppendLine(" UNION ");
			stringBuilder.AppendLine(" SELECT 1 from t_iv_payment where MorgID =@MOrgID and MContactID=@ItemID and MIsDelete=0  ");
			stringBuilder.AppendLine(" UNION  ");
			stringBuilder.AppendLine(" SELECT 1 from t_fc_cashcodingmodule where substring(MContactID, 1, length(MContactID) - 2) = @ItemID and MOrgID = @MOrgID and MIsDelete = 0 ");
			stringBuilder.AppendLine(GetBaseDataRelationCheckTyepValueSql("MEmployeeID"));
			return stringBuilder.ToString();
		}

		private static string GetCheckInventoryItemDeleteSql()
		{
			string str = " SELECT 1 FROM T_IV_Invoice t1 JOIN T_IV_InvoiceEntry t2 ON t1.MID=t2.MID and t1.MOrgID = t2.MOrgID\r\n                     WHERE t1.MOrgID=@MOrgID AND t2.MItemID=@ItemID AND t1.MIsDelete=0   AND t2.MIsDelete=0 \r\n                    UNION\r\n                     SELECT 1 FROM T_IV_Receive t1 JOIN T_IV_ReceiveEntry t2 ON t1.MID=t2.MID and t1.MOrgID = t2.MOrgID\r\n                     WHERE t1.MOrgID=@MOrgID AND t2.MItemID=@ItemID AND t1.MIsDelete=0   AND t2.MIsDelete=0 \r\n                    UNION\r\n                     SELECT 1 FROM T_FP_Coding t1\r\n                     WHERE t1.MOrgID=@MOrgID AND t1.MMerItemID=@ItemID AND t1.MIsDelete=0\r\n                    UNION\r\n                     SELECT 1 FROM T_FC_FapiaoModule t1\r\n                     WHERE t1.MOrgID=@MOrgID AND t1.MMerItemID=@ItemID AND t1.MIsDelete=0\r\n                    UNION\r\n                     SELECT 1 FROM T_IV_Payment t1 JOIN T_IV_PaymentEntry t2 ON t1.MID=t2.MID and t1.MOrgID = t2.MOrgID\r\n                     WHERE t1.MOrgID=@MOrgID AND t2.MItemID=@ItemID AND t1.MIsDelete=0   AND t2.MIsDelete=0 \r\n                    UNION\r\n                     SELECT 1 FROM t_fp_fapiao t1 JOIN t_fp_fapiaoentry t2 ON  t1.MID = t2.MID AND t1.MIsDelete =t2.MIsDelete\r\n                     WHERE t1.MOrgID=@MOrgID AND t2.MItemID=@ItemID AND t1.MIsDelete=0 \r\n                    UNION\r\n                     SELECT 1 from t_iv_repeatinvoiceentry where MOrgID=@MOrgID and MItemID=@ItemID and MIsDelete=0 ";
			return str + GetBaseDataRelationCheckTyepValueSql("MMerItemID");
		}

		private static string GetCheckExpenseItemDeleteSql()
		{
			string str = "SELECT 1 FROM T_IV_Expense t1 JOIN T_IV_ExpenseEntry t2 ON t1.MID=t2.MID and t1.MOrgID = t2.MOrgID AND t2.MIsDelete=0 \r\n                     WHERE t1.MOrgID=@MOrgID AND t2.MItemID=@ItemID AND t1.MIsDelete=0 \r\n                    UNION\r\n                     SELECT 1 FROM T_IV_Payment t1 JOIN T_IV_PaymentEntry t2 ON t1.MID=t2.MID and t1.MOrgID = t2.MOrgID  AND t2.MIsDelete=0 \r\n                     WHERE t1.MOrgID= @MOrgID AND t2.MItemID=@ItemID AND t1.MIsDelete=0 ";
			return str + GetBaseDataRelationCheckTyepValueSql("MExpItemID");
		}

		private static string GetCheckTaxRateDeleteSql()
		{
			return "SELECT 1 FROM T_IV_Invoice t1 JOIN T_IV_InvoiceEntry t2 ON t1.MID=t2.MID and t1.MOrgID = t2.MOrgID  AND t2.MIsDelete=0 \r\n                     WHERE t1.MOrgID=@MOrgID AND t2.MTaxID=@ItemID AND t1.MIsDelete=0 \r\n                    UNION\r\n                     SELECT 1 FROM T_IV_Receive t1 JOIN T_IV_ReceiveEntry t2 ON t1.MID=t2.MID and t1.MOrgID = t2.MOrgID  AND t2.MIsDelete=0 \r\n                     WHERE t1.MOrgID=@MOrgID AND t2.MTaxID=@ItemID AND t1.MIsDelete=0 \r\n                    UNION\r\n                     SELECT 1 FROM T_IV_Payment t1 JOIN T_IV_PaymentEntry t2 ON t1.MID=t2.MID and t1.MOrgID = t2.MOrgID  AND t2.MIsDelete=0 \r\n                     WHERE t1.MOrgID=@MOrgID AND t2.MTaxID=@ItemID AND t1.MIsDelete=0\r\n                    UNION\r\n                     SELECT 1 from t_fc_cashcodingmodule where MTaxID=@ItemID and MOrgID=@MOrgID and MIsDelete=0 \r\n                    UNION \r\n\t\t\t\t     SELECT 1 from T_BD_Item t1 \r\n                     where t1.MOrgID=@MOrgID  AND t1.MIsDelete=0 AND t1.MIsActive=1 and (t1.MSalTaxTypeID = @ItemID or t1.MPurTaxTypeID=@ItemID)\r\n                    UNION\r\n                     select 1 FROM t_iv_repeatinvoiceentry  WHERE MOrgID=@MOrgID  and MIsDelete=0 and MTaxID=@ItemID \r\n                    UNION \r\n\t\t\t\t     SELECT 1 FROM T_BD_Contacts t1 \r\n                     WHERE t1.MOrgID=@MOrgID AND t1.MIsDelete=0 AND t1.MIsActive=1 and (t1.MSalTaxTypeID = @ItemID or t1.MPurTaxTypeID=@ItemID)";
		}

		private static string GetCheckCurrencyDeleteSql()
		{
			return "SELECT 1 FROM T_IV_Invoice a WHERE a.MOrgID=@MOrgID AND a.MCyID=@ItemID AND a.MIsDelete=0 \r\n                    UNION\r\n                     SELECT 1 FROM T_IV_Expense b WHERE b.MOrgID=@MOrgID AND b.MCyID=@ItemID AND b.MIsDelete=0 \r\n                    UNION\r\n                     SELECT 1 FROM T_IV_Receive c WHERE c.MOrgID=@MOrgID AND c.MCyID=@ItemID AND c.MIsDelete=0 \r\n                    UNION\r\n                     SELECT 1 FROM T_IV_Payment d WHERE d.MOrgID=@MOrgID AND d.MCyID=@ItemID AND d.MIsDelete=0 \r\n                    UNION\r\n                     SELECT 1 FROM T_BD_BankAccount f WHERE f.MOrgID=@MOrgID AND f.MCyID=@ItemID AND f.MIsDelete=0 \r\n                    UNION\r\n                     SELECT 1 FROM T_BD_ExchangeRate g WHERE g.MOrgID=@MOrgID AND (g.MSourceCurrencyID=(SELECT gg.MCurrencyID  from t_reg_currency gg where gg.MItemID=@ItemID and MOrgID = @MOrgID and MIsDelete = 0) OR g.MTargetCurrencyID=(SELECT ggg.MCurrencyID  from t_reg_currency ggg where ggg.MItemID=@ItemID and ggg.MOrgID = @MOrgID and ggg.MIsDelete = 0 )) AND g.MIsDelete=0 ";
		}

		private static string GetCheckExchangeRateDeleteSql()
		{
			return "SELECT 1 FROM T_BD_ExchangeRate a where a.mitemid is null and MOrgID = @MOrgID and MIsDelete = 0 ";
		}

		private static string GetCheckTrackDeleteSql(MContext ctx, string trackId)
		{
			List<string> list = GetNeedValidateTrackOptions(ctx, trackId);
			if (list == null || list.Count() == 0)
			{
				list = new List<string>();
				list.Add("-1");
			}
			string sqlWhere = string.Format("IN ({0})", "'" + string.Join("','", list) + "'");
			string checkTrackCommonDeleteSql = GetCheckTrackCommonDeleteSql(sqlWhere);
			int trackIndex = new BDTrackRepository().GetTrackIndex(ctx, trackId);
			checkTrackCommonDeleteSql += " UNION ";
			return checkTrackCommonDeleteSql + string.Format(" SELECT 1 from t_bd_account a\r\n                        INNER JOIN\r\n                        t_gl_checkgroup b on a.MCheckGroupID = b.MItemID\r\n                        where MOrgID = @MOrgID and ({0}) and a.MIsDelete = 0 and b.MIsDelete=0 ", GetTrackIndexFilter(trackIndex, "b"));
		}

		private static string GetTrackIndexFilter(int trackIndex, string tableAlias)
		{
			//string text = "";
			List<string> list = new List<string>();
			for (int i = trackIndex; i <= 5; i++)
			{
				list.Add($" {tableAlias}.MTrackItem{i} <> 0 ");
			}
			return string.Join(" OR ", list);
		}

		private static List<string> GetNeedValidateTrackOptions(MContext ctx, string trackId)
		{
			List<string> list = new List<string>();
			string sql = "SELECT a.MEntryID FROM T_BD_TrackEntry a WHERE a.MItemID=@ItemID and a.MOrgID = @MOrgID AND a.MIsDelete=0 \r\n                           UNION\r\n                           SELECT MEntryID from t_bd_trackentry  where \r\n                           MItemID in (SELECT MItemID from t_bd_track where MIsDelete=0 and MOrgID=@MOrgID and\r\n                           MCreateDate > (SELECT MCreateDate from t_bd_track where MItemID=@ItemID and MOrgID=@MOrgID and MIsDelete=0)) \r\n                           and MOrgID=@MOrgID and MIsDelete=0 ";
			MySqlParameter[] array = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@ItemID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = trackId;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(sql, array);
			if (dataSet != null && dataSet.Tables[0].Rows.Count > 0)
			{
				DataTable dataTable = dataSet.Tables[0];
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					string text = Convert.ToString(dataTable.Rows[i]["MEntryID"]);
					if (!string.IsNullOrWhiteSpace(text))
					{
						list.Add(text);
					}
				}
			}
			return list;
		}

		private static string GetCheckTrackEntryDeleteSql()
		{
			return GetCheckTrackCommonDeleteSql("=@ItemID");
		}

		private static string GetCheckTrackCommonDeleteSql(string sqlWhere)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT 1 FROM T_IV_InvoiceEntry t2 \r\n                                   WHERE t2.MOrgID=@MOrgID AND t2.MIsDelete=0  AND \r\n                                   (t2.MTrackItem1 {0} OR t2.MTrackItem2 {0} OR t2.MTrackItem3 {0} OR t2.MTrackItem4 {0} OR t2.MTrackItem5 {0})\r\n                                   UNION\r\n                                   SELECT 1 FROM T_IV_ExpenseEntry t2 \r\n                                    WHERE t2.MOrgID=@MOrgID AND t2.MIsDelete=0 AND \r\n                                  (t2.MTrackItem1 {0} OR t2.MTrackItem2 {0} OR t2.MTrackItem3 {0} OR t2.MTrackItem4 {0} OR t2.MTrackItem5 {0})\r\n                                   UNION\r\n                                   SELECT 1 FROM  T_IV_ReceiveEntry t2 \r\n                                   WHERE  t2.MOrgID=@MOrgID AND t2.MIsDelete=0  AND \r\n                                   (t2.MTrackItem1 {0} OR t2.MTrackItem2 {0} OR t2.MTrackItem3 {0} OR t2.MTrackItem4 {0} OR t2.MTrackItem5 {0})\r\n                                   UNION\r\n                                   SELECT 1 FROM  T_IV_PaymentEntry t2 \r\n                                    WHERE t2.MOrgID=@MOrgID AND t2.MIsDelete=0  AND \r\n                                   (t2.MTrackItem1 {0} OR t2.MTrackItem2 {0} OR t2.MTrackItem3 {0} OR t2.MTrackItem4 {0} OR t2.MTrackItem5 {0})\r\n                                   UNION\r\n                                   SELECT 1 FROM  T_IV_BankBillEntry t2 \r\n                                    WHERE t2.MOrgID=@MOrgID AND t2.MIsDelete=0  AND \r\n                                   (t2.MTrackItem1 {0} OR t2.MTrackItem2 {0} OR t2.MTrackItem3 {0} OR t2.MTrackItem4 {0} OR t2.MTrackItem5 {0})\r\n                                    UNION\r\n                                    SELECT 1 from t_fc_vouchermoduleentry t2 \r\n                                    WHERE t2.MOrgID=@MOrgID AND t2.MIsDelete=0  AND \r\n                                   (t2.MTrackItem1 {0} OR t2.MTrackItem2 {0} OR t2.MTrackItem3 {0} OR t2.MTrackItem4 {0} OR t2.MTrackItem5 {0})                 \r\n                                    UNION\r\n                                    SELECT 1 from t_fc_cashcodingmodule where ( MTrackItem1 {0} OR MTrackItem2 {0} OR MTrackItem3 {0} OR MTrackItem4 {0} OR MTrackItem5 {0}) and MOrgID=@MOrgID and MIsDelete=0 \r\n                                    UNION\r\n                                    SELECT 1 from t_fc_fapiaomodule t2 \r\n                                    WHERE t2.MOrgID=@MOrgID AND t2.MIsDelete=0  AND \r\n                                   (t2.MTrackItem1 {0} OR t2.MTrackItem2 {0} OR t2.MTrackItem3 {0} OR t2.MTrackItem4 {0} OR t2.MTrackItem5 {0})\r\n                                    UNION\r\n                                    SELECT 1 from t_fp_coding t2 \r\n                                    WHERE t2.MOrgID=@MOrgID AND t2.MIsDelete=0  AND \r\n                                   (t2.MTrackItem1 {0} OR t2.MTrackItem2 {0} OR t2.MTrackItem3 {0} OR t2.MTrackItem4 {0} OR t2.MTrackItem5 {0})\r\n                                    UNION\r\n                                    SELECT 1 from  t_gl_voucherentry t2 \r\n                                    WHERE t2.MOrgID=@MOrgID AND t2.MIsDelete=0  AND \r\n                                   (t2.MTrackItem1 {0} OR t2.MTrackItem2 {0} OR t2.MTrackItem3 {0} OR t2.MTrackItem4 {0} OR t2.MTrackItem5 {0})\r\n                                    UNION\r\n                                    SELECT 1 FROM t_bd_contacts t2\r\n                                    INNER JOIN t_bd_contactstracklink t3 ON t2.MOrgID = t3.MOrgID AND t2.MIsDelete = 0\r\n                                    AND t2.MItemID = t3.MContactID AND t3.MIsDelete = 0\r\n                                    WHERE t2.MOrgID = @MOrgID AND (t3.MSalTrackId {0} or t3.MPurTrackId {0})\r\n                                   UNION\r\n                                    SELECT 1 FROM t_iv_repeatinvoiceentry WHERE  MOrgID=@MOrgID AND MIsDelete=0  AND \r\n                                   (MTrackItem1 {0} OR MTrackItem2 {0} OR MTrackItem3 {0} OR MTrackItem4 {0} OR MTrackItem5 {0})\r\n                                   ", sqlWhere);
			stringBuilder.AppendFormat("UNION\r\n                               SELECT 1 from (\r\n                                    SELECT c.MTrackItem1 , c.MTrackItem2 , c.MTrackItem3 , c.MTrackItem4 , c.MTrackItem5 ,c.MOrgID from t_bd_account a\r\n                                    INNER JOIN t_gl_initbalance b on a.MItemID =b.MAccountID and a.MOrgID=b.MOrgID  and b.MIsDelete=0 and (ABS(b.MInitBalanceFor) + ABS(b.MInitBalance)+ABS(b.MYtdDebit)+ABS(b.MYtdCredit))<>0\r\n                                    INNER JOIN t_gl_checkgroupvalue c on b.MCheckGroupValueID = c.MItemID and a.MOrgID=c.MOrgID\r\n                                WHERE a.MOrgID=@MOrgID and a.MIsDelete=0 and b.MIsDelete=0\r\n                                UNION\r\n                                SELECT c.MTrackItem1 , c.MTrackItem2 , c.MTrackItem3 , c.MTrackItem4 , c.MTrackItem5 ,c.MOrgID from t_bd_account a\r\n                                    INNER JOIN t_gl_balance b on a.MItemID =b.MAccountID and a.MOrgID=b.MOrgID and b.MIsDelete=0 and (ABS(b.MBeginBalance) + ABS(b.MDebit)+ABS(b.MCredit)+ABS(b.MEndBalance))<>0\r\n                                    INNER JOIN t_gl_checkgroupvalue c on b.MCheckGroupValueID = c.MItemID and a.MOrgID=c.MOrgID\r\n                                WHERE a.MOrgID=@MOrgID and a.MIsDelete=0\r\n                                UNION\r\n                                SELECT c.MTrackItem1 , c.MTrackItem2 , c.MTrackItem3 , c.MTrackItem4 , c.MTrackItem5,c.MOrgID from t_bd_account a\r\n                                    INNER JOIN t_gl_voucherentry b on a.MItemID =b.MAccountID and a.MOrgID=b.MOrgID  and b.MIsDelete=0\r\n                                    INNER JOIN t_gl_checkgroupvalue c on b.MCheckGroupValueID = c.MItemID and a.MOrgID=c.MOrgID\r\n                                WHERE a.MOrgID=@MOrgID and a.MIsDelete=0\r\n\r\n\r\n                                UNION\r\n                                SELECT c.MTrackItem1 , c.MTrackItem2 , c.MTrackItem3 , c.MTrackItem4 , c.MTrackItem5,c.MOrgID from t_fa_fixassets a\r\n                                    INNER JOIN t_gl_checkgroupvalue c on a.MFixCheckGroupValueID = c.MItemID and a.MOrgID=c.MOrgID\r\n                                WHERE a.MOrgID=@MOrgID and a.MIsDelete=0\r\n                                UNION\r\n                                SELECT c.MTrackItem1 , c.MTrackItem2 , c.MTrackItem3 , c.MTrackItem4 , c.MTrackItem5,c.MOrgID from t_fa_fixassets a\r\n                                    INNER JOIN t_gl_checkgroupvalue c on a.MDepCheckGroupValueID = c.MItemID and a.MOrgID=c.MOrgID\r\n                                WHERE a.MOrgID=@MOrgID and a.MIsDelete=0\r\n                                UNION\r\n                                SELECT c.MTrackItem1 , c.MTrackItem2 , c.MTrackItem3 , c.MTrackItem4 , c.MTrackItem5,c.MOrgID from t_fa_fixassets a\r\n                                    INNER JOIN t_gl_checkgroupvalue c on a.MExpCheckGroupValueID = c.MItemID and a.MOrgID=c.MOrgID\r\n                                WHERE a.MOrgID=@MOrgID and a.MIsDelete=0\r\n\r\n\r\n                                UNION\r\n                                SELECT c.MTrackItem1 , c.MTrackItem2 , c.MTrackItem3 , c.MTrackItem4 , c.MTrackItem5,c.MOrgID from t_fa_fixassetstype a\r\n                                    INNER JOIN t_gl_checkgroupvalue c on a.MFixCheckGroupValueID = c.MItemID and a.MOrgID=c.MOrgID\r\n                                WHERE a.MOrgID=@MOrgID and a.MIsDelete=0\r\n                                UNION\r\n                                SELECT c.MTrackItem1 , c.MTrackItem2 , c.MTrackItem3 , c.MTrackItem4 , c.MTrackItem5,c.MOrgID from t_fa_fixassetstype a\r\n                                    INNER JOIN t_gl_checkgroupvalue c on a.MDepCheckGroupValueID = c.MItemID and a.MOrgID=c.MOrgID\r\n                                WHERE a.MOrgID=@MOrgID and a.MIsDelete=0\r\n                                UNION\r\n                                SELECT c.MTrackItem1 , c.MTrackItem2 , c.MTrackItem3 , c.MTrackItem4 , c.MTrackItem5,c.MOrgID from t_fa_fixassetstype a\r\n                                    INNER JOIN t_gl_checkgroupvalue c on a.MExpCheckGroupValueID = c.MItemID and a.MOrgID=c.MOrgID\r\n                                WHERE a.MOrgID=@MOrgID and a.MIsDelete=0\r\n\r\n                                UNION\r\n                                SELECT c.MTrackItem1 , c.MTrackItem2 , c.MTrackItem3 , c.MTrackItem4 , c.MTrackItem5,c.MOrgID from t_fa_depreciation a\r\n                                    INNER JOIN t_gl_checkgroupvalue c on a.MFixCheckGroupValueID = c.MItemID and a.MOrgID=c.MOrgID\r\n                                WHERE a.MOrgID=@MOrgID and a.MIsDelete=0\r\n                                UNION\r\n                                SELECT c.MTrackItem1 , c.MTrackItem2 , c.MTrackItem3 , c.MTrackItem4 , c.MTrackItem5,c.MOrgID from t_fa_depreciation a\r\n                                    INNER JOIN t_gl_checkgroupvalue c on a.MDepCheckGroupValueID = c.MItemID and a.MOrgID=c.MOrgID\r\n                                WHERE a.MOrgID=@MOrgID and a.MIsDelete=0\r\n                                UNION\r\n                                SELECT c.MTrackItem1 , c.MTrackItem2 , c.MTrackItem3 , c.MTrackItem4 , c.MTrackItem5,c.MOrgID from t_fa_depreciation a\r\n                                    INNER JOIN t_gl_checkgroupvalue c on a.MExpCheckGroupValueID = c.MItemID and a.MOrgID=c.MOrgID\r\n                                WHERE a.MOrgID=@MOrgID and a.MIsDelete=0\r\n                                UNION\r\n                                SELECT c.MTrackItem1 , c.MTrackItem2 , c.MTrackItem3 , c.MTrackItem4 , c.MTrackItem5 ,c.MOrgID from t_bd_account a\r\n                                   INNER JOIN t_fc_vouchermoduleentry b on a.MItemID =b.MAccountID and a.MOrgID=b.MOrgID  and b.MIsDelete=0\r\n                                   INNER JOIN t_gl_checkgroupvalue c on b.MCheckGroupValueID = c.MItemID and a.MOrgID=c.MOrgID\r\n                               WHERE a.MOrgID=@MOrgID and a.MIsDelete=0) v1\r\n                                where v1.MTrackItem1 {0} or v1.MTrackItem2 {0} or MTrackItem3 {0} or MTrackItem4 {0} or MTrackItem5 {0}", sqlWhere);
			return stringBuilder.ToString();
		}

		public static int ExecuteSqlTran(MContext ctx, List<CommandInfo> cmdList)
		{
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return dynamicDbHelperMySQL.ExecuteSqlTran(cmdList);
		}

		private static string GetBaseDataRelationCheckTyepValueSql(string checkTypeValueFeildName)
		{
			return string.Format(" \r\n                     UNION\r\n                     select 1 from t_gl_checkgroupvalue a\r\n                     inner join t_gl_initBalance b on a.MOrgID=b.MOrgID and a.MItemID=b.MCheckGroupValueID and b.MIsDelete=0 \r\n                     where a.MOrgID=@MOrgID  and a.MIsDelete=0 and a.{0}=@ItemID\r\n                    UNION\r\n                     select 1 from t_gl_checkgroupvalue a\r\n                     inner join t_gl_balance c on a.MOrgID=c.MOrgID and a.MItemID=c.MCheckGroupValueID and c.MIsDelete=0 \r\n                     where a.MOrgID=@MOrgID  and a.MIsDelete=0 and a.{0}=@ItemID\r\n                    UNION\r\n                     select 1 from t_gl_checkgroupvalue a\r\n                     inner join t_gl_voucherentry d on a.MOrgID=d.MOrgID and a.MItemID=d.MCheckGroupValueID and d.MIsDelete=0 \r\n                     where a.MOrgID=@MOrgID  and a.MIsDelete=0 and a.{0}=@ItemID\r\n\r\n                    UNION\r\n                     select 1 from t_gl_checkgroupvalue a\r\n                     inner join t_fa_fixassets d on a.MOrgID=d.MOrgID and a.MItemID=d.MFixCheckGroupValueID and d.MIsDelete=0 \r\n                     where a.MOrgID=@MOrgID  and a.MIsDelete=0 and a.{0}=@ItemID\r\n                    UNION\r\n                     select 1 from t_gl_checkgroupvalue a\r\n                     inner join t_fa_fixassets d on a.MOrgID=d.MOrgID and a.MItemID=d.MDepCheckGroupValueID and d.MIsDelete=0 \r\n                     where a.MOrgID=@MOrgID  and a.MIsDelete=0 and a.{0}=@ItemID\r\n                    UNION\r\n                     select 1 from t_gl_checkgroupvalue a\r\n                     inner join t_fa_fixassets d on a.MOrgID=d.MOrgID and a.MItemID=d.MExpCheckGroupValueID and d.MIsDelete=0 \r\n                     where a.MOrgID=@MOrgID  and a.MIsDelete=0 and a.{0}=@ItemID\r\n\r\n                    UNION\r\n                     select 1 from t_gl_checkgroupvalue a\r\n                     inner join t_fa_fixassetstype d on a.MOrgID=d.MOrgID and a.MItemID=d.MFixCheckGroupValueID and d.MIsDelete=0 \r\n                     where a.MOrgID=@MOrgID  and a.MIsDelete=0 and a.{0}=@ItemID\r\n                    UNION\r\n                     select 1 from t_gl_checkgroupvalue a\r\n                     inner join t_fa_fixassetstype d on a.MOrgID=d.MOrgID and a.MItemID=d.MDepCheckGroupValueID and d.MIsDelete=0 \r\n                     where a.MOrgID=@MOrgID  and a.MIsDelete=0 and a.{0}=@ItemID\r\n                    UNION\r\n                     select 1 from t_gl_checkgroupvalue a\r\n                     inner join t_fa_fixassetstype d on a.MOrgID=d.MOrgID and a.MItemID=d.MExpCheckGroupValueID and d.MIsDelete=0 \r\n                     where a.MOrgID=@MOrgID  and a.MIsDelete=0 and a.{0}=@ItemID\r\n\r\n\r\n                    UNION\r\n                     select 1 from t_gl_checkgroupvalue a\r\n                     inner join t_fa_depreciation d on a.MOrgID=d.MOrgID and a.MItemID=d.MFixCheckGroupValueID and d.MIsDelete=0 \r\n                     where a.MOrgID=@MOrgID  and a.MIsDelete=0 and a.{0}=@ItemID\r\n                    UNION\r\n                     select 1 from t_gl_checkgroupvalue a\r\n                     inner join t_fa_depreciation d on a.MOrgID=d.MOrgID and a.MItemID=d.MDepCheckGroupValueID and d.MIsDelete=0 \r\n                     where a.MOrgID=@MOrgID  and a.MIsDelete=0 and a.{0}=@ItemID\r\n                    UNION\r\n                     select 1 from t_gl_checkgroupvalue a\r\n                     inner join t_fa_depreciation d on a.MOrgID=d.MOrgID and a.MItemID=d.MExpCheckGroupValueID and d.MIsDelete=0 \r\n                     where a.MOrgID=@MOrgID  and a.MIsDelete=0 and a.{0}=@ItemID\r\n\r\n\r\n                    UNION\r\n                     select 1 from t_gl_checkgroupvalue a\r\n                     inner join t_fc_vouchermoduleentry e on a.MOrgID=e.MOrgID and a.MItemID=e.MCheckGroupValueID and e.MIsDelete=0 \r\n                     where a.MOrgID=@MOrgID  and a.MIsDelete=0 and a.{0}=@ItemID", checkTypeValueFeildName);
		}

		private static string GetBaseDataRelationCheckTyepValueExistsSql(string checkTypeValueFeildName)
		{
			return string.Format(" \r\n                     OR EXISTS(\r\n                     select 1 from t_gl_checkgroupvalue a\r\n                     inner join t_gl_initBalance b on a.MOrgID=b.MOrgID and a.MItemID=b.MCheckGroupValueID and b.MIsDelete=0 \r\n                     where a.MOrgID=@MOrgID  and a.MIsDelete=0 and a.{0}=@ItemID and (ABS(b.MInitBalanceFor) + ABS(b.MInitBalance)+ABS(b.MYtdDebit)+ABS(b.MYtdCredit))<>0)\r\n                    OR EXISTS(\r\n                     select 1 from t_gl_checkgroupvalue a\r\n                     inner join t_gl_balance c on a.MOrgID=c.MOrgID and a.MItemID=c.MCheckGroupValueID and c.MIsDelete=0 \r\n                     where a.MOrgID=@MOrgID  and a.MIsDelete=0 and a.{0}=@ItemID and (ABS(c.MBeginBalance) + ABS(c.MDebit)+ABS(c.MCredit)+ABS(c.MEndBalance))<>0 )\r\n                    OR EXISTS(\r\n                     select 1 from t_gl_checkgroupvalue a\r\n                     inner join t_gl_voucherentry d on a.MOrgID=d.MOrgID and a.MItemID=d.MCheckGroupValueID and d.MIsDelete=0 \r\n                     where a.MOrgID=@MOrgID  and a.MIsDelete=0 and a.{0}=@ItemID)\r\n                    \r\n                    OR EXISTS(\r\n                     select 1 from t_gl_checkgroupvalue a\r\n                     inner join t_fa_fixassets d on a.MOrgID=d.MOrgID and a.MItemID=d.MFixCheckGroupValueID and d.MIsDelete=0 \r\n                     where a.MOrgID=@MOrgID  and a.MIsDelete=0 and a.{0}=@ItemID)\r\n                    OR EXISTS(\r\n                     select 1 from t_gl_checkgroupvalue a\r\n                     inner join t_fa_fixassets d on a.MOrgID=d.MOrgID and a.MItemID=d.MDepCheckGroupValueID and d.MIsDelete=0 \r\n                     where a.MOrgID=@MOrgID  and a.MIsDelete=0 and a.{0}=@ItemID)\r\n                    OR EXISTS(\r\n                     select 1 from t_gl_checkgroupvalue a\r\n                     inner join t_fa_fixassets d on a.MOrgID=d.MOrgID and a.MItemID=d.MExpCheckGroupValueID and d.MIsDelete=0 \r\n                     where a.MOrgID=@MOrgID  and a.MIsDelete=0 and a.{0}=@ItemID)\r\n\r\n                    OR EXISTS(\r\n                     select 1 from t_gl_checkgroupvalue a\r\n                     inner join t_fa_fixassetstype d on a.MOrgID=d.MOrgID and a.MItemID=d.MFixCheckGroupValueID and d.MIsDelete=0 \r\n                     where a.MOrgID=@MOrgID  and a.MIsDelete=0 and a.{0}=@ItemID)\r\n                    OR EXISTS(\r\n                     select 1 from t_gl_checkgroupvalue a\r\n                     inner join t_fa_fixassetstype d on a.MOrgID=d.MOrgID and a.MItemID=d.MDepCheckGroupValueID and d.MIsDelete=0 \r\n                     where a.MOrgID=@MOrgID  and a.MIsDelete=0 and a.{0}=@ItemID)\r\n                    OR EXISTS(\r\n                     select 1 from t_gl_checkgroupvalue a\r\n                     inner join t_fa_fixassetstype d on a.MOrgID=d.MOrgID and a.MItemID=d.MExpCheckGroupValueID and d.MIsDelete=0 \r\n                     where a.MOrgID=@MOrgID  and a.MIsDelete=0 and a.{0}=@ItemID)\r\n\r\n                    OR EXISTS(\r\n                     select 1 from t_gl_checkgroupvalue a\r\n                     inner join t_fa_depreciation d on a.MOrgID=d.MOrgID and a.MItemID=d.MFixCheckGroupValueID and d.MIsDelete=0 \r\n                     where a.MOrgID=@MOrgID  and a.MIsDelete=0 and a.{0}=@ItemID)\r\n                    OR EXISTS(\r\n                     select 1 from t_gl_checkgroupvalue a\r\n                     inner join t_fa_depreciation d on a.MOrgID=d.MOrgID and a.MItemID=d.MDepCheckGroupValueID and d.MIsDelete=0 \r\n                     where a.MOrgID=@MOrgID  and a.MIsDelete=0 and a.{0}=@ItemID)\r\n                    OR EXISTS(\r\n                     select 1 from t_gl_checkgroupvalue a\r\n                     inner join t_fa_depreciation d on a.MOrgID=d.MOrgID and a.MItemID=d.MExpCheckGroupValueID and d.MIsDelete=0 \r\n                     where a.MOrgID=@MOrgID  and a.MIsDelete=0 and a.{0}=@ItemID)\r\n\r\n                    OR EXISTS(\r\n                     select 1 from t_gl_checkgroupvalue a\r\n                     inner join t_fc_vouchermoduleentry e on a.MOrgID=e.MOrgID and a.MItemID=e.MCheckGroupValueID and e.MIsDelete=0 \r\n                     where a.MOrgID=@MOrgID  and a.MIsDelete=0 and a.{0}=@ItemID)", checkTypeValueFeildName);
		}

		public static string GetInSql(MContext ctx, List<string> idList, out List<MySqlParameter> paramList)
		{
			paramList = new List<MySqlParameter>();
			if (idList == null || idList.Count() == 0)
			{
				throw new Exception("idList is not be null");
			}
			List<string> list = new List<string>();
			for (int i = 0; i < idList.Count(); i++)
			{
				string text = "@InParamsID" + i;
				MySqlParameter item = new MySqlParameter
				{
					ParameterName = text,
					Value = idList[i]
				};
				paramList.Add(item);
				list.Add(text);
			}
			string result = "";
			if (list.Count() > 0)
			{
				result = string.Format("({0})", string.Join(",", list));
			}
			return result;
		}

		public static List<BDCheckInactiveModel> GetBDInactiveLists(MContext ctx)
		{
			string sql = " ( SELECT t1.MItemID,t1.MOrgID,'Contact' AS ObjectType FROM t_bd_contacts t1 \r\n\t                            WHERE t1.MOrgID = @MOrgID AND t1.MIsActive=0 AND t1.MIsDelete = 0 )\r\n                                UNION\r\n                                SELECT MItemID,MOrgID,'Item' AS ObjectType FROM t_bd_item WHERE MOrgID=@MOrgID AND MIsActive = 0\r\n                                UNION\r\n                                SELECT MEntryID,MOrgID,'Track' AS ObjectType FROM t_bd_trackentry WHERE MOrgID=@MOrgID AND MIsActive = 0\r\n                                UNION\r\n                                SELECT MItemID,MOrgID,'Employees' AS ObjectType  FROM t_bd_employees WHERE MOrgID=@MOrgID AND MIsActive=0 \r\n                                UNION\r\n                                SELECT MItemID,MOrgID,'ExpenseItem' AS ObjectType  FROM t_bd_expenseitem WHERE MOrgID=@MOrgID AND MIsActive=0;";
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			MySqlParameter[] cmdParms = new MySqlParameter[1]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			DataSet dataSet = dynamicDbHelperMySQL.Query(sql, cmdParms);
			return ModelInfoManager.DataTableToList<BDCheckInactiveModel>(dataSet.Tables[0]);
		}

		public static string GetPayItemExistsSql()
		{
			return string.Format(" SELECT 1 FROM t_pa_salarypaymententry \r\n                    WHERE (MPayItemID = @ItemID OR MParentPayItemID = @ItemID) AND MOrgID=@MOrgID AND MIsDelete=0 AND (length(trim(MDesc))>0 or MAmount > 0) \r\n                UNION SELECT 1 FROM t_pa_salarypayment t1 \r\n                    JOIN t_pa_payitemgroup t2 ON t2.MOrgID=t1.MOrgID AND t2.MIsDelete=0 AND t2.MItemID=@ItemID AND t2.MItemType={0} \r\n                    WHERE t1.MOrgID=@MOrgID AND t1.MIsDelete=0 AND t1.MTaxSalary > 0 \r\n                {1}", 3010, GetBaseDataRelationCheckTyepValueSql("MPaItemID"));
		}
	}
}
