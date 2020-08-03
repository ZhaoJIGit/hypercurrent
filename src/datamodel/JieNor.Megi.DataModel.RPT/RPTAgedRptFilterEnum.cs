using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RPT
{
	[DataContract]
	public enum RPTAgedRptFilterEnum
	{
		[EnumMember]
		None,
		[EnumMember]
		Payables,
		[EnumMember]
		Receivables
	}
}
