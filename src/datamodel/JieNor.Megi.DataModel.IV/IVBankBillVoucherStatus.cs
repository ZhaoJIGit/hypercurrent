using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public enum IVBankBillVoucherStatus
	{
		[EnumMember]
		NonGenerated = 1,
		[EnumMember]
		UnGenerate,
		[EnumMember]
		Generated
	}
}
