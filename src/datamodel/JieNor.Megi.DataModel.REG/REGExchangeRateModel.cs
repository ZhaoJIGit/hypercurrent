using JieNor.Megi.Core.Attribute;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.REG
{
	[DataContract]
	public class REGExchangeRateModel
	{
		[DataMember]
		[ApiMember("ExchangeRate")]
		public decimal MExchangeRate
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("EffectiveDate")]
		public DateTime MEffectiveDate
		{
			get;
			set;
		}
	}
}
