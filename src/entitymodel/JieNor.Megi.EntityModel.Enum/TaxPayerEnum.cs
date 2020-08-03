using System.Runtime.Serialization;

namespace JieNor.Megi.EntityModel.Enum
{
	[DataContract]
	public enum TaxPayerEnum
	{
		[EnumMember]
		None,
		[EnumMember]
		GeneralTaxpayer,
		[EnumMember]
		SmallScaleTaxpayer
	}
}
