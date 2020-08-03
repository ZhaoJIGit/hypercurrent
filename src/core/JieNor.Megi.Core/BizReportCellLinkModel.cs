using System.Runtime.Serialization;

namespace JieNor.Megi.Core
{
	[DataContract]
	public class BizReportCellLinkModel
	{
		[DataMember]
		public string Text
		{
			get;
			set;
		}

		[DataMember]
		public string Url
		{
			get;
			set;
		}

		[DataMember]
		public string Title
		{
			get;
			set;
		}

		[DataMember]
		public bool DisabledEvent
		{
			get;
			set;
		}
	}
}
