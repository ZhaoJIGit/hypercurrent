using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.DataRowModel
{
	[DataContract]
	public class AccountBalanceRowModel : BaseReportRowModel
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
		public decimal? InitialDebitBalanceCNY
		{
			get;
			set;
		}

		[DataMember]
		public decimal? InitialDebitBalance
		{
			get;
			set;
		}

		[DataMember]
		public decimal? InitialCreditBalanceCNY
		{
			get;
			set;
		}

		[DataMember]
		public decimal? InitialCreditBalance
		{
			get;
			set;
		}

		[DataMember]
		public decimal? CurrentPeriodDebitBalanceCNY
		{
			get;
			set;
		}

		[DataMember]
		public decimal? CurrentPeriodDebitBalance
		{
			get;
			set;
		}

		[DataMember]
		public decimal? CurrentPeriodCreditBalanceCNY
		{
			get;
			set;
		}

		[DataMember]
		public decimal? CurrentPeriodCreditBalance
		{
			get;
			set;
		}

		[DataMember]
		public decimal? CurrentYearCumulativeDebitBalanceCNY
		{
			get;
			set;
		}

		[DataMember]
		public decimal? CurrentYearCumulativeDebitBalance
		{
			get;
			set;
		}

		[DataMember]
		public decimal? CurrentYearCumulativeCreditBalanceCNY
		{
			get;
			set;
		}

		[DataMember]
		public decimal? CurrentYearCumulativeCreditBalance
		{
			get;
			set;
		}

		[DataMember]
		public decimal? FinalDebitBalanceCNY
		{
			get;
			set;
		}

		[DataMember]
		public decimal? FinalDebitBalance
		{
			get;
			set;
		}

		[DataMember]
		public decimal? FinalCreditBalanceCNY
		{
			get;
			set;
		}

		[DataMember]
		public decimal? FinalCreditBalance
		{
			get;
			set;
		}

		[DataMember]
		public string InitialDebitBalanceCNYStr
		{
			get;
			set;
		}

		[DataMember]
		public string InitialDebitBalanceStr
		{
			get;
			set;
		}

		[DataMember]
		public string InitialCreditBalanceCNYStr
		{
			get;
			set;
		}

		[DataMember]
		public string InitialCreditBalanceStr
		{
			get;
			set;
		}

		[DataMember]
		public string CurrentPeriodDebitBalanceCNYStr
		{
			get;
			set;
		}

		[DataMember]
		public string CurrentPeriodDebitBalanceStr
		{
			get;
			set;
		}

		[DataMember]
		public string CurrentPeriodCreditBalanceCNYStr
		{
			get;
			set;
		}

		[DataMember]
		public string CurrentPeriodCreditBalanceStr
		{
			get;
			set;
		}

		[DataMember]
		public string CurrentYearCumulativeDebitBalanceCNYStr
		{
			get;
			set;
		}

		[DataMember]
		public string CurrentYearCumulativeDebitBalanceStr
		{
			get;
			set;
		}

		[DataMember]
		public string CurrentYearCumulativeCreditBalanceCNYStr
		{
			get;
			set;
		}

		[DataMember]
		public string CurrentYearCumulativeCreditBalanceStr
		{
			get;
			set;
		}

		[DataMember]
		public string FinalDebitBalanceCNYStr
		{
			get;
			set;
		}

		[DataMember]
		public string FinalDebitBalanceStr
		{
			get;
			set;
		}

		[DataMember]
		public string FinalCreditBalanceCNYStr
		{
			get;
			set;
		}

		[DataMember]
		public string FinalCreditBalanceStr
		{
			get;
			set;
		}

		public string InitialBalanceTitle
		{
			get;
			set;
		}

		public string CurrentPeriodBalanceTitle
		{
			get;
			set;
		}

		public string CurrentYearCumulativeBalanceTitle
		{
			get;
			set;
		}

		public string FinalBalanceTitle
		{
			get;
			set;
		}
	}
}
