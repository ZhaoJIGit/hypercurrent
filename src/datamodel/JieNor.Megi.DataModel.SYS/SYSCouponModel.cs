using JieNor.Megi.Core.DataModel;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.SYS
{
	[DataContract]
	public class SYSCouponModel : BaseModel
	{
		[DataMember]
		public string MItemID
		{
			get;
			set;
		}

		[DataMember]
		public string MCode
		{
			get;
			set;
		}

		[DataMember]
		public int MTypeID
		{
			get;
			set;
		}

		[DataMember]
		public string MValue
		{
			get;
			set;
		}

		[DataMember]
		public int MStatuID
		{
			get;
			set;
		}

		[DataMember]
		public string MAffiliateID
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MFromDate
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MToDate
		{
			get;
			set;
		}

		[DataMember]
		public string MApplyOrgID
		{
			get;
			set;
		}

		[DataMember]
		public string MApplyUserID
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MApplyDate
		{
			get;
			set;
		}

		public SYSCouponModel()
			: base("T_Sys_Coupon")
		{
		}
	}
}
