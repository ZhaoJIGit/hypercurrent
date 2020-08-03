using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.SEC
{
	[DataContract]
	public class SECAccountModel
	{
		[DataMember]
		public string MFristName
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
		public string MLocation
		{
			get;
			set;
		}

		[DataMember]
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
		public string SendLinkID
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
	}
}
