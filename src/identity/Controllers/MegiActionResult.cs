using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Web.Mvc;
using Microsoft.CSharp.RuntimeBinder;

namespace JieNor.Megi.Identity.Controllers
{
	// Token: 0x02000017 RID: 23
	public class MegiActionResult : ViewResult
	{
		// Token: 0x0600006E RID: 110 RVA: 0x000050EE File Offset: 0x000032EE
		public MegiActionResult()
		{
		}

		// Token: 0x0600006F RID: 111 RVA: 0x000050F8 File Offset: 0x000032F8
		public MegiActionResult(string _ViewName)
		{
			base.ViewName = _ViewName;
		}

		// Token: 0x06000070 RID: 112 RVA: 0x0000510A File Offset: 0x0000330A
		public MegiActionResult(ViewDataDictionary _ViewData)
		{
			base.ViewData = _ViewData;
		}

		// Token: 0x06000071 RID: 113 RVA: 0x0000511C File Offset: 0x0000331C
		public MegiActionResult(TempDataDictionary _TempData)
		{
			base.TempData = _TempData;
		}

		// Token: 0x06000072 RID: 114 RVA: 0x0000512E File Offset: 0x0000332E
		public MegiActionResult(ViewDataDictionary _ViewData, TempDataDictionary _TempData)
		{
			base.ViewData = _ViewData;
			base.TempData = _TempData;
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00005148 File Offset: 0x00003348
		public MegiActionResult(string _ViewName, ViewDataDictionary _ViewData)
		{
			base.ViewName = _ViewName;
			base.ViewData = _ViewData;
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00005162 File Offset: 0x00003362
		public MegiActionResult(string _ViewName, TempDataDictionary _TempData)
		{
			base.ViewName = _ViewName;
			base.TempData = _TempData;
		}

		// Token: 0x06000075 RID: 117 RVA: 0x0000517C File Offset: 0x0000337C
		public MegiActionResult(string _ViewName, ViewDataDictionary _ViewData, TempDataDictionary _TempData)
		{
			base.ViewName = _ViewName;
			base.ViewData = _ViewData;
			base.TempData = _TempData;
		}

		// Token: 0x06000076 RID: 118 RVA: 0x000051A0 File Offset: 0x000033A0
		public override void ExecuteResult(ControllerContext context)
		{
			string authority = context.RequestContext.HttpContext.Request.Url.Authority;
			string text = context.RequestContext.RouteData.Values["controller"].ToString();
			string text2 = context.RequestContext.RouteData.Values["action"].ToString();
			object obj = context.RequestContext.RouteData.DataTokens["area"];
			string text3 = (obj == null) ? "" : string.Format("/{0}", obj.ToString());
			this.MMegiUrl = string.Format("{0}{1}/{2}/{3}", new object[]
			{
				authority,
				text3,
				text,
				text2
			});
			//if (MegiActionResult.<> o__8.<> p__0 == null)
			//{
			//	MegiActionResult.<> o__8.<> p__0 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "MMegiUrl", typeof(MegiActionResult), new CSharpArgumentInfo[]
			//	{
			//		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
			//		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
			//	}));
			//}
			//MegiActionResult.<> o__8.<> p__0.Target(MegiActionResult.<> o__8.<> p__0, base.ViewBag, this.MMegiUrl);


			ViewBag.MMegiUrl = this.MMegiUrl;


			bool flag = context == null;
			if (flag)
			{
				throw new ArgumentNullException("context");
			}
			bool flag2 = string.IsNullOrEmpty(base.ViewName);
			if (flag2)
			{
				base.ViewName = context.RouteData.Values["action"].ToString();
			}
			ViewEngineResult viewEngineResult = null;
			bool flag3 = base.View == null;
			if (flag3)
			{
				viewEngineResult = this.FindView(context);
				base.View = viewEngineResult.View;
			}
			TextWriter output = context.HttpContext.Response.Output;
			ViewContext viewContext = new ViewContext(context, base.View, base.ViewData, base.TempData, output);
			base.View.Render(viewContext, output);
			bool flag4 = viewEngineResult != null;
			if (flag4)
			{
				viewEngineResult.ViewEngine.ReleaseView(context, base.View);
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000077 RID: 119 RVA: 0x0000539F File Offset: 0x0000359F
		// (set) Token: 0x06000078 RID: 120 RVA: 0x000053A7 File Offset: 0x000035A7
		private string MMegiUrl { get; set; }
	}
}
