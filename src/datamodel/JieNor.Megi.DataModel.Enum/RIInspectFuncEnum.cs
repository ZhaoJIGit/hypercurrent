using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.Enum
{
	[DataContract]
	public static class RIInspectFuncEnum
	{
		[DataMember]
		public static string Biz_Sale = "Biz_Sale";

		[DataMember]
		public static string Biz_Purchase = "Biz_Purchase";

		[DataMember]
		public static string Biz_Expense = "Biz_Expense";

		[DataMember]
		public static string Biz_Payment = "Biz_Payment";

		[DataMember]
		public static string Biz_Receive = "Biz_Receive";

		[DataMember]
		public static string Biz_Transfer = "Biz_Transfer";

		[DataMember]
		public static string Biz_BankBill = "Biz_BankBill";

		[DataMember]
		public static string Biz_DocVoucher = "Biz_DocVoucher";

		[DataMember]
		public static string Assets_Cash = "Assets_Cash";

		[DataMember]
		public static string Assets_BankDeposit = "Assets_BankDeposit";

		[DataMember]
		public static string Assets_Material = "Assets_Material";

		[DataMember]
		public static string Assets_Inventory = "Assets_Inventory";

		[DataMember]
		public static string Assets_FixAssets = "Assets_FixAssets";

		[DataMember]
		public static string Assets_CIP = "Assets_CIP";

		[DataMember]
		public static string Assets_InvisibleAssets = "Assets_InvisibleAssets";

		[DataMember]
		public static string Assets_OtherAssets = "Assets_OtherAssets";

		[DataMember]
		public static string MP_CostOfSale = "MP_CostOfSale";

		[DataMember]
		public static string MP_Depreciation = "MP_Depreciation";

		[DataMember]
		public static string MP_AmortizationExpense = "MP_AmortizationExpense";

		[DataMember]
		public static string MP_Wages = "MP_Wages";

		[DataMember]
		public static string MP_Exchange = "MP_Exchange";

		[DataMember]
		public static string MP_VAT = "MP_VAT";

		[DataMember]
		public static string MP_IncomeTax = "MP_IncomeTax";

		[DataMember]
		public static string MP_ProfitLoss = "MP_ProfitLoss";

		[DataMember]
		public static string MP_NDP = "MP_NDP";

		[DataMember]
		public static string Current_Receivable = "Current_Receivable";

		[DataMember]
		public static string Current_PrePay = "Current_PrePay";

		[DataMember]
		public static string Current_OtherReceivable = "Current_OtherReceivable";

		[DataMember]
		public static string Current_Payable = "Current_Payable";

		[DataMember]
		public static string Current_PreReceive = "Current_PreReceive";

		[DataMember]
		public static string Current_OtherPayable = "Current_OtherPayable";

		[DataMember]
		public static string EXP_Tax = "EXP_Tax";

		[DataMember]
		public static string EXP_Wages = "EXP_Wages";

		[DataMember]
		public static string EXP_AllExpense = "EXP_AllExpense";

		[DataMember]
		public static string Other_FixAssets = "Other_FixAssets";

		[DataMember]
		public static string Other_AssetsDebit = "Other_AssetsDebit";

		[DataMember]
		public static string Other_VoucherNumber = "Other_VoucherNumber";

		[DataMember]
		public static string Other_NewAccount = "Other_NewAccount";
	}
}
