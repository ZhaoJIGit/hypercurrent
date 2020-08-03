using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Import.Account
{
	[DataContract]
	public enum IOAccountMatchResultEnum
	{
		[EnumMember]
		None,
		[EnumMember]
		Matched,
		[EnumMember]
		AutoAdd,
		[EnumMember]
		ManualMatch,
		[EnumMember]
		AutoUpdate
	}
}
