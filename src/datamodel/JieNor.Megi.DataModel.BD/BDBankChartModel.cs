using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDBankChartModel
	{
		[DataMember]
		public ChartModel InChart
		{
			get;
			set;
		}

		[DataMember]
		public ChartModel OutChart
		{
			get;
			set;
		}

		public BDBankChartModel()
		{
			InChart = new ChartModel();
			OutChart = new ChartModel();
		}
	}
}
