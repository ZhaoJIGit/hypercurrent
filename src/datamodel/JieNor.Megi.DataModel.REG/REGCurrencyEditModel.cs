using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.REG
{
	[DataContract]
	public class REGCurrencyEditModel
	{
		[DataMember]
		public string MOrgID
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
		public string MTargetCurrencyName
		{
			get;
			set;
		}

		[DataMember]
		public decimal MSourceToTargetRate
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTargetToSourceRate
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MSourceToTargetRateDate
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MTargetToSourceRateDate
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
		public decimal MSourceToTargetUserRate
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTargetToSourceUserRate
		{
			get;
			set;
		}
	}
}
