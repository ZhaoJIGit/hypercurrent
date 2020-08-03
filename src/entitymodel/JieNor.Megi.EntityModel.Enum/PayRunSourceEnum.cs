using System.Runtime.Serialization;

namespace JieNor.Megi.EntityModel.Enum
{
	[DataContract]
	public enum PayRunSourceEnum
	{
		[EnumMember]
		None,
		[EnumMember]
		Copy,
		[EnumMember]
		New,
		[EnumMember]
		Import
	}
}
