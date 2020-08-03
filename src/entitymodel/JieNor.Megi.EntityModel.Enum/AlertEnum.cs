using System.Runtime.Serialization;

namespace JieNor.Megi.EntityModel.Enum
{
	[DataContract]
	public enum AlertEnum
	{
		[EnumMember]
		Default,
		[EnumMember]
		Success,
		[EnumMember]
		Info,
		[EnumMember]
		Warning,
		[EnumMember]
		Danger,
		[EnumMember]
		Error
	}
}
