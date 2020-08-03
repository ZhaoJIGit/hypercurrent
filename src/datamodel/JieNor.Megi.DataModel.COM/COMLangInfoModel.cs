using JieNor.Megi.EntityModel.MultiLanguage;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.COM
{
	[DataContract]
	public class COMLangInfoModel
	{
		[DataMember]
		public LangModule Module
		{
			get;
			set;
		}

		[DataMember]
		public string Key
		{
			get;
			set;
		}

		[DataMember]
		public string DefaultValue
		{
			get;
			set;
		}

		[DataMember]
		public string Comment
		{
			get;
			set;
		}

		public COMLangInfoModel(LangModule module, string key, string defaultValue, string comment = null)
		{
			Module = module;
			Key = key;
			DefaultValue = defaultValue;
			Comment = comment;
		}
	}
}
