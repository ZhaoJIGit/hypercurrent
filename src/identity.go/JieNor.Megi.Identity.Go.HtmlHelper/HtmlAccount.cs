using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.Logger;
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
			MLogger.Log("AccountPeriodOptions");
			List<DateTime> list = isFullPeriod ? GLSettlementManager.GetFullPeriod() : GLSettlementManager.GetSettledPeriodFromBeginDate();
			StringBuilder stringBuilder = new StringBuilder();
			MContext mContext = ContextHelper.MContext;

			if (list != null)
			{
				MLogger.Log("list::");

				list = (from x in list
						orderby x.Date descending
						select x).ToList();
				foreach (DateTime item in list)
				{
					MLogger.Log("DateNow::" + mContext.DateNow);

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
			else
			{
				//var i = mContext.MGLBeginDate;
				//while (mContext.MGLBeginDate.Year * 100 + mContext.MGLBeginDate.Month <= mContext.MGLBeginDate.Year * 100 + dateTime.Month)
				//{
				//	list.Add(i);
				//	i = i.AddMonths(1);
				//}
			}
			MLogger.Log("stringBuilder::" + stringBuilder.ToString());

			return new MvcHtmlString(stringBuilder.ToString());
		}
	}
}
