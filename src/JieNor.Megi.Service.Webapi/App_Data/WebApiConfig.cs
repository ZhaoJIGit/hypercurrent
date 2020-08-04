using JieNor.Megi.Core.Context;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace JieNor.Megi.Service.Webapi
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			config.MapHttpAttributeRoutes();
			config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{action}/{id}", new
			{
				id = RouteParameter.Optional
			});
			JsonMediaTypeFormatter formatter = new JsonMediaTypeFormatter();
			config.Services.Replace(typeof(IContentNegotiator), new JsonContentNegotiator(formatter));
		}
	}
}
