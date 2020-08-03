using JieNor.Megi.Common.Logger;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.MultiLanguage;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.Enum;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.DataRepository.API;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.FA;
using JieNor.Megi.DataRepository.FP;
using JieNor.Megi.DataRepository.Log.GlLog;
using JieNor.Megi.DataRepository.SEC;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace JieNor.Megi.DataRepository.GL
{
	public class GLVoucherRepository : DataServiceT<GLVoucherModel>
	{
		private string CommonSelect = "SELECT \n                                            t1.MItemID,\n                                            t1.MNumber,\n                                            t1.MNumber as MVoucherNumber,\n                                            t1.MDate,\n                                            t1.MYear,\n                                            t1.MPeriod,\n                                            t1.MAttachments,\n                                            t1.MReference,\n                                            t1.MDebitTotal,\n                                            t1.MCreditTotal,\n                                            t1.MStatus,\n                                            t1.MUrl,\n                                            t1.MAuditDate,\n                                            t1.MCreateBy,\n                                            t1.MModifyDate,\n                                            t1.MSourceBillKey,\n                                            t2.MEntryID AS MVoucherEntrys_MJournalLineID,\n                                            t2.MExplanation AS MVoucherEntrys_MExplanation,\n                                            t2.MAccountID AS MVoucherEntrys_MAccountID,\n                                            t2.MAmountFor AS MVoucherEntrys_MAmountFor,\n                                            t2.MAmount AS MVoucherEntrys_MAmount,\n                                            t2.MCurrencyID AS MVoucherEntrys_MCurrencyID,\n                                            t2.MExchangeRate AS MVoucherEntrys_MExchangeRate,\n                                            t2.MCheckGroupValueID AS MVoucherEntrys_MCheckGroupValueID,\n                                            t2.MEntrySeq AS MVoucherEntrys_MEntrySeq,\n                                            t2.MDC AS MVoucherEntrys_MDC,\n                                            t2.MDebit AS MVoucherEntrys_MDebit,\n                                            t2.MCredit AS MVoucherEntrys_MCredit,\n                                            t4.MNumber as MVoucherEntrys_MAccountCode,\n                                            t3.MContactID as MVoucherEntrys_MCheckGroupValueModel_MContactID,\n                                            t3.MEmployeeID as MVoucherEntrys_MCheckGroupValueModel_MEmployeeID,\n                                            t3.MMerItemID as MVoucherEntrys_MCheckGroupValueModel_MMerItemID,\n                                            t3.MExpItemID as MVoucherEntrys_MCheckGroupValueModel_MExpItemID,\n                                            t3.MPaItemID as MVoucherEntrys_MCheckGroupValueModel_MPaItemID,\n                                            t3.MTrackItem1 as MVoucherEntrys_MCheckGroupValueModel_MTrackItem1,\n                                            t3.MTrackItem2 as MVoucherEntrys_MCheckGroupValueModel_MTrackItem2,\n                                            t3.MTrackItem3 as MVoucherEntrys_MCheckGroupValueModel_MTrackItem3,\n                                            t3.MTrackItem4 as MVoucherEntrys_MCheckGroupValueModel_MTrackItem4,\n                                            t3.MTrackItem5 as MVoucherEntrys_MCheckGroupValueModel_MTrackItem5\n                                        FROM\n                                            t_gl_voucher t1\n                                                INNER JOIN\n                                            t_gl_voucherentry t2 ON t1.MItemID = t2.MID\n                                                AND t1.MOrgID = t2.MOrgID\n                                                AND t1.MIsDelete = t2.MIsDelete\n                                                LEFT JOIN\n                                            t_gl_checkgroupvalue t3 ON t2.MOrgID = t3.MOrgID\n                                                AND t2.MIsDelete = t3.MIsDelete\n                                                AND t2.MCheckGroupValueID = t3.MItemID\n                                                LEFT JOIN\n                                            t_bd_account t4 ON t4.MOrgID = t2.MOrgID\n                                                AND t4.MIsDelete = t2.MIsDelete\n                                                AND t4.MItemID = t2.MAccountID                          \n                                        WHERE\n                                            t1.MOrgID = @MOrgID AND t1.MIsDelete = 0\n                                                AND LENGTH(IFNULL(t1.MNumber, '')) > 0";

		private GLUtility utility = new GLUtility();

		private GLCheckGroupValueRepository checkGroupValueDal = new GLCheckGroupValueRepository();

		public DataGridJson<GLVoucherModel> Get(MContext ctx, GetParam param)
		{
			return new APIDataRepository().Get<GLVoucherModel>(ctx, param, CommonSelect, false, true, null);
		}

		public static List<GLVoucherModel> ValidateVouchers(MContext ctx, List<GLVoucherModel> voucherList, List<GLSimpleVoucherModel> existsVouchers = null, bool fix = false)
		{
			if (voucherList == null || !voucherList.Any())
			{
				return voucherList;
			}
			GLDataPool instance = GLDataPool.GetInstance(ctx, false, 0, 0, 0);
			List<int> periods = (from x in voucherList
			where !string.IsNullOrWhiteSpace(x.MItemID)
			select x.MYear * 100 + x.MPeriod).Distinct().ToList();
			existsVouchers = (existsVouchers ?? instance.GetSimpleVouchersByPeriods(periods));
			List<GLSettlementModel> closedPeriods = instance.ClosedPeriods;
			List<GLCheckGroupValueModel> checkGroupValueList = instance.CheckGroupValueList;
			DateTime mGLBeginDate = ctx.MGLBeginDate;
			int num = mGLBeginDate.Year * 100;
			mGLBeginDate = ctx.MGLBeginDate;
			int num2 = num + mGLBeginDate.Month;
			for (int i = 0; i < voucherList.Count; i++)
			{
				GLVoucherModel voucher = voucherList[i];
				int yearPeriod = voucher.MYear * 100 + voucher.MPeriod;
				voucher.MRowIndex = i;
				voucher.Validate(ctx, yearPeriod < num2, "PeriodBeforeStart", "选择的期间在总账启用之前！", LangModule.GL);
				voucher.Validate(ctx, closedPeriods.Exists((GLSettlementModel x) => x.MYear * 100 + x.MYear == yearPeriod), "PeriodClosed", "选择的期间已经结账！", LangModule.GL);
				voucher.Validate(ctx, voucher.MStatus == 1 && !ctx.MInitBalanceOver, "InitBalanceOver", "总账未完成初始化", LangModule.Common);
				voucher.Validate(ctx, voucher.MStatus == 1 && yearPeriod != num2 && (from x in closedPeriods
				select x.MYear * 100 + x.MPeriod into x
				where x < yearPeriod
				select x).Count() < yearPeriod - num2, "HasUnsettledPeriodBefore", "本期之前有未结账的期", LangModule.Common);
				if (!string.IsNullOrWhiteSpace(voucher.MNumber))
				{
					GLSimpleVoucherModel gLSimpleVoucherModel = existsVouchers.FirstOrDefault((GLSimpleVoucherModel x) => x.MYear * 100 + x.MPeriod == voucher.MYear * 100 + voucher.MPeriod && x.MNumber == voucher.MNumber);
					voucher.Validate(ctx, gLSimpleVoucherModel != null && gLSimpleVoucherModel.MItemID != voucher.MItemID, "VoucherNumberDuplicated", "凭证编号重复了", LangModule.GL);
				}
				voucher.Validate(ctx, voucher.MVoucherEntrys == null, "VoucherHasNotEntry", "凭证分录为空", LangModule.Common);
				if (voucher.MVoucherEntrys != null)
				{
					voucher.Validate(ctx, voucher.MVoucherEntrys.Sum((GLVoucherEntryModel x) => x.MDebit) != voucher.MVoucherEntrys.Sum((GLVoucherEntryModel x) => x.MCredit), "CreditDebitImbalance", "凭证分录借贷方不平衡", LangModule.Common);
					for (int j = 0; j < voucher.MVoucherEntrys.Count; j++)
					{
						GLVoucherEntryModel entry = voucher.MVoucherEntrys[j];
						bool flag = entry.Validate(ctx, !string.IsNullOrWhiteSpace(voucher.MNumber) && string.IsNullOrWhiteSpace(entry.MAccountID), "VoucherEntryAccountNotEmpty", "凭证分录的科目不可为空", LangModule.Common);
						if (!string.IsNullOrWhiteSpace(entry.MAccountID))
						{
							BDAccountModel bDAccountModel = instance.AccountListWitchCheckType.FirstOrDefault((BDAccountModel x) => x.MItemID == entry.MAccountID);
							if (entry.Validate(ctx, bDAccountModel == null, "AccountInvalid", "科目被删除或者禁用", LangModule.Common))
							{
								if (entry.MCurrencyID != ctx.MBasCurrencyID && !bDAccountModel.MIsCheckForCurrency)
								{
									if (fix)
									{
										entry.MCurrencyID = ctx.MBasCurrencyID;
										entry.MAmountFor = entry.MAmount;
										entry.MExchangeRate = 1.0m;
										entry.MModified = true;
									}
									else
									{
										entry.Validate(ctx, true, "CurrencyNotMatchAccount", "非外币核算的科目录入了外币", LangModule.Common);
									}
								}
								entry.MCheckGroupValueModel = (entry.MCheckGroupValueModel ?? new GLCheckGroupValueModel());
								if (!entry.Validate(ctx, !string.IsNullOrWhiteSpace(entry.MCheckGroupValueModel.MContactID) && !instance.ContactList.Exists((BDContactsModel x) => x.MItemID == entry.MCheckGroupValueModel.MContactID), "ContactInvalid", "联系人被删除或者禁用", LangModule.Common))
								{
									entry.MCheckGroupValueModel.MContactID = null;
								}
								if (!entry.Validate(ctx, !string.IsNullOrWhiteSpace(entry.MCheckGroupValueModel.MEmployeeID) && !instance.EmployeeList.Exists((BDEmployeesModel x) => x.MItemID == entry.MCheckGroupValueModel.MEmployeeID), "EmployeeInvalid", "员工被删除或者禁用", LangModule.Common))
								{
									entry.MCheckGroupValueModel.MEmployeeID = null;
								}
								if (!entry.Validate(ctx, !string.IsNullOrWhiteSpace(entry.MCheckGroupValueModel.MMerItemID) && !instance.MerItemList.Exists((BDItemModel x) => x.MItemID == entry.MCheckGroupValueModel.MMerItemID), "MerItemInvalid", "商品项目被删除或者禁用", LangModule.Common))
								{
									entry.MCheckGroupValueModel.MMerItemID = null;
								}
								if (!entry.Validate(ctx, !string.IsNullOrWhiteSpace(entry.MCheckGroupValueModel.MExpItemID) && !instance.ExpenseItemList.Exists((BDExpenseItemModel x) => x.MItemID == entry.MCheckGroupValueModel.MExpItemID), "ExpItemInvalid", "费用项目被删除或者禁用", LangModule.Common))
								{
									entry.MCheckGroupValueModel.MExpItemID = null;
								}
								if (!entry.Validate(ctx, !string.IsNullOrWhiteSpace(entry.MCheckGroupValueModel.MPaItemID) && !instance.PayitemList.Exists((PAPayItemModel x) => x.MItemID == entry.MCheckGroupValueModel.MPaItemID), "PaItemInvalid", "工资项目被删除或者禁用", LangModule.Common))
								{
									entry.MCheckGroupValueModel.MPaItemID = null;
								}
								if (!entry.Validate(ctx, !string.IsNullOrWhiteSpace(entry.MCheckGroupValueModel.MTrackItem1) && instance.TrackListWithEntry.Count > 0 && !instance.TrackListWithEntry[0].MEntryList.Exists((BDTrackEntryModel x) => x.MEntryID == entry.MCheckGroupValueModel.MTrackItem1), "TrackItemInvalid", "跟踪项被删除或者禁用", LangModule.Common))
								{
									entry.MCheckGroupValueModel.MTrackItem1 = null;
								}
								if (!entry.Validate(ctx, !string.IsNullOrWhiteSpace(entry.MCheckGroupValueModel.MTrackItem2) && instance.TrackListWithEntry.Count > 1 && !instance.TrackListWithEntry[1].MEntryList.Exists((BDTrackEntryModel x) => x.MEntryID == entry.MCheckGroupValueModel.MTrackItem2), "TrackItemInvalid", "跟踪项被删除或者禁用", LangModule.Common))
								{
									entry.MCheckGroupValueModel.MTrackItem2 = null;
								}
								if (!entry.Validate(ctx, !string.IsNullOrWhiteSpace(entry.MCheckGroupValueModel.MTrackItem3) && instance.TrackListWithEntry.Count > 2 && !instance.TrackListWithEntry[2].MEntryList.Exists((BDTrackEntryModel x) => x.MEntryID == entry.MCheckGroupValueModel.MTrackItem3), "TrackItemInvalid", "跟踪项被删除或者禁用", LangModule.Common))
								{
									entry.MCheckGroupValueModel.MTrackItem3 = null;
								}
								if (!entry.Validate(ctx, !string.IsNullOrWhiteSpace(entry.MCheckGroupValueModel.MTrackItem4) && instance.TrackListWithEntry.Count > 3 && !instance.TrackListWithEntry[3].MEntryList.Exists((BDTrackEntryModel x) => x.MEntryID == entry.MCheckGroupValueModel.MTrackItem4), "TrackItemInvalid", "跟踪项被删除或者禁用", LangModule.Common))
								{
									entry.MCheckGroupValueModel.MTrackItem4 = null;
								}
								if (!entry.Validate(ctx, !string.IsNullOrWhiteSpace(entry.MCheckGroupValueModel.MTrackItem5) && instance.TrackListWithEntry.Count > 4 && !instance.TrackListWithEntry[4].MEntryList.Exists((BDTrackEntryModel x) => x.MEntryID == entry.MCheckGroupValueModel.MTrackItem5), "TrackItemInvalid", "跟踪项被删除或者禁用", LangModule.Common))
								{
									entry.MCheckGroupValueModel.MTrackItem5 = null;
								}
								entry.MCheckGroupValueModel = instance.GetCheckGroupValueModel(entry.MCheckGroupValueModel);
								ValidateAccountCheckGroup(ctx, instance, entry, bDAccountModel, fix);
								voucher.ValidationErrors = (voucher.ValidationErrors ?? new List<ValidationError>());
								voucher.ValidationErrors.AddRange(entry.ValidationErrors ?? new List<ValidationError>());
							}
						}
					}
				}
			}
			return voucherList;
		}

		private static void ValidateAccountCheckGroup(MContext ctx, GLDataPool pool, GLVoucherEntryModel entry, BDAccountModel account, bool fix = false)
		{
			GLCheckGroupModel mCheckGroupModel = account.MCheckGroupModel;
			GLCheckGroupValueModel gLCheckGroupValueModel = Clone(entry.MCheckGroupValueModel);
			if (mCheckGroupModel.MContactID == CheckTypeStatusEnum.Required && string.IsNullOrWhiteSpace(gLCheckGroupValueModel.MContactID))
			{
				entry.Validate(ctx, true, "AccountWithContactDimensionRequired", "科目[{0}]对应的联系人维度是必录，但是凭证分录却未设置相应的值", LangModule.GL, account.MFullName);
			}
			if (mCheckGroupModel.MContactID != CheckTypeStatusEnum.Optional && mCheckGroupModel.MContactID != CheckTypeStatusEnum.Required && !string.IsNullOrWhiteSpace(gLCheckGroupValueModel.MContactID))
			{
				if (fix)
				{
					gLCheckGroupValueModel.MContactID = null;
					entry.MModified = true;
				}
				else
				{
					entry.Validate(ctx, true, "AccountWitContactDimensionDisabled", "科目[{0}]对应的联系人维度未启用，但是凭证分录却设置了相应的值", LangModule.GL, account.MFullName);
				}
			}
			if (mCheckGroupModel.MEmployeeID == CheckTypeStatusEnum.Required && string.IsNullOrWhiteSpace(gLCheckGroupValueModel.MEmployeeID))
			{
				entry.Validate(ctx, true, "AccountWithEmployeeDimensionRequired", "科目[{0}]对应的员工维度是必录，但是凭证分录却未设置相应的值", LangModule.GL, account.MFullName);
			}
			if (mCheckGroupModel.MEmployeeID != CheckTypeStatusEnum.Optional && mCheckGroupModel.MEmployeeID != CheckTypeStatusEnum.Required && !string.IsNullOrWhiteSpace(gLCheckGroupValueModel.MEmployeeID))
			{
				if (fix)
				{
					gLCheckGroupValueModel.MEmployeeID = null;
					entry.MModified = true;
				}
				else
				{
					entry.Validate(ctx, true, "AccountWithEmployeeItemDimensionDisabled", "科目[{0}]对应的员工项目维度未启用，但是凭证分录却设置了相应的值", LangModule.GL, account.MFullName);
				}
			}
			if (mCheckGroupModel.MMerItemID == CheckTypeStatusEnum.Required && string.IsNullOrWhiteSpace(gLCheckGroupValueModel.MMerItemID))
			{
				entry.Validate(ctx, true, "AccountWithItemDimensionRequired", "科目[{0}]对应的商品项目维度是必录，但是凭证分录却未设置相应的值", LangModule.GL, account.MFullName);
			}
			if (mCheckGroupModel.MMerItemID != CheckTypeStatusEnum.Optional && mCheckGroupModel.MMerItemID != CheckTypeStatusEnum.Required && !string.IsNullOrWhiteSpace(gLCheckGroupValueModel.MMerItemID))
			{
				if (fix)
				{
					gLCheckGroupValueModel.MMerItemID = null;
					entry.MModified = true;
				}
				else
				{
					entry.Validate(ctx, true, "AccountWithItemDimensionDisabled", "科目[{0}]对应的商品项目维度未启用，但是凭证分录却设置了相应的值", LangModule.GL, account.MFullName, pool.TrackList[0].MName);
				}
			}
			if (mCheckGroupModel.MExpItemID == CheckTypeStatusEnum.Required && string.IsNullOrWhiteSpace(gLCheckGroupValueModel.MExpItemID))
			{
				entry.Validate(ctx, true, "AccountWithExpenseDimensionRequired", "科目[{0}]对应的费用项目维度是必录，但是凭证分录却未设置相应的值", LangModule.GL, account.MFullName);
			}
			if (mCheckGroupModel.MExpItemID != CheckTypeStatusEnum.Optional && mCheckGroupModel.MExpItemID != CheckTypeStatusEnum.Required && !string.IsNullOrWhiteSpace(gLCheckGroupValueModel.MExpItemID))
			{
				if (fix)
				{
					gLCheckGroupValueModel.MExpItemID = null;
					entry.MModified = true;
				}
				else
				{
					entry.Validate(ctx, true, "AccountWithExpenseDimensionDisabled", "科目[{0}]对应的费用项目维度未启用，但是凭证分录却设置了相应的值", LangModule.GL, account.MFullName);
				}
			}
			if (mCheckGroupModel.MPaItemID == CheckTypeStatusEnum.Required && string.IsNullOrWhiteSpace(gLCheckGroupValueModel.MPaItemID))
			{
				entry.Validate(ctx, true, "AccountWithSalaryItemDimensionRequired", "科目[{0}]对应的工资项目维度是必录，但是凭证分录却未设置相应的值", LangModule.GL, account.MFullName);
			}
			if (mCheckGroupModel.MPaItemID != CheckTypeStatusEnum.Optional && mCheckGroupModel.MPaItemID != CheckTypeStatusEnum.Required && !string.IsNullOrWhiteSpace(gLCheckGroupValueModel.MPaItemID))
			{
				if (fix)
				{
					gLCheckGroupValueModel.MPaItemID = null;
					entry.MModified = true;
				}
				else
				{
					entry.Validate(ctx, true, "AccountWithSalaryItemDimensionDisabled", "科目[{0}]对应的工资项目维度未启用，但是凭证分录却设置了相应的值", LangModule.GL, account.MFullName);
				}
			}
			if (pool.TrackList != null && pool.TrackList.Count >= 1 && mCheckGroupModel.MTrackItem1 == CheckTypeStatusEnum.Required && string.IsNullOrWhiteSpace(gLCheckGroupValueModel.MTrackItem1))
			{
				entry.Validate(ctx, true, "AccountWithTrackDimensionRequired", "科目[{0}]对应的跟踪项[{1}]维度是必录，但是凭证分录却未设置相应的值", LangModule.GL, account.MFullName, pool.TrackList[0].MName);
			}
			if (pool.TrackList != null && pool.TrackList.Count >= 1 && mCheckGroupModel.MTrackItem1 != CheckTypeStatusEnum.Optional && mCheckGroupModel.MTrackItem1 != CheckTypeStatusEnum.Required && !string.IsNullOrWhiteSpace(gLCheckGroupValueModel.MTrackItem1))
			{
				if (fix)
				{
					gLCheckGroupValueModel.MTrackItem1 = null;
					entry.MModified = true;
				}
				else
				{
					entry.Validate(ctx, true, "AccountWithTrackDimensionDisabled", "科目[{0}]对应的跟踪项[{1}]维度未启用，但是凭证分录却设置了相应的值", LangModule.GL, account.MFullName, pool.TrackList[0].MName);
				}
			}
			if (pool.TrackList != null && pool.TrackList.Count >= 2 && mCheckGroupModel.MTrackItem2 == CheckTypeStatusEnum.Required && string.IsNullOrWhiteSpace(gLCheckGroupValueModel.MTrackItem2))
			{
				entry.Validate(ctx, true, "AccountWithTrackDimensionRequired", "科目[{0}]对应的跟踪项[{1}]维度是必录，但是凭证分录却未设置相应的值", LangModule.GL, account.MFullName, pool.TrackList[1].MName);
			}
			if (pool.TrackList != null && pool.TrackList.Count >= 2 && mCheckGroupModel.MTrackItem2 != CheckTypeStatusEnum.Optional && mCheckGroupModel.MTrackItem2 != CheckTypeStatusEnum.Required && !string.IsNullOrWhiteSpace(gLCheckGroupValueModel.MTrackItem2))
			{
				if (fix)
				{
					gLCheckGroupValueModel.MTrackItem2 = null;
					entry.MModified = true;
				}
				else
				{
					entry.Validate(ctx, true, "AccountWithTrackDimensionDisabled", "科目[{0}]对应的跟踪项[{1}]维度未启用，但是凭证分录却设置了相应的值", LangModule.GL, account.MFullName, pool.TrackList[1].MName);
				}
			}
			if (pool.TrackList != null && pool.TrackList.Count >= 3 && mCheckGroupModel.MTrackItem3 == CheckTypeStatusEnum.Required && string.IsNullOrWhiteSpace(gLCheckGroupValueModel.MTrackItem3))
			{
				entry.Validate(ctx, true, "AccountWithTrackDimensionRequired", "科目[{0}]对应的跟踪项[{1}]维度是必录，但是凭证分录却未设置相应的值", LangModule.GL, account.MFullName, pool.TrackList[2].MName);
			}
			if (pool.TrackList != null && pool.TrackList.Count >= 3 && mCheckGroupModel.MTrackItem3 != CheckTypeStatusEnum.Optional && mCheckGroupModel.MTrackItem3 != CheckTypeStatusEnum.Required && !string.IsNullOrWhiteSpace(gLCheckGroupValueModel.MTrackItem3))
			{
				if (fix)
				{
					gLCheckGroupValueModel.MTrackItem3 = null;
					entry.MModified = true;
				}
				else
				{
					entry.Validate(ctx, true, "AccountWithTrackDimensionDisabled", "科目[{0}]对应的跟踪项[{1}]维度未启用，但是凭证分录却设置了相应的值", LangModule.GL, account.MFullName, pool.TrackList[2].MName);
				}
			}
			if (pool.TrackList != null && pool.TrackList.Count >= 4 && mCheckGroupModel.MTrackItem4 == CheckTypeStatusEnum.Required && string.IsNullOrWhiteSpace(gLCheckGroupValueModel.MTrackItem4))
			{
				entry.Validate(ctx, true, "AccountWithTrackDimensionRequired", "科目[{0}]对应的跟踪项[1}]维度是必录，但是凭证分录却未设置相应的值", LangModule.GL, account.MFullName, pool.TrackList[3].MName);
			}
			if (pool.TrackList != null && pool.TrackList.Count >= 4 && mCheckGroupModel.MTrackItem4 != CheckTypeStatusEnum.Optional && mCheckGroupModel.MTrackItem4 != CheckTypeStatusEnum.Required && !string.IsNullOrWhiteSpace(gLCheckGroupValueModel.MTrackItem4))
			{
				if (fix)
				{
					gLCheckGroupValueModel.MTrackItem4 = null;
					entry.MModified = true;
				}
				else
				{
					entry.Validate(ctx, true, "AccountWithTrackDimensionDisabled", "科目[{0}]对应的跟踪项[{1}]维度未启用，但是凭证分录却设置了相应的值", LangModule.GL, account.MFullName, pool.TrackList[3].MName);
				}
			}
			if (pool.TrackList != null && pool.TrackList.Count >= 5 && mCheckGroupModel.MTrackItem5 == CheckTypeStatusEnum.Required && string.IsNullOrWhiteSpace(gLCheckGroupValueModel.MTrackItem5))
			{
				entry.Validate(ctx, true, "AccountWithTrackDimensionRequired", "科目[{0}]对应的跟踪项[{1}]维度是必录，但是凭证分录却未设置相应的值", LangModule.GL, account.MFullName, pool.TrackList[4].MName);
			}
			if (pool.TrackList != null && pool.TrackList.Count >= 5 && mCheckGroupModel.MTrackItem5 != CheckTypeStatusEnum.Optional && mCheckGroupModel.MTrackItem5 != CheckTypeStatusEnum.Required && !string.IsNullOrWhiteSpace(gLCheckGroupValueModel.MTrackItem5))
			{
				if (fix)
				{
					gLCheckGroupValueModel.MTrackItem5 = null;
					entry.MModified = true;
				}
				else
				{
					entry.Validate(ctx, true, "AccountWithTrackDimensionDisabled", "科目[{0}]对应的跟踪项[{1}]维度未启用，但是凭证分录却设置了相应的值", LangModule.GL, account.MFullName, pool.TrackList[4].MName);
				}
			}
			if (fix)
			{
				entry.MCheckGroupValueModel = pool.GetCheckGroupValueModel(gLCheckGroupValueModel);
				entry.MCheckGroupValueID = entry.MCheckGroupValueModel.MItemID;
			}
		}

		private static GLCheckGroupValueModel Clone(GLCheckGroupValueModel value)
		{
			return new GLCheckGroupValueModel
			{
				MContactID = value.MContactID,
				MEmployeeID = value.MEmployeeID,
				MMerItemID = value.MMerItemID,
				MExpItemID = value.MExpItemID,
				MPaItemID = value.MPaItemID,
				MTrackItem1 = value.MTrackItem1,
				MTrackItem2 = value.MTrackItem2,
				MTrackItem3 = value.MTrackItem3,
				MTrackItem4 = value.MTrackItem4,
				MTrackItem5 = value.MTrackItem5
			};
		}

		public GLVoucherModel GetVoucherModel(MContext ctx, string itemID, bool includeDraft = false)
		{
			string format = "\r\n                SELECT \r\n                t1.MOrgID,\r\n                t1.MEntryID,\r\n                t1.MID,\r\n                t1.MExplanation,\r\n                t1.MAmount,\r\n                t1.MAmountFor,\r\n                t1.MAccountID,\r\n                t1.MCheckGroupValueID,\r\n                t1.MCurrencyID,\r\n                t1.MExchangeRate,\r\n                t1.MDC,\r\n                t1.MDebit,\r\n                t1.MCredit,\r\n                t1.MEntrySeq,\r\n                t2.MRVoucherID,\r\n                t30.MItemID as MOVoucherID,\r\n                t2.MNumber,\r\n                t2.MDate,\r\n                t2.MYear,\r\n                t2.MPeriod,\r\n                t2.MAttachments,\r\n                case when ifnull(t31.MTransferTypeID,'')='' then -1 else CAST(t31.MTransferTypeID as SIGNED INTEGER) end MTransferTypeID,\r\n                t2.MDebittotal,\r\n                t2.MCreditTotal,\r\n                t2.MSourceBillKey,\r\n                t2.MStatus,\r\n                t3.MNumber AS MAccountNumber,\r\n                (CASE\r\n                    WHEN LENGTH(IFNULL(t3_0.MItemID, '')) > 0 THEN '1'\r\n                    ELSE '0'\r\n                END) AS MIsBankAccount,\r\n                t3.MIsCheckForCurrency,\r\n                t3.MCheckGroupID,\r\n                t3.MDC AS MAccountDC,\r\n                t3_1.MContactID AS MCheckGroupContactID,\r\n                t3_1.MEmployeeID AS MCheckGroupEmployeeID,\r\n                t3_1.MMerItemID AS MCheckGroupMerItemID,\r\n                t3_1.MExpItemID AS MCheckGroupExpItemID,\r\n                t3_1.MPaItemID AS MCheckGroupPaItemID,\r\n                t3_1.MTrackItem1 AS MCheckGroupTrackItem1,\r\n                t3_1.MTrackItem2 AS MCheckGroupTrackItem2,\r\n                t3_1.MTrackItem3 AS MCheckGroupTrackItem3,\r\n                t3_1.MTrackItem4 AS MCheckGroupTrackItem4,\r\n                t3_1.MTrackItem5 AS MCheckGroupTrackItem5,\r\n                t4.MFullName AS MAccountName,\r\n                t5.MContactID,\r\n                t5.MEmployeeID,\r\n                t5.MMerItemID,\r\n                t5.MExpItemID,\r\n                t5.MPaItemID,\r\n                t5.MTrackItem1,\r\n                t5.MTrackItem2,\r\n                t5.MTrackItem3,\r\n                t5.MTrackItem4,\r\n                t5.MTrackItem5,\r\n                CONVERT( AES_DECRYPT(t6.MName, '{0}') USING UTF8) AS MContactName,\r\n                F_GETUSERNAME(t7.MFirstName, t7.MLastName) AS MEmployeeName,\r\n                concat(t8_0.MNumber,':',t8.MDesc) AS MMerItemName,\r\n                t9.MName AS MExpItemName,\r\n                t10.MName AS MPaItemName,\r\n                t10_0.MGroupID AS MPaItemGroupID,\r\n                t10_1.MName AS MPaItemGroupName,\r\n                t11.MItemID AS MTrackItem1GroupID,\r\n                t12.MName AS MTrackItem1GroupName,\r\n                t13.MName AS MTrackItem1Name,\r\n                t14.MItemID AS MTrackItem2GroupID,\r\n                t15.MName AS MTrackItem2GroupName,\r\n                t16.MName AS MTrackItem2Name,\r\n                t17.MItemID AS MTrackItem3GroupID,\r\n                t18.MName AS MTrackItem3GroupName,\r\n                t19.MName AS MTrackItem3Name,\r\n                t20.MItemID AS MTrackItem4GroupID,\r\n                t21.MName AS MTrackItem4GroupName,\r\n                t22.MName AS MTrackItem4Name,\r\n                t23.MItemID AS MTrackItem5GroupID,\r\n                t24.MName AS MTrackItem5GroupName,\r\n                t25.MName AS MTrackItem5Name,\r\n                t26.MItemID AS MDocVoucherID,\r\n                t26.MDocID,\r\n                t26.MDocType,\r\n                F_GETUSERNAME(t27.MFristName, t27.MLastName) AS MCreatorName,\r\n                t28.MStatus as MSettlementStatus\r\n            FROM\r\n                t_gl_voucherentry t1\r\n                    INNER JOIN\r\n                t_gl_voucher t2 ON t1.MID = t2.MItemID\r\n                    AND t2.MOrgID = t1.MOrgID\r\n                    AND t2.MIsDelete = t1.MIsDelete \r\n                    {1} JOIN\r\n                t_bd_account t3 ON t3.MItemID = t1.MAccountID\r\n                    AND t3.MOrgID = t1.MOrgID\r\n                    AND t3.MIsDelete = t1.MIsDelete\r\n                    {1} JOIN\r\n                t_bd_account_l t4 ON t4.MParentID = t3.MItemID\r\n                    AND t3.MOrgID = t1.MOrgID\r\n                    AND t3.MIsDelete = t1.MIsDelete\r\n                    AND t4.MLocaleID = @MLocaleID\r\n                    {1} JOIN\r\n                t_gl_checkgroupvalue t5 ON t5.MItemID = t1.MCheckGroupValueID\r\n                    AND t5.MOrgID = t1.MOrgID\r\n                    AND t5.MIsDelete = t1.MIsDelete \r\n                    LEFT JOIN\r\n                t_bd_bankaccount t3_0 ON t3_0.MItemID = t1.MAccountID\r\n                    AND t3_0.MOrgID = t1.MOrgID\r\n                    AND t3_0.MIsDelete = t1.MIsDelete\r\n                    LEFT JOIN\r\n                t_gl_checkgroup t3_1 ON t3_1.MItemID = t3.MCheckGroupID\r\n                    AND t3_1.MIsDelete = t1.MIsDelete \r\n                    LEFT JOIN\r\n                t_bd_contacts_l t6 ON t6.MParentID = t5.MContactID\r\n                    AND t6.MOrgID = t1.MOrgId\r\n                    AND t6.MIsDelete = t1.MIsDelete\r\n                    AND t6.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_employees_l t7 ON t7.MParentID = t5.MEmployeeID\r\n                    AND t7.MOrgID = t1.MOrgId\r\n                    AND t7.MIsDelete = t1.MIsDelete\r\n                    AND t7.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_item t8_0 ON t8_0.MItemID = t5.MMerItemID\r\n                    AND t8_0.MOrgID = t1.MOrgId\r\n                    AND t8_0.MIsDelete = t1.MIsDelete\r\n                    LEFT JOIN\r\n                t_bd_item_l t8 ON t8.MParentID = t5.MMerItemID\r\n                    AND t8.MOrgID = t1.MOrgId\r\n                    AND t8.MIsDelete = t1.MIsDelete\r\n                    AND t8.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_expenseitem_l t9 ON t9.MParentID = t5.MExpItemID\r\n                    AND t9.MOrgID = t1.MOrgId\r\n                    AND t9.MIsDelete = t1.MIsDelete\r\n                    AND t9.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_pa_payitem_l t10 ON t10.MParentID = t5.MPaItemID\r\n                    AND t10.MOrgID = t1.MOrgId\r\n                    AND t10.MIsDelete = t1.MIsDelete\r\n                    AND t10.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_pa_payitem t10_0 ON t10_0.MItemID = t5.MPaItemID\r\n                    AND t10_0.MOrgID = t1.MOrgId\r\n                    AND t10_0.MIsDelete = t1.MIsDelete\r\n                    LEFT JOIN\r\n                t_pa_payitemgroup_l t10_1 ON t10_1.MParentID = t5.MPaItemID\r\n                    AND t10_1.MOrgID = t1.MOrgId\r\n                    AND t10_1.MIsDelete = t1.MIsDelete\r\n                    AND t10_1.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_trackentry t11 ON t11.MEntryID = t5.MTrackItem1\r\n                    AND t11.MOrgID = t1.MOrgID\r\n                    AND t11.MIsDelete = t1.MIsDelete\r\n                    LEFT JOIN\r\n                t_bd_track_l t12 ON t12.MParentID = t11.MItemID\r\n                    AND t12.MOrgID = t1.MOrgID\r\n                    AND t12.MIsDelete = t1.MIsDelete\r\n                    AND t12.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_trackentry_l t13 ON t13.MParentID = t5.MTrackItem1\r\n                    AND t13.MOrgID = t1.MOrgId\r\n                    AND t13.MIsDelete = t1.MIsDelete\r\n                    AND t13.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_trackentry t14 ON t14.MEntryID = t5.MTrackItem2\r\n                    AND t14.MOrgID = t1.MOrgID\r\n                    AND t14.MIsDelete = t1.MIsDelete\r\n                    LEFT JOIN\r\n                t_bd_track_l t15 ON t15.MParentID = t14.MItemID\r\n                    AND t15.MOrgID = t1.MOrgID\r\n                    AND t15.MIsDelete = t1.MIsDelete\r\n                    AND t15.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_trackentry_l t16 ON t16.MParentID = t5.MTrackItem2\r\n                    AND t16.MOrgID = t1.MOrgId\r\n                    AND t16.MIsDelete = t1.MIsDelete\r\n                    AND t16.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_trackentry t17 ON t17.MEntryID = t5.MTrackItem3\r\n                    AND t17.MOrgID = t1.MOrgID\r\n                    AND t17.MIsDelete = t1.MIsDelete\r\n                    LEFT JOIN\r\n                t_bd_track_l t18 ON t18.MParentID = t17.MItemID\r\n                    AND t18.MOrgID = t1.MOrgID\r\n                    AND t18.MIsDelete = t1.MIsDelete\r\n                    AND t18.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_trackentry_l t19 ON t19.MParentID = t5.MTrackItem3\r\n                    AND t19.MOrgID = t1.MOrgId\r\n                    AND t19.MIsDelete = t1.MIsDelete\r\n                    AND t19.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_trackentry t20 ON t20.MEntryID = t5.MTrackItem4\r\n                    AND t20.MOrgID = t1.MOrgID\r\n                    AND t20.MIsDelete = t1.MIsDelete\r\n                    LEFT JOIN\r\n                t_bd_track_l t21 ON t21.MParentID = t20.MItemID\r\n                    AND t21.MOrgID = t1.MOrgID\r\n                    AND t21.MIsDelete = t1.MIsDelete\r\n                    AND t21.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_trackentry_l t22 ON t22.MParentID = t5.MTrackItem4\r\n                    AND t22.MOrgID = t1.MOrgID\r\n                    AND t22.MIsDelete = t1.MIsDelete\r\n                    AND t22.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_trackentry t23 ON t23.MEntryID = t5.MTrackItem5\r\n                    AND t23.MOrgID = t1.MOrgID\r\n                    AND t23.MIsDelete = t1.MIsDelete\r\n                    LEFT JOIN\r\n                t_bd_track_l t24 ON t24.MParentID = t23.MItemID\r\n                    AND t24.MOrgID = t1.MOrgID\r\n                    AND t24.MIsDelete = t1.MIsDelete\r\n                    AND t24.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_trackentry_l t25 ON t25.MParentID = t5.MTrackItem5\r\n                    AND t25.MOrgID = t1.MOrgId\r\n                    AND t25.MIsDelete = t1.MIsDelete\r\n                    AND t25.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                (SELECT \r\n                    MItemID, MVoucherID AS MDocVoucherID, MDocID, MDocType\r\n                FROM\r\n                    t_gl_doc_voucher\r\n                WHERE\r\n                    MOrgID = @MOrgID\r\n                        AND MVoucherID = @MItemID\r\n                        AND MisDelete = 0 \r\n                LIMIT 0 , 1) t26 ON t26.MDocVoucherID = t1.MID\r\n                    LEFT JOIN\r\n                t_sec_user_l t27 ON t1.MCreatorID = t27.MParentID\r\n                    AND t27.MIsDelete = t1.MIsDelete \r\n                    AND t27.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_gl_settlement t28 on t1.MOrgID = t28.MOrgID\r\n                    AND t28.MYear = t2.MYear \r\n                    AND t28.MPeriod = t2.MPeriod\r\n                    AND t28.MIsDelete = t1.MIsDelete \r\n                    LEFT JOIN\r\n               t_gl_voucher t30 on t2.MOrgID = t30.MOrgID\r\n                    and t30.MRVoucherID = t2.MItemID\r\n                    AND t30.MIsDelete = t2.MIsDelete\r\n                left join t_gl_periodtransfer t31 on t1.MID = t31.MVoucherID\r\n                    AND t1.MOrgID = t31.MOrgID\r\n                    AND t1.MIsDelete = t3.MIsDelete \r\n            WHERE\r\n                t1.MOrgID = @MOrgID\r\n                    AND t1.MID = @MItemID\r\n                    AND t1.MIsDelete = 0 \r\n            ORDER BY t1.MEntrySeq ASC\r\n            ";
			format = string.Format(format, "JieNor-001", includeDraft ? " LEFT " : " INNER ");
			DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(format, ctx.GetParameters("@MItemID", itemID));
			if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return null;
			}
			return BindDataset2Voucher(ctx, dataSet.Tables[0]);
		}

		public List<MActionResultCodeEnum> ValidateVouchers(MContext ctx, List<GLVoucherModel> vouchers, int nowStatus = -2, int oldStatus = -2, bool throwException = false, bool clearCheckGroupValue = false, bool validateNumber = true, bool validateCheckGroupValue = true)
		{
			if (vouchers == null || vouchers.Count == 0)
			{
				throw new MActionException
				{
					Codes = new List<MActionResultCodeEnum>
					{
						MActionResultCodeEnum.MVoucherDeleted
					}
				};
			}
			if ((from t in vouchers
			group t by t.MYear * 100 + t.MPeriod).Distinct().Count() > 1 && nowStatus == 1)
			{
				throw new MActionException
				{
					Codes = new List<MActionResultCodeEnum>
					{
						MActionResultCodeEnum.MVoucherSpanApprove
					}
				};
			}
			List<string> ids = (from x in vouchers
			where !x.IsNew && !string.IsNullOrWhiteSpace(x.MItemID)
			select x.MItemID).ToList();
			string otherFilter = (nowStatus != -1) ? " and length(ifnull(MNumber,'')) > 0 " : "";
			ValidateQueryModel validateCommonModelSql = utility.GetValidateCommonModelSql<GLVoucherModel>(MActionResultCodeEnum.MVoucherDeleted, ids, null, otherFilter);
			ValidateQueryModel validateQueryModel = (vouchers.Exists((GLVoucherModel x) => x.MStatus == 1) || nowStatus == 1) ? utility.GetValidateInitBalanceOverSql(ctx, true) : new ValidateQueryModel();
			int yearPeriod = vouchers.Max((GLVoucherModel t) => t.MYear * 100 + t.MPeriod);
			ValidateQueryModel validateQueryModel2 = (nowStatus == 1) ? utility.GetValidateHasNotSettledPeriod(ctx, yearPeriod.GetYearByYearPeriod(), yearPeriod.GetPeriodByYearPeriod()) : new ValidateQueryModel();
			int yearPeriod2 = vouchers.Min((GLVoucherModel t) => t.MYear * 100 + t.MPeriod);
			ValidateQueryModel validatePeriodBeforeStartSql = utility.GetValidatePeriodBeforeStartSql(ctx, yearPeriod2.GetYearByYearPeriod(), yearPeriod2.GetPeriodByYearPeriod());
			ValidateQueryModel validatePeriodClosedSql = utility.GetValidatePeriodClosedSql((from t in vouchers
			select t.MYear * 100 + t.MPeriod).ToList());
			ValidateQueryModel validateQueryModel3 = validateNumber ? utility.GetValidateVoucherNumberSql(vouchers) : new ValidateQueryModel();
			ValidateQueryModel validateVoucherApproveSql = utility.GetValidateVoucherApproveSql(vouchers, nowStatus, oldStatus);
			List<int> list = (from f in vouchers
			where f.MPeriodTransfer != null
			select f.MPeriodTransfer.MTransferTypeID).ToList();
			ValidateQueryModel validateQueryModel4 = (!list.Any() || nowStatus == 1) ? new ValidateQueryModel() : utility.GetValidatePeriodTransferSql(vouchers[0].MYear, vouchers[0].MPeriod, list);
			List<GLVoucherEntryModel> source = utility.Union((from x in vouchers
			select x.MVoucherEntrys).ToList());
			List<string> ids2 = (from x in source
			select x.MAccountID).ToList();
			ValidateQueryModel validateCommonModelSql2 = utility.GetValidateCommonModelSql<BDAccountModel>(MActionResultCodeEnum.MAccountInvalid, ids2, null, null);
			ValidateQueryModel validateAccountHasSubSql = utility.GetValidateAccountHasSubSql(ids2, false);
			List<string> ids3 = (from x in source
			select x.MCheckGroupValueModel.MContactID).ToList();
			ValidateQueryModel validateCommonModelSql3 = utility.GetValidateCommonModelSql<BDContactsModel>(MActionResultCodeEnum.MContactInvalid, ids3, null, null);
			List<string> ids4 = (from x in source
			select x.MCheckGroupValueModel.MEmployeeID).ToList();
			ValidateQueryModel validateCommonModelSql4 = utility.GetValidateCommonModelSql<BDEmployeesModel>(MActionResultCodeEnum.MEmployeeInvalid, ids4, null, null);
			List<string> ids5 = (from x in source
			select x.MCheckGroupValueModel.MMerItemID).ToList();
			ValidateQueryModel validateCommonModelSql5 = utility.GetValidateCommonModelSql<BDItemModel>(MActionResultCodeEnum.MMerItemInvalid, ids5, null, null);
			List<string> ids6 = (from x in source
			select x.MCheckGroupValueModel.MExpItemID).ToList();
			ValidateQueryModel validateCommonModelSql6 = utility.GetValidateCommonModelSql<BDExpenseItemModel>(MActionResultCodeEnum.MExpItemInvalid, ids6, null, null);
			ValidateQueryModel validatExpItemHasSubSql = utility.GetValidatExpItemHasSubSql(ids6);
			List<string> ids7 = (from x in source
			select x.MCheckGroupValueModel into x
			where x.MPaItemGroupID != x.MPaItemID && !string.IsNullOrWhiteSpace(x.MPaItemID)
			select x.MPaItemID).ToList();
			ValidateQueryModel validateCommonModelSql7 = utility.GetValidateCommonModelSql<PAPayItemModel>(MActionResultCodeEnum.MPaItemInvalid, ids7, null, null);
			List<string> ids8 = (from x in source
			select x.MCheckGroupValueModel.MPaItemGroupID).ToList();
			ValidateQueryModel validateCommonModelSql8 = utility.GetValidateCommonModelSql<PAPayItemGroupModel>(MActionResultCodeEnum.MPaItemInvalid, ids8, null, null);
			List<List<string>> list2 = (from x in source
			select new List<string>
			{
				x.MCheckGroupValueModel.MTrackItem1,
				x.MCheckGroupValueModel.MTrackItem2,
				x.MCheckGroupValueModel.MTrackItem3,
				x.MCheckGroupValueModel.MTrackItem4,
				x.MCheckGroupValueModel.MTrackItem5
			}).ToList();
			List<string> ids9 = utility.Union(list2);
			ValidateQueryModel validateCommonModelSql9 = utility.GetValidateCommonModelSql<BDTrackEntryModel>(MActionResultCodeEnum.MTrackItemInvalid, ids9, null, null);
			List<List<string>> list3 = (from x in source
			select new List<string>
			{
				x.MCheckGroupValueModel.MTrackItem1GroupID,
				x.MCheckGroupValueModel.MTrackItem2GroupID,
				x.MCheckGroupValueModel.MTrackItem3GroupID,
				x.MCheckGroupValueModel.MTrackItem4GroupID,
				x.MCheckGroupValueModel.MTrackItem5GroupID
			}).ToList();
			List<string> ids10 = utility.Union(list3);
			ValidateQueryModel validateCommonModelSql10 = utility.GetValidateCommonModelSql<BDTrackModel>(MActionResultCodeEnum.MTrackGroupInvalid, ids10, null, null);
			List<MActionResultCodeEnum> list4 = utility.QueryValidateSql(ctx, true, validateCommonModelSql, validateQueryModel, validatePeriodBeforeStartSql, validatePeriodClosedSql, validateQueryModel2, validateQueryModel3, validateVoucherApproveSql, validateQueryModel4, validateCommonModelSql2, validateAccountHasSubSql, validateCommonModelSql3, validateCommonModelSql4, validateCommonModelSql5, validateCommonModelSql6, validateCommonModelSql7, validateCommonModelSql9);
			for (int i = 0; i < vouchers.Count; i++)
			{
				if (vouchers[i].MVoucherEntrys == null || vouchers[i].MVoucherEntrys.Count == 0)
				{
					list4.Add(MActionResultCodeEnum.MVoucherHasNotEntry);
				}
				else
				{
					if (vouchers[i].MVoucherEntrys.Sum((GLVoucherEntryModel x) => x.MDebit) != vouchers[i].MVoucherEntrys.Sum((GLVoucherEntryModel x) => x.MCredit))
					{
						list4.Add(MActionResultCodeEnum.MCreditDebitImbalance);
					}
					if (!string.IsNullOrWhiteSpace(vouchers[i].MNumber) && vouchers[i].MStatus != -1 && vouchers[i].MVoucherEntrys.Exists((GLVoucherEntryModel x) => string.IsNullOrWhiteSpace(x.MAccountID)))
					{
						list4.Add(MActionResultCodeEnum.MVoucherEntryHasNotAccountOrAccountNotMatchCheckGroup);
					}
				}
			}
			if (validateCheckGroupValue)
			{
				utility.CheckVoucherCheckGroupValueMatchCheckGroup(ctx, vouchers, clearCheckGroupValue);
			}
			if (!utility.CheckVoucheCurrencyMatchAccount(ctx, vouchers, clearCheckGroupValue))
			{
				throw new MActionException
				{
					Codes = new List<MActionResultCodeEnum>
					{
						MActionResultCodeEnum.MCurrencyNotMatchAccount
					}
				};
			}
			if (list4.Count > 0 & throwException)
			{
				throw new MActionException
				{
					Codes = list4
				};
			}
			return list4;
		}

		public List<MActionResultCodeEnum> ValidateDeleteVouchers(MContext ctx, List<string> pkIDS)
		{
			if (pkIDS == null || pkIDS.Count == 0)
			{
				return null;
			}
			List<GLVoucherModel> vouchers = (from x in pkIDS
			select new GLVoucherModel
			{
				MItemID = x
			}).Distinct().ToList();
			ValidateQueryModel validateCommonModelSql = utility.GetValidateCommonModelSql<GLVoucherModel>(MActionResultCodeEnum.MVoucherInvalid, pkIDS, null, null);
			ValidateQueryModel validateVoucherApproveSql = utility.GetValidateVoucherApproveSql(vouchers, 1, 0);
			ValidateQueryModel validateExitsCreatedDepreciationVoucher = utility.GetValidateExitsCreatedDepreciationVoucher(ctx, pkIDS);
			ValidateQueryModel validateExitsChange = utility.GetValidateExitsChange(ctx, pkIDS);
			return utility.QueryValidateSql(ctx, new List<ValidateQueryModel>
			{
				validateCommonModelSql,
				validateVoucherApproveSql,
				validateExitsChange,
				validateExitsCreatedDepreciationVoucher
			}, true);
		}

		public List<CommandInfo> GetDeleteVoucherRelatedCmds(MContext ctx, List<string> voucherIDs)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			List<CommandInfo> list2 = new List<CommandInfo>();
			List<CommandInfo> collection = new List<CommandInfo>();
			List<CommandInfo> collection2 = new List<CommandInfo>();
			list2 = GLPeriodTransferRepository.GetDeleteCmdByVoucherID(ctx, voucherIDs);
			if (ctx.MFABeginDate > new DateTime(1900, 1, 1))
			{
				collection = new FADepreciationRepository().GetDeleteDepreciatedVoucherCmds(ctx, voucherIDs);
			}
			if (ctx.MOrgVersionID == 1)
			{
				collection2 = new FPFapiaoRepository().GetDeleteFapiaoVoucherCmds(ctx, voucherIDs);
			}
			list.AddRange(list2);
			list.AddRange(collection);
			list.AddRange(collection2);
			return list;
		}

		private DateTime GetDefaultVoucherDate(MContext ctx)
		{
			DateTime dateTime = ctx.MGLBeginDate;
			DateTime dateTime2 = dateTime.AddMonths(-1);
			List<DateTime> nextSettledPeriod = new GLSettlementRepository().GetNextSettledPeriod(ctx, new GLSettlementModel
			{
				MOrgID = ctx.MOrgID,
				MYear = dateTime2.Year,
				MPeriod = dateTime2.Month
			});
			DateTime result;
			if (nextSettledPeriod == null || nextSettledPeriod.Count == 0)
			{
				result = ctx.MGLBeginDate;
			}
			else
			{
				dateTime = nextSettledPeriod.Max();
				result = dateTime.AddMonths(1);
			}
			int year = result.Year;
			dateTime = ctx.DateNow;
			int num;
			if (year == dateTime.Year)
			{
				int month = result.Month;
				dateTime = ctx.DateNow;
				num = ((month == dateTime.Month) ? 1 : 0);
			}
			else
			{
				num = 0;
			}
			if (num != 0)
			{
				result = ctx.DateNow;
			}
			else
			{
				int num2 = result.Year * 100 + result.Month;
				dateTime = ctx.DateNow;
				int num3 = dateTime.Year * 100;
				dateTime = ctx.DateNow;
				if (num2 > num3 + dateTime.Month)
				{
					result = new DateTime(result.Year, result.Month, 1);
				}
				else
				{
					dateTime = new DateTime(result.Year, result.Month, 1);
					dateTime = dateTime.AddMonths(1);
					result = dateTime.AddDays(-1.0);
				}
			}
			return result;
		}

		public void PreHandleVouchers(MContext ctx, List<GLVoucherModel> vouchers)
		{
			vouchers.ForEach(delegate(GLVoucherModel x)
			{
				x.MOrgID = ctx.MOrgID;
				x.MNumber = ((!string.IsNullOrWhiteSpace(x.MNumber)) ? COMResourceHelper.ToVoucherNumber(ctx, x.MNumber, 0) : x.MNumber);
				DateTime dateTime = x.MDate;
				if (dateTime.Year <= 1900)
				{
					if (x.MYear > 1900 && x.MPeriod != 0)
					{
						int num = x.MYear * 12 + x.MPeriod;
						dateTime = ctx.DateNow;
						int num2 = dateTime.Year * 12;
						dateTime = ctx.DateNow;
						if (num > num2 + dateTime.Month)
						{
							x.MDate = new DateTime(x.MYear, x.MPeriod, 1);
						}
						else
						{
							int num3 = x.MYear * 12 + x.MPeriod;
							dateTime = ctx.DateNow;
							int num4 = dateTime.Year * 12;
							dateTime = ctx.DateNow;
							if (num3 == num4 + dateTime.Month)
							{
								x.MDate = ctx.DateNow;
							}
							else
							{
								dateTime = new DateTime(x.MYear, x.MPeriod, 1);
								dateTime = dateTime.AddMonths(1);
								x.MDate = dateTime.AddDays(-1.0);
							}
						}
					}
					else
					{
						x.MDate = GetDefaultVoucherDate(ctx);
					}
				}
				dateTime = x.MDate;
				x.MYear = dateTime.Year;
				dateTime = x.MDate;
				x.MPeriod = dateTime.Month;
				x.MVoucherEntrys = (x.MVoucherEntrys ?? new List<GLVoucherEntryModel>());
				x.MCreatorName = (string.IsNullOrWhiteSpace(x.MCreatorName) ? GlobalFormat.GetUserName(ctx.MFirstName, ctx.MLastName, null) : x.MCreatorName);
				x.MVoucherEntrys.ForEach(delegate(GLVoucherEntryModel y)
				{
					y.MOrgID = ctx.MOrgID;
					y.MCurrencyID = (string.IsNullOrWhiteSpace(y.MCurrencyID) ? ctx.MBasCurrencyID : y.MCurrencyID);
					y.MExchangeRate = ((y.MCurrencyID == ctx.MBasCurrencyID) ? 1.0m : y.MExchangeRate);
					y.MAmount = ((y.MDC == 1) ? y.MDebit : y.MCredit);
					y.IsNew = string.IsNullOrWhiteSpace(y.MEntryID);
					y.MCheckGroupValueModel = (y.MCheckGroupValueModel ?? new GLCheckGroupValueModel());
					if (y.MCheckGroupValueModel != null)
					{
						y.MCheckGroupValueModel.MOrgID = ctx.MOrgID;
					}
				});
			});
		}

		private List<CommandInfo> GetInsertCheckGroupValue(MContext ctx, List<GLVoucherModel> vouchers)
		{
			List<CommandInfo> result = new List<CommandInfo>();
			List<GLVoucherEntryModel> list = utility.Union((from x in vouchers
			select x.MVoucherEntrys).ToList());
			for (int i = 0; i < list.Count; i++)
			{
				if (string.IsNullOrEmpty(list[i].MCheckGroupValueModel.MItemID))
				{
					list[i].MCheckGroupValueModel.MItemID = utility.GetCheckGroupValueModel(ctx, list[i].MCheckGroupValueModel).MItemID;
				}
				list[i].MCheckGroupValueID = list[i].MCheckGroupValueModel.MItemID;
			}
			return result;
		}

		public OperationResult UpdateVoucher(MContext ctx, GLVoucherModel voucher, List<string> numberList = null)
		{
			OperationResult operationResult = new OperationResult
			{
				Success = true
			};
			PreHandleVouchers(ctx, new List<GLVoucherModel>
			{
				voucher
			});
			if (!string.IsNullOrWhiteSpace(voucher.MDocID))
			{
				if (string.IsNullOrWhiteSpace(voucher.MNumber))
				{
					return new GLVoucherEntryRepository().UpdateVoucherEntrys(ctx, voucher);
				}
				voucher.MVoucherEntrys.ForEach(delegate(GLVoucherEntryModel x)
				{
					x.MCheckGroupValueID = utility.GetCheckGroupValueModel(ctx, x.MCheckGroupValueModel).MItemID;
				});
				new GLUtility().CheckVoucherCheckGroupValueMatchCheckGroup(ctx, new List<GLVoucherModel>
				{
					voucher
				}, true);
			}
			if (!string.IsNullOrWhiteSpace(voucher.MNumber))
			{
				voucher.MNumber = COMResourceHelper.ToVoucherNumber(ctx, voucher.MNumber, 0);
				COMResourceHelper.TryLockVoucherNumber(ctx, voucher.MYear, voucher.MPeriod, new List<string>
				{
					voucher.MNumber
				});
			}
			int mStatus = voucher.MStatus;
			int oldStatus = (voucher.MStatus != 1) ? (-2) : 0;
			List<MActionResultCodeEnum> list = ValidateVouchers(ctx, new List<GLVoucherModel>
			{
				voucher
			}, mStatus, oldStatus, false, false, true, true);
			if (list.Count > 0)
			{
				List<string> actionExceptionsMessagesByCodes = GLInterfaceRepository.GetActionExceptionsMessagesByCodes(ctx, list);
				voucher.ErrorMessage = string.Join(",", actionExceptionsMessagesByCodes);
				operationResult.Success = false;
			}
			else
			{
				List<CommandInfo> updateVoucherCommandList = GetUpdateVoucherCommandList(ctx, voucher, numberList);
				updateVoucherCommandList.AddRange(new GLVoucherReferenceRepository().GetInsertReferenceCmds(ctx, (from x in voucher.MVoucherEntrys
				select x.MExplanation into x
				where !string.IsNullOrWhiteSpace(x)
				select x).Distinct().ToList()));
				operationResult.Success = (new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(updateVoucherCommandList) > 0);
			}
			return operationResult;
		}

		public OperationResult UpdateVouchers(MContext ctx, List<GLVoucherModel> vouchers, List<CommandInfo> basCmdList = null, int nowStatus = -2, int oldStatus = -2, bool validateCheckGroupValue = true, List<CommandInfo> sysCmdList = null, bool throwValidateResult = false)
		{
			basCmdList = (basCmdList ?? new List<CommandInfo>());
			List<string> numberList = (from f in vouchers
			select f.MNumber into x
			where !string.IsNullOrWhiteSpace(x)
			select x).ToList();
			PreHandleVouchers(ctx, vouchers);
			List<MActionResultCodeEnum> validate = ValidateVouchers(ctx, vouchers, nowStatus, oldStatus, false, false, true, validateCheckGroupValue);
			HandleVoucherValidateResult(ctx, vouchers, throwValidateResult, validate);
			for (int i = 0; i < vouchers.Count; i++)
			{
				basCmdList.AddRange(GetUpdateVoucherCommandList(ctx, vouchers[i], numberList));
			}
			List<MultiDBCommand> list = new List<MultiDBCommand>();
			list.Add(new MultiDBCommand(ctx)
			{
				CommandList = basCmdList,
				DBType = SysOrBas.Bas
			});
			if (sysCmdList?.Any() ?? false)
			{
				list.Add(new MultiDBCommand(ctx)
				{
					CommandList = sysCmdList,
					DBType = SysOrBas.Sys
				});
			}
			return new OperationResult
			{
				Success = DbHelperMySQL.ExecuteSqlTran(ctx, list.ToArray())
			};
		}

		public List<MultiDBCommand> GetUpdateVouchersCmd(MContext ctx, List<GLVoucherModel> vouchers, List<CommandInfo> basCmdList = null, int nowStatus = -2, int oldStatus = -2, bool validateCheckGroupValue = true, List<CommandInfo> sysCmdList = null, bool throwValidateResult = false)
		{
			basCmdList = (basCmdList ?? new List<CommandInfo>());
			List<string> numberList = (from f in vouchers
			select f.MNumber into x
			where !string.IsNullOrWhiteSpace(x)
			select x).ToList();
			PreHandleVouchers(ctx, vouchers);
			List<MActionResultCodeEnum> validate = ValidateVouchers(ctx, vouchers, nowStatus, oldStatus, false, false, true, validateCheckGroupValue);
			HandleVoucherValidateResult(ctx, vouchers, throwValidateResult, validate);
			for (int i = 0; i < vouchers.Count; i++)
			{
				basCmdList.AddRange(GetUpdateVoucherCommandList(ctx, vouchers[i], numberList));
			}
			List<MultiDBCommand> list = new List<MultiDBCommand>();
			list.Add(new MultiDBCommand(ctx)
			{
				CommandList = basCmdList,
				DBType = SysOrBas.Bas
			});
			if (sysCmdList?.Any() ?? false)
			{
				list.Add(new MultiDBCommand(ctx)
				{
					CommandList = sysCmdList,
					DBType = SysOrBas.Sys
				});
			}
			return list;
		}

		private static void HandleVoucherValidateResult(MContext ctx, List<GLVoucherModel> vouchers, bool throwValidateResult, List<MActionResultCodeEnum> validate)
		{
			if (!throwValidateResult || !validate.Any())
			{
				return;
			}
			MActionException ex = new MActionException();
			ex.Messages = new List<string>();
			if (validate.Contains(MActionResultCodeEnum.MCreditDebitImbalance))
			{
				validate.Remove(MActionResultCodeEnum.MCreditDebitImbalance);
				IEnumerable<GLVoucherModel> source = from f in vouchers
				where f.MVoucherEntrys.Sum((GLVoucherEntryModel c) => c.MCredit) != f.MVoucherEntrys.Sum((GLVoucherEntryModel d) => d.MDebit)
				select f;
				if (source.Any())
				{
					ex.Messages.Add(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "CreditDebitImbalance", "凭证分录借贷方不平衡") + string.Format("（{0}）", string.Join("、", from f in source
					select $"{f.MYear}-{f.MPeriod}：{f.MNumber}")));
				}
			}
			ex.Codes = validate;
			throw ex;
		}

		public List<CommandInfo> GetUpdateVoucherCommandList(MContext ctx, GLVoucherModel voucher, List<string> numberList = null)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			if (voucher != null && string.IsNullOrWhiteSpace(voucher.MDocID))
			{
				list.AddRange(GLVoucherEntryRepository.GetDeleteVoucherEntryCmd(ctx, new List<string>
				{
					voucher.MItemID
				}));
				voucher.IsNew = string.IsNullOrWhiteSpace(voucher.MItemID);
				voucher.MVoucherEntrys.ForEach(delegate(GLVoucherEntryModel x)
				{
					x.IsNew = true;
					x.MEntryID = null;
				});
				if (voucher.MPeriodTransfer != null && !string.IsNullOrWhiteSpace(voucher.MPeriodTransfer.MItemID))
				{
					voucher.MPeriodTransfer.MAmount = voucher.MDebitTotal;
					list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<GLPeriodTransferModel>(ctx, voucher.MPeriodTransfer, new List<string>
					{
						"MAmount"
					}, true));
				}
			}
			list.AddRange(GetInsertCheckGroupValue(ctx, new List<GLVoucherModel>
			{
				voucher
			}));
			list.AddRange(GetInsertVoucherCmds(ctx, voucher, null));
			list.AddRange(GetUpdateReverseVoucherCmds(ctx, voucher));
			list.AddRange(GlVoucherLogHelper.GetSaveLog(ctx, voucher));
			if (voucher.MPeriodTransfer != null)
			{
				voucher.MPeriodTransfer.MVoucherID = voucher.MItemID;
				voucher.MPeriodTransfer.MYear = voucher.MYear;
				voucher.MPeriodTransfer.MPeriod = voucher.MPeriod;
				voucher.MPeriodTransfer.MOrgID = ctx.MOrgID;
				voucher.MPeriodTransfer.MAmount = voucher.MDebitTotal;
				list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<GLPeriodTransferModel>(ctx, voucher.MPeriodTransfer, null, true));
			}
			if (voucher.MStatus == 1)
			{
				voucher.MStatus = 0;
				list.AddRange(GetApproveVouchersCmd(ctx, null, 1.ToString(), new List<GLVoucherModel>
				{
					voucher
				}));
				voucher.MStatus = 1;
			}
			return list;
		}

		public List<CommandInfo> GetUpdateReverseVoucherCmds(MContext ctx, GLVoucherModel voucher)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			if (voucher.MIsReverse && !string.IsNullOrWhiteSpace(voucher.MOVoucherID))
			{
				string mOVoucherID = voucher.MOVoucherID;
				voucher.MRVoucherID = null;
				SqlWhere sqlWhere = new SqlWhere();
				sqlWhere.Equal("MItemID", mOVoucherID);
				GLVoucherModel dataModelByFilter = GetDataModelByFilter(ctx, sqlWhere);
				if (dataModelByFilter != null)
				{
					List<MySqlParameter> list2 = ctx.GetParameters((MySqlParameter)null).ToList();
					list2.Add(new MySqlParameter("@MOVoucherID", voucher.MItemID));
					list2.Add(new MySqlParameter("@MVoucherID", mOVoucherID));
					List<CommandInfo> list3 = list;
					CommandInfo obj = new CommandInfo
					{
						CommandText = "update t_gl_voucher set MRVoucherID = @MOVoucherID where MOrgID = @MOrgID and MIsDelete = 0 and MItemID = @MVoucherID"
					};
					DbParameter[] array = obj.Parameters = list2.ToArray();
					list3.Add(obj);
					list.AddRange(GlVoucherLogHelper.GetCreateReverseCmd(ctx, voucher));
					list.AddRange(GlVoucherLogHelper.GetApplyReverseCmd(ctx, voucher));
				}
			}
			return list;
		}

		public GLDashboardModel GetDashboardData(MContext ctx, int year, int period, int type = 0)
		{
			string str = "\r\n                        SELECT \r\n                            (CASE\r\n                                WHEN\r\n                                    EXISTS( SELECT \r\n                                            mstatus\r\n                                        FROM\r\n                                            t_gl_settlement\r\n                                        WHERE\r\n                                            morgid = @MOrgID\r\n                                                AND MYear = @MYear\r\n                                                AND mperiod = @MPeriod\r\n                                                AND MisDelete = 0 \r\n                                                AND MStatus = 1)\r\n                                THEN\r\n                                    '1'\r\n                                ELSE '0'\r\n                            END) AS MValue ";
			str += " UNION ALL ";
			str += " SELECT\r\n                            (CASE\r\n                                WHEN (@MYear * 12 + @MPeriod) = (@MBeginYear * 12 +  @MBeginPeriod) THEN '1'\r\n                                ELSE(CASE\r\n                                    WHEN COUNT(MStatus) >= 1 THEN '1'\r\n                                    ELSE '0'\r\n                                END)\r\n                            END) AS MValue\r\n                            FROM\r\n                                t_gl_settlement\r\n                            WHERE\r\n                                morgid = @MOrgID\r\n                                    AND (MYear * 12 + MPeriod) = (@MYear * 12 +  @MPeriod - 1)\r\n                                    AND MisDelete = 0 \r\n                                    AND MStatus = 1\r\n                                    AND MisDelete = 0  ";
			str += " UNION ALL ";
			str += " SELECT\r\n                            (CASE\r\n                                WHEN\r\n                                    EXISTS(SELECT\r\n                                            t1.MStatus AS MValue\r\n                                        FROM\r\n                                            t_gl_voucher t1\r\n                                                INNER JOIN\r\n                                            t_gl_periodtransfer t2 ON t1.MItemId = t2.MVoucherId\r\n                                                AND t1.MOrgID = t2.MOrgId\r\n                                                AND t1.MIsDelete = t2.MIsDelete \r\n                                                AND t1.MYear = t2.MYear\r\n                                                AND t2.MPeriod = t2.MPeriod\r\n                                        WHERE\r\n                                            t1.morgid = @MOrgID\r\n                                                AND t1.MIsDelete = 0 \r\n                                                AND t2.MTransferTypeID = 7\r\n                                                AND t1.MYear = @MYear\r\n                                                AND t1.mperiod = @MPeriod)\r\n                                THEN\r\n                                    '1'\r\n                                ELSE '0'\r\n                            END) AS MValue";
			str += " UNION ALL ";
			str += " select* from (SELECT\r\n                            COUNT(t1.MItemID) AS MValue\r\n                        FROM\r\n                            t_gl_voucher t1\r\n                        WHERE\r\n                            t1.MOrgID = @MOrgID\r\n                                AND t1.MYear = @MYear\r\n                                AND t1.mperiod = @MPeriod\r\n                                AND t1.MIsDelete = 0  \r\n                                AND LENGTH(IFNULL(t1.MNumber, '')) > 0\r\n                                AND  (MStatus  = 0 or MStatus = 1) ) x";
			str += " UNION ALL ";
			str += " select *from(SELECT\r\n                           COUNT(t1.MItemID) AS MValue\r\n                       FROM\r\n                           t_gl_voucher t1\r\n                       WHERE\r\n                           t1.MOrgID = @MOrgID\r\n                               AND t1.MYear = @MYear\r\n                               AND t1.mperiod = @MPeriod\r\n                               AND LENGTH(IFNULL(t1.MNumber, '')) > 0\r\n                               AND t1.MIsDelete = 0  \r\n                               AND (t1.MStatus  = 0 or t1.MStatus = 1)\r\n                               AND t1.MStatus = 1) y";
			str += " UNION ALL ";
			str += " select *from( SELECT\r\n                              COUNT(t1.MItemID) AS MValue\r\n                          FROM\r\n                              t_gl_voucher t1\r\n                          WHERE\r\n                              t1.MOrgID = @MOrgID\r\n                                  AND t1.MYear = @MYear\r\n                                  AND t1.mperiod = @MPeriod\r\n                                  AND LENGTH(IFNULL(t1.MNumber, '')) > 0\r\n                                  AND t1.MIsDelete = 0   \r\n                                  AND  (MStatus  = 0 or MStatus = 1)\r\n                                  AND MSourceBillKey = '1') z";
			str += " UNION ALL ";
			str += " select * from ( \r\n\t                            select count(*) AS MValue from(\r\n\t                            SELECT --\r\n                                distinct t2.MDocID \r\n                            FROM\r\n                                t_gl_voucher t1\r\n                                    INNER JOIN\r\n                                t_gl_doc_voucher t2 ON t1.MitemID = t2.MVoucherID\r\n                                    AND t1.MOrgID = t2.MOrgID\r\n                                    AND t1.MIsdelete = t2.MIsDelete \r\n                            WHERE\r\n                                t1.MOrgID = @MOrgID\r\n                                    AND t1.MYear = @MYear\r\n                                    AND t1.mperiod = @MPeriod\r\n                                    AND t1.MIsDelete = 0  \r\n                                    AND LENGTH(IFNULL(t1.MNumber, '')) > 0\r\n                                    AND t1.MStatus != - 1 \r\n                                    and (t2.MergeStatus = 0 Or t2.MergeStatus = 1))xx )x";
			str += " UNION ALL ";
			str += " select * from ( \r\n\t                            select count(*) AS MValue from(\r\n\t                            SELECT --\r\n                                distinct t2.MDocID\r\n                            FROM\r\n                                t_gl_voucher t1\r\n                                    INNER JOIN\r\n                                t_gl_doc_voucher t2 ON t1.MitemID = t2.MVoucherID\r\n                                    AND t1.MOrgID = t2.MOrgID\r\n                                    AND t1.MIsdelete = t2.MIsDelete \r\n                            WHERE\r\n                                t1.MOrgID = @MOrgID\r\n                                    AND t1.MYear = @MYear\r\n                                    AND t1.mperiod = @MPeriod\r\n                                    AND t1.MIsDelete = 0  \r\n                                    AND LENGTH(IFNULL(t1.MNumber, '')) = 0\r\n                                    AND t1.MStatus = - 1\r\n                                    and (t2.MergeStatus = 0 Or t2.MergeStatus = 1))yy )y";
			str += " UNION ALL ";
			str += " select * from(SELECT\r\n                              COUNT(*) AS MValue\r\n                          FROM\r\n                              T_IV_BankBillEntry a\r\n                                  INNER JOIN\r\n                              T_IV_BankBill b ON a.MOrgID = b.MOrgID AND a.MID = b.MID \r\n                                  AND b.MIsDelete = 0\r\n                          WHERE\r\n                              a.MIsDelete = 0\r\n                                  AND b.MOrgID = @MOrgID \r\n                                  AND a.MDate >= @MDataBeginDate\r\n                                  AND a.MDate <= @MDataEndDate\r\n                                  AND a.MCheckState <> 2\r\n                                  AND IFNULL(a.MParentID,'')=''\r\n                                  AND NOT EXISTS(SELECT\r\n                                      1\r\n                                  FROM\r\n                                      T_IV_BankBillReconcile c\r\n                                  WHERE\r\n                                      c.MOrgID = @MOrgID\r\n                                          AND c.MBankBillEntryID = a.MEntryID \r\n                                          AND c.MIsDelete = 0\r\n                                          AND b.MBankID IS NOT NULL)) z";
			str += " UNION ALL ";
			str += " select *from( SELECT\r\n                              COUNT(t1.MItemID) AS MValue\r\n                          FROM\r\n                              t_gl_voucher t1\r\n                          WHERE\r\n                              t1.MOrgID = @MOrgID\r\n                                  AND t1.MYear = @MYear\r\n                                  AND t1.mperiod = @MPeriod\r\n                                  AND LENGTH(IFNULL(t1.MNumber, '')) > 0\r\n                                  AND t1.MIsDelete = 0   \r\n                                  AND  (MStatus  = 0 or MStatus = 1)\r\n                                  AND MSourceBillKey = '3') z";
			str += " UNION ALL ";
			str += " select *from (select tmp.MValue from ( SELECT\n\t                           CONCAT(MYear,'-',LPAD(MPeriod,2,'0')) AS MValue\n                            FROM\n\t                            t_gl_settlement a\n                            WHERE\n\t                            a.MOrgID = @MOrgID\n                                AND a.MIsDelete = 0\n                                AND a.MStatus = 1\n\t\t\t                union all select '-' as MValue\n                             )tmp where ifnull(tmp.MValue,'') <> '' order by MValue desc limit 0,1)z";
			MySqlParameter[] parameters = ctx.GetParameters((MySqlParameter)null);
			MySqlParameter[] obj = new MySqlParameter[6]
			{
				new MySqlParameter("@MYear", year),
				new MySqlParameter("@MPeriod", period),
				null,
				null,
				null,
				null
			};
			DateTime dateTime = ctx.MGLBeginDate;
			obj[2] = new MySqlParameter("@MBeginYear", dateTime.Year);
			dateTime = ctx.MGLBeginDate;
			obj[3] = new MySqlParameter("@MBeginPeriod", dateTime.Month);
			obj[4] = new MySqlParameter("@MDataBeginDate", new DateTime(year, period, 1));
			dateTime = new DateTime(year, period, 1);
			dateTime = dateTime.AddMonths(1);
			obj[5] = new MySqlParameter("@MDataEndDate", dateTime.AddDays(-1.0));
			MySqlParameter[] cmdParms = parameters.Concat(obj).ToArray();
			DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(str, cmdParms);
			GLDashboardModel gLDashboardModel = new GLDashboardModel();
			gLDashboardModel.Settled = (dataSet.Tables[0].Rows[0].MField<string>("MValue") == "1");
			gLDashboardModel.PeriodBalanceInited = (dataSet.Tables[0].Rows[1].MField<string>("MValue") == "1" && ctx.MInitBalanceOver);
			gLDashboardModel.MonthProcessFinished = (dataSet.Tables[0].Rows[2].MField<string>("MValue") == "1");
			gLDashboardModel.VoucherCount = int.Parse(dataSet.Tables[0].Rows[3].MField<string>("MValue"));
			gLDashboardModel.VoucherApprovedCount = int.Parse(dataSet.Tables[0].Rows[4].MField<string>("MValue"));
			gLDashboardModel.VoucherSavedCount = gLDashboardModel.VoucherCount - gLDashboardModel.VoucherApprovedCount;
			gLDashboardModel.ImportedVoucherCount = int.Parse(dataSet.Tables[0].Rows[5].MField<string>("MValue"));
			gLDashboardModel.CreatedDocVoucherCount = int.Parse(dataSet.Tables[0].Rows[6].MField<string>("MValue"));
			gLDashboardModel.UncreatedDocVoucherCount = int.Parse(dataSet.Tables[0].Rows[7].MField<string>("MValue"));
			gLDashboardModel.ReconcileFinished = (dataSet.Tables[0].Rows[8].MField<string>("MValue") == "0");
			gLDashboardModel.FromAppImportedVoucherCount = int.Parse(dataSet.Tables[0].Rows[9].MField<string>("MValue"));
			gLDashboardModel.ClosingPeriod = dataSet.Tables[0].Rows[10].MField<string>("MValue");
			gLDashboardModel.Year = year;
			gLDashboardModel.Period = period;
			return gLDashboardModel;
		}

		public List<CommandInfo> GetApproveVouchersCmd(MContext ctx, List<string> itemIDS, string status, List<GLVoucherModel> modelList = null)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			int approveStatus = (status == 1.ToString()) ? 1 : (-1);
			if (itemIDS == null || itemIDS.Count == 0)
			{
				list.AddRange(new GLBalanceRepository().GetApproveVoucherBalanceCmds(ctx, modelList, approveStatus));
			}
			else
			{
				list.AddRange(new GLBalanceRepository().GetApproveVoucherBalanceCmds(ctx, itemIDS, approveStatus));
				list.Add(GetApproveVoucherCmds(ctx, itemIDS, status));
			}
			return list;
		}

		public OperationResult ApproveVouchers(MContext ctx, List<string> itemIDS, string status)
		{
			OperationResult operationResult = new OperationResult();
			List<GLVoucherModel> voucherModelList = GetVoucherModelList(ctx, itemIDS, false, 0, 0);
			int num = int.Parse(status);
			int oldStatus = (num != 1) ? 1 : 0;
			ValidateVouchers(ctx, voucherModelList, num, oldStatus, false, false, true, true);
			List<CommandInfo> approveVouchersCmd = GetApproveVouchersCmd(ctx, itemIDS, status, null);
			List<CommandInfo> list = new List<CommandInfo>();
			if (voucherModelList.Count > 0)
			{
				list.AddRange(GlVoucherLogHelper.GetBatchUpdateStatusLogCmds(ctx, voucherModelList, status));
			}
			approveVouchersCmd.AddRange(list);
			operationResult.Success = (new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(approveVouchersCmd) > 0);
			return operationResult;
		}

		private List<CommandInfo> OptimizeCommandInfo(List<CommandInfo> cmdList)
		{
			List<CommandInfo> result = new List<CommandInfo>();
			CommandInfo commandInfo = new CommandInfo();
			Regex regex = new Regex("values", RegexOptions.IgnoreCase);
			string str = "";
			for (int i = 0; i < cmdList.Count(); i++)
			{
				CommandInfo commandInfo2 = cmdList[i];
				MatchCollection matchCollection = Regex.Matches(commandInfo2.CommandText, "values", RegexOptions.IgnoreCase);
				if (i == 0 && matchCollection.Count >= 2)
				{
					str = matchCollection[0].Value;
					continue;
				}
				if (matchCollection.Count < 2)
				{
					return null;
				}
				str += matchCollection[1].Value;
			}
			return result;
		}

		public CommandInfo GetApproveVoucherCmds(MContext ctx, List<string> ids, string status)
		{
			CommandInfo commandInfo = new CommandInfo();
			commandInfo.CommandText = "update t_gl_voucher set MStatus = @MStatus,MModifierID = @ModifierID, MModifyDate = @ModifyDate where MItemID in ('" + string.Join("','", ids) + "') and MOrgID=@MOrgID and MIsDelete = 0 ";
			DbParameter[] array = commandInfo.Parameters = new MySqlParameter[4]
			{
				new MySqlParameter
				{
					ParameterName = "@MStatus",
					Value = status
				},
				new MySqlParameter
				{
					ParameterName = "@MOrgID",
					Value = ctx.MOrgID
				},
				new MySqlParameter
				{
					ParameterName = "@ModifierID",
					Value = ctx.MUserID
				},
				new MySqlParameter
				{
					ParameterName = "@ModifyDate",
					Value = (object)ctx.DateNow
				}
			};
			return commandInfo;
		}

		public List<GLSimpleVoucherModel> GetSimpleVouchersByPeriods(MContext ctx, List<int> periods)
		{
			List<MySqlParameter> list = ctx.GetParameters((MySqlParameter)null).ToList();
			string sql = "SELECT DISTINCT\r\n                            t1.MItemID,\r\n                            t1.MYear,\r\n                            t1.MPeriod,\r\n                            t1.MStatus,\r\n                            t1.MNumber,\r\n                            t2.MDocID\r\n                        FROM\r\n                            t_gl_voucher t1\r\n                                LEFT JOIN\r\n                            t_gl_doc_voucher t2 ON t1.MItemID = t2.MVoucherID\r\n                                AND t1.MOrgID = t2.MOrgID\r\n                                AND t2.MIsDelete = 0\r\n                        WHERE\r\n                            t1.MOrgID = @MOrgID AND t1.MIsDelete = 0\r\n                                AND(t1.MYear * 100 + t1.MPeriod) " + GLUtility.GetInFilterQuery(periods, ref list, "M_ID");
			return ModelInfoManager.GetDataModelBySql<GLSimpleVoucherModel>(ctx, sql, list.ToArray());
		}

		public List<GLSimpleVoucherModel> GetSimpleVouchersByDocIDs(MContext ctx, List<string> ids)
		{
			List<MySqlParameter> list = ctx.GetParameters((MySqlParameter)null).ToList();
			string sql = "SELECT DISTINCT\r\n                            t1.MItemID,\r\n                            t1.MYear,\r\n                            t1.MPeriod,\r\n                            t1.MStatus,\r\n                            t1.MNumber,\r\n                            t2.MDocID\r\n                        FROM\r\n                            t_gl_voucher t1\r\n                                LEFT JOIN\r\n                            t_gl_doc_voucher t2 ON t1.MItemID = t2.MVoucherID\r\n                                AND t1.MOrgID = t2.MOrgID\r\n                                AND t2.MIsDelete = 0\r\n                        WHERE\r\n                            t1.MOrgID = @MOrgID AND t1.MIsDelete = 0\r\n                                AND MDocID " + GLUtility.GetInFilterQuery(ids, ref list, "M_ID");
			List<GLSimpleVoucherModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<GLSimpleVoucherModel>(ctx, sql, list.ToArray());
			return (dataModelBySql == null || !dataModelBySql.Any()) ? new List<GLSimpleVoucherModel>() : GetSimpleVouchersByPeriods(ctx, (from x in dataModelBySql
			select x.MYear * 100 + x.MPeriod).ToList());
		}

		public List<GLCheckGroupValueModel> GetCheckGroupValueList(MContext ctx)
		{
			string sql = "select * from t_gl_checkgroupvalue where MOrgID = @MOrgID and MIsDelete = 0 ";
			return ModelInfoManager.GetDataModelBySql<GLCheckGroupValueModel>(ctx, sql, ctx.GetParameters((MySqlParameter)null));
		}

		public List<GLVoucherModel> GetVoucherModelList(MContext ctx, List<string> pkIDS, bool includeDraft = false, int year = 0, int period = 0)
		{
			List<GLVoucherModel> result = new List<GLVoucherModel>();
			GLVoucherListFilterModel gLVoucherListFilterModel = new GLVoucherListFilterModel
			{
				Year = year,
				Period = period,
				IncludeDraft = includeDraft
			};
			if (pkIDS != null && pkIDS.Count > 0)
			{
				List<string> newList = new List<string>();
				pkIDS.ForEach(delegate(string x)
				{
					newList.Add("'" + x + "'");
				});
				gLVoucherListFilterModel.MItemID = string.Join(",", newList);
			}
			List<MySqlParameter> parameterList = GetParameterList(ctx, gLVoucherListFilterModel);
			string voucherSelectSql = GetVoucherSelectSql(gLVoucherListFilterModel, false, false);
			voucherSelectSql += " ORDER BY   t1.MNumber,t1.MItemID,t1.MYear,t1.MPeriod,t1.MNumber,t2.Mentryseq ";
			DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(voucherSelectSql, parameterList.ToArray());
			if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
			{
				result = DataTable2VoucherList(ctx, dataSet.Tables[0]);
			}
			return result;
		}

		public List<GLVoucherViewModel> GetVoucherViewModelList(MContext ctx, List<string> pkIDS, bool includeDraft = false, int year = 0, int period = 0, string orderBy = null)
		{
			List<GLVoucherViewModel> result = new List<GLVoucherViewModel>();
			GLVoucherListFilterModel gLVoucherListFilterModel = new GLVoucherListFilterModel
			{
				Year = year,
				Period = period,
				IncludeDraft = includeDraft
			};
			if (pkIDS != null && pkIDS.Count > 0)
			{
				List<string> newList = new List<string>();
				pkIDS.ForEach(delegate(string x)
				{
					newList.Add("'" + x + "'");
				});
				gLVoucherListFilterModel.MItemID = string.Join(",", newList);
			}
			List<MySqlParameter> parameterList = GetParameterList(ctx, gLVoucherListFilterModel);
			string voucherSelectSql = GetVoucherSelectSql(gLVoucherListFilterModel, false, false);
			voucherSelectSql = (string.IsNullOrWhiteSpace(orderBy) ? (voucherSelectSql + " ORDER BY  t1.MNumber DESC , t1.MItemID , t1.MYear , t1.MPeriod , t2.Mentryseq ") : (voucherSelectSql + orderBy));
			DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(voucherSelectSql, parameterList.ToArray());
			if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
			{
				result = DataTable2VoucherViewList(ctx, dataSet.Tables[0]);
			}
			return result;
		}

		public List<GLVoucherViewModel> GetVoucherPageList(MContext ctx, GLVoucherListFilterModel filter, bool includeDraft = false)
		{
			if (IsSimpleQuery(filter))
			{
				return GetVoucherPageListByProcedure(ctx, filter);
			}
			filter.IncludeDraft = includeDraft;
			return GetVoucherPageListByFilterByProcedure(ctx, filter);
		}

		private bool IsSimpleQuery(GLVoucherListFilterModel filter)
		{
			return string.IsNullOrWhiteSpace(filter.MNumber) && string.IsNullOrWhiteSpace(filter.From) && string.IsNullOrWhiteSpace(filter.Status) && string.IsNullOrWhiteSpace(filter.MItemID) && string.IsNullOrWhiteSpace(filter.KeyWord) && !filter.DecimalKeyWord.HasValue && filter.SortByType == 0 && filter.SortType == 0;
		}

		private List<GLVoucherViewModel> GetVoucherPageListByProcedure(MContext ctx, GLVoucherListFilterModel filter)
		{
			string procedureName = "Pro_GetVoucherPageList";
			DataTable dataTable = new DynamicDbHelperMySQL(ctx).ExcecuteProcudure(procedureName, GetVoucherQueryParams(ctx, filter, false));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return DataTable2VoucherViewList(ctx, dataTable);
			}
			return new List<GLVoucherViewModel>();
		}

		private List<GLVoucherViewModel> GetVoucherPageListByFilterByProcedure(MContext ctx, GLVoucherListFilterModel filter)
		{
			string procedureName = "Pro_GetVoucherPageListByFilter";
			DataTable dataTable = new DynamicDbHelperMySQL(ctx).ExcecuteProcudure(procedureName, GetVoucherQueryParams(ctx, filter, false));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return DataTable2VoucherViewList(ctx, dataTable);
			}
			return new List<GLVoucherViewModel>();
		}

		private int GetVoucherPageCountByFilterByProcedure(MContext ctx, GLVoucherListFilterModel filter)
		{
			string procedureName = "Pro_GetVoucherPageListByFilter";
			DataTable dataTable = new DynamicDbHelperMySQL(ctx).ExcecuteProcudure(procedureName, GetVoucherQueryParams(ctx, filter, true));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				int result = 0;
				if (int.TryParse(dataTable.Rows[0]["total"].ToString(), out result))
				{
					return result;
				}
			}
			return 0;
		}

		private int GetVoucherPageCountByProcedure(MContext ctx, GLVoucherListFilterModel filter)
		{
			string procedureName = "Pro_GetVoucherPageCount";
			DataTable dataTable = new DynamicDbHelperMySQL(ctx).ExcecuteProcudure(procedureName, GetVoucherQueryParams(ctx, filter, false));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				int result = 0;
				if (int.TryParse(dataTable.Rows[0]["total"].ToString(), out result))
				{
					return result;
				}
			}
			return 0;
		}

		private MySqlParameter[] GetVoucherQueryParams(MContext ctx, GLVoucherListFilterModel filter, bool isCount = false)
		{
			if (IsSimpleQuery(filter))
			{
				return new MySqlParameter[6]
				{
					new MySqlParameter
					{
						Direction = ParameterDirection.Input,
						ParameterName = "in_orgID",
						Value = ctx.MOrgID
					},
					new MySqlParameter
					{
						Direction = ParameterDirection.Input,
						ParameterName = "in_startYearPeriod",
						Value = (object)(filter.Year * 100 + filter.Period)
					},
					new MySqlParameter
					{
						Direction = ParameterDirection.Input,
						ParameterName = "in_endYearPeriod",
						Value = (object)(filter.EndYear * 100 + filter.EndPeriod)
					},
					new MySqlParameter
					{
						Direction = ParameterDirection.Input,
						ParameterName = "in_localeID",
						Value = ctx.MLCID
					},
					new MySqlParameter
					{
						Direction = ParameterDirection.Input,
						ParameterName = "in_startIndex",
						Value = (object)((filter.page - 1) * filter.rows)
					},
					new MySqlParameter
					{
						Direction = ParameterDirection.Input,
						ParameterName = "in_pageRows",
						Value = (object)filter.rows
					}
				};
			}
			return new MySqlParameter[17]
			{
				new MySqlParameter
				{
					Direction = ParameterDirection.Input,
					ParameterName = "in_orgID",
					Value = ctx.MOrgID
				},
				new MySqlParameter
				{
					Direction = ParameterDirection.Input,
					ParameterName = "in_startYearPeriod",
					Value = (object)(filter.Year * 100 + filter.Period)
				},
				new MySqlParameter
				{
					Direction = ParameterDirection.Input,
					ParameterName = "in_endYearPeriod",
					Value = (object)(filter.EndYear * 100 + filter.EndPeriod)
				},
				new MySqlParameter
				{
					Direction = ParameterDirection.Input,
					ParameterName = "in_localeID",
					Value = ctx.MLCID
				},
				new MySqlParameter
				{
					Direction = ParameterDirection.Input,
					ParameterName = "in_startIndex",
					Value = (object)((filter.page - 1) * filter.rows)
				},
				new MySqlParameter
				{
					Direction = ParameterDirection.Input,
					ParameterName = "in_pageRows",
					Value = (object)filter.rows
				},
				new MySqlParameter
				{
					Direction = ParameterDirection.Input,
					ParameterName = "in_getCount",
					Value = (object)(isCount ? 1 : 0)
				},
				new MySqlParameter
				{
					Direction = ParameterDirection.Input,
					ParameterName = "in_keyword",
					Value = (string.IsNullOrWhiteSpace(filter.KeyWord) ? filter.KeyWord : filter.KeyWord.Replace("\\", "\\\\"))
				},
				new MySqlParameter
				{
					Direction = ParameterDirection.Input,
					ParameterName = "in_hasDecimalKeyword",
					Value = (object)(filter.DecimalKeyWord.HasValue ? 1 : 0)
				},
				new MySqlParameter
				{
					Direction = ParameterDirection.Input,
					ParameterName = "in_decimalKeyword",
					Value = (object)(filter.DecimalKeyWord.HasValue ? filter.DecimalKeyWord.Value : decimal.Zero)
				},
				new MySqlParameter
				{
					Direction = ParameterDirection.Input,
					ParameterName = "in_number",
					Value = filter.MNumber
				},
				new MySqlParameter
				{
					Direction = ParameterDirection.Input,
					ParameterName = "in_status",
					Value = filter.Status
				},
				new MySqlParameter
				{
					Direction = ParameterDirection.Input,
					ParameterName = "in_from",
					Value = filter.From
				},
				new MySqlParameter
				{
					Direction = ParameterDirection.Input,
					ParameterName = "in_itemID",
					Value = filter.MItemID
				},
				new MySqlParameter
				{
					Direction = ParameterDirection.Input,
					ParameterName = "in_includeDraft",
					Value = (object)filter.IncludeDraft
				},
				new MySqlParameter
				{
					Direction = ParameterDirection.Input,
					ParameterName = "in_sortByType",
					Value = (object)filter.SortByType
				},
				new MySqlParameter
				{
					Direction = ParameterDirection.Input,
					ParameterName = "in_sortType",
					Value = (object)filter.SortType
				}
			};
		}

		public List<GLVoucherModel> GetVoucherList(MContext ctx, GLVoucherListFilterModel filter)
		{
			List<GLVoucherModel> result = new List<GLVoucherModel>();
			List<MySqlParameter> parameterList = GetParameterList(ctx, filter);
			string voucherSelectSql = GetVoucherSelectSql(filter, false, false);
			DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(voucherSelectSql, parameterList.ToArray());
			if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
			{
				result = DataTable2VoucherList(ctx, dataSet.Tables[0]);
			}
			return result;
		}

		public string GetVoucherSelectSql(GLVoucherListFilterModel filter, bool isCount = false, bool isPager = true)
		{
			string str = "";
			str = ((!isCount) ? (str + " \r\n                     SELECT \r\n                        F_GETUSERNAME(t27.MFristName, t27.MLastName) AS MCreatorName,\r\n                        t1.MItemID,\r\n                        t1.MOrgID,\r\n                        t1.MDate,\r\n                        t1.MPeriod,\r\n                        t1.MYear,\r\n                        t1.MNumber,\r\n                        t1.MAttachments ,\r\n                        t1.MSourceBillKey,                 \r\n                        t1.MReference,\r\n                        t1.MDebitTotal,\r\n                        t1.MCreditTotal,\r\n                        t1.MStatus,\r\n                        t1.MAUDITORID, \r\n                        t2.MEntryID,\r\n                        t2.MID,\r\n                        t2.MExplanation,\r\n                        t2.MAccountID ,\r\n                        t2.MCheckGroupValueID,\r\n                        t2.MAmountFor,\r\n                        t2.MAmount,\r\n                        t2.MCurrencyID,\r\n                        t2.MExchangeRate,\r\n                        t2.MDC,\r\n                        t2.MDebit,\r\n                        t2.MCredit,\r\n                        t2.MEntrySeq, \r\n                        t4.MNumber as MAccountNo,\r\n                        t3.MName as MAccountNameOnly,\r\n                        t3.MFullName as MAccountName,\r\n                        t4.MIsCheckForCurrency,\r\n                        t4.MCheckGroupID,\r\n                        t4.MCode as MAccountCode,\r\n                        t4.MAccountTypeID,\r\n                        t5_0.MContactID as MContact,\r\n                        t5_0.MEmployeeID as MEmployee,\r\n                        t5_0.MMerItemID as MMerItem,\r\n                        t5_0.MExpItemID as MExpItem,\r\n                        t5_0.MPaItemID as MPaItem,\r\n                        t5_0.MTrackItem1 as MTrack1,\r\n                        t5_0.MTrackItem2  as MTrack2,\r\n                        t5_0.MTrackItem3 as MTrack3,\r\n                        t5_0.MTrackItem4 as MTrack4,\r\n                        t5_0.MTrackItem5 as MTrack5,\r\n                        t5.MContactID,t5.MEmployeeID,\r\n                        t5.MMerItemID,t5.MExpItemID,\r\n                        t5.MPaItemID,\r\n                        t5.MTrackItem1,\r\n                        t5.MTrackItem2,\r\n                        t5.MTrackItem3,\r\n                        t5.MTrackItem4,\r\n                        t5.MTrackItem5,\r\n                        convert(AES_DECRYPT(t6.MName,'{0}') using utf8) as MContactName,\r\n                        F_GETUSERNAME(t7.MFirstName, t7.MLastName) AS MEmployeeName,\r\n                        concat(t8_0.MNumber,':',t8.MDesc) AS MMerItemName,\r\n                        t9.MName AS MExpItemName,\r\n                        t10.MName AS MPaItemName,\r\n                        t10_0.MGroupID AS MPaItemGroupID,t10_1.MName as MPaItemGroupName,\r\n                        t11.MName AS MTrackItem1Name,\r\n                        t11_2.MName AS MTrackItem1GroupName,\r\n                        t12.MName AS MTrackItem2Name,\r\n                        t12_2.MName AS MTrackItem2GroupName,\r\n                        t13.MName AS MTrackItem3Name,\r\n                        t13_2.MName AS MTrackItem3GroupName,\r\n                        t14.MName AS MTrackItem4Name,\r\n                        t14_2.MName AS MTrackItem4GroupName,\r\n                        t15.MName AS MTrackItem5Name,\r\n                        t15_2.MName AS MTrackItem5GroupName,\r\n                        t16.MTransferTypeID ") : (str + " SELECT COUNT(*) AS total From (SELECT t1.MItemID "));
			str += " FROM ";
			str += "( SELECT *\r\n                FROM t_gl_voucher mm\r\n                WHERE mm.MOrgID = @MOrgID  AND mm.MIsDelete = 0  ";
			if (!filter.IncludeDraft)
			{
				str += "  AND (mm.MStatus  = 0 or mm.MStatus = 1) AND length(ifnull(mm.MNumber,'')) > 0   ";
			}
			if (filter.Year > 0 && filter.Period > 0)
			{
				str += " AND mm.MYear = @MYear AND mm.MPeriod = @MPeriod  ";
			}
			if (!string.IsNullOrWhiteSpace(filter.MNumber))
			{
				str += " AND mm.MNumber = @MNumber ";
			}
			if (!string.IsNullOrWhiteSpace(filter.Status))
			{
				str += " AND  mm.MStatus = @MStatus ";
			}
			if (!string.IsNullOrWhiteSpace(filter.From))
			{
				switch (filter.From)
				{
				case "0":
					str += " and (ifnull(mm.MSourceBillKey,'') = '' OR mm.MSourceBillKey='0') and not exists( select 1 from t_gl_doc_voucher tx where tx.MVoucherID = mm.MItemID and tx.MIsDelete = 0 ) ";
					break;
				case "1":
					str += " and mm.MSourceBillKey = '1' ";
					break;
				case "2":
					str += " and exists( select 1 from t_gl_doc_voucher tx where tx.MVoucherID = mm.MItemID  and tx.MIsDelete = 0 ) ";
					break;
				case "3":
					str += " and mm.MSourceBillKey = '3' ";
					break;
				}
			}
			if (!string.IsNullOrWhiteSpace(filter.MItemID))
			{
				str = ((filter.MItemID.Length != 34) ? (str + $" AND mm.MItemID in ({filter.MItemID}) ") : (str + $" AND mm.MItemID = {filter.MItemID} "));
			}
			str += "  ) t1 \r\n                INNER JOIN\r\n                t_gl_voucherentry t2 \r\n                ON t1.MItemID = t2.MID AND t2.MOrgID = t1.MOrgID AND t2.MIsDelete = 0  ";
			str += "\r\n                {1} JOIN  \r\n                t_bd_account_l t3 \r\n                ON t3.MParentID = t2.MAccountID AND t3.MLocaleID =@MLCID AND t3.MOrgID = t1.MOrgID And t3.MIsDelete = 0\r\n                {1} JOIN \r\n                t_bd_account t4 \r\n                ON t4.MItemID = t2.MAccountID AND t4.MOrgID = t1.MOrgID AND t4.MIsDelete = 0 \r\n                {1} JOIN \r\n                t_gl_checkgroup t5_0 \r\n                ON t4.MCheckGroupID = t5_0.MItemID  AND t5_0.MIsDelete = 0 \r\n                {1} JOIN \r\n                t_gl_checkgroupvalue t5 \r\n                ON t5.MItemID = t2.MCheckGroupValueID AND t5.MOrgID = t1.MOrgID AND t5.MIsDelete = t1.MIsDelete \r\n                LEFT JOIN \r\n                t_bd_contacts_l t6 \r\n                ON t6.MParentID = t5.MContactID AND t6.MLocaleID = t3.MLocaleID AND t6.MOrgID = t1.MOrgID AND t6.MIsDelete = 0\r\n                LEFT JOIN \r\n                t_bd_employees_l t7 \r\n                ON t7.MParentID = t5.MEmployeeID AND t7.MOrgID = t1.MOrgId AND t7.MIsDelete = t1.MIsDelete AND t7.MLocaleID = t3.MLocaleID\r\n                LEFT JOIN\r\n                t_bd_item t8_0 ON t8_0.MItemID = t5.MMerItemID\r\n                    AND t8_0.MOrgID = t1.MOrgId\r\n                    AND t8_0.MIsDelete = t1.MIsDelete\r\n                LEFT JOIN \r\n                t_bd_item_l t8 \r\n                ON t8.MParentID = t5.MMerItemID AND t8.MOrgID = t1.MOrgId AND t8.MIsDelete = t1.MIsDelete AND t8.MLocaleID = t3.MLocaleID\r\n                LEFT JOIN \r\n                t_bd_expenseitem_l t9 \r\n                ON t9.MParentID = t5.MExpItemID AND t9.MOrgID = t1.MOrgId AND t9.MIsDelete = t1.MIsDelete AND t9.MLocaleID = t3.MLocaleID\r\n                LEFT JOIN \r\n                t_pa_payitem_l t10 \r\n                ON t10.MParentID = t5.MPaItemID AND t10.MOrgID = t1.MOrgId AND t10.MIsDelete = t1.MIsDelete AND t10.MLocaleID = t3.MLocaleID\r\n                LEFT JOIN\r\n                t_pa_payitem t10_0\r\n                ON t10_0.MItemID = t5.MPaItemID AND t10_0.MOrgID = t1.MOrgId AND t10_0.MIsDelete = t1.MIsDelete \r\n                LEFT JOIN \r\n                t_pa_payitemgroup_l t10_1 \r\n                ON t10_1.MParentID = t5.MPaItemID AND t10_1.MOrgID = t1.MOrgId AND t10_1.MIsDelete = t1.MIsDelete AND t10_1.MLocaleID = t3.MLocaleID\r\n\r\n\r\n                LEFT JOIN \r\n                t_bd_trackentry_l t11 \r\n                ON t11.MParentID = t5.MTrackItem1 AND t11.MLocaleID = t3.MLocaleID AND t11.MOrgID = t1.MOrgID AND t11.MIsDelete = 0\r\n                LEFT JOIN \r\n                t_bd_trackentry t11_1 \r\n                ON t11_1.MEntryID = t5.MTrackItem1 AND t11_1.MOrgID = t1.MOrgID AND t11_1.MIsDelete = 0\r\n                LEFT JOIN \r\n                t_bd_track_l t11_2 \r\n                ON t11_2.MParentID = t11_1.MItemID AND t11_2.MLocaleID = t3.MLocaleID AND t11_2.MOrgID = t1.MOrgID AND t11_2.MIsDelete = 0\r\n\r\n                 LEFT JOIN \r\n                t_bd_trackentry_l t12 \r\n                ON t12.MParentID = t5.MTrackItem2 AND t12.MLocaleID = t3.MLocaleID AND t12.MOrgID = t1.MOrgID AND t12.MIsDelete = 0\r\n                LEFT JOIN \r\n                t_bd_trackentry t12_1 \r\n                ON t12_1.MEntryID = t5.MTrackItem2 AND t12_1.MOrgID = t1.MOrgID AND t12_1.MIsDelete = 0\r\n                LEFT JOIN \r\n                t_bd_track_l t12_2 \r\n                ON t12_2.MParentID = t12_1.MItemID AND t12_2.MLocaleID = t3.MLocaleID AND t12_2.MOrgID = t1.MOrgID AND t12_2.MIsDelete = 0\r\n\r\n                LEFT JOIN \r\n                t_bd_trackentry_l t13 \r\n                ON t13.MParentID = t5.MTrackItem3 AND t13.MLocaleID = t3.MLocaleID AND t13.MOrgID = t1.MOrgID AND t13.MIsDelete = 0\r\n                LEFT JOIN \r\n                t_bd_trackentry t13_1 \r\n                ON t13_1.MEntryID = t5.MTrackItem3 AND t13_1.MOrgID = t1.MOrgID AND t13_1.MIsDelete = 0\r\n                LEFT JOIN \r\n                t_bd_track_l t13_2 \r\n                ON t13_2.MParentID = t13_1.MItemID AND t13_2.MLocaleID = t3.MLocaleID AND t13_2.MOrgID = t1.MOrgID AND t13_2.MIsDelete = 0\r\n\r\n                LEFT JOIN \r\n                t_bd_trackentry_l t14 \r\n                ON t14.MParentID = t5.MTrackItem4 AND t14.MLocaleID = t3.MLocaleID AND t14.MOrgID = t1.MOrgID AND t14.MIsDelete = 0\r\n                LEFT JOIN \r\n                t_bd_trackentry t14_1 \r\n                ON t14_1.MEntryID = t5.MTrackItem4 AND t14_1.MOrgID = t1.MOrgID AND t14_1.MIsDelete = 0\r\n                LEFT JOIN \r\n                t_bd_track_l t14_2 \r\n                ON t14_2.MParentID = t14_1.MItemID AND t14_2.MLocaleID = t3.MLocaleID AND t14_2.MOrgID = t1.MOrgID AND t14_2.MIsDelete = 0\r\n\r\n                LEFT JOIN \r\n                t_bd_trackentry_l t15 \r\n                ON t15.MParentID = t5.MTrackItem5 AND t15.MLocaleID = t3.MLocaleID AND t15.MOrgID = t1.MOrgID AND t15.MIsDelete = 0\r\n                LEFT JOIN \r\n                t_bd_trackentry t15_1 \r\n                ON t15_1.MEntryID = t5.MTrackItem5 AND t15_1.MOrgID = t1.MOrgID AND t15_1.MIsDelete = 0\r\n                LEFT JOIN \r\n                t_bd_track_l t15_2 \r\n                ON t15_2.MParentID = t15_1.MItemID AND t15_2.MLocaleID = t3.MLocaleID AND t15_2.MOrgID = t1.MOrgID AND t15_2.MIsDelete = 0\r\n\r\n                LEFT JOIN \r\n                t_gl_periodtransfer t16 \r\n                ON t16.MVoucherID = t1.MItemID AND t16.MOrgID = t1.MOrgID AND t16.MIsDelete = 0\r\n                LEFT JOIN\r\n                t_sec_user_l t27 ON t2.MCreatorID = t27.MParentID AND t27.MIsDelete =0 AND t27.MLocaleID =@MLCID\r\n                ";
			if (isPager && !isCount)
			{
				str += " WHERE t1.MItemID IN ( SELECT * FROM (\r\n                SELECT t1.MItemID FROM t_gl_voucher t1 \r\n                 INNER JOIN \r\n                t_gl_voucherentry t2 \r\n                ON t1.MItemID = t2.MID AND t2.MOrgID = t1.MOrgID AND t2.MIsDelete = 0 \r\n                {1} JOIN \r\n                t_bd_account_l t3 \r\n                ON t3.MParentID = t2.MAccountID AND t3.MLocaleID =@MLCID AND t3.MOrgID = t1.MOrgID And t3.MIsDelete = 0\r\n                 {1} JOIN  \r\n               t_bd_account t4 \r\n                ON t4.MItemID = t2.MAccountID AND t4.MOrgID = t1.MOrgID AND t4.MIsDelete = 0 \r\n                {1} JOIN \r\n                t_gl_checkgroupvalue t5 \r\n                ON t5.MItemID = t2.MCheckGroupValueID AND t5.MOrgID = t1.MOrgID AND t5.MIsDelete = t1.MIsDelete ";
				if (!string.IsNullOrWhiteSpace(filter.KeyWord))
				{
					str += " LEFT JOIN \r\n                     t_bd_contacts_l t6 \r\n                     ON t6.MParentID = t5.MContactID AND t6.MLocaleID = t3.MLocaleID AND t6.MOrgID = t1.MOrgID AND t6.MIsDelete = 0\r\n                      LEFT JOIN \r\n                      t_bd_employees_l t7 \r\n                      ON t7.MParentID = t5.MEmployeeID AND t7.MOrgID = t1.MOrgId AND t7.MIsDelete = t1.MIsDelete AND t7.MLocaleID = t3.MLocaleID \r\n                    LEFT JOIN\r\n                        t_bd_item t8_0 ON t8_0.MItemID = t5.MMerItemID\r\n                            AND t8_0.MOrgID = t1.MOrgId\r\n                            AND t8_0.MIsDelete = t1.MIsDelete\r\n                     LEFT JOIN \r\n                      t_bd_item_l t8 \r\n                     ON t8.MParentID = t5.MMerItemID AND t8.MOrgID = t1.MOrgId AND t8.MIsDelete = t1.MIsDelete AND t8.MLocaleID = t3.MLocaleID \r\n                     LEFT JOIN \r\n                     t_bd_expenseitem_l t9 \r\n                     ON t9.MParentID = t5.MExpItemID AND t9.MOrgID = t1.MOrgId AND t9.MIsDelete = t1.MIsDelete AND t9.MLocaleID = t3.MLocaleID \r\n\r\n                     LEFT JOIN \r\n                     t_pa_payitem_l t10 \r\n                     ON t10.MParentID = t5.MPaItemID AND t10.MOrgID = t1.MOrgId AND t10.MIsDelete = t1.MIsDelete AND t10.MLocaleID = t3.MLocaleID \r\n                    LEFT JOIN\r\n                    t_pa_payitem t10_0\r\n                    ON t10_0.MItemID = t5.MPaItemID AND t10_0.MOrgID = t1.MOrgId AND t10_0.MIsDelete = t1.MIsDelete \r\n                    LEFT JOIN \r\n                    t_pa_payitemgroup_l t10_1 \r\n                    ON t10_1.MParentID = t5.MPaItemID AND t10_1.MOrgID = t1.MOrgId AND t10_1.MIsDelete = t1.MIsDelete AND t10_1.MLocaleID = t3.MLocaleID\r\n\r\n                     LEFT JOIN \r\n                     t_bd_trackentry_l t11 \r\n                     ON t11.MParentID = t5.MTrackItem1 AND t11.MLocaleID = t3.MLocaleID AND t11.MOrgID = t1.MOrgID AND t11.MIsDelete = 0 \r\n                     LEFT JOIN \r\n                     t_bd_trackentry_l t12 \r\n                     ON t12.MParentID = t5.MTrackItem2 AND t12.MLocaleID = t3.MLocaleID AND t12.MOrgID = t1.MOrgID AND t12.MIsDelete = 0 \r\n                     LEFT JOIN \r\n                     t_bd_trackentry_l t13 \r\n                     ON t13.MParentID = t5.MTrackItem3 AND t13.MLocaleID = t3.MLocaleID AND t13.MOrgID = t1.MOrgID AND t13.MIsDelete = 0 \r\n                     LEFT JOIN \r\n                     t_bd_trackentry_l t14 \r\n                     ON t14.MParentID = t5.MTrackItem4 AND t14.MLocaleID = t3.MLocaleID AND t14.MOrgID = t1.MOrgID AND t14.MIsDelete = 0 \r\n                     LEFT JOIN \r\n                     t_bd_trackentry_l t15 \r\n                     ON t15.MParentID = t5.MTrackItem5 AND t15.MLocaleID = t3.MLocaleID AND t15.MOrgID = t1.MOrgID AND t15.MIsDelete = 0 \r\n                     LEFT JOIN \r\n                     t_gl_periodtransfer t16 \r\n                     ON t16.MVoucherID = t1.MItemID AND t16.MOrgID = t1.MOrgID AND t16.MIsDelete = 0 ";
				}
				str += " WHERE t1.MOrgID =  @MOrgID AND t1.MIsDelete = 0  ";
				if (!filter.IncludeDraft)
				{
					str += "  AND  (t1.MStatus  = 0 or t1.MStatus = 1) AND length(ifnull(t1.MNumber,'')) > 0  ";
				}
				if (filter.Year > 0 && filter.Period > 0)
				{
					str += " AND t1.MYear = @MYear AND t1.MPeriod = @MPeriod  ";
				}
				if (!string.IsNullOrWhiteSpace(filter.MNumber))
				{
					str += " AND t1.MNumber = @MNumber ";
				}
				if (!string.IsNullOrWhiteSpace(filter.Status))
				{
					str += " AND  t1.MStatus = @MStatus ";
				}
				if (!string.IsNullOrWhiteSpace(filter.From))
				{
					switch (filter.From)
					{
					case "0":
						str += " and ifnull(t1.MSourceBillKey,'') = '' and not exists( select 1 from t_gl_doc_voucher tx where tx.MVoucherID = t1.MItemID ) ";
						break;
					case "1":
						str += " and t1.MSourceBillKey = '1' ";
						break;
					case "2":
						str += " and exists( select 1 from t_gl_doc_voucher tx where tx.MVoucherID = t1.MItemID ) ";
						break;
					case "3":
						str += " and t1.MSourceBillKey = '3' ";
						break;
					}
				}
				if (!string.IsNullOrWhiteSpace(filter.KeyWord))
				{
					str += " AND ( t1.MNumber LIKE concat('%',@Keyword,'%')\r\n                     OR t2.MCurrencyID LIKE concat('%',@Keyword,'%')\r\n                     OR t2.MEXPLANATION LIKE concat('%',@Keyword,'%')\r\n                     OR t3.MFullName LIKE concat('%',@Keyword,'%')\r\n                     OR convert(AES_DECRYPT(t6.MName,'{0}') using utf8)  LIKE concat('%',@Keyword,'%')\r\n                     OR F_GETUSERNAME(t7.MFirstName, t7.MLastName) LIKE concat('%',@Keyword,'%')\r\n                     OR concat(t8_0.MNumber,':',t8.MDesc) LIKE concat('%',@Keyword,'%')\r\n                     OR t9.MName LIKE concat('%',@Keyword,'%')\r\n                     OR t10.MName LIKE concat('%',@Keyword,'%')\r\n                     OR t10_1.MName LIKE concat('%',@Keyword,'%')\r\n                     OR t11.MName  LIKE concat('%',@Keyword,'%')\r\n                     OR t12.MName  LIKE concat('%',@Keyword,'%') \r\n                     OR t13.MName  LIKE concat('%',@Keyword,'%') \r\n                     OR t14.MName  LIKE concat('%',@Keyword,'%') \r\n                     OR t15.MName  LIKE concat('%',@Keyword,'%')  ";
					if (filter.DecimalKeyWord.HasValue)
					{
						str += " OR t2.MDebit = @DecimalKeyWord  \r\n                                       OR t2.MCredit = @DecimalKeyWord  ";
					}
					str += " )";
				}
				str += " GROUP BY t1.MITEMID  ";
				str = str + " ORDER BY t1.MNumber DESC LIMIT " + (filter.page - 1) * filter.rows + "," + filter.rows + " ) t20 ) ";
				str += " ORDER BY t1.MNumber DESC,t1.MItemID, t1.MYear,t1.MPeriod,t2.Mentryseq ";
			}
			else
			{
				if (!string.IsNullOrWhiteSpace(filter.KeyWord))
				{
					str += " WHERE  t1.MNumber LIKE concat('%',@Keyword,'%')\r\n                      OR t2.MCurrencyID LIKE concat('%',@Keyword,'%')\r\n                      OR t2.MEXPLANATION LIKE concat('%',@Keyword,'%')\r\n                      OR t3.MFullName LIKE concat('%',@Keyword,'%')\r\n                      OR convert(AES_DECRYPT(t6.MName,'{0}') using utf8)  LIKE concat('%',@Keyword,'%')\r\n                      OR F_GETUSERNAME(t7.MFirstName, t7.MLastName) LIKE concat('%',@Keyword,'%')\r\n                      OR concat(t8_0.MNumber,':',t8.MDesc) LIKE concat('%',@Keyword,'%')\r\n                      OR t9.MName LIKE concat('%',@Keyword,'%')\r\n                      OR t10.MName LIKE concat('%',@Keyword,'%')\r\n                      OR t10_1.MName LIKE concat('%',@Keyword,'%')\r\n                      OR t11.MName  LIKE concat('%',@Keyword,'%')\r\n                      OR t12.MName  LIKE concat('%',@Keyword,'%') \r\n                      OR t13.MName  LIKE concat('%',@Keyword,'%') \r\n                      OR t14.MName  LIKE concat('%',@Keyword,'%') \r\n                      OR t15.MName  LIKE concat('%',@Keyword,'%') ";
					if (filter.DecimalKeyWord.HasValue)
					{
						str += " OR t2.MDebit = @DecimalKeyWord  \r\n                                       OR t2.MCredit = @DecimalKeyWord  ";
					}
				}
				if (isCount)
				{
					str += " GROUP BY t1.MItemID ) t100  ";
				}
			}
			return string.Format(str, "JieNor-001", filter.IncludeDraft ? " LEFT " : " INNER ");
		}

		public int GetVoucherPageListCount(MContext ctx, GLVoucherListFilterModel filter)
		{
			if (IsSimpleQuery(filter))
			{
				return GetVoucherPageCountByProcedure(ctx, filter);
			}
			return GetVoucherPageCountByFilterByProcedure(ctx, filter);
		}

		public List<GLVoucherModel> DataTable2VoucherList(MContext ctx, DataTable dt)
		{
			List<GLVoucherModel> list = new List<GLVoucherModel>();
			GLVoucherModel gLVoucherModel = new GLVoucherModel();
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				DataRow dataRow = dt.Rows[i];
				string text = dataRow.MField<string>("MItemID");
				if (text != gLVoucherModel.MItemID)
				{
					if (!string.IsNullOrWhiteSpace(gLVoucherModel.MItemID))
					{
						list.Add(gLVoucherModel);
					}
					gLVoucherModel = new GLVoucherModel
					{
						MItemID = text,
						MOrgID = dataRow.MField<string>("MOrgID"),
						MDate = dataRow.MField<DateTime>("MDate"),
						MPeriod = dataRow.MField<int>("MPeriod"),
						MYear = dataRow.MField<int>("MYear"),
						MNumber = dataRow.MField<string>("MNumber"),
						MAttachments = dataRow.MField<int>("MAttachments"),
						MReference = dataRow.MField<string>("MReference"),
						MDebitTotal = dataRow.MField<decimal>("MDebitTotal"),
						MCreditTotal = dataRow.MField<decimal>("MCreditTotal"),
						MStatus = dataRow.MField<int>("MStatus"),
						MSourceBillKey = dataRow.MField<string>("MSourceBillKey"),
						MCreatorName = dataRow.MField<string>("MCreatorName"),
						MVoucherEntrys = new List<GLVoucherEntryModel>()
					};
					gLVoucherModel.MCreatorName = (string.IsNullOrWhiteSpace(gLVoucherModel.MCreatorName) ? GlobalFormat.GetUserName(ctx.MFirstName, ctx.MLastName, null) : gLVoucherModel.MCreatorName);
					if (dt.Columns.Contains("MTransferTypeID"))
					{
						gLVoucherModel.MTransferTypeID = ((dataRow["MTransferTypeID"] == null || string.IsNullOrWhiteSpace(dataRow["MTransferTypeID"].ToString())) ? (-1) : Convert.ToInt32(dataRow["MTransferTypeID"]));
					}
				}
				GLVoucherEntryModel gLVoucherEntryModel = new GLVoucherEntryModel
				{
					MEntryID = dataRow.MField<string>("MEntryID"),
					MID = dataRow.MField<string>("MID"),
					MOrgID = dataRow.MField<string>("MOrgID"),
					MExplanation = dataRow.MField<string>("MExplanation"),
					MAccountID = dataRow.MField<string>("MAccountID"),
					MAmountFor = dataRow.MField<decimal>("MAmountFor"),
					MAmount = dataRow.MField<decimal>("MAmount"),
					MAccountNo = dataRow.MField<string>("MAccountNo"),
					MAccountNameOnly = dataRow.MField<string>("MAccountNameOnly"),
					MAccountName = dataRow.MField<string>("MAccountName"),
					MAccountCode = dataRow.MField<string>("MAccountCode"),
					MCheckGroupValueID = dataRow.MField<string>("MCheckGroupValueID"),
					MIsCheckForCurrency = dataRow.MField<bool>("MIsCheckForCurrency"),
					MCurrencyID = dataRow.MField<string>("MCurrencyID"),
					MExchangeRate = dataRow.MField<decimal>("MExchangeRate"),
					MDC = dataRow.MField<int>("MDC"),
					MCredit = dataRow.MField<decimal>("MCredit"),
					MDebit = dataRow.MField<decimal>("MDebit"),
					MEntrySeq = dataRow.MField<int>("MEntrySeq")
				};
				BDAccountModel bDAccountModel = new BDAccountModel
				{
					MItemID = gLVoucherEntryModel.MAccountID,
					MCheckGroupID = gLVoucherEntryModel.MCheckGroupID,
					MDC = dataRow.MField<int>("MDC")
				};
				GLCheckGroupModel mCheckGroupModel = new GLCheckGroupModel
				{
					MItemID = dataRow.MField<string>("MCheckGroupID"),
					MContactID = dataRow.MField<int>("MContact"),
					MEmployeeID = dataRow.MField<int>("MEmployee"),
					MMerItemID = dataRow.MField<int>("MMerItem"),
					MExpItemID = dataRow.MField<int>("MExpItem"),
					MPaItemID = dataRow.MField<int>("MPaItem"),
					MTrackItem1 = dataRow.MField<int>("MTrack1"),
					MTrackItem2 = dataRow.MField<int>("MTrack2"),
					MTrackItem3 = dataRow.MField<int>("MTrack3"),
					MTrackItem4 = dataRow.MField<int>("MTrack4"),
					MTrackItem5 = dataRow.MField<int>("MTrack5")
				};
				GLCurrencyDataModel obj = new GLCurrencyDataModel
				{
					MCurrencyID = gLVoucherEntryModel.MCurrencyID,
					MAmount = gLVoucherEntryModel.MAmount,
					MAmountFor = gLVoucherEntryModel.MAmountFor,
					MExchangeRate = gLVoucherEntryModel.MExchangeRate
				};
				GLCurrencyDataModel gLCurrencyDataModel2 = bDAccountModel.MCurrencyDataModel = obj;
				bDAccountModel.MCheckGroupModel = mCheckGroupModel;
				gLVoucherEntryModel.MCheckGroupValueModel = new GLCheckGroupValueModel
				{
					MItemID = dataRow.MField<string>("MCheckGroupValueID"),
					MTrackItem1 = dataRow.MField<string>("MTrackItem1"),
					MTrackItem2 = dataRow.MField<string>("MTrackItem2"),
					MTrackItem3 = dataRow.MField<string>("MTrackItem3"),
					MTrackItem4 = dataRow.MField<string>("MTrackItem4"),
					MTrackItem5 = dataRow.MField<string>("MTrackItem5"),
					MTrackItem1Name = dataRow.MField<string>("MTrackItem1Name"),
					MTrackItem2Name = dataRow.MField<string>("MTrackItem2Name"),
					MTrackItem3Name = dataRow.MField<string>("MTrackItem3Name"),
					MTrackItem4Name = dataRow.MField<string>("MTrackItem4Name"),
					MTrackItem5Name = dataRow.MField<string>("MTrackItem5Name"),
					MTrackItem1GroupName = dataRow.MField<string>("MTrackItem1GroupName"),
					MTrackItem2GroupName = dataRow.MField<string>("MTrackItem2GroupName"),
					MTrackItem3GroupName = dataRow.MField<string>("MTrackItem3GroupName"),
					MTrackItem4GroupName = dataRow.MField<string>("MTrackItem4GroupName"),
					MTrackItem5GroupName = dataRow.MField<string>("MTrackItem5GroupName"),
					MContactID = dataRow.MField<string>("MContactID"),
					MEmployeeID = dataRow.MField<string>("MEmployeeID"),
					MMerItemID = dataRow.MField<string>("MMerItemID"),
					MExpItemID = dataRow.MField<string>("MExpItemID"),
					MPaItemID = dataRow.MField<string>("MPaItemID"),
					MPaItemGroupID = dataRow.MField<string>("MPaItemGroupID"),
					MPaItemGroupName = dataRow.MField<string>("MPaItemGroupName"),
					MContactName = dataRow.MField<string>("MContactName"),
					MEmployeeName = dataRow.MField<string>("MEmployeeName"),
					MMerItemName = dataRow.MField<string>("MMerItemName"),
					MExpItemName = dataRow.MField<string>("MExpItemName"),
					MPaItemName = dataRow.MField<string>("MPaItemName")
				};
				bDAccountModel.MCheckGroupValueModel = gLVoucherEntryModel.MCheckGroupValueModel;
				if (!string.IsNullOrWhiteSpace(gLVoucherEntryModel.MCheckGroupValueModel.MPaItemGroupName))
				{
					gLVoucherEntryModel.MCheckGroupValueModel.MPaItemGroupID = gLVoucherEntryModel.MCheckGroupValueModel.MPaItemID;
					gLVoucherEntryModel.MCheckGroupValueModel.MPaItemName = gLVoucherEntryModel.MCheckGroupValueModel.MPaItemGroupName;
				}
				gLVoucherEntryModel.MAccountModel = bDAccountModel;
				gLVoucherModel.MVoucherEntrys.Add(gLVoucherEntryModel);
			}
			list.Add(gLVoucherModel);
			list.ForEach(delegate(GLVoucherModel x)
			{
				if (x.MTransferTypeID != -1)
				{
					x.MTransferTypeName = new GLPeriodTransferRepository().GetTransferTypeNameByID(ctx, x.MTransferTypeID);
				}
			});
			return list;
		}

		public List<GLVoucherModel> GetVoucherListIncludeEntry(MContext ctx, GLVoucherListFilterModel filter)
		{
			string str = " \r\n                     SELECT \r\n                        t1.MItemID,\r\n                        t1.MOrgID,\r\n                        t1.MDate,\r\n                        t1.MPeriod,\r\n                        t1.MYear,\r\n                        t1.MNumber,\r\n                        t1.MAttachments ,\r\n                        t1.MSourceBillKey,                 \r\n                        t1.MReference,\r\n                        t1.MDebitTotal,\r\n                        t1.MCreditTotal,\r\n                        t1.MStatus,\r\n                        t1.MAUDITORID, \r\n                        '' as MCreatorName,\r\n                        t2.MEntryID,\r\n                        t2.MID,\r\n                        t2.MExplanation,\r\n                        t2.MAccountID ,\r\n                        t2.MCheckGroupValueID,\r\n                        t2.MAmountFor,\r\n                        t2.MAmount,\r\n                        t2.MCurrencyID,\r\n                        t2.MExchangeRate,\r\n                        t2.MDC,\r\n                        t2.MDebit,\r\n                        t2.MCredit,\r\n                        t2.MEntrySeq, \r\n                        t4.MNumber as MAccountNo,\r\n                        t3.MName as MAccountNameOnly,\r\n                        t3.MFullName as MAccountName,\r\n                        t4.MIsCheckForCurrency,\r\n                        t4.MCheckGroupID,\r\n                        t4.MCode as MAccountCode,\r\n                        t4.MAccountTypeID,\r\n                        t5_0.MContactID as MContact,\r\n                        t5_0.MEmployeeID as MEmployee,\r\n                        t5_0.MMerItemID as MMerItem,\r\n                        t5_0.MExpItemID as MExpItem,\r\n                        t5_0.MPaItemID as MPaItem,\r\n                        t5_0.MTrackItem1 as MTrack1,\r\n                        t5_0.MTrackItem2  as MTrack2,\r\n                        t5_0.MTrackItem3 as MTrack3,\r\n                        t5_0.MTrackItem4 as MTrack4,\r\n                        t5_0.MTrackItem5 as MTrack5,\r\n                        t5.MContactID,t5.MEmployeeID,\r\n                        t5.MMerItemID,t5.MExpItemID,\r\n                        t5.MPaItemID,\r\n                        t5.MTrackItem1,t5.MTrackItem2,\r\n                        t5.MTrackItem3,\r\n                        t5.MTrackItem4,\r\n                        t5.MTrackItem5,\r\n                        convert(AES_DECRYPT(t6.MName,'{0}') using utf8) as MContactName,\r\n                        F_GETUSERNAME(t7.MFirstName, t7.MLastName) AS MEmployeeName,\r\n                        concat(t8_0.MNumber,':',t8.MDesc) AS MMerItemName,\r\n                        t9.MName AS MExpItemName,\r\n                        t10.MName AS MPaItemName,\r\n                        t10_0.MGroupID AS MPaItemGroupID,t10_1.MName as MPaItemGroupName,\r\n                        t11.MName AS MTrackItem1Name,\r\n                        t11_2.MName AS MTrackItem1GroupName,\r\n                        t12.MName AS MTrackItem2Name,\r\n                        t12_2.MName AS MTrackItem2GroupName,\r\n                        t13.MName AS MTrackItem3Name,\r\n                        t13_2.MName AS MTrackItem3GroupName,\r\n                        t14.MName AS MTrackItem4Name,\r\n                        t14_2.MName AS MTrackItem4GroupName,\r\n                        t15.MName AS MTrackItem5Name,\r\n                        t15_2.MName AS MTrackItem5GroupName ";
			str += " FROM ";
			str += "( SELECT *\r\n                FROM t_gl_voucher\r\n                WHERE MOrgID = @MOrgID  AND MIsDelete = 0  ";
			if (!filter.IncludeDraft)
			{
				str += "  AND (MStatus  = 0 or MStatus = 1) AND length(ifnull(MNumber,'')) > 0  ";
			}
			if (filter.StartPeriod > 0 && filter.StartYear > 0)
			{
				str = str + " AND (MYear*100+MPeriod)>=" + (filter.StartYear * 100 + filter.StartPeriod);
			}
			if (filter.EndYear > 0 && filter.EndPeriod > 0)
			{
				str = str + " AND (MYear*100+MPeriod)>=" + (filter.StartYear * 100 + filter.StartPeriod);
			}
			if (filter.MStartYearPeriod > 0)
			{
				str = str + " AND (MYear*100+MPeriod)>=" + filter.MStartYearPeriod;
			}
			if (filter.MEndYearPeriod > 0)
			{
				str = str + " AND (MYear*100+MPeriod)<=" + filter.MEndYearPeriod;
			}
			if (!string.IsNullOrWhiteSpace(filter.Status))
			{
				str += " AND  MStatus = @MStatus ";
			}
			str += "  ) t1 \r\n                INNER JOIN\r\n                t_gl_voucherentry t2 \r\n                ON t1.MItemID = t2.MID AND t2.MOrgID = t1.MOrgID AND t2.MIsDelete = 0  ";
			if (filter.AccountIDList != null && filter.AccountIDList.Count() > 0)
			{
				str = str + " and t2.MAccountID in ('" + string.Join("','", filter.AccountIDList) + "')";
			}
			if (!string.IsNullOrWhiteSpace(filter.CheckGroupValueId))
			{
				str += $" and t2.MCheckGroupValueID='{filter.CheckGroupValueId}' ";
			}
			if (!string.IsNullOrWhiteSpace(filter.MCurrencyID) && filter.MCurrencyID != "0")
			{
				str += " and t2.MCurrencyID=@MCurrencyID";
			}
			str += "\r\n                INNER JOIN  \r\n                t_bd_account_l t3 \r\n                ON t3.MParentID = t2.MAccountID AND t3.MLocaleID =@MLCID AND t3.MOrgID = t1.MOrgID And t3.MIsDelete = 0\r\n                INNER JOIN \r\n                t_bd_account t4 \r\n                ON t4.MItemID = t2.MAccountID AND t4.MOrgID = t1.MOrgID AND t4.MIsDelete = 0 \r\n                INNER JOIN \r\n                t_gl_checkgroup t5_0 \r\n                ON t4.MCheckGroupID = t5_0.MItemID  AND t5_0.MIsDelete = 0 \r\n                INNER JOIN \r\n                t_gl_checkgroupvalue t5 \r\n                ON t5.MItemID = t2.MCheckGroupValueID AND t5.MOrgID = t2.MOrgID AND t5.MIsDelete = t2.MIsDelete \r\n                LEFT JOIN \r\n                t_bd_contacts_l t6 \r\n                ON t6.MParentID = t5.MContactID AND t6.MLocaleID = t3.MLocaleID AND t6.MOrgID = t1.MOrgID AND t6.MIsDelete = 0\r\n                LEFT JOIN \r\n                t_bd_employees_l t7 \r\n                ON t7.MParentID = t5.MEmployeeID AND t7.MOrgID = t1.MOrgId AND t7.MIsDelete = t1.MIsDelete AND t7.MLocaleID = t3.MLocaleID\r\n                LEFT JOIN\r\n                t_bd_item t8_0 ON t8_0.MItemID = t5.MMerItemID\r\n                    AND t8_0.MOrgID = t1.MOrgId\r\n                    AND t8_0.MIsDelete = t1.MIsDelete\r\n                LEFT JOIN \r\n                t_bd_item_l t8 \r\n                ON t8.MParentID = t5.MMerItemID AND t8.MOrgID = t1.MOrgId AND t8.MIsDelete = t1.MIsDelete AND t8.MLocaleID = t3.MLocaleID\r\n                LEFT JOIN \r\n                t_bd_expenseitem_l t9 \r\n                ON t9.MParentID = t5.MExpItemID AND t9.MOrgID = t1.MOrgId AND t9.MIsDelete = t1.MIsDelete AND t9.MLocaleID = t3.MLocaleID\r\n                LEFT JOIN \r\n                t_pa_payitem_l t10 \r\n                ON t10.MParentID = t5.MPaItemID AND t10.MOrgID = t1.MOrgId AND t10.MIsDelete = t1.MIsDelete AND t10.MLocaleID = t3.MLocaleID\r\n                LEFT JOIN\r\n                t_pa_payitem t10_0\r\n                ON t10_0.MItemID = t5.MPaItemID AND t10_0.MOrgID = t1.MOrgId AND t10_0.MIsDelete = t1.MIsDelete \r\n                LEFT JOIN \r\n                t_pa_payitemgroup_l t10_1 \r\n                ON t10_1.MParentID = t5.MPaItemID AND t10_1.MOrgID = t1.MOrgId AND t10_1.MIsDelete = t1.MIsDelete AND t10_1.MLocaleID = t3.MLocaleID\r\n                LEFT JOIN \r\n                t_bd_trackentry_l t11 \r\n                ON t11.MParentID = t5.MTrackItem1 AND t11.MLocaleID = t3.MLocaleID AND t11.MOrgID = t1.MOrgID  AND t11.MIsDelete = 0\r\n                LEFT JOIN \r\n                t_bd_trackentry t11_1 \r\n                ON t11_1.MEntryID = t5.MTrackItem1 AND t11_1.MOrgID = t1.MOrgID AND t11_1.MIsDelete = 0\r\n                LEFT JOIN \r\n                t_bd_track_l t11_2 \r\n                ON t11_2.MParentID = t11_1.MItemID AND t11_2.MLocaleID = t3.MLocaleID AND t11_2.MOrgID = t1.MOrgID AND t11_2.MIsDelete = 0\r\n\r\n                 LEFT JOIN \r\n                t_bd_trackentry_l t12 \r\n                ON t12.MParentID = t5.MTrackItem2 AND t12.MLocaleID = t3.MLocaleID AND t12.MOrgID = t1.MOrgID AND t12.MIsDelete = 0\r\n                LEFT JOIN \r\n                t_bd_trackentry t12_1 \r\n                ON t12_1.MEntryID = t5.MTrackItem2 AND t12_1.MOrgID = t1.MOrgID AND t12_1.MIsDelete = 0\r\n                LEFT JOIN \r\n                t_bd_track_l t12_2 \r\n                ON t12_2.MParentID = t12_1.MItemID AND t12_2.MLocaleID = t3.MLocaleID AND t12_2.MOrgID = t1.MOrgID  AND t12_2.MIsDelete = 0\r\n\r\n                LEFT JOIN \r\n                t_bd_trackentry_l t13 \r\n                ON t13.MParentID = t5.MTrackItem3 AND t13.MLocaleID = t3.MLocaleID AND t13.MOrgID = t1.MOrgID AND t13.MIsDelete = 0\r\n                LEFT JOIN \r\n                t_bd_trackentry t13_1 \r\n                ON t13_1.MEntryID = t5.MTrackItem3 AND t13_1.MOrgID = t1.MOrgID AND t13_1.MIsDelete = 0\r\n                LEFT JOIN \r\n                t_bd_track_l t13_2 \r\n                ON t13_2.MParentID = t13_1.MItemID AND t13_2.MLocaleID = t3.MLocaleID AND t13_2.MOrgID = t1.MOrgID AND t13_2.MIsDelete = 0\r\n\r\n                LEFT JOIN \r\n                t_bd_trackentry_l t14 \r\n                ON t14.MParentID = t5.MTrackItem4 AND t14.MLocaleID = t3.MLocaleID AND t14.MOrgID = t1.MOrgID AND t14.MIsDelete = 0\r\n                LEFT JOIN \r\n                t_bd_trackentry t14_1 \r\n                ON t14_1.MEntryID = t5.MTrackItem4 AND t14_1.MOrgID = t1.MOrgID AND t14_1.MIsDelete = 0\r\n                LEFT JOIN \r\n                t_bd_track_l t14_2 \r\n                ON t14_2.MParentID = t14_1.MItemID AND t14_2.MLocaleID = t3.MLocaleID AND t14_2.MOrgID = t1.MOrgID AND t14_2.MIsDelete = 0\r\n\r\n                LEFT JOIN \r\n                t_bd_trackentry_l t15 \r\n                ON t15.MParentID = t5.MTrackItem5 AND t15.MLocaleID = t3.MLocaleID AND t15.MOrgID = t1.MOrgID AND t15.MIsDelete = 0\r\n                LEFT JOIN \r\n                t_bd_trackentry t15_1 \r\n                ON t15_1.MEntryID = t5.MTrackItem5 AND t15_1.MOrgID = t1.MOrgID AND t15_1.MIsDelete = 0\r\n                LEFT JOIN \r\n                t_bd_track_l t15_2 \r\n                ON t15_2.MParentID = t15_1.MItemID AND t15_2.MLocaleID = t3.MLocaleID AND t15_2.MOrgID = t1.MOrgID  AND t15_2.MIsDelete = 0";
			str = string.Format(str, "JieNor-001");
			List<MySqlParameter> parameterList = GetParameterList(ctx, filter);
			DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(str, parameterList.ToArray());
			List<GLVoucherModel> result = new List<GLVoucherModel>();
			if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
			{
				result = DataTable2VoucherList(ctx, dataSet.Tables[0]);
			}
			return result;
		}

		public List<GLVoucherViewModel> DataTable2VoucherViewList(MContext ctx, DataTable dt)
		{
			List<GLVoucherViewModel> list = new List<GLVoucherViewModel>();
			GLVoucherViewModel gLVoucherViewModel = new GLVoucherViewModel();
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				DataRow dataRow = dt.Rows[i];
				string text = dataRow.MField<string>("MItemID");
				if (text != gLVoucherViewModel.MItemID)
				{
					if (!string.IsNullOrWhiteSpace(gLVoucherViewModel.MItemID))
					{
						list.Add(gLVoucherViewModel);
					}
					gLVoucherViewModel = new GLVoucherViewModel
					{
						MItemID = text,
						MOrgID = dataRow.MField<string>("MOrgID"),
						MDate = dataRow.MField<DateTime>("MDate"),
						MNumber = dataRow.MField<string>("MNumber"),
						MDebitTotal = dataRow.MField<decimal>("MDebitTotal"),
						MCreditTotal = dataRow.MField<decimal>("MCreditTotal"),
						MStatus = dataRow.MField<int>("MStatus"),
						MCreatorName = dataRow.MField<string>("MCreatorName"),
						MAttachments = dataRow.MField<int>("MAttachments"),
						MTransferTypeID = ((dataRow["MTransferTypeID"] == null || string.IsNullOrWhiteSpace(dataRow["MTransferTypeID"].ToString())) ? (-1) : Convert.ToInt32(dataRow["MTransferTypeID"])),
						MYear = dataRow.MField<int>("MYear"),
						MPeriod = dataRow.MField<int>("MPeriod"),
						MVoucherEntrys = new List<GLVoucherEntryViewModel>()
					};
					gLVoucherViewModel.MCreatorName = (string.IsNullOrWhiteSpace(gLVoucherViewModel.MCreatorName) ? GlobalFormat.GetUserName(ctx.MFirstName, ctx.MLastName, null) : gLVoucherViewModel.MCreatorName);
				}
				GLVoucherEntryViewModel gLVoucherEntryViewModel = new GLVoucherEntryViewModel
				{
					MEntryID = dataRow.MField<string>("MEntryID"),
					MExplanation = dataRow.MField<string>("MExplanation"),
					MAccountID = dataRow.MField<string>("MAccountID"),
					MAccountName = dataRow.MField<string>("MAccountName"),
					MIsCheckForCurrency = dataRow.MField<bool>("MIsCheckForCurrency"),
					MCurrencyID = dataRow.MField<string>("MCurrencyID"),
					MExchangeRate = dataRow.MField<decimal>("MExchangeRate"),
					MDC = dataRow.MField<int>("MDC"),
					MAmountFor = dataRow.MField<decimal>("MAmountFor"),
					MAmount = dataRow.MField<decimal>("MAmount"),
					MCredit = dataRow.MField<decimal>("MCredit"),
					MDebit = dataRow.MField<decimal>("MDebit"),
					MEntrySeq = dataRow.MField<int>("MEntrySeq")
				};
				JieNor.Megi.DataModel.GL.BDAccountViewModel bDAccountViewModel = new JieNor.Megi.DataModel.GL.BDAccountViewModel
				{
					MItemID = gLVoucherEntryViewModel.MAccountID
				};
				GLCheckGroupViewModel mCheckGroupModel = new GLCheckGroupViewModel
				{
					MItemID = dataRow.MField<string>("MCheckGroupID"),
					MContactID = dataRow.MField<int>("MContact"),
					MEmployeeID = dataRow.MField<int>("MEmployee"),
					MMerItemID = dataRow.MField<int>("MMerItem"),
					MExpItemID = dataRow.MField<int>("MExpItem"),
					MPaItemID = dataRow.MField<int>("MPaItem"),
					MTrackItem1 = dataRow.MField<int>("MTrack1"),
					MTrackItem2 = dataRow.MField<int>("MTrack2"),
					MTrackItem3 = dataRow.MField<int>("MTrack3"),
					MTrackItem4 = dataRow.MField<int>("MTrack4"),
					MTrackItem5 = dataRow.MField<int>("MTrack5")
				};
				BDCurrrencyDataViewModel obj = new BDCurrrencyDataViewModel
				{
					MCurrencyID = gLVoucherEntryViewModel.MCurrencyID,
					MAmount = gLVoucherEntryViewModel.MAmount,
					MAmountFor = gLVoucherEntryViewModel.MAmountFor,
					MExchangeRate = gLVoucherEntryViewModel.MExchangeRate
				};
				BDCurrrencyDataViewModel bDCurrrencyDataViewModel2 = bDAccountViewModel.MCurrencyDataModel = obj;
				bDAccountViewModel.MCheckGroupModel = mCheckGroupModel;
				GLCheckGroupValueViewModel obj2 = new GLCheckGroupValueViewModel
				{
					MItemID = dataRow.MField<string>("MCheckGroupValueID"),
					MTrackItem1 = dataRow.MField<string>("MTrackItem1"),
					MTrackItem2 = dataRow.MField<string>("MTrackItem2"),
					MTrackItem3 = dataRow.MField<string>("MTrackItem3"),
					MTrackItem4 = dataRow.MField<string>("MTrackItem4"),
					MTrackItem5 = dataRow.MField<string>("MTrackItem5"),
					MTrackItem1Name = dataRow.MField<string>("MTrackItem1Name"),
					MTrackItem2Name = dataRow.MField<string>("MTrackItem2Name"),
					MTrackItem3Name = dataRow.MField<string>("MTrackItem3Name"),
					MTrackItem4Name = dataRow.MField<string>("MTrackItem4Name"),
					MTrackItem5Name = dataRow.MField<string>("MTrackItem5Name"),
					MTrackItem1GroupName = dataRow.MField<string>("MTrackItem1GroupName"),
					MTrackItem2GroupName = dataRow.MField<string>("MTrackItem2GroupName"),
					MTrackItem3GroupName = dataRow.MField<string>("MTrackItem3GroupName"),
					MTrackItem4GroupName = dataRow.MField<string>("MTrackItem4GroupName"),
					MTrackItem5GroupName = dataRow.MField<string>("MTrackItem5GroupName"),
					MContactID = dataRow.MField<string>("MContactID"),
					MEmployeeID = dataRow.MField<string>("MEmployeeID"),
					MMerItemID = dataRow.MField<string>("MMerItemID"),
					MExpItemID = dataRow.MField<string>("MExpItemID"),
					MPaItemID = dataRow.MField<string>("MPaItemID"),
					MPaItemGroupID = dataRow.MField<string>("MPaItemGroupID"),
					MPaItemGroupName = dataRow.MField<string>("MPaItemGroupName"),
					MContactName = dataRow.MField<string>("MContactName"),
					MEmployeeName = dataRow.MField<string>("MEmployeeName"),
					MMerItemName = dataRow.MField<string>("MMerItemName"),
					MExpItemName = dataRow.MField<string>("MExpItemName"),
					MPaItemName = dataRow.MField<string>("MPaItemName")
				};
				GLCheckGroupValueViewModel gLCheckGroupValueViewModel2 = bDAccountViewModel.MCheckGroupValueModel = obj2;
				if (!string.IsNullOrWhiteSpace(gLCheckGroupValueViewModel2.MPaItemGroupName))
				{
					gLCheckGroupValueViewModel2.MPaItemGroupID = gLCheckGroupValueViewModel2.MPaItemID;
					gLCheckGroupValueViewModel2.MPaItemName = gLCheckGroupValueViewModel2.MPaItemGroupName;
				}
				gLVoucherEntryViewModel.MAccountModel = bDAccountViewModel;
				gLVoucherViewModel.MVoucherEntrys.Add(gLVoucherEntryViewModel);
			}
			list.Add(gLVoucherViewModel);
			list.ForEach(delegate(GLVoucherViewModel x)
			{
				if (x.MTransferTypeID != -1)
				{
					x.MTransferTypeName = new GLPeriodTransferRepository().GetTransferTypeNameByID(ctx, x.MTransferTypeID);
				}
			});
			return list;
		}

		private bool ConvertBoolean(DataRow row, string name)
		{
			return row.Field<ulong>(name) == 1;
		}

		private string ConvertString(DataRow row, string name, string defaultValue = null)
		{
			return row.IsNull(name) ? defaultValue : row.MField<string>(name);
		}

		private decimal ConvertDecimal(DataRow row, string name)
		{
			return row.MField<decimal>(name);
		}

		private int ConvertInt(DataRow row, string name, int defaultValue = 0)
		{
			return row.IsNull(name) ? defaultValue : row.MField<int>(name);
		}

		private DateTime ConvertDate(DataRow row, string name)
		{
			return Convert.ToDateTime(row[name].ToString());
		}

		public GLVoucherModel BindDataset2Voucher(MContext ctx, DataTable dt)
		{
			List<GLVoucherModel> list = new List<GLVoucherModel>();
			List<GLVoucherEntryModel> list2 = new List<GLVoucherEntryModel>();
			DataRow dataRow = dt.Rows[0];
			GLVoucherModel gLVoucherModel = new GLVoucherModel
			{
				MItemID = dataRow.MField<string>("MID"),
				MOrgID = dataRow.MField<string>("MOrgID"),
				MDate = dataRow.MField<DateTime>("MDate"),
				MPeriod = dataRow.MField<int>("MPeriod"),
				MYear = dataRow.MField<int>("MYear"),
				MNumber = dataRow.MField<string>("MNumber"),
				MRVoucherID = dataRow.MField<string>("MRVoucherID"),
				MOVoucherID = dataRow.MField<string>("MOVoucherID"),
				MCreatorName = dataRow.MField<string>("MCreatorName"),
				MAttachments = dataRow.MField<int>("MAttachments"),
				MDebitTotal = dataRow.MField<decimal>("MDebitTotal"),
				MCreditTotal = dataRow.MField<decimal>("MCreditTotal"),
				MSourceBillKey = dataRow.MField<string>("MSourceBillKey"),
				MStatus = dataRow.MField<int>("MStatus"),
				MDocType = dataRow.MField<int>("MDocType"),
				MDocVoucherID = dataRow.MField<string>("MDocVoucherID"),
				MDocID = dataRow.MField<string>("MDocID"),
				MTransferTypeID = Convert.ToInt32(dataRow["MTransferTypeID"]),
				MVoucherEntrys = new List<GLVoucherEntryModel>(),
				MSettlementStatus = dataRow.MField<int>("MSettlementStatus")
			};
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				dataRow = dt.Rows[i];
				GLVoucherEntryModel gLVoucherEntryModel = new GLVoucherEntryModel
				{
					MEntryID = dataRow.MField<string>("MEntryID"),
					MID = dataRow.MField<string>("MID"),
					MOrgID = dataRow.MField<string>("MOrgID"),
					MExplanation = dataRow.MField<string>("MExplanation"),
					MAmountFor = ConvertDecimal(dataRow, "MAmountFor"),
					MAmount = ConvertDecimal(dataRow, "MAmount"),
					MAccountID = dataRow.MField<string>("MAccountID"),
					MAccountName = dataRow.MField<string>("MAccountName"),
					MCurrencyID = (dataRow.MField<string>("MCurrencyID") ?? ctx.MBasCurrencyID),
					MExchangeRate = ConvertDecimal(dataRow, "MExchangeRate"),
					MCheckGroupValueID = dataRow.MField<string>("MCheckGroupValueID"),
					MDC = dataRow.MField<int>("MDC"),
					MCredit = ConvertDecimal(dataRow, "MCredit"),
					MDebit = ConvertDecimal(dataRow, "MDebit"),
					MEntrySeq = dataRow.MField<int>("MEntrySeq")
				};
				BDAccountModel bDAccountModel = string.IsNullOrWhiteSpace(gLVoucherEntryModel.MAccountID) ? new BDAccountModel() : new BDAccountModel
				{
					MItemID = dataRow.MField<string>("MAccountID"),
					MIsCheckForCurrency = ConvertBoolean(dataRow, "MIsCheckForCurrency"),
					MDC = dataRow.MField<int>("MAccountDC"),
					MIsBankAccount = (dataRow.MField<string>("MIsBankAccount") == "1"),
					MFullName = dataRow.MField<string>("MAccountName"),
					MNumber = dataRow.MField<string>("MAccountNumber"),
					MCheckGroupID = dataRow.MField<string>("MCheckGroupID")
				};
				GLCheckGroupModel obj = string.IsNullOrWhiteSpace(bDAccountModel.MItemID) ? new GLCheckGroupModel() : new GLCheckGroupModel
				{
					MItemID = dataRow.MField<string>("MCheckGroupID"),
					MContactID = dataRow.MField<int>("MCheckGroupContactID"),
					MEmployeeID = dataRow.MField<int>("MCheckGroupEmployeeID"),
					MMerItemID = dataRow.MField<int>("MCheckGroupMerItemID"),
					MExpItemID = dataRow.MField<int>("MCheckGroupExpItemID"),
					MPaItemID = dataRow.MField<int>("MCheckGroupPaItemID"),
					MTrackItem1 = dataRow.MField<int>("MCheckGroupTrackItem1"),
					MTrackItem2 = dataRow.MField<int>("MCheckGroupTrackItem2"),
					MTrackItem3 = dataRow.MField<int>("MCheckGroupTrackItem3"),
					MTrackItem4 = dataRow.MField<int>("MCheckGroupTrackItem4"),
					MTrackItem5 = dataRow.MField<int>("MCheckGroupTrackItem5")
				};
				GLCheckGroupModel gLCheckGroupModel2 = bDAccountModel.MCheckGroupModel = obj;
				GLCurrencyDataModel obj2 = new GLCurrencyDataModel
				{
					MCurrencyID = gLVoucherEntryModel.MCurrencyID,
					MAmount = gLVoucherEntryModel.MAmount,
					MAmountFor = gLVoucherEntryModel.MAmountFor,
					MExchangeRate = gLVoucherEntryModel.MExchangeRate
				};
				GLCurrencyDataModel gLCurrencyDataModel2 = bDAccountModel.MCurrencyDataModel = obj2;
				GLCheckGroupValueModel gLCheckGroupValueModel = new GLCheckGroupValueModel
				{
					MItemID = dataRow.MField<string>("MCheckGroupValueID"),
					MContactID = dataRow.MField<string>("MContactID"),
					MContactName = dataRow.MField<string>("MContactName"),
					MEmployeeID = dataRow.MField<string>("MEmployeeID"),
					MEmployeeName = dataRow.MField<string>("MEmployeeName"),
					MMerItemID = dataRow.MField<string>("MMerItemID"),
					MMerItemName = dataRow.MField<string>("MMerItemName"),
					MExpItemID = dataRow.MField<string>("MExpItemID"),
					MExpItemName = dataRow.MField<string>("MExpItemName"),
					MPaItemID = dataRow.MField<string>("MPaItemID"),
					MPaItemGroupID = dataRow.MField<string>("MPaItemGroupID"),
					MPaItemGroupName = dataRow.MField<string>("MPaItemGroupName"),
					MPaItemName = dataRow.MField<string>("MPaItemName"),
					MTrackItem1 = dataRow.MField<string>("MTrackItem1"),
					MTrackItem2 = dataRow.MField<string>("MTrackItem2"),
					MTrackItem3 = dataRow.MField<string>("MTrackItem3"),
					MTrackItem4 = dataRow.MField<string>("MTrackItem4"),
					MTrackItem5 = dataRow.MField<string>("MTrackItem5"),
					MTrackItem1GroupID = dataRow.MField<string>("MTrackItem1GroupID"),
					MTrackItem2GroupID = dataRow.MField<string>("MTrackItem2GroupID"),
					MTrackItem3GroupID = dataRow.MField<string>("MTrackItem3GroupID"),
					MTrackItem4GroupID = dataRow.MField<string>("MTrackItem4GroupID"),
					MTrackItem5GroupID = dataRow.MField<string>("MTrackItem5GroupID"),
					MTrackItem1Name = dataRow.MField<string>("MTrackItem1Name"),
					MTrackItem2Name = dataRow.MField<string>("MTrackItem2Name"),
					MTrackItem3Name = dataRow.MField<string>("MTrackItem3Name"),
					MTrackItem4Name = dataRow.MField<string>("MTrackItem4Name"),
					MTrackItem5Name = dataRow.MField<string>("MTrackItem5Name"),
					MTrackItem1GroupName = dataRow.MField<string>("MTrackItem1GroupName"),
					MTrackItem2GroupName = dataRow.MField<string>("MTrackItem2GroupName"),
					MTrackItem3GroupName = dataRow.MField<string>("MTrackItem3GroupName"),
					MTrackItem4GroupName = dataRow.MField<string>("MTrackItem4GroupName"),
					MTrackItem5GroupName = dataRow.MField<string>("MTrackItem5GroupName")
				};
				if (!string.IsNullOrWhiteSpace(gLCheckGroupValueModel.MPaItemGroupName))
				{
					gLCheckGroupValueModel.MPaItemGroupID = gLCheckGroupValueModel.MPaItemID;
					gLCheckGroupValueModel.MPaItemName = gLCheckGroupValueModel.MPaItemGroupName;
				}
				gLVoucherEntryModel.MCheckGroupValueModel = gLCheckGroupValueModel;
				bDAccountModel.MCheckGroupValueModel = gLCheckGroupValueModel;
				gLVoucherEntryModel.MAccountModel = bDAccountModel;
				list2.Add(gLVoucherEntryModel);
			}
			gLVoucherModel.MVoucherEntrys = list2;
			return gLVoucherModel;
		}

		public void ValidateVoucherNumberOutOfRange(MContext ctx, int maxCount)
		{
			ValidateQueryModel validateVoucherNumberOutOfRange = new GLUtility().GetValidateVoucherNumberOutOfRange(ctx, maxCount);
			utility.QueryValidateSql(ctx, true, validateVoucherNumberOutOfRange);
		}

		public List<CommandInfo> GetUpdateVoucherNumberCmds(MContext ctx, BASOrgPrefixSettingModel model)
		{
			ValidateVoucherNumberOutOfRange(ctx, model.MNumberCount);
			List<CommandInfo> list = new List<CommandInfo>();
			if (model.MFillBlankChar != "0")
			{
				List<CommandInfo> list2 = list;
				CommandInfo obj = new CommandInfo
				{
					CommandText = "UPDATE t_gl_voucher \r\n                                    SET\r\n                                        MNumber = TRIM(LEADING '0' FROM MNumber)\r\n                                    WHERE\r\n                                        MOrgID = @MOrgID and MIsDelete = 0\r\n                                            AND LENGTH(IFNULL(MNumber, '')) != 0; "
				};
				DbParameter[] array = obj.Parameters = ctx.GetParameters((MySqlParameter)null);
				list2.Add(obj);
			}
			else
			{
				List<CommandInfo> list3 = list;
				CommandInfo obj2 = new CommandInfo
				{
					CommandText = "UPDATE t_gl_voucher \r\n                                    SET\r\n                                        MNumber = CONCAT(SUBSTRING('0000000000',\r\n                                                    10 + 1 - @MaxCount + LENGTH(CAST(CAST(MNumber AS SIGNED) AS CHAR))),\r\n                                                CAST(CAST(MNumber AS SIGNED) AS CHAR))\r\n                                    WHERE\r\n                                        MOrgID = @MOrgID\r\n                                            AND MIsDelete = 0\r\n                                            AND LENGTH(IFNULL(MNumber, '')) != 0 "
				};
				DbParameter[] array = obj2.Parameters = ctx.GetParameters("@MaxCount", model.MNumberCount);
				list3.Add(obj2);
			}
			return list;
		}

		public List<GLVoucherModel> GetVoucherModelByPeriod(MContext ctx, int? year, int? period, int status = -2)
		{
			SqlWhere sqlWhere = new SqlWhere().AddFilter("MOrgID", SqlOperators.Equal, ctx.MOrgID).AddFilter("MNumber", SqlOperators.IsNotNull, "");
			if (year.HasValue && period.HasValue)
			{
				sqlWhere.AddFilter("MYear", SqlOperators.Equal, year.Value).AddFilter("MPeriod", SqlOperators.Equal, period.Value);
			}
			if (status != -2)
			{
				sqlWhere.AddFilter("MStatus", SqlOperators.Equal, status);
			}
			sqlWhere.AddOrderBy("MNumber", SqlOrderDir.Asc);
			return GetModelList(ctx, sqlWhere, false);
		}

		public bool CheckPeriodHasBrokenNumber(MContext ctx, int year, int period)
		{
			string sql = "SELECT \r\n                        (COUNT(t.MNumber) - cast((case when max(t.MNumber) is null then 0 else max(t.MNumber) end) as signed)) as differ\r\n                    FROM\r\n                        t_gl_voucher t\r\n                    WHERE\r\n                        t.MIsDelete = 0 and length(ifnull(t.MNumber,'')) > 0 and  (t.MStatus  = 0 or t.MStatus = 1)\r\n                    AND t.MOrgID = @MOrgID\r\n                    AND t.MYear = @MYear\r\n                    AND t.MPeriod = @MPeriod";
			MySqlParameter[] cmdParms = new MySqlParameter[3]
			{
				new MySqlParameter
				{
					ParameterName = "@MOrgID",
					Value = ctx.MOrgID
				},
				new MySqlParameter
				{
					ParameterName = "@MYear",
					Value = (object)year
				},
				new MySqlParameter
				{
					ParameterName = "@MPeriod",
					Value = (object)period
				}
			};
			DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(sql, cmdParms);
			if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return false;
			}
			return int.Parse(dataSet.Tables[0].Rows[0]["differ"].ToString()) != 0;
		}

		public bool CheckVoucherHasUnapproved(MContext ctx, GLSettlementModel model)
		{
			return base.ExistsByFilter(ctx, new SqlWhere().AddFilter("MOrgID", SqlOperators.Equal, ctx.MOrgID).AddFilter("MYear", SqlOperators.Equal, model.MYear).AddFilter("MPeriod", SqlOperators.Equal, model.MPeriod)
				.AddFilter("MStatus", SqlOperators.Equal, 0)
				.AddFilter("MNumber", SqlOperators.IsNotNull, string.Empty));
		}

		public Dictionary<string, string> IsMNumberUsed(MContext ctx, Dictionary<string, List<string>> dateNumberList)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			string empty = string.Empty;
			string empty2 = string.Empty;
			string empty3 = string.Empty;
			string empty4 = string.Empty;
			List<MySqlParameter> list = new List<MySqlParameter>
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			stringBuilder.Append("select * from (");
			foreach (string key in dateNumberList.Keys)
			{
				DateTime dateTime = Convert.ToDateTime(key);
				empty3 = "@MNumber" + num;
				empty = "@MYear" + num;
				empty2 = "@MPeriod" + num;
				list.AddRange(new List<MySqlParameter>
				{
					new MySqlParameter(empty, dateTime.Year),
					new MySqlParameter(empty2, dateTime.Month)
				});
				MySqlParameter[] source = null;
				empty4 = base.GetWhereInSql(string.Join(",", from v in dateNumberList[key]
				orderby v
				select v), ref source, empty3);
				list.AddRange(source.ToList());
				if (num > 0 && num < dateNumberList.Count())
				{
					stringBuilder.Append(" union all");
				}
				stringBuilder.AppendFormat(" select MDate,group_concat(MNumber) as MNumber from t_gl_voucher where MOrgID=@MOrgID and MIsDelete=0  and MNumber in ({0}) and MYear={1} and MPeriod={2}", empty4, empty, empty2);
				num++;
			}
			stringBuilder.Append(") u group by MDate order by MDate asc");
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), list.ToArray());
			if (dataSet.Tables[0].Rows.Count > 0)
			{
				foreach (DataRow row in dataSet.Tables[0].Rows)
				{
					if (row["MDate"] != null && !string.IsNullOrWhiteSpace(row["MDate"].ToString()))
					{
						dictionary.Add(Convert.ToDateTime(row["MDate"]).ToString("yyyy-MM-dd"), Convert.ToString(row["MNumber"]).Replace(',', '、'));
					}
				}
			}
			return dictionary;
		}

		public OperationResult DeleteVoucherModels(MContext ctx, List<string> pkIDS)
		{
			OperationResult operationResult = new OperationResult();
			List<CommandInfo> deleteVoucherModelsCmd = GetDeleteVoucherModelsCmd(ctx, pkIDS);
			operationResult.Success = (new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(deleteVoucherModelsCmd) > 0);
			return operationResult;
		}

		public GLVoucherModel GetVoucherModel(MContext ctx, string MItemID = null, int year = 0, int period = 0, int day = 0, string lastVoucherNumber = null)
		{
			GLVoucherModel gLVoucherModel = new GLVoucherModel();
			if (string.IsNullOrWhiteSpace(MItemID))
			{
				if (year * period == 0)
				{
					DateTime avaliableVoucherDate = new GLSettlementRepository().GetAvaliableVoucherDate(ctx);
					year = avaliableVoucherDate.Year;
					period = avaliableVoucherDate.Month;
					day = avaliableVoucherDate.Day;
				}
				else
				{
					DateTime dateTime;
					if (day == 0)
					{
						DateTime t = new DateTime(year, period, 1);
						dateTime = ctx.DateNow;
						int year2 = dateTime.Year;
						dateTime = ctx.DateNow;
						if (t < new DateTime(year2, dateTime.Month, 1))
						{
							dateTime = new DateTime(year, period, 1);
							dateTime = dateTime.AddMonths(1);
							dateTime = dateTime.AddDays(-1.0);
							day = dateTime.Day;
						}
						else
						{
							int num = year;
							dateTime = ctx.DateNow;
							int num3;
							if (num == dateTime.Year)
							{
								int num2 = period;
								dateTime = ctx.DateNow;
								num3 = ((num2 == dateTime.Month) ? 1 : 0);
							}
							else
							{
								num3 = 0;
							}
							if (num3 != 0)
							{
								dateTime = ctx.DateNow;
								day = dateTime.Day;
							}
							else
							{
								day = 1;
							}
						}
					}
					List<GLSettlementModel> settlementModel = new GLSettlementRepository().GetSettlementModel(ctx, year, period, -1);
					int num4;
					if (settlementModel == null || settlementModel.Count <= 0 || (from x in settlementModel
					where x.MStatus == 1
					select x).Count() <= 0)
					{
						dateTime = new DateTime(year, period, 1, 23, 59, 59);
						dateTime = dateTime.AddMonths(1);
						num4 = ((dateTime.AddDays(-1.0) < ctx.MGLBeginDate) ? 1 : 0);
					}
					else
					{
						num4 = 1;
					}
					if (num4 != 0)
					{
						DateTime avaliableVoucherDate2 = new GLSettlementRepository().GetAvaliableVoucherDate(ctx);
						year = avaliableVoucherDate2.Year;
						period = avaliableVoucherDate2.Month;
						day = avaliableVoucherDate2.Day;
					}
				}
				gLVoucherModel.MNumber = COMResourceHelper.GetNextVoucherNumber(ctx, year, period, lastVoucherNumber, null);
				gLVoucherModel.MDate = new DateTime(year, period, day);
				gLVoucherModel.MAttachments = 0;
				gLVoucherModel.MVoucherEntrys = new List<GLVoucherEntryModel>
				{
					new GLVoucherEntryModel(),
					new GLVoucherEntryModel(),
					new GLVoucherEntryModel(),
					new GLVoucherEntryModel(),
					new GLVoucherEntryModel()
				};
				gLVoucherModel.MCreatorName = ((ctx.MLCID == LangCodeEnum.EN_US) ? (ctx.MFirstName + " " + ctx.MLastName) : (ctx.MLastName + " " + ctx.MFirstName));
				gLVoucherModel.MOrgID = ctx.MOrgID;
				gLVoucherModel.MYear = year;
				gLVoucherModel.MPeriod = period;
				gLVoucherModel.MStatus = -1;
			}
			else
			{
				gLVoucherModel = new GLVoucherRepository().GetVoucherModel(ctx, MItemID, true);
				List<SECUserModel> allUserByID = new SECUserRepository().GetAllUserByID(ctx, gLVoucherModel.MCreatorID);
				if (allUserByID != null && allUserByID.Count > 0)
				{
					for (int i = 0; i < allUserByID.Count; i++)
					{
						if (!string.IsNullOrWhiteSpace(allUserByID[i].MFirstName))
						{
							gLVoucherModel.MCreatorName = ((ctx.MLCID == LangCodeEnum.EN_US) ? (allUserByID[i].MFirstName + " " + allUserByID[i].MLastName) : (allUserByID[i].MLastName + " " + allUserByID[i].MFirstName));
						}
					}
				}
				if (gLVoucherModel.MStatus == -1 && string.IsNullOrWhiteSpace(gLVoucherModel.MNumber))
				{
					gLVoucherModel.MNumber = COMResourceHelper.GetNextVoucherNumber(ctx, gLVoucherModel.MYear, gLVoucherModel.MPeriod, null, null);
				}
			}
			return gLVoucherModel;
		}

		public List<CommandInfo> GetDeleteVoucherModelsCmd(MContext ctx, List<string> voucherIds)
		{
			ValidateDeleteVouchers(ctx, voucherIds);
			List<CommandInfo> list = new List<CommandInfo>();
			list.AddRange(GLPeriodTransferRepository.GetDeleteCmdByVoucherID(ctx, voucherIds));
			if (ctx.MFABeginDate.IsValidDateTime())
			{
				list.AddRange(new FADepreciationRepository().GetDeleteDepreciatedVoucherCmds(ctx, voucherIds));
			}
			if (ctx.MOrgVersionID == 1)
			{
				list.AddRange(new FPFapiaoRepository().GetDeleteFapiaoVoucherCmds(ctx, voucherIds));
			}
			list.AddRange(GetDeleteVoucherCmds(ctx, voucherIds));
			list.AddRange(GLInterfaceRepository.DeleteBizDataByVoucher(ctx, voucherIds));
			list.AddRange(GlVoucherLogHelper.GetDeleteLogCmds(ctx, (from x in voucherIds
			select new GLVoucherModel
			{
				MItemID = x
			}).ToList()));
			return list;
		}

		public CommandInfo ApproveVoucherByStatus(MContext ctx, string status, string oldStatus)
		{
			CommandInfo commandInfo = new CommandInfo();
			commandInfo.CommandText = "update t_gl_voucher set MStatus = @MStatus where MOrgID=@MOrgID and MStatus=@OldStatus  and MIsDelete = 0";
			DbParameter[] array = commandInfo.Parameters = new MySqlParameter[3]
			{
				new MySqlParameter
				{
					ParameterName = "@MStatus",
					Value = status
				},
				new MySqlParameter
				{
					ParameterName = "@MOrgID",
					Value = ctx.MOrgID
				},
				new MySqlParameter
				{
					ParameterName = "@OldStatus",
					Value = oldStatus
				}
			};
			return commandInfo;
		}

		public static List<CommandInfo> GetDeleteVoucherCmds(MContext ctx, List<string> voucherIds)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			List<MySqlParameter> list2 = ctx.GetParameters((MySqlParameter)null).ToList();
			string inFilterQuery = GLUtility.GetInFilterQuery(voucherIds, ref list2, "M_ID");
			CommandInfo commandInfo = new CommandInfo();
			commandInfo.CommandText = "update t_gl_voucher set MNumber = null, MStatus = -1  where MOrgID = @MOrgID and MIsDelete = 0  and  MItemID " + inFilterQuery + " and exists(select 1 from t_gl_doc_voucher where MOrgID = @MOrgID and MVoucherID " + inFilterQuery + " and MIsDelete = 0  and MergeStatus != 1 )";
			DbParameter[] array = commandInfo.Parameters = list2.ToArray();
			CommandInfo item = commandInfo;
			list.Add(item);
			commandInfo = new CommandInfo();
			commandInfo.CommandText = "update t_gl_voucher set MIsDelete = 1  where MOrgID = @MOrgID and MIsDelete = 0  and  MItemID " + inFilterQuery + " and not exists(select 1 from t_gl_doc_voucher where MOrgID = @MOrgID and MVoucherID " + inFilterQuery + " and MergeStatus != 1 )";
			array = (commandInfo.Parameters = list2.ToArray());
			CommandInfo item2 = commandInfo;
			list.Add(item2);
			commandInfo = new CommandInfo();
			commandInfo.CommandText = "update t_gl_voucherentry set MIsDelete = 1 where MOrgID = @MOrgID and MIsDelete = 0  and  MID " + inFilterQuery + " and not exists(select 1 from t_gl_doc_voucher where MOrgID = @MOrgID and MVoucherID " + inFilterQuery + " and MergeStatus != 1 )";
			array = (commandInfo.Parameters = list2.ToArray());
			CommandInfo item3 = commandInfo;
			list.Add(item3);
			CommandInfo obj = new CommandInfo
			{
				CommandText = "update t_gl_voucher set MRVoucherID = null where MOrgID = @MOrgID and MIsDelete = 0 and MRVoucherID " + inFilterQuery
			};
			array = (obj.Parameters = list2.ToArray());
			CommandInfo item4 = obj;
			list.Add(item4);
			CommandInfo obj2 = new CommandInfo
			{
				CommandText = "update t_gl_doc_voucher t1 set t1.MIsDelete = 1 where t1.MOrgID = @MOrgID and t1.MDocID in (select * from (select MDocID from t_gl_doc_voucher where MVoucherID " + inFilterQuery + " and MOrgID = @MOrgID)t ) and t1.MergeStatus = 1 "
			};
			array = (obj2.Parameters = list2.ToArray());
			CommandInfo item5 = obj2;
			list.Add(item5);
			CommandInfo obj3 = new CommandInfo
			{
				CommandText = "update t_gl_doc_voucher t1 set t1.MIsActive = 1, t1.MergeStatus = 0 where t1.MOrgID = @MOrgID and t1.MDocID in (select * from (select MDocID from t_gl_doc_voucher where MVoucherID " + inFilterQuery + " and MOrgID = @MOrgID)t ) and t1.MergeStatus = -1 "
			};
			array = (obj3.Parameters = list2.ToArray());
			CommandInfo item6 = obj3;
			list.Add(item6);
			return list;
		}

		public static List<GLVoucherModel> GetRelateDeleteVoucherList(MContext ctx, List<string> pkIDS)
		{
			if (ctx.MOrgVersionID != Convert.ToInt32(1))
			{
				return new List<GLVoucherModel>();
			}
			List<MySqlParameter> list = new List<MySqlParameter>
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			string deletedVoucherIDSql = GLInterfaceRepository.GetDeletedVoucherIDSql(ctx, pkIDS, ref list);
			if (string.IsNullOrEmpty(deletedVoucherIDSql))
			{
				return new List<GLVoucherModel>();
			}
			string sql = $"SELECT MItemID,MNumber,MStatus FROM t_gl_voucher WHERE MOrgID=@MOrgID AND MIsDelete=0 AND MItemID IN ({deletedVoucherIDSql})";
			List<GLVoucherModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<GLVoucherModel>(ctx, sql, list.ToArray());
			if (dataModelBySql == null || dataModelBySql.Count == 0)
			{
				return new List<GLVoucherModel>();
			}
			return (from t in dataModelBySql
			where !pkIDS.Contains(t.MItemID)
			select t).ToList();
		}

		public List<GLVoucherModel> GetVoucherByCodes(MContext ctx, int year, int period, List<string> codes, bool sameCount = false)
		{
			List<GLVoucherModel> list = new List<GLVoucherModel>();
			List<GLVoucherModel> voucherModelPageList = GetVoucherModelPageList<GLVoucherModel>(ctx, new GLVoucherListFilterModel
			{
				Year = year,
				Period = period,
				rows = 0,
				Status = 1.ToString()
			}, false);
			int num = 0;
			while (voucherModelPageList != null && num < voucherModelPageList.Count)
			{
				if (!sameCount || voucherModelPageList[num].MVoucherEntrys.Count == codes.Count)
				{
					int num2 = 0;
					for (int i = 0; i < voucherModelPageList[num].MVoucherEntrys.Count; i++)
					{
						for (int j = 0; j < codes.Count; j++)
						{
							IEnumerable<string> source = from x in new BDAccountRepository().GetChildrenAccountByCode(ctx, new List<string>
							{
								codes[j]
							})
							select x.MCode;
							if (source.Contains(voucherModelPageList[num].MVoucherEntrys[i].MAccountCode))
							{
								num2++;
								codes.Remove(codes[j]);
								break;
							}
						}
					}
					if (num2 == voucherModelPageList[num].MVoucherEntrys.Count)
					{
						list.Add(voucherModelPageList[num]);
					}
				}
				num++;
			}
			return list;
		}

		public string GetVoucherModelSelectSql()
		{
			return string.Format("SELECT distinct\r\n                t1.MItemID,\r\n                t1.MOrgID,\r\n                t1.MDate,\r\n                t1.MPeriod,\r\n                t1.MYear,\r\n                t1.MNumber,\r\n                t1.MVoucherGroupID,\r\n                t1.MAttachments,\r\n                t1.MInternalind,\r\n                t1.MReference,\r\n                t1.MDebittotal,\r\n                t1.MCredittotal,\r\n                t1.MStatus,\r\n                t1.MAuditorid,\r\n                t1.MAuditdate,\r\n                t1.MSourceBillKey,\r\n                t1.MIsDelete,\r\n                t1.MCreatorID,\r\n                t1.MCreateDate,\r\n                t1.MModifierID,\r\n                t1.MModifyDate,\r\n                t2.MEntryID,\r\n                t2.MID,\r\n                t2.MExplanation,\r\n                t2.MAccountID,\r\n                t2.MAmountfor,\r\n                t2.MAmount,\r\n                t2.MContactID,\r\n                t2.MCurrencyID,\r\n                t14.MName as MCurrencyName,\r\n                t2.MTrackItem1,\r\n                t2.MTrackItem2,\r\n                t2.MTrackItem3,\r\n                t2.MTrackItem4,\r\n                t2.MTrackItem5,\r\n                t2.MExchangeRate,\r\n                t2.MDC,\r\n                t2.MDebit,\r\n                t2.MCredit,\r\n                t2.Mentryseq,\r\n                t2.MSideEntrySeq,\r\n                t31.MNumber as MAccountNo,\r\n                t3.MName as MAccountNameOnly,\r\n                t3.MFullName as MAccountName,\r\n                t31.IsCanRelateContact,\r\n                t31.MIsCheckForCurrency,\r\n                t31.MCode,\r\n                t31.MAccountTypeID,\r\n                convert(AES_DECRYPT(t4.MName,'{0}') using utf8) as MContactName,\r\n                t5.MName as MTrackItem1Name,\r\n                t6.MName as MTrackItem2Name,\r\n                t7.MName as MTrackItem3Name,\r\n                t8.MName as MTrackItem4Name,\r\n                t9.MName as MTrackItem5Name,\r\n                F_GetUserName(t10.MFristName,t10.MLastName) as MCreatorName,\r\n                t11.MStatus as MSettlementStatus,\r\n                t12.MDocType,\r\n                t12.MDocID,\r\n                t13.MTransferTypeID\r\n            FROM\r\n                t_gl_voucher t1\r\n                    JOIN\r\n                t_gl_voucherentry t2 ON t1.MItemID = t2.MID\r\n                    AND t1.MOrgID = @MOrgID \r\n                    and t2.MIsDelete = 0\r\n                    left JOIN\r\n                t_bd_account_l t3 ON t3.MParentID = t2.MAccountID\r\n                    AND t3.MLocaleID = @MLCID\r\n                    and t3.MOrgID = t1.MOrgID\r\n                    and t3.MIsDelete = 0\r\n                    left JOIN\r\n                t_bd_account t31 ON t31.MItemID = t2.MAccountID\r\n                    and t31.MOrgID = t1.MOrgID\r\n                    and t31.MIsDelete = 0\r\n                    LEFT JOIN\r\n                t_bd_contacts_l t4 ON t4.MParentID = t2.MContactID\r\n                    AND t4.MLocaleID = @MLCID\r\n                    and t4.MOrgID = t1.MOrgID\r\n                    and t4.MIsDelete = 0\r\n                    LEFT JOIN\r\n                t_bd_trackentry_l t5 ON t5.MParentID = t2.MTrackItem1\r\n                    AND t5.MLocaleID = @MLCID\r\n                    and t5.MOrgID = t1.MOrgID\r\n                    and t5.MIsDelete = 0\r\n                    LEFT JOIN\r\n                t_bd_trackentry_l t6 ON t6.MParentID = t2.MTrackItem2\r\n                    and t6.MOrgID = t1.MOrgID\r\n                    AND t6.MLocaleID = @MLCID\r\n                    and t6.MIsDelete = 0\r\n                    LEFT JOIN\r\n                t_bd_trackentry_l t7 ON t7.MParentID = t2.MTrackItem3\r\n                    and t7.MOrgID = t1.MOrgID\r\n                    AND t7.MLocaleID = @MLCID\r\n                    and t7.MIsDelete = 0\r\n                    LEFT JOIN\r\n                t_bd_trackentry_l t8 ON t8.MParentID = t2.MTrackItem4\r\n                    and t8.MOrgID = t1.MOrgID\r\n                    AND t8.MLocaleID = @MLCID\r\n                    and t8.MIsDelete = 0\r\n                    LEFT JOIN\r\n                t_bd_trackentry_l t9 ON t9.MParentID = t2.MTrackItem5\r\n                    and t9.MOrgID = t1.MOrgID\r\n                    AND t9.MLocaleID = @MLCID\r\n                    and t9.MIsDelete = 0\r\n                 LEFT JOIN\r\n                ({2}) t10 on t10.MParentID = t1.MCreatorID\r\n                 LEFT JOIN\r\n                t_gl_settlement t11 on t11.MYear = t1.MYear\r\n                    and t11.MOrgID = t1.MOrgID\r\n                    AND t11.MPeriod = t1.MPeriod\r\n                    AND t11.MOrgID = t1.MOrgID\r\n                    and t11.MIsDelete = 0\r\n                 LEFT JOIN\r\n                t_gl_doc_voucher t12 \r\n                    on t12.MVoucherID = t1.MItemID\r\n                    and t12.MOrgID = t1.MOrgID\r\n                    and t12.MIsDelete = 0\r\n                 LEFT JOIN\r\n                t_gl_periodtransfer t13 \r\n                    on t13.MVoucherID = t1.MItemID\r\n                    and t13.MOrgID = t1.MOrgID\r\n                    and t13.MIsDelete = 0\r\n                 LEFT JOIN\r\n                t_bas_currency_l t14 \r\n                    on t2.MCurrencyID=t14.MParentID\r\n                    AND t14.MLocaleID = @MLCID\r\n                    and t14.MIsDelete = 0\r\n                where t1.MOrgID = @MOrgID and t1.MIsDelete = 0\r\n                    ", "JieNor-001", LangCodeEnum.EN_US, GetUserMultiSql());
		}

		public List<MySqlParameter> GetParameterList(MContext ctx, GLVoucherListFilterModel filter)
		{
			List<MySqlParameter> list = new List<MySqlParameter>();
			list.Add(new MySqlParameter
			{
				ParameterName = "@Keyword",
				Value = filter.KeyWord,
				MySqlDbType = MySqlDbType.VarChar
			});
			list.Add(new MySqlParameter
			{
				ParameterName = "@DecimalKeyWord",
				Value = (object)filter.DecimalKeyWord,
				MySqlDbType = MySqlDbType.Decimal
			});
			list.Add(new MySqlParameter
			{
				ParameterName = "@MLCID",
				Value = ctx.MLCID,
				MySqlDbType = MySqlDbType.VarChar
			});
			list.Add(new MySqlParameter
			{
				ParameterName = "@MOrgID",
				Value = ctx.MOrgID,
				MySqlDbType = MySqlDbType.VarChar
			});
			list.Add(new MySqlParameter
			{
				ParameterName = "@MYear",
				Value = (object)filter.Year,
				MySqlDbType = MySqlDbType.Int32
			});
			list.Add(new MySqlParameter
			{
				ParameterName = "@MPeriod",
				Value = (object)filter.Period,
				MySqlDbType = MySqlDbType.Int32
			});
			list.Add(new MySqlParameter
			{
				ParameterName = "@StartPeriod",
				Value = (object)(filter.StartYear * 12 + filter.StartPeriod),
				MySqlDbType = MySqlDbType.Int32
			});
			list.Add(new MySqlParameter
			{
				ParameterName = "@EndPeriod",
				Value = (object)(filter.EndYear * 12 + filter.EndPeriod),
				MySqlDbType = MySqlDbType.Int32
			});
			list.Add(new MySqlParameter
			{
				ParameterName = "@MNumber",
				Value = filter.MNumber,
				MySqlDbType = MySqlDbType.VarChar
			});
			if (!string.IsNullOrWhiteSpace(filter.Status))
			{
				int num = 0;
				int.TryParse(filter.Status, out num);
				list.Add(new MySqlParameter
				{
					ParameterName = "@MStatus",
					Value = (object)num,
					MySqlDbType = MySqlDbType.Int32
				});
			}
			else
			{
				list.Add(new MySqlParameter
				{
					ParameterName = "@MStatus",
					Value = filter.Status,
					MySqlDbType = MySqlDbType.VarChar
				});
			}
			list.Add(new MySqlParameter
			{
				ParameterName = "@MCode",
				Value = filter.AccountCode,
				MySqlDbType = MySqlDbType.VarChar
			});
			list.Add(new MySqlParameter
			{
				ParameterName = "@MAccountTypeID",
				Value = filter.AccountTypeID,
				MySqlDbType = MySqlDbType.VarChar
			});
			list.Add(new MySqlParameter
			{
				ParameterName = "@MTrackItem1",
				Value = filter.MTrackItem1,
				MySqlDbType = MySqlDbType.VarChar
			});
			list.Add(new MySqlParameter
			{
				ParameterName = "@MTrackItem2",
				Value = filter.MTrackItem2,
				MySqlDbType = MySqlDbType.VarChar
			});
			list.Add(new MySqlParameter
			{
				ParameterName = "@MTrackItem3",
				Value = filter.MTrackItem3,
				MySqlDbType = MySqlDbType.VarChar
			});
			list.Add(new MySqlParameter
			{
				ParameterName = "@MTrackItem4",
				Value = filter.MTrackItem4,
				MySqlDbType = MySqlDbType.VarChar
			});
			list.Add(new MySqlParameter
			{
				ParameterName = "@MTrackItem5",
				Value = filter.MTrackItem5,
				MySqlDbType = MySqlDbType.VarChar
			});
			list.Add(new MySqlParameter
			{
				ParameterName = "@MTransferTypeID",
				Value = filter.MTransferTypeID,
				MySqlDbType = MySqlDbType.VarChar
			});
			list.Add(new MySqlParameter
			{
				ParameterName = "@MCurrencyID",
				Value = filter.MCurrencyID,
				MySqlDbType = MySqlDbType.VarChar
			});
			return list;
		}

		public List<T> GetVoucherModelPageList<T>(MContext ctx, GLVoucherListFilterModel filter, bool includeDraft = false)
		{
			List<T> result = new List<T>();
			List<MySqlParameter> parameterList = GetParameterList(ctx, filter);
			string voucherModelSelectSql = GetVoucherModelSelectSql();
			string str = string.IsNullOrWhiteSpace(filter.KeyWord) ? GetVoucherMItemIDs(filter, false, false) : GetVoucherMItemIDsWithKeyword(filter, false, false);
			voucherModelSelectSql = voucherModelSelectSql + " and t1.MItemID in (" + str + ")";
			if (!includeDraft)
			{
				voucherModelSelectSql = voucherModelSelectSql + " and t1.MStatus != " + -1 + " and length(ifnull(t1.MNumber,'')) > 0 ";
			}
			voucherModelSelectSql += " order by t1.MItemID,t1.MYear,t1.MPeriod,t1.MNumber,t2.Mentryseq ";
			DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(voucherModelSelectSql, parameterList.ToArray());
			if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
			{
				result = TranslateDataTable2List<T>(ctx, dataSet.Tables[0]);
			}
			return result;
		}

		public int GetVoucherModelPageListCount(MContext ctx, GLVoucherListFilterModel filter)
		{
			string sql = (!string.IsNullOrWhiteSpace(filter.KeyWord)) ? GetVoucherMItemIDsWithKeyword(filter, true, false) : GetVoucherMItemIDs(filter, true, false);
			List<MySqlParameter> parameterList = GetParameterList(ctx, filter);
			DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(sql, parameterList.ToArray());
			if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
			{
				int result = 0;
				if (int.TryParse(dataSet.Tables[0].Rows[0]["total"].ToString(), out result))
				{
					return result;
				}
			}
			return 0;
		}

		private string GetVoucherMItemIDsWithKeyword(GLVoucherListFilterModel filter, bool calCount = false, bool includeDraft = false)
		{
			string text = string.Format("SELECT DISTINCT\r\n                    t1.MItemID\r\n                FROM\r\n                    t_gl_voucher t1\r\n                        INNER JOIN\r\n                    t_gl_voucherentry t2 ON t1.MItemID = t2.MID\r\n                        AND t2.MOrgID = t1.MOrgID\r\n                        and t2.MIsDelete = 0 \r\n                        INNER JOIN\r\n                    t_bd_account_l t3 ON t3.MParentID = t2.MAccountID\r\n                        AND t3.MLocaleID = @MLCID\r\n                        AND t3.MOrgID = t1.MOrgID\r\n                        and t3.MIsDelete = 0 \r\n                        INNER JOIN\r\n                    t_bd_account t31 ON t31.MItemID = t2.MAccountID\r\n                        AND t31.MOrgID = t1.MOrgID\r\n                        and t31.MIsDelete = 0 \r\n                        LEFT JOIN\r\n                    t_bd_contacts_l t4 ON t4.MParentID = t2.MContactID\r\n                        AND t4.MOrgID = t1.MOrgID\r\n                        and t4.MIsDelete = 0 \r\n                        AND t4.MLocaleID = @MLCID\r\n                        LEFT JOIN\r\n                    t_bd_trackentry_l t5 ON t5.MParentID = t2.MTrackItem1\r\n                        AND t5.MOrgID = t1.MOrgID\r\n                        and t5.MIsDelete = 0 \r\n                        AND t5.MLocaleID = @MLCID\r\n                        LEFT JOIN\r\n                    t_bd_trackentry_l t6 ON t6.MParentID = t2.MTrackItem2\r\n                        AND t6.MLocaleID = @MLCID\r\n                        AND t6.MOrgID = t1.MOrgID\r\n                        and t6.MIsDelete = 0 \r\n                        LEFT JOIN\r\n                    t_bd_trackentry_l t7 ON t7.MParentID = t2.MTrackItem3\r\n                        AND t7.MLocaleID = @MLCID\r\n                        AND t7.MOrgID = t1.MOrgID\r\n                        and t7.MIsDelete = 0 \r\n                        LEFT JOIN\r\n                    t_bd_trackentry_l t8 ON t8.MParentID = t2.MTrackItem4\r\n                        AND t8.MLocaleID = @MLCID\r\n                        AND t8.MOrgID = t1.MOrgID\r\n                        and t8.MIsDelete = 0 \r\n                        LEFT JOIN\r\n                    t_bd_trackentry_l t9 ON t9.MParentID = t2.MTrackItem5\r\n                        AND t9.MLocaleID = @MLCID\r\n                        AND t9.MOrgID = t1.MOrgID\r\n                        and t9.MIsDelete = 0 \r\n                        LEFT JOIN\r\n                    ({1}) t10 on t10.MParentID = t1.MCreatorID\r\n                        LEFT JOIN\r\n                    t_gl_settlement t11 on t11.MYear = t1.MYear\r\n                        AND t11.MPeriod = t1.MPeriod\r\n                        AND t11.MOrgID = t1.MOrgID\r\n                        AND t11.MOrgID = t1.MOrgID\r\n                        and t11.MIsDelete = 0 \r\n                        LEFT JOIN\r\n                    t_gl_doc_voucher t12 on t12.MVoucherID = t1.MItemID\r\n                        AND t12.MOrgID = t1.MOrgID\r\n                        and t12.MIsDelete = 0 \r\n                        LEFT JOIN\r\n                    t_gl_periodtransfer t13 on t13.MVoucherID = t1.MItemID\r\n                        AND t13.MOrgID = t1.MOrgID\r\n                        and t13.MIsDelete = 0 \r\n                WHERE length(ifnull(t1.MNumber,'')) > 0 and t1.MOrgID = @MOrgID and t1.MIsDelete = 0 \r\n                    AND (t1.MNumber LIKE concat('%',@Keyword,'%')\r\n                        OR t1.MYear LIKE concat('%',@Keyword,'%')\r\n                        OR t1.MPeriod LIKE concat('%',@Keyword,'%')\r\n                        OR t1.MDebittotal LIKE concat('%',@Keyword,'%')\r\n                        OR t1.MCredittotal LIKE concat('%',@Keyword,'%')\r\n                        OR t2.MExplanation LIKE concat('%',@Keyword,'%')\r\n                        OR t2.MDebit LIKE concat('%',@Keyword,'%')\r\n                        OR t2.MCredit LIKE concat('%',@Keyword,'%')\r\n                        OR t2.MExchangeRate LIKE concat('%',@Keyword,'%')\r\n                        OR t2.MCurrencyID LIKE  concat('%',@Keyword,'%')\r\n                        OR t3.MName LIKE concat('%',@Keyword,'%')\r\n                        OR t3.MFullName LIKE concat('%',@Keyword,'%')\r\n                        OR t31.MNumber LIKE concat('%',@Keyword,'%')\r\n                        OR t31.MCode LIKE concat('%',@Keyword,'%')\r\n                        OR t31.MAccountTypeID LIKE concat('%',@Keyword,'%')\r\n                        OR convert(AES_DECRYPT(t4.MName,'{0}') using utf8) LIKE concat('%',@Keyword,'%')\r\n                        OR t5.MName LIKE concat('%',@Keyword,'%')\r\n                        OR t6.MName LIKE concat('%',@Keyword,'%')\r\n                        OR t7.MName LIKE concat('%',@Keyword,'%')\r\n                        OR t8.MName LIKE concat('%',@Keyword,'%')\r\n                        OR t9.MName LIKE concat('%',@Keyword,'%')\r\n                        OR t10.MFristName LIKE concat('%',@Keyword,'%')\r\n                        OR t10.MLastName LIKE concat('%',@Keyword,'%'))", "JieNor-001", GetUserMultiSql());
			if (!includeDraft)
			{
				text = text + " and t1.MStatus != " + -1 + " and length(ifnull(t1.MNumber,'')) > 0 ";
			}
			if (filter.Year > 0 && filter.Period > 0)
			{
				text += " and t1.MYear = @MYear and t1.MPeriod = @MPeriod ";
			}
			if (!string.IsNullOrWhiteSpace(filter.MNumber))
			{
				text += " and t1.MNumber = @MNumber ";
			}
			if (!string.IsNullOrWhiteSpace(filter.Status))
			{
				text += " and t1.MStatus = @MStatus ";
			}
			if (!string.IsNullOrWhiteSpace(filter.MTransferTypeID))
			{
				text += " and t13.MTransferTypeID = @MTransferTypeID";
			}
			if (!string.IsNullOrWhiteSpace(filter.From))
			{
				switch (filter.From)
				{
				case "0":
					text += " and t1.MSourceBillKey is null and not exists( select 1 from t_gl_doc_voucher tx where tx.MVoucherID = t1.MItemID ) ";
					break;
				case "1":
					text += " and t1.MSourceBillKey = '1'";
					break;
				case "2":
					text += " and exists( select 1 from t_gl_doc_voucher tx where tx.MVoucherID = t1.MItemID ) ";
					break;
				case "3":
					text += " and t1.MSourceBillKey = '3'";
					break;
				}
			}
			if (!string.IsNullOrWhiteSpace(filter.AccountCode))
			{
				text += " and t31.MCode = @MCode ";
			}
			if (!string.IsNullOrWhiteSpace(filter.AccountTypeID))
			{
				text += " and t31.MAccountTypeID = @MAccountTypeID ";
			}
			text += "  ORDER BY t1.MNumber desc ";
			if (filter.rows > 0 && !calCount)
			{
				text = text + " LIMIT " + (filter.page - 1) * filter.rows + "," + filter.rows;
			}
			return "select " + (calCount ? "count(*) total " : "*") + " from (" + text + ") t10";
		}

		private string GetVoucherMItemIDs(GLVoucherListFilterModel filter, bool calCount = false, bool includeDraft = false)
		{
			string text = "SELECT DISTINCT\r\n                    t1.MItemID\r\n                FROM\r\n                    t_gl_voucher t1\r\n                        INNER JOIN\r\n                    t_gl_voucherentry t2 ON t1.MItemID = t2.MID \r\n                        and t1.MOrgID = t2.MOrgID\r\n                        and t2.MIsDelete = 0 \r\n                WHERE t1.MOrgID = @MOrgID  and t1.MIsDelete = 0 ";
			if (!includeDraft)
			{
				text = text + " and t1.MStatus != " + -1 + " and length(ifnull(t1.MNumber,'')) > 0 ";
			}
			if (filter.Year > 0 && filter.Period > 0)
			{
				text += " and t1.MYear = @MYear and t1.MPeriod = @MPeriod ";
			}
			if (!string.IsNullOrWhiteSpace(filter.MNumber))
			{
				text += " and t1.MNumber = @MNumber ";
			}
			if (!string.IsNullOrWhiteSpace(filter.MTransferTypeID))
			{
				text += " and t13.MTransferTypeID = @MTransferTypeID";
			}
			if (!string.IsNullOrWhiteSpace(filter.Status))
			{
				text += " and t1.MStatus = @MStatus ";
			}
			if (!string.IsNullOrWhiteSpace(filter.From))
			{
				switch (filter.From)
				{
				case "0":
					text += " and t1.MSourceBillKey is null and not exists( select 1 from t_gl_doc_voucher tx where tx.MVoucherID = t1.MItemID ) ";
					break;
				case "1":
					text += " and t1.MSourceBillKey = '1'";
					break;
				case "2":
					text += " and exists( select 1 from t_gl_doc_voucher tx where tx.MVoucherID = t1.MItemID ) ";
					break;
				case "3":
					text += " and t1.MSourceBillKey = '3'";
					break;
				}
			}
			if (!string.IsNullOrWhiteSpace(filter.AccountCode))
			{
				text += " and t31.MCode = @MCode ";
			}
			if (!string.IsNullOrWhiteSpace(filter.AccountTypeID))
			{
				text += " and t31.MAccountTypeID = @MAccountTypeID ";
			}
			text += " ORDER BY t1.MNumber desc ";
			if (filter.rows > 0 && !calCount)
			{
				text = text + " LIMIT " + (filter.page - 1) * filter.rows + "," + filter.rows;
			}
			return "select " + (calCount ? "count(*) total " : "*") + " from (" + text + ") t10";
		}

		public List<T> TranslateDataTable2List<T>(MContext ctx, DataTable dt)
		{
			List<GLVoucherModel> list = new List<GLVoucherModel>();
			List<GLVoucherEntryModel> list2 = new List<GLVoucherEntryModel>();
			GLVoucherModel gLVoucherModel = new GLVoucherModel();
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				DataRow dataRow = dt.Rows[i];
				string text = dataRow["MItemID"].ToString();
				if (text != gLVoucherModel.MItemID)
				{
					if (!string.IsNullOrWhiteSpace(gLVoucherModel.MItemID))
					{
						list.Add(gLVoucherModel);
					}
					gLVoucherModel = new GLVoucherModel
					{
						MItemID = text,
						MOrgID = dataRow["MOrgID"].ToString(),
						MDate = Convert.ToDateTime(dataRow["MDate"].ToString()),
						MPeriod = int.Parse(dataRow["MPeriod"].ToString()),
						MYear = int.Parse(dataRow["MYear"].ToString()),
						MNumber = dataRow["MNumber"].ToString(),
						MVoucherGroupID = ((dataRow["MVoucherGroupID"] == null) ? null : dataRow["MVoucherGroupID"].ToString()),
						MAttachments = int.Parse(dataRow["MAttachments"].ToString()),
						MInternalIND = ((dataRow["MInternalind"] == null) ? null : dataRow["MInternalind"].ToString()),
						MReference = ((dataRow["MReference"] == null) ? null : dataRow["MReference"].ToString()),
						MDebitTotal = decimal.Parse(dataRow["MDebittotal"].ToString()),
						MCreditTotal = decimal.Parse(dataRow["MCredittotal"].ToString()),
						MStatus = ((dataRow["MStatus"] != null) ? int.Parse(dataRow["MStatus"].ToString()) : 0),
						MSettlementStatus = ((dataRow["MSettlementStatus"] != null && !string.IsNullOrWhiteSpace(dataRow["MSettlementStatus"].ToString())) ? int.Parse(dataRow["MSettlementStatus"].ToString()) : 0),
						MAuditorID = ((dataRow["MAuditorid"] == null) ? null : dataRow["MAuditorid"].ToString()),
						MAuditDate = ((dataRow["MAuditdate"] == null) ? DateTime.MinValue : Convert.ToDateTime(dataRow["MAuditdate"].ToString())),
						MSourceBillKey = ((dataRow["MSourceBillKey"] == null) ? null : dataRow["MSourceBillKey"].ToString()),
						MIsDelete = (dataRow["MIsDelete"] != null && dataRow["MIsDelete"].ToString() == "1" && true),
						MCreatorID = ((dataRow["MCreatorID"] == null) ? null : dataRow["MCreatorID"].ToString()),
						MCreateDate = ((dataRow["MCreateDate"] == null) ? DateTime.MinValue : Convert.ToDateTime(dataRow["MCreateDate"].ToString())),
						MCreatorName = ((dataRow["MCreatorName"] == null) ? null : dataRow["MCreatorName"].ToString()),
						MModifierID = ((dataRow["MModifierID"] == null) ? null : dataRow["MModifierID"].ToString()),
						MModifyDate = ((dataRow["MModifyDate"] == null) ? DateTime.MinValue : Convert.ToDateTime(dataRow["MModifyDate"].ToString())),
						MVoucherEntrys = new List<GLVoucherEntryModel>(),
						MDocID = ((dataRow["MDocID"] == null) ? null : dataRow["MDocID"].ToString()),
						MDocType = ((dataRow["MDocType"] == null || string.IsNullOrWhiteSpace(dataRow["MDocType"].ToString())) ? (-1) : int.Parse(dataRow["MDocType"].ToString())),
						MTransferTypeID = ((dataRow["MTransferTypeID"] == null || string.IsNullOrWhiteSpace(dataRow["MTransferTypeID"].ToString())) ? (-1) : int.Parse(dataRow["MTransferTypeID"].ToString()))
					};
				}
				GLVoucherEntryModel gLVoucherEntryModel = new GLVoucherEntryModel();
				gLVoucherEntryModel.MEntryID = dataRow["MEntryID"].ToString();
				gLVoucherEntryModel.MID = dataRow["MID"].ToString();
				gLVoucherEntryModel.MOrgID = dataRow["MOrgID"].ToString();
				gLVoucherEntryModel.MExplanation = ((dataRow["MExplanation"] == null) ? null : dataRow["MExplanation"].ToString());
				gLVoucherEntryModel.MAccountID = ((dataRow["MAccountID"] == null) ? "" : dataRow["MAccountID"].ToString());
				gLVoucherEntryModel.MAmountFor = decimal.Parse((dataRow["MAmountfor"] == null) ? "0" : dataRow["MAmountfor"].ToString());
				gLVoucherEntryModel.MAmount = decimal.Parse((dataRow["MAmount"] == null) ? "0" : dataRow["MAmount"].ToString());
				gLVoucherEntryModel.MContactID = ((dataRow["MContactID"] == null) ? "" : dataRow["MContactID"].ToString());
				gLVoucherEntryModel.MAccountNo = ((dataRow["MAccountNo"] == null) ? "" : Convert.ToString(dataRow["MAccountNo"]));
				gLVoucherEntryModel.MAccountNameOnly = ((dataRow["MAccountNameOnly"] == null) ? "" : Convert.ToString(dataRow["MAccountNameOnly"]));
				gLVoucherEntryModel.MAccountName = ((dataRow["MAccountName"] == null) ? "" : dataRow["MAccountName"].ToString());
				gLVoucherEntryModel.MAccountCode = ((dataRow["MCode"] == null) ? "" : dataRow["MCode"].ToString());
				gLVoucherEntryModel.IsCanRelateContact = (dataRow["IsCanRelateContact"] != null && dataRow["IsCanRelateContact"].ToString() == "1");
				gLVoucherEntryModel.MIsCheckForCurrency = (dataRow["MIsCheckForCurrency"] != null && dataRow["MIsCheckForCurrency"].ToString() == "1");
				gLVoucherEntryModel.MCurrencyID = ((dataRow["MCurrencyID"] == null) ? null : dataRow["MCurrencyID"].ToString());
				gLVoucherEntryModel.MCurrencyName = ((dataRow["MCurrencyName"] == null) ? null : dataRow["MCurrencyName"].ToString());
				gLVoucherEntryModel.MExchangeRate = decimal.Parse(Convert.IsDBNull(dataRow["MExchangeRate"]) ? "0" : dataRow["MExchangeRate"].ToString());
				gLVoucherEntryModel.MDC = int.Parse((dataRow["MDC"] == null) ? null : dataRow["MDC"].ToString());
				gLVoucherEntryModel.MCredit = decimal.Parse((dataRow["MCredit"] == null) ? "0" : dataRow["MCredit"].ToString());
				gLVoucherEntryModel.MDebit = decimal.Parse((dataRow["MDebit"] == null) ? "0" : dataRow["MDebit"].ToString());
				gLVoucherEntryModel.MEntrySeq = int.Parse((dataRow["Mentryseq"] == null) ? "0" : dataRow["Mentryseq"].ToString());
				gLVoucherEntryModel.MSideEntrySeq = ((dataRow["MSideEntrySeq"] == null) ? null : dataRow["MSideEntrySeq"].ToString());
				gLVoucherEntryModel.MTrackItem1 = ((dataRow["MTrackItem1"] == null) ? null : dataRow["MTrackItem1"].ToString());
				gLVoucherEntryModel.MTrackItem2 = ((dataRow["MTrackItem2"] == null) ? null : dataRow["MTrackItem2"].ToString());
				gLVoucherEntryModel.MTrackItem3 = ((dataRow["MTrackItem3"] == null) ? null : dataRow["MTrackItem3"].ToString());
				gLVoucherEntryModel.MTrackItem4 = ((dataRow["MTrackItem4"] == null) ? null : dataRow["MTrackItem4"].ToString());
				gLVoucherEntryModel.MTrackItem5 = ((dataRow["MTrackItem5"] == null) ? null : dataRow["MTrackItem5"].ToString());
				gLVoucherEntryModel.MTrackItem1Name = ((dataRow["MTrackItem1Name"] == null) ? null : dataRow["MTrackItem1Name"].ToString());
				gLVoucherEntryModel.MTrackItem2Name = ((dataRow["MTrackItem2Name"] == null) ? null : dataRow["MTrackItem2Name"].ToString());
				gLVoucherEntryModel.MTrackItem3Name = ((dataRow["MTrackItem3Name"] == null) ? null : dataRow["MTrackItem3Name"].ToString());
				gLVoucherEntryModel.MTrackItem4Name = ((dataRow["MTrackItem4Name"] == null) ? null : dataRow["MTrackItem4Name"].ToString());
				gLVoucherEntryModel.MTrackItem5Name = ((dataRow["MTrackItem5Name"] == null) ? null : dataRow["MTrackItem5Name"].ToString());
				gLVoucherModel.MVoucherEntrys.Add(gLVoucherEntryModel);
				list2.Add(gLVoucherEntryModel);
			}
			list.Add(gLVoucherModel);
			if (typeof(T) == typeof(GLVoucherEntryModel))
			{
				return list2 as List<T>;
			}
			list = (from x in list
			orderby x.MNumber descending
			select x).ToList();
			list.ForEach((Action<GLVoucherModel>)delegate(GLVoucherModel x)
			{
				if (x.MTransferTypeID != -1)
				{
					x.MTransferTypeName = new GLPeriodTransferRepository().GetTransferTypeNameByID(ctx, x.MTransferTypeID);
				}
			});
			return list as List<T>;
		}

		public bool HandleDuplicatedVoucherNumber(MContext ctx, int year, int period)
		{
			List<GLVoucherModel> list = (from x in GetVoucherModelByPeriod(ctx, year, period, -2)
			where !string.IsNullOrWhiteSpace(x.MNumber)
			select x).ToList();
			if (list.Count > 0)
			{
				list = (from x in list
				orderby int.Parse(x.MNumber)
				select x).ToList();
				List<int> list2 = (from x in list
				select int.Parse(x.MNumber)).ToList();
				IEnumerable<IGrouping<string, GLVoucherModel>> enumerable = from a in list
				group a by a.MNumber into a
				where a.Count() > 1
				select a;
				if (enumerable.Any())
				{
					foreach (IGrouping<string, GLVoucherModel> item in enumerable)
					{
						List<GLVoucherModel> list3 = (from a in item
						select (a) into y
						orderby y.MDate
						select y into m
						orderby m.MCreateDate
						select m).ToList();
						for (int i = 1; i < item.Count(); i++)
						{
							GLVoucherModel gLVoucherModel = list3[i];
							int nextAvaliableNumber = GetNextAvaliableNumber(list2);
							gLVoucherModel.MNumber = COMResourceHelper.ToVoucherNumber(ctx, null, nextAvaliableNumber);
							list2.Remove(int.Parse(list3[i].MNumber));
							list2.Add(nextAvaliableNumber);
							list2.Sort();
						}
					}
					List<CommandInfo> insertOrUpdateCmds = ModelInfoManager.GetInsertOrUpdateCmds(ctx, list, new List<string>
					{
						"MNumber"
					}, false);
					return new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(insertOrUpdateCmds) > 0;
				}
			}
			return true;
		}

		private int GetNextAvaliableNumber(List<int> numbers)
		{
			for (int i = 1; i < 1000; i++)
			{
				if (!numbers.Contains(i))
				{
					return i;
				}
			}
			return 1;
		}

		public bool ReorderVoucherNumber(MContext ctx, int year, int period, int start = 1)
		{
			List<GLVoucherModel> list = (from x in GetVoucherModelByPeriod(ctx, year, period, -2)
			where !string.IsNullOrWhiteSpace(x.MNumber) && x.MStatus >= 0 && int.Parse(x.MNumber) >= start
			select x).ToList();
			if (list.Count > 0)
			{
				list = (from x in list
				orderby int.Parse(x.MNumber)
				select x).ToList();
				if (start == -1)
				{
					start = list.Max((GLVoucherModel x) => int.Parse(x.MNumber));
				}
				GLVoucherModel gLVoucherModel = new GLVoucherModel
				{
					MNumber = COMResourceHelper.ToVoucherNumber(ctx, null, start)
				};
				List<CommandInfo> list2 = new List<CommandInfo>();
				for (int i = start; i < start + list.Count; i++)
				{
					gLVoucherModel = list[i - start];
					if (int.Parse(gLVoucherModel.MNumber) != i)
					{
						gLVoucherModel.MNumber = COMResourceHelper.ToVoucherNumber(ctx, null, i);
						list2.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<GLVoucherModel>(ctx, gLVoucherModel, new List<string>
						{
							"MNumber"
						}, false));
					}
				}
				return list2.Count == 0 || new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(list2) > 0;
			}
			return false;
		}

		public List<GLVoucherModel> GetModelListIncludeEntry(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			if (filter == null)
			{
				filter = new SqlWhere();
			}
			if (!includeDelete)
			{
				filter.AddDeleteFilter("MIsDelete", SqlOperators.Equal, false);
			}
			return ModelInfoManager.GetDataModelList<GLVoucherModel>(ctx, filter, false, true);
		}

		public List<GLVoucherEntryModel> GetVoucherEntryList(MContext ctx, GLVoucherListFilterModel filter)
		{
			return GetVoucherModelPageList<GLVoucherEntryModel>(ctx, filter, false);
		}

		public bool ExistVoucherEntryModel(MContext ctx, string accountId)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool result = false;
			stringBuilder.AppendLine("SELECT count(*) FROM t_gl_voucher a ");
			stringBuilder.AppendLine("INNER JOIN t_gl_voucherentry b on a.MItemID = b.MID and a.MOrgID = b.MOrgID and b.MIsDelete = 0 where  a.morgid=@MOrgID and b.maccountid=@accountId and a.MIsDelete = 0 ");
			List<MySqlParameter> list = new List<MySqlParameter>
			{
				new MySqlParameter
				{
					ParameterName = "@MOrgID",
					Value = ctx.MOrgID
				},
				new MySqlParameter
				{
					ParameterName = "@accountId",
					Value = accountId
				}
			};
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			object single = dynamicDbHelperMySQL.GetSingle(stringBuilder.ToString(), list.ToArray());
			int num = 0;
			if (int.TryParse(single.ToString(), out num))
			{
				result = (num > 0);
			}
			return result;
		}

		public static List<CommandInfo> GetInsertVoucherCmds(MContext ctx, GLVoucherModel model, List<string> fields = null)
		{
			new GLSettlementRepository().CheckPeriodHasSettlement(ctx, model.MYear, model.MPeriod);
			if ((fields == null || fields.Count == 0) && !PrehandleUpdateVoucher(ctx, model))
			{
				throw new Exception("凭证" + model.MNumber + "保存预处理出错");
			}
			return ModelInfoManager.GetInsertOrUpdateCmd<GLVoucherModel>(ctx, model, fields, true);
		}

		public static List<CommandInfo> GetInsertVoucherCmds(MContext ctx, List<GLVoucherModel> models, List<string> fields = null)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			int num = 0;
			while (models != null && models.Count > 0 && num < models.Count)
			{
				list.AddRange(GetInsertVoucherCmds(ctx, models[num], fields));
				list.AddRange(GlVoucherLogHelper.GetCreateLogCmd(ctx, models[num]));
				num++;
			}
			return list;
		}

		public static string GetUserMultiSql()
		{
			return "select MParentID, MFristName,MLastName,MIsDelete from T_Sec_User_L \r\n                    where IFNULL(MFristName,'') <>'' and IFNULL(MLastName,'')<>'' and MIsDelete=0\r\n                    group by MParentID";
		}

		public static void ProcessVoucher(MContext ctx, GLVoucherModel model)
		{
			decimal num = default(decimal);
			decimal num2 = default(decimal);
			decimal d = default(decimal);
			decimal d2 = default(decimal);
			if (model.MVoucherEntrys.Count < 2)
			{
				throw new MActionException(new List<MActionResultCodeEnum>
				{
					MActionResultCodeEnum.MVoucherEntryMoreThanOne
				});
			}
			for (int i = 0; i < model.MVoucherEntrys.Count; i++)
			{
				GLVoucherEntryModel gLVoucherEntryModel = model.MVoucherEntrys[i];
				gLVoucherEntryModel.MCurrencyID = (string.IsNullOrWhiteSpace(gLVoucherEntryModel.MCurrencyID) ? ctx.MBasCurrencyID : gLVoucherEntryModel.MCurrencyID);
				gLVoucherEntryModel.MExchangeRate = ((gLVoucherEntryModel.MCurrencyID == ctx.MBasCurrencyID) ? 1.0m : gLVoucherEntryModel.MExchangeRate);
				if (string.IsNullOrWhiteSpace(gLVoucherEntryModel.MCurrencyID))
				{
					gLVoucherEntryModel.MCurrencyID = ctx.MBasCurrencyID;
					gLVoucherEntryModel.MExchangeRate = 1.0m;
				}
				gLVoucherEntryModel.MDebit = Math.Round(gLVoucherEntryModel.MDebit, 2, MidpointRounding.AwayFromZero);
				gLVoucherEntryModel.MCredit = Math.Round(gLVoucherEntryModel.MCredit, 2, MidpointRounding.AwayFromZero);
				gLVoucherEntryModel.MDC = ((gLVoucherEntryModel.MDebit != decimal.Zero) ? 1 : (-1));
				gLVoucherEntryModel.MAmount = Math.Round((gLVoucherEntryModel.MDC == 1) ? gLVoucherEntryModel.MDebit : gLVoucherEntryModel.MCredit, 2, MidpointRounding.AwayFromZero);
				gLVoucherEntryModel.MAmountFor = Math.Round(gLVoucherEntryModel.MAmountFor, 2, MidpointRounding.AwayFromZero);
				num += gLVoucherEntryModel.MCredit;
				num2 += gLVoucherEntryModel.MDebit;
				d += gLVoucherEntryModel.MAmount;
				d2 += gLVoucherEntryModel.MAmountFor;
				if (gLVoucherEntryModel.MExplanation.Contains('·'))
				{
					gLVoucherEntryModel.MExplanation = string.Join(";", gLVoucherEntryModel.MExplanation.Split('·').Distinct());
				}
			}
			model.MDebitTotal = num2;
			model.MCreditTotal = num;
			if (!(model.MDebitTotal != model.MCreditTotal))
			{
				return;
			}
			model.Validate(ctx, true, "MCreditDebitImbalance", "凭证借贷不平衡", LangModule.Common);
			throw new MActionException(new List<MActionResultCodeEnum>
			{
				MActionResultCodeEnum.MCreditDebitImbalance
			});
		}

		public static bool PrehandleUpdateVoucher(MContext ctx, GLVoucherModel model)
		{
			if (model != null && model.MVoucherEntrys.Count > 1)
			{
				decimal num = default(decimal);
				decimal num2 = default(decimal);
				decimal num3 = default(decimal);
				decimal d = default(decimal);
				List<BDAccountModel> accountList = GLDataPool.GetInstance(ctx, false, 0, 0, 0).AccountList;
				for (int i = 0; i < model.MVoucherEntrys.Count; i++)
				{
					GLVoucherEntryModel entry = model.MVoucherEntrys[i];
					if (!string.IsNullOrWhiteSpace(model.MNumber) && string.IsNullOrWhiteSpace(entry.MAccountID))
					{
						return false;
					}
					BDAccountModel bDAccountModel = accountList.FirstOrDefault((BDAccountModel x) => x.MItemID == entry.MAccountID);
					if (!string.IsNullOrWhiteSpace(model.MNumber) && bDAccountModel == null)
					{
						MLogger.Log("科目未找到[" + entry.MAccountID + "]科目总数" + accountList.Count, (MContext)null);
						return false;
					}
					if (!string.IsNullOrWhiteSpace(entry.MCurrencyID) && entry.MCurrencyID != ctx.MBasCurrencyID && entry.MExchangeRate <= decimal.Zero)
					{
						return false;
					}
					entry.MCurrencyID = (string.IsNullOrWhiteSpace(entry.MCurrencyID) ? ctx.MBasCurrencyID : entry.MCurrencyID);
					entry.MExchangeRate = ((entry.MCurrencyID == ctx.MBasCurrencyID) ? 1.0m : entry.MExchangeRate);
					if (!string.IsNullOrWhiteSpace(model.MNumber) && bDAccountModel != null && entry.MCurrencyID != ctx.MBasCurrencyID && !bDAccountModel.MIsCheckForCurrency)
					{
						entry.MCurrencyID = ctx.MBasCurrencyID;
						entry.MExchangeRate = 1.0m;
					}
					entry.MDebit = Math.Round(entry.MDebit, 2, MidpointRounding.AwayFromZero);
					entry.MCredit = Math.Round(entry.MCredit, 2, MidpointRounding.AwayFromZero);
					entry.MDC = ((entry.MDC == 0) ? ((entry.MDebit != decimal.Zero) ? 1 : (-1)) : entry.MDC);
					entry.MAmount = Math.Round((entry.MDC == 1) ? entry.MDebit : entry.MCredit, 2, MidpointRounding.AwayFromZero);
					entry.MAmountFor = Math.Round(entry.MAmountFor, 2, MidpointRounding.AwayFromZero);
					num += entry.MCredit;
					num2 += entry.MDebit;
					num3 += entry.MAmount;
					d += entry.MAmountFor;
					if (entry.MExplanation.Contains('·') && !string.IsNullOrWhiteSpace(model.MNumber))
					{
						entry.MExplanation = string.Join(";", entry.MExplanation.Split('·').Distinct());
					}
				}
				model.MSourceBillKey = (string.IsNullOrWhiteSpace(model.MSourceBillKey) ? 0.ToString() : model.MSourceBillKey);
				model.MDebitTotal = num2;
				model.MCreditTotal = num;
				if (num == num2 && Math.Abs(num * 2m) == Math.Abs(num3))
				{
					return true;
				}
			}
			return false;
		}
	}
}
