using JieNor.Megi.Core;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RPT
{
	[DataContract]
	public class RPTCashFlowStatementFilterModel : ReportFilterBase
	{
		[DataMember]
		public string MType
		{
			get;
			set;
		}

		public DateTime MFromDate
		{
			get
			{
				return Convert.ToDateTime(MFromDateString);
			}
		}

		public DateTime MToDate
		{
			get
			{
				if (MType == "Monthly")
				{
					DateTime dateTime = MFromDate;
					dateTime = dateTime.AddMonths(1);
					return dateTime.AddSeconds(-1.0);
				}
				return Convert.ToDateTime(MToDateString);
			}
		}

		[DataMember]
		public string MFromDateString
		{
			get;
			set;
		}

		[DataMember]
		public string MToDateString
		{
			get;
			set;
		}
	}
}
