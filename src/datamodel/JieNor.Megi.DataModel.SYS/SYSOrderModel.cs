using JieNor.Megi.Core.DataModel;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.SYS
{
	[DataContract]
	public class SYSOrderModel : BaseModel
    {
		public SYSOrderModel() : base("T_SYS_Order")
		{
		}

		[DataMember] 
		public string MItemID { get; set; }

		[DataMember] 
		public string MNumber { get; set; }

		[DataMember] 
		public string MOrgID { get; set; }

		[DataMember] 
		public decimal MAmount { get; set; }


		[DataMember] 
		public string MDesc { get; set; }


		[DataMember] 
		public DateTime MSubmitTime { get; set; }


		[DataMember] 
		public DateTime MPayTime { get; set; }


		[DataMember] 
		public string MBizType { get; set; }


		[DataMember] 
		public SYSOrderStatus MStatus { get; set; }


		[DataMember] 
		public DateTime MCancelTime { get; set; }


		[DataMember] 
		public DateTime MCompleteTime { get; set; }


		[DataMember] 
		public string MOutOrderId { get; set; }


		[DataMember] 
		public decimal? MOutFee { get; set; }


		[DataMember] 
		public decimal? MActualAmount { get; set; }


		[DataMember] 
		public SYSPayType MPayType { get; set; }


		[DataMember] 
		public string MPayAccountType { get; set; }

	}

	public enum SYSPayType
	{
		[EnumMember]
		Alipay=100
	}

	public enum SYSPayAccountType
	{

	}

	public enum SYSOrderStatus
	{
		[EnumMember] Draft = 100,
		[EnumMember] WatiPay ,
		[EnumMember] Paid,
		[EnumMember] Complete,
		[EnumMember] Cancel = 1000,
	}
}
