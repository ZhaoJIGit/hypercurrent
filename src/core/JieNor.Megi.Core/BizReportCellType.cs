using System.Runtime.Serialization;

namespace JieNor.Megi.Core
{
	[DataContract]
	public enum BizReportCellType
	{
		[EnumMember]
		Text = 1,
		[EnumMember]
		Money,
		[EnumMember]
		Date,
		[EnumMember]
		DateTime,
		[EnumMember]
		Price,
		[EnumMember]
		TextRight,
		[EnumMember]
		TreeText
	}
}
