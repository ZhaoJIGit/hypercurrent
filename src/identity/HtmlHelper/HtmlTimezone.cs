using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.Identity.AutoManager;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace JieNor.Megi.Identity.HtmlHelper
{
	public static class HtmlTimezone
	{
		public static MvcHtmlString SelectOptions()
		{
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Expected O, but got Unknown
			List<BASTimezoneModel> list = BASTimezoneManager.GetList();
			StringBuilder stringBuilder = new StringBuilder();
			if (list != null)
			{
				//from x in list
				//orderby x.MName
				//select x;

				//list = list.OrderBy(x => x.MName).ToList();

				foreach (BASTimezoneModel item in list.OrderBy(x => x.MName))
				{
					stringBuilder.Append($"<option value=\"{item.MName}\">{item.MLocalName}</option>");
				}
			}
			return new MvcHtmlString(stringBuilder.ToString());
		}
	}
}
