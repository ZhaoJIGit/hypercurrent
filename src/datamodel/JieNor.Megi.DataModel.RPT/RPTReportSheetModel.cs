using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RPT
{
	public class RPTReportSheetModel
	{
		[DataMember]
		public string MID
		{
			get;
			set;
		}

		[DataMember]
		public int MType
		{
			get;
			set;
		}

		[DataMember]
		public string MSheetName
		{
			get;
			set;
		}
	}
}
