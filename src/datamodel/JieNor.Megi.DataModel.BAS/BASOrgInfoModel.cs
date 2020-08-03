using JieNor.Megi.Core.Attribute;
using JieNor.Megi.DataModel.REG;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BAS
{
	[DataContract]
	public class BASOrgInfoModel
	{
		[DataMember]
		public string MOrgID
		{
			get;
			set;
		}

		[DataMember]
		public string MOrgName
		{
			get;
			set;
		}

		[DBFieldValidate("t_bas_organisation", "MName")]
		[DataMember]
		public string MDisplayName
		{
			get;
			set;
		}

		[DBFieldValidate("t_bas_organisation", "MLegalTradingName")]
		[DataMember]
		public string MLagalTrading
		{
			get;
			set;
		}

		[DataMember]
		public string MOrgTypeID
		{
			get;
			set;
		}

		[DataMember]
		public string MOrgDesc
		{
			get;
			set;
		}

		[DataMember]
		public string MCountryID
		{
			get;
			set;
		}

		[DBFieldValidate("t_bas_organisation", "MStateID")]
		[DataMember]
		public string MStateID
		{
			get;
			set;
		}

		[DBFieldValidate("t_bas_organisation", "MCityID")]
		[DataMember]
		public string MCityID
		{
			get;
			set;
		}

		[DBFieldValidate("t_bas_organisation", "MStreet")]
		[DataMember]
		public string MStreet
		{
			get;
			set;
		}

		[DBFieldValidate("t_bas_organisation", "MPostalNo")]
		[DataMember]
		public string MPostalNo
		{
			get;
			set;
		}

		[DataMember]
		public string MPostalID
		{
			get;
			set;
		}

		[DataMember]
		public string MPostalStreet
		{
			get;
			set;
		}

		[DataMember]
		public string MPostalTownCity
		{
			get;
			set;
		}

		[DataMember]
		public string MPostalStateRegion
		{
			get;
			set;
		}

		[DataMember]
		public string MPostalZipcode
		{
			get;
			set;
		}

		[DataMember]
		public string MPostalCountry
		{
			get;
			set;
		}

		[DataMember]
		public string MPostalAttention
		{
			get;
			set;
		}

		[DataMember]
		public string MPhysicalID
		{
			get;
			set;
		}

		[DataMember]
		public string MPhysicalStreet
		{
			get;
			set;
		}

		[DataMember]
		public string MPhysicalTownCity
		{
			get;
			set;
		}

		[DataMember]
		public string MPhysicalStateRegion
		{
			get;
			set;
		}

		[DataMember]
		public string MPhysicalZipcode
		{
			get;
			set;
		}

		[DataMember]
		public string MPhysicalCountry
		{
			get;
			set;
		}

		[DataMember]
		public string MPhysicalAttention
		{
			get;
			set;
		}

		[DataMember]
		public string MContactID
		{
			get;
			set;
		}

		[DataMember]
		public string MFacebook
		{
			get;
			set;
		}

		[DataMember]
		public string MLinkedIn
		{
			get;
			set;
		}

		[DataMember]
		public string MTwitter
		{
			get;
			set;
		}

		[DataMember]
		public string MGooglePlus
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
		public string MTBShop
		{
			get;
			set;
		}

		[DataMember]
		public string MWeChat
		{
			get;
			set;
		}

		[DataMember]
		public string MWeibo
		{
			get;
			set;
		}

		[DataMember]
		public string MQQ
		{
			get;
			set;
		}

		[DataMember]
		public int MVersionID
		{
			get;
			set;
		}

		[DataMember]
		public REGGlobalizationModel GlobalizationModel
		{
			get;
			set;
		}
	}
}
