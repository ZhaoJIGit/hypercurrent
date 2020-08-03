using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.DataRowModel
{
	[DataContract]
	public class IncomeStatementRowModel : BaseReportRowModel
	{
		[DataMember]
		public string Account
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
		public decimal? CurrentYearAccruingAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal? CurrentMonthAccruingAmount
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

		[DataMember]
		public string CurrentYearAccruingAmountStr
		{
			get;
			set;
		}

		[DataMember]
		public string CurrentMonthAccruingAmountStr
		{
			get;
			set;
		}
	}
}
