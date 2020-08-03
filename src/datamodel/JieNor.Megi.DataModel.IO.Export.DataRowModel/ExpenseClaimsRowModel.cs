using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.DataRowModel
{
	[DataContract]
	public class ExpenseClaimsRowModel : BaseReportRowModel
	{
		[DataMember]
		public string StatisticsItem
		{
			get;
			set;
		}

		[DataMember]
		public decimal? Prior
		{
			get;
			set;
		}

		[DataMember]
		public decimal? CurrentMonth
		{
			get;
			set;
		}

		[DataMember]
		public decimal? Previous1Month
		{
			get;
			set;
		}

		[DataMember]
		public decimal? Previous2Month
		{
			get;
			set;
		}

		[DataMember]
		public decimal? Previous3Month
		{
			get;
			set;
		}

		[DataMember]
		public decimal? Previous4Month
		{
			get;
			set;
		}

		[DataMember]
		public decimal? Previous5Month
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
		public string PriorStr
		{
			get;
			set;
		}

		[DataMember]
		public string CurrentMonthStr
		{
			get;
			set;
		}

		[DataMember]
		public string Previous1MonthStr
		{
			get;
			set;
		}

		[DataMember]
		public string Previous2MonthStr
		{
			get;
			set;
		}

		[DataMember]
		public string Previous3MonthStr
		{
			get;
			set;
		}

		[DataMember]
		public string Previous4MonthStr
		{
			get;
			set;
		}

		[DataMember]
		public string Previous5MonthStr
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
