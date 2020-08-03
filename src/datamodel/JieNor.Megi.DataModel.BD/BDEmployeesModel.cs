using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.Const;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.EntityModel.Enum;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDEmployeesModel : BDModel
	{
		[DataMember]
		public int __active__ = 1;

		[DataMember]
		[ApiMember("EmployeeID", IsPKField = true, IgnoreLengthValidate = true)]
		public string MEmployeeID
		{
			get
			{
				return base.MItemID;
			}
			set
			{
				base.MItemID = value;
			}
		}

		[DataMember]
		public string MDeptIDS
		{
			get;
			set;
		}

		[DataMember]
		public string MFullName
		{
			get;
			set;
		}

		[DataMember]
		public string MAccountID
		{
			get;
			set;
		}

		[DataMember]
		[DBFieldValidate(IgnoreLengthValidate = true)]
		public string MUserID
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("JobStatus", IgnoreLengthValidate = true)]
		[ApiEnum(EnumMappingType.EmployeeStatus, IsRequired = false)]
		public string MStatus
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("FirstName", ApiMemberType.MultiLang, false, false)]
		public string MFirstName
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("LastName", ApiMemberType.MultiLang, false, false)]
		public string MLastName
		{
			get;
			set;
		}

		[DataMember]
		[Email("Email Address", OperateTime.Save)]
		[ApiMember("Email")]
		public string MEmail
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("Gender", IgnoreLengthValidate = true)]
		[ApiEnum(EnumMappingType.Gender, IsRequired = false)]
		public string MSex
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("UserEmailAddress")]
		public string MUserEmailAddress
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("CurrentAccountCode", IgnoreLengthValidate = true)]
		[DBFieldValidate(IgnoreLengthValidate = true)]
		public string MCurrentAccountCode
		{
			get;
			set;
		}

		[DataMember]
		public string MCurrentAccountId
		{
			get;
			set;
		}

		[DataMember]
		public string MExpenseAccountCode
		{
			get;
			set;
		}

		[DataMember]
		public string MExpenseAccountId
		{
			get;
			set;
		}

		[DataMember]
		[DBFieldValidate(IgnoreLengthValidate = true)]
		public string MPCountryID
		{
			get;
			set;
		}

		[DataMember]
		[DBFieldValidate(IgnoreLengthValidate = true)]
		public string MPCityID
		{
			get;
			set;
		}

		[DataMember]
		public string MPPostalNo
		{
			get;
			set;
		}

		[DataMember]
		[DBFieldValidate(IgnoreLengthValidate = true)]
		public string MRealCountryID
		{
			get;
			set;
		}

		[DataMember]
		[DBFieldValidate(IgnoreLengthValidate = true)]
		public string MRealCityID
		{
			get;
			set;
		}

		[DataMember]
		public string MRealPostalNo
		{
			get;
			set;
		}

		[DataMember]
		public string MPhone
		{
			get;
			set;
		}

		[DataMember]
		public string MFax
		{
			get;
			set;
		}

		[DataMember]
		public string MMobile
		{
			get;
			set;
		}

		[DataMember]
		public string MDirectPhone
		{
			get;
			set;
		}

		[DataMember]
		public string MQQNo
		{
			get;
			set;
		}

		[DataMember]
		public string MWeChatNo
		{
			get;
			set;
		}

		[DataMember]
		public string MSkypeName
		{
			get;
			set;
		}

		[DataMember]
		public string MWebsite
		{
			get;
			set;
		}

		[DataMember]
		[Email("Email Address", OperateTime.Save)]
		public string MOurEmail
		{
			get;
			set;
		}

		[DataMember]
		[ApiPrecision(2)]
		public decimal MDiscount
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("DefaultCurrencyCode", IgnoreLengthValidate = true)]
		[DBFieldValidate(IgnoreLengthValidate = true)]
		public string MDefaultCyID
		{
			get;
			set;
		}

		[DataMember]
		[ColumnEncrypt]
		[ApiMember("BankAccountNumber", MaxLength = 36)]
		public string MBankAcctNo
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("BankAccountName", ApiMemberType.MultiLang, false, false)]
		public string MBankAccName
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("BankName", ApiMemberType.MultiLang, false, false)]
		public string MBankName
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("PaymentTerms", true, ErrorType = ValidationErrorType.ContactBillsPaymentTerms)]
		public BDEmployeesPaymentTermModel MPaymentTerms
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("Status", IgnoreInPost = true)]
		[ApiEnum(EnumMappingType.CommonStatus)]
		public bool MIsActive
		{
			get
			{
				return __active__ == 1;
			}
			set
			{
				__active__ = (value ? 1 : (-1));
			}
		}

		[DataMember]
		public string MTaxNo
		{
			get;
			set;
		}

		[DataMember]
		public string MSalTaxTypeID
		{
			get;
			set;
		}

		[DataMember]
		public string MPurTaxTypeID
		{
			get;
			set;
		}

		[DataMember]
		public string MDefaultSaleTaxID
		{
			get;
			set;
		}

		[DataMember]
		public string MDefaultPurchaseTaxID
		{
			get;
			set;
		}

		[DataMember]
		public int MPurDueDate
		{
			get;
			set;
		}

		[DataMember]
		public int MSalDueDate
		{
			get;
			set;
		}

		[DataMember]
		public string MPurDueCondition
		{
			get;
			set;
		}

		[DataMember]
		public string MSalDueCondition
		{
			get;
			set;
		}

		[DataMember]
		public string MRecAcctID
		{
			get;
			set;
		}

		[DataMember]
		public string MPayAcctID
		{
			get;
			set;
		}

		[DataMember]
		public string MBorrowAcctID
		{
			get;
			set;
		}

		[DataMember]
		public string MDefaultCurrencyID
		{
			get;
			set;
		}

		[DataMember]
		public string Name
		{
			get;
			set;
		}

		[DataMember]
		public string Sex
		{
			get;
			set;
		}

		[DataMember]
		public string PAttention
		{
			get;
			set;
		}

		[DataMember]
		public string PStreet
		{
			get;
			set;
		}

		[DataMember]
		public string PRegion
		{
			get;
			set;
		}

		[DataMember]
		public string RealAttention
		{
			get;
			set;
		}

		[DataMember]
		public string RealStreet
		{
			get;
			set;
		}

		[DataMember]
		public string RealRegion
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

		[DataMember]
		public DateTime MJoinTime
		{
			get;
			set;
		}

		[DataMember]
		public decimal MBaseSalary
		{
			get;
			set;
		}

		[DataMember]
		public decimal? MIncomeTaxThreshold
		{
			get;
			set;
		}

		[DataMember]
		public decimal? MIncomeTaxThresholdNew
		{
			get;
			set;
		}

		[DataMember]
		[DBFieldValidate(IgnoreLengthValidate = true)]
		public string MIDType
		{
			get;
			set;
		}

		[DataMember]
		public string MIDNumber
		{
			get;
			set;
		}

		[DataMember]
		public string MSocialSecurityAccount
		{
			get;
			set;
		}

		[DataMember]
		public decimal? MRetirementSecurityPercentage
		{
			get;
			set;
		}

		[DataMember]
		public decimal MRetirementSecurityAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal? MMedicalInsurancePercentage
		{
			get;
			set;
		}

		[DataMember]
		public decimal MMedicalInsuranceAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal? MUmemploymentPercentage
		{
			get;
			set;
		}

		[DataMember]
		public decimal MUmemploymentAmount
		{
			get;
			set;
		}

		[DataMember]
		public string MProvidentAccount
		{
			get;
			set;
		}

		[DataMember]
		public decimal? MProvidentPercentage
		{
			get;
			set;
		}

		[DataMember]
		public decimal MProvidentAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal? MProvidentAdditionalPercentage
		{
			get;
			set;
		}

		[DataMember]
		public decimal MProvidentAdditionalAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MSocialSecurityBase
		{
			get;
			set;
		}

		[DataMember]
		public decimal MHosingProvidentFundBase
		{
			get;
			set;
		}

		[DataMember]
		public BDPayrollDetailModel PayrollDetail
		{
			get;
			set;
		}

		[DataMember]
		public string MNameLetter
		{
			get;
			set;
		}

		[DataMember]
		public string MNameFirstLetter
		{
			get;
			set;
		}

		[DataMember]
		public string MPCountryName
		{
			get;
			set;
		}

		[DataMember]
		public string MRealCountryName
		{
			get;
			set;
		}

		public BDEmployeesModel()
			: base("T_BD_Employees")
		{
		}
	}
}
