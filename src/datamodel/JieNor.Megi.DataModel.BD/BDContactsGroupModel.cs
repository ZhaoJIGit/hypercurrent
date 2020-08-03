using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDContactsGroupModel : BDModel
	{
		public BDContactsGroupModel()
			: base("T_BD_ContactsType")
		{
		}
	}
}
