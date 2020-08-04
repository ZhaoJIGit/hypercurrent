using JieNor.Megi.Common.Context;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.Identity.Go.AutoManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace JieNor.Megi.Identity.Go.HtmlHelper
{
	public class HtmlAccount
	{
		public static MvcHtmlString AccountPeriodOptions(bool isFullPeriod = false)
		{
			List<DateTime> list = isFullPeriod ? GLSettlementManager.GetFullPeriod() : GLSettlementManager.GetSettledPeriodFromBeginDate();
			StringBuilder stringBuilder = new StringBuilder();
			if (list != null)
			{
				list = (from x in list
				orderby x.Date descending
				select x).ToList();
				MContext mContext = ContextHelper.MContext;
				foreach (DateTime item in list)
				{
					if (mContext.DateNow.ToString("yyyy-MM") == item.ToString("yyyy-MM"))
					{
						stringBuilder.Append(string.Format("<option value=\"{0}\" selected>{1}</option>", item.ToString("yyyyMM"), item.ToString("yyyy-MM")));
					}
					else
					{
						stringBuilder.Append(string.Format("<option value=\"{0}\">{1}</option>", item.ToString("yyyyMM"), item.ToString("yyyy-MM")));
					}
				}
			}
			return new MvcHtmlString(stringBuilder.ToString());
		}
	}
}
