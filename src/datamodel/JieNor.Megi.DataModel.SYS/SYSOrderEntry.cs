using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.SYS
{

	[DataContract]
	public class SYSOrderEntry 
	{
		[DataMember]
		public string MItemID { get; set; }

		[DataMember]
		public string MNumber { get; set; }

		[DataMember]
		public string MOrgID { get; set; }

		[DataMember]
		public decimal MAmount { get; set; }


		//[DataMember]
		//public string MDesc { get; set; }


		[DataMember]
		public DateTime MSubmitTime { get; set; }


		[DataMember]
		public DateTime? MPayTime { get; set; }


		//[DataMember]
		//public string MBizType { get; set; }


		[DataMember]
		public SYSOrderStatus MStatus { get; set; }


		[DataMember]
		public DateTime? MCancelTime { get; set; }


		[DataMember]
		public DateTime? MCompleteTime { get; set; }


		//[DataMember]
		//public string MOutOrderId { get; set; }


		//[DataMember]
		//public decimal? MOutFee { get; set; }


		[DataMember]
		public decimal? MActualAmount { get; set; }


		[DataMember]
		public SYSPayType MPayType { get; set; }
		/// <summary>
		/// 只支持3,6，12期
		/// </summary>
		[DataMember]
		public string HBFQNum { get; set; }
		/// <summary>
		/// 商家付费 100，客户付费 0
		/// </summary>
		[DataMember]
		public string HbFqSellerPercent { get; set; }
		//[DataMember]
		//public string MPayAccountType { get; set; }

	}
}
