using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDBankBalanceModel
	{
		[DataMember]
		public string MBankID
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MBizDate
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTotalAmtFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTotalAmt
		{
			get;
			set;
		}
	}
}
