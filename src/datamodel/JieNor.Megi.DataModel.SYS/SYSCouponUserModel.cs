using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.SYS
{
	[DataContract]
	public class SYSCouponUserModel : BaseModel
	{
		[DataMember]
		public string MCouponID
		{
			get;
			set;
		}

		[DataMember]
		public string MUserID
		{
			get;
			set;
		}

		public SYSCouponUserModel()
			: base("T_Sys_CouponUser")
		{
		}
	}
}
