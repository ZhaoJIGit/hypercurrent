using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BAS
{
	[DataContract]
	public class BASOrgPrefixSettingFilterModel
	{
		[DataMember]
		public int MConversionYear
		{
			get;
			set;
		}

		[DataMember]
		public int MConversionMonth
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MFABeginDate
		{
			get;
			set;
		}
	}
}
