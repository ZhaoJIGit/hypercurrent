using JieNor.Megi.EntityModel.MultiLanguage;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDAccountTypeEditModel : BDAcctTypeModel
	{
		public BDAccountTypeEditModel()
		{
			base.MultiLanguage = new List<MultiLanguageFieldList>();
		}
	}
}
