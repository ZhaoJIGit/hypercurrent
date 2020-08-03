using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.DataModel.SYS;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BAS
{
	[DataContract]
	public class SYSOrgAddModel
	{
		[DataMember]
		public BASOrganisationModel OrgModel
		{
			get;
			set;
		}

		[DataMember]
		public BASOrgContactModel OrgContactModel
		{
			get;
			set;
		}

		[DataMember]
		public BASOrgAddressModel OrgAddressPostalModel
		{
			get;
			set;
		}

		[DataMember]
		public BASOrgAddressModel OrgAddressPhysicalModel
		{
			get;
			set;
		}

		[DataMember]
		public SYSOrgAppModel OrgAppModel
		{
			get;
			set;
		}

		[DataMember]
		public SYSOrgAppStorageModel OrgAppStorageModel
		{
			get;
			set;
		}

		[DataMember]
		public SECUserModel UserModel
		{
			get;
			set;
		}

		[DataMember]
		public SECUserlModel UserlModel
		{
			get;
			set;
		}

		[DataMember]
		public SECRoleModel SecRoleModel
		{
			get;
			set;
		}

		[DataMember]
		public SECRoleUserModel SecRoleUserModel
		{
			get;
			set;
		}

		[DataMember]
		public SECOrgUserModel SecOrgUserModel
		{
			get;
			set;
		}
	}
}
