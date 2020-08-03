using JieNor.Megi.Tools.Attribute;
using System.Web.Mvc;

namespace JieNor.Megi.Login.Web
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new UserLogAttribute());
			//filters.Add(new AuthorizationAttribute());
		}
	}
}
