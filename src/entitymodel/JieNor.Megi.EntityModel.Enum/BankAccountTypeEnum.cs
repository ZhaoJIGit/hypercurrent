using System.Runtime.Serialization;

namespace JieNor.Megi.EntityModel.Enum
{
	[DataContract]
	public enum BankAccountTypeEnum
	{
		[EnumMember]
		Others,
		[EnumMember]
		Bank,
		[EnumMember]
		Credit,
		[EnumMember]
		Cash,
		[EnumMember]
		PayPal,
		[EnumMember]
		Alipay
	}
}
