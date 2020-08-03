using System.Runtime.Serialization;

namespace JieNor.Megi.Core.DataModel
{
	[DataContract]
	public class ChartScaleModel
	{
		[DataMember]
		public double start_scale
		{
			get;
			set;
		}

		[DataMember]
		public double? end_scale
		{
			get;
			set;
		}

		[DataMember]
		public double? originy
		{
			get;
			set;
		}

		[DataMember]
		public double scale_space
		{
			get;
			set;
		}

		[DataMember]
		public double max_scale
		{
			get;
			set;
		}

		public ChartScaleModel()
		{
			scale_space = 10.0;
			max_scale = 10.0;
		}
	}
}
