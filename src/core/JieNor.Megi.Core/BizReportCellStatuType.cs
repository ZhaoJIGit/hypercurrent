using System.Runtime.Serialization;

namespace JieNor.Megi.Core
{
	[DataContract]
	public enum BizReportCellStatuType
	{
		[EnumMember]
		None,
		[EnumMember]
		UpRed,
		[EnumMember]
		UpGreen,
		[EnumMember]
		DownRed,
		[EnumMember]
		DownGreen
	}
}
