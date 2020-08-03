using System.Runtime.Serialization;

namespace JieNor.Megi.Core.DataModel
{
	[DataContract]
	public class ChartPie2DModel
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
		public decimal value
		{
			get;
			set;
		}

		[DataMember]
		public decimal MOverDue
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
	}
}
