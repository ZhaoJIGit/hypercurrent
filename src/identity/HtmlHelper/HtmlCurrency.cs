using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.Identity.AutoManager;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

namespace JieNor.Megi.Identity.HtmlHelper
{
	public static class HtmlCurrency
	{
		public static MvcHtmlString SelectOptions()
		{
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Expected O, but got Unknown
			List<BASCurrencyViewModel> viewList = BASCurrencyManager.GetViewList();
			StringBuilder stringBuilder = new StringBuilder();
			if (viewList != null)
			{
				foreach (BASCurrencyViewModel item in viewList)
				{
					stringBuilder.Append($"<option value=\"{item.MCurrencyID}\">{item.MLocalName}</option>");
				}
			}
			return new MvcHtmlString(stringBuilder.ToString());
		}
	}
}
