using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.SEC
{
	[DataContract]
	public class SECPermissionEditModel
	{
		[DataMember]
		public string MRoleID
		{
			get;
			set;
		}

		[DataMember]
		public string MRoleName
		{
			get;
			set;
		}

		[DataMember]
		public List<ObjectPermissionModel> BizObjects
		{
			get;
			set;
		}
	}
}
