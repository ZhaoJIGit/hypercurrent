using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RPT
{
	public class RPTDepreciationSummaryModel : RPTDepreciationBaseModel
	{
		[DataMember]
		public string MFATypeID
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
		public string MFATypeIDName
		{
			get;
			set;
		}

		[DataMember]
		public string MDepreciationID
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
