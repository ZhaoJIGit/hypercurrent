using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JieNor.Megi.Core.Helper
{
	public static class ListSortHelper
	{
		public static List<T> Sort<T>(this List<T> list, string sort, string order)
		{
			List<T> result = list;
			if (!string.IsNullOrEmpty(sort))
			{
				PropertyInfo propInfo = typeof(T).GetProperty(sort);
				result = ((!(order.ToUpper().Trim() == "ASC")) ? (from f in list
				orderby propInfo.GetValue(f, null) descending
				select f).ToList() : (from f in list
				orderby propInfo.GetValue(f, null)
				select f).ToList());
			}
			return result;
		}
	}
}
