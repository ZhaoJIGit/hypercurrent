using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.InvoiceList
{
	[DataContract]
	public class InvoiceListModel
	{
		public string ToTitle
		{
			get;
			set;
		}

		public string NumberTitle
		{
			get;
			set;
		}

		public string RefTitle
		{
			get;
			set;
		}

		public string DateTitle
		{
			get;
			set;
		}

		public string DueDateTitle
		{
			get;
			set;
		}

		public string ExpectedDateTitle
		{
			get;
			set;
		}

		public string TotalTitle
		{
			get;
			set;
		}

		public string OriginalCurrencyTotalTitle
		{
			get;
			set;
		}

		public string TaxTotalTitle
		{
			get;
			set;
		}

		public string OriginalCurrencyTaxTotalTitle
		{
			get;
			set;
		}

		public string PaidTitle
		{
			get;
			set;
		}

		public string OriginalCurrencyPaidTitle
		{
			get;
			set;
		}

		public string DueTitle
		{
			get;
			set;
		}

		public string OriginalCurrencyDueTitle
		{
			get;
			set;
		}

		public string TaxTypeTitle
		{
			get;
			set;
		}

		public string CurrencyTitle
		{
			get;
			set;
		}

		public string ExchangeRateTitle
		{
			get;
			set;
		}

		public string TypeTitle
		{
			get;
			set;
		}

		public string StatusTitle
		{
			get;
			set;
		}

		public string IsSentTitle
		{
			get;
			set;
		}

		public string ItemCodeTitle
		{
			get;
			set;
		}

		public string DescriptionTitle
		{
			get;
			set;
		}

		public string QuantityTitle
		{
			get;
			set;
		}

		public string UnitPriceTitle
		{
			get;
			set;
		}

		public string DiscountTitle
		{
			get;
			set;
		}

		public string TaxRateTitle
		{
			get;
			set;
		}

		public string AmountTitle
		{
			get;
			set;
		}

		public string OriginalCurrencyAmountTitle
		{
			get;
			set;
		}

		public string TaxAmountTitle
		{
			get;
			set;
		}

		public string OriginalCurrencyTaxAmountTitle
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
		public InvoiceListRowCollection InvoiceListRows
		{
			get;
			set;
		}
	}
}
