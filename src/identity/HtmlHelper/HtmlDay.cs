using System.Text;
using System.Web.Mvc;

namespace JieNor.Megi.Identity.HtmlHelper
{
	public static class HtmlDay
	{
		public static MvcHtmlString SelectOptions(int count)
		{
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Expected O, but got Unknown
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < count; i++)
			{
				if (i == 0)
				{
					stringBuilder.AppendFormat("<option value='{0}'>{1}</option>", i + 1, i + 1);
				}
				else
				{
					stringBuilder.AppendFormat("<option value='{0}' selected>{1}</option>", i + 1, i + 1);
				}
			}
			return new MvcHtmlString(stringBuilder.ToString());
		}
	}
}
