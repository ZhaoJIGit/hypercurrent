using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.SEC
{
	[DataContract]
	public class SECRolePermisionGroupModel : BDModel
	{
		[DataMember]
		public string MRoleID
		{
			get;
			set;
		}

		[DataMember]
		public string MPermGrpID
		{
			get;
			set;
		}

		[DataMember]
		public bool MView
		{
			get;
			set;
		}

		[DataMember]
		public bool MChange
		{
			get;
			set;
		}

		[DataMember]
		public bool MApprove
		{
			get;
			set;
		}

		[DataMember]
		public bool MExport
		{
			get;
			set;
		}

		public SECRolePermisionGroupModel()
			: base("T_Sec_RoleObjectGroup")
		{
		}
	}
}
