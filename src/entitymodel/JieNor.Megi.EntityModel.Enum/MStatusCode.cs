using System.Runtime.Serialization;

namespace JieNor.Megi.EntityModel.Enum
{
	[DataContract]
	public enum MStatusCode
	{
		[EnumMember]
		Others,
		[EnumMember]
		ModelInvalid = 250,
		[EnumMember]
		AccessDenied = 252
	}
}
