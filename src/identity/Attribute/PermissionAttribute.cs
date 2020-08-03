using System;
using System.Web.Mvc;
using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.Core.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Identity.HtmlHelper;
using JieNor.Megi.ServiceContract.SEC;

namespace JieNor.Megi.Identity.Attribute
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
	public class PermissionAttribute : ActionFilterAttribute
	{
		public PermissionAttribute(string bizObjectKey, string permissionItem, string orgId = "")
		{
			this.bizObjectKey = bizObjectKey;
			this.permissionItem = permissionItem;
			this.orgId = orgId;
		}

		public PermissionAttribute(string bizObjectKey, string permissionItem, string tips, string orgId = "")
		{
			this.bizObjectKey = bizObjectKey;
			this.permissionItem = permissionItem;
			this.tips = tips;
			this.orgId = orgId;
		}

		public PermissionAttribute(string bizObjectKey, string permissionItem, OperateTypeEnum op, string bizObjectKey1, string permissionItem1, string orgId = "")
		{
			this.bizObjectKey = bizObjectKey;
			this.permissionItem = permissionItem;
			this.op = op;
			this.bizObjectKey1 = bizObjectKey1;
			this.permissionItem1 = permissionItem1;
			this.orgId = orgId;
		}

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);
			bool flag = this.HavePermission();
			bool flag2 = !flag;
			if (flag2)
			{
				bool flag3 = AjaxRequestExtensions.IsAjaxRequest(filterContext.HttpContext.Request);
				if (flag3)
				{
					OperationResult operationResult = new OperationResult();
					operationResult.VerificationInfor.Add(new BizVerificationInfor
					{
						Level = AlertEnum.Error,
						Message = this.tips
					});
					filterContext.Result = new JsonResult
					{
						Data = operationResult
					};
				}
				else
				{
					filterContext.Result = new ContentResult
					{
						Content = this.tips
					};
					string text = "http://";
					bool flag4 = filterContext.HttpContext.Request.Url.ToString().StartsWith("https");
					if (flag4)
					{
						text = "https://";
					}
					string url = string.Format("{0}{1}/BD/{2}/{3}", new object[]
					{
						text,
						filterContext.HttpContext.Request.Url.Host,
						"User",
						"NotAllowAccess"
					});
					filterContext.HttpContext.Response.Redirect(url);
				}
			}
		}

		private bool HavePermission()
		{
			ISECPermission sysService = ServiceHostManager.GetSysService<ISECPermission>();
			bool result;
			using (sysService as IDisposable)
			{
				bool flag = string.IsNullOrWhiteSpace(this.permissionItem1);
				if (flag)
				{
					result = sysService.HavePermission(this.bizObjectKey, this.permissionItem, this.orgId, null).ResultData;
				}
				else
				{
					bool resultData = sysService.HavePermission(this.bizObjectKey, this.permissionItem, this.orgId, null).ResultData;
					bool resultData2 = sysService.HavePermission(this.bizObjectKey1, this.permissionItem1, this.orgId, null).ResultData;
					result = ((this.op == OperateTypeEnum.AND) ? (resultData && resultData2) : (resultData || resultData2));
				}
			}
			return result;
		}

		private string bizObjectKey = "";

		private string permissionItem = "";

		private string bizObjectKey1 = "";

		private string permissionItem1 = "";
		private OperateTypeEnum op = OperateTypeEnum.AND;
		private string orgId = "";
		private string tips = HtmlLang.GetText(LangModule.BD, "YouNoHavePermissions", "You don't have appropriate permissions to operate !");
	}
}
