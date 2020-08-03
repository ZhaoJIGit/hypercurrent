using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDOrgCurrencyModel
	{
		[DataMember]
		public bool IsSource
		{
			get;
			set;
		}

		[DataMember]
		public string CurrencyID
		{
			get;
			set;
		}

		[DataMember]
		public string SrcCurrencyID
		{
			get;
			set;
		}

		[DataMember]
		public string CurrencyName
		{
			get;
			set;
		}

		[DataMember]
		public decimal SrcToTgtRate
		{
			get;
			set;
		}

		[DataMember]
		public decimal TgtToSrcRate
		{
			get;
			set;
		}
	}
}
