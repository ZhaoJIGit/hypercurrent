using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.SYS
{
	[DataContract]
	public enum SYSCouponType
	{
		[EnumMember]
		Trial = 101,
		[EnumMember]
		Discount
	}
}
