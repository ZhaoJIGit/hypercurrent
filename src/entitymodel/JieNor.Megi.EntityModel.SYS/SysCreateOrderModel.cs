using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.EntityModel.SYS
{
	[DataContract]
     	public class SysCreateOrderModel
     	{
     		[DataMember]
     		public string OrgId
     		{
     			get;
     			set;
     		}
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
        /// <summary>
        /// 支付类型
        /// </summary>
        [DataMember]
            public int MPayType { get; set; }

            [DataMember]
     		public List<SysCreateOrderItemModel> Items
     		{
     			get;
     			set;
     		}
     	}
        
        
        public class SysCreateOrderItemModel
        {
	        [DataMember]
	        public string SkuId 
	        {
		        get;
		        set;
	        }
     
	        [DataMember]
	        public int Qty 
	        {
		        get;
		        set;
	        }
        }
        
        
        [DataContract]
        public class SysCreatePaymentResultModel
        {
	        [DataMember]
	        public string PaymentUrl
	        {
		        get;
		        set;
	        }

        }
}
