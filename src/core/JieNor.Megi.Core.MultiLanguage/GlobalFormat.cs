using JieNor.Megi.Common.Context;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace JieNor.Megi.Core.MultiLanguage
{
	public static class GlobalFormat
	{
		private static Dictionary<string, List<GlobalCurrencyModel>> CurrencyGlobalDict = new Dictionary<string, List<GlobalCurrencyModel>>();

		private static void GetCurrencyGlobalDict()
		{
			if (CurrencyGlobalDict == null || CurrencyGlobalDict.Count == 0 || !CurrencyGlobalDict.ContainsKey(ContextHelper.MContext.MOrgID))
			{
				GetCurrencyGlobalModel();
			}
		}

		public static string ToMShortDateString(this DateTime date, MContext ctx = null)
		{
			ctx = (ctx ?? ContextHelper.MContext);
			return date.ToString(GetDateFormat(ctx));
		}

		public static string ToMLongDateString(this DateTime date, MContext ctx = null)
		{
			ctx = (ctx ?? ContextHelper.MContext);
			return date.ToMShortDateString(ctx) + " " + date.ToString(GetTimeFormat(ctx));
		}

		private static void GetCurrencyGlobalModel()
		{
		}

		public static void ClearDictByOrgID(string orgid)
		{
			if (CurrencyGlobalDict.ContainsKey(orgid))
			{
				CurrencyGlobalDict.Remove(orgid);
			}
		}

		public static string GetUserName(string firstName, string lastName, MContext ctx = null)
		{
			string pattern = "[一-龥]+";
			if ((firstName != null && Regex.IsMatch(firstName, pattern)) || (lastName != null && Regex.IsMatch(lastName, pattern)))
			{
				return string.Format("{0}{1}", lastName, firstName);
			}
			return string.Format("{0} {1}", firstName, lastName);
		}

		public static string GetUserName(string fullName, MContext ctx = null)
		{
			if (ctx != null && ctx.MLCID != LangCodeEnum.EN_US && !string.IsNullOrEmpty(fullName))
			{
				string[] array = fullName.Split(' ');
				if (array.Length > 1)
				{
					string str = array[array.Length - 1];
					string text = "";
					for (int i = 0; i < array.Length - 1; i++)
					{
						text += string.Format(" {0}", array[i]);
					}
					fullName = str + text;
				}
			}
			return fullName;
		}

		public static DateTime ToOrgZoneDateTime(this DateTime utcDateTime, MContext ctx = null)
		{
			ctx = (ctx ?? ContextHelper.MContext);
			utcDateTime = (utcDateTime.IsInValidDateTime() ? ctx.DateNow : utcDateTime);
			DateTime dateTime = TimeZoneInfo.ConvertTimeToUtc(utcDateTime, TimeZoneInfo.Local);
			return TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo.FindSystemTimeZoneById(GetTimeFormat(ctx)));
		}

		public static DateTime ToOrgZoneDate(this DateTime utcDateTime, MContext ctx = null)
		{
			ctx = (ctx ?? ContextHelper.MContext);
			utcDateTime = (utcDateTime.IsInValidDateTime() ? ctx.DateNow : utcDateTime);
			DateTime dateTime = TimeZoneInfo.ConvertTimeToUtc(utcDateTime, TimeZoneInfo.Local);
			return TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo.FindSystemTimeZoneById(GetZoneFormat(ctx)));
		}

		public static string ToOrgZoneDateFormat(this DateTime utcDateTime, MContext ctx = null)
		{
			ctx = (ctx ?? ContextHelper.MContext);
			utcDateTime = (utcDateTime.IsInValidDateTime() ? ctx.DateNow : utcDateTime);
			return utcDateTime.ToString(GetDateFormat(ctx));
		}

		private static bool IsInValidDateTime(this DateTime dateTime)
		{
			return dateTime.Year <= 1900;
		}

		public static DateTime ToOrgZoneTime(this DateTime utcDateTime, MContext ctx = null)
		{
			//bool flag = false;
			DateTime dateTime = TimeZoneInfo.ConvertTimeToUtc(utcDateTime, TimeZoneInfo.Local);
			return TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo.FindSystemTimeZoneById(GetZoneFormat(ctx)));
		}

		public static string ToOrgZoneDateTimeString(this DateTime utcDateTime, MContext ctx = null)
		{
			return utcDateTime.ToOrgZoneDateTime(ctx).ToString(GetDateTimeFormat(ctx));
		}

		public static string ToOrgZoneDateString(this DateTime utcDateTime, MContext ctx = null)
		{
			ctx = (ctx ?? ContextHelper.MContext);
			return utcDateTime.ToOrgZoneDate(ctx).ToString(GetDateFormat(ctx));
		}

		public static string ToOrgZoneTimeString(this DateTime utcDateTime, MContext ctx = null)
		{
			ctx = (ctx ?? ContextHelper.MContext);
			return utcDateTime.ToOrgZoneTime(ctx).ToString(GetTimeFormat(ctx));
		}

		public static string ToOrgZoneYearMonth(this DateTime utcDateTime, MContext ctx = null)
		{
			string dateFormat = GetDateFormat(ctx);
			dateFormat = dateFormat.Replace("-dd", "").Replace("/dd", "").Replace(".dd", "")
				.Replace("-d", "")
				.Replace("/d", "")
				.Replace(".d", "");
			return utcDateTime.ToString(dateFormat);
		}

		public static string ToOrgDigitalFormat(this decimal dec, MContext ctx = null)
		{
			ctx = (ctx ?? ContextHelper.MContext);
			return GetDecimalFormat(ctx, dec);
		}

		public static decimal ToOrgAmountPrecision(this decimal dec, AmountType AmountType = AmountType.Amount, MContext ctx = null, string currencyID = "")
		{
			GetCurrencyGlobalDict();
			return GetAmountFormat(dec, (int)AmountType, ctx, currencyID);
		}

		public static string ToOrgStringAmountPrecision(this decimal dec, AmountType AmountType = AmountType.Amount, MContext ctx = null, string currencyID = "")
		{
			GetCurrencyGlobalDict();
			return GetAmountStringFormat(dec, (int)AmountType, ctx, currencyID);
		}

		private static string GetDateTimeFormat(MContext ctx)
		{
			return GetDateFormat(ctx) + " " + GetTimeFormat(ctx);
		}

		private static string GetDateFormat(MContext ctx)
		{
			return (ctx == null || string.IsNullOrEmpty(ctx.MDateFormat)) ? "yyyy-MM-dd" : ctx.MDateFormat;
		}

		private static string GetTimeFormat(MContext ctx)
		{
			return (ctx == null || string.IsNullOrEmpty(ctx.MTimeFormat)) ? "HH:mm:ss" : ctx.MTimeFormat;
		}

		private static string GetZoneFormat(MContext ctx)
		{
			return (ctx == null || string.IsNullOrEmpty(ctx.MZoneFormat)) ? "China Standard Time" : ctx.MZoneFormat;
		}

		private static string GetDecimalFormat(MContext ctx, decimal dec)
		{
			return (ctx == null || string.IsNullOrEmpty(ctx.MDigitGrpFormat)) ? string.Format("{0:N}", dec) : string.Format("{0:N}", dec).Replace(',', Convert.ToChar(ctx.MDigitGrpFormat));
		}

		private static decimal GetAmountFormat(decimal dec, int AmountType, MContext ctx, string currencyID)
		{
			int decimals = (!IsExistDict(ctx, currencyID)) ? ((AmountType == 0) ? 2 : 4) : GetAmountDigit(AmountType, ctx, currencyID);
			return Math.Round(dec, decimals);
		}

		private static string GetAmountStringFormat(decimal dec, int AmountType, MContext ctx, string currencyID)
		{
			string text = Convert.ToString(GetAmountFormat(dec, AmountType, ctx, currencyID));
			return (!IsExistDict(ctx, currencyID) || !ctx.MIsShowCSymbol) ? text : (CurrencyGlobalDict[ctx.MOrgID].FirstOrDefault((GlobalCurrencyModel w) => w.MCurrencyID == currencyID).MCurrencySymbol + text);
		}

		private static bool IsExistDict(MContext ctx, string currencyID)
		{
			if (ctx == null || string.IsNullOrWhiteSpace(currencyID) || CurrencyGlobalDict == null || !CurrencyGlobalDict.ContainsKey(ctx.MOrgID) || CurrencyGlobalDict[ctx.MOrgID] == null || CurrencyGlobalDict[ctx.MOrgID].FirstOrDefault((GlobalCurrencyModel w) => w.MCurrencyID == currencyID) == null)
			{
				return false;
			}
			return true;
		}

		private static int GetAmountDigit(int AmountType, MContext ctx, string currencyID)
		{
			GlobalCurrencyModel globalCurrencyModel = CurrencyGlobalDict[ctx.MOrgID].FirstOrDefault((GlobalCurrencyModel w) => w.MCurrencyID == currencyID);
			return (AmountType == 0) ? globalCurrencyModel.MAmountPrecision : globalCurrencyModel.MPricePrecision;
		}
	}
}
