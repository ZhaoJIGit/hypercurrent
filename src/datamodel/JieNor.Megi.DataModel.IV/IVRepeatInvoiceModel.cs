using JieNor.Megi.Core.Attribute;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVRepeatInvoiceModel : IVBaseModel<IVRepeatInvoiceEntryModel>
	{
		[DataMember(Order = 201)]
		[ApiMember("Type")]
		public string MType
		{
			get;
			set;
		}

		[DataMember]
		public int MRepeatNumber
		{
			get;
			set;
		}

		[DataMember]
		public string MRepeatType
		{
			get;
			set;
		}

		[DataMember]
		public int MDueDateNumber
		{
			get;
			set;
		}

		[DataMember]
		public string MDueDateType
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MEndDate
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MExpectedDate
		{
			get;
			set;
		}

		[DataMember]
		public string MBranding
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
		public bool MIsMarkAsSent
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

		[ModelEntry]
		[DataMember]
		[ApiDetail]
		public List<IVRepeatInvoiceEntryModel> RepeatInvoiceEntry
		{
			get
			{
				return base.MEntryList;
			}
			set
			{
				base.MEntryList = value;
			}
		}

		[DataMember]
		public List<IVInvoiceAttachmentModel> RepeatInvoiceAttachment
		{
			get;
			set;
		}

		public IVRepeatInvoiceModel()
			: base("T_IV_RepeatInvoice")
		{
			RepeatInvoiceEntry = new List<IVRepeatInvoiceEntryModel>();
			RepeatInvoiceAttachment = new List<IVInvoiceAttachmentModel>();
		}
	}
}
