using JieNor.Megi.BusinessContract.GL;
using JieNor.Megi.BusinessService.BAS;
using JieNor.Megi.BusinessService.BD;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace JieNor.Megi.BusinessService.GL
{
	public class GLPeriodTransferBusiness : IGLPeriodTransferBusiness, IDataContract<GLPeriodTransferModel>
	{
		private readonly GLPeriodTransferRepository dal = new GLPeriodTransferRepository();

		private readonly BASLangBusiness lang = new BASLangBusiness();

		private readonly GLVoucherRepository voucher = new GLVoucherRepository();

		private readonly GLVoucherEntryRepository entry = new GLVoucherEntryRepository();

		private readonly BDAccountBusiness accountBiz = new BDAccountBusiness();

		private readonly GLBalanceBusiness balanceBiz = new GLBalanceBusiness();

		private readonly GLInitBalanceBusiness initBalanceBiz = new GLInitBalanceBusiness();

		private readonly BDExchangeRateBusiness rateBiz = new BDExchangeRateBusiness();

		private string GetAccountNameByCode(List<BDAccountListModel> accounts, string code)
		{
			return accountBiz.GetAccountByCode(accounts, code).MName;
		}

		public List<GLPeriodTransferModel> GetExsitsAndCalculatedPeriodTransfer(MContext ctx, GLPeriodTransferModel model)
		{
			GLPeriodTransferModel gLPeriodTransferModel = GetPeriodTransfer(ctx, model, false);
			if (string.IsNullOrWhiteSpace(gLPeriodTransferModel.MVoucherID))
			{
				gLPeriodTransferModel = null;
			}
			GLPeriodTransferModel item = CalculatePeriodTransfer(ctx, model);
			return new List<GLPeriodTransferModel>
			{
				gLPeriodTransferModel,
				item
			};
		}

		public GLPeriodTransferModel CalculatePeriodTransfer(MContext ctx, GLPeriodTransferModel model)
		{
			model.MOrgID = (model.MOrgID ?? ctx.MOrgID);
			GLPeriodTransferModel periodTransfer = GetPeriodTransfer(ctx, model, false);
			DateTime dateTime = new DateTime(model.MYear, model.MPeriod, 1);
			DateTime dateTime2 = dateTime.AddMonths(-1);
			GLPeriodTransferModel lastPeriodTransferModel = GetLastPeriodTransferModel(ctx, model);
			List<BDAccountListModel> accountListIncludeBalance = accountBiz.GetAccountListIncludeBalance(ctx, new SqlWhere(), true);
			string text = lang.GetText(ctx, LangModule.Common, "Balance", "余额");
			string text2 = lang.GetText(ctx, LangModule.Common, "LastPeriod", "上期");
			string text3 = lang.GetText(ctx, LangModule.Common, "ThisPeriod", "本期");
			string text4 = lang.GetText(ctx, LangModule.Common, "TaxRate", "税率");
			string text5 = lang.GetText(ctx, LangModule.Common, "Income", "收入");
			string text6 = lang.GetText(ctx, LangModule.Common, "Expense", "费用");
			string text7 = lang.GetText(ctx, LangModule.Common, "TotalOf", "总额");
			string text8 = lang.GetText(ctx, LangModule.Common, "FinalTransfer", "期末调汇");
			string text9 = lang.GetText(ctx, LangModule.GL, "monthlyExchangeRateNotUpdate", "期末调汇汇率未维护");
			string text10 = (ctx.MLCID == "0x0009") ? " " : "";
			bool flag = periodTransfer != null && !string.IsNullOrWhiteSpace(periodTransfer.MVoucherID) && periodTransfer.MVoucherStatus == 1;
			bool flag2 = periodTransfer != null && !string.IsNullOrWhiteSpace(periodTransfer.MVoucherID);
			decimal num = flag2 ? periodTransfer.MPercent0 : (lastPeriodTransferModel?.MPercent0 ?? decimal.Zero);
			decimal num2 = flag2 ? periodTransfer.MPercent1 : (lastPeriodTransferModel?.MPercent1 ?? decimal.Zero);
			model.MTransferTypeName = dal.GetTransferTypeNameByID(ctx, model.MTransferTypeID);
			if (flag2)
			{
				model.MVoucherID = periodTransfer.MVoucherID;
				model.MVoucherNumber = periodTransfer.MVoucherNumber;
				model.MVoucherStatus = periodTransfer.MVoucherStatus;
			}
			decimal num8;
			BDAccountListModel toyearProfitAccount;
			List<string> revenueAccountList;
			List<string> expenseAccountList;
			switch (model.MTransferTypeID)
			{
			case 0:
			{
				BDAccountListModel accountByCode2 = BDAccountRepository.GetAccountByCode(accountListIncludeBalance, "6001");
				BDAccountListModel accountByCode3 = BDAccountRepository.GetAccountByCode(accountListIncludeBalance, "1405");
				GLBalanceModel balanceByAccountID = balanceBiz.GetBalanceByAccountID(ctx, accountByCode2.MItemID, model.MYear, model.MPeriod);
				GLBalanceModel balanceByAccountID2 = balanceBiz.GetBalanceByAccountID(ctx, accountByCode3.MItemID, model.MYear, model.MPeriod);
				decimal num5 = balanceByAccountID2.MBeginBalance + balanceByAccountID2.MDebit - balanceByAccountID2.MCredit;
				if (flag)
				{
					GLBalanceModel balanceModelByAccountID2 = GetBalanceModelByAccountID(ctx, periodTransfer, new List<string>
					{
						accountByCode3.MItemID
					}, accountListIncludeBalance);
					num5 += balanceModelByAccountID2.MCredit;
				}
				decimal num6 = balanceByAccountID.MCredit - balanceByAccountID.MDebit;
				decimal num7 = default(decimal);
				if (num6 == decimal.Zero)
				{
					model.MNotNeedCreateVoucher = true;
					model.MNeedEdit = false;
					num = default(decimal);
				}
				else
				{
					if (num == decimal.Zero || num * num6 / 100m > num5)
					{
						num = num5 / num6 * 100m;
						if (num > 100m)
						{
							num = 100m;
							num7 = num6;
						}
						else
						{
							num7 = num5;
						}
					}
					num = ((num < decimal.Zero) ? decimal.Zero : num);
					num = ((num == decimal.Zero) ? decimal.Zero : decimal.Parse(num.To2Decimal()));
					num7 = ((num7 != decimal.Zero) ? num7 : (num6 * num / 100m));
					model.MNeedEdit = true;
					model.MNotNeedCreateVoucher = false;
				}
				model.MAmount = num7;
				model.MNameValueModels = new List<NameValueModel>
				{
					new NameValueModel
					{
						MName = accountByCode2.MName,
						MValue = num6.ToString()
					},
					new NameValueModel
					{
						MName = lang.GetText(ctx, LangModule.Common, "CarryForwardPercent", "Carry forward percnet"),
						MValue = num.ToString()
					},
					new NameValueModel
					{
						MName = accountByCode3.MName + text,
						MValue = num5.ToString()
					}
				};
				break;
			}
			case 1:
			{
				BDAccountListModel leafAccountByCode = accountBiz.GetLeafAccountByCode(accountListIncludeBalance, "2211");
				GLBalanceModel balanceByAccountID = balanceBiz.GetBalanceByAccountID(ctx, leafAccountByCode.MItemID, model.MYear, model.MPeriod);
				if (flag)
				{
					GLBalanceModel balanceModelByAccountID3 = GetBalanceModelByAccountID(ctx, periodTransfer, new List<string>
					{
						leafAccountByCode.MItemID
					}, accountListIncludeBalance);
					balanceByAccountID.MDebit -= balanceModelByAccountID3.MDebit;
				}
				model.MAmount = balanceByAccountID.MDebit;
				List<NameValueModel> list = new List<NameValueModel>();
				NameValueModel obj = new NameValueModel
				{
					MName = leafAccountByCode.MName
				};
				num8 = model.MAmount;
				obj.MValue = num8.ToString();
				list.Add(obj);
				model.MNameValueModels = list;
				model.MNeedEdit = false;
				break;
			}
			case 2:
			{
				List<BDAccountListModel> leafAccountListByCode = accountBiz.GetLeafAccountListByCode(accountListIncludeBalance, "1602");
				BDAccountListModel bDAccountListModel = accountListIncludeBalance.FirstOrDefault((BDAccountListModel f) => f.MCode == "1602");
				List<string> accountIDs = (from f in leafAccountListByCode
				select f.MItemID).ToList();
				List<GLBalanceModel> balanceListByAccountIDs2 = balanceBiz.GetBalanceListByAccountIDs(ctx, accountIDs, dateTime2.Year, dateTime2.Month, true);
				List<GLBalanceModel> balanceListByAccountIDs3 = balanceBiz.GetBalanceListByAccountIDs(ctx, accountIDs, model.MYear, model.MPeriod, true);
				decimal num14 = default(decimal);
				decimal num15 = default(decimal);
				if (flag)
				{
					GLBalanceModel balanceModelByAccountID5 = GetBalanceModelByAccountID(ctx, periodTransfer, accountIDs, accountListIncludeBalance);
					num14 = balanceListByAccountIDs3.Sum((GLBalanceModel f) => f.MCredit) - balanceModelByAccountID5.MCredit;
				}
				num15 = balanceListByAccountIDs2.Sum((GLBalanceModel f) => f.MCredit);
				model.MAmount = ((num14 != decimal.Zero) ? num14 : num15);
				model.MNameValueModels = new List<NameValueModel>
				{
					new NameValueModel
					{
						MName = text3 + text10 + bDAccountListModel.MName,
						MValue = num14.ToString()
					},
					new NameValueModel
					{
						MName = text2 + text10 + bDAccountListModel.MName,
						MValue = num15.ToString()
					}
				};
				model.MNeedEdit = false;
				break;
			}
			case 3:
			{
				BDAccountListModel accountByCode8 = BDAccountRepository.GetAccountByCode(accountListIncludeBalance, "660207");
				BDAccountListModel accountByCode9 = BDAccountRepository.GetAccountByCode(accountListIncludeBalance, "1801");
				GLBalanceModel balanceByAccountID3 = balanceBiz.GetBalanceByAccountID(ctx, accountByCode9.MItemID, model.MYear, model.MPeriod);
				decimal num17 = balanceByAccountID3.MBeginBalance + balanceByAccountID3.MDebit - balanceByAccountID3.MCredit;
				if (flag)
				{
					GLBalanceModel balanceModelByAccountID6 = GetBalanceModelByAccountID(ctx, periodTransfer, new List<string>
					{
						accountByCode9.MItemID
					}, accountListIncludeBalance);
					num17 -= balanceModelByAccountID6.MCredit;
				}
				GLBalanceModel balanceByAccountID = balanceBiz.GetBalanceByAccountID(ctx, accountByCode8.MItemID, dateTime2.Year, dateTime2.Month);
				GLBalanceModel balanceByAccountID2 = balanceBiz.GetBalanceByAccountID(ctx, accountByCode9.MItemID, dateTime2.Year, dateTime2.Month);
				List<NameValueModel> obj4 = new List<NameValueModel>
				{
					new NameValueModel
					{
						MName = accountByCode9.MName,
						MValue = num17.ToString()
					}
				};
				NameValueModel obj5 = new NameValueModel
				{
					MName = text2 + text10 + accountByCode8.MName
				};
				num8 = balanceByAccountID.MDebit;
				obj5.MValue = num8.ToString();
				obj4.Add(obj5);
				model.MNameValueModels = obj4;
				model.MAmount = ((balanceByAccountID.MDebit > decimal.Zero && balanceByAccountID.MDebit <= num17) ? balanceByAccountID.MDebit : num17);
				model.MNeedEdit = false;
				break;
			}
			case 9:
			{
				BDAccountListModel exchangeLossAccount = BDAccountRepository.GetAccountByCode(accountListIncludeBalance, "660302");
				List<BDExchangeRateModel> monthlyExchangeRateList = rateBiz.GetMonthlyExchangeRateList(ctx, new DateTime(model.MYear, model.MPeriod, 1));
				model.MNameValueModels = new List<NameValueModel>
				{
					new NameValueModel
					{
						MName = text8,
						MValue = "0"
					}
				};
				if (monthlyExchangeRateList == null || monthlyExchangeRateList.Count == 0)
				{
					model.MNotNeedCreateVoucher = true;
					model.MNeedEdit = false;
				}
				else if ((from x in monthlyExchangeRateList
				where x.MRate * x.MUserRate == decimal.Zero
				select x).Count() > 0)
				{
					model.MErrorMessage = text9;
					model.MNeedEdit = true;
				}
				else
				{
					SqlWhere sqlWhere = new SqlWhere();
					sqlWhere.AddFilter("a.MIsCheckForCurrency", SqlOperators.Equal, "1");
					List<BDAccountListModel> accountListIncludeBalance2 = accountBiz.GetAccountListIncludeBalance(ctx, sqlWhere, false);
					accountListIncludeBalance2 = (from x in accountListIncludeBalance2
					where x.MAccountGroupID.Split(',').Contains("1") || x.MAccountGroupID.Split(',').Contains("2")
					select x).ToList();
					bool flag3 = false;
					if (accountListIncludeBalance2 != null && accountListIncludeBalance2.Count > 0)
					{
						List<GLBalanceModel> balanceList = balanceBiz.GetBalanceByPeriods(ctx, new List<DateTime>
						{
							new DateTime(model.MYear, model.MPeriod, 1)
						}, (from x in accountListIncludeBalance2
						select x.MItemID).ToList());
						decimal num4 = default(decimal);
						int i;
						for (i = 0; i < balanceList.Count; i++)
						{
							if ((!(balanceList[i].MCheckGroupValueID == "0") || (from x in balanceList
							where x.MCheckGroupValueID != "0" && x.MAccountID == balanceList[i].MAccountID
							select x).Count() <= 0) && balanceList[i].MCurrencyID != ctx.MBasCurrencyID)
							{
								if (!monthlyExchangeRateList.Any((BDExchangeRateModel x) => x.MTargetCurrencyID == balanceList[i].MCurrencyID))
								{
									model.MErrorMessage = text9;
									model.MNeedEdit = true;
									break;
								}
								bool flag4 = balanceList[i].MDC == 1;
								decimal mRate = monthlyExchangeRateList.FirstOrDefault((BDExchangeRateModel x) => x.MTargetCurrencyID == balanceList[i].MCurrencyID).MRate;
								decimal d = (balanceList[i].MDebitFor + (flag4 ? balanceList[i].MBeginBalanceFor : decimal.Zero)) * mRate - (balanceList[i].MDebit + (flag4 ? balanceList[i].MBeginBalance : decimal.Zero));
								decimal d2 = (balanceList[i].MCreditFor + ((!flag4) ? balanceList[i].MBeginBalanceFor : decimal.Zero)) * mRate - (balanceList[i].MCredit + ((!flag4) ? balanceList[i].MBeginBalance : decimal.Zero));
								d = Math.Round(d, 2, MidpointRounding.AwayFromZero);
								d2 = Math.Round(d2, 2, MidpointRounding.AwayFromZero);
								if (flag4)
								{
									if (d - d2 > decimal.Zero)
									{
										num4 -= d - d2;
										flag3 = true;
									}
									else if (d - d2 < decimal.Zero)
									{
										num4 += d2 - d;
										flag3 = true;
									}
								}
								else if (d2 - d > decimal.Zero)
								{
									num4 += d2 - d;
									flag3 = true;
								}
								else if (d2 - d < decimal.Zero)
								{
									num4 -= d - d2;
									flag3 = true;
								}
							}
						}
						if (flag)
						{
							GLVoucherModel voucherModel = voucher.GetVoucherModel(ctx, model.MVoucherID, false);
							if (voucherModel != null && voucherModel.MStatus == 1)
							{
								num4 += (from x in voucherModel.MVoucherEntrys
								where x.MAccountID == exchangeLossAccount.MItemID
								select x).ToList().Sum((GLVoucherEntryModel x) => x.MDebit - x.MCredit);
							}
						}
						model.MAmount = num4;
					}
					model.MNotNeedCreateVoucher = !flag3;
				}
				break;
			}
			case 4:
			{
				BDAccountListModel accountByCode2 = BDAccountRepository.GetAccountByCode(accountListIncludeBalance, "6001");
				BDAccountListModel accountByCode4 = BDAccountRepository.GetAccountByCode(accountListIncludeBalance, "6403");
				BDAccountListModel accountByCode5 = BDAccountRepository.GetAccountByCode(accountListIncludeBalance, "222101");
				BDAccountListModel accountByCode6 = BDAccountRepository.GetAccountByCode(accountListIncludeBalance, "222108");
				BDAccountListModel accountByCode7 = BDAccountRepository.GetAccountByCode(accountListIncludeBalance, "222113");
				GLBalanceModel balanceByAccountID = balanceBiz.GetBalanceByAccountID(ctx, accountByCode2.MItemID, model.MYear, model.MPeriod);
				GLBalanceModel balanceByAccountID2 = balanceBiz.GetBalanceByAccountID(ctx, accountByCode4.MItemID, model.MYear, model.MPeriod);
				decimal num16 = balanceByAccountID.MDebit + balanceByAccountID2.MDebit;
				GLPeriodTransferModel lastTaxModel = GetLastTaxModel(ctx, model);
				List<NameValueModel> list5 = new List<NameValueModel>();
				NameValueModel obj3 = new NameValueModel
				{
					MName = accountByCode5.MName
				};
				num8 = balanceByAccountID2.MDebit + balanceByAccountID2.MDebit;
				obj3.MValue = num8.ToString();
				list5.Add(obj3);
				model.MNameValueModels = list5;
				if (lastTaxModel != null)
				{
					GLVoucherModel gLVoucherModel = voucher.GetVoucherModelList(ctx, new List<string>
					{
						lastTaxModel.MVoucherID
					}, false, 0, 0).FirstOrDefault();
					List<GLVoucherEntryModel> mVoucherEntrys = gLVoucherModel.MVoucherEntrys;
					for (int k = 0; k < mVoucherEntrys.Count; k++)
					{
						decimal d5 = mVoucherEntrys[k].MAmount / gLVoucherModel.MCreditTotal;
						model.MNameValueModels.Add(new NameValueModel
						{
							MName = accountBiz.GetDataModel(ctx, mVoucherEntrys[k].MAccountID, false).MName,
							MValue = d5.ToString()
						});
						model.MAmount += d5 * num16;
					}
				}
				else
				{
					num = ((num <= decimal.Zero) ? 7m : num);
					num2 = ((num2 <= decimal.Zero) ? 3m : num2);
					model.MAmount = num16 * (num + num2) * 0.01m;
					model.MNameValueModels.AddRange(new List<NameValueModel>
					{
						new NameValueModel
						{
							MName = accountByCode6.MName,
							MValue = num.ToString()
						},
						new NameValueModel
						{
							MName = accountByCode7.MName,
							MValue = num2.ToString()
						}
					});
				}
				model.MNeedEdit = true;
				break;
			}
			case 5:
			{
				BDAccountListModel accountByCode5 = BDAccountRepository.GetAccountByCode(accountListIncludeBalance, "222101");
				BDAccountListModel leafAccountByCode2 = accountBiz.GetLeafAccountByCode(accountListIncludeBalance, "222102");
				GLBalanceModel balanceByAccountID = balanceBiz.GetBalanceByAccountID(ctx, accountByCode5.MItemID, model.MYear, model.MPeriod);
				model.MAmount = balanceByAccountID.MBeginBalance + balanceByAccountID.MCredit - balanceByAccountID.MDebit;
				if (flag)
				{
					GLBalanceModel balanceModelByAccountID7 = GetBalanceModelByAccountID(ctx, periodTransfer, new List<string>
					{
						leafAccountByCode2.MItemID
					}, accountListIncludeBalance);
					model.MAmount -= balanceModelByAccountID7.MCredit;
				}
				List<NameValueModel> list6 = new List<NameValueModel>();
				NameValueModel obj6 = new NameValueModel
				{
					MName = text3 + text10 + accountByCode5.MName
				};
				num8 = model.MAmount;
				obj6.MValue = num8.ToString();
				list6.Add(obj6);
				model.MNameValueModels = list6;
				model.MNotNeedCreateVoucher = (model.MAmount <= decimal.Zero);
				model.MNeedEdit = false;
				break;
			}
			case 6:
			{
				AccountTypeEnum accountTypeEnum = new AccountTypeEnum(ctx.MAccountTableID);
				toyearProfitAccount = BDAccountRepository.GetAccountByCode(accountListIncludeBalance, "4103");
				revenueAccountList = BDAccountRepository.GetItemListByType(accountListIncludeBalance, new List<string>
				{
					accountTypeEnum.OperatingRevenue,
					accountTypeEnum.OtherRevenue
				}, false, true);
				expenseAccountList = BDAccountRepository.GetItemListByType(accountListIncludeBalance, new List<string>
				{
					accountTypeEnum.OperatingCostsAndTaxes,
					accountTypeEnum.OtherLoss,
					accountTypeEnum.PeriodCharge
				}, false, true);
				BDAccountListModel payableIncomeTaxAccount = BDAccountRepository.GetAccountByCode(accountListIncludeBalance, "222106");
				List<GLInitBalanceModel> source = new List<GLInitBalanceModel>();
				dateTime = ctx.MGLBeginDate;
				int num9;
				if (dateTime.Year == model.MYear)
				{
					dateTime = ctx.MGLBeginDate;
					num9 = ((dateTime.Month > 1) ? 1 : 0);
				}
				else
				{
					num9 = 0;
				}
				if (num9 != 0)
				{
					source = initBalanceBiz.GetInitBalanceList(ctx, new GLInitBalanceListFilterModel
					{
						IncludeCheckTypeData = false
					});
				}
				List<GLBalanceModel> periodBalanceListByAccountIDs = balanceBiz.GetPeriodBalanceListByAccountIDs(ctx, revenueAccountList, model.MYear * 12 + 1, model.MYear * 12 + model.MPeriod);
				List<GLInitBalanceModel> source2 = (from x in source
				where revenueAccountList.Contains(x.MAccountID)
				select x).ToList();
				decimal d3 = periodBalanceListByAccountIDs.Sum((GLBalanceModel x) => x.MCredit - x.MDebit) + source2.Sum((GLInitBalanceModel x) => x.MYtdCredit);
				List<GLBalanceModel> periodBalanceListByAccountIDs2 = balanceBiz.GetPeriodBalanceListByAccountIDs(ctx, expenseAccountList, model.MYear * 12 + 1, model.MYear * 12 + model.MPeriod);
				List<GLInitBalanceModel> source3 = (from x in source
				where expenseAccountList.Contains(x.MAccountID)
				select x).ToList();
				decimal num10 = periodBalanceListByAccountIDs2.Sum((GLBalanceModel x) => x.MDebit - x.MCredit) + source3.Sum((GLInitBalanceModel x) => x.MYtdDebit);
				List<GLPeriodTransferModel> modelByPeriodAndType = GetModelByPeriodAndType(ctx, model.MYear * 12 + 1, model.MYear * 12 + model.MPeriod, 7, 1);
				for (int j = 0; j < modelByPeriodAndType.Count; j++)
				{
					GLVoucherModel voucherModel2 = voucher.GetVoucherModel(ctx, modelByPeriodAndType[j].MVoucherID, false);
					List<GLVoucherEntryModel> list2 = (from x in voucherModel2.MVoucherEntrys
					where x.MAccountID != toyearProfitAccount.MItemID
					select x).ToList();
					List<GLVoucherEntryModel> list3 = (from x in voucherModel2.MVoucherEntrys
					where revenueAccountList.Contains(x.MAccountID)
					select x).ToList();
					List<GLVoucherEntryModel> list4 = (from x in voucherModel2.MVoucherEntrys
					where expenseAccountList.Contains(x.MAccountID)
					select x).ToList();
					d3 += ((list3.Count > 0) ? list3.Sum((GLVoucherEntryModel x) => x.MDebit - x.MCredit) : decimal.Zero);
					num10 += ((list4.Count > 0) ? list4.Sum((GLVoucherEntryModel x) => x.MCredit - x.MDebit) : decimal.Zero);
				}
				decimal d4 = d3 - num10;
				num = ((num <= decimal.Zero) ? 25m : num);
				decimal num11 = num * d4 / 100m;
				num11 = ((num11 < decimal.Zero) ? decimal.Zero : num11);
				decimal num12 = balanceBiz.GetPeriodBalanceListByAccountIDs(ctx, new List<string>
				{
					payableIncomeTaxAccount.MItemID
				}, model.MYear * 12 + 1, model.MYear * 12 + model.MPeriod).Sum((GLBalanceModel x) => x.MCredit);
				num12 += (from x in source
				where x.MAccountID == payableIncomeTaxAccount.MItemID
				select x).Sum((GLInitBalanceModel x) => x.MYtdCredit);
				if (flag)
				{
					GLBalanceModel balanceModelByAccountID4 = GetBalanceModelByAccountID(ctx, periodTransfer, new List<string>
					{
						payableIncomeTaxAccount.MItemID
					}, accountListIncludeBalance);
					num12 -= balanceModelByAccountID4.MCredit;
				}
				decimal num7 = num11 - num12;
				decimal obj2 = (num7 < decimal.Zero) ? decimal.Zero : num7;
				num7 = (model.MAmount = obj2);
				model.MNotNeedCreateVoucher = (num7 == decimal.Zero);
				model.MNameValueModels = new List<NameValueModel>
				{
					new NameValueModel
					{
						MName = text7 + text10 + toyearProfitAccount.MName,
						MValue = d4.ToString()
					},
					new NameValueModel
					{
						MName = text4 + text10 + text5,
						MValue = num.ToString()
					},
					new NameValueModel
					{
						MName = text7 + text10 + payableIncomeTaxAccount.MName,
						MValue = num12.ToString()
					}
				};
				model.MNeedEdit = true;
				break;
			}
			case 7:
			{
				AccountTypeEnum accountTypeEnum = new AccountTypeEnum(ctx.MAccountTableID);
				revenueAccountList = BDAccountRepository.GetItemListByType(accountListIncludeBalance, new List<string>
				{
					accountTypeEnum.OperatingRevenue,
					accountTypeEnum.OtherRevenue
				}, false, true);
				expenseAccountList = BDAccountRepository.GetItemListByType(accountListIncludeBalance, new List<string>
				{
					accountTypeEnum.OperatingCostsAndTaxes,
					accountTypeEnum.OtherLoss,
					accountTypeEnum.PeriodCharge,
					accountTypeEnum.IncomeTax
				}, false, true);
				List<GLBalanceModel> balanceListByAccountIDs4 = balanceBiz.GetBalanceListByAccountIDs(ctx, revenueAccountList, model.MYear, model.MPeriod, true);
				decimal d6 = balanceListByAccountIDs4.Sum((GLBalanceModel x) => x.MCredit - x.MDebit);
				if (flag)
				{
					GLBalanceModel balanceModelByAccountID8 = GetBalanceModelByAccountID(ctx, periodTransfer, revenueAccountList, accountListIncludeBalance);
					d6 -= balanceModelByAccountID8.MCredit - balanceModelByAccountID8.MDebit;
				}
				List<GLBalanceModel> balanceListByAccountIDs5 = balanceBiz.GetBalanceListByAccountIDs(ctx, expenseAccountList, model.MYear, model.MPeriod, true);
				decimal num18 = balanceListByAccountIDs5.Sum((GLBalanceModel x) => x.MDebit - x.MCredit);
				if (flag)
				{
					GLBalanceModel balanceModelByAccountID9 = GetBalanceModelByAccountID(ctx, periodTransfer, expenseAccountList, accountListIncludeBalance);
					num18 -= balanceModelByAccountID9.MDebit - balanceModelByAccountID9.MCredit;
				}
				model.MAmount = d6 - num18;
				List<NameValueModel> list7 = new List<NameValueModel>();
				List<NameValueModel> list8 = list7;
				NameValueModel nameValueModel = new NameValueModel
				{
					MName = text7 + text10 + text3 + text10 + text5
				};
				NameValueModel nameValueModel2 = nameValueModel;
				num8 = balanceListByAccountIDs4.Sum((GLBalanceModel x) => x.MCredit - x.MDebit);
				nameValueModel2.MValue = num8.ToString();
				list8.Add(nameValueModel);
				List<NameValueModel> list9 = list7;
				nameValueModel = new NameValueModel
				{
					MName = text7 + text10 + text3 + text10 + text6
				};
				NameValueModel nameValueModel3 = nameValueModel;
				num8 = balanceListByAccountIDs5.Sum((GLBalanceModel x) => x.MDebit - x.MCredit);
				nameValueModel3.MValue = num8.ToString();
				list9.Add(nameValueModel);
				model.MNameValueModels = list7;
				model.MNeedEdit = false;
				break;
			}
			case 8:
			{
				toyearProfitAccount = BDAccountRepository.GetAccountByCode(accountListIncludeBalance, "4103");
				BDAccountListModel accountByCode = BDAccountRepository.GetAccountByCode(accountListIncludeBalance, "410407");
				decimal num3 = default(decimal);
				List<GLBalanceModel> balanceListByAccountIDs = balanceBiz.GetBalanceListByAccountIDs(ctx, new List<string>
				{
					toyearProfitAccount.MItemID
				}, model.MYear, null, true);
				balanceListByAccountIDs = (from x in balanceListByAccountIDs
				orderby x.MPeriod
				select x).ToList();
				if (balanceListByAccountIDs != null)
				{
					num3 += balanceListByAccountIDs.Sum((GLBalanceModel x) => x.MCredit - x.MDebit);
					num3 += balanceListByAccountIDs[0].MBeginBalance;
				}
				if (flag)
				{
					GLBalanceModel balanceModelByAccountID = GetBalanceModelByAccountID(ctx, periodTransfer, new List<string>
					{
						toyearProfitAccount.MItemID
					}, accountListIncludeBalance);
					num3 -= balanceModelByAccountID.MCredit - balanceModelByAccountID.MDebit;
				}
				model.MAmount = num3;
				model.MNameValueModels = new List<NameValueModel>
				{
					new NameValueModel
					{
						MName = text7 + text10 + toyearProfitAccount.MName,
						MValue = num3.ToString()
					},
					new NameValueModel
					{
						MName = accountByCode.MName,
						MValue = num3.ToString()
					}
				};
				model.MNeedEdit = false;
				break;
			}
			}
			model.IsCalculateMatch = (periodTransfer == null || !(periodTransfer.MAmount != model.MAmount));
			return model;
		}

		public GLBalanceModel GetBalanceModelByAccountID(MContext ctx, GLPeriodTransferModel model, List<string> accountIDs, List<BDAccountListModel> accounts)
		{
			if (model == null || model.MVoucherStatus != 1 || accountIDs == null || accountIDs.Count == 0)
			{
				return new GLBalanceModel();
			}
			List<GLVoucherModel> voucherModelList = new GLVoucherBusiness().GetVoucherModelList(ctx, new List<string>
			{
				model.MVoucherID
			}, false, 0, 0);
			List<GLVoucherEntryModel> entrys = new List<GLVoucherEntryModel>();
			voucherModelList.ForEach(delegate(GLVoucherModel x)
			{
				entrys.AddRange((from y in x.MVoucherEntrys
				where BDAccountRepository.IsAccountContainsSub(accounts, accountIDs, y.MAccountID)
				select y).ToList());
			});
			return new GLBalanceModel
			{
				MDebit = entrys.Sum((GLVoucherEntryModel x) => x.MDebit),
				MDebitFor = entrys.Sum((GLVoucherEntryModel x) => x.MDebit),
				MCredit = entrys.Sum((GLVoucherEntryModel x) => x.MCredit),
				MCreditFor = entrys.Sum((GLVoucherEntryModel x) => x.MCredit)
			};
		}

		public GLBalanceModel GetBalanceSumFromPeriodTransferByAccounts(MContext ctx, int fromYear, int fromMonth, int toYear, int toMonth, string accountId, int typeID = 7)
		{
			List<GLBalanceModel> balanceListFromPeriodTransferByAccounts = GetBalanceListFromPeriodTransferByAccounts(ctx, fromYear, fromMonth, toYear, toMonth, new List<string>
			{
				accountId
			}, typeID, 1);
			if (balanceListFromPeriodTransferByAccounts == null || balanceListFromPeriodTransferByAccounts.Count == 0)
			{
				return new GLBalanceModel();
			}
			return new GLBalanceModel
			{
				MAccountID = accountId,
				MCredit = balanceListFromPeriodTransferByAccounts.Sum((GLBalanceModel t) => t.MCredit),
				MCreditFor = balanceListFromPeriodTransferByAccounts.Sum((GLBalanceModel t) => t.MCreditFor),
				MDebit = balanceListFromPeriodTransferByAccounts.Sum((GLBalanceModel t) => t.MDebit),
				MDebitFor = balanceListFromPeriodTransferByAccounts.Sum((GLBalanceModel t) => t.MDebitFor)
			};
		}

		public List<GLBalanceModel> GetBalanceListFromPeriodTransferByAccounts(MContext ctx, int fromYear, int fromMonth, int toYear, int toMonth, List<string> accountIDs, int typeID = 7, int status = 1)
		{
			int startYearPeriod = fromYear * 12 + fromMonth;
			int endYearPeriod = toYear * 12 + toMonth;
			List<GLBalanceModel> list = new List<GLBalanceModel>();
			int i;
			for (i = 0; i < accountIDs.Count; i++)
			{
				GLBalanceModel balance = new GLBalanceModel
				{
					MAccountID = accountIDs[i]
				};
				List<GLPeriodTransferModel> modelByPeriodAndType = GetModelByPeriodAndType(ctx, startYearPeriod, endYearPeriod, typeID, status);
				List<string> pkIDS = (from x in modelByPeriodAndType
				select x.MVoucherID).ToList();
				List<GLVoucherModel> voucherModelList = new GLVoucherBusiness().GetVoucherModelList(ctx, pkIDS, false, 0, 0);
				List<GLVoucherEntryModel> list2 = new List<GLVoucherEntryModel>();
				voucherModelList.ForEach(delegate(GLVoucherModel x)
				{
					x.MVoucherEntrys.ForEach(delegate(GLVoucherEntryModel y)
					{
						if (y.MAccountID == accountIDs[i])
						{
							GLBalanceModel gLBalanceModel = balance;
							gLBalanceModel.MDebit += y.MDebit;
							GLBalanceModel gLBalanceModel2 = balance;
							gLBalanceModel2.MCredit += y.MCredit;
						}
					});
				});
				list.Add(balance);
			}
			return list;
		}

		public List<GLBalanceModel> GLBalanceListFromPeriodTransfer(MContext ctx, int fromYear, int fromMonth, int toYear, int toMonth, int typeID = 7, int status = 1)
		{
			int startYearPeriod = fromYear * 12 + fromMonth;
			int endYearPeriod = toYear * 12 + toMonth;
			List<GLPeriodTransferModel> modelByPeriodAndType = GetModelByPeriodAndType(ctx, startYearPeriod, endYearPeriod, typeID, status);
			List<string> list = (from x in modelByPeriodAndType
			select x.MVoucherID).ToList();
			List<GLVoucherModel> list2 = new List<GLVoucherModel>();
			if (list != null && list.Count > 0)
			{
				list2 = new GLVoucherBusiness().GetVoucherModelList(ctx, list, false, 0, 0);
			}
			List<GLVoucherEntryModel> entryList = new List<GLVoucherEntryModel>();
			list2.ForEach(delegate(GLVoucherModel x)
			{
				entryList.AddRange(x.MVoucherEntrys);
			});
			List<string> accountIDS = (from x in entryList
			select x.MAccountID).Distinct().ToList();
			List<GLBalanceModel> list3 = new List<GLBalanceModel>();
			int i;
			for (i = 0; i < accountIDS.Count; i++)
			{
				GLBalanceModel gLBalanceModel = new GLBalanceModel
				{
					MAccountID = accountIDS[i],
					MAccountCode = (from x in entryList
					where x.MAccountID == accountIDS[i]
					select x).FirstOrDefault().MAccountCode
				};
				GLBalanceModel gLBalanceModel2 = gLBalanceModel;
				gLBalanceModel2.MDebit += (from x in entryList
				where x.MAccountID == accountIDS[i]
				select x).Sum((GLVoucherEntryModel x) => x.MDebit);
				GLBalanceModel gLBalanceModel3 = gLBalanceModel;
				gLBalanceModel3.MCredit += (from x in entryList
				where x.MAccountID == accountIDS[i]
				select x).Sum((GLVoucherEntryModel x) => x.MCredit);
				list3.Add(gLBalanceModel);
			}
			return list3;
		}

		public List<GLVoucherModel> GetTransferVoucherList(MContext ctx, int fromYear, int fromMonth, int toYear, int toMonth, int typeID = 7, int status = 1)
		{
			int startYearPeriod = fromYear * 12 + fromMonth;
			int endYearPeriod = toYear * 12 + toMonth;
			List<GLPeriodTransferModel> modelByPeriodAndType = GetModelByPeriodAndType(ctx, startYearPeriod, endYearPeriod, typeID, status);
			List<string> list = (from x in modelByPeriodAndType
			select x.MVoucherID).ToList();
			List<GLVoucherModel> result = new List<GLVoucherModel>();
			if (list != null && list.Count > 0)
			{
				result = new GLVoucherBusiness().GetVoucherModelList(ctx, list, false, 0, 0);
			}
			return result;
		}

		private List<GLPeriodTransferModel> GetModelByPeriodAndType(MContext ctx, int startYearPeriod, int endYearPeriod, int typeID, int status)
		{
			return dal.GetModelByPeriodAndType(ctx, startYearPeriod, endYearPeriod, typeID, status);
		}

		public GLPeriodTransferModel GetLastTaxModel(MContext ctx, GLPeriodTransferModel model)
		{
			GLPeriodTransferModel dataModelByFilter = GetDataModelByFilter(ctx, new SqlWhere().AddFilter("MOrgID", SqlOperators.Equal, ctx.MOrgID).AddFilter("MYear", SqlOperators.Equal, model.MYear).AddFilter("MPeriod", SqlOperators.Equal, model.MPeriod)
				.AddFilter("MTransferTypeID", SqlOperators.Equal, model.MTransferTypeID));
			if (dataModelByFilter != null)
			{
				return dataModelByFilter;
			}
			GLPeriodTransferModel lastPeriodTransferModel = GetLastPeriodTransferModel(ctx, model);
			if (dataModelByFilter != null)
			{
				return dataModelByFilter;
			}
			return null;
		}

		private void InsertModelFromVoucherList(MContext ctx, List<GLVoucherModel> modelList, ref GLPeriodTransferModel model)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			int num = 0;
			while (modelList != null && num < modelList.Count)
			{
				list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<GLPeriodTransferModel>(ctx, new GLPeriodTransferModel
				{
					MYear = model.MYear,
					MPeriod = model.MPeriod,
					MTransferTypeID = model.MTransferTypeID,
					MVoucherID = modelList[num].MItemID,
					MAmount = modelList[num].MDebitTotal,
					MOrgID = ctx.MOrgID,
					MPercent0 = modelList[num].Percent0,
					MPercent1 = modelList[num].Percent1,
					MPercent2 = modelList[num].Percent2
				}, null, true));
				model.MVoucherNumber = modelList[num].MNumber;
				model.MVoucherStatus = modelList[num].MStatus;
				GLPeriodTransferModel obj = model;
				obj.MAmount += modelList[num].MDebitTotal;
				if (modelList[num].MTransferTypeID != model.MTransferTypeID)
				{
					list.AddRange(GLVoucherRepository.GetInsertVoucherCmds(ctx, modelList[num], new List<string>
					{
						"MType"
					}));
				}
				num++;
			}
			new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(list);
		}

		public GLPeriodTransferModel GetPeriodTransfer(MContext ctx, GLPeriodTransferModel model, bool test = true)
		{
			model.MOrgID = (model.MOrgID ?? ctx.MOrgID);
			GLPeriodTransferModel gLPeriodTransferModel = model;
			List<GLPeriodTransferModel> modelByFilter = dal.GetModelByFilter(ctx, model.MYear, model.MPeriod, model.MTransferTypeID, null);
			if (modelByFilter != null && modelByFilter.Count > 0)
			{
				gLPeriodTransferModel = modelByFilter[0];
				GLVoucherModel voucherModel = voucher.GetVoucherModel(ctx, gLPeriodTransferModel.MVoucherID, false);
				gLPeriodTransferModel.MVoucherNumber = voucherModel.MNumber;
				gLPeriodTransferModel.MAmount = voucherModel.MDebitTotal;
				BDAccountListModel account;
				switch (model.MTransferTypeID)
				{
				case 7:
				{
					List<BDAccountListModel> accountListIncludeBalance = accountBiz.GetAccountListIncludeBalance(ctx, new SqlWhere(), true);
					account = accountBiz.GetLeafAccountByCode(accountListIncludeBalance, "4103");
					gLPeriodTransferModel.MAmount = (from x in voucherModel.MVoucherEntrys
					where x.MAccountID == account.MItemID
					select x).Sum((GLVoucherEntryModel x) => x.MCredit - x.MDebit);
					break;
				}
				case 8:
				{
					List<BDAccountListModel> accountListIncludeBalance = accountBiz.GetAccountListIncludeBalance(ctx, new SqlWhere(), true);
					account = accountBiz.GetLeafAccountByCode(accountListIncludeBalance, "4103");
					gLPeriodTransferModel.MAmount = (from x in voucherModel.MVoucherEntrys
					where x.MAccountID == account.MItemID
					select x).Sum((GLVoucherEntryModel x) => x.MDebit - x.MCredit);
					break;
				}
				case 9:
				{
					List<BDAccountListModel> accountListIncludeBalance = accountBiz.GetAccountListIncludeBalance(ctx, new SqlWhere(), true);
					account = accountBiz.GetLeafAccountByCode(accountListIncludeBalance, "660302");
					gLPeriodTransferModel.MAmount = (from x in voucherModel.MVoucherEntrys
					where x.MAccountID == account.MItemID
					select x).Sum((GLVoucherEntryModel x) => x.MDebit - x.MCredit);
					break;
				}
				}
				gLPeriodTransferModel.MVoucherStatus = voucherModel.MStatus;
			}
			gLPeriodTransferModel.MTransferTypeName = dal.GetTransferTypeNameByID(ctx, model.MTransferTypeID);
			if (!(string.IsNullOrWhiteSpace(gLPeriodTransferModel.MVoucherID) & test) && false)
			{
				List<GLVoucherModel> list = TestMatchedPeriodTransferVoucher(ctx, ref gLPeriodTransferModel);
			}
			return gLPeriodTransferModel;
		}

		public List<GLVoucherModel> TestMatchedPeriodTransferVoucher(MContext ctx, ref GLPeriodTransferModel model)
		{
			List<BDAccountListModel> bDAccountList = accountBiz.GetBDAccountList(ctx, "");
			List<GLVoucherModel> list = new List<GLVoucherModel>();
			switch (model.MTransferTypeID)
			{
			case 0:
			{
				BDAccountListModel accountByCode10 = BDAccountRepository.GetAccountByCode(bDAccountList, "6401");
				BDAccountListModel accountByCode11 = BDAccountRepository.GetAccountByCode(bDAccountList, "1405");
				list = voucher.GetVoucherByCodes(ctx, model.MYear, model.MPeriod, new List<string>
				{
					accountByCode10.MCode,
					accountByCode11.MCode
				}, true);
				if (list != null && list.Count > 0)
				{
					GLBalanceModel balance = balanceBiz.GetBalanceByAccountID(ctx, accountByCode10.MItemID, model.MYear, model.MPeriod);
					list.ForEach(delegate(GLVoucherModel x)
					{
						x.Percent0 = ((balance.MCredit == decimal.Zero) ? 100m : (x.MCreditTotal / balance.MCredit));
					});
				}
				break;
			}
			case 2:
			{
				BDAccountListModel accountByCode14 = BDAccountRepository.GetAccountByCode(bDAccountList, "1602");
				BDAccountListModel accountByCode15 = BDAccountRepository.GetAccountByCode(bDAccountList, "660205");
				list = voucher.GetVoucherByCodes(ctx, model.MYear, model.MPeriod, new List<string>
				{
					accountByCode15.MCode,
					accountByCode14.MCode
				}, true);
				break;
			}
			case 3:
			{
				BDAccountListModel accountByCode8 = BDAccountRepository.GetAccountByCode(bDAccountList, "660207");
				BDAccountListModel accountByCode9 = BDAccountRepository.GetAccountByCode(bDAccountList, "1801");
				list = voucher.GetVoucherByCodes(ctx, model.MYear, model.MPeriod, new List<string>
				{
					accountByCode9.MCode,
					accountByCode8.MCode
				}, true);
				break;
			}
			case 4:
			{
				BDAccountListModel accountByCode12 = BDAccountRepository.GetAccountByCode(bDAccountList, "222101");
				BDAccountListModel accountByCode5 = BDAccountRepository.GetAccountByCode(bDAccountList, "222102");
				BDAccountListModel accountByCode13 = BDAccountRepository.GetAccountByCode(bDAccountList, "6403");
				BDAccountListModel cityMaintenanceAndConstructionTaxAccount = BDAccountRepository.GetAccountByCode(bDAccountList, "222108");
				BDAccountListModel educationFundsAdditionalAccount = BDAccountRepository.GetAccountByCode(bDAccountList, "222113");
				GLPeriodTransferModel lastPeriodTransferModel = GetLastPeriodTransferModel(ctx, model);
				if (lastPeriodTransferModel != null)
				{
					List<string> codeList = new List<string>();
					GLVoucherModel gLVoucherModel = voucher.GetVoucherModelList(ctx, new List<string>
					{
						lastPeriodTransferModel.MVoucherID
					}, false, 0, 0).FirstOrDefault();
					gLVoucherModel.MVoucherEntrys.ForEach(delegate(GLVoucherEntryModel x)
					{
						codeList.Add(x.MAccountCode);
					});
					list = voucher.GetVoucherByCodes(ctx, model.MYear, model.MPeriod, codeList, true);
				}
				else
				{
					list = voucher.GetVoucherByCodes(ctx, model.MYear, model.MPeriod, new List<string>
					{
						accountByCode13.MCode,
						cityMaintenanceAndConstructionTaxAccount.MCode,
						educationFundsAdditionalAccount.MCode
					}, true);
					if (list != null && list.Count > 0)
					{
						DateTime dateTime = new DateTime(model.MYear, model.MPeriod, 1).AddMonths(-1);
						GLVoucherEntryModel sumEntryByAccountCodeOrType = entry.GetSumEntryByAccountCodeOrType(ctx, dateTime.Year, dateTime.Month, new List<string>
						{
							accountByCode5.MCode
						}, null);
						GLVoucherEntryModel sumEntryByAccountCodeOrType2 = entry.GetSumEntryByAccountCodeOrType(ctx, model.MYear, model.MPeriod, new List<string>
						{
							accountByCode12.MCode
						}, null);
						list.ForEach(delegate(GLVoucherModel x)
						{
							x.Percent0 = x.MVoucherEntrys.Find((GLVoucherEntryModel y) => y.MAccountCode == cityMaintenanceAndConstructionTaxAccount.MCode).MCredit / x.MVoucherEntrys.Find((GLVoucherEntryModel y) => y.MAccountCode == "6403").MDebit;
							x.Percent1 = x.MVoucherEntrys.Find((GLVoucherEntryModel y) => y.MAccountCode == educationFundsAdditionalAccount.MCode).MCredit / x.MVoucherEntrys.Find((GLVoucherEntryModel y) => y.MAccountCode == "6403").MDebit;
						});
					}
				}
				break;
			}
			case 5:
			{
				BDAccountListModel accountByCode5 = BDAccountRepository.GetAccountByCode(bDAccountList, "222102");
				BDAccountListModel accountByCode6 = BDAccountRepository.GetAccountByCode(bDAccountList, "22210109");
				BDAccountListModel accountByCode7 = BDAccountRepository.GetAccountByCode(bDAccountList, "22210103");
				list = voucher.GetVoucherByCodes(ctx, model.MYear, model.MPeriod, new List<string>
				{
					accountByCode5.MCode,
					accountByCode6.MCode
				}, true);
				if (list == null || list.Count == 0)
				{
					list = voucher.GetVoucherByCodes(ctx, model.MYear, model.MPeriod, new List<string>
					{
						accountByCode5.MCode,
						accountByCode6.MCode
					}, true);
					list.AddRange(voucher.GetVoucherByCodes(ctx, model.MYear, model.MPeriod, new List<string>
					{
						accountByCode7.MCode,
						accountByCode5.MCode
					}, true));
				}
				break;
			}
			case 6:
			{
				BDAccountListModel accountByCode3 = BDAccountRepository.GetAccountByCode(bDAccountList, "6801");
				BDAccountListModel accountByCode4 = BDAccountRepository.GetAccountByCode(bDAccountList, "222106");
				list = voucher.GetVoucherByCodes(ctx, model.MYear, model.MPeriod, new List<string>
				{
					accountByCode3.MCode,
					accountByCode4.MCode
				}, true);
				break;
			}
			case 8:
			{
				BDAccountListModel accountByCode = BDAccountRepository.GetAccountByCode(bDAccountList, "4103");
				BDAccountListModel accountByCode2 = BDAccountRepository.GetAccountByCode(bDAccountList, "410407");
				list = voucher.GetVoucherByCodes(ctx, model.MYear, model.MPeriod, new List<string>
				{
					accountByCode.MCode,
					accountByCode2.MCode
				}, true);
				break;
			}
			}
			if (list != null && list.Count > 0)
			{
				InsertModelFromVoucherList(ctx, list, ref model);
			}
			return list;
		}

		public GLVoucherModel CreatePeriodTransfer(MContext ctx, GLPeriodTransferModel model)
		{
			GLPeriodTransferModel dataModelByFilter = GetDataModelByFilter(ctx, new SqlWhere().AddFilter("MYear", SqlOperators.Equal, model.MYear).AddFilter("MPeriod", SqlOperators.Equal, model.MPeriod).AddFilter("MTransferTypeID", SqlOperators.Equal, model.MTransferTypeID));
			return null;
		}

		public GLPeriodTransferModel GetLastPeriodTransferModel(MContext ctx, GLPeriodTransferModel model)
		{
			DateTime dateTime = new DateTime(model.MYear.ToMInt32(), model.MPeriod.ToMInt32(), 1).AddMonths(-1);
			List<GLPeriodTransferModel> modelList = GetModelList(ctx, new SqlWhere().AddFilter("MOrgID", SqlOperators.Equal, ctx.MOrgID).AddFilter("MYear", SqlOperators.Equal, dateTime.Year).AddFilter("MPeriod", SqlOperators.Equal, dateTime.Month)
				.AddFilter("MTransferTypeID", SqlOperators.Equal, model.MTransferTypeID), false);
			return (modelList == null || modelList.Count == 0) ? null : modelList[0];
		}

		public GLPeriodTransferModel GetPeriodTransferModel(MContext ctx, GLPeriodTransferModel model)
		{
			List<GLPeriodTransferModel> modelList = GetModelList(ctx, new SqlWhere().AddFilter("MOrgID", SqlOperators.Equal, ctx.MOrgID).AddFilter("MYear", SqlOperators.Equal, model.MYear).AddFilter("MPeriod", SqlOperators.Equal, model.MPeriod)
				.AddFilter("MTransferTypeID", SqlOperators.Equal, model.MTransferTypeID), false);
			return (modelList == null || modelList.Count == 0) ? null : modelList[0];
		}

		private List<DateTime> GetPeriodList(int year, int period, int type)
		{
			List<DateTime> list = new List<DateTime>();
			switch (type)
			{
			case 0:
				for (int j = 1; j <= 12; j++)
				{
					list.Add(new DateTime(year, j, 1));
				}
				break;
			case 1:
				for (int k = period - (period - 1) % 3; k <= period; k++)
				{
					list.Add(new DateTime(year, k, 1));
				}
				break;
			case 2:
				list.Add(new DateTime(year, period, 1));
				break;
			case 3:
				for (int i = 1; i <= period; i++)
				{
					list.Add(new DateTime(year, i, 1));
				}
				break;
			}
			return list;
		}

		public DateTime GetLastTransferPeriod(MContext ctx, int tranferType)
		{
			DateTime result = default(DateTime);
			int lastTranferPeriod = dal.GetLastTranferPeriod(ctx, tranferType);
			if (lastTranferPeriod == 0)
			{
				return result;
			}
			return DateTime.ParseExact(lastTranferPeriod + "01", "yyyyMMdd", CultureInfo.InvariantCulture);
		}

		public bool Exists(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.Exists(ctx, pkID, includeDelete);
		}

		public bool ExistsByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.ExistsByFilter(ctx, filter);
		}

		public OperationResult InsertOrUpdate(MContext ctx, GLPeriodTransferModel modelData, string fields = null)
		{
			return dal.InsertOrUpdate(ctx, modelData, fields);
		}

		public OperationResult InsertOrUpdateModels(MContext ctx, List<GLPeriodTransferModel> modelData, string fields = null)
		{
			return dal.InsertOrUpdateModels(ctx, modelData, fields);
		}

		public OperationResult Delete(MContext ctx, string pkID)
		{
			return dal.Delete(ctx, pkID);
		}

		public OperationResult DeleteModels(MContext ctx, List<string> pkID)
		{
			return dal.DeleteModels(ctx, pkID);
		}

		public GLPeriodTransferModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.GetDataModel(ctx, pkID, includeDelete);
		}

		public GLPeriodTransferModel GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.GetDataModelByFilter(ctx, filter);
		}

		public List<GLPeriodTransferModel> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelList(ctx, filter, includeDelete);
		}

		public DataGridJson<GLPeriodTransferModel> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelPageList(ctx, filter, includeDelete);
		}
	}
}
