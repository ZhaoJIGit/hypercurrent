using JieNor.Megi.Tools.Attribute;
using System.Web.Mvc;

namespace JieNor.Megi.Main.Web
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new UserLogAttribute());
		}
	}
}
