using JieNor.Megi.Core;
using System.Web;
using System.Web.Mvc;

namespace JieNor.Megi.Tools.Resubmit
{
	public static class MHtmlHelper
	{
		public static HtmlString GenerateVerficationToken()
		{
			string guid = UUIDHelper.GetGuid();
			string guid2 = UUIDHelper.GetGuid();
			HttpContext.Current.Session[guid2] = guid;
			TagBuilder tagBuilder = new TagBuilder("input");
			tagBuilder.Attributes["type"] = "hidden";
			tagBuilder.Attributes["name"] = PageTokenViewBase.HiddenTokenName;
			tagBuilder.Attributes["value"] = guid;
			tagBuilder.Attributes["pageid"] = guid2;
			return new HtmlString(tagBuilder.ToString(TagRenderMode.SelfClosing));
		}
	}
}
