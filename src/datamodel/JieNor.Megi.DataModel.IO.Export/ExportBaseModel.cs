using System.ComponentModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export
{
	[DataContract]
	public class ExportBaseModel
	{
		[DisplayName("OrgnizationName")]
		[DataMember]
		public string OrgName
		{
			get;
			set;
		}

		public int Index
		{
			get;
			set;
		}
	}
}
