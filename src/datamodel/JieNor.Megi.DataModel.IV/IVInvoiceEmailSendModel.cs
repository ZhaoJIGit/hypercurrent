using JieNor.Megi.EntityModel.Enum;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVInvoiceEmailSendModel
	{
		[DataMember]
		public string MEmailTemplate
		{
			get;
			set;
		}

		[DataMember]
		public bool MIncludePDF
		{
			get;
			set;
		}

		[DataMember]
		public bool MSendMeACopy
		{
			get;
			set;
		}

		[DataMember]
		public string MReplyEmail
		{
			get;
			set;
		}

		[DataMember]
		public List<IVInvoiceSendModel> SendEntryList
		{
			get;
			set;
		}

		[DataMember]
		public EmailSendTypeEnum MSendType
		{
			get;
			set;
		}

		[DataMember]
		public int MStatus
		{
			get;
			set;
		}

		[DataMember]
		public string MBeginDate
		{
			get;
			set;
		}

		[DataMember]
		public string MEndDate
		{
			get;
			set;
		}

		[DataMember]
		public string MFromUserName
		{
			get;
			set;
		}

		[DataMember]
		public string MSubject
		{
			get;
			set;
		}

		[DataMember]
		public string MContent
		{
			get;
			set;
		}

		[DataMember]
		public EmailPreviewTypeEnum PreviewType
		{
			get;
			set;
		}

		[DataMember]
		public int PreviewEntryIndex
		{
			get;
			set;
		}

		[DataMember]
		public string MSalaryPeriod
		{
			get;
			set;
		}
	}
}
