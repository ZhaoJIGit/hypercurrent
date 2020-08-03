using JieNor.Megi.BusinessContract.BAS;
using JieNor.Megi.BusinessContract.BD;
using JieNor.Megi.BusinessService.BAS;
using JieNor.Megi.BusinessService.REG;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Helper;
using JieNor.Megi.Core.Log;
using JieNor.Megi.Core.MultiLanguage;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.COM;
using JieNor.Megi.DataModel.Enum;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.IO;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.DataRepository.API;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.IV;
using JieNor.Megi.DataRepository.REG;
using JieNor.Megi.DataRepository.SEC;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace JieNor.Megi.BusinessService.BD
{
	public class BDContactsBusiness : APIBusinessBase<BDContactsInfoModel>, IBDContactsBusiness, IDataContract<BDContactsModel>
	{
		private readonly BDContactsRepository apiDal = new BDContactsRepository();

		private string[] currentAccountCodes = new string[6]
		{
			"1122",
			"1123",
			"1221",
			"2202",
			"2203",
			"2241"
		};

		private string[] defaultGroupIds = new string[4]
		{
			"1",
			"2",
			"4",
			"3"
		};

		private List<BDContactsInfoModel> contactList = new List<BDContactsInfoModel>();

		private List<REGTaxRateModel> taxRateList = new List<REGTaxRateModel>();

		private List<BDContactsInfoModel> disabledContactList = new List<BDContactsInfoModel>();

		private List<CommandInfo> cmdList;

		private List<BDContactsInfoModel> _contactDataPool;

		private List<REGCurrencyViewModel> _currencyDataPool;

		private List<BDAccountEditModel> _accountDataPool;

		private List<BDTrackModel> _trackDataPool;

		private List<BDContactsTrackLinkModel> _contactTrackDataPool;

		private List<BDContactsTypeModel> _contactGroupDataPool;

		private List<BDContactsTypeLinkModel> _contactTypeLinkDataPool;

		private List<REGTaxRateModel> _taxRateDataPool;

		private List<BDTrackModel> _trackList;

		private readonly BDContactsRepository _contacts = new BDContactsRepository();

		private readonly BDContactsTrackLinkRepository _consTrc = new BDContactsTrackLinkRepository();

		private BDAccountRepository _account = new BDAccountRepository();

		private REGTaxRateRepository _taxRate = new REGTaxRateRepository();

		private BDTrackRepository _track = new BDTrackRepository();

		private REGCurrencyRepository _currency = new REGCurrencyRepository();

		private IBASLangBusiness _langBiz = new BASLangBusiness();

		private BDAccountBusiness _accountBiz = new BDAccountBusiness();

		private REGTaxRateBusiness _taxRateBiz = new REGTaxRateBusiness();

		private REGCurrencyBusiness _currencyBiz = new REGCurrencyBusiness();

		private BDTrackBusiness _trackBiz = new BDTrackBusiness();

		private BASCountryBusiness _country = new BASCountryBusiness();

		protected override DataGridJson<BDContactsInfoModel> OnGet(MContext ctx, GetParam param)
		{
			APIDataPool instance = APIDataPool.GetInstance(ctx);
			_contactDataPool = instance.Contacts;
			_taxRateDataPool = instance.TaxRates;
			_contactGroupDataPool = instance.ContactGroups;
			_trackDataPool = instance.TrackingCategories;
			_contactTrackDataPool = instance.ContactTrackLinks;
			return apiDal.Get(ctx, param);
		}

		protected override void OnGetAfter(MContext ctx, GetParam param, APIDataPool dataPool, BDContactsInfoModel model)
		{
			SetContactGroupInfo(model);
			SetContactTrackInfo(model);
			model.MBalances = (model.MBalances ?? new BDContactsBalanceModel());
			if (ctx.MOrgVersionID == 0 && param.IncludeDetail.HasValue && param.IncludeDetail.Value)
			{
				model.MBalances.MAccountsPayable = (model.MBalances.MAccountsPayable ?? new BDContactsBalanceDetailModel());
				model.MBalances.MAccountsReceivable = (model.MBalances.MAccountsReceivable ?? new BDContactsBalanceDetailModel());
			}
			SetConctactInfoByType(model);
			if (model.MPaymentTerms != null)
			{
				if (ctx.MOrgVersionID == 1 || (model.MPaymentTerms.MSales != null && model.MPaymentTerms.MSales.MDay == 0 && string.IsNullOrWhiteSpace(model.MPaymentTerms.MSales.MDayType)))
				{
					model.MPaymentTerms.MSales = null;
				}
				if (ctx.MOrgVersionID == 1 || (model.MPaymentTerms.MBills != null && model.MPaymentTerms.MBills.MDay == 0 && string.IsNullOrWhiteSpace(model.MPaymentTerms.MBills.MDayType)))
				{
					model.MPaymentTerms.MBills = null;
				}
			}
		}

		protected override void OnPostBefore(MContext ctx, PostParam<BDContactsInfoModel> param, APIDataPool dataPool)
		{
			_currencyDataPool = dataPool.Currencies;
			_accountDataPool = dataPool.Accounts;
			_trackDataPool = dataPool.TrackingCategories;
			_contactTrackDataPool = dataPool.ContactTrackLinks;
			_contactGroupDataPool = dataPool.ContactGroups;
			_contactTypeLinkDataPool = dataPool.ContactTypeLinks;
			_trackList = ModelInfoManager.GetDataModelList<BDTrackModel>(ctx, new SqlWhere(), false, true);
			contactList = ModelInfoManager.GetDataModelList<BDContactsInfoModel>(ctx, new SqlWhere(), false, false);
			taxRateList = ModelInfoManager.GetDataModelList<REGTaxRateModel>(ctx, new SqlWhere(), false, false);
			GetParam getParam = new GetParam();
			getParam.WhereString = "Status==\"DISABLED\"";
			disabledContactList = base.Get(ctx, getParam).rows;
		}

		protected override void OnPostValidate(MContext ctx, PostParam<BDContactsInfoModel> param, APIDataPool dataPool, BDContactsInfoModel model, bool isPut, ref Dictionary<string, List<string>> validNameList, ref Dictionary<string, string> updNameList)
		{
			if (!model.IsIgnore)
			{
				ValidateData(ctx, param.IsPut, model, contactList, ref validNameList, ref updNameList);
			}
		}

		protected override List<CommandInfo> OnPostGetCmd(MContext ctx, PostParam<BDContactsInfoModel> param, APIDataPool dataPool, BDContactsInfoModel model)
		{
			cmdList = new List<CommandInfo>();
			base.IgnoreCommandValidate = model.IsIgnore;
			if (model.IsIgnore)
			{
				return cmdList;
			}
			SetModelInfo(ctx, param.IsPut, model, cmdList);
			string[] second = new string[4]
			{
				"MContactID",
				"MName",
				"MNameLetter",
				"MNameFirstLetter"
			};
			List<string> list = (from f in model.UpdateFieldList
			where f != "MModifyDate"
			select f).ToList();
			int num = list.Intersect(second).Count();
			if (model.IsUpdate && model.MSourceBizObject == "Invoice" && ((num == 3 && list.Count == 3) || (num == 4 && list.Count == 4)) && ((list.Exists((string a) => a == "MName") && model.IsPerfectMatchName) || !list.Exists((string a) => a == "MName")))
			{
				base.IgnoreCommandValidate = true;
				return cmdList;
			}
			cmdList.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDContactsInfoModel>(ctx, model, model.UpdateFieldList, true));
			OptLogTemplate template = (string.IsNullOrWhiteSpace(model.MItemID) || model.IsNew) ? OptLogTemplate.Contact_Created : OptLogTemplate.Contact_Edited;
			cmdList.Add(OptLog.GetAddLogCommand(template, ctx, model.MItemID, model.MName));
			if (!param.IsPut && model.IsNew)
			{
				contactList.Add(model);
			}
			return cmdList;
		}

		protected override void OnDeleteValidate(MContext ctx, DeleteParam param, APIDataPool dataPool, BDContactsInfoModel model)
		{
			if (APIBusinessBase<BDContactsInfoModel>.isAPITestMode && JudgeContactIsQuote(ctx, param.ElementID))
			{
				model.ValidationErrors.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangKey.DataHasReference)));
			}
		}

		public override void CheckRequestMethodImplemented(MContext ctx, string methodName, string groupName)
		{
			if (methodName == "OnDeleteGetCmd" && !APIBusinessBase<BDContactsInfoModel>.isAPITestMode)
			{
				base.ThrowNotImplementedException(ctx);
			}
			base.CheckRequestMethodImplemented(ctx, methodName, groupName);
		}

		protected override List<CommandInfo> OnDeleteGetCmd(MContext ctx, DeleteParam param, APIDataPool dataPool, BDContactsInfoModel model)
		{
			if (APIBusinessBase<BDContactsInfoModel>.isAPITestMode)
			{
				return ModelInfoManager.GetDeleteCmd<BDContactsInfoModel>(ctx, param.ElementID);
			}
			return new List<CommandInfo>();
		}

		private bool ValidateData(MContext ctx, bool isPut, BDContactsInfoModel model, List<BDContactsInfoModel> contactList, ref Dictionary<string, List<string>> validNameList, ref Dictionary<string, string> updNameList)
		{
			string mItemID = model.MItemID;
			bool flag = string.IsNullOrWhiteSpace(model.MItemID) || !contactList.Any((BDContactsInfoModel f) => f.MItemID == model.MItemID);
			bool flag2 = model.MSourceBizObject == "Invoice";
			string key = flag2 ? "ContactNameEmpty" : "FieldEmpty";
			string defaultValue = flag2 ? "联系人名称不能为空。" : "“{0}”不能为空。";
			if (model.UpdateFieldList.Contains("MName") && string.IsNullOrEmpty(model.MName))
			{
				model.Validate(ctx, true, key, defaultValue, LangModule.Common, "Name");
				if (flag2)
				{
					model.IsHaveNameOrIdError = true;
					return false;
				}
			}
			if (flag2 && string.IsNullOrWhiteSpace(mItemID) && string.IsNullOrWhiteSpace(model.MName))
			{
				model.Validate(ctx, true, "ContactIsNull", "联系人信息中必须包含“ContactID”或“Name”中的一个。", LangModule.IV);
				model.IsHaveNameOrIdError = true;
				return false;
			}
			model.Validate(ctx, flag && !model.UpdateFieldList.Contains("MName") && !flag2, "FieldMustProvide", "必须提供“{0}”。", LangModule.Common, "Name");
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "ContactNameUsed", "联系人名称“{0}”在系统中已经存在。");
			string editDisabledMsg = flag2 ? COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "ProvideContactDisabled", "提供的联系人已禁用。") : COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "EditDisabledContact", "禁用状态的联系人不能被更新。");
			BasicDataReferenceTypeEnum referenceType = flag2 ? BasicDataReferenceTypeEnum.ReferenceAndUpdate : BasicDataReferenceTypeEnum.NotReference;
			int count = model.ValidationErrors.Count;
			BDContactsInfoModel bDContactsInfoModel = APIValidator.MatchByIdThenName(ctx, isPut, model, contactList, text, ref updNameList, "MName", "MItemID", true, referenceType, editDisabledMsg, true, disabledContactList);
			if (flag2 && model.ValidationErrors.Count > count)
			{
				return false;
			}
			APIValidator.ValidateDuplicateName(ctx, model, text, ref validNameList, ref updNameList, "MName", "MItemID");
			if (flag2 && model.ValidationErrors.Count > count)
			{
				return false;
			}
			if (flag2 && string.IsNullOrWhiteSpace(model.MName) && !string.IsNullOrWhiteSpace(mItemID) && string.IsNullOrWhiteSpace(bDContactsInfoModel?.MItemID))
			{
				model.Validate(ctx, true, "ContactNameMustProvide", "指定的联系人不存在，在创建新联系人时，联系人名称必须提供。", LangModule.Common);
				model.IsHaveNameOrIdError = true;
				return false;
			}
			List<string> list = new List<string>();
			bool invalid = IsContactTypeInValid(model, bDContactsInfoModel, ref list);
			model.Validate(ctx, invalid, "ContactTypeEmpty", "必须选择联系人类型。", LangModule.BD);
			if (list.Any() && bDContactsInfoModel != null && !string.IsNullOrWhiteSpace(bDContactsInfoModel.MItemID) && JudgeContactIsQuote(ctx, bDContactsInfoModel.MItemID))
			{
				foreach (string item in list)
				{
					model.Validate(ctx, true, "ContactTypeUpdateError", "此联系人已被使用，不能修改联系人类型“{0}”为 “false”。", LangModule.BD, item.TrimStart('M'));
				}
			}
			ValidateTaxRate(ctx, model);
			if (!string.IsNullOrEmpty(model.MDefaultCyID) && model.MDefaultCyID != ctx.MBasCurrencyID)
			{
				REGCurrencyViewModel rEGCurrencyViewModel = _currencyDataPool.FirstOrDefault((REGCurrencyViewModel f) => f.MCurrencyID.Equals(model.MDefaultCyID, StringComparison.CurrentCultureIgnoreCase));
				if (rEGCurrencyViewModel != null)
				{
					model.MDefaultCyID = rEGCurrencyViewModel.MCurrencyID;
				}
				else
				{
					model.Validate(ctx, true, "APIKeyFieldNameNotExists", "{0}“{1}”不存在", LangModule.Common, "DefaultCurrencyCode", model.MDefaultCyID);
				}
			}
			model.Validate(ctx, model.MDiscount < decimal.Zero || model.MDiscount > 100m, "DiscountInvoid", "折扣率必须大于等于零且小于等于100。", LangModule.IV);
			APIValidator.ValidateAccountNumber(ctx, model, _accountDataPool, "MCCurrentAccountCode", null, currentAccountCodes, COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "AccountNotCurrentAcct", "只能选择往来科目,即科目代码为“1122”,“1123”, “1221”,“2202”,“2203”,“2241”的本级或下级明细科目。"));
			if (model.MPaymentTerms != null)
			{
				if (ctx.MOrgVersionID == 0)
				{
					BDContactsSalesPaymentTermModel mSales = model.MPaymentTerms.MSales;
					if (mSales != null)
					{
						if (!model.Validate(ctx, mSales.UpdateFieldList.Any() && (!mSales.UpdateFieldList.Contains("MDayType") || !mSales.UpdateFieldList.Contains("MSalDueDate")), "PaymentTermsFieldNotProvide", "当添加或更新员工默认付款方式信息时，天数类型和天数都必须指定。", LangModule.BD))
						{
							model.MSalDueDate = mSales.MDay;
							model.MSalDueCondition = mSales.MDayType;
						}
						model.Validate(ctx, mSales.UpdateFieldList.Contains("MSalDueDate") && mSales.MDay < 0, "PaymentTermsDayError", "{0}必须大于零。", LangModule.BD, "Day");
					}
					BDContactsBillsPaymentTermModel mBills = model.MPaymentTerms.MBills;
					if (mBills != null)
					{
						if (!model.Validate(ctx, mBills.UpdateFieldList.Any() && (!mBills.UpdateFieldList.Contains("MDayType") || !mBills.UpdateFieldList.Contains("MPurDueDate")), "PaymentTermsFieldNotProvide", "当添加或更新员工默认付款方式信息时，天数类型和天数都必须指定。", LangModule.BD))
						{
							model.MPurDueDate = mBills.MDay;
							model.MPurDueCondition = mBills.MDayType;
						}
						model.Validate(ctx, mBills.UpdateFieldList.Contains("MPurDueDate") && mBills.MDay < 0, "PaymentTermsDayError", "{0}必须大于零。", LangModule.BD, "Day");
					}
				}
				else
				{
					model.ValidationErrors = (from f in model.ValidationErrors
					where f.Type != 3 && f.Type != 4
					select f).ToList();
				}
			}
			List<string> matchedCategoryIdList = new List<string>();
			if (model.MSalesTrackingCategories != null)
			{
				foreach (BDTrackSimpleModel mSalesTrackingCategory in model.MSalesTrackingCategories)
				{
					ValidateTrack(ctx, mSalesTrackingCategory, model, matchedCategoryIdList);
				}
			}
			matchedCategoryIdList = new List<string>();
			if (model.MPurchasesTrackingCategories != null)
			{
				foreach (BDTrackSimpleModel mPurchasesTrackingCategory in model.MPurchasesTrackingCategories)
				{
					ValidateTrack(ctx, mPurchasesTrackingCategory, model, matchedCategoryIdList);
				}
			}
			SetNamePerfectMatch(ctx, model, bDContactsInfoModel);
			return !model.ValidationErrors.Any();
		}

		private void ValidateContactInfoByType(BDContactsInfoModel model, Dictionary<string, bool> combineTypeList)
		{
			bool flag = combineTypeList.ContainsKey("MIsOther") ? combineTypeList["MIsOther"] : model.MIsOther;
			bool flag2 = combineTypeList.ContainsKey("MIsCustomer") ? combineTypeList["MIsCustomer"] : model.MIsCustomer;
			bool flag3 = combineTypeList.ContainsKey("MIsSupplier") ? combineTypeList["MIsSupplier"] : model.MIsSupplier;
			if (!flag)
			{
				if (flag2 && !flag3)
				{
					model.MPayableTaxRate = null;
					model.MPurchasesTrackingCategories = null;
					if (model.MPaymentTerms != null)
					{
						model.MPaymentTerms.MBills = null;
						model.ValidationErrors = (from f in model.ValidationErrors
						where f.Type != 4
						select f).ToList();
					}
					RemoveSaleOrPurchaseInfo(model, false);
				}
				else if (!flag2 & flag3)
				{
					model.MReceivableTaxRate = null;
					model.MSalesTrackingCategories = null;
					if (model.MPaymentTerms != null)
					{
						model.MPaymentTerms.MSales = null;
						model.ValidationErrors = (from f in model.ValidationErrors
						where f.Type != 3
						select f).ToList();
					}
					RemoveSaleOrPurchaseInfo(model, true);
				}
			}
		}

		private bool IsContactTypeInValid(BDContactsInfoModel model, BDContactsInfoModel matchModel, ref List<string> updTypeFieldList)
		{
			bool flag = false;
			Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
			if (string.IsNullOrWhiteSpace(model.MItemID) || model.IsNew)
			{
				flag = (!model.MIsCustomer && !model.MIsSupplier && !model.MIsOther);
			}
			else
			{
				string[] array = new string[3]
				{
					"MIsCustomer",
					"MIsSupplier",
					"MIsOther"
				};
				IEnumerable<string> enumerable = array.Intersect(model.UpdateFieldList);
				List<bool> list = new List<bool>();
				foreach (string item in enumerable)
				{
					dictionary.Add(item, Convert.ToBoolean(ModelHelper.GetModelValue(model, item)));
				}
				string[] array2 = array;
				foreach (string text in array2)
				{
					bool flag2 = Convert.ToBoolean(ModelHelper.GetModelValue(matchModel, text));
					if (!enumerable.Contains(text))
					{
						dictionary.Add(text, flag2);
					}
					if (flag2 && enumerable.Contains(text) && !Convert.ToBoolean(ModelHelper.GetModelValue(model, text)))
					{
						updTypeFieldList.Add(text);
					}
				}
				flag = dictionary.Values.All((bool f) => !f);
			}
			if (!flag)
			{
				ValidateContactInfoByType(model, dictionary);
			}
			return flag;
		}

		private void ValidateTaxRate(MContext ctx, BDContactsInfoModel model)
		{
			APIValidator.ValidateTaxRate(ctx, model, model, false, "MPayableTaxRate", taxRateList, BasicDataReferenceTypeEnum.ReferenceOnly);
			APIValidator.ValidateTaxRate(ctx, model, model, true, "MReceivableTaxRate", taxRateList, BasicDataReferenceTypeEnum.ReferenceOnly);
		}

		private void SetModelInfo(MContext ctx, bool isPut, BDContactsInfoModel model, List<CommandInfo> cmdList)
		{
			if (!model.IsOnlySetDisbled())
			{
				if (string.IsNullOrWhiteSpace(model.MItemID))
				{
					model.MItemID = UUIDHelper.GetGuid();
					model.IsNew = true;
				}
				model.MIsValidateName = true;
				model = _contacts.MultiLanguageAdd(model);
				model.UpdateFieldList.AddRange(new string[2]
				{
					"MNameLetter",
					"MNameFirstLetter"
				});
				List<string> matchedCategoryIdList = new List<string>();
				List<BDContactsTrackLinkModel> list = new List<BDContactsTrackLinkModel>();
				if (model.MSalesTrackingCategories != null)
				{
					foreach (BDTrackSimpleModel mSalesTrackingCategory in model.MSalesTrackingCategories)
					{
						AddContactTrackLink(ctx, true, model, mSalesTrackingCategory, _trackDataPool, _contactTrackDataPool, list, matchedCategoryIdList);
					}
				}
				else if (model.UpdateFieldList.Contains("MSalesTrackingCategories"))
				{
					RemoveContactTrackLink(true, model, _contactTrackDataPool, list);
				}
				matchedCategoryIdList = new List<string>();
				if (model.MPurchasesTrackingCategories != null)
				{
					foreach (BDTrackSimpleModel mPurchasesTrackingCategory in model.MPurchasesTrackingCategories)
					{
						AddContactTrackLink(ctx, false, model, mPurchasesTrackingCategory, _trackDataPool, _contactTrackDataPool, list, matchedCategoryIdList);
					}
				}
				else if (model.UpdateFieldList.Contains("MPurchasesTrackingCategories"))
				{
					RemoveContactTrackLink(false, model, _contactTrackDataPool, list);
				}
				if (list.Any())
				{
					cmdList.AddRange(ModelInfoManager.GetInsertOrUpdateCmds(ctx, list, null, true));
				}
				if (ctx.MOrgVersionID == 0)
				{
					if (model.MPaymentTerms != null)
					{
						if (model.MPaymentTerms.MSales != null)
						{
							if (model.MPaymentTerms.MSales.UpdateFieldList.Any())
							{
								model.MSalDueDate = model.MPaymentTerms.MSales.MDay;
								model.MSalDueCondition = model.MPaymentTerms.MSales.MDayType;
								UpdatePaymentTerms(model, true);
							}
						}
						else if (model.MPaymentTerms.UpdateFieldList.Contains("MSales"))
						{
							UpdatePaymentTerms(model, true);
						}
						if (model.MPaymentTerms.MBills != null)
						{
							if (model.MPaymentTerms.MBills.UpdateFieldList.Any())
							{
								model.MPurDueDate = model.MPaymentTerms.MBills.MDay;
								model.MPurDueCondition = model.MPaymentTerms.MBills.MDayType;
								UpdatePaymentTerms(model, false);
							}
						}
						else if (model.MPaymentTerms.UpdateFieldList.Contains("MBills"))
						{
							UpdatePaymentTerms(model, false);
						}
					}
					else if (model.UpdateFieldList.Contains("MPaymentTerms"))
					{
						UpdatePaymentTerms(model, true);
						UpdatePaymentTerms(model, false);
					}
				}
			}
		}

		private void RemoveSaleOrPurchaseInfo(BDContactsInfoModel model, bool isSale)
		{
			if (isSale)
			{
				model.MSalTaxTypeID = string.Empty;
			}
			else
			{
				model.MPurTaxTypeID = string.Empty;
			}
			model.UpdateFieldList.Add(isSale ? "MSalTaxTypeID" : "MPurTaxTypeID");
			if (isSale)
			{
				model.MSalDueDate = 0;
				model.MSalDueCondition = string.Empty;
			}
			else
			{
				model.MPurDueDate = 0;
				model.MPurDueCondition = string.Empty;
			}
			UpdatePaymentTerms(model, isSale);
			if (isSale)
			{
				model.MSalesTrackingCategories = null;
				model.UpdateFieldList.Add("MSalesTrackingCategories");
			}
			else
			{
				model.MPurchasesTrackingCategories = null;
				model.UpdateFieldList.Add("MPurchasesTrackingCategories");
			}
		}

		private void UpdatePaymentTerms(BDContactsInfoModel model, bool isSale)
		{
			if (isSale)
			{
				model.UpdateFieldList.AddRange(new string[2]
				{
					"MSalDueDate",
					"MSalDueCondition"
				});
			}
			else
			{
				model.UpdateFieldList.AddRange(new string[2]
				{
					"MPurDueDate",
					"MPurDueCondition"
				});
			}
		}

		private void RemoveContactTrackLink(bool isSaleTrack, BDContactsInfoModel model, List<BDContactsTrackLinkModel> contactTrackLinkList, List<BDContactsTrackLinkModel> updContactTrackLinkList)
		{
			IEnumerable<BDContactsTrackLinkModel> enumerable = from f in contactTrackLinkList
			where f.MContactID == model.MContactID
			select f;
			foreach (BDContactsTrackLinkModel item in enumerable)
			{
				BDContactsTrackLinkModel contactTrackLink = updContactTrackLinkList.FirstOrDefault((BDContactsTrackLinkModel f) => f.MID == item.MID);
				if (contactTrackLink == null)
				{
					contactTrackLink = item;
				}
				if (isSaleTrack)
				{
					contactTrackLink.MSalTrackId = string.Empty;
				}
				else
				{
					contactTrackLink.MPurTrackId = string.Empty;
				}
				if (!updContactTrackLinkList.Any((BDContactsTrackLinkModel f) => f.MID == contactTrackLink.MID))
				{
					updContactTrackLinkList.Add(contactTrackLink);
				}
			}
		}

		private void AddContactTrackLink(MContext ctx, bool isSaleTrack, BDContactsInfoModel model, BDTrackSimpleModel trackInfo, List<BDTrackModel> trackList, List<BDContactsTrackLinkModel> contactTrackLinkList, List<BDContactsTrackLinkModel> updContactTrackLinkList, List<string> matchedCategoryIdList)
		{
			BDTrackModel bDTrackModel = base.MultLanguageMatchRecord(ctx, _trackList, trackInfo.MultiLanguage, "MName", "MCategoryName");
			if (!string.IsNullOrEmpty(bDTrackModel?.MItemID) && !matchedCategoryIdList.Contains(bDTrackModel.MItemID))
			{
				BDTrackEntryModel bDTrackEntryModel = base.MultLanguageMatchRecord(ctx, bDTrackModel.MEntryList, trackInfo.MultiLanguage, "MName", "MOptionName");
				if (!string.IsNullOrWhiteSpace(bDTrackEntryModel?.MTrackingOptionID))
				{
					matchedCategoryIdList.Add(bDTrackModel.MItemID);
					CreateContactTrackLink(isSaleTrack, model, contactTrackLinkList, updContactTrackLinkList, bDTrackModel, bDTrackEntryModel);
				}
				else
				{
					RemoveContactTrackLink(isSaleTrack, model, contactTrackLinkList, updContactTrackLinkList, bDTrackModel);
				}
			}
		}

		private void CreateContactTrackLink(bool isSaleTrack, BDContactsInfoModel model, List<BDContactsTrackLinkModel> contactTrackLinkList, List<BDContactsTrackLinkModel> updContactTrackLinkList, BDTrackModel existTrack, BDTrackEntryModel existTrackOption)
		{
			BDContactsTrackLinkModel contactTrackLink = updContactTrackLinkList.FirstOrDefault((BDContactsTrackLinkModel f) => f.MTrackID == existTrack.MTrackingCategoryID && f.MContactID == model.MContactID);
			bool flag = contactTrackLink != null;
			if (!flag)
			{
				contactTrackLink = contactTrackLinkList.FirstOrDefault((BDContactsTrackLinkModel f) => f.MTrackID == existTrack.MTrackingCategoryID && f.MContactID == model.MContactID);
			}
			if (contactTrackLink == null)
			{
				contactTrackLink = new BDContactsTrackLinkModel();
				contactTrackLink.MContactID = model.MContactID;
				contactTrackLink.MTrackID = existTrack.MTrackingCategoryID;
			}
			if (isSaleTrack)
			{
				contactTrackLink.MSalTrackId = existTrackOption.MTrackingOptionID;
			}
			else
			{
				contactTrackLink.MPurTrackId = existTrackOption.MTrackingOptionID;
			}
			if ((string.IsNullOrWhiteSpace(contactTrackLink.MID) && !flag) || (!string.IsNullOrWhiteSpace(contactTrackLink.MID) && !updContactTrackLinkList.Any((BDContactsTrackLinkModel f) => f.MID == contactTrackLink.MID)))
			{
				updContactTrackLinkList.Add(contactTrackLink);
			}
		}

		private void RemoveContactTrackLink(bool isSaleTrack, BDContactsInfoModel model, List<BDContactsTrackLinkModel> contactTrackLinkList, List<BDContactsTrackLinkModel> updContactTrackLinkList, BDTrackModel existTrack)
		{
			BDContactsTrackLinkModel contactTrackLink = updContactTrackLinkList.FirstOrDefault((BDContactsTrackLinkModel f) => f.MTrackID == existTrack.MTrackingCategoryID && f.MContactID == model.MContactID);
			if (contactTrackLink == null)
			{
				contactTrackLink = contactTrackLinkList.FirstOrDefault((BDContactsTrackLinkModel f) => f.MTrackID == existTrack.MTrackingCategoryID && f.MContactID == model.MContactID);
			}
			if (contactTrackLink != null)
			{
				if (isSaleTrack)
				{
					contactTrackLink.MSalTrackId = string.Empty;
				}
				else
				{
					contactTrackLink.MPurTrackId = string.Empty;
				}
				if (!updContactTrackLinkList.Any((BDContactsTrackLinkModel f) => f.MID == contactTrackLink.MID))
				{
					updContactTrackLinkList.Add(contactTrackLink);
				}
			}
		}

		private void AddContactTypeLink(MContext ctx, string contactId, string groupId, List<BDContactsTypeLinkModel> contactTypeLinks, List<CommandInfo> cmdList)
		{
			BDContactsTypeLinkModel bDContactsTypeLinkModel = contactTypeLinks.FirstOrDefault((BDContactsTypeLinkModel f) => f.MContactID == contactId);
			if (bDContactsTypeLinkModel == null)
			{
				bDContactsTypeLinkModel = new BDContactsTypeLinkModel();
			}
			bDContactsTypeLinkModel.MOrgID = ctx.MOrgID;
			bDContactsTypeLinkModel.MContactID = contactId;
			bDContactsTypeLinkModel.MTypeID = groupId;
			cmdList.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDContactsTypeLinkModel>(ctx, bDContactsTypeLinkModel, null, true));
		}

		private void SetConctactInfoByType(BDContactsInfoModel contact)
		{
			if (!contact.MIsOther)
			{
				if (contact.MIsCustomer)
				{
					if (!contact.MIsSupplier)
					{
						contact.MPayableTaxRate = null;
						if (contact.MPayableTaxRate != null)
						{
							contact.MPayableTaxRate.ValidationErrors = null;
						}
						contact.MPurchasesTrackingCategories = null;
						if (contact.MPaymentTerms != null)
						{
							contact.MPaymentTerms.MBills = null;
						}
						if (contact.MBalances != null)
						{
							contact.MBalances.MAccountsPayable = null;
						}
					}
				}
				else if (contact.MIsSupplier)
				{
					contact.MReceivableTaxRate = null;
					if (contact.MReceivableTaxRate != null)
					{
						contact.MReceivableTaxRate.ValidationErrors = null;
					}
					contact.MSalesTrackingCategories = null;
					if (contact.MPaymentTerms != null)
					{
						contact.MPaymentTerms.MSales = null;
					}
					if (contact.MBalances != null)
					{
						contact.MBalances.MAccountsReceivable = null;
					}
				}
			}
		}

		private void SetContactMultiLangInfo(MContext ctx, BDContactsInfoModel contact)
		{
			if (ctx.MultiLang)
			{
				BDContactsInfoModel bDContactsInfoModel = _contactDataPool.FirstOrDefault((BDContactsInfoModel f) => f.MItemID == contact.MItemID);
				if (bDContactsInfoModel != null)
				{
					contact.MName = bDContactsInfoModel.MName;
					contact.MFirstName = bDContactsInfoModel.MFirstName;
					contact.MLastName = bDContactsInfoModel.MLastName;
					contact.MRealAttention = bDContactsInfoModel.MRealAttention;
					contact.MRealStreet = bDContactsInfoModel.MRealStreet;
					contact.MRealRegion = bDContactsInfoModel.MRealRegion;
					contact.MPAttention = bDContactsInfoModel.MPAttention;
					contact.MPStreet = bDContactsInfoModel.MPStreet;
					contact.MPRegion = bDContactsInfoModel.MPRegion;
					contact.MBankAccName = bDContactsInfoModel.MBankAccName;
					contact.MBankName = bDContactsInfoModel.MBankName;
				}
				REGTaxRateModel rEGTaxRateModel = (contact.MReceivableTaxRate == null) ? null : _taxRateDataPool.FirstOrDefault((REGTaxRateModel f) => f.MTaxRateID == contact.MReceivableTaxRate.MTaxRateID);
				if (rEGTaxRateModel != null)
				{
					contact.MReceivableTaxRate.MName = rEGTaxRateModel.MName;
				}
				rEGTaxRateModel = ((contact.MPayableTaxRate == null) ? null : _taxRateDataPool.FirstOrDefault((REGTaxRateModel f) => f.MTaxRateID == contact.MPayableTaxRate.MTaxRateID));
				if (rEGTaxRateModel != null)
				{
					contact.MPayableTaxRate.MName = rEGTaxRateModel.MName;
				}
			}
		}

		private void SetContactTrackInfo(BDContactsInfoModel contact)
		{
			contact.MSalesTrackingCategories = new List<BDTrackSimpleModel>();
			contact.MPurchasesTrackingCategories = new List<BDTrackSimpleModel>();
			IEnumerable<BDContactsTrackLinkModel> source = from f in _contactTrackDataPool
			where f.MContactID == contact.MItemID
			select f;
			foreach (BDTrackModel item in _trackDataPool)
			{
				BDContactsTrackLinkModel bDContactsTrackLinkModel = source.FirstOrDefault((BDContactsTrackLinkModel f) => f.MTrackID == item.MItemID);
				if (bDContactsTrackLinkModel != null)
				{
					AddContactTrack(true, bDContactsTrackLinkModel.MSalTrackId, contact, item);
					AddContactTrack(false, bDContactsTrackLinkModel.MPurTrackId, contact, item);
				}
			}
		}

		private void AddContactTrack(bool isSale, string trackOptionId, BDContactsInfoModel contact, BDTrackModel track)
		{
			BDTrackSimpleModel bDTrackSimpleModel = new BDTrackSimpleModel();
			if (!string.IsNullOrWhiteSpace(trackOptionId))
			{
				BDTrackEntryModel bDTrackEntryModel = track.MEntryList.FirstOrDefault((BDTrackEntryModel f) => f.MEntryID == trackOptionId);
				if (bDTrackEntryModel != null)
				{
					bDTrackSimpleModel.MCategoryName = track.MName;
					bDTrackSimpleModel.MOptionName = bDTrackEntryModel.MName;
					if (isSale)
					{
						contact.MSalesTrackingCategories.Add(bDTrackSimpleModel);
					}
					else
					{
						contact.MPurchasesTrackingCategories.Add(bDTrackSimpleModel);
					}
				}
			}
		}

		private void SetContactGroupInfo(BDContactsInfoModel contact)
		{
			contact.MContactGroups = new List<BDContactsTypeSimpleModel>();
			if (!string.IsNullOrWhiteSpace(contact.MContactGroupID))
			{
				IEnumerable<string> enumerable = from f in contact.MContactGroupID.Split(',')
				where !defaultGroupIds.Contains(f)
				select f;
				foreach (string item in enumerable)
				{
					BDContactsTypeModel bDContactsTypeModel = _contactGroupDataPool.FirstOrDefault((BDContactsTypeModel f) => f.MItemID == item);
					if (bDContactsTypeModel != null)
					{
						contact.MContactGroups.Add(new BDContactsTypeSimpleModel
						{
							MContactGroupID = item,
							MName = bDContactsTypeModel.MName
						});
					}
				}
			}
			contact.MContactGroupID = null;
		}

		protected override void OnPostAfter(MContext ctx, PostParam<BDContactsInfoModel> param, APIDataPool dataPool)
		{
			List<string> list = (from t in param.DataList
			where !string.IsNullOrEmpty(t.MItemID)
			select t.MItemID).ToList();
			list = (from t in param.DataList
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
				base.SetWhereString(param2, "ContactID", list, true);
				IBasicBusiness<BDContactsInfoModel> basicBusiness = new BDContactsBusiness();
				DataGridJson<BDContactsInfoModel> dataGridJson = basicBusiness.Get(ctx, param2);
				List<BDContactsInfoModel> list2 = new List<BDContactsInfoModel>();
				for (int i = 0; i < param.DataList.Count; i++)
				{
					BDContactsInfoModel model = param.DataList[i];
					if (model.ValidationErrors.Count > 0)
					{
						list2.Add(model);
					}
					else
					{
						BDContactsInfoModel bDContactsInfoModel = dataGridJson.rows.FirstOrDefault((BDContactsInfoModel a) => a.MItemID == model.MItemID);
						if (bDContactsInfoModel != null)
						{
							bDContactsInfoModel.IsNew = model.IsNew;
							list2.Add(bDContactsInfoModel);
						}
					}
				}
				param.DataList = list2;
			}
		}

		private void ValidateTrack(MContext ctx, BDTrackSimpleModel trackInfo, BDContactsInfoModel model, List<string> matchedCategoryIdList)
		{
			if (base.IsMatchMultRecord(ctx, _trackList, trackInfo.MultiLanguage, "MName", "MCategoryName"))
			{
				base.AddValidationError(ctx, model, "TrackingCategoryName", trackInfo.MCategoryName);
			}
			else
			{
				BDTrackModel bDTrackModel = base.MultLanguageMatchRecord(ctx, _trackList, trackInfo.MultiLanguage, "MName", "MCategoryName");
				if (!string.IsNullOrEmpty(bDTrackModel?.MItemID) && !matchedCategoryIdList.Contains(bDTrackModel.MItemID))
				{
					if (base.IsMatchMultRecord(ctx, bDTrackModel.MEntryList, trackInfo.MultiLanguage, "MName", "MOptionName"))
					{
						matchedCategoryIdList.Add(bDTrackModel.MItemID);
						base.AddValidationError(ctx, model, "TrackingOptionName", trackInfo.MOptionName);
					}
					else
					{
						BDTrackEntryModel bDTrackEntryModel = base.MultLanguageMatchRecord(ctx, bDTrackModel.MEntryList, trackInfo.MultiLanguage, "MName", "MOptionName");
						if (!string.IsNullOrWhiteSpace(bDTrackEntryModel?.MTrackingOptionID))
						{
							matchedCategoryIdList.Add(bDTrackModel.MItemID);
							model.Validate(ctx, !bDTrackEntryModel.MIsActive, "TrackOptionDisabled", "提供的跟踪项选项“{0}”已禁用。", LangModule.Common, bDTrackEntryModel.MName);
						}
					}
				}
			}
		}

		private void SetNamePerfectMatch(MContext ctx, BDContactsInfoModel model, BDContactsInfoModel matchModel)
		{
			if (model.IsUpdate && model.MSourceBizObject == "Invoice")
			{
				bool flag = true;
				MultiLanguageFieldList multiLanguageFieldList = model.MultiLanguage.FirstOrDefault((MultiLanguageFieldList a) => a.MFieldName == "MName");
				MultiLanguageFieldList multiLanguageFieldList2 = matchModel.MultiLanguage.FirstOrDefault((MultiLanguageFieldList a) => a.MFieldName == "MName");
				foreach (string mActiveLocaleID in ctx.MActiveLocaleIDS)
				{
					string a2 = multiLanguageFieldList?.MMultiLanguageField.FirstOrDefault((MultiLanguageField lang) => lang.MLocaleID == mActiveLocaleID)?.MValue;
					string b = multiLanguageFieldList2?.MMultiLanguageField.FirstOrDefault((MultiLanguageField lang) => lang.MLocaleID == mActiveLocaleID)?.MValue;
					if (!a2.EqualsIgnoreCase(b))
					{
						flag = false;
					}
				}
				if (flag)
				{
					model.IsPerfectMatchName = true;
				}
			}
		}

		public DataGridJson<BDContactsInfoModel> GetContactPageList(MContext ctx, BDContactsInfoFilterModel filter)
		{
			return _contacts.GetContactPageList(ctx, filter);
		}

		public List<BDContactsInfoModel> GetContactListForExport(MContext ctx, BDContactsInfoFilterModel filter)
		{
			List<BDContactsInfoModel> list = new List<BDContactsInfoModel>();
			filter.IsFromExport = true;
			filter.PageSize = 2147483647;
			List<BDContactsInfoModel> rows = _contacts.GetContactPageList(ctx, filter).rows;
			if (!rows.Any())
			{
				return list;
			}
			SqlWhere filter2 = new SqlWhere().Equal("MOrgID", ctx.MOrgID).In("MItemID", (from f in rows
			select f.MItemID).ToList());
			List<BDContactsInfoModel> dataModelList = ModelInfoManager.GetDataModelList<BDContactsInfoModel>(ctx, filter2, false, false);
			foreach (BDContactsInfoModel item in dataModelList)
			{
				BDContactsInfoModel bDContactsInfoModel = rows.SingleOrDefault((BDContactsInfoModel f) => f.MItemID == item.MItemID);
				item.MName = bDContactsInfoModel.MName;
				item.MTypeID = bDContactsInfoModel.MTypeID;
				if (ctx.MOrgVersionID != 1)
				{
					item.MSaleDueAmt = bDContactsInfoModel.MSaleDueAmt;
					item.MSaleOverDueAmt = bDContactsInfoModel.MSaleOverDueAmt;
					item.MBillDueAmt = bDContactsInfoModel.MBillDueAmt;
					item.MBillOverDueAmt = bDContactsInfoModel.MBillOverDueAmt;
				}
				list.Add(item);
			}
			return list.Sort("MName", "ASC");
		}

		public BDContactsEditModel GetContactsEditModel(MContext ctx, BDContactsEditModel model)
		{
			return _contacts.GetEditModel(ctx, model);
		}

		public OperationResult IsImportContactNamesExist(MContext ctx, List<BDContactsInfoModel> list)
		{
			return _contacts.IsImportContactNamesExist(ctx, list);
		}

		public OperationResult IsImportContactHaveSameName(MContext ctx, List<BDContactsInfoModel> list)
		{
			return _contacts.IsImportContactHaveSameName(ctx, list);
		}

		public OperationResult ImportContactList(MContext ctx, List<BDContactsInfoModel> list)
		{
			OperationResult operationResult = new OperationResult();
			List<IOValidationResultModel> list2 = new List<IOValidationResultModel>();
			List<BDAccountModel> bDAccountList = _accountBiz.GetBDAccountList(ctx, new BDAccountListFilterModel
			{
				IncludeDisable = true,
				ParentCodes = "1122,2203,2202,1123,1221,2241"
			}, false, false);
			List<REGTaxRateModel> listIgnoreLocale = _taxRate.GetListIgnoreLocale(ctx, true);
			List<BASCountryModel> countryList = _country.GetCountryList(ctx);
			List<BDContactsInfoModel> disableContactList = _contacts.GetDisableContactList(ctx);
			_currencyBiz.CheckCurrencyExist(ctx, list, "MDefaultCyID", list2);
			List<BDTrackModel> list3 = (from m in ModelInfoManager.GetDataModelList<BDTrackModel>(ctx, new SqlWhere(), false, true)
			orderby m.MCreateDate
			select m).ToList();
			foreach (BDContactsInfoModel item in list)
			{
				COMModelValidateHelper.ValidateModel(ctx, item, list2, IOValidationTypeEnum.Contact);
				CheckContactsIsDisable(ctx, disableContactList, item, list2);
				_accountBiz.CheckImportAccountExist(ctx, item, bDAccountList, "MCCurrentAccountCode", list2, "MCode", false);
				_taxRateBiz.CheckTaxRateExist(ctx, item, list2, listIgnoreLocale, "MSalTaxTypeID", -1);
				_taxRateBiz.CheckTaxRateExist(ctx, item, list2, listIgnoreLocale, "MPurTaxTypeID", -1);
				_country.CheckCountryExist(ctx, item, list2, countryList, "MPCountryID", -1);
				_country.CheckCountryExist(ctx, item, list2, countryList, "MRealCountryID", -1);
				CheckTrack(ctx, list3, item, 1, list2);
				CheckTrack(ctx, list3, item, 2, list2);
				CheckTrack(ctx, list3, item, 3, list2);
				CheckTrack(ctx, list3, item, 4, list2);
				CheckTrack(ctx, list3, item, 5, list2);
			}
			base.SetValidationResult(ctx, operationResult, list2, false);
			if (!operationResult.Success)
			{
				return operationResult;
			}
			return _contacts.ImportContactList(ctx, list);
		}

		private void CheckFieldLength(MContext ctx, BDContactsInfoModel model, List<IOValidationResultModel> validationResult)
		{
			model.TableName = new BDContactsInfoModel().TableName;
			List<string> list = COMModelValidateHelper.ValidateModel(ctx, model);
			if (list.Any())
			{
				validationResult.Add(new IOValidationResultModel
				{
					FieldType = IOValidationTypeEnum.Contact,
					Message = string.Join("\r\n", list)
				});
			}
		}

		private void CheckContactsIsDisable(MContext ctx, List<BDContactsInfoModel> disableContactList, BDContactsInfoModel model, List<IOValidationResultModel> validationResult)
		{
			BDContactsInfoModel bDContactsInfoModel = disableContactList.FirstOrDefault((BDContactsInfoModel a) => a.MName.EqualsIgnoreCase(model.MName));
			if (bDContactsInfoModel != null)
			{
				validationResult.Add(new IOValidationResultModel
				{
					FieldType = IOValidationTypeEnum.Contact,
					Message = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "ContactHasDisabled", "联系人：{0}已禁用!"), model.MName)
				});
			}
		}

		private void CheckTrack(MContext ctx, List<BDTrackModel> list, BDContactsInfoModel model, int entryNumber, List<IOValidationResultModel> validationResult)
		{
			int count = list.Count;
			if (count >= entryNumber)
			{
				BDTrackModel bDTrackModel = list[entryNumber - 1];
				List<BDTrackEntryModel> mEntryList = bDTrackModel.MEntryList;
				CheckTrackField(ctx, model, mEntryList, "MSalTrackEntry" + entryNumber, validationResult);
				CheckTrackField(ctx, model, mEntryList, "MPurTrackEntry" + entryNumber, validationResult);
			}
		}

		private void CheckTrackField(MContext ctx, BDContactsInfoModel model, List<BDTrackEntryModel> entryTrackList, string trackFieldName, List<IOValidationResultModel> validationResult)
		{
			string value = string.Empty;
			string trackFieldValue = ModelHelper.GetModelValue(model, trackFieldName);
			if (!string.IsNullOrWhiteSpace(trackFieldValue))
			{
				BDTrackEntryModel bDTrackEntryModel = entryTrackList.FirstOrDefault((BDTrackEntryModel m) => m.PKFieldValue == trackFieldValue || m.MName == trackFieldValue);
				if (bDTrackEntryModel == null)
				{
					validationResult.Add(new IOValidationResultModel
					{
						FieldType = IOValidationTypeEnum.TrackOption,
						Message = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "NoTrackItemExist", "跟踪项{0}不存在！"), trackFieldValue)
					});
				}
				else if (!bDTrackEntryModel.MIsActive)
				{
					validationResult.Add(new IOValidationResultModel
					{
						FieldType = IOValidationTypeEnum.TrackOption,
						Message = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "TrackItemDisabled", "跟踪项{0}已禁用！"), trackFieldValue)
					});
				}
				else
				{
					value = bDTrackEntryModel.MEntryID;
				}
				ModelHelper.SetModelValue(model, trackFieldName, value, null);
			}
		}

		private string GetAcctFullNameByCode(List<BDAccountModel> acctList, List<string> codeList)
		{
			if (acctList == null)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			IEnumerable<BDAccountModel> enumerable = from f in acctList
			where codeList.Contains(f.MNumber)
			select f;
			foreach (BDAccountModel item in enumerable)
			{
				stringBuilder.AppendFormat("{0} {1}、", item.MNumber, item.MName);
			}
			return stringBuilder.ToString().TrimEnd('、');
		}

		public List<IOTemplateConfigModel> GetTemplateConfig(MContext ctx, bool isFromExcel = false)
		{
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "Country", "Country");
			string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "Area", "Area");
			string text3 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "Number", "Number");
			string comment = $"({text} {text2} {text3})";
			string comment2 = $"({text} {text3})";
			List<IOTemplateConfigModel> list = new List<IOTemplateConfigModel>();
			string[] megiLangTypes = ServerHelper.MegiLangTypes;
			string comment3 = "-";
			list.AddRange(new List<IOTemplateConfigModel>
			{
				new IOTemplateConfigModel("MName", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Common, "Name", "Name", null)
				}), true, false, 2, null),
				new IOTemplateConfigModel("MIsCustomer", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Common, "IsCustomer", "Is Customer", null)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MIsSupplier", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Common, "IsSupplier", "Is Supplier", null)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MIsOther", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Common, "IsOther", "Is Other", null)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MGroupName", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Common, "GroupName", "Group Name", null)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MFirstName", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Common, "FirstName", "FirstName", null)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MLastName", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Common, "LastName", "LastName", null)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MEmail", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Common, "Email", "Email", null)
				}), false, false, 2, null)
			});
			if (ctx.MEnabledModules.Contains(ModuleEnum.GL))
			{
				list.AddRange(new List<IOTemplateConfigModel>
				{
					new IOTemplateConfigModel("MCCurrentAccountCode", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
					{
						new COMLangInfoModel(LangModule.Contact, "CurrentMoneyAccount", "Current Money Account", null)
					}), false, false, 2, null)
				});
			}
			COMLangInfoModel cOMLangInfoModel = new COMLangInfoModel(LangModule.Contact, "PostalAddress", "Postal Address", comment3);
			COMLangInfoModel cOMLangInfoModel2 = new COMLangInfoModel(LangModule.Contact, "PhysicalAddress", "Physical Address", comment3);
			list.AddRange(new List<IOTemplateConfigModel>
			{
				new IOTemplateConfigModel("MPAttention", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[2]
				{
					cOMLangInfoModel,
					new COMLangInfoModel(LangModule.Common, "Attention", "Attention", null)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MPStreet", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[2]
				{
					cOMLangInfoModel,
					new COMLangInfoModel(LangModule.Contact, "StreetAddressORPOBox", "Street Address or PO Box", null)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MPCityID", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[2]
				{
					cOMLangInfoModel,
					new COMLangInfoModel(LangModule.Contact, "TownCity", "Town / City", null)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MPRegion", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[2]
				{
					cOMLangInfoModel,
					new COMLangInfoModel(LangModule.Contact, "StateRegion", "State / Region", null)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MPPostalNo", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[2]
				{
					cOMLangInfoModel,
					new COMLangInfoModel(LangModule.Contact, "PostalZipCode", "Postal / Zip Code", null)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MPCountryID", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[2]
				{
					cOMLangInfoModel,
					new COMLangInfoModel(LangModule.Contact, "Country", "Country", null)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MRealAttention", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[2]
				{
					cOMLangInfoModel2,
					new COMLangInfoModel(LangModule.Common, "Attention", "Attention", null)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MRealStreet", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[2]
				{
					cOMLangInfoModel2,
					new COMLangInfoModel(LangModule.Contact, "StreetAddressORPOBox", "Street Address or PO Box", null)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MRealCityID", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[2]
				{
					cOMLangInfoModel2,
					new COMLangInfoModel(LangModule.Contact, "TownCity", "Town / City", null)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MRealRegion", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[2]
				{
					cOMLangInfoModel2,
					new COMLangInfoModel(LangModule.Contact, "StateRegion", "State / Region", null)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MRealPostalNo", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[2]
				{
					cOMLangInfoModel2,
					new COMLangInfoModel(LangModule.Contact, "PostalZipCode", "Postal / Zip Code", null)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MRealCountryID", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[2]
				{
					cOMLangInfoModel2,
					new COMLangInfoModel(LangModule.Contact, "Country", "Country", null)
				}), false, false, 2, null)
			});
			list.AddRange(new List<IOTemplateConfigModel>
			{
				new IOTemplateConfigModel("MPhone", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Contact, "PhoneNumber", "Phone Number", comment)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MFax", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Contact, "FaxNumber", "Fax Number", comment)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MMobile", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Contact, "MobileNumber", "Mobile Number", comment2)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MDirectPhone", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Contact, "DirectDialNumber", "Direct Dial Number", comment)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MSkypeName", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Contact, "SkypeNameOrNumber", "Skype Name/Number", null)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MWebSite", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Contact, "Website", "Website", null)
				}), false, false, 2, null)
			});
			bool flag = ctx.MOrgVersionID == 1;
			List<BDTrackModel> trackNameList = _track.GetTrackNameList(ctx, false);
			IEnumerable<IGrouping<string, BDTrackModel>> categoryList = from f in trackNameList
			group f by f.MItemID;
			Dictionary<string, string> allText;
			Dictionary<string, string> allText2;
			if (!flag)
			{
				allText = COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[2]
				{
					new COMLangInfoModel(LangModule.BD, "Tracking", "Tracking", ":"),
					new COMLangInfoModel(LangModule.Contact, "ForSales", "For Sales", null)
				});
				allText2 = COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[2]
				{
					new COMLangInfoModel(LangModule.BD, "Tracking", "Tracking", ":"),
					new COMLangInfoModel(LangModule.Contact, "ForPurchases", "For Purchases", null)
				});
			}
			else
			{
				allText = COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[2]
				{
					new COMLangInfoModel(LangModule.BD, "Tracking", "Tracking", ":"),
					new COMLangInfoModel(LangModule.Contact, "ContactOutputItem", "销项", null)
				});
				allText2 = COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[2]
				{
					new COMLangInfoModel(LangModule.BD, "Tracking", "Tracking", ":"),
					new COMLangInfoModel(LangModule.Contact, "ContactInputItem", "进项", null)
				});
			}
			AddTrackConfigList(list, categoryList, "MSalTrackEntry", allText);
			AddTrackConfigList(list, categoryList, "MPurTrackEntry", allText2);
			list.AddRange(new List<IOTemplateConfigModel>
			{
				new IOTemplateConfigModel("MTaxNo", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Contact, "TaxIDNumber", "Tax ID Number", null)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MSalTaxTypeID", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Contact, "SalesTax", "Sales Tax", null)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MPurTaxTypeID", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Contact, "PurchasesTax", "Purchases Tax", null)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MDiscount", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Contact, "Discount", "Discount %", null)
				}), IOTemplateFieldType.Decimal, false, false, 2, null),
				new IOTemplateConfigModel("MDefaultCyID", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Contact, "DefaultCurrency", "Default Currency", null)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MBankAcctNo", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Common, "BankAccount_Number", "Bank Account Number", null)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MBankAccName", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Common, "Account_Name", "Account Name", null)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MBankName", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Contact, "Details", "Details", null)
				}), false, false, 2, null)
			});
			if (!flag)
			{
				list.AddRange(new List<IOTemplateConfigModel>
				{
					new IOTemplateConfigModel("MPurDueDate", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
					{
						new COMLangInfoModel(LangModule.Contact, "DueDateBillDay", "Due Date Bill Day", null)
					}), IOTemplateFieldType.Decimal, false, false, 0, null),
					new IOTemplateConfigModel("MPurDueCondition", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
					{
						new COMLangInfoModel(LangModule.Contact, "DueDateBillTerm", "Due Date Bill Term", null)
					}), false, false, 2, null),
					new IOTemplateConfigModel("MSalDueDate", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
					{
						new COMLangInfoModel(LangModule.Contact, "DueDateSalesDay", "Due Date Sales Day", null)
					}), IOTemplateFieldType.Decimal, false, false, 0, null),
					new IOTemplateConfigModel("MSalDueCondition", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
					{
						new COMLangInfoModel(LangModule.Contact, "DueDateSalesTerm", "Due Date Sales Term", null)
					}), false, false, 2, null)
				});
			}
			return list;
		}

		private static void AddTrackConfigList(List<IOTemplateConfigModel> configList, IEnumerable<IGrouping<string, BDTrackModel>> categoryList, string fieldPrefix, Dictionary<string, string> trackingPrefix)
		{
			int num = 0;
			foreach (IGrouping<string, BDTrackModel> category in categoryList)
			{
				List<BDTrackModel> list = category.ToList();
				string fieldName = fieldPrefix + (num + 1);
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				foreach (BDTrackModel item in list)
				{
					dictionary.Add(item.MLocaleID, $"{trackingPrefix[item.MLocaleID]}-{item.MName}");
				}
				configList.Add(new IOTemplateConfigModel(fieldName, dictionary, false, false, 2, null));
				num++;
			}
		}

		public List<ImportTemplateDataSource> GetTemplateBasicData(MContext ctx, Dictionary<string, string> columnList, bool isFromExcel = false, Dictionary<string, string[]> exampleDataList = null)
		{
			List<ImportTemplateDataSource> list = new List<ImportTemplateDataSource>();
			BDContactsInfoModel emptyDataEditModel = ModelInfoManager.GetEmptyDataEditModel<BDContactsInfoModel>(ctx);
			List<string> list2 = (from f in emptyDataEditModel.MultiLanguage
			where columnList.Keys.Contains(f.MFieldName)
			select f.MFieldName).ToList();
			list2.Add("MGroupName");
			List<BASLangModel> orgLangList = _langBiz.GetOrgLangList(ctx);
			List<BDAccountTypeListModel> bDAccountTypeList = _account.GetBDAccountTypeList(ctx, string.Empty);
			List<REGTaxRateModel> list3 = _taxRate.GetList(ctx, null, false);
			BASCurrencyViewModel @base = _currency.GetBase(ctx, false, null, null);
			List<BASCountryModel> countryList = _country.GetCountryList(ctx);
			List<REGCurrencyViewModel> viewList = _currency.GetViewList(ctx, null, null, false, null);
			List<BDAccountModel> bDAccountList = _accountBiz.GetBDAccountList(ctx, new BDAccountListFilterModel
			{
				IsActive = true,
				ParentCodes = "1122,2203,2202,1123,1221,2241"
			}, false, false);
			List<NameValueModel> trackBasicInfo = _track.GetTrackBasicInfo(ctx, null, false, false);
			List<ImportDataSourceInfo> dicLangList = new List<ImportDataSourceInfo>();
			orgLangList.ForEach(delegate(BASLangModel f)
			{
				dicLangList.Add(new ImportDataSourceInfo
				{
					Key = f.LangID,
					Value = f.LangName
				});
			});
			list.Add(new ImportTemplateDataSource(false)
			{
				FieldType = ImportTemplateColumnType.MultiLanguage,
				FieldList = list2,
				DataSourceList = dicLangList
			});
			List<ImportDataSourceInfo> taxRateDatasource = new List<ImportDataSourceInfo>();
			list3.ForEach(delegate(REGTaxRateModel f)
			{
				taxRateDatasource.Add(new ImportDataSourceInfo
				{
					Key = f.MItemID,
					Value = $"{f.MName}({f.MEffectiveTaxRate.ToOrgDigitalFormat(ctx)}%)"
				});
			});
			list.Add(new ImportTemplateDataSource(true)
			{
				FieldType = ImportTemplateColumnType.TaxRate,
				FieldList = new List<string>
				{
					"MSalTaxTypeID",
					"MPurTaxTypeID"
				},
				DataSourceList = taxRateDatasource
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
			foreach (REGCurrencyViewModel item in viewList)
			{
				if (!list4.Any((ImportDataSourceInfo f) => f.Key == item.MCurrencyID))
				{
					list4.Add(new ImportDataSourceInfo
					{
						Key = item.MCurrencyID,
						Value = $"{item.MCurrencyID} {item.MName}"
					});
				}
			}
			list.Add(new ImportTemplateDataSource(true)
			{
				FieldType = ImportTemplateColumnType.Currency,
				FieldList = new List<string>
				{
					"MDefaultCyID"
				},
				DataSourceList = list4
			});
			List<ImportDataSourceInfo> countryDatasource = new List<ImportDataSourceInfo>();
			countryList.ForEach(delegate(BASCountryModel f)
			{
				countryDatasource.Add(new ImportDataSourceInfo
				{
					Key = f.MItemID,
					Value = f.MCountryName
				});
			});
			list.Add(new ImportTemplateDataSource(true)
			{
				FieldType = ImportTemplateColumnType.Country,
				FieldList = new List<string>
				{
					"MPCountryID",
					"MRealCountryID"
				},
				DataSourceList = countryDatasource
			});
			if (ctx.MOrgVersionID != 1)
			{
				List<ImportDataSourceInfo> list5 = new List<ImportDataSourceInfo>();
				list5.Add(new ImportDataSourceInfo
				{
					Key = "item0",
					Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "OfTheFollowingMonth", "of the following month")
				});
				list5.Add(new ImportDataSourceInfo
				{
					Key = "item1",
					Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "DayAfterTheBillDate", "day(s) after the bill date")
				});
				list5.Add(new ImportDataSourceInfo
				{
					Key = "item2",
					Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "DayDfterEndBillMonth", "day(s) after the end of the bill month")
				});
				list5.Add(new ImportDataSourceInfo
				{
					Key = "item3",
					Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "OfTheCurrentMonth", "of the current month")
				});
				list.Add(new ImportTemplateDataSource(true)
				{
					FieldType = ImportTemplateColumnType.DueDate,
					FieldList = new List<string>
					{
						"MPurDueCondition",
						"MSalDueCondition"
					},
					DataSourceList = list5
				});
			}
			List<ImportDataSourceInfo> list6 = new List<ImportDataSourceInfo>();
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangKey.Yes);
			string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangKey.No);
			list6.Add(new ImportDataSourceInfo
			{
				Key = "1",
				Value = text
			});
			list6.Add(new ImportDataSourceInfo
			{
				Key = "0",
				Value = text2
			});
			list.Add(new ImportTemplateDataSource(true)
			{
				FieldType = ImportTemplateColumnType.ContactType,
				FieldList = new List<string>
				{
					"MIsCustomer",
					"MIsSupplier",
					"MIsOther"
				},
				DataSourceList = list6
			});
			if (ctx.MEnabledModules.Contains(ModuleEnum.GL))
			{
				List<ImportDataSourceInfo> list7 = new List<ImportDataSourceInfo>();
				foreach (BDAccountModel item2 in bDAccountList)
				{
					if (!string.IsNullOrWhiteSpace(item2.MCode))
					{
						list7.Add(new ImportDataSourceInfo
						{
							Key = item2.MCode,
							Value = item2.MFullName
						});
					}
				}
				list.Add(new ImportTemplateDataSource(true)
				{
					FieldType = ImportTemplateColumnType.CurrentAccount,
					FieldList = new List<string>
					{
						"MCCurrentAccountCode"
					},
					DataSourceList = list7
				});
			}
			for (int i = 0; i < trackBasicInfo.Count; i++)
			{
				if (trackBasicInfo[i].MChildren != null)
				{
					string str = (i + 1).ToString();
					List<ImportDataSourceInfo> list8 = new List<ImportDataSourceInfo>();
					foreach (NameValueModel mChild in trackBasicInfo[i].MChildren)
					{
						if (!string.IsNullOrWhiteSpace(mChild.MValue))
						{
							list8.Add(new ImportDataSourceInfo
							{
								Key = mChild.MValue,
								Value = mChild.MName
							});
						}
					}
					ImportTemplateColumnType fieldType = (ImportTemplateColumnType)Enum.Parse(typeof(ImportTemplateColumnType), "TrackItem" + str);
					list.Add(new ImportTemplateDataSource(true)
					{
						FieldType = fieldType,
						FieldList = new List<string>
						{
							"MSalTrackEntry" + str,
							"MPurTrackEntry" + str
						},
						DataSourceList = list8
					});
				}
			}
			return list;
		}

		public ImportTemplateModel GetImportTemplateModel(MContext ctx)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			List<IOTemplateConfigModel> templateConfig = GetTemplateConfig(ctx, false);
			foreach (IOTemplateConfigModel item in templateConfig)
			{
				dictionary.Add(item.MFieldName, item.MLangList.FirstOrDefault((KeyValuePair<string, string> f) => f.Key == ctx.MLCID).Value);
			}
			Dictionary<string, string[]> dictionary2 = new Dictionary<string, string[]>();
			List<ImportTemplateDataSource> templateBasicData = GetTemplateBasicData(ctx, dictionary, false, dictionary2);
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangKey.Yes);
			string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangKey.No);
			dictionary2.Add("MName", new string[1]
			{
				COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "KFC", "KFC")
			});
			dictionary2.Add("MIsCustomer", new string[1]
			{
				text
			});
			dictionary2.Add("MIsSupplier", new string[1]
			{
				text2
			});
			dictionary2.Add("MIsOther", new string[1]
			{
				text2
			});
			dictionary2.Add("MPurDueDate", new string[1]
			{
				"1"
			});
			dictionary2.Add("MSalDueDate", new string[1]
			{
				"2"
			});
			List<string> alignRightFieldList = "MPurDueDate,MSalDueDate".Split(',').ToList();
			List<string> requiredColumnList = new List<string>
			{
				"ContactType"
			}.Union((from f in templateConfig
			where f.MIsRequired
			select f.MFieldName).ToList()).ToList();
			return new ImportTemplateModel
			{
				TemplateType = "Contact",
				LocaleID = ctx.MLCID,
				ColumnList = dictionary,
				FieldConfigList = templateConfig,
				RequiredColumnList = requiredColumnList,
				TemplateDictionaryList = templateBasicData,
				ExampleDataList = dictionary2,
				AlignRightFieldList = alignRightFieldList
			};
		}

		public BDIsCanDeleteModel IsCanDeleteOrInactive(MContext ctx, ParamBase param)
		{
			return BDRepository.IsCanDeleteOrInactive(ctx, "Contact", param.KeyIDs.Split(',').ToList(), param.IsDelete);
		}

		public bool Exists(MContext ctx, string pkID, bool includeDelete = false)
		{
			return _contacts.Exists(ctx, pkID, includeDelete);
		}

		public bool ExistsByFilter(MContext ctx, SqlWhere filter)
		{
			return _contacts.ExistsByFilter(ctx, filter);
		}

		public OperationResult InsertOrUpdate(MContext ctx, BDContactsModel modelData, string fields = null)
		{
			return _contacts.InsertOrUpdate(ctx, modelData, fields);
		}

		public OperationResult InsertOrUpdateModels(MContext ctx, List<BDContactsModel> modelData, string fields = null)
		{
			return _contacts.InsertOrUpdateModels(ctx, modelData, fields);
		}

		public OperationResult Delete(MContext ctx, string pkID)
		{
			OperationResult operationResult = BDRepository.IsCanDelete(ctx, "Contact", pkID);
			if (operationResult.Success)
			{
				operationResult = _contacts.Delete(ctx, pkID);
			}
			return operationResult;
		}

		public OperationResult DeleteContact(MContext ctx, ParamBase param)
		{
			string keyIDs = param.KeyIDs;
			OperationResult operationResult = new OperationResult();
			if (keyIDs.IndexOf(',') >= 0)
			{
				List<string> list = new List<string>();
				operationResult = BDRepository.IsCanDelete(ctx, "Contact", keyIDs, out list);
				string message = operationResult.Message;
				if (list.Count != 0)
				{
					operationResult = ModelInfoManager.DeleteFlag<BDContactsModel>(ctx, list);
					operationResult.Message = message;
				}
			}
			else
			{
				operationResult = BDRepository.IsCanDelete(ctx, "Contact", keyIDs);
				BDContactsRepository bDContactsRepository = new BDContactsRepository();
				BDContactsModel dataModel = bDContactsRepository.GetDataModel(ctx, param.KeyIDSWithNoSingleQuote, false);
				if (operationResult.Success)
				{
					operationResult = ModelInfoManager.DeleteFlag<BDContactsModel>(ctx, keyIDs);
				}
				string text = "";
				if (dataModel != null)
				{
					MultiLanguageFieldList multiLanguageFieldList = (from t in dataModel.MultiLanguage
					where t.MFieldName == "MName"
					select t).FirstOrDefault();
					if (multiLanguageFieldList != null && multiLanguageFieldList.MMultiLanguageField != null)
					{
						text = (from t in multiLanguageFieldList.MMultiLanguageField
						where t.MLocaleID == ctx.MLCID
						select t.MValue).FirstOrDefault();
					}
				}
				if (operationResult.Success)
				{
					operationResult.Message = (string.IsNullOrEmpty(text) ? "" : text);
				}
				else
				{
					operationResult.Message = (string.IsNullOrEmpty(text) ? "" : (text + ":" + COMMultiLangRepository.GetText(ctx.MLCID, LangKey.DataHasReference)));
				}
			}
			return operationResult;
		}

		public OperationResult DeleteModels(MContext ctx, List<string> pkID)
		{
			return _contacts.DeleteModels(ctx, pkID);
		}

		public BDContactsModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			return _contacts.GetDataModel(ctx, pkID, includeDelete);
		}

		public BDContactsModel GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			return _contacts.GetDataModelByFilter(ctx, filter);
		}

		public List<BDContactsModel> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return _contacts.GetModelList(ctx, filter, includeDelete);
		}

		public DataGridJson<BDContactsModel> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return _contacts.GetModelPageList(ctx, filter, includeDelete);
		}

		public string GetContactName(MContext ctx, string itemId)
		{
			List<BDContactItem> contactItemList = GetContactItemList(ctx, new BDContactsListFilter
			{
				MaxCount = 0
			});
			if (contactItemList == null || contactItemList.Count == 0)
			{
				return string.Empty;
			}
			BDContactItem bDContactItem = contactItemList.FirstOrDefault((BDContactItem t) => t.MItemID == itemId);
			return (bDContactItem == null) ? "" : bDContactItem.MName;
		}

		public List<BDContactItem> GetContactItemList(MContext ctx, BDContactsListFilter filter)
		{
			List<BDContactItem> list = new List<BDContactItem>();
			string containId = string.IsNullOrEmpty(filter.MID) ? "" : filter.MID.Split('_')[0];
			List<BDContactsInfoModel> contactsInfo = _contacts.GetContactsInfo(ctx, "", "");
			BDContactsInfoModel containContactModel = null;
			if (contactsInfo != null && contactsInfo.Count > 0)
			{
				containContactModel = (from t in contactsInfo
				where t.MItemID == containId
				select t).FirstOrDefault();
				list.AddRange(GetSupplierList(ctx, contactsInfo, filter, containContactModel));
				list.AddRange(GetCustomerList(ctx, contactsInfo, filter, containContactModel));
			}
			list.AddRange(GetEmployeeList(ctx, filter, containId));
			if (contactsInfo != null && contactsInfo.Count > 0)
			{
				list.AddRange(GetOtherContactList(ctx, contactsInfo, filter, containContactModel));
			}
			return list;
		}

		private List<BDContactItem> GetEmployeeList(MContext ctx, BDContactsListFilter filter, string containId)
		{
			List<BDContactItem> list = new List<BDContactItem>();
			BDEmployeesRepository bDEmployeesRepository = new BDEmployeesRepository();
			List<BDEmployeesListModel> list2 = bDEmployeesRepository.GetBDEmployeesList(ctx, filter.QueryString, false);
			if (list2 != null && list2.Count > 0)
			{
				if (filter.MaxCount > 0)
				{
					list2 = list2.Take(filter.MaxCount).ToList();
				}
				string contactGroupName = GetContactGroupName(ctx, BDContactType.Employee);
				foreach (BDEmployeesListModel item in list2)
				{
					BDContactItem bDContactItem = new BDContactItem();
					bDContactItem.MGroupName = contactGroupName;
					bDContactItem.MItemID = $"{item.MItemID}_{Convert.ToInt32(BDContactType.Employee)}";
					bDContactItem.MName = item.MName;
					bDContactItem.MType = BDContactType.Employee;
					bDContactItem.MAccountID = item.MCurrentAccountId;
					list.Add(bDContactItem);
				}
			}
			return list;
		}

		private List<BDContactsInfoModel> FilterContact(List<BDContactsInfoModel> contactList, BDContactsListFilter filter)
		{
			if (filter.MaxCount > 0)
			{
				return (from t in contactList
				where string.IsNullOrEmpty(filter.QueryString) || t.MName.Contains(filter.QueryString)
				select t).Take(filter.MaxCount).ToList();
			}
			return (from t in contactList
			where string.IsNullOrEmpty(filter.QueryString) || t.MName.Contains(filter.QueryString)
			select t).ToList();
		}

		private List<BDContactItem> GetCustomerList(MContext ctx, List<BDContactsInfoModel> contactList, BDContactsListFilter filter, BDContactsInfoModel containContactModel)
		{
			List<BDContactItem> list = new List<BDContactItem>();
			if (containContactModel?.MIsCustomer ?? false)
			{
				list.AddRange(GetContactItemList(ctx, BDContactType.Customer, containContactModel));
			}
			List<BDContactsInfoModel> list2 = (from t in contactList
			where t.MIsCustomer
			select t).ToList();
			list2 = FilterContact(list2, filter);
			list.AddRange(GetContactItemList(ctx, BDContactType.Customer, list2));
			return list;
		}

		private List<BDContactItem> GetSupplierList(MContext ctx, List<BDContactsInfoModel> contactList, BDContactsListFilter filter, BDContactsInfoModel containContactModel)
		{
			List<BDContactItem> list = new List<BDContactItem>();
			if (containContactModel?.MIsSupplier ?? false)
			{
				list.AddRange(GetContactItemList(ctx, BDContactType.Supplier, containContactModel));
			}
			List<BDContactsInfoModel> list2 = (from t in contactList
			where t.MIsSupplier
			select t).ToList();
			list2 = FilterContact(list2, filter);
			list.AddRange(GetContactItemList(ctx, BDContactType.Supplier, list2));
			return list;
		}

		private List<BDContactItem> GetOtherContactList(MContext ctx, List<BDContactsInfoModel> contactList, BDContactsListFilter filter, BDContactsInfoModel containContactModel)
		{
			List<BDContactItem> list = new List<BDContactItem>();
			if (containContactModel?.MIsOther ?? false)
			{
				list.AddRange(GetContactItemList(ctx, BDContactType.Other, containContactModel));
			}
			List<BDContactsInfoModel> list2 = (from t in contactList
			where t.MIsOther
			select t).ToList();
			list2 = FilterContact(list2, filter);
			list.AddRange(GetContactItemList(ctx, BDContactType.Other, list2));
			return list;
		}

		private List<BDContactItem> GetContactItemList(MContext ctx, BDContactType type, BDContactsInfoModel contactModel)
		{
			List<BDContactItem> result = new List<BDContactItem>();
			if (contactModel == null)
			{
				return result;
			}
			return GetContactItemList(ctx, type, new List<BDContactsInfoModel>
			{
				contactModel
			});
		}

		private List<BDContactItem> GetContactItemList(MContext ctx, BDContactType type, List<BDContactsInfoModel> contactList)
		{
			List<BDContactItem> list = new List<BDContactItem>();
			if (contactList == null || contactList.Count == 0)
			{
				return list;
			}
			string contactGroupName = GetContactGroupName(ctx, type);
			foreach (BDContactsInfoModel contact in contactList)
			{
				BDContactItem bDContactItem = new BDContactItem();
				bDContactItem.MGroupName = contactGroupName;
				bDContactItem.MItemID = $"{contact.MItemID}_{Convert.ToInt32(type)}";
				bDContactItem.MName = contact.MName;
				bDContactItem.MType = type;
				bDContactItem.MAccountID = contact.MCCurrentAccountId;
				list.Add(bDContactItem);
			}
			return list;
		}

		private string GetContactGroupName(MContext ctx, BDContactType type)
		{
			switch (type)
			{
			case BDContactType.Supplier:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "Supplier", "Supplier");
			case BDContactType.Customer:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "Customer", "Customer");
			case BDContactType.Employee:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "Employee", "Employee");
			default:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "Other", "Other");
			}
		}

		public List<BDContactsInfoModel> GetContactsInfo(MContext ctx, string typeId, string searchFilter)
		{
			return _contacts.GetContactsInfo(ctx, typeId, searchFilter);
		}

		public List<BDContactsTypeLModel> GetTypeListByWhere(MContext ctx, bool isAll = true)
		{
			return _contacts.GetTypeListByWhere(ctx, isAll);
		}

		public OperationResult ContactsUpdate(MContext ctx, BDContactsInfoModel info, List<string> fields = null)
		{
			return ContactsUpdate(ctx, info, null, fields);
		}

		public OperationResult ContactsUpdate(MContext ctx, BDContactsInfoModel info, List<CommandInfo> cmdList, List<string> fields = null)
		{
			OperationResult operationResult = new OperationResult();
			List<string> codeList = new List<string>
			{
				info.MCCurrentAccountCode
			};
			_accountBiz.CheckAccountExist(ctx, codeList, operationResult);
			if (!info.MIsCustomer && !info.MIsSupplier && !info.MIsOther)
			{
				operationResult.Success = false;
				string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "PleaseSelectCustomerSupplierOrOther", "请选择联系人类型！");
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = text,
					CheckItem = "Contact"
				});
			}
			if (operationResult.Success)
			{
				operationResult = _contacts.ContactsUpdate(ctx, info, cmdList, fields);
			}
			return operationResult;
		}

		public OperationResult AllowContactChangeAccount(MContext ctx, BDContactsInfoModel info)
		{
			OperationResult operationResult = new OperationResult();
			bool flag;
			int num;
			if (ctx.MRegProgress >= 13 && !ctx.MInitBalanceOver && !string.IsNullOrWhiteSpace(info.MItemID))
			{
				BDContactsModel dataModel = _contacts.GetDataModel(ctx, info.MItemID, false);
				if (dataModel.MCCurrentAccountCode != info.MCCurrentAccountCode)
				{
					flag = false;
					List<IVInvoiceModel> initInvoiceList = IVInvoiceRepository.GetInitInvoiceList(ctx);
					List<IVReceiveModel> initList = IVReceiveRepository.GetInitList(ctx);
					List<IVPaymentModel> initList2 = IVPaymentRepository.GetInitList(ctx);
					if (initInvoiceList != null && initInvoiceList.Exists((IVInvoiceModel x) => x.MContactID == info.MItemID))
					{
						goto IL_00e6;
					}
					if (initList != null && initList.Exists((IVReceiveModel x) => x.MContactID == info.MItemID))
					{
						goto IL_00e6;
					}
					num = ((initList2?.Exists((IVPaymentModel x) => x.MContactID == info.MItemID) ?? false) ? 1 : 0);
					goto IL_00e7;
				}
				operationResult.Success = true;
			}
			else
			{
				operationResult.Success = true;
			}
			goto IL_0135;
			IL_00e7:
			if (num != 0)
			{
				flag = true;
			}
			if (flag && ctx.MInitBalanceOver)
			{
				operationResult.Success = false;
			}
			else
			{
				operationResult.Success = true;
			}
			goto IL_0135;
			IL_00e6:
			num = 1;
			goto IL_00e7;
			IL_0135:
			return operationResult;
		}

		public OperationResult ContactsGroupUpdate(MContext ctx, BDContactsGroupModel info)
		{
			OperationResult operationResult = new OperationResult();
			if (CheckContactGroupIsExist(ctx, info))
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "ContactGroupNameExist", "联系人分组名称已经存在，请使用其他名称");
				return operationResult;
			}
			return _contacts.ContactsGroupUpdate(ctx, info);
		}

		private bool CheckContactGroupIsExist(MContext ctx, BDContactsGroupModel contactGroup)
		{
			bool result = false;
			List<BDContactsTypeLModel> groupTypeList = _contacts.GetGroupTypeList(ctx);
			if (groupTypeList != null && groupTypeList.Count() > 0)
			{
				IEnumerable<MultiLanguageFieldList> enumerable = from m in contactGroup.MultiLanguage
				where m.MFieldName == "MName"
				select m;
				foreach (MultiLanguageFieldList item in enumerable)
				{
					List<MultiLanguageField> mMultiLanguageField = item.MMultiLanguageField;
					foreach (MultiLanguageField item2 in mMultiLanguageField)
					{
						if (groupTypeList.Any((BDContactsTypeLModel m) => m.MName == item2.MValue && m.MParentID != contactGroup.MItemID))
						{
							result = true;
						}
					}
				}
			}
			return result;
		}

		public OperationResult ContactToGroup(MContext ctx, string selIds, string typeId, MultiLanguageFieldList newGroupLangModel, bool isExist, string moveFromTypeId)
		{
			OperationResult operationResult = new OperationResult();
			if (!isExist)
			{
				BDContactsGroupModel emptyDataEditModel = ModelInfoManager.GetEmptyDataEditModel<BDContactsGroupModel>(ctx);
				emptyDataEditModel.MOrgID = ctx.MOrgID;
				foreach (MultiLanguageFieldList item in emptyDataEditModel.MultiLanguage)
				{
					item.MParentID = emptyDataEditModel.MItemID;
					item.MMultiLanguageField = newGroupLangModel.MMultiLanguageField;
				}
				if (CheckContactGroupIsExist(ctx, emptyDataEditModel))
				{
					operationResult.Success = false;
					operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "ContactGroupNameExist", "联系人分组名称已经存在，请使用其他名称");
					return operationResult;
				}
			}
			return _contacts.ContactToGroup(ctx, selIds, typeId, newGroupLangModel, isExist, moveFromTypeId);
		}

		public void ContactToArchivedGroup(MContext ctx, string selIds, string typeName = "Archived")
		{
			_contacts.ContactToArchivedGroup(ctx, selIds, "Archived");
		}

		public void ContactMoveOutGroup(MContext ctx, string selIds, string moveFromTypeId)
		{
			_contacts.ContactMoveOutGroup(ctx, selIds, moveFromTypeId);
		}

		public OperationResult ArchiveContact(MContext ctx, List<string> contactIds, bool isActive)
		{
			return _contacts.ArchiveContact(ctx, contactIds, isActive);
		}

		public OperationResult DelGroupAndLink(MContext ctx, string typeId)
		{
			OperationResult operationResult = new OperationResult();
			if (string.IsNullOrWhiteSpace(typeId))
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "CanNotSelectGroup", "没有需要删除的分组！");
				return operationResult;
			}
			if (_contacts.IsSysContactType(ctx, typeId))
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "SystemDefaultGroupUnAllowDelete", "系统默认分组不允许删除！");
				return operationResult;
			}
			List<BDContactsTypeLinkModel> contactTypeLinkList = _contacts.GetContactTypeLinkList(ctx, typeId, null, false);
			if (contactTypeLinkList != null && contactTypeLinkList.Count() > 0)
			{
				List<string> ids = (from x in contactTypeLinkList
				select x.MContactID).ToList();
				BDContactsRepository bDContactsRepository = new BDContactsRepository();
				List<BDContactsInfoModel> contactByIDs = bDContactsRepository.GetContactByIDs(ctx, ids, false);
				if (contactByIDs.Exists((BDContactsInfoModel x) => x.MIsActive))
				{
					operationResult.Success = false;
					operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "ContactGroupHasContact", "需要删除的分组有联系人，请先移除这些联系人！");
					return operationResult;
				}
			}
			return _contacts.DelGroupAndLink(ctx, typeId);
		}

		public BDContactsInfoModel GetContactEditInfo(MContext ctx, BDContactsInfoModel model)
		{
			BDContactsInfoModel contactEditInfo = _contacts.GetContactEditInfo(ctx, model);
			if (contactEditInfo != null && ctx.MRegProgress == 15)
			{
				string cCurrencyMoneyCode = (contactEditInfo.MCCurrentAccountCode == null) ? "1122" : contactEditInfo.MCCurrentAccountCode;
				BDAccountRepository bDAccountRepository = new BDAccountRepository();
				SqlWhere sqlWhere = new SqlWhere();
				sqlWhere.Equal("MAccountTableId", ctx.MAccountTableID);
				sqlWhere.In("MCode", new string[1]
				{
					cCurrencyMoneyCode
				});
				List<BDAccountModel> baseBDAccountList = bDAccountRepository.GetBaseBDAccountList(ctx, sqlWhere, false, null);
				if (baseBDAccountList != null && baseBDAccountList.Count() > 0)
				{
					BDAccountModel bDAccountModel = (from x in baseBDAccountList
					where x.MCode == cCurrencyMoneyCode
					select x).FirstOrDefault();
					contactEditInfo.MCCurrentAccountId = ((bDAccountModel != null) ? bDAccountModel.MItemID : "");
				}
			}
			return contactEditInfo;
		}

		public List<BDContactsTrackLinkModel> GetTrackLinkList(MContext ctx, SqlWhere filter)
		{
			return _consTrc.GetModelList(ctx, filter, false);
		}

		public BDContactBasicInfoModel GetContactTrackList(MContext ctx, string contactId)
		{
			BDContactBasicInfoModel bDContactBasicInfoModel = new BDContactBasicInfoModel();
			SetContactTrackInfo(ctx, ref bDContactBasicInfoModel, contactId);
			BDContactsInfoModel contactViewData = _contacts.GetContactViewData(ctx, contactId);
			if (contactViewData != null)
			{
				bDContactBasicInfoModel.MDefaultCurrencyID = contactViewData.MDefaultCurrencyID;
				bDContactBasicInfoModel.MDefaultDiscount = contactViewData.MDiscount;
				bDContactBasicInfoModel.MDefaultSaleTaxID = contactViewData.MDefaultSaleTaxID;
				bDContactBasicInfoModel.MDefaultPurchaseTaxID = contactViewData.MDefaultPurchaseTaxID;
				bDContactBasicInfoModel.MPurchaseDueCondition = contactViewData.MPurDueCondition;
				bDContactBasicInfoModel.MPurchaseDueDate = contactViewData.MPurDueDate;
				bDContactBasicInfoModel.MSaleDueDate = contactViewData.MSalDueDate;
				bDContactBasicInfoModel.MSaleDueCondition = contactViewData.MSalDueCondition;
			}
			return bDContactBasicInfoModel;
		}

		private void SetContactTrackInfo(MContext ctx, ref BDContactBasicInfoModel model, string contactId)
		{
			List<BDContactsTrackLinkModel> modelList = _consTrc.GetModelList(ctx, new SqlWhere().AddFilter("MContactID", SqlOperators.Equal, contactId), false);
			if (modelList != null && modelList.Count != 0)
			{
				int num = 1;
				foreach (BDContactsTrackLinkModel item in modelList)
				{
					switch (num)
					{
					case 1:
						model.MSaleTrackItem1 = item.MSalTrackId;
						model.MPurchaseTrackItem1 = item.MPurTrackId;
						break;
					case 2:
						model.MSaleTrackItem2 = item.MSalTrackId;
						model.MPurchaseTrackItem2 = item.MPurTrackId;
						break;
					case 3:
						model.MSaleTrackItem3 = item.MSalTrackId;
						model.MPurchaseTrackItem3 = item.MPurTrackId;
						break;
					case 4:
						model.MSaleTrackItem4 = item.MSalTrackId;
						model.MPurchaseTrackItem4 = item.MPurTrackId;
						break;
					case 5:
						model.MSaleTrackItem5 = item.MSalTrackId;
						model.MPurchaseTrackItem5 = item.MPurTrackId;
						break;
					}
					num++;
				}
			}
		}

		public BDContactsInfoModel GetStatementContData(MContext ctx, string contactID)
		{
			BDContactsInfoModel statementContData = _contacts.GetStatementContData(ctx, contactID);
			if (statementContData != null)
			{
				statementContData.IsQuote = JudgeContactIsQuote(ctx, contactID);
			}
			return statementContData;
		}

		public void CheckContactExist<T>(MContext ctx, T model, List<IOValidationResultModel> validationResult, List<BDContactsModel> contactList, string contactField = "MContactID", string idFieldName = "MItemID")
		{
			string contactValue = ModelHelper.GetModelValue(model, contactField);
			if (!string.IsNullOrWhiteSpace(contactValue))
			{
				string empty = string.Empty;
				BDContactsModel bDContactsModel = contactList.FirstOrDefault((BDContactsModel f) => HttpUtility.HtmlDecode(ModelHelper.GetModelValue<BDContactsModel>(f, "MName")).ToUpper().Trim() == contactValue.ToUpper().Trim());
				if (bDContactsModel != null)
				{
					ModelHelper.SetModelValue(model, contactField, bDContactsModel.MItemID, null);
				}
				else
				{
					int rowIndex = 0;
					int.TryParse(ModelHelper.TryGetModelValue(model, "MRowIndex"), out rowIndex);
					validationResult.Add(new IOValidationResultModel
					{
						Id = ModelHelper.GetModelValue(model, idFieldName),
						FieldType = IOValidationTypeEnum.Contact,
						FieldValue = contactValue,
						Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "ContactNotFound", "The contact:{0} can't be found!"),
						RowIndex = rowIndex
					});
				}
			}
		}

		public List<CommandInfo> CheckContactExist<T>(MContext ctx, List<T> modelList, ref string newContacts, List<IOValidationResultModel> validationResult, bool autoCreateUnExistContact = true, string idFieldName = "MItemID", int rowIndex = -1)
		{
			List<CommandInfo> result = new List<CommandInfo>();
			string nameField = "MName";
			string contactField = "MContactID";
			List<string> list = (from f in modelList
			where !string.IsNullOrWhiteSpace(ModelHelper.GetModelValue<T>(f, contactField))
			select ModelHelper.GetModelValue<T>(f, contactField).Trim()).Distinct().ToList();
			if (!list.Any())
			{
				return result;
			}
			List<BDContactsModel> contactListByNameOrId = _contacts.GetContactListByNameOrId(ctx, list, false, true);
			List<BDContactsModel> list2 = new List<BDContactsModel>();
			List<string> list3 = new List<string>();
			List<string> list4 = new List<string>();
			bool flag = typeof(T) == typeof(GLVoucherEntryModel);
			bool flag2 = typeof(T) == typeof(IVPaymentModel);
			bool flag3 = typeof(T) == typeof(IVReceiveModel);
			List<BDContactsModel> list5 = new List<BDContactsModel>();
			string text = string.Empty;
			if (typeof(T) == typeof(IVInvoiceModel) || typeof(T) == typeof(IOInvoiceImportModel))
			{
				string modelValue = ModelHelper.GetModelValue(modelList[0], "MType");
				switch (modelValue)
				{
				case "Invoice_Sale":
				case "Invoice_Sale_Red":
					text = "Customer";
					break;
				case "Invoice_Purchase":
				case "Invoice_Purchase_Red":
					text = "Supplier";
					break;
				}
			}
			int num = 0;
			foreach (T model in modelList)
			{
				string contactValue = ModelHelper.GetModelValue(model, contactField);
				string value;
				BDContactsModel bDContactsModel;
				int num2;
				if (!string.IsNullOrWhiteSpace(contactValue))
				{
					value = string.Empty;
					contactValue = contactValue.Trim();
					bDContactsModel = contactListByNameOrId.FirstOrDefault((BDContactsModel f) => f.MItemID == contactValue || HttpUtility.HtmlDecode(ModelHelper.GetModelValue<BDContactsModel>(f, nameField)).ToUpper().Trim() == contactValue.ToUpper().Trim());
					if (bDContactsModel == null)
					{
						bDContactsModel = list5.FirstOrDefault((BDContactsModel f) => HttpUtility.HtmlDecode(ModelHelper.GetModelValue<BDContactsModel>(f, nameField)).ToUpper().Trim() == contactValue.ToUpper().Trim());
					}
					if (flag2 | flag3)
					{
						text = ModelHelper.GetModelValue(model, "MContactType");
					}
					if (bDContactsModel != null)
					{
						ModelHelper.SetModelValue(model, "IsNew", false, null);
						if (text == "Customer" && bDContactsModel.MIsCustomer)
						{
							goto IL_02c3;
						}
						if (text == "Supplier" && bDContactsModel.MIsSupplier)
						{
							goto IL_02c3;
						}
						num2 = ((text == "Other" && bDContactsModel.MIsOther) ? 1 : 0);
						goto IL_02c4;
					}
					if (autoCreateUnExistContact)
					{
						list4.Add(contactValue);
						BDContactsModel bDContactsModel2 = AddNewContactCmd(ctx, result, text, contactValue, contactField, validationResult);
						ModelHelper.SetModelValue(model, contactField, bDContactsModel2.MItemID, null);
						list5.Add(bDContactsModel2);
					}
					else
					{
						value = contactValue;
					}
					goto IL_043d;
				}
				continue;
				IL_02c4:
				bool flag4 = (byte)num2 != 0;
				if (!bDContactsModel.MIsActive)
				{
					validationResult.Add(new IOValidationResultModel
					{
						FieldType = IOValidationTypeEnum.Contact,
						FieldValue = contactValue,
						Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "ContactHasDisabled", "联系人：{0}已禁用!")
					});
				}
				else if (flag | flag4)
				{
					ModelHelper.SetModelValue(model, contactField, bDContactsModel.MItemID, null);
				}
				else if (autoCreateUnExistContact)
				{
					AddContactTypeUpdCmd(ctx, result, text, contactValue, bDContactsModel, validationResult);
					ModelHelper.SetModelValue(model, contactField, bDContactsModel.MItemID, null);
				}
				else
				{
					validationResult.Add(new IOValidationResultModel
					{
						FieldType = IOValidationTypeEnum.Contact,
						FieldValue = contactValue,
						Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "ExistOtherTypeContact", "存在其他类型的联系人：{0}!")
					});
				}
				goto IL_043d;
				IL_043d:
				if (!string.IsNullOrWhiteSpace(value))
				{
					if (rowIndex == -1)
					{
						int.TryParse(ModelHelper.TryGetModelValue(model, "MRowIndex"), out num);
					}
					validationResult.Add(new IOValidationResultModel
					{
						Id = ModelHelper.TryGetModelValue(model, idFieldName),
						FieldType = IOValidationTypeEnum.Contact,
						FieldValue = contactValue,
						Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "ContactNotFound", "The contact:{0} can't be found!"),
						RowIndex = num
					});
				}
				num++;
				continue;
				IL_02c3:
				num2 = 1;
				goto IL_02c4;
			}
			newContacts = string.Join("、", list4);
			return result;
		}

		private void AddContactTypeUpdCmd(MContext ctx, List<CommandInfo> cmdList, string contactType, string contactName, BDContactsModel matchedContact, List<IOValidationResultModel> validationResult)
		{
			if (!SECPermissionRepository.HavePermission(ctx, "Contact", "Change", ""))
			{
				validationResult.Add(new IOValidationResultModel
				{
					FieldType = IOValidationTypeEnum.Contact,
					FieldValue = contactName,
					Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "NoPermissionAddContactType", "您没有权限添加联系人类型！")
				});
			}
			else
			{
				List<string> list = new List<string>();
				if (contactType == "Customer")
				{
					matchedContact.MIsCustomer = true;
					list.Add("MIsCustomer");
				}
				else if (contactType == "Supplier")
				{
					matchedContact.MIsSupplier = true;
					list.Add("MIsSupplier");
				}
				else
				{
					matchedContact.MIsOther = true;
					list.Add("MIsOther");
				}
				matchedContact.IsNew = false;
				matchedContact.IsUpdate = true;
				cmdList.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDContactsModel>(ctx, matchedContact, list, false));
			}
		}

		private BDContactsModel AddNewContactCmd(MContext ctx, List<CommandInfo> cmdList, string contactType, string contactName, string contactField, List<IOValidationResultModel> validationResult)
		{
			if (!SECPermissionRepository.HavePermission(ctx, "Contact", "Change", ""))
			{
				validationResult.Add(new IOValidationResultModel
				{
					FieldType = IOValidationTypeEnum.Contact,
					FieldValue = contactName,
					Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "NoPermissionCreateContact", "您没有权限创建找不到的联系人：{0}!")
				});
				return new BDContactsModel();
			}
			BDContactsModel bDContactsModel = new BDContactsModel();
			bDContactsModel.MName = contactName;
			bDContactsModel.MOrgID = ctx.MOrgID;
			bDContactsModel.MultiLanguage = new List<MultiLanguageFieldList>();
			bDContactsModel.MultiLanguage.Add(base.GetMultiLanguageList("MName", contactName));
			BDContactsInfoModel bDContactsInfoModel = new BDContactsInfoModel
			{
				MultiLanguage = bDContactsModel.MultiLanguage
			};
			new BDContactsRepository().MultiLanguageAdd(bDContactsInfoModel);
			bDContactsModel.MultiLanguage = bDContactsInfoModel.MultiLanguage;
			bDContactsModel.MIsCustomer = (contactType == "Customer");
			bDContactsModel.MIsSupplier = (contactType == "Supplier");
			bDContactsModel.MIsOther = (contactType == "Other");
			cmdList.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDContactsModel>(ctx, bDContactsModel, null, true));
			return bDContactsModel;
		}

		private bool JudgeContactIsQuote(MContext ctx, string contactId)
		{
			bool result = true;
			OperationResult operationResult = BDRepository.IsCanDelete(ctx, "Contact", contactId);
			if (operationResult != null)
			{
				result = !operationResult.Success;
			}
			return result;
		}

		public BDContactsInfoModel GetContactViewData(MContext ctx, string contactID)
		{
			BDContactsInfoModel contactViewData = _contacts.GetContactViewData(ctx, contactID);
			SetPintInfo(ctx, contactViewData);
			return contactViewData;
		}

		public List<BDContactsInfoModel> GetContactViewDataList(MContext ctx, string contactIDs)
		{
			List<BDContactsInfoModel> contactViewDataList = _contacts.GetContactViewDataList(ctx, contactIDs);
			foreach (BDContactsInfoModel item in contactViewDataList)
			{
				SetPintInfo(ctx, item);
			}
			return contactViewDataList;
		}

		public List<BDContactsInfoModel> GetContactsListByContactType(MContext ctx, int contactType = 0, string keyWord = null)
		{
			return _contacts.GetContactsListByContactType(ctx, contactType, keyWord, 0, false, true);
		}

		public List<BDContactsInfoModel> GetContactsList(MContext ctx, BDContactsListFilter filter)
		{
			List<BDContactsInfoModel> list = new List<BDContactsInfoModel>();
			list = _contacts.GetContactsListByContactType(ctx, filter.ContactType, filter.QueryString, filter.MaxCount, filter.IncludeDisable, true);
			if (string.IsNullOrEmpty(filter.MID))
			{
				return GetSingleContactList(list);
			}
			if (list == null)
			{
				list = new List<BDContactsInfoModel>();
			}
			if (list.Count > 0 && list.Count((BDContactsInfoModel t) => t.MItemID == filter.MID) > 0)
			{
				return GetSingleContactList(list);
			}
			List<BDContactsInfoModel> list2 = _contacts.GetContactByIDs(ctx, new List<string>
			{
				filter.MID
			}, filter.IncludeDisable);
			if (list2 == null || list2.Count == 0)
			{
				return list;
			}
			switch (filter.ContactType)
			{
			case 1:
				list2 = (from t in list2
				where t.MIsCustomer
				select t).ToList();
				break;
			case 2:
				list2 = (from t in list2
				where t.MIsSupplier
				select t).ToList();
				break;
			case 3:
				list2 = (from t in list2
				where t.MIsCustomer || t.MIsSupplier
				select t).ToList();
				break;
			case 4:
				list2 = (from t in list2
				where t.MIsOther
				select t).ToList();
				break;
			}
			if (list2 == null || list2.Count == 0)
			{
				return list;
			}
			list.InsertRange(0, list2);
			return GetSingleContactList(list);
		}

		private List<BDContactsInfoModel> GetSingleContactList(List<BDContactsInfoModel> contactList)
		{
			if (contactList == null || contactList.Count() == 0)
			{
				return contactList;
			}
			List<string> list = (from x in contactList
			select x.MItemID).Distinct().ToList();
			List<BDContactsInfoModel> list2 = new List<BDContactsInfoModel>();
			foreach (string item in list)
			{
				list2.Add(contactList.First((BDContactsInfoModel x) => x.MItemID == item));
			}
			return list2;
		}

		private void SetPintInfo(MContext ctx, BDContactsInfoModel model)
		{
			if (model != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				StringBuilder stringBuilder2 = new StringBuilder();
				if (!string.IsNullOrWhiteSpace(model.MPAttention))
				{
					stringBuilder.AppendFormat("{0}: {1}", COMMultiLangRepository.GetText(ctx.MLCID, LangKey.Attention), model.MPAttention);
				}
				if (!string.IsNullOrWhiteSpace(model.MRealAttention))
				{
					stringBuilder2.AppendFormat("{0}: {1}", COMMultiLangRepository.GetText(ctx.MLCID, LangKey.Attention), model.MRealAttention);
				}
				string text = '┇'.ToString();
				if (ctx.MLCID == "0x0009")
				{
					if (!string.IsNullOrWhiteSpace(model.MPStreet))
					{
						stringBuilder.Append(text);
						stringBuilder.Append(model.MPStreet);
					}
					if (!string.IsNullOrWhiteSpace(model.MRealStreet))
					{
						stringBuilder2.Append(text);
						stringBuilder2.Append(model.MRealStreet);
					}
					if (!string.IsNullOrWhiteSpace(model.MPCityID))
					{
						stringBuilder.Append(text);
						stringBuilder.Append(model.MPCityID);
					}
					if (!string.IsNullOrWhiteSpace(model.MRealCityID))
					{
						stringBuilder2.Append(text);
						stringBuilder2.Append(model.MRealCityID);
					}
					if (!string.IsNullOrWhiteSpace(model.MPRegion))
					{
						stringBuilder.Append(" ");
						stringBuilder.Append(model.MPRegion);
					}
					if (!string.IsNullOrWhiteSpace(model.MRealRegion))
					{
						stringBuilder2.Append(" ");
						stringBuilder2.Append(model.MRealRegion);
					}
					if (!string.IsNullOrWhiteSpace(model.MPCountryName))
					{
						stringBuilder.Append(text);
						stringBuilder.Append(model.MPCountryName);
					}
					if (!string.IsNullOrWhiteSpace(model.MRealCountryName))
					{
						stringBuilder2.Append(text);
						stringBuilder2.Append(model.MRealCountryName);
					}
					if (!string.IsNullOrWhiteSpace(model.MPPostalNo))
					{
						stringBuilder.Append(" ");
						stringBuilder.Append(model.MPPostalNo);
					}
					if (!string.IsNullOrWhiteSpace(model.MRealPostalNo))
					{
						stringBuilder2.Append(" ");
						stringBuilder2.Append(model.MRealPostalNo);
					}
				}
				else
				{
					stringBuilder.AppendFormat("{4}{0}{1}{2}{3}", model.MPCountryName, model.MPRegion, model.MPCityID, model.MPStreet, text);
					if (!string.IsNullOrWhiteSpace(model.MPPostalNo))
					{
						stringBuilder.Append(", ");
						stringBuilder.Append(model.MPPostalNo);
					}
					stringBuilder2.AppendFormat("{4}{0}{1}{2}{3}", model.MRealCountryName, model.MRealRegion, model.MRealCityID, model.MRealStreet, text);
					if (!string.IsNullOrWhiteSpace(model.MRealPostalNo))
					{
						stringBuilder2.Append(", ");
						stringBuilder2.Append(model.MRealPostalNo);
					}
				}
				model.MPostalAddressInfo = stringBuilder.ToString();
				model.MPhysicalAddressInfo = stringBuilder2.ToString();
				StringBuilder stringBuilder3 = new StringBuilder();
				if (model.MPhone != null && !string.IsNullOrWhiteSpace(model.MPhone.Trim('-')))
				{
					stringBuilder3.AppendFormat("{0}: {1}", COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "Telephone", "Telephone"), model.MPhone.Trim('-'));
				}
				if (model.MFax != null && !string.IsNullOrWhiteSpace(model.MFax.Trim('-')))
				{
					stringBuilder3.Append(text);
					stringBuilder3.AppendFormat("{0}: {1}", COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "Fax", "Fax"), model.MFax.Trim('-'));
				}
				if (model.MMobile != null && !string.IsNullOrWhiteSpace(model.MMobile.Trim('-')))
				{
					stringBuilder3.Append(text);
					stringBuilder3.AppendFormat("{0}: {1}", COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "Mobile", "Mobile"), model.MMobile.Trim('-'));
				}
				if (model.MDirectPhone != null && !string.IsNullOrWhiteSpace(model.MDirectPhone.Trim('-')))
				{
					stringBuilder3.Append(text);
					stringBuilder3.AppendFormat("{0}: {1}", COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "DirectDial", "Direct Dial"), model.MDirectPhone.Trim('-'));
				}
				model.MContactInfo = stringBuilder3.ToString();
			}
		}

		public OperationResult AddContactNoteLog(MContext ctx, BDContactsModel model)
		{
			return BDContactsRepository.AddContactNoteLog(ctx, model);
		}

		public List<BDContactsModel> GetContactListByNameOrId(MContext ctx, List<string> nameOrIdList)
		{
			return _contacts.GetContactListByNameOrId(ctx, nameOrIdList, true, false);
		}

		public BDContactsInfoModel GetInsertContactModel(MContext ctx, string contactName, List<BASLangModel> languageList)
		{
			BDContactsInfoModel bDContactsInfoModel = new BDContactsInfoModel();
			bDContactsInfoModel.MItemID = UUIDHelper.GetGuid();
			bDContactsInfoModel.MName = contactName;
			bDContactsInfoModel.MContactName = contactName;
			bDContactsInfoModel.IsNew = true;
			if (languageList == null)
			{
				languageList = new BASLangBusiness().GetOrgLangList(ctx);
			}
			MultiLanguageFieldList multiLanguageFieldList = new MultiLanguageFieldList();
			multiLanguageFieldList.MMultiLanguageField = new List<MultiLanguageField>();
			multiLanguageFieldList.MFieldName = "MName";
			foreach (BASLangModel language in languageList)
			{
				MultiLanguageField multiLanguageField = new MultiLanguageField();
				multiLanguageField.MLocaleID = language.LangID;
				multiLanguageField.MOrgID = ctx.MOrgID;
				multiLanguageField.MValue = contactName;
				multiLanguageFieldList.MMultiLanguageField.Add(multiLanguageField);
			}
			bDContactsInfoModel.MultiLanguage = new List<MultiLanguageFieldList>();
			bDContactsInfoModel.MultiLanguage.Add(multiLanguageFieldList);
			return bDContactsInfoModel;
		}

		public bool ValidateContactRequiredFields(MContext ctx, BDContactsInfoModel contactModel, List<ValidationError> validationErrors)
		{
			bool result = true;
			MultiLanguageFieldList multiLanguageFieldList = contactModel.MultiLanguage.FirstOrDefault((MultiLanguageFieldList f) => f.MFieldName == "MName");
			if (string.IsNullOrWhiteSpace(contactModel.MName) || (multiLanguageFieldList?.MMultiLanguageField.Any((MultiLanguageField f) => string.IsNullOrWhiteSpace(f.MValue)) ?? false))
			{
				result = false;
				validationErrors.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "ContactIsNull", "联系人为空!")));
			}
			if (!contactModel.MIsCustomer && !contactModel.MIsSupplier && !contactModel.MIsOther)
			{
				result = false;
				validationErrors.Add(new ValidationError
				{
					Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "PleaseSelectCustomerSupplierOrOther", "请选择联系人类型！")
				});
			}
			return result;
		}
	}
}
