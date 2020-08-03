using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.DataRowModel
{
	[DataContract]
	public class VoucherListRowModel : ExportListBaseModel, ICloneable
	{
		[DisplayName("ContactName")]
		[DataMember]
		public string MContactName
		{
			get;
			set;
		}

		[DisplayName("Date")]
		[DataMember]
		public string MDate
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

		[DisplayName("Status")]
		[DataMember]
		public string MStatus
		{
			get;
			set;
		}

		[DisplayName("Creator")]
		[DataMember]
		public string MCreatorName
		{
			get;
			set;
		}

		[DisplayName("Reference")]
		[DataMember]
		public string MExplanation
		{
			get;
			set;
		}

		[DisplayName("AccountCode")]
		[DataMember]
		public string MAccountNo
		{
			get;
			set;
		}

		[DisplayName("AccountName")]
		[DataMember]
		public string MAccountNameOnly
		{
			get;
			set;
		}

		[DisplayName("AccountCodeAndName")]
		[DataMember]
		public string MAccountName
		{
			get;
			set;
		}

		[DisplayName("OriginalCurrencyAmount")]
		[DataMember]
		public decimal? MAmountFor
		{
			get;
			set;
		}

		[DisplayName("Currency")]
		[DataMember]
		public string MCurrencyID
		{
			get;
			set;
		}

		[DisplayName("CurrencyName")]
		[DataMember]
		public string MCurrencyName
		{
			get;
			set;
		}

		[DisplayName("ExchangeRate")]
		[DataMember]
		public decimal? MExchangeRate
		{
			get;
			set;
		}

		[DisplayName("ExchangeRateString")]
		[DataMember]
		public string MExchangeRateString
		{
			get;
			set;
		}

		[DataMember]
		public int MDC
		{
			get;
			set;
		}

		[DisplayName("Debit")]
		[DataMember]
		public decimal? MDebit
		{
			get;
			set;
		}

		[DisplayName("Credit")]
		[DataMember]
		public decimal? MCredit
		{
			get;
			set;
		}

		[DataMember]
		public string AccountingDimension
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
