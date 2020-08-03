using System;
using System.Collections.Generic;
using System.Globalization;

namespace JieNor.Megi.Core.Helper
{
	public class AccountHelper
	{
		public static string MoneyToString(decimal money, string defaultValue = "")
		{
			if (money == decimal.Zero)
			{
				return defaultValue;
			}
			return Convert.ToString(money);
		}

		public static int GetYearPeriod(int startYearPeried, int add)
		{
			return int.Parse(DateTime.ParseExact(startYearPeried.ToString(), "yyyyMM", CultureInfo.InvariantCulture).AddMonths(add).ToString("yyyyMM"));
		}

		public static int GetCirculationCount(int startPeriod, int endPeriod)
		{
			//int num = 0;
			string text = Convert.ToString(startPeriod);
			string text2 = Convert.ToString(endPeriod);
			int num2 = int.Parse(text.Substring(0, 4));
			int num3 = (text.Length > 5) ? int.Parse(text.Substring(4, 2)) : int.Parse(text.Substring(4, 1));
			int num4 = int.Parse(text2.Substring(0, 4));
			int num5 = (text2.Length > 5) ? int.Parse(text2.Substring(4, 2)) : int.Parse(text2.Substring(4, 1));
			if (num5 < num3)
			{
				return num5 + 12 - num3 + 1;
			}
			return (num4 - num2) * 12 + num5 - num3 + 1;
		}

		public static List<int> GetYearPeriod(string yearPeriod)
		{
			List<int> list = new List<int>();
			string value = yearPeriod.Substring(0, 4);
			list.Add(Convert.ToInt32(value));
			string value2 = (yearPeriod.Length > 5) ? yearPeriod.Substring(4, 2) : yearPeriod.Substring(4, 1);
			list.Add(Convert.ToInt32(value2));
			return list;
		}

		public static DateTime GetDateTimeByYearPeriod(string yearPeriod, int type)
		{
			string text = "";
			List<int> yearPeriod2 = GetYearPeriod(yearPeriod);
			int num = yearPeriod2[0];
			string str = num.ToString();
			num = yearPeriod2[1];
			string str2 = num.ToString();
			if (yearPeriod2[1] < 10)
			{
				str2 = "0" + str2;
			}
			if (type == 0)
			{
				text = str + str2 + "01";
			}
			else
			{
				int value = DateTime.DaysInMonth(yearPeriod2[0], yearPeriod2[1]);
				text = str + str2 + Convert.ToString(value);
			}
			return DateTime.ParseExact(text, "yyyyMMdd", CultureInfo.CurrentCulture);
		}

		public static int GetBalanceDirection(int accountDr, decimal balance)
		{
			//int num = 1;
			if (balance == decimal.Zero)
			{
				return 0;
			}
			return accountDr;
		}
	}
}
