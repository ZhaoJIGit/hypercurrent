using JieNor.Megi.DataModel.BAS;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace JieNor.Megi.Identity.HtmlHelper
{
	public static class MenuHtmlHepler
	{
		public static MvcHtmlString GetMenuHtml(List<BASMenuModel> list)
		{
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Expected O, but got Unknown
			List<BASMenuModel> list2 = (from t in list
			where t.MParentCode == ""
			orderby t.MSequence
			select t).ToList();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<ul class=\"mg-menu-m-container\">");
			foreach (BASMenuModel item in list2)
			{
				stringBuilder.Append("<li class=\"mg-menu-m\">");
				stringBuilder.Append($"<a href=\"{item.MUrl}\" class=\"mg-menu-m-name\" data-module=\"{item.MModuleCode}\">{item.MName}</a>");
				List<BASMenuModel> list3 = (from t in list
				where t.MParentCode == item.MCode
				select t).ToList();
				if (list3 != null && list3.Count > 0)
				{
					stringBuilder.Append("<ul class=\"mg-menu-s\">");
					foreach (BASMenuModel item2 in list3)
					{
						stringBuilder.Append($"<li><a href=\"{item2.MUrl}\" >{item2.MName}</a></li>");
					}
					stringBuilder.Append("</ul>");
				}
				stringBuilder.Append("</li>");
			}
			stringBuilder.Append("</ul>");
			return new MvcHtmlString(stringBuilder.ToString());
		}

		public static MvcHtmlString GetCrumbHtml(BASMenuModel currentMenu, List<BASMenuModel> menuList, string currentName)
		{
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Expected O, but got Unknown
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Expected O, but got Unknown
			string[] arr = currentMenu.MPath.Split(',');
			if (arr.Length == 1)
			{
				return new MvcHtmlString($"<span>{currentMenu.MName}</span>");
			}
			StringBuilder stringBuilder = new StringBuilder();
			int i;
			for (i = 0; i < arr.Length; i++)
			{
				BASMenuModel bASMenuModel = (from t in menuList
				where t.MCode == arr[i]
				select t).FirstOrDefault();
				stringBuilder.Append($"<a href='{bASMenuModel.MUrl}'>{bASMenuModel.MName}</a>");
			}
			stringBuilder.Append($"<span>{(string.IsNullOrEmpty(currentName) ? currentMenu.MName : currentName)}</span>");
			return new MvcHtmlString(stringBuilder.ToString());
		}
	}
}
