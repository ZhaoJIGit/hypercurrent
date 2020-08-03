using JieNor.Megi.EntityModel.MultiLanguage;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDContactsAddGroupModel
	{
		[DataMember]
		public string ContactIds
		{
			get;
			set;
		}

		[DataMember]
		public string GroupId
		{
			get;
			set;
		}

		[DataMember]
		public MultiLanguageFieldList NewGroupMultiLangModel
		{
			get;
			set;
		}

		[DataMember]
		public bool IsGroupExist
		{
			get;
			set;
		}

		[DataMember]
		public string MoveFromGroupId
		{
			get;
			set;
		}
	}
}
