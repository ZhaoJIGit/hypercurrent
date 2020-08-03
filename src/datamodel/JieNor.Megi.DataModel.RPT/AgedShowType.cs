using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RPT
{
	[DataContract]
	public enum AgedShowType
	{
		[EnumMember]
		None,
		[EnumMember]
		MonthName,
		[EnumMember]
		MonthNumber
	}
}
