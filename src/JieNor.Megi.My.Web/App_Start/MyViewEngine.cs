using System.Web.Mvc;

namespace JieNor.Megi.My.Web.App_Start
{
	public sealed class MyViewEngine : RazorViewEngine
	{
		private static readonly string[] _MasterLocationFormats = new string[9]
		{
			"~/Views/{0}.cshtml",
			"~/Views/{1}/{0}.cshtml",
			"~/Views/Shared/{0}.cshtml",
			"~/Views/Shared/{0}.cshtml",
			"~/Areas/Views/{1}/{0}.cshtml",
			"~/Areas/FW/Views/{1}/{0}.cshtml",
			"~/Areas/Org/Views/{1}/{0}.cshtml",
			"~/Areas/Org/Views/{0}.cshtml",
			"~/Areas/MSg/Views/{1}/{0}.cshtml"
		};

		private static readonly string[] _ViewLocationFormats = new string[17]
		{
			"~/Views/{0}.cshtml",
			"~/Views/{1}/{0}.cshtml",
			"~/Views/Shared/{0}.cshtml",
			"~/Areas/Views/{1}/{0}.cshtml",
			"~/Areas/FW/Views/{1}/{0}.cshtml",
			"~/Areas/Org/Views/{1}/{0}.cshtml",
			"~/Areas/MSg/Views/{1}/{0}.cshtml",
			"~/Areas/Org/Views/{0}.cshtml",
			"~/Areas/Adviser/Views/Shared/{0}.cshtml",
			"~/Areas/Bank/Views/Shared/{0}.cshtml",
			"~/Areas/BD/Views/Shared/{0}.cshtml",
			"~/Areas/Chart/Views/Shared/{0}.cshtml",
			"~/Areas/CN/Views/Shared/{0}.cshtml",
			"~/Areas/FW/Views/Shared/{0}.cshtml",
			"~/Areas/IV/Views/Shared/{0}.cshtml",
			"~/Areas/Report/Views/Shared/{0}.cshtml",
			"~/Areas/Setting/Views/Shared/{0}.cshtml"
		};

		private static readonly string[] _PartialViewLocationFormats = new string[17]
		{
			"~/Views/{0}.cshtml",
			"~/Views/{1}/{0}.cshtml",
			"~/Views/Shared/{0}.cshtml",
			"~/Areas/Views/{1}/{0}.cshtml",
			"~/Areas/FW/Views/{1}/{0}.cshtml",
			"~/Areas/Org/Views/{0}.cshtml",
			"~/Areas/Org/Views/{1}/{0}.cshtml",
			"~/Areas/MSg/Views/{1}/{0}.cshtml",
			"~/Areas/Adviser/Views/Shared/{0}.cshtml",
			"~/Areas/Bank/Views/Shared/{0}.cshtml",
			"~/Areas/BD/Views/Shared/{0}.cshtml",
			"~/Areas/Chart/Views/Shared/{0}.cshtml",
			"~/Areas/CN/Views/Shared/{0}.cshtml",
			"~/Areas/FW/Views/Shared/{0}.cshtml",
			"~/Areas/IV/Views/Shared/{0}.cshtml",
			"~/Areas/Report/Views/Shared/{0}.cshtml",
			"~/Areas/Setting/Views/Shared/{0}.cshtml"
		};

		protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
		{
			return base.CreatePartialView(controllerContext, partialPath);
		}

		protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
		{
			return base.CreateView(controllerContext, viewPath, masterPath);
		}

		protected override bool FileExists(ControllerContext controllerContext, string virtualPath)
		{
			return base.FileExists(controllerContext, virtualPath);
		}

		public override void ReleaseView(ControllerContext controllerContext, IView view)
		{
			base.ReleaseView(controllerContext, view);
		}

		public override ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
		{
			base.MasterLocationFormats = _MasterLocationFormats;
			base.ViewLocationFormats = _ViewLocationFormats;
			base.PartialViewLocationFormats = _PartialViewLocationFormats;
			return base.FindPartialView(controllerContext, partialViewName, useCache);
		}

		public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
		{
			base.MasterLocationFormats = _MasterLocationFormats;
			base.ViewLocationFormats = _ViewLocationFormats;
			base.PartialViewLocationFormats = _PartialViewLocationFormats;
			return base.FindView(controllerContext, viewName, masterName, useCache);
		}
	}
}
