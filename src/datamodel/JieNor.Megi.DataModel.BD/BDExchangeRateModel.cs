using JieNor.Megi.Core.DataModel;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDExchangeRateModel : BDModel
	{
		private decimal rate;

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
			get
			{
				return rate;
			}
			set
			{
				rate = value;
			}
		}

		[DataMember]
		public decimal MUserRate
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

		[DataMember]
		public int MRateType
		{
			get;
			set;
		}

		[DataMember]
		public string MNote
		{
			get;
			set;
		}

		public BDExchangeRateModel()
			: base("T_BD_ExchangeRate")
		{
		}
	}
}
