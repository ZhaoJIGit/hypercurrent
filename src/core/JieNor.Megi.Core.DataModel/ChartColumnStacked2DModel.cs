using System.Runtime.Serialization;

namespace JieNor.Megi.Core.DataModel
{
	[DataContract]
	public class ChartColumnStacked2DModel
	{
		[DataMember]
		public string name
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

		[DataMember]
		public string MContactID
		{
			get;
			set;
		}

		[DataMember]
		public string MChartFirstName
		{
			get;
			set;
		}

		[DataMember]
		public string MChartLastName
		{
			get;
			set;
		}

		[DataMember]
		public string[] MChartDueOrOwing
		{
			get;
			set;
		}
	}
}
