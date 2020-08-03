using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.DataRowModel
{
	[DataContract]
	public class AssetDepreciationRowModel : BaseReportRowModel
	{
		[DataMember]
		public string Date
		{
			get;
			set;
		}

		[DataMember]
		public string Code
		{
			get;
			set;
		}

		[DataMember]
		public string Name
		{
			get;
			set;
		}

		[DataMember]
		public string CheckGroup
		{
			get;
			set;
		}

		[DataMember]
		public decimal? Debit
		{
			get;
			set;
		}

		[DataMember]
		public string DebitStr
		{
			get;
			set;
		}

		[DataMember]
		public decimal? Credit
		{
			get;
			set;
		}

		[DataMember]
		public string CreditStr
		{
			get;
			set;
		}

		[DataMember]
		public decimal? Balance
		{
			get;
			set;
		}

		[DataMember]
		public string BalanceStr
		{
			get;
			set;
		}

		[DataMember]
		public decimal? DirectionDebit
		{
			get;
			set;
		}

		[DataMember]
		public string DirectionDebitStr
		{
			get;
			set;
		}

		[DataMember]
		public decimal? DirectionCredit
		{
			get;
			set;
		}

		[DataMember]
		public string DirectionCreditStr
		{
			get;
			set;
		}

		[DataMember]
		public decimal? DirectionBalance
		{
			get;
			set;
		}

		[DataMember]
		public string DirectionBalanceStr
		{
			get;
			set;
		}

		[DataMember]
		public decimal? NetAmount
		{
			get;
			set;
		}

		[DataMember]
		public string NetAmountStr
		{
			get;
			set;
		}

		[DataMember]
		public decimal? DirectionPrepareDebit
		{
			get;
			set;
		}

		[DataMember]
		public string DirectionPrepareDebitStr
		{
			get;
			set;
		}

		[DataMember]
		public decimal? DirectionPrepareCredit
		{
			get;
			set;
		}

		[DataMember]
		public string DirectionPrepareCreditStr
		{
			get;
			set;
		}

		[DataMember]
		public decimal? DirectionPrepareBalance
		{
			get;
			set;
		}

		[DataMember]
		public string DirectionPrepareBalanceStr
		{
			get;
			set;
		}

		[DataMember]
		public decimal? NetWorth
		{
			get;
			set;
		}

		[DataMember]
		public string NetWorthStr
		{
			get;
			set;
		}
	}
}
