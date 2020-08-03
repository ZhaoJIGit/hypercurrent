using System.Runtime.Serialization;

namespace JieNor.Megi.EntityModel.MultiLanguage
{
	[DataContract]
	public class GlobalCurrencyModel
	{
		[DataMember]
		public string MCurrencyID
		{
			get;
			set;
		}

		[DataMember]
		public int MPricePrecision
		{
			get;
			set;
		}

		[DataMember]
		public int MAmountPrecision
		{
			get;
			set;
		}

		[DataMember]
		public string MCurrencySymbol
		{
			get;
			set;
		}
	}
}
