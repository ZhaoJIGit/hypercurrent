using JieNor.Megi.Core.MultiLanguage;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Globalization;

namespace JieNor.Megi.BusinessService.RPT
{
	public class RPTBaseService
	{
		protected string GetDateHeadTitle(MContext ctx, DateTime period)
		{
			if (ctx.MLCID == "0x0009")
			{
				return string.Format("{0} {1}", period.ToString("MMMMMMMMMMMMMMMM", CultureInfo.CreateSpecificCulture("en-US")), period.Year);
			}
			return $"{period.Year}年{period.Month}月";
		}

		protected string GetCurrentDateHeadTitle(MContext ctx)
		{
			if (ctx.MLCID == "0x0009")
			{
				return "Current";
			}
			return "本月";
		}

		protected string GetDateHeadTitle(MContext ctx, DateTime fromDate, DateTime toDate, bool displayYear = false)
		{
			if (fromDate.Year != toDate.Year | displayYear)
			{
				if (ctx.MLCID == "0x0009")
				{
					return $"{fromDate.ToOrgZoneYearMonth(ctx)} to {toDate.ToOrgZoneYearMonth(ctx)}";
				}
				return $"{fromDate.Year}年{fromDate.Month}月 至 {toDate.Year}年{toDate.Month}月";
			}
			if (ctx.MLCID == "0x0009")
			{
				return string.Format("{0} to {1}", fromDate.ToString("MMMMMMMMMMMMMMMM", CultureInfo.CreateSpecificCulture("en-US")), toDate.ToString("MMMMMMMMMMMMMMMM", CultureInfo.CreateSpecificCulture("en-US")));
			}
			return $"{fromDate.Month}月 至 {toDate.Month}月";
		}
	}
}
