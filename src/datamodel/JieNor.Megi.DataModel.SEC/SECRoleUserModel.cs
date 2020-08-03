using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.SEC
{
	[DataContract]
	public class SECRoleUserModel : BDModel
	{
		[DataMember]
		public string MUserID
		{
			get;
			set;
		}

		[DataMember]
		public string MRoleID
		{
			get;
			set;
		}

		public SECRoleUserModel()
			: base("T_Sec_RoleUser")
		{
		}
	}
}
