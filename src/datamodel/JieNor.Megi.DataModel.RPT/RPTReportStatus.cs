using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RPT
{
	[DataContract]
	public enum RPTReportStatus
	{
		[EnumMember]
		Draft = 1,
		[EnumMember]
		Published
	}
}
