using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.SYS
{
	[DataContract]
	public enum SYSCouponStatu
	{
		[EnumMember]
		Draf = 201,
		[EnumMember]
		Active,
		[EnumMember]
		Disabled,
		[EnumMember]
		Used
	}
}
