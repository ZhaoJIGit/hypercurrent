using System.ComponentModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.ContactList
{
	[DataContract]
	public class ContactListModel
	{
		[DataMember]
		public string ContactNameTitle
		{
			get;
			set;
		}

		[DataMember]
		public string ContactTypeTitle
		{
			get;
			set;
		}

		[DataMember]
		public string FirstNameTitle
		{
			get;
			set;
		}

		[DataMember]
		public string LastNameTitle
		{
			get;
			set;
		}

		[DataMember]
		public string EmailTitle
		{
			get;
			set;
		}

		[DataMember]
		public string POAttentionTitle
		{
			get;
			set;
		}

		[DataMember]
		public string POAddressTitle
		{
			get;
			set;
		}

		[DataMember]
		public string POCityTitle
		{
			get;
			set;
		}

		[DataMember]
		public string POProvinceTitle
		{
			get;
			set;
		}

		[DataMember]
		public string POPostalCodeTitle
		{
			get;
			set;
		}

		[DataMember]
		public string POCountryTitle
		{
			get;
			set;
		}

		[DataMember]
		public string RealAttentionTitle
		{
			get;
			set;
		}

		[DataMember]
		public string RealAddressTitle
		{
			get;
			set;
		}

		[DataMember]
		public string RealCityTitle
		{
			get;
			set;
		}

		[DataMember]
		public string RealProvinceTitle
		{
			get;
			set;
		}

		[DataMember]
		public string RealPostalCodeTitle
		{
			get;
			set;
		}

		[DataMember]
		public string RealCountryTitle
		{
			get;
			set;
		}

		[DataMember]
		public string TelephoneTitle
		{
			get;
			set;
		}

		[DataMember]
		public string FaxTitle
		{
			get;
			set;
		}

		[DataMember]
		public string MobileTitle
		{
			get;
			set;
		}

		[DataMember]
		public string DirectDialTitle
		{
			get;
			set;
		}

		[DataMember]
		public string SkypeTitle
		{
			get;
			set;
		}

		[DataMember]
		public string WebsiteTitle
		{
			get;
			set;
		}

		[DataMember]
		public string TaxIDNumberTitle
		{
			get;
			set;
		}

		[DataMember]
		public string SalesTaxTitle
		{
			get;
			set;
		}

		[DataMember]
		public string PurchasesTaxTitle
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
		public string DefaultCurrencyTitle
		{
			get;
			set;
		}

		[DataMember]
		public string BankAccountNumberTitle
		{
			get;
			set;
		}

		[DataMember]
		public string AccountNameTitle
		{
			get;
			set;
		}

		[DataMember]
		public string DetailsTitle
		{
			get;
			set;
		}

		[DataMember]
		public string DueTitle
		{
			get;
			set;
		}

		[DataMember]
		public string BillsDefaultDueDateTitle
		{
			get;
			set;
		}

		[DataMember]
		public string SalesInvDefDueDateTitle
		{
			get;
			set;
		}

		[DataMember]
		public string SaleDueAmtTitle
		{
			get;
			set;
		}

		[DataMember]
		public string SaleOverDueAmtTitle
		{
			get;
			set;
		}

		[DataMember]
		public string BillDueAmtTitle
		{
			get;
			set;
		}

		[DataMember]
		public string BillOverDueAmtTitle
		{
			get;
			set;
		}

		[DataMember]
		public string CustomerCurrentAccountTitle
		{
			get;
			set;
		}

		[DataMember]
		public string SupplierCurrentAccountTitle
		{
			get;
			set;
		}

		[DataMember]
		public string SaleIncomeAccountTitle
		{
			get;
			set;
		}

		[DisplayName("Track Item1 Name")]
		[DataMember]
		public string TrackItem1Name
		{
			get;
			set;
		}

		[DisplayName("Track Item2 Name")]
		[DataMember]
		public string TrackItem2Name
		{
			get;
			set;
		}

		[DisplayName("Track Item3 Name")]
		[DataMember]
		public string TrackItem3Name
		{
			get;
			set;
		}

		[DisplayName("Track Item4 Name")]
		[DataMember]
		public string TrackItem4Name
		{
			get;
			set;
		}

		[DisplayName("Track Item5 Name")]
		[DataMember]
		public string TrackItem5Name
		{
			get;
			set;
		}

		[DisplayName("Contact List Rows")]
		[DataMember]
		public ContactListRowCollection ContactListRows
		{
			get;
			set;
		}
	}
}
