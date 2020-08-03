using System.Collections.Generic;

namespace JieNor.Megi.DataModel.RPT.GL
{
	public class FilterSchemeContentModel
	{
		public List<FilterSchemeCheckGroupModel> CheckGroup
		{
			get;
			set;
		}

		public List<FilterSchemeConditionModel> Condition
		{
			get;
			set;
		}
	}
}
