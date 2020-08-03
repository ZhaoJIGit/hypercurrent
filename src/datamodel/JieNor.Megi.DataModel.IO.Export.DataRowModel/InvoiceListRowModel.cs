using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.DataRowModel
{
	[DataContract]
	public class InvoiceListRowModel : ExportListBaseModel, ICloneable
	{
		[DataMember]
		public string MID
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
		public string ContactName
		{
			get;
			set;
		}

		[DisplayName("Number")]
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

		[DisplayName("InvoiceDate")]
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

		[DisplayName("ExpectedDate")]
		[DataMember]
		public string MExpectedDate
		{
			get;
			set;
		}

		[DataMember]
		public decimal? Total
		{
			get;
			set;
		}

		[DataMember]
		public decimal MExchangeRate
		{
			get;
			set;
		}

		[DataMember]
		public decimal? TaxTotal
		{
			get;
			set;
		}

		[DataMember]
		public decimal? Paid
		{
			get;
			set;
		}

		[DataMember]
		public decimal? Due
		{
			get;
			set;
		}

		[DataMember]
		public decimal? OriginalCurrencyTotal
		{
			get;
			set;
		}

		[DataMember]
		public decimal? OriginalCurrencyTaxTotal
		{
			get;
			set;
		}

		[DataMember]
		public decimal? OriginalCurrencyPaid
		{
			get;
			set;
		}

		[DataMember]
		public decimal? OriginalCurrencyDue
		{
			get;
			set;
		}

		[DataMember]
		public string TaxType
		{
			get;
			set;
		}

		[DataMember]
		public string Currency
		{
			get;
			set;
		}

		[DataMember]
		public string Type
		{
			get;
			set;
		}

		[DataMember]
		public string MType
		{
			get;
			set;
		}

		[DataMember]
		public string Status
		{
			get;
			set;
		}

		[DisplayName("Is Sent")]
		[DataMember]
		public string MIsSent
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
		public decimal MTaxTotalAmtFor
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
		public decimal MTotalAmtFor
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
		public string MVerifyAmt
		{
			get;
			set;
		}

		[DataMember]
		public string MVerifyAmtFor
		{
			get;
			set;
		}

		[DataMember]
		public string MItemID
		{
			get;
			set;
		}

		[DataMember]
		public string MTaxID
		{
			get;
			set;
		}

		[DisplayName("Inventory Item Code")]
		[DataMember]
		public string ItemCode
		{
			get;
			set;
		}

		[DisplayName("Description")]
		[DataMember]
		public string MDesc
		{
			get;
			set;
		}

		[DisplayName("Quantity")]
		[DataMember]
		public string MQty
		{
			get;
			set;
		}

		[DisplayName("Unit Price")]
		[DataMember]
		public string MPrice
		{
			get;
			set;
		}

		[DisplayName("Discount")]
		[DataMember]
		public string MDiscount
		{
			get;
			set;
		}

		[DataMember]
		public string TaxRate
		{
			get;
			set;
		}

		[DataMember]
		public decimal MAmountFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MAmountForExcludeTax
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTaxAmtFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTaxAmt
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
		public decimal MAmountExcludeTax
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTaxAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTaxAmountFor
		{
			get;
			set;
		}

		[DisplayName("Track Item1")]
		[DataMember]
		public string MTrackItem1
		{
			get;
			set;
		}

		[DisplayName("Track Item2")]
		[DataMember]
		public string MTrackItem2
		{
			get;
			set;
		}

		[DisplayName("Track Item3")]
		[DataMember]
		public string MTrackItem3
		{
			get;
			set;
		}

		[DisplayName("Track Item4")]
		[DataMember]
		public string MTrackItem4
		{
			get;
			set;
		}

		[DisplayName("Track Item5")]
		[DataMember]
		public string MTrackItem5
		{
			get;
			set;
		}

		public object Clone()
		{
			return base.MemberwiseClone();
		}
	}
}
