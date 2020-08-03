using System.Runtime.Serialization;

namespace JieNor.Megi.Core.DataModel
{
	[DataContract]
	public class ChartDataModel
	{
		[DataMember]
		public string name
		{
			get;
			set;
		}

		[DataMember]
		public string value
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
	}
}
