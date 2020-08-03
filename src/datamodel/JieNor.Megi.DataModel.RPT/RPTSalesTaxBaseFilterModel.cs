using JieNor.Megi.Core;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RPT
{
	[DataContract]
	public class RPTSalesTaxBaseFilterModel : ReportFilterBase
	{
		[DataMember]
		public DateTime MFromDate
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MToDate
		{
			get;
			set;
		}

		[DataMember]
		public bool ShowByTaxRate
		{
			get;
			set;
		}

		[DataMember]
		public bool ShowByTaxComponent
		{
			get;
			set;
		}
	}
}
