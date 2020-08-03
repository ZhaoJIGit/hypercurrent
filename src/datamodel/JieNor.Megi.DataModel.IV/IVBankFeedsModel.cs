using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVBankFeedsModel
	{
		[DataMember]
		public string AcctID
		{
			get;
			set;
		}

		[DataMember]
		public string AcctTypeID
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
	}
}
