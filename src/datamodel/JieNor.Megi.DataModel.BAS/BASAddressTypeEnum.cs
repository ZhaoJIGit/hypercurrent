using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BAS
{
	[DataContract]
	public enum BASAddressTypeEnum
	{
		[DataMember]
		Postal = 1,
		[DataMember]
		Physical
	}
}
