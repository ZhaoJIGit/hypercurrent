using System;
using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.SYS
{
	[DataContract]
	public class SYSOrderEntryModel : BaseModel
	{
		public SYSOrderEntryModel():base("T_SYS_OrderEntry")
		{

		}
		[DataMember] public string MItemID { get; set; }
		[DataMember] public string MEntryID { get; set; }
		[DataMember] public string MSeq { get; set; }
		[DataMember] public int MQty { get; set; }
		[DataMember] public decimal MPrice { get; set; }
		[DataMember] public decimal MAmount { get; set; }
		[DataMember] public decimal MDiscountAmount { get; set; }
		[DataMember] public string MDesc { get; set; }
	}
}
