using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.DataRowModel
{
	[DataContract]
	public class TransactionListRowModel : BaseReportRowModel, ICloneable
	{
		[DataMember]
		public string AccountName
		{
			get;
			set;
		}

		[DisplayName("ContactType")]
		[DataMember]
		public string MContactType
		{
			get;
			set;
		}

		[DisplayName("Contact")]
		[DataMember]
		public string MContactID
		{
			get;
			set;
		}

		[DisplayName("TransactionType")]
		[DataMember]
		public string MType
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

		[DisplayName("Date")]
		[DataMember]
		public string MBizDate
		{
			get;
			set;
		}

		[DisplayName("TransactionDescription")]
		[DataMember]
		public string MDescription
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

		[DataMember]
		public string TaxType
		{
			get;
			set;
		}

		[DisplayName("Currency")]
		[DataMember]
		public string MCyID
		{
			get;
			set;
		}

		[DisplayName("ExchangeRate")]
		[DataMember]
		public string MExchangeRate
		{
			get;
			set;
		}

		[DisplayName("Spent")]
		[DataMember]
		public decimal? MSpent
		{
			get;
			set;
		}

		[DisplayName("Received")]
		[DataMember]
		public decimal? MReceived
		{
			get;
			set;
		}

		[DisplayName("Status")]
		[DataMember]
		public string MStatus
		{
			get;
			set;
		}

		[DataMember]
		public string MTotalAmtFor
		{
			get;
			set;
		}

		[DisplayName("Total")]
		[DataMember]
		public string MTotalAmt
		{
			get;
			set;
		}

		[DataMember]
		public string MTaxTotalAmtFor
		{
			get;
			set;
		}

		[DataMember]
		public string MTaxTotalAmt
		{
			get;
			set;
		}

		[DataMember]
		public string MVerificationAmt
		{
			get;
			set;
		}

		[DataMember]
		public string MVerificationAmtBaseCurrency
		{
			get;
			set;
		}

		[DataMember]
		public decimal MVerifyAmt
		{
			get;
			set;
		}

		[DataMember]
		public decimal MVerifyAmtFor
		{
			get;
			set;
		}

		[DataMember]
		public string MReconcileAmt
		{
			get;
			set;
		}

		[DataMember]
		public string MReconcileAmtFor
		{
			get;
			set;
		}

		[DataMember]
		public string MID
		{
			get;
			set;
		}

		[DisplayName("IsAdvances")]
		[DataMember]
		public string MIsAdvances
		{
			get;
			set;
		}

		[DisplayName("Department")]
		[DataMember]
		public string MDepartment
		{
			get;
			set;
		}

		[DisplayName("Item")]
		[DataMember]
		public string MItemID
		{
			get;
			set;
		}

		[DisplayName("ItemDescription")]
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

		[DisplayName("UnitPrice")]
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

		[DisplayName("TaxRate")]
		[DataMember]
		public string MTaxID
		{
			get;
			set;
		}

		[DisplayName("Amount")]
		[DataMember]
		public decimal MAmountFor
		{
			get;
			set;
		}

		[DataMember]
		public string MAmount
		{
			get;
			set;
		}

		[DataMember]
		public string MTaxAmountFor
		{
			get;
			set;
		}

		[DataMember]
		public string MTaxAmount
		{
			get;
			set;
		}

		[DisplayName("TaxAmount")]
		[DataMember]
		public decimal MTaxAmtFor
		{
			get;
			set;
		}

		[DataMember]
		public string MTaxAmt
		{
			get;
			set;
		}

		[DisplayName("TrackItem1")]
		[DataMember]
		public string MTrackItem1
		{
			get;
			set;
		}

		[DisplayName("TrackItem2")]
		[DataMember]
		public string MTrackItem2
		{
			get;
			set;
		}

		[DisplayName("TrackItem3")]
		[DataMember]
		public string MTrackItem3
		{
			get;
			set;
		}

		[DisplayName("TrackItem4")]
		[DataMember]
		public string MTrackItem4
		{
			get;
			set;
		}

		[DisplayName("TrackItem5")]
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
