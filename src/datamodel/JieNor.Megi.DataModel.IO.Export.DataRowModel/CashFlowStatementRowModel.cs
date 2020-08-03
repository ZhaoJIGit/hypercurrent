using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.DataRowModel
{
	[DataContract]
	public class CashFlowStatementRowModel : BaseReportRowModel
	{
		[DataMember]
		public string Item
		{
			get;
			set;
		}

		[DataMember]
		public decimal? LineNo
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

		[DataMember]
		public string LineNoStr
		{
			get;
			set;
		}
	}
}
