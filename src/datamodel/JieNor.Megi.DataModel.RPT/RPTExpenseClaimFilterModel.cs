using JieNor.Megi.Core;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RPT
{
	public class RPTExpenseClaimFilterModel : ReportFilterBase
	{
		[DataMember]
		public DateTime MStartDate
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MEndDate
		{
			get;
			set;
		}

		[DataMember]
		public string SortBy
		{
			get;
			set;
		}

		[DataMember]
		public string[] SortByValue
		{
			get;
			set;
		}

		[DataMember]
		public string MonthShowType
		{
			get;
			set;
		}

		[DataMember]
		public string[] TrackIds
		{
			get;
			set;
		}

		[DataMember]
		public int MPeriodCount
		{
			get;
			set;
		}
	}
}
