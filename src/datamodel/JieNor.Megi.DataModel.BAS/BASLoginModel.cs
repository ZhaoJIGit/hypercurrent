using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BAS
{
	[DataContract]
	public class BASLoginModel
	{
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
	}
}
