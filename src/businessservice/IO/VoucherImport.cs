using JieNor.Megi.BusinessService.BD;
using JieNor.Megi.BusinessService.GL;
using JieNor.Megi.Common.Logger;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Helper;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.IO;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace JieNor.Megi.BusinessService.IO
{
	public class VoucherImport : ImportBase, IImport
	{
		private const string TOTAL_ROW_KEYWORD = "合计";

		private IOSolutionBusiness solution = new IOSolutionBusiness();

		private List<CommandInfo> cmdList = new List<CommandInfo>();

		private List<CommandInfo> newTrackCmdList = new List<CommandInfo>();

		private List<REGCurrencyViewModel> currencyList = null;

		private List<BDAccountListModel> acctList = null;

		private List<string> newCustomerList = new List<string>();

		private List<string> newSupplierList = new List<string>();

		private List<NameValueModel> trackList = new List<NameValueModel>();

		private List<BDContactsModel> contactList = new List<BDContactsModel>();

		private string[] importTrackNameList = null;

		private List<BDTrackModel> trackNameList = null;

		private List<BDTrackModel> newTrackList = new List<BDTrackModel>();

		private Dictionary<string, string> newTrackEntryList = new Dictionary<string, string>();

		private List<IOValidationResultModel> validationResult = new List<IOValidationResultModel>();

		private IOSolutionModel solutionModel = null;

		private string rowNo = null;

		private string tmpRowNo = null;

		private int dataRowNo = 0;

		public string[][] MultiSelectConfig => new string[8][]
		{
			new string[1]
			{
				"MContact"
			},
			null,
			null,
			null,
			null,
			null,
			null,
			null
		};

		public ImportResult ImportData(MContext ctx, IOImportDataModel data)
		{
			ImportResult importResult = new ImportResult();
			try
			{
				OperationResult operationResult = solution.SaveSolution(ctx, data, false);
				if (!operationResult.Success)
				{
					importResult.Message = operationResult.Message;
					return importResult;
				}
				List<IOSolutionConfigModel> solutionConfigList = solution.GetSolutionConfigList(ctx, data.TemplateType, data.SolutionID);
				if (solutionConfigList == null || solutionConfigList.Count == 0)
				{
					importResult.Success = false;
					return importResult;
				}
				List<string> list;
				IOImportDataModel effectiveData = GetEffectiveData(ctx, data, solutionConfigList, data.SourceData, data.MHeaderRowIndex, data.MDataRowIndex, out list);
				data.EffectiveData = effectiveData.EffectiveData;
				BDTrackBusiness bDTrackBusiness = new BDTrackBusiness();
				data.TrackItemNameList = bDTrackBusiness.TrimTrackPrefix(effectiveData.TrackItemNameList).ToArray();
				if (list.Count > 0)
				{
					importResult.MessageList = list;
					importResult.Success = false;
					return importResult;
				}
				if (data.EffectiveData == null || data.EffectiveData.Rows.Count == 0)
				{
					importResult.Success = false;
					importResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "NoDataToImport", "That's no data to import!");
					return importResult;
				}
				List<IOVoucherImportModel> list2 = base.ConvertToList<IOVoucherImportModel>(data.EffectiveData);
				ValidateBasicData(ctx, list2, data);
				List<GLVoucherModel> list3 = new List<GLVoucherModel>();
				List<IOVoucherImportModel> mainVoucherList = GetMainVoucherList(list2);
				List<GLVoucherEntryModel> list4 = new List<GLVoucherEntryModel>();
				rowNo = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "ErrorRowNo", "Row {0}: ");
				solutionModel = ModelInfoManager.GetDataEditModel<IOSolutionModel>(ctx, data.SolutionID, false, true);
				foreach (IOVoucherImportModel item in mainVoucherList)
				{
					GLVoucherModel voucherModel = GetVoucherModel(ctx, item, list2);
					list4.AddRange(voucherModel.MVoucherEntrys);
					list3.Add(voucherModel);
				}
				CheckNumberDuplicate(ctx, list3);
				if (newTrackList.Any())
				{
					string empty = string.Empty;
					newTrackCmdList.AddRange(base.GetNewTrackCommandList(ctx, newTrackList, ref empty));
					if (!string.IsNullOrWhiteSpace(empty))
					{
						throw new Exception(empty);
					}
				}
				if (newTrackCmdList.Any())
				{
					DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
					importResult.Success = (dynamicDbHelperMySQL.ExecuteSqlTran(newTrackCmdList) > 0);
					if (!importResult.Success)
					{
						List<string> list5 = new List<string>();
						foreach (BDTrackModel newTrack in newTrackList)
						{
							string arg = string.Join("、", from f in newTrack.MEntryList
							select f.MName);
							list5.Add($"{newTrack.MName}：{arg}");
						}
						importResult.Message = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "NewTrackSaveFailed", "The new Track:{0} save failed!"), string.Join("，", list5));
						return importResult;
					}
				}
				base.SetDimensionByName(ctx, validationResult, list4);
				base.ValidateVoucherCheckGroupInfo(ctx, validationResult, list4, solutionModel.MDataRowIndex);
				base.SetValidationResult(ctx, importResult, validationResult, false);
				if (!importResult.Success)
				{
					return importResult;
				}
				OperationResult operationResult2 = new GLVoucherBusiness().UpdateVouchers(ctx, list3, cmdList, 0, -2);
				importResult.Success = operationResult2.Success;
				importResult.Message = operationResult2.Message;
				importResult.Tag = list3.Count().ToString();
			}
			catch (MActionException ex)
			{
				importResult.Success = false;
				GLInterfaceRepository.HandleActionException(ctx, importResult, ex, false);
			}
			catch (Exception ex2)
			{
				importResult.Success = false;
				importResult.Message = ex2.Message;
				MLogger.Log(ex2);
			}
			return importResult;
		}

		private void CheckNumberDuplicate(MContext ctx, List<GLVoucherModel> vchList)
		{
			IEnumerable<IGrouping<string, GLVoucherModel>> enumerable = from f in vchList
			group f by f.MDate.ToString("yyyy-MM");
			if (enumerable.Any())
			{
				foreach (IGrouping<string, GLVoucherModel> item in enumerable)
				{
					List<GLVoucherModel> list = new List<GLVoucherModel>();
					list.AddRange(item.ToList());
					string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "DuplicateVoucherNo", "The importing data have duplicate voucher number!({0}-{{0}})");
					base.CheckFieldDuplicate(list, "MNumber", string.Format(text, item.Key));
				}
			}
		}

		private void ValidateBasicData(MContext ctx, List<IOVoucherImportModel> list, IOImportDataModel data)
		{
			DateTime t = (from f in list
			select f.MDate).Distinct().Min();
			if (t < ctx.MBeginDate)
			{
				throw new Exception(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "CannotImportVoucherBeforeBeginDate", "You can't import voucher before begin date:{0}!"), ctx.MGLBeginDate.ToString("yyyy-MM-dd")));
			}
			DateTime currentPeriod = new GLSettlementRepository().GetCurrentPeriod(ctx);
			DateTime errorDate = DateTime.MinValue;
			if ((from x in list
			select x.MDate).ToList().Exists((DateTime y) => (errorDate = new DateTime(y.Year, y.Month, 1)) < new DateTime(currentPeriod.Year, currentPeriod.Month, 1)))
			{
				string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "PeriodOfVoucherClosed", "凭证所在期间[{0}-{1}]已经结账了");
				throw new Exception(string.Format(text, errorDate.Year, errorDate.Month));
			}
			currencyList = base.GetCurrencyList(ctx, true);
			CheckNumberIsUsed(ctx, list);
			acctList = base.GetAccountList(ctx);
			List<string> source = (from f in list
			select f.MAccountID.GetValidAccountNo()).Distinct().ToList();
			List<string> list2 = (from r in source
			where !acctList.Any((BDAccountListModel f) => f.MNumber.GetValidAccountNo() == r.Trim())
			select r).ToList();
			if (list2.Any())
			{
				throw new Exception(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "AccountNotFound", "The account:{0} can't be found!"), string.Join("、", list2)));
			}
			List<string> list3 = (from f in list
			select f.MContact into v
			where !string.IsNullOrWhiteSpace(v)
			select v.Trim()).Distinct().ToList();
			contactList = base.GetContactListByName(ctx, list3);
			IEnumerable<string> enumerable = from v in list3
			where !contactList.Any((BDContactsModel f) => f.MName == v)
			select v;
			if (enumerable?.Any() ?? false)
			{
				throw new Exception(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "ContactNotFound", "The contact:{0} can't be found!"), string.Join("、", enumerable)));
			}
			if (data.TrackItemNameList != null)
			{
				data.TrackItemNameList = base.TrimTrackPrefix(data.TrackItemNameList).ToArray();
				IEnumerable<string> source2 = from v in data.TrackItemNameList
				where !string.IsNullOrWhiteSpace(v)
				select v;
				if (source2.Any())
				{
					importTrackNameList = data.TrackItemNameList;
					trackList = base.GetTrackBasicInfo(ctx);
					trackNameList = base.GetTrackListByName(ctx, source2.ToList());
				}
			}
		}

		private void CheckNumberIsUsed(MContext ctx, List<IOVoucherImportModel> list)
		{
			Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
			List<IOVoucherImportModel> mainVoucherList = GetMainVoucherList(list);
			foreach (IOVoucherImportModel item in mainVoucherList)
			{
				string key = item.MDate.ToString("yyyy-MM-dd");
				if (dictionary.ContainsKey(key))
				{
					if (dictionary[key] == null)
					{
						dictionary[key] = new List<string>();
					}
					dictionary[key].Add(COMResourceHelper.ToVoucherNumber(ctx, item.MNumber, 0));
				}
				else
				{
					dictionary.Add(key, new List<string>
					{
						COMResourceHelper.ToVoucherNumber(ctx, item.MNumber, 0)
					});
				}
			}
			Dictionary<string, string> dictionary2 = new GLVoucherBusiness().IsMNumberUsed(ctx, dictionary);
			if (!dictionary2.Any())
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string key2 in dictionary2.Keys)
			{
				stringBuilder.AppendFormat("{0}:{1}", key2, dictionary2[key2]);
				stringBuilder.AppendLine();
			}
			throw new Exception(stringBuilder + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "isoccupied", "被占用了"));
		}

		private GLVoucherModel GetVoucherModel(MContext ctx, IOVoucherImportModel mainModel, List<IOVoucherImportModel> list)
		{
			List<IOVoucherImportModel> list2 = (from t in list
			where t.GroupByID == mainModel.GroupByID
			select t into f
			orderby f.MEntrySeq
			select f).ToList();
			if (list2 == null || list2.Count == 0)
			{
				return null;
			}
			GLVoucherModel gLVoucherModel = new GLVoucherModel();
			gLVoucherModel.MDate = mainModel.MDate;
			GLVoucherModel gLVoucherModel2 = gLVoucherModel;
			DateTime dateTime = gLVoucherModel.MDate;
			gLVoucherModel2.MYear = dateTime.Year;
			GLVoucherModel gLVoucherModel3 = gLVoucherModel;
			dateTime = gLVoucherModel.MDate;
			gLVoucherModel3.MPeriod = dateTime.Month;
			gLVoucherModel.MNumber = COMResourceHelper.ToVoucherNumber(ctx, mainModel.MNumber, 0);
			gLVoucherModel.MSourceBillKey = 1.ToString();
			gLVoucherModel.MVoucherEntrys = GetVoucherEntryList(ctx, gLVoucherModel, list2);
			gLVoucherModel.MCreditTotal = gLVoucherModel.MVoucherEntrys.Sum((GLVoucherEntryModel f) => f.MCredit);
			gLVoucherModel.MDebitTotal = gLVoucherModel.MVoucherEntrys.Sum((GLVoucherEntryModel f) => f.MDebit);
			DateTime currentPeriod = new GLSettlementRepository().GetCurrentPeriod(ctx);
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "CannotImportVoucherBeforeBeginDate", "You can't import voucher before begin date:{0}!");
			dateTime = ctx.MGLBeginDate;
			string message = string.Format(text, dateTime.ToString("yyyy-MM-dd"));
			string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "PeriodOfVoucherClosed", "凭证所在期间[{0}-{1}]已经结账了");
			int num = gLVoucherModel.MYear * 100 + gLVoucherModel.MPeriod;
			dateTime = ctx.MGLBeginDate;
			int num2 = dateTime.Year * 100;
			dateTime = ctx.MGLBeginDate;
			if (num < num2 + dateTime.Month)
			{
				throw new Exception(message);
			}
			if (gLVoucherModel.MYear * 100 + gLVoucherModel.MPeriod < currentPeriod.Year * 100 + currentPeriod.Month)
			{
				throw new Exception(string.Format(text2, gLVoucherModel.MYear, gLVoucherModel.MPeriod));
			}
			if (gLVoucherModel.MVoucherEntrys.Count <= 1 || gLVoucherModel.MCreditTotal != gLVoucherModel.MDebitTotal)
			{
				string message2 = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "CreditAndDebitImbalanceForImport!", "{0}：Credit({1}) and Debit({2}) imbalance"), gLVoucherModel.MNumber, gLVoucherModel.MCreditTotal, gLVoucherModel.MDebitTotal);
				throw new Exception(message2);
			}
			return gLVoucherModel;
		}

		private void SetDirection(GLVoucherEntryModel entry, string debitValue)
		{
			if (entry.MDebit != decimal.Zero)
			{
				entry.MDC = 1;
			}
			else if (entry.MCredit != decimal.Zero)
			{
				entry.MDC = -1;
			}
			else
			{
				entry.MDC = ((!string.IsNullOrWhiteSpace(debitValue)) ? 1 : (-1));
			}
		}

		private List<GLVoucherEntryModel> GetVoucherEntryList(MContext ctx, GLVoucherModel model, List<IOVoucherImportModel> entryList)
		{
			int num = 1;
			List<GLVoucherEntryModel> list = new List<GLVoucherEntryModel>();
			foreach (IOVoucherImportModel entry in entryList)
			{
				GLVoucherEntryModel entryModel = new GLVoucherEntryModel();
				ModelHelper.CopyModelValue(entry, entryModel, false);
				SetDirection(entryModel, entry.MDebit);
				tmpRowNo = string.Format(rowNo, solutionModel.MDataRowIndex + dataRowNo);
				entryModel.MEntrySeq = num;
				num++;
				string acctNumber = entryModel.MAccountID.GetValidAccountNo();
				BDAccountListModel bDAccountListModel = acctList.FirstOrDefault((BDAccountListModel f) => f.MNumber.GetValidAccountNo() == acctNumber);
				if (bDAccountListModel != null)
				{
					entryModel.MAccountID = bDAccountListModel.MItemID;
				}
				BDContactsModel bDContactsModel = contactList.FirstOrDefault((BDContactsModel f) => f.MName == entry.MContact.Trim());
				if (bDContactsModel != null)
				{
					entryModel.MContactID = bDContactsModel.MItemID;
				}
				decimal num2 = decimal.One;
				bool flag = !string.IsNullOrWhiteSpace(entry.MExchangeRate) && decimal.TryParse(entry.MExchangeRate, out num2) && num2 > decimal.Zero;
				num2 = ((num2 == decimal.Zero) ? decimal.One : num2);
				SetTransMoney(entryModel);
				if (!string.IsNullOrWhiteSpace(entry.MCurrencyID))
				{
					REGCurrencyViewModel rEGCurrencyViewModel = currencyList.FirstOrDefault((REGCurrencyViewModel f) => entry.MCurrencyID.ToUpper().Contains(f.MName.ToUpper()) || entry.MCurrencyID.ToUpper().Contains(f.MCurrencyID));
					if (rEGCurrencyViewModel != null)
					{
						entryModel.MCurrencyID = rEGCurrencyViewModel.MCurrencyID;
						decimal d = entryModel.MCredit + entryModel.MDebit;
						if (string.IsNullOrWhiteSpace(entry.MExchangeRate))
						{
							if (entryModel.MAmountFor > decimal.Zero && entryModel.MAmountFor != d)
							{
								num2 = Math.Round(entryModel.MAmountFor / d, 6);
								entry.MExchangeRate = num2.ToString();
							}
						}
						else if (flag && num2 != decimal.One && Convert.ToDecimal(rEGCurrencyViewModel.MUserRate) != decimal.One && !bDAccountListModel.MIsCheckForCurrency)
						{
							if (entry.MAccountID.StartsWith("1001") || entry.MAccountID.StartsWith("1002"))
							{
								throw new Exception(string.Format(tmpRowNo + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "UnSupportForeignAccounting", "该科目:{0}不支持外币核算!"), entry.MAccountID));
							}
							UpdateForeignAccount(ctx, bDAccountListModel.MItemID);
						}
						if (num2 != decimal.One && Convert.ToDecimal(rEGCurrencyViewModel.MUserRate) == decimal.One)
						{
							throw new Exception(tmpRowNo + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "ExchangeRateUnConsistentWithCurrency", "所选币别为本位币时，原币金额必须等于借/贷方金额！"));
						}
						if (string.IsNullOrWhiteSpace(entry.MExchangeRate) && Convert.ToDecimal(rEGCurrencyViewModel.MUserRate) != decimal.One)
						{
							throw new Exception(tmpRowNo + string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "CannotFindExchangeRate", "找不到导入的外币：{0}对应的汇率，请检查汇率或原币金额是否有值!"), entry.MCurrencyID));
						}
						if (num2 != decimal.One)
						{
							REGCurrencyViewModel rEGCurrencyViewModel2 = currencyList.FirstOrDefault((REGCurrencyViewModel f) => f.MCurrencyID == entryModel.MCurrencyID && f.MRateDate == model.MDate);
							if (rEGCurrencyViewModel2 == null)
							{
								AddExchangeRate(ctx, currencyList, rEGCurrencyViewModel, entry);
							}
						}
						goto IL_05f9;
					}
					throw new Exception(tmpRowNo + string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "CurrencyNotFound", "The currency:{0} can't be found!"), entryModel.MCurrencyID));
				}
				decimal mAmount = entryModel.MAmount;
				if (entryModel.MAmountFor == decimal.Zero)
				{
					entryModel.MAmountFor = mAmount;
				}
				if (flag || (entryModel.MAmountFor > decimal.Zero && entryModel.MAmountFor != mAmount))
				{
					if (bDAccountListModel.MIsCheckForCurrency)
					{
						throw new Exception(tmpRowNo + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "NotSelectForeignCurrency", "原币金额不等于本位币金额时，币别必须为外币!"));
					}
					throw new Exception(tmpRowNo + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "OriginalAmtNotEqualDebitOrCreditAmt", "非外币核算的科目，原币金额必须等于借/贷方金额!"));
				}
				goto IL_05f9;
				IL_05f9:
				entryModel.MExchangeRate = Math.Round(decimal.One / num2, 6);
				if (entryModel.MAmountFor == decimal.Zero)
				{
					entryModel.MAmountFor = entryModel.MAmount * num2;
				}
				if ((entryModel.MDebit != decimal.Zero && entryModel.MCredit != decimal.Zero) || (!string.IsNullOrWhiteSpace(entry.MDebit) && !string.IsNullOrWhiteSpace(entry.MCredit) && entryModel.MDebit == decimal.Zero && entryModel.MCredit == decimal.Zero))
				{
					throw new Exception(string.Format(tmpRowNo + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "DebitAndCreditCanOnlyFillOne", "{0}: Debit and Credit can only fill one!"), model.MNumber));
				}
				SetTrackItem(ctx, entryModel);
				list.Add(entryModel);
				dataRowNo++;
			}
			return list;
		}

		private void SetTrackItem(MContext ctx, GLVoucherEntryModel entryModel)
		{
			if (importTrackNameList != null)
			{
				int i;
				for (i = 0; i < importTrackNameList.Length; i++)
				{
					if (!string.IsNullOrWhiteSpace(importTrackNameList[i]))
					{
						string propName = "MTrackItem" + (i + 1);
						string entryName = ModelHelper.GetModelValue(entryModel, propName);
						if (!string.IsNullOrWhiteSpace(entryName))
						{
							BDTrackModel matchedTrack = trackNameList.FirstOrDefault((BDTrackModel f) => !string.IsNullOrWhiteSpace(f.MName) && HttpUtility.HtmlDecode(f.MName.Trim()) == importTrackNameList[i]);
							string key = importTrackNameList[i] + entryName;
							string empty = string.Empty;
							if (matchedTrack != null)
							{
								BDTrackModel bDTrackModel = trackNameList.FirstOrDefault((BDTrackModel f) => f.MItemID == matchedTrack.MItemID && !string.IsNullOrWhiteSpace(f.MEntryName) && HttpUtility.HtmlDecode(f.MEntryName.Trim()) == entryName);
								if (bDTrackModel != null)
								{
									empty = bDTrackModel.MEntryID;
								}
								else if (!newTrackEntryList.ContainsKey(key))
								{
									empty = AddTrackEntry(ctx, matchedTrack.MItemID, entryName);
									newTrackEntryList.Add(key, empty);
								}
								else
								{
									empty = newTrackEntryList[key];
								}
							}
							else
							{
								BDTrackModel bDTrackModel2 = newTrackList.SingleOrDefault((BDTrackModel f) => f.MName == importTrackNameList[i]);
								if (bDTrackModel2 != null)
								{
									if (bDTrackModel2.MEntryName == entryName)
									{
										empty = bDTrackModel2.MEntryID;
									}
									else if (!newTrackEntryList.ContainsKey(key))
									{
										empty = AddTrackEntry(ctx, bDTrackModel2.MItemID, entryName);
										newTrackEntryList.Add(key, empty);
									}
									else
									{
										empty = newTrackEntryList[key];
									}
								}
								else
								{
									empty = AddTrack(ctx, importTrackNameList[i], entryName);
								}
							}
							ModelHelper.SetModelValue(entryModel, propName, empty, null);
						}
					}
				}
			}
		}

		private void SetTransMoney(GLVoucherEntryModel entryModel)
		{
			if (entryModel.MCredit == entryModel.MDebit && entryModel.MCredit != decimal.Zero)
			{
				if (entryModel.MCredit > decimal.Zero)
				{
					entryModel.MCredit = decimal.Zero;
				}
				else
				{
					entryModel.MDebit = decimal.Zero;
					entryModel.MCredit = Math.Abs(entryModel.MCredit);
				}
			}
			entryModel.MAmount = entryModel.MCredit + entryModel.MDebit;
		}

		private void UpdateForeignAccount(MContext ctx, string acctId)
		{
			BDAccountModel modelData = new BDAccountModel
			{
				MItemID = acctId,
				MIsCheckForCurrency = true
			};
			List<string> fields = new List<string>
			{
				"MIsCheckForCurrency"
			};
			cmdList.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDAccountModel>(ctx, modelData, fields, true));
		}

		private void AddExchangeRate(MContext ctx, List<REGCurrencyViewModel> currencyList, REGCurrencyViewModel tgtCurrency, IOVoucherImportModel entry)
		{
			REGCurrencyViewModel rEGCurrencyViewModel = currencyList.FirstOrDefault((REGCurrencyViewModel f) => Convert.ToDecimal(f.MUserRate) == decimal.One);
			if (rEGCurrencyViewModel == null || tgtCurrency == null)
			{
				return;
			}
			BDExchangeRateModel model = new BDExchangeRateModel
			{
				MOrgID = ctx.MOrgID,
				MRateDate = entry.MDate,
				MUserRate = Convert.ToDecimal(entry.MExchangeRate),
				MRate = Math.Round(decimal.One / Convert.ToDecimal(entry.MExchangeRate), 6),
				MTargetCurrencyID = tgtCurrency.MCurrencyID,
				MSourceCurrencyID = rEGCurrencyViewModel.MCurrencyID
			};
			OperationResult operationResult = new OperationResult();
			cmdList.AddRange(new BDExchangeRateBusiness().GetUpdateExchangeRateCmdList(ctx, model, ref operationResult));
			if (operationResult.Success)
			{
				return;
			}
			throw new Exception(operationResult.Message);
		}

		private string AddTrack(MContext ctx, string name, string entryName)
		{
			string guid = UUIDHelper.GetGuid();
			string guid2 = UUIDHelper.GetGuid();
			BDTrackModel item = new BDTrackModel
			{
				MOrgID = ctx.MOrgID,
				MItemID = guid,
				IsNew = true,
				MName = name,
				MEntryName = entryName,
				MultiLanguage = new List<MultiLanguageFieldList>
				{
					base.GetMultiLanguageList("MName", name)
				},
				MEntryList = new List<BDTrackEntryModel>
				{
					new BDTrackEntryModel
					{
						MEntryID = guid2,
						IsNew = true,
						MItemID = guid,
						MultiLanguage = new List<MultiLanguageFieldList>
						{
							base.GetMultiLanguageList("MName", entryName)
						}
					}
				}
			};
			newTrackList.Add(item);
			return guid2;
		}

		private string AddTrackEntry(MContext ctx, string itemId, string entryName)
		{
			string guid = UUIDHelper.GetGuid();
			BDTrackEntryModel modelData = new BDTrackEntryModel
			{
				MEntryID = guid,
				IsNew = true,
				MItemID = itemId,
				MultiLanguage = new List<MultiLanguageFieldList>
				{
					base.GetMultiLanguageList("MName", entryName)
				}
			};
			newTrackCmdList.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDTrackEntryModel>(ctx, modelData, null, true));
			return guid;
		}

		private void AddContactCommandList(MContext ctx, BDContactsModel contactModel)
		{
			if (contactModel.MIsCustomer)
			{
				if (!newCustomerList.Contains(contactModel.MName))
				{
					cmdList.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDContactsModel>(ctx, contactModel, null, true));
				}
				newCustomerList.Add(contactModel.MName);
			}
			else if (contactModel.MIsSupplier)
			{
				if (!newSupplierList.Contains(contactModel.MName))
				{
					cmdList.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDContactsModel>(ctx, contactModel, null, true));
				}
				newSupplierList.Add(contactModel.MName);
			}
		}

		private List<IOVoucherImportModel> GetMainVoucherList(List<IOVoucherImportModel> list)
		{
			return (from p in list
			group p by new
			{
				p.GroupByID,
				p.MDate,
				p.MNumber
			} into g
			select new IOVoucherImportModel
			{
				GroupByID = g.Key.GroupByID,
				MDate = g.Key.MDate,
				MNumber = g.Key.MNumber
			}).ToList();
		}

		private IOImportDataModel GetEffectiveData(MContext ctx, IOImportDataModel model, List<IOSolutionConfigModel> soluConfig, DataTable sourceData, int headerRowIndex, int dataRowIndex, out List<string> errorMsgList)
		{
			IOImportDataModel iOImportDataModel = new IOImportDataModel();
			iOImportDataModel.TrackItemNameList = new string[5];
			errorMsgList = new List<string>();
			if (sourceData == null || sourceData.Rows.Count < headerRowIndex)
			{
				return null;
			}
			DataRow dataRow = sourceData.Rows[headerRowIndex - 1];
			if (dataRow.ItemArray.Length == 0)
			{
				return null;
			}
			List<KeyValuePair<int, IOSolutionConfigModel>> columnMath = base.GetColumnMath(dataRow, soluConfig);
			DataTable dataTable = new DataTable();
			dataTable.TableName = "datatable1";
			foreach (IOSolutionConfigModel item in soluConfig)
			{
				dataTable.Columns.Add(item.MConfigStandardName);
			}
			dataTable.Columns.Add("GroupByID");
			DataRow dataRow2 = null;
			int num = 0;
			bool flag = true;
			string[] array = MultiSelectConfig[(int)(model.TemplateType - 1)];
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "FieldMappingDataRequired", "“{0}”字段不能为空。（{1}）");
			string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "FieldMappingDataFormatError", "“{0}”字段格式不正确。（{1}）");
			for (int i = dataRowIndex - 1; i < sourceData.Rows.Count; i++)
			{
				DataRow dataRow3 = sourceData.Rows[i];
				if (i == sourceData.Rows.Count - 1)
				{
					bool flag2 = false;
					int num2 = 0;
					while (num2 < sourceData.Columns.Count)
					{
						if (dataRow3[num2] == null || !"合计".Equals(Convert.ToString(dataRow3[num2]).Trim()))
						{
							num2++;
							continue;
						}
						flag2 = true;
						break;
					}
					if (flag2)
					{
						break;
					}
				}
				bool flag3 = true;
				DataRow dataRow4 = dataTable.NewRow();
				int num3 = 0;
				int num4 = 0;
				foreach (KeyValuePair<int, IOSolutionConfigModel> item2 in columnMath)
				{
					if (dataRow3[item2.Key] != null && !string.IsNullOrWhiteSpace(dataRow3[item2.Key].ToString()))
					{
						break;
					}
					num4++;
				}
				if (num4 != columnMath.Count())
				{
					foreach (KeyValuePair<int, IOSolutionConfigModel> item3 in columnMath)
					{
						object obj = dataRow3[item3.Key];
						if (item3.Value.MIsKey && dataRow2 != null && IsMainData(dataRow3, columnMath))
						{
							obj = dataRow2[item3.Key];
							flag3 = false;
						}
						string text3 = obj?.ToString() ?? string.Empty;
						if (item3.Value.MDataType == "decimal")
						{
							text3 = text3.Replace(",", "").Replace("，", "");
						}
						if (flag3 && item3.Value.MIsKey && dataRow2 != null && text3 == Convert.ToString(dataRow2[item3.Key]))
						{
							num3++;
						}
						string arg = $"{sourceData.Columns[item3.Key].ColumnName}{i + 1}";
						if (item3.Value.MIsDataRequired && text3 == string.Empty)
						{
							errorMsgList.Add(string.Format(text, item3.Value.MConfigName, arg));
						}
						else if (!string.IsNullOrEmpty(item3.Value.MExpression) && text3 != string.Empty)
						{
							Regex regex = new Regex(item3.Value.MExpression);
							if (!regex.IsMatch(text3))
							{
								errorMsgList.Add(string.Format(text2, item3.Value.MConfigName, arg));
							}
						}
						string text4 = Convert.ToString(dataRow[item3.Key]);
						if (i == dataRowIndex - 1 && item3.Value.MConfigStandardName.StartsWith("MTrackItem") && !string.IsNullOrWhiteSpace(text4))
						{
							int num5 = Convert.ToInt32(item3.Value.MConfigStandardName.Replace("MTrackItem", ""));
							iOImportDataModel.TrackItemNameList[num5 - 1] = text4;
						}
						if (array?.Contains(item3.Value.MConfigStandardName) ?? false)
						{
							if (string.IsNullOrWhiteSpace(Convert.ToString(dataRow4[item3.Value.MConfigStandardName])))
							{
								dataRow4[item3.Value.MConfigStandardName] = text3;
							}
						}
						else
						{
							dataRow4[item3.Value.MConfigStandardName] = text3;
						}
					}
					if (flag3 && num3 == columnMath.Count((KeyValuePair<int, IOSolutionConfigModel> f) => f.Value.MIsKey))
					{
						flag3 = false;
					}
					if (flag)
					{
						if (!flag3)
						{
							flag = false;
						}
					}
					else if (flag3)
					{
						flag = true;
					}
					if (flag)
					{
						num++;
					}
					dataRow4["GroupByID"] = num;
					dataTable.Rows.Add(dataRow4);
					if (flag)
					{
						dataRow2 = dataRow3;
					}
				}
			}
			iOImportDataModel.EffectiveData = dataTable;
			return iOImportDataModel;
		}

		private bool IsMainData(DataRow srcDr, List<KeyValuePair<int, IOSolutionConfigModel>> columnMath)
		{
			return !(from _003C_003Eh__TransparentIdentifier0 in columnMath.Select(delegate(KeyValuePair<int, IOSolutionConfigModel> kv)
			{
				DataRow dataRow = srcDr;
				KeyValuePair<int, IOSolutionConfigModel> keyValuePair = kv;
				return new
				{
					kv = kv,
					value = dataRow[keyValuePair.Key]
				};
			})
			where _003C_003Eh__TransparentIdentifier0.kv.Value.MIsKey && _003C_003Eh__TransparentIdentifier0.value != DBNull.Value && _003C_003Eh__TransparentIdentifier0.value != null && _003C_003Eh__TransparentIdentifier0.value.ToString() != string.Empty
			select _003C_003Eh__TransparentIdentifier0.kv).Any();
		}
	}
}
