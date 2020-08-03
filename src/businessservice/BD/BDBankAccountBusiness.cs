using JieNor.Megi.BusinessContract.BD;
using JieNor.Megi.BusinessService.GL;
using JieNor.Megi.Common.Logger;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Helper;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.IO;
using JieNor.Megi.DataModel.SYS;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.DataRepository.IV;
using JieNor.Megi.DataRepository.SYS;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace JieNor.Megi.BusinessService.BD
{
	public class BDBankAccountBusiness : APIBusinessBase<BDBankAccountEditModel>, IBDBankAccountBusiness
	{
		private readonly BDBankAccountRepository dalBankAccount = new BDBankAccountRepository();

		protected override DataGridJson<BDBankAccountEditModel> OnGet(MContext ctx, GetParam param)
		{
			return dalBankAccount.Get(ctx, param);
		}

		public BDBankChartModel GetBankTotalChartModel(MContext ctx)
		{
			return BDBankAccountRepository.GetBankTotalChartModel(ctx);
		}

		public List<NameValueModel> GetSimpleBankAccountList(MContext ctx, string orgIds)
		{
			List<NameValueModel> list = new List<NameValueModel>();
			SYSStorageRepository sYSStorageRepository = new SYSStorageRepository();
			var enumerable = from f in sYSStorageRepository.GetOrgServerList(ctx, orgIds, false)
			group f by new
			{
				f.MDBServerName,
				f.MDBServerPort,
				f.MUserName,
				f.MPassWord,
				f.MStandardDBName
			};
			foreach (var item in enumerable)
			{
				List<string> orgIdList = (from f in item.ToList()
				select f.MOrgID).ToList();
				SYSStorageServerModel serverModel = new SYSStorageServerModel
				{
					MDBServerName = item.Key.MDBServerName,
					MDBServerPort = item.Key.MDBServerPort,
					MUserName = item.Key.MUserName,
					MPassWord = item.Key.MPassWord,
					MStandardDBName = item.Key.MStandardDBName
				};
				string serverConnectionString = sYSStorageRepository.GetServerConnectionString(serverModel);
				List<NameValueModel> simpleBankAccountList = BDBankAccountRepository.GetSimpleBankAccountList(ctx, orgIdList, serverConnectionString);
				list.AddRange(simpleBankAccountList);
			}
			return list;
		}

		public List<BDBankAccountEditModel> GetBDBankAccountEditList(MContext ctx, DateTime startDate, DateTime endDate, string[] accountIds, bool useBase = false, bool needSum = false, bool needChart = false)
		{
			//List<BDBankAccountEditModel> list = null;
			if (ctx.MOrgVersionID == 1)
			{
				return GetBankEditListFromGLBalance(ctx, accountIds, startDate, endDate);
			}
			return BDBankAccountRepository.GetBDBankAccountEditList(ctx, startDate, endDate, accountIds, useBase, needSum, needChart);
		}

		public List<BDBankAccountEditModel> GetBankAccountList(MContext ctx)
		{
			return BDBankAccountRepository.GetBankAccountList(ctx, null, false, null);
		}

		private List<BDBankAccountEditModel> GetBankEditListFromGLBalance(MContext ctx, string[] accountIds, DateTime startDateTime, DateTime endDateTime)
		{
			List<BDBankAccountEditModel> bankAccountList = BDBankAccountRepository.GetBankAccountList(ctx, accountIds, false, null);
			if (bankAccountList == null || bankAccountList.Count() == 0)
			{
				return null;
			}
			List<string> bankIdList = (from x in bankAccountList
			select x.MItemID).ToList();
			GLBalanceBusiness gLBalanceBusiness = new GLBalanceBusiness();
			DateTime dateTime = ctx.DateNow;
			dateTime = dateTime.AddMonths(-12);
			int startYearPeriod = int.Parse(dateTime.ToString("yyyyMM"));
			dateTime = ctx.DateNow;
			int endYearPeriod = int.Parse(dateTime.ToString("yyyyMM"));
			List<GLBalanceModel> bankAccountGLBalance = gLBalanceBusiness.GetBankAccountGLBalance(ctx, startYearPeriod, endYearPeriod, bankIdList);
			if (bankAccountList == null)
			{
				return bankAccountList;
			}
			List<NameValueModel> statementGroupInfo = IVBankBillEntryRepository.GetStatementGroupInfo(ctx, ctx.MBeginDate, endDateTime);
			foreach (BDBankAccountEditModel item in bankAccountList)
			{
				List<GLBalanceModel> list = (from x in bankAccountGLBalance
				where x.MAccountID == item.MItemID && x.MCheckGroupValueID == "0"
				select x).ToList();
				if (list != null && list.Count() > 0)
				{
					int lastYearPeriod = list.Max((GLBalanceModel x) => x.MYearPeriod);
					GLBalanceModel gLBalanceModel = (from x in list
					where x.MYearPeriod == lastYearPeriod
					select x).First();
					item.MMegiBalance = gLBalanceModel.MBeginBalanceFor + gLBalanceModel.MDebitFor - gLBalanceModel.MCreditFor;
					item.MLastUpdateDate = gLBalanceModel.MModifyDate;
					item.MBankChartInfo = GetBankChartModel(ctx, list);
				}
				if (statementGroupInfo != null && statementGroupInfo.Count > 0)
				{
					NameValueModel nameValueModel = statementGroupInfo.Find((NameValueModel x) => x.MName.Equals(item.MItemID));
					decimal mBankStatement = nameValueModel?.MValue.ToMDecimal() ?? decimal.Zero;
					item.MBankStatement = mBankStatement;
				}
			}
			return bankAccountList;
		}

		private ChartModel GetBankChartModel(MContext ctx, List<GLBalanceModel> bankBalanceList)
		{
			ChartModel chartModel = new ChartModel();
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			List<object> list3 = new List<object>();
			bankBalanceList = (from x in bankBalanceList
			orderby x.MYearPeriod
			select x).ToList();
			IEnumerable<IGrouping<int, GLBalanceModel>> enumerable = from x in bankBalanceList
			group x by x.MYearPeriod;
			int num = bankBalanceList.Min((GLBalanceModel x) => x.MYearPeriod);
			int num2 = enumerable.Count();
			int num3 = 1;
			foreach (IGrouping<int, GLBalanceModel> item in enumerable)
			{
				int key = item.Key;
				List<GLBalanceModel> source = item.ToList();
				DateTime dateTime = DateTime.ParseExact(key + "01", "yyyyMMdd", CultureInfo.CurrentCulture);
				decimal num4 = default(decimal);
				if (num3 != num2)
				{
					dateTime = dateTime.AddMonths(1).AddDays(-1.0);
					num4 = source.Sum((GLBalanceModel x) => x.MEndBalanceFor);
				}
				else
				{
					dateTime = source.First().MModifyDate;
					num4 = source.Sum((GLBalanceModel x) => x.MBeginBalanceFor) + source.Sum((GLBalanceModel x) => x.MDebitFor) - source.Sum((GLBalanceModel x) => x.MCreditFor);
				}
				list2.Add(dateTime.ToString("yyyy-MM-dd"));
				list3.Add(num4);
			}
			if (list2.Count() == 1)
			{
				DateTime dateTime2 = DateTime.ParseExact(num + "01", "yyyyMMdd", CultureInfo.CurrentCulture);
				decimal num5 = default(decimal);
				list2.Insert(0, dateTime2.ToString("yyyy-MM-dd"));
				list3.Insert(0, num5);
			}
			chartModel.MLabels = list.ToArray();
			chartModel.MValue = list3.ToArray();
			chartModel.MTipLabels = list2.ToArray();
			double maxAmt = list3.Max().ToMDouble();
			double minAmt = list3.Min().ToMDouble();
			chartModel.MScale = ChartHelper.GetChartScaleModel(maxAmt, minAmt);
			return chartModel;
		}

		public List<BDBankAccountEditModel> GetBankAccountList(MContext ctx, GetParam param)
		{
			return BDBankAccountRepository.GetBankAccountList(ctx, null, false, param);
		}

		public OperationResult DeleteBankbill(MContext ctx, string[] mids)
		{
			return new BDBankAccountRepository().DeleteBankbill(ctx, mids);
		}

		public BDBankAccountEditModel GetBDBankAccountEditModel(MContext ctx, string pkID)
		{
			BDBankAccountRepository bDBankAccountRepository = new BDBankAccountRepository();
			BDBankAccountEditModel bDBankAccountEditModel = bDBankAccountRepository.GetBDBankAccountEditModel(ctx, pkID);
			if (bDBankAccountEditModel.MBankAccountType == Convert.ToInt32(BankAccountTypeEnum.Bank))
			{
				bDBankAccountEditModel.MIsNeedReconcile = true;
			}
			if (bDBankAccountEditModel.MIsNeedReconcile)
			{
				bDBankAccountEditModel.MHasBankBillData = IVBankBillEntryRepository.HasBankBillData(ctx, pkID);
			}
			bDBankAccountEditModel.MBankIsUse = bDBankAccountRepository.IsBankAccountUsed(ctx, pkID);
			return bDBankAccountEditModel;
		}

		public List<BDBankAccountEditModel> GetBDBankDashboardData(MContext ctx, DateTime? startDate, DateTime? endDate)
		{
			return new BDBankAccountRepository().GetBDBankDashboardData(ctx, startDate, endDate);
		}

		public OperationResult UpdateBankAccount(MContext ctx, BDBankAccountEditModel model)
		{
			OperationResult operationResult = new OperationResult();
			BDBankAccountRepository bDBankAccountRepository = new BDBankAccountRepository();
			if (model.MIsCheckExists)
			{
				List<MultiLanguageFieldList> langList = (from t in model.MultiLanguage
				where t.MFieldName == "MName"
				select t).ToList();
				if (ModelInfoManager.IsLangColumnValueExists<BDBankAccountEditModel>(ctx, "MName", langList, model.MItemID, "", "", false))
				{
					operationResult.Success = false;
					string obj = (model.MBankTypeID == "Cash") ? COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "CashAccountExists", "现金账号已经存在，请使用其他名称。") : COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "BankAccountExists", "The bank account is aready exists. please enter a different name");
					string text2 = operationResult.Message = obj;
					return operationResult;
				}
			}
			if (model.MBankAccountType == Convert.ToInt32(BankAccountTypeEnum.Bank))
			{
				model.MIsNeedReconcile = true;
			}

			string text3 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "DataHasBeenChanged", "Data has been changed！");
			bool flag = false;
			string currency = "";
			if (!string.IsNullOrEmpty(model.MItemID))
			{
				OperationResult operationResult2 = BDRepository.IsCanDelete(ctx, "BankAccount", model.MItemID);
				BDBankAccountEditModel bDBankAccountEditModel = bDBankAccountRepository.GetBDBankAccountEditModel(ctx, model.MItemID);
				flag = (bDBankAccountEditModel.MCyID != model.MCyID);
				currency = bDBankAccountEditModel.MCyID;
				if (flag && !operationResult2.Success)
				{
					string message = string.Format("{0},{1}", operationResult2.Message, COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "NotAllowChangeCurrency", "不允许修改币别！"));
					operationResult.Success = false;
					operationResult.Message = message;
					return operationResult;
				}
			}
			if (!model.MIsNeedReconcile && IVBankBillEntryRepository.HasBankBillData(ctx, model.MItemID))
			{
				operationResult.Success = false;
				operationResult.Message = text3;
				return operationResult;
			}
			if (ctx.MRegProgress > 11 && ctx.MAccountTableID == "3")
			{
				BDAccountBusiness bDAccountBusiness = new BDAccountBusiness();
				operationResult = bDAccountBusiness.CheckCustomAccountIsFinish(ctx);
				if (!operationResult.Success)
				{
					operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "GLAccountNotFind", "没有找到任何总账会计科目，请先完成科目的生成！");
					return operationResult;
				}
				operationResult = bDAccountBusiness.CheckCustomAccountIsMatch(ctx);
				if (!operationResult.Success)
				{
					return operationResult;
				}
			}

			MLogger.Log($"ctx.MRegProgress:{ctx.MRegProgress}");
			bool flag2 = ctx.MRegProgress == 15;
			BDAccountRepository bDAccountRepository = new BDAccountRepository();
			BDBankAccountRepository bDBankAccountRepository2 = new BDBankAccountRepository();
			BDAccountEditModel bDAccountEditModel = bDAccountRepository.GetBDAccountEditModel(ctx, model.MAccountID);
			model.MOrgID = ctx.MOrgID;

			MLogger.Log($"model.MBankAccountType:{model.MBankAccountType}");
			BDAccountModel bDAccountModel = null;
			SqlWhere sqlWhere = new SqlWhere();
			if (model.MBankAccountType == 1 || model.MBankAccountType == 2 || model.MBankAccountType == 5 || model.MBankAccountType == 4)
			{
				sqlWhere.Equal("MCode", "1002");
				bDAccountModel = bDAccountRepository.GetDataModelByFilter(ctx, sqlWhere);
			}
			else if (model.MBankAccountType == 3)
			{
				sqlWhere.Equal("MCode", "1001");
				bDAccountModel = bDAccountRepository.GetDataModelByFilter(ctx, sqlWhere);
			}

			string text4 = string.IsNullOrEmpty(model.MItemID) ? UUIDHelper.GetGuid() : model.MItemID;
			if (bDAccountModel != null)
			{
				string accountIncreasingNumber = bDAccountRepository.GetAccountIncreasingNumber(ctx, bDAccountModel.MItemID);
				bDAccountEditModel.MParentID = bDAccountModel.MItemID;
				bDAccountEditModel.MNumber = (string.IsNullOrWhiteSpace(bDAccountEditModel.MNumber) ? (bDAccountModel.MNumber + "." + accountIncreasingNumber) : bDAccountEditModel.MNumber);
				bDAccountEditModel.MDC = bDAccountModel.MDC;
				bDAccountEditModel.MName = model.MBankName;
				bDAccountEditModel.MultiLanguage = GetAccountMulitLang(model.MultiLanguage, bDAccountEditModel.MultiLanguage);
				bDAccountEditModel.MAccountGroupID = bDAccountModel.MAccountGroupID;
				bDAccountEditModel.MAccountTypeID = bDAccountModel.MAccountTypeID;
				bDAccountEditModel.MAccountTableID = bDAccountModel.MAccountTableID;
				bDAccountEditModel.MOrgID = ctx.MOrgID;
				bDAccountEditModel.MIsCheckForCurrency = (model.MCyID != ctx.MBasCurrencyID);
				string accountCodeIncreaseNumber = bDAccountRepository.GetAccountCodeIncreaseNumber(ctx, bDAccountModel.MItemID, bDAccountModel.MCode);
				bDAccountEditModel.MCheckGroupID = ((int.Parse(accountCodeIncreaseNumber) > 1) ? "0" : bDAccountModel.MCheckGroupID);
				if (string.IsNullOrEmpty(bDAccountEditModel.MItemID))
				{
					bDAccountEditModel.MItemID = text4;
					bDAccountEditModel.IsNew = true;
					bDAccountEditModel.MCode = bDAccountModel.MCode + accountCodeIncreaseNumber;
				}
				flag2 = true;
			}
			

			if (string.IsNullOrEmpty(model.MItemID))
			{
				model.MItemID = text4;
				model.IsNew = true;
			}

			if (flag2)
			{
				List<CommandInfo> list = new List<CommandInfo>();
				if (!bDAccountModel.MIsCheckForCurrency && bDAccountEditModel.MIsCheckForCurrency)
				{
					List<string> list2 = new List<string>();
					list2.Add("MIsCheckForCurrency");
					bDAccountModel.MIsCheckForCurrency = true;
					list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDAccountModel>(ctx, bDAccountModel, list2, true));
				}
				BDAccountBusiness bDAccountBusiness2 = new BDAccountBusiness();
				string text5 = "";

				List<CommandInfo> updateAccountCmds = bDAccountBusiness2.GetUpdateAccountCmds(ctx, bDAccountEditModel, out text5, null, null, null);
				if (string.IsNullOrWhiteSpace(text5))
				{
					list.AddRange(updateAccountCmds);
					model.MAccountID = text4;
					list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDBankAccountEditModel>(ctx, model, null, true));

					if (flag)
					{
						list.AddRange(GetUpdateBankAccountInitBalanceCmds(ctx, model.MItemID, bDAccountModel.MItemID, currency));
					}
					list.AddRange(bDAccountBusiness2.GetUpdateAccountFullNameCmds(ctx, bDAccountEditModel, bDAccountModel, null));
					int num = BDRepository.ExecuteSqlTran(ctx, list);
					operationResult.Success = (num > 0);
					if (!operationResult.Success)
					{
						goto IL_0631;
					}
					goto IL_0631;
				}
				operationResult.Success = false;
				operationResult.Message = text5;
				return operationResult;
			}

			
			operationResult = bDBankAccountRepository2.InsertOrUpdate(ctx, model, null);
			goto IL_0631;
			IL_0631:
			return operationResult;
		}

		private List<CommandInfo> GetUpdateBankAccountInitBalanceCmds(MContext ctx, string accountId, string parentAccountId, string currency)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			if (string.IsNullOrWhiteSpace(accountId) || string.IsNullOrWhiteSpace(parentAccountId) || string.IsNullOrWhiteSpace(currency))
			{
				return list;
			}
			GLInitBalanceRepository gLInitBalanceRepository = new GLInitBalanceRepository();
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.Equal("a.MAccountID", accountId);
			sqlWhere.Equal("a.MCurrencyID", currency);
			sqlWhere.Equal("a.MIsDelete", 0);
			sqlWhere.Equal("a.MOrgID", ctx.MOrgID);
			GLInitBalanceModel initBalanceModel = gLInitBalanceRepository.GetInitBalanceModel(ctx, sqlWhere);
			if (initBalanceModel != null)
			{
				List<string> list2 = new List<string>();
				list2.Add("MIsDelete");
				initBalanceModel.MIsDelete = true;
				list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<GLInitBalanceModel>(ctx, initBalanceModel, list2, true));
				SqlWhere sqlWhere2 = new SqlWhere();
				sqlWhere2.Equal("a.MAccountID", parentAccountId);
				sqlWhere.Equal("a.MCurrencyID", currency);
				sqlWhere.Equal("a.MIsDelete", 0);
				sqlWhere.Equal("a.MOrgID", ctx.MOrgID);
				GLInitBalanceModel initBalanceModel2 = gLInitBalanceRepository.GetInitBalanceModel(ctx, sqlWhere2);
				if (initBalanceModel2 != null && Math.Abs(initBalanceModel2.MInitBalance) + Math.Abs(initBalanceModel2.MInitBalanceFor) + Math.Abs(initBalanceModel2.MYtdCredit) + Math.Abs(initBalanceModel2.MYtdCreditFor) + Math.Abs(initBalanceModel2.MYtdDebit) + Math.Abs(initBalanceModel2.MYtdDebitFor) == decimal.Zero)
				{
					initBalanceModel2.MIsDelete = true;
					list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<GLInitBalanceModel>(ctx, initBalanceModel2, list2, true));
				}
			}
			return list;
		}

		public static List<MultiLanguageFieldList> GetAccountMulitLang(List<MultiLanguageFieldList> bankAccountMulitLang, List<MultiLanguageFieldList> accountMulitLang)
		{
			if (accountMulitLang == null)
			{
				return bankAccountMulitLang;
			}
			MultiLanguageFieldList multiLanguageFieldList = bankAccountMulitLang.First();
			if (multiLanguageFieldList.MMultiLanguageField != null && multiLanguageFieldList.MMultiLanguageField.Count() > 0)
			{
				MultiLanguageFieldList multiLanguageFieldList2 = accountMulitLang.FirstOrDefault();
				if (multiLanguageFieldList2 != null)
				{
					foreach (MultiLanguageField item in multiLanguageFieldList2.MMultiLanguageField)
					{
						MultiLanguageField multiLanguageField = (from x in multiLanguageFieldList.MMultiLanguageField
						where x.MLocaleID == item.MLocaleID
						select x).FirstOrDefault();
						if (multiLanguageField != null)
						{
							item.MValue = multiLanguageField.MValue;
						}
					}
				}
				else
				{
					List<MultiLanguageFieldList> list = new List<MultiLanguageFieldList>();
					MultiLanguageFieldList multiLanguageFieldList3 = new MultiLanguageFieldList();
					multiLanguageFieldList3.MFieldName = multiLanguageFieldList.MFieldName;
					multiLanguageFieldList3.MMultiLanguageValue = multiLanguageFieldList.MMultiLanguageValue;
					foreach (MultiLanguageField item2 in multiLanguageFieldList.MMultiLanguageField)
					{
						MultiLanguageField multiLanguageField2 = new MultiLanguageField();
						multiLanguageFieldList3.CreateMultiLanguageFieldValue(item2.MLocaleID, item2.MValue, null);
					}
					list.Add(multiLanguageFieldList3);
					accountMulitLang = list;
				}
			}
			return accountMulitLang;
		}

		public List<BDBankAccountListModel> GetBDBankAccountList(MContext ctx)
		{
			BDBankAccountRepository bDBankAccountRepository = new BDBankAccountRepository();
			return bDBankAccountRepository.GetBDBankAccountList(ctx);
		}

		public DataGridJson<BDBankInitBalanceModel> GetInitBalanceListByPage(MContext ctx, SqlWhere filter)
		{
			return BDBankAccountRepository.GetInitBalanceListByPage(ctx, filter);
		}

		public BDBankInitBalanceModel GetBDBankInitBalance(MContext ctx, string bankId)
		{
			return BDBankAccountRepository.GetBankInitBalanceByBankId(ctx, bankId);
		}

		public OperationResult UpdateBankInitBalance(MContext ctx, BDBankInitBalanceModel model)
		{
			OperationResult operationResult = new OperationResult();
			if (ctx.MInitBalanceOver)
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "InitBalanceIsOver", "Initialization has been completed,  it is not allowed to operate any record again!");
				return operationResult;
			}
			model.MOrgID = ctx.MOrgID;
			List<CommandInfo> list = new List<CommandInfo>();
			list.Add(BDBankAccountRepository.GetUpdateBankInitBalanceCmd(ctx, model));
			int num = BDRepository.ExecuteSqlTran(ctx, list);
			operationResult.Success = (num > 0);
			return operationResult;
		}

		public void CheckBankAccountExist<T>(MContext ctx, List<T> entryList, string bankFieldName, List<IOValidationResultModel> validationResult)
		{
			List<string> source = (from f in entryList
			where !string.IsNullOrWhiteSpace(ModelHelper.GetModelValue<T>(f, bankFieldName))
			select ModelHelper.GetModelValue<T>(f, bankFieldName).Trim()).Distinct().ToList();
			if (source.Any())
			{
				List<BDBankAccountEditModel> bankAccountList = GetBankAccountList(ctx);
				foreach (T entry in entryList)
				{
					string bankName = ModelHelper.GetModelValue(entry, bankFieldName);
					if (!string.IsNullOrWhiteSpace(bankName))
					{
						BDBankAccountEditModel bDBankAccountEditModel = bankAccountList.FirstOrDefault((BDBankAccountEditModel f) => !string.IsNullOrWhiteSpace(f.MBankName) && HttpUtility.HtmlDecode(f.MBankName.ToUpper().Trim()) == bankName.ToUpper());
						if (bDBankAccountEditModel != null)
						{
							ModelHelper.SetModelValue(entry, bankFieldName, bDBankAccountEditModel.MItemID, null);
						}
						else
						{
							int rowIndex = 0;
							int.TryParse(ModelHelper.TryGetModelValue(entry, "MRowIndex"), out rowIndex);
							validationResult.Add(new IOValidationResultModel
							{
								FieldType = IOValidationTypeEnum.BankAccount,
								FieldValue = bankName,
								Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "BankAccountNotFound", "The Bank Account:{0} can't be found!"),
								RowIndex = rowIndex
							});
						}
					}
				}
			}
		}

		public List<CommandInfo> GetDeleteBankInitBalanceCmds(MContext ctx, List<string> bankIdList)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			BDBankAccountRepository bDBankAccountRepository = new BDBankAccountRepository();
			return bDBankAccountRepository.GetDeleteBankInitBalanceCmds(ctx, bankIdList);
		}
	}
}
