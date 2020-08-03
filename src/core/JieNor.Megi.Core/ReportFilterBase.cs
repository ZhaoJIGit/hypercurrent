using System.Runtime.Serialization;

namespace JieNor.Megi.Core
{
	[DataContract]
	public class ReportFilterBase
	{
		[DataMember]
		public string MParentReportID
		{
			get;
			set;
		}

		[DataMember]
		public string MReportID
		{
			get;
			set;
		}

		[DataMember]
		public bool IsReload
		{
			get;
			set;
		}
	}
}
