using JieNor.Megi.Core;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RPT
{
	[DataContract]
	public class RPTIncomeStatementFilterModel : ReportFilterBase
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
				DateTime dateTime;
				if (MType == "Monthly")
				{
					dateTime = MFromDate;
					dateTime = dateTime.AddMonths(1);
					return dateTime.AddSeconds(-1.0);
				}
				dateTime = Convert.ToDateTime(MToDateString);
				dateTime = dateTime.Date;
				dateTime = dateTime.AddMonths(1);
				return dateTime.AddSeconds(-1.0);
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

		[DataMember]
		public int MCompareSpan
		{
			get;
			set;
		}
	}
}
