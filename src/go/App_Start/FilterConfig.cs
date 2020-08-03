using JieNor.Megi.Tools;
using JieNor.Megi.Tools.Attribute;
using JieNor.Megi.Tools.Resubmit;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new UserLogAttribute());
			filters.Add(new AuthBrowserTabAttribute());
			filters.Add(new AuthorizationAttribute());
			filters.Add(new ModelValidateFilter());
			filters.Add(new ValidateReHttpPostTokenAttribute());
		}
	}
}
