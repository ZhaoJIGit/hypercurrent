using JieNor.Megi.Common.Context;
using JieNor.Megi.Core.MultiLanguage;
using System;

namespace JieNor.Megi.Identity.HtmlHelper
{
	public static class LangConverter
	{
		public static string ToOrgMoneyFormat(this decimal money)
		{
			return money.ToOrgDigitalFormat(ContextHelper.MContext);
		}

		public static string ToDateString(this DateTime date)
		{
			return date.ToOrgZoneDateString(ContextHelper.MContext);
		}

		public static string ToDateFormat(this DateTime date)
		{
			return date.ToOrgZoneDateFormat(ContextHelper.MContext);
		}
	}
}
