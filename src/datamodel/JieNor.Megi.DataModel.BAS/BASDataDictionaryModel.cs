using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BAS
{
	[DataContract]
	public class BASDataDictionaryModel
	{
		[DataMember]
		public string DictCode
		{
			get;
			set;
		}

		[DataMember]
		public string DictName
		{
			get;
			set;
		}

		[DataMember]
		public string DictValue
		{
			get;
			set;
		}

		[DataMember]
		public string ParentDictCode
		{
			get;
			set;
		}
	}
}
