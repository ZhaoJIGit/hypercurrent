using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.SYS
{
	[DataContract]
	public class SYSOrderLogModel : BaseModel
	{
        public SYSOrderLogModel() : base("T_SYS_OrderLog")
        {
        }

        [DataMember] public string MItemID { get; set; }
        [DataMember] public string MOrderID { get; set; }
        [DataMember] public string MOrderOpType { get; set; }
        [DataMember] public string MDesc { get; set; }


    }
}
