using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.DataRowModel
{
	[DataContract]
	public class BankReconciliationSummaryRowModel : BaseReportRowModel
	{
		[DataMember]
		public string Date
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
		public decimal? Amount
		{
			get;
			set;
		}

		[DataMember]
		public string AmountStr
		{
			get;
			set;
		}
	}
}
