using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BAS
{
	[DataContract]
	public class BASLangModel
	{
		[DataMember]
		public string LangID
		{
			get;
			set;
		}

		[DataMember]
		public string LangName
		{
			get;
			set;
		}

		[DataMember]
		public string StandardName
		{
			get;
			set;
		}

		[DataMember]
		public int MItemID
		{
			get;
			set;
		}

		[DataMember]
		public string LangKey
		{
			get;
			set;
		}

		[DataMember]
		public int MModuleID
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
