using JieNor.Megi.EntityModel.MultiLanguage;
using System.Collections.Generic;

namespace JieNor.Megi.DataModel.BD
{
	public class BDContactsEditModel : BDContactsInfoModel
	{
		public BDContactsEditModel()
		{
			base.MultiLanguage = new List<MultiLanguageFieldList>();
		}
	}
}
