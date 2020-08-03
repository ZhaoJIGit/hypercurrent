using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.DataRowModel
{
	[DataContract]
	public class BalanceSheetRowModel : BaseReportRowModel
	{
		[DataMember]
		public string Asset
		{
			get;
			set;
		}

		[DataMember]
		public decimal? LineNo1
		{
			get;
			set;
		}

		[DataMember]
		public decimal? EndingBalance1
		{
			get;
			set;
		}

		[DataMember]
		public decimal? BeginningBalance1
		{
			get;
			set;
		}

		[DataMember]
		public string LiabilitiesAndOwnerEquity
		{
			get;
			set;
		}

		[DataMember]
		public decimal? LineNo2
		{
			get;
			set;
		}

		[DataMember]
		public decimal? EndingBalance2
		{
			get;
			set;
		}

		[DataMember]
		public decimal? BeginningBalance2
		{
			get;
			set;
		}

		[DataMember]
		public string LineNo1Str
		{
			get;
			set;
		}

		[DataMember]
		public string LineNo2Str
		{
			get;
			set;
		}

		[DataMember]
		public string EndingBalance1Str
		{
			get;
			set;
		}

		[DataMember]
		public string BeginningBalance1Str
		{
			get;
			set;
		}

		[DataMember]
		public string EndingBalance2Str
		{
			get;
			set;
		}

		[DataMember]
		public string BeginningBalance2Str
		{
			get;
			set;
		}
	}
}
