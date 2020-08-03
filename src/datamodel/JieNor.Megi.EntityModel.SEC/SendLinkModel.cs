using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.EntityModel.SEC
{
	[DataContract]
	public class SendLinkModel
	{
		[DataMember]
		public string MItemID
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
		public string MPhone
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
		public DateTime MSendDate
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
	}
}
