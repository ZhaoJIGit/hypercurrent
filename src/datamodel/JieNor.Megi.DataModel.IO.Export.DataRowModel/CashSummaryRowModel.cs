using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.DataRowModel
{
	[DataContract]
	public class CashSummaryRowModel : BaseReportRowModel
	{
		[DataMember]
		public string Date
		{
			get;
			set;
		}

		[DataMember]
		public string Type
		{
			get;
			set;
		}

		[DataMember]
		public string Transaction
		{
			get;
			set;
		}

		[DataMember]
		public string Reference
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
	}
}
