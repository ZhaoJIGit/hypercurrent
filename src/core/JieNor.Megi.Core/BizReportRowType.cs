using System.Runtime.Serialization;

namespace JieNor.Megi.Core
{
	[DataContract]
	public enum BizReportRowType
	{
		[EnumMember]
		Header = 1,
		[EnumMember]
		Group,
		[EnumMember]
		Item,
		[EnumMember]
		SubItem = 0x1F,
		[EnumMember]
		SubTotal = 4,
		[EnumMember]
		Total
	}
}
