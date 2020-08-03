using System;
using System.Text;
using System.Web.Mvc;

namespace JieNor.Megi.Identity.HtmlHelper
{
	public static class HtmlYear
	{
		public static MvcHtmlString SelectOptions(int fromYear, int toYear, int selectedYear)
		{
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Expected O, but got Unknown
			StringBuilder stringBuilder = new StringBuilder();
			if (fromYear > toYear)
			{
				for (int num = fromYear; num >= toYear; num--)
				{
					stringBuilder.Append(string.Format("<option value='{0}'{1}>{0}</option>", num, (selectedYear == num) ? " selected='selected'" : ""));
				}
			}
			else
			{
				for (int i = fromYear; i <= toYear; i++)
				{
					stringBuilder.Append(string.Format("<option value='{0}'{1}>{0}</option>", i, (selectedYear == i) ? " selected='selected'" : ""));
				}
			}
			return new MvcHtmlString(stringBuilder.ToString());
		}

		public static MvcHtmlString SelectOptions()
		{
			int year = DateTime.Now.Year;
			return SelectOptions(year + 1, 2006, year);
		}
	}
}
