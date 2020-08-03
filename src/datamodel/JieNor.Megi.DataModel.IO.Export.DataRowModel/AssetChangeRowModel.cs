using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.DataRowModel
{
	[DataContract]
	public class AssetChangeRowModel : BaseReportRowModel
	{
		[DataMember]
		public string Category
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
		public decimal? OriginalBeginPeriod
		{
			get;
			set;
		}

		[DataMember]
		public string OriginalBeginPeriodStr
		{
			get;
			set;
		}

		[DataMember]
		public decimal? OriginalAdd
		{
			get;
			set;
		}

		[DataMember]
		public string OriginalAddStr
		{
			get;
			set;
		}

		[DataMember]
		public decimal? OriginalTurnOut
		{
			get;
			set;
		}

		[DataMember]
		public string OriginalTurnOutStr
		{
			get;
			set;
		}

		[DataMember]
		public decimal? OriginalEndBlance
		{
			get;
			set;
		}

		[DataMember]
		public string OriginalEndBlanceStr
		{
			get;
			set;
		}

		[DataMember]
		public decimal? DepreciationBeginPeriod
		{
			get;
			set;
		}

		[DataMember]
		public string DepreciationBeginPeriodStr
		{
			get;
			set;
		}

		[DataMember]
		public decimal? DepreciationAdd
		{
			get;
			set;
		}

		[DataMember]
		public string DepreciationAddStr
		{
			get;
			set;
		}

		[DataMember]
		public decimal? DepreciationTurnOut
		{
			get;
			set;
		}

		[DataMember]
		public string DepreciationTurnOutStr
		{
			get;
			set;
		}

		[DataMember]
		public decimal? DepreciationEndBlance
		{
			get;
			set;
		}

		[DataMember]
		public string DepreciationEndBlanceStr
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
		public decimal? PeriodDepreciatedAmount
		{
			get;
			set;
		}

		[DataMember]
		public string PeriodDepreciatedAmountStr
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
