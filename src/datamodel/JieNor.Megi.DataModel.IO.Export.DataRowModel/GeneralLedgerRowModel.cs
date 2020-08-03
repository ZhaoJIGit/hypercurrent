using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.DataRowModel
{
	[DataContract]
	public class GeneralLedgerRowModel : BaseReportRowModel
	{
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
		public string AccountingPeriod
		{
			get;
			set;
		}

		[DataMember]
		public string Summary
		{
			get;
			set;
		}

		[DataMember]
		public decimal? DebitCNY
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
		public decimal? CreditCNY
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
		public string Direction
		{
			get;
			set;
		}

		[DataMember]
		public decimal? AccountBalanceCNY
		{
			get;
			set;
		}

		[DataMember]
		public decimal? AccountBalance
		{
			get;
			set;
		}

		[DataMember]
		public string DebitCNYStr
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
		public string CreditCNYStr
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
		public string AccountBalanceCNYStr
		{
			get;
			set;
		}

		[DataMember]
		public string AccountBalanceStr
		{
			get;
			set;
		}
	}
}
