using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVInvoiceSendModel
	{
		[DataMember]
		public string MID
		{
			get;
			set;
		}

		[DataMember]
		public string MInvNumber
		{
			get;
			set;
		}

		[DataMember]
		public decimal MAmount
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsIncludePDFAttachment
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsSendMeACopy
		{
			get;
			set;
		}

		[DataMember]
		public string MContactID
		{
			get;
			set;
		}

		[DataMember]
		public string MContactName
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
		public string MContactPrimaryPerson
		{
			get;
			set;
		}

		[DataMember]
		public string MOriginalContactEmail
		{
			get;
			set;
		}

		[DataMember]
		public string MContactEmail
		{
			get;
			set;
		}

		[DataMember]
		public string MContactNetwork
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsSent
		{
			get;
			set;
		}

		[DataMember]
		public string MFilePath
		{
			get;
			set;
		}
	}
}
