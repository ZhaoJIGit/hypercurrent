using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.EntityModel.Enum;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.SEC
{
	[DataContract]
	public class SECUserModel : BDModel
	{
		[DataMember(Order = 1)]
		[ApiMember("UserID", IsPKField = true)]
		public string MUserID
		{
			get
			{
				return base.MItemID;
			}
			set
			{
				base.MItemID = value;
			}
		}

		[DataMember(Order = 5)]
		[Email("Email Address", OperateTime.Save)]
		[ApiMember("Email")]
		public string MEmailAddress
		{
			get;
			set;
		}

		[DataMember]
		public string MPassWord
		{
			get;
			set;
		}

		[DataMember]
		public string MMobilePhone
		{
			get;
			set;
		}

		[DataMember]
		public string MQQNumber
		{
			get;
			set;
		}

		[DataMember]
		public string MAppID
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MLastLoginDate
		{
			get;
			set;
		}

		[DataMember]
		public string MLastLoginOrgID
		{
			get;
			set;
		}

		[DataMember]
		public string MLastLoginAppID
		{
			get;
			set;
		}

		[DataMember]
		public string MLastLoginLCID
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

		[DataMember(Order = 15)]
		[ApiMember("FirstName")]
		public string MFirstName
		{
			get;
			set;
		}

		[DataMember(Order = 20)]
		[ApiMember("LastName")]
		public string MLastName
		{
			get;
			set;
		}

		[DataMember]
		public string MCurPassWord
		{
			get;
			set;
		}

		[DataMember]
		public string MLastLoginOrgName
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
		public bool MPublicProfile
		{
			get;
			set;
		}

		[DataMember]
		public string MProfileImage
		{
			get;
			set;
		}

		[DataMember]
		public SECUserlModel SECUserLModel
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsChangeEmail
		{
			get;
			set;
		}

		[DataMember]
		public bool MInitBalanceOver
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
		public string MLCID
		{
			get;
			set;
		}

		[DataMember]
		public int MOrgListShowType
		{
			get;
			set;
		}

		[DataMember(Order = 25)]
		[ApiMember("Role")]
		[ApiEnum(EnumMappingType.UserRole)]
		public string MRole
		{
			get;
			set;
		}

		[DataMember(Order = 30)]
		public string MPosition
		{
			get;
			set;
		}

		[DataMember(Order = 30)]
		[ApiMember("Position")]
		[ApiEnum(EnumMappingType.UserPosition)]
		public string[] Position
		{
			get;
			set;
		}

		[DataMember(Order = 35)]
		public bool MUserIsActive
		{
			get;
			set;
		}

		public bool MIsArchive
		{
			get;
			set;
		}

		[ApiMember("Status")]
		public string MStatus
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsHadAddOrgAuth
		{
			get;
			set;
		}

		public SECUserModel()
			: base("T_Sec_User")
		{
		}
	}
}
