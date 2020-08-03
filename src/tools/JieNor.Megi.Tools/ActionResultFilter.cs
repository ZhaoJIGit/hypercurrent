using System;
using System.Web.Mvc;

namespace JieNor.Megi.Tools
{
	public class ActionResultFilter : IResultFilter
	{
		public void OnResultExecuted(ResultExecutedContext filterContext)
		{
			if (filterContext.Result != null && filterContext.Result.GetType() == typeof(JsonResult))
			{
				JsonResult jsonResult = (JsonResult)filterContext.Result;
				filterContext.Result = new JsonResult
				{
					Data = new
					{
						Data = jsonResult,
						MyData = "123"
					},
					JsonRequestBehavior = jsonResult.JsonRequestBehavior,
					MaxJsonLength = jsonResult.MaxJsonLength,
					ContentType = jsonResult.ContentType,
					ContentEncoding = jsonResult.ContentEncoding,
					RecursionLimit = jsonResult.RecursionLimit
				};
			}
		}

		public void OnResultExecuting(ResultExecutingContext filterContext)
		{
			if (filterContext.Result != null && filterContext.Result.GetType() == typeof(JsonResult))
			{
				JsonResult jsonResult = (JsonResult)filterContext.Result;
				filterContext.Result = new JsonResult
				{
					Data = new
					{
						Data = jsonResult,
						MyData = "123"
					},
					JsonRequestBehavior = jsonResult.JsonRequestBehavior,
					MaxJsonLength = jsonResult.MaxJsonLength,
					ContentType = jsonResult.ContentType,
					ContentEncoding = jsonResult.ContentEncoding,
					RecursionLimit = jsonResult.RecursionLimit
				};
			}
		}
	}
}
