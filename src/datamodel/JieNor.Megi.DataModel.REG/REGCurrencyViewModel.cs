using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.REG
{
	[DataContract]
	public class REGCurrencyViewModel : REGCurrencyModel
	{
		[DataMember]
		public DateTime MRateDate
		{
			get;
			set;
		}

		[DataMember]
		public string MCurrencyRateID
		{
			get
			{
				return base.MItemID;
			}
			set
			{
				base.MItemID = value;
			}
		}

		[DataMember]
		public string MUserRate
		{
			get;
			set;
		}

		[DataMember]
		public string MRate
		{
			get;
			set;
		}

		[DataMember]
		public string MExchangeRateID
		{
			get;
			set;
		}
	}
}
