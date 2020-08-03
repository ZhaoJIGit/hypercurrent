using JieNor.Megi.Core.DataModel;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BAS
{
	[DataContract]
	public class BASTimezonesModel : BDModel
	{
		[DataMember]
		public int MUtcOffSetM
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MUtcOffset
		{
			get;
			set;
		}

		[DataMember]
		public bool MSupportsDST
		{
			get;
			set;
		}

		public BASTimezonesModel()
			: base("T_Bas_Timezones")
		{
		}
	}
}
