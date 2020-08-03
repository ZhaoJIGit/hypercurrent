using System;
using System.Web;
using System.Web.Mvc;
using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;

namespace JieNor.Megi.Tools
{
	public class AuthBrowserTabAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext actionContext)
		{
			HttpRequestBase request = actionContext.HttpContext.Request;
			MContext mcontext = ContextHelper.MContext;
			if (this.IsLoginUrl(request.Path))
			{
				return;
			}
			string text = request.QueryString["bti"];
			string value = (request.UrlReferrer != null) ? request.UrlReferrer.ToString() : null;
			if (mcontext == null)
			{
				this.SetActionResult(actionContext);
				return;
			}
			if (string.IsNullOrWhiteSpace(value))
			{
				actionContext.Result = new RedirectResult(ServerHelper.LoginServer);
				return;
			}
			if ((request.Path.ToLower().IndexOf(ContextHelper.MCheckTokenUrl.ToLower()) >= 0 || request.Path.ToLower() == "/") && (string.IsNullOrWhiteSpace(text) || text != mcontext.MBrowserTabIndex))
			{
				this.SetActionResult(actionContext);
			}
		}

		public override void OnActionExecuted(ActionExecutedContext filterContext)
		{
		}

		private bool IsLoginUrl(string url)
		{
			return url.ToLower().Equals(ContextHelper.MLoginBoxSignInUrl);
		}

		private void SetActionResult(ActionExecutingContext actionContext)
		{
			if (AjaxRequestExtensions.IsAjaxRequest(actionContext.HttpContext.Request))
			{
				actionContext.HttpContext.Response.StatusCode = 252;
				actionContext.Result = new JsonResult
				{
					Data = new
					{
						accessDenied = 1,
						type = LoginStateEnum.ForceLogout
					},
					JsonRequestBehavior = 0
				};
				return;
			}
			actionContext.Result = new RedirectResult(ServerHelper.LoginServer);
		}
	}
}
