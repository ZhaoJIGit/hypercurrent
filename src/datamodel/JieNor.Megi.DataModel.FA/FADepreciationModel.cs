using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.GL;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.FA
{
	[DataContract]
	public class FADepreciationModel : BDModel
	{
		[DataMember]
		public string MID
		{
			get;
			set;
		}

		[DataMember]
		public string MFixAssetsName
		{
			get;
			set;
		}

		[DataMember]
		public string MFixAssetsNumber
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
		public int MYear
		{
			get;
			set;
		}

		[DataMember]
		public int MPeriod
		{
			get;
			set;
		}

		[DataMember]
		public int MYearPeriod
		{
			get
			{
				return MYear * 100 + MPeriod;
			}
			set
			{
			}
		}

		[DataMember]
		public string MVoucherID
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
		public decimal MAdjustAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MLastAdjustAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTempPeriodDepreciatedAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MDepreciatedAmount
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
		public string MVoucherStatus
		{
			get;
			set;
		}

		[DataMember]
		public string MVoucherNumber
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
		public string MDepAccountFullName
		{
			get;
			set;
		}

		[DataMember]
		public string MExpAccountFullName
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
		public GLCheckGroupValueModel MCheckGroupValueModel
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

		public bool MIsChanged
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsLoadedCheckGroup
		{
			get;
			set;
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
		public decimal MLastDepreciatedAmount
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
		public DateTime MPurchaseDate
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
		public decimal MSalvageAmount
		{
			get
			{
				return Math.Round(MOriginalAmount * MRateOfSalvage / 100m, 2, MidpointRounding.AwayFromZero);
			}
			set
			{
			}
		}

		public FADepreciationModel()
			: base("T_FA_Depreciation")
		{
		}

		public FADepreciationModel(string tableName)
			: base(tableName)
		{
		}
	}
}
