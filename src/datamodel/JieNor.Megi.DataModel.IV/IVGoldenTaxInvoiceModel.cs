using JieNor.Megi.Core.DataModel;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVGoldenTaxInvoiceModel : BizDataModel
	{
		[DataMember]
		public string MType
		{
			get;
			set;
		}

		[DataMember]
		public string MNO
		{
			get;
			set;
		}

		[DataMember]
		public int MInvoiceSource
		{
			get;
			set;
		}

		[DataMember]
		public int MInvoiceType
		{
			get;
			set;
		}

		[DataMember]
		public string MTypeCode
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MOpenDate
		{
			get;
			set;
		}

		[DataMember]
		public string MPurName
		{
			get;
			set;
		}

		[DataMember]
		public string MPurTaxNumber
		{
			get;
			set;
		}

		[DataMember]
		public string MPurBankAccount
		{
			get;
			set;
		}

		[DataMember]
		public string MPurAddress
		{
			get;
			set;
		}

		[DataMember]
		public string MSalName
		{
			get;
			set;
		}

		[DataMember]
		public string MSalTaxNumber
		{
			get;
			set;
		}

		[DataMember]
		public string MSalBankAccount
		{
			get;
			set;
		}

		[DataMember]
		public string MSalAddress
		{
			get;
			set;
		}

		[DataMember]
		public string MCurrency
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTotalAmt
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTaxTotalAmt
		{
			get;
			set;
		}

		[DataMember]
		public decimal MVerificationAmt
		{
			get;
			set;
		}

		[DataMember]
		public string MDrawer
		{
			get;
			set;
		}

		[DataMember]
		public string MReview
		{
			get;
			set;
		}

		[DataMember]
		public string MReceipt
		{
			get;
			set;
		}

		[DataMember]
		public string MExpressCompany
		{
			get;
			set;
		}

		[DataMember]
		public string MExpressNumber
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MExpressDate
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsPrint
		{
			get;
			set;
		}

		[DataMember]
		public List<IVGoldenTaxInvoiceEntryModel> Entry
		{
			get;
			set;
		}

		[DataMember]
		public List<IVGoldenTaxInvoiceAttachmentModel> Attachment
		{
			get;
			set;
		}

		[DataMember]
		public string MReference
		{
			get;
			set;
		}

		public IVGoldenTaxInvoiceModel()
			: base("T_IV_GoldenTaxInvoice")
		{
			Entry = new List<IVGoldenTaxInvoiceEntryModel>();
			Attachment = new List<IVGoldenTaxInvoiceAttachmentModel>();
		}
	}
}
