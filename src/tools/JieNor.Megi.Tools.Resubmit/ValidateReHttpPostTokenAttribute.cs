using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Fasterflect;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;

namespace JieNor.Megi.Tools.Resubmit
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public sealed class ValidateReHttpPostTokenAttribute : FilterAttribute, IActionFilter
	{
		public IPageTokenView PageTokenView { get; set; }

		public ValidateReHttpPostTokenAttribute()
		{
			this.PageTokenView = new SessionPageTokenView();
		}

		public void OnActionExecuting(ActionExecutingContext filterContext)
		{
			if (filterContext == null)
			{
				filterContext.Result = new JsonResult
				{
					Data = new MJsonResult
					{
						Codes = new List<MActionResultCodeEnum>
						{
							MActionResultCodeEnum.MLoginInfoLost
						},
						IsJsonResult = true
					},
					JsonRequestBehavior = JsonRequestBehavior.DenyGet
				};
				return;
			}
			if (!AjaxRequestExtensions.IsAjaxRequest(filterContext.HttpContext.Request))
			{
				return;
			}
			if (!this.PageTokenView.TokensMatch)
			{
				filterContext.Result = new JsonResult
				{
					Data = new MJsonResult
					{
						Codes = new List<MActionResultCodeEnum>
						{
							MActionResultCodeEnum.MResubmitException
						},
						IsJsonResult = true
					},
					JsonRequestBehavior = JsonRequestBehavior.DenyGet
				};
				return;
			}
		}

		public void OnActionExecuted(ActionExecutedContext filterContext)
		{
			if (filterContext.Result != null && AjaxRequestExtensions.IsAjaxRequest(filterContext.HttpContext.Request))
			{
				string getLastPageID = this.PageTokenView.GetLastPageID;
				object obj = filterContext.Result.TryGetPropertyValue("Data");
				if (obj != null && !string.IsNullOrWhiteSpace(getLastPageID) && obj.TrySetPropertyValue(PageTokenViewBase.HiddenPageIDName, getLastPageID) && obj.TrySetPropertyValue(PageTokenViewBase.HiddenTokenName, this.PageTokenView.GeneratePageToken(getLastPageID)))
				{
					filterContext.Result.TrySetPropertyValue("Data", obj);
				}
			}
		}
	}
}
