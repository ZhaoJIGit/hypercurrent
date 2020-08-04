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
            /// ÷ß∏∂¿‡–Õ
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
