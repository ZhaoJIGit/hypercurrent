using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.Report
{
	[DataContract]
	public class ReportCombineModel
	{
		[DataMember]
		public ReportHeaderCollection ReportHeaders
		{
			get;
			set;
		}

		[DataMember]
		public ReportGroupCollection ReportGroups
		{
			get;
			set;
		}

		[DataMember]
		public ReportItemCollection ReportItems
		{
			get;
			set;
		}

		[DataMember]
		public ReportSubTotalCollection ReportSubTotals
		{
			get;
			set;
		}

		[DataMember]
		public ReportTotalCollection ReportTotals
		{
			get;
			set;
		}
	}
}
