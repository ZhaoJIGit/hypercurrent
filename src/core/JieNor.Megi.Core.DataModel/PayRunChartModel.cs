using System.Runtime.Serialization;

namespace JieNor.Megi.Core.DataModel
{
	[DataContract]
	public class PayRunChartModel
	{
		[DataMember]
		public string name
		{
			get;
			set;
		}

		[DataMember]
		public string field
		{
			get;
			set;
		}

		[DataMember]
		public string color
		{
			get;
			set;
		}

		[DataMember]
		public double[] value
		{
			get;
			set;
		}
	}
}
