using JieNor.Megi.BusinessService.BAS;
using JieNor.Megi.BusinessService.BD;
using JieNor.Megi.BusinessService.GL;
using JieNor.Megi.BusinessService.IV;
using JieNor.Megi.BusinessService.REG;
using JieNor.Megi.Core;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Helper;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.IO;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.DataRepository.BAS;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace JieNor.Megi.BusinessService.IO
{
	public class ImportBase : BusinessServiceBase
	{
		private BDContactsRepository _contact = new BDContactsRepository();

		private REGCurrencyBusiness _currencyBiz = new REGCurrencyBusiness();

		private BDAccountBusiness _accountBiz = new BDAccountBusiness();

		private BDTrackBusiness _trackBiz = new BDTrackBusiness();

		private REGTaxRateBusiness _taxRateBiz = new REGTaxRateBusiness();

		private BASDataDictionaryBusiness _dictBiz = new BASDataDictionaryBusiness();

		private BDExpenseItemBusiness _expenseItemBiz = new BDExpenseItemBusiness();

		private BDBankAccountBusiness _bankAccountBiz = new BDBankAccountBusiness();

		private BDEmployeesBusiness _employeeBiz = new BDEmployeesBusiness();

		private BDContactsBusiness _contactBiz = new BDContactsBusiness();

		private BDItemBusiness _itemBiz = new BDItemBusiness();

		private IVInvoiceBusiness _invoiceBiz = new IVInvoiceBusiness();

		private GLVoucherBusiness voucherBiz = new GLVoucherBusiness();

		protected BASCurrencyViewModel _basCurrency = null;

		protected List<REGCurrencyViewModel> _foreignCurrencyList = null;

		protected BASCurrencyViewModel BasCurrency
		{
			get;
			set;
		}

		protected List<REGCurrencyViewModel> ForeignCurrencyList
		{
			get;
			set;
		}

		public Dictionary<string, bool> CurrentAccountList
		{
			get
			{
				Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
				dictionary.Add("1122", true);
				dictionary.Add("2203", true);
				dictionary.Add("2202", true);
				dictionary.Add("1123", true);
				dictionary.Add("1221", false);
				dictionary.Add("2241", false);
				return dictionary;
			}
		}

		public static Dictionary<string, ContactTypeEnum> DicAcctCanAssoContact
		{
			get
			{
				Dictionary<string, ContactTypeEnum> dictionary = new Dictionary<string, ContactTypeEnum>();
				dictionary.Add("1122", ContactTypeEnum.Customer);
				dictionary.Add("1123", ContactTypeEnum.Supplier);
				dictionary.Add("2202", ContactTypeEnum.Supplier);
				dictionary.Add("2203", ContactTypeEnum.Customer);
				return dictionary;
			}
		}

		protected virtual void SetCacheData(MContext ctx)
		{
			BasCurrency = GetBaseCurrency(ctx);
			ForeignCurrencyList = _currencyBiz.GetAllCurrencyList(ctx, false, false);
		}

		public List<T> ConvertToList<T>(DataTable dt)
		{
			return ModelInfoManager.DataTableToList<T>(dt);
		}

		public List<MultiLanguageFieldList> SetMultiLang(string name, string value)
		{
			return new List<MultiLanguageFieldList>
			{
				new MultiLanguageFieldList
				{
					MFieldName = name,
					MMultiLanguageField = new List<MultiLanguageField>
					{
						new MultiLanguageField
						{
							MLocaleID = "0x0009",
							MValue = value
						},
						new MultiLanguageField
						{
							MLocaleID = "0x7804",
							MValue = value
						},
						new MultiLanguageField
						{
							MLocaleID = "0x7C04",
							MValue = value
						}
					}
				}
			};
		}

		public bool IsLangValueExists(List<MultiLanguageFieldList> langFiledList, string fieldName, string fieldValue, bool includeStartWith = false)
		{
			if (langFiledList == null || langFiledList.Count == 0 || string.IsNullOrEmpty(fieldValue))
			{
				return false;
			}
			MultiLanguageFieldList multiLanguageFieldList = langFiledList.SingleOrDefault((MultiLanguageFieldList f) => f.MFieldName == fieldName);
			if (multiLanguageFieldList == null || multiLanguageFieldList.MMultiLanguageField == null || multiLanguageFieldList.MMultiLanguageField.Count == 0)
			{
				return false;
			}
			if (multiLanguageFieldList.MMultiLanguageField.Count((MultiLanguageField t) => !string.IsNullOrEmpty(t.MValue) && (t.MValue.ToLower() == fieldValue.ToLower() || (includeStartWith && t.MValue.ToLower().StartsWith(fieldValue.ToLower())))) == 0)
			{
				return false;
			}
			return true;
		}

		protected void CheckFieldDuplicate<T>(List<T> list, string fieldName, string errMsg)
		{
			List<string> list2 = new List<string>();
			string empty = string.Empty;
			foreach (T item in list)
			{
				empty = ModelHelper.GetModelValue(item, fieldName);
				if (!string.IsNullOrWhiteSpace(empty))
				{
					list2.Add(empty);
				}
			}
			List<string> list3 = (from c in list2
			group c by c into g
			where Enumerable.Count<string>((IEnumerable<string>)g) > 1
			select g into f
			select f.Key).ToList();
			if (!list3.Any())
			{
				return;
			}
			string message = string.Format(errMsg, string.Join(",", list3));
			throw new Exception(message);
		}

		protected BDContactsModel GetContactModel(MContext ctx, string supplierName, string customerName, string accountNo)
		{
			List<string> list = new List<string>();
			ContactTypeEnum value = DicAcctCanAssoContact.SingleOrDefault((KeyValuePair<string, ContactTypeEnum> f) => accountNo.StartsWith(f.Key)).Value;
			switch (value)
			{
			case ContactTypeEnum.Customer:
				list.Add(customerName);
				break;
			case ContactTypeEnum.Supplier:
				list.Add(supplierName);
				break;
			}
			List<BDContactsModel> contactListByName = GetContactListByName(ctx, list);
			if (contactListByName.Any())
			{
				BDContactsModel bDContactsModel = null;
				switch (value)
				{
				case ContactTypeEnum.Customer:
					bDContactsModel = contactListByName.FirstOrDefault((BDContactsModel f) => f.MIsCustomer);
					break;
				case ContactTypeEnum.Supplier:
					bDContactsModel = contactListByName.FirstOrDefault((BDContactsModel f) => f.MIsSupplier);
					break;
				}
				if (bDContactsModel == null)
				{
					bDContactsModel = contactListByName.First();
					switch (value)
					{
					case ContactTypeEnum.Customer:
						bDContactsModel.MIsCustomer = true;
						break;
					case ContactTypeEnum.Supplier:
						bDContactsModel.MIsSupplier = true;
						break;
					}
					bDContactsModel.IsUpdate = true;
				}
				return bDContactsModel;
			}
			string text = list.Any() ? list[0] : string.Empty;
			BDContactsModel newContactModel = GetNewContactModel(text, value);
			if (newContactModel != null)
			{
				newContactModel.MName = text;
			}
			return newContactModel;
		}

		protected BASCurrencyViewModel GetBaseCurrency(MContext ctx)
		{
			return _currencyBiz.GetBaseCurrency(ctx);
		}

		protected REGCurrencyViewModel GetCurrencyModel(List<REGCurrencyViewModel> list, string currencyName)
		{
			foreach (REGCurrencyViewModel item in list)
			{
				if (IsLangValueExists(item.MultiLanguage, "MName", currencyName, true))
				{
					return item;
				}
			}
			return null;
		}

		protected List<REGCurrencyViewModel> GetCurrencyList(MContext ctx, bool ignoreLocale = false)
		{
			return _currencyBiz.GetCurrencyListByName(ctx, true, ignoreLocale);
		}

		protected List<BDContactsModel> GetContactsList(MContext ctx)
		{
			return _contact.GetModelList(ctx, null, false);
		}

		protected List<BDContactsModel> GetContactListByName(MContext ctx, List<string> nameList)
		{
			return _contact.GetContactListByName(ctx, nameList, false);
		}

		protected List<BDAccountListModel> GetAccountList(MContext ctx)
		{
			return _accountBiz.GetAccountListIncludeBalance(ctx, new SqlWhere(), false);
		}

		protected List<BDBankAccountEditModel> GetBankAccountList(MContext ctx, bool ignoreLocale)
		{
			return BDBankAccountRepository.GetBankAccountList(ctx, null, ignoreLocale, null);
		}

		protected List<NameValueModel> GetTrackBasicInfo(MContext ctx)
		{
			return _trackBiz.GetTrackBasicInfo(ctx, null);
		}

		protected List<BDTrackModel> GetTrackListByName(MContext ctx, List<string> nameList)
		{
			return _trackBiz.GetListByName(ctx, nameList);
		}

		protected List<CommandInfo> GetNewTrackCommandList(MContext ctx, List<BDTrackModel> trackList, ref string errMsg)
		{
			return _trackBiz.GetNewTrackCommandList(ctx, trackList, ref errMsg);
		}

		protected List<BASDataDictionaryModel> GetDictList(MContext ctx, string type)
		{
			return BASDataDictionary.GetDictList(type, ctx.MLCID);
		}

		protected void CheckInvoiceNumberExist<T>(MContext ctx, List<T> modelList, List<IOValidationResultModel> validationResult)
		{
			_invoiceBiz.CheckInvoiceNumberExist(ctx, modelList, validationResult);
		}

		protected void CheckContactExist<T>(MContext ctx, List<T> modelList, List<IOValidationResultModel> validationResult)
		{
			string empty = string.Empty;
			_contactBiz.CheckContactExist(ctx, modelList, ref empty, validationResult, false, "MItemID", -1);
		}

		protected void CheckEmployeeExist<T>(MContext ctx, List<T> modelList, string fieldName, List<IOValidationResultModel> validationResult)
		{
			string empty = string.Empty;
			_employeeBiz.CheckEmployeeExist(ctx, modelList, fieldName, validationResult, ref empty, false, false);
		}

		protected void CheckCurrencyExist<T>(MContext ctx, List<T> modelList, string currencyField, List<IOValidationResultModel> validationResult)
		{
			_currencyBiz.CheckCurrencyExist(ctx, modelList, currencyField, validationResult);
		}

		protected void CheckTaxTypeExist<T>(MContext ctx, List<T> modelList, List<IOValidationResultModel> validationResult, string fieldName = null)
		{
			_dictBiz.CheckTaxTypeExist(ctx, modelList, validationResult, fieldName);
		}

		protected void CheckItemExist<T>(MContext ctx, List<T> modelList, List<IOValidationResultModel> validationResult)
		{
			_itemBiz.CheckItemExist(ctx, modelList, validationResult, "MItemID", "MItemID");
		}

		protected void CheckExpenseItemExist<T>(MContext ctx, List<T> modelList, ref List<IOValidationResultModel> validationResult)
		{
			_expenseItemBiz.CheckExpenseItemExist(ctx, modelList, validationResult, "MItemID");
		}

		protected void CheckTaxRateExist<T>(MContext ctx, List<T> entryList, List<IOValidationResultModel> validationResult)
		{
			_taxRateBiz.CheckTaxRateExist(ctx, entryList, validationResult);
		}

		protected void CheckBankAccountExist<T>(MContext ctx, List<T> entryList, string bankAccountField, List<IOValidationResultModel> validationResult)
		{
			_bankAccountBiz.CheckBankAccountExist(ctx, entryList, bankAccountField, validationResult);
		}

		protected void CheckTrackExist<T>(MContext ctx, string[] importTrackNameList, List<T> entryList, List<IOValidationResultModel> validationResult)
		{
			_trackBiz.CheckTrackExist(ctx, importTrackNameList, entryList, validationResult);
		}

		protected void CheckAccountExist<T1, T2>(MContext ctx, T1 model, List<T2> acctList, string fieldName, List<IOValidationResultModel> validationList)
		{
			_accountBiz.CheckImportAccountExist(ctx, model, acctList, fieldName, validationList, "MItemID", false);
		}

		public string GetValidAccountNo(string str)
		{
			if (str == null)
			{
				return string.Empty;
			}
			return Regex.Match(str.Replace(".", "").Trim(), "^[\\d]+").Value;
		}

		public void ValidateVoucherCheckGroupInfo(MContext ctx, List<IOValidationResultModel> validationResult, List<GLVoucherEntryModel> entryList, List<CommandInfo> cmdList, int dataRowNo)
		{
			voucherBiz.ValidateVoucherCheckGroupInfo(ctx, validationResult, entryList, cmdList, false, false, dataRowNo);
		}

		public void ValidateVoucherCheckGroupInfo(MContext ctx, List<IOValidationResultModel> validationResult, List<GLVoucherEntryModel> entryList, int dataRowNo)
		{
			voucherBiz.ValidateVoucherCheckGroupInfo(ctx, validationResult, entryList, dataRowNo);
		}

		public void SetDimensionByName(MContext ctx, List<IOValidationResultModel> validationResult, List<GLVoucherEntryModel> entryList)
		{
			voucherBiz.SetDimensionByName(ctx, entryList, validationResult, false);
		}

		public List<string> TrimTrackPrefix(string[] importTrackNameList)
		{
			return _trackBiz.TrimTrackPrefix(importTrackNameList);
		}

		private BDContactsModel GetNewContactModel(string contactName, ContactTypeEnum contactType)
		{
			if (string.IsNullOrEmpty(contactName))
			{
				return null;
			}
			BDContactsModel bDContactsModel = new BDContactsModel();
			bDContactsModel.MIsSupplier = (contactType == ContactTypeEnum.Supplier);
			bDContactsModel.MIsCustomer = (contactType == ContactTypeEnum.Customer);
			bDContactsModel.MultiLanguage = SetMultiLang("MName", contactName);
			return bDContactsModel;
		}

		public List<KeyValuePair<int, IOSolutionConfigModel>> GetColumnMath(DataRow headerDr, List<IOSolutionConfigModel> soluConfig)
		{
			List<KeyValuePair<int, IOSolutionConfigModel>> list = new List<KeyValuePair<int, IOSolutionConfigModel>>();
			foreach (IOSolutionConfigModel item2 in soluConfig)
			{
				for (int i = 0; i < headerDr.ItemArray.Length; i++)
				{
					object obj = headerDr.ItemArray[i];
					string text = (obj == null || obj == DBNull.Value) ? string.Empty : obj.ToString();
					string[] array = null;
					if (!string.IsNullOrWhiteSpace(item2.MColumnName))
					{
						array = item2.MColumnName.Split(',');
					}
					bool flag = array != null && array.Length > 1 && array.Contains(text);
					if (!string.IsNullOrEmpty(text) && (text == HttpUtility.HtmlDecode(item2.MColumnName) | flag))
					{
						IOSolutionConfigModel iOSolutionConfigModel = item2;
						if (flag)
						{
							iOSolutionConfigModel = (item2.Clone() as IOSolutionConfigModel);
						}
						if (iOSolutionConfigModel != null)
						{
							iOSolutionConfigModel.MColumnName = text;
							KeyValuePair<int, IOSolutionConfigModel> item = new KeyValuePair<int, IOSolutionConfigModel>(i, iOSolutionConfigModel);
							list.Add(item);
						}
					}
				}
			}
			return list;
		}
	}
}
