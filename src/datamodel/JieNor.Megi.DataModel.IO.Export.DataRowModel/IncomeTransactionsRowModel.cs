using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.DataRowModel
{
	[DataContract]
	public class IncomeTransactionsRowModel : BaseReportRowModel
	{
		[DataMember]
		public string Date
		{
			get;
			set;
		}

		[DataMember]
		public string Details
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
		public decimal? Total
		{
			get;
			set;
		}

		[DataMember]
		public string TotalStr
		{
			get;
			set;
		}
	}
}
