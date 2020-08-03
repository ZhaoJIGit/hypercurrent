using System.Runtime.Serialization;

namespace JieNor.Megi.EntityModel.Enum
{
	[DataContract]
	public enum ContactTypeEnum
	{
		[EnumMember]
		None = -1,
		[EnumMember]
		Customer = 1,
		[EnumMember]
		Supplier,
		[EnumMember]
		Archived,
		[EnumMember]
		Other
	}
}
