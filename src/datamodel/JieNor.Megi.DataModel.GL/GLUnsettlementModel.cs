using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.GL
{
	[DataContract]
	public class GLUnsettlementModel
	{
		[DataMember]
		public int MYear
		{
			get;
			set;
		}

		[DataMember]
		public int MPeriod
		{
			get;
			set;
		}

		[DataMember]
		public int MStatus
		{
			get;
			set;
		}

		[DataMember]
		public string MSettlerID
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MSettleDate
		{
			get;
			set;
		}

		[DataMember]
		public bool IsNumberBroken
		{
			get;
			set;
		}
	}
}
