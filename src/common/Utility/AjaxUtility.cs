using System.Web;

namespace JieNor.Megi.Common.Utility
{
	public class AjaxUtility
	{
		public static bool IsAjaxRequest(HttpContextBase request)
		{
			string requestType = request.Request.Headers["X-Requested-With"];
			return requestType?.Equals("XMLHttpRequest") ?? false;
		}
	}
}
