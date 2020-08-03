using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.GL
{
	[Serializable]
	public class GLCurrencyDataModel
	{
		[DataMember]
		public string MCurrencyID
		{
			get;
			set;
		}

		[DataMember]
		public decimal MExchangeRate
		{
			get;
			set;
		}

		[DataMember]
		public decimal MAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MAmountFor
		{
			get;
			set;
		}
	}
}
