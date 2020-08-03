using JieNor.Megi.Core;

namespace JieNor.Megi.DataModel.BD
{
	public class BDContactsListFilter : ListFilter
	{
		public int ContactType
		{
			get;
			set;
		}

		public bool IncludeDisable
		{
			get;
			set;
		}
	}
}
