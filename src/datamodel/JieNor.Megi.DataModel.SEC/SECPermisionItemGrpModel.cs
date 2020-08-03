using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.SEC
{
	[DataContract]
	public class SECPermisionItemGrpModel : BDModel
	{
		[DataMember]
		public string MGroupID
		{
			get;
			set;
		}

		[DataMember]
		public string MPermItemID
		{
			get;
			set;
		}

		[DataMember]
		public string MBizObjID
		{
			get;
			set;
		}

		public SECPermisionItemGrpModel()
			: base("T_Sec_ObjectPermissionGrp")
		{
		}
	}
}
