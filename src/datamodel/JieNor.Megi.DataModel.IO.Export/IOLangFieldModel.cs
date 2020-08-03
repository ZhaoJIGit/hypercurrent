using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export
{
	[DataContract]
	public class IOLangFieldModel
	{
		[DataMember]
		public string MFieldName
		{
			get;
			set;
		}

		[DataMember]
		public string MLocalID
		{
			get;
			set;
		}

		[DataMember]
		public string MName
		{
			get;
			set;
		}
	}
}
