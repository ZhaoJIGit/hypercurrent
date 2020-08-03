using JieNor.Megi.Common.Context;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web.Mvc;

namespace JieNor.Megi.Identity.HtmlHelper
{
	public static class HtmlDateTime
	{
		public static MvcHtmlString FullYearMonthDesc(DateTime dateFrom, int count, bool fromBeginDate = false)
		{
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Expected O, but got Unknown
			StringBuilder stringBuilder = new StringBuilder();
			string multiLangKey = GetMultiLangKey();
			DateTime now = DateTime.Now;
			int year = now.Year;
			now = DateTime.Now;
			int month = now.Month;
			DateTime mGLBeginDate = ContextHelper.MContext.MGLBeginDate;
			if (fromBeginDate)
			{
				count = (dateFrom.Year - mGLBeginDate.Year) * 12 + 12;
			}
			for (int i = 0; i < count; i++)
			{
				DateTime dateTime = dateFrom.AddMonths(-i);
				dateTime = new DateTime(dateTime.Year, dateTime.Month, 1);
				if (fromBeginDate && dateTime < mGLBeginDate)
				{
					break;
				}
				if (dateTime.Year == year && dateTime.Month == month)
				{
					stringBuilder.AppendFormat("<option value='{0}' selected='selected'>{1}</option>", dateTime.ToString("yyyy-MM-dd"), dateTime.ToString("MMMM yyyy", new CultureInfo($"{multiLangKey}")));
				}
				else
				{
					stringBuilder.AppendFormat("<option value='{0}'>{1}</option>", dateTime.ToDateString(), dateTime.ToString("MMMM yyyy", new CultureInfo($"{multiLangKey}")));
				}
			}
			return new MvcHtmlString(stringBuilder.ToString());
		}

		public static MvcHtmlString GetSelectFullYearMonthOptions(int forwardMonths = 3)
		{
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Expected O, but got Unknown
			string multiLangKey = GetMultiLangKey();
			DateTime now = DateTime.Now;
			int year = now.Year;
			now = DateTime.Now;
			int month = now.Month;
			DateTime mGLBeginDate = ContextHelper.MContext.MBeginDate;
			now = DateTime.Now;
			DateTime t = now.AddMonths(forwardMonths);
			StringBuilder stringBuilder = new StringBuilder();
			while (t >= mGLBeginDate)
			{
				DateTime date = new DateTime(t.Year, t.Month, 1);
				if (date.Year == year && date.Month == month)
				{
					stringBuilder.AppendFormat("<option value='{0}' selected='selected'>{1}</option>", date.ToString("yyyy-MM-dd"), date.ToString("MMMM yyyy", new CultureInfo($"{multiLangKey}")));
				}
				else
				{
					stringBuilder.AppendFormat("<option value='{0}'>{1}</option>", date.ToDateString(), date.ToString("MMMM yyyy", new CultureInfo($"{multiLangKey}")));
				}
				t = t.AddMonths(-1);
			}
			return new MvcHtmlString(stringBuilder.ToString());
		}

		public static MvcHtmlString FromMonth(List<DateTime> list)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Expected O, but got Unknown
			if (list == null || list.Count == 0)
			{
				return new MvcHtmlString("");
			}
			int count = list.Count;
			return FromMonth(list[count - 1], count);
		}

		public static MvcHtmlString FromMonth(DateTime dateFrom, int count)
		{
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Expected O, but got Unknown
			StringBuilder stringBuilder = new StringBuilder();
			string multiLangKey = GetMultiLangKey();
			DateTime now = DateTime.Now;
			int year = now.Year;
			now = DateTime.Now;
			int month = now.Month;
			for (int i = 0; i < count; i++)
			{
				DateTime dateTime = dateFrom.AddMonths(-i);
				dateTime = new DateTime(dateTime.Year, dateTime.Month, 1);
				if (dateTime.Year == year && dateTime.Month == month)
				{
					stringBuilder.AppendFormat("<option value='{0}' selected='selected'>{1}</option>", dateTime.ToString("yyyy-MM-01 00:00:00"), dateTime.ToString("MMMM yyyy", new CultureInfo($"{multiLangKey}")));
				}
				else
				{
					stringBuilder.AppendFormat("<option value='{0}'>{1}</option>", dateTime.ToString("yyyy-MM-01"), dateTime.ToString("MMMM yyyy", new CultureInfo($"{multiLangKey}")));
				}
			}
			return new MvcHtmlString(stringBuilder.ToString());
		}

		public static MvcHtmlString ToMonth(DateTime dateFrom, int count)
		{
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Expected O, but got Unknown
			StringBuilder stringBuilder = new StringBuilder();
			string multiLangKey = GetMultiLangKey();
			DateTime dateTime = DateTime.Now;
			int year = dateTime.Year;
			dateTime = DateTime.Now;
			int month = dateTime.Month;
			for (int i = 0; i < count; i++)
			{
				DateTime dateTime2 = dateFrom.AddMonths(-i);
				dateTime2 = new DateTime(dateTime2.Year, dateTime2.Month, 1);
				dateTime = dateTime2.AddMonths(1);
				dateTime2 = dateTime.AddSeconds(-1.0);
				if (dateTime2.Year == year && dateTime2.Month == month)
				{
					stringBuilder.AppendFormat("<option value='{0}' selected='selected'>{1}</option>", dateTime2.ToString("yyyy-MM-dd HH:mm:ss"), dateTime2.ToString("MMMM yyyy", new CultureInfo($"{multiLangKey}")));
				}
				else
				{
					stringBuilder.AppendFormat("<option value='{0}'>{1}</option>", dateTime2.ToString("yyyy-MM-dd HH:mm:ss"), dateTime2.ToString("MMMM yyyy", new CultureInfo($"{multiLangKey}")));
				}
			}
			return new MvcHtmlString(stringBuilder.ToString());
		}

		public static MvcHtmlString FullQuarterly(DateTime dateFrom, int count)
		{
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Expected O, but got Unknown
			StringBuilder stringBuilder = new StringBuilder();
			string multiLangKey = GetMultiLangKey();
			DateTime now = DateTime.Now;
			int year = now.Year;
			now = DateTime.Now;
			int month = now.Month;
			for (int i = 0; i < count; i++)
			{
				DateTime dateTime = dateFrom.AddMonths(-i * 3);
				dateTime = new DateTime(dateTime.Year, dateTime.Month, 1);
				switch ((dateTime.Month % 3 == 0) ? (dateTime.Month / 3) : (dateTime.Month / 3 + 1))
				{
				case 1:
					stringBuilder.AppendFormat("<option value='{0}_1_3'>{1} {0}</option>", dateTime.Year, HtmlLang.Write(LangModule.Report, "Q1th", "Q1th"));
					break;
				case 2:
					stringBuilder.AppendFormat("<option value='{0}_4_6'>{1} {0}</option>", dateTime.Year, HtmlLang.Write(LangModule.Report, "Q2th", "Q2th"));
					break;
				case 3:
					stringBuilder.AppendFormat("<option value='{0}_7_9'>{1} {0}</option>", dateTime.Year, HtmlLang.Write(LangModule.Report, "Q3th", "Q3th"));
					break;
				case 4:
					stringBuilder.AppendFormat("<option value='{0}_10_12'>{1} {0}</option>", dateTime.Year, HtmlLang.Write(LangModule.Report, "Q4th", "Q4th"));
					break;
				}
			}
			return new MvcHtmlString(stringBuilder.ToString());
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
	}
}
