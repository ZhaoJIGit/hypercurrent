using System;
using System.Web;
using System.Web.Mvc;
using log4net;

namespace JieNor.Megi.Core.Attribute
{
	public class LogErrorAttribute : ActionFilterAttribute, IExceptionFilter
	{
		public void OnException(ExceptionContext filterContext)
		{
			Exception exception = filterContext.Exception;
			this.SetLog(exception);
			string message = exception.Message;
			string rawUrl = HttpContext.Current.Request.RawUrl;
			filterContext.ExceptionHandled = true;
			filterContext.Result = new RedirectResult("/Error");
		}

		private void SetLog(Exception error)
		{
			try
			{
				ILog logger = LogManager.GetLogger("loginfo");
				logger.Info(error);
			}
			catch
			{
				throw;
			}
		}
	}
}
