using JieNor.Megi.Core;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RPT
{
	public class RPTExpenseClaimDeatailFilterModel : ReportFilterBase
	{
		[DataMember]
		public string StatisticsType
		{
			get;
			set;
		}

		[DataMember]
		public string StatisticsFieldId
		{
			get;
			set;
		}

		public string[] StatisticsFieldOptionIds
		{
			get;
			set;
		}

		[DataMember]
		public DateTime StartDate
		{
			get;
			set;
		}

		[DataMember]
		public DateTime EndDate
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
	}
}
