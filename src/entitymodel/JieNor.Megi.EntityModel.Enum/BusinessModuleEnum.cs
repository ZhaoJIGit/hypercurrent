using System.Runtime.Serialization;

namespace JieNor.Megi.EntityModel.Enum
{
	[DataContract]
	public enum BusinessModuleEnum
	{
		[EnumMember]
		Home,
		[EnumMember]
		Bank,
		[EnumMember]
		Invoice,
		[EnumMember]
		Purchase,
		[EnumMember]
		Expense,
		[EnumMember]
		Report
	}
}
