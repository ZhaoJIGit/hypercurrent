using JieNor.Megi.Core;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RPT
{
	[DataContract]
	public class RPTBalanceSheetFilterModel : ReportFilterBase
	{
		[DataMember]
		public string MDateString
		{
			get;
			set;
		}

		public DateTime MDate
		{
			get
			{
				return Convert.ToDateTime(MDateString);
			}
		}
	}
}
