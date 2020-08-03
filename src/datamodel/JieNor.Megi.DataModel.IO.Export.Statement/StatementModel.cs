using System.ComponentModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.Statement
{
	[DataContract]
	public class StatementModel : ExportIVBaseModel
	{
		[DataMember]
		public string StatementTitle
		{
			get;
			set;
		}

		[DataMember]
		public string ToTitle
		{
			get;
			set;
		}

		[DataMember]
		public string StatementDateTitle
		{
			get;
			set;
		}

		[DataMember]
		public string StatementType
		{
			get;
			set;
		}

		[DisplayName("StatementDate")]
		[DataMember]
		public string EndDate
		{
			get;
			set;
		}

		[DataMember]
		public string ContactName
		{
			get;
			set;
		}

		[DataMember]
		public StatementGroupCollection StatementGroups
		{
			get;
			set;
		}
	}
}
