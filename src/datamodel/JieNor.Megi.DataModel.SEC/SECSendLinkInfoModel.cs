using JieNor.Megi.Core.DataModel;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.SEC
{
	[DataContract]
	public class SECSendLinkInfoModel : BDModel
	{
		[DataMember]
		public string PlanCode { get; set; }


		[DataMember]
		public string MEmail
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MExpireDate
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
		public string MPhone
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MSendDate
		{
			get;
			set;
		}

		[DataMember]
		public int MLinkType
		{
			get;
			set;
		}

		[DataMember]
		public string MInvitationOrgID
		{
			get;
			set;
		}

		[DataMember]
		public string MInvitationEmail
		{
			get;
			set;
		}

		public SECSendLinkInfoModel()
			: base("T_Sec_SendLinkInfo")
		{
		}
	}
}
