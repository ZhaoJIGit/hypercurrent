using JieNor.Megi.Core;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class RPTBankBillRecSummaryFilterModel : ReportFilterBase
	{
		[DataMember]
		public string MBankAccountID
		{
			get;
			set;
		}

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
	}
}
