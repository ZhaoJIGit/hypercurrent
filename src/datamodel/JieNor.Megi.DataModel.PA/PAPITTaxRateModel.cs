using JieNor.Megi.Core.DataModel;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.PA
{
	[DataContract]
	public class PAPITTaxRateModel : BDModel
	{
		[DataMember]
		public decimal MTaxThresholdAmount
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MEffectiveDate
		{
			get;
			set;
		}

		[DataMember]
		public decimal MBeginAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MEndAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTaxRate
		{
			get;
			set;
		}

		[DataMember]
		public decimal MDeductionAmount
		{
			get;
			set;
		}

		public PAPITTaxRateModel()
			: base("t_pa_pittaxrate")
		{
		}
	}
}
