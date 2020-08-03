using JieNor.Megi.Core;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDExchangeRateFilterModel : SqlWhere
	{
		[DataMember]
		public string Sort
		{
			get;
			set;
		}

		[DataMember]
		public string Order
		{
			get;
			set;
		}

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
		public string MLCID
		{
			get;
			set;
		}

		[DataMember]
		public string MOrgID
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
	}
}
