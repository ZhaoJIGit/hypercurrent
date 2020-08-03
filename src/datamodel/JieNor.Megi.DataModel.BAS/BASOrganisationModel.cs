using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BAS
{
	[DataContract]
	public class BASOrganisationModel : BDModel
	{
		[DataMember(Order = 5)]
		[ApiMember("OrganizationID", IsPKField = true)]
		public string MOrganizationID
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
		public string MRegionID
		{
			get;
			set;
		}

		[DataMember(Order = 10)]
		[ApiMember("OrganizationName")]
		public string MName
		{
			get;
			set;
		}

		[DataMember]
		public string MMasterID
		{
			get;
			set;
		}

		[DataMember(Order = 15)]
		[ApiMember("LegalName")]
		public string MLegalTradingName
		{
			get;
			set;
		}

		[DataMember(Order = 20)]
		[ApiMember("Version")]
		[ApiEnum(EnumMappingType.OrganizationVersion)]
		public int MVersionID
		{
			get;
			set;
		}

		[DataMember(Order = 25)]
		[ApiMember("OrganizationType")]
		[ApiEnum(EnumMappingType.OrganizationType)]
		public string MOrgTypeID
		{
			get;
			set;
		}

		[DataMember(Order = 30)]
		[ApiMember("OrganizationIndustry")]
		[ApiEnum(EnumMappingType.OrganizationIndustry)]
		public string MOrgBusiness
		{
			get;
			set;
		}

		[DataMember(Order = 35)]
		[ApiMember("BaseCurrencyCode")]
		public string MCurrencyID
		{
			get;
			set;
		}

		[DataMember(Order = 40)]
		[ApiMember("AccountingStandard")]
		[ApiEnum(EnumMappingType.AccountingStandards)]
		public string MAccountingStandard
		{
			get;
			set;
		}

		[DataMember(Order = 45)]
		[ApiMember("ConversionPeriod")]
		public string MConversionPeriod
		{
			get;
			set;
		}

		[DataMember]
		public int MRegProgress
		{
			get;
			set;
		}

		[DataMember]
		public int MOriRegProgress
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MConversionDate
		{
			get;
			set;
		}

		[DataMember]
		public string MDefaulLocaleID
		{
			get;
			set;
		}

		[DataMember(Order = 50)]
		[ApiMember("Country")]
		[ApiEnum(EnumMappingType.Country, IsRequired = false)]
		public string MCountryID
		{
			get;
			set;
		}

		[DataMember]
		public string MStateID
		{
			get;
			set;
		}

		[DataMember]
		public string MCountryName
		{
			get;
			set;
		}

		[DataMember(Order = 55)]
		[ApiMember("Region")]
		public string MStateName
		{
			get;
			set;
		}

		[DataMember(Order = 60)]
		[ApiMember("City")]
		public string MCityID
		{
			get;
			set;
		}

		[DataMember(Order = 65)]
		[ApiMember("Street")]
		public string MStreet
		{
			get;
			set;
		}

		[DataMember(Order = 70)]
		[ApiMember("PostCode")]
		public string MPostalNo
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsPaid
		{
			get;
			set;
		}

		[DataMember]
		public string MRegAddress
		{
			get;
			set;
		}

		[DataMember]
		public bool MInitBalanceOver
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MGLConversionDate
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MFABeginDate
		{
			get;
			set;
		}

		[DataMember]
		public int MigrateProgress
		{
			get;
			set;
		}

		[DataMember]
		public string MAccountTableID
		{
			get;
			set;
		}

		[DataMember]
		public string MViewBizObjectIDs
		{
			get;
			set;
		}

		[DataMember]
		public string MChangeBizObjectIDs
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsExpired
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsBeta
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("TaxpayerType")]
		[ApiEnum(EnumMappingType.TaxpayerType)]
		public string MTaxPayer
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("TaxNumber")]
		public string MTaxNo
		{
			get;
			set;
		}

		[DataMember(Order = 75)]
		public string MSystemLanguage
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("Language")]
		public string[] MLanguage
		{
			get;
			set;
		}

		[DataMember(Order = 80)]
		[ApiMember("CurrentAccountingPeriod")]
		public string MCurrentAccountingPeriod
		{
			get;
			set;
		}

		[DataMember(Order = 85)]
		[ApiMember("IsDemoCompany")]
		public bool MIsDemo
		{
			get;
			set;
		}

		[DataMember(Order = 100)]
		[ApiMember("SubscriptionStatus")]
		[ApiEnum(EnumMappingType.SubscriptionStatus)]
		public int MSubscriptionStatus
		{
			get;
			set;
		}

		[DataMember(Order = 105)]
		[ApiMember("Timezone")]
		[ApiEnum(EnumMappingType.Timezone)]
		public string MSystemZone
		{
			get;
			set;
		}

		[ApiMember("CreatedDateUTC")]
		public DateTime MOrgCreateDate
		{
			get
			{
				return base.MCreateDate;
			}
			set
			{
				base.MCreateDate = value;
			}
		}

		[DataMember(Order = 95)]
		[ApiMember("ExpirationDateUTC")]
		public DateTime MExpiredDate
		{
			get;
			set;
		}

		[DataMember]
		public string MShortCode
		{
			get;
			set;
		}

		[DataMember]
		public string MOrgCreateBy
		{
			get
			{
				return base.MCreateBy;
			}
			set
			{
				base.MCreateBy = value;
			}
		}

		public BASOrganisationModel()
			: base("T_Bas_Organisation")
		{
		}
	}
}
