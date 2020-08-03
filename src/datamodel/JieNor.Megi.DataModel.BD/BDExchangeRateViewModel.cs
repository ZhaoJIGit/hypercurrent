using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDExchangeRateViewModel : BDExchangeRateModel
	{
		[DataMember]
		public string MSourceCurrencyName
		{
			get;
			set;
		}

		[DataMember]
		public string MTargetCurrencyName
		{
			get;
			set;
		}
	}
}
