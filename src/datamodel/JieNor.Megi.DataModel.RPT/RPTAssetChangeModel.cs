using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RPT
{
	public class RPTAssetChangeModel : RPTDepreciationBaseModel
	{
		[DataMember]
		public DateTime MPurchaseDate
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
		public string MPrefix
		{
			get;
			set;
		}

		[DataMember]
		public string MNumber
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
		public decimal MDepreciatedAmount
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
		public string MFixAssetsName
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
		public string MFixAssetsNumber
		{
			get
			{
				return MPrefix + MNumber;
			}
			set
			{
			}
		}

		[DataMember]
		public decimal MOriginalAmountChange
		{
			get;
			set;
		}

		[DataMember]
		public decimal MDepreciatedAmountChange
		{
			get;
			set;
		}

		[DataMember]
		public decimal MNetAmountChange
		{
			get;
			set;
		}

		[DataMember]
		public int MChanged
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
		public int MIndex
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
		public string MId
		{
			get;
			set;
		}

		[DataMember]
		public int MIsAdjust
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
		public int MLastDepreciatedYear
		{
			get;
			set;
		}

		[DataMember]
		public decimal MOriginalBeginPeriod
		{
			get;
			set;
		}

		[DataMember]
		public decimal MOriginalAdd
		{
			get;
			set;
		}

		[DataMember]
		public decimal MOriginalTurnOut
		{
			get;
			set;
		}

		[DataMember]
		public decimal MOriginalEndBlance
		{
			get;
			set;
		}

		[DataMember]
		public decimal MDepreciationBeginPeriod
		{
			get;
			set;
		}

		[DataMember]
		public decimal MDepreciationAdd
		{
			get;
			set;
		}

		[DataMember]
		public decimal MDepreciationTurnOut
		{
			get;
			set;
		}

		[DataMember]
		public decimal MDepreciationEndBlance
		{
			get;
			set;
		}
	}
}
