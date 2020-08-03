using JieNor.Megi.Core;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.DataRepository.BAS
{
	public class BASSearchRepository
	{
		private string GetSalarySql(MContext ctx, string typeName)
		{
			string str = "select a.MID, '' as MNumber ,'' as MBankID, 'Pay_Salary' as MType, '' as MBizDate , a.MNetSalary MAmount, '{1}' as MCyID, '' as MReference,  F_GetUserName(b.MFirstName,b.MLastName)  AS MContactName ,'' as MDesc \n                        ,'' as MValue1,'' as MValue2,'' as MValue3\n                        from T_PA_SalaryPayment a\n                        inner join t_bd_employees_l  b on b.MParentID= a.MEmployeeID  AND  b.MLocaleID= @MLocaleID AND b.MIsDelete=0 AND b.MOrgID= a.MOrgID\n                        where a.MorgID = @MOrgID and a.MIsDelete = 0  ";
			str += (ctx.IsSelfData ? " and a.MCreatorID = @MUserID " : "");
			return GetAssembleSql(ctx, str, typeName);
		}

		private string GetReceiveSql(MContext ctx, string typeName)
		{
			string str = "select a.MID,'' as MNumber, MBankID,  'Receive' as MType, a.MBizDate, MTaxTotalAmtFor as MAmount, a.MCyID as MCyID, a.MReference, convert(AES_DECRYPT(b.MName,'{0}') using utf8)  AS MContactName, '' as MDesc\n                        ,'' as MValue1,'' as MValue2,'' as MValue3\n                        from   T_IV_Receive a\n                        left join t_bd_contacts_l  b on b.MParentID= a.MContactID  AND  b.MLocaleID= @MLocaleID AND b.MIsDelete=0  AND b.MOrgID= a.MOrgID\n                        where a.MorgID = @MOrgID and a.MIsDelete = 0 ";
			str += (ctx.IsSelfData ? " and a.MCreatorID = @MUserID " : "");
			return GetAssembleSql(ctx, str, typeName);
		}

		private string GetPaymentSql(MContext ctx, string typeName)
		{
			string str = "select  a.MID,'' as MNumber, MBankID,  'Payment' as MType, a.MBizDate, a.MTaxTotalAmtFor as MAmount, a.MCyID , a.MReference, convert(AES_DECRYPT(b.MName,'{0}') using utf8)  AS MContactName, '' as MDesc\n                        ,'' as MValue1,'' as MValue2,'' as MValue3\n                        from   T_IV_Payment a\n                        left join t_bd_contacts_l  b on b.MParentID= a.MContactID  AND  b.MLocaleID= @MLocaleID AND b.MIsDelete=0   AND b.MOrgID= a.MOrgID\n                        where a.MorgID = @MOrgID and a.MIsDelete = 0  and MContactType != 'Employees' ";
			str += (ctx.IsSelfData ? " and a.MCreatorID = @MUserID " : "");
			str += " union all ";
			str += " select  a.MID,'' as MNumber, MBankID,  'Payment' as MType, a.MBizDate, a.MTaxTotalAmtFor as MAmount, a.MCyID , a.MReference, F_GetUserName(b.MFirstName,b.MLastName)  AS MContactName, '' as MDesc\n                        ,'' as MValue1,'' as MValue2,'' as MValue3\n                        from   T_IV_Payment a\n                        left join t_bd_employees_l  b on b.MParentID= a.MContactID  AND  b.MLocaleID= @MLocaleID AND b.MIsDelete=0  AND b.MOrgID= a.MOrgID\n                        where a.MorgID = @MOrgID and a.MIsDelete = 0  and MContactType = 'Employees' ";
			str += (ctx.IsSelfData ? " and a.MCreatorID = @MUserID " : "");
			return GetAssembleSql(ctx, str, typeName);
		}

		private string GetTransferSql(MContext ctx, string typeName)
		{
			string str = "select  a.MID,'' as MNumber,'' MBankID,  'Transfer' as MType, a.MBizDate, a.MFromTotalAmt as MAmount,a.MFromCyID as MCyID, a.MReference,  ''  AS MContactName, concat(b.MName,'-->',c.MName) as MDesc\n                        ,'' as MValue1,'' as MValue2,'' as MValue3\n                        from   T_IV_Transfer a\n                        inner join t_bd_bankaccount_l  b on b.MParentID= a.MFromAcctID  AND  b.MLocaleID= @MLocaleID AND b.MIsDelete=0   AND b.MOrgID= a.MOrgID\n                        inner join t_bd_bankaccount_l  c on c.MParentID= a.MToAcctID  AND  c.MLocaleID= @MLocaleID AND c.MIsDelete=0   AND c.MOrgID= a.MOrgID\n                        where a.MorgID = @MOrgID and a.MIsDelete = 0 ";
			str += (ctx.IsSelfData ? " and a.MCreatorID = @MUserID " : "");
			return GetAssembleSql(ctx, str, typeName);
		}

		private string GetVoucherSql(MContext ctx, string typeName)
		{
			string str = "select a.MItemID as MID, a.MNumber,'' MBankID, 'Voucher' as MType,a.MDate as MBizDate, a.MDebitTotal as MAmount,'{1}' as MCyID, a.MReference ,'' as MContactName, '' as MDesc\n                        ,MStatus as MValue1,'' as MValue2,'' as MValue3\n                        from t_gl_voucher a \n                        where a.MorgID = @MOrgID and a.MIsDelete = 0  and (MStatus  = 0 or MStatus = 1) and Length(ifnull(a.MNumber,'')) > 0 ";
			str += (ctx.IsSelfData ? " and a.MCreatorID = @MUserID " : "");
			return GetAssembleSql(ctx, str, typeName);
		}

		private string GetInvoiceSaleSql(MContext ctx, string typeName)
		{
			string str = " select a.MID , a.MNumber,'' MBankID, MType, a.MBizDate, a.MTaxTotalAmtFor as MAmount,  a.MCyID, a.MReference, convert(AES_DECRYPT(b.MName,'{0}') using utf8)  AS MContactName ,'' as MDesc\n                        ,a.MStatus as MValue1,'' as MValue2,'' as MValue3\n                        from t_iv_invoice a\n                        inner join t_bd_contacts_l  b on b.MParentID= a.MContactID  AND  b.MLocaleID= @MLocaleID AND b.MIsDelete=0  AND b.MOrgID= a.MOrgID\n                        where a.MorgID = @MOrgID and a.MIsDelete = 0  and (a.MType = 'Invoice_Sale' or a.MType = 'Invoice_Sale_Red') ";
			str += (ctx.IsSelfData ? " and a.MCreatorID = @MUserID " : "");
			return GetAssembleSql(ctx, str, typeName);
		}

		private string GetInvoicePurchaseSql(MContext ctx, string typeName)
		{
			string str = " select a.MID ,'' as MNumber,'' MBankID, MType, a.MBizDate, a.MTaxTotalAmtFor as MAmount, a.MCyID , a.MReference, convert(AES_DECRYPT(b.MName,'{0}') using utf8)  AS MContactName ,'' as MDesc\n                        ,a.MStatus as MValue1,'' as MValue2,'' as MValue3\n                        from t_iv_invoice a\n                        inner join t_bd_contacts_l  b on b.MParentID= a.MContactID  AND  b.MLocaleID= @MLocaleID AND b.MIsDelete=0 AND b.MOrgID= a.MOrgID\n                        where a.MorgID = @MOrgID and a.MIsDelete = 0  and (a.MType = 'Invoice_Purchase' or a.MType = 'Invoice_Purchase_Red') ";
			str += (ctx.IsSelfData ? " and a.MCreatorID = @MUserID " : "");
			return GetAssembleSql(ctx, str, typeName);
		}

		private string GetExpenseSql(MContext ctx, string typeName)
		{
			string str = "select a.MID, '' as MNumber ,'' as MBankID, 'Expense' as MType, a.MBizDate , a.MTaxTotalAmtFor as MAmount, a.MCyID ,  a.MReference,  F_GetUserName(b.MFirstName,b.MLastName)  AS MContactName,'' as MDesc\n                        ,a.MStatus as MValue1,'' as MValue2,'' as MValue3\n                        from t_iv_expense a\n                        left join t_bd_employees_l  b on b.MParentID= a.MContactID  AND  b.MLocaleID= @MLocaleID AND b.MIsDelete=0  AND b.MOrgID= a.MOrgID\n                        where a.MorgID = @MOrgID and a.MIsDelete = 0  ";
			str += (ctx.IsSelfData ? " and a.MCreatorID = @MUserID " : "");
			return GetAssembleSql(ctx, str, typeName);
		}

		private string GetTableSql(MContext ctx, string typeName)
		{
			string str = "select a.MItemID as MID, a.MNumber ,'' as MBankID, 'MegiFapiao' as MType, a.MBizDate , a.MTotalAmount as MAmount ,'{1}' as MCyID,MExplanation as  MReference,  convert(AES_DECRYPT(b.MName,'{0}') using utf8)  AS MContactName , '' as MDesc\n                        ,a.MIssueStatus as MValue1, a.MInvoiceType as MValue2,'' as MValue3\n                        from t_fp_table a\n                        inner join t_bd_contacts_l  b on b.MParentID= a.MContactID  AND  b.MLocaleID= @MLocaleID AND b.MIsDelete=0 AND b.MOrgID= a.MOrgID\n                        where a.MorgID = @MOrgID and a.MIsDelete = 0 ";
			str += (ctx.IsSelfData ? " and a.MCreatorID = @MUserID " : "");
			return GetAssembleSql(ctx, str, typeName);
		}

		private string GetAssembleSql(MContext ctx, string sql, string typeName)
		{
			return string.Format(" select * from  (" + sql + ") a ", "JieNor-001", ctx.MBasCurrencyID, typeName);
		}

		private string GetQuerySqlByFilter(MContext ctx, BASSearchFilterModel filter)
		{
			Dictionary<string, bool> access = COMAccess.GetAccessResult(ctx, (List<MAccessRequestModel>)null).Access;
			bool flag = access["BankAccountView"];
			bool flag2 = filter.BizObjectList == null || filter.BizObjectList.Count == 0;
			List<string> list = new List<string>();
			if ((flag2 || filter.BizObjectList.Contains("Invoice_Sales")) && (access["Invoice_SalesView"] || access["Invoice_SalesApprove"]))
			{
				list.Add(GetInvoiceSaleSql(ctx, "Invoice_Sales"));
			}
			if ((flag2 || filter.BizObjectList.Contains("Invoice_Purchases")) && (access["Invoice_PurchasesView"] || access["Invoice_PurchasesApprove"]))
			{
				list.Add(GetInvoicePurchaseSql(ctx, "Invoice_Purchases"));
			}
			if ((flag2 || filter.BizObjectList.Contains("Expense")) && (access["ExpenseView"] || access["ExpenseApprove"]))
			{
				list.Add(GetExpenseSql(ctx, "Expense"));
			}
			if ((flag2 || filter.BizObjectList.Contains("Receive")) && (access["Receive"] & flag))
			{
				list.Add(GetReceiveSql(ctx, "Receive"));
			}
			if ((flag2 || filter.BizObjectList.Contains("Payment")) && (access["Payment"] & flag))
			{
				list.Add(GetPaymentSql(ctx, "Payment"));
			}
			if ((flag2 || filter.BizObjectList.Contains("Transfer")) && (access["Transfer"] & flag))
			{
				list.Add(GetTransferSql(ctx, "Transfer"));
			}
			if ((flag2 || filter.BizObjectList.Contains("Pay_Salary")) && access["PayRunView"])
			{
				list.Add(GetSalarySql(ctx, "Pay_Salary"));
			}
			if ((flag2 || filter.BizObjectList.Contains("Voucher")) && (access["General_LedgerView"] || access["General_LedgerApprove"]))
			{
				list.Add(GetVoucherSql(ctx, "Voucher"));
			}
			if ((flag2 || filter.BizObjectList.Contains("MegiFapiao")) && (access["Purchases_FapiaoView"] || access["Purchases_FapiaoApprove"] || access["Sales_FapiaoView"] || access["Sales_FapiaoApprove"]))
			{
				list.Add(GetTableSql(ctx, "MegiFapiao"));
			}
			if (list.Count == 0)
			{
				return null;
			}
			return string.Join(" union all ", list) + " " + filter.OrderBySqlString;
		}

		public int GetTotalCount(MContext ctx, BASSearchFilterModel filter)
		{
			string querySqlByFilter = GetQuerySqlByFilter(ctx, filter);
			if (string.IsNullOrWhiteSpace(querySqlByFilter))
			{
				return 0;
			}
			List<MySqlParameter> list = ctx.GetParameters((MySqlParameter)null).ToList();
			list.Add(GetParameter(filter));
			querySqlByFilter = " select count(*) as t from ( " + querySqlByFilter + ") x  where 1=1 " + GetSqlWhereByFilter(ctx, filter);
			return int.Parse(new DynamicDbHelperMySQL(ctx).GetSingle(querySqlByFilter, list.ToArray()).ToString());
		}

		public List<BASSearchModel> GetDataPageList(MContext ctx, BASSearchFilterModel filter)
		{
			string querySqlByFilter = GetQuerySqlByFilter(ctx, filter);
			if (string.IsNullOrWhiteSpace(querySqlByFilter))
			{
				return new List<BASSearchModel>();
			}
			List<MySqlParameter> list = ctx.GetParameters((MySqlParameter)null).ToList();
			list.Add(GetParameter(filter));
			querySqlByFilter = "select * from ( " + querySqlByFilter + ") x where 1=1 " + GetSqlWhereByFilter(ctx, filter) + " limit " + (filter.PageIndex - 1) * filter.PageSize + "," + filter.PageSize;
			return ModelInfoManager.GetDataModelBySql<BASSearchModel>(ctx, querySqlByFilter, list.ToArray());
		}

		private MySqlParameter GetParameter(BASSearchFilterModel filter)
		{
			if (filter.BizColumn == "MBizDate")
			{
				return new MySqlParameter
				{
					ParameterName = "@Keyword",
					Value = (object)DateTime.Parse(filter.KeyWord)
				};
			}
			string value = filter.KeyWord;
			if (filter.SqlOperator == SqlOperators.Like || filter.SqlOperator == SqlOperators.NotLike)
			{
				value = filter.KeyWord.Replace("\\", "\\\\");
			}
			return new MySqlParameter
			{
				ParameterName = "@Keyword",
				Value = value
			};
		}

		private string GetSqlWhereByFilter(MContext ctx, BASSearchFilterModel filter)
		{
			return (string.IsNullOrWhiteSpace(filter.KeyWord) || string.IsNullOrWhiteSpace(filter.BizColumn)) ? string.Empty : GetOperateFilterSql(filter.SqlOperator, filter.BizColumn);
		}

		private string GetOperateFilterSql(SqlOperators sqlOperator, string columnName)
		{
			string format = string.Empty;
			switch (sqlOperator)
			{
			case SqlOperators.Greater:
				format = " and {0} > @KeyWord ";
				break;
			case SqlOperators.Less:
				format = " and {0} < @KeyWord ";
				break;
			case SqlOperators.Equal:
				format = " and {0} = @KeyWord ";
				break;
			case SqlOperators.LessOrEqual:
				format = " and {0} <= @KeyWord ";
				break;
			case SqlOperators.GreaterOrEqual:
				format = " and {0} >= @KeyWord ";
				break;
			case SqlOperators.NotEqual:
				format = " and {0} != @KeyWord ";
				break;
			case SqlOperators.Like:
				format = " and {0} like concat('%',@KeyWord,'%') ";
				break;
			case SqlOperators.NotLike:
				format = " and {0} not like concat('%',@KeyWord,'%') ";
				break;
			}
			return string.Format(format, columnName);
		}
	}
}
