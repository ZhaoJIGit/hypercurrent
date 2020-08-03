using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.EntityModel
{
	public class DeleteParam : ExParamBase
	{
		public List<string> GetIdList()
		{
			if (string.IsNullOrWhiteSpace(base.ElementID))
			{
				return null;
			}
			return base.ElementID.Split(',').ToList();
		}
	}
}
