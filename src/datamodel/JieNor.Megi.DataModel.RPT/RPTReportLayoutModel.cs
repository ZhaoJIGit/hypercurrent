using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RPT
{
	[DataContract]
	public class RPTReportLayoutModel : BizDataModel
	{
		[DataMember]
		public string MBizObject
		{
			get;
			set;
		}

		[DataMember]
		public string MReportType
		{
			get;
			set;
		}

		[DataMember]
		public string MReportLayout
		{
			get;
			set;
		}

		[DataMember]
		public string MPrintSettingID
		{
			get;
			set;
		}

		public RPTReportLayoutModel()
			: base("T_RPT_ReportLayout")
		{
		}
	}
}
