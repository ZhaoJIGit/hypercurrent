using JieNor.Megi.BusinessContract.BAS;
using JieNor.Megi.BusinessContract.GL;
using JieNor.Megi.BusinessService.BAS;
using JieNor.Megi.BusinessService.BD;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataMode;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.COM;
using JieNor.Megi.DataModel.Enum;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.IV.Expense;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.DataRepository.BAS;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.FP;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.DataRepository.IV;
using JieNor.Megi.DataRepository.REG;
using JieNor.Megi.EntityModel.BD.AccountItem;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace JieNor.Megi.BusinessService.GL
{
	public class GLInitBalanceBusiness : IGLInitBalanceBusiness, IDataContract<GLInitBalanceModel>
	{
		private readonly GLInitBalanceRepository dal = new GLInitBalanceRepository();

		private IBASLangBusiness LanguageBLL = new BASLangBusiness();

		private REGCurrencyRepository CurrencyBLL = new REGCurrencyRepository();

		private BDEmployeesBusiness EmployeeBLL = new BDEmployeesBusiness();

		private List<BASLangModel> LanguageList = new List<BASLangModel>();

		private List<ImportTemplateDataSource> GloablColumnDatasource = new List<ImportTemplateDataSource>();

		private List<BDAccountModel> GlobalAccountList = new List<BDAccountModel>();

		private GLUtility GLUnility = new GLUtility();

		private GLCheckTypeBusiness CheckTypeBLL = new GLCheckTypeBusiness();

		private List<BDContactsInfoModel> ContactList = new List<BDContactsInfoModel>();

		private List<GLTreeModel> EmployeeList = new List<GLTreeModel>();

		private List<GLTreeModel> MerItemList = new List<GLTreeModel>();

		private List<GLTreeModel> ExpenseItemList = new List<GLTreeModel>();

		private List<GLTreeModel> TrackList = new List<GLTreeModel>();

		private List<GLTreeModel> PaItemList = new List<GLTreeModel>();

		private List<REGCurrencyViewModel> CurrencyList = new List<REGCurrencyViewModel>();

		private List<IOTemplateConfigModel> commonColumnModelList = null;

		private List<NameValueModel> TrackNameList = null;

		private Dictionary<int, string> CheckTypeNameDic = new Dictionary<int, string>();

		private List<BDBankAccountModel> BankList = new List<BDBankAccountModel>();

		public GLInitBalanceModel GetBankInitBalance(MContext ctx, string accountId, string bankName)
		{
			return dal.GetBankInitBalance(ctx, accountId, bankName);
		}

		private List<GLInitBalanceModel> GetInitBalanceFromBank(MContext ctx)
		{
			BDBankAccountRepository bDBankAccountRepository = new BDBankAccountRepository();
			List<BDBankAccountEditModel> modelList = bDBankAccountRepository.GetModelList(ctx, new SqlWhere(), false);
			List<string> list = (from x in modelList
			select x.MItemID).ToList();
			DateTime dateTime = ctx.MBeginDate;
			DateTime dateTime2 = ctx.MGLBeginDate;
			DateTime dateTime3;
			if (dateTime > dateTime2)
			{
				dateTime3 = new DateTime(dateTime2.Year, dateTime2.Month, 1);
				dateTime2 = dateTime3.AddDays(-1.0);
				dateTime = dateTime2;
			}
			else
			{
				dateTime3 = new DateTime(dateTime2.Year, dateTime2.Month, 1);
				dateTime2 = dateTime3.AddDays(-1.0);
			}
			List<BDBankBalanceModel> bankInitBalanceList = BDBankAccountRepository.GetBankInitBalanceList(ctx, dateTime, dateTime2, null);
			BDAccountRepository bDAccountRepository = new BDAccountRepository();
			SqlWhere filter = new SqlWhere();
			List<BDAccountModel> baseBDAccountList = bDAccountRepository.GetBaseBDAccountList(ctx, filter, false, null);
			BDAccountModel bDAccountModel = null;
			BDAccountModel bDAccountModel2 = null;
			if (baseBDAccountList != null && baseBDAccountList.Count() > 0)
			{
				bDAccountModel = (from x in baseBDAccountList
				where x.MCode == "1001"
				select x).FirstOrDefault();
				bDAccountModel2 = (from x in baseBDAccountList
				where x.MCode == "1002"
				select x).FirstOrDefault();
			}
			List<string> list2 = new List<string>();
			list2.AddRange((from x in bankInitBalanceList
			select x.MBankID).ToList());
			list2.Add(bDAccountModel.MItemID);
			list2.Add(bDAccountModel2.MItemID);
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.In("MAccountID", list2.ToArray());
			sqlWhere.Equal("MIsDelete", 0);
			dal.DeleteInitBalanceByFilter(ctx, sqlWhere);
			List<GLInitBalanceModel> list3 = new List<GLInitBalanceModel>();
			SqlWhere sqlWhere2 = new SqlWhere();
			if (bDAccountModel != null || bDAccountModel2 != null)
			{
				sqlWhere2.In("MAccountID", new string[2]
				{
					bDAccountModel.MItemID,
					bDAccountModel2.MItemID
				});
			}
			GLUtility gLUtility = new GLUtility();
			foreach (BDBankBalanceModel item in bankInitBalanceList)
			{
				BDAccountModel bDAccountModel3 = (from x in baseBDAccountList
				where x.MItemID == item.MBankID
				select x).FirstOrDefault();
				if (bDAccountModel3 != null)
				{
					BDBankAccountEditModel bDBankAccountEditModel = (from x in modelList
					where x.MItemID == item.MBankID
					select x).FirstOrDefault();
					GLInitBalanceModel gLInitBalanceModel = new GLInitBalanceModel();
					gLInitBalanceModel.MAccountID = bDAccountModel3.MItemID;
					gLInitBalanceModel.MOrgID = ctx.MOrgID;
					gLInitBalanceModel.MCurrencyID = bDBankAccountEditModel.MCyID;
					gLInitBalanceModel.MInitBalanceFor = item.MTotalAmtFor;
					gLInitBalanceModel.MInitBalance = item.MTotalAmt;
					gLInitBalanceModel.IsNew = true;
					gLInitBalanceModel.MCheckGroupValueModel = new GLCheckGroupValueModel();
					GLCheckGroupValueModel checkGroupValueModel = gLUtility.GetCheckGroupValueModel(ctx, gLInitBalanceModel.MCheckGroupValueModel);
					gLInitBalanceModel.MCheckGroupValueID = checkGroupValueModel.MItemID;
					list3.Add(gLInitBalanceModel);
				}
			}
			return list3;
		}

		public OperationResult UpdateInitBalanceFromBankInitBalance(MContext ctx)
		{
			OperationResult operationResult = new OperationResult();
			List<GLInitBalanceModel> initBalanceFromBank = GetInitBalanceFromBank(ctx);
			return UpdateInitBalance(ctx, initBalanceFromBank);
		}

		public OperationResult ClearInitBalance(MContext ctx, string initBalanceId = null)
		{
			try
			{
				OperationResult operationResult = new OperationResult();
				BASOrganisationRepository bASOrganisationRepository = new BASOrganisationRepository();
				ctx.IsSys = true;
				BASOrganisationModel dataModel = bASOrganisationRepository.GetDataModel(ctx, ctx.MOrgID, false);
				ctx.IsSys = false;
				if (ctx.MInitBalanceOver || dataModel.MInitBalanceOver)
				{
					operationResult.Success = false;
					operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "NotAllowClearInitBalance", "科目期初余额已完成，不允许清空清除余额数据，如果您要清除，请先反初始化！");
					return operationResult;
				}
				List<CommandInfo> clearInitBalanceCmds = GetClearInitBalanceCmds(ctx, initBalanceId);
				if (clearInitBalanceCmds == null || clearInitBalanceCmds.Count() == 0)
				{
					operationResult.Success = false;
					operationResult.Message = ((!string.IsNullOrWhiteSpace(initBalanceId)) ? COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "NotFindInitBalance", "没有找到该条期初余额记录!") : COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "NotAnyInitBalance", "期初余额表为空!"));
					return operationResult;
				}
				operationResult.Success = (BDRepository.ExecuteSqlTran(ctx, clearInitBalanceCmds) > 0);
				return operationResult;
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				ctx.IsSys = false;
			}
		}

		private List<CommandInfo> GetClearInitBalanceCmds(MContext ctx, string initBalanceId = null)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.Equal("MIsDelete", 0);
			bool flag = !string.IsNullOrWhiteSpace(initBalanceId);
			if (flag)
			{
				sqlWhere.Equal("MItemID", initBalanceId);
			}
			List<GLInitBalanceModel> modelList = dal.GetModelList(ctx, sqlWhere, false);
			if (modelList == null || modelList.Count() == 0)
			{
				return list;
			}
			if (flag)
			{
				list = GetDeleteInitBalanceCmds(ctx, modelList.First());
			}
			else
			{
				CommandInfo deleteInitBalanceCmds = dal.GetDeleteInitBalanceCmds(ctx, sqlWhere);
				list.Add(deleteInitBalanceCmds);
				List<string> list2 = new List<string>();
				List<CommandInfo> deleteInitBillCmds = GetDeleteInitBillCmds(ctx, out list2);
				if (list2 != null && list2.Count() > 0)
				{
					List<IVVerificationModel> verificationRecordList = IVVerificationRepository.GetVerificationRecordList(ctx, list2);
					List<CommandInfo> deleteVerificationCmdList = IVVerificationRepository.GetDeleteVerificationCmdList(ctx, verificationRecordList);
					if (deleteVerificationCmdList != null)
					{
						list.AddRange(deleteVerificationCmdList);
					}
					foreach (IVVerificationModel item in verificationRecordList)
					{
						list.AddRange(IVVerificationRepository.GetCalculateBillVerificationAmountCmd(ctx, item, true));
					}
				}
				list.AddRange(deleteInitBillCmds);
			}
			return list;
		}

		private List<CommandInfo> GetDeleteInitBillCmds(MContext ctx, List<string> initBillIdList)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			ParamBase paramBase = new ParamBase();
			paramBase.KeyIDs = string.Join(",", initBillIdList);
			List<CommandInfo> deleteBillCmd = IVBaseRepository<IVInvoiceModel>.GetDeleteBillCmd<IVInvoiceModel>(ctx, paramBase, null);
			if (deleteBillCmd != null)
			{
				list.AddRange(deleteBillCmd);
			}
			List<CommandInfo> deleteBillCmd2 = IVBaseRepository<IVInvoiceModel>.GetDeleteBillCmd<IVReceiveModel>(ctx, paramBase, null);
			if (deleteBillCmd2 != null)
			{
				list.AddRange(deleteBillCmd2);
			}
			List<CommandInfo> deleteBillCmd3 = IVBaseRepository<IVInvoiceModel>.GetDeleteBillCmd<IVPaymentModel>(ctx, paramBase, null);
			if (deleteBillCmd3 != null)
			{
				list.AddRange(deleteBillCmd3);
			}
			List<CommandInfo> deleteBillCmd4 = IVBaseRepository<IVInvoiceModel>.GetDeleteBillCmd<IVExpenseModel>(ctx, paramBase, null);
			if (deleteBillCmd4 != null)
			{
				list.AddRange(deleteBillCmd4);
			}
			return list;
		}

		private List<CommandInfo> GetDeleteInitBillCmds(MContext ctx, out List<string> deleteBillIdList)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			deleteBillIdList = new List<string>();
			List<IVInvoiceModel> initInvoiceList = IVInvoiceRepository.GetInitInvoiceList(ctx);
			List<string> list2 = (from x in initInvoiceList
			select x.MID).ToList();
			if (initInvoiceList != null && initInvoiceList.Count() > 0)
			{
				foreach (IVInvoiceModel item in initInvoiceList)
				{
					string mID = item.MID;
					list.AddRange(ModelInfoManager.GetDeleteFlagCmd<IVInvoiceModel>(ctx, mID));
					deleteBillIdList.Add(mID);
				}
			}
			List<IVPaymentModel> initList = IVPaymentRepository.GetInitList(ctx);
			if (initList != null && initList.Count() > 0)
			{
				foreach (IVPaymentModel item2 in initList)
				{
					string mID2 = item2.MID;
					list.AddRange(ModelInfoManager.GetDeleteFlagCmd<IVPaymentModel>(ctx, mID2));
					deleteBillIdList.Add(mID2);
				}
			}
			List<IVReceiveModel> initList2 = IVReceiveRepository.GetInitList(ctx);
			if (initList2 != null && initList2.Count() > 0)
			{
				foreach (IVReceiveModel item3 in initList2)
				{
					string mID3 = item3.MID;
					list.AddRange(ModelInfoManager.GetDeleteFlagCmd<IVReceiveModel>(ctx, mID3));
					deleteBillIdList.Add(mID3);
				}
			}
			List<IVExpenseModel> initList3 = IVExpenseRepository.GetInitList(ctx);
			if (initList3 != null && initList3.Count() > 0)
			{
				foreach (IVExpenseModel item4 in initList3)
				{
					string mID4 = item4.MID;
					list.AddRange(ModelInfoManager.GetDeleteFlagCmd<IVExpenseModel>(ctx, mID4));
					deleteBillIdList.Add(mID4);
				}
			}
			return list;
		}

		public List<CommandInfo> GetDeleteInitBalanceCmds(MContext ctx, GLInitBalanceModel initBalance)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			if (initBalance == null)
			{
				return list;
			}
			BDAccountRepository bDAccountRepository = new BDAccountRepository();
			List<BDAccountModel> baseBDAccountList = bDAccountRepository.GetBaseBDAccountList(ctx, new SqlWhere(), false, null);
			if (baseBDAccountList == null || baseBDAccountList.Count() == 0)
			{
				return list;
			}
			BDAccountModel bDAccountModel = (from x in baseBDAccountList
			where x.MItemID == initBalance.MAccountID
			select x).FirstOrDefault();
			if (bDAccountModel == null)
			{
				return list;
			}
			initBalance.OldCheckGroupValueID = initBalance.MCheckGroupValueID;
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.Equal("MCurrencyID", initBalance.MCurrencyID);
			List<string> list2 = new List<string>();
			list2.Add(bDAccountModel.MItemID);
			List<BDAccountModel> parentAccountByRecursion = BDAccountHelper.GetParentAccountByRecursion(bDAccountModel, baseBDAccountList);
			if (parentAccountByRecursion != null)
			{
				list2.AddRange((from x in parentAccountByRecursion
				select x.MItemID).ToList());
				sqlWhere.In("MAccountID", list2);
			}
			List<GLInitBalanceModel> initBalanceListIncludeCheckGroupValue = dal.GetInitBalanceListIncludeCheckGroupValue(ctx, sqlWhere);
			List<GLInitBalanceModel> list3 = new List<GLInitBalanceModel>();
			if (parentAccountByRecursion != null)
			{
				initBalance.MInitBalance = decimal.Zero;
				initBalance.MInitBalanceFor = decimal.Zero;
				initBalance.MYtdCredit = decimal.Zero;
				initBalance.MYtdCreditFor = decimal.Zero;
				initBalance.MYtdDebit = decimal.Zero;
				initBalance.MYtdDebitFor = decimal.Zero;
				foreach (BDAccountModel item in parentAccountByRecursion)
				{
					int balanceDirection = (item.MDC == bDAccountModel.MDC) ? 1 : (-1);
					ProcessUpdateInitBalanceList(ctx, initBalance, initBalanceListIncludeCheckGroupValue, list3, item.MItemID, balanceDirection);
				}
				ProcessUpdateInitBalanceList(ctx, initBalance, initBalanceListIncludeCheckGroupValue, list3, bDAccountModel.MItemID, 1);
				BDInitDocumentBusiness bDInitDocumentBusiness = new BDInitDocumentBusiness();
				foreach (GLInitBalanceModel item2 in list3)
				{
					decimal d = Math.Abs(item2.MInitBalance) + Math.Abs(item2.MInitBalanceFor) + Math.Abs(item2.MYtdCredit) + Math.Abs(item2.MYtdCreditFor) + Math.Abs(item2.MYtdDebit) + Math.Abs(item2.MYtdDebitFor);
					if (d == decimal.Zero)
					{
						item2.MIsDelete = true;
					}
					if (ctx.MOrgVersionID == 0 && !string.IsNullOrWhiteSpace(initBalance.MBillID))
					{
						list.AddRange(bDInitDocumentBusiness.GetDeleteInitDocCmds(ctx, item2));
					}
				}
			}
			list.AddRange(ModelInfoManager.GetInsertOrUpdateCmds(ctx, list3, null, true));
			if (BDAccountHelper.IsBankAccount(bDAccountModel.MCode))
			{
				BDBankAccountRepository bDBankAccountRepository = new BDBankAccountRepository();
				list.AddRange(bDBankAccountRepository.GetDeleteBankInitBalanceCmds(ctx, new List<string>
				{
					bDAccountModel.MItemID
				}));
			}
			return list;
		}

		public List<AccountInitBalanceTreeModel> GetInitBalanceTreeList(MContext ctx, SqlWhere filter = null)
		{
			List<AccountItemTreeModel> accountItemTreeList = new BDAccountBusiness().GetAccountItemTreeList(ctx, null);
			BASCurrencyViewModel @base = new REGCurrencyRepository().GetBase(ctx, false, null, null);
			List<GLInitBalanceModel> initBalanceList = BDAccountRepository.GetInitBalanceList(ctx, filter);
			List<GLInitBalanceModel> balanceSubList = (from x in initBalanceList
			where x.MCheckGroupValueID != "0"
			select x).ToList();
			List<AccountInitBalanceTreeModel> list = new List<AccountInitBalanceTreeModel>();
			foreach (AccountItemTreeModel item2 in accountItemTreeList)
			{
				AccountInitBalanceTreeModel item = FindAccountBalanceChildrenItem(item2, initBalanceList, balanceSubList);
				list.Add(item);
			}
			return list;
		}

		public AccountInitBalanceTreeModel FindAccountBalanceChildrenItem(AccountItemTreeModel parentModel, List<GLInitBalanceModel> balanceList, List<GLInitBalanceModel> balanceSubList)
		{
			AccountInitBalanceTreeModel model = new AccountInitBalanceTreeModel();
			model.id = parentModel.id;
			model.text = parentModel.text;
			model.MNumber = parentModel.MNumber;
			model.MDC = parentModel.MDC;
			model.MCode = parentModel.MCode;
			model.MAccountGroupID = parentModel.MAccountGroupID;
			model.children = new List<AccountInitBalanceTreeModel>();
			model.MIsCheckForCurrency = parentModel.MIsCheckForCurrency;
			List<GLInitBalanceModel> list = (from x in balanceList
			where x.MAccountID == model.id && x.MCheckGroupValueID == "0"
			select x).ToList();
			model.Balances = ((model.Balances == null) ? new List<InitBalanceModel>() : model.Balances);
			if (list != null)
			{
				foreach (GLInitBalanceModel item in list)
				{
					InitBalanceModel initBalanceModel = new InitBalanceModel();
					initBalanceModel.MItemID = item.MItemID;
					initBalanceModel.MCurrencyID = item.MCurrencyID;
					initBalanceModel.MInitBalance = item.MInitBalance;
					initBalanceModel.MInitBalanceFor = item.MInitBalanceFor;
					initBalanceModel.MYtdCredit = item.MYtdCredit;
					initBalanceModel.MYtdCreditFor = item.MYtdCreditFor;
					initBalanceModel.MYtdDebit = item.MYtdDebit;
					initBalanceModel.MYtdDebitFor = item.MYtdDebitFor;
					model.Balances.Add(initBalanceModel);
				}
			}
			if (parentModel.children != null && parentModel.children.Count() > 0)
			{
				foreach (AccountItemTreeModel child in parentModel.children)
				{
					model.children.Add(FindAccountBalanceChildrenItem(child, balanceList, balanceSubList));
				}
			}
			return model;
		}

		public List<ImportTemplateModel> GetImportTemplateModel(MContext ctx)
		{
			List<ImportTemplateModel> list = new List<ImportTemplateModel>();
			BDAccountRepository bDAccountRepository = new BDAccountRepository();
			List<BDAccountModel> accountListWithCheckType = bDAccountRepository.GetAccountListWithCheckType(ctx, null, false, false);
			if (accountListWithCheckType == null || accountListWithCheckType.Count() == 0)
			{
				return list;
			}
			accountListWithCheckType = (from x in accountListWithCheckType
			where x.MCheckGroupID != "0"
			select x).ToList();
			foreach (BDAccountModel item in accountListWithCheckType)
			{
				ImportTemplateModel accountingDimensionTemplateModel = GetAccountingDimensionTemplateModel(ctx, item);
				if (accountingDimensionTemplateModel != null)
				{
					list.Add(accountingDimensionTemplateModel);
				}
			}
			ImportTemplateModel mainImportTemlateModel = GetMainImportTemlateModel(ctx, accountListWithCheckType);
			list.Insert(0, mainImportTemlateModel);
			return list;
		}

		private ImportTemplateModel GetMainImportTemlateModel(MContext ctx, List<BDAccountModel> accountList)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			List<IOTemplateConfigModel> templateConfig = GetTemplateConfig(ctx, true, null);
			foreach (IOTemplateConfigModel item in templateConfig)
			{
				dictionary.Add(item.MFieldName, item.MLangList.FirstOrDefault((KeyValuePair<string, string> f) => f.Key == ctx.MLCID).Value);
			}
			Dictionary<string, string[]> dictionary2 = new Dictionary<string, string[]>();
			GetTemplateBasicData(ctx, dictionary, null, dictionary2);
			dictionary2.Add("MNumber", new string[1]
			{
				"1601.01"
			});
			dictionary2.Add("MName", new string[1]
			{
				COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "FixedAssetsOfficeEquipment", "固定资产 - 办公设备")
			});
			Dictionary<string, int> dictionary3 = new Dictionary<string, int>();
			dictionary3.Add("MNumber", 20);
			dictionary3.Add("MName", 20);
			dictionary3.Add("MCurrencyID", 10);
			return new ImportTemplateModel
			{
				TemplateType = "OpeningBalance",
				LocaleID = ctx.MLCID,
				ColumnList = dictionary,
				FieldConfigList = templateConfig,
				RequiredColumnList = (from f in templateConfig
				where f.MIsRequired
				select f.MFieldName).ToList(),
				TemplateDictionaryList = GloablColumnDatasource,
				ExampleDataList = dictionary2,
				ColumnWidthList = dictionary3,
				TemplateName = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "AllAccount", "所有科目"),
				IsMainSheet = true
			};
		}

		private ImportTemplateModel GetAccountingDimensionTemplateModel(MContext ctx, BDAccountModel account)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			List<IOTemplateConfigModel> templateConfig = GetTemplateConfig(ctx, false, account.MCheckGroupModel);
			foreach (IOTemplateConfigModel item in templateConfig)
			{
				dictionary.Add(item.MFieldName, item.MLangList.FirstOrDefault((KeyValuePair<string, string> f) => f.Key == ctx.MLCID).Value);
			}
			Dictionary<string, string[]> dictionary2 = new Dictionary<string, string[]>();
			List<BDAccountModel> accountList = new List<BDAccountModel>
			{
				account
			};
			GetTemplateBasicData(ctx, dictionary, accountList, dictionary2);
			dictionary2.Add("MNumber", new string[1]
			{
				"1601.01"
			});
			dictionary2.Add("MName", new string[1]
			{
				COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "FixedAssetsOfficeEquipment", "固定资产 - 办公设备")
			});
			Dictionary<string, int> dictionary3 = new Dictionary<string, int>();
			string checkTypeColumnExplainName = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "CheckType", "核算维度");
			List<IOTemplateConfigModel> list = (from x in templateConfig
			where x.MComment == checkTypeColumnExplainName
			select x).ToList();
			if (list != null && list.Count() > 0)
			{
				List<string> list2 = (from x in list
				select x.MFieldName).ToList();
				foreach (string item2 in list2)
				{
					dictionary3.Add(item2, 15);
				}
			}
			dictionary3.Add("MCurrencyID", 10);
			return new ImportTemplateModel
			{
				TemplateType = "OpeningBalance",
				LocaleID = ctx.MLCID,
				ColumnList = dictionary,
				FieldConfigList = templateConfig,
				RequiredColumnList = (from f in templateConfig
				where f.MIsRequired
				select f.MFieldName).ToList(),
				TemplateDictionaryList = GloablColumnDatasource,
				ExampleDataList = dictionary2,
				ColumnWidthList = dictionary3,
				TemplateName = account.MNumber,
				IsMainSheet = false
			};
		}

		public List<IOTemplateConfigModel> GetTemplateConfig(MContext ctx, bool isMainSheet, GLCheckGroupModel checkGroupModel)
		{
			List<IOTemplateConfigModel> list = new List<IOTemplateConfigModel>();
			string[] megiLangTypes = ServerHelper.MegiLangTypes;
			if (isMainSheet)
			{
				string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "ForbidEditColumn", "不可编辑列");
				list.AddRange(new List<IOTemplateConfigModel>
				{
					new IOTemplateConfigModel("MNumber", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
					{
						new COMLangInfoModel(LangModule.Common, "Code", "Code", null)
					}), text, false),
					new IOTemplateConfigModel("MName", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
					{
						new COMLangInfoModel(LangModule.Common, "Name", "Name", null)
					}), text, false)
				});
			}
			else
			{
				List<IOTemplateConfigModel> templateConfigListByCheckGroup = GetTemplateConfigListByCheckGroup(ctx, checkGroupModel);
				if (templateConfigListByCheckGroup != null && templateConfigListByCheckGroup.Count() > 0)
				{
					list.AddRange(templateConfigListByCheckGroup);
				}
			}
			COMLangInfoModel originalCurreny = new COMLangInfoModel(LangModule.Acct, "OriginalCurrency", "Original Currency", ")");
			COMLangInfoModel standardCurreny = new COMLangInfoModel(LangModule.Acct, "StandardCurrency", "Standard currency", ")");
			string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "ManualInput", "手动录入");
			list.AddRange(GetCommonColumnModelList(ctx, megiLangTypes, text2, originalCurreny, standardCurreny));
			if (isMainSheet && ctx.MOrgVersionID == 0)
			{
				list.Add(new IOTemplateConfigModel("MCreateInitBill", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Acct, "IsAutoCreateInitDocument", "是否自动生成期初单据", null)
				}), COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "DropdownSelection", "下拉选择"), false));
			}
			return list;
		}

		private List<IOTemplateConfigModel> GetCommonColumnModelList(MContext ctx, string[] languageArray, string comment, COMLangInfoModel originalCurreny, COMLangInfoModel standardCurreny)
		{
			if (commonColumnModelList == null)
			{
				commonColumnModelList = new List<IOTemplateConfigModel>();
				commonColumnModelList.AddRange(new List<IOTemplateConfigModel>
				{
					new IOTemplateConfigModel("MCurrencyID", COMMultiLangRepository.GetAllText(languageArray, new COMLangInfoModel[1]
					{
						new COMLangInfoModel(LangModule.Report, "Currency", "Currency", null)
					}), COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "CurrentComment", "系统中已增加的币别"), true),
					new IOTemplateConfigModel("MInitDebitBalanceFor", COMMultiLangRepository.GetAllText(languageArray, new COMLangInfoModel[2]
					{
						new COMLangInfoModel(LangModule.Acct, "InitialDebitBalance", "期初借方余额", "("),
						originalCurreny
					}), IOTemplateFieldType.Decimal, comment, true),
					new IOTemplateConfigModel("MInitDebitBalance", COMMultiLangRepository.GetAllText(languageArray, new COMLangInfoModel[2]
					{
						new COMLangInfoModel(LangModule.Acct, "InitialDebitBalance", "期初借方余额", "("),
						standardCurreny
					}), IOTemplateFieldType.Decimal, comment, false),
					new IOTemplateConfigModel("MInitCreditBalanceFor", COMMultiLangRepository.GetAllText(languageArray, new COMLangInfoModel[2]
					{
						new COMLangInfoModel(LangModule.Acct, "InitialCreditBalance", "期初贷方余额", "("),
						originalCurreny
					}), IOTemplateFieldType.Decimal, comment, true),
					new IOTemplateConfigModel("MInitCreditBalance", COMMultiLangRepository.GetAllText(languageArray, new COMLangInfoModel[2]
					{
						new COMLangInfoModel(LangModule.Acct, "InitialCreditBalance", "期初贷方余额", "("),
						standardCurreny
					}), IOTemplateFieldType.Decimal, comment, false)
				});
				if (ctx.MGLBeginDate.Month != 1)
				{
					commonColumnModelList.AddRange(new List<IOTemplateConfigModel>
					{
						new IOTemplateConfigModel("MYtdDebitFor", COMMultiLangRepository.GetAllText(languageArray, new COMLangInfoModel[2]
						{
							new COMLangInfoModel(LangModule.Acct, "CumulativeDebitThisYear", "Cumulative debit this year", "("),
							originalCurreny
						}), IOTemplateFieldType.Decimal, comment, false),
						new IOTemplateConfigModel("MYtdDebit", COMMultiLangRepository.GetAllText(languageArray, new COMLangInfoModel[2]
						{
							new COMLangInfoModel(LangModule.Acct, "CumulativeDebitThisYear", "Cumulative debit this year", "("),
							standardCurreny
						}), IOTemplateFieldType.Decimal, comment, false),
						new IOTemplateConfigModel("MYtdCreditFor", COMMultiLangRepository.GetAllText(languageArray, new COMLangInfoModel[2]
						{
							new COMLangInfoModel(LangModule.Acct, "CumulativeCreditThisYear", "Cumulative credit this year", "("),
							originalCurreny
						}), IOTemplateFieldType.Decimal, comment, false),
						new IOTemplateConfigModel("MYtdCredit", COMMultiLangRepository.GetAllText(languageArray, new COMLangInfoModel[2]
						{
							new COMLangInfoModel(LangModule.Acct, "CumulativeCreditThisYear", "Cumulative credit this year", "("),
							standardCurreny
						}), IOTemplateFieldType.Decimal, comment, false)
					});
				}
			}
			return commonColumnModelList;
		}

		private List<IOTemplateConfigModel> GetTemplateConfigListByCheckGroup(MContext ctx, GLCheckGroupModel checkGroupModel)
		{
			List<IOTemplateConfigModel> list = new List<IOTemplateConfigModel>();
			string[] megiLangTypes = ServerHelper.MegiLangTypes;
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "CheckType", "核算维度");
			int mContactID = checkGroupModel.MContactID;
			if (mContactID == CheckTypeStatusEnum.Optional || mContactID == CheckTypeStatusEnum.Required)
			{
				Dictionary<string, string> allText = COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Contact, "Contact", "联系人", null)
				});
				bool isRequired = mContactID == CheckTypeStatusEnum.Required;
				IOTemplateConfigModel item = new IOTemplateConfigModel("MContactID", allText, text, isRequired);
				list.Add(item);
			}
			int mEmployeeID = checkGroupModel.MEmployeeID;
			if (mEmployeeID == CheckTypeStatusEnum.Optional || mEmployeeID == CheckTypeStatusEnum.Required)
			{
				Dictionary<string, string> allText2 = COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Contact, "Employee", "员工", null)
				});
				bool isRequired2 = mEmployeeID == CheckTypeStatusEnum.Required;
				IOTemplateConfigModel item2 = new IOTemplateConfigModel("MEmployeeID", allText2, text, isRequired2);
				list.Add(item2);
			}
			int mMerItemID = checkGroupModel.MMerItemID;
			if (mMerItemID == CheckTypeStatusEnum.Optional || mMerItemID == CheckTypeStatusEnum.Required)
			{
				Dictionary<string, string> allText3 = COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.BD, "MerItem", "商品项目", null)
				});
				bool isRequired3 = mMerItemID == CheckTypeStatusEnum.Required;
				IOTemplateConfigModel item3 = new IOTemplateConfigModel("MMerItemID", allText3, text, isRequired3);
				list.Add(item3);
			}
			int mExpItemID = checkGroupModel.MExpItemID;
			if (mExpItemID == CheckTypeStatusEnum.Optional || mExpItemID == CheckTypeStatusEnum.Required)
			{
				Dictionary<string, string> allText4 = COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.BD, "ExpenseItem", "费用项目", null)
				});
				bool isRequired4 = mExpItemID == CheckTypeStatusEnum.Required;
				IOTemplateConfigModel item4 = new IOTemplateConfigModel("MExpItemID", allText4, text, isRequired4);
				list.Add(item4);
			}
			int mPaItemID = checkGroupModel.MPaItemID;
			if (mPaItemID == CheckTypeStatusEnum.Optional || mPaItemID == CheckTypeStatusEnum.Required)
			{
				Dictionary<string, string> allText5 = COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.BD, "PaItem", "工资项目", null)
				});
				bool isRequired5 = mPaItemID == CheckTypeStatusEnum.Required;
				IOTemplateConfigModel item5 = new IOTemplateConfigModel("MPaItemID", allText5, text, isRequired5);
				list.Add(item5);
			}
			if (TrackNameList == null)
			{
				TrackNameList = new BDTrackRepository().GetTrackBasicInfo(ctx, null, false, true);
			}
			List<NameValueModel> trackNameList = TrackNameList;
			if (trackNameList == null || trackNameList.Count() == 0)
			{
				return list;
			}
			int mTrackItem = checkGroupModel.MTrackItem1;
			if ((mTrackItem == CheckTypeStatusEnum.Optional || mTrackItem == CheckTypeStatusEnum.Required) && trackNameList.Count() >= 1)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				string[] array = megiLangTypes;
				foreach (string key in array)
				{
					dictionary.Add(key, trackNameList[0].MName);
				}
				bool isRequired6 = mTrackItem == CheckTypeStatusEnum.Required;
				IOTemplateConfigModel item6 = new IOTemplateConfigModel("MTrackItem1", dictionary, text, isRequired6);
				list.Add(item6);
			}
			int mTrackItem2 = checkGroupModel.MTrackItem2;
			if ((mTrackItem2 == CheckTypeStatusEnum.Optional || mTrackItem2 == CheckTypeStatusEnum.Required) && trackNameList.Count() >= 2)
			{
				Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
				string[] array2 = megiLangTypes;
				foreach (string key2 in array2)
				{
					dictionary2.Add(key2, trackNameList[1].MName);
				}
				bool isRequired7 = mTrackItem2 == CheckTypeStatusEnum.Required;
				IOTemplateConfigModel item7 = new IOTemplateConfigModel("MTrackItem2", dictionary2, text, isRequired7);
				list.Add(item7);
			}
			int mTrackItem3 = checkGroupModel.MTrackItem3;
			if ((mTrackItem3 == CheckTypeStatusEnum.Optional || mTrackItem3 == CheckTypeStatusEnum.Required) && trackNameList.Count() >= 3)
			{
				Dictionary<string, string> dictionary3 = new Dictionary<string, string>();
				string[] array3 = megiLangTypes;
				foreach (string key3 in array3)
				{
					dictionary3.Add(key3, trackNameList[2].MName);
				}
				bool isRequired8 = mTrackItem3 == CheckTypeStatusEnum.Required;
				IOTemplateConfigModel item8 = new IOTemplateConfigModel("MTrackItem3", dictionary3, text, isRequired8);
				list.Add(item8);
			}
			int mTrackItem4 = checkGroupModel.MTrackItem4;
			if ((mTrackItem4 == CheckTypeStatusEnum.Optional || mTrackItem4 == CheckTypeStatusEnum.Required) && trackNameList.Count() >= 4)
			{
				Dictionary<string, string> dictionary4 = new Dictionary<string, string>();
				string[] array4 = megiLangTypes;
				foreach (string key4 in array4)
				{
					dictionary4.Add(key4, trackNameList[3].MName);
				}
				bool isRequired9 = mTrackItem4 == CheckTypeStatusEnum.Required;
				IOTemplateConfigModel item9 = new IOTemplateConfigModel("MTrackItem4", dictionary4, text, isRequired9);
				list.Add(item9);
			}
			int mTrackItem5 = checkGroupModel.MTrackItem5;
			if ((mTrackItem5 == CheckTypeStatusEnum.Optional || mTrackItem5 == CheckTypeStatusEnum.Required) && trackNameList.Count() >= 5)
			{
				Dictionary<string, string> dictionary5 = new Dictionary<string, string>();
				string[] array5 = megiLangTypes;
				foreach (string key5 in array5)
				{
					dictionary5.Add(key5, trackNameList[4].MName);
				}
				bool isRequired10 = mTrackItem5 == CheckTypeStatusEnum.Required;
				IOTemplateConfigModel item10 = new IOTemplateConfigModel("MTrackItem5", dictionary5, text, isRequired10);
				list.Add(item10);
			}
			return list;
		}

		public void GetTemplateBasicData(MContext ctx, Dictionary<string, string> columnList, List<BDAccountModel> accountList, Dictionary<string, string[]> exampleDataList = null)
		{
			List<ImportTemplateDataSource> list = new List<ImportTemplateDataSource>();
			if (!GloablColumnDatasource.Exists((ImportTemplateDataSource x) => x.FieldType == ImportTemplateColumnType.Currency))
			{
				BASCurrencyViewModel @base = CurrencyBLL.GetBase(ctx, false, null, null);
				List<REGCurrencyViewModel> viewList = CurrencyBLL.GetViewList(ctx, null, null, false, null);
				List<ImportDataSourceInfo> list2 = new List<ImportDataSourceInfo>();
				if (@base != null)
				{
					list2.Add(new ImportDataSourceInfo
					{
						Key = @base.MCurrencyID,
						Value = $"{@base.MCurrencyID} {@base.MLocalName}",
						Info = "1"
					});
				}
				foreach (REGCurrencyViewModel item9 in viewList)
				{
					if (!list2.Any((ImportDataSourceInfo f) => f.Key == item9.MCurrencyID))
					{
						list2.Add(new ImportDataSourceInfo
						{
							Key = item9.MCurrencyID,
							Value = $"{item9.MCurrencyID} {item9.MName}"
						});
					}
				}
				ImportTemplateDataSource item2 = new ImportTemplateDataSource(true)
				{
					FieldType = ImportTemplateColumnType.Currency,
					FieldList = new List<string>
					{
						"MCurrencyID"
					},
					DataSourceList = list2
				};
				GloablColumnDatasource.Add(item2);
			}
			if (!GloablColumnDatasource.Exists((ImportTemplateDataSource x) => x.FieldType == ImportTemplateColumnType.CreateInitBill))
			{
				List<ImportDataSourceInfo> list3 = new List<ImportDataSourceInfo>();
				list3.Add(new ImportDataSourceInfo
				{
					Key = "1",
					Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "Yes", "是")
				});
				list3.Add(new ImportDataSourceInfo
				{
					Key = "0",
					Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "No", "否")
				});
				ImportTemplateDataSource item3 = new ImportTemplateDataSource(true)
				{
					FieldType = ImportTemplateColumnType.CreateInitBill,
					FieldList = new List<string>
					{
						"MCreateInitBill"
					},
					DataSourceList = list3
				};
				GloablColumnDatasource.Add(item3);
			}
			if (accountList != null && accountList.Count() != 0)
			{
				GLUtility gLUtility = new GLUtility();
				foreach (BDAccountModel account in accountList)
				{
					GLCheckGroupModel mCheckGroupModel = account.MCheckGroupModel;
					if ((mCheckGroupModel.MContactID == CheckTypeStatusEnum.Optional || mCheckGroupModel.MContactID == CheckTypeStatusEnum.Required) && !GloablColumnDatasource.Exists((ImportTemplateDataSource x) => x.FieldType == ImportTemplateColumnType.Contact))
					{
						List<GLTreeModel> mDataList = gLUtility.GetContactCheckTypeData(ctx).MDataList;
						List<ImportDataSourceInfo> templateDataSourceListFromBaseData = GetTemplateDataSourceListFromBaseData(mDataList, true, false);
						ImportTemplateDataSource item4 = new ImportTemplateDataSource(true)
						{
							FieldType = ImportTemplateColumnType.Contact,
							FieldList = new List<string>
							{
								"MContactID"
							},
							DataSourceList = templateDataSourceListFromBaseData
						};
						GloablColumnDatasource.Add(item4);
					}
					if ((mCheckGroupModel.MEmployeeID == CheckTypeStatusEnum.Optional || mCheckGroupModel.MEmployeeID == CheckTypeStatusEnum.Required) && !GloablColumnDatasource.Exists((ImportTemplateDataSource x) => x.FieldType == ImportTemplateColumnType.Employee))
					{
						List<GLTreeModel> mDataList2 = gLUtility.GetEmployeeCheckTypeData(ctx).MDataList;
						List<ImportDataSourceInfo> templateDataSourceListFromBaseData2 = GetTemplateDataSourceListFromBaseData(mDataList2, false, false);
						ImportTemplateDataSource item5 = new ImportTemplateDataSource(true)
						{
							FieldType = ImportTemplateColumnType.Employee,
							FieldList = new List<string>
							{
								"MEmployeeID"
							},
							DataSourceList = templateDataSourceListFromBaseData2
						};
						GloablColumnDatasource.Add(item5);
					}
					if ((mCheckGroupModel.MMerItemID == CheckTypeStatusEnum.Optional || mCheckGroupModel.MMerItemID == CheckTypeStatusEnum.Required) && !GloablColumnDatasource.Exists((ImportTemplateDataSource x) => x.FieldType == ImportTemplateColumnType.InventoryItem))
					{
						List<GLTreeModel> mDataList3 = gLUtility.GetMerItemCheckTypeData(ctx).MDataList;
						mDataList3 = (from x in mDataList3
						where x.MIsActive
						select x).ToList();
						List<ImportDataSourceInfo> templateDataSourceListFromBaseData3 = GetTemplateDataSourceListFromBaseData(mDataList3, false, false);
						ImportTemplateDataSource item6 = new ImportTemplateDataSource(true)
						{
							FieldType = ImportTemplateColumnType.InventoryItem,
							FieldList = new List<string>
							{
								"MMerItemID"
							},
							DataSourceList = templateDataSourceListFromBaseData3
						};
						GloablColumnDatasource.Add(item6);
					}
					if ((mCheckGroupModel.MExpItemID == CheckTypeStatusEnum.Optional || mCheckGroupModel.MExpItemID == CheckTypeStatusEnum.Required) && !GloablColumnDatasource.Exists((ImportTemplateDataSource x) => x.FieldType == ImportTemplateColumnType.ExpenseItem))
					{
						List<GLTreeModel> mDataList4 = gLUtility.GetExpItemCheckTypeData(ctx).MDataList;
						List<ImportDataSourceInfo> templateDataSourceListFromBaseData4 = GetTemplateDataSourceListFromBaseData(mDataList4, true, true);
						ImportTemplateDataSource item7 = new ImportTemplateDataSource(true)
						{
							FieldType = ImportTemplateColumnType.ExpenseItem,
							FieldList = new List<string>
							{
								"MExpItemID"
							},
							DataSourceList = templateDataSourceListFromBaseData4
						};
						GloablColumnDatasource.Add(item7);
					}
					if ((mCheckGroupModel.MPaItemID == CheckTypeStatusEnum.Optional || mCheckGroupModel.MPaItemID == CheckTypeStatusEnum.Required) && !GloablColumnDatasource.Exists((ImportTemplateDataSource x) => x.FieldType == ImportTemplateColumnType.SalaryItem))
					{
						List<GLTreeModel> mDataList5 = gLUtility.GetPaItemCheckTypeData(ctx).MDataList;
						List<ImportDataSourceInfo> templateDataSourceListFromBaseData5 = GetTemplateDataSourceListFromBaseData(mDataList5, true, true);
						ImportTemplateDataSource item8 = new ImportTemplateDataSource(true)
						{
							FieldType = ImportTemplateColumnType.SalaryItem,
							FieldList = new List<string>
							{
								"MPaItemID"
							},
							DataSourceList = templateDataSourceListFromBaseData5
						};
						GloablColumnDatasource.Add(item8);
					}
					if ((mCheckGroupModel.MTrackItem1 == CheckTypeStatusEnum.Optional || mCheckGroupModel.MTrackItem1 == CheckTypeStatusEnum.Required) && !GloablColumnDatasource.Exists((ImportTemplateDataSource x) => x.FieldType == ImportTemplateColumnType.TrackItem1))
					{
						ImportTemplateDataSource trackItemBaseData = GetTrackItemBaseData(ctx, gLUtility, 5);
						if (trackItemBaseData != null)
						{
							GloablColumnDatasource.Add(trackItemBaseData);
						}
					}
					if ((mCheckGroupModel.MTrackItem2 == CheckTypeStatusEnum.Optional || mCheckGroupModel.MTrackItem2 == CheckTypeStatusEnum.Required) && !GloablColumnDatasource.Exists((ImportTemplateDataSource x) => x.FieldType == ImportTemplateColumnType.TrackItem2))
					{
						ImportTemplateDataSource trackItemBaseData2 = GetTrackItemBaseData(ctx, gLUtility, 6);
						if (trackItemBaseData2 != null)
						{
							GloablColumnDatasource.Add(trackItemBaseData2);
						}
					}
					if ((mCheckGroupModel.MTrackItem3 == CheckTypeStatusEnum.Optional || mCheckGroupModel.MTrackItem3 == CheckTypeStatusEnum.Required) && !GloablColumnDatasource.Exists((ImportTemplateDataSource x) => x.FieldType == ImportTemplateColumnType.TrackItem3))
					{
						ImportTemplateDataSource trackItemBaseData3 = GetTrackItemBaseData(ctx, gLUtility, 7);
						if (trackItemBaseData3 != null)
						{
							GloablColumnDatasource.Add(trackItemBaseData3);
						}
					}
					if ((mCheckGroupModel.MTrackItem4 == CheckTypeStatusEnum.Optional || mCheckGroupModel.MTrackItem4 == CheckTypeStatusEnum.Required) && !GloablColumnDatasource.Exists((ImportTemplateDataSource x) => x.FieldType == ImportTemplateColumnType.TrackItem4))
					{
						ImportTemplateDataSource trackItemBaseData4 = GetTrackItemBaseData(ctx, gLUtility, 8);
						if (trackItemBaseData4 != null)
						{
							GloablColumnDatasource.Add(trackItemBaseData4);
						}
					}
					if ((mCheckGroupModel.MTrackItem5 == CheckTypeStatusEnum.Optional || mCheckGroupModel.MTrackItem5 == CheckTypeStatusEnum.Required) && !GloablColumnDatasource.Exists((ImportTemplateDataSource x) => x.FieldType == ImportTemplateColumnType.TrackItem5))
					{
						ImportTemplateDataSource trackItemBaseData5 = GetTrackItemBaseData(ctx, gLUtility, 9);
						if (trackItemBaseData5 != null)
						{
							GloablColumnDatasource.Add(trackItemBaseData5);
						}
					}
				}
			}
		}

		private ImportTemplateDataSource GetTrackItemBaseData(MContext ctx, GLUtility glUtility, int trackIndex)
		{
			GLCheckTypeDataModel trackCheckTypeData = glUtility.GetTrackCheckTypeData(ctx, trackIndex, false);
			if (trackCheckTypeData != null)
			{
				List<GLTreeModel> mDataList = trackCheckTypeData.MDataList;
				List<ImportDataSourceInfo> templateDataSourceListFromBaseData = GetTemplateDataSourceListFromBaseData(mDataList, false, false);
				string item = "";
				ImportTemplateColumnType importTemplateColumnType;
				switch (trackIndex)
				{
				case 5:
					importTemplateColumnType = ImportTemplateColumnType.TrackItem1;
					item = "MTrackItem" + 1;
					break;
				case 6:
					importTemplateColumnType = ImportTemplateColumnType.TrackItem2;
					item = "MTrackItem" + 2;
					break;
				case 7:
					importTemplateColumnType = ImportTemplateColumnType.TrackItem3;
					item = "MTrackItem" + 3;
					break;
				case 8:
					importTemplateColumnType = ImportTemplateColumnType.TrackItem4;
					item = "MTrackItem" + 4;
					break;
				case 9:
					importTemplateColumnType = ImportTemplateColumnType.TrackItem5;
					item = "MTrackItem" + 5;
					break;
				default:
					importTemplateColumnType = ImportTemplateColumnType.None;
					break;
				}
				if (importTemplateColumnType == ImportTemplateColumnType.None)
				{
					return null;
				}
				return new ImportTemplateDataSource(true)
				{
					FieldType = importTemplateColumnType,
					FieldList = new List<string>
					{
						item
					},
					DataSourceList = templateDataSourceListFromBaseData
				};
			}
			return null;
		}

		private List<ImportDataSourceInfo> GetTemplateDataSourceListFromBaseData(List<GLTreeModel> baseDataList, bool isTree, bool isIncludeParent = false)
		{
			List<ImportDataSourceInfo> list = new List<ImportDataSourceInfo>();
			if (baseDataList != null && baseDataList.Count() > 0)
			{
				foreach (GLTreeModel baseData in baseDataList)
				{
					if (isTree)
					{
						List<GLTreeModel> children = baseData.children;
						if (children == null || children.Count() == 0)
						{
							if (isIncludeParent && baseData.MIsActive)
							{
								ImportDataSourceInfo importDataSourceInfo = new ImportDataSourceInfo();
								importDataSourceInfo.Key = baseData.id;
								importDataSourceInfo.Value = baseData.text;
								list.Add(importDataSourceInfo);
							}
						}
						else
						{
							foreach (GLTreeModel item in children)
							{
								if (!list.Exists((ImportDataSourceInfo x) => x.Key == item.id) && item.MIsActive)
								{
									ImportDataSourceInfo importDataSourceInfo2 = new ImportDataSourceInfo();
									importDataSourceInfo2.Key = item.id;
									importDataSourceInfo2.Value = item.text;
									list.Add(importDataSourceInfo2);
								}
							}
						}
					}
					else if (!list.Exists((ImportDataSourceInfo x) => x.Key == baseData.id) && baseData.MIsActive)
					{
						ImportDataSourceInfo importDataSourceInfo3 = new ImportDataSourceInfo();
						importDataSourceInfo3.Key = baseData.id;
						importDataSourceInfo3.Value = baseData.text;
						list.Add(importDataSourceInfo3);
					}
				}
			}
			return list;
		}

		public List<GLInitBalanceModel> GetCheckGroupInitBalanceModelList(MContext ctx, BDAccountEditModel accountModel)
		{
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.Equal("MAccountID", accountModel.MItemID);
			sqlWhere.NotEqual("MCheckGroupValueID", "0");
			return dal.GetInitBalanceListIncludeCheckGroupValue(ctx, sqlWhere);
		}

		public OperationResult IsCanUpdateAccountCheckGroup(MContext ctx, BDAccountEditModel accountModel, BDAccountEditModel oldAccountModel = null)
		{
			OperationResult operationResult = new OperationResult();
			operationResult.Success = true;
			GLCheckGroupModel mCheckGroupModel = accountModel.MCheckGroupModel;
			if (mCheckGroupModel == null)
			{
				return operationResult;
			}
			List<GLInitBalanceModel> checkGroupInitBalanceModelList = GetCheckGroupInitBalanceModelList(ctx, accountModel);
			if (checkGroupInitBalanceModelList == null || checkGroupInitBalanceModelList.Count() == 0)
			{
				return operationResult;
			}
			if (oldAccountModel != null)
			{
				GLCheckGroupModel mCheckGroupModel2 = oldAccountModel.MCheckGroupModel;
				string empty = string.Empty;
				bool flag = IsCanChangeCheckTypeStatus(ctx, mCheckGroupModel, mCheckGroupModel2, checkGroupInitBalanceModelList, out empty);
				if (!flag)
				{
					operationResult.Success = flag;
					operationResult.Message = empty;
					return operationResult;
				}
			}
			foreach (GLInitBalanceModel item in checkGroupInitBalanceModelList)
			{
				string empty2 = string.Empty;
				operationResult.Success = InitBalanceCheckTypeMatchCheckTypeRule(mCheckGroupModel, item, ctx, out empty2);
				if (!operationResult.Success)
				{
					break;
				}
			}
			return operationResult;
		}

		public bool IsCanChangeCheckTypeStatus(MContext ctx, GLCheckGroupModel checkGroupModel, GLCheckGroupModel oldCheckGroupModel, List<GLInitBalanceModel> initBalanceList, out string tips)
		{
			bool flag = true;
			tips = string.Empty;
			if (oldCheckGroupModel == null)
			{
				return flag;
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (BDAccountHelper.IsChangeCheckTypeStatus(checkGroupModel.MContactID, oldCheckGroupModel.MContactID))
			{
				bool flag2 = initBalanceList.Exists((GLInitBalanceModel x) => x.MCheckGroupValueModel != null && !string.IsNullOrWhiteSpace(x.MCheckGroupValueModel.MContactID));
				string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "ContactCheckTypeRefInInitBalance", "联系人核算维度已经在期初余额中使用，不可更改为禁用状态");
				stringBuilder.AppendLine(text);
				flag = (!flag && false);
			}
			if (BDAccountHelper.IsChangeCheckTypeStatus(checkGroupModel.MEmployeeID, oldCheckGroupModel.MEmployeeID))
			{
				bool flag3 = initBalanceList.Exists((GLInitBalanceModel x) => x.MCheckGroupValueModel != null && !string.IsNullOrWhiteSpace(x.MCheckGroupValueModel.MEmployeeID));
				string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "EmployeeCheckTypeRefInInitBalance", "员工核算维度已经在期初余额中使用，不可更改为禁用状态");
				stringBuilder.AppendLine(text2);
				flag = (!flag && false);
			}
			if (BDAccountHelper.IsChangeCheckTypeStatus(checkGroupModel.MMerItemID, oldCheckGroupModel.MMerItemID))
			{
				bool flag4 = initBalanceList.Exists((GLInitBalanceModel x) => x.MCheckGroupValueModel != null && !string.IsNullOrWhiteSpace(x.MCheckGroupValueModel.MMerItemID));
				string text3 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "ItemCheckTypeRefInInitBalance", "商品项目核算维度已经在期初余额中使用，不可更改为禁用状态");
				stringBuilder.AppendLine(text3);
				flag = (!flag && false);
			}
			if (BDAccountHelper.IsChangeCheckTypeStatus(checkGroupModel.MExpItemID, oldCheckGroupModel.MExpItemID))
			{
				bool flag5 = initBalanceList.Exists((GLInitBalanceModel x) => x.MCheckGroupValueModel != null && !string.IsNullOrWhiteSpace(x.MCheckGroupValueModel.MExpItemID));
				string text4 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "ExpenseCheckTypeRefInInitBalance", "费用项目核算维度已经在期初余额中使用，不可更改为禁用状态");
				stringBuilder.AppendLine(text4);
				flag = (!flag && false);
			}
			if (BDAccountHelper.IsChangeCheckTypeStatus(checkGroupModel.MPaItemID, oldCheckGroupModel.MPaItemID))
			{
				bool flag6 = initBalanceList.Exists((GLInitBalanceModel x) => x.MCheckGroupValueModel != null && !string.IsNullOrWhiteSpace(x.MCheckGroupValueModel.MPaItemID));
				string text5 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "PACheckTypeRefInInitBalance", "工资项目核算维度已经在期初余额中使用，不可更改为禁用状态");
				stringBuilder.AppendLine(text5);
				flag = (!flag && false);
			}
			if (BDAccountHelper.IsChangeCheckTypeStatus(checkGroupModel.MTrackItem1, oldCheckGroupModel.MTrackItem1))
			{
				bool flag7 = initBalanceList.Exists((GLInitBalanceModel x) => x.MCheckGroupValueModel != null && !string.IsNullOrWhiteSpace(x.MCheckGroupValueModel.MTrackItem1));
				string text6 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "MTrackItemCheckTypeRefInInitBalance", "跟踪项核算维度已经在期初余额中使用，不可更改为禁用状态");
				stringBuilder.AppendLine(text6);
				flag = (!flag && false);
			}
			if (BDAccountHelper.IsChangeCheckTypeStatus(checkGroupModel.MTrackItem2, oldCheckGroupModel.MTrackItem2))
			{
				bool flag8 = initBalanceList.Exists((GLInitBalanceModel x) => x.MCheckGroupValueModel != null && !string.IsNullOrWhiteSpace(x.MCheckGroupValueModel.MTrackItem2));
				string text7 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "MTrackItemCheckTypeRefInInitBalance", "跟踪项核算维度已经在期初余额中使用，不可更改为禁用状态");
				stringBuilder.AppendLine(text7);
				flag = (!flag && false);
			}
			if (BDAccountHelper.IsChangeCheckTypeStatus(checkGroupModel.MTrackItem3, oldCheckGroupModel.MTrackItem3))
			{
				bool flag9 = initBalanceList.Exists((GLInitBalanceModel x) => x.MCheckGroupValueModel != null && !string.IsNullOrWhiteSpace(x.MCheckGroupValueModel.MTrackItem3));
				string text8 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "MTrackItemCheckTypeRefInInitBalance", "跟踪项核算维度已经在期初余额中使用，不可更改为禁用状态");
				stringBuilder.AppendLine(text8);
				flag = (!flag && false);
			}
			if (BDAccountHelper.IsChangeCheckTypeStatus(checkGroupModel.MTrackItem4, oldCheckGroupModel.MTrackItem4))
			{
				bool flag10 = initBalanceList.Exists((GLInitBalanceModel x) => x.MCheckGroupValueModel != null && !string.IsNullOrWhiteSpace(x.MCheckGroupValueModel.MTrackItem4));
				string text9 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "MTrackItemCheckTypeRefInInitBalance", "跟踪项核算维度已经在期初余额中使用，不可更改为禁用状态");
				stringBuilder.AppendLine(text9);
				flag = (!flag && false);
			}
			if (BDAccountHelper.IsChangeCheckTypeStatus(checkGroupModel.MTrackItem5, oldCheckGroupModel.MTrackItem5))
			{
				bool flag11 = initBalanceList.Exists((GLInitBalanceModel x) => x.MCheckGroupValueModel != null && !string.IsNullOrWhiteSpace(x.MCheckGroupValueModel.MTrackItem5));
				string text10 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "MTrackItemCheckTypeRefInInitBalance", "跟踪项核算维度已经在期初余额中使用，不可更改为禁用状态");
				stringBuilder.AppendLine(text10);
				flag = (!flag && false);
			}
			tips = stringBuilder.ToString();
			return flag;
		}

		public bool InitBalanceCheckTypeMatchCheckTypeRule(GLCheckGroupModel checkGroupModel, GLInitBalanceModel initBalanceModel, MContext ctx, out string error)
		{
			bool result = true;
			error = string.Empty;
			StringBuilder stringBuilder = new StringBuilder();
			GLCheckGroupValueModel mCheckGroupValueModel = initBalanceModel.MCheckGroupValueModel;
			PropertyInfo[] properties = checkGroupModel.GetType().GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				string name = propertyInfo.Name;
				if (!(name == "PKFieldValue") && !(name == "MItemID"))
				{
					int num = 0;
					string s = Convert.ToString(propertyInfo.GetValue(checkGroupModel));
					if (int.TryParse(s, out num))
					{
						if (num != CheckTypeStatusEnum.Required)
						{
							goto IL_012d;
						}
						if (mCheckGroupValueModel == null)
						{
							result = false;
						}
						else
						{
							Type type = mCheckGroupValueModel.GetType();
							PropertyInfo property = type.GetProperty(name);
							if (!(property == (PropertyInfo)null))
							{
								string value = Convert.ToString(property.GetValue(mCheckGroupValueModel));
								if (string.IsNullOrWhiteSpace(value))
								{
									result = false;
									string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "CheckTypeIsRequired", "核算维度:{0}是必录项目 ");
									string checkTypeName = GetCheckTypeName(ctx, name);
									stringBuilder.AppendLine(string.Format(text, checkTypeName));
								}
								goto IL_012d;
							}
						}
					}
				}
				continue;
				IL_012d:
				error = stringBuilder.ToString();
			}
			return result;
		}

		private string GetCheckTypeName(MContext ctx, string fieldName)
		{
			int num = -1;
			string text = string.Empty;
			GLUtility gLUtility = new GLUtility();
			switch (fieldName)
			{
			case "MContactID":
			{
				num = 0;
				bool flag = CheckTypeNameDic.Keys.Contains(num);
				text = (flag ? CheckTypeNameDic[num] : gLUtility.GetCheckTypeName(ctx, num));
				if (!flag)
				{
					CheckTypeNameDic.Add(num, text);
				}
				break;
			}
			case "MEmployeeID":
			{
				num = 1;
				bool flag = CheckTypeNameDic.Keys.Contains(num);
				text = (flag ? CheckTypeNameDic[num] : gLUtility.GetCheckTypeName(ctx, num));
				if (!flag)
				{
					CheckTypeNameDic.Add(num, text);
				}
				break;
			}
			case "MExpItemID":
			{
				num = 3;
				bool flag = CheckTypeNameDic.Keys.Contains(num);
				text = (flag ? CheckTypeNameDic[num] : gLUtility.GetCheckTypeName(ctx, num));
				if (!flag)
				{
					CheckTypeNameDic.Add(num, text);
				}
				break;
			}
			case "MMerItemID":
			{
				num = 2;
				bool flag = CheckTypeNameDic.Keys.Contains(num);
				text = (flag ? CheckTypeNameDic[num] : gLUtility.GetCheckTypeName(ctx, num));
				if (!flag)
				{
					CheckTypeNameDic.Add(num, text);
				}
				break;
			}
			case "MPaItemID":
			{
				num = 4;
				bool flag = CheckTypeNameDic.Keys.Contains(num);
				text = (flag ? CheckTypeNameDic[num] : gLUtility.GetCheckTypeName(ctx, num));
				if (!flag)
				{
					CheckTypeNameDic.Add(num, text);
				}
				break;
			}
			case "MTrackItem1":
			{
				num = 5;
				bool flag = CheckTypeNameDic.Keys.Contains(num);
				text = (flag ? CheckTypeNameDic[num] : gLUtility.GetCheckTypeName(ctx, num));
				if (!flag)
				{
					CheckTypeNameDic.Add(num, text);
				}
				break;
			}
			case "MTrackItem2":
			{
				num = 6;
				bool flag = CheckTypeNameDic.Keys.Contains(num);
				text = (flag ? CheckTypeNameDic[num] : gLUtility.GetCheckTypeName(ctx, num));
				if (!flag)
				{
					CheckTypeNameDic.Add(num, text);
				}
				break;
			}
			case "MTrackItem3":
			{
				num = 7;
				bool flag = CheckTypeNameDic.Keys.Contains(num);
				text = (flag ? CheckTypeNameDic[num] : gLUtility.GetCheckTypeName(ctx, num));
				if (!flag)
				{
					CheckTypeNameDic.Add(num, text);
				}
				break;
			}
			case "MTrackItem4":
			{
				num = 8;
				bool flag = CheckTypeNameDic.Keys.Contains(num);
				text = (flag ? CheckTypeNameDic[num] : gLUtility.GetCheckTypeName(ctx, num));
				if (!flag)
				{
					CheckTypeNameDic.Add(num, text);
				}
				break;
			}
			case "MTrackItem5":
			{
				num = 9;
				bool flag = CheckTypeNameDic.Keys.Contains(num);
				text = (flag ? CheckTypeNameDic[num] : gLUtility.GetCheckTypeName(ctx, num));
				if (!flag)
				{
					CheckTypeNameDic.Add(num, text);
				}
				break;
			}
			}
			return text;
		}

		public List<GLInitBalanceModel> GetInitBalanceList(MContext ctx, GLInitBalanceListFilterModel filter)
		{
			if (!string.IsNullOrWhiteSpace(filter.AccountID))
			{
				filter.Equal("MAccountID", filter.AccountID);
			}
			if (!filter.IncludeCheckTypeData)
			{
				filter.Equal("MCheckGroupValueID", "0");
			}
			return dal.GetInitBalanceListIncludeCheckGroupValue(ctx, filter);
		}

		public List<GLInitBalanceModel> GetCompleteInitBalanceList(MContext ctx, GLInitBalanceListFilterModel filter)
		{
			List<GLInitBalanceModel> completeInitBalanceList = dal.GetCompleteInitBalanceList(ctx, filter);
			if (filter.IsExport && completeInitBalanceList != null)
			{
				completeInitBalanceList.ForEach(delegate(GLInitBalanceModel x)
				{
					if (x.MAccountModel != null && x.MAccountModel.MCreateInitBill)
					{
						x.MAccountModel.MCreateInitBillName = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "Yes", "是");
					}
					if (x.MDC == 1)
					{
						x.MInitDebitBalance = x.MInitBalance;
						x.MInitDebitBalanceFor = x.MInitBalanceFor;
					}
					else
					{
						x.MInitCreditBalance = x.MInitBalance;
						x.MInitCreditBalanceFor = x.MInitBalanceFor;
					}
				});
			}
			return BDAccountHelper.OrderBy(completeInitBalanceList);
		}

		public OperationResult SaveInitBalance(MContext ctx, List<GLInitBalanceModel> initBalanceList)
		{
			OperationResult operationResult = new OperationResult();
			OperationResult operationResult2 = ValidateInitBalance(ctx, initBalanceList);
			if (!operationResult2.Success)
			{
				return operationResult2;
			}
			operationResult = UpdateInitBalance(ctx, initBalanceList);
			operationResult.MessageList.Add(operationResult.Message);
			return operationResult;
		}

		public OperationResult ValidateInitBalance(MContext ctx, List<GLInitBalanceModel> initBalanceList)
		{
			OperationResult operationResult = new OperationResult();
			if (ctx.MInitBalanceOver)
			{
				operationResult.Success = false;
				string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "FinishInitBalanceForbidInputInitBalance", "科目期初余额已完成初始化，不允许录入期初余额！");
				operationResult.MessageList.Add(text);
				return operationResult;
			}
			if (initBalanceList == null || initBalanceList.Count() == 0)
			{
				operationResult.Success = true;
				return operationResult;
			}
			BDCheckValidteListModel bDCheckValidteListModel = new BDCheckValidteListModel();
			List<string> list = (from x in initBalanceList
			select x.MAccountID).ToList();
			if (list != null)
			{
				bDCheckValidteListModel.AccountIdList.AddRange(list);
			}
			List<GLCheckGroupValueModel> list2 = (from x in initBalanceList
			select x.MCheckGroupValueModel).ToList();
			if (list2 != null)
			{
				List<string> list3 = (from x in list2
				select x.MContactID into x
				where !string.IsNullOrWhiteSpace(x)
				select x).ToList();
				if (list3 != null)
				{
					bDCheckValidteListModel.ContactIdList.AddRange(list3);
				}
				List<string> list4 = (from x in list2
				select x.MEmployeeID into x
				where !string.IsNullOrWhiteSpace(x)
				select x).ToList();
				if (list4 != null)
				{
					bDCheckValidteListModel.EmployeeIdList.AddRange(list4);
				}
				List<string> list5 = (from x in list2
				select x.MMerItemID into x
				where !string.IsNullOrWhiteSpace(x)
				select x).ToList();
				if (list5 != null)
				{
					bDCheckValidteListModel.MerchandiseIdList.AddRange(list5);
				}
				List<string> list6 = (from x in list2
				select x.MExpItemID into x
				where !string.IsNullOrWhiteSpace(x)
				select x).ToList();
				if (list6 != null)
				{
					bDCheckValidteListModel.ExpenseIdList.AddRange(list6);
				}
				List<string> list7 = (from x in list2
				where !string.IsNullOrWhiteSpace(x.MPaItemGroupID)
				select x.MPaItemID into x
				where !string.IsNullOrWhiteSpace(x)
				select x).ToList();
				if (list7 != null)
				{
					bDCheckValidteListModel.PaIdList.AddRange(list7);
				}
				List<string> list8 = (from x in list2
				select (!string.IsNullOrWhiteSpace(x.MPaItemGroupID)) ? x.MPaItemGroupID : x.MPaItemID into x
				where !string.IsNullOrWhiteSpace(x)
				select x).ToList();
				if (list8 != null)
				{
					bDCheckValidteListModel.PaGroupIdList.AddRange(list8);
				}
				List<string> list9 = new List<string>();
				List<string> list10 = (from x in list2
				select x.MTrackItem1 into x
				where !string.IsNullOrWhiteSpace(x)
				select x).ToList();
				if (list10 != null)
				{
					list9.AddRange(list10);
				}
				List<string> list11 = (from x in list2
				select x.MTrackItem2 into x
				where !string.IsNullOrWhiteSpace(x)
				select x).ToList();
				if (list11 != null)
				{
					list9.AddRange(list11);
				}
				List<string> list12 = (from x in list2
				select x.MTrackItem3 into x
				where !string.IsNullOrWhiteSpace(x)
				select x).ToList();
				if (list12 != null)
				{
					list9.AddRange(list12);
				}
				List<string> list13 = (from x in list2
				select x.MTrackItem4 into x
				where !string.IsNullOrWhiteSpace(x)
				select x).ToList();
				if (list13 != null)
				{
					list9.AddRange(list13);
				}
				List<string> list14 = (from x in list2
				select x.MTrackItem5 into x
				where !string.IsNullOrWhiteSpace(x)
				select x).ToList();
				if (list14 != null)
				{
					list9.AddRange(list14);
				}
				bDCheckValidteListModel.TrackEntryIdList.AddRange(list9);
			}
			List<string> list15 = (from x in initBalanceList
			select x.MBankID).ToList();
			if (list15 != null)
			{
				bDCheckValidteListModel.BankIdList.AddRange(list15);
			}
			GLUtility gLUtility = new GLUtility();
			List<ValidateQueryModel> validateQueryModel = gLUtility.GetValidateQueryModel(ctx, bDCheckValidteListModel);
			List<MActionResultCodeEnum> list16 = gLUtility.QueryValidateSql(ctx, true, validateQueryModel.ToArray());
			BDAccountRepository bDAccountRepository = new BDAccountRepository();
			IEnumerable<IGrouping<string, GLInitBalanceModel>> enumerable = from x in initBalanceList
			group x by x.MAccountID;
			List<BDAccountModel> accountWithParentList = GLDataPool.GetInstance(ctx, false, 0, 0, 0).AccountWithParentList;
			foreach (IGrouping<string, GLInitBalanceModel> item2 in enumerable)
			{
				string accountId = item2.Key;
				BDAccountModel bDAccountModel = accountWithParentList.FirstOrDefault((BDAccountModel x) => x.MItemID == accountId);
				if (bDAccountModel != null)
				{
					GLCheckGroupModel mCheckGroupModel = bDAccountModel.MCheckGroupModel;
					if (mCheckGroupModel != null)
					{
						List<GLInitBalanceModel> list17 = item2.ToList();
						if (list17 != null && list17.Count() != 0)
						{
							List<string> billIdList = (from x in list17
							where !string.IsNullOrWhiteSpace(x.MBillID)
							select x.MBillID).ToList();
							List<IVVerificationListModel> verificationList = IVVerificationRepository.GetVerificationList(ctx, billIdList);
							foreach (GLInitBalanceModel item3 in list17)
							{
								if (verificationList != null && verificationList.Count() > 0)
								{
									OperationResult operationResult2 = ValidateBillIsVerification(ctx, item3, verificationList);
									if (operationResult2.Success)
									{
										operationResult.Success = false;
										string arg = (item3.MCheckGroupValueModel != null) ? item3.MCheckGroupValueModel.CheckGroupCombinationName : "";
										string item = string.Format(operationResult2.Message, bDAccountModel.MNumber, arg);
										operationResult.MessageList.Add(item);
									}
								}
								string empty = string.Empty;
								operationResult.Success = (InitBalanceCheckTypeMatchCheckTypeRule(mCheckGroupModel, item3, ctx, out empty) && operationResult.Success);
								if (!operationResult.Success && !string.IsNullOrWhiteSpace(empty))
								{
									operationResult.MessageList.Add(empty);
								}
							}
						}
					}
				}
			}
			return operationResult;
		}

		private OperationResult ValidateBillIsVerification(MContext ctx, GLInitBalanceModel initBalance, List<IVVerificationListModel> verificationRecordList)
		{
			OperationResult operationResult = new OperationResult();
			string billId = initBalance.MBillID;
			if (string.IsNullOrWhiteSpace(initBalance.MBillID))
			{
				return operationResult;
			}
			if (verificationRecordList.Exists((IVVerificationListModel x) => x.MSourceBillID == billId || x.MTargetBillID == billId))
			{
				operationResult.Success = true;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "BillHadVerification", "科目：{0}，{1}自动生成的单据存在核销关系，请先删除核销关系后再进行修改！");
			}
			return operationResult;
		}

		public OperationResult UpdateInitBalance(MContext ctx, List<GLInitBalanceModel> initBalanceList)
		{
			OperationResult operationResult = new OperationResult();
			List<CommandInfo> updateInitBalanceCommandList = GetUpdateInitBalanceCommandList(ctx, initBalanceList, out operationResult, false, true);
			if (!operationResult.Success)
			{
				return operationResult;
			}
			operationResult.Success = (BDRepository.ExecuteSqlTran(ctx, updateInitBalanceCommandList) > 0);
			return operationResult;
		}

		private List<CommandInfo> GetUpdateInitBalanceCommandList(MContext ctx, List<GLInitBalanceModel> initBalanceList, out OperationResult result, bool isCover = false, bool isMatchBill = true)
		{
			result = new OperationResult();
			BDAccountRepository bDAccountRepository = new BDAccountRepository();
			BDInitDocumentBusiness bDInitDocumentBusiness = new BDInitDocumentBusiness();
			List<BDAccountModel> accountWithParentList = GLDataPool.GetInstance(ctx, false, 0, 0, 0).AccountWithParentList;
			List<BDExchangeRateModel> insertExchangeRateList = new List<BDExchangeRateModel>();
			bool flag = ctx.MBeginDate == ctx.MGLBeginDate;
			IEnumerable<IGrouping<string, GLInitBalanceModel>> enumerable = from x in initBalanceList
			group x by x.MAccountID;
			List<CommandInfo> list = new List<CommandInfo>();
			List<GLInitBalanceModel> list2 = new List<GLInitBalanceModel>();
			string empty = string.Empty;
			foreach (IGrouping<string, GLInitBalanceModel> item in enumerable)
			{
				string accountId = item.Key;
				BDAccountModel accountModel = (from x in accountWithParentList
				where x.MItemID == accountId
				select x).FirstOrDefault();
				if (accountModel != null)
				{
					initBalanceList = item.ToList();
					if (accountWithParentList.Exists((BDAccountModel x) => x.MParentID == accountModel.MItemID))
					{
						result.Success = false;
						string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "IsNotLeafAccount", "科目:{0} 存在子级，不允许录入期初余额");
						string message = string.Format(text, accountModel.MNumber + "-" + accountModel.MName);
						result.Message = message;
						GLInitBalanceModel gLInitBalanceModel = initBalanceList.First();
						BizVerificationInfor erroInfo = GetErroInfo(message, 2, gLInitBalanceModel.DataOrigin, gLInitBalanceModel.MRowIndex, null);
						result.VerificationInfor.Add(erroInfo);
						list.Clear();
					}
					else
					{
						List<string> list3 = new List<string>();
						list3.Add(accountId);
						List<BDAccountModel> parentAccountByRecursion = BDAccountHelper.GetParentAccountByRecursion(accountModel, accountWithParentList);
						if (parentAccountByRecursion != null && parentAccountByRecursion.Count() > 0)
						{
							list3.AddRange((from x in parentAccountByRecursion
							select x.MItemID).ToList());
						}
						SqlWhere sqlWhere = new SqlWhere();
						sqlWhere.In("MAccountID", list3);
						List<GLInitBalanceModel> list4 = isCover ? null : dal.GetInitBalanceListIncludeCheckGroupValue(ctx, sqlWhere);
						list4 = ((list4 == null) ? new List<GLInitBalanceModel>() : list4);
						foreach (GLInitBalanceModel initBalance in initBalanceList)
						{
							ProcessAmountPrecision(initBalance, 2);
							initBalance.MCheckGroupValueModel = ((initBalance.MCheckGroupValueModel == null) ? new GLCheckGroupValueModel() : initBalance.MCheckGroupValueModel);
							GLCheckGroupValueModel checkGroupValueModel = new GLUtility().GetCheckGroupValueModel(ctx, initBalance.MCheckGroupValueModel);
							initBalance.OldCheckGroupValueID = (string.IsNullOrWhiteSpace(initBalance.MCheckGroupValueID) ? checkGroupValueModel.MItemID : initBalance.MCheckGroupValueID);
							initBalance.MCheckGroupValueID = checkGroupValueModel.MItemID;
							string empty2 = string.Empty;
							BDAccountModel bDAccountModel = (from x in accountWithParentList
							where x.MItemID == initBalance.MAccountID
							select x).FirstOrDefault();
							BDAccountModel mAccountModel = initBalance.MAccountModel;
							if (ctx.MOrgVersionID == 0 && bDAccountModel != null && (bDAccountModel.MCreateInitBill || (mAccountModel?.MCreateInitBill ?? false)))
							{
								list.AddRange(bDInitDocumentBusiness.GetInitDocumentInsertOrUpdateCmds(ctx, initBalance, empty, insertExchangeRateList, out empty2, out empty, isMatchBill));
							}
							foreach (BDAccountModel item2 in parentAccountByRecursion)
							{
								int balanceDirection = (item2.MDC == accountModel.MDC) ? 1 : (-1);
								ProcessUpdateInitBalanceList(ctx, initBalance, list4, list2, item2.MItemID, balanceDirection);
							}
							initBalance.MBillID = empty2;
							ProcessUpdateInitBalanceList(ctx, initBalance, list4, list2, initBalance.MAccountID, 1);
						}
						if (accountModel.MIsBankAccount & flag)
						{
							string mItemID = accountModel.MItemID;
							BDBankInitBalanceModel bankInitBalanceByBankId = BDBankAccountRepository.GetBankInitBalanceByBankId(ctx, mItemID);
							if (bankInitBalanceByBankId == null)
							{
								bankInitBalanceByBankId.MAccountID = accountModel.MItemID;
								bankInitBalanceByBankId.MCyID = accountModel.MCurrencyID;
							}
							BDBankBalanceModel bankBalanceModel = BDBankAccountRepository.GetBankBalanceModel(ctx, ctx.MBeginDate, ctx.MGLBeginDate.AddDays(-1.0), mItemID);
							if (bankBalanceModel != null)
							{
								BDBankInitBalanceModel bDBankInitBalanceModel = bankInitBalanceByBankId;
								bDBankInitBalanceModel.MBeginBalance += initBalanceList.Sum((GLInitBalanceModel x) => x.MInitBalance) - bankBalanceModel.MTotalAmt;
								BDBankInitBalanceModel bDBankInitBalanceModel2 = bankInitBalanceByBankId;
								bDBankInitBalanceModel2.MBeginBalanceFor += initBalanceList.Sum((GLInitBalanceModel x) => x.MInitBalanceFor) - bankBalanceModel.MTotalAmtFor;
							}
							else
							{
								bankInitBalanceByBankId.MBeginBalance = initBalanceList.Sum((GLInitBalanceModel x) => x.MInitBalance);
								bankInitBalanceByBankId.MBeginBalanceFor = initBalanceList.Sum((GLInitBalanceModel x) => x.MInitBalanceFor);
							}
							CommandInfo updateBankInitBalanceCmd = BDBankAccountRepository.GetUpdateBankInitBalanceCmd(ctx, bankInitBalanceByBankId);
							if (updateBankInitBalanceCmd != null)
							{
								list.Add(updateBankInitBalanceCmd);
							}
						}
					}
				}
			}
			if (list2.Count() > 0)
			{
				list.AddRange(ModelInfoManager.GetInsertOrUpdateCmds(ctx, list2, null, true));
			}
			return list;
		}

		private void ProcessAmountPrecision(GLInitBalanceModel initBalance, int decimalDigit)
		{
			initBalance.MInitBalance = Math.Round(initBalance.MInitBalance, decimalDigit);
			initBalance.MInitBalanceFor = Math.Round(initBalance.MInitBalanceFor, decimalDigit);
			initBalance.MYtdDebit = Math.Round(initBalance.MYtdDebit, decimalDigit);
			initBalance.MYtdDebitFor = Math.Round(initBalance.MYtdDebitFor, decimalDigit);
			initBalance.MYtdCredit = Math.Round(initBalance.MYtdCredit, decimalDigit);
			initBalance.MYtdCreditFor = Math.Round(initBalance.MYtdCreditFor, decimalDigit);
		}

		public void ProcessUpdateInitBalanceList(MContext ctx, GLInitBalanceModel initBalance, List<GLInitBalanceModel> initBalanceListInDB, List<GLInitBalanceModel> planUpdateList, string accountId, int balanceDirection)
		{
			bool flag = string.IsNullOrWhiteSpace(initBalance.MItemID) || initBalance.IsNew;
			GLInitBalanceModel initBalanceInDB = null;
			initBalanceInDB = (from x in planUpdateList
			where x.MAccountID == accountId && x.MCurrencyID == initBalance.MCurrencyID && x.MCheckGroupValueID == initBalance.MCheckGroupValueID
			select x).FirstOrDefault();
			if (initBalanceInDB == null)
			{
				initBalanceInDB = (from x in initBalanceListInDB
				where x.MAccountID == accountId && x.MCurrencyID == initBalance.MCurrencyID && x.MCheckGroupValueID == initBalance.OldCheckGroupValueID
				select x).FirstOrDefault();
			}
			if (initBalanceInDB == null)
			{
				initBalanceInDB = new GLInitBalanceModel();
				initBalanceInDB.MCheckGroupValueID = initBalance.MCheckGroupValueID;
				initBalanceInDB.MCheckGroupValueModel = initBalance.MCheckGroupValueModel;
				initBalanceInDB.MItemID = UUIDHelper.GetGuid();
				initBalanceInDB.MAccountID = accountId;
				initBalanceInDB.MCurrencyID = initBalance.MCurrencyID;
				initBalanceInDB.IsNew = true;
			}
			else
			{
				initBalanceInDB.MCheckGroupValueModel = initBalance.MCheckGroupValueModel;
				initBalanceInDB.MCheckGroupValueID = initBalance.MCheckGroupValueID;
			}
			if (initBalance.MAccountID == accountId)
			{
				initBalanceInDB.MBillID = initBalance.MBillID;
				initBalanceInDB.MContactType = initBalance.MContactType;
			}
			decimal d = default(decimal);
			decimal d2 = default(decimal);
			decimal d3 = default(decimal);
			decimal d4 = default(decimal);
			decimal d5 = default(decimal);
			decimal d6 = default(decimal);
			if (!flag)
			{
				if (initBalance.MAccountID == accountId)
				{
					d = initBalance.MInitBalance - initBalanceInDB.MInitBalance;
					d2 = initBalance.MInitBalanceFor - initBalanceInDB.MInitBalanceFor;
					d3 = initBalance.MYtdCredit - initBalanceInDB.MYtdCredit;
					d4 = initBalance.MYtdCreditFor - initBalanceInDB.MYtdCreditFor;
					d5 = initBalance.MYtdDebit - initBalanceInDB.MYtdDebit;
					d6 = initBalance.MYtdDebitFor - initBalanceInDB.MYtdDebitFor;
				}
				else
				{
					GLInitBalanceModel gLInitBalanceModel = (from x in initBalanceListInDB
					where x.MAccountID == initBalance.MAccountID && x.MCurrencyID == initBalance.MCurrencyID && x.MCheckGroupValueID == initBalance.OldCheckGroupValueID
					select x).FirstOrDefault();
					if (gLInitBalanceModel == null)
					{
						gLInitBalanceModel = new GLInitBalanceModel();
					}
					d = initBalance.MInitBalance - gLInitBalanceModel.MInitBalance;
					d2 = initBalance.MInitBalanceFor - gLInitBalanceModel.MInitBalanceFor;
					d3 = initBalance.MYtdCredit - gLInitBalanceModel.MYtdCredit;
					d4 = initBalance.MYtdCreditFor - gLInitBalanceModel.MYtdCreditFor;
					d5 = initBalance.MYtdDebit - gLInitBalanceModel.MYtdDebit;
					d6 = initBalance.MYtdDebitFor - gLInitBalanceModel.MYtdDebitFor;
				}
				GLInitBalanceModel gLInitBalanceModel2 = initBalanceInDB;
				gLInitBalanceModel2.MInitBalance += d * (decimal)balanceDirection;
				GLInitBalanceModel gLInitBalanceModel3 = initBalanceInDB;
				gLInitBalanceModel3.MInitBalanceFor += d2 * (decimal)balanceDirection;
				GLInitBalanceModel gLInitBalanceModel4 = initBalanceInDB;
				gLInitBalanceModel4.MYtdCredit += d3;
				GLInitBalanceModel gLInitBalanceModel5 = initBalanceInDB;
				gLInitBalanceModel5.MYtdCreditFor += d4;
				GLInitBalanceModel gLInitBalanceModel6 = initBalanceInDB;
				gLInitBalanceModel6.MYtdDebit += d5;
				GLInitBalanceModel gLInitBalanceModel7 = initBalanceInDB;
				gLInitBalanceModel7.MYtdDebitFor += d6;
			}
			else
			{
				GLInitBalanceModel gLInitBalanceModel8 = initBalanceInDB;
				gLInitBalanceModel8.MInitBalance += initBalance.MInitBalance * (decimal)balanceDirection;
				GLInitBalanceModel gLInitBalanceModel9 = initBalanceInDB;
				gLInitBalanceModel9.MInitBalanceFor += initBalance.MInitBalanceFor * (decimal)balanceDirection;
				GLInitBalanceModel gLInitBalanceModel10 = initBalanceInDB;
				gLInitBalanceModel10.MYtdCredit += initBalance.MYtdCredit;
				GLInitBalanceModel gLInitBalanceModel11 = initBalanceInDB;
				gLInitBalanceModel11.MYtdCreditFor += initBalance.MYtdCreditFor;
				GLInitBalanceModel gLInitBalanceModel12 = initBalanceInDB;
				gLInitBalanceModel12.MYtdDebit += initBalance.MYtdDebit;
				GLInitBalanceModel gLInitBalanceModel13 = initBalanceInDB;
				gLInitBalanceModel13.MYtdDebitFor += initBalance.MYtdDebitFor;
			}
			GLInitBalanceModel summaryInitBalanceInDB = null;
			summaryInitBalanceInDB = (from x in planUpdateList
			where x.MAccountID == accountId && x.MCurrencyID == initBalance.MCurrencyID && x.MCheckGroupValueID == "0"
			select x).FirstOrDefault();
			if (summaryInitBalanceInDB == null)
			{
				summaryInitBalanceInDB = (from x in initBalanceListInDB
				where x.MAccountID == accountId && x.MCurrencyID == initBalance.MCurrencyID && x.MCheckGroupValueID == "0"
				select x).FirstOrDefault();
			}
			if (summaryInitBalanceInDB == null)
			{
				summaryInitBalanceInDB = new GLInitBalanceModel();
				summaryInitBalanceInDB.MCurrencyID = initBalance.MCurrencyID;
				summaryInitBalanceInDB.MItemID = UUIDHelper.GetGuid();
				summaryInitBalanceInDB.MAccountID = accountId;
				summaryInitBalanceInDB.MCheckGroupValueID = "0";
				summaryInitBalanceInDB.IsNew = true;
			}
			if (!flag)
			{
				GLInitBalanceModel gLInitBalanceModel14 = summaryInitBalanceInDB;
				gLInitBalanceModel14.MInitBalance += d * (decimal)balanceDirection;
				GLInitBalanceModel gLInitBalanceModel15 = summaryInitBalanceInDB;
				gLInitBalanceModel15.MInitBalanceFor += d2 * (decimal)balanceDirection;
				GLInitBalanceModel gLInitBalanceModel16 = summaryInitBalanceInDB;
				gLInitBalanceModel16.MYtdCredit += d3;
				GLInitBalanceModel gLInitBalanceModel17 = summaryInitBalanceInDB;
				gLInitBalanceModel17.MYtdCreditFor += d4;
				GLInitBalanceModel gLInitBalanceModel18 = summaryInitBalanceInDB;
				gLInitBalanceModel18.MYtdDebit += d5;
				GLInitBalanceModel gLInitBalanceModel19 = summaryInitBalanceInDB;
				gLInitBalanceModel19.MYtdDebitFor += d6;
			}
			else
			{
				GLInitBalanceModel gLInitBalanceModel20 = summaryInitBalanceInDB;
				gLInitBalanceModel20.MInitBalance += initBalance.MInitBalance * (decimal)balanceDirection;
				GLInitBalanceModel gLInitBalanceModel21 = summaryInitBalanceInDB;
				gLInitBalanceModel21.MInitBalanceFor += initBalance.MInitBalanceFor * (decimal)balanceDirection;
				GLInitBalanceModel gLInitBalanceModel22 = summaryInitBalanceInDB;
				gLInitBalanceModel22.MYtdCredit += initBalance.MYtdCredit;
				GLInitBalanceModel gLInitBalanceModel23 = summaryInitBalanceInDB;
				gLInitBalanceModel23.MYtdCreditFor += initBalance.MYtdCreditFor;
				GLInitBalanceModel gLInitBalanceModel24 = summaryInitBalanceInDB;
				gLInitBalanceModel24.MYtdDebit += initBalance.MYtdDebit;
				GLInitBalanceModel gLInitBalanceModel25 = summaryInitBalanceInDB;
				gLInitBalanceModel25.MYtdDebitFor += initBalance.MYtdDebitFor;
			}
			if (!planUpdateList.Exists((GLInitBalanceModel x) => x.MItemID == initBalanceInDB.MItemID))
			{
				planUpdateList.Add(initBalanceInDB);
			}
			if (!planUpdateList.Exists((GLInitBalanceModel x) => x.MItemID == summaryInitBalanceInDB.MItemID))
			{
				planUpdateList.Add(summaryInitBalanceInDB);
			}
		}

		private GLInitBalanceModel GetNewestInitBalanceModel(MContext ctx, GLInitBalanceModel initBalance, GLInitBalanceModel initBalanceInDB)
		{
			GLInitBalanceModel gLInitBalanceModel = CloneInitBalanceModel(initBalance);
			if (initBalanceInDB != null)
			{
				gLInitBalanceModel.MItemID = initBalanceInDB.MItemID;
			}
			if (gLInitBalanceModel.MCheckGroupValueModel != null)
			{
				GLUtility gLUtility = new GLUtility();
				gLInitBalanceModel.MCheckGroupValueModel.MItemID = null;
				GLCheckGroupValueModel gLCheckGroupValueModel = gLInitBalanceModel.MCheckGroupValueModel = gLUtility.GetCheckGroupValueModel(ctx, initBalance.MCheckGroupValueModel);
				gLInitBalanceModel.MCheckGroupValueID = gLCheckGroupValueModel.MItemID;
			}
			return gLInitBalanceModel;
		}

		private GLInitBalanceModel GetSummaryInitBalanceModel(GLInitBalanceModel initBalance, List<GLInitBalanceModel> initBalanceList)
		{
			GLInitBalanceModel gLInitBalanceModel = (from x in initBalanceList
			where x.MAccountID == initBalance.MAccountID && x.MCurrencyID == initBalance.MCurrencyID
			select x).FirstOrDefault();
			if (gLInitBalanceModel == null)
			{
				gLInitBalanceModel = CloneInitBalanceModel(initBalance);
				gLInitBalanceModel.MCheckGroupValueID = "0";
			}
			return gLInitBalanceModel;
		}

		private GLInitBalanceModel CloneInitBalanceModel(GLInitBalanceModel initBalance)
		{
			GLInitBalanceModel gLInitBalanceModel = new GLInitBalanceModel();
			gLInitBalanceModel.MCheckGroupValueID = initBalance.MCheckGroupValueID;
			gLInitBalanceModel.MCheckGroupValueModel = initBalance.MCheckGroupValueModel;
			gLInitBalanceModel.MAccountID = initBalance.MAccountID;
			gLInitBalanceModel.MCurrencyID = initBalance.MCurrencyID;
			gLInitBalanceModel.MInitBalance = initBalance.MInitBalance;
			gLInitBalanceModel.MInitBalanceFor = initBalance.MInitBalanceFor;
			gLInitBalanceModel.MYtdCredit = initBalance.MYtdCredit;
			gLInitBalanceModel.MYtdCreditFor = initBalance.MYtdCreditFor;
			gLInitBalanceModel.MYtdDebit = initBalance.MYtdDebit;
			gLInitBalanceModel.MYtdDebitFor = initBalance.MYtdDebitFor;
			return gLInitBalanceModel;
		}

		private List<CommandInfo> GetCreateInitDocumnetCmds(MContext ctx, List<GLInitBalanceModel> initBalance, BDAccountEditModel accountModel)
		{
			List<CommandInfo> result = new List<CommandInfo>();
			BDInitDocumentBusiness bDInitDocumentBusiness = new BDInitDocumentBusiness();
			return result;
		}

		public List<GLInitBalanceModel> ConvertToInitBalanceList(List<GLBalanceModel> balanceList)
		{
			List<GLInitBalanceModel> list = new List<GLInitBalanceModel>();
			foreach (GLBalanceModel balance in balanceList)
			{
				GLInitBalanceModel gLInitBalanceModel = new GLInitBalanceModel();
				gLInitBalanceModel.MAccountID = balance.MAccountID;
				gLInitBalanceModel.MInitBalance = balance.MBeginBalance;
				gLInitBalanceModel.MInitBalanceFor = balance.MBeginBalanceFor;
				gLInitBalanceModel.MYearPeriod = balance.MYearPeriod;
				gLInitBalanceModel.MCurrencyID = balance.MCurrencyID;
				gLInitBalanceModel.MYtdCredit = balance.MYtdCredit;
				gLInitBalanceModel.MYtdCreditFor = balance.MYtdCreditFor;
				gLInitBalanceModel.MYtdDebit = balance.MYtdDebit;
				gLInitBalanceModel.MYtdDebitFor = balance.MYtdDebitFor;
				gLInitBalanceModel.MCheckGroupValueID = balance.MCheckGroupValueID;
				list.Add(gLInitBalanceModel);
			}
			return list;
		}

		public OperationResult ImportInitBalanceList(MContext ctx, List<GLInitBalanceModel> list)
		{
			List<CommandInfo> list2 = new List<CommandInfo>();
			OperationResult operationResult = ValidateData(ctx, list, list2, true);
			if (!operationResult.Success)
			{
				return operationResult;
			}
			List<CommandInfo> list3 = new List<CommandInfo>();
			List<CommandInfo> clearInitBalanceCmds = GetClearInitBalanceCmds(ctx, null);
			if (clearInitBalanceCmds != null)
			{
				list3.AddRange(clearInitBalanceCmds);
			}
			list3.AddRange(list2);
			operationResult.Success = (BDRepository.ExecuteSqlTran(ctx, list3) > 0);
			return operationResult;
		}

		public OperationResult ValidateData(MContext ctx, List<GLInitBalanceModel> list)
		{
			List<CommandInfo> cmdList = new List<CommandInfo>();
			return ValidateData(ctx, list, cmdList, true);
		}

		private OperationResult ValidateData(MContext ctx, List<GLInitBalanceModel> list, List<CommandInfo> cmdList, bool isMatchBill = true)
		{
			OperationResult operationResult = new OperationResult();
			operationResult.Success = true;
			if (ctx.MInitBalanceOver)
			{
				operationResult.Success = false;
				string text = COMMultiLangRepository.GetText(LangModule.Common, "NotAllowImportBecauseInitBalanceOver", "科目期初余额已完成了初始化，不允许在进行科目覆盖导入！");
				int displayType = 1;
				BizVerificationInfor erroInfo = GetErroInfo(text, displayType, null, -1, "");
				operationResult.VerificationInfor.Add(erroInfo);
				return operationResult;
			}
			if (list == null || list.Count() == 0)
			{
				operationResult.Success = false;
				string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "NonExistentInitBalanceRecord", "没有需要导入的期初余额！");
				int displayType2 = 1;
				BizVerificationInfor erroInfo2 = GetErroInfo(text2, displayType2, null, -1, "");
				operationResult.VerificationInfor.Add(erroInfo2);
				return operationResult;
			}
			List<GLInitBalanceModel> list2 = new List<GLInitBalanceModel>();
			LanguageList = new BASLangBusiness().GetOrgLangList(ctx);
			StringBuilder stringBuilder = new StringBuilder();
			List<string> list3 = new List<string>();
			List<string> list4 = new List<string>();
			List<BDBankAccountEditModel> modelList = new BDBankAccountRepository().GetModelList(ctx, new SqlWhere(), false);
			foreach (GLInitBalanceModel item in list)
			{
				if (!item.IsEmptyData)
				{
					BizVerificationInfor bizVerificationInfor = VaildateImportRecordAccount(ctx, item, cmdList);
					if (bizVerificationInfor != null)
					{
						operationResult.Success = false;
						operationResult.VerificationInfor.Add(bizVerificationInfor);
					}
					List<BizVerificationInfor> list5 = ValidataInitBalanceResult(ctx, item);
					if (list5 != null && list5.Count > 0)
					{
						operationResult.Success = false;
						operationResult.VerificationInfor.AddRange(list5);
					}
					List<BizVerificationInfor> list6 = VaildataImportRecordCurrency(ctx, item, modelList);
					if (list6.Count > 0)
					{
						operationResult.Success = false;
						operationResult.VerificationInfor.AddRange(list6);
					}
					if (item.MCheckGroupValueID != "0")
					{
						List<BizVerificationInfor> list7 = ValidateImportRecordCheckType(ctx, item, cmdList);
						if (list7.Count > 0)
						{
							operationResult.Success = (!operationResult.Success && false);
							operationResult.VerificationInfor.AddRange(list7);
						}
					}
					list2.Add(item);
				}
			}
			list = (from x in list2
			where x.MInitBalance != decimal.Zero || x.MInitBalanceFor != decimal.Zero || x.MYtdCredit != decimal.Zero || x.MYtdCreditFor != decimal.Zero || x.MYtdDebit != decimal.Zero || x.MYtdDebitFor != decimal.Zero || x.MInitCreditBalance != decimal.Zero || x.MInitCreditBalanceFor != decimal.Zero || x.MInitDebitBalance != decimal.Zero || x.MInitDebitBalanceFor != decimal.Zero
			select x).ToList();
			if (list.Count() == 0)
			{
				operationResult.Success = false;
				string text3 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "NonExistentInitBalanceRecord", "没有需要导入的期初余额！");
				int displayType3 = 1;
				BizVerificationInfor erroInfo3 = GetErroInfo(text3, displayType3, null, -1, "");
				operationResult.VerificationInfor.Add(erroInfo3);
				return operationResult;
			}
			List<BizVerificationInfor> list8 = ValidateImportInitbalanceList(ctx, list, cmdList, false);
			if (list8.Count() > 0)
			{
				operationResult.VerificationInfor.AddRange(list8);
			}
			if (cmdList.Count == 0 && operationResult.VerificationInfor.Count == 0)
			{
				operationResult.Success = false;
				string text4 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "NonExistentInitBalanceRecord", "没有需要导入的期初余额！");
				int displayType4 = 1;
				BizVerificationInfor erroInfo4 = GetErroInfo(text4, displayType4, null, -1, "");
				operationResult.VerificationInfor.Add(erroInfo4);
				return operationResult;
			}
			return operationResult;
		}

		private bool IsEmptyInitBalance(GLInitBalanceModel initBalance)
		{
			if (initBalance.MInitBalance != decimal.Zero || initBalance.MInitBalanceFor != decimal.Zero || initBalance.MYtdCredit != decimal.Zero || initBalance.MYtdCreditFor != decimal.Zero || initBalance.MYtdDebit != decimal.Zero || initBalance.MYtdDebitFor != decimal.Zero || initBalance.MInitCreditBalance != decimal.Zero || initBalance.MInitCreditBalanceFor != decimal.Zero || initBalance.MInitDebitBalance != decimal.Zero || initBalance.MInitDebitBalanceFor != decimal.Zero || !string.IsNullOrWhiteSpace(initBalance.MCurrencyID))
			{
				return false;
			}
			GLCheckGroupValueModel mCheckGroupValueModel = initBalance.MCheckGroupValueModel;
			if (mCheckGroupValueModel != null && (!string.IsNullOrWhiteSpace(mCheckGroupValueModel.MContactID) || !string.IsNullOrWhiteSpace(mCheckGroupValueModel.MEmployeeID) || !string.IsNullOrWhiteSpace(mCheckGroupValueModel.MExpItemID) || !string.IsNullOrWhiteSpace(mCheckGroupValueModel.MMerItemID) || !string.IsNullOrWhiteSpace(mCheckGroupValueModel.MPaItemGroupID) || !string.IsNullOrWhiteSpace(mCheckGroupValueModel.MPaItemID) || !string.IsNullOrWhiteSpace(mCheckGroupValueModel.MTrackItem1) || !string.IsNullOrWhiteSpace(mCheckGroupValueModel.MTrackItem2) || !string.IsNullOrWhiteSpace(mCheckGroupValueModel.MTrackItem3) || !string.IsNullOrWhiteSpace(mCheckGroupValueModel.MTrackItem4) || !string.IsNullOrWhiteSpace(mCheckGroupValueModel.MTrackItem5)))
			{
				return false;
			}
			return true;
		}

		private void SetInitBillData(MContext ctx, GLInitBalanceModel initBalance, BDAccountModel account)
		{
			GLCheckGroupValueModel mCheckGroupValueModel;
			int num;
			if (BDAccountHelper.IsCurrentAccount(account.MCode) && !(initBalance.MInitBalance <= decimal.Zero) && !(initBalance.MInitBalanceFor <= decimal.Zero))
			{
				mCheckGroupValueModel = initBalance.MCheckGroupValueModel;
				if (mCheckGroupValueModel != null)
				{
					string mCode = account.MCode;
					initBalance.MAccountCode = account.MCode;
					if (mCode.IndexOf("1122") == 0)
					{
						initBalance.MBillType = "Invoice_Sale";
						if (string.IsNullOrWhiteSpace(mCheckGroupValueModel.MContactID))
						{
							initBalance.MBillType = "";
						}
						else
						{
							initBalance.MContactTypeFromBill = "Customer";
						}
					}
					else if (mCode.IndexOf("2202") == 0)
					{
						initBalance.MBillType = "Invoice_Purchase";
						if (string.IsNullOrWhiteSpace(mCheckGroupValueModel.MContactID))
						{
							initBalance.MBillType = "";
						}
						else
						{
							initBalance.MContactTypeFromBill = "Supplier";
						}
					}
					else
					{
						if (mCode.IndexOf("2203") == 0 || mCode.IndexOf("2241") == 0)
						{
							initBalance.MBillType = "Receive_Sale";
							initBalance.MContactTypeFromBill = ((mCode.IndexOf("2203") == 0) ? "Customer" : "Other");
							if (initBalance.MContactTypeFromBill == "Supplier" || initBalance.MContactTypeFromBill == "Employees" || (string.IsNullOrWhiteSpace(mCheckGroupValueModel.MContactID) && string.IsNullOrWhiteSpace(mCheckGroupValueModel.MEmployeeID)))
							{
								initBalance.MBillType = "";
								return;
							}
							if (!(initBalance.MContactTypeFromBill == "Supplier") && !(initBalance.MContactTypeFromBill == "Customer") && !(initBalance.MContactTypeFromBill == "Other"))
							{
								goto IL_027d;
							}
							if (!string.IsNullOrWhiteSpace(mCheckGroupValueModel.MContactID))
							{
								goto IL_027d;
							}
							num = 1;
							goto IL_02a5;
						}
						if (mCode.IndexOf("1123") == 0 || mCode.IndexOf("1221") == 0)
						{
							initBalance.MBillType = "Pay_Purchase";
							if (mCode.IndexOf("1123") == 0)
							{
								initBalance.MContactTypeFromBill = ((!string.IsNullOrWhiteSpace(mCheckGroupValueModel.MContactID)) ? "Supplier" : "Other");
							}
							else if (mCode.IndexOf("1221") == 0)
							{
								initBalance.MContactTypeFromBill = ((!string.IsNullOrWhiteSpace(mCheckGroupValueModel.MEmployeeID)) ? "Employees" : "Other");
							}
							if (initBalance.MContactTypeFromBill == "Customer" || (string.IsNullOrWhiteSpace(mCheckGroupValueModel.MContactID) && string.IsNullOrWhiteSpace(mCheckGroupValueModel.MEmployeeID)))
							{
								initBalance.MBillType = "";
							}
							else
							{
								if (BankList.Count() == 0)
								{
									List<BDBankAccountEditModel> bankAccountList = BDBankAccountRepository.GetBankAccountList(ctx, null, false, null);
									if (bankAccountList != null)
									{
										BankList.AddRange(bankAccountList);
									}
								}
								BDBankAccountModel bDBankAccountModel = (from x in BankList
								where x.MCyID == initBalance.MCurrencyID
								select x).FirstOrDefault();
								if (bDBankAccountModel != null)
								{
									initBalance.MBankID = bDBankAccountModel.MItemID;
								}
								else
								{
									initBalance.MBillType = "";
								}
							}
						}
					}
				}
			}
			return;
			IL_027d:
			num = ((initBalance.MContactTypeFromBill == "Employees" && string.IsNullOrWhiteSpace(mCheckGroupValueModel.MEmployeeID)) ? 1 : 0);
			goto IL_02a5;
			IL_02a5:
			if (num != 0)
			{
				initBalance.MBillType = "";
			}
			else
			{
				if (BankList.Count() == 0)
				{
					List<BDBankAccountEditModel> bankAccountList2 = BDBankAccountRepository.GetBankAccountList(ctx, null, false, null);
					if (bankAccountList2 != null)
					{
						BankList.AddRange(bankAccountList2);
					}
				}
				BDBankAccountModel bDBankAccountModel2 = (from x in BankList
				where x.MCyID == initBalance.MCurrencyID
				select x).FirstOrDefault();
				if (bDBankAccountModel2 != null)
				{
					initBalance.MBankID = bDBankAccountModel2.MItemID;
				}
				else
				{
					initBalance.MBillType = "";
				}
			}
		}

		private List<BizVerificationInfor> ValidateImportInitbalanceList(MContext ctx, List<GLInitBalanceModel> list, List<CommandInfo> cmdList, bool isMatchBill = true)
		{
			List<BizVerificationInfor> list2 = new List<BizVerificationInfor>();
			BDAccountRepository bDAccountRepository = new BDAccountRepository();
			if (GlobalAccountList.Count() == 0)
			{
				List<BDAccountModel> accountListWithCheckType = bDAccountRepository.GetAccountListWithCheckType(ctx, null, false, false);
				if (accountListWithCheckType != null)
				{
					GlobalAccountList.AddRange(accountListWithCheckType);
				}
			}
			List<BDAccountModel> childrenList = BDAccountHelper.GetChildrenList(GlobalAccountList);
			List<GLInitBalanceModel> list3 = new List<GLInitBalanceModel>();
			GLCheckTypeBusiness gLCheckTypeBusiness = new GLCheckTypeBusiness();
			foreach (BDAccountModel item in childrenList)
			{
				List<GLInitBalanceModel> list4 = (from x in list
				where x.MAccountID == item.MItemID
				select x).ToList();
				List<GLInitBalanceModel> list5 = (from x in list
				where x.MAccountID == item.MItemID && x.MCheckGroupValueID == "0"
				select x).ToList();
				List<GLInitBalanceModel> list6 = (from x in list
				where x.MAccountID == item.MItemID && x.MCheckGroupValueID != "0"
				select x).ToList();
				int num = list5?.Count() ?? 0;
				int num2 = list6?.Count() ?? 0;
				if (ctx.MOrgVersionID != 1 && num2 > 0)
				{
					foreach (GLInitBalanceModel item2 in list6)
					{
						SetInitBillData(ctx, item2, item);
					}
				}
				if (num != 0 || num2 != 0)
				{
					if (num > 0 && num2 == 0)
					{
						if (item.MCheckGroupID == "0")
						{
							list3.AddRange(list5);
						}
						else if (gLCheckTypeBusiness.IsExistRequireCheckType(item))
						{
							GLInitBalanceModel gLInitBalanceModel = list5.First();
							string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "AccountNeedCheckTypeInitBalance", "科目:{0}需要录入具体核算维度期初余额");
							string message = string.Format(text, item.MNumber + "-" + item.MName);
							int displayType = 2;
							BizVerificationInfor erroInfo = GetErroInfo(message, displayType, gLInitBalanceModel.DataOrigin, gLInitBalanceModel.MRowIndex, null);
							list2.Add(erroInfo);
						}
						else
						{
							GLCheckGroupValueModel checkGroupValueModel = new GLUtility().GetCheckGroupValueModel(ctx, new GLCheckGroupValueModel());
							list5.ForEach(delegate(GLInitBalanceModel x)
							{
								x.MCheckGroupValueID = checkGroupValueModel.MItemID;
							});
							list3.AddRange(list5);
						}
					}
					else if (num == 0 && num2 > 0)
					{
						list3.AddRange(list6);
					}
					else if (num > 0 && num2 > 0)
					{
						IEnumerable<IGrouping<string, GLInitBalanceModel>> enumerable = from x in list5
						group x by x.MCurrencyID;
						foreach (IGrouping<string, GLInitBalanceModel> item3 in enumerable)
						{
							string currencyId = item3.Key;
							List<GLInitBalanceModel> source = item3.ToList();
							List<GLInitBalanceModel> list7 = (from x in list6
							where x.MCurrencyID == currencyId
							select x).ToList();
							GLInitBalanceModel gLInitBalanceModel2 = source.First();
							if (list7 == null || list7.Count() == 0)
							{
								string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "AccountNeedCheckTypeInitBalance", "科目:{0}需要录入具体核算维度期初余额");
								string message2 = string.Format(text2, item.MNumber + "-" + item.MName);
								int displayType2 = 2;
								BizVerificationInfor erroInfo2 = GetErroInfo(message2, displayType2, gLInitBalanceModel2.DataOrigin, gLInitBalanceModel2.MRowIndex, null);
								list2.Add(erroInfo2);
							}
							else
							{
								decimal d = source.Sum((GLInitBalanceModel x) => x.MInitBalance);
								decimal d2 = list7.Sum((GLInitBalanceModel x) => x.MInitBalance);
								decimal d3 = source.Sum((GLInitBalanceModel x) => x.MInitBalanceFor);
								decimal d4 = list7.Sum((GLInitBalanceModel x) => x.MInitBalanceFor);
								if (d != d2 || d3 != d4)
								{
									string text3 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "InitBalanceNotEqual", "科目:{0}录入的汇总期初金额和核算维度期初金额不相等");
									string message3 = string.Format(text3, item.MNumber + "-" + item.MName);
									int displayType3 = 2;
									BizVerificationInfor erroInfo3 = GetErroInfo(message3, displayType3, gLInitBalanceModel2.DataOrigin, gLInitBalanceModel2.MRowIndex, null);
									list2.Add(erroInfo3);
								}
								decimal d5 = source.Sum((GLInitBalanceModel x) => x.MYtdCredit);
								decimal d6 = list7.Sum((GLInitBalanceModel x) => x.MYtdCredit);
								decimal d7 = source.Sum((GLInitBalanceModel x) => x.MYtdCreditFor);
								decimal d8 = list7.Sum((GLInitBalanceModel x) => x.MYtdCreditFor);
								if (d5 != d6 || d7 != d8)
								{
									string text4 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "YTDCreditNotEqual", "科目:{0}录入的汇总本年累计贷方和核算维度年累计贷方金额不相等");
									string message4 = string.Format(text4, item.MNumber + "-" + item.MName);
									int displayType4 = 2;
									BizVerificationInfor erroInfo4 = GetErroInfo(message4, displayType4, gLInitBalanceModel2.DataOrigin, gLInitBalanceModel2.MRowIndex, null);
									list2.Add(erroInfo4);
								}
								decimal d9 = source.Sum((GLInitBalanceModel x) => x.MYtdDebit);
								decimal d10 = list7.Sum((GLInitBalanceModel x) => x.MYtdDebit);
								decimal d11 = source.Sum((GLInitBalanceModel x) => x.MYtdDebitFor);
								decimal d12 = list7.Sum((GLInitBalanceModel x) => x.MYtdDebitFor);
								if (d9 != d10 || d11 != d12)
								{
									string text5 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "YTDDebitNotEqual", "科目:{0}录入的汇总本年累计借方和核算维度年累计借方金额不相等");
									string message5 = string.Format(text5, item.MNumber + "-" + item.MName);
									int displayType5 = 2;
									BizVerificationInfor erroInfo5 = GetErroInfo(message5, displayType5, gLInitBalanceModel2.DataOrigin, gLInitBalanceModel2.MRowIndex, null);
									list2.Add(erroInfo5);
								}
								list3.AddRange(list7);
							}
						}
					}
				}
			}
			if (list3.Count() > 0)
			{
				OperationResult operationResult = new OperationResult();
				List<CommandInfo> updateInitBalanceCommandList = GetUpdateInitBalanceCommandList(ctx, list3, out operationResult, true, isMatchBill);
				if (!operationResult.Success)
				{
					if (operationResult.VerificationInfor != null && operationResult.VerificationInfor.Count > 0)
					{
						list2.AddRange(operationResult.VerificationInfor);
					}
				}
				else
				{
					cmdList.AddRange(updateInitBalanceCommandList);
				}
			}
			return list2;
		}

		private BizVerificationInfor VaildateImportRecordAccount(MContext ctx, GLInitBalanceModel initBalance, List<CommandInfo> cmdList)
		{
			BizVerificationInfor result = null;
			BDAccountRepository bDAccountRepository = new BDAccountRepository();
			if (GlobalAccountList.Count() == 0)
			{
				List<BDAccountModel> accountListWithCheckType = bDAccountRepository.GetAccountListWithCheckType(ctx, null, true, true);
				if (accountListWithCheckType != null)
				{
					GlobalAccountList.AddRange(accountListWithCheckType);
				}
			}
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "ImportInitBalanceRecordNotLegal", "导入的期初余额数据不合法:");
			BDAccountModel accountInInitBalance = initBalance.MAccountModel;
			if (accountInInitBalance == null || string.IsNullOrWhiteSpace(accountInInitBalance.MNumber))
			{
				string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "ImportInitBalanceError1", "期初余额记录没有填写科目代码和名称,记录详情：{0}");
				string message = string.Format(text2, "?-" + initBalance.MCurrencyID + "-" + initBalance.MInitBalanceFor + "-" + initBalance.MInitBalance);
				return GetErroInfo(message, 2, initBalance.DataOrigin, initBalance.MRowIndex, "MNumber");
			}
			BDAccountModel bDAccountModel = (from x in GlobalAccountList
			where x.MNumber == accountInInitBalance.MNumber.Trim()
			select x).FirstOrDefault();
			if (bDAccountModel == null)
			{
				string text3 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "ImportInitBalanceError2", "期初余额记录中科目不存在：{0}");
				string message2 = string.Format(text3, accountInInitBalance.MNumber + "-" + accountInInitBalance.MName);
				return GetErroInfo(message2, 2, initBalance.DataOrigin, initBalance.MRowIndex, "MNumber");
			}
			if (!bDAccountModel.MIsActive)
			{
				string text4 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "AccountIsDisabled", "科目已经禁用，不能导入科目期初余额:{0}");
				string message3 = string.Format(text4, accountInInitBalance.MNumber + "-" + accountInInitBalance.MName);
				return GetErroInfo(message3, 2, initBalance.DataOrigin, initBalance.MRowIndex, "MNumber");
			}
			if (BDAccountHelper.IsProfitLossAccount(ctx, bDAccountModel) && (initBalance.MInitBalance != decimal.Zero || initBalance.MInitBalanceFor != decimal.Zero || initBalance.MInitDebitBalance != decimal.Zero || initBalance.MInitDebitBalanceFor != decimal.Zero || initBalance.MInitCreditBalance != decimal.Zero || initBalance.MInitCreditBalanceFor != decimal.Zero))
			{
				string text5 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "ImportInitBalanceError6", "损益类科目:{0}，不允许录入期初金额！");
				string message4 = string.Format(text5, accountInInitBalance.MNumber + "-" + accountInInitBalance.MName);
				return GetErroInfo(message4, 2, initBalance.DataOrigin, initBalance.MRowIndex, "MNumber");
			}
			initBalance.MAccountID = bDAccountModel.MItemID;
			if (bDAccountModel.MCreateInitBill != initBalance.MAccountModel.MCreateInitBill)
			{
				bDAccountModel.MCreateInitBill = (initBalance.MAccountModel.MCreateInitBill || bDAccountModel.MCreateInitBill);
				List<string> list = new List<string>();
				list.Add("MCreateInitBill");
				cmdList.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDAccountModel>(ctx, bDAccountModel, list, true));
			}
			initBalance.MAccountModel = bDAccountModel;
			return result;
		}

		private List<BizVerificationInfor> ValidataInitBalanceResult(MContext ctx, GLInitBalanceModel initBalance)
		{
			List<BizVerificationInfor> list = new List<BizVerificationInfor>();
			if (initBalance.MInitBalance != decimal.Zero || initBalance.MInitBalanceFor != decimal.Zero)
			{
				return list;
			}
			BDAccountModel mAccountModel = initBalance.MAccountModel;
			if ((initBalance.MInitDebitBalance != decimal.Zero || initBalance.MInitDebitBalanceFor != decimal.Zero) && (initBalance.MInitCreditBalance != decimal.Zero || initBalance.MInitCreditBalanceFor != decimal.Zero))
			{
				string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "InitBalanceOnlyOneDirection", "科目期初余额只能填写一个方向，且方向需和科目方向一致");
				BizVerificationInfor erroInfo = GetErroInfo(text, 2, initBalance.DataOrigin, initBalance.MRowIndex, null);
				list.Add(erroInfo);
			}
			if (mAccountModel.MDC == 1)
			{
				if (initBalance.MInitCreditBalance != decimal.Zero || initBalance.MInitCreditBalanceFor != decimal.Zero)
				{
					string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "DebitInitBalanceDirectionError", "科目：{0}是借方科目，只能填写借方期初余额");
					text2 = string.Format(text2, mAccountModel.MNumber);
					BizVerificationInfor erroInfo2 = GetErroInfo(text2, 2, initBalance.DataOrigin, initBalance.MRowIndex, null);
					list.Add(erroInfo2);
				}
				else
				{
					initBalance.MInitBalance = initBalance.MInitDebitBalance;
					initBalance.MInitBalanceFor = initBalance.MInitDebitBalanceFor;
				}
			}
			else if (mAccountModel.MDC == -1)
			{
				if (initBalance.MInitDebitBalance != decimal.Zero || initBalance.MInitDebitBalanceFor != decimal.Zero)
				{
					string text3 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "CreditInitBalanceDirectionError", "科目：{0}是贷方科目，只能填写贷方期初余额");
					text3 = string.Format(text3, mAccountModel.MNumber);
					BizVerificationInfor erroInfo3 = GetErroInfo(text3, 2, initBalance.DataOrigin, initBalance.MRowIndex, null);
					list.Add(erroInfo3);
				}
				else
				{
					initBalance.MInitBalance = initBalance.MInitCreditBalance;
					initBalance.MInitBalanceFor = initBalance.MInitCreditBalanceFor;
				}
			}
			return list;
		}

		private List<BizVerificationInfor> ValidateImportRecordCheckType(MContext ctx, GLInitBalanceModel initBalance, List<CommandInfo> cmdList)
		{
			List<BizVerificationInfor> list = new List<BizVerificationInfor>();
			GLCheckGroupValueModel checkGroupValueModel = initBalance.MCheckGroupValueModel;
			if (cmdList == null)
			{
				cmdList = new List<CommandInfo>();
			}
			if (initBalance.MCheckGroupValueModel == null)
			{
				return list;
			}
			BDAccountModel mAccountModel = initBalance.MAccountModel;
			if (mAccountModel.MCheckGroupID != "0" && !ValidateInitBalanceCheckTypeIsPass(ctx, mAccountModel, initBalance, out list))
			{
				return list;
			}
			if (!string.IsNullOrWhiteSpace(checkGroupValueModel.MContactID))
			{
				if (ContactList.Count() == 0)
				{
					BDContactsInfoFilterModel bDContactsInfoFilterModel = new BDContactsInfoFilterModel();
					bDContactsInfoFilterModel.PageSize = 2147483647;
					List<BDContactsInfoModel> contactsListByContactType = new BDContactsRepository().GetContactsListByContactType(ctx, 0, null, 0, true, true);
					if (contactsListByContactType != null)
					{
						ContactList.AddRange(contactsListByContactType);
					}
				}
				BDContactsInfoModel bDContactsInfoModel = (from x in ContactList
				where x.MContactName.EqualsIgnoreCase(checkGroupValueModel.MContactID)
				select x).FirstOrDefault();
				if (bDContactsInfoModel == null)
				{
					string mContactID = checkGroupValueModel.MContactID;
					BDContactsInfoModel insertContactModel = new BDContactsBusiness().GetInsertContactModel(ctx, mContactID, LanguageList);
					checkGroupValueModel.MContactID = insertContactModel.MItemID;
					BDAccountModel mAccountModel2 = initBalance.MAccountModel;
					string text = "";
					SetContactType(mAccountModel2, insertContactModel, ref text);
					insertContactModel.MultiLanguage = new FPUtility().GetMultiLanguage(ctx, "MName", mContactID);
					new BDContactsRepository().MultiLanguageAdd(insertContactModel);
					cmdList.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDContactsInfoModel>(ctx, insertContactModel, null, true));
					ContactList.Add(insertContactModel);
				}
				else if (!bDContactsInfoModel.MIsActive)
				{
					string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "ContactDisabledErrorTips", "联系人：{0}已经禁用！");
					string message = string.Format(text2, checkGroupValueModel.MContactID);
					BizVerificationInfor erroInfo = GetErroInfo(message, 2, initBalance.DataOrigin, initBalance.MRowIndex, "MContactID");
					list.Add(erroInfo);
				}
				else
				{
					checkGroupValueModel.MContactID = bDContactsInfoModel.MItemID;
					string text3 = "";
					if (SetContactType(initBalance.MAccountModel, bDContactsInfoModel, ref text3))
					{
						List<string> list2 = new List<string>();
						if (!string.IsNullOrWhiteSpace(text3))
						{
							list2.Add(text3);
						}
						bDContactsInfoModel.IsNew = false;
						cmdList.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDContactsInfoModel>(ctx, bDContactsInfoModel, list2, false));
					}
				}
			}
			if (!string.IsNullOrWhiteSpace(checkGroupValueModel.MEmployeeID))
			{
				if (EmployeeList.Count() == 0)
				{
					List<GLTreeModel> mDataList = GLUnility.GetAllEmployeeCheckTypeData(ctx).MDataList;
					if (mDataList != null)
					{
						EmployeeList.AddRange(mDataList);
					}
				}
				GLTreeModel gLTreeModel = (from x in EmployeeList
				where x.text.EqualsIgnoreCase(checkGroupValueModel.MEmployeeID)
				select x).FirstOrDefault();
				string guid = UUIDHelper.GetGuid();
				string text4 = checkGroupValueModel.MEmployeeID.Trim();
				if (gLTreeModel == null)
				{
					string[] array = EmployeeBLL.AnalysisEmployeeName(ctx, text4).ToArray();
					if (array.Length < 2)
					{
						string text5 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "EmployeeNameErrorTips", "员工：{0}名称不合法,如果是手动新增，姓和名请用1个空格隔开");
						string message2 = string.Format(text5, checkGroupValueModel.MEmployeeID);
						BizVerificationInfor erroInfo2 = GetErroInfo(message2, 2, initBalance.DataOrigin, initBalance.MRowIndex, "MEmployeeID");
						list.Add(erroInfo2);
					}
					else
					{
						BDEmployeesModel insertEmployeeModel = EmployeeBLL.GetInsertEmployeeModel(ctx, array[0], array[1], LanguageList);
						checkGroupValueModel.MEmployeeID = insertEmployeeModel.MItemID;
						cmdList.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDEmployeesModel>(ctx, insertEmployeeModel, null, true));
						EmployeeList.Add(new GLTreeModel
						{
							id = insertEmployeeModel.MItemID,
							text = text4
						});
					}
				}
				else if (!gLTreeModel.MIsActive)
				{
					string text6 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "EmployeeDisabledErrorTips", "员工{0}已经禁用！");
					string message3 = string.Format(text6, text4);
					BizVerificationInfor erroInfo3 = GetErroInfo(message3, 2, initBalance.DataOrigin, initBalance.MRowIndex, "MEmployeeID");
					list.Add(erroInfo3);
				}
				else
				{
					checkGroupValueModel.MEmployeeID = gLTreeModel.id;
				}
			}
			if (!string.IsNullOrWhiteSpace(checkGroupValueModel.MMerItemID))
			{
				if (MerItemList.Count() == 0)
				{
					List<GLTreeModel> mDataList2 = GLUnility.GetMerItemCheckTypeData(ctx).MDataList;
					if (mDataList2 != null)
					{
						MerItemList.AddRange(mDataList2);
					}
				}
				string[] array2 = checkGroupValueModel.MMerItemID.Split(':', '：');
				string code = "";
				string desc = "";
				if (array2.Length == 1)
				{
					code = array2[0].Trim();
					desc = array2[0].Trim();
				}
				else if (array2.Length >= 2)
				{
					code = array2[0].Trim();
					desc = checkGroupValueModel.MMerItemID.Substring(code.Length + 1, checkGroupValueModel.MMerItemID.Length - code.Length - 1);
				}
				GLTreeModel gLTreeModel2 = (from x in MerItemList
				where x.text.EqualsIgnoreCase(code) || x.text.EqualsIgnoreCase(desc) || x.text.EqualsIgnoreCase(code + ":" + desc)
				select x).FirstOrDefault();
				if (gLTreeModel2 == null)
				{
					BDItemModel insertItemModel = new BDItemBusiness().GetInsertItemModel(ctx, code, desc, LanguageList);
					checkGroupValueModel.MMerItemID = insertItemModel.MItemID;
					cmdList.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDItemModel>(ctx, insertItemModel, null, true));
					MerItemList.Add(new GLTreeModel
					{
						id = insertItemModel.MItemID,
						text = insertItemModel.MNumber + ":" + insertItemModel.MDesc
					});
				}
				else if (!gLTreeModel2.MIsActive)
				{
					string text7 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "MerItemDisabledErrorTips", "商品项目{0}已经禁用！");
					string message4 = string.Format(text7, checkGroupValueModel.MMerItemID);
					BizVerificationInfor erroInfo4 = GetErroInfo(message4, 2, initBalance.DataOrigin, initBalance.MRowIndex, "MMerItemID");
					list.Add(erroInfo4);
				}
				else
				{
					checkGroupValueModel.MMerItemID = gLTreeModel2.id;
				}
			}
			if (!string.IsNullOrWhiteSpace(checkGroupValueModel.MExpItemID))
			{
				string expenseName = checkGroupValueModel.MExpItemID.Trim();
				if (ExpenseItemList.Count() == 0)
				{
					List<GLTreeModel> mDataList3 = GLUnility.GetAllExpItemCheckTypeData(ctx).MDataList;
					if (mDataList3 != null)
					{
						ExpenseItemList.AddRange(mDataList3);
					}
				}
				GLTreeModel gLTreeModel3 = (from x in ExpenseItemList
				where x.text.EqualsIgnoreCase(expenseName)
				select x).FirstOrDefault();
				if (gLTreeModel3 == null)
				{
					List<GLTreeModel> childExpenseItemList = new List<GLTreeModel>();
					ExpenseItemList.ForEach(delegate(GLTreeModel x)
					{
						if (x.children != null && x.children.Count() > 0)
						{
							childExpenseItemList.AddRange(x.children);
						}
					});
					gLTreeModel3 = (from x in childExpenseItemList
					where x.text == expenseName
					select x).FirstOrDefault();
				}
				if (gLTreeModel3 == null)
				{
					BDExpenseItemModel insertExpenseIitemModel = new BDExpenseItemBusiness().GetInsertExpenseIitemModel(ctx, expenseName, LanguageList);
					checkGroupValueModel.MExpItemID = insertExpenseIitemModel.MItemID;
					cmdList.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDExpenseItemModel>(ctx, insertExpenseIitemModel, null, true));
					ExpenseItemList.Add(new GLTreeModel
					{
						id = insertExpenseIitemModel.MItemID,
						text = insertExpenseIitemModel.MName
					});
				}
				else if (!gLTreeModel3.MIsActive)
				{
					string text8 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "ExpenseItemDisabledErrorTips", "费用项目{0}已经禁用！");
					string message5 = string.Format(text8, expenseName);
					BizVerificationInfor erroInfo5 = GetErroInfo(message5, 2, initBalance.DataOrigin, initBalance.MRowIndex, "MExpItemID");
					list.Add(erroInfo5);
				}
				else
				{
					checkGroupValueModel.MExpItemID = gLTreeModel3.id;
				}
			}
			if (!string.IsNullOrWhiteSpace(checkGroupValueModel.MPaItemID))
			{
				string paName = checkGroupValueModel.MPaItemID;
				if (PaItemList.Count() == 0)
				{
					List<GLTreeModel> mDataList4 = GLUnility.GetPaItemCheckTypeData(ctx).MDataList;
					if (mDataList4 != null)
					{
						PaItemList.AddRange(mDataList4);
					}
				}
				GLTreeModel gLTreeModel4 = (from x in PaItemList
				where x.text.EqualsIgnoreCase(paName)
				select x).FirstOrDefault();
				if (gLTreeModel4 == null)
				{
					foreach (GLTreeModel paItem in PaItemList)
					{
						List<GLTreeModel> children = paItem.children;
						if (children != null)
						{
							gLTreeModel4 = (from x in children
							where x.text.EqualsIgnoreCase(paName)
							select x).FirstOrDefault();
							if (gLTreeModel4 != null)
							{
								break;
							}
						}
					}
				}
				if (gLTreeModel4 == null)
				{
					string text9 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "PaItemErrorTips", "工资项{0}不存在");
					string message6 = string.Format(text9, checkGroupValueModel.MPaItemID);
					BizVerificationInfor erroInfo6 = GetErroInfo(message6, 2, initBalance.DataOrigin, initBalance.MRowIndex, "MPaItemID");
					list.Add(erroInfo6);
				}
				else if (!gLTreeModel4.MIsActive)
				{
					string text10 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "PAItemDisabledErrorTips", "工资项目{0}已经禁用！");
					string message7 = string.Format(text10, checkGroupValueModel.MPaItemID);
					BizVerificationInfor erroInfo7 = GetErroInfo(message7, 2, initBalance.DataOrigin, initBalance.MRowIndex, "MPaItemID");
					list.Add(erroInfo7);
				}
				else
				{
					checkGroupValueModel.MPaItemID = gLTreeModel4.id;
				}
			}
			for (int i = 0; i < 5; i++)
			{
				string trackEntryName = null;
				switch (i)
				{
				case 0:
					trackEntryName = checkGroupValueModel.MTrackItem1;
					break;
				case 1:
					trackEntryName = checkGroupValueModel.MTrackItem2;
					break;
				case 2:
					trackEntryName = checkGroupValueModel.MTrackItem3;
					break;
				case 3:
					trackEntryName = checkGroupValueModel.MTrackItem4;
					break;
				case 4:
					trackEntryName = checkGroupValueModel.MTrackItem5;
					break;
				}
				if (!string.IsNullOrWhiteSpace(trackEntryName))
				{
					if (TrackList.Count() == 0)
					{
						TrackList = GLUnility.GetAllTrackItemList(ctx, true);
					}
					if (TrackList == null || TrackList.Count() < i + 1)
					{
						string text11 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "ImportInitBalanceCannotAddTrackItem", "导入模板中不能新增跟踪大项");
						BizVerificationInfor erroInfo8 = GetErroInfo(text11, 2, initBalance.DataOrigin, initBalance.MRowIndex, null);
						list.Add(erroInfo8);
					}
					else
					{
						List<GLTreeModel> children2 = TrackList[i].children;
						GLTreeModel gLTreeModel5 = null;
						if (children2 != null)
						{
							gLTreeModel5 = (from x in children2
							where x.text.EqualsIgnoreCase(trackEntryName)
							select x).FirstOrDefault();
						}
						if (gLTreeModel5 == null)
						{
							BDTrackEntryModel insertTrackEntryModel = new BDTrackBusiness().GetInsertTrackEntryModel(ctx, TrackList[i].id, trackEntryName, LanguageList);
							switch (i)
							{
							case 0:
								checkGroupValueModel.MTrackItem1 = insertTrackEntryModel.MItemID;
								break;
							case 1:
								checkGroupValueModel.MTrackItem2 = insertTrackEntryModel.MItemID;
								break;
							case 2:
								checkGroupValueModel.MTrackItem3 = insertTrackEntryModel.MItemID;
								break;
							case 3:
								checkGroupValueModel.MTrackItem4 = insertTrackEntryModel.MItemID;
								break;
							case 4:
								checkGroupValueModel.MTrackItem5 = insertTrackEntryModel.MItemID;
								break;
							}
							cmdList.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDTrackEntryModel>(ctx, insertTrackEntryModel, null, true));
							TrackList[i].children = ((TrackList[i].children == null) ? new List<GLTreeModel>() : TrackList[i].children);
							gLTreeModel5 = new GLTreeModel();
							gLTreeModel5.id = insertTrackEntryModel.MEntryID;
							gLTreeModel5.text = insertTrackEntryModel.MName;
							TrackList[i].children.Add(gLTreeModel5);
						}
						if (!gLTreeModel5.MIsActive)
						{
							string text12 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "TrackItemDisabledErrorTips", "跟踪项{0}已经禁用");
							string message8 = string.Format(text12, trackEntryName);
							BizVerificationInfor erroInfo9 = GetErroInfo(message8, 2, initBalance.DataOrigin, initBalance.MRowIndex, "TrackItem" + (i + 1));
							list.Add(erroInfo9);
						}
						else
						{
							switch (i)
							{
							case 0:
								checkGroupValueModel.MTrackItem1 = gLTreeModel5.id;
								break;
							case 1:
								checkGroupValueModel.MTrackItem2 = gLTreeModel5.id;
								break;
							case 2:
								checkGroupValueModel.MTrackItem3 = gLTreeModel5.id;
								break;
							case 3:
								checkGroupValueModel.MTrackItem4 = gLTreeModel5.id;
								break;
							case 4:
								checkGroupValueModel.MTrackItem5 = gLTreeModel5.id;
								break;
							}
						}
					}
				}
			}
			if (list.Count == 0)
			{
				GLCheckGroupValueModel checkGroupValueModel2 = GLUnility.GetCheckGroupValueModel(ctx, checkGroupValueModel);
				initBalance.MCheckGroupValueID = checkGroupValueModel2.MItemID;
			}
			return list;
		}

		private bool ValidateInitBalanceCheckTypeIsPass(MContext ctx, BDAccountModel account, GLInitBalanceModel initBalance, out List<BizVerificationInfor> checkResult)
		{
			bool result = true;
			checkResult = new List<BizVerificationInfor>();
			if (account.MCheckGroupID == "0" || initBalance.MCheckGroupValueID == "0")
			{
				return result;
			}
			GLCheckGroupModel mCheckGroupModel = account.MCheckGroupModel;
			GLCheckGroupValueModel mCheckGroupValueModel = initBalance.MCheckGroupValueModel;
			if (mCheckGroupModel == null)
			{
				return result;
			}
			List<string> list = new List<string>();
			string arg = account.MNumber + "-" + account.MName;
			if (mCheckGroupModel.MContactID == CheckTypeStatusEnum.Required && string.IsNullOrWhiteSpace(mCheckGroupValueModel.MContactID))
			{
				string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "ContactIsRequireCheckType", "科目{0},联系人是必填核算维度！");
				string message = string.Format(text, arg);
				BizVerificationInfor erroInfo = GetErroInfo(message, 2, initBalance.DataOrigin, initBalance.MRowIndex, "MContactID");
				checkResult.Add(erroInfo);
			}
			else if ((mCheckGroupModel.MContactID == CheckTypeStatusEnum.DisabledOptional || mCheckGroupModel.MContactID == CheckTypeStatusEnum.DisabledRequired) && !string.IsNullOrWhiteSpace(mCheckGroupValueModel.MContactID))
			{
				string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "ContactIsDisableCheckType", "科目{0},联系人核算维度已经禁用，不允许录入余额！");
				string message2 = string.Format(text2, arg);
				BizVerificationInfor erroInfo2 = GetErroInfo(message2, 2, initBalance.DataOrigin, initBalance.MRowIndex, "MContactID");
				checkResult.Add(erroInfo2);
			}
			if (mCheckGroupModel.MEmployeeID == CheckTypeStatusEnum.Required && string.IsNullOrWhiteSpace(mCheckGroupValueModel.MEmployeeID))
			{
				string text3 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "EmployeeIsRequireCheckType", "科目{0},员工是必填核算维度！");
				string message3 = string.Format(text3, arg);
				BizVerificationInfor erroInfo3 = GetErroInfo(message3, 2, initBalance.DataOrigin, initBalance.MRowIndex, "MEmployeeID");
				checkResult.Add(erroInfo3);
			}
			else if ((mCheckGroupModel.MEmployeeID == CheckTypeStatusEnum.DisabledOptional || mCheckGroupModel.MEmployeeID == CheckTypeStatusEnum.DisabledRequired) && !string.IsNullOrWhiteSpace(mCheckGroupValueModel.MEmployeeID))
			{
				string text4 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "EmployeeIsDisableCheckType", "科目{0},员工核算维度已经禁用，不允许录入余额！");
				string message4 = string.Format(text4, arg);
				BizVerificationInfor erroInfo4 = GetErroInfo(message4, 2, initBalance.DataOrigin, initBalance.MRowIndex, "MEmployeeID");
				checkResult.Add(erroInfo4);
			}
			if (mCheckGroupModel.MMerItemID == CheckTypeStatusEnum.Required && string.IsNullOrWhiteSpace(mCheckGroupValueModel.MMerItemID))
			{
				string text5 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "MMerItemIsRequireCheckType", "科目{0},商品项目是必填核算维度！");
				string message5 = string.Format(text5, arg);
				BizVerificationInfor erroInfo5 = GetErroInfo(message5, 2, initBalance.DataOrigin, initBalance.MRowIndex, "MMerItemID");
				checkResult.Add(erroInfo5);
			}
			else if ((mCheckGroupModel.MMerItemID == CheckTypeStatusEnum.DisabledOptional || mCheckGroupModel.MMerItemID == CheckTypeStatusEnum.DisabledRequired) && !string.IsNullOrWhiteSpace(mCheckGroupValueModel.MMerItemID))
			{
				string text6 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "MerItemIsDisableCheckType", "科目{0},商品项目核算维度已经禁用，不允许录入余额！");
				string message6 = string.Format(text6, arg);
				BizVerificationInfor erroInfo6 = GetErroInfo(message6, 2, initBalance.DataOrigin, initBalance.MRowIndex, "MMerItemID");
				checkResult.Add(erroInfo6);
			}
			if (mCheckGroupModel.MExpItemID == CheckTypeStatusEnum.Required && string.IsNullOrWhiteSpace(mCheckGroupValueModel.MExpItemID))
			{
				string text7 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "MExpItemIsRequireCheckType", "科目{0},费用项目是必填核算维度！");
				string message7 = string.Format(text7, arg);
				BizVerificationInfor erroInfo7 = GetErroInfo(message7, 2, initBalance.DataOrigin, initBalance.MRowIndex, "MExpItemID");
				checkResult.Add(erroInfo7);
			}
			else if ((mCheckGroupModel.MExpItemID == CheckTypeStatusEnum.DisabledOptional || mCheckGroupModel.MExpItemID == CheckTypeStatusEnum.DisabledRequired) && !string.IsNullOrWhiteSpace(mCheckGroupValueModel.MExpItemID))
			{
				string text8 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "ExpenseItemIsDisableCheckType", "科目{0},费用项目核算维度已经禁用，不允许录入余额！");
				string message8 = string.Format(text8, arg);
				BizVerificationInfor erroInfo8 = GetErroInfo(message8, 2, initBalance.DataOrigin, initBalance.MRowIndex, "MExpItemID");
				checkResult.Add(erroInfo8);
			}
			if (mCheckGroupModel.MPaItemID == CheckTypeStatusEnum.Required && string.IsNullOrWhiteSpace(mCheckGroupValueModel.MPaItemID))
			{
				string text9 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "PAItemIsRequireCheckType", "科目{0},工资项目是必填核算维度！");
				string message9 = string.Format(text9, arg);
				BizVerificationInfor erroInfo9 = GetErroInfo(message9, 2, initBalance.DataOrigin, initBalance.MRowIndex, "MPaItemID");
				checkResult.Add(erroInfo9);
			}
			else if ((mCheckGroupModel.MPaItemID == CheckTypeStatusEnum.DisabledOptional || mCheckGroupModel.MPaItemID == CheckTypeStatusEnum.DisabledRequired) && !string.IsNullOrWhiteSpace(mCheckGroupValueModel.MPaItemID))
			{
				string text10 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "PaItemIsDisableCheckType", "科目{0},工资项目核算维度已经禁用，不允许录入余额！");
				string message10 = string.Format(text10, arg);
				BizVerificationInfor erroInfo10 = GetErroInfo(message10, 2, initBalance.DataOrigin, initBalance.MRowIndex, "MPaItemID");
				checkResult.Add(erroInfo10);
			}
			if (mCheckGroupModel.MTrackItem1 == CheckTypeStatusEnum.Required && string.IsNullOrWhiteSpace(mCheckGroupValueModel.MTrackItem1))
			{
				if (TrackList.Count() == 0)
				{
					List<GLTreeModel> trackItemList = new GLUtility().GetTrackItemList(ctx, false);
					if (trackItemList != null)
					{
						TrackList.AddRange(trackItemList);
					}
				}
				string text11 = TrackList[0].text;
				string text12 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "TrackItemIsRequireCheckType", "科目{0},{1}是必填核算维度！");
				string message11 = string.Format(text12, arg, text11);
				BizVerificationInfor erroInfo11 = GetErroInfo(message11, 2, initBalance.DataOrigin, initBalance.MRowIndex, "MTrackItem1");
				checkResult.Add(erroInfo11);
			}
			else if ((mCheckGroupModel.MTrackItem1 == CheckTypeStatusEnum.DisabledOptional || mCheckGroupModel.MTrackItem1 == CheckTypeStatusEnum.DisabledRequired) && !string.IsNullOrWhiteSpace(mCheckGroupValueModel.MTrackItem1))
			{
				if (TrackList.Count() == 0)
				{
					List<GLTreeModel> trackItemList2 = new GLUtility().GetTrackItemList(ctx, false);
					if (trackItemList2 != null)
					{
						TrackList.AddRange(trackItemList2);
					}
				}
				string text13 = TrackList[0].text;
				string text14 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "TrackItemIsDisableCheckType", "科目{0},{1}是禁用核算维度,不允许录入余额！");
				string message12 = string.Format(text14, arg, text13);
				BizVerificationInfor erroInfo12 = GetErroInfo(message12, 2, initBalance.DataOrigin, initBalance.MRowIndex, "MTrackItem1");
				checkResult.Add(erroInfo12);
			}
			if (mCheckGroupModel.MTrackItem2 == CheckTypeStatusEnum.Required && string.IsNullOrWhiteSpace(mCheckGroupValueModel.MTrackItem2))
			{
				if (TrackList.Count() == 0)
				{
					List<GLTreeModel> trackItemList3 = new GLUtility().GetTrackItemList(ctx, false);
					if (trackItemList3 != null)
					{
						TrackList.AddRange(trackItemList3);
					}
				}
				string text15 = TrackList[1].text;
				string text16 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "TrackItemIsRequireCheckType", "科目{0},{1}是必填核算维度！");
				string message13 = string.Format(text16, arg, text15);
				BizVerificationInfor erroInfo13 = GetErroInfo(message13, 2, initBalance.DataOrigin, initBalance.MRowIndex, "MTrackItem2");
				checkResult.Add(erroInfo13);
			}
			else if ((mCheckGroupModel.MTrackItem2 == CheckTypeStatusEnum.DisabledOptional || mCheckGroupModel.MTrackItem2 == CheckTypeStatusEnum.DisabledRequired) && !string.IsNullOrWhiteSpace(mCheckGroupValueModel.MTrackItem2))
			{
				if (TrackList.Count() == 0)
				{
					List<GLTreeModel> trackItemList4 = new GLUtility().GetTrackItemList(ctx, false);
					if (trackItemList4 != null)
					{
						TrackList.AddRange(trackItemList4);
					}
				}
				string text17 = TrackList[1].text;
				string text18 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "TrackItemIsDisableCheckType", "科目{0},{1}是禁用核算维度,不允许录入余额！");
				string message14 = string.Format(text18, arg, text17);
				BizVerificationInfor erroInfo14 = GetErroInfo(message14, 2, initBalance.DataOrigin, initBalance.MRowIndex, "MTrackItem2");
				checkResult.Add(erroInfo14);
			}
			if (mCheckGroupModel.MTrackItem3 == CheckTypeStatusEnum.Required && string.IsNullOrWhiteSpace(mCheckGroupValueModel.MTrackItem3))
			{
				if (TrackList.Count() == 0)
				{
					List<GLTreeModel> trackItemList5 = new GLUtility().GetTrackItemList(ctx, false);
					if (trackItemList5 != null)
					{
						TrackList.AddRange(trackItemList5);
					}
				}
				string text19 = TrackList[2].text;
				string text20 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "TrackItemIsRequireCheckType", "科目{0},{1}是必填核算维度！");
				string message15 = string.Format(text20, arg, text19);
				BizVerificationInfor erroInfo15 = GetErroInfo(message15, 2, initBalance.DataOrigin, initBalance.MRowIndex, "MTrackItem3");
				checkResult.Add(erroInfo15);
			}
			else if ((mCheckGroupModel.MTrackItem3 == CheckTypeStatusEnum.DisabledOptional || mCheckGroupModel.MTrackItem3 == CheckTypeStatusEnum.DisabledRequired) && !string.IsNullOrWhiteSpace(mCheckGroupValueModel.MTrackItem3))
			{
				if (TrackList.Count() == 0)
				{
					List<GLTreeModel> trackItemList6 = new GLUtility().GetTrackItemList(ctx, false);
					if (trackItemList6 != null)
					{
						TrackList.AddRange(trackItemList6);
					}
				}
				string text21 = TrackList[2].text;
				string text22 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "TrackItemIsDisableCheckType", "科目{0},{1}是禁用核算维度,不允许录入余额！");
				string message16 = string.Format(text22, arg, text21);
				BizVerificationInfor erroInfo16 = GetErroInfo(message16, 2, initBalance.DataOrigin, initBalance.MRowIndex, "MTrackItem3");
				checkResult.Add(erroInfo16);
			}
			if (mCheckGroupModel.MTrackItem4 == CheckTypeStatusEnum.Required && string.IsNullOrWhiteSpace(mCheckGroupValueModel.MTrackItem4))
			{
				if (TrackList.Count() == 0)
				{
					List<GLTreeModel> trackItemList7 = new GLUtility().GetTrackItemList(ctx, false);
					if (trackItemList7 != null)
					{
						TrackList.AddRange(trackItemList7);
					}
				}
				string text23 = TrackList[3].text;
				string text24 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "TrackItemIsRequireCheckType", "科目{0},{1}是必填核算维度！");
				string message17 = string.Format(text24, arg, text23);
				BizVerificationInfor erroInfo17 = GetErroInfo(message17, 2, initBalance.DataOrigin, initBalance.MRowIndex, "MTrackItem4");
				checkResult.Add(erroInfo17);
			}
			else if ((mCheckGroupModel.MTrackItem4 == CheckTypeStatusEnum.DisabledOptional || mCheckGroupModel.MTrackItem4 == CheckTypeStatusEnum.DisabledRequired) && !string.IsNullOrWhiteSpace(mCheckGroupValueModel.MTrackItem4))
			{
				if (TrackList.Count() == 0)
				{
					List<GLTreeModel> trackItemList8 = new GLUtility().GetTrackItemList(ctx, false);
					if (trackItemList8 != null)
					{
						TrackList.AddRange(trackItemList8);
					}
				}
				string text25 = TrackList[3].text;
				string text26 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "TrackItemIsDisableCheckType", "科目{0},{1}是禁用核算维度,不允许录入余额！");
				string message18 = string.Format(text26, arg, text25);
				BizVerificationInfor erroInfo18 = GetErroInfo(message18, 2, initBalance.DataOrigin, initBalance.MRowIndex, "MTrackItem4");
				checkResult.Add(erroInfo18);
			}
			if (mCheckGroupModel.MTrackItem5 == CheckTypeStatusEnum.Required && string.IsNullOrWhiteSpace(mCheckGroupValueModel.MTrackItem5))
			{
				if (TrackList.Count() == 0)
				{
					List<GLTreeModel> trackItemList9 = new GLUtility().GetTrackItemList(ctx, false);
					if (trackItemList9 != null)
					{
						TrackList.AddRange(trackItemList9);
					}
				}
				string text27 = TrackList[4].text;
				string text28 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "TrackItemIsRequireCheckType", "科目{0},{1}是必填核算维度！");
				string message19 = string.Format(text28, arg, text27);
				BizVerificationInfor erroInfo19 = GetErroInfo(message19, 2, initBalance.DataOrigin, initBalance.MRowIndex, "MTrackItem5");
				checkResult.Add(erroInfo19);
			}
			else if ((mCheckGroupModel.MTrackItem5 == CheckTypeStatusEnum.DisabledOptional || mCheckGroupModel.MTrackItem5 == CheckTypeStatusEnum.DisabledRequired) && !string.IsNullOrWhiteSpace(mCheckGroupValueModel.MTrackItem5))
			{
				if (TrackList.Count() == 0)
				{
					List<GLTreeModel> trackItemList10 = new GLUtility().GetTrackItemList(ctx, false);
					if (trackItemList10 != null)
					{
						TrackList.AddRange(trackItemList10);
					}
				}
				string text29 = TrackList[4].text;
				string text30 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "TrackItemIsDisableCheckType", "科目{0},{1}是禁用核算维度,不允许录入余额！");
				string message20 = string.Format(text30, arg, text29);
				BizVerificationInfor erroInfo20 = GetErroInfo(message20, 2, initBalance.DataOrigin, initBalance.MRowIndex, "MTrackItem5");
				checkResult.Add(erroInfo20);
			}
			if (account.MCode.IndexOf("1221") == 0 || account.MCode.IndexOf("2241") == 0)
			{
				if (!string.IsNullOrWhiteSpace(mCheckGroupValueModel.MContactID) && !string.IsNullOrWhiteSpace(mCheckGroupValueModel.MEmployeeID))
				{
					string text31 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "OtherReceivableSelectCheckTypeError1", "科目{0},不能同时选择联系人和员工！");
					string message21 = string.Format(text31, arg);
					BizVerificationInfor erroInfo21 = GetErroInfo(message21, 2, initBalance.DataOrigin, initBalance.MRowIndex, null);
					checkResult.Add(erroInfo21);
				}
				if (!string.IsNullOrWhiteSpace(mCheckGroupValueModel.MMerItemID) && !string.IsNullOrWhiteSpace(mCheckGroupValueModel.MExpItemID))
				{
					string text32 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "OtherReceivableSelectCheckTypeError2", "科目{0},不能同时选择商品项目和费用项目！");
					string message22 = string.Format(text32, arg);
					BizVerificationInfor erroInfo22 = GetErroInfo(message22, 2, initBalance.DataOrigin, initBalance.MRowIndex, null);
					checkResult.Add(erroInfo22);
				}
				if (!string.IsNullOrWhiteSpace(mCheckGroupValueModel.MContactID) && !string.IsNullOrWhiteSpace(mCheckGroupValueModel.MExpItemID))
				{
					string text33 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "OtherReceivableSelectCheckTypeError3", "科目{0},不能同时选择联系人和费用项目！");
					string message23 = string.Format(text33, arg);
					BizVerificationInfor erroInfo23 = GetErroInfo(message23, 2, initBalance.DataOrigin, initBalance.MRowIndex, null);
					checkResult.Add(erroInfo23);
				}
				if (!string.IsNullOrWhiteSpace(mCheckGroupValueModel.MEmployeeID) && !string.IsNullOrWhiteSpace(mCheckGroupValueModel.MMerItemID))
				{
					string text34 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "OtherReceivableSelectCheckTypeError4", "科目{0},不能同时选择员工和商品项目！");
					string message24 = string.Format(text34, arg);
					BizVerificationInfor erroInfo24 = GetErroInfo(message24, 2, initBalance.DataOrigin, initBalance.MRowIndex, null);
					checkResult.Add(erroInfo24);
				}
			}
			return checkResult.Count <= 0 && true;
		}

		private bool SetContactType(BDAccountModel accountInInitBalance, BDContactsInfoModel contact, ref string field)
		{
			bool result = false;
			string mCode = accountInInitBalance.MCode;
			if (mCode == null)
			{
				return result;
			}
			bool flag = BDAccountHelper.IsCurrentAccount(mCode);
			if (mCode.IndexOf("1122") == 0 || mCode.IndexOf("2203") == 0)
			{
				result = !contact.MIsCustomer;
				contact.MIsCustomer = true;
				field = "MIsCustomer";
			}
			else if (mCode.IndexOf("2202") == 0 || mCode.IndexOf("1123") == 0)
			{
				result = !contact.MIsSupplier;
				contact.MIsSupplier = true;
				field = "MIsSupplier";
			}
			else
			{
				result = !contact.MIsOther;
				contact.MIsOther = true;
				field = "MIsOther";
			}
			return result;
		}

		private List<BizVerificationInfor> VaildataImportRecordCurrency(MContext ctx, GLInitBalanceModel initBalance, List<BDBankAccountEditModel> bankAccountList)
		{
			List<BizVerificationInfor> list = new List<BizVerificationInfor>();
			if (string.IsNullOrWhiteSpace(initBalance.MCurrencyID) && initBalance.MAccountModel != null && initBalance.MAccountModel.MCreateInitBill)
			{
				return list;
			}
			string field = "MCurrencyID";
			REGCurrencyRepository rEGCurrencyRepository = new REGCurrencyRepository();
			if (CurrencyList.Count() == 0)
			{
				List<REGCurrencyViewModel> currencyViewList = rEGCurrencyRepository.GetCurrencyViewList(ctx, null, true, null);
				if (currencyViewList != null)
				{
					CurrencyList.AddRange(currencyViewList);
				}
			}
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "ImportInitBalanceRecordNotLegal", "导入的期初余额数据不合法:");
			if (string.IsNullOrWhiteSpace(initBalance.MCurrencyID))
			{
				string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "ImportInitBalanceError3", "期初余额记录没有填写币别,记录详情：{0}");
				string message = string.Format(text2, initBalance.MAccountModel.MNumber + "-?-" + initBalance.MInitBalanceFor + "-" + initBalance.MInitBalance);
				BizVerificationInfor erroInfo = GetErroInfo(message, 2, initBalance.DataOrigin, initBalance.MRowIndex, field);
				list.Add(erroInfo);
				return list;
			}
			string[] array = initBalance.MCurrencyID.Trim().Split(' ');
			if (array.Length == 0)
			{
				string text3 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "ImportInitBalanceError4", "无法识别币别,记录详情：{0}");
				string message2 = string.Format(text3, initBalance.MAccountModel.MNumber + "-?-" + initBalance.MInitBalanceFor + "-" + initBalance.MInitBalance);
				BizVerificationInfor erroInfo2 = GetErroInfo(message2, 2, initBalance.DataOrigin, initBalance.MRowIndex, field);
				list.Add(erroInfo2);
				return list;
			}
			string currencyInInitBalance = array[0].Trim();
			REGCurrencyViewModel rEGCurrencyViewModel = (from x in CurrencyList
			where x.MCurrencyID == currencyInInitBalance || x.MName == currencyInInitBalance
			select x).FirstOrDefault();
			if (rEGCurrencyViewModel == null)
			{
				string text4 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "ImportInitBalanceError5", "币别不存在,记录详情：{0}");
				string message3 = string.Format(text4, initBalance.MAccountModel.MNumber + "-" + initBalance.MCurrencyID + "-" + initBalance.MInitBalanceFor + "-" + initBalance.MInitBalance);
				BizVerificationInfor erroInfo3 = GetErroInfo(message3, 2, initBalance.DataOrigin, initBalance.MRowIndex, field);
				list.Add(erroInfo3);
				return list;
			}
			initBalance.MCurrencyID = rEGCurrencyViewModel.MCurrencyID;
			BDAccountModel account = initBalance.MAccountModel;
			string arg = account.MNumber + "-" + account.MName;
			if (!account.MIsCheckForCurrency && initBalance.MCurrencyID != ctx.MBasCurrencyID)
			{
				string text5 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "AccountNotAllowInputForeginCurrency", "科目{0}是非外币核算，不允许录入外币期初余额!");
				string message4 = string.Format(text5, arg);
				BizVerificationInfor erroInfo4 = GetErroInfo(message4, 2, initBalance.DataOrigin, initBalance.MRowIndex, field);
				list.Add(erroInfo4);
				return list;
			}
			BDBankAccountEditModel bDBankAccountEditModel = (from x in bankAccountList
			where x.MItemID == account.MItemID
			select x).FirstOrDefault();
			if (bDBankAccountEditModel != null)
			{
				string mCyID = bDBankAccountEditModel.MCyID;
				if (initBalance.MCurrencyID != mCyID)
				{
					string text6 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "BankAccountNotAllowInputForeginCurrency", "科目{0}是库存现金或者银行科目，不允许录入非{1}金额!");
					string message5 = string.Format(text6, arg, mCyID);
					BizVerificationInfor erroInfo5 = GetErroInfo(message5, 2, initBalance.DataOrigin, initBalance.MRowIndex, field);
					list.Add(erroInfo5);
					return list;
				}
			}
			if (initBalance.MCurrencyID == ctx.MBasCurrencyID)
			{
				if (initBalance.MInitBalance != decimal.Zero && initBalance.MInitBalanceFor != decimal.Zero && initBalance.MInitBalanceFor != initBalance.MInitBalance)
				{
					string text7 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "ImportInitBalanceError7", "科目:{0},本位币和原币期初金额不一致");
					string message6 = string.Format(text7, arg);
					BizVerificationInfor erroInfo6 = GetErroInfo(message6, 2, initBalance.DataOrigin, initBalance.MRowIndex, "");
					list.Add(erroInfo6);
				}
				else if (initBalance.MInitBalanceFor != initBalance.MInitBalance && (initBalance.MInitBalance == decimal.Zero || initBalance.MInitBalanceFor == decimal.Zero))
				{
					decimal obj = (initBalance.MInitBalance == decimal.Zero) ? initBalance.MInitBalanceFor : initBalance.MInitBalance;
					decimal num3 = initBalance.MInitBalanceFor = (initBalance.MInitBalance = obj);
				}
				if (initBalance.MYtdCredit != decimal.Zero && initBalance.MYtdCreditFor != decimal.Zero && initBalance.MYtdCredit != initBalance.MYtdCreditFor)
				{
					string text8 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "ImportInitBalanceError8", "科目:{0},本位币和原币本年累计贷方金额不一致");
					string message7 = string.Format(text8, arg);
					BizVerificationInfor erroInfo7 = GetErroInfo(message7, 2, initBalance.DataOrigin, initBalance.MRowIndex, "");
					list.Add(erroInfo7);
				}
				else if (initBalance.MYtdCredit != initBalance.MYtdCreditFor && (initBalance.MYtdCredit == decimal.Zero || initBalance.MYtdCreditFor == decimal.Zero))
				{
					decimal obj2 = (initBalance.MYtdCredit == decimal.Zero) ? initBalance.MYtdCreditFor : initBalance.MYtdCredit;
					decimal num6 = initBalance.MYtdCreditFor = (initBalance.MYtdCredit = obj2);
				}
				if (initBalance.MYtdDebit != decimal.Zero && initBalance.MYtdDebitFor != decimal.Zero && initBalance.MYtdDebit != initBalance.MYtdDebitFor)
				{
					string text9 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "ImportInitBalanceError9", "科目:{0},本位币和原币本年累计借方金额不一致");
					string message8 = string.Format(text9, arg);
					BizVerificationInfor erroInfo8 = GetErroInfo(message8, 2, initBalance.DataOrigin, initBalance.MRowIndex, "");
					list.Add(erroInfo8);
				}
				else if (initBalance.MYtdDebit != initBalance.MYtdDebitFor && (initBalance.MYtdDebit == decimal.Zero || initBalance.MYtdDebitFor == decimal.Zero))
				{
					decimal obj3 = (initBalance.MYtdDebit == decimal.Zero) ? initBalance.MYtdDebitFor : initBalance.MYtdDebit;
					decimal num9 = initBalance.MYtdDebitFor = (initBalance.MYtdDebit = obj3);
				}
			}
			return list;
		}

		public OperationResult CheckAutoCreateBillHadVerifiyRecord(MContext ctx)
		{
			OperationResult operationResult = new OperationResult();
			List<GLInitBalanceModel> initBalanceModels = dal.GetInitBalanceModels(ctx, null);
			if (initBalanceModels == null || initBalanceModels.Count() == 0)
			{
				operationResult.Success = false;
				return operationResult;
			}
			List<string> list = (from x in initBalanceModels
			where !string.IsNullOrWhiteSpace(x.MBillID)
			select x.MBillID).ToList();
			if (list == null || list.Count == 0)
			{
				operationResult.Success = false;
				return operationResult;
			}
			List<IVVerificationListModel> verificationList = IVVerificationRepository.GetVerificationList(ctx, list);
			if (verificationList == null || verificationList.Count == 0)
			{
				operationResult.Success = false;
				return operationResult;
			}
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "AutoCreateBillExistVerificationRecord", "科目{0}期初余额自动推送的单据存在核销记录");
			foreach (string item2 in list)
			{
				if (verificationList.Any((IVVerificationListModel x) => x.MSourceBillID == item2 || x.MTargetBillID == item2))
				{
					GLInitBalanceModel gLInitBalanceModel = initBalanceModels.First((GLInitBalanceModel x) => x.MBillID == item2);
					string arg = (gLInitBalanceModel.MAccountModel != null) ? gLInitBalanceModel.MAccountModel.MNumber : "";
					string item = string.Format(text, arg);
					operationResult.Success = true;
					operationResult.MessageList.Add(item);
				}
			}
			return operationResult;
		}

		private BizVerificationInfor GetErroInfo(string message, int displayType, string dataOrigin, int rowIndex, string field)
		{
			BizVerificationInfor bizVerificationInfor = new BizVerificationInfor();
			bizVerificationInfor.DisplayType = displayType;
			bizVerificationInfor.Message = message;
			bizVerificationInfor.ExtendField = dataOrigin;
			bizVerificationInfor.RowIndex = rowIndex;
			bizVerificationInfor.CheckItem = field;
			return bizVerificationInfor;
		}

		public bool Exists(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.Exists(ctx, pkID, includeDelete);
		}

		public bool ExistsByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.ExistsByFilter(ctx, filter);
		}

		public OperationResult InsertOrUpdate(MContext ctx, GLInitBalanceModel modelData, string fields = null)
		{
			return dal.InsertOrUpdate(ctx, modelData, fields);
		}

		public OperationResult InsertOrUpdateModels(MContext ctx, List<GLInitBalanceModel> modelData, string fields = null)
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

		public GLInitBalanceModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.GetDataModel(ctx, pkID, includeDelete);
		}

		public GLInitBalanceModel GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.GetDataModelByFilter(ctx, filter);
		}

		public List<GLInitBalanceModel> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelList(ctx, filter, includeDelete);
		}

		public DataGridJson<GLInitBalanceModel> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelPageList(ctx, filter, includeDelete);
		}
	}
}
