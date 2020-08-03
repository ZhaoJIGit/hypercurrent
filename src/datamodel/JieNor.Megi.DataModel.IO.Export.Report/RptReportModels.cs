using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.Report
{
	[DataContract]
	public class RptReportModels<T>
	{
		[DataMember]
		public ReportCollection<T> ReportModels
		{
			get;
			set;
		}
	}
}
