using System;

namespace JieNor.Megi.Common.Utility
{
	public static class DateUtility
	{
		public static DateTime ToShortDate(this DateTime dt)
		{
			return dt.Date;
		}

		public static DateTime ToDayLastSecond(this DateTime date)
		{
			return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
		}

		public static DateTime ToDayFirstSecond(this DateTime date)
		{
			return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
		}

		public static DateTime ToMonthLastDay(this DateTime date)
		{
			DateTime dateTime = date.AddMonths(1);
			int year = dateTime.Year;
			dateTime = date.AddMonths(1);
			return new DateTime(year, dateTime.Month, 1).ToDayLastSecond();
		}

		public static DateTime ToMonthFirstDay(this DateTime date)
		{
			return new DateTime(date.Year, date.Month, 1).ToDayLastSecond();
		}

		public static bool IsValidDateTime(this DateTime date)
		{
			return (date - new DateTime(1970, 1, 1)).TotalMilliseconds >= 0.0;
		}

		public static DateTime MegiDefaultDate(this DateTime date)
		{
			return new DateTime(2000, 1, 1).ToDayFirstSecond();
		}

		public static int GetWeekOfMonth(DateTime dt)
		{
			DateTime yearMonthStart = DateTime.Parse(dt.Year + "-" + dt.Month + "-1");
			int d = dt.Day - yearMonthStart.Day;
			int weekNum = 1;
			for (int i = 1; i <= d; i++)
			{
				if (yearMonthStart.AddDays((double)i).DayOfWeek == DayOfWeek.Sunday)
				{
					weekNum++;
				}
			}
			return weekNum;
		}

		public static int GetWeekOfYear(DateTime dt)
		{
			int i = Convert.ToInt32(DateTime.Parse(dt.Year + "-01-01").DayOfWeek);
			if (i == 0)
			{
				i = 7;
			}
			int j = Convert.ToInt32(dt.DayOfYear);
			j -= 7 - i + 1;
			if (j <= 0)
			{
				return 1;
			}
			if (j % 7 == 0)
			{
				return j / 7 + 1;
			}
			return j / 7 + 2;
		}

		public static bool IsEmpty(this DateTime date)
		{
			//bool result = false;
			return date <= new DateTime(1900, 1, 1);
		}
	}
}
