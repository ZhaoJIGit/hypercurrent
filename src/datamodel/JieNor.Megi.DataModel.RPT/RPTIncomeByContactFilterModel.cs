using JieNor.Megi.Core;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RPT
{
	[DataContract]
	public class RPTIncomeByContactFilterModel : ReportFilterBase
	{
		[DataMember]
		public DateTime MEndDate
		{
			get;
			set;
		}

		[DataMember]
		public int MPeriod
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
