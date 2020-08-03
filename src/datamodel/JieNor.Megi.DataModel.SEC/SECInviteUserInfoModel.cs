using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.SEC
{
	[DataContract]
	public class SECInviteUserInfoModel
	{
		[DataMember]
		public string MItemID
		{
			get;
			set;
		}

		[DataMember]
		public string MFirstName
		{
			get;
			set;
		}

		[DataMember]
		public string MLastName
		{
			get;
			set;
		}

		[DataMember]
		public string MEmail
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
		public List<SECPermisionGrpOperateModel> MGrpOperateList
		{
			get;
			set;
		}

		[DataMember]
		public string MContactRoles
		{
			get;
			set;
		}

		[DataMember]
		public string MRoleEmployeeInvoice
		{
			get;
			set;
		}

		[DataMember]
		public string MRoleEmployeeInvExpense
		{
			get;
			set;
		}

		[DataMember]
		public string MRoleEmployeeExpExpense
		{
			get;
			set;
		}

		[DataMember]
		public string MRoleAdminBank
		{
			get;
			set;
		}

		[DataMember]
		public string MRoleAdminReport
		{
			get;
			set;
		}

		[DataMember]
		public string MContactRolesManageUsers
		{
			get;
			set;
		}

		[DataMember]
		public string MContactRolesPayrollAdmin
		{
			get;
			set;
		}

		[DataMember]
		public MContext MContext
		{
			get;
			set;
		}

		[DataMember]
		public string MPermStatus
		{
			get;
			set;
		}

		[DataMember]
		public string MIsArchive
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsTemp
		{
			get;
			set;
		}

		[DataMember]
		public string MPassword
		{
			get;
			set;
		}

		[DataMember]
		public string MOrgID
		{
			get;
			set;
		}

		[DataMember]
		public string DefaultEmail
		{
			get;
			set;
		}

		[DataMember]
		public string SendLinkID
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

		public SECInviteUserInfoModel()
		{
			MGrpOperateList = new List<SECPermisionGrpOperateModel>();
		}
	}
}
