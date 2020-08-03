using JieNor.Megi.Common.Context;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.Identity.Go.AutoManager;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace JieNor.Megi.Identity.Go.HtmlHelper
{
	public class HtmlGLSettlement
	{
		public static MvcHtmlString FromMonth()
		{
			List<DateTime> settledPeriodFromBeginDate = GLSettlementManager.GetSettledPeriodFromBeginDate();
			if (settledPeriodFromBeginDate != null && settledPeriodFromBeginDate.Count != 0)
			{
				settledPeriodFromBeginDate = (from t in settledPeriodFromBeginDate
				orderby t descending
				select t).ToList();
				string multiLangKey = GetMultiLangKey();
				StringBuilder stringBuilder = new StringBuilder();
				foreach (DateTime item in settledPeriodFromBeginDate)
				{
					stringBuilder.AppendFormat("<option value='{0}'>{1}</option>", item.ToString("yyyy-MM-dd 00:00:00"), item.ToString("MMMM yyyy", new CultureInfo($"{multiLangKey}")));
				}
				return new MvcHtmlString(stringBuilder.ToString());
			}
			return new MvcHtmlString("");
		}

		public static MvcHtmlString ToMonth()
		{
			List<DateTime> settledPeriodFromBeginDate = GLSettlementManager.GetSettledPeriodFromBeginDate();
			if (settledPeriodFromBeginDate != null && settledPeriodFromBeginDate.Count != 0)
			{
				settledPeriodFromBeginDate = (from t in settledPeriodFromBeginDate
				orderby t descending
				select t).ToList();
				string multiLangKey = GetMultiLangKey();
				StringBuilder stringBuilder = new StringBuilder();
				foreach (DateTime item in settledPeriodFromBeginDate)
				{
					stringBuilder.AppendFormat("<option value='{0}'>{1}</option>", item.ToString("yyyy-MM-dd 23:59:59"), item.ToString("MMMM yyyy", new CultureInfo($"{multiLangKey}")));
				}
				return new MvcHtmlString(stringBuilder.ToString());
			}
			return new MvcHtmlString("");
		}

		private static string GetMultiLangKey()
		{
			string empty = string.Empty;
			MContext mContext = ContextHelper.MContext;
			if (mContext.MLCID == LangCodeEnum.ZH_CN)
			{
				return "zh-cn";
			}
			if (mContext.MLCID == LangCodeEnum.ZH_TW)
			{
				return "zh-tw";
			}
			return "en-us";
		}

		public static MvcHtmlString BeginYearOptions()
		{
			int year = ContextHelper.MContext.MBeginDate.Year;
			int num = year + 10;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(string.Format("<option value='{0}' selected='selected'>{0}</option>", year));
			for (int i = year + 1; i <= num; i++)
			{
				stringBuilder.Append(string.Format("<option value='{0}'>{0}</option>", i));
			}
			return new MvcHtmlString(stringBuilder.ToString());
		}
	}
}
