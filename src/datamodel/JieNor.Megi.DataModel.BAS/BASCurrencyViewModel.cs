using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BAS
{
	[DataContract]
	public class BASCurrencyViewModel : BASCurrencyModel
	{
		[DataMember]
		public string MLocalName
		{
			get;
			set;
		}

		[DataMember]
		public string MCurrencyID
		{
			get;
			set;
		}
	}
}
