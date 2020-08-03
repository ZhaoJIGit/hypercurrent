using System.Runtime.Serialization;

namespace JieNor.Megi.EntityModel.Enum
{
	[DataContract]
	public enum EmailPreviewTypeEnum
	{
		[EnumMember]
		None,
		[EnumMember]
		Subject,
		[EnumMember]
		Content
	}
}
