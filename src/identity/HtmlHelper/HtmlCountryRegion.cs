using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Identity.AutoManager;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

namespace JieNor.Megi.Identity.HtmlHelper
{
	public static class HtmlCountryRegion
	{
		public static MvcHtmlString SelectOptions()
		{
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Expected O, but got Unknown
			List<BASCountryRegionModel> listByWhere = BASCountryRegionManager.GetListByWhere(new SqlWhere());
			StringBuilder stringBuilder = new StringBuilder();
			if (listByWhere != null)
			{
				foreach (BASCountryRegionModel item in listByWhere)
				{
					foreach (MultiLanguageFieldList item2 in item.MultiLanguage)
					{
						if (item2.MFieldName.EqualsIgnoreCase("MName"))
						{
							stringBuilder.Append($"<option value=\"{item.MItemID}\">{item2.MMultiLanguageValue}</option>");
						}
					}
				}
			}
			return new MvcHtmlString(stringBuilder.ToString());
		}
	}
}
