using JieNor.Megi.EntityModel.MultiLanguage;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDBankTypeEditModel : BDBankTypeModel
	{
		[DataMember]
		public int MSeq
		{
			get;
			set;
		}

		public BDBankTypeEditModel()
		{
			base.MultiLanguage = new List<MultiLanguageFieldList>();
		}

		public BDBankTypeEditModel(string tableName)
			: base(tableName)
		{
		}
	}
}
