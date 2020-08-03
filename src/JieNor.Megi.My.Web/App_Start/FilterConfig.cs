using JieNor.Megi.Tools;
using JieNor.Megi.Tools.Attribute;
using System.Web.Mvc;

namespace JieNor.Megi.My.Web
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new AuthBrowserTabAttribute());
			filters.Add(new UserLogAttribute());
			filters.Add(new AuthorizationAttribute());
		}
	}
}
