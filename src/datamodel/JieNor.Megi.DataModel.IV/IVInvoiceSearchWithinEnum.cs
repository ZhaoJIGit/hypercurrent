using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public enum IVInvoiceSearchWithinEnum
	{
		[EnumMember]
		None,
		[EnumMember]
		AnyDate,
		[EnumMember]
		TransactionDate,
		[EnumMember]
		DueDate,
		[EnumMember]
		ExpectedDate,
		[EnumMember]
		NextInvoiceDate,
		[EnumMember]
		EndDate,
		[EnumMember]
		OpenDate
	}
}
