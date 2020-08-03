using JieNor.Megi.Core;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RPT
{
	[DataContract]
	public class RPTSalseByItemFilterModel : ReportFilterBase
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
		public string MSortBy
		{
			get;
			set;
		}

		[DataMember]
		public bool ShowInCNY
		{
			get;
			set;
		}
	}
}
