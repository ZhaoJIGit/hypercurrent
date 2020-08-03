using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.Core.DataModel
{
	[DataContract]
	public class ChartModel
	{
		public List<ChartDataModel> Data
		{
			get;
			set;
		}

		[DataMember]
		public object[] MValue
		{
			get;
			set;
		}

		[DataMember]
		public string[] MLabels
		{
			get;
			set;
		}

		[DataMember]
		public string[] MTipLabels
		{
			get;
			set;
		}

		[DataMember]
		public ChartScaleModel MScale
		{
			get;
			set;
		}

		public ChartModel()
		{
			MScale = new ChartScaleModel();
			Data = new List<ChartDataModel>();
		}
	}
}
