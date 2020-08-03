using JieNor.Megi.BusinessContract.FA;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.Enum;
using JieNor.Megi.DataModel.FA;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.FA;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.DataRepository.Log.GlLog;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.BusinessService.FA
{
	public class FADepreciationBusiness : IFADepreciationBusiness, IDataContract<FADepreciationModel>
	{
		private readonly FADepreciationRepository dal = new FADepreciationRepository();

		private readonly GLUtility utility = new GLUtility();

		public DataGridJson<FADepreciationModel> GetSummaryDepreciationPageList(MContext ctx, FAFixAssetsFilterModel filter)
		{
			int summaryDepreciationPageCount = dal.GetSummaryDepreciationPageCount(ctx, filter);
			List<FADepreciationModel> summaryDepreciationList = dal.GetSummaryDepreciationList(ctx, filter);
			return new DataGridJson<FADepreciationModel>
			{
				total = summaryDepreciationPageCount,
				rows = summaryDepreciationList
			};
		}

		public List<FADepreciationModel> GetSummaryDepreciationList(MContext ctx, FAFixAssetsFilterModel filter)
		{
			return dal.GetSummaryDepreciationList(ctx, filter);
		}

		public List<FADepreciationModel> GetDetailDepreciationList(MContext ctx, FAFixAssetsFilterModel filter)
		{
			return dal.GetDetailDepreciationList(ctx, filter);
		}

		public OperationResult SaveDepreciationList(MContext ctx, FAFixAssetsFilterModel filter)
		{
			List<FADepreciationModel> depreciationModels = filter.DepreciationModels;
			ValidateDepreciationModels(ctx, filter);
			depreciationModels.ForEach(delegate(FADepreciationModel x)
			{
				if (x.MCheckGroupValueModel != null)
				{
					x.MDepCheckGroupValueID = utility.GetCheckGroupValueID(ctx, x.MDepAccountCode, x.MCheckGroupValueModel);
					x.MExpCheckGroupValueID = utility.GetCheckGroupValueID(ctx, x.MExpAccountCode, x.MCheckGroupValueModel);
				}
				else if (!x.MIsLoadedCheckGroup)
				{
					x.MDepCheckGroupValueID = utility.GetCheckGroupValueID(ctx, x.MDepAccountCode, utility.GetCheckGroupValueModelByID(ctx, x.MDepCheckGroupValueID));
					x.MExpCheckGroupValueID = utility.GetCheckGroupValueID(ctx, x.MExpAccountCode, utility.GetCheckGroupValueModelByID(ctx, x.MExpCheckGroupValueID));
				}
				else
				{
					x.MDepCheckGroupValueID = null;
					x.MExpCheckGroupValueID = null;
				}
			});
			return dal.SaveDepreciationList(ctx, filter);
		}

		public OperationResult DepreciatePeriod(MContext ctx, FAFixAssetsFilterModel filter)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			List<CommandInfo> list2 = new List<CommandInfo>();
			if (filter.DepreciationModels == null || filter.DepreciationModels.Count == 0)
			{
				filter.IsCalculate = true;
				filter.DepreciationModels = GetDetailDepreciationList(ctx, filter);
			}
			else
			{
				filter.DepreciationModels.ForEach(delegate(FADepreciationModel x)
				{
					if (x.MCheckGroupValueModel != null)
					{
						x.MDepCheckGroupValueID = utility.GetCheckGroupValueID(ctx, x.MDepAccountCode, x.MCheckGroupValueModel);
						x.MExpCheckGroupValueID = utility.GetCheckGroupValueID(ctx, x.MExpAccountCode, x.MCheckGroupValueModel);
					}
					else if (!x.MIsLoadedCheckGroup)
					{
						x.MDepCheckGroupValueID = utility.GetCheckGroupValueID(ctx, x.MDepAccountCode, utility.GetCheckGroupValueModelByID(ctx, x.MDepCheckGroupValueID));
						x.MExpCheckGroupValueID = utility.GetCheckGroupValueID(ctx, x.MExpAccountCode, utility.GetCheckGroupValueModelByID(ctx, x.MExpCheckGroupValueID));
					}
					else
					{
						x.MDepCheckGroupValueID = null;
						x.MExpCheckGroupValueID = null;
					}
				});
			}
			List<string> codes = ValidateDepreciationModels(ctx, filter);
			string voucherNumber = COMResourceHelper.GetNextVoucherNumber(ctx, filter.Year, filter.Period, null, null);
			if (filter.IsRedepreciate)
			{
				FADepreciationModel fADepreciationModel = (from x in filter.DepreciationModels
				where !string.IsNullOrWhiteSpace(x.MVoucherID)
				select x).FirstOrDefault();
				if (fADepreciationModel != null)
				{
					list.AddRange(list2 = new GLVoucherRepository().GetDeleteVoucherModelsCmd(ctx, new List<string>
					{
						fADepreciationModel.MVoucherID
					}));
					voucherNumber = fADepreciationModel.MVoucherNumber;
				}
			}
			list.AddRange(list2 = GetCreateDepreciteVoucherNFixAssetsCmds(ctx, filter, codes, voucherNumber));
			list.AddRange(list2 = ModelInfoManager.GetInsertOrUpdateCmds(ctx, filter.DepreciationModels, null, true));
			return new OperationResult
			{
				Success = (new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(list) > 0)
			};
		}

		public List<CommandInfo> GetCreateDepreciteVoucherNFixAssetsCmds(MContext ctx, FAFixAssetsFilterModel filter, List<string> codes, string voucherNumber)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			GLUtility gLUtility = new GLUtility();
			GLDataPool instance = GLDataPool.GetInstance(ctx, false, 0, 0, 0);
			List<BDAccountModel> accountList = instance.AccountList;
			List<BDAccountModel> source = (from x in accountList
			where codes.Contains(x.MCode)
			select x).ToList();
			List<FAFixAssetsModel> fixAssetsList = new FAFixAssetsBusiness().GetFixAssetsList(ctx, new FAFixAssetsFilterModel
			{
				Status = -1
			});
			GLVoucherModel obj = new GLVoucherModel
			{
				MOrgID = ctx.MOrgID,
				MItemID = UUIDHelper.GetGuid(),
				IsNew = true,
				MNumber = voucherNumber,
				MYear = filter.Year,
				MPeriod = filter.Period,
				MStatus = 0,
				MTransferTypeID = 2
			};
			DateTime dateTime = new DateTime(filter.Year, filter.Period, 1);
			dateTime = dateTime.AddMonths(1);
			obj.MDate = dateTime.AddDays(-1.0);
			obj.MVoucherEntrys = new List<GLVoucherEntryModel>();
			GLVoucherModel gLVoucherModel = obj;
			List<GLVoucherEntryModel> mVoucherEntrys = gLVoucherModel.MVoucherEntrys;
			dateTime = new DateTime(filter.Year, filter.Period, 1);
			DateTime dateTime2 = dateTime.AddMonths(-1);
			int num = dateTime2.Year * 12 + dateTime2.Month;
			dateTime = ctx.MFABeginDate;
			int num2 = dateTime.Year * 12;
			dateTime = ctx.MFABeginDate;
			List<FADepreciationModel> source2 = (num < num2 + dateTime.Month) ? new List<FADepreciationModel>() : GetModelList(ctx, new SqlWhere().Equal("MYear", dateTime2.Year).Equal("MPeriod", dateTime2.Month), false);
			List<FADepreciationModel> list2 = (from x in filter.DepreciationModels
			where !x.MIsAdjust
			select x).ToList();
			List<FADepreciationModel> list3 = (from x in filter.DepreciationModels
			where x.MIsAdjust
			select x).ToList();
			List<FAFixAssetsChangeModel> list4 = null;
			for (int i = 0; i < list2.Count; i++)
			{
				FADepreciationModel dep = list2[i];
				FAFixAssetsModel fixAsset = fixAssetsList.FirstOrDefault((FAFixAssetsModel x) => x.MItemID == dep.MID);
				List<FADepreciationModel> list5 = (from x in source2
				where x.MID == dep.MID
				select x).ToList();
				decimal num3 = (list5.Count == 0) ? (filter.IsRedepreciate ? fixAsset.MSrcNetAmount : fixAsset.MNetAmount) : dep.MNetAmount;
				if (list5.Count > 0)
				{
					list4 = (list4 ?? new FAFixAssetsChangeRepository().GetFixAssetsChangeListForDepreciate(ctx, filter));
					List<FAFixAssetsChangeModel> list6 = (from x in list4.Where(delegate(FAFixAssetsChangeModel x)
					{
						int result;
						if (x.MID == fixAsset.MItemID)
						{
							DateTime mChangeFromPeriod = x.MChangeFromPeriod;
							int num5 = mChangeFromPeriod.Year * 12;
							mChangeFromPeriod = x.MChangeFromPeriod;
							result = ((num5 + mChangeFromPeriod.Month == filter.Year * 12 + filter.Period) ? 1 : 0);
						}
						else
						{
							result = 0;
						}
						return (byte)result != 0;
					})
					orderby x.MIndex
					select x).ToList();
					if (list6 != null && list6.Count > 1)
					{
						num3 -= list6.Last().MPrepareForDecreaseAmount - list6.First().MPrepareForDecreaseAmount;
						num3 += list6.Last().MOriginalAmount - list6.First().MOriginalAmount;
					}
				}
				if (num3 - fixAsset.MSalvageAmount < decimal.Zero)
				{
					num3 = fixAsset.MSalvageAmount;
					dep.MPeriodDepreciatedAmount = ((list5.Count == 0) ? fixAsset.MSrcNetAmount : list5.Sum((FADepreciationModel x) => x.MNetAmount)) - fixAsset.MSalvageAmount;
				}
				dep.MDepreciatedAmount = ((list5.Count == 0) ? fixAsset.MSrcDepreciatedAmount : list5.Sum((FADepreciationModel x) => x.MDepreciatedAmount)) + dep.MPeriodDepreciatedAmount;
				dep.MDepreciatedAmountOfYear = ((list5.Count == 0) ? fixAsset.MSrcDepreciatedAmountOfYear : list5.Sum((FADepreciationModel x) => x.MDepreciatedAmountOfYear)) + dep.MPeriodDepreciatedAmount;
				dep.MDepreciatedAmountOfYear = ((filter.Period == 1) ? dep.MPeriodDepreciatedAmount : dep.MDepreciatedAmountOfYear);
				dep.MDepreciatedPeriods = ((list5.Count == 0) ? fixAsset.MSrcDepreciatedPeriods : list5.Sum((FADepreciationModel x) => x.MDepreciatedPeriods)) + 1;
				dep.MNetAmount = num3;
				fixAsset.MNetAmount = num3;
				fixAsset.MLastDepreciatedDate = new DateTime(filter.Year, filter.Period, 1);
				list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<FAFixAssetsModel>(ctx, fixAsset, new List<string>
				{
					"MLastDepreciatedYear",
					"MLastDepreciatedPeriod"
				}, true));
				dep.MVoucherID = gLVoucherModel.MItemID;
				BDAccountModel bDAccountModel = source.FirstOrDefault((BDAccountModel x) => x.MCode == dep.MDepAccountCode) ?? new BDAccountModel();
				BDAccountModel bDAccountModel2 = source.FirstOrDefault((BDAccountModel x) => x.MCode == dep.MExpAccountCode) ?? new BDAccountModel();
				string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "PeriodDepreciation", "计提折旧");
				if (dep.MCheckGroupValueModel == null)
				{
					GLCheckGroupValueModel a = string.IsNullOrWhiteSpace(dep.MDepCheckGroupValueID) ? null : gLUtility.GetCheckGroupValueModelByID(ctx, dep.MDepCheckGroupValueID);
					GLCheckGroupValueModel b = string.IsNullOrWhiteSpace(dep.MExpCheckGroupValueID) ? null : gLUtility.GetCheckGroupValueModelByID(ctx, dep.MExpCheckGroupValueID);
					dep.MCheckGroupValueModel = gLUtility.MergeCheckGroupValueModel(a, b);
				}
				dep.MDepCheckGroupValueID = gLUtility.GetCheckGroupValueID(ctx, bDAccountModel, dep.MCheckGroupValueModel, dep.MDepCheckGroupValueID);
				dep.MExpCheckGroupValueID = gLUtility.GetCheckGroupValueID(ctx, bDAccountModel2, dep.MCheckGroupValueModel, dep.MExpCheckGroupValueID);
				mVoucherEntrys.Add(new GLVoucherEntryModel
				{
					MID = gLVoucherModel.MItemID,
					MEntryID = UUIDHelper.GetGuid(),
					IsNew = true,
					MExplanation = text,
					MAccountID = bDAccountModel2.MItemID,
					MCheckGroupValueID = dep.MExpCheckGroupValueID,
					MCurrencyID = ctx.MBasCurrencyID,
					MDebit = dep.MPeriodDepreciatedAmount,
					MAmount = dep.MPeriodDepreciatedAmount,
					MAmountFor = dep.MPeriodDepreciatedAmount,
					MExchangeRate = 1.0m,
					MDC = 1
				});
				mVoucherEntrys.Add(new GLVoucherEntryModel
				{
					MID = gLVoucherModel.MItemID,
					MEntryID = UUIDHelper.GetGuid(),
					IsNew = true,
					MExplanation = text,
					MAccountID = bDAccountModel.MItemID,
					MCheckGroupValueID = dep.MDepCheckGroupValueID,
					MCurrencyID = ctx.MBasCurrencyID,
					MCredit = dep.MPeriodDepreciatedAmount,
					MAmount = dep.MPeriodDepreciatedAmount,
					MAmountFor = dep.MPeriodDepreciatedAmount,
					MExchangeRate = 1.0m,
					MDC = -1
				});
			}
			for (int j = 0; j < list3.Count; j++)
			{
				FADepreciationModel dep2 = list3[j];
				FAFixAssetsModel fAFixAssetsModel = fixAssetsList.FirstOrDefault((FAFixAssetsModel x) => x.MItemID == dep2.MID);
				FADepreciationModel fADepreciationModel = list2.FirstOrDefault((FADepreciationModel x) => x.MID == dep2.MID);
				decimal num4 = fADepreciationModel.MNetAmount - dep2.MPeriodDepreciatedAmount;
				if (num4 - fAFixAssetsModel.MSalvageAmount < decimal.Zero)
				{
					num4 = fAFixAssetsModel.MSalvageAmount;
					dep2.MPeriodDepreciatedAmount = fADepreciationModel.MNetAmount - fAFixAssetsModel.MSalvageAmount;
				}
				dep2.MDepreciatedAmount = dep2.MPeriodDepreciatedAmount;
				dep2.MDepreciatedAmountOfYear = dep2.MPeriodDepreciatedAmount;
				dep2.MDepreciatedAmountOfYear = dep2.MPeriodDepreciatedAmount;
				dep2.MDepreciatedPeriods = 0;
				dep2.MVoucherID = gLVoucherModel.MItemID;
				BDAccountModel bDAccountModel3 = source.FirstOrDefault((BDAccountModel x) => x.MCode == dep2.MDepAccountCode) ?? new BDAccountModel();
				BDAccountModel bDAccountModel4 = source.FirstOrDefault((BDAccountModel x) => x.MCode == dep2.MExpAccountCode) ?? new BDAccountModel();
				string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "DepreciationAdjust", "折旧调整");
				dep2.MDepCheckGroupValueID = gLUtility.GetCheckGroupValueID(ctx, bDAccountModel3, dep2.MCheckGroupValueModel, dep2.MDepCheckGroupValueID);
				dep2.MExpCheckGroupValueID = gLUtility.GetCheckGroupValueID(ctx, bDAccountModel4, dep2.MCheckGroupValueModel, dep2.MExpCheckGroupValueID);
				fADepreciationModel.MNetAmount = num4;
				mVoucherEntrys.Add(new GLVoucherEntryModel
				{
					MID = gLVoucherModel.MItemID,
					MEntryID = UUIDHelper.GetGuid(),
					IsNew = true,
					MExplanation = text2,
					MAccountID = bDAccountModel4.MItemID,
					MCheckGroupValueID = dep2.MExpCheckGroupValueID,
					MCurrencyID = ctx.MBasCurrencyID,
					MDebit = dep2.MPeriodDepreciatedAmount,
					MExchangeRate = 1.0m,
					MAmount = dep2.MPeriodDepreciatedAmount,
					MAmountFor = dep2.MPeriodDepreciatedAmount,
					MDC = 1
				});
				mVoucherEntrys.Add(new GLVoucherEntryModel
				{
					MID = gLVoucherModel.MItemID,
					MEntryID = UUIDHelper.GetGuid(),
					IsNew = true,
					MExplanation = text2,
					MAccountID = bDAccountModel3.MItemID,
					MCheckGroupValueID = dep2.MDepCheckGroupValueID,
					MCurrencyID = ctx.MBasCurrencyID,
					MCredit = dep2.MPeriodDepreciatedAmount,
					MExchangeRate = 1.0m,
					MAmount = dep2.MPeriodDepreciatedAmount,
					MAmountFor = dep2.MPeriodDepreciatedAmount,
					MDC = -1
				});
			}
			MergeVoucherEntrys(gLVoucherModel);
			GLPeriodTransferModel modelData = new GLPeriodTransferModel
			{
				MItemID = UUIDHelper.GetGuid(),
				IsNew = true,
				MOrgID = ctx.MOrgID,
				MYear = gLVoucherModel.MYear,
				MPeriod = gLVoucherModel.MPeriod,
				MVoucherID = gLVoucherModel.MItemID,
				MTransferTypeID = 2,
				MAmount = gLVoucherModel.MDebitTotal
			};
			list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<GLPeriodTransferModel>(ctx, modelData, null, true));
			GLUtility.ReorderVoucherEntry(new List<GLVoucherModel>
			{
				gLVoucherModel
			});
			list.AddRange(GLVoucherRepository.GetInsertVoucherCmds(ctx, gLVoucherModel, null));
			list.AddRange(GlVoucherLogHelper.GetCreateLogCmd(ctx, gLVoucherModel));
			return list;
		}

		private void MergeVoucherEntrys(GLVoucherModel voucher)
		{
			List<GLVoucherEntryModel> list = (from x in voucher.MVoucherEntrys
			where !x.MIsDelete
			group x by new
			{
				x.MExplanation,
				x.MAccountID,
				x.MCurrencyID,
				x.MDC,
				x.MCheckGroupValueID
			} into y
			select new GLVoucherEntryModel
			{
				MID = voucher.MItemID,
				MEntryID = UUIDHelper.GetGuid(),
				IsNew = true,
				MExplanation = y.Key.MExplanation,
				MAccountID = y.Key.MAccountID,
				MCurrencyID = y.Key.MCurrencyID,
				MCheckGroupValueID = y.Key.MCheckGroupValueID,
				MDebit = y.Sum((GLVoucherEntryModel z) => z.MDebit),
				MCredit = y.Sum((GLVoucherEntryModel z) => z.MCredit),
				MExchangeRate = 1.0m,
				MAmount = y.Sum((GLVoucherEntryModel z) => z.MAmount),
				MAmountFor = y.Sum((GLVoucherEntryModel z) => z.MAmountFor),
				MYear = voucher.MYear,
				MDC = y.Key.MDC,
				MPeriod = voucher.MYear,
				MOrgID = voucher.MOrgID
			}).ToList();
			voucher.MDebitTotal = list.Sum((GLVoucherEntryModel x) => x.MDebit);
			voucher.MCreditTotal = list.Sum((GLVoucherEntryModel x) => x.MCredit);
			voucher.MVoucherEntrys = list;
		}

		public List<string> ValidateDepreciationModels(MContext ctx, FAFixAssetsFilterModel filter)
		{
			GLUtility gLUtility = new GLUtility();
			GLDataPool pool = GLDataPool.GetInstance(ctx, false, 0, 0, 0);
			filter.DepreciationModels.ForEach(delegate(FADepreciationModel x)
			{
				if (!string.IsNullOrWhiteSpace(x.MDepAccountCode) && !pool.AccountList.Exists((BDAccountModel y) => y.MCode == x.MDepAccountCode && y.MIsActive))
				{
					x.MDepAccountCode = null;
				}
				if (!string.IsNullOrWhiteSpace(x.MExpAccountCode) && !pool.AccountList.Exists((BDAccountModel y) => y.MCode == x.MExpAccountCode && y.MIsActive))
				{
					x.MExpAccountCode = null;
				}
			});
			List<string> list = (from x in (from x in filter.DepreciationModels
			select x.MDepAccountCode).Concat(from x in filter.DepreciationModels
			select x.MExpAccountCode)
			where !string.IsNullOrWhiteSpace(x)
			select x).Distinct().ToList();
			ValidateQueryModel validateQueryModel = new ValidateQueryModel(filter.DepreciationModels.Exists((FADepreciationModel x) => string.IsNullOrWhiteSpace(x.MDepAccountCode) || string.IsNullOrWhiteSpace(x.MExpAccountCode)) ? MActionResultCodeEnum.MAccountAreRequiredForDepreciation : MActionResultCodeEnum.MValid);
			ValidateQueryModel validateCommonModelSql = gLUtility.GetValidateCommonModelSql<FAFixAssetsModel>(MActionResultCodeEnum.MDeprecationVoucherCreated, (from x in filter.DepreciationModels
			select x.MID).ToList(), null, null);
			ValidateQueryModel validateCommonModelSql2 = gLUtility.GetValidateCommonModelSql<BDAccountModel>(MActionResultCodeEnum.MAccountInvalid, list, "MCode", null);
			ValidateQueryModel validateAccountHasSubSql = gLUtility.GetValidateAccountHasSubSql(list, true);
			ValidateQueryModel validateQueryModel2 = filter.IsRedepreciate ? new ValidateQueryModel() : gLUtility.GetValidateCreatedDepreciationVoucher(filter.Year, filter.Period);
			ValidateQueryModel checkDepreciationVoucherApproved = gLUtility.GetCheckDepreciationVoucherApproved(filter.Year, filter.Period);
			ValidateQueryModel validateDepreciatedBeforePeriod = gLUtility.GetValidateDepreciatedBeforePeriod(ctx, filter.Year, filter.Period);
			ValidateQueryModel validatePeriodClosedSql = gLUtility.GetValidatePeriodClosedSql(filter.Year, filter.Period);
			List<MActionResultCodeEnum> list2 = gLUtility.QueryValidateSql(ctx, true, validateQueryModel, validateCommonModelSql, validateCommonModelSql2, validateAccountHasSubSql, validateQueryModel2, checkDepreciationVoucherApproved, validateDepreciatedBeforePeriod, validatePeriodClosedSql);
			return list;
		}

		public bool Exists(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.Exists(ctx, pkID, includeDelete);
		}

		public bool ExistsByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.ExistsByFilter(ctx, filter);
		}

		public OperationResult InsertOrUpdate(MContext ctx, FADepreciationModel modelData, string fields = null)
		{
			return dal.InsertOrUpdate(ctx, modelData, fields);
		}

		public OperationResult InsertOrUpdateModels(MContext ctx, List<FADepreciationModel> modelData, string fields = null)
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

		public FADepreciationModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.GetDataModel(ctx, pkID, includeDelete);
		}

		public FADepreciationModel GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.GetDataModelByFilter(ctx, filter);
		}

		public List<FADepreciationModel> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelList(ctx, filter, includeDelete);
		}

		public DataGridJson<FADepreciationModel> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelPageList(ctx, filter, includeDelete);
		}
	}
}
