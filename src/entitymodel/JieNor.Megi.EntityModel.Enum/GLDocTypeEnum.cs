using System.Runtime.Serialization;

namespace JieNor.Megi.EntityModel.Enum
{
	[DataContract]
	public enum GLDocTypeEnum
	{
		[EnumMember]
		Invoice,
		[EnumMember]
		Bill,
		[EnumMember]
		Expense,
		[EnumMember]
		Receive,
		[EnumMember]
		Payment,
		[EnumMember]
		Transfer,
		[EnumMember]
		Salary
	}
}
