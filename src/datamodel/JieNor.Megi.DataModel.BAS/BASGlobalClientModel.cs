using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BAS
{
	[DataContract]
	public class BASGlobalClientModel
	{
		[DataMember]
		public string MDateFormat
		{
			get;
			set;
		}

		[DataMember]
		public string MTimeFormat
		{
			get;
			set;
		}

		[DataMember]
		public string MDigitDot
		{
			get;
			set;
		}

		[DataMember]
		public string MDigitGroupingSymbol
		{
			get;
			set;
		}

		[DataMember]
		public string MDigitNegative
		{
			get;
			set;
		}

		[DataMember]
		public string BaseCurrencyID
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MBeginDate
		{
			get;
			set;
		}
	}
}
