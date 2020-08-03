using JieNor.Megi.Core.DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace JieNor.Megi.Core
{
	public static class MConverter
	{
		public static int ToMInt32(this object obj)
		{
			return (obj != null && obj != DBNull.Value && !string.IsNullOrEmpty(obj.ToString())) ? Convert.ToInt32(obj) : 0;
		}

		public static string ToMoneyFormat(this decimal obj)
		{
			return string.Format("{0:N}", obj);
		}

		public static decimal ToMDecimal(this object obj)
		{
			return (obj == null || obj == DBNull.Value || string.IsNullOrEmpty(obj.ToString())) ? decimal.Zero : Convert.ToDecimal(obj);
		}

		public static double ToMDouble(this object obj)
		{
			return (obj == null || obj == DBNull.Value || string.IsNullOrEmpty(obj.ToString())) ? 0.0 : Convert.ToDouble(obj);
		}

		public static string ToMString(this object str)
		{
			if (str == null || str == DBNull.Value)
			{
				return "";
			}
			return str.ToString();
		}

		public static DateTime ToMDateTime(this object str)
		{
			if (str == null || str == DBNull.Value)
			{
				return new DateTime(1900, 1, 1);
			}
			return Convert.ToDateTime(str);
		}

		public static string To2Decimal(this decimal s)
		{
			return s.ToString("#0.00");
		}

		public static string To2Decimal(this string s)
		{
			if (string.IsNullOrWhiteSpace(s))
			{
				return "0.00";
			}
			decimal num = default(decimal);
			decimal.TryParse(s, out num);
			return num.ToString("#0.00");
		}

		public static string To4Decimal(this decimal s)
		{
			return s.ToString("#0.0000");
		}

		public static string ToUserName(string localeId, string firstName, string lastName)
		{
			if (localeId == "0x0009")
			{
				return string.Format("{0} {1}", firstName, lastName);
			}
			return string.Format("{0} {1}", lastName, firstName);
		}

		public static string ToUserName(string localeId, string fullName, char splitChar)
		{
			string[] array = fullName.Split(splitChar);
			if (array.Length == 2)
			{
				if (localeId == "0x0009")
				{
					return string.Format("{0} {1}", array[0], array[1]);
				}
				return string.Format("{0} {1}", array[1], array[0]);
			}
			return fullName;
		}

		public static string ToString(this List<ValidationError> list, string separator)
		{
			if (list == null || list.Count == 0)
			{
				return "";
			}
			List<string> list2 = new List<string>();
			StringBuilder stringBuilder = new StringBuilder();
			int num = 1;
			foreach (ValidationError item in list)
			{
				if (!list2.Contains(item.Message))
				{
					list2.Add(item.Message);
					stringBuilder.Append(item.Message);
					if (num != list.Count)
					{
						stringBuilder.Append(separator);
					}
				}
				num++;
			}
			return stringBuilder.ToString();
		}
	}
}
