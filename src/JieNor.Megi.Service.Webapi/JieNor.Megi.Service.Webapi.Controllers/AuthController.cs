using JieNor.Megi.Common.Context;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;

namespace JieNor.Megi.Service.Webapi.Controllers
{
	public class AuthController : ApiController
	{
		[HttpGet]
		public int Check(string t = null, string e = null)
		{
			int result = 2;
			List<string> accessWhiteList = ContextHelper.AccessWhiteList;
			string dnsSafeHost = HttpContext.Current.Request.Url.DnsSafeHost;
			if (!accessWhiteList.Contains(dnsSafeHost))
			{
				result = (int)ContextHelper.ValidateAccessToken(null, t, e, null, null);
			}
			return result;
		}
	}
}
