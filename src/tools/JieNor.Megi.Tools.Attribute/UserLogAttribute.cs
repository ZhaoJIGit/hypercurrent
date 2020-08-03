using System;
using System.Web.Mvc;
using JieNor.Megi.Common.Context;

namespace JieNor.Megi.Tools.Attribute
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
	public class UserLogAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext actionContext)
		{
			MUserLogManager.SaveUserLog(actionContext);
		}
	}
}
