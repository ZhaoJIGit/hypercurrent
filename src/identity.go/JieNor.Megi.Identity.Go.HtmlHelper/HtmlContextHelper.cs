using Fasterflect;
using JieNor.Megi.Common.Context;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace JieNor.Megi.Identity.Go.HtmlHelper
{
	public static class HtmlContextHelper
	{
		public static MvcHtmlString ContextHtml()
		{
			MContext mContext = ContextHelper.MContext;
			List<string> list = new List<string>
			{
				"MVoucherNumberFilledChar",
				"MVoucherNumberLength",
				"MGLBeginDate",
				"MFABeginDate",
				"MBeginDate",
				"MOrgVersionID"
			};
			string text = string.Empty;
			for (int i = 0; i < list.Count; i++)
			{
				object obj = mContext.GetPropertyValue(list[i]);
				if (obj != null && obj.GetType() == typeof(DateTime))
				{
					obj = ((DateTime)obj).ToString("yyyy-MM-dd");
				}
				text = text + " <input type='hidden' context='1' name='" + list[i] + "' value='" + ((obj == null) ? "" : obj.ToString()) + "'>";
			}
			return new MvcHtmlString(text);
		}
	}
}
