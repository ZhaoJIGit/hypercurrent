using JieNor.Megi.BusinessContract.BAS;
using JieNor.Megi.BusinessContract.BD;
using JieNor.Megi.BusinessService.BAS;
using JieNor.Megi.BusinessService.GL;
using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.Logger;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Helper;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.API;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.COM;
using JieNor.Megi.DataModel.Enum;
using JieNor.Megi.DataModel.FA;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.IO;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.DataModel.IO.Import.Account;
using JieNor.Megi.DataModel.MI;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.DataRepository.API;
using JieNor.Megi.DataRepository.BAS;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.FA;
using JieNor.Megi.DataRepository.FC;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.DataRepository.IV;
using JieNor.Megi.DataRepository.MI;
using JieNor.Megi.DataRepository.PA;
using JieNor.Megi.DataRepository.REG;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.BD.AccountItem;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace JieNor.Megi.BusinessService.BD
{
	public class BDAccountBusiness : APIBusinessBase<BDAccountEditModel>, IBDAccountBusiness, IDataContract<BDAccountModel>, IBasicBusiness<BDAccountEditModel>
	{
		private bool isFromApi;

		private List<BDAccountEditModel> _accountDataPool;

		public BDAccountRepository newDal = new BDAccountRepository();

		private readonly GLUtility utility = new GLUtility();

		private List<NameValueModel> TrackModelList = null;

		private readonly BDAccountRepository dal = new BDAccountRepository();

		private IBASLangBusiness _lang = new BASLangBusiness();

		private REGCurrencyRepository _bdCurrency = new REGCurrencyRepository();

		private BDTrackRepository _track = new BDTrackRepository();

		private GLCheckTypeBusiness _checkType = new GLCheckTypeBusiness();

		private GLCheckGroupBusiness _checkGroup = new GLCheckGroupBusiness();

		private BDAccountMatchLogBusiness bizAcctMatchLog = new BDAccountMatchLogBusiness();

		private GLBalanceRepository balanceDal = new GLBalanceRepository();

		private List<BASLangModel> SysLangList = null;

		private string[] bankNumberPrefix = new string[2]
		{
			"1001",
			"1002"
		};

		private string[] requiredCurrentAcctList = new string[4]
		{
			"1122",
			"2203",
			"2202",
			"1123"
		};

		private string[] otherCurrentAcctList = new string[2]
		{
			"1221",
			"2241"
		};

		private string[] allCurrentAcctList = new string[6]
		{
			"1122",
			"2203",
			"2202",
			"1123",
			"1221",
			"2241"
		};

		private List<BDTrackModel> existTrackList = null;

		private List<BDAccountEditModel> allAutoUpdateAcctList = new List<BDAccountEditModel>();

		private List<CommandInfo> updateAcctFullNameCmdList = new List<CommandInfo>();

		private Dictionary<string, Dictionary<string, string>> updateAcctFullNameList = new Dictionary<string, Dictionary<string, string>>();

		protected override DataGridJson<BDAccountEditModel> OnGet(MContext ctx, GetParam param)
		{
			APIDataPool instance = APIDataPool.GetInstance(ctx);
			_accountDataPool = instance.Accounts;
			DataGridJson<BDAccountEditModel> dataGridJson = new DataGridJson<BDAccountEditModel>();
			string where = param.Where;
			List<BDAccountEditModel> list;
			if (!(where == "MI.Log"))
			{
				if (where == "MI.PresetMatch")
				{
					List<BDAccountEditModel> acctList = JsonConvert.DeserializeObject<List<BDAccountEditModel>>(param.PostData);
					list = (dataGridJson.rows = GetMigrateAccountPresetMatch(ctx, acctList));
					dataGridJson.total = ((list != null) ? list.Count : 0);
					return dataGridJson;
				}
				isFromApi = true;
				return dal.Get(ctx, param);
			}
			list = (dataGridJson.rows = MigrateLogRepository.GetMigrateLogList<BDAccountEditModel>(ctx, MigrateTypeEnum.Account));
			dataGridJson.total = ((list != null) ? list.Count : 0);
			return dataGridJson;
		}

		protected override void OnGetAfter(MContext ctx, GetParam param, APIDataPool dataPool, BDAccountEditModel model)
		{
			if (isFromApi && model != null)
			{
				if (model.MAccountDimensions != null && model.MAccountDimensions.Any())
				{
					List<BDAccountDimensionModel> list = new List<BDAccountDimensionModel>();
					for (int i = 0; i < model.MAccountDimensions.Count; i++)
					{
						if (!string.IsNullOrWhiteSpace(model.MAccountDimensions[i].MType))
						{
							list.Add(model.MAccountDimensions[i]);
						}
					}
					model.MAccountDimensions = (from a in list
					orderby a.MSeq
					select a).ToList();
				}
				if (ctx.MultiLang)
				{
					model.MName = _accountDataPool.FirstOrDefault((BDAccountEditModel x) => x.MItemID == model.MItemID)?.MName;
					model.MFullName = _accountDataPool.FirstOrDefault((BDAccountEditModel x) => x.MItemID == model.MItemID)?.MFullName;
					if (!string.IsNullOrWhiteSpace(model.MFullName))
					{
						List<APILangModel> list2 = JsonConvert.DeserializeObject<List<APILangModel>>(model.MFullName);
						foreach (APILangModel item in list2)
						{
							item.Value = item.Value.Replace(model.MNumber + " ", "");
						}
						model.MFullName = JsonConvert.SerializeObject(list2);
					}
				}
				model.MCheckGroupID = null;
			}
		}

		private AccountItemTreeModel GetAccountTreeModel(MContext ctx, BDAccountListModel parentAccount, List<BDAccountListModel> accountList, bool includeNumber = false)
		{
			AccountItemTreeModel accountItemTreeModel = CovertToTreeModel(ctx, parentAccount, includeNumber, null);
			List<BDAccountListModel> list = (from x in accountList
			where x.MParentID == parentAccount.MItemID
			select x).ToList();
			if (list == null || list.Count() == 0)
			{
				return accountItemTreeModel;
			}
			list = (from f in list
			orderby Convert.ToInt32(f.MNumber.Replace(parentAccount.MNumber + ".", ""))
			select f).ToList();
			foreach (BDAccountListModel item in list)
			{
				AccountItemTreeModel accountTreeModel = GetAccountTreeModel(ctx, item, accountList, includeNumber);
				accountTreeModel.AccountLevel = accountItemTreeModel.AccountLevel + 1;
				accountItemTreeModel.children.Add(accountTreeModel);
			}
			return accountItemTreeModel;
		}

		private AccountItemTreeModel GetAccountTreeModel(MContext ctx, BDAccountListModel parentAccount, List<BDAccountListModel> accountList, ref int treeIndex, bool includeNumber = false, AccountItemTreeModel parentTree = null)
		{
			AccountItemTreeModel accountItemTreeModel = CovertToTreeModel(ctx, parentAccount, includeNumber, parentTree);
			accountItemTreeModel.Index = ++treeIndex;
			List<BDAccountListModel> list = (from x in accountList
			where x.MParentID == parentAccount.MItemID
			select x).ToList();
			if (list == null || list.Count() == 0)
			{
				return accountItemTreeModel;
			}
			list = (from f in list
			orderby Convert.ToInt32(f.MNumber.Replace(parentAccount.MNumber + ".", ""))
			select f).ToList();
			foreach (BDAccountListModel item in list)
			{
				AccountItemTreeModel accountTreeModel = GetAccountTreeModel(ctx, item, accountList, ref treeIndex, includeNumber, accountItemTreeModel);
				accountTreeModel.AccountLevel = accountItemTreeModel.AccountLevel + 1;
				accountItemTreeModel.children.Add(accountTreeModel);
			}
			return accountItemTreeModel;
		}

		public List<CommandInfo> GetCheckGroupInsertOrUpdateCmds(MContext ctx, GLCheckGroupModel checkGroupModel, out string checkGroupItemId)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			checkGroupItemId = string.Empty;
			if (checkGroupModel == null)
			{
				return list;
			}
			GLCheckGroupBusiness gLCheckGroupBusiness = new GLCheckGroupBusiness();
			GLCheckGroupModel modelByFilter = gLCheckGroupBusiness.GetModelByFilter(ctx, checkGroupModel);
			if (modelByFilter == null)
			{
				checkGroupItemId = UUIDHelper.GetGuid();
				checkGroupModel.MItemID = checkGroupItemId;
				checkGroupModel.IsNew = true;
				List<CommandInfo> insertOrUpdateCmd = gLCheckGroupBusiness.GetInsertOrUpdateCmd(ctx, checkGroupModel, null);
				list.AddRange(insertOrUpdateCmd);
			}
			else
			{
				checkGroupItemId = modelByFilter.MItemID;
			}
			return list;
		}

		public BDIsCanDeleteModel IsCanDeleteOrInactive(MContext ctx, ParamBase param)
		{
			return BDRepository.IsCanDeleteOrInactive(ctx, "Account", param.KeyIDs.Split(',').ToList(), param.IsDelete);
		}

		public List<AccountItemTreeModel> GetAccountItemTreeList(MContext ctx, BDAccountListFilterModel filter = null)
		{
			BDTrackRepository bDTrackRepository = new BDTrackRepository();
			TrackModelList = bDTrackRepository.GetTrackBasicInfo(ctx, ctx.MOrgID, false, true);
			List<AccountItemTreeModel> list = new List<AccountItemTreeModel>();
			List<BDAccountListModel> accountListIncludeCheckType = dal.GetAccountListIncludeCheckType(ctx, filter);
			if (accountListIncludeCheckType == null || accountListIncludeCheckType.Count() <= 0)
			{
				return list;
			}
			List<BDAccountListModel> parentAccountList = GetParentAccountList(accountListIncludeCheckType);
			if (parentAccountList == null || parentAccountList.Count() == 0)
			{
				return list;
			}
			bool includeNumber = filter?.ShowNumber ?? false;
			List<string> list2 = (filter != null && !string.IsNullOrWhiteSpace(filter.NotParentCodes)) ? filter.NotParentCodes.Split(',').ToList() : null;
			int num = 0;
			foreach (BDAccountListModel item in parentAccountList)
			{
				if (!(list2?.Contains(item.MCode) ?? false))
				{
					AccountItemTreeModel accountTreeModel = GetAccountTreeModel(ctx, item, accountListIncludeCheckType, ref num, includeNumber, null);
					list.Add(accountTreeModel);
				}
			}
			if (filter != null && (!string.IsNullOrWhiteSpace(filter.MStartAccountID) || !string.IsNullOrWhiteSpace(filter.MEndAccountID) || filter.IsLeafAccount || filter.AccountLevel > 0))
			{
				return BDAccountHelper.FilterAccountTreeList(ctx, filter, true, list);
			}
			return list;
		}

		private List<BDAccountListModel> GetParentAccountList(List<BDAccountListModel> accountList)
		{
			List<BDAccountListModel> list = new List<BDAccountListModel>();
			foreach (BDAccountListModel account in accountList)
			{
				if (!accountList.Exists((BDAccountListModel x) => x.MItemID == account.MParentID))
				{
					list.Add(account);
				}
			}
			return list;
		}

		private List<BDAccountListModel> GetChildrenAccountList(List<BDAccountListModel> accountList)
		{
			List<BDAccountListModel> list = new List<BDAccountListModel>();
			foreach (BDAccountListModel account in accountList)
			{
				if (!accountList.Exists((BDAccountListModel x) => x.MParentID == account.MItemID))
				{
					list.Add(account);
				}
			}
			return list;
		}

		public AccountItemTreeModel CovertToTreeModel(MContext ctx, BDAccountListModel model, bool includeNumber = false, AccountItemTreeModel parentTree = null)
		{
			AccountItemTreeModel accountItemTreeModel = new AccountItemTreeModel();
			accountItemTreeModel.id = model.MItemID;
			accountItemTreeModel.text = (includeNumber ? (model.MNumber + " " + model.MName) : model.MName);
			accountItemTreeModel.MIsSys = model.MIsSys;
			accountItemTreeModel.MNumber = model.MNumber;
			accountItemTreeModel.MAccountGroupName = model.MAcctGroupName;
			accountItemTreeModel.MAccountGroupID = model.MAccountGroupID;
			accountItemTreeModel.MAccountTypeID = model.MAccountTypeID;
			accountItemTreeModel.MAcctTypeName = model.MAcctTypeName;
			accountItemTreeModel.MDC = model.MDC;
			accountItemTreeModel.MIsCheckForCurrency = model.MIsCheckForCurrency;
			accountItemTreeModel.MCode = model.MCode;
			accountItemTreeModel.MFullName = model.MFullName;
			accountItemTreeModel.MCreateInitBill = model.MCreateInitBill;
			accountItemTreeModel.CheckGroupNameList = GetCheckTypeList(ctx, model.MCheckGroupModel);
			accountItemTreeModel.children = new List<AccountItemTreeModel>();
			if (model.MParentID == "0")
			{
				accountItemTreeModel.AccountLevel = 1;
			}
			else if (parentTree != null)
			{
				accountItemTreeModel.AccountLevel = parentTree.AccountLevel + 1;
			}
			return accountItemTreeModel;
		}

		public string GetAccountCheckTypeName(MContext ctx, GLCheckGroupModel checkGroupModel, List<string> checkGroupNameList)
		{
			List<NameValueModel> accountCheckTypeListDic = GetAccountCheckTypeListDic(ctx, checkGroupModel);
			string text = string.Empty;
			if (accountCheckTypeListDic != null && accountCheckTypeListDic.Count() > 0)
			{
				foreach (NameValueModel item in accountCheckTypeListDic)
				{
					text = text + GetCheckTypeStatusString(ctx, item.MValue2) + " " + item.MName + ",";
					checkGroupNameList?.Add(item.MName);
				}
			}
			if (!string.IsNullOrWhiteSpace(text))
			{
				text = text.Substring(0, text.Length - 1);
			}
			return text;
		}

		public List<NameValueModel> GetCheckTypeList(MContext ctx, GLCheckGroupModel checkGroupModel)
		{
			return GetAccountCheckTypeListDic(ctx, checkGroupModel);
		}

		private string GetCheckTypeStatusString(MContext ctx, string status)
		{
			string result = string.Empty;
			int num = -1;
			if (!int.TryParse(status, out num))
			{
				return result;
			}
			if (num == CheckTypeStatusEnum.Required)
			{
				result = "*";
			}
			else if (num != CheckTypeStatusEnum.Optional && (num == CheckTypeStatusEnum.DisabledRequired || num == CheckTypeStatusEnum.DisabledOptional))
			{
				result = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "DisabledRequired", "禁用");
			}
			return result;
		}

		public List<NameValueModel> GetAccountCheckTypeDic(MContext ctx, GLCheckGroupModel checkGroupModel)
		{
			return GetAccountCheckTypeListDic(ctx, checkGroupModel);
		}

		public List<NameValueModel> GetAccountCheckTypeListDic(MContext ctx, GLCheckGroupModel checkGroupModel)
		{
			List<NameValueModel> list = new List<NameValueModel>();
			if (checkGroupModel == null)
			{
				return list;
			}
			PropertyInfo[] properties = checkGroupModel.GetType().GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				string name = propertyInfo.Name;
				int num;
				string mName;
				if (!(name == "PKFieldValue") && !(name == "MItemID"))
				{
					num = 0;
					string s = Convert.ToString(propertyInfo.GetValue(checkGroupModel));
					if (int.TryParse(s, out num) && (num == CheckTypeStatusEnum.Optional || num == CheckTypeStatusEnum.Required))
					{
						mName = string.Empty;
						if (name.Contains("TrackItem"))
						{
							if (TrackModelList != null && TrackModelList.Count() > 0)
							{
								int num2 = -1;
								string s2 = name.Replace("MTrackItem", "");
								if (int.TryParse(s2, out num2) && num2 <= TrackModelList.Count())
								{
									mName = TrackModelList[num2 - 1].MName;
									goto IL_017e;
								}
								continue;
							}
						}
						else
						{
							string text = "CheckTypeName" + name.Replace("ID", "");
							mName = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, text, text);
						}
						goto IL_017e;
					}
				}
				continue;
				IL_017e:
				int checkTypeEnum = new GLCheckTypeBusiness().GetCheckTypeEnumByName(name);
				if (!list.Exists((NameValueModel x) => x.MValue == checkTypeEnum.ToString()))
				{
					NameValueModel item = new NameValueModel
					{
						MValue = checkTypeEnum.ToString(),
						MName = mName,
						MValue1 = name,
						MValue2 = num.ToString()
					};
					list.Add(item);
				}
			}
			return list;
		}

		public OperationResult CheckCheckTypeIsUsed(MContext ctx, string accountId, int checkTypeEunm)
		{
			OperationResult operationResult = new OperationResult();
			DataSet dataSet = dal.CheckTypeIsUsed(ctx, accountId, checkTypeEunm);
			if (dataSet == null || dataSet.Tables == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				operationResult.Success = true;
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (DataRow row in dataSet.Tables[0].Rows)
				{
					int num = 0;
					if (int.TryParse(row[0].ToString(), out num))
					{
						switch (num)
						{
						case 1:
						{
							string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "ChecktypeUsedInInitbalance", "核算维度在期初余额中有使用");
							stringBuilder.AppendLine(text);
							break;
						}
						case 2:
						{
							string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "ChecktypeUsedInbalance", "核算维度在科目余额中有使用");
							stringBuilder.AppendLine(text);
							break;
						}
						case 3:
						{
							string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "ChecktypeUsedInVoucher", "核算维度在凭证中有使用");
							stringBuilder.AppendLine(text);
							break;
						}
						case 4:
						{
							string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "ChecktypeUsedInVoucherTemplate", "核算维度在凭证模板中有使用");
							stringBuilder.AppendLine(text);
							break;
						}
						}
					}
				}
				operationResult.Success = false;
				operationResult.Message = stringBuilder.ToString();
			}
			return operationResult;
		}

		public OperationResult DeleteCheckType(MContext ctx, string accountId, int checkTypeEnum)
		{
			OperationResult operationResult = new OperationResult();
			operationResult = CheckCheckTypeIsUsed(ctx, accountId, checkTypeEnum);
			if (!operationResult.Success)
			{
				return operationResult;
			}
			BDAccountModel bDAccountModel = GetAccountListWithCheckType(ctx, accountId, false).FirstOrDefault();
			if (bDAccountModel == null)
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "AccountNotExist", "科目不存在");
				return operationResult;
			}
			GLCheckGroupModel mCheckGroupModel = bDAccountModel.MCheckGroupModel;
			if (mCheckGroupModel == null)
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "AccountNotExistCheckType", "科目不存在核算维度");
				return operationResult;
			}
			mCheckGroupModel = GetDeleteCheckGroupModel(mCheckGroupModel, checkTypeEnum);
			List<CommandInfo> list = new List<CommandInfo>();
			GLCheckGroupBusiness gLCheckGroupBusiness = new GLCheckGroupBusiness();
			GLCheckGroupModel modelByFilter = gLCheckGroupBusiness.GetModelByFilter(ctx, mCheckGroupModel);
			string empty = string.Empty;
			if (modelByFilter == null)
			{
				empty = (mCheckGroupModel.MItemID = UUIDHelper.GetGuid());
				mCheckGroupModel.IsNew = true;
				List<CommandInfo> insertOrUpdateCmd = gLCheckGroupBusiness.GetInsertOrUpdateCmd(ctx, mCheckGroupModel, null);
				list.AddRange(insertOrUpdateCmd);
			}
			else
			{
				empty = modelByFilter.MItemID;
			}
			bDAccountModel.MCheckGroupID = empty;
			List<string> list2 = new List<string>();
			list2.Add("MCheckGroupID");
			list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDAccountModel>(ctx, bDAccountModel, list2, true));
			int num = BDRepository.ExecuteSqlTran(ctx, list);
			operationResult.Success = (num > 0);
			return operationResult;
		}

		private GLCheckGroupModel GetDeleteCheckGroupModel(GLCheckGroupModel checkGroupModel, int deleteCheckType)
		{
			switch (deleteCheckType)
			{
			case 0:
				checkGroupModel.MContactID = 0;
				break;
			case 1:
				checkGroupModel.MEmployeeID = 0;
				break;
			case 3:
				checkGroupModel.MExpItemID = 0;
				break;
			case 2:
				checkGroupModel.MMerItemID = 0;
				break;
			case 4:
				checkGroupModel.MPaItemID = 0;
				break;
			case 5:
				checkGroupModel.MTrackItem1 = 0;
				break;
			case 6:
				checkGroupModel.MTrackItem2 = 0;
				break;
			case 7:
				checkGroupModel.MTrackItem3 = 0;
				break;
			case 8:
				checkGroupModel.MTrackItem4 = 0;
				break;
			case 9:
				checkGroupModel.MTrackItem5 = 0;
				break;
			}
			return checkGroupModel;
		}

		public bool IsAllowUpdateCheckType(MContext ctx, BDAccountEditModel accountModel, BDAccountEditModel oldAccountModel, BDAccountEditModel parentAccount, out string errorMessage)
		{
			errorMessage = string.Empty;
			bool result = true;
			GLCheckGroupModel mCheckGroupModel = accountModel.MCheckGroupModel;
			if (mCheckGroupModel == null)
			{
				return result;
			}
			StringBuilder stringBuilder = new StringBuilder();
			GLInitBalanceBusiness gLInitBalanceBusiness = new GLInitBalanceBusiness();
			OperationResult operationResult = null;
			operationResult = ((!string.IsNullOrWhiteSpace(accountModel.MItemID)) ? gLInitBalanceBusiness.IsCanUpdateAccountCheckGroup(ctx, accountModel, oldAccountModel) : gLInitBalanceBusiness.IsCanUpdateAccountCheckGroup(ctx, parentAccount, oldAccountModel));
			if (!operationResult.Success)
			{
				string value = (!string.IsNullOrWhiteSpace(operationResult.Message)) ? operationResult.Message : COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "InitBalanceIsExistData", "科目的期初余额存在数据，不允许更改科目核算维度！");
				stringBuilder.AppendLine(value);
				errorMessage = stringBuilder.ToString();
				return false;
			}
			GLBalanceBusiness gLBalanceBusiness = new GLBalanceBusiness();
			OperationResult operationResult2 = null;
			operationResult2 = ((!string.IsNullOrWhiteSpace(accountModel.MItemID)) ? gLBalanceBusiness.IsCanUpdateAccountCheckGroup(ctx, accountModel, oldAccountModel) : gLBalanceBusiness.IsCanUpdateAccountCheckGroup(ctx, parentAccount, oldAccountModel));
			if (!operationResult2.Success)
			{
				string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "BalanceIsExistData", "科目的余额存在数据，不允许更改科目核算维度！");
				stringBuilder.AppendLine(text);
				errorMessage = stringBuilder.ToString();
				return false;
			}
			OperationResult operationResult3 = new OperationResult();
			operationResult3.Success = true;
			if (!operationResult3.Success)
			{
				string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "VoucherIsExistData", "存在凭证数据，不允许更改科目核算维度！");
				stringBuilder.AppendLine(text2);
				errorMessage = stringBuilder.ToString();
				return result;
			}
			return result;
		}

		public List<BDAccountModel> GetAccountListWithCheckType(MContext ctx, string itemID = null, bool includeParent = false)
		{
			List<BDAccountModel> accountListWithCheckType = newDal.GetAccountListWithCheckType(ctx, itemID, false, false);
			return BDAccountHelper.OrderBy(accountListWithCheckType);
		}

		public List<BDAccountModel> GetCurrentAccountBaseData(MContext ctx)
		{
			return newDal.GetCurrentAccountBaseData(ctx, true);
		}

		private void ValidateAccountUpdate(MContext ctx, BDAccountModel account)
		{
			ValidateQueryModel validateAccountSql = utility.GetValidateAccountSql(ctx, account);
			ValidateQueryModel validteAccountNameSql = utility.GetValidteAccountNameSql(ctx, account);
			ValidateQueryModel validateAccountNumberSql = utility.GetValidateAccountNumberSql(ctx, account);
			ValidateQueryModel validateQueryModel = null;
			if (account.MParentID != "0")
			{
				validateQueryModel = utility.GetValidateCommonModelSql<BDAccountModel>(MActionResultCodeEnum.MAccountParentInvalid, new List<string>
				{
					account.MParentID
				}, null, null);
			}
			ValidateQueryModel validateTrackItemGroupQuery = utility.GetValidateTrackItemGroupQuery(ctx, account.MCheckGroupModel);
			ValidateQueryModel validateQueryModel2 = new ValidateQueryModel();
			List<BDAccountModel> accountWithParentList = GLDataPool.GetInstance(ctx, false, 0, 0, 0).AccountWithParentList;
			BDAccountModel bDAccountModel = accountWithParentList.FirstOrDefault((BDAccountModel x) => x.MItemID == account.MParentID);
			List<BDAccountModel> list = (from x in accountWithParentList
			where x.MParentID == account.MParentID
			select x).ToList();
			string text = account.IsNew ? string.Empty : account.MItemID;
			GLCheckGroupModel mCheckGroupModel = account.MCheckGroupModel;
			if ((account.IsNew || string.IsNullOrWhiteSpace(account.MItemID)) && list.Count == 0)
			{
				text = account.MParentID;
			}
			utility.QueryValidateSql(ctx, true, validateAccountSql, validteAccountNameSql, validateAccountNumberSql, validateQueryModel, validateTrackItemGroupQuery, validateQueryModel2);
		}

		public BDAccountModel GetFirstLeafAccountByCode(MContext ctx, string code, List<BDAccountModel> accountList = null)
		{
			accountList = (accountList ?? GetAccountListWithCheckType(ctx, null, false));
			return accountList.FirstOrDefault((BDAccountModel x) => x.MCode.IndexOf(code) == 0);
		}

		public List<BDAccountModel> GetAllLeafAccountByCode(MContext ctx, string code, List<BDAccountModel> accountList = null)
		{
			accountList = (accountList ?? GetAccountListWithCheckType(ctx, null, false));
			return (from x in accountList
			where x.MCode.IndexOf(code) == 0
			select x).ToList();
		}

		public OperationResult ValidataData(MContext ctx, List<BDAccountEditModel> acctList)
		{
			OperationResult operationResult = new OperationResult();
			operationResult.Success = true;
			if (acctList == null || acctList.Count == 0)
			{
				return operationResult;
			}
			List<BDAccountModel> accountIncludeDisable = GLDataPool.GetInstance(ctx, false, 0, 0, 0).AccountIncludeDisable;
			List<GLCheckTypeModel> modelList = new GLCheckTypeBusiness().GetModelList(ctx, new SqlWhere(), false);
			foreach (BDAccountEditModel acct in acctList)
			{
				decimal num = default(decimal);
				if (!string.IsNullOrWhiteSpace(acct.MNumber))
				{
					if (!decimal.TryParse(acct.MNumber.Replace(".", ""), out num))
					{
						string message = acct.MNumber + ":" + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "AccountNumberInvalidity", "科目代码值无效！");
						BizVerificationInfor erroInfo = GetErroInfo(message, 2, null, acct.MRowIndex, "MNumber");
						operationResult.VerificationInfor.Add(erroInfo);
					}
					else if (acct.MNumber.IndexOf("1001") == 0 || acct.MNumber.IndexOf("1002") == 0)
					{
						string message2 = string.Format(COMMultiLangRepository.GetText(LangModule.Acct, "CashOrBankAccountCantBeImport", "现金或银行科目（{0}）不支持导入，请在会计科目页面添加！"), acct.MNumber);
						BizVerificationInfor erroInfo2 = GetErroInfo(message2, 2, null, acct.MRowIndex, "MNumber");
						operationResult.VerificationInfor.Add(erroInfo2);
					}
				}
				if (!string.IsNullOrWhiteSpace(acct.MCheckGroupNames))
				{
					string[] array = acct.MCheckGroupNames.Trim('/').Split('/');
					List<string> list = (from c in array
					group c by c into g
					where g.Count() > 1
					select g into f
					select f.Key).ToList();
					if (list.Any())
					{
						string message3 = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "DuplicateCheckGroup", "辅助核算不能重复:{0}"), string.Join("、", list));
						BizVerificationInfor erroInfo3 = GetErroInfo(message3, 2, null, acct.MRowIndex, "MCheckGroupNames");
						operationResult.VerificationInfor.Add(erroInfo3);
					}
					string[] array2 = array;
					foreach (string checkType in array2)
					{
						if (modelList != null && modelList.Count != 0 && !modelList.Exists((GLCheckTypeModel x) => x.MName == checkType))
						{
							string message4 = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "CanNotFindCheckGroup", "Can not find check group:{0}"), checkType);
							BizVerificationInfor erroInfo4 = GetErroInfo(message4, 2, null, acct.MRowIndex, "MCheckGroupNames");
							operationResult.VerificationInfor.Add(erroInfo4);
						}
					}
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

		public List<BDAccountGroupEditModel> GetBDAccountGroupList(MContext ctx, string filterString)
		{
			bool isSys = ctx.IsSys;
			try
			{
				ctx.IsSys = false;
				BDAccountRepository bDAccountRepository = new BDAccountRepository();
				List<BDAccountGroupEditModel> list = bDAccountRepository.GetBDAccountGroupList(ctx, filterString);
				if (ctx.MAccountTableID == "2")
				{
					list = (from x in list
					where x.MItemID != 6.ToString()
					select x).ToList();
				}
				return list;
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				ctx.IsSys = isSys;
			}
		}

		public List<BDAccountGroupEditModel> GetBDTopAccountGroupList(MContext ctx)
		{
			BDAccountRepository bDAccountRepository = new BDAccountRepository();
			return bDAccountRepository.GetBDAccountGroupList(ctx, null);
		}

		public List<BDAccountTypeListModel> GetBDAccountTypeList(MContext ctx, string filterString)
		{
			BDAccountRepository bDAccountRepository = new BDAccountRepository();
			return bDAccountRepository.GetBDAccountTypeList(ctx, filterString);
		}

		public List<BDAccountListModel> GetBDAccountList(MContext ctx, string strWhere)
		{
			BDAccountRepository bDAccountRepository = new BDAccountRepository();
			return bDAccountRepository.GetBDAccountList(ctx, strWhere);
		}

		public List<BDAccountListModel> GetAccountList(MContext ctx, SqlWhere filter, bool ignoreLocale = false)
		{
			BDAccountRepository bDAccountRepository = new BDAccountRepository();
			return bDAccountRepository.GetBDAccountList(ctx, filter, null, true, ignoreLocale);
		}

		public List<BDAccountListModel> GetAccountList(MContext ctx, SqlWhere filter)
		{
			return GetAccountList(ctx, filter, false);
		}

		public BDAccountListModel GetAccountByCode(List<BDAccountListModel> accounts, string code)
		{
			return accounts.Find((BDAccountListModel x) => x.MCode != null && x.MCode.Equals(code));
		}

		public List<BDAccountListModel> GetCurrentAccountInfo(MContext ctx)
		{
			return dal.GetCurrentAccountInfo(ctx, false);
		}

		public List<BDAccountModel> GetBDAccountList(MContext ctx, SqlWhere filter, bool includeParent = false)
		{
			BDAccountRepository bDAccountRepository = new BDAccountRepository();
			List<BDAccountModel> baseBDAccountList = bDAccountRepository.GetBaseBDAccountList(ctx, filter, false, null);
			List<BDAccountModel> list = new List<BDAccountModel>();
			if (!includeParent)
			{
				foreach (BDAccountModel item in baseBDAccountList)
				{
					if (baseBDAccountList.Count((BDAccountModel x) => x.MParentID == item.MItemID) == 0)
					{
						list.Add(item);
					}
				}
			}
			else
			{
				list = baseBDAccountList;
			}
			return list;
		}

		public List<BDAccountListModel> GetAccountListIncludeBalance(MContext ctx, SqlWhere filter, bool includeParent = false)
		{
			BDAccountRepository bDAccountRepository = new BDAccountRepository();
			List<BDAccountListModel> bDAccountList = bDAccountRepository.GetBDAccountList(ctx, filter, null, true, false);
			List<BDAccountListModel> list = new List<BDAccountListModel>();
			if (!includeParent)
			{
				foreach (BDAccountListModel item in bDAccountList)
				{
					if (bDAccountList.Count((BDAccountListModel x) => x.MParentID == item.MItemID) == 0)
					{
						list.Add(item);
					}
				}
			}
			else
			{
				list = bDAccountList;
			}
			return list;
		}

		public List<BDAccountListModel> GetAccountListForExport(MContext ctx, BDAccountListFilterModel filter = null)
		{
			if (filter == null)
			{
				filter = new BDAccountListFilterModel();
			}
			filter.IncludeAllLangName = true;
			filter.IsAll = false;
			List<BDAccountListModel> accountListIncludeCheckType = dal.GetAccountListIncludeCheckType(ctx, filter);
			TrackModelList = _track.GetTrackBasicInfo(ctx, ctx.MOrgID, false, true);
			UpdateAccountList(ctx, accountListIncludeCheckType, null);
			return accountListIncludeCheckType;
		}

		public List<BDAccountModel> GetBDAccountList(MContext ctx, BDAccountListFilterModel filter, bool includeParent = false, bool isParentName = false)
		{
			List<BDAccountModel> baseBDAccountList = dal.GetBaseBDAccountList(ctx, filter, filter.IncludeDisable, null);
			List<BDAccountModel> list = new List<BDAccountModel>();
			if (!includeParent)
			{
				foreach (BDAccountModel item in baseBDAccountList)
				{
					bool flag = baseBDAccountList.Count((BDAccountModel x) => x.MParentID == item.MItemID) == 0;
					bool flag2 = false;
					if (filter != null && !string.IsNullOrWhiteSpace(filter.ParentCodes))
					{
						List<string> list2 = new List<string>(filter.ParentCodes.Split(','));
						foreach (string item2 in list2)
						{
							if (item.MCode != null && item.MCode.IndexOf(item2) == 0)
							{
								flag2 = true;
							}
						}
					}
					if (flag && (flag2 || string.IsNullOrWhiteSpace(filter.ParentCodes)))
					{
						list.Add(item);
					}
				}
			}
			else
			{
				list = baseBDAccountList;
			}
			if (isParentName)
			{
				foreach (BDAccountModel item3 in list)
				{
					if (!(item3.MParentID == "0"))
					{
						string[] array = item3.MNumber.Split('.');
						string text = "";
						string tempNumber = "";
						for (int i = 0; i < array.Length; i++)
						{
							tempNumber += array[i];
							BDAccountModel bDAccountModel = (from x in baseBDAccountList
							where x.MNumber == tempNumber
							select x).FirstOrDefault();
							if (bDAccountModel != null)
							{
								text = text + bDAccountModel.MName + "-";
							}
							tempNumber += ".";
						}
						if (!string.IsNullOrWhiteSpace(text))
						{
							text = text.Substring(0, text.Length - 1);
						}
						item3.MName = text;
					}
				}
			}
			return list;
		}

		private SqlWhere ConvertFilterToSqlWhere(BDAccountListFilterModel filter)
		{
			SqlWhere sqlWhere = new SqlWhere();
			if (filter != null)
			{
				if (filter.IsActive)
				{
					sqlWhere.Equal("a.MIsActive ", "1");
				}
				else
				{
					sqlWhere.Equal("a.MIsActive ", "0");
				}
				if (!string.IsNullOrWhiteSpace(filter.Group))
				{
					sqlWhere.Equal("cl.MName", filter.Group);
				}
			}
			else
			{
				sqlWhere.Equal("a.MIsActive ", "1");
			}
			return sqlWhere;
		}

		private void UpdateAccountList(MContext ctx, IEnumerable<BDAccountListModel> allAcctList, IEnumerable<BDAccountListModel> acctList = null)
		{
			string empty = string.Empty;
			IEnumerable<BDAccountListModel> enumerable = acctList ?? allAcctList;
			foreach (BDAccountListModel item in enumerable)
			{
				IEnumerable<BDAccountListModel> enumerable2 = from f in allAcctList
				where f.MParentID == item.MItemID
				select f;
				if (enumerable2.Any())
				{
					foreach (BDAccountListModel item2 in enumerable2)
					{
						item2.MName = "   " + item2.MName;
					}
					UpdateAccountList(ctx, allAcctList, from f in allAcctList
					where f.MParentID == item.MItemID
					select f);
				}
				List<string> list = new List<string>();
				item.MCheckGroupName = GetAccountCheckTypeName(ctx, item.MCheckGroupModel, list);
				item.MCheckGroupNameList = list;
			}
		}

		private AccountItemTreeModel FindAccountChildItem(BDAccountListModel parentModel, List<BDAccountListModel> accountList, bool showNumber = false)
		{
			AccountItemTreeModel accountItemTreeModel = ToAccountItemTreeModel(parentModel, showNumber);
			IEnumerable<BDAccountListModel> enumerable = from x in accountList
			where x.MParentID == parentModel.MItemID
			select x;
			List<BDAccountListModel> list = (enumerable != null) ? enumerable.ToList() : new List<BDAccountListModel>();
			accountItemTreeModel.children = ((accountItemTreeModel.children == null) ? new List<AccountItemTreeModel>() : accountItemTreeModel.children);
			if (list.Count == 0)
			{
				return accountItemTreeModel;
			}
			foreach (BDAccountListModel item in list)
			{
				accountItemTreeModel.children.Add(FindAccountChildItem(item, accountList, showNumber));
			}
			return accountItemTreeModel;
		}

		public AccountItemTreeModel ToAccountItemTreeModel(BDAccountListModel model, bool showNumber = false)
		{
			AccountItemTreeModel accountItemTreeModel = new AccountItemTreeModel();
			accountItemTreeModel.id = model.MItemID;
			accountItemTreeModel.text = (showNumber ? (model.MNumber + " " + model.MName) : model.MName);
			accountItemTreeModel.MIsSys = model.MIsSys;
			accountItemTreeModel.MNumber = model.MNumber;
			accountItemTreeModel.MAccountGroupName = model.MAcctGroupName;
			accountItemTreeModel.MAccountGroupID = model.MAccountGroupID;
			accountItemTreeModel.MAccountTypeID = model.MAccountTypeID;
			accountItemTreeModel.MAcctTypeName = model.MAcctTypeName;
			accountItemTreeModel.MDC = model.MDC;
			accountItemTreeModel.MIsCheckForCurrency = model.MIsCheckForCurrency;
			accountItemTreeModel.IsCanRelateContact = model.IsCanRelateContact;
			accountItemTreeModel.MCode = model.MCode;
			return accountItemTreeModel;
		}

		public OperationResult ImportCustomAccountList(MContext ctx, List<BDAccountEditModel> accountList)
		{
			OperationResult operationResult = new OperationResult();
			if (accountList == null && accountList.Count() == 0)
			{
				string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "NotUpdateContent", "没有需要更新的内容");
				throw new Exception(text);
			}
			List<CommandInfo> list = new List<CommandInfo>();
			List<CommandInfo> clearAccountCmds = BDAccountRepository.GetClearAccountCmds(ctx);
			list.AddRange(clearAccountCmds);
			BDAcctTypeRepository bDAcctTypeRepository = new BDAcctTypeRepository();
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.Equal("MAccountTableID", ctx.MAccountTableID);
			List<BDAcctTypeModel> modelList = bDAcctTypeRepository.GetModelList(ctx, sqlWhere, false);
			string empty = string.Empty;
			int num = 7000;
			accountList = (from x in accountList
			orderby x.MNumber.Trim()
			select x).ToList();
			ValidateAccountName(ctx, accountList, operationResult, null);
			if (!operationResult.Success)
			{
				return operationResult;
			}
			foreach (BDAccountEditModel account in accountList)
			{
				account.MOrgID = ctx.MOrgID;
				account.MAccountTableID = "3";
				account.MCode = null;
				BDAcctTypeModel bDAcctTypeModel = (from x in modelList
				where x.MItemID == account.MAccountTypeID
				select x).First();
				if (bDAcctTypeModel != null)
				{
					account.MAccountGroupID = bDAcctTypeModel.MAccountGroupID;
				}
				BDAccountEditModel parentAccountModel = (from x in accountList
				where x.MItemID == account.MParentID
				select x).FirstOrDefault();
				if (parentAccountModel != null)
				{
					List<BDAccountEditModel> list2 = (from x in accountList
					where x.MParentID == parentAccountModel.MItemID && !string.IsNullOrWhiteSpace(x.MCode)
					select x).ToList();
					string text2 = "";
					if (list2 != null && list2.Count() > 0)
					{
						list2 = (from x in list2
						orderby x.MNumber.Trim()
						select x).ToList();
						string mCode = list2.Last().MCode;
						int value = int.Parse(mCode) + 1;
						text2 = Convert.ToString(value);
					}
					else
					{
						text2 = parentAccountModel.MCode + "01";
					}
					account.MCode = text2;
				}
				else
				{
					account.MCode = Convert.ToString(num);
					num++;
				}
				account.IsNew = true;
				List<CommandInfo> insertOrUpdateCmd = ModelInfoManager.GetInsertOrUpdateCmd<BDAccountEditModel>(ctx, account, null, true);
				list.AddRange(insertOrUpdateCmd);
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num2 = dynamicDbHelperMySQL.ExecuteSqlTran(list);
			operationResult.Success = (num2 > 0);
			return operationResult;
		}

		public OperationResult UpdateAccount(MContext ctx, BDAccountEditModel model)
		{
			BDAccountRepository bDAccountRepository = new BDAccountRepository();
			GLCheckGroupRepository gLCheckGroupRepository = new GLCheckGroupRepository();
			List<CommandInfo> list = new List<CommandInfo>();
			OperationResult operationResult = new OperationResult();
			string text = "";
			GLCheckGroupModel mCheckGroupModel = model.MCheckGroupModel;
			List<BDAccountModel> bDModelList = GetBDModelList(ctx);
			ValidateAccountUpdate(ctx, model);
			list.AddRange(GetUpdateAccountCmds(ctx, model, out text, null, null, null));
			List<BDAccountModel> parentAccountByRecursion = BDAccountHelper.GetParentAccountByRecursion(model, bDModelList);
			if (parentAccountByRecursion != null && parentAccountByRecursion.Count() > 0 && string.IsNullOrWhiteSpace(text))
			{
				parentAccountByRecursion.ForEach(delegate(BDAccountModel x)
				{
					x.MCheckGroupID = "0";
				});
				List<string> list2 = new List<string>();
				list2.Add("MCheckGroupID");
				list.AddRange(ModelInfoManager.GetInsertOrUpdateCmds(ctx, parentAccountByRecursion, list2, true));
			}
			list.AddRange(GetUpdateAccountFullNameCmds(ctx, model, null, bDModelList));
			if (string.IsNullOrEmpty(text))
			{
				int num = BDRepository.ExecuteSqlTran(ctx, list);
				operationResult.Success = (num > 0);
				IsoDateTimeConverter isoDateTimeConverter = new IsoDateTimeConverter
				{
					DateTimeFormat = "yyyy-MM-dd HH:mm:ss"
				};
				JsonConverter[] converters = new JsonConverter[1]
				{
					isoDateTimeConverter
				};
				string text3 = operationResult.ObjectID = JsonConvert.SerializeObject(model, Formatting.None, converters);
			}
			else
			{
				operationResult.Success = false;
				operationResult.Message = text;
			}
			return operationResult;
		}

		public OperationResult SetAccountCreateInitBill(MContext ctx, string accountId, bool createInitBill)
		{
			OperationResult operationResult = new OperationResult();
			BDAccountModel bDAccountEditModel = dal.GetBDAccountEditModel(ctx, accountId);
			if (bDAccountEditModel == null)
			{
				string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "NotFindMatchAccount", "没有找到匹配的科目！");
				operationResult.Success = false;
				operationResult.Message = text;
				return operationResult;
			}
			if (!createInitBill)
			{
				GLInitBalanceRepository gLInitBalanceRepository = new GLInitBalanceRepository();
				SqlWhere sqlWhere = new SqlWhere();
				sqlWhere.Equal("MAccountID", accountId);
				List<GLInitBalanceModel> initBalanceListIncludeCheckGroupValue = gLInitBalanceRepository.GetInitBalanceListIncludeCheckGroupValue(ctx, sqlWhere);
				if (initBalanceListIncludeCheckGroupValue != null && initBalanceListIncludeCheckGroupValue.Count() > 0 && initBalanceListIncludeCheckGroupValue.Exists((GLInitBalanceModel x) => !string.IsNullOrWhiteSpace(x.MBillID) && !string.IsNullOrWhiteSpace(x.MBillType)))
				{
					operationResult.Success = false;
					operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "AccountExistInitBill", "科目存在期初单据，无法取消是否生成期初单据！");
					return operationResult;
				}
			}
			bDAccountEditModel.MCreateInitBill = createInitBill;
			return dal.InsertOrUpdate(ctx, bDAccountEditModel, "MCreateInitBill");
		}

		public List<CommandInfo> GetUpdateAccountCmds(MContext ctx, BDAccountEditModel model, out string message, List<BDAccountEditModel> importList = null, List<GLInitBalanceModel> initBalanceList = null, List<GLBalanceModel> balanceList = null)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			BDAccountRepository bDAccountRepository = new BDAccountRepository();
			List<BDAccountModel> list2 = new List<BDAccountModel>();
			message = "";
			bool flag = importList != null;
			if (!flag)
			{
				list2 = dal.GetBaseBDAccountList(ctx, new SqlWhere(), true, null);
			}
			if (!flag)
			{
				decimal num = default(decimal);
				if (!decimal.TryParse(model.MNumber.Replace(".", ""), out num))
				{
					message = model.MNumber + ":" + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "AccountNumberOnlyNumber", "科目代码只能是数字！");
					return list;
				}
				if (CheckAccountNumberTooLength(model.MNumber))
				{
					message = model.MNumber + ":" + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "AccountNumberTooLength", "科目代码每级最大4位!");
					return list;
				}
				SqlWhere sqlWhere = new SqlWhere();
				sqlWhere.Equal("MNumber", model.MNumber);
				sqlWhere.Equal("MOrgID", ctx.MOrgID);
				sqlWhere.Equal("MIsDelete", 0);
				bool flag2 = true;
				if (!string.IsNullOrWhiteSpace(model.MItemID))
				{
					BDAccountEditModel bDAccountEditModel = dal.GetBDAccountEditModel(ctx, model.MItemID);
					flag2 = (bDAccountEditModel.MNumber != model.MNumber);
					if (bDAccountEditModel.MIsCheckForCurrency != model.MIsCheckForCurrency && bDAccountEditModel.MIsCheckForCurrency)
					{
						OperationResult operationResult = BDRepository.IsCanDelete(ctx, "Account", bDAccountEditModel.MItemID);
						if (!operationResult.Success)
						{
							message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "CanNotChangeAccountCurrencyType", "科目已经使用，不能更改为非外币核算！");
							list.Clear();
							return list;
						}
					}
					model.MCreateInitBill = bDAccountEditModel.MCreateInitBill;
				}
				if (CheckAccountNameIsExist(ctx, model))
				{
					message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "AccountNameExist", "科目名称已经存在，请使用其他名称！");
					return list;
				}
				if (ctx.MAccountTableID == "3")
				{
					model.MAccountTableID = "3";
					BDAcctTypeRepository bDAcctTypeRepository = new BDAcctTypeRepository();
					if (string.IsNullOrWhiteSpace(model.MAccountTypeID))
					{
						message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "NotSelectAccountType", "请选择一个科目类型！");
						return list;
					}
					BDAcctTypeModel dataModel = bDAcctTypeRepository.GetDataModel(ctx, model.MAccountTypeID, false);
					model.MAccountGroupID = dataModel.MAccountGroupID;
					if (string.IsNullOrWhiteSpace(model.MItemID) && (string.IsNullOrWhiteSpace(model.MParentID) || model.MParentID == "0"))
					{
						model.MCode = GetCustomAccountCode(ctx, model, list2);
					}
				}
				if ((flag2 && bDAccountRepository.ExistsByFilter(ctx, sqlWhere)) || (string.IsNullOrWhiteSpace(model.MItemID) && bDAccountRepository.ExistsByFilter(ctx, new SqlWhere().Equal("MOrgID", ctx.MOrgID).Equal("MCode", model.MCode).Equal("MIsDelete", 0))))
				{
					message = model.MName + ":" + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "AccountCodeExist", "Account code Already exist,please use other!");
					return list;
				}
				if (string.IsNullOrEmpty(model.MParentID))
				{
					message = model.MName + ":" + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "NoParentItem", "Please select a parent item");
					return list;
				}
			}
			string pkID = model.MParentID;
			if (!string.IsNullOrWhiteSpace(model.MExistParentID))
			{
				pkID = model.MExistParentID;
			}
			BDAccountModel parentModel = bDAccountRepository.GetDataModel(ctx, pkID, false);
			if (parentModel == null && model.MParentID != "0")
			{
				message = model.MName + ":" + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "CanNotFindParentItem", "Can not find parent item");
				return list;
			}
			OperationResult operationResult2 = new OperationResult();
			operationResult2.Success = false;
			if (model.MParentID != "0")
			{
				model.MAccountTableID = parentModel.MAccountTableID;
				model.MAccountGroupID = parentModel.MAccountGroupID;
				model.MAccountTypeID = parentModel.MAccountTypeID;
				model.MDC = ((model.MDC != 0) ? model.MDC : parentModel.MDC);
				if (string.IsNullOrWhiteSpace(model.MCode))
				{
					model.MCode = parentModel.MCode + dal.GetAccountCodeIncreaseNumber(ctx, parentModel.MItemID, parentModel.MCode);
				}
				else
				{
					model.MCode = model.MCode;
				}
				model.IsCanRelateContact = parentModel.IsCanRelateContact;
				operationResult2 = IsFirstLowerLevelAccount(ctx, parentModel.MItemID, null);
				if (operationResult2.Success)
				{
					model.MCreateInitBill = parentModel.MCreateInitBill;
				}
			}
			SetDefaultAccountCheckType(ctx, model);
			GLCheckGroupModel mCheckGroupModel = model.MCheckGroupModel;
			if (mCheckGroupModel != null)
			{
				string empty = string.Empty;
				BDAccountEditModel bDAccountEditModel2 = null;
				BDAccountEditModel bDAccountEditModel3 = null;
				if (string.IsNullOrWhiteSpace(model.MItemID))
				{
					bDAccountEditModel2 = GetBDAccountEditModel(ctx, model.MParentID, null);
				}
				else
				{
					bDAccountEditModel3 = GetBDAccountEditModel(ctx, model.MItemID, null);
				}
				string empty2 = string.Empty;
				list.AddRange(GetCheckGroupInsertOrUpdateCmds(ctx, model.MCheckGroupModel, out empty2));
				if (!string.IsNullOrEmpty(empty2))
				{
					model.MCheckGroupID = empty2;
				}
			}
			model.MOrgID = ctx.MOrgID;
			if (!flag && string.IsNullOrEmpty(model.MItemID))
			{
				model.MItemID = UUIDHelper.GetGuid();
				model.IsNew = true;
			}
			if (!flag)
			{
				BDBankAccountEditModel bDBankAccountEditModel = new BDBankAccountEditModel();
				if (IsBankOrCashAccount(ctx, model.MItemID, out bDBankAccountEditModel))
				{
					BDBankAccountRepository bDBankAccountRepository = new BDBankAccountRepository();
					bDBankAccountEditModel.MultiLanguage = GetBankAccountMulitLang(model.MultiLanguage, bDBankAccountEditModel.MultiLanguage);
					bDBankAccountRepository.InsertOrUpdate(ctx, bDBankAccountEditModel, null);
					list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDBankAccountEditModel>(ctx, bDBankAccountEditModel, null, true));
				}
			}
			list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDAccountEditModel>(ctx, model, model.UpdateFieldList, true));
			if (!flag && model.MIsCheckForCurrency && model.MParentID != "0")
			{
				BDAccountModel bDAccountModel = new BDAccountModel();
				bDAccountModel.MItemID = model.MItemID;
				bDAccountModel.MParentID = model.MParentID;
				List<BDAccountModel> parentAccountByRecursion = GetParentAccountByRecursion(bDAccountModel, list2);
				if (parentAccountByRecursion != null && parentAccountByRecursion.Count() > 0)
				{
					foreach (BDAccountModel item in parentAccountByRecursion)
					{
						item.MIsCheckForCurrency = true;
					}
					List<string> list3 = new List<string>();
					list3.Add("MIsCheckForCurrency");
					list.AddRange(ModelInfoManager.GetInsertOrUpdateCmds(ctx, parentAccountByRecursion, list3, true));
				}
			}
			if (!flag && !string.IsNullOrWhiteSpace(model.MItemID))
			{
				if (list2 == null)
				{
					list2 = dal.GetBaseBDAccountList(ctx, new SqlWhere(), false, null);
				}
				BDAccountModel bDAccountModel2 = new BDAccountModel();
				bDAccountModel2.MItemID = model.MItemID;
				bDAccountModel2.MParentID = model.MParentID;
				List<BDAccountModel> childrenAccountByRecursion = GetChildrenAccountByRecursion(bDAccountModel2, list2, false);
				if (childrenAccountByRecursion != null && childrenAccountByRecursion.Count() > 0)
				{
					string[] array = model.MNumber.Split('.');
					int num2 = array.Length;
					string text = array[num2 - 1];
					foreach (BDAccountModel item2 in childrenAccountByRecursion)
					{
						string[] array2 = item2.MNumber.Split('.');
						array2[num2 - 1] = text;
						item2.MNumber = string.Join(".", array2);
					}
					List<string> list4 = new List<string>();
					list4.Add("MNumber");
					list.AddRange(ModelInfoManager.GetInsertOrUpdateCmds(ctx, childrenAccountByRecursion, list4, true));
				}
			}
			if (operationResult2.Success)
			{
				List<CommandInfo> updateBaseDataAccountInfoCmdList = GetUpdateBaseDataAccountInfoCmdList(ctx, model, parentModel);
				if (updateBaseDataAccountInfoCmdList != null)
				{
					list.AddRange(updateBaseDataAccountInfoCmdList);
				}
			}
			if (operationResult2.Success)
			{
				list.Add(GLVoucherEntryRepository.GetUpdateEntryAccountIDCmd(ctx, model.MItemID, model.MParentID));
				SqlWhere sqlWhere2 = new SqlWhere();
				sqlWhere2.Equal("MAccountID", parentModel.MItemID);
				initBalanceList = ((initBalanceList != null) ? (from f in initBalanceList
				where f.MAccountID == parentModel.MItemID
				select f).ToList() : BDAccountRepository.GetInitBalanceList(ctx, sqlWhere2));
				if (initBalanceList != null)
				{
					foreach (GLInitBalanceModel initBalance in initBalanceList)
					{
						initBalance.MItemID = null;
						initBalance.MAccountID = model.MItemID;
						list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<GLInitBalanceModel>(ctx, initBalance, null, true));
					}
				}
				balanceList = ((balanceList != null) ? (from f in balanceList
				where f.MAccountID == parentModel.MItemID
				select f).ToList() : balanceDal.GetModelList(ctx, sqlWhere2, true));
				if (balanceList != null)
				{
					foreach (GLBalanceModel balance in balanceList)
					{
						balance.MItemID = null;
						balance.MAccountID = model.MItemID;
						list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<GLBalanceModel>(ctx, balance, null, true));
					}
				}
			}
			return list;
		}

		private bool CheckAccountNameIsExist(MContext ctx, BDAccountEditModel model)
		{
			bool result = false;
			List<BDAccountModel> bDModelList = GetBDModelList(ctx);
			List<BDAccountModel> list = (from x in bDModelList
			where x.MParentID == model.MParentID
			select x).ToList();
			if (!string.IsNullOrWhiteSpace(model.MItemID))
			{
				list = (from x in list
				where x.MItemID != model.MItemID
				select x).ToList();
			}
			if (list == null || list.Count == 0)
			{
				return result;
			}
			List<BASLangModel> orgLangList = new BASLangBusiness().GetOrgLangList(ctx);
			if (orgLangList == null || orgLangList.Count == 0)
			{
				throw new NullReferenceException("org not select any language");
			}
			string languageFieldName = "MName";
			foreach (BASLangModel item in orgLangList)
			{
				if (!(item.LangID == LangCodeEnum.EN_US))
				{
					List<string> source = (from x in list
					select x.GetMultiLanguageValue(item.LangID, languageFieldName)).ToList();
					string currentName = model.GetMultiLanguageValue(item.LangID, languageFieldName);
					if (source.Any((string x) => x == currentName))
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}

		private bool CheckAccountNumberTooLength(string number)
		{
			if (string.IsNullOrWhiteSpace(number))
			{
				return true;
			}
			List<string> list = number.Split('.').ToList();
			return list.Exists((string x) => x.Length > 4);
		}

		public List<CommandInfo> GetUpdateBaseDataAccountInfoCmdList(MContext ctx, BDAccountModel model, BDAccountModel parentModel)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			BDContactsRepository bDContactsRepository = new BDContactsRepository();
			list.AddRange(bDContactsRepository.UpdateContactMapAccount(ctx, parentModel.MCode, model.MCode));
			BDEmployeesRepository bDEmployeesRepository = new BDEmployeesRepository();
			list.AddRange(bDEmployeesRepository.UpdateEmpMapAccount(ctx, parentModel.MCode, model.MCode));
			BDExpenseItemRepository bDExpenseItemRepository = new BDExpenseItemRepository();
			list.AddRange(bDExpenseItemRepository.UpdateExpenseItemMapAccount(ctx, parentModel.MCode, model.MCode));
			BDItemRepository bDItemRepository = new BDItemRepository();
			list.AddRange(bDItemRepository.UpdateItemMapAccount(ctx, parentModel.MCode, model.MCode));
			PAPayItemRepository pAPayItemRepository = new PAPayItemRepository();
			list.AddRange(pAPayItemRepository.UpdatePayItemMapAccount(ctx, parentModel.MCode, model.MCode));
			REGTaxRateRepository rEGTaxRateRepository = new REGTaxRateRepository();
			list.AddRange(rEGTaxRateRepository.UpdateTaxrateMapAccount(ctx, parentModel.MCode, model.MCode));
			list.AddRange(IVInvoiceRepository.UpdateCurrentAccount(ctx, parentModel.MCode, model.MCode));
			list.AddRange(IVReceiveRepository.UpdateCurrentAccount(ctx, parentModel.MCode, model.MCode));
			list.AddRange(IVPaymentRepository.UpdateCurrentAccount(ctx, parentModel.MCode, model.MCode));
			list.AddRange(IVExpenseRepository.UpdateCurrentAccount(ctx, parentModel.MCode, model.MCode));
			list.AddRange(FCCashCodingModuleRepository.UpdateAccountId(ctx, parentModel.MItemID, model.MItemID));
			list.AddRange(FCVoucherModuleEntryRepository.UpdateAccountId(ctx, parentModel.MItemID, model.MItemID));
			list.AddRange(FAFixAssetsRepository.UpdateAccountCode<FAFixAssetsModel>(ctx, parentModel.MCode, model.MCode));
			list.AddRange(FAFixAssetsRepository.UpdateAccountCode<FAFixAssetsChangeModel>(ctx, parentModel.MCode, model.MCode));
			list.AddRange(FAFixAssetsRepository.UpdateAccountCode<FAFixAssetsTypeModel>(ctx, parentModel.MCode, model.MCode));
			list.AddRange(FAFixAssetsRepository.UpdateAccountCode<FADepreciationModel>(ctx, parentModel.MCode, model.MCode));
			return list;
		}

		private void SetDefaultAccountCheckType(MContext ctx, BDAccountEditModel model)
		{
			string mCode = model.MCode;
			GLCheckGroupModel mCheckGroupModel = model.MCheckGroupModel;
			if (mCode.IndexOf("1122") == 0 || mCode.IndexOf("1123") == 0 || mCode.IndexOf("2202") == 0 || mCode.IndexOf("2203") == 0)
			{
				mCheckGroupModel = ((mCheckGroupModel == null) ? new GLCheckGroupModel() : mCheckGroupModel);
				if (mCheckGroupModel.MContactID == CheckTypeStatusEnum.Disabled)
				{
					mCheckGroupModel.MContactID = CheckTypeStatusEnum.Required;
				}
			}
			else if (mCode.IndexOf("1221") == 0)
			{
				mCheckGroupModel = ((mCheckGroupModel == null) ? new GLCheckGroupModel() : mCheckGroupModel);
			}
			else if (mCode.IndexOf("2241") == 0)
			{
				mCheckGroupModel = ((mCheckGroupModel == null) ? new GLCheckGroupModel() : mCheckGroupModel);
			}
		}


		public List<CommandInfo> GetUpdateAccountFullNameCmds(MContext ctx, BDAccountModel model, BDAccountModel parentModel, List<BDAccountModel> accountList)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			List<string> list2 = new List<string>();
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			SysLangList = ((SysLangList == null) ? BASLangRepository.GetOrgLangList(ctx) : SysLangList);
			if (SysLangList == null || SysLangList.Count() == 0)
			{
				throw new NullReferenceException("not setting system language info");
			}
			BDAccountModel bDAccountModel = null;
			if (parentModel != null)
			{
				bDAccountModel = parentModel;
			}
			else if (model.MParentID != "0")
			{
				bDAccountModel = (from x in accountList
								  where x.MItemID == model.MParentID
								  select x).FirstOrDefault();
				if (bDAccountModel == null)
				{
					throw new NullReferenceException("can find parent account:" + model.MNumber);
				}
			}
			else
			{
				bDAccountModel = model;
			}
			MultiLanguageFieldList multiLanguageFieldList = null;
			if (!string.IsNullOrWhiteSpace(model.MItemID) && accountList != null)
			{
				BDAccountModel bDAccountModel2 = (from x in accountList
												  where x.MItemID == model.MItemID
												  select x).FirstOrDefault();
				if (bDAccountModel2 != null)
				{
					multiLanguageFieldList = (from x in bDAccountModel2.MultiLanguage
											  where x.MFieldName == "MFullName"
											  select x).FirstOrDefault();
				}
			}
			foreach (BASLangModel sysLang in SysLangList)
			{
				string key = sysLang.LangID;
				string text;
				if (!dictionary.Keys.Contains(key))
				{
					text = "";
					if (model.MParentID == "0")
					{
						text = string.Format("{0} {1}", model.MNumber, model.GetMultiLanguageValue(key, "MName"));
						goto IL_029c;
					}
					string multiLanguageValue = bDAccountModel.GetMultiLanguageValue(key, "MFullName");
					text = string.Format("{0}-{1}", multiLanguageValue, model.GetMultiLanguageValue(key, "MName"));
					if (!string.IsNullOrWhiteSpace(text))
					{
						int num = text.IndexOf(" ");
						string arg = text.Substring(num + 1, text.Length - num - 1);
						text = $"{model.MNumber} {arg}";
						goto IL_029c;
					}
				}
				continue;
			IL_029c:
				if (multiLanguageFieldList != null)
				{
					MultiLanguageField multiLanguageField = (from x in multiLanguageFieldList.MMultiLanguageField
															 where x.MLocaleID == key
															 select x).FirstOrDefault();
					if (multiLanguageField != null)
					{
						multiLanguageField.MValue = text;
					}
				}
				if (key == ctx.MLCID)
				{
					model.MFullName = text;
				}
				dictionary.Add(key, text);
			}

			MLogger.Log("GetUpdateAccountFullNameCmds -> accountList is null ? " + (accountList == null));

			//List<BDAccountModel> list3 =  (from x in accountList
			//	where x.MParentID == model.MItemID
			//							  select x).ToList();
			List<BDAccountModel> list3 = (accountList == null) ? null : (from x in accountList
																		 where x.MParentID == model.MItemID
																		 select x).ToList<BDAccountModel>();


			if (list3 != null && list3.Count() > 0)
			{
				foreach (BDAccountModel item in list3)
				{
					list.AddRange(GetUpdateAccountFullNameCmds(ctx, item, null, accountList));
				}
			}
			list.AddRange(BDAccountRepository.GetUpdateFullNameCmds(ctx, dictionary, model));
			return list;
		}


		public OperationResult UpdateAccountType(MContext ctx, BDAccountTypeEditModel model)
		{
			BDAccountRepository bDAccountRepository = new BDAccountRepository();
			return bDAccountRepository.UpdateAccountType(ctx, model);
		}

		public OperationResult IsFirstLowerLevelAccount(MContext ctx, string parentId, List<BDAccountModel> accountList = null)
		{
			OperationResult operationResult = new OperationResult();
			if (accountList == null)
			{
				BDAccountRepository bDAccountRepository = new BDAccountRepository();
				accountList = bDAccountRepository.GetBaseBDAccountList(ctx, new SqlWhere(), false, null);
			}
			IEnumerable<BDAccountModel> enumerable = from x in accountList
			where x.MItemID == parentId
			select x;
			if (enumerable == null)
			{
				operationResult.Success = false;
				return operationResult;
			}
			operationResult.Success = ((from x in accountList
			where x.MParentID == parentId
			select x).Count() == 0);
			return operationResult;
		}

		public BDAccountEditModel GetBDAccountEditModel(MContext ctx, string pkID, string parentId)
		{
			BDAccountRepository bDAccountRepository = new BDAccountRepository();
			BDAccountEditModel bDAccountEditModel = new BDAccountEditModel();
			if (string.IsNullOrEmpty(pkID))
			{
				BDAccountModel bDAccountModel = GLDataPool.GetInstance(ctx, false, 0, 0, 0).AccountWithParentList.FirstOrDefault((BDAccountModel x) => x.MItemID == parentId);
				bDAccountEditModel.MNumber = bDAccountModel.MNumber + "." + bDAccountRepository.GetAccountIncreasingNumber(ctx, parentId);
				bDAccountEditModel.MAccountTypeID = bDAccountModel.MAccountTypeID;
				bDAccountEditModel.MAccountGroupID = bDAccountModel.MAccountGroupID;
				bDAccountEditModel.MDC = bDAccountModel.MDC;
				bDAccountEditModel.MIsCheckForCurrency = bDAccountModel.MIsCheckForCurrency;
				bDAccountEditModel.MIsAllowEdit = true;
				bDAccountEditModel.MCheckGroupModel = bDAccountModel.MCheckGroupModel;
				bDAccountEditModel.MParentCode = bDAccountModel.MCode;
			}
			else
			{
				bDAccountEditModel = bDAccountRepository.GetBDAccountEditModel(ctx, pkID);
				List<BDAccountModel> accountWithParentList = GLDataPool.GetInstance(ctx, false, 0, 0, 0).AccountWithParentList;
				BDAccountModel bDAccountModel2 = accountWithParentList.FirstOrDefault((BDAccountModel x) => x.MItemID == pkID);
				if (bDAccountModel2 != null)
				{
					BDAccountModel bDAccountModel3 = bDAccountModel2;
					bDAccountEditModel.MCheckGroupID = bDAccountModel3.MItemID;
					bDAccountEditModel.MCheckGroupModel = bDAccountModel3.MCheckGroupModel;
					BDTrackRepository bDTrackRepository = new BDTrackRepository();
					TrackModelList = bDTrackRepository.GetTrackBasicInfo(ctx, ctx.MOrgID, false, true);
					bDAccountEditModel.MCheckTypeNameRelationList = GetAccountCheckTypeListDic(ctx, bDAccountModel3.MCheckGroupModel);
				}
				OperationResult operationResult = BDRepository.IsCanDelete(ctx, "Account", pkID);
				bDAccountEditModel.MIsAllowEdit = operationResult.Success;
			}
			return bDAccountEditModel;
		}

		public JieNor.Megi.DataModel.BD.BDAccountViewModel GetBDAccountViewModel(MContext ctx, string pkID)
		{
			BDAccountRepository bDAccountRepository = new BDAccountRepository();
			return bDAccountRepository.GetBDAccountViewModel(ctx, pkID);
		}

		public BDAccountTypeEditModel GetBDAccountTypeEditModel(MContext ctx, string pkID)
		{
			BDAccountRepository bDAccountRepository = new BDAccountRepository();
			return bDAccountRepository.GetBDAccountTypeEditModel(ctx, pkID);
		}

		public OperationResult DeleteAccount(MContext ctx, ParamBase param)
		{
			BDAccountRepository bDAccountRepository = new BDAccountRepository();
			OperationResult operationResult = new OperationResult();
			if (string.IsNullOrEmpty(param.KeyIDs))
			{
				operationResult.Success = false;
				return operationResult;
			}
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			List<BDAccountModel> baseBDAccountList = bDAccountRepository.GetBaseBDAccountList(ctx, new SqlWhere(), false, null);
			if (param.KeyIDs.IndexOf(',') >= 0)
			{
				string[] array = param.KeyIDs.Split(',');
				if (array.Length != 0)
				{
					list2 = array.ToList();
				}
				operationResult = BDRepository.IsCanDelete(ctx, "Account", list2, out list);
			}
			else
			{
				BDAccountModel parentAccount = (from x in baseBDAccountList
				where x.MItemID == param.KeyIDs
				select x).FirstOrDefault();
				List<BDAccountModel> childrenAccountByRecursion = GetChildrenAccountByRecursion(parentAccount, baseBDAccountList, true);
				list2 = (from x in childrenAccountByRecursion
				select x.MItemID).ToList();
				operationResult = BDRepository.IsCanDelete(ctx, "Account", list2, out list);
			}
			if (list2.Count() == list.Count())
			{
				param.KeyIDs = string.Join(",", list);
				operationResult = bDAccountRepository.DeleteAccount(ctx, param);
				if (operationResult.Success)
				{
					List<BDAccountListModel> accountListIncludeCheckType = dal.GetAccountListIncludeCheckType(ctx, null);
					if (accountListIncludeCheckType == null || accountListIncludeCheckType.Count() == 0)
					{
						return operationResult;
					}
					List<string> currentMcodeList = BDAccountHelper.GetCurrentAccoutMCode();
					List<BDAccountListModel> list3 = (from x in accountListIncludeCheckType
					where currentMcodeList.Contains(x.MCode) && x.MCheckGroupID == "0"
					select x).ToList();
					List<CommandInfo> list4 = new List<CommandInfo>();
					foreach (BDAccountListModel item in list3)
					{
						List<BDAccountListModel> childrenAccountRecursion = BDAccountHelper.GetChildrenAccountRecursion(item, accountListIncludeCheckType);
						childrenAccountRecursion.Remove(item);
						if (childrenAccountRecursion == null || childrenAccountRecursion.Count() == 0)
						{
							GLCheckGroupModel gLCheckGroupModel = new GLCheckGroupModel();
							if (item.MCode == "2241")
							{
								gLCheckGroupModel.MContactID = CheckTypeStatusEnum.Optional;
								gLCheckGroupModel.MEmployeeID = CheckTypeStatusEnum.Optional;
							}
							else if (item.MCode == "1221")
							{
								gLCheckGroupModel.MContactID = CheckTypeStatusEnum.Optional;
							}
							else
							{
								gLCheckGroupModel.MContactID = CheckTypeStatusEnum.Required;
							}
							item.MCheckGroupModel = gLCheckGroupModel;
							string empty = string.Empty;
							list4.AddRange(GetCheckGroupInsertOrUpdateCmds(ctx, gLCheckGroupModel, out empty));
							BDAccountEditModel bDAccountEditModel = new BDAccountEditModel();
							bDAccountEditModel.MItemID = item.MItemID;
							bDAccountEditModel.MCheckGroupID = empty;
							List<string> fields = new List<string>
							{
								"MCheckGroupID"
							};
							list4.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDAccountEditModel>(ctx, bDAccountEditModel, fields, true));
						}
					}
					if (list4.Count() > 0)
					{
						BDRepository.ExecuteSqlTran(ctx, list4);
					}
				}
			}
			return operationResult;
		}

		public OperationResult DeleteAccount(MContext ctx, string pkID)
		{
			OperationResult operationResult = new OperationResult();
			operationResult = BDRepository.IsCanDelete(ctx, "BankAccount", pkID);
			OperationResult operationResult2 = new OperationResult();
			if (ctx.MRegProgress > 4)
			{
				operationResult2 = BDRepository.IsCanDelete(ctx, "Account", pkID);
			}
			else
			{
				operationResult2.Success = true;
			}
			if (operationResult.Success && operationResult2.Success)
			{
				BDAccountRepository bDAccountRepository = new BDAccountRepository();
				operationResult = bDAccountRepository.DeleteAccount(ctx, pkID);
			}
			if (operationResult.Success && !operationResult2.Success)
			{
				operationResult.Message = operationResult2.Message;
				operationResult.Success = false;
			}
			return operationResult;
		}

		public OperationResult ArchiveAccount(MContext ctx, ParamBase param)
		{
			OperationResult operationResult = new OperationResult();
			string keyIDs = param.KeyIDs;
			List<string> list = keyIDs.Split(',').ToList();
			List<BDAccountModel> list2 = new List<BDAccountModel>();
			BDAccountRepository bDAccountRepository = new BDAccountRepository();
			List<BDAccountModel> baseBDAccountList = bDAccountRepository.GetBaseBDAccountList(ctx, new SqlWhere(), false, null);
			List<BDAccountModel> list3 = new List<BDAccountModel>();
			List<string> list4 = new List<string>();
			foreach (string item in list)
			{
				BDAccountModel model = (from x in baseBDAccountList
				where x.MItemID == item
				select x).FirstOrDefault();
				if (model != null)
				{
					List<BDAccountModel> childrenAccountByRecursion = GetChildrenAccountByRecursion(model, baseBDAccountList, true);
					bool flag = false;
					List<string> list5 = new List<string>();
					list5.AddRange(BDAccountHelper.GetCurrentAccoutMCode());
					list5.AddRange(BDAccountHelper.GetBankAccountMCode());
					foreach (BDAccountModel item2 in childrenAccountByRecursion)
					{
						if (!item2.MIsSys && item2.MCode.IndexOf("1001") != 0 && item2.MCode.IndexOf("1002") != 0)
						{
							list2.Add(item2);
						}
						else
						{
							foreach (string item3 in list5)
							{
								if (!string.IsNullOrWhiteSpace(item2.MCode) && item2.MCode.IndexOf(item3) == 0)
								{
									flag = true;
									if (!list3.Exists((BDAccountModel x) => x.MItemID == model.MItemID))
									{
										list3.Add(model);
									}
									break;
								}
							}
							if (!flag)
							{
								list2.Add(item2);
							}
						}
					}
				}
			}
			List<BDAccountModel> source = list2;
			List<string> values = (from x in source
			select x.MItemID).ToList();
			List<string> list6 = new List<string>();
			BDRepository.IsCanDelete(ctx, "AccountEdit", string.Join(",", values), out list6);
			if (list6 == null || list6.Count() == 0)
			{
				string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "NoAcountCanDisable", "没有科目能够禁用，它们可能在其他地方有使用！");
				operationResult.Success = false;
				operationResult.Message = text;
				return operationResult;
			}
			keyIDs = (param.KeyIDs = string.Join(",", list6));
			operationResult = bDAccountRepository.ArchiveAccount(ctx, param);
			if (list3.Count() > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "UnAllowArchiveAccount", "下面这些会计科目没有禁用成功,它们(包括子级科目)可能是库存现金，银行存款，往来科目"));
				foreach (BDAccountModel item4 in list3)
				{
					stringBuilder.Append(item4.MNumber + "-" + item4.MName + "\r\n");
				}
				string text3 = stringBuilder.ToString();
				text3 = text3.Substring(0, text3.Length - 1);
				operationResult.Success = false;
				operationResult.Message = text3;
			}
			return operationResult;
		}

		public OperationResult UnArchiveAccount(MContext ctx, ParamBase param)
		{
			string keyIDs = param.KeyIDs;
			List<string> list = keyIDs.Split(',').ToList();
			List<BDAccountModel> list2 = new List<BDAccountModel>();
			BDAccountRepository bDAccountRepository = new BDAccountRepository();
			List<BDAccountModel> baseBDAccountList = bDAccountRepository.GetBaseBDAccountList(ctx, new SqlWhere(), true, null);
			foreach (string item in list)
			{
				BDAccountModel bDAccountModel = (from x in baseBDAccountList
				where x.MItemID == item
				select x).FirstOrDefault();
				list2.Add(bDAccountModel);
				list2.AddRange(GetParentAccountByRecursion(bDAccountModel, baseBDAccountList));
			}
			List<string> list3 = (from x in list2
			select x.MItemID).ToList();
			keyIDs = (param.KeyIDs = string.Join(",", list3.ToArray()));
			return bDAccountRepository.UnArchiveAccount(ctx, param);
		}

		public bool IsCodeExists(MContext ctx, string id, BDAccountTypeEditModel model)
		{
			BDAccountRepository bDAccountRepository = new BDAccountRepository();
			return bDAccountRepository.IsCodeExists(id, ctx, model);
		}

		public List<GLBalanceModel> GetBalance(MContext ctx, string itemid = "")
		{
			return BDAccountRepository.GetAccountBalance(ctx, itemid);
		}

		public List<GLInitBalanceModel> GetInitBalanceList(MContext ctx, SqlWhere filter = null)
		{
			return BDAccountRepository.GetInitBalanceList(ctx, filter);
		}

		public List<AccountInitBalanceTreeModel> GetInitBalanceTreeList(MContext ctx, string cyId, SqlWhere filter = null)
		{
			List<AccountItemTreeModel> accountItemTreeList = GetAccountItemTreeList(ctx, null);
			BASCurrencyViewModel @base = new REGCurrencyRepository().GetBase(ctx, false, null, null);
			filter?.Equal("MCheckGroupValueID", "0");
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

		private bool CheckAccountTreeCurrency(AccountItemTreeModel accountTree, string cyId)
		{
			bool flag = false;
			List<AccountItemTreeModel> children = accountTree.children;
			if ((children == null || children.Count == 0) && accountTree.MIsCheckForCurrency)
			{
				return true;
			}
			foreach (AccountItemTreeModel item in children)
			{
				flag = CheckAccountTreeCurrency(item, cyId);
				if (flag)
				{
					break;
				}
			}
			return flag;
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

		public OperationResult UpdateInitBalance(MContext ctx, List<GLInitBalanceModel> modelList, string contactId)
		{
			OperationResult operationResult = new OperationResult();
			List<BDAccountModel> bDAccountList = GetBDAccountList(ctx, null, true, false);
			if (ctx.MRegProgress <= 12)
			{
				BASOrganisationRepository bASOrganisationRepository = new BASOrganisationRepository();
				ctx.IsSys = true;
				BASOrganisationModel dataModel = bASOrganisationRepository.GetDataModel(ctx, ctx.MOrgID, false);
				ctx.IsSys = false;
				if (dataModel != null)
				{
					foreach (GLInitBalanceModel model in modelList)
					{
						List<GLInitBalanceModel> dataModelList = ModelInfoManager.GetDataModelList<GLInitBalanceModel>(ctx, new SqlWhere().Equal("MOrgID", ctx.MOrgID).Equal("MAccountID", model.MAccountID), false, false);
						if (dataModelList != null && dataModelList.Count > 0)
						{
							model.MItemID = dataModelList[0].MItemID;
						}
						model.MOrgID = ctx.MOrgID;
						model.MContactID = "0";
					}
				}
				return ModelInfoManager.InsertOrUpdate(ctx, modelList, null);
			}
			if (ctx.MInitBalanceOver)
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "InitBalanceHadFinish", "Initialize the balance is over!");
				return operationResult;
			}
			foreach (GLInitBalanceModel model2 in modelList)
			{
				List<CommandInfo> updateInitBalanceCmds = BDAccountRepository.GetUpdateInitBalanceCmds(ctx, model2, bDAccountList);
				int num = BDRepository.ExecuteSqlTran(ctx, updateInitBalanceCmds);
				operationResult.Success = (num > 0);
			}
			return operationResult;
		}

		public DataGridJson<GLInitBalanceModel> GetInitBalanceListByPage(MContext ctx, GLBalanceListFilterModel param)
		{
			return BDAccountRepository.GetBanlInitBalanceListByPage(ctx, param);
		}

		public TrialInitBalanceModel TrialInitBalance(MContext ctx)
		{
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.Equal("b.MParentID", "0");
			sqlWhere.Equal("a.MCheckGroupValueID", "0");
			List<GLInitBalanceModel> initBalanceList = BDAccountRepository.GetInitBalanceList(ctx, sqlWhere);
			decimal num = default(decimal);
			decimal num2 = default(decimal);
			decimal num3 = default(decimal);
			decimal num4 = default(decimal);
			foreach (GLInitBalanceModel item in initBalanceList)
			{
				if (item.MDC > 0)
				{
					num += item.MInitBalance;
				}
				else
				{
					num2 += item.MInitBalance;
				}
				num4 += item.MYtdCredit;
				num3 += item.MYtdDebit;
			}
			TrialInitBalanceModel trialInitBalanceModel = new TrialInitBalanceModel();
			trialInitBalanceModel.Success = (decimal.Round(num, 2) == decimal.Round(num2, 2) && decimal.Round(num4, 2) == decimal.Round(num3, 2));
			trialInitBalanceModel.CreditAmount = num2;
			trialInitBalanceModel.DebitAmount = num;
			trialInitBalanceModel.YtdCredit = num4;
			trialInitBalanceModel.YtdDebit = num3;
			return trialInitBalanceModel;
		}

		public OperationResult AddAccountingProject(MContext ctx, GLInitBalanceModel model)
		{
			OperationResult operationResult = new OperationResult();
			if (ctx.MInitBalanceOver)
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "InitBalanceHadFinish", "Initialize the balance is over!If you want to re-enter, please click on the re-initialization!");
				return operationResult;
			}
			GLInitBalanceRepository gLInitBalanceRepository = new GLInitBalanceRepository();
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.Equal("MAccountID", model.MAccountID);
			sqlWhere.Equal("MContactID", model.MContactID);
			if (gLInitBalanceRepository.ExistsByFilter(ctx, sqlWhere))
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "ContactIsExist", "This account already contains this contact!");
			}
			else
			{
				sqlWhere = new SqlWhere();
				sqlWhere.Equal("MAccountID", model.MAccountID);
				sqlWhere.Equal("MContactID", "0");
				List<GLInitBalanceModel> modelList = gLInitBalanceRepository.GetModelList(ctx, sqlWhere, true);
				if (modelList != null && modelList.Count() > 0)
				{
					List<CommandInfo> list = new List<CommandInfo>();
					foreach (GLInitBalanceModel item in modelList)
					{
						GLInitBalanceModel gLInitBalanceModel = new GLInitBalanceModel();
						gLInitBalanceModel.MAccountID = model.MAccountID;
						gLInitBalanceModel.MCurrencyID = item.MCurrencyID;
						gLInitBalanceModel.MContactID = model.MContactID;
						gLInitBalanceModel.MOrgID = ctx.MOrgID;
						SqlWhere sqlWhere2 = new SqlWhere();
						sqlWhere2.Equal("MAccountID", model.MAccountID);
						sqlWhere2.Equal("MCurrencyID", item.MCurrencyID);
						sqlWhere2.NotEqual("MContactID", "0");
						if (!gLInitBalanceRepository.ExistsByFilter(ctx, sqlWhere2))
						{
							gLInitBalanceModel.MInitBalance = item.MInitBalance;
							gLInitBalanceModel.MInitBalanceFor = item.MInitBalanceFor;
							gLInitBalanceModel.MYtdCredit = item.MYtdCredit;
							gLInitBalanceModel.MYtdDebit = item.MYtdDebit;
						}
						list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<GLBalanceModel>(ctx, gLInitBalanceModel, null, true));
					}
					int num = gLInitBalanceRepository.UpdateInitBalanceByTran(ctx, list);
					operationResult.Success = (num > 0);
				}
				else
				{
					model.MCurrencyID = ctx.MBasCurrencyID;
					model.MOrgID = ctx.MOrgID;
					operationResult = gLInitBalanceRepository.InsertOrUpdate(ctx, model, null);
				}
			}
			return operationResult;
		}

		public OperationResult InitBalanceFinish(MContext ctx)
		{
			OperationResult operationResult = new OperationResult();
			try
			{
				BASOrganisationRepository bASOrganisationRepository = new BASOrganisationRepository();
				ctx.IsSys = true;
				BASOrganisationModel dataModel = bASOrganisationRepository.GetDataModel(ctx, ctx.MOrgID, false);
				ctx.IsSys = false;
				if (ctx.MInitBalanceOver || dataModel.MInitBalanceOver)
				{
					operationResult.Success = false;
					operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "InitBalanceHadFinish", "Initialize the balance is over!If you want to re-enter, please click on the re-initialization!");
					return operationResult;
				}
				if (ctx.MAccountTableID == "3")
				{
					operationResult = CheckCustomAccountIsFinish(ctx);
					if (!operationResult.Success)
					{
						operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "CanFindAnyCustomAccount", "没有查找到任何自定义科目");
						return operationResult;
					}
					operationResult = CheckCustomAccountIsMatch(ctx);
					if (!operationResult.Success)
					{
						return operationResult;
					}
				}
				operationResult = CheckBalanceEqualWithBill(ctx);
				if (!operationResult.Success)
				{
					return operationResult;
				}
				TrialInitBalanceModel trialInitBalanceModel = TrialInitBalance(ctx);
				if (!trialInitBalanceModel.Success)
				{
					operationResult.Success = false;
					operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "TrialBalanceFail", "Sorry, you input the initial balance is not balanced, please input again!");
					return operationResult;
				}
				GLBalanceRepository gLBalanceRepository = new GLBalanceRepository();
				GLInitBalanceRepository gLInitBalanceRepository = new GLInitBalanceRepository();
				List<GLInitBalanceModel> initBalanceList = BDAccountRepository.GetInitBalanceList(ctx, new SqlWhere());
				List<CommandInfo> list = new List<CommandInfo>();
				list.Add(gLBalanceRepository.GetDeleteAllBalanceCmd(ctx));
				if (initBalanceList != null)
				{
					DateTime mGLBeginDate = ctx.MGLBeginDate;
					int year = mGLBeginDate.Year;
					mGLBeginDate = ctx.MGLBeginDate;
					int month = mGLBeginDate.Month;
					foreach (GLInitBalanceModel item in initBalanceList)
					{
						GLBalanceModel modelData = InitBalanceModelToBalanceModel(item, year, month);
						list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<GLBalanceModel>(ctx, modelData, null, true));
					}
				}
				BASOrganisationModel orgModel = bASOrganisationRepository.GetOrgModel(ctx);
				orgModel.MInitBalanceOver = true;
				List<string> list2 = new List<string>();
				list2.Add("MInitBalanceOver");
				list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BASOrganisationModel>(ctx, orgModel, list2, true));
				ctx.IsSys = true;
				operationResult = bASOrganisationRepository.InsertOrUpdate(ctx, orgModel, "MInitBalanceOver");
				if (operationResult.Success)
				{
					ctx.IsSys = false;
					int num = BDRepository.ExecuteSqlTran(ctx, list);
					operationResult.Success = (num > 0);
					if (operationResult.Success)
					{
						ctx.MInitBalanceOver = true;
						ContextHelper.MContext = ctx;
						ContextHelper.UpdateMContextByKeyField("MOrgID", ctx.MOrgID, "MInitBalanceOver", true, true);
					}
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				ctx.IsSys = false;
			}
			return operationResult;
		}

		private GLBalanceModel InitBalanceModelToBalanceModel(GLInitBalanceModel initBalanceModel, int year, int period)
		{
			GLBalanceModel gLBalanceModel = new GLBalanceModel();
			gLBalanceModel.MYear = year;
			gLBalanceModel.MPeriod = period;
			gLBalanceModel.MOrgID = initBalanceModel.MOrgID;
			gLBalanceModel.MContactID = initBalanceModel.MContactID;
			gLBalanceModel.MAccountID = initBalanceModel.MAccountID;
			gLBalanceModel.MCurrencyID = initBalanceModel.MCurrencyID;
			gLBalanceModel.MCheckGroupValueID = initBalanceModel.MCheckGroupValueID;
			gLBalanceModel.MBeginBalance = initBalanceModel.MInitBalance;
			gLBalanceModel.MBeginBalanceFor = initBalanceModel.MInitBalanceFor;
			gLBalanceModel.MYtdCredit = initBalanceModel.MYtdCredit;
			gLBalanceModel.MYtdCreditFor = initBalanceModel.MYtdCreditFor;
			gLBalanceModel.MYtdDebit = initBalanceModel.MYtdDebit;
			gLBalanceModel.MYtdDebitFor = initBalanceModel.MYtdDebitFor;
			gLBalanceModel.MEndBalance = initBalanceModel.MInitBalance;
			gLBalanceModel.MEndBalanceFor = initBalanceModel.MInitBalanceFor;
			gLBalanceModel.MYearPeriod = year * 100 + period;
			return gLBalanceModel;
		}

		public OperationResult ReInitBalance(MContext ctx)
		{
			OperationResult operationResult = new OperationResult();
			GLSettlementRepository gLSettlementRepository = new GLSettlementRepository();
			List<GLSettlementModel> modelList = gLSettlementRepository.GetModelList(ctx, new SqlWhere().Equal("MStatus", 1), false);
			if (modelList != null && modelList.Count() > 0)
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "CanNotReInitHadSettlementPeriod", "无法重新初始化，因为存在已结账期间,请先反结账这些期间!");
				return operationResult;
			}
			List<CommandInfo> list = new List<CommandInfo>();
			BASOrganisationRepository bASOrganisationRepository = new BASOrganisationRepository();
			BASOrganisationModel orgModel = bASOrganisationRepository.GetOrgModel(ctx);
			orgModel.MInitBalanceOver = false;
			List<string> list2 = new List<string>();
			list2.Add("MInitBalanceOver");
			GLVoucherRepository gLVoucherRepository = new GLVoucherRepository();
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.Equal("MStatus", "1");
			List<GLVoucherModel> modelList2 = gLVoucherRepository.GetModelList(ctx, sqlWhere, false);
			if (modelList2 != null && modelList2.Count() > 0)
			{
				list.Add(gLVoucherRepository.ApproveVoucherByStatus(ctx, "0", "1"));
				list.Add(gLSettlementRepository.UpdateSettlementByStatus(ctx, "0", "1"));
			}
			try
			{
				ctx.IsSys = true;
				operationResult = bASOrganisationRepository.InsertOrUpdate(ctx, orgModel, "MInitBalanceOver");
				if (operationResult.Success)
				{
					ctx.IsSys = false;
					GLBalanceRepository gLBalanceRepository = new GLBalanceRepository();
					list.AddRange(gLBalanceRepository.GetDeleteBalanceCmds(ctx));
					list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BASOrganisationModel>(ctx, orgModel, null, true));
					int num = gLBalanceRepository.ExecuteSqlTran(ctx, list);
					operationResult.Success = (num > 0);
					if (operationResult.Success)
					{
						ctx.MInitBalanceOver = false;
						ContextHelper.MContext = ctx;
						ContextHelper.UpdateMContextByKeyField("MOrgID", ctx.MOrgID, "MInitBalanceOver", false, true);
					}
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				ctx.IsSys = false;
			}
			return operationResult;
		}

		private bool IsBankOrCashAccount(MContext ctx, string accountId, out BDBankAccountEditModel bankModel)
		{
			bool result = false;
			bankModel = null;
			if (!string.IsNullOrEmpty(accountId))
			{
				BDBankAccountRepository bDBankAccountRepository = new BDBankAccountRepository();
				SqlWhere sqlWhere = new SqlWhere();
				sqlWhere.Equal("MAccountID", accountId);
				bankModel = bDBankAccountRepository.GetDataModelByFilter(ctx, sqlWhere);
				if (bankModel != null)
				{
					result = true;
				}
			}
			return result;
		}

		public static List<MultiLanguageFieldList> GetBankAccountMulitLang(List<MultiLanguageFieldList> accountMulitLang, List<MultiLanguageFieldList> bankAccountMulitLang)
		{
			if (bankAccountMulitLang == null)
			{
				return accountMulitLang;
			}
			MultiLanguageFieldList multiLanguageFieldList = accountMulitLang.First();
			if (multiLanguageFieldList.MMultiLanguageField != null && multiLanguageFieldList.MMultiLanguageField.Count() > 0)
			{
				MultiLanguageFieldList multiLanguageFieldList2 = bankAccountMulitLang.FirstOrDefault();
				if (multiLanguageFieldList2 != null)
				{
					foreach (MultiLanguageField item in multiLanguageFieldList2.MMultiLanguageField)
					{
						item.MValue = (from x in multiLanguageFieldList.MMultiLanguageField
						where x.MLocaleID == item.MLocaleID
						select x).First().MValue;
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
						MultiLanguageField multiLanguageField = new MultiLanguageField();
						multiLanguageFieldList3.CreateMultiLanguageFieldValue(item2.MLocaleID, item2.MValue, null);
					}
					list.Add(multiLanguageFieldList3);
					accountMulitLang = list;
				}
			}
			return bankAccountMulitLang;
		}

		public List<BDAccountModel> GetChildrenAccountByRecursion(BDAccountModel parentAccount, List<BDAccountModel> accoutList, bool isSelf)
		{
			List<BDAccountModel> list = new List<BDAccountModel>();
			if (isSelf)
			{
				list.Add(parentAccount);
			}
			List<BDAccountModel> list2 = (from x in accoutList
			where x.MParentID == parentAccount.MItemID
			select x).ToList();
			if (list2 == null || list2.Count() == 0)
			{
				return list;
			}
			foreach (BDAccountModel item in list2)
			{
				list.AddRange(GetChildrenAccountByRecursion(item, accoutList, true));
			}
			return list;
		}

		public List<BDAccountModel> GetParentAccountByRecursion(BDAccountModel childAccount, List<BDAccountModel> accoutList)
		{
			List<BDAccountModel> list = new List<BDAccountModel>();
			if (childAccount.MParentID == "0")
			{
				return list;
			}
			BDAccountModel bDAccountModel = (from x in accoutList
			where x.MItemID == childAccount.MParentID
			select x).FirstOrDefault();
			if (bDAccountModel == null)
			{
				throw new Exception("can find parent account");
			}
			list.Add(bDAccountModel);
			if (bDAccountModel.MParentID != "0")
			{
				list.AddRange(GetParentAccountByRecursion(bDAccountModel, accoutList));
			}
			return list;
		}

		public List<BDAccountListModel> GetParentAccountByRecursion(BDAccountListModel childAccount, List<BDAccountListModel> accoutList)
		{
			List<BDAccountListModel> list = new List<BDAccountListModel>();
			if (childAccount.MParentID == "0")
			{
				return list;
			}
			BDAccountListModel bDAccountListModel = (from x in accoutList
			where x.MItemID == childAccount.MParentID
			select x).FirstOrDefault();
			if (bDAccountListModel == null)
			{
				return list;
			}
			list.Add(bDAccountListModel);
			if (bDAccountListModel.MParentID != "0")
			{
				list.AddRange(GetParentAccountByRecursion(bDAccountListModel, accoutList));
			}
			return list;
		}

		public bool CheckAccountExist(MContext ctx, List<string> codeList, OperationResult result)
		{
			if (!ctx.MEnabledModules.Contains(ModuleEnum.GL) || codeList == null || !codeList.Any((string c) => !string.IsNullOrWhiteSpace(c)))
			{
				return true;
			}
			List<BDAccountModel> bDAccountList = GetBDAccountList(ctx, new BDAccountListFilterModel
			{
				IncludeDisable = true
			}, false, false);
			return CheckAccountCodeExist(ctx, codeList, result, bDAccountList);
		}

		public bool CheckAccountNumberExist(MContext ctx, List<string> numberOrIdList, List<IOValidationResultModel> validationResult)
		{
			if (numberOrIdList == null)
			{
				return false;
			}
			numberOrIdList = (from v in numberOrIdList
			where !string.IsNullOrWhiteSpace(v)
			select v).ToList();
			List<BDAccountModel> bDAccountList = GetBDAccountList(ctx, new BDAccountListFilterModel
			{
				IncludeDisable = true
			}, false, false);
			foreach (string numberOrId in numberOrIdList)
			{
				string text = string.Empty;
				string fieldValue = numberOrId;
				BDAccountModel bDAccountModel = bDAccountList.FirstOrDefault((BDAccountModel f) => f.MItemID == numberOrId || f.MNumber == numberOrId);
				if (bDAccountModel == null)
				{
					text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "AccountNotFound", "The account:{0} can't be found!");
				}
				else if (!bDAccountModel.MIsActive)
				{
					fieldValue = bDAccountModel.MNumber;
					text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "AccountHasDisabled", "科目：{0}已禁用！");
				}
				if (!string.IsNullOrWhiteSpace(text))
				{
					validationResult.Add(new IOValidationResultModel
					{
						FieldType = IOValidationTypeEnum.Account,
						FieldValue = fieldValue,
						Message = text
					});
				}
			}
			return !validationResult.Any();
		}

		private bool CheckAccountCodeExist(MContext ctx, List<string> codeList, OperationResult result, List<BDAccountModel> acctList)
		{
			codeList = codeList.Distinct().ToList();
			foreach (string code in codeList)
			{
				if (!string.IsNullOrWhiteSpace(code))
				{
					string text = string.Empty;
					BDAccountModel bDAccountModel = acctList.FirstOrDefault((BDAccountModel f) => f.MCode == code);
					if (bDAccountModel == null)
					{
						text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "AccountNotFound", "The account:{0} can't be found!");
					}
					else if (!bDAccountModel.MIsActive)
					{
						text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "AccountHasDisabled", "科目：{0}已禁用！");
					}
					if (!string.IsNullOrWhiteSpace(text))
					{
						string text2 = code.ToStandardAcctNumber(true, 2);
						result.VerificationInfor.Add(new BizVerificationInfor
						{
							Level = AlertEnum.Error,
							Message = string.Format(text, text2),
							Id = text2,
							CheckItem = "Account"
						});
					}
				}
			}
			return result.Success;
		}

		public void CheckImportAccountExist<T1, T2>(MContext ctx, T1 model, List<T2> acctList, string fieldName, List<IOValidationResultModel> validationModel, string targetFieldName = "MCode", bool validateLeafCode = false)
		{
			if (ctx.MEnabledModules.Contains(ModuleEnum.GL))
			{
				string fieldValue = ModelHelper.GetModelValue(model, fieldName);
				if (!string.IsNullOrWhiteSpace(fieldValue))
				{
					string text = string.Empty;
					fieldValue = fieldValue.Trim();
					T2 val = acctList.FirstOrDefault((T2 f) => ModelHelper.GetModelValue<T2>(f, "MCode") == fieldValue || (!string.IsNullOrWhiteSpace(ModelHelper.GetModelValue<T2>(f, "MNumber")) && (ModelHelper.GetModelValue<T2>(f, "MNumber").Trim() == fieldValue || fieldValue.StartsWith(ModelHelper.GetModelValue<T2>(f, "MNumber").Trim() + " "))));
					if (val != null)
					{
						if (!Convert.ToBoolean(ModelHelper.GetModelValueO(val, "MIsActive", false)))
						{
							text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "AccountHasDisabled", "科目：{0}已禁用！");
						}
						else
						{
							ModelHelper.SetModelValue(model, fieldName, ModelHelper.GetModelValue(val, targetFieldName), null);
						}
					}
					else
					{
						text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "AccountNotFound", "The account:{0} can't be found!");
						if (validateLeafCode)
						{
							string pureNumber = fieldValue.Split(' ')[0];
							if (acctList.Any((T2 f) => !string.IsNullOrWhiteSpace(ModelHelper.GetModelValue<T2>(f, "MNumber")) && ModelHelper.GetModelValue<T2>(f, "MNumber").StartsWith(pureNumber)))
							{
								fieldValue = pureNumber;
								text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "AccountNotLeaf", "科目{0}不是末级科目!");
							}
						}
					}
					if (!string.IsNullOrWhiteSpace(text))
					{
						int rowIndex = 0;
						int.TryParse(ModelHelper.TryGetModelValue(model, "MRowIndex"), out rowIndex);
						validationModel.Add(new IOValidationResultModel
						{
							FieldType = IOValidationTypeEnum.Account,
							FieldValue = fieldValue,
							RowIndex = rowIndex,
							Message = text
						});
					}
				}
			}
		}

		public void CheckImportAccountExist<T>(MContext ctx, List<T> modelList, string fieldName, List<IOValidationResultModel> validationModel, string targetFieldName = "MCode", List<BDAccountModel> accountList = null)
		{
			accountList = ((accountList == null) ? GetBDAccountList(ctx, null, true, false) : accountList);
			foreach (T model in modelList)
			{
				CheckImportAccountExist(ctx, model, accountList, fieldName, validationModel, targetFieldName, false);
			}
		}

		private List<string> GetDocVoucherDefaultCode()
		{
			List<string> list = new List<string>();
			AccountCodeEnum accountCodeEnum = new AccountCodeEnum();
			Type type = accountCodeEnum.GetType();
			FieldInfo[] fields = type.GetFields();
			foreach (FieldInfo fieldInfo in fields)
			{
				string item = fieldInfo.GetValue(accountCodeEnum).ToString();
				list.Add(item);
			}
			return list;
		}

		public List<IOImportHelperSheetModel> GetImportHelperSheetList(MContext ctx)
		{
			List<IOImportHelperSheetModel> list = new List<IOImportHelperSheetModel>();
			List<BDTrackModel> trackNameList = _track.GetTrackNameList(ctx, false);
			List<string> list2 = new List<string>();
			foreach (BDTrackModel item2 in trackNameList)
			{
				list2.Add(item2.MName);
			}
			List<string> list3 = new List<string>
			{
				COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "Contact", "Contact"),
				COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "Employee", "Employee"),
				COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "InventoryItems", "Inventory Items"),
				COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "ExpenseItem", "Expense Items")
			};
			if (ctx.MOrgVersionID == 0)
			{
				list3.Add(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "SalaryComponent", "Salary Component"));
			}
			IOImportHelperSheetModel item = new IOImportHelperSheetModel
			{
				MSheetName = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "HelperAccountingData", "辅助核算数据"),
				MColumnDataList = new List<IOImportHelperColumnModel>
				{
					new IOImportHelperColumnModel
					{
						MComment = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "HelperAccountingDataComment", "辅助核算备查数据,不会被导入"),
						MTitle = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "Name", "名称"),
						MDataList = list3.Union(list2).ToList()
					}
				}
			};
			list.Add(item);
			return list;
		}

		public List<IOTemplateConfigModel> GetTemplateConfig(MContext ctx, IOImportAccountFilterModel filter)
		{
			List<IOTemplateConfigModel> list = new List<IOTemplateConfigModel>();
			string[] megiLangTypes = ServerHelper.MegiLangTypes;
			if (ctx.MAccountTableID == "3" && filter.IsCover)
			{
				list.Add(new IOTemplateConfigModel("MParentID", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Acct, "ParentAccount", "Parent Account", null)
				}), false, false, 2, null));
				list.Add(new IOTemplateConfigModel("MAccountTypeId", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Acct, "AccountType", "Account Type", null)
				}), true, false, 2, null));
			}
			list.AddRange(new List<IOTemplateConfigModel>
			{
				new IOTemplateConfigModel("MNumber", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Common, "Code", "Code", null)
				}), filter.IsCover ? string.Empty : COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "CodeComment", "按科目编码规则编码，用“.“代表科目分级"), true),
				new IOTemplateConfigModel("MName", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Common, "Name", "Name", null)
				}), filter.IsCover ? string.Empty : COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "NameComment", "文本，不支持特殊符号！@#‘%等，只录本级科目名称即可"), true),
				new IOTemplateConfigModel("MDC", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Acct, "Direction", "Direction", null)
				}), filter.IsCover ? string.Empty : COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "DirectionComment", "借或贷"), true),
				new IOTemplateConfigModel("MCheckGroupNames", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Common, "AccountingDimension", "核算维度", null)
				}), filter.IsCover ? string.Empty : COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "AccountDimensionComment", "辅助类别名称，多辅助核算以“/”隔开。辅助核算维度可查看“辅助核算数据”表"), false),
				new IOTemplateConfigModel("MIsCheckForCurrency", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Acct, "CurrencyAccountingWay", "Currency accounting way", null)
				}), filter.IsCover ? string.Empty : COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "CurrentAccountingComment", "外币核算的科目录入“是”，非外币核算科目，此项可为空"), false)
			});
			if (filter.Type == 1)
			{
				COMLangInfoModel cOMLangInfoModel = new COMLangInfoModel(LangModule.Acct, "OriginalCurrency", "Original Currency", ")");
				COMLangInfoModel cOMLangInfoModel2 = new COMLangInfoModel(LangModule.Acct, "StandardCurrency", "Standard currency", ")");
				string comment = filter.IsCover ? string.Empty : COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "ManualInput", "手动录入");
				list.AddRange(new List<IOTemplateConfigModel>
				{
					new IOTemplateConfigModel("MCurrencyID", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
					{
						new COMLangInfoModel(LangModule.Report, "Currency", "Currency", null)
					}), COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "CurrentComment", filter.IsCover ? string.Empty : "系统中已增加的币别"), false),
					new IOTemplateConfigModel("MInitBalanceFor", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[2]
					{
						new COMLangInfoModel(LangModule.Acct, "InitialBalance", "Initial balance", "("),
						cOMLangInfoModel
					}), comment, false),
					new IOTemplateConfigModel("MInitBalance", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[2]
					{
						new COMLangInfoModel(LangModule.Acct, "InitialBalance", "Initial balance", "("),
						cOMLangInfoModel2
					}), comment, false),
					new IOTemplateConfigModel("MYtdDebitFor", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[2]
					{
						new COMLangInfoModel(LangModule.Acct, "CumulativeDebitThisYear", "Cumulative debit this year", "("),
						cOMLangInfoModel
					}), comment, false),
					new IOTemplateConfigModel("MYtdDebit", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[2]
					{
						new COMLangInfoModel(LangModule.Acct, "CumulativeDebitThisYear", "Cumulative debit this year", "("),
						cOMLangInfoModel2
					}), comment, false),
					new IOTemplateConfigModel("MYtdCreditFor", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[2]
					{
						new COMLangInfoModel(LangModule.Acct, "CumulativeCreditThisYear", "Cumulative credit this year", "("),
						cOMLangInfoModel
					}), comment, false),
					new IOTemplateConfigModel("MYtdCredit", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[2]
					{
						new COMLangInfoModel(LangModule.Acct, "CumulativeCreditThisYear", "Cumulative credit this year", "("),
						cOMLangInfoModel2
					}), comment, false)
				});
			}
			return list;
		}

		public List<ImportTemplateDataSource> GetTemplateBasicData(MContext ctx, Dictionary<string, string> columnList, IOImportAccountFilterModel filter, Dictionary<string, string[]> exampleDataList = null)
		{
			List<ImportTemplateDataSource> list = new List<ImportTemplateDataSource>();
			BDContactsInfoModel emptyDataEditModel = ModelInfoManager.GetEmptyDataEditModel<BDContactsInfoModel>(ctx);
			List<string> fieldList = (from f in emptyDataEditModel.MultiLanguage
			where columnList.Keys.Contains(f.MFieldName)
			select f.MFieldName).ToList();
			List<BASLangModel> orgLangList = _lang.GetOrgLangList(ctx);
			List<ImportDataSourceInfo> dicLangList = new List<ImportDataSourceInfo>();
			orgLangList.ForEach(delegate(BASLangModel f)
			{
				dicLangList.Add(new ImportDataSourceInfo
				{
					Key = f.LangID,
					Value = f.LangName
				});
			});
			ImportDataSourceInfo item2 = dicLangList.FirstOrDefault((ImportDataSourceInfo f) => f.Key == ctx.MLCID);
			dicLangList.Remove(item2);
			dicLangList.Insert(0, item2);
			list.Add(new ImportTemplateDataSource(false)
			{
				FieldType = ImportTemplateColumnType.MultiLanguage,
				FieldList = fieldList,
				DataSourceList = dicLangList
			});
			if (ctx.MAccountTableID == "3" && filter.IsCover)
			{
				List<BDAccountListModel> bDAccountList = dal.GetBDAccountList(ctx, new SqlWhere().NotLike("a.MNumber", "1001").NotLike("a.MNumber", "1002"), null, true, false);
				List<ImportDataSourceInfo> list2 = new List<ImportDataSourceInfo>();
				foreach (BDAccountListModel item3 in bDAccountList)
				{
					list2.Add(new ImportDataSourceInfo
					{
						Key = item3.MItemID,
						Value = $"{item3.MNumber}-{HttpUtility.HtmlDecode(item3.MName)}"
					});
				}
				list.Add(new ImportTemplateDataSource(true)
				{
					FieldType = ImportTemplateColumnType.Account,
					FieldList = new List<string>
					{
						"MParentID"
					},
					DataSourceList = list2
				});
				List<BDAccountTypeListModel> bDAccountTypeList = dal.GetBDAccountTypeList(ctx, "MACCOUNTTABLEID='" + ctx.MAccountTableID + "'");
				List<ImportDataSourceInfo> list3 = new List<ImportDataSourceInfo>();
				bDAccountTypeList = (from x in bDAccountTypeList
				orderby x.MAccountGroupID
				select x).ToList();
				foreach (BDAccountTypeListModel item4 in bDAccountTypeList)
				{
					string value = string.IsNullOrWhiteSpace(item4.MAcctGroupName) ? item4.MName : $"{item4.MName}[{item4.MAcctGroupName}]";
					list3.Add(new ImportDataSourceInfo
					{
						Key = item4.MItemID,
						Value = value
					});
				}
				list.Add(new ImportTemplateDataSource(true)
				{
					FieldType = ImportTemplateColumnType.AccountType,
					FieldList = new List<string>
					{
						"MAccountTypeId"
					},
					DataSourceList = list3
				});
			}
			List<ImportDataSourceInfo> list4 = new List<ImportDataSourceInfo>();
			list4.Add(new ImportDataSourceInfo
			{
				Key = "1",
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangKey.Yes)
			});
			list.Add(new ImportTemplateDataSource(true)
			{
				FieldType = ImportTemplateColumnType.CurrencyAccountingWay,
				FieldList = new List<string>
				{
					"MIsCheckForCurrency"
				},
				DataSourceList = list4
			});
			List<ImportDataSourceInfo> list5 = new List<ImportDataSourceInfo>();
			list5.Add(new ImportDataSourceInfo
			{
				Key = "1",
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "Borrow", "Borrow")
			});
			list5.Add(new ImportDataSourceInfo
			{
				Key = "-1",
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "Loan", "Loan")
			});
			list.Add(new ImportTemplateDataSource(true)
			{
				FieldType = ImportTemplateColumnType.AccountDirection,
				FieldList = new List<string>
				{
					"MDC"
				},
				DataSourceList = list5
			});
			if (filter.Type == 1)
			{
				BASCurrencyViewModel @base = _bdCurrency.GetBase(ctx, false, null, null);
				List<REGCurrencyViewModel> viewList = _bdCurrency.GetViewList(ctx, null, null, false, null);
				List<ImportDataSourceInfo> list6 = new List<ImportDataSourceInfo>();
				if (@base != null)
				{
					list6.Add(new ImportDataSourceInfo
					{
						Key = @base.MCurrencyID,
						Value = $"{@base.MCurrencyID} {@base.MLocalName}",
						Info = "1"
					});
				}
				foreach (REGCurrencyViewModel item5 in viewList)
				{
					if (!list6.Any((ImportDataSourceInfo f) => f.Key == item5.MCurrencyID))
					{
						list6.Add(new ImportDataSourceInfo
						{
							Key = item5.MCurrencyID,
							Value = $"{item5.MCurrencyID} {item5.MName}"
						});
					}
				}
				list.Add(new ImportTemplateDataSource(true)
				{
					FieldType = ImportTemplateColumnType.Currency,
					FieldList = new List<string>
					{
						"MCurrencyID"
					},
					DataSourceList = list6
				});
			}
			return list;
		}

		public ImportTemplateModel GetImportTemplateModel(MContext ctx, IOImportAccountFilterModel filter)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			List<IOTemplateConfigModel> templateConfig = GetTemplateConfig(ctx, filter);
			foreach (IOTemplateConfigModel item in templateConfig)
			{
				dictionary.Add(item.MFieldName, item.MLangList.FirstOrDefault((KeyValuePair<string, string> f) => f.Key == ctx.MLCID).Value);
			}
			Dictionary<string, string[]> dictionary2 = new Dictionary<string, string[]>();
			List<ImportTemplateDataSource> templateBasicData = GetTemplateBasicData(ctx, dictionary, filter, dictionary2);
			dictionary2.Add("MNumber", new string[1]
			{
				"1601.01"
			});
			dictionary2.Add("MName", new string[1]
			{
				COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "FixedAssetsOfficeEquipment", "固定资产 - 办公设备")
			});
			Dictionary<string, int> dictionary3 = new Dictionary<string, int>();
			dictionary3.Add("MParentID", 18);
			dictionary3.Add("MAccountTypeId", 10);
			dictionary3.Add("MNumber", 8);
			dictionary3.Add("MName", 8);
			dictionary3.Add("MCurrencyID", 6);
			dictionary3.Add("MIsCheckForCurrency", 5);
			dictionary3.Add("MDC", 3);
			dictionary3.Add("MCheckGroupNames", 10);
			return new ImportTemplateModel
			{
				TemplateType = (filter.IsCover ? "AccountCover" : "Account"),
				HelperSheetList = GetImportHelperSheetList(ctx),
				LocaleID = ctx.MLCID,
				ColumnList = dictionary,
				FieldConfigList = templateConfig,
				RequiredColumnList = (from f in templateConfig
				where f.MIsRequired
				select f.MFieldName).ToList(),
				TemplateDictionaryList = templateBasicData,
				ExampleDataList = dictionary2,
				ColumnWidthList = dictionary3
			};
		}

		public BDAccountListModel GetAccountByCode(MContext ctx, string code, bool includeBalance = true)
		{
			List<BDAccountListModel> accountListIncludeBalance = GetAccountListIncludeBalance(ctx, new SqlWhere().AddFilter("MCode", SqlOperators.Equal, code).AddFilter("MOrgID", SqlOperators.Equal, ctx.MOrgID), true);
			return accountListIncludeBalance.FirstOrDefault();
		}

		public BDAccountListModel GetLeafAccountByCode(List<BDAccountListModel> accounts, string code)
		{
			return dal.GetLeafAccountByCode(accounts, code);
		}

		public List<T> GetLeafAccountListByCode<T>(List<T> accounts, string code)
		{
			List<T> result = new List<T>();
			dal.GetLeafAccountListByCode(accounts, code, result);
			return result;
		}

		public OperationResult AddAccountCheck(MContext ctx, BDAccountEditModel model)
		{
			bool flag = true;
			BDAccountRepository bDAccountRepository = new BDAccountRepository();
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.Equal("MParentID", model.MParentID);
			OperationResult operationResult = new OperationResult();
			if (bDAccountRepository.ExistsByFilter(ctx, sqlWhere))
			{
				operationResult.Success = false;
				return operationResult;
			}
			SqlWhere sqlWhere2 = new SqlWhere();
			sqlWhere2.Equal("MOrgID", ctx.MOrgID);
			sqlWhere2.Equal("MAccountID", model.MParentID);
			GLInitBalanceRepository gLInitBalanceRepository = new GLInitBalanceRepository();
			flag = gLInitBalanceRepository.ExistsByFilter(ctx, sqlWhere2);
			if (!flag)
			{
				GLBalanceRepository gLBalanceRepository = new GLBalanceRepository();
				flag = gLBalanceRepository.ExistsByFilter(ctx, sqlWhere2);
			}
			if (!flag)
			{
				GLVoucherRepository gLVoucherRepository = new GLVoucherRepository();
				flag = gLVoucherRepository.ExistVoucherEntryModel(ctx, model.MParentID);
			}
			if (flag)
			{
				BDAccountModel dataModel = dal.GetDataModel(ctx, model.MParentID, false);
				if (dataModel.MIsCheckForCurrency && !model.MIsCheckForCurrency)
				{
					operationResult.Success = true;
					operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "ForbitChangeCurrenyAccounting", "因为是第一个子级，并且父级已经存在了期初或者科目余额，或者凭证数据，不允许更改第一个子级为非外币核算！");
					return operationResult;
				}
			}
			operationResult.Success = flag;
			return operationResult;
		}

		public OperationResult CheckBalanceEqualWithBill(MContext ctx)
		{
			OperationResult operationResult = new OperationResult();
			if (ctx.MOrgVersionID == 0)
			{
				DateTime dateTime = ctx.MBeginDate;
				DateTime dateTime2 = ctx.MGLBeginDate;
				List<string> list = new List<string>();
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
				List<BDBankAccountEditModel> bankAccountList = BDBankAccountRepository.GetBankAccountList(ctx, null, false, null);
				List<BDBankBalanceModel> bankInitBalanceList = BDBankAccountRepository.GetBankInitBalanceList(ctx, dateTime, dateTime2, null);
				if (bankInitBalanceList != null && bankInitBalanceList.Count() > 0)
				{
					List<string> values = (from x in bankInitBalanceList
					select x.MBankID).ToList();
					BDAccountRepository bDAccountRepository = new BDAccountRepository();
					GLInitBalanceRepository gLInitBalanceRepository = new GLInitBalanceRepository();
					SqlWhere sqlWhere = new SqlWhere();
					sqlWhere.In("MAccountID", values);
					sqlWhere.Equal("MCheckGroupValueID", "0");
					List<GLInitBalanceModel> modelList = gLInitBalanceRepository.GetModelList(ctx, sqlWhere, true);
					foreach (BDBankBalanceModel item in bankInitBalanceList)
					{
						List<GLInitBalanceModel> list2 = (from x in modelList
						where x.MAccountID == item.MBankID
						select x).ToList();
						if ((list2 != null && list2.Count() != 0) || bDAccountRepository.Exists(ctx, item.MBankID, false))
						{
							if ((list2 == null || list2.Count() == 0) && bDAccountRepository.Exists(ctx, item.MBankID, false))
							{
								if (item.MTotalAmt != decimal.Zero)
								{
									list.Add(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "BankAccountInitBalanceNotEqualWithBankBalance", "银行科目:{0},初始化余额与银行余额不相等"), (from x in bankAccountList
									where x.MItemID == item.MBankID
									select x).FirstOrDefault().MBankName));
								}
							}
							else if (item.MTotalAmt != list2.Sum((GLInitBalanceModel x) => x.MInitBalance))
							{
								list.Add(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "BankAccountInitBalanceNotEqualWithBill", "银行科目:{0},初始化余额与单据金额不相等"), (from x in bankAccountList
								where x.MItemID == item.MBankID
								select x).FirstOrDefault().MBankName));
							}
						}
					}
				}
				if (list.Count > 0)
				{
					operationResult.Success = false;
					operationResult.Message = string.Join(";", list);
				}
			}
			else
			{
				operationResult.Success = true;
			}
			OperationResult operationResult2 = new BDInitDocumentBusiness().CheckInitBalanceEqualWithBill(ctx);
			operationResult.Success = (operationResult.Success && operationResult2.Success);
			OperationResult operationResult3 = operationResult;
			operationResult3.Message += ((operationResult2.Message.Length > 0) ? (";" + operationResult2.Message) : "");
			return operationResult;
		}

		public OperationResult CheckCustomAccountIsFinish(MContext ctx)
		{
			OperationResult operationResult = new OperationResult();
			bool isSys = ctx.IsSys;
			try
			{
				ctx.IsSys = false;
				SqlWhere sqlWhere = new SqlWhere();
				sqlWhere.Equal("MOrgID", ctx.MOrgID);
				sqlWhere.Equal("MAccountTableId", "3");
				List<BDAccountModel> modelList = dal.GetModelList(ctx, sqlWhere, false);
				if (modelList == null || modelList.Count() <= 0)
				{
					operationResult.Success = false;
				}
				else
				{
					operationResult.Success = true;
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				ctx.IsSys = isSys;
			}
			return operationResult;
		}

		public OperationResult CheckCustomAccountIsMatch(MContext ctx)
		{
			OperationResult operationResult = new OperationResult();
			bool isSys = ctx.IsSys;
			try
			{
				ctx.IsSys = false;
				Dictionary<string, string> customAccountMacthCode = GetCustomAccountMacthCode(ctx.MLCID);
				List<BDAccountModel> baseBDAccountList = dal.GetBaseBDAccountList(ctx, new SqlWhere(), false, null);
				if (baseBDAccountList == null)
				{
					operationResult.Success = false;
					operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "CanFindAnyCustomAccount", "没有查找到任何自定义科目");
					return operationResult;
				}
				List<BDAccountModel> source = (from x in baseBDAccountList
				where !string.IsNullOrWhiteSpace(x.MCode)
				select x).ToList();
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				foreach (KeyValuePair<string, string> item in customAccountMacthCode)
				{
					BDAccountModel bDAccountModel = (from x in source
					where x.MCode == item.Key
					select x).FirstOrDefault();
					if (bDAccountModel == null)
					{
						dictionary.Add(item.Key, item.Value);
					}
				}
				if (dictionary.Count() > 0)
				{
					operationResult.Success = false;
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.AppendLine(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "AccountNotMatch", "下面这些科目没有匹配成功:"));
					foreach (KeyValuePair<string, string> item2 in dictionary)
					{
						stringBuilder.AppendLine(item2.Value);
					}
					stringBuilder.AppendLine(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "PelaseMatchAccount", "请先进行匹配！"));
					operationResult.Message = stringBuilder.ToString();
				}
				else
				{
					operationResult.Success = true;
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				ctx.IsSys = isSys;
			}
			return operationResult;
		}

		public OperationResult CustomAccountFinishMatch(MContext ctx, List<BDAccountModel> accountList)
		{
			OperationResult operationResult = new OperationResult();
			if (accountList == null || accountList.Count() == 0)
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "NotFindMatchAccount", "没有找到匹配的科目！");
				return operationResult;
			}
			if (ctx.MInitBalanceOver)
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "NotAllowMatchBecauseInitBalanceOver", "科目期初余额已完成了初始化，不允许在进行科目匹配！");
				return operationResult;
			}
			GLInitBalanceRepository gLInitBalanceRepository = new GLInitBalanceRepository();
			List<GLInitBalanceModel> modelList = gLInitBalanceRepository.GetModelList(ctx, new SqlWhere(), true);
			if (modelList != null && modelList.Count() > 0)
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "NotAllowMatchBecauseHaveInitBalance", "已存在科目期初余额，不允许在进行科目匹配！");
				return operationResult;
			}
			Dictionary<string, string> customAccountMacthCode = GetCustomAccountMacthCode(ctx.MLCID);
			List<BDAccountModel> modelList2 = dal.GetModelList(ctx, new SqlWhere(), false);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (KeyValuePair<string, string> item in customAccountMacthCode)
			{
				BDAccountModel bDAccountModel = (from x in accountList
				where x.MCode == item.Key
				select x).FirstOrDefault();
				if (bDAccountModel == null)
				{
					dictionary.Add(item.Key, item.Value);
				}
			}
			if (dictionary.Count() > 0)
			{
				operationResult.Success = false;
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "AccountNotMatch", "下面这些科目没有匹配成功:"));
				foreach (KeyValuePair<string, string> item2 in dictionary)
				{
					stringBuilder.AppendLine(item2.Value);
				}
				stringBuilder.AppendLine(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "PelaseMatchAccount", "请先进行匹配！"));
				operationResult.Message = stringBuilder.ToString();
				return operationResult;
			}
			List<CommandInfo> list = new List<CommandInfo>();
			BDBankAccountRepository bDBankAccountRepository = new BDBankAccountRepository();
			List<BDBankAccountEditModel> modelList3 = bDBankAccountRepository.GetModelList(ctx, new SqlWhere(), false);
			List<string> list2 = new List<string>();
			list2.Add("MCode");
			list2.Add("MIsSys");
			List<BDAccountModel> list3 = (from x in modelList2
			where x.MIsSys
			select x).ToList();
			foreach (BDAccountModel item3 in list3)
			{
				item3.MIsSys = false;
				if (!accountList.Exists((BDAccountModel x) => x.MItemID == item3.MItemID))
				{
					item3.MCode = GetCustomAccountCode(ctx, item3, modelList2);
				}
				else
				{
					item3.MCode = null;
				}
				list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDAccountModel>(ctx, item3, list2, true));
			}
			foreach (BDBankAccountEditModel item4 in modelList3)
			{
				List<string> list4 = new List<string>();
				list4.Add("MAccountID");
				item4.MAccountID = null;
				list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDBankAccountEditModel>(ctx, item4, list4, true));
				list.AddRange(ModelInfoManager.GetDeleteCmd<BDAccountModel>(ctx, item4.MItemID));
			}
			List<BDAccountModel> list5 = new List<BDAccountModel>();
			foreach (BDAccountModel account in accountList)
			{
				account.MOrgID = (string.IsNullOrWhiteSpace(account.MOrgID) ? ctx.MOrgID : account.MOrgID);
				account.IsNew = false;
				account.MIsSys = CheckCustomeIsSys(ctx, account);
				List<BDAccountModel> childrenAccountByRecursion = GetChildrenAccountByRecursion(account, modelList2, false);
				list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDAccountModel>(ctx, account, list2, true));
				if (childrenAccountByRecursion != null && childrenAccountByRecursion.Count() > 0)
				{
					int num = 1;
					childrenAccountByRecursion = (from x in childrenAccountByRecursion
					orderby x.MNumber
					select x).ToList();
					foreach (BDAccountModel item5 in childrenAccountByRecursion)
					{
						item5.MCode = ((num < 10) ? (account.MCode + "0" + num) : (account.MCode + Convert.ToString(num)));
						list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDAccountModel>(ctx, item5, list2, true));
					}
				}
			}
			int num2 = BDRepository.ExecuteSqlTran(ctx, list);
			operationResult.Success = (num2 > 0);
			if (operationResult.Success)
			{
				List<CommandInfo> bankAccountAsynCmds = GetBankAccountAsynCmds(ctx);
				BDRepository.ExecuteSqlTran(ctx, bankAccountAsynCmds);
			}
			return operationResult;
		}

		public bool CheckCustomeIsSys(MContext ctx, BDAccountModel model)
		{
			Dictionary<string, string> customAccountMacthCode = GetCustomAccountMacthCode(ctx.MLCID);
			bool result = false;
			if (string.IsNullOrWhiteSpace(model.MCode))
			{
				return result;
			}
			foreach (KeyValuePair<string, string> item in customAccountMacthCode)
			{
				if (model.MCode == item.Key)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		public Dictionary<string, string> GetCustomAccountMacthCode(string langeId)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("1001", COMMultiLangRepository.GetText(langeId, LangModule.Acct, "CashInHand", "Cash in Hand"));
			dictionary.Add("1002", COMMultiLangRepository.GetText(langeId, LangModule.Acct, "Bank", "Bank"));
			dictionary.Add("1122", COMMultiLangRepository.GetText(langeId, LangModule.Acct, "AccountsReceivable", "Accounts Receivable"));
			dictionary.Add("1123", COMMultiLangRepository.GetText(langeId, LangModule.Acct, "Prepayment", "Prepayment"));
			dictionary.Add("2202", COMMultiLangRepository.GetText(langeId, LangModule.Acct, "AccountsPayable", "Accounts Payable"));
			dictionary.Add("2203", COMMultiLangRepository.GetText(langeId, LangModule.Acct, "AdvanceCustomers", "Advance from customers"));
			dictionary.Add("1221", COMMultiLangRepository.GetText(langeId, LangModule.Acct, "OtherReceivable", "Other Receivable"));
			dictionary.Add("2241", COMMultiLangRepository.GetText(langeId, LangModule.Acct, "OtherPayables", "Other Payables"));
			dictionary.Add("22210101", COMMultiLangRepository.GetText(langeId, LangModule.Acct, "VATIN", "进项税额"));
			dictionary.Add("22210105", COMMultiLangRepository.GetText(langeId, LangModule.Acct, "VATOUT", "销项税额"));
			dictionary.Add("1405", COMMultiLangRepository.GetText(langeId, LangModule.Acct, "FinishedGoods", "库存商品"));
			dictionary.Add("660303", COMMultiLangRepository.GetText(langeId, LangModule.Acct, "BankCharges", "银行手续费"));
			dictionary.Add("660301", COMMultiLangRepository.GetText(langeId, LangModule.Acct, "InterestExpenses", "利息费用"));
			dictionary.Add("6401", COMMultiLangRepository.GetText(langeId, LangModule.Acct, "PrimeOperatingCosts", "主营业务成本"));
			return dictionary;
		}

		public string GetCustomAccountCode(MContext ctx, BDAccountModel account, List<BDAccountModel> allAcountList)
		{
			string text = "";
			BDAccountModel parentModel = (from x in allAcountList
			where x.MItemID == account.MParentID
			select x).FirstOrDefault();
			int num = 0;
			if (parentModel == null)
			{
				List<BDAccountModel> source = (from x in allAcountList
				where x.MParentID == "0"
				select x).ToList();
				source = (from x in source
				orderby int.Parse(x.MCode)
				select x).ToList();
				num = ((source.Count() > 0) ? (int.Parse(source.Last().MCode) + 1) : 7000);
				text = Convert.ToString(num);
			}
			else
			{
				List<BDAccountModel> source2 = (from x in allAcountList
				where x.MParentID == parentModel.MItemID
				select x).ToList();
				source2 = (from x in source2
				orderby int.Parse(x.MCode)
				select x).ToList();
				num = int.Parse(source2.Last().MCode) + 1;
			}
			return Convert.ToString(num);
		}

		public OperationResult InsertDefaultAccount(MContext ctx)
		{
			OperationResult operationResult = new OperationResult();
			BASOrgInitSettingRepository bASOrgInitSettingRepository = new BASOrgInitSettingRepository();
			List<CommandInfo> list = new List<CommandInfo>();
			string mAccountTableID = ctx.MAccountTableID;
			List<BDAccountModel> dataModelList = ModelInfoManager.GetDataModelList<BDAccountModel>(ctx, new SqlWhere().Equal("MOrgID", "0").Equal("MAccountTableID", mAccountTableID), false, false);
			List<NameValueModel> fullNameList = new List<NameValueModel>();
			bASOrgInitSettingRepository.UpdateAccountList(ctx, dataModelList, fullNameList, null, 7000);
			if (ctx.MAccountTableID == "3")
			{
				list.AddRange(bASOrgInitSettingRepository.GetAcctInsertCmds(ctx, dataModelList, fullNameList));
				int num = BDRepository.ExecuteSqlTran(ctx, list);
				operationResult.Success = (num > 0);
				if (operationResult.Success)
				{
					List<CommandInfo> bankAccountAsynCmds = GetBankAccountAsynCmds(ctx);
					BDRepository.ExecuteSqlTran(ctx, bankAccountAsynCmds);
				}
			}
			return operationResult;
		}

		public List<CommandInfo> GetBankAccountAsynCmds(MContext ctx)
		{
			BDBankAccountRepository bDBankAccountRepository = new BDBankAccountRepository();
			BDAccountRepository bDAccountRepository = new BDAccountRepository();
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.Equal("MIsDelete", 0);
			sqlWhere.Equal("MIsActive", 1);
			List<BDAccountModel> baseBDAccountList = bDAccountRepository.GetBaseBDAccountList(ctx, null, false, null);
			BDAccountModel bDAccountModel = (from x in baseBDAccountList
			where x.MCode == "1001" && x.MIsSys
			select x).FirstOrDefault();
			BDAccountModel bDAccountModel2 = (from x in baseBDAccountList
			where x.MCode == "1002" && x.MIsSys
			select x).FirstOrDefault();
			List<BDBankAccountEditModel> modelList = bDBankAccountRepository.GetModelList(ctx, sqlWhere, false);
			List<CommandInfo> list = new List<CommandInfo>();
			if (modelList != null && bDAccountModel != null && bDAccountModel2 != null)
			{
				modelList = (from x in modelList
				orderby (from y in x.MultiLanguage
				where y.MFieldName == "MName"
				select y).First().MMultiLanguageValue
				select x).ToList();
				modelList = (from x in modelList
				where string.IsNullOrEmpty(x.MAccountID)
				select x).ToList();
				List<string> list2 = new List<string>();
				list2.Add("MAccountID");
				List<string> list3 = new List<string>();
				list3.Add("MParentID");
				list3.Add("MNumber");
				list3.Add("MDC");
				list3.Add("MAccountTypeID");
				list3.Add("MAccountGroupID");
				list3.Add("MAccountTableID");
				list3.Add("MCode");
				int num = 1;
				int num2 = 1;
				foreach (BDBankAccountEditModel item in modelList)
				{
					BDAccountModel bDAccountModel3 = (from x in baseBDAccountList
					where x.MItemID == item.MItemID
					select x).FirstOrDefault();
					if (bDAccountModel3 != null)
					{
						item.MAccountID = bDAccountModel3.MItemID;
						list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDBankAccountEditModel>(ctx, item, list2, true));
					}
					else
					{
						item.MAccountID = item.MItemID;
						list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDBankAccountEditModel>(ctx, item, list2, true));
						bDAccountModel3 = new BDAccountModel();
						bDAccountModel3.MItemID = item.MItemID;
						bDAccountModel3.MIsActive = true;
						bDAccountModel3.MIsDelete = false;
						bDAccountModel3.MOrgID = ctx.MOrgID;
						bDAccountModel3.MultiLanguage = BDBankAccountBusiness.GetAccountMulitLang(item.MultiLanguage, bDAccountModel3.MultiLanguage);
						bDAccountModel3.IsNew = true;
					}
					if (item.MBankAccountType == 3)
					{
						string text = (num2 < 10) ? ("0" + Convert.ToString(num2)) : Convert.ToString(num2);
						bDAccountModel3.MParentID = bDAccountModel.MItemID;
						bDAccountModel3.MDC = bDAccountModel.MDC;
						bDAccountModel3.MAccountTypeID = bDAccountModel.MAccountTypeID;
						bDAccountModel3.MAccountGroupID = bDAccountModel.MAccountGroupID;
						bDAccountModel3.MAccountTableID = bDAccountModel.MAccountTableID;
						bDAccountModel3.MNumber = bDAccountModel.MNumber + "." + text;
						bDAccountModel3.MIsCheckForCurrency = (item.MCyID != ctx.MBasCurrencyID);
						bDAccountModel3.MCode = bDAccountModel.MCode + text;
						num2++;
					}
					else
					{
						string text2 = (num < 10) ? ("0" + Convert.ToString(num)) : Convert.ToString(num);
						bDAccountModel3.MParentID = bDAccountModel2.MItemID;
						bDAccountModel3.MDC = bDAccountModel2.MDC;
						bDAccountModel3.MAccountTypeID = bDAccountModel2.MAccountTypeID;
						bDAccountModel3.MAccountGroupID = bDAccountModel2.MAccountGroupID;
						bDAccountModel3.MAccountTableID = bDAccountModel2.MAccountTableID;
						bDAccountModel3.MNumber = bDAccountModel2.MNumber + "." + text2;
						bDAccountModel3.MIsCheckForCurrency = (item.MCyID != ctx.MBasCurrencyID);
						bDAccountModel3.MCode = bDAccountModel2.MCode + text2;
						num++;
					}
					list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDAccountModel>(ctx, bDAccountModel3, null, true));
				}
			}
			return list;
		}

		private string GetPresetMatchNumber(List<BDAccountListModel> acctList, string parentAcctMatchNumber)
		{
			string result = string.Empty;
			if (acctList == null || !acctList.Any())
			{
				return result;
			}
			if (string.IsNullOrWhiteSpace(parentAcctMatchNumber))
			{
				result = acctList.FirstOrDefault().MNumber;
			}
			else
			{
				BDAccountListModel bDAccountListModel = acctList.FirstOrDefault((BDAccountListModel f) => f.MNumber.StartsWith(parentAcctMatchNumber));
				if (bDAccountListModel != null)
				{
					result = bDAccountListModel.MNumber;
				}
			}
			return result;
		}

		public List<BDAccountEditModel> GetMigrateAccountPresetMatch(MContext ctx, List<BDAccountEditModel> acctList)
		{
			GetImportAccountMatchResult(ctx, ref acctList, true);
			return acctList;
		}

		public IOAccountMatchModel GetImportAccountMatchResult(MContext ctx, List<BDAccountEditModel> acctList)
		{
			return GetImportAccountMatchResult(ctx, ref acctList, false);
		}

		public IOAccountMatchModel GetImportAccountMatchResult(MContext ctx, ref List<BDAccountEditModel> acctList, bool isFromMigration = false)
		{
			IOAccountMatchModel iOAccountMatchModel = new IOAccountMatchModel();
			List<BDAccountListModel> list = GetAccountList(ctx, new SqlWhere(), true);
			if (!isFromMigration)
			{
				list = (from f in list
				where !bankNumberPrefix.Any((string n) => f.MNumber.StartsWith(n)) && f.MIsActive
				select f).ToList();
			}
			Dictionary<int, int> importAcctNumberFmt = GetImportAcctNumberFmt(acctList);
			foreach (BDAccountEditModel acct in acctList)
			{
				acct.MOriNumber = acct.MNumber;
				acct.MNumber = acct.MNumber.ToStandardAcctNumber(true, 2);
				acct.MNumberStandard = GetStandardAcctNumber(acct.MNumber, importAcctNumberFmt);
			}
			acctList = (from f in acctList
			orderby f.MNumberStandard
			select f).ToList();
			UpdateAccountRelation(acctList, null, null);
			List<string> list2 = new List<string>();
			int num = 0;
			foreach (BDAccountEditModel acct2 in acctList)
			{
				acct2.MRowIndex = num;
				acct2.IsUpdate = false;
				string standardNumber = acct2.MNumber.ToStandardAcctNumber(true, 2);
				bool flag = false;
				BDAccountListModel bDAccountListModel = null;
				BDAccountListModel bDAccountListModel2 = list.FirstOrDefault((BDAccountListModel f) => f.MNumber == standardNumber && !string.IsNullOrWhiteSpace(f.MName) && f.MName.Trim().ToUpper() == acct2.MName.Trim().ToUpper());
				if (bDAccountListModel2 != null && isFromMigration)
				{
					flag = acctList.Any((BDAccountEditModel f) => f.MParentNumber == standardNumber);
					List<BDAccountListModel> leafAccountListByCode = GetLeafAccountListByCode(list, standardNumber.Replace(".", ""));
					IEnumerable<IGrouping<string, BDAccountListModel>> source = from f in leafAccountListByCode
					group f by f.MItemID;
					if (!flag)
					{
						if (source.Count() == 1)
						{
							bDAccountListModel2 = leafAccountListByCode.FirstOrDefault();
						}
						else
						{
							bDAccountListModel2 = null;
							bDAccountListModel = leafAccountListByCode.FirstOrDefault();
						}
					}
				}
				if (bDAccountListModel2 != null)
				{
					acct2.MatchResult = IOAccountMatchResultEnum.Matched;
					acct2.MItemID = bDAccountListModel2.MItemID;
					acct2.MCode = bDAccountListModel2.MCode;
					acct2.MParentID = bDAccountListModel2.MParentID;
					acct2.MMatchNumber = bDAccountListModel2.MNumber;
					acct2.MIsSys = bDAccountListModel2.MIsSys;
					acct2.MAccountTypeID = bDAccountListModel2.MAccountTypeID;
					acct2.MAccountGroupID = bDAccountListModel2.MAccountGroupID;
					acct2.MAccountTableID = ctx.MAccountTableID;
					acct2.IsCanRelateContact = bDAccountListModel2.IsCanRelateContact;
					acct2.MCheckGroupID = bDAccountListModel2.MCheckGroupID;
					list2.Add(bDAccountListModel2.MNumber);
				}
				else
				{
					BDAccountListModel bDAccountListModel3 = (bDAccountListModel != null) ? bDAccountListModel : list.FirstOrDefault((BDAccountListModel f) => !string.IsNullOrWhiteSpace(f.MName) && (f.MName.Trim().ToUpper() == acct2.MName.Trim().ToUpper() || acct2.MName.EndsWith(" " + f.MName) || acct2.MName.EndsWith("-" + f.MName) || acct2.MName.EndsWith("_" + f.MName) || acct2.MName.EndsWith("/" + f.MName)));
					string[] array = acct2.MNumber.Split('.');
					string[] array2 = (bDAccountListModel3 != null) ? bDAccountListModel3.MNumber.Split('.') : new string[1]
					{
						string.Empty
					};
					if (bDAccountListModel3 != null && !list2.Contains(bDAccountListModel3.MNumber) && ((array.Length == array2.Length && array[0] == array2[0]) || (isFromMigration && !flag)))
					{
						acct2.MMatchNumber = bDAccountListModel3.MNumber;
						list2.Add(bDAccountListModel3.MNumber);
					}
				}
				num++;
			}
			IEnumerable<BDAccountEditModel> enumerable = from f in acctList
			where string.IsNullOrWhiteSpace(f.MMatchNumber)
			select f;
			List<string> preMatchNumberList = (from f in acctList
			where !string.IsNullOrWhiteSpace(f.MMatchNumber)
			select f.MMatchNumber).ToList();
			foreach (BDAccountEditModel item in enumerable)
			{
				List<BDAccountListModel> list3 = (from f in list
				where !string.IsNullOrWhiteSpace(f.MName) && f.MName.Trim().ToUpper() == item.MName.Trim().ToUpper() && !preMatchNumberList.Contains(f.MNumber)
				select f).ToList();
				if (list3?.Any() ?? false)
				{
					BDAccountEditModel bDAccountEditModel = acctList.FirstOrDefault((BDAccountEditModel f) => f.MNumber == GetParentNo(item.MNumber));
					string parentAcctMatchNumber = (bDAccountEditModel == null) ? string.Empty : bDAccountEditModel.MMatchNumber;
					List<BDAccountListModel> list4 = (from f in list3
					where f.MNumber.Split('.').Length == item.MNumber.Split('.').Length
					select f).ToList();
					string text = string.Empty;
					if (list4 != null)
					{
						text = GetPresetMatchNumber(list4, parentAcctMatchNumber);
					}
					if (string.IsNullOrWhiteSpace(text))
					{
						text = GetPresetMatchNumber(list3, parentAcctMatchNumber);
					}
					if (!string.IsNullOrWhiteSpace(text))
					{
						item.MMatchNumber = text;
						preMatchNumberList.Add(item.MMatchNumber);
					}
				}
			}
			List<IOAccountModel> resultList = new List<IOAccountModel>();
			ConvertToAccountTreeModel(resultList, list, acctList, null, null, isFromMigration);
			acctList.ForEach(delegate(BDAccountEditModel f)
			{
				f.IsUpdate = false;
			});
			List<BDAccountMatchLogModel> dataModelList = ModelInfoManager.GetDataModelList<BDAccountMatchLogModel>(ctx, new SqlWhere().Equal("MIsDelete", 0).Equal("MMatchResult", 3), false, false);
			foreach (BDAccountEditModel acct3 in acctList)
			{
				string text2 = string.Empty;
				BDAccountMatchLogModel bDAccountMatchLogModel = dataModelList.FirstOrDefault((BDAccountMatchLogModel f) => f.MNumber == acct3.MNumber && !string.IsNullOrWhiteSpace(f.MName) && f.MName.Trim().ToUpper() == acct3.MName.Trim().ToUpper());
				if (bDAccountMatchLogModel != null && acct3.MatchResult == IOAccountMatchResultEnum.ManualMatch && bDAccountMatchLogModel.MMatchResult == 3)
				{
					acct3.MatchResult = (IOAccountMatchResultEnum)bDAccountMatchLogModel.MMatchResult;
					if (!string.IsNullOrWhiteSpace(bDAccountMatchLogModel.MMatchNumber))
					{
						acct3.MMatchNumber = bDAccountMatchLogModel.MMatchNumber;
						text2 = bDAccountMatchLogModel.MMatchNumber;
					}
					if (!string.IsNullOrWhiteSpace(bDAccountMatchLogModel.MNewNumber))
					{
						if (string.IsNullOrWhiteSpace(acct3.MMatchNumber) || acct3.MMatchNumber != bDAccountMatchLogModel.MNewNumber)
						{
							acct3.MNewNumberSaved = bDAccountMatchLogModel.MNewNumber;
							acct3.MNewNumber = bDAccountMatchLogModel.MNewNumber;
							text2 = bDAccountMatchLogModel.MNewNumber;
							if (!string.IsNullOrWhiteSpace(acct3.MMatchNumber))
							{
								acct3.MMatchNumber = string.Empty;
							}
						}
					}
					else
					{
						ReSortNewNumber(acctList, acct3);
						acct3.MNewNumber = "";
					}
				}
				if (!string.IsNullOrWhiteSpace(text2))
				{
					ReplaceParentNo(acctList, acct3.MNumber, text2, dataModelList);
				}
			}
			List<BDAccountEditModel> list5 = (from f in acctList
			where f.MatchResult == IOAccountMatchResultEnum.ManualMatch
			select f).ToList();
			List<IOAccountModel> list6 = new List<IOAccountModel>();
			ConvertToAccountTreeModel(list6, list, list5, null, null, isFromMigration);
			iOAccountMatchModel.ManualMatchList = list6;
			list5.ForEach(delegate(BDAccountEditModel f)
			{
				f.IsUpdate = false;
			});
			if (!iOAccountMatchModel.ManualMatchList.Any())
			{
				iOAccountMatchModel.ManualMatchList = new List<IOAccountModel>
				{
					new IOAccountModel()
				};
			}
			IOAccountModel iOAccountModel = iOAccountMatchModel.ManualMatchList[0];
			iOAccountModel.MatchResultList = acctList;
			List<BDAccountListModel> sysAcctList = iOAccountModel.SystemAccountList = (from f in list
			where f.MLocaleID == ctx.MLCID
			select f).ToList();
			if (isFromMigration)
			{
				acctList[0].SysAcctList = sysAcctList;
			}
			return iOAccountMatchModel;
		}

		private void ReSortNewNumber(List<BDAccountEditModel> acctList, BDAccountEditModel model)
		{
			if (!string.IsNullOrWhiteSpace(model.MNewNumber))
			{
				string[] array = model.MNewNumber.Split('.');
				int num = Convert.ToInt32(array[array.Length - 1]);
				int numericNewNumber = Convert.ToInt32(model.MNewNumber.GetValidAccountNo());
				string parentNewNo = GetParentNo(model.MNewNumber);
				IEnumerable<BDAccountEditModel> source = from f in acctList
				where !string.IsNullOrWhiteSpace(f.MNewNumber) && GetParentNo(f.MNewNumber) == parentNewNo && Convert.ToInt32(f.MNewNumber.GetValidAccountNo()) > numericNewNumber
				select f;
				if (source.Any())
				{
					BDAccountEditModel bDAccountEditModel = (from f in source
					orderby f.MNewNumber descending
					select f).FirstOrDefault();
					string[] array2 = bDAccountEditModel.MNewNumber.Split('.');
					int num2 = Convert.ToInt32(array2[array2.Length - 1]);
					string text = model.MNewNumber;
					for (int i = num + 1; i <= num2; i++)
					{
						array[array.Length - 1] = i.ToString().PadLeft(2, '0');
						string parentNo = string.Join(".", array);
						IEnumerable<BDAccountEditModel> enumerable = from f in acctList
						where !string.IsNullOrWhiteSpace(f.MNewNumber) && f.MNewNumber.StartsWith(parentNo)
						select f;
						foreach (BDAccountEditModel item in enumerable)
						{
							item.MNewNumber = item.MNewNumber.Replace(parentNo, text);
						}
						string[] array3 = text.Split('.');
						int num3 = Convert.ToInt32(array3[array.Length - 1]);
						num3++;
						array3[array3.Length - 1] = num3.ToString().PadLeft(2, '0');
						text = string.Join(".", array3);
					}
				}
			}
		}

		private void ReplaceParentNo(List<BDAccountEditModel> acctList, string oriNumber, string changedNumber, List<BDAccountMatchLogModel> matchLogList)
		{
			IEnumerable<BDAccountEditModel> enumerable = from f in acctList
			where GetParentNo(f.MNumber) == oriNumber
			select f;
			foreach (BDAccountEditModel item in enumerable)
			{
				if (!matchLogList.Any((BDAccountMatchLogModel f) => f.MNumber == item.MNumber) && string.IsNullOrWhiteSpace(item.MMatchNumber))
				{
					changedNumber = changedNumber.Split(' ')[0];
					item.MNewNumber = item.MNumber.Replace(oriNumber, changedNumber);
					ReplaceParentNo(acctList, item.MNumber, item.MNewNumber, matchLogList);
				}
			}
		}

		public List<IOAccountModel> PreviewAccountMatch(MContext ctx, List<BDAccountEditModel> acctList)
		{
			List<BDAccountListModel> bDAccountList = dal.GetBDAccountList(ctx, new SqlWhere().NotLike("a.MNumber", "1001").NotLike("a.MNumber", "1002"), null, true, false);
			List<IOAccountModel> list = new List<IOAccountModel>();
			OperationResult operationResult = new OperationResult();
			bool flag = false;
			bool flag2 = false;
			GetUpdateAccountListByMatch(ctx, acctList, operationResult, ref flag, ref flag2, null, null, false);
			List<BASAccountTypeModel> accountTypeList = new BASAccountTypeBusiness().GetAccountTypeList(ctx, false);
			Dictionary<int, int> importAcctNumberFmt = GetImportAcctNumberFmt(acctList);
			foreach (BDAccountEditModel acct in acctList)
			{
				acct.IsUpdate = false;
				BASAccountTypeModel bASAccountTypeModel = accountTypeList.FirstOrDefault((BASAccountTypeModel f) => f.MItemID == acct.MAccountTypeID);
				if (bASAccountTypeModel != null)
				{
					acct.MAccountTypeID = bASAccountTypeModel.MName;
				}
				acct.MNumberStandard = GetStandardAcctNumber(acct.MNumber, importAcctNumberFmt);
			}
			acctList = (from f in acctList
			orderby f.MNumberStandard
			select f).ToList();
			ConvertToAccountTreeModel(list, bDAccountList, acctList, null, null, false);
			if (!operationResult.Success)
			{
				list[0].Message = operationResult.Message;
			}
			list[0].IsNeedTransfer = AddAccountCheck(ctx, acctList).Success;
			return list;
		}

		public OperationResult AddAccountCheck(MContext ctx, List<BDAccountEditModel> list)
		{
			OperationResult operationResult = new OperationResult();
			bool success = false;
			IEnumerable<BDAccountEditModel> enumerable = from f in list
			where f.MatchResult == IOAccountMatchResultEnum.AutoAdd || (f.MatchResult == IOAccountMatchResultEnum.ManualMatch && !string.IsNullOrWhiteSpace(f.MNewNumber))
			select f;
			List<GLInitBalanceModel> dataModelList = ModelInfoManager.GetDataModelList<GLInitBalanceModel>(ctx, new SqlWhere(), false, false);
			List<GLBalanceModel> dataModelList2 = ModelInfoManager.GetDataModelList<GLBalanceModel>(ctx, new SqlWhere(), false, false);
			List<GLVoucherEntryModel> dataModelList3 = ModelInfoManager.GetDataModelList<GLVoucherEntryModel>(ctx, new SqlWhere(), false, false);
			List<BDAccountListModel> bDAccountList = dal.GetBDAccountList(ctx, new SqlWhere().NotLike("a.MNumber", "1001").NotLike("a.MNumber", "1002"), null, true, false);
			foreach (BDAccountEditModel item in enumerable)
			{
				if (!bDAccountList.Any((BDAccountListModel f) => f.MParentID == item.MParentID) && (dataModelList.Any((GLInitBalanceModel f) => f.MAccountID == item.MParentID) || dataModelList2.Any((GLBalanceModel f) => f.MAccountID == item.MParentID) || dataModelList3.Any((GLVoucherEntryModel f) => f.MAccountID == item.MParentID)))
				{
					success = true;
					break;
				}
			}
			operationResult.Success = success;
			return operationResult;
		}

		private void SetAccountCheckGroupModel(MContext ctx, BDAccountModel model, List<GLCheckTypeModel> checkTypeList, List<GLCheckGroupModel> checkGroupList, bool isSave = false, bool isFromMigration = false, string matchNumber = null, string newNumber = null)
		{
			if (!string.IsNullOrWhiteSpace(model.MCheckGroupNames))
			{
				List<string> list = new List<string>();
				string[] array = model.MCheckGroupNames.Trim('/').Split('/');
				List<string> list2 = (from c in array
				group c by c into g
				where g.Count() > 1
				select g into f
				select f.Key).ToList();
				if (list2.Any())
				{
					throw new Exception(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "DuplicateCheckGroup", "辅助核算不能重复:{0}"), string.Join("、", list2)));
				}
				GLCheckGroupModel gLCheckGroupModel = new GLCheckGroupModel();
				int[] source = new int[7]
				{
					0,
					2,
					5,
					6,
					7,
					8,
					9
				};
				if (isFromMigration && existTrackList == null)
				{
					existTrackList = _track.GetModelList(ctx, new SqlWhere(), false);
				}
				string[] array2 = array;
				foreach (string str in array2)
				{
					GLCheckTypeModel gLCheckTypeModel = checkTypeList.FirstOrDefault((GLCheckTypeModel f) => f.MName.ToUpper().Trim() == str.ToUpper().Trim());
					if (gLCheckTypeModel == null & isFromMigration)
					{
						int num = existTrackList.Count + checkTypeList.Count((GLCheckTypeModel f) => f.MColumnName.StartsWith("MTrackItem"));
						gLCheckTypeModel = new GLCheckTypeModel
						{
							MType = (5 + num).ToString(),
							MColumnName = $"MTrackItem{num + 1}",
							MName = str
						};
						checkTypeList.Add(gLCheckTypeModel);
					}
					if (gLCheckTypeModel == null)
					{
						if (!isFromMigration)
						{
							throw new Exception(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "CanNotFindCheckGroup", "Can not find check group:{0}"), str));
						}
					}
					else
					{
						bool flag = requiredCurrentAcctList.Contains(model.CurrentAcctNumber) && ((!string.IsNullOrWhiteSpace(matchNumber) && matchNumber.StartsWith(model.CurrentAcctNumber)) || (!string.IsNullOrWhiteSpace(newNumber) && newNumber.StartsWith(model.CurrentAcctNumber)));
						if (string.IsNullOrWhiteSpace(model.CurrentAcctNumber) || otherCurrentAcctList.Contains(model.CurrentAcctNumber) || (flag && source.Contains(Convert.ToInt32(gLCheckTypeModel.MType))))
						{
							list.Add(str);
							int num2 = isFromMigration ? CheckTypeStatusEnum.Required : CheckTypeStatusEnum.Optional;
							ModelHelper.SetModelValue(gLCheckGroupModel, gLCheckTypeModel.MColumnName, num2, null);
						}
						if ((isFromMigration & flag) && !source.Contains(Convert.ToInt32(gLCheckTypeModel.MType)))
						{
							string empty = string.Empty;
							string empty2 = string.Empty;
							if (!string.IsNullOrWhiteSpace(matchNumber))
							{
								empty = "匹配";
								empty2 = matchNumber;
							}
							else
							{
								empty = "新增";
								empty2 = newNumber;
							}
							string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "AcctNotSupportCheckGroup", "源账套中科目（{0}）{1}的美记科目（{2}）无法使用核算维度“{3}”，请匹配到其他科目！");
							throw new Exception(string.Format(text, model.MNumber, empty, empty2, gLCheckTypeModel.MName));
						}
					}
				}
				if (requiredCurrentAcctList.Contains(model.CurrentAcctNumber) && gLCheckGroupModel.MContactID != CheckTypeStatusEnum.Required)
				{
					ModelHelper.SetModelValue(gLCheckGroupModel, "MContactID", CheckTypeStatusEnum.Required, null);
				}
				if (model.MCheckGroupID == "0" || string.IsNullOrWhiteSpace(model.MCheckGroupID))
				{
					model.MCheckGroupModel = gLCheckGroupModel;
				}
				else
				{
					GLCheckGroupModel gLCheckGroupModel2 = (model.MCheckGroupModel == null) ? (checkGroupList.FirstOrDefault((GLCheckGroupModel f) => f.MItemID == model.MCheckGroupID).Clone() as GLCheckGroupModel) : model.MCheckGroupModel;
					if (gLCheckGroupModel2 == null)
					{
						model.MCheckGroupModel = gLCheckGroupModel;
						return;
					}
					foreach (GLCheckTypeModel checkType in checkTypeList)
					{
						int num3 = Convert.ToInt32(ModelHelper.GetModelValue(gLCheckGroupModel2, checkType.MColumnName));
						int num4 = Convert.ToInt32(ModelHelper.GetModelValue(gLCheckGroupModel, checkType.MColumnName));
						if (num3 != CheckTypeStatusEnum.Required)
						{
							ModelHelper.SetModelValue(gLCheckGroupModel2, checkType.MColumnName, num4, null);
						}
					}
					model.MCheckGroupModel = gLCheckGroupModel2;
				}
				model.MCheckGroupNames = string.Join("/", list);
			}
		}

		public void MoveCheckGroupToLeafAcct(MContext ctx, List<BDAccountEditModel> list, List<BDAccountEditModel> updateAcctList, List<BDAccountEditModel> sysAcctList, List<GLCheckTypeModel> checkTypeList)
		{
			List<GLCheckGroupModel> modelList = _checkGroup.GetModelList(ctx, new SqlWhere(), false);
			foreach (BDAccountEditModel item in list)
			{
				if (!string.IsNullOrWhiteSpace(item.MCheckGroupNames) && !list.Any((BDAccountEditModel f) => f.MParentID == item.MItemID))
				{
					IEnumerable<BDAccountEditModel> enumerable = from f in GetLeafAccountListByCode(sysAcctList, item.MCode)
					where f.MNumber != item.MNumber
					select f;
					if (enumerable.Any())
					{
						GLCheckGroupModel mCheckGroupModel = item.MCheckGroupModel;
						foreach (BDAccountEditModel item2 in enumerable)
						{
							GLCheckGroupModel gLCheckGroupModel = string.IsNullOrWhiteSpace(item2.MCheckGroupID) ? null : modelList.FirstOrDefault((GLCheckGroupModel f) => f.MItemID == item2.MCheckGroupID);
							if (gLCheckGroupModel == null)
							{
								gLCheckGroupModel = mCheckGroupModel;
							}
							else
							{
								foreach (GLCheckTypeModel checkType in checkTypeList)
								{
									int num = Convert.ToInt32(ModelHelper.GetModelValue(mCheckGroupModel, checkType.MColumnName));
									if (num != 0)
									{
										ModelHelper.SetModelValue(gLCheckGroupModel, checkType.MColumnName, num, null);
									}
								}
							}
							item2.MCheckGroupModel = gLCheckGroupModel;
							item2.IsUpdate = true;
							updateAcctList.Add(item2);
						}
						item.MCheckGroupModel = new GLCheckGroupModel();
					}
				}
			}
		}

		public OperationResult ImportAccountList(MContext ctx, List<BDAccountEditModel> acctList)
		{
			return ImportAccountList(ctx, acctList, false, false);
		}

		public OperationResult ImportAccountList(MContext ctx, List<BDAccountEditModel> acctList, bool isFromMigration = false, bool isFromApi = false)
		{
			OperationResult operationResult = new OperationResult();
			List<CommandInfo> list = new List<CommandInfo>();
			List<BDAccountEditModel> list2 = ModelInfoManager.GetDataModelList<BDAccountEditModel>(ctx, new SqlWhere().Equal("MIsDelete", 0).Equal("MIsActive", 1), false, false);
			if (!isFromMigration && !isFromApi)
			{
				list2 = (from f in list2
				where !bankNumberPrefix.Any((string n) => f.MNumber.StartsWith(n))
				select f).ToList();
			}
			SetAcctInfoFromApi(acctList, list2);
			List<BDAccountEditModel> list3 = new List<BDAccountEditModel>();
			bool flag = false;
			bool flag2 = false;
			GetUpdateAccountListByMatch(ctx, acctList, operationResult, ref flag, ref flag2, list2, list3, isFromMigration);
			if (!operationResult.Success)
			{
				return operationResult;
			}
			if (flag)
			{
				IEnumerable<IGrouping<string, BDAccountEditModel>> enumerable = from x in acctList
				group x by x.MParentID;
				foreach (IGrouping<string, BDAccountEditModel> item2 in enumerable)
				{
					if (!(item2.Key == "0"))
					{
						List<BDAccountEditModel> list4 = item2.ToList();
						BDAccountEditModel bDAccountEditModel = list2.SingleOrDefault((BDAccountEditModel f) => f.MItemID == item2.Key);
						if (bDAccountEditModel == null || string.IsNullOrWhiteSpace(bDAccountEditModel.MItemID))
						{
							string parentAccountID = FindExistParentAccountID(ctx, item2.Key, acctList);
							if (!string.IsNullOrWhiteSpace(parentAccountID))
							{
								list4.ForEach(delegate(BDAccountEditModel f)
								{
									f.MExistParentID = parentAccountID;
								});
								continue;
							}
							operationResult.Success = false;
							BDAccountEditModel bDAccountEditModel2 = list2.FirstOrDefault((BDAccountEditModel f) => f.MItemID == item2.Key);
							operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "CanNotFindParent", "Can not find parent account ,please check account:") + ((bDAccountEditModel2 != null) ? bDAccountEditModel2.MNumber : string.Empty);
							return operationResult;
						}
						list4.ForEach(delegate(BDAccountEditModel f)
						{
							f.MExistParentID = item2.Key;
						});
					}
				}
			}
			List<BDAccountEditModel> list5 = (from f in acctList
			where f.MIsCheckForCurrency
			select f).ToList();
			for (int i = 0; i < list5.Count(); i++)
			{
				SetParentAcctForeignAccting(list5[i], acctList, list2);
			}
			try
			{
				string text = string.Empty;
				IEnumerable<BDAccountEditModel> source = from f in acctList
				where f.MatchResult == IOAccountMatchResultEnum.AutoAdd || (f.MatchResult == IOAccountMatchResultEnum.ManualMatch && !string.IsNullOrWhiteSpace(f.MNewNumber))
				select f;
				List<GLInitBalanceModel> initBalanceList = BDAccountRepository.GetInitBalanceList(ctx, new SqlWhere());
				List<GLBalanceModel> modelList = balanceDal.GetModelList(ctx, new SqlWhere(), true);
				foreach (BDAccountEditModel acct in acctList)
				{
					string empty = string.Empty;
					list.AddRange(GetCheckGroupInsertOrUpdateCmds(ctx, acct.MCheckGroupModel, out empty));
					if (!string.IsNullOrEmpty(empty))
					{
						acct.MCheckGroupID = empty;
					}
					List<BDAccountEditModel> list6 = (from f in source
					where f.MParentID == acct.MParentID
					orderby f.MNumber
					select f).ToList();
					if (!string.IsNullOrWhiteSpace(acct.MParentID) && (flag || (list2.Any((BDAccountEditModel f) => f.MItemID == acct.MParentID) && !BDRepository.IsCanDelete(ctx, "AccountEdit", acct.MParentID).Success)) && !list2.Any((BDAccountEditModel f) => f.MParentID == acct.MParentID) && list6[0].MNumber == acct.MNumber)
					{
						list.AddRange(GetUpdateAccountCmds(ctx, acct, out text, acctList, initBalanceList, modelList));
					}
					else if (CheckAccountNumberTooLength(acct.MNumber))
					{
						text = acct.MNumber + ":" + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "AccountNumberTooLength", "科目代码每级最大4位!");
					}
					else
					{
						list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDAccountEditModel>(ctx, acct, null, true));
					}
					if (!string.IsNullOrWhiteSpace(text))
					{
						throw new Exception(text);
					}
					if (isFromMigration)
					{
						AddAccountMigrationLog(ctx, acctList, acct);
					}
				}
				foreach (BDAccountEditModel item3 in list3)
				{
					string empty2 = string.Empty;
					list.AddRange(GetCheckGroupInsertOrUpdateCmds(ctx, item3.MCheckGroupModel, out empty2));
					if (!string.IsNullOrEmpty(empty2))
					{
						item3.MCheckGroupID = empty2;
					}
				}
				if (list3.Any())
				{
					list.AddRange(ModelInfoManager.GetInsertOrUpdateCmds(ctx, list3, null, true));
				}
				if (updateAcctFullNameCmdList.Any())
				{
					list.AddRange(updateAcctFullNameCmdList);
				}
				bizAcctMatchLog.GetUpdateMatchLogCmds(ctx, acctList, list, list2, isFromMigration);
				List<CommandInfo> list7 = new List<CommandInfo>();
				if (isFromMigration)
				{
					AddMigrationCmdList(ctx, acctList, list7, list);
				}
				List<MultiDBCommand> list8 = new List<MultiDBCommand>();
				list8.Add(new MultiDBCommand(ctx)
				{
					CommandList = list,
					DBType = SysOrBas.Bas
				});
				if (list7.Any())
				{
					list8.Add(new MultiDBCommand(ctx)
					{
						CommandList = list7,
						DBType = SysOrBas.Sys
					});
				}
				operationResult.Success = DbHelperMySQL.ExecuteSqlTran(ctx, list8.ToArray());
			}
			catch (Exception ex)
			{
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = ex.Message
				});
			}
			return operationResult;
		}

		private void SetAcctInfoFromApi(List<BDAccountEditModel> acctList, List<BDAccountEditModel> sysAcctList)
		{
			if (acctList.Count((BDAccountEditModel f) => f.MatchResult == IOAccountMatchResultEnum.None) == acctList.Count)
			{
				UpdateAccountRelation(acctList, null, null);
				foreach (BDAccountEditModel acct in acctList)
				{
					acct.MatchResult = IOAccountMatchResultEnum.ManualMatch;
					if (string.IsNullOrWhiteSpace(acct.MItemID))
					{
						acct.MNewNumber = acct.MNumber;
					}
					else
					{
						BDAccountEditModel bDAccountEditModel = sysAcctList.FirstOrDefault((BDAccountEditModel f) => f.MItemID == acct.MItemID);
						acct.MMatchNumber = bDAccountEditModel.MNumber;
					}
				}
			}
		}

		private void AddMigrationCmdList(MContext ctx, List<BDAccountEditModel> acctList, List<CommandInfo> sysCmdList, List<CommandInfo> basCmdList)
		{
			if (acctList[0].ContactList != null && acctList[0].ContactList.Any())
			{
				basCmdList.AddRange(ModelInfoManager.GetInsertOrUpdateCmds(ctx, acctList[0].ContactList, null, true));
			}
			if (acctList[0].EmployeeList != null && acctList[0].EmployeeList.Any())
			{
				basCmdList.AddRange(ModelInfoManager.GetInsertOrUpdateCmds(ctx, acctList[0].EmployeeList, null, true));
			}
			if (acctList[0].ItemList != null && acctList[0].ItemList.Any())
			{
				basCmdList.AddRange(ModelInfoManager.GetInsertOrUpdateCmds(ctx, acctList[0].ItemList, null, true));
			}
			if (acctList[0].TrackList != null && acctList[0].TrackList.Any())
			{
				foreach (BDTrackModel track in acctList[0].TrackList)
				{
					foreach (BDTrackEntryModel mEntry in track.MEntryList)
					{
						mEntry.MIsActive = true;
					}
				}
				basCmdList.AddRange(ModelInfoManager.GetInsertOrUpdateCmds(ctx, acctList[0].TrackList, null, true));
			}
			if (!string.IsNullOrWhiteSpace(acctList[0].MMigrationID))
			{
				MigrateConfigModel migrateConfigModel = new MigrateConfigModel();
				migrateConfigModel.MItemID = acctList[0].MMigrationID;
				migrateConfigModel.MType = 3;
				ctx.IsSys = true;
				sysCmdList.AddRange(MigrateConfigRepository.GetUpdateProgressCmdList(ctx, migrateConfigModel, "MType"));
				ctx.IsSys = false;
			}
			if (acctList[0].MigrateLogList != null && acctList[0].MigrateLogList.Any())
			{
				basCmdList.AddRange(ModelInfoManager.GetInsertOrUpdateCmds(ctx, acctList[0].MigrateLogList, null, true));
			}
		}

		private void AddAccountMigrationLog(MContext ctx, List<BDAccountEditModel> acctList, BDAccountEditModel item)
		{
			if (acctList[0].MigrateLogList == null)
			{
				acctList[0].MigrateLogList = new List<MigrateLogBaseModel>();
			}
			acctList[0].MigrateLogList.Add(new MigrateLogBaseModel
			{
				MMigrationID = item.MMigrationID,
				MOrgID = ctx.MOrgID,
				MType = 3,
				MSourceID = item.MSourceID,
				MSourceCode = item.MSourceCode,
				MSourceName = item.MSourceName,
				MMegiID = item.MItemID
			});
		}

		private void SetParentAcctForeignAccting(BDAccountEditModel child, List<BDAccountEditModel> acctList, List<BDAccountEditModel> sysAcctList)
		{
			if (!string.IsNullOrWhiteSpace(child.MParentID) && !(child.MParentID == "0"))
			{
				BDAccountEditModel bDAccountEditModel = acctList.FirstOrDefault((BDAccountEditModel f) => f.MItemID == child.MParentID);
				bool flag = bDAccountEditModel != null;
				if (bDAccountEditModel == null)
				{
					bDAccountEditModel = (from x in sysAcctList
					where x.MItemID == child.MParentID
					select x).FirstOrDefault();
				}
				if (bDAccountEditModel != null)
				{
					bDAccountEditModel.MIsCheckForCurrency = true;
					if (!flag)
					{
						bDAccountEditModel.IsUpdate = true;
						acctList.Add(bDAccountEditModel);
					}
					SetParentAcctForeignAccting(bDAccountEditModel, acctList, sysAcctList);
				}
			}
		}

		private string GetParentNo(string str)
		{
			if (str.Length == 4)
			{
				return string.Empty;
			}
			List<string> list = str.ToStandardAcctNumber(true, 2).Split('.').ToList();
			list.RemoveAt(list.Count() - 1);
			return string.Join(".", list);
		}

		private string FindExistParentAccountID(MContext ctx, string notExistParentID, List<BDAccountEditModel> list)
		{
			string empty = string.Empty;
			BDAccountEditModel bDAccountEditModel = list.FirstOrDefault((BDAccountEditModel f) => f.MItemID == notExistParentID);
			if (bDAccountEditModel != null)
			{
				if (string.IsNullOrWhiteSpace(bDAccountEditModel.MExistParentID))
				{
					return FindExistParentAccountID(ctx, bDAccountEditModel.MParentID, list);
				}
				return bDAccountEditModel.MExistParentID;
			}
			return empty;
		}

		private void ValidateAccountName(MContext ctx, List<BDAccountEditModel> acctList, OperationResult result, List<BDAccountEditModel> sysAcctEditList)
		{
			IEnumerable<IGrouping<string, BDAccountEditModel>> enumerable = from f in acctList
			where f.MatchResult != IOAccountMatchResultEnum.AutoUpdate && f.MatchResult != IOAccountMatchResultEnum.Matched
			group f by f.MParentID;
			Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
			List<string> list = new List<string>();
			List<BASLangModel> orgLangList = BASLangRepository.GetOrgLangList(ctx);
			if (orgLangList == null)
			{
				throw new NullReferenceException("Organization no exist language");
			}
			string languageFieldName = "MName";
			foreach (IGrouping<string, BDAccountEditModel> item in enumerable)
			{
				List<BDAccountEditModel> list2 = item.ToList();
				IEnumerable<BDAccountEditModel> source = from f in sysAcctEditList
				where f.MParentID == item.Key
				select f;
				foreach (BASLangModel item2 in orgLangList)
				{
					if (!(item2.LangID == LangCodeEnum.EN_US))
					{
						IEnumerable<string> source2 = from x in list2
						select x.GetMultiLanguageValue(item2.LangID, languageFieldName);
						IEnumerable<IGrouping<string, string>> source3 = from c in source2
						group c by c into g
						where g.Count() > 1
						select g;
						if (source3.Any())
						{
							string parentNo = GetParentNo(list2[0].MNumber);
							dictionary.Add(parentNo, (from g in source3
							select g.Key).ToList());
						}
						if (sysAcctEditList != null)
						{
							foreach (BDAccountEditModel item3 in list2)
							{
								if (item3.MatchResult == IOAccountMatchResultEnum.ManualMatch)
								{
									source = from f in source
									where f.MItemID != item3.MItemID
									select f;
								}
								if (source.Any((BDAccountEditModel f) => f.MultiLanguage.FirstOrDefault((MultiLanguageFieldList d) => d.MFieldName == languageFieldName).MMultiLanguageField.Any((MultiLanguageField s) => s.MLocaleID == item2.LangID && s.MValue == item3.MName)))
								{
									list.Add(item3.MName);
								}
							}
						}
					}
				}
			}
			if (dictionary.Any())
			{
				foreach (string key in dictionary.Keys)
				{
					string message = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "DuplicateAccountName", "同级科目名称不能重复!({0})"), string.Join("、", dictionary[key].Distinct())) + string.Format("({0}:{1})", COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "ParentAccount", "上级科目"), key);
					result.VerificationInfor.Add(new BizVerificationInfor
					{
						Level = AlertEnum.Error,
						Message = message,
						Id = string.Join("、", dictionary[key].Distinct())
					});
				}
			}
			if (list.Any())
			{
				string message2 = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "ExistSiblingAccountName", "导入的科目名称：{0} 不能与系统的同级科目同名!"), string.Join("、", list.Distinct()));
				result.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = message2
				});
			}
		}

		private void GetUpdateAccountListByMatch(MContext ctx, List<BDAccountEditModel> acctList, OperationResult result, ref bool isExistInitBalance, ref bool isExistVoucher, List<BDAccountEditModel> sysAcctEditList = null, List<BDAccountEditModel> updateAcctList = null, bool isFromMigration = false)
		{
			if (sysAcctEditList == null)
			{
				sysAcctEditList = ModelInfoManager.GetDataModelList<BDAccountEditModel>(ctx, new SqlWhere().Equal("MIsDelete", 0).Equal("MIsActive", 1).NotLike("MNumber", "1001")
					.NotLike("MNumber", "1002"), false, false);
				if (!isFromMigration)
				{
					sysAcctEditList = (from f in sysAcctEditList
					where !bankNumberPrefix.Any((string n) => f.MNumber.StartsWith(n))
					select f).ToList();
				}
			}
			List<GLCheckTypeModel> modelList = _checkType.GetModelList(ctx, new SqlWhere(), false);
			List<GLCheckGroupModel> modelList2 = _checkGroup.GetModelList(ctx, new SqlWhere(), false);
			ValidateMatchResult(ctx, acctList, result, sysAcctEditList, isFromMigration);
			if (result.Success)
			{
				try
				{
					List<BDAccountEditModel> list = allAutoUpdateAcctList = (from f in acctList
					where !string.IsNullOrWhiteSpace(f.MMatchNumber) && f.MMatchNumber != f.MNumber
					select f).ToList();
					SetAccountInfoByMatchResult(ctx, sysAcctEditList, acctList, null, null, null, modelList, modelList2, updateAcctList, isFromMigration);
					if (updateAcctList != null)
					{
						MoveCheckGroupToLeafAcct(ctx, acctList, updateAcctList, sysAcctEditList, modelList);
						IEnumerable<BDAccountEditModel> enumerable = from f in acctList
						where string.IsNullOrWhiteSpace(f.MCheckGroupID)
						select f;
						foreach (BDAccountEditModel item in enumerable)
						{
							item.MCheckGroupID = "0";
						}
					}
					ValidateAccountName(ctx, acctList, result, sysAcctEditList);
					IEnumerable<string> matchNumList = from f in acctList
					where !string.IsNullOrWhiteSpace(f.MMatchNumber)
					select f.MMatchNumber;
					List<string> list2 = (from f in sysAcctEditList
					where matchNumList.Contains(f.MNumber)
					select f.MItemID).ToList();
					if (list2.Any())
					{
						SqlWhere filter = new SqlWhere().In("MAccountID", list2);
						GLInitBalanceRepository gLInitBalanceRepository = new GLInitBalanceRepository();
						isExistInitBalance = gLInitBalanceRepository.ExistsByFilter(ctx, filter);
						isExistVoucher = ModelInfoManager.ExistsByFilter<GLVoucherEntryModel>(ctx, filter);
					}
					if ((isExistInitBalance | isExistVoucher) && acctList.Any((BDAccountEditModel f) => f.IsUpdate))
					{
						string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "InitBalanceOverCannotUpdateAccountByImport", "初始化已完成，已有的会计科目不能再次导入，如需修改，请在会计科目表里手动操作！");
						result.VerificationInfor.Add(new BizVerificationInfor
						{
							Level = AlertEnum.Error,
							Message = text
						});
					}
					foreach (BDAccountEditModel acct in acctList)
					{
						int rowIndex = 0;
						int.TryParse(ModelHelper.TryGetModelValue(acct, "MRowIndex"), out rowIndex);
						if (acct.MDC != 1 && acct.MDC != -1)
						{
							result.Success = false;
							string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "DcIllegal", "借贷方向的值不合法！");
							result.VerificationInfor.Add(new BizVerificationInfor
							{
								Level = AlertEnum.Error,
								Message = text2,
								Id = acct.MNumber,
								CheckItem = "RowIndex",
								RowIndex = rowIndex
							});
						}
						if (string.IsNullOrWhiteSpace(acct.MName))
						{
							result.Success = false;
							string text3 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "AccountNameIsNull", "科目名称为空！");
							result.VerificationInfor.Add(new BizVerificationInfor
							{
								Level = AlertEnum.Error,
								Message = text3,
								Id = acct.MNumber,
								CheckItem = "RowIndex",
								RowIndex = rowIndex
							});
						}
						GLCheckGroupModel mCheckGroupModel = acct.MCheckGroupModel;
						if (mCheckGroupModel != null)
						{
							int[] source = new int[10]
							{
								mCheckGroupModel.MContactID,
								mCheckGroupModel.MEmployeeID,
								mCheckGroupModel.MMerItemID,
								mCheckGroupModel.MExpItemID,
								mCheckGroupModel.MPaItemID,
								mCheckGroupModel.MTrackItem1,
								mCheckGroupModel.MTrackItem2,
								mCheckGroupModel.MTrackItem3,
								mCheckGroupModel.MTrackItem4,
								mCheckGroupModel.MTrackItem5
							};
							if (source.Any((int m) => m > 2 || m < 0))
							{
								result.Success = false;
								string text4 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "CheckGroupError", "核算维度录入数值有误！");
								result.VerificationInfor.Add(new BizVerificationInfor
								{
									Level = AlertEnum.Error,
									Message = text4,
									Id = acct.MNumber,
									CheckItem = "RowIndex",
									RowIndex = rowIndex
								});
							}
						}
					}
				}
				catch (Exception ex)
				{
					result.VerificationInfor.Add(new BizVerificationInfor
					{
						Level = AlertEnum.Error,
						Message = ex.Message
					});
				}
			}
		}

		private void SetMatchAccoutInfo(List<BDAccountEditModel> sysAcctEditList, BDAccountEditModel model)
		{
			if (!string.IsNullOrWhiteSpace(model.MMatchNumber))
			{
				model.MNumber = model.MMatchNumber;
				model.MParentNumber = GetParentNo(model.MMatchNumber);
				string text = string.Empty;
				BDAccountEditModel bDAccountEditModel = sysAcctEditList.FirstOrDefault((BDAccountEditModel f) => f.MNumber == model.MMatchNumber);
				if (bDAccountEditModel != null)
				{
					List<MultiLanguageFieldList> list = (from f in bDAccountEditModel.MultiLanguage
					where f.MFieldName == "MName"
					select f).ToList();
					if (model.MultiLanguage.Any())
					{
						List<MultiLanguageField> mMultiLanguageField = model.MultiLanguage.FirstOrDefault((MultiLanguageFieldList f) => f.MFieldName == "MName").MMultiLanguageField;
						MultiLanguageField multiLanguageField = mMultiLanguageField.FirstOrDefault((MultiLanguageField f) => f.MLocaleID == "0x0009");
						if (multiLanguageField != null)
						{
							string mValue = mMultiLanguageField.FirstOrDefault((MultiLanguageField f) => f.MLocaleID == "0x0009").MValue;
							model.MultiLanguage = list;
							string pattern = "[一-龥]+";
							if (!string.IsNullOrWhiteSpace(mValue) && !Regex.IsMatch(mValue, pattern))
							{
								model.MultiLanguage.FirstOrDefault().MMultiLanguageField.FirstOrDefault((MultiLanguageField f) => f.MLocaleID == "0x0009").MValue = mValue;
							}
						}
						text = list.FirstOrDefault().MMultiLanguageValue;
					}
					else if (list.Any())
					{
						model.MultiLanguage = list;
					}
					if (bDAccountEditModel.MIsSys)
					{
						model.MDC = bDAccountEditModel.MDC;
					}
				}
				if (!string.IsNullOrWhiteSpace(text))
				{
					model.MOriName = model.MName;
					model.MName = text;
				}
			}
		}

		private void ValidateMatchResult(MContext ctx, List<BDAccountEditModel> list, OperationResult result, List<BDAccountEditModel> sysAcctList, bool isFromMigration = false)
		{
			List<BDAccountEditModel> source = (from f in list
			where !bankNumberPrefix.Any((string b) => f.MNumber.StartsWith(b))
			select f).ToList();
			IEnumerable<BDAccountEditModel> source2 = from f in source
			where string.IsNullOrWhiteSpace(f.MMatchNumber) && string.IsNullOrWhiteSpace(f.MNewNumber) && f.MatchResult == IOAccountMatchResultEnum.ManualMatch
			select f;
			if (source2.Any())
			{
				string message = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "UnCompleteMatch", "科目（{0}）没完成匹配！"), string.Join("、", from f in source2
				select f.MNumber));
				result.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = message,
					Id = string.Join("、", from f in source2
					select f.MNumber)
				});
			}
			IEnumerable<BDAccountEditModel> source3 = from f in source
			where !string.IsNullOrWhiteSpace(f.MNewNumber) && sysAcctList.Any((BDAccountEditModel s) => s.MNumber == f.MNewNumber)
			select f;
			if (source3.Any())
			{
				string message2 = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "NewNumberExist", "新增的科目（{0}）已经存在，请使用一个新的代码！"), string.Join("、", (from f in source3
				select f.MNewNumber).Distinct()));
				result.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = message2,
					Id = string.Join("、", (from f in source3
					select f.MNewNumber).Distinct())
				});
			}
			else
			{
				IEnumerable<BDAccountEditModel> source4 = from f in source
				where f.MatchResult == IOAccountMatchResultEnum.ManualMatch && !string.IsNullOrWhiteSpace(f.MNumber) && !string.IsNullOrWhiteSpace(f.MNewNumber) && f.MNumber.Length == 4 && f.MNewNumber.Length == 4
				select f;
				if (source4.Any())
				{
					string message3 = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "FirstLayerAccountCannotNew", "一级科目不能新增（{0}）！"), string.Join("、", from f in source4
					select f.MNewNumber));
					result.VerificationInfor.Add(new BizVerificationInfor
					{
						Level = AlertEnum.Error,
						Message = message3,
						Id = string.Join("、", from f in source4
						select f.MNewNumber)
					});
				}
			}
			IEnumerable<BDAccountEditModel> enumerable = from f in source
			where f.MatchResult == IOAccountMatchResultEnum.ManualMatch && !string.IsNullOrWhiteSpace(f.MNewNumber)
			select f;
			IEnumerable<string> source5 = from f in enumerable
			select f.MNewNumber;
			List<string> list2 = (from c in source5
			group c by c into g
			where g.Count() > 1
			select g into f
			select f.Key).ToList();
			if (list2.Any())
			{
				string message4 = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "DuplicateNewAcct", "新增的科目（{0}）不能重复！"), string.Join("、", list2));
				result.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = message4,
					Id = string.Join("、", list2)
				});
			}
			IEnumerable<string> source6 = from f in source
			where f.MatchResult == IOAccountMatchResultEnum.ManualMatch && !string.IsNullOrWhiteSpace(f.MMatchNumber)
			select f.MMatchNumber;
			List<string> list3 = (from c in source6
			group c by c into g
			where g.Count() > 1
			select g into f
			select f.Key).ToList();
			if (list3.Any())
			{
				string message5 = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "DuplicateMatch", "手动匹配的科目（{0}）不能重复！"), string.Join("、", list3));
				result.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = message5,
					Id = string.Join("、", list3)
				});
			}
			IEnumerable<BDAccountEditModel> enumerable2 = from f in source
			where f.MatchResult == IOAccountMatchResultEnum.AutoAdd
			select f;
			if (enumerable2.Any())
			{
				List<string> list4 = new List<string>();
				IEnumerable<IGrouping<string, BDAccountEditModel>> enumerable3 = from f in enumerable.Concat(enumerable2)
				group f by f.MNumber;
				foreach (IGrouping<string, BDAccountEditModel> item in enumerable3)
				{
					list4.Add(item.ToList()[0].MNewNumber);
				}
				List<string> list5 = (from c in list4
				group c by c into g
				where g.Count() > 1
				select g into f
				select f.Key).ToList();
				if (list5.Any())
				{
					string message6 = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "DuplicateNewMatchWithAuto", "新增的科目代码（{0}）不能与自动新增的科目代码重复！"), string.Join("、", list5));
					result.VerificationInfor.Add(new BizVerificationInfor
					{
						Level = AlertEnum.Error,
						Message = message6,
						Id = string.Join("、", list5)
					});
				}
			}
			if (isFromMigration)
			{
				List<string> list6 = new List<string>();
				foreach (BDAccountEditModel item2 in list)
				{
					if (!string.IsNullOrWhiteSpace(item2.MMatchNumber))
					{
						bool flag = list.Any((BDAccountEditModel f) => f.MParentNumber == item2.MNumber);
						BDAccountEditModel sysMatchAcct = sysAcctList.FirstOrDefault((BDAccountEditModel f) => f.MNumber == item2.MMatchNumber);
						bool flag2 = sysAcctList.Any((BDAccountEditModel f) => f.MParentID == sysMatchAcct.MItemID);
						if (!flag & flag2)
						{
							list6.Add(item2.MMatchNumber);
						}
					}
				}
				if (list6.Any())
				{
					string text = string.Join("、", list6.Distinct());
					string message7 = $"匹配的科目（{text}）包含子科目，请选择子科目！";
					result.VerificationInfor.Add(new BizVerificationInfor
					{
						Level = AlertEnum.Error,
						Message = message7,
						Id = text
					});
				}
			}
		}

		private void UpdateAccountRelation(List<BDAccountEditModel> allAcctList, List<BDAccountEditModel> parentAcctList = null, string parentNumber = null)
		{
			parentAcctList = (parentAcctList ?? allAcctList);
			foreach (BDAccountEditModel parentAcct in parentAcctList)
			{
				if (!parentAcct.IsUpdate)
				{
					if (!string.IsNullOrWhiteSpace(parentNumber))
					{
						parentAcct.MParentNumber = parentNumber;
						parentAcct.MOriParentNumber = parentNumber;
						parentAcct.IsUpdate = true;
					}
					List<BDAccountEditModel> list = (from f in allAcctList
					where f.MNumber != parentAcct.MNumber && f.MNumber.Split('.').Length == parentAcct.MNumber.Split('.').Length + 1 && f.MNumber.StartsWith(parentAcct.MNumber)
					select f).ToList();
					if (list != null && list.Any())
					{
						UpdateAccountRelation(allAcctList, list, parentAcct.MNumber);
					}
				}
			}
		}

		private Dictionary<int, int> GetImportAcctNumberFmt(List<BDAccountEditModel> acctList)
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			dictionary[0] = 4;
			foreach (BDAccountEditModel acct in acctList)
			{
				string[] array = acct.MNumber.ToStandardAcctNumber(true, 2).Split('.');
				for (int i = 1; i < array.Length; i++)
				{
					if (!dictionary.ContainsKey(i))
					{
						dictionary[i] = array[i].Length;
					}
					else
					{
						dictionary[i] = Math.Max(dictionary[i], array[i].Length);
					}
				}
			}
			return dictionary;
		}

		private string GetStandardAcctNumber(string number, Dictionary<int, int> fmt)
		{
			string[] array = number.Split('.');
			if (array.Count() > 1)
			{
				for (int i = 1; i < array.Length; i++)
				{
					array[i] = array[i].PadLeft(fmt[i], '0');
				}
			}
			return string.Join("", array);
		}

		private void ConvertToAccountTreeModel(List<IOAccountModel> resultList, List<BDAccountListModel> sysAcctList, List<BDAccountEditModel> acctList, List<BDAccountEditModel> parentAcctList = null, IOAccountModel parentIOAcct = null, bool isFromMigration = false)
		{
			parentAcctList = (parentAcctList ?? acctList);
			int num = 0;
			foreach (BDAccountEditModel parentAcct2 in parentAcctList)
			{
				if (!parentAcct2.IsUpdate)
				{
					parentAcct2.IsUpdate = true;
					BDAccountEditModel parentAcct = acctList.FirstOrDefault((BDAccountEditModel f) => f.MNumber == parentAcct2.MParentNumber);
					if (parentAcct2.MatchResult == IOAccountMatchResultEnum.None)
					{
						parentAcct2.MatchResult = IOAccountMatchResultEnum.ManualMatch;
						if (parentAcct != null)
						{
							IOAccountMatchResultEnum matchResult = parentAcct.MatchResult;
							if (matchResult == IOAccountMatchResultEnum.Matched)
							{
								IEnumerable<BDAccountListModel> source = from f in sysAcctList
								where f.MParentID == parentAcct.MItemID
								select f;
								if (!source.Any() && !bankNumberPrefix.Any((string f) => parentAcct2.MNumber.StartsWith(f)))
								{
									parentAcct2.MatchResult = IOAccountMatchResultEnum.AutoAdd;
								}
							}
							else if (parentAcct.MatchResult != 0)
							{
								parentAcct2.MatchResult = parentAcct.MatchResult;
							}
							if (parentAcct2.MatchResult == IOAccountMatchResultEnum.AutoAdd)
							{
								string[] array = parentAcct2.MNumber.Split('.');
								array[array.Length - 1] = (num + 1).ToString().PadLeft(2, '0');
								parentAcct2.MNewNumber = string.Join(".", array);
								if (!string.IsNullOrWhiteSpace(parentAcct.MNewNumber))
								{
									parentAcct2.MNewNumber = parentAcct2.MNewNumber.Replace(parentAcct.MNumber, parentAcct.MNewNumber);
								}
								if (!string.IsNullOrWhiteSpace(parentAcct2.MMatchNumber))
								{
									parentAcct2.MMatchNumber = string.Empty;
								}
							}
							parentAcct2.MCheckGroupID = parentAcct.MCheckGroupID;
							CopyAcctBasicInfo(parentAcct, parentAcct2);
						}
					}
					PresetNewNumber(sysAcctList, acctList, parentAcct2, parentAcct, isFromMigration);
					IOAccountModel iOAccountModel = GetIOAccountModel(parentAcct2);
					if (parentIOAcct == null)
					{
						resultList.Add(iOAccountModel);
					}
					else
					{
						if (parentIOAcct.children == null)
						{
							parentIOAcct.children = new List<IOAccountModel>();
						}
						parentIOAcct.children.Add(iOAccountModel);
					}
					num++;
					List<BDAccountEditModel> list = (from f in acctList
					where f.MParentNumber == parentAcct2.MNumber
					select f).ToList();
					if (list != null && list.Any())
					{
						ConvertToAccountTreeModel(resultList, sysAcctList, acctList, list, iOAccountModel, isFromMigration);
					}
				}
			}
		}

		private void PresetNewNumber(List<BDAccountListModel> sysAcctList, List<BDAccountEditModel> acctList, BDAccountEditModel acct, BDAccountEditModel parentAcct, bool isFromMigration = false)
		{
			if (!bankNumberPrefix.Any((string e) => acct.MNumber.StartsWith(e)) && acct.MatchResult == IOAccountMatchResultEnum.ManualMatch && acct.MNumber.Length > 4 && string.IsNullOrWhiteSpace(acct.MMatchNumber))
			{
				bool flag = false;
				if (parentAcct == null)
				{
					BDAccountListModel sysParentAcct = sysAcctList.FirstOrDefault((BDAccountListModel f) => f.MNumber == GetParentNo(acct.MNumber));
					if (sysParentAcct != null)
					{
						IEnumerable<BDAccountListModel> source = from f in sysAcctList
						where f.MParentID == sysParentAcct.MItemID
						select f;
						if (!source.Any())
						{
							if (string.IsNullOrWhiteSpace(acct.MNewNumberSaved))
							{
								acct.MNewNumber = acct.MNumber;
							}
							flag = true;
						}
					}
				}
				else if (!string.IsNullOrWhiteSpace(parentAcct.MMatchNumber) && string.IsNullOrWhiteSpace(acct.MNewNumber))
				{
					string pNumber = isFromMigration ? parentAcct.MNumber : parentAcct.MMatchNumber;
					IEnumerable<BDAccountListModel> enumerable = from f in sysAcctList
					where GetParentNo(f.MNumber) == pNumber
					select f;
					if (enumerable?.Any() ?? false)
					{
						IEnumerable<BDAccountEditModel> source2 = from f in acctList
						where GetParentNo(f.MNumber) == parentAcct.MNumber && !string.IsNullOrWhiteSpace(f.MNewNumber)
						select f;
						List<string> numberList = (from f in enumerable
						select f.MNumber).Union(from f in source2
						select f.MNewNumber).ToList();
						bool flag2 = true;
						int num = GetMaxSubNo(numberList, parentAcct.MNumber, ref flag2);
						if (string.IsNullOrWhiteSpace(acct.MNewNumberSaved))
						{
							if (flag2)
							{
								num++;
							}
							acct.MNewNumber = parentAcct.MMatchNumber + "." + num.ToString().PadLeft(2, '0');
						}
					}
					else
					{
						acct.MNewNumber = FilterAccountNumber(acct.MNumber).Replace(parentAcct.MNumber, parentAcct.MMatchNumber);
						acct.MMatchNumber = string.Empty;
					}
					flag = true;
				}
				if (flag)
				{
					IEnumerable<BDAccountEditModel> enumerable2 = from f in acctList
					where f.MNumber.StartsWith(acct.MNumber) && f.MNumber.Split('.').Length > acct.MNumber.Split('.').Length && string.IsNullOrWhiteSpace(f.MNewNumberSaved)
					select f;
					foreach (BDAccountEditModel item in enumerable2)
					{
						item.MNewNumber = item.MNumber.Replace(acct.MNumber, acct.MNewNumber);
					}
				}
			}
		}

		private string FilterAccountNumber(string number)
		{
			List<string> list = new List<string>();
			string[] array = number.Split('.');
			string[] array2 = array;
			foreach (string input in array2)
			{
				list.Add(Regex.Replace(input, "[^\\d]", ""));
			}
			return string.Join(".", list);
		}

		private int GetMaxSubNo(List<string> numberList, string parentNumber, ref bool isUsed)
		{
			if (!numberList.Any())
			{
				return 0;
			}
			List<int> list = new List<int>();
			int num = parentNumber.Split('.').Length;
			foreach (string number in numberList)
			{
				string[] array = number.Split('.');
				if (num < array.Length)
				{
					list.Add(Convert.ToInt32(array[num]));
				}
			}
			int num2 = 0;
			if (list.Any())
			{
				num2 = list.Max();
			}
			int num3 = numberList.Count();
			if (num2 > num3)
			{
				int num4 = 1;
				while (num4 < num2)
				{
					if (list.Contains(num4))
					{
						num4++;
						continue;
					}
					num2 = num4;
					isUsed = false;
					break;
				}
			}
			return num2;
		}

		private IOAccountModel GetIOAccountModel(BDAccountEditModel acct)
		{
			return new IOAccountModel
			{
				id = acct.MItemID,
				text = acct.MName,
				MNumber = acct.MNumber,
				MIsCheckForCurrency = acct.MIsCheckForCurrency,
				MAccountTypeID = acct.MAccountTypeID,
				MDC = acct.MDC,
				MatchResult = acct.MatchResult,
				MMatchNumber = acct.MMatchNumber,
				MNewNumber = acct.MNewNumber,
				MCheckGroupNames = acct.MCheckGroupNames
			};
		}

		private void SetAccountTypeId(BDAccountEditModel model, List<BDAccountEditModel> sysAcctList)
		{
			if (string.IsNullOrWhiteSpace(model.MAccountTypeID))
			{
				string firstNo = model.MNumber.Split('.')[0];
				BDAccountEditModel bDAccountEditModel = sysAcctList.FirstOrDefault((BDAccountEditModel f) => f.MNumber == firstNo);
				if (bDAccountEditModel != null)
				{
					model.MAccountTypeID = bDAccountEditModel.MAccountTypeID;
				}
			}
		}

		private void CombineCheckGroupNames(BDAccountEditModel model, string parentCheckGroupNames)
		{
			if (!string.IsNullOrWhiteSpace(parentCheckGroupNames))
			{
				if (string.IsNullOrWhiteSpace(model.MCheckGroupNames))
				{
					model.MCheckGroupNames = parentCheckGroupNames;
				}
				else
				{
					string[] first = parentCheckGroupNames.Split('/');
					string[] second = model.MCheckGroupNames.Split('/');
					model.MCheckGroupNames = string.Join("/", first.Union(second));
				}
			}
		}

		private void InheritParentCheckGroup(BDAccountEditModel model, BDAccountEditModel parentAcct, List<BDAccountEditModel> updateAcctList = null)
		{
			SetCurrenctAcctFlag(model, false);
			if (parentAcct != null)
			{
				model.CurrentAcctNumber = parentAcct.CurrentAcctNumber;
				string mCheckGroupID = parentAcct.MCheckGroupID;
				if (mCheckGroupID != "0")
				{
					model.MCheckGroupID = parentAcct.MCheckGroupID;
				}
				else if (requiredCurrentAcctList.Contains(parentAcct.CurrentAcctNumber))
				{
					model.MCheckGroupID = "1";
				}
				else
				{
					model.MCheckGroupID = "0";
				}
				parentAcct.MCheckGroupID = "0";
				parentAcct.MCheckGroupModel = null;
				CombineCheckGroupNames(model, parentAcct.MCheckGroupNames);
				parentAcct.MCheckGroupNames = string.Empty;
				if (mCheckGroupID != "0" && updateAcctList != null && parentAcct.IsUpdate)
				{
					updateAcctList.Add(parentAcct);
				}
			}
		}

		private void SetCurrenctAcctFlag(BDAccountEditModel model, bool isSysAcct = false)
		{
			if (isSysAcct && allCurrentAcctList.Any((string v) => model.MNumber.StartsWith(v)))
			{
				model.CurrentAcctNumber = model.MNumber.Split('.')[0];
			}
			else if (!string.IsNullOrWhiteSpace(model.MMatchNumber) && allCurrentAcctList.Any((string v) => model.MMatchNumber.StartsWith(v)))
			{
				model.CurrentAcctNumber = model.MMatchNumber.Split('.')[0];
			}
		}

		private void SetAccountFullName(MContext ctx, BDAccountEditModel model, BDAccountEditModel parent, List<BDAccountEditModel> sysAcctList)
		{
			string text = string.Empty;
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
			string parentNumber = GetParentNo(model.MNumber);
			parent = sysAcctList.FirstOrDefault((BDAccountEditModel f) => f.MNumber == parentNumber);
			if (parent == null && updateAcctFullNameList.ContainsKey(parentNumber))
			{
				dictionary2 = updateAcctFullNameList[parentNumber];
			}
			SysLangList = ((SysLangList == null) ? BASLangRepository.GetOrgLangList(ctx) : SysLangList);
			foreach (BASLangModel sysLang in SysLangList)
			{
				string multiLanguageValue = model.GetMultiLanguageValue(sysLang.LangID, "MName");
				if (parent != null)
				{
					text = parent.GetMultiLanguageValue(sysLang.LangID, "MFullName");
				}
				else if (dictionary2.Any())
				{
					text = dictionary2[sysLang.LangID];
				}
				if (!string.IsNullOrWhiteSpace(text))
				{
					text = text.Replace(parentNumber + " ", "");
					dictionary.Add(sysLang.LangID, $"{model.MNumber} {text}-{multiLanguageValue}");
				}
				else
				{
					dictionary.Add(sysLang.LangID, $"{model.MNumber} {multiLanguageValue}");
				}
			}
			if (dictionary.Any())
			{
				if (!updateAcctFullNameList.ContainsKey(model.MNumber))
				{
					updateAcctFullNameList.Add(model.MNumber, dictionary);
				}
				updateAcctFullNameCmdList.AddRange(BDAccountRepository.GetUpdateFullNameCmds(ctx, dictionary, model));
			}
		}

		private List<BDAccountModel> GetBDModelList(MContext ctx)
		{
			List<BDAccountModel> list = new List<BDAccountModel>();
			list = dal.GetBaseBDAccountList(ctx, null, true, null);
			List<MultiLanguageFieldList> accountListMultiLanguageList = dal.GetAccountListMultiLanguageList(ctx);
			foreach (BDAccountModel item in list)
			{
				List<MultiLanguageFieldList> list2 = (from x in accountListMultiLanguageList
				where x.MParentID == item.MItemID
				select x).ToList();
				if (list2 == null || list2.Count() == 0)
				{
					throw new NullReferenceException("not find multilanguage info");
				}
				item.MultiLanguage = list2;
			}
			return list;
		}

		private void SetAccountInfoByMatchResult(MContext ctx, List<BDAccountEditModel> sysAcctList, List<BDAccountEditModel> acctList, List<BDAccountEditModel> subParentAcctList = null, BDAccountEditModel parentAcct = null, int? subParentIdx = default(int?), List<GLCheckTypeModel> checkTypeList = null, List<GLCheckGroupModel> checkGroupList = null, List<BDAccountEditModel> updateAcctList = null, bool isFromMigration = false)
		{
			subParentAcctList = (subParentAcctList ?? (from f in acctList
			where string.IsNullOrWhiteSpace(f.MParentNumber) || !acctList.Any((BDAccountEditModel t) => t.MNumber == f.MParentNumber)
			select f).ToList());
			int num = (!subParentIdx.HasValue) ? 1 : subParentIdx.Value;
			string empty = string.Empty;
			foreach (BDAccountEditModel subParentAcct in subParentAcctList)
			{
				string oriMNumber = subParentAcct.MNumber;
				switch (subParentAcct.MatchResult)
				{
				case IOAccountMatchResultEnum.Matched:
					subParentAcct.IsUpdate = true;
					SetMatchAccoutInfo(sysAcctList, subParentAcct);
					InheritParentCheckGroup(subParentAcct, parentAcct, updateAcctList);
					break;
				case IOAccountMatchResultEnum.AutoAdd:
					subParentAcct.MItemID = UUIDHelper.GetGuid();
					subParentAcct.IsNew = true;
					if (!string.IsNullOrWhiteSpace(subParentAcct.MNewNumber))
					{
						subParentAcct.MNumber = subParentAcct.MNewNumber;
					}
					if (parentAcct != null)
					{
						if (!string.IsNullOrWhiteSpace(parentAcct.MNewNumber))
						{
							subParentAcct.MParentNumber = parentAcct.MNumber;
						}
						subParentAcct.MParentID = parentAcct.MItemID;
						InheritParentCheckGroup(subParentAcct, parentAcct, updateAcctList);
						string[] array = subParentAcct.MNumber.Split('.');
						subParentAcct.MCode = parentAcct.MCode + array[array.Length - 1];
					}
					break;
				case IOAccountMatchResultEnum.ManualMatch:
				{
					if (!string.IsNullOrWhiteSpace(subParentAcct.MMatchNumber))
					{
						BDAccountEditModel bDAccountEditModel = sysAcctList.FirstOrDefault((BDAccountEditModel f) => f.MNumber == subParentAcct.MMatchNumber);
						if (bDAccountEditModel != null)
						{
							subParentAcct.MItemID = bDAccountEditModel.MItemID;
							subParentAcct.MParentID = bDAccountEditModel.MParentID;
							SetMatchAccoutInfo(sysAcctList, subParentAcct);
							subParentAcct.MCode = bDAccountEditModel.MCode;
							subParentAcct.MIsSys = bDAccountEditModel.MIsSys;
							subParentAcct.MCheckGroupID = bDAccountEditModel.MCheckGroupID;
							SetCurrenctAcctFlag(subParentAcct, false);
							CopyAcctBasicInfo(bDAccountEditModel, subParentAcct);
							subParentAcct.IsUpdate = true;
							break;
						}
						throw new Exception(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "MatchAccountNotFind", "找不到匹配的科目:{0},请检查科目是否删除或者禁用！"), subParentAcct.MMatchNumber));
					}
					if (string.IsNullOrWhiteSpace(subParentAcct.MNewNumber))
					{
						break;
					}
					subParentAcct.MNumber = subParentAcct.MNewNumber;
					subParentAcct.MItemID = UUIDHelper.GetGuid();
					subParentAcct.IsNew = true;
					List<BDAccountEditModel> list = null;
					int num2;
					if (parentAcct != null && (string.IsNullOrWhiteSpace(parentAcct.MMatchNumber) || subParentAcct.MNumber.StartsWith(parentAcct.MMatchNumber)))
					{
						BDAccountEditModel bDAccountEditModel2 = null;
						if (subParentAcct.MNumber.Split('.').Length - 1 != parentAcct.MNumber.Split('.').Length)
						{
							bDAccountEditModel2 = parentAcct;
							string parentNum = GetParentNo(subParentAcct.MNumber);
							parentAcct = sysAcctList.FirstOrDefault((BDAccountEditModel f) => f.MNumber == parentNum);
							if (parentAcct == null)
							{
								parentAcct = allAutoUpdateAcctList.FirstOrDefault((BDAccountEditModel f) => f.MNumber == parentNum);
							}
							else
							{
								parentAcct.IsUpdate = true;
							}
							if (parentAcct == null)
							{
								throw new Exception(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "CanNotFoundParentAccount", "系统无法找到新增科目代码（{0}）的上级！"), subParentAcct.MNewNumber));
							}
							empty = parentAcct.MCheckGroupID;
						}
						InheritParentCheckGroup(subParentAcct, parentAcct, updateAcctList);
						subParentAcct.MParentID = parentAcct.MItemID;
						subParentAcct.MParentNumber = parentAcct.MNumber;
						CopyAcctBasicInfo(parentAcct, subParentAcct);
						list = (from f in acctList
						where f.MParentNumber == subParentAcct.MParentNumber
						select f).ToList();
						if (!string.IsNullOrWhiteSpace(parentAcct.MMatchNumber))
						{
							int childAcctMaxSubCode = GetChildAcctMaxSubCode(parentAcct.MItemID, sysAcctList, list);
							BDAccountEditModel bDAccountEditModel3 = subParentAcct;
							string mCode = parentAcct.MCode;
							num2 = Math.Max(childAcctMaxSubCode + 1, num);
							bDAccountEditModel3.MCode = mCode + num2.ToString().PadLeft(2, '0');
						}
						else
						{
							subParentAcct.MCode = parentAcct.MCode + num.ToString().PadLeft(2, '0');
						}
						if (!string.IsNullOrWhiteSpace(parentAcct.MNewNumber) && !subParentAcct.MNewNumber.StartsWith(parentAcct.MNewNumber))
						{
							throw new Exception(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "NewAcctNumberInconsistentWithParent", "新增的科目代码：{0}与上级不一致！"), subParentAcct.MNumber));
						}
						if (bDAccountEditModel2 != null)
						{
							parentAcct = bDAccountEditModel2;
						}
						break;
					}
					string newNumParentNo = GetParentNo(subParentAcct.MNewNumber);
					BDAccountEditModel bDAccountEditModel4 = sysAcctList.FirstOrDefault((BDAccountEditModel f) => f.MNumber == newNumParentNo);
					if (bDAccountEditModel4 == null)
					{
						bDAccountEditModel4 = allAutoUpdateAcctList.FirstOrDefault((BDAccountEditModel f) => f.MNumber == newNumParentNo);
					}
					else
					{
						SetCurrenctAcctFlag(bDAccountEditModel4, true);
					}
					if (bDAccountEditModel4 != null)
					{
						bDAccountEditModel4.IsUpdate = true;
						InheritParentCheckGroup(subParentAcct, bDAccountEditModel4, updateAcctList);
						subParentAcct.MParentID = bDAccountEditModel4.MItemID;
						subParentAcct.MParentNumber = bDAccountEditModel4.MNumber;
						list = (from f in acctList
						where f.MParentNumber == subParentAcct.MParentNumber
						select f).ToList();
						int childAcctMaxSubCode2 = GetChildAcctMaxSubCode(bDAccountEditModel4.MItemID, sysAcctList, list);
						BDAccountEditModel bDAccountEditModel5 = subParentAcct;
						string mCode2 = bDAccountEditModel4.MCode;
						num2 = Math.Max(childAcctMaxSubCode2 + 1, num);
						bDAccountEditModel5.MCode = mCode2 + num2.ToString().PadLeft(2, '0');
						CopyAcctBasicInfo(bDAccountEditModel4, subParentAcct);
						break;
					}
					throw new Exception(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "CanNotFoundParentAccount", "系统无法找到新增科目代码（{0}）的上级！"), subParentAcct.MNewNumber));
				}
				}
				SetAccountTypeId(subParentAcct, sysAcctList);
				SetAccountCheckGroupModel(ctx, subParentAcct, checkTypeList, checkGroupList, updateAcctList != null, isFromMigration, subParentAcct.MMatchNumber, subParentAcct.MNewNumber);
				SetAccountFullName(ctx, subParentAcct, parentAcct, sysAcctList);
				if (subParentIdx.HasValue)
				{
					num++;
				}
				List<BDAccountEditModel> list2 = (from f in acctList
				where f.MOriParentNumber == oriMNumber || (subParentAcct.MatchResult == IOAccountMatchResultEnum.AutoUpdate && f.MParentNumber == oriMNumber)
				select f).ToList();
				if (list2.Any())
				{
					SetAccountInfoByMatchResult(ctx, sysAcctList, acctList, list2, subParentAcct, 1, checkTypeList, checkGroupList, updateAcctList, isFromMigration);
				}
			}
		}

		private void CopyAcctBasicInfo(BDAccountEditModel ori, BDAccountEditModel target)
		{
			target.MAccountTypeID = ori.MAccountTypeID;
			target.MAccountGroupID = ori.MAccountGroupID;
			target.MAccountTableID = ori.MAccountTableID;
			target.IsCanRelateContact = ori.IsCanRelateContact;
		}

		private int GetChildAcctMaxSubCode(string parentId, List<BDAccountEditModel> sysAcctList, List<BDAccountEditModel> childAcctList)
		{
			IEnumerable<BDAccountEditModel> enumerable = (from f in sysAcctList
			where f.MParentID == parentId
			select f).Union(childAcctList);
			List<int> list = new List<int>
			{
				0
			};
			foreach (BDAccountEditModel item in enumerable)
			{
				if (item.MatchResult == IOAccountMatchResultEnum.AutoAdd)
				{
					string[] array = item.MNumber.ToStandardAcctNumber(true, 2).Split('.');
					list.Add(Convert.ToInt32(array[array.Length - 1]));
				}
				else if (!string.IsNullOrWhiteSpace(item.MCode))
				{
					string[] array2 = item.MCode.ToStandardAcctNumber(true, 2).Split('.');
					list.Add(Convert.ToInt32(array2[array2.Length - 1]));
				}
			}
			return list.Max();
		}

		public bool Exists(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.Exists(ctx, pkID, includeDelete);
		}

		public bool ExistsByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.ExistsByFilter(ctx, filter);
		}

		public OperationResult InsertOrUpdate(MContext ctx, BDAccountModel modelData, string fields = null)
		{
			return dal.InsertOrUpdate(ctx, modelData, fields);
		}

		public OperationResult InsertOrUpdateModels(MContext ctx, List<BDAccountModel> modelData, string fields = null)
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

		public BDAccountModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.GetDataModel(ctx, pkID, includeDelete);
		}

		public BDAccountModel GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.GetDataModelByFilter(ctx, filter);
		}

		public List<BDAccountModel> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelList(ctx, filter, includeDelete);
		}

		public DataGridJson<BDAccountModel> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelPageList(ctx, filter, includeDelete);
		}

		public List<BDAccountEditModel> GetAccountLists(MContext ctx, SqlWhere sqlWhere, bool isPage = false)
		{
			List<BDAccountEditModel> accountEditModelList = dal.GetAccountEditModelList(ctx, sqlWhere, isPage);
			List<GLCheckGroupModel> modelList = _checkGroup.GetModelList(ctx, new SqlWhere(), false);
			foreach (BDAccountEditModel item in accountEditModelList)
			{
				GLCheckGroupModel mCheckGroupModel = modelList.FirstOrDefault((GLCheckGroupModel m) => m.MItemID == item.MCheckGroupID);
				item.MCheckGroupModel = mCheckGroupModel;
			}
			return accountEditModelList;
		}

		public T ModelToModel<T, T1>(T t, T1 t1) where T : BaseModel, new()
		{
			PropertyInfo[] properties = typeof(T).GetProperties();
			PropertyInfo[] properties2 = typeof(T1).GetProperties();
			PropertyInfo[] array = properties2;
			foreach (PropertyInfo propertyInfo in array)
			{
				string infoName = propertyInfo.Name;
				PropertyInfo propertyInfo2 = properties.FirstOrDefault((PropertyInfo m) => m.Name == infoName);
				if (propertyInfo2 != (PropertyInfo)null && propertyInfo.CanRead)
				{
					object value = propertyInfo.GetValue(t1);
					if (propertyInfo2.CanWrite)
					{
						propertyInfo2.SetValue(t, value);
					}
				}
			}
			return t;
		}

		public List<T> ModelsToModels<T, T1>(List<T> ts, List<T1> t1s) where T : BaseModel, new()
		{
			PropertyInfo[] properties = typeof(T).GetProperties();
			PropertyInfo[] properties2 = typeof(T1).GetProperties();
			foreach (T1 t in t1s)
			{
				T val = new T();
				PropertyInfo[] array = properties2;
				foreach (PropertyInfo propertyInfo in array)
				{
					string infoName = propertyInfo.Name;
					PropertyInfo propertyInfo2 = properties.FirstOrDefault((PropertyInfo m) => m.Name == infoName);
					if (propertyInfo2 != (PropertyInfo)null && propertyInfo.CanRead)
					{
						object value = propertyInfo.GetValue(t);
						if (propertyInfo2.CanWrite)
						{
							propertyInfo2.SetValue(val, value);
						}
					}
				}
				ts.Add(val);
			}
			return ts;
		}
	}
}
