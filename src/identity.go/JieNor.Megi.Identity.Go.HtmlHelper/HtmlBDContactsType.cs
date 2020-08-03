using JieNor.Megi.Common.Utility;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.Identity.Go.AutoManager;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

namespace JieNor.Megi.Identity.Go.HtmlHelper
{
	public static class HtmlBDContactsType
	{
		public static MvcHtmlString ContactsTypeOptions()
		{
			List<BDContactsTypeLModel> typeListByWhere = BDContactsTypeManager.GetTypeListByWhere(false);
			StringBuilder stringBuilder = new StringBuilder();
			if (typeListByWhere != null)
			{
				foreach (BDContactsTypeLModel item in typeListByWhere)
				{
					stringBuilder.Append($"<option value=\"{item.MParentID}\">{MText.Encode(item.MName)}</option>");
				}
			}
			return new MvcHtmlString(stringBuilder.ToString());
		}
	}
}
