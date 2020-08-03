using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.SEC
{
	[DataContract]
	public class SECRolePermissionModel : BDModel
	{
		[DataMember]
		public string MRoleID
		{
			get;
			set;
		}

		[DataMember]
		public string MBizObjectID
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
		public bool MIsGrant
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsRefuset
		{
			get;
			set;
		}

		public SECRolePermissionModel()
			: base("T_Sec_RolePermission")
		{
		}
	}
}
