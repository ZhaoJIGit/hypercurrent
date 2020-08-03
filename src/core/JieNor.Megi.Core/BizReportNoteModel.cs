using System.Runtime.Serialization;

namespace JieNor.Megi.Core
{
	[DataContract]
	public class BizReportNoteModel
	{
		[DataMember]
		public string No
		{
			get;
			set;
		}

		[DataMember]
		public string Value
		{
			get;
			set;
		}
	}
}
