using JieNor.Megi.Core.DataModel;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BAS
{
	[DataContract]
	public class BASExchangeRateModel : BDModel
	{
		[DataMember]
		public string MSourceCurrencyID
		{
			get;
			set;
		}

		[DataMember]
		public string MTargetCurrencyID
		{
			get;
			set;
		}

		[DataMember]
		public decimal MRate
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MRateDate
		{
			get;
			set;
		}

		public BASExchangeRateModel()
			: base("T_Bas_ExchangeRate")
		{
		}
	}
}
