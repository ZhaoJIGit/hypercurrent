using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.IV.Expense;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD.InitDocument
{
	[DataContract]
	public class BDInitDocumentViewModel
	{
		[DataMember]
		public string MAccountCode
		{
			get;
			set;
		}

		[DataMember]
		public string MNewAccountCode
		{
			get;
			set;
		}

		[DataMember]
		public string MAccountID
		{
			get;
			set;
		}

		[DataMember]
		public string MNewAccountID
		{
			get;
			set;
		}

		[DataMember]
		public decimal MInitBalance
		{
			get;
			set;
		}

		[DataMember]
		public bool SaveBillOnly
		{
			get;
			set;
		}

		[DataMember]
		public List<BDInitBillViewModel> InitBillList
		{
			get;
			set;
		}

		[DataMember]
		public List<InitBillAmountFor> InitBillAmountFor
		{
			get;
			set;
		}

		[DataMember]
		public List<IVInvoiceModel> MSaleInvoiceList
		{
			get;
			set;
		}

		[DataMember]
		public List<IVInvoiceModel> MBillInvoiceList
		{
			get;
			set;
		}

		[DataMember]
		public List<IVReceiveModel> MReceiveList
		{
			get;
			set;
		}

		[DataMember]
		public List<IVPaymentModel> MPaymentList
		{
			get;
			set;
		}

		[DataMember]
		public List<IVExpenseModel> MExpenseList
		{
			get;
			set;
		}
	}
}
