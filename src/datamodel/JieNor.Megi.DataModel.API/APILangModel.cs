using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.API
{
	[DataContract]
	public class APILangModel
	{
		[DataMember]
		public string LCID
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
