using System.ComponentModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.Invoice
{
	[DataContract]
	public class InvoiceModel : ExportIVBaseModel
	{
		[DisplayName("BillToTitle")]
		[DataMember]
		public string ToTitle
		{
			get;
			set;
		}

		[DisplayName("BillFromTitle")]
		[DataMember]
		public string FromTitle
		{
			get;
			set;
		}

		[DataMember]
		public string NumberTitle
		{
			get;
			set;
		}

		[DataMember]
		public string NumberContainTitle
		{
			get;
			set;
		}

		[DataMember]
		public string RefTitle
		{
			get;
			set;
		}

		[DataMember]
		public string RefContainTitle
		{
			get;
			set;
		}

		[DataMember]
		public string DateTitle
		{
			get;
			set;
		}

		[DataMember]
		public string DateContainTitle
		{
			get;
			set;
		}

		[DataMember]
		public string DueDateTitle
		{
			get;
			set;
		}

		[DataMember]
		public string DueDateContainTitle
		{
			get;
			set;
		}

		[DataMember]
		public string ExpectedDateTitle
		{
			get;
			set;
		}

		[DataMember]
		public string ExpectedDateContainTitle
		{
			get;
			set;
		}

		[DataMember]
		public string CurrencyTitle
		{
			get;
			set;
		}

		[DataMember]
		public string TaxTypeTitle
		{
			get;
			set;
		}

		[DataMember]
		public string TaxAmountTitle
		{
			get;
			set;
		}

		[DataMember]
		public string SubTotalTitle
		{
			get;
			set;
		}

		[DataMember]
		public string TotalTitle
		{
			get;
			set;
		}

		[DataMember]
		public string ItemCodeTitle
		{
			get;
			set;
		}

		[DataMember]
		public string DescriptionTitle
		{
			get;
			set;
		}

		[DataMember]
		public string ItemAndDescriptionTitle
		{
			get;
			set;
		}

		[DataMember]
		public string QuantityTitle
		{
			get;
			set;
		}

		[DataMember]
		public string UnitPriceTitle
		{
			get;
			set;
		}

		[DataMember]
		public string DiscountTitle
		{
			get;
			set;
		}

		[DataMember]
		public string TaxRateTitle
		{
			get;
			set;
		}

		[DataMember]
		public string PrimaryPersonTitle
		{
			get;
			set;
		}

		[DataMember]
		public string NotesTitle
		{
			get;
			set;
		}

		[DataMember]
		public string NotesContent
		{
			get;
			set;
		}

		[DisplayName("Type")]
		[DataMember]
		public string MType
		{
			get;
			set;
		}

		[DataMember]
		public string InvoiceStatus
		{
			get;
			set;
		}

		[DisplayName("ContactName")]
		[DataMember]
		public string MContactName
		{
			get;
			set;
		}

		[DataMember]
		public string PrimaryPersonName
		{
			get;
			set;
		}

		[DisplayName("Date")]
		[DataMember]
		public string MBizDate
		{
			get;
			set;
		}

		[DisplayName("DueDate")]
		[DataMember]
		public string MDueDate
		{
			get;
			set;
		}

		[DisplayName("ExpectedPaymentDate")]
		[DataMember]
		public string MExpectedDate
		{
			get;
			set;
		}

		[DisplayName("InvoiceNumber")]
		[DataMember]
		public string MNumber
		{
			get;
			set;
		}

		[DisplayName("Reference")]
		[DataMember]
		public string MReference
		{
			get;
			set;
		}

		[DisplayName("TaxType")]
		[DataMember]
		public string MTaxID
		{
			get;
			set;
		}

		[DisplayName("CurrencyFullName")]
		[DataMember]
		public string MCyID
		{
			get;
			set;
		}

		[DataMember]
		public string ExchangeInfo
		{
			get;
			set;
		}

		[DataMember]
		public string TrackItem1Name
		{
			get;
			set;
		}

		[DataMember]
		public string TrackItem2Name
		{
			get;
			set;
		}

		[DataMember]
		public string TrackItem3Name
		{
			get;
			set;
		}

		[DataMember]
		public string TrackItem4Name
		{
			get;
			set;
		}

		[DataMember]
		public string TrackItem5Name
		{
			get;
			set;
		}

		[DataMember]
		public string AmountTitle
		{
			get;
			set;
		}

		[DataMember]
		public string SubTotal
		{
			get;
			set;
		}

		[DataMember]
		public string Total
		{
			get;
			set;
		}

		[DisplayName("Currency")]
		[DataMember]
		public string TotalCurrency
		{
			get;
			set;
		}

		[DataMember]
		public InvoiceEntryCollection InvoiceEntries
		{
			get;
			set;
		}

		[DataMember]
		public InvoiceTaxCollection InvoiceTaxes
		{
			get;
			set;
		}

		[DataMember]
		public string AmountDueTitle
		{
			get;
			set;
		}

		[DataMember]
		public string AmountDue
		{
			get;
			set;
		}

		[DataMember]
		public InvoiceVerificationCollection InvoiceVerifications
		{
			get;
			set;
		}
	}
}
