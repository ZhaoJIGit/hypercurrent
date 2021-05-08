using System.Runtime.Serialization;

namespace JieNor.Megi.EntityModel.SEC
{
	[DataContract]
	public class SECLoginModel
	{
		[DataMember]
		public bool IsConsole
		{
			get;
			set;
		} = false;

		
		[DataMember]
		public string Email
		{
			get;
			set;
		}

		[DataMember]
		public string Password
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
		public string UserId
		{
			get;
			set;
		}

		[DataMember]
		public string UserIp
		{
			get;
			set;
		}

		[DataMember]
		public string OrgId
		{
			get;
			set;
		}

		[DataMember]
		public string OrgName
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
		public int TokenValidMinute
		{
			get;
			set;
		}

		[DataMember]
		public string RedirectUrl
		{
			get;
			set;
		}

		[DataMember]
		public bool Relogin
		{
			get;
			set;
		}

		[DataMember]
		public string ValidateCode
		{
			get;
			set;
		}
	}
}
