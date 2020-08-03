using System.Runtime.Serialization;

namespace JieNor.Megi.EntityModel.Enum
{
	[DataContract]
	public enum EmailSendTypeEnum
	{
		[EnumMember]
		None,
		[EnumMember]
		Invoice,
		[EnumMember]
		Statement,
		[EnumMember]
		RepeatingInvoice,
		[EnumMember]
		Payslip
	}
}
