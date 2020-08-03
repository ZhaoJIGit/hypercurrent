using JieNor.Megi.Core.DataModel;
using JieNor.Megi.EntityModel.Enum;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.SEC
{
	[DataContract]
	public class SECOrgUserModel : BDModel
	{
		[DataMember]
		public string MUserID
		{
			get;
			set;
		}

		[DataMember]
		[Email("Email Address", OperateTime.Save)]
		public string MEmailAddress
		{
			get;
			set;
		}

		[DataMember]
		public bool MUserIsActive
		{
			get;
			set;
		}

		[DataMember]
		public string MPosition
		{
			get;
			set;
		}

		[DataMember]
		public string MRole
		{
			get;
			set;
		}

		[DataMember]
		public bool IsSelfData
		{
			get;
			set;
		}

		public SECOrgUserModel()
			: base("T_Sec_OrgUser")
		{
		}
	}
}
