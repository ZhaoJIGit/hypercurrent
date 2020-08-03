using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.MResource;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MResource;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace JieNor.Megi.DataRepository.COM
{
	public static class COMResourceHelper
	{
		private static string GetAvaliableVoucherNumber(MContext ctx, int year, int period, int startNumber)
		{
			List<MResource> resource = MResourceHelper.GetResource(ctx, GetVoucherResourceFilter(ctx, year, period, 1, 1, null));
			if (resource == null || resource.Count == 0 || resource[0].MFieldValue == null)
			{
				throw new MActionException(new List<MActionResultCodeEnum>
				{
					MActionResultCodeEnum.MVoucherNumberNotMeetDemand
				});
			}
			return resource[0].MFieldValue.ToString();
		}

		private static List<string> GetAvaliableVoucherNumbers(MContext ctx, int count, int year, int period, int startNumber)
		{
			List<MResource> resource = MResourceHelper.GetResource(ctx, GetVoucherResourceFilter(ctx, year, period, startNumber, count, null));
			if (resource == null || resource.Count != count || (from x in resource
			select x.MFieldValue != null).Count() != count)
			{
				throw new MActionException(new List<MActionResultCodeEnum>
				{
					MActionResultCodeEnum.MVoucherNumberNotMeetDemand
				});
			}
			return (from x in resource
			select x.MFieldValue.ToString()).ToList();
		}

		public static List<string> GetNextVoucherNumbers(MContext ctx, int? year, int? period, int count = 1, string lastNumber = null, List<string> excludeNumberList = null)
		{
			DateTime dateNow;
			int num;
			if (!year.HasValue)
			{
				dateNow = ctx.DateNow;
				num = dateNow.Year;
			}
			else
			{
				num = year.Value;
			}
			int year2 = num;
			int num2;
			if (!period.HasValue)
			{
				dateNow = ctx.DateNow;
				num2 = dateNow.Month;
			}
			else
			{
				num2 = period.Value;
			}
			int period2 = num2;
			int startNumber = string.IsNullOrWhiteSpace(lastNumber) ? 1 : (int.Parse(lastNumber) + 1);
			List<string> avaliableVoucherNumbers = GetAvaliableVoucherNumbers(ctx, count, year2, period2, startNumber);
			for (int i = 0; i < avaliableVoucherNumbers.Count; i++)
			{
				while (excludeNumberList != null && excludeNumberList.Count > 0 && excludeNumberList.Contains(avaliableVoucherNumbers[i]))
				{
					avaliableVoucherNumbers[i] = GetAvaliableVoucherNumber(ctx, year2, period2, startNumber);
				}
			}
			return avaliableVoucherNumbers;
		}

		public static string GetNextVoucherNumber(MContext ctx, int? year, int? period, string lastNumber = null, List<string> excludeNumberList = null)
		{
			DateTime dateNow;
			int num;
			if (!year.HasValue)
			{
				dateNow = ctx.DateNow;
				num = dateNow.Year;
			}
			else
			{
				num = year.Value;
			}
			int year2 = num;
			int num2;
			if (!period.HasValue)
			{
				dateNow = ctx.DateNow;
				num2 = dateNow.Month;
			}
			else
			{
				num2 = period.Value;
			}
			int period2 = num2;
			int startNumber = string.IsNullOrWhiteSpace(lastNumber) ? 1 : (int.Parse(lastNumber) + 1);
			string avaliableVoucherNumber = GetAvaliableVoucherNumber(ctx, year2, period2, startNumber);
			while (excludeNumberList != null && excludeNumberList.Count > 0 && excludeNumberList.Contains(avaliableVoucherNumber))
			{
				avaliableVoucherNumber = GetAvaliableVoucherNumber(ctx, year2, period2, startNumber);
			}
			return avaliableVoucherNumber;
		}

		public static MResourceFilter GetVoucherResourceFilter(MContext ctx, int year, int period, int start = 1, int count = 1, string pkID = null)
		{
			MResourceFilter mResourceFilter = new MResourceFilter(typeof(GLVoucherModel));
			mResourceFilter.Adapter = new GLVoucherResourceAdapter();
			mResourceFilter.Count = count;
			mResourceFilter.ResourcePrefix = year + "-" + period;
			mResourceFilter.FilledChar = ctx.MVoucherNumberFilledChar;
			mResourceFilter.MaxLength = ctx.MVoucherNumberLength;
			mResourceFilter.StartWith = start;
			mResourceFilter.StartAfterMax = false;
			mResourceFilter.SqlFilter = " and MYear = @MYear and MPeriod = @MPeriod" + (string.IsNullOrWhiteSpace(pkID) ? "" : " and MItemID != @MItemID ");
			mResourceFilter.SqlFitlerParams = new MySqlParameter[3]
			{
				new MySqlParameter("@MYear", year),
				new MySqlParameter("@MPeriod", period),
				new MySqlParameter("@MItemID", pkID)
			};
			mResourceFilter.Type = typeof(GLVoucherModel);
			return mResourceFilter;
		}

		public static bool IsMNumberUsed(MContext ctx, int year, int period, string number, string pkID = null)
		{
			number = ToVoucherNumber(ctx, number, 0);
			return MResourceHelper.IsResourceUsed(ctx, GetVoucherResourceFilter(ctx, year, period, 1, 1, pkID), number);
		}

		public static void TryLockVoucherNumber(MContext ctx, int year, int period, List<string> numbers)
		{
			if (numbers != null && numbers.Count != 0)
			{
				MResourceFilter filter = GetVoucherResourceFilter(ctx, year, period, 1, 1, null);
				List<MResource> resources = new List<MResource>();
				numbers.ForEach(delegate(string x)
				{
					resources.Add(MResourceHelper.GetResource(ctx, filter, x));
				});
				MResourceHelper.TryLockResource(ctx, filter, resources);
			}
		}

		public static List<BizVerificationInfor> PrehandleVouchersNumber(MContext ctx, List<GLVoucherModel> vouchers)
		{
			List<BizVerificationInfor> list = new List<BizVerificationInfor>();
			if (vouchers == null || vouchers.Count == 0)
			{
				return list;
			}
			List<int> yearPeriods = new List<int>();
			vouchers.ForEach(delegate(GLVoucherModel t)
			{
				if (!yearPeriods.Contains(t.MYear * 100 + t.MPeriod))
				{
					yearPeriods.Add(t.MYear * 100 + t.MPeriod);
				}
			});
			HandleHasNumbers(ctx, (from t in vouchers
			where !string.IsNullOrWhiteSpace(t.MNumber)
			select t).ToList(), yearPeriods, ref list);
			if (list.Count > 0)
			{
				return list;
			}
			HandleNoneNumbers(ctx, (from t in vouchers
			where string.IsNullOrWhiteSpace(t.MNumber)
			select t).ToList(), yearPeriods);
			return list;
		}

		private static void HandleHasNumbers(MContext ctx, List<GLVoucherModel> hasNumberVouchers, List<int> yearPeriods, ref List<BizVerificationInfor> infors)
		{
			string format = "{0}-{1}: {2}";
			foreach (int yearPeriod in yearPeriods)
			{
				int year = yearPeriod / 100;
				int period = yearPeriod % 100;
				List<GLVoucherModel> list = (from t in hasNumberVouchers
				where t.MYear == year && t.MPeriod == period
				select t).ToList();
				if (list != null && list.Count >= 1)
				{
					List<string> list2 = new List<string>();
					foreach (GLVoucherModel item in list)
					{
						item.MNumber = ToVoucherNumber(ctx, item.MNumber, 0);
						if (!list2.Contains(item.MNumber))
						{
							list2.Add(item.MNumber);
						}
						else
						{
							infors.Add(new BizVerificationInfor
							{
								Level = AlertEnum.Error,
								Message = string.Format(format, year, period, string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "VoucherNumberHasBeenUsed", "凭证号{0}已经被使用了!"), item.MNumber))
							});
						}
					}
					if (infors.Count <= 0)
					{
						TryLockVoucherNumber(ctx, year, period, list2);
						continue;
					}
					break;
				}
			}
		}

		private static void HandleNoneNumbers(MContext ctx, List<GLVoucherModel> noneNumberVouchers, List<int> yearPeriods)
		{
			foreach (int yearPeriod in yearPeriods)
			{
				int year = yearPeriod / 100;
				int period = yearPeriod % 100;
				List<GLVoucherModel> list = (from t in noneNumberVouchers
				where t.MYear == year && t.MPeriod == period
				select t).ToList();
				if (list != null && list.Count >= 1)
				{
					List<string> nextVoucherNumbers = GetNextVoucherNumbers(ctx, year, period, list.Count, null, null);
					for (int i = 0; i < list.Count; i++)
					{
						list[i].MNumber = nextVoucherNumbers[i];
					}
				}
			}
		}

		public static string ToVoucherNumber(MContext ctx, string str, int val = 0)
		{
			if (string.IsNullOrWhiteSpace(str) && val >= 0)
			{
				str = val.ToString();
			}
			if (!string.IsNullOrWhiteSpace(str))
			{
				str = Regex.Replace(str, "[^\\d]", "");
			}
			if (string.IsNullOrWhiteSpace(str) || int.Parse(str) <= 0)
			{
				throw new Exception("凭证编码不合法");
			}
			str = int.Parse(str).ToString();
			return str.ToFixLengthString(ctx.MVoucherNumberLength, ctx.MVoucherNumberFilledChar);
		}

		public static string MaxVoucherNumber(MContext ctx)
		{
			return "999999999999999999999999".Substring(0, ctx.MVoucherNumberLength);
		}

		public static string MinVoucherNumber(MContext ctx)
		{
			return "1".PadLeft(ctx.MVoucherNumberLength, Convert.ToChar(ctx.MVoucherNumberFilledChar));
		}
	}
}
