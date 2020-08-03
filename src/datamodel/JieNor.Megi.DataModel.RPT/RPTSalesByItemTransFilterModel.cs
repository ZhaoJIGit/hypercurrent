using JieNor.Megi.Core;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RPT
{
	public class RPTSalesByItemTransFilterModel : ReportFilterBase
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
		public string MItem
		{
			get;
			set;
		}

		public string MCurrency
		{
			get;
			set;
		}
	}
}
