using System.Runtime.Serialization;

namespace JieNor.Megi.Core
{
	[DataContract]
	public class BizSubRptCreateModel
	{
		[DataMember]
		public string Text
		{
			get;
			set;
		}

		[DataMember]
		public BizReportType ReportType
		{
			get;
			set;
		}

		[DataMember]
		public ReportFilterBase ReportFilter
		{
			get;
			set;
		}
	}
}
