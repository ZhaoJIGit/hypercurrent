using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVCashCodingEditModel
	{
		[DataMember]
		public string MBankID
		{
			get;
			set;
		}

		[DataMember]
		public decimal MExchangeRate
		{
			get;
			set;
		}

		[DataMember]
		public List<IVBankBillEntryModel> MBankBillEntryList
		{
			get;
			set;
		}
	}
}
