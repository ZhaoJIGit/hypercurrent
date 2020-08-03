using System.Runtime.Serialization;

namespace JieNor.Megi.EntityModel.Enum
{
	[DataContract]
	public enum SearchColEnum
	{
		[EnumMember]
		Reference,
		[EnumMember]
		Number,
		[EnumMember]
		Amount,
		[EnumMember]
		Contact,
		[EnumMember]
		All
	}
}
