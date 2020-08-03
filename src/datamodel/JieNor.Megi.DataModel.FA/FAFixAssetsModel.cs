using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.GL;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.FA
{
	[DataContract]
	public class FAFixAssetsModel : BDModel
	{
		[DataMember]
		public bool MChanged
		{
			get;
			set;
		}

		[DataMember]
		public string MFATypeID
		{
			get;
			set;
		}

		[DataMember]
		public string MDepreciationTypeID
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsAdjust
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("Name", ApiMemberType.MultiLang, false, false)]
		public string MName
		{
			get;
			set;
		}

		[DataMember]
		public string MPrefix
		{
			get;
			set;
		}

		[DataMember]
		public decimal MQuantity
		{
			get;
			set;
		}

		[DataMember]
		public string _fullNumber
		{
			get;
			set;
		}

		[DataMember]
		public string MFullNumber
		{
			get
			{
				return string.IsNullOrWhiteSpace(_fullNumber) ? (MPrefix + base.MNumber) : _fullNumber;
			}
			set
			{
				_fullNumber = value;
			}
		}

		[DataMember]
		public string MRemark
		{
			get;
			set;
		}

		[DataMember]
		public string MFixAccountCode
		{
			get;
			set;
		}

		[DataMember]
		public string MDepAccountCode
		{
			get;
			set;
		}

		[DataMember]
		public string MExpAccountCode
		{
			get;
			set;
		}

		[DataMember]
		public string MFixCheckGroupValueID
		{
			get;
			set;
		}

		[DataMember]
		public string MDepCheckGroupValueID
		{
			get;
			set;
		}

		[DataMember]
		public string MExpCheckGroupValueID
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MDepreciationFromPeriod
		{
			get;
			set;
		}

		[DataMember]
		public bool MDepreciationFromCurrentPeriod
		{
			get;
			set;
		}

		[DataMember]
		public decimal MOriginalAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MPeriodDepreciatedAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MDepreciatedAmountOfYear
		{
			get;
			set;
		}

		[DataMember]
		public decimal MRateOfSalvage
		{
			get;
			set;
		}

		[DataMember]
		public decimal MSalvageAmount
		{
			get
			{
				return Math.Round(MOriginalAmount * (MRateOfSalvage / 100m), 2, MidpointRounding.AwayFromZero);
			}
			set
			{
			}
		}

		[DataMember]
		public decimal MPrepareForDecreaseAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MNetAmount
		{
			get
			{
				return MOriginalAmount - MPrepareForDecreaseAmount - MDepreciatedAmount;
			}
			set
			{
			}
		}

		[DataMember]
		public decimal MDepreciatedAmount
		{
			get;
			set;
		}

		[DataMember]
		public int MLastDepreciatedYear
		{
			get;
			set;
		}

		[DataMember]
		public int MLastDepreciatedPeriod
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MLastDepreciatedDate
		{
			get
			{
				return (MLastDepreciatedYear == 0) ? DateTime.MinValue : new DateTime(MLastDepreciatedYear, MLastDepreciatedPeriod, 1);
			}
			set
			{
				MLastDepreciatedYear = value.Year;
				MLastDepreciatedPeriod = value.Month;
			}
		}

		[DataMember]
		public decimal MLastDepreciatedAmount
		{
			get;
			set;
		}

		[DataMember]
		public int MStatus
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MHandledDate
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MPurchaseDate
		{
			get;
			set;
		}

		[DataMember]
		public int MUsefulPeriods
		{
			get;
			set;
		}

		[DataMember]
		public int MDepreciatedPeriods
		{
			get;
			set;
		}

		[DataMember]
		public string MDepreciationTypeName
		{
			get;
			set;
		}

		[DataMember]
		public string MFATypeIDName
		{
			get;
			set;
		}

		[DataMember]
		public string MFixAccountID
		{
			get;
			set;
		}

		[DataMember]
		public string MDepAccountID
		{
			get;
			set;
		}

		[DataMember]
		public string MExpAccountID
		{
			get;
			set;
		}

		[DataMember]
		public string MFixAccountName
		{
			get;
			set;
		}

		[DataMember]
		public string MDepAccountName
		{
			get;
			set;
		}

		[DataMember]
		public string MExpAccountName
		{
			get;
			set;
		}

		[DataMember]
		public GLCheckGroupValueModel MFixCheckGroupValueModel
		{
			get;
			set;
		}

		[DataMember]
		public GLCheckGroupValueModel MDepCheckGroupValueModel
		{
			get;
			set;
		}

		[DataMember]
		public GLCheckGroupValueModel MExpCheckGroupValueModel
		{
			get;
			set;
		}

		[DataMember]
		public GLCheckGroupValueModel MCheckGroupValueModel
		{
			get;
			set;
		}

		[DataMember]
		public BASOrgPrefixSettingModel MPrefixModel
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MChangeFromPeriod
		{
			get;
			set;
		}

		[DataMember]
		public bool MBackAdjust
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsChange
		{
			get;
			set;
		}

		[DataMember]
		public decimal MSrcPeriodDepreciatedAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MSrcDepreciatedAmountOfYear
		{
			get;
			set;
		}

		[DataMember]
		public int MSrcDepreciatedPeriods
		{
			get;
			set;
		}

		[DataMember]
		public decimal MSrcDepreciatedAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MSrcNetAmount
		{
			get;
			set;
		}

		[DataMember]
		public FAFixAssetsModel MSrcModel
		{
			get;
			set;
		}

		[DataMember]
		public int? MDepreciatedPeriod
		{
			get;
			set;
		}

		[DataMember]
		public decimal? MSalvageRate
		{
			get;
			set;
		}

		[DataMember]
		public string MHandleDateString
		{
			get;
			set;
		}

		[DataMember]
		public string MName0x0009
		{
			get;
			set;
		}

		[DataMember]
		public string MName0x7804
		{
			get;
			set;
		}

		[DataMember]
		public string MName0x7C04
		{
			get;
			set;
		}

		public FAFixAssetsModel()
			: base("T_FA_FixAssets")
		{
		}

		public FAFixAssetsModel(string tableName)
			: base(tableName)
		{
		}
	}
}
