using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.DataRowModel
{
	[DataContract]
	public class BaseReportRowModel
	{
		[DataMember]
		public int RowNumber
		{
			get;
			set;
		}

		[DataMember]
		public string RowType
		{
			get;
			set;
		}
	}
}
