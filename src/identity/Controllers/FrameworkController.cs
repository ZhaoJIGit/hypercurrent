using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.Mongo.Service;
using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Identity.HtmlHelper;
using JieNor.Megi.ServiceContract.BAS;
using Microsoft.CSharp.RuntimeBinder;

namespace JieNor.Megi.Identity.Controllers
{
	public class FrameworkController : Controller
	{
		protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
		{
			return new JsonResult
			{
				Data = data,
				ContentType = contentType,
				ContentEncoding = contentEncoding,
				JsonRequestBehavior = behavior,
				MaxJsonLength = new int?(int.MaxValue)
			};
		}

		protected new JsonResult Json(object data, JsonRequestBehavior behavior)
		{
			bool flag = data != null;
			if (flag)
			{
				bool flag2 = data.GetType().ToString().StartsWith("JieNor.Megi.EntityModel.Context.MActionResult");
				if (flag2)
				{
					MethodInfo method = data.GetType().GetMethod("ToJsonResult");
					bool flag3 = method != null;
					if (flag3)
					{
						data = method.Invoke(data, null);
					}
				}
				else
				{
					data = new MJsonResult
					{
						Data = data
					};
				}
			}
			return new CustomJson
			{
				Data = data,
				JsonRequestBehavior = behavior
			};
		}

		protected new JsonResult Json(object data)
		{
			return Json(data, JsonRequestBehavior.DenyGet);
		}

		protected void SetModule(string module)
		{
			base.ViewData["Module"] = module;
		}

		protected string LoginUserName
		{
			get
			{
				MContext mcontext = ContextHelper.MContext;
				bool flag = mcontext == null;
				string result;
				if (flag)
				{
					result = string.Empty;
				}
				else
				{
					bool flag2 = mcontext.MLCID == LangCodeEnum.EN_US;
					if (flag2)
					{
						result = string.Format("{0} {1}", mcontext.MFirstName, mcontext.MLastName);
					}
					else
					{
						result = string.Format("{0}{1}", mcontext.MLastName, mcontext.MFirstName);
					}
				}
				return result;
			}
		}

		protected void SetTitle(string title)
		{
			//if (FrameworkController.<> o__7.<> p__0 == null)
			//{
			//	FrameworkController.<> o__7.<> p__0 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.SetMember(CSharpBinderFlags.None, "Title", typeof(FrameworkController), new CSharpArgumentInfo[]
			//	{
			//		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
			//		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
			//	}));
			//}
			//FrameworkController.<> o__7.<> p__0.Target(FrameworkController.<> o__7.<> p__0, base.ViewBag, string.Format("{0} - Megi", title));

			ViewBag.Title = string.Format("{0} - Hypercurrent", title);

			//if (FrameworkController.<> o__7.<> p__1 == null)
			//{
			//	FrameworkController.<> o__7.<> p__1 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.SetMember(CSharpBinderFlags.None, "Position", typeof(FrameworkController), new CSharpArgumentInfo[]
			//	{
			//		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
			//		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
			//	}));
			//}
			//FrameworkController.<> o__7.<> p__1.Target(FrameworkController.<> o__7.<> p__1, base.ViewBag, string.Format("<lable>{0}</lable>", title));

			ViewBag.Position = string.Format("<lable>{0}</lable>", title);
		}

		protected void SetTitleAndCrumb(string title, string crumbName)
		{
			string text = string.Empty;
			bool flag = !string.IsNullOrEmpty(title);
			if (flag)
			{
				text = Regex.Replace(title, "<(.[^>]*)>", "", RegexOptions.IgnoreCase);
			}
			//if (FrameworkController.<> o__8.<> p__0 == null)
			//{
			//	FrameworkController.<> o__8.<> p__0 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.SetMember(CSharpBinderFlags.None, "Title", typeof(FrameworkController), new CSharpArgumentInfo[]
			//	{
			//		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
			//		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
			//	}));
			//}
			//FrameworkController.<> o__8.<> p__0.Target(FrameworkController.<> o__8.<> p__0, base.ViewBag, string.Format("{0} - Megi", text));

			ViewBag.Title = string.Format("{0} - Megi", text);


			//if (FrameworkController.<> o__8.<> p__1 == null)
			//{
			//	FrameworkController.<> o__8.<> p__1 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.SetMember(CSharpBinderFlags.None, "Position", typeof(FrameworkController), new CSharpArgumentInfo[]
			//	{
			//		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
			//		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
			//	}));
			//}
			//FrameworkController.<> o__8.<> p__1.Target(FrameworkController.<> o__8.<> p__1, base.ViewBag, string.Format("{0}<br/><span>{1}</span>", crumbName, text));

			ViewBag.Position = string.Format("{0}<br/><span>{1}</span>", crumbName, text);
		}

		protected ActionResult MView()
		{
			return new MegiActionResult(base.ViewData);
		}

		protected IMongoMContext MongoMContextService
		{
			get
			{
				return ServiceHostManager.GetMongoService<IMongoMContext>();
			}
			private set
			{
			}
		}

		protected MContext MContext
		{
			get
			{
				return ContextHelper.MContext;
			}
			set
			{
				bool flag = value != null;
				if (flag)
				{
					ContextHelper.MContext = value;
				}
			}
		}

		protected string MMegiUrl
		{
			get
			{
				return string.Concat(new string[]
				{
					base.HttpContext.Request.Url.Authority,
					"/",
					base.RouteData.Values["controller"].ToString(),
					"/",
					base.RouteData.Values["action"].ToString()
				});
			}
		}

		public JsonResult ChangeLang(string langId)
		{
			LangIndentity.ChangeLang(langId);
			return this.Json(true);
		}

		public JsonResult UpdateLang(LangModule module, string key, string defaultValue)
		{
			IBASLang sysService = ServiceHostManager.GetSysService<IBASLang>();
			using (sysService as IDisposable)
			{
				sysService.UpdateScriptLang(module, key, defaultValue, null);
			}
			return this.Json(true);
		}

		public int GetReceiveMessageCount()
		{
			return HtmlMessage.GetReceiveMessageCount();
		}

		public ActionResult NotFound(string statusCode)
		{
			int statusCode2 = 404;
			int num = 0;
			bool flag = int.TryParse(statusCode, out num);
			if (flag)
			{
				statusCode2 = num;
			}
			base.Response.StatusCode = statusCode2;
			return base.View("~\\Views\\CustomError\\NotFound.cshtml");
		}

		public ActionResult Error(string statusCode)
		{
			int statusCode2 = 500;
			int num = 0;
			bool flag = int.TryParse(statusCode, out num);
			if (flag)
			{
				statusCode2 = num;
			}
			base.Response.StatusCode = statusCode2;
			return base.View("~\\Views\\CustomError\\Error.cshtml");
		}
	}
}
