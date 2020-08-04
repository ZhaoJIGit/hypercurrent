using JieNor.Megi.DataModel.GL;
using JieNor.Megi.Service.Webapi.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JieNor.Megi.Service.Webapi.Helper
{
	public class ModelConvertHelper
	{
		public static List<T> ConvertToViewModel<T, T1>(List<T1> modelList) where T : new()
		{
			if (modelList == null)
			{
				throw new NullReferenceException("ConvertToViewModel: model is not null");
			}
			List<T> list = new List<T>();
			PropertyInfo[] properties = typeof(T).GetProperties();
			if (properties.Length != 0 && modelList != null)
			{
				PropertyInfo[] properties2 = typeof(T1).GetProperties();
				{
					foreach (T1 model in modelList)
					{
						T val = new T();
						PropertyInfo[] array = properties;
						foreach (PropertyInfo propertyInfo in array)
						{
							string propertypName = propertyInfo.Name;
							PropertyInfo propertyInfo2 = (from x in properties2
							where x.Name == propertypName
							select x).FirstOrDefault();
							if (!(propertyInfo2 == (PropertyInfo)null))
							{
								propertyInfo.SetValue(val, propertyInfo2.GetValue(model));
							}
						}
						list.Add(val);
					}
					return list;
				}
			}
			return list;
		}

		public static List<GLBalanceViewModel> CovertToBalanceViewModel(List<GLBalanceModel> balanceList)
		{
			List<GLBalanceViewModel> list = new List<GLBalanceViewModel>();
			if (balanceList != null && balanceList.Count() != 0)
			{
				{
					foreach (GLBalanceModel balance in balanceList)
					{
						GLBalanceViewModel gLBalanceViewModel = new GLBalanceViewModel();
						gLBalanceViewModel.MAccountID = balance.MAccountID;
						gLBalanceViewModel.MNumber = balance.MNumber;
						gLBalanceViewModel.MAccountName = balance.MAccountName;
						gLBalanceViewModel.MAccountFullName = balance.MAccountName;
						gLBalanceViewModel.MDC = balance.MDC;
						gLBalanceViewModel.MDebit = balance.MDebit;
						gLBalanceViewModel.MDebitFor = balance.MDebitFor;
						gLBalanceViewModel.MCredit = balance.MCredit;
						gLBalanceViewModel.MCreditFor = balance.MCreditFor;
						gLBalanceViewModel.MYtdCredit = balance.MYtdCredit;
						gLBalanceViewModel.MYtdCreditFor = balance.MYtdCreditFor;
						gLBalanceViewModel.MYtdDebit = balance.MYtdDebit;
						gLBalanceViewModel.MYtdDebitFor = balance.MYtdDebitFor;
						gLBalanceViewModel.MYear = balance.MYear;
						gLBalanceViewModel.MPeriod = balance.MPeriod;
						gLBalanceViewModel.MYearPeriod = balance.MYearPeriod;
						gLBalanceViewModel.MCheckGroupValueID = balance.MCheckGroupValueID;
						gLBalanceViewModel.MBeginBalance = balance.MBeginBalance;
						gLBalanceViewModel.MBeginBalanceFor = balance.MBeginBalanceFor;
						gLBalanceViewModel.MCheckGroupValueModel = balance.MCheckGroupValueModel;
						list.Add(gLBalanceViewModel);
					}
					return list;
				}
			}
			return list;
		}

		public static List<GLVoucherViewModel> ConvertToVoucherViewModel(List<GLVoucherModel> voucherList)
		{
			List<GLVoucherViewModel> list = new List<GLVoucherViewModel>();
			if (voucherList != null && voucherList.Count() != 0)
			{
				{
					foreach (GLVoucherModel voucher in voucherList)
					{
						GLVoucherViewModel gLVoucherViewModel = new GLVoucherViewModel();
						gLVoucherViewModel.MNumber = voucher.MNumber;
						gLVoucherViewModel.MDate = voucher.MDate;
						gLVoucherViewModel.MDebitTotal = voucher.MDebitTotal;
						gLVoucherViewModel.MCreditTotal = voucher.MCreditTotal;
						gLVoucherViewModel.MOrgID = voucher.MOrgID;
						gLVoucherViewModel.MYear = voucher.MYear;
						gLVoucherViewModel.MPeriod = voucher.MPeriod;
						gLVoucherViewModel.MVoucherEntrys = ConvertToVoucherEntryViewModel(voucher.MVoucherEntrys);
						list.Add(gLVoucherViewModel);
					}
					return list;
				}
			}
			return list;
		}

		public static List<GLVoucherEntryViewModel> ConvertToVoucherEntryViewModel(List<GLVoucherEntryModel> voucherEntryList)
		{
			List<GLVoucherEntryViewModel> list = new List<GLVoucherEntryViewModel>();
			if (voucherEntryList != null && voucherEntryList.Count() != 0)
			{
				{
					foreach (GLVoucherEntryModel voucherEntry in voucherEntryList)
					{
						GLVoucherEntryViewModel gLVoucherEntryViewModel = new GLVoucherEntryViewModel();
						gLVoucherEntryViewModel.MExplanation = voucherEntry.MExplanation;
						gLVoucherEntryViewModel.MAccountID = voucherEntry.MAccountID;
						gLVoucherEntryViewModel.MAccountName = voucherEntry.MAccountName;
						gLVoucherEntryViewModel.MAmountFor = voucherEntry.MAmountFor;
						gLVoucherEntryViewModel.MAmount = voucherEntry.MAmount;
						gLVoucherEntryViewModel.MDebit = voucherEntry.MDebit;
						gLVoucherEntryViewModel.MCredit = voucherEntry.MCredit;
						gLVoucherEntryViewModel.MCheckGroupValueID = voucherEntry.MCheckGroupValueID;
						list.Add(gLVoucherEntryViewModel);
					}
					return list;
				}
			}
			return list;
		}
	}
}
