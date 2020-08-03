using JieNor.Megi.Core.DataModel;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDPrintSettingModel : BDModel
	{
		[DataMember]
		public string MMeasureIn
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTopMargin
		{
			get;
			set;
		}

		[DataMember]
		public string MTopMarginWithUnit
		{
			get;
			set;
		}

		[DataMember]
		public decimal MBottomMargin
		{
			get;
			set;
		}

		[DataMember]
		public string MBottomMarginWithUnit
		{
			get;
			set;
		}

		[DataMember]
		public decimal MAddressPadding
		{
			get;
			set;
		}

		[DataMember]
		public string MAddressPaddingWithUnit
		{
			get;
			set;
		}

		[DataMember]
		public bool MShowTaxNumber
		{
			get;
			set;
		}

		[DataMember]
		public bool MShowHeading
		{
			get;
			set;
		}

		[DataMember]
		public bool MShowUnitPriceAndQuantity
		{
			get;
			set;
		}

		[DataMember]
		public bool MShowTaxColumn
		{
			get;
			set;
		}

		[DataMember]
		public bool MShowRegAddress
		{
			get;
			set;
		}

		[DataMember]
		public string MLogoID
		{
			get;
			set;
		}

		[DataMember]
		public bool MShowLogo
		{
			get;
			set;
		}

		[DataMember]
		public bool MShowTracking
		{
			get;
			set;
		}

		[DataMember]
		public bool MHideDiscount
		{
			get;
			set;
		}

		[DataMember]
		public string MShowTaxSubTotalWay
		{
			get;
			set;
		}

		[DataMember]
		public string MShowCurrencyConversionWay
		{
			get;
			set;
		}

		[DisplayName("PayeeAccount")]
		[DataMember]
		public string MPayService
		{
			get;
			set;
		}

		[DataMember]
		public string MTermsAndPayAdvice
		{
			get;
			set;
		}

		[DataMember]
		public string MLogoAlignment
		{
			get;
			set;
		}

		[DataMember]
		public string MShowTaxType
		{
			get;
			set;
		}

		[DataMember]
		public string MContactDetails
		{
			get;
			set;
		}

		[DataMember]
		public int MSeq
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsSys
		{
			get;
			set;
		}

		[DataMember]
		public string MName
		{
			get;
			set;
		}

		[DataMember]
		public string MDraftInvoiceTitle
		{
			get;
			set;
		}

		[DataMember]
		public string MApprovedInvoiceTitle
		{
			get;
			set;
		}

		[DataMember]
		public string MOverdueInvoiceTitle
		{
			get;
			set;
		}

		[DataMember]
		public string MCreditNoteTitle
		{
			get;
			set;
		}

		[DataMember]
		public string MStatementTitle
		{
			get;
			set;
		}

		[DataMember]
		public string MPaymentTitle
		{
			get;
			set;
		}

		[DataMember]
		public string MReceiptTitle
		{
			get;
			set;
		}

		[DataMember]
		public string Headings
		{
			get;
			set;
		}

		public BDPrintSettingModel()
			: base("T_BD_PrintSetting")
		{
		}

		public BDPrintSettingModel(string tableName)
			: base(tableName)
		{
		}
	}
}
