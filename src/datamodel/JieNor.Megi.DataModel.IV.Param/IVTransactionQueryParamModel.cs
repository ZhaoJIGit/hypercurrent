using JieNor.Megi.Core;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV.Param
{
	[DataContract]
	public class IVTransactionQueryParamModel : SqlWhere
	{
		[DataMember]
		public string MBankID
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
		public string MID
		{
			get;
			set;
		}
	}
}
