using System.ComponentModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.EmployeeList
{
	[DataContract]
	public class EmployeeListModel
	{
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
		public string LinkUserTitle
		{
			get;
			set;
		}

		[DataMember]
		public string SexTitle
		{
			get;
			set;
		}

		[DataMember]
		public string StatusTitle
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
		public string ExpenseDueDayTitle
		{
			get;
			set;
		}

		[DataMember]
		public string ExpenseDueDayConditionTitle
		{
			get;
			set;
		}

		[DataMember]
		public string JoinTimeTitle
		{
			get;
			set;
		}

		[DataMember]
		public string BaseSalaryTitle
		{
			get;
			set;
		}

		[DataMember]
		public string PITThresholdTitle
		{
			get;
			set;
		}

		[DataMember]
		public string PITThresholdNewTitle
		{
			get;
			set;
		}

		[DataMember]
		public string IDTypeTitle
		{
			get;
			set;
		}

		[DataMember]
		public string IDNumberTitle
		{
			get;
			set;
		}

		[DataMember]
		public string SocialSecurityBaseTitle
		{
			get;
			set;
		}

		[DataMember]
		public string PersonalAccountNumberTitle
		{
			get;
			set;
		}

		[DataMember]
		public string RetirementSecurityPercentTitle
		{
			get;
			set;
		}

		[DataMember]
		public string MedicalInsurancePercentTitle
		{
			get;
			set;
		}

		[DataMember]
		public string UmemploymentInsurancePercentTitle
		{
			get;
			set;
		}

		[DataMember]
		public string RetirementSecurityAmountTitle
		{
			get;
			set;
		}

		[DataMember]
		public string MedicalInsuranceAmountTitle
		{
			get;
			set;
		}

		[DataMember]
		public string UmemploymentInsuranceAmountTitle
		{
			get;
			set;
		}

		[DataMember]
		public string HosingProvidentFundBaseTitle
		{
			get;
			set;
		}

		[DataMember]
		public string HousingProvidentFundPercentTitle
		{
			get;
			set;
		}

		[DataMember]
		public string HousingProvidentFundAmountTitle
		{
			get;
			set;
		}

		[DataMember]
		public string HousingProvidentFundAdditionalPercentTitle
		{
			get;
			set;
		}

		[DataMember]
		public string HousingProvidentFundAdditionalAmountTitle
		{
			get;
			set;
		}

		[DataMember]
		public string CurrentAccountTitle
		{
			get;
			set;
		}

		[DataMember]
		public string ExpenseAccountTitle
		{
			get;
			set;
		}

		[DisplayName("Employee List Rows")]
		[DataMember]
		public EmployeeListRowCollection EmployeeListRows
		{
			get;
			set;
		}
	}
}
