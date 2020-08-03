using JieNor.Megi.Core;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class RPTBankStatementFilterModel : ReportFilterBase
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
		public DateTime MEndDate
		{
			get;
			set;
		}

		public bool MIsReconciled
		{
			get;
			set;
		}
	}
}
