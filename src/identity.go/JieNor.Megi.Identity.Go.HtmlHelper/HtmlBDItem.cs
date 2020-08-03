using JieNor.Megi.DataModel.BD;
using JieNor.Megi.Identity.Go.AutoManager;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

namespace JieNor.Megi.Identity.Go.HtmlHelper
{
	public static class HtmlBDItem
	{
		public static MvcHtmlString SelectOptions()
		{
			List<BDItemModel> listByWhere = BDItemManager.GetListByWhere("");
			StringBuilder stringBuilder = new StringBuilder();
			if (listByWhere != null)
			{
				foreach (BDItemModel item in listByWhere)
				{
					stringBuilder.Append($"<option value=\"{item.MItemID}\">{item.MNumber}</option>");
				}
			}
			return new MvcHtmlString(stringBuilder.ToString());
		}
	}
}
