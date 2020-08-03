using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.Chart
{
	public class PieChart2DModel
	{
		[DataMember]
		public string name
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
		public string color
		{
			get;
			set;
		}

		public decimal MTotalAmount
		{
			get;
			set;
		}
	}
}
