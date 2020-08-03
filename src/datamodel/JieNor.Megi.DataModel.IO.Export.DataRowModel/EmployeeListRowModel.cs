using System.ComponentModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.DataRowModel
{
	[DataContract]
	public class EmployeeListRowModel
	{
		[DisplayName("FirstName")]
		[DataMember]
		public string MFirstName
		{
			get;
			set;
		}

		[DisplayName("LastName")]
		[DataMember]
		public string MLastName
		{
			get;
			set;
		}

		[DisplayName("Email")]
		[DataMember]
		public string MEmail
		{
			get;
			set;
		}

		[DisplayName("LinkUser")]
		[DataMember]
		public string MUserID
		{
			get;
			set;
		}

		[DisplayName("Sex")]
		[DataMember]
		public string MSex
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

		[DisplayName("POAttention")]
		[DataMember]
		public string MPAttention
		{
			get;
			set;
		}

		[DisplayName("POStreetAddressOrPOBox")]
		[DataMember]
		public string MPStreet
		{
			get;
			set;
		}

		[DisplayName("POTown/City")]
		[DataMember]
		public string MPCityID
		{
			get;
			set;
		}

		[DisplayName("POState/Region")]
		[DataMember]
		public string MPRegion
		{
			get;
			set;
		}

		[DisplayName("POPostal/ZipCode")]
		[DataMember]
		public string MPPostalNo
		{
			get;
			set;
		}

		[DisplayName("POCountry")]
		[DataMember]
		public string MPCountryID
		{
			get;
			set;
		}

		[DisplayName("PhysicalAttention")]
		[DataMember]
		public string MRealAttention
		{
			get;
			set;
		}

		[DisplayName("PhysicalStreetAddressOrPOBox")]
		[DataMember]
		public string MRealStreet
		{
			get;
			set;
		}

		[DisplayName("PhysicalTown/City")]
		[DataMember]
		public string MRealCityID
		{
			get;
			set;
		}

		[DisplayName("PhysicalState/Region")]
		[DataMember]
		public string MRealRegion
		{
			get;
			set;
		}

		[DisplayName("PhysicalPostal/ZipCode")]
		[DataMember]
		public string MRealPostalNo
		{
			get;
			set;
		}

		[DisplayName("PhysicalCountry")]
		[DataMember]
		public string MRealCountryID
		{
			get;
			set;
		}

		[DisplayName("PhoneNumber")]
		[DataMember]
		public string MPhone
		{
			get;
			set;
		}

		[DisplayName("FaxNumber")]
		[DataMember]
		public string MFax
		{
			get;
			set;
		}

		[DisplayName("MobileNumber")]
		[DataMember]
		public string MMobile
		{
			get;
			set;
		}

		[DisplayName("DirectDialNumber")]
		[DataMember]
		public string MDirectPhone
		{
			get;
			set;
		}

		[DisplayName("Skype Name/Number")]
		[DataMember]
		public string MSkypeName
		{
			get;
			set;
		}

		[DisplayName("Website")]
		[DataMember]
		public string MWebsite
		{
			get;
			set;
		}

		[DisplayName("TaxIDNumber")]
		[DataMember]
		public string MTaxNo
		{
			get;
			set;
		}

		[DisplayName("SalesTax")]
		[DataMember]
		public string MSalTaxTypeID
		{
			get;
			set;
		}

		[DisplayName("PurchasesTax")]
		[DataMember]
		public string MPurTaxTypeID
		{
			get;
			set;
		}

		[DisplayName("Discount(%)")]
		[DataMember]
		public decimal? MDiscount
		{
			get;
			set;
		}

		[DisplayName("DefaultCurrency")]
		[DataMember]
		public string MDefaultCyID
		{
			get;
			set;
		}

		[DisplayName("BankAccountNumber")]
		[DataMember]
		public string MBankAcctNo
		{
			get;
			set;
		}

		[DisplayName("AccountName")]
		[DataMember]
		public string MBankAccName
		{
			get;
			set;
		}

		[DisplayName("Details")]
		[DataMember]
		public string MBankName
		{
			get;
			set;
		}

		[DisplayName("DisplayName")]
		[DataMember]
		public string MDisplayName
		{
			get;
			set;
		}

		[DisplayName("CurrentAccount")]
		[DataMember]
		public string MCurrentAccountCode
		{
			get;
			set;
		}

		[DisplayName("ExpenseAccount")]
		[DataMember]
		public string MExpenseAccountCode
		{
			get;
			set;
		}

		[DisplayName("Expense Claims default due day")]
		[DataMember]
		public string MPurDueDate
		{
			get;
			set;
		}

		[DisplayName("Expense Claims default due day condition")]
		[DataMember]
		public string MPurDueCondition
		{
			get;
			set;
		}

		[DisplayName("JoinTime")]
		[DataMember]
		public string MJoinTime
		{
			get;
			set;
		}

		[DisplayName("BaseSalary")]
		[DataMember]
		public decimal? MBaseSalary
		{
			get;
			set;
		}

		[DisplayName("IncomeTaxThreshold")]
		[DataMember]
		public decimal? MIncomeTaxThreshold
		{
			get;
			set;
		}

		[DisplayName("IncomeTaxThresholdNew")]
		[DataMember]
		public decimal? MIncomeTaxThresholdNew
		{
			get;
			set;
		}

		[DisplayName("IDType")]
		[DataMember]
		public string MIDType
		{
			get;
			set;
		}

		[DisplayName("IDNumber")]
		[DataMember]
		public string MIDNumber
		{
			get;
			set;
		}

		[DisplayName("SocialSecurityAccount")]
		[DataMember]
		public decimal? MSocialSecurityAccount
		{
			get;
			set;
		}

		[DisplayName("RetirementSecurityPercentage")]
		[DataMember]
		public decimal? MRetirementSecurityPercentage
		{
			get;
			set;
		}

		[DisplayName("RetirementSecurityAmount")]
		[DataMember]
		public decimal? MRetirementSecurityAmount
		{
			get;
			set;
		}

		[DisplayName("MedicalInsurancePercentage")]
		[DataMember]
		public decimal? MMedicalInsurancePercentage
		{
			get;
			set;
		}

		[DisplayName("MedicalInsuranceAmount")]
		[DataMember]
		public decimal? MMedicalInsuranceAmount
		{
			get;
			set;
		}

		[DisplayName("UmemploymentPercentage")]
		[DataMember]
		public decimal? MUmemploymentPercentage
		{
			get;
			set;
		}

		[DisplayName("UmemploymentAmount")]
		[DataMember]
		public decimal? MUmemploymentAmount
		{
			get;
			set;
		}

		[DisplayName("ProvidentAccount")]
		[DataMember]
		public string MProvidentAccount
		{
			get;
			set;
		}

		[DisplayName("ProvidentPercentage")]
		[DataMember]
		public decimal? MProvidentPercentage
		{
			get;
			set;
		}

		[DisplayName("ProvidentAmount")]
		[DataMember]
		public decimal? MProvidentAmount
		{
			get;
			set;
		}

		[DisplayName("ProvidentAdditionalPercentage")]
		[DataMember]
		public decimal? MProvidentAdditionalPercentage
		{
			get;
			set;
		}

		[DisplayName("ProvidentAdditionalAmount")]
		[DataMember]
		public decimal? MProvidentAdditionalAmount
		{
			get;
			set;
		}

		[DisplayName("SocialSecurityBase")]
		[DataMember]
		public decimal? MSocialSecurityBase
		{
			get;
			set;
		}

		[DisplayName("HosingProvidentFundBase")]
		[DataMember]
		public decimal? MHosingProvidentFundBase
		{
			get;
			set;
		}
	}
}
