using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RPT.GL
{
	[DataContract]
	public class RPTFilterSchemeFilterModel
	{
		[DataMember]
		public string MOrgID
		{
			get;
			set;
		}

		[DataMember]
		public int MReportType
		{
			get;
			set;
		}

		public string MItemID
		{
			get;
			set;
		}
	}
}
