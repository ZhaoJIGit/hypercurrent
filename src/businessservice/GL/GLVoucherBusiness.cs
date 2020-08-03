using JieNor.Megi.BusinessContract.BAS;
using JieNor.Megi.BusinessContract.GL;
using JieNor.Megi.BusinessService.BAS;
using JieNor.Megi.BusinessService.BD;
using JieNor.Megi.BusinessService.FA;
using JieNor.Megi.BusinessService.REG;
using JieNor.Megi.BusinessService.SEC;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Helper;
using JieNor.Megi.Core.MResource;
using JieNor.Megi.Core.MultiLanguage;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.COM;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.IO;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.DataModel.MI;
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.DataRepository.API;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.DataRepository.Log.GlLog;
using JieNor.Megi.DataRepository.PA;
using JieNor.Megi.DataRepository.REG;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.BusinessService.GL
{
	public class GLVoucherBusiness : APIBusinessBase<GLVoucherModel>, IGLVoucherBusiness, IDataContract<GLVoucherModel>, IBasicBusiness<GLVoucherModel>
	{
		private readonly GLVoucherRepository _apiVoucherDal = new GLVoucherRepository();

		private readonly GLSettlementBusiness _settlementBiz = new GLSettlementBusiness();

		private List<GLVoucherModel> _dbList = new List<GLVoucherModel>();

		private List<REGCurrencyViewModel> _currencyDataPool;

		private List<BDAccountEditModel> _accountDataPool;

		private List<BDBankAccountModel> _bankAccountDataPool;

		private List<BDContactsInfoModel> _contactDataPool;

		private List<BDExpenseItemModel> _expenseItemDataPool;

		private List<BDItemModel> _itemDataPool;

		private List<BDEmployeesModel> _employeeDataPool;

		private List<PAPayItemModel> _payItemDataPool;

		private List<BDTrackModel> _trackDataPool;

		private List<GLCheckGroupModel> _checkGroupDataPool;

		private List<GLCheckGroupValueModel> _checkGroupValueDataPool;

		private List<PAPayItemGroupModel> _payItemGroupsDataPool;

		private DateTime _currentPeriod;

		private GLVoucherRepository newDal = new GLVoucherRepository();

		private GLCheckGroupBusiness checkGroupBiz = new GLCheckGroupBusiness();

		private GLCheckGroupValueBusiness checkGroupValueBiz = new GLCheckGroupValueBusiness();

		private GLSettlementBusiness settleBiz = new GLSettlementBusiness();

		private GLUtility utility = new GLUtility();

		private readonly GLVoucherRepository dal = new GLVoucherRepository();

		private readonly GLPeriodTransferBusiness transferBiz = new GLPeriodTransferBusiness();

		private readonly GLPeriodTransferRepository transferDal = new GLPeriodTransferRepository();

		private readonly GLVoucherEntryRepository entry = new GLVoucherEntryRepository();

		private readonly BDAccountRepository acctDal = new BDAccountRepository();

		private readonly BDContactsRepository contactDal = new BDContactsRepository();

		private readonly REGCurrencyRepository currencyDal = new REGCurrencyRepository();

		private readonly BDTrackRepository trackDal = new BDTrackRepository();

		private readonly BASLangBusiness lang = new BASLangBusiness();

		private readonly BDAccountBusiness accountBiz = new BDAccountBusiness();

		private readonly IBASLangBusiness langBiz = new BASLangBusiness();

		private readonly GLVoucherReferenceBusiness referenceBiz = new GLVoucherReferenceBusiness();

		private readonly GLBalanceBusiness balanceBiz = new GLBalanceBusiness();

		private readonly GLBalanceRepository balanceRep = new GLBalanceRepository();

		private readonly BDEmployeesRepository empDal = new BDEmployeesRepository();

		private readonly BDExchangeRateBusiness rateBiz = new BDExchangeRateBusiness();

		private readonly BDTrackBusiness trackBiz = new BDTrackBusiness();

		private readonly REGCurrencyBusiness currencyBiz = new REGCurrencyBusiness();

		private readonly BDItemRepository itemDal = new BDItemRepository();

		private readonly BDExpenseItemBusiness expenseItemBiz = new BDExpenseItemBusiness();

		private readonly PAPayItemGroupRepository payItemGroupDal = new PAPayItemGroupRepository();

		private readonly GLCheckGroupRepository checkGroupDal = new GLCheckGroupRepository();

		private readonly GLCheckTypeBusiness checkTypeBiz = new GLCheckTypeBusiness();

		private readonly BDContactsBusiness contactBiz = new BDContactsBusiness();

		private readonly BDEmployeesBusiness empBiz = new BDEmployeesBusiness();

		private readonly BDItemBusiness itemBiz = new BDItemBusiness();

		private readonly PAPayItemGroupBussiness paItemBiz = new PAPayItemGroupBussiness();

		private readonly BASOrganisationBusiness orgBiz = new BASOrganisationBusiness();

		protected override void OnGetBefore(MContext ctx, GetParam param)
		{
			APIDataPool instance = APIDataPool.GetInstance(ctx);
			_contactDataPool = instance.Contacts;
			_expenseItemDataPool = instance.ExpenseItems;
			_itemDataPool = instance.Items;
			_employeeDataPool = instance.Employees;
			_payItemDataPool = instance.PayItems;
			_payItemGroupsDataPool = instance.PayItemGroups;
			_trackDataPool = instance.TrackingCategories;
		}

		protected override DataGridJson<GLVoucherModel> OnGet(MContext ctx, GetParam param)
		{
			if (!param.IncludeDetail.HasValue)
			{
				param.IncludeDetail = true;
			}
			return _apiVoucherDal.Get(ctx, param);
		}

		protected override void OnGetAfter(MContext ctx, GetParam param, APIDataPool dataPool, GLVoucherModel model)
		{
			if (param.IncludeDetail.HasValue && !param.IncludeDetail.Value)
			{
				model.MVoucherEntrys = null;
			}
			else
			{
				model.MVoucherEntrys.ForEach(delegate(GLVoucherEntryModel y)
				{
					y.MAccountDimensions = APIDataRepository.ToApiDimension(ctx, y.MCheckGroupValueModel, _contactDataPool, _employeeDataPool, _expenseItemDataPool, _itemDataPool, _payItemDataPool, _payItemGroupsDataPool, _trackDataPool);
					y.MCheckGroupValueModel = null;
				});
				model.MVoucherEntrys = (from a in model.MVoucherEntrys
				orderby a.MEntrySeq
				select a).ToList();
			}
		}

		protected override void OnPostBefore(MContext ctx, PostParam<GLVoucherModel> param, APIDataPool dataPool)
		{
			_accountDataPool = dataPool.Accounts;
			_currencyDataPool = currencyDal.GetCurrencyViewList(ctx, null, true, null);
			_bankAccountDataPool = dataPool.Banks;
			_contactDataPool = dataPool.Contacts;
			_expenseItemDataPool = dataPool.ExpenseItems;
			_itemDataPool = dataPool.Items;
			_employeeDataPool = dataPool.Employees;
			_payItemDataPool = dataPool.PayItems;
			_payItemGroupsDataPool = dataPool.PayItemGroups;
			_trackDataPool = dataPool.TrackingCategories;
			_checkGroupDataPool = dataPool.CheckGroups;
			_checkGroupValueDataPool = dataPool.CheckGroupValues;
			_currentPeriod = _settlementBiz.GetCurrentPeriod(ctx);
			List<string> list = (from t in param.DataList
			where !string.IsNullOrEmpty(t.MItemID)
			select t.MItemID).ToList();
			if (list.Count > 0)
			{
				GetParam param2 = new GetParam
				{
					MOrgID = ctx.MOrgID,
					MUserID = ctx.MUserID,
					IncludeDetail = new bool?(true)
				};
				if (list.Count > 0)
				{
					base.SetWhereString(param2, "MItemID", list, true);
				}
				DataGridJson<GLVoucherModel> dataGridJson = _apiVoucherDal.Get(ctx, param2);
				_dbList = dataGridJson.rows;
			}
		}

		protected override void OnPostValidate(MContext ctx, PostParam<GLVoucherModel> param, APIDataPool dataPool, GLVoucherModel model, bool isPut, ref Dictionary<string, List<string>> validNameList, ref Dictionary<string, string> updNameList)
		{
			ProcessVoucherModel(ctx, model);
			ProcessNexVoucherNumber(ctx, model);
		}

		protected override List<CommandInfo> OnPostGetCmd(MContext ctx, PostParam<GLVoucherModel> param, APIDataPool dataPool, GLVoucherModel model)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			GLVoucherModel gLVoucherModel = param.DataList.LastOrDefault((GLVoucherModel t) => !string.IsNullOrEmpty(t.MItemID) && t.MItemID == model.MItemID && t.ValidationErrors.Count == 0 && t.UniqueIndex < model.UniqueIndex);
			if (gLVoucherModel != null && !model.IsNew)
			{
				foreach (GLVoucherEntryModel mVoucherEntry in gLVoucherModel.MVoucherEntrys)
				{
					if (mVoucherEntry != null)
					{
						list.AddRange(ModelInfoManager.GetDeleteCmd<GLVoucherEntryModel>(ctx, mVoucherEntry.MEntryID));
					}
				}
			}
			list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<GLVoucherModel>(ctx, model, model.UpdateFieldList, true));
			list.AddRange(ModelInfoManager.GetInsertOrUpdateCmds(ctx, (from a in _checkGroupValueDataPool
			where a.IsNew
			select a).ToList(), null, true));
			foreach (GLCheckGroupValueModel item in (from a in _checkGroupValueDataPool
			where a.IsNew
			select a).ToList())
			{
				item.IsNew = false;
			}
			list.AddRange(GlVoucherLogHelper.GetSaveLog(ctx, model));
			return list;
		}

		protected override void OnPostAfter(MContext ctx, PostParam<GLVoucherModel> param, APIDataPool dataPool)
		{
			List<string> list = (from t in param.DataList
			where !string.IsNullOrEmpty(t.MItemID) && t.ValidationErrors.Count == 0
			select t.MItemID).ToList();
			if (list.Count > 0)
			{
				GetParam param2 = new GetParam
				{
					MOrgID = ctx.MOrgID,
					MUserID = ctx.MUserID,
					IncludeDetail = new bool?(true)
				};
				base.SetWhereString(param2, "MItemID", list, true);
				DataGridJson<GLVoucherModel> dataGridJson = base.Get(ctx, param2);
				List<GLVoucherModel> list2 = new List<GLVoucherModel>();
				for (int i = 0; i < param.DataList.Count; i++)
				{
					GLVoucherModel model = param.DataList[i];
					if (model.ValidationErrors.Count > 0)
					{
						list2.Add(model);
					}
					else
					{
						GLVoucherModel gLVoucherModel = dataGridJson.rows.FirstOrDefault((GLVoucherModel a) => a.MItemID == model.MItemID);
						if (gLVoucherModel != null)
						{
							gLVoucherModel.IsNew = model.IsNew;
							list2.Add(gLVoucherModel);
						}
					}
				}
				param.DataList = list2;
			}
		}

		protected override void OnDeleteValidate(MContext ctx, DeleteParam param, APIDataPool dataPool, GLVoucherModel model)
		{
			if (model == null || model.MStatus != 0)
			{
				model.Validate(ctx, true, "ApprovedJournalCanNotDelete", "只有“已保存”状态的凭证才可以被删除。", LangModule.GL);
			}
		}

		protected override List<CommandInfo> OnDeleteGetCmd(MContext ctx, DeleteParam param, APIDataPool dataPool, GLVoucherModel model)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			List<string> voucherIds = new List<string>
			{
				model.MItemID
			};
			list.AddRange(new GLVoucherRepository().GetDeleteVoucherModelsCmd(ctx, voucherIds));
			return list;
		}

		private void ProcessVoucherModel(MContext ctx, GLVoucherModel model)
		{
			GLVoucherModel gLVoucherModel = null;
			if (!string.IsNullOrEmpty(model.MItemID))
			{
				gLVoucherModel = _dbList.FirstOrDefault((GLVoucherModel t) => t.MItemID == model.MItemID);
			}
			model.IsNew = (gLVoucherModel == null);
			if (gLVoucherModel == null)
			{
				model.MCreateBy = ctx.MConsumerKey;
				model.UpdateFieldList.Add("MCreateBy");
				model.MSourceBillKey = 3.ToString();
			}
			bool flag = false;
			bool dateIsOk = ValidateDate(ctx, model, gLVoucherModel, ref flag);
			if (!flag)
			{
				ProcessIdAndNumber(ctx, model, gLVoucherModel, dateIsOk);
				ProcessNumberOfAttachment(ctx, model);
				ProcessUrl(ctx, model);
				ProcessStatus(ctx, model, gLVoucherModel);
				bool flag2 = false;
				ProcessEntryCount(ctx, model, gLVoucherModel, ref flag2);
				if (!flag2)
				{
					bool flag3 = false;
					ProcessEntryList(ctx, model, gLVoucherModel, dateIsOk, ref flag3);
					if (!flag3 && ValidateCreditDebitImbalance(ctx, model))
					{
						ValidateMDebitTotal(ctx, model);
					}
				}
			}
		}

		private bool ValidateDate(MContext ctx, GLVoucherModel model, GLVoucherModel dbModel, ref bool dateIsEmpty)
		{
			bool result = true;
			DateTime dateTime;
			if (!model.IsUpdateFieldExists("MDate"))
			{
				dateTime = ctx.DateNow;
				int year = dateTime.Year;
				dateTime = ctx.DateNow;
				DateTime t = new DateTime(year, dateTime.Month, 1);
				DateTime mDate = new DateTime(_currentPeriod.Year, _currentPeriod.Month, 1);
				if (dbModel != null)
				{
					model.MDate = dbModel.MDate;
				}
				else if (_currentPeriod < t)
				{
					dateTime = mDate.AddMonths(1);
					model.MDate = dateTime.AddDays(-1.0);
				}
				else if (_currentPeriod.Year * 12 + _currentPeriod.Month == t.Year * 12 + t.Month)
				{
					model.MDate = ctx.DateNow;
				}
				else
				{
					model.MDate = mDate;
				}
			}
			else
			{
				model.MDate = model.MDate.ToShortDate();
				if (model.MDate == DateTime.MinValue)
				{
					model.Validate(ctx, true, "FieldEmpty", "'{0}' cannot be empty.", LangModule.Common, "Date");
					dateIsEmpty = true;
					return false;
				}
				int num2;
				if (model.MDate >= ctx.MGLBeginDate)
				{
					dateTime = model.MDate;
					int num = dateTime.Year * 12;
					dateTime = model.MDate;
					num2 = ((num + dateTime.Month < _currentPeriod.Year * 12 + _currentPeriod.Month) ? 1 : 0);
				}
				else
				{
					num2 = 0;
				}
				if (num2 != 0)
				{
					object[] obj = new object[1];
					dateTime = model.MDate;
					obj[0] = dateTime.ToString("yyyy-MM-dd");
					model.Validate(ctx, true, "VoucherDateCannotDuringClosedPeriod", "日期“{0}”所在期间已经结账。", LangModule.GL, obj);
					result = false;
				}
				if (model.MDate < ctx.MGLBeginDate)
				{
					bool invalid = model.MDate < ctx.MGLBeginDate;
					object[] obj2 = new object[2];
					dateTime = model.MDate;
					obj2[0] = dateTime.ToString("yyyy-MM-dd");
					dateTime = ctx.MGLBeginDate;
					obj2[1] = dateTime.ToString("yyyy-MM");
					model.Validate(ctx, invalid, "VoucherDateMustGreaterThanTheGLBeginDate", "日期“{0}”所在期间不能在组织启用期间“{1}”之前。", LangModule.GL, obj2);
					result = false;
				}
			}
			dateTime = model.MDate;
			model.MYear = dateTime.Year;
			dateTime = model.MDate;
			model.MPeriod = dateTime.Month;
			model.UpdateFieldList.Add("MYear");
			model.UpdateFieldList.Add("MPeriod");
			return result;
		}

		private void ProcessIdAndNumber(MContext ctx, GLVoucherModel model, GLVoucherModel dbModel, bool dateIsOk)
		{
			bool flag = model.IsUpdateFieldExists("MNumber");
			bool flag2 = flag && string.IsNullOrEmpty(model.MVoucherNumber);
			bool flag3 = model.IsUpdateFieldExists("MItemID");
			bool flag4 = flag3 && string.IsNullOrEmpty(model.MItemID);
			if (flag2)
			{
				model.Validate(ctx, true, "JournalNumberCanNotEmpty", "凭证号码不能为空。", LangModule.GL);
			}
			else if (flag3 && !flag4 && dbModel == null)
			{
				model.Validate(ctx, true, "JournalIsNotExists", "凭证内码对应的凭证不存在。", LangModule.GL);
			}
			else if (flag || !model.IsNew)
			{
				if (!flag && !model.IsNew)
				{
					model.MVoucherNumber = dbModel.MNumber;
				}
				model.MVoucherNumber = model.MVoucherNumber.PadLeft(ctx.MVoucherNumberLength, Convert.ToChar(ctx.MVoucherNumberFilledChar));
				if (dateIsOk)
				{
					if (!RegExp.IsInt(model.MVoucherNumber) || model.MVoucherNumber.Length > ctx.MVoucherNumberLength || int.Parse(model.MVoucherNumber) < 1)
					{
						model.Validate(ctx, true, "VoucherNumberOutRange", "凭证号必须是凭证设置中设置的{0}到{1}之间的号码。", LangModule.GL, COMResourceHelper.MinVoucherNumber(ctx), COMResourceHelper.MaxVoucherNumber(ctx));
					}
					else if (COMResourceHelper.IsMNumberUsed(ctx, model.MYear, model.MPeriod, model.MVoucherNumber, model.MItemID))
					{
						model.Validate(ctx, true, "JournalMustBeUnique", "凭证号码必须唯一。", LangModule.GL);
					}
					else if (dbModel?.MSourceBillKey == 2.ToString())
					{
						model.Validate(ctx, true, "BillJournalCanNotUpdate", "业务单据生成的凭证不能被更新。", LangModule.GL);
					}
				}
			}
		}

		private void ProcessNumberOfAttachment(MContext ctx, GLVoucherModel model)
		{
			if (model.MAttachments < 0 || model.MAttachments > 999)
			{
				model.Validate(ctx, true, "AttachmentMustGreaterThanZero", "附件数必须为0~{0}之间的整数。", LangModule.GL, 999);
			}
		}

		private void ProcessUrl(MContext ctx, GLVoucherModel model)
		{
			if (!string.IsNullOrEmpty(model.MUrl) && !RegExp.IsUrl(model.MUrl))
			{
				model.Validate(ctx, true, "UrlFormatIsError", "来源必须为有效的url地址格式。", LangModule.IV);
			}
		}

		private void ProcessStatus(MContext ctx, GLVoucherModel model, GLVoucherModel dbModel)
		{
			bool flag = model.IsUpdateFieldExists("MStatus");
			if ((model.IsNew & flag) && model.MStatus != 0)
			{
				model.Validate(ctx, true, "OnlySupportSavedJournal", "目前只支持创建“SAVED”状态的凭证。", LangModule.GL);
			}
			if ((!model.IsNew & flag) && model.MStatus == 1)
			{
				model.Validate(ctx, true, "NotSupportUpdateJournalApproved", "目前不支持将凭证更新为“APPROVED”状态。", LangModule.GL);
			}
			else
			{
				if (dbModel != null && dbModel.MStatus == 1)
				{
					model.Validate(ctx, true, "ApprovedJournalCanNotUpdate", "已审核的凭证不能被更新。", LangModule.GL);
				}
				if (!flag && model.IsNew)
				{
					model.UpdateFieldList.Add("MStatus");
					model.MStatus = 0;
				}
			}
		}

		private void ProcessEntryCount(MContext ctx, GLVoucherModel model, GLVoucherModel dbModel, ref bool entryCountError)
		{
			if (dbModel != null && !model.IsUpdateFieldExists("MVoucherEntrys"))
			{
				model.MVoucherEntrys = dbModel.MVoucherEntrys;
			}
			else if (model.MVoucherEntrys == null || model.MVoucherEntrys.Count < 2)
			{
				model.Validate(ctx, true, "LeastTwoJournalLines", "至少指定2行凭证分录信息。", LangModule.GL);
				entryCountError = true;
			}
		}

		private void ProcessEntryList(MContext ctx, GLVoucherModel model, GLVoucherModel dbModel, bool dateIsOk, ref bool entryListError)
		{
			if (model.IsNew || model.IsUpdateFieldExists("MVoucherEntrys"))
			{
				if (!model.IsNew && model.IsUpdateFieldExists("MVoucherEntrys"))
				{
					List<string> list = (from a in model.MVoucherEntrys
					where !string.IsNullOrEmpty(a.MJournalLineID)
					select a.MJournalLineID).ToList();
					if (list.Count != list.Distinct().ToList().Count)
					{
						model.Validate(ctx, true, "CanNotSupportMultSameEntryID", "不允许多行分录有相同的“{0}”。", LangModule.Common, "JournalLineID");
						entryListError = true;
						return;
					}
				}
				int num = 0;
				foreach (GLVoucherEntryModel mVoucherEntry in model.MVoucherEntrys)
				{
					mVoucherEntry.MEntrySeq = num;
					GLVoucherEntryModel gLVoucherEntryModel = null;
					if (dbModel != null)
					{
						gLVoucherEntryModel = dbModel.MVoucherEntrys.FirstOrDefault((GLVoucherEntryModel a) => a.MEntryID == mVoucherEntry.MEntryID);
					}
					if (gLVoucherEntryModel != null)
					{
						mVoucherEntry.IsNew = false;
						mVoucherEntry.MCheckGroupValueID = gLVoucherEntryModel.MCheckGroupValueID;
					}
					else
					{
						mVoucherEntry.MEntryID = UUIDHelper.GetGuid();
						mVoucherEntry.IsNew = true;
					}
					if (!ProcessEntryModel(ctx, model, mVoucherEntry, dateIsOk) || mVoucherEntry.ValidationErrors.Count > 0)
					{
						model.ValidationErrors = model.ValidationErrors.Concat(mVoucherEntry.ValidationErrors).ToList();
						mVoucherEntry.ValidationErrors.Clear();
						entryListError = true;
					}
					num++;
				}
			}
		}

		private bool ProcessEntryModel(MContext ctx, GLVoucherModel model, GLVoucherEntryModel entryModel, bool dateIsOk)
		{
			BDAccountEditModel acctModel = _accountDataPool.FirstOrDefault((BDAccountEditModel a) => a.MNumber == entryModel.MAccountCode);
			if (!CheckOnceEntryField(ctx, entryModel, acctModel))
			{
				return false;
			}
			bool flag = ValidateAccountCode(ctx, entryModel, acctModel);
			bool flag2 = true;
			bool flag3 = true;
			if (entryModel.MCurrencyID.EqualsIgnoreCase(ctx.MBasCurrencyID) || dateIsOk)
			{
				flag2 = ProcessCurrencyRate(ctx, model, entryModel);
				flag3 = ProcessOriginalAmount(ctx, entryModel);
			}
			bool flag4 = ValidateAccountDimension(ctx, entryModel, acctModel);
			ProcessDebitAndCredit(ctx, entryModel);
			return flag & flag2 & flag3 & flag4;
		}

		private bool CheckOnceEntryField(MContext ctx, GLVoucherEntryModel entryModel, BDAccountEditModel acctModel)
		{
			bool flag = CheckExplanation(ctx, entryModel);
			bool flag2 = CheckCurrencyRate(ctx, entryModel);
			bool flag3 = CheckAmount(ctx, entryModel);
			bool flag4 = ProcessAccountCode(ctx, entryModel, acctModel);
			bool flag5 = ProcessCurrencyCode(ctx, entryModel);
			bool flag6 = CheckDirection(ctx, entryModel);
			return flag & flag2 & flag3 & flag4 & flag5 & flag6;
		}

		private bool CheckExplanation(MContext ctx, GLVoucherEntryModel entryModel)
		{
			if (!entryModel.IsUpdateFieldExists("MExplanation"))
			{
				entryModel.Validate(ctx, true, "FieldMustProvide", "必须提供“{0}”。", LangModule.Common, "Explanation");
				return false;
			}
			if (string.IsNullOrEmpty(entryModel.MExplanation))
			{
				entryModel.Validate(ctx, true, "FieldEmpty", "'{0}' cannot be empty.", LangModule.Common, "Explanation");
				return false;
			}
			return true;
		}

		private bool CheckCurrencyRate(MContext ctx, GLVoucherEntryModel entryModel)
		{
			if (entryModel.IsUpdateFieldExists("MExchangeRate") && entryModel.MExchangeRate <= decimal.Zero)
			{
				entryModel.Validate(ctx, true, "ExchangeRateGreaterThan0", "汇率必须大于0。", LangModule.IV);
				return false;
			}
			return true;
		}

		private bool CheckAmount(MContext ctx, GLVoucherEntryModel entryModel)
		{
			if (!entryModel.IsUpdateFieldExists("MAmount"))
			{
				entryModel.Validate(ctx, true, "FieldMustProvide", "必须提供“{0}”。", LangModule.Common, "Amount");
				return false;
			}
			return true;
		}

		private bool ProcessAccountCode(MContext ctx, GLVoucherEntryModel entryModel, BDAccountEditModel acctModel)
		{
			bool result = true;
			if (!entryModel.IsUpdateFieldExists("MAccountCode"))
			{
				entryModel.Validate(ctx, true, "FieldMustProvide", "必须提供“{0}”。", LangModule.Common, "AccountCode");
				result = false;
			}
			else if (string.IsNullOrEmpty(entryModel.MAccountCode))
			{
				entryModel.Validate(ctx, true, "FieldEmpty", "'{0}' cannot be empty.", LangModule.Common, "AccountCode");
				result = false;
			}
			else if (acctModel == null)
			{
				entryModel.Validate(ctx, true, "AccountCodeNotExist", "科目代码“{0}”不存在。", LangModule.BD, entryModel.MAccountCode);
				result = false;
			}
			else
			{
				if (_accountDataPool.Exists((BDAccountEditModel a) => a.MParentID == acctModel.MItemID))
				{
					entryModel.Validate(ctx, true, "AccountExistSubAcct", "Account“{0}”不是末级科目。", LangModule.BD, entryModel.MAccountCode);
					result = false;
				}
				if (!acctModel.MIsActive)
				{
					entryModel.Validate(ctx, true, "AccountDisabled", "科目（{0}）已禁用", LangModule.Common, entryModel.MAccountCode);
					result = false;
				}
				entryModel.MAccountID = acctModel.MAccountID;
			}
			return result;
		}

		private bool CheckDirection(MContext ctx, GLVoucherEntryModel entryModel)
		{
			if (!entryModel.IsUpdateFieldExists("MDC"))
			{
				entryModel.Validate(ctx, true, "FieldMustProvide", "必须提供“{0}”。", LangModule.Common, "Direction");
				return false;
			}
			return true;
		}

		private bool ProcessCurrencyCode(MContext ctx, GLVoucherEntryModel entryModel)
		{
			if (entryModel.IsUpdateFieldExists("MCurrencyID"))
			{
				if (string.IsNullOrEmpty(entryModel.MCurrencyID))
				{
					entryModel.Validate(ctx, true, "FieldEmpty", "'{0}' cannot be empty.", LangModule.Common, "CurrencyCode");
					return false;
				}
				REGCurrencyViewModel rEGCurrencyViewModel = _currencyDataPool.FirstOrDefault((REGCurrencyViewModel x) => x.MCurrencyID.EqualsIgnoreCase(entryModel.MCurrencyID));
				if (rEGCurrencyViewModel == null)
				{
					entryModel.Validate(ctx, true, "FieldNotExists", "{0}“{1}”不存在。", LangModule.Common, "CurrencyCode", entryModel.MCurrencyID);
					return false;
				}
				entryModel.MCurrencyID = rEGCurrencyViewModel.MCurrencyID;
			}
			else
			{
				entryModel.MExchangeRate = 1.0m;
				entryModel.MCurrencyID = ctx.MBasCurrencyID;
			}
			return true;
		}

		private bool ValidateAccountCode(MContext ctx, GLVoucherEntryModel entryModel, BDAccountEditModel acctModel)
		{
			bool result = true;
			BDBankAccountModel bDBankAccountModel = _bankAccountDataPool.FirstOrDefault((BDBankAccountModel a) => a.MItemID == entryModel.MAccountID);
			if (bDBankAccountModel != null && bDBankAccountModel.MCyID != entryModel.MCurrencyID)
			{
				entryModel.Validate(ctx, true, "EntryCurrencyNotMatchBank", "币别“{0}”和银行科目“{1}”的核算币别不匹配。", LangModule.GL, entryModel.MCurrencyID, entryModel.MAccountCode);
				result = false;
			}
			if (bDBankAccountModel == null && acctModel != null && !entryModel.MCurrencyID.EqualsIgnoreCase(ctx.MBasCurrencyID) && !acctModel.MIsCheckForCurrency)
			{
				entryModel.Validate(ctx, true, "ForigneCurrencyAccountDisable", "科目“{0}”没有启用外币核算。", LangModule.GL, entryModel.MAccountCode);
				result = false;
			}
			return result;
		}

		private bool ProcessCurrencyRate(MContext ctx, GLVoucherModel model, GLVoucherEntryModel entryModel)
		{
			if (entryModel.MCurrencyID.EqualsIgnoreCase(ctx.MBasCurrencyID))
			{
				entryModel.MExchangeRate = 1.0m;
			}
			else if (!entryModel.IsUpdateFieldExists("MExchangeRate"))
			{
				REGCurrencyViewModel rEGCurrencyViewModel = (from x in _currencyDataPool
				where x.MCurrencyID == entryModel.MCurrencyID && x.MRateDate <= model.MDate
				orderby x.MRateDate descending
				select x).FirstOrDefault();
				if (string.IsNullOrWhiteSpace(rEGCurrencyViewModel?.MRate))
				{
					entryModel.Validate(ctx, true, "CurrencyRateIsNotMaintained", "币别“{0}”没有维护凭证日期当天的汇率。", LangModule.GL, entryModel.MCurrencyID);
					return false;
				}
				entryModel.MExchangeRate = Convert.ToDecimal(rEGCurrencyViewModel.MRate);
			}
			return true;
		}

		private bool ProcessOriginalAmount(MContext ctx, GLVoucherEntryModel entryModel)
		{
			bool result = true;
			bool flag = entryModel.IsUpdateFieldExists("MAmountFor");
			bool flag2 = entryModel.MCurrencyID.EqualsIgnoreCase(ctx.MBasCurrencyID);
			if ((flag && entryModel.MAmountFor != entryModel.MAmount) & flag2)
			{
				entryModel.Validate(ctx, true, "BaseCurrencyOriginalAmountMustEqualAmount", "币别为本位币时，原币金额必须等于分录金额。", LangModule.GL);
				result = false;
			}
			if (flag2 && !flag)
			{
				entryModel.MAmountFor = entryModel.MAmount;
			}
			if (!flag && !flag2)
			{
				entryModel.Validate(ctx, true, "OriginalCurrencyAmountMustProvide", "币别为外币时，必须指定原币金额。", LangModule.GL);
				result = false;
			}
			if (flag && !flag2 && entryModel.MAmountFor * entryModel.MExchangeRate * entryModel.MAmount < decimal.Zero)
			{
				entryModel.Validate(ctx, true, "AbsIsNotMatchOriginalCurrencyAmountAndAmount", "原币金额和分录金额不能正负不一致。", LangModule.GL);
				result = false;
			}
			return result;
		}

		private bool ValidateAccountDimension(MContext ctx, GLVoucherEntryModel entryModel, BDAccountEditModel acctModel)
		{
			entryModel.MCheckGroupValueModel = APIDataRepository.ToLocalDimension(ctx, entryModel.MAccountDimensions, _contactDataPool, _expenseItemDataPool, _itemDataPool, _employeeDataPool, _payItemDataPool, _payItemGroupsDataPool, _trackDataPool);
			ProcessAccountCheckGroup(acctModel);
			if (acctModel != null && !entryModel.Validate(ctx, entryModel.MAccountCode, acctModel.MCheckGroupModel, entryModel.MCheckGroupValueModel))
			{
				return false;
			}
			ProcessCheckGroupValueModel(entryModel, entryModel.MCheckGroupValueModel);
			return true;
		}

		private void ProcessCheckGroupValueModel(GLVoucherEntryModel entryModel, GLCheckGroupValueModel checkGroupValueModel)
		{
			GLCheckGroupValueModel checkGroupValueModel2 = GetCheckGroupValueModel(checkGroupValueModel);
			if (checkGroupValueModel2 != null)
			{
				entryModel.MCheckGroupValueID = checkGroupValueModel2.MItemID;
				entryModel.MCheckGroupValueModel = checkGroupValueModel2;
			}
			else
			{
				string text2 = checkGroupValueModel.MItemID = (entryModel.MCheckGroupValueID = UUIDHelper.GetGuid());
				checkGroupValueModel.IsNew = true;
				_checkGroupValueDataPool.Add(checkGroupValueModel);
			}
		}

		private void ProcessDebitAndCredit(MContext ctx, GLVoucherEntryModel entryModel)
		{
			entryModel.MCredit = ((entryModel.MDC == -1) ? entryModel.MAmount : decimal.Zero);
			entryModel.MDebit = ((entryModel.MDC == 1) ? entryModel.MAmount : decimal.Zero);
		}

		private bool ValidateCreditDebitImbalance(MContext ctx, GLVoucherModel model)
		{
			model.MDebitTotal = model.MVoucherEntrys.Sum((GLVoucherEntryModel x) => x.MDebit);
			model.MCreditTotal = model.MVoucherEntrys.Sum((GLVoucherEntryModel x) => x.MCredit);
			model.UpdateFieldList.Add("MDebitTotal");
			model.UpdateFieldList.Add("MCreditTotal");
			if (model.MDebitTotal != model.MCreditTotal)
			{
				model.Validate(ctx, true, "CreditNotBalancedDebit", "凭证借贷方不平。", LangModule.GL);
				return false;
			}
			return true;
		}

		private void ValidateMDebitTotal(MContext ctx, GLVoucherModel model)
		{
			if (DecimalUtility.IsDecimalValueTooLong(Math.Abs(model.MDebitTotal)))
			{
				string msg = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "DecimalCanNotExceedMaxInteger", "“{0}”的整数位不能超过12位。"), "TotalAmount");
				if (model.ValidationErrors.All((ValidationError a) => a.Message != msg))
				{
					model.ValidationErrors.Add(new ValidationError(msg));
				}
			}
		}

		private void ProcessNexVoucherNumber(MContext ctx, GLVoucherModel model)
		{
			bool flag = model.IsUpdateFieldExists("MNumber");
			if (model.ValidationErrors.Count > 0)
			{
				string numberErrorMsg = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "JournalMustBeUnique", "凭证号码必须唯一。");
				bool flag2 = flag && !string.IsNullOrEmpty(model.MVoucherNumber) && RegExp.IsInt(model.MVoucherNumber) && model.MVoucherNumber.Length <= ctx.MVoucherNumberLength && int.Parse(model.MVoucherNumber) > 0;
				if (!model.ValidationErrors.Exists((ValidationError a) => a.Message == numberErrorMsg) & flag2)
				{
					string text = ctx.MResourceIds?.LastOrDefault();
					if (!string.IsNullOrEmpty(text))
					{
						MResourceHelper.RemoveByIDs(new List<string>
						{
							text
						});
					}
				}
			}
			else
			{
				try
				{
					string nextVoucherNumber = COMResourceHelper.GetNextVoucherNumber(ctx, model.MYear, model.MPeriod, null, null);
					if (!flag && model.IsNew)
					{
						model.MNumber = nextVoucherNumber;
						model.UpdateFieldList.Add("MNumber");
					}
				}
				catch (MActionException ex)
				{
					if (ex.Codes.Contains(MActionResultCodeEnum.MVoucherNumberNotMeetDemand))
					{
						model.Validate(ctx, true, "VourcherNumerGreaterMaxLength", "凭证编号的位数已达到系统设置的最大值，请在美记中【设置-总账设置-凭证设置-凭证编号设置】进行调整!", LangModule.GL);
					}
				}
			}
		}

		private void ProcessAccountCheckGroup(BDAccountEditModel acctModel)
		{
			if (acctModel != null)
			{
				GLCheckGroupModel gLCheckGroupModel = _checkGroupDataPool.FirstOrDefault((GLCheckGroupModel a) => a.MItemID == acctModel.MCheckGroupID);
				if (gLCheckGroupModel != null)
				{
					acctModel.MCheckGroupModel = new GLCheckGroupModel
					{
						MContactID = gLCheckGroupModel.MContactID,
						MMerItemID = gLCheckGroupModel.MMerItemID,
						MEmployeeID = gLCheckGroupModel.MEmployeeID,
						MExpItemID = gLCheckGroupModel.MExpItemID,
						MPaItemID = gLCheckGroupModel.MPaItemID,
						MTrackItem1 = gLCheckGroupModel.MTrackItem1,
						MTrackItem2 = gLCheckGroupModel.MTrackItem2,
						MTrackItem3 = gLCheckGroupModel.MTrackItem3,
						MTrackItem4 = gLCheckGroupModel.MTrackItem4,
						MTrackItem5 = gLCheckGroupModel.MTrackItem5
					};
				}
			}
		}

		private GLCheckGroupValueModel GetCheckGroupValueModel(GLCheckGroupValueModel checkGroupValueModel)
		{
			if (checkGroupValueModel == null)
			{
				checkGroupValueModel = new GLCheckGroupValueModel();
			}
			return _checkGroupValueDataPool.FirstOrDefault((GLCheckGroupValueModel a) => checkGroupValueModel.Equals(checkGroupValueModel, a));
		}

		public GLVoucherModel GetVoucherEditModel(MContext ctx, GLVoucherModel model)
		{
			GLVoucherModel gLVoucherModel = new GLVoucherModel();
			if (string.IsNullOrWhiteSpace(model.MItemID))
			{
				if (model.MPeriodTransfer != null)
				{
					gLVoucherModel = GetVoucherByPeriodTransfer(ctx, model.MPeriodTransfer);
				}
				else
				{
					PreHandleVouchers(ctx, new List<GLVoucherModel>
					{
						model
					});
					if (!settleBiz.IsPeriodValid(ctx, new DateTime(model.MYear, model.MPeriod, 1)))
					{
						DateTime avaliableVoucherDate = settleBiz.GetAvaliableVoucherDate(ctx);
						model.MYear = avaliableVoucherDate.Year;
						model.MPeriod = avaliableVoucherDate.Month;
						model.MDate = avaliableVoucherDate;
					}
					gLVoucherModel = GetNewVoucherEditModel(ctx, model);
				}
			}
			else
			{
				GLVoucherModel voucherModel = newDal.GetVoucherModel(ctx, model.MItemID, !string.IsNullOrWhiteSpace(model.MDocVoucherID));
				if (model.MDir != 0)
				{
					gLVoucherModel = ((voucherModel != null && !string.IsNullOrEmpty(voucherModel.MNumber)) ? GetPrevNextVoucherModel(ctx, voucherModel, model.MDir) : GetPrevNextVoucherModel(ctx, model, model.MDir));
				}
				else
				{
					if (voucherModel == null)
					{
						throw new MActionException(new List<MActionResultCodeEnum>
						{
							MActionResultCodeEnum.MVoucherDeleted
						});
					}
					gLVoucherModel = ((!model.MIsCopy) ? ((!model.MIsReverse) ? (string.IsNullOrWhiteSpace(model.MEntryAccountPair) ? voucherModel : SetVoucherUserAccount(ctx, voucherModel, model.MEntryAccountPair)) : GetReverseVoucherModel(ctx, voucherModel)) : GetCopyVoucherModel(ctx, voucherModel));
				}
			}
			gLVoucherModel.MCreatorName = (string.IsNullOrWhiteSpace(gLVoucherModel.MCreatorName) ? GlobalFormat.GetUserName(ctx.MFirstName, ctx.MLastName, null) : gLVoucherModel.MCreatorName);
			return gLVoucherModel;
		}

		private GLVoucherModel SetVoucherUserAccount(MContext ctx, GLVoucherModel model, string entryAccountPair)
		{
			string[] array = entryAccountPair.Split(',');
			if (entryAccountPair != null && array.Count() > 0 && (from x in array
			where !string.IsNullOrWhiteSpace(x)
			select x).Count() > 0)
			{
				List<BDAccountModel> accountWithParentList = GLDataPool.GetInstance(ctx, false, 0, 0, 0).AccountWithParentList;
				for (int i = 0; i < array.Count(); i++)
				{
					string[] entryAccount = array[i].Split('-');
					if (entryAccount != null && entryAccount.Count() > 0 && (from x in entryAccount
					where !string.IsNullOrWhiteSpace(x)
					select x).Count() == 2)
					{
						GLVoucherEntryModel gLVoucherEntryModel = model.MVoucherEntrys.FirstOrDefault((GLVoucherEntryModel x) => x.MEntryID == entryAccount[0]);
						BDAccountModel bDAccountModel = accountWithParentList.FirstOrDefault((BDAccountModel x) => x.MItemID == entryAccount[1]);
						if (bDAccountModel != null && gLVoucherEntryModel != null)
						{
							gLVoucherEntryModel.MAccountID = bDAccountModel.MItemID;
							bDAccountModel.MCurrencyDataModel = gLVoucherEntryModel.MAccountModel.MCurrencyDataModel;
							bDAccountModel.MCheckGroupValueModel = gLVoucherEntryModel.MCheckGroupValueModel;
							gLVoucherEntryModel.MAccountModel = bDAccountModel;
						}
					}
				}
			}
			return model;
		}

		private GLVoucherModel GetPrevNextVoucherModel(MContext ctx, GLVoucherModel model, int dir)
		{
			if (string.IsNullOrEmpty(model.MNumber))
			{
				throw new MActionException(new List<MActionResultCodeEnum>
				{
					MActionResultCodeEnum.MVoucherDeleted
				});
			}
			List<GLVoucherModel> list = newDal.GetVoucherModelList(ctx, null, false, model.MYear, model.MPeriod).ToList();
			if (list == null || list.Count <= 0)
			{
				throw new MActionException(new List<MActionResultCodeEnum>
				{
					MActionResultCodeEnum.MVoucherDeleted
				});
			}
			GLVoucherModel gLVoucherModel = model;
			switch (dir)
			{
			case -1:
				gLVoucherModel = (from x in list
				orderby int.Parse(x.MNumber) descending
				select x).FirstOrDefault((GLVoucherModel x) => int.Parse(x.MNumber) < int.Parse(model.MNumber));
				if (gLVoucherModel == null)
				{
					gLVoucherModel = (from x in list
					orderby int.Parse(x.MNumber)
					select x).FirstOrDefault();
				}
				break;
			case 1:
				gLVoucherModel = (from x in list
				orderby int.Parse(x.MNumber)
				select x).FirstOrDefault((GLVoucherModel x) => int.Parse(x.MNumber) > int.Parse(model.MNumber));
				if (gLVoucherModel == null)
				{
					gLVoucherModel = (from x in list
					orderby int.Parse(x.MNumber) descending
					select x).FirstOrDefault();
				}
				break;
			}
			if (gLVoucherModel != null)
			{
				gLVoucherModel = dal.GetVoucherModel(ctx, gLVoucherModel.MItemID, false);
			}
			return gLVoucherModel ?? model;
		}

		private GLVoucherModel GetReverseVoucherModel(MContext ctx, GLVoucherModel model)
		{
			if (!settleBiz.IsPeriodValid(ctx, new DateTime(model.MYear, model.MPeriod, 1)))
			{
				DateTime avaliableVoucherDate = settleBiz.GetAvaliableVoucherDate(ctx);
				model.MYear = avaliableVoucherDate.Year;
				model.MPeriod = avaliableVoucherDate.Month;
				model.MDate = avaliableVoucherDate;
			}
			string reverseLang = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "ReverseVoucher", "冲销凭证");
			model.MNumber = COMResourceHelper.GetNextVoucherNumber(ctx, model.MYear, model.MPeriod, null, null);
			model.MOVoucherID = model.MItemID;
			model.MIsReverse = true;
			model.MItemID = string.Empty;
			model.MStatus = -1;
			model.MDocID = string.Empty;
			model.MSourceBillKey = 0.ToString();
			model.MVoucherEntrys.ForEach(delegate(GLVoucherEntryModel x)
			{
				x.MEntryID = string.Empty;
				x.MStatus = -1;
				x.MDebit *= -1.0m;
				x.MCredit *= -1.0m;
				x.MAmount *= -1.0m;
				x.MAmountFor *= -1.0m;
				x.MExplanation = reverseLang + "-" + x.MExplanation;
				if (x.MAccountModel != null && x.MAccountModel.MCurrencyDataModel != null)
				{
					x.MAccountModel.MCurrencyDataModel.MAmount = x.MAccountModel.MCurrencyDataModel.MAmount * -1.0m;
					x.MAccountModel.MCurrencyDataModel.MAmountFor = x.MAccountModel.MCurrencyDataModel.MAmountFor * -1.0m;
				}
			});
			return model;
		}

		private GLVoucherModel GetCopyVoucherModel(MContext ctx, GLVoucherModel model)
		{
			if (!settleBiz.IsPeriodValid(ctx, new DateTime(model.MYear, model.MPeriod, 1)))
			{
				DateTime avaliableVoucherDate = settleBiz.GetAvaliableVoucherDate(ctx);
				model.MYear = avaliableVoucherDate.Year;
				model.MPeriod = avaliableVoucherDate.Month;
				model.MDate = avaliableVoucherDate;
			}
			model.MNumber = COMResourceHelper.GetNextVoucherNumber(ctx, model.MYear, model.MPeriod, null, null);
			model.MItemID = string.Empty;
			model.MStatus = -1;
			model.MDocID = string.Empty;
			model.MVoucherEntrys.ForEach(delegate(GLVoucherEntryModel x)
			{
				x.MEntryID = string.Empty;
				x.MCheckGroupValueID = string.Empty;
				x.MStatus = -1;
			});
			model.MSourceBillKey = 0.ToString();
			ProcessDisabelBaseData(ctx, model);
			return model;
		}

		private bool IsCheckTypeEnabled(int status)
		{
			return status == CheckTypeStatusEnum.Required || status == CheckTypeStatusEnum.Optional;
		}

		private void ProcessDisabelBaseData(MContext ctx, GLVoucherModel model)
		{
			List<GLVoucherEntryModel> mVoucherEntrys = model.MVoucherEntrys;
			if (mVoucherEntrys != null && mVoucherEntrys.Count() != 0)
			{
				GLDataPool instance = GLDataPool.GetInstance(ctx, false, 0, 0, 0);
				GLUtility gLUtility = new GLUtility();
				GLCheckTypeDataModel trackCheckTypeData = gLUtility.GetTrackCheckTypeData(ctx, 5, false);
				GLCheckTypeDataModel trackCheckTypeData2 = gLUtility.GetTrackCheckTypeData(ctx, 6, false);
				GLCheckTypeDataModel trackCheckTypeData3 = gLUtility.GetTrackCheckTypeData(ctx, 7, false);
				GLCheckTypeDataModel trackCheckTypeData4 = gLUtility.GetTrackCheckTypeData(ctx, 8, false);
				GLCheckTypeDataModel trackCheckTypeData5 = gLUtility.GetTrackCheckTypeData(ctx, 9, false);
				List<GLTreeModel> list = trackCheckTypeData?.MDataList;
				List<GLTreeModel> list2 = trackCheckTypeData2?.MDataList;
				List<GLTreeModel> list3 = trackCheckTypeData3?.MDataList;
				List<GLTreeModel> list4 = trackCheckTypeData4?.MDataList;
				List<GLTreeModel> list5 = trackCheckTypeData5?.MDataList;
				foreach (GLVoucherEntryModel item in mVoucherEntrys)
				{
					BDAccountModel bDAccountModel = instance.AccountList.FirstOrDefault((BDAccountModel x) => x.MItemID == item.MAccountID);
					if (bDAccountModel != null)
					{
						GLCheckGroupModel mCheckGroupModel = bDAccountModel.MCheckGroupModel;
						GLCheckGroupValueModel checkValueModel = item.MCheckGroupValueModel;
						if (!IsCheckTypeEnabled(mCheckGroupModel.MContactID))
						{
							checkValueModel.MContactID = null;
							checkValueModel.MContactName = null;
						}
						if (!IsCheckTypeEnabled(mCheckGroupModel.MEmployeeID))
						{
							checkValueModel.MEmployeeID = null;
							checkValueModel.MEmployeeIDName = null;
						}
						if (!IsCheckTypeEnabled(mCheckGroupModel.MExpItemID))
						{
							checkValueModel.MExpItemID = null;
							checkValueModel.MExpItemIDName = null;
						}
						if (!IsCheckTypeEnabled(mCheckGroupModel.MMerItemID))
						{
							checkValueModel.MMerItemID = null;
							checkValueModel.MMerItemIDName = null;
						}
						if (!IsCheckTypeEnabled(mCheckGroupModel.MPaItemID))
						{
							checkValueModel.MPaItemID = null;
							checkValueModel.MPaItemIDName = null;
						}
						if (!IsCheckTypeEnabled(mCheckGroupModel.MTrackItem1))
						{
							checkValueModel.MTrackItem1 = null;
							checkValueModel.MTrackItem1Name = null;
						}
						if (!IsCheckTypeEnabled(mCheckGroupModel.MTrackItem2))
						{
							checkValueModel.MTrackItem2 = null;
							checkValueModel.MTrackItem2Name = null;
						}
						if (!IsCheckTypeEnabled(mCheckGroupModel.MTrackItem3))
						{
							checkValueModel.MTrackItem3 = null;
							checkValueModel.MTrackItem3Name = null;
						}
						if (!IsCheckTypeEnabled(mCheckGroupModel.MTrackItem4))
						{
							checkValueModel.MTrackItem4 = null;
							checkValueModel.MTrackItem4Name = null;
						}
						if (!IsCheckTypeEnabled(mCheckGroupModel.MTrackItem5))
						{
							checkValueModel.MTrackItem5 = null;
							checkValueModel.MTrackItem5Name = null;
						}
						if (!string.IsNullOrWhiteSpace(checkValueModel.MContactID) && (instance.ContactList == null || instance.ContactList.Count == 0 || !instance.ContactList.Exists((BDContactsModel x) => x.MItemID == checkValueModel.MContactID && x.MIsActive)))
						{
							checkValueModel.MContactID = null;
							checkValueModel.MContactName = null;
						}
						if (!string.IsNullOrWhiteSpace(checkValueModel.MEmployeeID) && (instance.EmployeeList == null || instance.EmployeeList.Count == 0 || !instance.EmployeeList.Exists((BDEmployeesModel x) => x.MItemID == checkValueModel.MEmployeeID && x.MIsActive)))
						{
							checkValueModel.MEmployeeID = null;
							checkValueModel.MEmployeeName = null;
						}
						if (!string.IsNullOrWhiteSpace(checkValueModel.MExpItemID) && (instance.ExpenseItemList == null || instance.ExpenseItemList.Count == 0 || !instance.ExpenseItemList.Exists((BDExpenseItemModel x) => x.MItemID == checkValueModel.MExpItemID && x.MIsActive)))
						{
							checkValueModel.MExpItemID = null;
							checkValueModel.MExpItemName = null;
						}
						if (!string.IsNullOrWhiteSpace(checkValueModel.MMerItemID) && (instance.MerItemList == null || instance.MerItemList.Count == 0 || !instance.MerItemList.Exists((BDItemModel x) => x.MItemID == checkValueModel.MMerItemID && x.MIsActive)))
						{
							checkValueModel.MMerItemID = null;
							checkValueModel.MMerItemName = null;
						}
						if (!string.IsNullOrWhiteSpace(checkValueModel.MPaItemGroupID) || !string.IsNullOrWhiteSpace(checkValueModel.MPaItemID))
						{
							List<PAPayItemGroupModel> allPayItemList = new PAPayItemGroupRepository().GetAllPayItemList(ctx, true);
							if (allPayItemList != null && allPayItemList.Count() > 0)
							{
								PAPayItemGroupModel pAPayItemGroupModel = allPayItemList.FirstOrDefault((PAPayItemGroupModel x) => !string.IsNullOrWhiteSpace(checkValueModel.MPaItemGroupID) && x.MItemID == checkValueModel.MPaItemGroupID);
								PAPayItemGroupModel pAPayItemGroupModel2 = allPayItemList.FirstOrDefault((PAPayItemGroupModel x) => !string.IsNullOrWhiteSpace(checkValueModel.MPaItemID) && x.MItemID == checkValueModel.MPaItemID);
								if (pAPayItemGroupModel != null && !pAPayItemGroupModel.MIsActive)
								{
									checkValueModel.MPaItemGroupID = null;
									checkValueModel.MPaItemGroupName = null;
								}
								if (pAPayItemGroupModel2 != null && !pAPayItemGroupModel2.MIsActive)
								{
									checkValueModel.MPaItemID = null;
									checkValueModel.MPaItemName = null;
								}
							}
						}
						if (!string.IsNullOrWhiteSpace(checkValueModel.MTrackItem1) && (list == null || list.Count == 0 || !list.Exists((GLTreeModel x) => x.id == checkValueModel.MTrackItem1 && x.MIsActive)))
						{
							checkValueModel.MTrackItem1 = null;
							checkValueModel.MTrackItem1Name = null;
						}
						if (!string.IsNullOrWhiteSpace(checkValueModel.MTrackItem2) && (list2 == null || list2.Count == 0 || !list2.Exists((GLTreeModel x) => x.id == checkValueModel.MTrackItem2 && x.MIsActive)))
						{
							checkValueModel.MTrackItem2 = null;
							checkValueModel.MTrackItem2Name = null;
						}
						if (!string.IsNullOrWhiteSpace(checkValueModel.MTrackItem3) && (list3 == null || list3.Count == 0 || !list3.Exists((GLTreeModel x) => x.id == checkValueModel.MTrackItem3 && x.MIsActive)))
						{
							checkValueModel.MTrackItem3 = null;
							checkValueModel.MTrackItem3Name = null;
						}
						if (!string.IsNullOrWhiteSpace(checkValueModel.MTrackItem4) && (list4 == null || list4.Count == 0 || !list4.Exists((GLTreeModel x) => x.id == checkValueModel.MTrackItem4 && x.MIsActive)))
						{
							checkValueModel.MTrackItem4 = null;
							checkValueModel.MTrackItem4Name = null;
						}
						if (!string.IsNullOrWhiteSpace(checkValueModel.MTrackItem5) && (list5 == null || list5.Count == 0 || !list5.Exists((GLTreeModel x) => x.id == checkValueModel.MTrackItem5 && x.MIsActive)))
						{
							checkValueModel.MTrackItem5 = null;
							checkValueModel.MTrackItem5Name = null;
						}
						item.MCheckGroupValueID = gLUtility.GetCheckGroupValueModel(ctx, checkValueModel).MItemID;
					}
				}
			}
		}

		public GLVoucherModel GetNewVoucherEditModel(MContext ctx, GLVoucherModel model)
		{
			GLVoucherModel gLVoucherModel = new GLVoucherModel();
			gLVoucherModel.MYear = model.MYear;
			gLVoucherModel.MPeriod = model.MPeriod;
			gLVoucherModel.MDate = model.MDate;
			gLVoucherModel.MStatus = -1;
			DateTime dateTime = model.MDate;
			if (dateTime.Year <= 1900)
			{
				int num = gLVoucherModel.MYear * 12 + gLVoucherModel.MPeriod;
				dateTime = ctx.DateNow;
				int num2 = dateTime.Year * 12;
				dateTime = ctx.DateNow;
				if (num == num2 + dateTime.Month || gLVoucherModel.MYear == 0 || gLVoucherModel.MPeriod == 0)
				{
					gLVoucherModel.MDate = ctx.DateNow;
				}
				else
				{
					int num3 = gLVoucherModel.MYear * 12 + gLVoucherModel.MPeriod;
					dateTime = ctx.DateNow;
					int num4 = dateTime.Year * 12;
					dateTime = ctx.DateNow;
					if (num3 < num4 + dateTime.Month)
					{
						GLVoucherModel gLVoucherModel2 = gLVoucherModel;
						dateTime = new DateTime(gLVoucherModel.MYear, gLVoucherModel.MPeriod, 1);
						dateTime = dateTime.AddMonths(1);
						gLVoucherModel2.MDate = dateTime.AddDays(-1.0);
					}
					else
					{
						gLVoucherModel.MDate = new DateTime(gLVoucherModel.MYear, gLVoucherModel.MPeriod, 1);
					}
				}
			}
			gLVoucherModel.MNumber = COMResourceHelper.GetNextVoucherNumber(ctx, gLVoucherModel.MYear, gLVoucherModel.MPeriod, null, null);
			gLVoucherModel.MTransferTypeID = -1;
			GLVoucherEntryModel item = new GLVoucherEntryModel
			{
				MAccountModel = new BDAccountModel
				{
					MCurrencyDataModel = new GLCurrencyDataModel(),
					MCheckGroupModel = new GLCheckGroupModel(),
					MCheckGroupValueModel = new GLCheckGroupValueModel()
				}
			};
			gLVoucherModel.MVoucherEntrys = new List<GLVoucherEntryModel>
			{
				item,
				item,
				item,
				item,
				item
			};
			return gLVoucherModel;
		}

		public List<GLVoucherEntryModel> GetVoucherEntryModelByBalance(MContext ctx, GLPeriodTransferModel periodTransferModel, List<GLBalanceModel> balanceList, List<GLCheckGroupModel> checkGroupList, List<BDAccountModel> accountList, BDAccountModel account, int MDC, bool deducation, string explanation)
		{
			GLUtility gLUtility = new GLUtility();
			List<GLVoucherEntryModel> list = new List<GLVoucherEntryModel>();
			if (balanceList == null || balanceList.Count == 0 || !balanceList.Exists((GLBalanceModel x) => x.MBeginBalance != decimal.Zero || x.MDebit != decimal.Zero || x.MCredit != decimal.Zero))
			{
				decimal num = default(decimal);
				decimal num2 = default(decimal);
				decimal exchangeRate = 1.0m;
				num = (num2 = periodTransferModel.MAmount);
				GLCheckGroupValueModel checkGroupValueModel = gLUtility.GetCheckGroupValueModel(ctx, null);
				list.Add(GetVoucherEntryModel(ctx, account, explanation, ctx.MBasCurrencyID, MDC, num, num2, exchangeRate, checkGroupValueModel.MItemID, checkGroupList));
			}
			else
			{
				decimal num3 = periodTransferModel.MAmount;
				int i = balanceList.Count - 1;
				while (i >= 0)
				{
					decimal num4 = default(decimal);
					decimal amountFor = default(decimal);
					decimal exchangeRate2 = 1.0m;
					account = accountList.FirstOrDefault((BDAccountModel x) => x.MItemID == balanceList[i].MAccountID);
					if (deducation)
					{
						decimal num5 = balanceList[i].MBeginBalance + (decimal)balanceList[i].MDC * (balanceList[i].MDebit - balanceList[i].MCredit);
						num4 = ((i != 0 || !(num3 != decimal.Zero)) ? ((num5 > num3) ? num3 : num5) : num3);
						num3 -= num4;
						if (num4 != decimal.Zero)
						{
							list.Add(GetVoucherEntryModel(ctx, account, explanation, ctx.MBasCurrencyID, MDC, num4, amountFor, exchangeRate2, balanceList[i].MCheckGroupValueID, checkGroupList));
						}
						i--;
						continue;
					}
					account = accountList.FirstOrDefault((BDAccountModel x) => x.MItemID == balanceList[0].MAccountID);
					num4 = num3;
					list.Add(GetVoucherEntryModel(ctx, account, explanation, ctx.MBasCurrencyID, MDC, num4, amountFor, exchangeRate2, balanceList[0].MCheckGroupValueID, checkGroupList));
					break;
				}
			}
			return list;
		}

		public GLVoucherEntryModel GetVoucherEntryModel(MContext ctx, BDAccountModel account, string explanation, string currencyID, int MDC, decimal amount, decimal amountFor, decimal exchangeRate, string checkGroupValueID, List<GLCheckGroupModel> checkGroupList)
		{
			GLCheckGroupValueModel checkGroupValueModelByID = utility.GetCheckGroupValueModelByID(ctx, checkGroupValueID);
			return new GLVoucherEntryModel
			{
				MAccountID = account.MItemID,
				MAmount = amount,
				MAmountFor = amount,
				MCheckGroupValueID = checkGroupValueID,
				MAccountFullName = account.MFullName,
				MExplanation = explanation,
				MDC = MDC,
				MDebit = ((MDC == 1) ? amount : decimal.Zero),
				MCredit = ((MDC == -1) ? amount : decimal.Zero),
				MCurrencyID = ctx.MBasCurrencyID,
				MExchangeRate = exchangeRate,
				MAccountModel = new BDAccountModel
				{
					MItemID = account.MItemID,
					MIsCheckForCurrency = account.MIsCheckForCurrency,
					MDC = account.MDC,
					MFullName = account.MFullName,
					MNumber = account.MNumber,
					MCurrencyDataModel = new GLCurrencyDataModel
					{
						MCurrencyID = ctx.MBasCurrencyID,
						MAmount = amount,
						MAmountFor = amountFor,
						MExchangeRate = exchangeRate
					},
					MCheckGroupModel = checkGroupList.FirstOrDefault((GLCheckGroupModel f) => f.MItemID == account.MCheckGroupID),
					MCheckGroupValueModel = checkGroupValueModelByID
				}
			};
		}

		public GLVoucherModel GetVoucherByPeriodTransfer(MContext ctx, GLPeriodTransferModel model)
		{
			GLVoucherModel voucherModel = GetVoucherModel(ctx, string.Empty, model.MYear, model.MPeriod, 0);
			voucherModel.MVoucherEntrys = new List<GLVoucherEntryModel>();
			voucherModel.MYear = model.MYear;
			voucherModel.MPeriod = model.MPeriod;
			voucherModel.MTransferTypeID = model.MTransferTypeID;
			GLVoucherModel gLVoucherModel = voucherModel;
			DateTime dateTime = new DateTime(voucherModel.MYear, voucherModel.MPeriod, 1);
			dateTime = dateTime.AddMonths(1);
			gLVoucherModel.MDate = dateTime.AddDays(-1.0);
			GLPeriodTransferModel periodTransfer = transferBiz.GetPeriodTransfer(ctx, model, true);
			List<BDAccountListModel> accountListIncludeBalance = accountBiz.GetAccountListIncludeBalance(ctx, new SqlWhere(), true);
			List<BDAccountModel> accountListWithCheckType = accountBiz.GetAccountListWithCheckType(ctx, null, false);
			List<GLCheckGroupModel> modelList = checkGroupBiz.GetModelList(ctx, new SqlWhere(), false);
			List<GLCheckGroupValueModel> checkGroupValueList = checkGroupValueBiz.GetCheckGroupValueList(ctx);
			BDAccountListModel account4;
			BDAccountListModel account3;
			switch (model.MTransferTypeID)
			{
			case 0:
				GetTransferCostVoucher(ctx, voucherModel, model, accountListWithCheckType, modelList);
				break;
			case 1:
			{
				account4 = accountBiz.GetLeafAccountByCode(accountListIncludeBalance, "660204");
				account3 = accountBiz.GetLeafAccountByCode(accountListIncludeBalance, "2211");
				GLVoucherEntryModel gLVoucherEntryModel3 = new GLVoucherEntryModel
				{
					MExplanation = lang.GetText(ctx, LangModule.GL, "AccrualPayroll4CurrentPeriod", "Accrual payroll for current period"),
					MDebit = model.MAmount,
					MCredit = decimal.Zero,
					MAmount = model.MAmount,
					MAmountFor = model.MAmount,
					MAccountID = account4.MItemID,
					MAccountName = account4.MFullName,
					MCurrencyID = ctx.MBasCurrencyID,
					MExchangeRate = 1.0m,
					MDC = 1,
					MEntrySeq = 0
				};
				GLVoucherEntryModel value2 = new GLVoucherEntryModel
				{
					MExplanation = lang.GetText(ctx, LangModule.GL, "AccrualPayroll4CurrentPeriod", "Accrual payroll for current period"),
					MCredit = model.MAmount,
					MDebit = decimal.Zero,
					MAmount = model.MAmount,
					MAmountFor = model.MAmount,
					MAccountID = account3.MItemID,
					MAccountName = account3.MFullName,
					MCurrencyID = ctx.MBasCurrencyID,
					MExchangeRate = 1.0m,
					MDC = -1,
					MEntrySeq = 1
				};
				voucherModel.MVoucherEntrys[0] = gLVoucherEntryModel3;
				voucherModel.MVoucherEntrys[1] = value2;
				break;
			}
			case 2:
			{
				BDAccountModel firstLeafAccountByCode3 = accountBiz.GetFirstLeafAccountByCode(ctx, "660205", accountListWithCheckType);
				List<BDAccountModel> allLeafAccountByCode = accountBiz.GetAllLeafAccountByCode(ctx, "1602", accountListWithCheckType);
				List<GLBalanceModel> balanceListByAccountIDs = balanceBiz.GetBalanceListByAccountIDs(ctx, (from f in allLeafAccountByCode
				select f.MItemID).ToList(), model.MYear, model.MPeriod, false);
				string text = lang.GetText(ctx, LangModule.GL, "AccrualDepreciations4CurrentPeriod", "Accrual depreciation for current period");
				voucherModel.MVoucherEntrys.AddRange(GetVoucherEntryModelByBalance(ctx, model, null, modelList, accountListWithCheckType, firstLeafAccountByCode3, 1, false, text));
				voucherModel.MVoucherEntrys.AddRange(GetVoucherEntryModelByBalance(ctx, model, balanceListByAccountIDs, modelList, accountListWithCheckType, allLeafAccountByCode[0], -1, false, text));
				break;
			}
			case 3:
			{
				List<BDAccountModel> allLeafAccountByCode2 = accountBiz.GetAllLeafAccountByCode(ctx, "660207", accountListWithCheckType);
				List<BDAccountModel> allLeafAccountByCode3 = accountBiz.GetAllLeafAccountByCode(ctx, "1801", accountListWithCheckType);
				List<GLBalanceModel> balanceListByAccountIDs2 = balanceBiz.GetBalanceListByAccountIDs(ctx, (from x in allLeafAccountByCode2
				select x.MItemID).ToList(), model.MYear, model.MPeriod, true);
				List<GLBalanceModel> balanceListByAccountIDs3 = balanceBiz.GetBalanceListByAccountIDs(ctx, (from x in allLeafAccountByCode3
				select x.MItemID).ToList(), model.MYear, model.MPeriod, true);
				string text = lang.GetText(ctx, LangModule.GL, "AmortizationPrepaidExpense4CurrentPeriod", "Amortize of deferred expenses for current period");
				voucherModel.MVoucherEntrys.AddRange(GetVoucherEntryModelByBalance(ctx, model, balanceListByAccountIDs2, modelList, accountListWithCheckType, allLeafAccountByCode2[0], 1, false, text));
				voucherModel.MVoucherEntrys.AddRange(GetVoucherEntryModelByBalance(ctx, model, balanceListByAccountIDs3, modelList, accountListWithCheckType, allLeafAccountByCode3[0], -1, true, text));
				break;
			}
			case 4:
			{
				string text = lang.GetText(ctx, LangModule.GL, "AccrualTax4CurrentPeriod", "Accrual tax for current period");
				GLPeriodTransferModel lastTaxModel = transferBiz.GetLastTaxModel(ctx, model);
				GLPeriodTransferModel gLPeriodTransferModel = periodTransfer ?? lastTaxModel;
				if (gLPeriodTransferModel != null)
				{
					GLVoucherModel gLVoucherModel2 = dal.GetVoucherModelList(ctx, new List<string>
					{
						lastTaxModel.MVoucherID
					}, false, 0, 0).FirstOrDefault();
					List<GLVoucherEntryModel> voucherEntrys = gLVoucherModel2.MVoucherEntrys;
					int i;
					for (i = 0; i < voucherEntrys.Count; i++)
					{
						decimal num5 = voucherEntrys[i].MAmount / gLVoucherModel2.MCreditTotal;
						BDAccountListModel account = GetAccountByItemID(accountListIncludeBalance, voucherEntrys[i].MAccountID);
						GLVoucherEntryModel gLVoucherEntryModel3 = new GLVoucherEntryModel
						{
							MExplanation = voucherEntrys[i].MExplanation,
							MDebit = ((voucherEntrys[i].MDC == -1) ? decimal.Zero : model.MAmount),
							MCredit = ((voucherEntrys[i].MDC == 1) ? decimal.Zero : (voucherEntrys[i].MCredit / gLVoucherModel2.MCreditTotal * model.MAmount)),
							MAccountID = voucherEntrys[i].MAccountID,
							MAccountModel = new BDAccountModel
							{
								MItemID = account.MItemID,
								MIsCheckForCurrency = account.MIsCheckForCurrency,
								MDC = account.MDC,
								MFullName = account.MFullName,
								MNumber = account.MNumber,
								MCurrencyDataModel = new GLCurrencyDataModel
								{
									MCurrencyID = voucherEntrys[i].MCurrencyID,
									MExchangeRate = voucherEntrys[i].MExchangeRate
								},
								MCheckGroupModel = modelList.FirstOrDefault((GLCheckGroupModel f) => f.MItemID == account.MCheckGroupID),
								MCheckGroupValueModel = (checkGroupValueList.FirstOrDefault((GLCheckGroupValueModel f) => f.MItemID == voucherEntrys[i].MCheckGroupValueID) ?? new GLCheckGroupValueModel())
							},
							MAccountName = GetAccountByItemID(accountListIncludeBalance, voucherEntrys[i].MAccountID).MFullName,
							MCurrencyID = voucherEntrys[i].MCurrencyID,
							MExchangeRate = voucherEntrys[i].MExchangeRate,
							MDC = voucherEntrys[i].MDC,
							MEntrySeq = i
						};
						gLVoucherEntryModel3.MAmount = ((gLVoucherEntryModel3.MCredit != decimal.Zero) ? gLVoucherEntryModel3.MCredit : gLVoucherEntryModel3.MDebit);
						gLVoucherEntryModel3.MAmountFor = gLVoucherEntryModel3.MAmount;
						gLVoucherEntryModel3.MAccountModel.MCurrencyDataModel.MAmount = gLVoucherEntryModel3.MAmount;
						gLVoucherEntryModel3.MAccountModel.MCurrencyDataModel.MAmountFor = gLVoucherEntryModel3.MAmountFor;
						voucherModel.MVoucherEntrys[i] = gLVoucherEntryModel3;
					}
				}
				else
				{
					BDAccountModel firstLeafAccountByCode4 = accountBiz.GetFirstLeafAccountByCode(ctx, "6403", accountListWithCheckType);
					BDAccountModel firstLeafAccountByCode5 = accountBiz.GetFirstLeafAccountByCode(ctx, "222108", accountListWithCheckType);
					BDAccountModel firstLeafAccountByCode6 = accountBiz.GetFirstLeafAccountByCode(ctx, "222113", accountListWithCheckType);
					List<GLBalanceModel> checkGroupBalanceByAccountID3 = balanceBiz.GetCheckGroupBalanceByAccountID(ctx, firstLeafAccountByCode4.MItemID, model.MYear, model.MPeriod);
					List<GLBalanceModel> checkGroupBalanceByAccountID4 = balanceBiz.GetCheckGroupBalanceByAccountID(ctx, firstLeafAccountByCode5.MItemID, model.MYear, model.MPeriod);
					List<GLBalanceModel> checkGroupBalanceByAccountID5 = balanceBiz.GetCheckGroupBalanceByAccountID(ctx, firstLeafAccountByCode6.MItemID, model.MYear, model.MPeriod);
					voucherModel.MVoucherEntrys.AddRange(GetVoucherEntryModelByBalance(ctx, model, checkGroupBalanceByAccountID3, modelList, accountListWithCheckType, firstLeafAccountByCode4, 1, false, text));
					voucherModel.MVoucherEntrys.AddRange(GetVoucherEntryModelByBalance(ctx, model, checkGroupBalanceByAccountID4, modelList, accountListWithCheckType, firstLeafAccountByCode5, -1, false, text));
					voucherModel.MVoucherEntrys.AddRange(GetVoucherEntryModelByBalance(ctx, model, checkGroupBalanceByAccountID5, modelList, accountListWithCheckType, firstLeafAccountByCode6, -1, false, text));
				}
				break;
			}
			case 5:
			{
				string text = lang.GetText(ctx, LangModule.GL, "CarryForwardOutOfVAT4CurrentPeriod", "Carry forward Unpaid VAT for current period");
				BDAccountModel firstLeafAccountByCode7 = accountBiz.GetFirstLeafAccountByCode(ctx, "22210103", accountListWithCheckType);
				List<BDAccountModel> allLeafAccountByCode4 = accountBiz.GetAllLeafAccountByCode(ctx, "222102", accountListWithCheckType);
				List<GLBalanceModel> balanceListByAccountIDs4 = balanceBiz.GetBalanceListByAccountIDs(ctx, (from f in allLeafAccountByCode4
				select f.MItemID).ToList(), model.MYear, model.MPeriod, true);
				voucherModel.MVoucherEntrys.AddRange(GetVoucherEntryModelByBalance(ctx, model, null, modelList, accountListWithCheckType, firstLeafAccountByCode7, 1, false, text));
				voucherModel.MVoucherEntrys.AddRange(GetVoucherEntryModelByBalance(ctx, model, balanceListByAccountIDs4, modelList, accountListWithCheckType, allLeafAccountByCode4[0], -1, false, text));
				break;
			}
			case 9:
			{
				voucherModel.MVoucherEntrys = new List<GLVoucherEntryModel>();
				account4 = accountBiz.GetLeafAccountByCode(accountListIncludeBalance, "660302");
				List<BDExchangeRateModel> monthlyExchangeRateList = rateBiz.GetMonthlyExchangeRateList(ctx, new DateTime(model.MYear, model.MPeriod, 1));
				SqlWhere sqlWhere = new SqlWhere();
				sqlWhere.AddFilter("a.MIsCheckForCurrency", SqlOperators.Equal, "1");
				List<BDAccountListModel> accountListIncludeBalance2 = accountBiz.GetAccountListIncludeBalance(ctx, sqlWhere, false);
				accountListIncludeBalance2 = (from x in accountListIncludeBalance2
				where x.MAccountGroupID.Split(',').Contains("1") || x.MAccountGroupID.Split(',').Contains("2")
				select x).ToList();
				List<GLBalanceModel> balanceByPeriods = balanceBiz.GetBalanceByPeriods(ctx, new List<DateTime>
				{
					new DateTime(model.MYear, model.MPeriod, 1)
				}, (from x in accountListIncludeBalance2
				select x.MItemID).ToList());
				balanceByPeriods = balanceBiz.GetGroupBalanceList(ctx, balanceByPeriods, true);
				decimal num = default(decimal);
				for (int j = 0; j < balanceByPeriods.Count; j++)
				{
					GLBalanceModel balance = balanceByPeriods[j];
					if ((!(balance.MCheckGroupValueID == "0") || (from x in balanceByPeriods
					where x.MCheckGroupValueID != "0" && x.MAccountID == balance.MAccountID
					select x).Count() <= 0) && balance.MCurrencyID != ctx.MBasCurrencyID)
					{
						bool flag = balanceByPeriods[j].MDC == 1;
						decimal mRate = monthlyExchangeRateList.FirstOrDefault((BDExchangeRateModel x) => x.MTargetCurrencyID == balance.MCurrencyID).MRate;
						decimal d = (balance.MDebitFor + (flag ? balance.MBeginBalanceFor : decimal.Zero)) * mRate - (balance.MDebit + (flag ? balance.MBeginBalance : decimal.Zero));
						decimal d2 = (balance.MCreditFor + ((!flag) ? balance.MBeginBalanceFor : decimal.Zero)) * mRate - (balance.MCredit + ((!flag) ? balance.MBeginBalance : decimal.Zero));
						d = Math.Round(d, 2, MidpointRounding.AwayFromZero);
						d2 = Math.Round(d2, 2, MidpointRounding.AwayFromZero);
						BDAccountListModel account2 = accountListIncludeBalance2.FirstOrDefault((BDAccountListModel f) => f.MItemID == balance.MAccountID);
						if (d - d2 != decimal.Zero)
						{
							decimal num2 = (d - d2) * (decimal)account2.MDC;
							int num3 = ((num2 > decimal.Zero) ? 1 : (-1)) * account2.MDC;
							int value = 0;
							decimal num4 = Math.Abs(num2);
							num += num2 * (decimal)account2.MDC * decimal.MinusOne;
							GLVoucherEntryModel item = new GLVoucherEntryModel
							{
								MExplanation = lang.GetText(ctx, LangModule.GL, "FinalTransfer", "期末调汇"),
								MDebit = ((num3 == 1) ? num4 : decimal.Zero),
								MCredit = ((num3 == -1) ? num4 : decimal.Zero),
								MAmount = num4,
								MAmountFor = (decimal)value,
								MAccountID = balance.MAccountID,
								MAccountModel = new BDAccountModel
								{
									MItemID = account2.MItemID,
									MIsCheckForCurrency = account2.MIsCheckForCurrency,
									MDC = account2.MDC,
									MFullName = account2.MFullName,
									MNumber = account2.MNumber,
									MCurrencyDataModel = new GLCurrencyDataModel
									{
										MCurrencyID = balance.MCurrencyID,
										MAmount = num4,
										MAmountFor = (decimal)value,
										MExchangeRate = mRate
									},
									MCheckGroupModel = modelList.FirstOrDefault((GLCheckGroupModel f) => f.MItemID == account2.MCheckGroupID),
									MCheckGroupValueModel = (checkGroupValueList.FirstOrDefault((GLCheckGroupValueModel f) => f.MItemID == balance.MCheckGroupValueID) ?? new GLCheckGroupValueModel())
								},
								MCurrencyID = balance.MCurrencyID,
								MIsCheckForCurrency = true,
								MExchangeRate = mRate,
								MDC = num3,
								MAccountName = account2.MFullName,
								MEntrySeq = voucherModel.MVoucherEntrys.Count
							};
							voucherModel.MVoucherEntrys.Add(item);
						}
					}
				}
				if (voucherModel.MVoucherEntrys.Count > 0 && num != decimal.Zero)
				{
					GLVoucherEntryModel item2 = new GLVoucherEntryModel
					{
						MExplanation = lang.GetText(ctx, LangModule.GL, "FinalTransfer", "期末调汇"),
						MDebit = num,
						MCredit = decimal.Zero,
						MAmount = num,
						MAmountFor = num,
						MAccountID = account4.MItemID,
						MAccountModel = new BDAccountModel
						{
							MItemID = account4.MItemID,
							MIsCheckForCurrency = account4.MIsCheckForCurrency,
							MDC = account4.MDC,
							MFullName = account4.MFullName,
							MNumber = account4.MNumber,
							MCurrencyDataModel = new GLCurrencyDataModel
							{
								MCurrencyID = ctx.MBasCurrencyID,
								MAmount = num,
								MAmountFor = decimal.Zero,
								MExchangeRate = 1.0m
							},
							MCheckGroupModel = modelList.FirstOrDefault((GLCheckGroupModel f) => f.MItemID == account4.MCheckGroupID),
							MCheckGroupValueModel = new GLCheckGroupValueModel()
						},
						MAccountName = account4.MFullName,
						MDC = 1,
						MEntrySeq = voucherModel.MVoucherEntrys.Count
					};
					voucherModel.MVoucherEntrys.Add(item2);
				}
				break;
			}
			case 6:
			{
				string text = lang.GetText(ctx, LangModule.GL, "AccrualIncomeTax4CurrentPeriod", "Accrual income tax for current period");
				BDAccountModel firstLeafAccountByCode = accountBiz.GetFirstLeafAccountByCode(ctx, "6801", accountListWithCheckType);
				BDAccountModel firstLeafAccountByCode2 = accountBiz.GetFirstLeafAccountByCode(ctx, "222106", accountListWithCheckType);
				List<GLBalanceModel> checkGroupBalanceByAccountID2 = balanceBiz.GetCheckGroupBalanceByAccountID(ctx, firstLeafAccountByCode2.MItemID, model.MYear, model.MPeriod);
				voucherModel.MVoucherEntrys.AddRange(GetVoucherEntryModelByBalance(ctx, model, null, modelList, accountListWithCheckType, firstLeafAccountByCode, 1, false, text));
				voucherModel.MVoucherEntrys.AddRange(GetVoucherEntryModelByBalance(ctx, model, checkGroupBalanceByAccountID2, modelList, accountListWithCheckType, firstLeafAccountByCode2, -1, false, text));
				break;
			}
			case 7:
				TransferProfitLoss(ctx, voucherModel, accountListIncludeBalance, model.MYear, model.MPeriod, modelList, checkGroupValueList);
				break;
			case 8:
			{
				account4 = accountBiz.GetLeafAccountByCode(accountListIncludeBalance, "4103");
				account3 = accountBiz.GetLeafAccountByCode(accountListIncludeBalance, "410407");
				List<GLBalanceModel> checkGroupBalanceByAccountID = balanceBiz.GetCheckGroupBalanceByAccountID(ctx, account4.MItemID, model.MYear, model.MPeriod);
				voucherModel.MVoucherEntrys = new List<GLVoucherEntryModel>();
				foreach (GLBalanceModel item3 in checkGroupBalanceByAccountID)
				{
					GLVoucherEntryModel gLVoucherEntryModel = new GLVoucherEntryModel
					{
						MExplanation = lang.GetText(ctx, LangModule.GL, "CarryForwardUndistributedProfits4CurrentPeriod", "结转本期未分配利润"),
						MDebit = ((model.MAmount > decimal.Zero) ? model.MAmount : decimal.Zero),
						MCredit = ((model.MAmount < decimal.Zero) ? Math.Abs(model.MAmount) : decimal.Zero),
						MAmount = Math.Abs(model.MAmount),
						MAmountFor = Math.Abs(model.MAmount),
						MAccountID = account4.MItemID,
						MAccountModel = new BDAccountModel
						{
							MItemID = account4.MItemID,
							MIsCheckForCurrency = account4.MIsCheckForCurrency,
							MDC = account4.MDC,
							MFullName = account4.MFullName,
							MNumber = account4.MNumber,
							MCurrencyDataModel = new GLCurrencyDataModel
							{
								MCurrencyID = ctx.MBasCurrencyID,
								MExchangeRate = 1.0m
							},
							MCheckGroupModel = modelList.FirstOrDefault((GLCheckGroupModel f) => f.MItemID == account4.MCheckGroupID),
							MCheckGroupValueModel = (checkGroupValueList.FirstOrDefault((GLCheckGroupValueModel f) => f.MItemID == item3.MCheckGroupValueID) ?? new GLCheckGroupValueModel())
						},
						MAccountName = account4.MFullName,
						MCurrencyID = ctx.MBasCurrencyID,
						MExchangeRate = 1.0m,
						MDC = ((model.MAmount > decimal.Zero) ? 1 : (-1)),
						MEntrySeq = voucherModel.MVoucherEntrys.Count
					};
					gLVoucherEntryModel.MAccountModel.MCurrencyDataModel.MAmount = gLVoucherEntryModel.MAmount;
					gLVoucherEntryModel.MAccountModel.MCurrencyDataModel.MAmountFor = gLVoucherEntryModel.MAmountFor;
					voucherModel.MVoucherEntrys.Add(gLVoucherEntryModel);
				}
				GLVoucherEntryModel gLVoucherEntryModel2 = new GLVoucherEntryModel
				{
					MExplanation = lang.GetText(ctx, LangModule.GL, "CarryForwardUndistributedProfits4CurrentPeriod", "结转本期未分配利润"),
					MCredit = ((model.MAmount > decimal.Zero) ? model.MAmount : decimal.Zero),
					MDebit = ((model.MAmount < decimal.Zero) ? Math.Abs(model.MAmount) : decimal.Zero),
					MAmount = Math.Abs(model.MAmount),
					MAmountFor = Math.Abs(model.MAmount),
					MAccountID = account3.MItemID,
					MAccountModel = new BDAccountModel
					{
						MItemID = account3.MItemID,
						MIsCheckForCurrency = account3.MIsCheckForCurrency,
						MDC = account3.MDC,
						MFullName = account3.MFullName,
						MNumber = account3.MNumber,
						MCurrencyDataModel = new GLCurrencyDataModel
						{
							MCurrencyID = ctx.MBasCurrencyID,
							MExchangeRate = 1.0m
						},
						MCheckGroupModel = modelList.FirstOrDefault((GLCheckGroupModel f) => f.MItemID == account3.MCheckGroupID),
						MCheckGroupValueModel = new GLCheckGroupValueModel()
					},
					MAccountName = account3.MFullName,
					MCurrencyID = ctx.MBasCurrencyID,
					MExchangeRate = 1.0m,
					MDC = ((!(model.MAmount > decimal.Zero)) ? 1 : (-1)),
					MEntrySeq = voucherModel.MVoucherEntrys.Count
				};
				gLVoucherEntryModel2.MAccountModel.MCurrencyDataModel.MAmount = gLVoucherEntryModel2.MAmount;
				gLVoucherEntryModel2.MAccountModel.MCurrencyDataModel.MAmountFor = gLVoucherEntryModel2.MAmountFor;
				voucherModel.MVoucherEntrys.Add(gLVoucherEntryModel2);
				break;
			}
			}
			for (int k = voucherModel.MVoucherEntrys.Count; k < 5; k++)
			{
				voucherModel.MVoucherEntrys.Add(new GLVoucherEntryModel
				{
					MAccountModel = new BDAccountModel()
				});
			}
			return voucherModel;
		}

		private void GetTransferCostVoucher(MContext ctx, GLVoucherModel voucher, GLPeriodTransferModel model, List<BDAccountModel> accountList, List<GLCheckGroupModel> checkGroupList)
		{
			List<BDAccountModel> mainBusinessCostAccountList = new List<BDAccountModel>();
			List<BDAccountModel> mainBusinessIncomeAccountList = new List<BDAccountModel>();
			List<BDAccountModel> merchandiseInventoryAccountList = new List<BDAccountModel>();
			BDAccountModel mainBusinessCostAccount = new BDAccountModel();
			BDAccountModel bDAccountModel = new BDAccountModel();
			BDAccountModel merchandiseInventoryAccount = new BDAccountModel();
			string explanation = lang.GetText(ctx, LangModule.GL, "CarryForwardCostOfSale4CurrentPeriod", "Carry forward the cost of sales for current period");
			accountList.ForEach(delegate(BDAccountModel x)
			{
				if (x.MCode.IndexOf("6401") == 0)
				{
					mainBusinessCostAccountList.Add(x);
				}
				else if (x.MCode.IndexOf("6001") == 0)
				{
					mainBusinessIncomeAccountList.Add(x);
				}
				else if (x.MCode.IndexOf("1405") == 0)
				{
					merchandiseInventoryAccountList.Add(x);
				}
			});
			if (mainBusinessCostAccountList.Count > 1 || merchandiseInventoryAccountList.Count > 1 || mainBusinessIncomeAccountList.Count > 1)
			{
				List<GLBalanceModel> balanceListByAccountIDs = balanceBiz.GetBalanceListByAccountIDs(ctx, (from f in merchandiseInventoryAccountList
				select f.MItemID).ToList(), model.MYear, model.MPeriod, false);
				voucher.MVoucherEntrys.AddRange(GetVoucherEntryModelByBalance(ctx, model, null, checkGroupList, accountList, mainBusinessCostAccountList[0], 1, false, explanation));
				voucher.MVoucherEntrys.AddRange(GetVoucherEntryModelByBalance(ctx, model, balanceListByAccountIDs, checkGroupList, accountList, merchandiseInventoryAccountList[0], -1, true, explanation));
			}
			else
			{
				mainBusinessCostAccount = mainBusinessCostAccountList[0];
				bDAccountModel = mainBusinessIncomeAccountList[0];
				merchandiseInventoryAccount = merchandiseInventoryAccountList[0];
				SqlWhere sqlWhere = new SqlWhere();
				sqlWhere.Equal("MAccountID", bDAccountModel.MItemID).Equal("MYear", model.MYear).Equal("MPeriod", model.MPeriod)
					.NotEqual("MCheckGroupValueID", "0")
					.Equal("MCurrencyID", ctx.MBasCurrencyID);
				List<GLBalanceModel> balanceListIncludeCheckGroupValue = balanceRep.GetBalanceListIncludeCheckGroupValue(ctx, sqlWhere, true);
				List<GLBalanceModel> list = new List<GLBalanceModel>();
				List<GLBalanceModel> list2 = new List<GLBalanceModel>();
				for (int i = 0; i < balanceListIncludeCheckGroupValue.Count; i++)
				{
					GLCheckGroupModel checkGroup = checkGroupList.FirstOrDefault((GLCheckGroupModel x) => x.MItemID == mainBusinessCostAccount.MCheckGroupID);
					GLCheckGroupModel checkGroup2 = checkGroupList.FirstOrDefault((GLCheckGroupModel x) => x.MItemID == merchandiseInventoryAccount.MCheckGroupID);
					GLBalanceModel item = MergeBalanceWithCheckGroup(ctx, balanceListIncludeCheckGroupValue[i], checkGroup);
					GLBalanceModel item2 = MergeBalanceWithCheckGroup(ctx, balanceListIncludeCheckGroupValue[i], checkGroup2);
					list.Add(item);
					list2.Add(item2);
				}
				IEnumerable<GLVoucherEntryModel> source = from x in list
				group x by new
				{
					x.MCheckGroupValueID
				} into y
				select new GLVoucherEntryModel
				{
					MExplanation = explanation,
					MAccountID = mainBusinessCostAccount.MItemID,
					MCheckGroupValueID = y.Key.MCheckGroupValueID,
					MDebit = Math.Round(y.Sum((GLBalanceModel z) => z.MCredit - z.MDebit) * model.MPercent0 / 100m, 6),
					MCredit = decimal.Zero,
					MCheckGroupValueModel = new GLUtility().GetCheckGroupValueModelByID(ctx, y.Key.MCheckGroupValueID),
					MAccountModel = new BDAccountModel
					{
						MItemID = mainBusinessCostAccount.MItemID,
						MIsCheckForCurrency = mainBusinessCostAccount.MIsCheckForCurrency,
						MDC = mainBusinessCostAccount.MDC,
						MIsBankAccount = mainBusinessCostAccount.MIsBankAccount,
						MFullName = mainBusinessCostAccount.MFullName,
						MNumber = mainBusinessCostAccount.MNumber,
						MCheckGroupID = mainBusinessCostAccount.MCheckGroupID,
						MCheckGroupValueModel = new GLUtility().GetCheckGroupValueModelByID(ctx, y.Key.MCheckGroupValueID),
						MCheckGroupModel = new GLUtility().GetCheckGroupModelByID(ctx, mainBusinessCostAccount.MCheckGroupID),
						MCurrencyDataModel = new GLCurrencyDataModel
						{
							MCurrencyID = ctx.MBasCurrencyID,
							MAmount = Math.Round(y.Sum((GLBalanceModel z) => z.MCredit - z.MDebit) * model.MPercent0 / 100m, 6),
							MAmountFor = Math.Round(y.Sum((GLBalanceModel z) => z.MCredit - z.MDebit) * model.MPercent0 / 100m, 6)
						}
					},
					MDC = 1
				};
				IEnumerable<GLVoucherEntryModel> source2 = from x in list2
				group x by new
				{
					x.MCheckGroupValueID
				} into y
				select new GLVoucherEntryModel
				{
					MExplanation = explanation,
					MAccountID = merchandiseInventoryAccount.MItemID,
					MCheckGroupValueID = y.Key.MCheckGroupValueID,
					MDebit = decimal.Zero,
					MCredit = Math.Round(y.Sum((GLBalanceModel z) => z.MCredit - z.MDebit) * model.MPercent0 / 100m, 6),
					MCheckGroupValueModel = new GLUtility().GetCheckGroupValueModelByID(ctx, y.Key.MCheckGroupValueID),
					MAccountModel = new BDAccountModel
					{
						MItemID = merchandiseInventoryAccount.MItemID,
						MIsCheckForCurrency = merchandiseInventoryAccount.MIsCheckForCurrency,
						MDC = merchandiseInventoryAccount.MDC,
						MIsBankAccount = merchandiseInventoryAccount.MIsBankAccount,
						MFullName = merchandiseInventoryAccount.MFullName,
						MNumber = merchandiseInventoryAccount.MNumber,
						MCheckGroupID = merchandiseInventoryAccount.MCheckGroupID,
						MCheckGroupModel = new GLUtility().GetCheckGroupModelByID(ctx, merchandiseInventoryAccount.MCheckGroupID),
						MCheckGroupValueModel = new GLUtility().GetCheckGroupValueModelByID(ctx, y.Key.MCheckGroupValueID),
						MCurrencyDataModel = new GLCurrencyDataModel
						{
							MCurrencyID = ctx.MBasCurrencyID,
							MAmount = Math.Round(y.Sum((GLBalanceModel z) => z.MCredit - z.MDebit) * model.MPercent0 / 100m, 6),
							MAmountFor = Math.Round(y.Sum((GLBalanceModel z) => z.MCredit - z.MDebit) * model.MPercent0 / 100m, 6)
						}
					},
					MDC = -1
				};
				voucher.MVoucherEntrys.AddRange(from x in source
				where !(x.MDebit == decimal.Zero) || !(x.MCredit == decimal.Zero)
				select x);
				voucher.MVoucherEntrys.AddRange(from x in source2
				where !(x.MDebit == decimal.Zero) || !(x.MCredit == decimal.Zero)
				select x);
			}
		}

		public GLBalanceModel MergeBalanceWithCheckGroup(MContext ctx, GLBalanceModel balance, GLCheckGroupModel checkGroup)
		{
			GLBalanceModel gLBalanceModel = new GLBalanceModel();
			GLCheckGroupValueModel gLCheckGroupValueModel = new GLCheckGroupValueModel();
			GLCheckGroupValueModel mCheckGroupValueModel = balance.MCheckGroupValueModel;
			if (!string.IsNullOrWhiteSpace(mCheckGroupValueModel.MContactID) && IsCheckGroupEnable(checkGroup.MContactID))
			{
				gLCheckGroupValueModel.MContactID = mCheckGroupValueModel.MContactID;
			}
			if (!string.IsNullOrWhiteSpace(mCheckGroupValueModel.MEmployeeID) && IsCheckGroupEnable(checkGroup.MEmployeeID))
			{
				gLCheckGroupValueModel.MEmployeeID = mCheckGroupValueModel.MEmployeeID;
			}
			if (!string.IsNullOrWhiteSpace(mCheckGroupValueModel.MMerItemID) && IsCheckGroupEnable(checkGroup.MMerItemID))
			{
				gLCheckGroupValueModel.MMerItemID = mCheckGroupValueModel.MMerItemID;
			}
			if (!string.IsNullOrWhiteSpace(mCheckGroupValueModel.MExpItemID) && IsCheckGroupEnable(checkGroup.MExpItemID))
			{
				gLCheckGroupValueModel.MExpItemID = mCheckGroupValueModel.MExpItemID;
			}
			if (!string.IsNullOrWhiteSpace(mCheckGroupValueModel.MPaItemID) && IsCheckGroupEnable(checkGroup.MPaItemID))
			{
				gLCheckGroupValueModel.MPaItemID = mCheckGroupValueModel.MPaItemID;
			}
			if (!string.IsNullOrWhiteSpace(mCheckGroupValueModel.MTrackItem1) && IsCheckGroupEnable(checkGroup.MTrackItem1))
			{
				gLCheckGroupValueModel.MTrackItem1 = mCheckGroupValueModel.MTrackItem1;
			}
			if (!string.IsNullOrWhiteSpace(mCheckGroupValueModel.MTrackItem2) && IsCheckGroupEnable(checkGroup.MTrackItem2))
			{
				gLCheckGroupValueModel.MTrackItem2 = mCheckGroupValueModel.MTrackItem2;
			}
			if (!string.IsNullOrWhiteSpace(mCheckGroupValueModel.MTrackItem3) && IsCheckGroupEnable(checkGroup.MTrackItem3))
			{
				gLCheckGroupValueModel.MTrackItem3 = mCheckGroupValueModel.MTrackItem3;
			}
			if (!string.IsNullOrWhiteSpace(mCheckGroupValueModel.MTrackItem4) && IsCheckGroupEnable(checkGroup.MTrackItem4))
			{
				gLCheckGroupValueModel.MTrackItem4 = mCheckGroupValueModel.MTrackItem4;
			}
			if (!string.IsNullOrWhiteSpace(mCheckGroupValueModel.MTrackItem5) && IsCheckGroupEnable(checkGroup.MTrackItem5))
			{
				gLCheckGroupValueModel.MTrackItem5 = mCheckGroupValueModel.MTrackItem5;
			}
			gLCheckGroupValueModel = new GLUtility().GetCheckGroupValueModel(ctx, gLCheckGroupValueModel);
			return new GLBalanceModel
			{
				MCredit = balance.MCredit,
				MDebit = balance.MDebit,
				MCreditFor = balance.MCreditFor,
				MDebitFor = balance.MDebitFor,
				MBeginBalance = balance.MBeginBalance,
				MBeginBalanceFor = balance.MBeginBalanceFor,
				MEndBalance = balance.MEndBalance,
				MEndBalanceFor = balance.MEndBalanceFor,
				MAccountID = balance.MAccountID,
				MCheckGroupValueID = gLCheckGroupValueModel.MItemID,
				MCheckGroupValueModel = gLCheckGroupValueModel,
				MYear = balance.MYear,
				MPeriod = balance.MPeriod,
				MYearPeriod = balance.MYear,
				MYtdCredit = balance.MYtdCredit,
				MYtdCreditFor = balance.MYtdCreditFor,
				MYtdDebit = balance.MYtdDebit,
				MYtdDebitFor = balance.MYtdDebitFor
			};
		}

		public bool IsCheckGroupEnable(int value)
		{
			return value == CheckTypeStatusEnum.Optional || value == CheckTypeStatusEnum.Required;
		}

		public GLVoucherModel UpdateVoucher(MContext ctx, GLVoucherModel voucher)
		{
			OperationResult operationResult = dal.UpdateVoucher(ctx, voucher, null);
			voucher.MVoucherEntrys = null;
			return voucher;
		}

		public void PreHandleVouchers(MContext ctx, List<GLVoucherModel> vouchers)
		{
			newDal.PreHandleVouchers(ctx, vouchers);
		}

		private List<MActionResultCodeEnum> ValidateVoucher(MContext ctx, List<GLVoucherModel> vouchers, int status = 0)
		{
			return newDal.ValidateVouchers(ctx, vouchers, status, -2, false, false, true, true);
		}

		public GLDashboardModel GetDashboardData(MContext ctx, int year, int period, int type = 0)
		{
			return dal.GetDashboardData(ctx, year, period, 0);
		}

		public OperationResult ApproveVouchers(MContext ctx, List<string> ids, string status)
		{
			return dal.ApproveVouchers(ctx, ids, status);
		}

		public OperationResult UpdateVouchers(MContext ctx, List<GLVoucherModel> vouchers, List<CommandInfo> list = null, int nowStatus = -2, int oldStatus = -2)
		{
			return dal.UpdateVouchers(ctx, vouchers, list, nowStatus, oldStatus, true, null, false);
		}

		public GLDashboardInfoModel GetDashboardInfo(MContext ctx)
		{
			SECPermissionBusiness sECPermissionBusiness = new SECPermissionBusiness();
			GLDashboardInfoModel gLDashboardInfoModel = new GLDashboardInfoModel();
			gLDashboardInfoModel.CanEdit = sECPermissionBusiness.HavePermission(ctx, "General_Ledger", "Change", "");
			gLDashboardInfoModel.CanView = sECPermissionBusiness.HavePermission(ctx, "General_Ledger", "View", "");
			gLDashboardInfoModel.CanExport = sECPermissionBusiness.HavePermission(ctx, "General_Ledger", "Export", "");
			gLDashboardInfoModel.CanImport = sECPermissionBusiness.HavePermission(ctx, "General_Ledger", "Change", "");
			gLDashboardInfoModel.CanViewReport = sECPermissionBusiness.HavePermission(ctx, "Financial_Reports", "View", "");
			gLDashboardInfoModel.LastFinishedPeriod = new GLSettlementBusiness().GetLastFinishedPeriod(ctx);
			gLDashboardInfoModel.FixAssetsLastFinishedPeriod = new FAFixAssetsBusiness().GetLastFinishedPeriod(ctx);
			gLDashboardInfoModel.LangID = ((ctx.MLCID == LangCodeEnum.EN_US) ? "en" : ((ctx.MLCID == LangCodeEnum.ZH_CN) ? "zh-cn" : "zh-tw"));
			gLDashboardInfoModel.IsUDAS = (ctx.MAccountTableID == "3");
			gLDashboardInfoModel.CanApprove = sECPermissionBusiness.HavePermission(ctx, "General_Ledger", "Approve", "");
			gLDashboardInfoModel.CanChangeAttachment = sECPermissionBusiness.HavePermission(ctx, "Attachment", "Change", "");
			gLDashboardInfoModel.CanEditFixAssets = sECPermissionBusiness.HavePermission(ctx, "Fixed_Assets", "Change", "");
			gLDashboardInfoModel.CanViewFixAssets = sECPermissionBusiness.HavePermission(ctx, "Fixed_Assets", "View", "");
			gLDashboardInfoModel.CanApproveFixAssets = sECPermissionBusiness.HavePermission(ctx, "Fixed_Assets", "Approve", "");
			gLDashboardInfoModel.ctx = ctx;
			return gLDashboardInfoModel;
		}

		public DataGridJson<GLVoucherViewModel> GetVoucherModelPageList(MContext ctx, GLVoucherListFilterModel filter)
		{
			DataGridJson<GLVoucherViewModel> dataGridJson = new DataGridJson<GLVoucherViewModel>();
			dataGridJson.rows = dal.GetVoucherPageList(ctx, filter, false);
			dataGridJson.total = dal.GetVoucherPageListCount(ctx, filter);
			return dataGridJson;
		}

		public List<BizVerificationInfor> ValidateVoucher(MContext ctx, List<GLVoucherModel> vouchers)
		{
			List<BizVerificationInfor> list = new List<BizVerificationInfor>();
			if (vouchers == null || vouchers.Count == 0)
			{
				return list;
			}
			GLUtility gLUtility = new GLUtility();
			List<DateTime> settledDateTime = settleBiz.GetSettledDateTime(ctx);
			GLDataPool instance = GLDataPool.GetInstance(ctx, false, 0, 0, 0);
			foreach (GLVoucherModel voucher in vouchers)
			{
				int num = voucher.MYear * 100 + voucher.MPeriod;
				DateTime mGLBeginDate = ctx.MGLBeginDate;
				int num2 = mGLBeginDate.Year * 100;
				mGLBeginDate = ctx.MGLBeginDate;
				if (num < num2 + mGLBeginDate.Month)
				{
					BizVerificationInfor item = new BizVerificationInfor
					{
						RowIndex = voucher.MRowIndex,
						Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "PeriodBeforeStart", "选择的期间在总账启用之前！")
					};
					list.Add(item);
				}
				else if (settledDateTime.Exists((DateTime x) => x.Year == voucher.MYear && x.Month == voucher.MPeriod))
				{
					BizVerificationInfor item2 = new BizVerificationInfor
					{
						RowIndex = voucher.MRowIndex,
						Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "PeriodClosed", "选择的期间已经结账！")
					};
					list.Add(item2);
				}
				List<GLVoucherEntryModel> mVoucherEntrys = voucher.MVoucherEntrys;
				if (voucher == null || voucher.MVoucherEntrys.Count == 0)
				{
					BizVerificationInfor item3 = new BizVerificationInfor
					{
						RowIndex = voucher.MRowIndex,
						Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "GenerateVoucherNoEntry", "生成的凭证没有分录！")
					};
					list.Add(item3);
				}
				else if (mVoucherEntrys.Sum((GLVoucherEntryModel x) => x.MDebit) != mVoucherEntrys.Sum((GLVoucherEntryModel x) => x.MCredit))
				{
					BizVerificationInfor item4 = new BizVerificationInfor
					{
						RowIndex = voucher.MRowIndex,
						Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "GenerateVoucherDebitNotEqualCrebit", "凭证借贷方不相等！")
					};
					list.Add(item4);
				}
				List<BDAccountModel> accountIncludeDisable = instance.AccountIncludeDisable;
				GLUtility gLUtility2 = new GLUtility();
				foreach (GLVoucherEntryModel item24 in mVoucherEntrys)
				{
					BDAccountModel account = accountIncludeDisable.FirstOrDefault((BDAccountModel x) => x.MItemID == item24.MAccountID);
					if (account == null)
					{
						BizVerificationInfor item5 = new BizVerificationInfor
						{
							RowIndex = voucher.MRowIndex,
							Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "GenerateVoucherAccountNotExist", "凭证没有选择科目或者科目不存在！")
						};
						list.Add(item5);
					}
					else if (accountIncludeDisable.Exists((BDAccountModel x) => x.MParentID == account.MItemID))
					{
						string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "GenerateVoucherAccountExistSubAccount", "科目：{0}存在子级科目！");
						BizVerificationInfor item6 = new BizVerificationInfor
						{
							RowIndex = voucher.MRowIndex,
							Message = string.Format(text, account.MNumber)
						};
						list.Add(item6);
					}
					else if (!account.MIsActive)
					{
						string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "GenerateVoucherAccountDisabled", "科目：{0}已经禁用！");
						BizVerificationInfor item7 = new BizVerificationInfor
						{
							RowIndex = voucher.MRowIndex,
							Message = string.Format(text2, account.MNumber)
						};
						list.Add(item7);
					}
					else
					{
						List<GLVoucherModel> list2 = new List<GLVoucherModel>();
						list2.Add(voucher);
						try
						{
							if (!string.IsNullOrWhiteSpace(item24.MAccountID) && gLUtility2.CheckGroupValueMathWithCheckGroupModel(ctx, item24.MCheckGroupValueModel, account.MCheckGroupModel, account, true, false))
							{
								item24.MCheckGroupValueModel.MItemID = null;
								item24.MCheckGroupValueModel = gLUtility2.GetCheckGroupValueModel(ctx, item24.MCheckGroupValueModel);
								item24.MCheckGroupValueID = item24.MCheckGroupValueModel.MItemID;
							}
						}
						catch (MActionException ex)
						{
							OperationResult operationResult = new OperationResult();
							GLInterfaceRepository.HandleActionException(ctx, operationResult, ex, true);
							if (operationResult.VerificationInfor != null)
							{
								operationResult.VerificationInfor.ForEach(delegate(BizVerificationInfor x)
								{
									x.RowIndex = voucher.MRowIndex;
								});
								list.AddRange(operationResult.VerificationInfor);
							}
						}
						if (!account.MIsCheckForCurrency && item24.MCurrencyID != ctx.MBasCurrencyID)
						{
							string text3 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "VoucherCurrencyNotMatchAccount", "凭证币别和科目对应的外币核算不匹配，请确认是否启用了外币核算！");
							BizVerificationInfor bizVerificationInfor = new BizVerificationInfor();
							bizVerificationInfor.RowIndex = voucher.MRowIndex;
							bizVerificationInfor.Message = string.Format(text3);
							BizVerificationInfor bizVerificationInfor2 = bizVerificationInfor;
						}
					}
					string contactId = (item24.MCheckGroupValueModel != null) ? item24.MCheckGroupValueModel.MContactID : "";
					if (!string.IsNullOrWhiteSpace(contactId))
					{
						List<BDContactsModel> contactList = instance.ContactList;
						BDContactsModel bDContactsModel = contactList.FirstOrDefault((BDContactsModel x) => x.MItemID == contactId);
						if (bDContactsModel == null)
						{
							BizVerificationInfor item8 = new BizVerificationInfor
							{
								RowIndex = voucher.MRowIndex,
								Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "ContactNotExist", "联系人不存在！")
							};
							list.Add(item8);
						}
						else if (!bDContactsModel.MIsActive)
						{
							string text4 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "ContactDisabled", "联系人：{0}已经禁用！");
							BizVerificationInfor item9 = new BizVerificationInfor
							{
								RowIndex = voucher.MRowIndex,
								Message = string.Format(text4, bDContactsModel.MName)
							};
							list.Add(item9);
						}
					}
					string employeeId = (item24.MCheckGroupValueModel != null) ? item24.MCheckGroupValueModel.MEmployeeID : "";
					if (!string.IsNullOrWhiteSpace(employeeId))
					{
						List<BDEmployeesModel> employeeList = instance.EmployeeList;
						BDEmployeesModel bDEmployeesModel = employeeList.FirstOrDefault((BDEmployeesModel x) => x.MItemID == employeeId);
						if (bDEmployeesModel == null)
						{
							BizVerificationInfor item10 = new BizVerificationInfor
							{
								RowIndex = voucher.MRowIndex,
								Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "EmployeeNotExist", "员工不存在！")
							};
							list.Add(item10);
						}
						else if (!bDEmployeesModel.MIsActive)
						{
							string text5 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "EmployeeDisabled", "员工：{0}已经禁用！");
							BizVerificationInfor item11 = new BizVerificationInfor
							{
								RowIndex = voucher.MRowIndex,
								Message = string.Format(text5, bDEmployeesModel.MFullName)
							};
							list.Add(item11);
						}
					}
					string expenseItemId = (item24.MCheckGroupValueModel != null) ? item24.MCheckGroupValueModel.MExpItemID : "";
					if (!string.IsNullOrWhiteSpace(expenseItemId))
					{
						List<BDExpenseItemModel> expenseItemList = instance.ExpenseItemList;
						BDExpenseItemModel bDExpenseItemModel = expenseItemList.FirstOrDefault((BDExpenseItemModel x) => x.MItemID == expenseItemId);
						if (bDExpenseItemModel == null)
						{
							BizVerificationInfor item12 = new BizVerificationInfor
							{
								RowIndex = voucher.MRowIndex,
								Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "ExpenseItemNotExist", "费用项目不存在！")
							};
							list.Add(item12);
						}
						else if (!bDExpenseItemModel.MIsActive)
						{
							string text6 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "ExpenseItemDisabled", "费用项目：{0}已经禁用！");
							BizVerificationInfor item13 = new BizVerificationInfor
							{
								RowIndex = voucher.MRowIndex,
								Message = string.Format(text6, bDExpenseItemModel.MNumber + ":" + bDExpenseItemModel.MName)
							};
							list.Add(item13);
						}
					}
					string trackItem1Id = (item24.MCheckGroupValueModel != null) ? item24.MCheckGroupValueModel.MTrackItem1 : "";
					List<BDTrackModel> trackList = instance.TrackList;
					if (trackList != null && trackList.Count >= 1 && !string.IsNullOrWhiteSpace(trackItem1Id) && trackList[0].MEntryList != null)
					{
						List<BDTrackEntryModel> mEntryList = trackList[0].MEntryList;
						BDTrackEntryModel bDTrackEntryModel = mEntryList.FirstOrDefault((BDTrackEntryModel x) => x.MEntryID == trackItem1Id);
						if (bDTrackEntryModel == null)
						{
							string text7 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "TrackItemNotExist", "跟踪项：{0}所选子项不存在！");
							BizVerificationInfor item14 = new BizVerificationInfor
							{
								RowIndex = voucher.MRowIndex,
								Message = string.Format(text7, trackList[0].MName)
							};
							list.Add(item14);
						}
						else if (!bDTrackEntryModel.MIsActive)
						{
							string text8 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "TrackItemDisabled", "跟踪项：{0}已经禁用！");
							BizVerificationInfor item15 = new BizVerificationInfor
							{
								RowIndex = voucher.MRowIndex,
								Message = string.Format(text8, bDTrackEntryModel.MName)
							};
							list.Add(item15);
						}
					}
					string trackItem2Id = (item24.MCheckGroupValueModel != null) ? item24.MCheckGroupValueModel.MTrackItem2 : "";
					if (trackList != null && trackList.Count >= 2 && !string.IsNullOrWhiteSpace(trackItem2Id) && trackList[1].MEntryList != null)
					{
						List<BDTrackEntryModel> mEntryList2 = trackList[1].MEntryList;
						BDTrackEntryModel bDTrackEntryModel2 = mEntryList2.FirstOrDefault((BDTrackEntryModel x) => x.MEntryID == trackItem2Id);
						if (bDTrackEntryModel2 == null)
						{
							string text9 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "TrackItemNotExist", "跟踪项：{0}所选子项不存在！");
							BizVerificationInfor item16 = new BizVerificationInfor
							{
								RowIndex = voucher.MRowIndex,
								Message = string.Format(text9, trackList[1].MName)
							};
							list.Add(item16);
						}
						else if (!bDTrackEntryModel2.MIsActive)
						{
							string text10 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "TrackItemDisabled", "跟踪项：{0}已经禁用！");
							BizVerificationInfor item17 = new BizVerificationInfor
							{
								RowIndex = voucher.MRowIndex,
								Message = string.Format(text10, bDTrackEntryModel2.MName)
							};
							list.Add(item17);
						}
					}
					string trackItem3Id = (item24.MCheckGroupValueModel != null) ? item24.MCheckGroupValueModel.MTrackItem3 : "";
					if (trackList != null && trackList.Count >= 3 && !string.IsNullOrWhiteSpace(trackItem3Id) && trackList[2].MEntryList != null)
					{
						List<BDTrackEntryModel> mEntryList3 = trackList[2].MEntryList;
						BDTrackEntryModel bDTrackEntryModel3 = mEntryList3.FirstOrDefault((BDTrackEntryModel x) => x.MEntryID == trackItem3Id);
						if (bDTrackEntryModel3 == null)
						{
							string text11 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "TrackItemNotExist", "跟踪项：{0}所选子项不存在！");
							BizVerificationInfor item18 = new BizVerificationInfor
							{
								RowIndex = voucher.MRowIndex,
								Message = string.Format(text11, trackList[2].MName)
							};
							list.Add(item18);
						}
						else if (!bDTrackEntryModel3.MIsActive)
						{
							string text12 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "TrackItemDisabled", "跟踪项：{0}已经禁用！");
							BizVerificationInfor item19 = new BizVerificationInfor
							{
								RowIndex = voucher.MRowIndex,
								Message = string.Format(text12, bDTrackEntryModel3.MName)
							};
							list.Add(item19);
						}
					}
					string trackItem4Id = (item24.MCheckGroupValueModel != null) ? item24.MCheckGroupValueModel.MTrackItem4 : "";
					if (trackList != null && trackList.Count >= 3 && !string.IsNullOrWhiteSpace(trackItem4Id) && trackList[3].MEntryList != null)
					{
						List<BDTrackEntryModel> mEntryList4 = trackList[3].MEntryList;
						BDTrackEntryModel bDTrackEntryModel4 = mEntryList4.FirstOrDefault((BDTrackEntryModel x) => x.MEntryID == trackItem4Id);
						if (bDTrackEntryModel4 == null)
						{
							string text13 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "TrackItemNotExist", "跟踪项：{0}所选子项不存在！");
							BizVerificationInfor item20 = new BizVerificationInfor
							{
								RowIndex = voucher.MRowIndex,
								Message = string.Format(text13, trackList[3].MName)
							};
							list.Add(item20);
						}
						else if (!bDTrackEntryModel4.MIsActive)
						{
							string text14 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "TrackItemDisabled", "跟踪项：{0}已经禁用！");
							BizVerificationInfor item21 = new BizVerificationInfor
							{
								RowIndex = voucher.MRowIndex,
								Message = string.Format(text14, bDTrackEntryModel4.MName)
							};
							list.Add(item21);
						}
					}
					string trackItem5Id = (item24.MCheckGroupValueModel != null) ? item24.MCheckGroupValueModel.MTrackItem5 : "";
					if (trackList != null && trackList.Count >= 4 && !string.IsNullOrWhiteSpace(trackItem5Id) && trackList[4].MEntryList != null)
					{
						List<BDTrackEntryModel> mEntryList5 = trackList[4].MEntryList;
						BDTrackEntryModel bDTrackEntryModel5 = mEntryList5.FirstOrDefault((BDTrackEntryModel x) => x.MEntryID == trackItem5Id);
						if (bDTrackEntryModel5 == null)
						{
							string text15 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "TrackItemNotExist", "跟踪项：{0}所选子项不存在！");
							BizVerificationInfor item22 = new BizVerificationInfor
							{
								RowIndex = voucher.MRowIndex,
								Message = string.Format(text15, trackList[4].MName)
							};
							list.Add(item22);
						}
						else if (!bDTrackEntryModel5.MIsActive)
						{
							string text16 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "TrackItemDisabled", "跟踪项：{0}已经禁用！");
							BizVerificationInfor item23 = new BizVerificationInfor
							{
								RowIndex = voucher.MRowIndex,
								Message = string.Format(text16, bDTrackEntryModel5.MName)
							};
							list.Add(item23);
						}
					}
				}
			}
			return list;
		}

		public OperationResult DeleteVoucherModels(MContext ctx, List<string> pkIDS)
		{
			return dal.DeleteVoucherModels(ctx, pkIDS);
		}

		public bool ReorderVoucherNumber(MContext ctx, int year, int period, int start = 1)
		{
			return dal.ReorderVoucherNumber(ctx, year, period, start);
		}

		public GLVoucherModel GetVoucherModel(MContext ctx, string MItemID = null, int year = 0, int period = 0, int day = 0)
		{
			return dal.GetVoucherModel(ctx, MItemID, year, period, day, null);
		}

		public string GetNextVoucherNumber(MContext ctx, int year, int period)
		{
			if (!settleBiz.IsPeriodValid(ctx, new DateTime(year, period, 1)))
			{
				return lang.GetText(ctx, LangModule.GL, "ThisPeriodIsClosed", "本期已经结账，请选择其他的期");
			}
			return COMResourceHelper.GetNextVoucherNumber(ctx, year, period, null, null);
		}

		public List<GLVoucherModel> GetVoucherModelByPeriod(MContext ctx, int? year, int? period, int status = -2)
		{
			return dal.GetVoucherModelByPeriod(ctx, year, period, status);
		}

		public bool IsMNumberUsed(MContext ctx, int year, int period, string MNumber)
		{
			return COMResourceHelper.IsMNumberUsed(ctx, year, period, MNumber, null);
		}

		public Dictionary<string, string> IsMNumberUsed(MContext ctx, Dictionary<string, List<string>> dateNumberList)
		{
			return dal.IsMNumberUsed(ctx, dateNumberList);
		}

		public bool CheckVoucherHasUnapproved(MContext ctx, GLSettlementModel model)
		{
			return dal.CheckVoucherHasUnapproved(ctx, model);
		}

		public DataGridJson<GLVoucherModel> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			DataGridJson<GLVoucherModel> dataGridJson = new DataGridJson<GLVoucherModel>();
			dataGridJson.rows = dal.GetVoucherModelPageList<GLVoucherModel>(ctx, filter as GLVoucherListFilterModel, false);
			dataGridJson.total = dal.GetVoucherModelPageListCount(ctx, filter as GLVoucherListFilterModel);
			return dataGridJson;
		}

		public List<GLVoucherModel> GetVoucherModelList(MContext ctx, List<string> pkIDS, bool includeDraft = false, int year = 0, int period = 0)
		{
			return dal.GetVoucherModelList(ctx, pkIDS, includeDraft, year, period);
		}

		public List<GLVoucherViewModel> GetVoucherViewModelList(MContext ctx, List<string> pkIDS, bool includeDraft = false, int year = 0, int period = 0)
		{
			return dal.GetVoucherViewModelList(ctx, pkIDS, includeDraft, year, period, null);
		}

		public List<GLVoucherViewModel> GetVoucherListForPrint(MContext ctx, GLVoucherListFilterModel filter)
		{
			if (filter.Year > 0 && filter.Period > 0)
			{
				return dal.GetVoucherPageList(ctx, filter, false);
			}
			List<string> idList = string.IsNullOrWhiteSpace(filter.MItemID) ? null : filter.MItemID.Split(',').ToList();
			string orderBy = " ORDER BY (t1.MYear * 12 + t1.MPeriod) DESC, t1.MNumber DESC, t2.Mentryseq ";
			List<GLVoucherViewModel> voucherViewModelList = dal.GetVoucherViewModelList(ctx, idList, false, 0, 0, orderBy);
			if (idList != null && idList.Count > 0)
			{
				List<GLVoucherViewModel> list = new List<GLVoucherViewModel>();
				int i;
				for (i = 0; i < idList.Count; i++)
				{
					string text = idList[i];
					GLVoucherViewModel gLVoucherViewModel = voucherViewModelList.FirstOrDefault((GLVoucherViewModel t) => t.MItemID == idList[i]);
					if (gLVoucherViewModel != null)
					{
						list.Add(gLVoucherViewModel);
					}
				}
				return list;
			}
			return voucherViewModelList;
		}

		public BDAccountListModel GetAccountByItemID(List<BDAccountListModel> list, string itemID)
		{
			return (from x in list
			where x.MItemID != null && x.MItemID.Equals(itemID)
			select x).FirstOrDefault();
		}

		public decimal TransferAccount2Voucher(MContext ctx, GLVoucherModel voucher, List<string> accountIds, List<BDAccountListModel> accounts, int year, int period, int dir, List<GLCheckGroupModel> checkGroupList, List<GLCheckGroupValueModel> checkGroupValueList)
		{
			decimal num = default(decimal);
			List<GLBalanceModel> balanceListByAccountIDs = balanceBiz.GetBalanceListByAccountIDs(ctx, accountIds, year, period, false);
			int i;
			BDAccountListModel account;
			for (i = 0; i < accountIds.Count; i++)
			{
				IEnumerable<GLBalanceModel> enumerable = from f in balanceListByAccountIDs
				where f.MAccountID == accountIds[i]
				select f;
				foreach (GLBalanceModel item2 in enumerable)
				{
					account = accounts.Find((BDAccountListModel x) => x.MItemID == accountIds[i]);
					decimal num2 = (decimal)dir * (item2.MCredit - item2.MDebit);
					decimal num3 = (decimal)dir * (item2.MCreditFor - item2.MDebitFor);
					decimal value = (num2 == num3 || num3 == decimal.Zero || num2 == decimal.Zero) ? 1.0m : Math.Round(num2 / num3, 6);
					num += num2;
					if (num2 != decimal.Zero)
					{
						GLVoucherEntryModel item = new GLVoucherEntryModel
						{
							MExplanation = lang.GetText(ctx, LangModule.GL, "CarryForwardProfitAndLoss4CurrentPeriod", "Carry forward profit & loss for current period"),
							MDebit = ((dir == 1) ? num2 : decimal.Zero),
							MCredit = ((dir == -1) ? num2 : decimal.Zero),
							MAmount = num2,
							MAmountFor = num3,
							MAccountID = accountIds[i],
							MAccountModel = new BDAccountModel
							{
								MItemID = account.MItemID,
								MIsCheckForCurrency = account.MIsCheckForCurrency,
								MDC = dir,
								MFullName = account.MFullName,
								MNumber = account.MNumber,
								MCurrencyDataModel = new GLCurrencyDataModel
								{
									MCurrencyID = item2.MCurrencyID,
									MAmount = num2,
									MAmountFor = num3,
									MExchangeRate = Math.Abs(value)
								},
								MCheckGroupModel = checkGroupList.FirstOrDefault((GLCheckGroupModel f) => f.MItemID == account.MCheckGroupID),
								MCheckGroupValueModel = (checkGroupValueList.FirstOrDefault((GLCheckGroupValueModel f) => f.MItemID == item2.MCheckGroupValueID) ?? new GLCheckGroupValueModel())
							},
							MAccountName = account.MFullName,
							MCurrencyID = item2.MCurrencyID,
							MExchangeRate = Math.Abs(value),
							MEntrySeq = voucher.MVoucherEntrys.Count,
							MDC = dir
						};
						voucher.MVoucherEntrys.Add(item);
					}
				}
			}
			return num;
		}

		public void TransferProfitLoss(MContext ctx, GLVoucherModel voucher, List<BDAccountListModel> accounts, int year, int period, List<GLCheckGroupModel> checkGroupList, List<GLCheckGroupValueModel> checkGroupValueList)
		{
			voucher.MVoucherEntrys = new List<GLVoucherEntryModel>();
			AccountTypeEnum accountTypeEnum = new AccountTypeEnum(ctx.MAccountTableID);
			List<string> itemListByType = BDAccountRepository.GetItemListByType(accounts, new List<string>
			{
				accountTypeEnum.OperatingRevenue,
				accountTypeEnum.OtherRevenue
			}, false, true);
			decimal d = TransferAccount2Voucher(ctx, voucher, itemListByType, accounts, year, period, 1, checkGroupList, checkGroupValueList);
			List<string> itemListByType2 = BDAccountRepository.GetItemListByType(accounts, new List<string>
			{
				accountTypeEnum.OperatingCostsAndTaxes,
				accountTypeEnum.OtherLoss,
				accountTypeEnum.PeriodCharge,
				accountTypeEnum.IncomeTax
			}, false, true);
			decimal d2 = TransferAccount2Voucher(ctx, voucher, itemListByType2, accounts, year, period, -1, checkGroupList, checkGroupValueList);
			BDAccountListModel account7 = accountBiz.GetLeafAccountByCode(accounts, "4103");
			decimal num = d - d2;
			int mDC = (!(num > decimal.Zero)) ? 1 : (-1);
			GLVoucherEntryModel item = new GLVoucherEntryModel
			{
				MExplanation = lang.GetText(ctx, LangModule.GL, "CarryForwardProfitAndLoss4CurrentPeriod", "Carry forward profit & loss for current period"),
				MCredit = ((num > decimal.Zero) ? num : decimal.Zero),
				MDebit = ((num < decimal.Zero) ? Math.Abs(num) : decimal.Zero),
				MAmount = Math.Abs(num),
				MAmountFor = Math.Abs(num),
				MAccountID = account7.MItemID,
				MAccountModel = new BDAccountModel
				{
					MItemID = account7.MItemID,
					MIsCheckForCurrency = account7.MIsCheckForCurrency,
					MDC = mDC,
					MFullName = account7.MFullName,
					MNumber = account7.MNumber,
					MCurrencyDataModel = new GLCurrencyDataModel
					{
						MCurrencyID = ctx.MBasCurrencyID,
						MAmount = Math.Abs(num),
						MAmountFor = Math.Abs(num),
						MExchangeRate = 1.0m
					},
					MCheckGroupModel = checkGroupList.FirstOrDefault((GLCheckGroupModel f) => f.MItemID == account7.MCheckGroupID),
					MCheckGroupValueModel = new GLCheckGroupValueModel()
				},
				MAccountName = account7.MFullName,
				MCurrencyID = ctx.MBasCurrencyID,
				MExchangeRate = 1.0m,
				MEntrySeq = voucher.MVoucherEntrys.Count,
				MDC = mDC
			};
			voucher.MVoucherEntrys.Add(item);
		}

		public decimal GetPeriodProfit(MContext ctx, DateTime date, int type)
		{
			int value = 0;
			switch (type)
			{
			default:
				return value;
			}
		}

		public List<IOTemplateConfigModel> GetTemplateConfig(MContext ctx, bool isFromExport = false)
		{
			List<IOTemplateConfigModel> list = new List<IOTemplateConfigModel>();
			string[] megiLangTypes = ServerHelper.MegiLangTypes;
			string empty = string.Empty;
			list.AddRange(new List<IOTemplateConfigModel>
			{
				new IOTemplateConfigModel("MDate", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Common, "Date", "Date", empty)
				}), IOTemplateFieldType.Date, true, true, 2, COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "DateComment", "必录；格式为YYYY-MM-DD")),
				new IOTemplateConfigModel("MNumber", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.GL, "NO", "NO", null)
				}), false, true, 2, COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "GLNumberComment", "非必录；不录入时系统会自动填写")),
				new IOTemplateConfigModel("MAttachments", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.GL, "Attachments", "Attachments", null)
				}), false, true, 2, COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "UnRequire", "非必录"))
			});
			list.AddRange(new List<IOTemplateConfigModel>
			{
				new IOTemplateConfigModel("MExplanation", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.GL, "Explanation", "Explanation", empty)
				}), COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "Require", "必录"), true),
				new IOTemplateConfigModel("MAccountID", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Common, "AccountCode", "Account Code", null)
				}), COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "FixAssetTypeNameTips", "必录；下拉框选择"), true),
				new IOTemplateConfigModel("MAccountName", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Common, "AccountName", "Account Name", null)
				}), COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "AutoFillComment", "无须录入，系统自动填写"), false),
				new IOTemplateConfigModel("MCurrencyID", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Report, "Currency", "Currency", null)
				}), COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "CurrencyComment", "非必录；不录入时系统默认本位币"), false),
				new IOTemplateConfigModel("MContactID", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Common, "Contact", "Contact", null)
				}), COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "CheckGroupComment", "非必录；如果科目选择了这个核算维度，需要录入"), false),
				new IOTemplateConfigModel("MEmployeeID", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.IV, "employee", "Employee", null)
				}), COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "CheckGroupComment", "非必录；如果科目选择了这个核算维度，需要录入"), false),
				new IOTemplateConfigModel("MMerItemID", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.BD, "MerItem", "Item", null)
				}), COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "CheckGroupComment", "非必录；如果科目选择了这个核算维度，需要录入"), false),
				new IOTemplateConfigModel("MExpItemID", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.IV, "ExpenseItem", "Expense Item", null)
				}), COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "CheckGroupComment", "非必录；如果科目选择了这个核算维度，需要录入"), false)
			});
			if (ctx.MOrgVersionID == 0)
			{
				list.AddRange(new List<IOTemplateConfigModel>
				{
					new IOTemplateConfigModel("MPaItemID", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
					{
						new COMLangInfoModel(LangModule.BD, "PaItem", "Salary Item", null)
					}), COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "CheckGroupComment", "非必录；如果科目选择了这个核算维度，需要录入"), false)
				});
			}
			List<BDTrackModel> trackNameList = trackDal.GetTrackNameList(ctx, false);
			IEnumerable<IGrouping<string, BDTrackModel>> enumerable = from f in trackNameList
			group f by f.MItemID;
			Dictionary<string, string> allText = COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
			{
				new COMLangInfoModel(LangModule.BD, "Tracking", "Tracking", ":")
			});
			int num = 0;
			foreach (IGrouping<string, BDTrackModel> item in enumerable)
			{
				List<BDTrackModel> list2 = item.ToList();
				string fieldName = "MTrackItem" + (num + 1);
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				foreach (BDTrackModel item2 in list2)
				{
					dictionary.Add(item2.MLocaleID, allText[item2.MLocaleID] + item2.MName);
				}
				list.Add(new IOTemplateConfigModel(fieldName, dictionary, COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "CheckGroupComment", "非必录；如果科目选择了这个核算维度，需要录入"), false));
				num++;
			}
			list.AddRange(new List<IOTemplateConfigModel>
			{
				new IOTemplateConfigModel("MAmountFor", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Common, "OriginalCurrencyAmount", "Original currency amount", null)
				}), IOTemplateFieldType.Decimal, COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "OriginalAmountComment", "非必录；录入外币时必须录入"), false),
				new IOTemplateConfigModel("MDebit", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.GL, "Debit", "Debit", null)
				}), IOTemplateFieldType.Decimal, COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "Require", "必录"), true),
				new IOTemplateConfigModel("MCredit", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.GL, "Credit", "Credit", null)
				}), IOTemplateFieldType.Decimal, COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "Require", "必录"), true)
			});
			if (isFromExport)
			{
				list.AddRange(new List<IOTemplateConfigModel>
				{
					new IOTemplateConfigModel("MCreatorName", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
					{
						new COMLangInfoModel(LangModule.Common, "DocCreator", "制单", null)
					}), COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "AutoFillComment", "无须录入，系统自动填写"), false),
					new IOTemplateConfigModel("MAuditor", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
					{
						new COMLangInfoModel(LangModule.GL, "Auditor", "审核", null)
					}), COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "AutoFillComment", "无须录入，系统自动填写"), false),
					new IOTemplateConfigModel("MStatus", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
					{
						new COMLangInfoModel(LangModule.Common, "Status", "Status", null)
					}), COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "AutoFillComment", "无须录入，系统自动填写"), false)
				});
			}
			return list;
		}

		public List<ImportTemplateDataSource> GetTemplateBasicData(MContext ctx, Dictionary<string, string[]> exampleDataList = null, Dictionary<string, int> columnWList = null)
		{
			List<ImportTemplateDataSource> list = new List<ImportTemplateDataSource>();
			List<BDContactsInfoModel> contactsInfo = contactDal.GetContactsInfo(ctx, string.Empty, string.Empty);
			BASCurrencyViewModel @base = currencyDal.GetBase(ctx, false, null, null);
			List<REGCurrencyViewModel> viewList = currencyDal.GetViewList(ctx, null, null, false, null);
			List<NameValueModel> trackBasicInfo = trackDal.GetTrackBasicInfo(ctx, null, false, false);
			List<BDEmployeesModel> employeeList = empDal.GetEmployeeList(ctx, false);
			List<BDItemModel> listByWhere = itemDal.GetListByWhere(ctx, string.Empty);
			List<BDExpenseItemModel> listByTier = expenseItemBiz.GetListByTier(ctx, false, false);
			List<BDAccountListModel> accountListIncludeBalance = new BDAccountBusiness().GetAccountListIncludeBalance(ctx, new SqlWhere().Equal("a.MIsActive", 1), false);
			List<ImportDataSourceInfo> list2 = new List<ImportDataSourceInfo>();
			List<ImportDataSourceInfo> list3 = new List<ImportDataSourceInfo>();
			foreach (BDAccountListModel item2 in accountListIncludeBalance)
			{
				list2.Add(new ImportDataSourceInfo
				{
					Key = item2.MItemID,
					Value = item2.MNumber
				});
				list3.Add(new ImportDataSourceInfo
				{
					Key = item2.MItemID,
					Value = item2.MFullName.Replace(item2.MNumber + " ", "")
				});
			}
			list.Add(new ImportTemplateDataSource(true)
			{
				FieldType = ImportTemplateColumnType.Account,
				FieldList = new List<string>
				{
					"MAccountID"
				},
				DataSourceList = list2
			});
			list.Add(new ImportTemplateDataSource(true)
			{
				FieldType = ImportTemplateColumnType.AccountName,
				FieldList = new List<string>
				{
					"MAccountName"
				},
				DataSourceList = list3
			});
			List<ImportDataSourceInfo> list4 = new List<ImportDataSourceInfo>();
			if (@base != null)
			{
				list4.Add(new ImportDataSourceInfo
				{
					Key = @base.MCurrencyID,
					Value = $"{@base.MCurrencyID} {@base.MLocalName}"
				});
			}
			foreach (REGCurrencyViewModel item3 in viewList)
			{
				if (!list4.Any((ImportDataSourceInfo f) => f.Key == item3.MCurrencyID))
				{
					list4.Add(new ImportDataSourceInfo
					{
						Key = item3.MCurrencyID,
						Value = $"{item3.MCurrencyID} {item3.MName}"
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
				DataSourceList = list4
			});
			List<ImportDataSourceInfo> list5 = new List<ImportDataSourceInfo>();
			foreach (BDContactsInfoModel item4 in contactsInfo)
			{
				list5.Add(new ImportDataSourceInfo
				{
					Key = item4.MItemID,
					Value = item4.MName
				});
			}
			list.Add(new ImportTemplateDataSource(true)
			{
				FieldType = ImportTemplateColumnType.Contact,
				FieldList = new List<string>
				{
					"MContactID"
				},
				DataSourceList = list5
			});
			List<ImportDataSourceInfo> list6 = new List<ImportDataSourceInfo>();
			foreach (BDEmployeesModel item5 in employeeList)
			{
				list6.Add(new ImportDataSourceInfo
				{
					Key = item5.MItemID,
					Value = item5.MFullName
				});
			}
			list.Add(new ImportTemplateDataSource(true)
			{
				FieldType = ImportTemplateColumnType.Employee,
				FieldList = new List<string>
				{
					"MEmployeeID"
				},
				DataSourceList = list6
			});
			List<ImportDataSourceInfo> list7 = new List<ImportDataSourceInfo>();
			foreach (BDItemModel item6 in listByWhere)
			{
				list7.Add(new ImportDataSourceInfo
				{
					Key = item6.MItemID,
					Value = $"{((item6.MNumber != null) ? item6.MNumber.Trim() : string.Empty)}:{item6.MDesc.RemoveLineBreaks()}"
				});
			}
			list.Add(new ImportTemplateDataSource(true)
			{
				FieldType = ImportTemplateColumnType.InventoryItem,
				FieldList = new List<string>
				{
					"MMerItemID"
				},
				DataSourceList = list7
			});
			List<ImportDataSourceInfo> list8 = new List<ImportDataSourceInfo>();
			foreach (BDExpenseItemModel item7 in listByTier)
			{
				list8.Add(new ImportDataSourceInfo
				{
					Key = item7.MItemID,
					Value = item7.MName
				});
			}
			list.Add(new ImportTemplateDataSource(true)
			{
				FieldType = ImportTemplateColumnType.ExpenseItem,
				FieldList = new List<string>
				{
					"MExpItemID"
				},
				DataSourceList = list8
			});
			if (ctx.MOrgVersionID == 0)
			{
				List<PAPayItemGroupModel> allPayItemList = payItemGroupDal.GetAllPayItemList(ctx, false);
				List<ImportDataSourceInfo> list9 = new List<ImportDataSourceInfo>();
				foreach (PAPayItemGroupModel item8 in allPayItemList)
				{
					list9.Add(new ImportDataSourceInfo
					{
						Key = item8.MItemID,
						Value = item8.MName
					});
				}
				list.Add(new ImportTemplateDataSource(true)
				{
					FieldType = ImportTemplateColumnType.SalaryItem,
					FieldList = new List<string>
					{
						"MPaItemID"
					},
					DataSourceList = list9
				});
			}
			for (int i = 0; i < trackBasicInfo.Count; i++)
			{
				if (trackBasicInfo[i].MChildren != null)
				{
					string str = (i + 1).ToString();
					columnWList?.Add("MTrackItem" + str, 6);
					List<ImportDataSourceInfo> dsTrackList = new List<ImportDataSourceInfo>();
					trackBasicInfo[i].MChildren.ForEach(delegate(NameValueModel f)
					{
						dsTrackList.Add(new ImportDataSourceInfo
						{
							Key = f.MValue,
							Value = f.MName
						});
					});
					ImportTemplateColumnType fieldType = (ImportTemplateColumnType)Enum.Parse(typeof(ImportTemplateColumnType), "TrackItem" + (i + 1));
					list.Add(new ImportTemplateDataSource(true)
					{
						FieldType = fieldType,
						FieldList = new List<string>
						{
							"MTrackItem" + (i + 1)
						},
						DataSourceList = dsTrackList
					});
				}
			}
			return list;
		}

		public ImportTemplateModel GetImportTemplateModel(MContext ctx, bool isFromExport = false)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			List<IOTemplateConfigModel> templateConfig = GetTemplateConfig(ctx, isFromExport);
			foreach (IOTemplateConfigModel item in templateConfig)
			{
				dictionary.Add(item.MFieldName, item.MLangList.FirstOrDefault((KeyValuePair<string, string> f) => f.Key == ctx.MLCID).Value);
			}
			Dictionary<string, string[]> dictionary2 = new Dictionary<string, string[]>();
			string text = ctx.DateNow.ToString("yyyy-MM-dd");
			dictionary2.Add("MDate", new string[2]
			{
				text,
				text
			});
			string text2 = COMResourceHelper.ToVoucherNumber(ctx, "1", 0);
			dictionary2.Add("MNumber", new string[2]
			{
				text2,
				text2
			});
			dictionary2.Add("MAttachments", new string[2]
			{
				"1",
				"1"
			});
			dictionary2.Add("MExplanation", new string[2]
			{
				"Sale product",
				"Sale product"
			});
			dictionary2.Add("MDebit", new string[2]
			{
				"100.00",
				""
			});
			dictionary2.Add("MCredit", new string[2]
			{
				"",
				"100.00"
			});
			Dictionary<string, int> dictionary3 = new Dictionary<string, int>();
			dictionary3.Add("MDate", 8);
			dictionary3.Add("MAccountID", 16);
			dictionary3.Add("MAccountName", 23);
			dictionary3.Add("MNumber", 9);
			dictionary3.Add("MExplanation", 8);
			dictionary3.Add("MCurrencyID", 15);
			dictionary3.Add("MContactID", 8);
			dictionary3.Add("MEmployeeID", 8);
			dictionary3.Add("MMerItemID", 8);
			dictionary3.Add("MExpItemID", 8);
			dictionary3.Add("MPaItemID", 8);
			dictionary3.Add("MDebit", 8);
			dictionary3.Add("MCredit", 8);
			dictionary3.Add("MCreatorName", 8);
			dictionary3.Add("MAuditor", 8);
			dictionary3.Add("MStatus", 8);
			List<ImportTemplateDataSource> templateBasicData = GetTemplateBasicData(ctx, dictionary2, null);
			return new ImportTemplateModel
			{
				TemplateType = "Voucher",
				ColumnList = dictionary,
				RequiredColumnList = (from f in templateConfig
				where f.MIsRequired
				select f.MFieldName).ToList(),
				FieldConfigList = templateConfig,
				TemplateDictionaryList = templateBasicData,
				ExampleDataList = dictionary2,
				LocaleID = ctx.MLCID,
				ColumnWidthList = dictionary3
			};
		}

		public OperationResult ImportVoucherList(MContext ctx, List<GLVoucherModel> list)
		{
			return ImportVoucherList(ctx, list, false, false);
		}

		public void ImportVoucherListFromApi(MContext ctx, List<GLVoucherModel> list)
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			Dictionary<int, List<int>> dictionary2 = new Dictionary<int, List<int>>();
			var enumerable = list.GroupBy(delegate(GLVoucherModel f)
			{
				DateTime mDate = f.MDate;
				int year = mDate.Year;
				mDate = f.MDate;
				return new
				{
					Year = year,
					Month = mDate.Month
				};
			});
			foreach (var item in enumerable)
			{
				int key = item.Key.Year * 12 + item.Key.Month;
				List<GLVoucherModel> list2 = item.ToList();
				dictionary2.Add(key, new List<int>());
				dictionary2[key].Add(0);
				foreach (GLVoucherModel item2 in list2)
				{
					if (!string.IsNullOrWhiteSpace(item2.MNumber))
					{
						dictionary2[key].Add(Convert.ToInt32(item2.MNumber));
					}
				}
				int val = dictionary2[key].Max();
				string nextVoucherNumber = COMResourceHelper.GetNextVoucherNumber(ctx, item.Key.Year, item.Key.Month, null, null);
				int val2 = Convert.ToInt32(nextVoucherNumber) - 1;
				dictionary.Add(key, Math.Max(val, val2));
			}
			CheckVoucherListValue(ctx, list, dictionary);
		}

		private void CheckVoucherListValue(MContext ctx, List<GLVoucherModel> list, Dictionary<int, int> maxNumberList)
		{
			int num = 0;
			List<BDAccountModel> accountList = GLDataPool.GetInstance(ctx, false, 0, 0, 0).AccountList;
			List<BDItemModel> merItemList = GLDataPool.GetInstance(ctx, false, 0, 0, 0).MerItemList;
			List<BDExpenseItemModel> expenseItemList = GLDataPool.GetInstance(ctx, false, 0, 0, 0).ExpenseItemList;
			List<BDContactsModel> contactList = GLDataPool.GetInstance(ctx, false, 0, 0, 0).ContactList;
			List<BDEmployeesModel> employeeList = GLDataPool.GetInstance(ctx, false, 0, 0, 0).EmployeeList;
			List<BDTrackModel> trackList = (from f in ModelInfoManager.GetDataModelList<BDTrackModel>(ctx, new SqlWhere(), false, true)
			orderby f.MCreateDate
			select f).ToList();
			List<PAPayItemModel> payitemList = GLDataPool.GetInstance(ctx, false, 0, 0, 0).PayitemList;
			BDBankAccountRepository bDBankAccountRepository = new BDBankAccountRepository();
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.Equal("MIsDelete", 0);
			sqlWhere.Equal("MIsActive", 1);
			List<BDBankAccountEditModel> modelList = bDBankAccountRepository.GetModelList(ctx, sqlWhere, false);
			foreach (GLVoucherModel item in list)
			{
				bool flag = false;
				List<ValidationError> list2 = item.ValidationErrors ?? new List<ValidationError>();
				DateTime mDate = item.MDate;
				int num2 = mDate.Year * 12;
				mDate = item.MDate;
				int key = num2 + mDate.Month;
				if (string.IsNullOrWhiteSpace(item.MNumber))
				{
					item.MNumber = (maxNumberList[key] + 1 + num).ToString().PadLeft(3, '0');
					num++;
				}
				if (item.MDebitTotal != item.MCreditTotal)
				{
					list2.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "CreditDebitImbalance", "凭证分录借贷方不平衡")));
				}
				List<GLVoucherEntryModel> mVoucherEntrys = item.MVoucherEntrys;
				if (mVoucherEntrys == null || !mVoucherEntrys.Any())
				{
					list2.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "VoucherHasNotEntry", "凭证分录为空")));
				}
				else
				{
					decimal d = mVoucherEntrys.Sum((GLVoucherEntryModel m) => m.MDebit);
					decimal d2 = mVoucherEntrys.Sum((GLVoucherEntryModel m) => m.MCredit);
					if (d != d2)
					{
						list2.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "CreditDebitImbalance", "凭证分录借贷方不平衡")));
					}
					foreach (GLVoucherEntryModel item2 in mVoucherEntrys)
					{
						List<ValidationError> list3 = item2.ValidationErrors ?? new List<ValidationError>();
						if (string.IsNullOrWhiteSpace(item2.MExplanation))
						{
							list3.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "ExplanationNotNull", "摘要不能为空!")));
						}
						if (string.IsNullOrWhiteSpace(item2.MAccountID))
						{
							list3.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "AccountIsNull", "科目不能为空!")));
						}
						else
						{
							BDAccountModel bDAccountModel = accountList.FirstOrDefault((BDAccountModel m) => m.MItemID == item2.MAccountID);
							if (bDAccountModel == null)
							{
								list3.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "AccountNotExist", "科目不存在!")));
							}
							else if (!bDAccountModel.MIsActive)
							{
								list3.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "AccountDisabled", "科目已禁用!")));
							}
							else
							{
								BDBankAccountEditModel bDBankAccountEditModel = modelList.FirstOrDefault((BDBankAccountEditModel f) => f.MItemID == item2.MAccountID);
								bool flag2 = string.IsNullOrWhiteSpace(item2.MCurrencyID);
								if (bDBankAccountEditModel != null)
								{
									if (!flag2 && item2.MCurrencyID != bDBankAccountEditModel.MCyID)
									{
										list3.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "EntryCurrencyInconsistentWithBank", "分录所选币别跟所选银行科目设置的币别不一致!")));
									}
								}
								else if (bDAccountModel.MIsCheckForCurrency)
								{
									if (flag2)
									{
										list3.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "ForeignAccountCurrencyNotNull", "外币核算的科目，币别不能为空!")));
									}
								}
								else if (flag2)
								{
									item2.MCurrencyID = ctx.MBasCurrencyID;
								}
								else if (item2.MCurrencyID != ctx.MBasCurrencyID)
								{
									list3.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "CurrencyNotMatchAccount", "非外币核算的科目录入了外币")));
								}
							}
							if (accountList.Any((BDAccountModel m) => m.MParentID == item2.MAccountID))
							{
								list3.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "AccountHasSub", "选择的科目含有子科目")));
							}
						}
						if (!string.IsNullOrWhiteSpace(item2.MEmployeeID))
						{
							COMValidateHelper.CheckBasicData(ctx, employeeList.FirstOrDefault((BDEmployeesModel f) => f.MItemID == item2.MEmployeeID), list3, true);
						}
						if (!string.IsNullOrWhiteSpace(item2.MContactID))
						{
							COMValidateHelper.CheckBasicData(ctx, contactList.FirstOrDefault((BDContactsModel f) => f.MItemID == item2.MContactID), list3, true);
						}
						if (!string.IsNullOrWhiteSpace(item2.MMerItemID))
						{
							COMValidateHelper.CheckBasicData(ctx, merItemList.FirstOrDefault((BDItemModel f) => f.MItemID == item2.MMerItemID), list3, true);
						}
						if (!string.IsNullOrWhiteSpace(item2.MExpItemID))
						{
							COMValidateHelper.CheckBasicData(ctx, expenseItemList.FirstOrDefault((BDExpenseItemModel f) => f.MItemID == item2.MExpItemID), list3, true);
						}
						if (!string.IsNullOrWhiteSpace(item2.MPaItemID))
						{
							COMValidateHelper.CheckBasicData(ctx, payitemList.FirstOrDefault((PAPayItemModel f) => f.MItemID == item2.MPaItemID), list3, true);
						}
						COMValidateHelper.CheckTrackValue(ctx, item2, trackList, list3);
						List<IOValidationResultModel> list4 = new List<IOValidationResultModel>();
						List<GLVoucherEntryModel> list5 = new List<GLVoucherEntryModel>();
						list5.Add(item2);
						ValidateVoucherCheckGroupInfo(ctx, list4, list5, null, false, true, 0);
						if (list4.Any())
						{
							flag = true;
						}
						if (list3.Count > 0)
						{
							flag = true;
						}
						item2.ValidationErrors = list3;
					}
				}
				if (flag && list2.All((ValidationError m) => m.Type != 1))
				{
					list2.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "EntryHaveError", "分录有错误!"), 1));
				}
				item.ValidationErrors = list2;
				if (!list2.Any())
				{
					try
					{
						List<MultiDBCommand> updateVouchersCmd = dal.GetUpdateVouchersCmd(ctx, list, null, 0, -2, false, null, true);
						DbHelperMySQL.ExecuteSqlTran(ctx, updateVouchersCmd.ToArray());
					}
					catch (MActionException ex)
					{
						List<MActionResultCodeEnum> codes = ex.Codes;
						foreach (MActionResultCodeEnum item3 in codes)
						{
							item.ValidationErrors.Add(new ValidationError
							{
								Message = GLInterfaceRepository.GetActionExceptionMessageByCode(ctx, item3)
							});
						}
					}
				}
			}
		}

		public OperationResult ImportVoucherList(MContext ctx, List<GLVoucherModel> list, bool isFromMigration = false, bool isFromApi = false)
		{
			OperationResult operationResult = new OperationResult();
			List<IOValidationResultModel> validationResult = new List<IOValidationResultModel>();
			List<GLVoucherEntryModel> list2 = new List<GLVoucherEntryModel>();
			List<REGCurrencyViewModel> currencyListByName = currencyBiz.GetCurrencyListByName(ctx, true, true);
			DateTime currentPeriod = new GLSettlementRepository().GetCurrentPeriod(ctx);
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "CannotImportVoucherBeforeBeginDate", "You can't import voucher before begin date:{0}!");
			DateTime dateTime = ctx.MGLBeginDate;
			string message = string.Format(text, dateTime.ToString("yyyy-MM-dd"));
			string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "PeriodOfVoucherClosed", "凭证所在期间[{0}-{1}]已经结账了");
			//string text3 = "{0}-{1}: {2}";
			try
			{
				foreach (GLVoucherModel item4 in list)
				{
					GLVoucherModel gLVoucherModel = item4;
					dateTime = item4.MDate;
					gLVoucherModel.MYear = dateTime.Year;
					GLVoucherModel gLVoucherModel2 = item4;
					dateTime = item4.MDate;
					gLVoucherModel2.MPeriod = dateTime.Month;
					int num = item4.MYear * 100 + item4.MPeriod;
					dateTime = ctx.MGLBeginDate;
					int num2 = dateTime.Year * 100;
					dateTime = ctx.MGLBeginDate;
					if (num < num2 + dateTime.Month)
					{
						operationResult.VerificationInfor.Add(new BizVerificationInfor
						{
							Level = AlertEnum.Error,
							Message = message
						});
					}
					else if (item4.MYear * 100 + item4.MPeriod < currentPeriod.Year * 100 + currentPeriod.Month)
					{
						operationResult.VerificationInfor.Add(new BizVerificationInfor
						{
							Level = AlertEnum.Error,
							Message = string.Format(text2, item4.MYear, item4.MPeriod)
						});
					}
					int num3 = 0;
					foreach (GLVoucherEntryModel mVoucherEntry in item4.MVoucherEntrys)
					{
						currencyBiz.CheckCurrencyExist(ctx, mVoucherEntry, "MCurrencyID", validationResult, currencyListByName, num3);
						list2.Add(mVoucherEntry);
						num3++;
					}
				}
				List<BizVerificationInfor> list3 = COMResourceHelper.PrehandleVouchersNumber(ctx, list);
				if (list3 != null)
				{
					operationResult.VerificationInfor.AddRange(list3);
				}
				if (list.Max((GLVoucherModel f) => Convert.ToInt32(f.MNumber)).ToString().Length > ctx.MVoucherNumberLength)
				{
					throw new MActionException(new List<MActionResultCodeEnum>
					{
						MActionResultCodeEnum.MVoucherNumberNotMeetDemand
					});
				}
			}
			catch (MActionException ex)
			{
				GLInterfaceRepository.HandleActionException(ctx, operationResult, ex, isFromMigration);
				return operationResult;
			}
			List<string> accountIds = (from f in list2
			select f.MAccountID).ToList();
			if (isFromApi)
			{
				BDAccountBusiness bDAccountBusiness = new BDAccountBusiness();
				List<BDAccountModel> modelList = bDAccountBusiness.GetModelList(ctx, new SqlWhere(), false);
				int i;
				for (i = 0; i < accountIds.Count; i++)
				{
					if (modelList.Any((BDAccountModel m) => m.MParentID == accountIds[i]))
					{
						IEnumerable<GLVoucherEntryModel> enumerable = from m in list2
						where m.MAccountID == accountIds[i]
						select m;
						foreach (GLVoucherEntryModel item5 in enumerable)
						{
							GLVoucherModel gLVoucherModel3 = list.FirstOrDefault((GLVoucherModel m) => m.MVoucherEntrys.Contains(item5));
							if (gLVoucherModel3 != null)
							{
								int mRowIndex = gLVoucherModel3.MRowIndex;
								operationResult.VerificationInfor.Add(new BizVerificationInfor
								{
									Level = AlertEnum.Error,
									Message = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "AccountHaveChildAccount", "选择的科目({0})含有子科目！"), accountIds[i]),
									RowIndex = mRowIndex
								});
							}
						}
					}
					if (modelList.Any((BDAccountModel m) => !m.MIsActive && m.MItemID == accountIds[i]) || modelList.All((BDAccountModel m) => m.MItemID != accountIds[i]))
					{
						IEnumerable<GLVoucherEntryModel> enumerable2 = from m in list2
						where m.MAccountID == accountIds[i]
						select m;
						foreach (GLVoucherEntryModel item6 in enumerable2)
						{
							GLVoucherModel gLVoucherModel4 = list.FirstOrDefault((GLVoucherModel m) => m.MVoucherEntrys.Contains(item6));
							if (gLVoucherModel4 != null)
							{
								int mRowIndex2 = gLVoucherModel4.MRowIndex;
								operationResult.VerificationInfor.Add(new BizVerificationInfor
								{
									Level = AlertEnum.Error,
									Message = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "AccountNotFound", "The account:{0} can't be found!"), accountIds[i]),
									RowIndex = mRowIndex2
								});
							}
						}
					}
				}
			}
			else
			{
				List<string> childAcctNumberList = BDAccountRepository.GetChildAcctNumberList(ctx, accountIds, "MNumber");
				if (childAcctNumberList.Any())
				{
					List<string> list4 = new List<string>();
					foreach (string item7 in childAcctNumberList)
					{
						string item3 = item7.Split('.')[0];
						if (!list4.Contains(item3))
						{
							list4.Add(item3);
						}
					}
					operationResult.VerificationInfor.Add(new BizVerificationInfor
					{
						Level = AlertEnum.Error,
						Message = string.Format("选择的科目({0})含有子科目！", string.Join("、", list4))
					});
				}
				List<string> numberOrIdList = (from f in list2
				select f.MAccountID).ToList();
				accountBiz.CheckAccountNumberExist(ctx, numberOrIdList, validationResult);
			}
			string empty = string.Empty;
			contactBiz.CheckContactExist(ctx, list2, ref empty, validationResult, false, "MID", 0);
			SetDimensionByName(ctx, list2, validationResult, false);
			trackBiz.CheckTrackExist(ctx, list[0].TrackItemNameList, list2, validationResult);
			List<CommandInfo> list5 = new List<CommandInfo>();
			ValidateVoucherCheckGroupInfo(ctx, validationResult, list2, list5, isFromMigration, false, 0);
			base.SetValidationResult(ctx, operationResult, validationResult, isFromApi);
			if (!operationResult.Success)
			{
				return operationResult;
			}
			if (isFromMigration)
			{
				List<GLBalanceModel> list6 = new List<GLBalanceModel>();
				List<GLBalanceModel> balanceList = list[0].BalanceList;
				if (balanceList != null && balanceList.Count() > 0)
				{
					list6.AddRange(balanceList);
				}
				List<GLBalanceModel> initBalanceList = list[0].InitBalanceList;
				if (initBalanceList != null && initBalanceList.Count() > 0)
				{
					list6.AddRange(initBalanceList);
				}
				if (list6.Count() > 0)
				{
					try
					{
						GLBalanceBusiness gLBalanceBusiness = new GLBalanceBusiness();
						OperationResult operationResult2 = gLBalanceBusiness.ValidateBalanceList(ctx, list6);
						gLBalanceBusiness.SetBalanceCheckTypeGroupValueId(ctx, list6);
						List<GLBalanceModel> balanceList2 = list[0].BalanceList;
						if (balanceList2 != null)
						{
							balanceList2 = gLBalanceBusiness.CorrectBalanceList(ctx, balanceList2);
							list5.AddRange(ModelInfoManager.GetInsertOrUpdateCmds(ctx, balanceList2, null, true));
						}
						List<GLBalanceModel> initBalanceList2 = list[0].InitBalanceList;
						if (initBalanceList2 != null)
						{
							initBalanceList2 = gLBalanceBusiness.CorrectBalanceList(ctx, initBalanceList2);
							List<GLInitBalanceModel> list7 = new GLInitBalanceBusiness().ConvertToInitBalanceList(initBalanceList2);
							if (list7.Count() > 0)
							{
								list5.AddRange(ModelInfoManager.GetInsertOrUpdateCmds(ctx, list7, null, true));
							}
						}
					}
					catch (MActionException ex2)
					{
						GLInterfaceRepository.HandleActionException(ctx, operationResult, ex2, isFromMigration);
						return operationResult;
					}
				}
			}
			List<CommandInfo> sysCmdList = new List<CommandInfo>();
			if (isFromMigration)
			{
				AddVoucherMigrationCommandList(ctx, list, list5, sysCmdList);
			}
			if (isFromApi)
			{
				foreach (GLVoucherModel item8 in list)
				{
					try
					{
						List<MultiDBCommand> updateVouchersCmd = dal.GetUpdateVouchersCmd(ctx, list, list5, 0, -2, !isFromMigration, sysCmdList, true);
						DbHelperMySQL.ExecuteSqlTran(ctx, updateVouchersCmd.ToArray());
					}
					catch (MActionException ex3)
					{
						List<MActionResultCodeEnum> codes = ex3.Codes;
						foreach (MActionResultCodeEnum item9 in codes)
						{
							item8.ValidationErrors.Add(new ValidationError
							{
								Message = GLInterfaceRepository.GetActionExceptionMessageByCode(ctx, item9)
							});
						}
					}
				}
			}
			else
			{
				try
				{
					operationResult = dal.UpdateVouchers(ctx, list, list5, 0, -2, !isFromMigration, sysCmdList, true);
				}
				catch (MActionException ex4)
				{
					GLInterfaceRepository.HandleActionException(ctx, operationResult, ex4, isFromMigration);
				}
			}
			return operationResult;
		}

		private void AddVoucherMigrationCommandList(MContext ctx, List<GLVoucherModel> list, List<CommandInfo> basCmdList, List<CommandInfo> sysCmdList)
		{
			BASOrganisationModel model = orgBiz.GetModel(ctx);
			if (model != null)
			{
				ctx.MInitBalanceOver = model.MInitBalanceOver;
			}
			ctx.IsSys = false;
			BASCurrencyViewModel baseCurrency = currencyBiz.GetBaseCurrency(ctx);
			if (baseCurrency != null)
			{
				ctx.MBasCurrencyID = baseCurrency.MCurrencyID;
			}
			if (list[0].MigrateLogList != null)
			{
				basCmdList.AddRange(ModelInfoManager.GetInsertOrUpdateCmds(ctx, list[0].MigrateLogList, null, true));
			}
			if (list[0].MigrateConfig != null)
			{
				ctx.IsSys = true;
				list[0].MigrateConfig.MStatus = 4;
				list[0].MigrateConfig.MType = 8;
				list[0].MigrateConfig.MConfirmedType = 8;
				sysCmdList.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<MigrateConfigModel>(ctx, list[0].MigrateConfig, new List<string>
				{
					"MStatus",
					"MType",
					"MConfirmedType"
				}, true));
				ctx.IsSys = false;
			}
		}

		public void SetDimensionByName(MContext ctx, List<GLVoucherEntryModel> entryList, List<IOValidationResultModel> validationResult, bool isFromMigration = false)
		{
			List<BDEmployeesModel> list = null;
			List<BDItemModel> list2 = null;
			List<BDExpenseItemModel> list3 = null;
			List<PAPayItemGroupModel> list4 = null;
			int num = 0;
			foreach (GLVoucherEntryModel entry2 in entryList)
			{
				empBiz.CheckEmployeeExist(ctx, entry2, ref list, validationResult, "MEmployeeID", "MID", num);
				itemBiz.CheckItemExist(ctx, entry2, ref list2, validationResult, "MMerItemID", "MID", num);
				expenseItemBiz.CheckExpenseItemExist(ctx, entry2, ref list3, validationResult, "MExpItemID", num);
				paItemBiz.CheckSalaryItemExist(ctx, entry2, ref list4, validationResult, "MPaItemID", num);
				num++;
			}
		}

		public void ValidateVoucherCheckGroupInfo(MContext ctx, List<IOValidationResultModel> validationResult, List<GLVoucherEntryModel> entryList, List<CommandInfo> cmdList, bool isFromMigration = false, bool isFromApi = false, int dataRowNo = 0)
		{
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "NotEnableAccountingDimension", "该科目{0}：没有启用{1}核算维度！");
			string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "TheAccountingDimensionIsRequired", "该科目{0}：{1}核算维度为必录！");
			string text3 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "NotEnableAccountingDimensionForApi", "没有启用{1}核算维度！");
			string text4 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "TheAccountingDimensionIsRequiredForApi", "{1}核算维度为必录！");
			int[] source = new int[2]
			{
				CheckTypeStatusEnum.Optional,
				CheckTypeStatusEnum.Required
			};
			List<GLCheckGroupModel> checkGroupListByAcct = checkGroupDal.GetCheckGroupListByAcct(ctx, (from f in entryList
			select f.MAccountID).ToList());
			List<GLCheckTypeModel> modelList = checkTypeBiz.GetModelList(ctx, new SqlWhere(), false);
			GLUtility gLUtility = new GLUtility();
			GLCheckGroupValueModel checkGroupValueModel = gLUtility.GetCheckGroupValueModel(ctx, null);
			int num = dataRowNo;
			foreach (GLVoucherEntryModel entry2 in entryList)
			{
				if (string.IsNullOrWhiteSpace(entry2.MAccountID))
				{
					validationResult.Add(new IOValidationResultModel
					{
						Id = entry2.MID,
						FieldType = IOValidationTypeEnum.Account,
						Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "AccountNotEmpty", "科目不允许为空！"),
						RowIndex = num
					});
					num++;
				}
				else
				{
					GLCheckGroupModel gLCheckGroupModel = checkGroupListByAcct.FirstOrDefault((GLCheckGroupModel f) => f.MAccountID == entry2.MAccountID);
					List<string> list = new List<string>();
					List<string> list2 = new List<string>();
					bool flag = false;
					GLCheckGroupValueModel gLCheckGroupValueModel = new GLCheckGroupValueModel();
					gLCheckGroupValueModel.MOrgID = ctx.MOrgID;
					if (isFromApi && entry2.ValidationErrors == null)
					{
						entry2.ValidationErrors = new List<ValidationError>();
					}
					foreach (GLCheckTypeModel item in modelList)
					{
						string modelValue = ModelHelper.GetModelValue(entry2, item.MColumnName);
						int num2 = 0;
						if (gLCheckGroupModel != null)
						{
							num2 = Convert.ToInt32(ModelHelper.GetModelValue(gLCheckGroupModel, item.MColumnName));
						}
						if (!string.IsNullOrWhiteSpace(modelValue))
						{
							if (!source.Contains(num2))
							{
								if (isFromApi)
								{
									entry2.ValidationErrors.Add(new ValidationError(string.Format(text3, item.MName)));
								}
								list.Add(item.MName);
							}
							else
							{
								flag = true;
								ModelHelper.SetModelValue(gLCheckGroupValueModel, item.MColumnName, modelValue, null);
							}
						}
						else if (num2 == CheckTypeStatusEnum.Required)
						{
							if (isFromApi)
							{
								entry2.ValidationErrors.Add(new ValidationError(string.Format(text4, item.MName)));
							}
							list2.Add(item.MName);
						}
					}
					string arg = (gLCheckGroupModel == null) ? "" : gLCheckGroupModel.MNumber;
					if (list.Any())
					{
						string fieldValue = string.Join("、", list);
						validationResult.Add(new IOValidationResultModel
						{
							Id = (string.IsNullOrEmpty(entry2.MID) ? entryList.IndexOf(entry2).ToString() : entry2.MID),
							FieldType = IOValidationTypeEnum.CheckGroup,
							FieldValue = fieldValue,
							Message = string.Format(text, arg, "{0}"),
							RowIndex = num
						});
					}
					else if (list2.Any() && !isFromMigration)
					{
						string fieldValue2 = string.Join("、", list2);
						validationResult.Add(new IOValidationResultModel
						{
							Id = entry2.MID,
							FieldType = IOValidationTypeEnum.CheckGroup,
							FieldValue = fieldValue2,
							Message = string.Format(text2, arg, "{0}"),
							RowIndex = num
						});
					}
					if (validationResult == null || validationResult.Count() == 0)
					{
						if (flag)
						{
							gLCheckGroupValueModel = GLCheckGroupValueBusiness.FindExistCheckGroupValueModel(ctx, gLCheckGroupValueModel);
							entry2.MCheckGroupValueID = gLCheckGroupValueModel.MItemID;
							entry2.MCheckGroupValueModel = gLCheckGroupValueModel;
						}
						else
						{
							entry2.MCheckGroupValueID = checkGroupValueModel.MItemID;
							entry2.MCheckGroupValueModel = checkGroupValueModel;
						}
					}
					num++;
				}
			}
		}

		public void ValidateVoucherCheckGroupInfo(MContext ctx, List<IOValidationResultModel> validationResult, List<GLVoucherEntryModel> entryList, int dataRowNo = 0)
		{
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "TheAccountingDimensionIsRequired", "该科目{0}：{1}核算维度为必录！");
			string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "TheAccountingDimensionIsRequiredForApi", "{1}核算维度为必录！");
			int[] source = new int[2]
			{
				CheckTypeStatusEnum.Optional,
				CheckTypeStatusEnum.Required
			};
			List<GLCheckGroupModel> checkGroupListByAcct = checkGroupDal.GetCheckGroupListByAcct(ctx, (from f in entryList
			select f.MAccountID).ToList());
			List<GLCheckTypeModel> modelList = checkTypeBiz.GetModelList(ctx, new SqlWhere(), false);
			GLUtility gLUtility = new GLUtility();
			GLCheckGroupValueModel checkGroupValueModel = gLUtility.GetCheckGroupValueModel(ctx, null);
			int num = dataRowNo;
			foreach (GLVoucherEntryModel entry2 in entryList)
			{
				if (string.IsNullOrWhiteSpace(entry2.MAccountID))
				{
					validationResult.Add(new IOValidationResultModel
					{
						Id = entry2.MID,
						FieldType = IOValidationTypeEnum.Account,
						Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "AccountNotEmpty", "科目不允许为空！"),
						RowIndex = num
					});
					num++;
				}
				else
				{
					GLCheckGroupModel gLCheckGroupModel = checkGroupListByAcct.FirstOrDefault((GLCheckGroupModel f) => f.MAccountID == entry2.MAccountID);
					List<string> list = new List<string>();
					bool flag = false;
					GLCheckGroupValueModel gLCheckGroupValueModel = new GLCheckGroupValueModel();
					gLCheckGroupValueModel.MOrgID = ctx.MOrgID;
					foreach (GLCheckTypeModel item in modelList)
					{
						string modelValue = ModelHelper.GetModelValue(entry2, item.MColumnName);
						int num2 = 0;
						if (gLCheckGroupModel != null)
						{
							num2 = Convert.ToInt32(ModelHelper.GetModelValue(gLCheckGroupModel, item.MColumnName));
						}
						if (!string.IsNullOrWhiteSpace(modelValue))
						{
							if (source.Contains(num2))
							{
								flag = true;
								ModelHelper.SetModelValue(gLCheckGroupValueModel, item.MColumnName, modelValue, null);
							}
						}
						else if (num2 == CheckTypeStatusEnum.Required)
						{
							list.Add(item.MName);
						}
					}
					string arg = (gLCheckGroupModel == null) ? "" : gLCheckGroupModel.MNumber;
					if (list.Any())
					{
						string fieldValue = string.Join("、", list);
						validationResult.Add(new IOValidationResultModel
						{
							Id = entry2.MID,
							FieldType = IOValidationTypeEnum.CheckGroup,
							FieldValue = fieldValue,
							Message = string.Format(text, arg, "{0}"),
							RowIndex = num
						});
					}
					if (validationResult == null || validationResult.Count() == 0)
					{
						if (flag)
						{
							gLCheckGroupValueModel = GLCheckGroupValueBusiness.FindExistCheckGroupValueModel(ctx, gLCheckGroupValueModel);
							entry2.MCheckGroupValueID = gLCheckGroupValueModel.MItemID;
							entry2.MCheckGroupValueModel = gLCheckGroupValueModel;
						}
						else
						{
							entry2.MCheckGroupValueID = checkGroupValueModel.MItemID;
							entry2.MCheckGroupValueModel = checkGroupValueModel;
						}
					}
					num++;
				}
			}
		}

		public List<GLVoucherModel> GetRelateDeleteVoucherList(MContext ctx, List<string> pkIDS)
		{
			return GLVoucherRepository.GetRelateDeleteVoucherList(ctx, pkIDS);
		}

		public bool Exists(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.Exists(ctx, pkID, includeDelete);
		}

		public bool ExistsByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.ExistsByFilter(ctx, filter);
		}

		public OperationResult InsertOrUpdate(MContext ctx, GLVoucherModel modelData, string fields = null)
		{
			return dal.InsertOrUpdate(ctx, modelData, fields);
		}

		public OperationResult InsertOrUpdateModels(MContext ctx, List<GLVoucherModel> modelData, string fields = null)
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

		public GLVoucherModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.GetDataModel(ctx, pkID, includeDelete);
		}

		public GLVoucherModel GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.GetDataModelByFilter(ctx, filter);
		}

		public List<GLVoucherModel> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelList(ctx, filter, includeDelete);
		}
	}
}
