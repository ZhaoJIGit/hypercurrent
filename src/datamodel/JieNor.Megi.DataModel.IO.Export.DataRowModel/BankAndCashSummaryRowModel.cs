using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.DataRowModel
{
	[DataContract]
	public class BankAndCashSummaryRowModel : BaseReportRowModel
	{
		[DataMember]
		public string BankAccounts
		{
			get;
			set;
		}

		[DataMember]
		public decimal? OpeningBlance
		{
			get;
			set;
		}

		[DataMember]
		public decimal? CashReceived
		{
			get;
			set;
		}

		[DataMember]
		public decimal? CashSpent
		{
			get;
			set;
		}

		[DataMember]
		public decimal? FXGain
		{
			get;
			set;
		}

		[DataMember]
		public decimal? ClosingBlance
		{
			get;
			set;
		}

		[DataMember]
		public string OpeningBlanceStr
		{
			get;
			set;
		}

		[DataMember]
		public string CashReceivedStr
		{
			get;
			set;
		}

		[DataMember]
		public string CashSpentStr
		{
			get;
			set;
		}

		[DataMember]
		public string FXGainStr
		{
			get;
			set;
		}

		[DataMember]
		public string ClosingBlanceStr
		{
			get;
			set;
		}
	}
}
