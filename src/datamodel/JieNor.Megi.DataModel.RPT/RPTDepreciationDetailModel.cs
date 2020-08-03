using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RPT
{
	public class RPTDepreciationDetailModel : RPTDepreciationBaseModel
	{
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
		public decimal MPrepareForDecreaseAmount
		{
			get;
			set;
		}

		public decimal MRateOfSalvage
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
		public decimal MNetAmount
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
		public decimal MOriginalAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MUsefulPeriods
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
		public int MIsAdjust
		{
			get;
			set;
		}

		[DataMember]
		public decimal MOrgDepreciatedAmountOfYear
		{
			get;
			set;
		}

		[DataMember]
		public decimal MOrgPrepareForDecreaseAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MOrgDepreciatedAmount
		{
			get;
			set;
		}
	}
}
