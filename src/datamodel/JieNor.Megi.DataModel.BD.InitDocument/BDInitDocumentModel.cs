using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.IV.Expense;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD.InitDocument
{
	[DataContract]
	public class BDInitDocumentModel : BDModel
	{
		[DataMember]
		public string MAccountCode
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
		public decimal MInitBalance
		{
			get;
			set;
		}

		[DataMember]
		public decimal MDebitYTD
		{
			get;
			set;
		}

		[DataMember]
		public decimal MCreditYTD
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

		public BDInitDocumentModel()
			: base("t")
		{
		}
	}
}
