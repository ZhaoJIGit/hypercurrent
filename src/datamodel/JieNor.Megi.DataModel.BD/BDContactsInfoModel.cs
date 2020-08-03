using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.EntityModel.Enum;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDContactsInfoModel : BDModel
	{
		[DataMember]
		public int __active__ = 1;

		[DataMember]
		[ApiMember("ContactID", IsPKField = true, IgnoreLengthValidate = true)]
		public string MContactID
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
		[ApiMember("Name", ApiMemberType.MultiLang, true, false, MaxLength = 200)]
		[ColumnEncrypt]
		public string MName
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("IsSupplier", IgnoreInSubModel = true)]
		public bool MIsSupplier
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("IsCustomer", IgnoreInSubModel = true)]
		public bool MIsCustomer
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("IsOther", IgnoreInSubModel = true)]
		public bool MIsOther
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("FirstName", ApiMemberType.MultiLang, false, false, IgnoreInSubModel = true)]
		public string MFirstName
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("LastName", ApiMemberType.MultiLang, false, false, IgnoreInSubModel = true)]
		public string MLastName
		{
			get;
			set;
		}

		[DataMember]
		[ColumnEncrypt]
		[Email("Email Address", OperateTime.Save)]
		[ApiMember("Email", MaxLength = 256, IgnoreInSubModel = true)]
		[EmailValidation]
		public string MEmail
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("LAddressee", ApiMemberType.MultiLang, false, false, IgnoreInSubModel = true)]
		public string MRealAttention
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("LStreet", ApiMemberType.MultiLang, false, false, IgnoreInSubModel = true)]
		public string MRealStreet
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("LCity", ApiMemberType.MultiLang, false, false, IgnoreInSubModel = true, MaxLength = 100)]
		[DBFieldValidate(IgnoreLengthValidate = true)]
		public string MRealCityID
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("LRegion", ApiMemberType.MultiLang, false, false, IgnoreInSubModel = true)]
		public string MRealRegion
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("LPostCode", IgnoreInSubModel = true)]
		public string MRealPostalNo
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("LCountry", IgnoreInSubModel = true, IgnoreLengthValidate = true)]
		[ApiEnum(EnumMappingType.Country, IsRequired = false)]
		[DBFieldValidate(IgnoreLengthValidate = true)]
		public string MRealCountryID
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("PAddressee", ApiMemberType.MultiLang, false, false, IgnoreInSubModel = true)]
		public string MPAttention
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("PStreet", ApiMemberType.MultiLang, false, false, IgnoreInSubModel = true)]
		public string MPStreet
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("PCity", ApiMemberType.MultiLang, false, false, IgnoreInSubModel = true, MaxLength = 100)]
		[DBFieldValidate(IgnoreLengthValidate = true)]
		public string MPCityID
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("PRegion", ApiMemberType.MultiLang, false, false, IgnoreInSubModel = true)]
		public string MPRegion
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("PPostCode", IgnoreInSubModel = true)]
		public string MPPostalNo
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("PCountry", IgnoreInSubModel = true, IgnoreLengthValidate = true)]
		[ApiEnum(EnumMappingType.Country, IsRequired = false)]
		[DBFieldValidate(IgnoreLengthValidate = true)]
		public string MPCountryID
		{
			get;
			set;
		}

		[DataMember]
		[ColumnEncrypt]
		[ApiMember("Phone", MaxLength = 36, IgnoreInSubModel = true)]
		public string MPhone
		{
			get;
			set;
		}

		[DataMember]
		[ColumnEncrypt]
		[ApiMember("Fax", MaxLength = 36, IgnoreInSubModel = true)]
		public string MFax
		{
			get;
			set;
		}

		[DataMember]
		[ColumnEncrypt]
		[ApiMember("Mobile", MaxLength = 36, IgnoreInSubModel = true)]
		public string MMobile
		{
			get;
			set;
		}

		[DataMember]
		[ColumnEncrypt]
		[ApiMember("DirectDial", MaxLength = 36, IgnoreInSubModel = true)]
		public string MDirectPhone
		{
			get;
			set;
		}

		[DataMember]
		[ColumnEncrypt]
		[ApiMember("Skype", MaxLength = 100, IgnoreInSubModel = true)]
		public string MSkypeName
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
		[Email("Email Address", OperateTime.Save)]
		public string MOurEmail
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("TaxNumber", IgnoreInSubModel = true)]
		public string MTaxNo
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("AccountsReceivableTaxRate", true, IgnoreInSubModel = true)]
		public REGTaxRateSimpleModel MReceivableTaxRate
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("AccountsPayableTaxRate", true, IgnoreInSubModel = true)]
		public REGTaxRateSimpleModel MPayableTaxRate
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("DefaultCurrencyCode", IgnoreLengthValidate = true, IgnoreInSubModel = true)]
		[DBFieldValidate(IgnoreLengthValidate = true)]
		public string MDefaultCyID
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
		[ColumnEncrypt]
		[ApiMember("BankAccountNumber", MaxLength = 36, IgnoreInSubModel = true)]
		public string MBankAcctNo
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("BankAccountName", ApiMemberType.MultiLang, false, false, IgnoreInSubModel = true)]
		public string MBankAccName
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("BankName", ApiMemberType.MultiLang, false, false, IgnoreInSubModel = true)]
		public string MBankName
		{
			get;
			set;
		}

		[DataMember]
		[ApiDetail]
		[ApiMember("Balances", IgnoreInSubModel = true)]
		public BDContactsBalanceModel MBalances
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("CurrentAccountCode", IgnoreInSubModel = true, IgnoreLengthValidate = true)]
		[DBFieldValidate(IgnoreLengthValidate = true)]
		public string MCCurrentAccountCode
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("SalesTrackingCategories", ApiMemberType.ObjectList, false, true, IgnoreInSubModel = true)]
		public List<BDTrackSimpleModel> MSalesTrackingCategories
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("PurchasesTrackingCategories", ApiMemberType.ObjectList, false, true, IgnoreInSubModel = true)]
		public List<BDTrackSimpleModel> MPurchasesTrackingCategories
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("ContactGroups", ApiMemberType.ObjectList, false, true, IgnoreInSubModel = true)]
		public List<BDContactsTypeSimpleModel> MContactGroups
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("Discount", IgnoreInSubModel = true)]
		[ApiPrecision(2)]
		public decimal MDiscount
		{
			get;
			set;
		}

		[DataMember]
		[ColumnEncrypt]
		[Http("Website", OperateTime.Save)]
		[ApiMember("Website", MaxLength = 256, IgnoreInSubModel = true)]
		public string MWebsite
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("PaymentTerms", ApiMemberType.Object, false, true, IgnoreInSubModel = true)]
		public BDContactsPaymentTermModel MPaymentTerms
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("Status", IgnoreInPost = true, IgnoreInSubModel = true)]
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
		[ApiMember("SalTaxTypeID", IsReference = true, IgnoreInGet = true, IgnoreInSubModel = true)]
		[DBFieldValidate(IgnoreLengthValidate = true)]
		public string MSalTaxTypeID
		{
			get;
			set;
		}

		[DataMember]
		public decimal MSalTaxRate
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("PurTaxTypeID", IsReference = true, IgnoreInGet = true, IgnoreInSubModel = true)]
		[DBFieldValidate(IgnoreLengthValidate = true)]
		public string MPurTaxTypeID
		{
			get;
			set;
		}

		[DataMember]
		public decimal MPurTaxRate
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
		public decimal MBaseSalary
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
		public bool MIsValidateName
		{
			get;
			set;
		}

		[DataMember]
		public string MTypeID
		{
			get;
			set;
		}

		[DataMember]
		public string MCCurrentAccountId
		{
			get;
			set;
		}

		public override string[] MMultiLangEncryptColumns
		{
			get
			{
				return new string[1]
				{
					"MName"
				};
			}
		}

		[DataMember]
		public string MContactName
		{
			get;
			set;
		}

		[DataMember]
		public string MDesc
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
		public string MOurFirstName
		{
			get;
			set;
		}

		[DataMember]
		public string MOurLastName
		{
			get;
			set;
		}

		[DataMember]
		public string MLocaleID
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackHead1
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackHead2
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackHead3
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackHead4
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackHead5
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackHead6
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackHead7
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackHead8
		{
			get;
			set;
		}

		[DataMember]
		public string MSalTrackEntry1
		{
			get;
			set;
		}

		[DataMember]
		public string MSalTrackEntry2
		{
			get;
			set;
		}

		[DataMember]
		public string MSalTrackEntry3
		{
			get;
			set;
		}

		[DataMember]
		public string MSalTrackEntry4
		{
			get;
			set;
		}

		[DataMember]
		public string MSalTrackEntry5
		{
			get;
			set;
		}

		[DataMember]
		public string MSalTrackEntry6
		{
			get;
			set;
		}

		[DataMember]
		public string MSalTrackEntry7
		{
			get;
			set;
		}

		[DataMember]
		public string MSalTrackEntry8
		{
			get;
			set;
		}

		[DataMember]
		public string MPurTrackEntry1
		{
			get;
			set;
		}

		[DataMember]
		public string MPurTrackEntry2
		{
			get;
			set;
		}

		[DataMember]
		public string MPurTrackEntry3
		{
			get;
			set;
		}

		[DataMember]
		public string MPurTrackEntry4
		{
			get;
			set;
		}

		[DataMember]
		public string MPurTrackEntry5
		{
			get;
			set;
		}

		[DataMember]
		public string MPurTrackEntry6
		{
			get;
			set;
		}

		[DataMember]
		public string MPurTrackEntry7
		{
			get;
			set;
		}

		[DataMember]
		public string MPurTrackEntry8
		{
			get;
			set;
		}

		[DataMember]
		public string MSaleDueAmt
		{
			get;
			set;
		}

		[DataMember]
		public string MSaleOverDueAmt
		{
			get;
			set;
		}

		[DataMember]
		public string MBillDueAmt
		{
			get;
			set;
		}

		[DataMember]
		public string MBillOverDueAmt
		{
			get;
			set;
		}

		[DataMember]
		public string MPostalAddressInfo
		{
			get;
			set;
		}

		[DataMember]
		public string MPhysicalAddressInfo
		{
			get;
			set;
		}

		[DataMember]
		public string MContactInfo
		{
			get;
			set;
		}

		[DataMember]
		public bool IsQuote
		{
			get;
			set;
		}

		[DataMember]
		public string AccountFullName
		{
			get;
			set;
		}

		[DataMember]
		public string MEntryID
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("ContactGroupID", IgnoreInGet = true, IgnoreInSubModel = true)]
		public string MContactGroupID
		{
			get;
			set;
		}

		public string MSourceBizObject
		{
			get;
			set;
		}

		public bool IsIgnore
		{
			get;
			set;
		}

		public bool IsPerfectMatchName
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

		[DataMember]
		public bool IsHaveNameOrIdError
		{
			get;
			set;
		}

		public BDContactsInfoModel()
			: base("T_BD_Contacts")
		{
		}

		public void SetPaymentTerms()
		{
			if (!string.IsNullOrWhiteSpace(MPurDueCondition))
			{
				MPaymentTerms = (MPaymentTerms ?? new BDContactsPaymentTermModel());
				MPaymentTerms.MBills = new BDContactsBillsPaymentTermModel
				{
					MDay = MPurDueDate,
					MDayType = MPurDueCondition
				};
			}
			if (!string.IsNullOrWhiteSpace(MSalDueCondition))
			{
				MPaymentTerms = (MPaymentTerms ?? new BDContactsPaymentTermModel());
				MPaymentTerms.MSales = new BDContactsSalesPaymentTermModel
				{
					MDay = MSalDueDate,
					MDayType = MSalDueCondition
				};
			}
		}

		public void GetPaymentTerms()
		{
			if (MPaymentTerms != null && MPaymentTerms.MBills != null)
			{
				MPurDueDate = MPaymentTerms.MBills.MDay;
				MPurDueCondition = MPaymentTerms.MBills.MDayType;
			}
			if (MPaymentTerms != null && MPaymentTerms.MSales != null)
			{
				MSalDueDate = MPaymentTerms.MSales.MDay;
				MSalDueCondition = MPaymentTerms.MSales.MDayType;
			}
		}

		public void AddSalesTracking(object obj)
		{
			BDTrackSimpleModel item = obj as BDTrackSimpleModel;
			if (obj != null)
			{
				if (MSalesTrackingCategories == null)
				{
					MSalesTrackingCategories = new List<BDTrackSimpleModel>();
				}
				MSalesTrackingCategories.Add(item);
			}
		}

		public void AddPurchasesTracking(object obj)
		{
			BDTrackSimpleModel item = obj as BDTrackSimpleModel;
			if (obj != null)
			{
				if (MPurchasesTrackingCategories == null)
				{
					MPurchasesTrackingCategories = new List<BDTrackSimpleModel>();
				}
				MPurchasesTrackingCategories.Add(item);
			}
		}

		public void AddContactGroups(object obj)
		{
			BDContactsTypeSimpleModel item = obj as BDContactsTypeSimpleModel;
			if (obj != null)
			{
				if (MContactGroups == null)
				{
					MContactGroups = new List<BDContactsTypeSimpleModel>();
				}
				MContactGroups.Add(item);
			}
		}
	}
}
