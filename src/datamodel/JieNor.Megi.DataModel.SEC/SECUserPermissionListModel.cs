using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.SEC
{
	[DataContract]
	public class SECUserPermissionListModel
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
		public string MFullName
		{
			get;
			set;
		}

		[DataMember]
		public string MPermissions
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
		public string MUserLastLogin
		{
			get;
			set;
		}

		[DataMember]
		public string MUserLoginTimes
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
		public string MIsArchive
		{
			get;
			set;
		}
	}
}
