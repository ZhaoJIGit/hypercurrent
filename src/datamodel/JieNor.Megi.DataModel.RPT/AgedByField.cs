using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RPT
{
	[DataContract]
	public enum AgedByField
	{
		[EnumMember]
		None,
		[EnumMember]
		InvoiceDate,
		[EnumMember]
		DueDate
	}
}
