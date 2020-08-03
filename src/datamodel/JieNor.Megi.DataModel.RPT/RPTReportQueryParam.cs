using JieNor.Megi.Core;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RPT
{
	[DataContract]
	public class RPTReportQueryParam : SqlWhere
	{
		[DataMember]
		public RPTReportStatus Status
		{
			get;
			set;
		}
	}
}
