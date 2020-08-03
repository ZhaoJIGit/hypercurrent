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
using JieNor.Megi.Core.MultiLanguage;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.COM;
using JieNor.Megi.DataModel.Enum;
using JieNor.Megi.DataModel.IO;
using JieNor.Megi.DataModel.IO.Export.DataRowModel;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.DataRepository.API;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.REG;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JieNor.Megi.BusinessService.BD
{
	public class BDItemBusiness : APIBusinessBase<BDItemModel>, IBDItemBusiness, IDataContract<BDItemModel>
	{
		private List<BDItemModel> itemList = new List<BDItemModel>();

		private List<REGTaxRateModel> taxRateList = new List<REGTaxRateModel>();

		private List<BDItemModel> _itemDataPool;

		private List<REGTaxRateModel> _taxRateDataPool;

		private readonly BDItemRepository dal = new BDItemRepository();

		private readonly BDAccountRepository acctDal = new BDAccountRepository();

		private readonly REGTaxRateRepository taxRateDal = new REGTaxRateRepository();

		private readonly BDAccountBusiness acctBiz = new BDAccountBusiness();

		private readonly REGTaxRateBusiness taxRateBiz = new REGTaxRateBusiness();

		private readonly IBASLangBusiness langBiz = new BASLangBusiness();

		protected override DataGridJson<BDItemModel> OnGet(MContext ctx, GetParam param)
		{
			APIDataPool instance = APIDataPool.GetInstance(ctx);
			_itemDataPool = instance.Items;
			_taxRateDataPool = instance.TaxRates;
			return dal.Get(ctx, param);
		}

		protected override void OnGetAfter(MContext ctx, GetParam param, APIDataPool dataPool, BDItemModel model)
		{
			BDItemModel bDItemModel = _itemDataPool.FirstOrDefault((BDItemModel f) => f.MItemID == model.MItemID);
			if (bDItemModel != null)
			{
				model.MDesc = bDItemModel.MDesc;
			}
			if (model.MPurchaseDetails != null && model.MPurchaseDetails.MTaxRate != null)
			{
				REGTaxRateModel rEGTaxRateModel = _taxRateDataPool.FirstOrDefault((REGTaxRateModel f) => f.MItemID == model.MPurchaseDetails.MTaxRate.MTaxRateID);
				if (rEGTaxRateModel != null)
				{
					model.MPurchaseDetails.MTaxRate.MName = rEGTaxRateModel.MName;
				}
				else
				{
					model.MPurchaseDetails.MTaxRate = null;
				}
			}
			if (model.MSalesDetails != null && model.MSalesDetails.MTaxRate != null)
			{
				REGTaxRateModel rEGTaxRateModel2 = _taxRateDataPool.FirstOrDefault((REGTaxRateModel f) => f.MItemID == model.MSalesDetails.MTaxRate.MTaxRateID);
				if (rEGTaxRateModel2 != null)
				{
					model.MSalesDetails.MTaxRate.MName = rEGTaxRateModel2.MName;
				}
				else
				{
					model.MSalesDetails.MTaxRate = null;
				}
			}
		}

		private void ValidateDetailInfo(MContext ctx, BDItemModel model)
		{
			if (model.MPurchaseDetails != null)
			{
				if (model.MPurchaseDetails.UpdateFieldList.Contains("MPurPrice"))
				{
					model.MPurPrice = model.MPurchaseDetails.MUnitPrice;
					model.UpdateFieldList.Add("MPurPrice");
					model.Validate(ctx, model.MPurPrice < decimal.Zero, "UnitPriceIsNegative", "单价不能为负数", LangModule.Common);
				}
				APIValidator.ValidateTaxRate(ctx, model, model.MPurchaseDetails, false, "MTaxRate", taxRateList, BasicDataReferenceTypeEnum.ReferenceOnly);
			}
			else if (model.UpdateFieldList.Contains("MPurchaseDetails"))
			{
				RemoveDetailInfo(model, false);
			}
			if (model.MSalesDetails != null)
			{
				if (model.MSalesDetails.UpdateFieldList.Contains("MSalPrice"))
				{
					model.MSalPrice = model.MSalesDetails.MUnitPrice;
					model.UpdateFieldList.Add("MSalPrice");
					model.Validate(ctx, model.MSalPrice < decimal.Zero, "UnitPriceIsNegative", "单价不能为负数", LangModule.Common);
				}
				APIValidator.ValidateTaxRate(ctx, model, model.MSalesDetails, true, "MTaxRate", taxRateList, BasicDataReferenceTypeEnum.ReferenceOnly);
			}
			else if (model.UpdateFieldList.Contains("MSalesDetails"))
			{
				RemoveDetailInfo(model, true);
			}
		}

		private void RemoveDetailInfo(BDItemModel model, bool isSale)
		{
			string item = isSale ? "MSalPrice" : "MPurPrice";
			string item2 = isSale ? "MSalTaxTypeID" : "MPurTaxTypeID";
			model.UpdateFieldList.Add(item);
			model.UpdateFieldList.Add(item2);
		}

		protected override void OnPostValidate(MContext ctx, PostParam<BDItemModel> param, APIDataPool dataPool, BDItemModel model, bool isPut, ref Dictionary<string, List<string>> validNameList, ref Dictionary<string, string> updNameList)
		{
			List<BDAccountEditModel> accounts = dataPool.Accounts;
			bool flag = string.IsNullOrWhiteSpace(model.MItemID) || itemList.All((BDItemModel f) => f.MItemID != model.MItemID);
			model.Validate(ctx, model.UpdateFieldList.Contains("MNumber") && string.IsNullOrEmpty(model.MNumber), "FieldEmpty", "“{0}”不能为空。", LangModule.Common, "Code");
			model.Validate(ctx, flag && !model.UpdateFieldList.Contains("MNumber"), "FieldMustProvide", "必须提供“{0}”。", LangModule.Common, "Code");
			ValidateDetailInfo(ctx, model);
			BDItemModel bDItemModel = APIValidator.MatchByIdThenName(ctx, isPut, model, itemList, COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "ItemCodeUsed", "“商品代码“{0}”在系统中已经存在"), ref updNameList, "MNumber", "MItemID", false, BasicDataReferenceTypeEnum.NotReference, COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "EditDisabledItem", "禁用状态的商品不能被更新"), true, null);
			APIValidator.ValidateDuplicateName(ctx, model, COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "ItemCodeUsed", "“商品代码“{0}”在系统中已经存在"), ref validNameList, ref updNameList, "MNumber", "MItemID");
			if (!string.IsNullOrWhiteSpace(bDItemModel?.MItemID))
			{
				if (!model.UpdateFieldList.Contains("MIsExpenseItem"))
				{
					model.MIsExpenseItem = bDItemModel.MIsExpenseItem;
				}
				APIValidator.SetMatchMultiFieldInfo(model.MultiLanguage, bDItemModel.MItemID, bDItemModel.MultiLanguage);
			}
			if (string.IsNullOrWhiteSpace(model.MItemID) || model.IsNew)
			{
				if (model.MultiLanguage == null || model.MultiLanguage.Count == 0)
				{
					model.MultiLanguage = new List<MultiLanguageFieldList>
					{
						base.GetMultiLanguageList("MDesc", string.Empty)
					};
				}
				model.MIsActive = true;
			}
			else
			{
				model.UpdateFieldList.Remove("MIsActive");
			}
			APIValidator.ValidateAccountNumber(ctx, model, accounts, "MIncomeAccountCode", null, null, null);
			if (model.MIsExpenseItem)
			{
				model.MInventoryAccountCode = string.Empty;
				model.MCostAccountCode = string.Empty;
				model.UpdateFieldList.Add("MInventoryAccountCode");
				APIValidator.ValidateAccountNumber(ctx, model, accounts, "MExpenseAccountCode", "MCostAccountCode", null, null);
			}
			else
			{
				model.MExpenseAccountCode = string.Empty;
				if (!model.UpdateFieldList.Contains("CostAccountCode"))
				{
					model.UpdateFieldList.Remove("MCostAccountCode");
				}
				APIValidator.ValidateAccountNumber(ctx, model, accounts, "MInventoryAccountCode", null, null, null);
				APIValidator.ValidateAccountNumber(ctx, model, accounts, "MCostAccountCode", null, null, null);
			}
		}

		protected override void OnPostBefore(MContext ctx, PostParam<BDItemModel> param, APIDataPool dataPool)
		{
			itemList = ModelInfoManager.GetDataModelList<BDItemModel>(ctx, new SqlWhere(), false, false);
			taxRateList = ModelInfoManager.GetDataModelList<REGTaxRateModel>(ctx, new SqlWhere(), false, false);
		}

		protected override List<CommandInfo> OnPostGetCmd(MContext ctx, PostParam<BDItemModel> param, APIDataPool dataPool, BDItemModel model)
		{
			List<CommandInfo> insertOrUpdateCmd = ModelInfoManager.GetInsertOrUpdateCmd<BDItemModel>(ctx, model, model.UpdateFieldList, true);
			if (!param.IsPut && model.IsNew)
			{
				itemList.Add(model);
			}
			if (model.MIsExpenseItem)
			{
				model.MCostAccountCode = string.Empty;
			}
			return insertOrUpdateCmd;
		}

		protected override void OnDeleteValidate(MContext ctx, DeleteParam param, APIDataPool dataPool, BDItemModel model)
		{
			if (model.Validate(ctx, !model.MIsActive, "DelDisabledItem", "禁用状态商品不能被删除", LangModule.BD))
			{
				List<string> list = new List<string>();
				OperationResult operationResult = BDRepository.IsCanDelete(ctx, "Item", model.MItemID, out list);
				if (list.Count == 0)
				{
					string format = (!string.IsNullOrWhiteSpace(model.MDesc)) ? ((ctx.MLCID == "0x0009") ? "{0}:{1}" : "{0}:{1}") : "{0}";
					model.Validate(ctx, true, "ItemHasReference", "“{0}”商品已被使用，不能被删除。", LangModule.Common, string.Format(format, model.MNumber, model.MDesc));
				}
			}
		}

		protected override List<CommandInfo> OnDeleteGetCmd(MContext ctx, DeleteParam param, APIDataPool dataPool, BDItemModel model)
		{
			return ModelInfoManager.GetDeleteCmd<BDItemModel>(ctx, model.MItemID);
		}

		protected override void OnPostAfter(MContext ctx, PostParam<BDItemModel> param, APIDataPool dataPool)
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
				base.SetWhereString(param2, "ItemID", list, true);
				IBasicBusiness<BDItemModel> basicBusiness = new BDItemBusiness();
				DataGridJson<BDItemModel> dataGridJson = basicBusiness.Get(ctx, param2);
				List<BDItemModel> list2 = new List<BDItemModel>();
				for (int i = 0; i < param.DataList.Count; i++)
				{
					BDItemModel model = param.DataList[i];
					if (model.ValidationErrors.Count > 0)
					{
						list2.Add(model);
					}
					else
					{
						BDItemModel item = dataGridJson.rows.FirstOrDefault((BDItemModel a) => a.MItemID == model.MItemID);
						list2.Add(item);
					}
				}
				param.DataList = list2;
			}
		}

		[Obsolete]
		public List<BDItemModel> GetListByWhere(MContext ctx, string filterString)
		{
			return dal.GetListByWhere(ctx, filterString);
		}

		public List<BDItemModel> GetItemList(MContext ctx, BDItemListFilterModel filter)
		{
			return dal.GetItemList(ctx, filter);
		}

		public DataGridJson<BDItemModel> GetPageList(MContext ctx, BDItemListFilterModel filter)
		{
			return dal.GetPageList(ctx, filter);
		}

		public BDItemModel GetEditInfo(MContext ctx, BDItemModel info)
		{
			BDItemModel modelByKey = dal.GetModelByKey(info.MItemID, ctx);
			if (modelByKey != null && ctx.MRegProgress == 15)
			{
				BDAccountRepository bDAccountRepository = new BDAccountRepository();
				SqlWhere filter = new SqlWhere();
				List<BDAccountModel> baseBDAccountList = bDAccountRepository.GetBaseBDAccountList(ctx, filter, false, null);
				if (baseBDAccountList != null && baseBDAccountList.Count() > 0)
				{
					string code = (modelByKey.MInventoryAccountCode == null) ? "1405" : modelByKey.MInventoryAccountCode;
					BDAccountModel bDAccountModel = (from x in baseBDAccountList
					where x.MCode == code
					select x).FirstOrDefault();
					modelByKey.MInventoryAccountId = bDAccountModel?.MItemID;
					code = ((modelByKey.MIncomeAccountCode == null) ? "6001" : modelByKey.MIncomeAccountCode);
					bDAccountModel = (from x in baseBDAccountList
					where x.MCode == code
					select x).FirstOrDefault();
					modelByKey.MIncomeAccountId = bDAccountModel?.MItemID;
					code = ((modelByKey.MCostAccountCode == null) ? "6401" : modelByKey.MCostAccountCode);
					bDAccountModel = (from x in baseBDAccountList
					where x.MCode == code
					select x).FirstOrDefault();
					modelByKey.MCostAccountId = bDAccountModel?.MItemID;
				}
			}
			return modelByKey;
		}

		public OperationResult ItemInfoUpd(MContext ctx, BDItemModel item)
		{
			OperationResult operationResult = new OperationResult();
			SqlWhere sqlWhere = new SqlWhere().Equal("MNumber", HtmlHelper.Encode(item.MNumber)).Equal("MOrgID", ctx.MOrgID).Equal("MIsDelete", 0);
			if (!item.IsNew && !string.IsNullOrEmpty(item.MItemID))
			{
				sqlWhere.NotEqual("MItemID", item.MItemID);
			}
			if (item.MIsExpenseItem)
			{
				item.MInventoryAccountCode = null;
			}
			List<string> codeList = new List<string>
			{
				item.MInventoryAccountCode,
				item.MIncomeAccountCode,
				item.MCostAccountCode
			};
			if (!acctBiz.CheckAccountExist(ctx, codeList, operationResult))
			{
				return operationResult;
			}
			if (ExistsByFilter(ctx, sqlWhere))
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "InventoryItemCodeExist", "Inventory Item Code already exists");
			}
			else
			{
				operationResult = dal.UpdateItemModel(ctx, item);
				operationResult.ObjectID = item.MItemID;
			}
			return operationResult;
		}

		public BDIsCanDeleteModel IsCanDeleteOrInactive(MContext ctx, ParamBase param)
		{
			return BDRepository.IsCanDeleteOrInactive(ctx, "Item", param.KeyIDs.Split(',').ToList(), param.IsDelete);
		}

		public OperationResult DeleteItemList(MContext ctx, ParamBase param)
		{
			return dal.DeleteList(ctx, param);
		}

		public bool IsItemCodeExists(MContext ctx, string id, BDItemModel model)
		{
			return dal.IsItemCodeExists(ctx, id, model);
		}

		public OperationResult IsImportItemsCodeExist(MContext ctx, List<BDItemModel> list)
		{
			return dal.IsImportItemsCodeExist(ctx, list);
		}

		public OperationResult ImportItemList(MContext ctx, List<BDItemModel> list)
		{
			OperationResult operationResult = new OperationResult();
			List<IOValidationResultModel> list2 = new List<IOValidationResultModel>();
			List<BDAccountModel> bDAccountList = acctBiz.GetBDAccountList(ctx, new BDAccountListFilterModel
			{
				IncludeDisable = true
			}, false, false);
			List<REGTaxRateModel> listIgnoreLocale = taxRateDal.GetListIgnoreLocale(ctx, true);
			foreach (BDItemModel item in list)
			{
				COMModelValidateHelper.ValidateModel(ctx, item, list2, IOValidationTypeEnum.InventoryItem);
				acctBiz.CheckImportAccountExist(ctx, item, bDAccountList, "MInventoryAccountCode", list2, "MCode", false);
				acctBiz.CheckImportAccountExist(ctx, item, bDAccountList, "MIncomeAccountCode", list2, "MCode", false);
				acctBiz.CheckImportAccountExist(ctx, item, bDAccountList, "MCostAccountCode", list2, "MCode", false);
				taxRateBiz.CheckTaxRateExist(ctx, item, list2, listIgnoreLocale, "MSalTaxTypeID", -1);
				taxRateBiz.CheckTaxRateExist(ctx, item, list2, listIgnoreLocale, "MPurTaxTypeID", -1);
			}
			base.SetValidationResult(ctx, operationResult, list2, false);
			if (!operationResult.Success)
			{
				return operationResult;
			}
			return dal.ImportItemList(ctx, list);
		}

		public List<IOTemplateConfigModel> GetTemplateConfig(MContext ctx, bool isFromExcel = false)
		{
			List<IOTemplateConfigModel> list = new List<IOTemplateConfigModel>();
			string[] megiLangTypes = ServerHelper.MegiLangTypes;
			string[] source = new string[2]
			{
				"0x7804",
				"0x7C04"
			};
			string comment = source.Contains(ctx.MLCID) ? string.Empty : " ";
			COMLangInfoModel cOMLangInfoModel = new COMLangInfoModel(LangModule.BD, "UnitPrice", "Unit Price", null);
			COMLangInfoModel cOMLangInfoModel2 = new COMLangInfoModel(LangModule.BD, "TaxRate", "Tax Rate", null);
			list.AddRange(new List<IOTemplateConfigModel>
			{
				new IOTemplateConfigModel("MNumber", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.BD, "ItemCode", "Item Code", null)
				}), true, false, 2, null),
				new IOTemplateConfigModel("MDesc", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.BD, "ItemName", "商品名称", null)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MPurPrice", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[2]
				{
					new COMLangInfoModel(LangModule.BD, "ForPurchases", "For Purchases", comment),
					cOMLangInfoModel
				}), IOTemplateFieldType.Decimal, false, false, 4, null),
				new IOTemplateConfigModel("MPurTaxTypeID", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[2]
				{
					new COMLangInfoModel(LangModule.BD, "ForPurchases", "For Purchases", comment),
					cOMLangInfoModel2
				}), false, false, 2, null),
				new IOTemplateConfigModel("MSalPrice", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[2]
				{
					new COMLangInfoModel(LangModule.BD, "ForSales", "For Sales", comment),
					cOMLangInfoModel
				}), IOTemplateFieldType.Decimal, false, false, 4, null),
				new IOTemplateConfigModel("MSalTaxTypeID", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[2]
				{
					new COMLangInfoModel(LangModule.BD, "ForSales", "For Sales", comment),
					cOMLangInfoModel2
				}), false, false, 2, null)
			});
			if (ctx.MEnabledModules.Contains(ModuleEnum.GL))
			{
				list.AddRange(new List<IOTemplateConfigModel>
				{
					new IOTemplateConfigModel("MIsExpenseItem", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
					{
						new COMLangInfoModel(LangModule.BD, "IsExpenseItem", "Expense item", null)
					}), false, false, 2, null),
					new IOTemplateConfigModel("MIncomeAccountCode", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
					{
						new COMLangInfoModel(LangModule.BD, "IncomeAccount", "Income account", null)
					}), false, false, 2, null),
					new IOTemplateConfigModel("MCostAccountCode", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
					{
						new COMLangInfoModel(LangModule.Acct, "ExpenseAccount", "Expense Account", null)
					}), false, false, 2, null),
					new IOTemplateConfigModel("MInventoryAccountCode", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
					{
						new COMLangInfoModel(LangModule.BD, "InventoryAccount", "Inventory Account", null)
					}), false, false, 2, null)
				});
			}
			return list;
		}

		public List<ImportTemplateDataSource> GetTemplateBasicData(MContext ctx, Dictionary<string, string> columnList, bool isFromExcel = false, Dictionary<string, string[]> exampleDataList = null)
		{
			List<ImportTemplateDataSource> list = new List<ImportTemplateDataSource>();
			BDItemModel emptyDataEditModel = ModelInfoManager.GetEmptyDataEditModel<BDItemModel>(ctx);
			List<string> fieldList = (from f in emptyDataEditModel.MultiLanguage
			where columnList.Keys.Contains(f.MFieldName)
			select f.MFieldName).ToList();
			List<BASLangModel> orgLangList = langBiz.GetOrgLangList(ctx);
			List<ImportDataSourceInfo> dicLangList = new List<ImportDataSourceInfo>();
			orgLangList.ForEach(delegate(BASLangModel f)
			{
				dicLangList.Add(new ImportDataSourceInfo
				{
					Key = f.LangID,
					Value = f.LangName
				});
			});
			ImportTemplateDataSource importTemplateDataSource = new ImportTemplateDataSource();
			importTemplateDataSource.FieldType = ImportTemplateColumnType.MultiLanguage;
			importTemplateDataSource.FieldList = fieldList;
			importTemplateDataSource.DataSourceList = dicLangList;
			list.Add(importTemplateDataSource);
			List<REGTaxRateModel> list2 = taxRateDal.GetList(ctx, null, false);
			List<ImportDataSourceInfo> dicTaxList = new List<ImportDataSourceInfo>();
			list2.ForEach(delegate(REGTaxRateModel f)
			{
				dicTaxList.Add(new ImportDataSourceInfo
				{
					Key = f.MItemID,
					Value = $"{f.MName}({f.MEffectiveTaxRate.ToOrgDigitalFormat(ctx)}%)"
				});
			});
			importTemplateDataSource = new ImportTemplateDataSource(true);
			importTemplateDataSource.FieldType = ImportTemplateColumnType.TaxRate;
			importTemplateDataSource.FieldList = new List<string>
			{
				"MPurTaxTypeID",
				"MSalTaxTypeID"
			};
			importTemplateDataSource.DataSourceList = dicTaxList;
			list.Add(importTemplateDataSource);
			if (ctx.MEnabledModules.Contains(ModuleEnum.GL))
			{
				string text = COMMultiLangRepository.GetText(ctx.MLCID, LangKey.Yes);
				string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangKey.No);
				List<ImportDataSourceInfo> list3 = new List<ImportDataSourceInfo>();
				list3.Add(new ImportDataSourceInfo
				{
					Key = "1",
					Value = text
				});
				list3.Add(new ImportDataSourceInfo
				{
					Key = "0",
					Value = text2
				});
				list.Add(new ImportTemplateDataSource(true)
				{
					FieldType = ImportTemplateColumnType.IsExpenseItem,
					FieldList = new List<string>
					{
						"MIsExpenseItem"
					},
					DataSourceList = list3
				});
				List<BDAccountModel> bDAccountList = acctBiz.GetBDAccountList(ctx, new BDAccountListFilterModel(), false, false);
				List<ImportDataSourceInfo> list4 = new List<ImportDataSourceInfo>();
				foreach (BDAccountModel item in bDAccountList)
				{
					if (!string.IsNullOrWhiteSpace(item.MCode))
					{
						list4.Add(new ImportDataSourceInfo
						{
							Key = item.MCode,
							Value = item.MFullName
						});
					}
				}
				list.Add(new ImportTemplateDataSource(true)
				{
					FieldType = ImportTemplateColumnType.Account,
					FieldList = new List<string>
					{
						"MInventoryAccountCode",
						"MIncomeAccountCode",
						"MCostAccountCode"
					},
					DataSourceList = list4
				});
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
			dictionary2.Add("MNumber", new string[1]
			{
				"1001"
			});
			dictionary2.Add("MDesc", new string[1]
			{
				"IPad Air"
			});
			dictionary2.Add("MPurPrice", new string[1]
			{
				"1999.0000"
			});
			dictionary2.Add("MSalPrice", new string[1]
			{
				"2599.0000"
			});
			List<string> alignRightFieldList = "MPurPrice,MSalPrice".Split(',').ToList();
			return new ImportTemplateModel
			{
				TemplateType = "Item",
				LocaleID = ctx.MLCID,
				ColumnList = dictionary,
				FieldConfigList = templateConfig,
				RequiredColumnList = (from f in templateConfig
				where f.MIsRequired
				select f.MFieldName).ToList(),
				TemplateDictionaryList = templateBasicData,
				ExampleDataList = dictionary2,
				AlignRightFieldList = alignRightFieldList
			};
		}

		public List<ItemRowModel> GetReportList(MContext ctx)
		{
			List<ItemRowModel> reportList = dal.GetReportList(ctx);
			List<BDAccountTypeListModel> bDAccountTypeList = acctDal.GetBDAccountTypeList(ctx, string.Empty);
			List<REGTaxRateModel> list = taxRateDal.GetList(ctx, null, false);
			List<BDAccountListModel> accountListIncludeBalance = new BDAccountBusiness().GetAccountListIncludeBalance(ctx, new SqlWhere(), false);
			foreach (ItemRowModel item in reportList)
			{
				item.IsExpenseItem = (item.MIsExpenseItem ? COMMultiLangRepository.GetText(ctx.MLCID, LangKey.Yes) : COMMultiLangRepository.GetText(ctx.MLCID, LangKey.No));
				if (accountListIncludeBalance.Any() && ctx.MEnabledModules.Contains(ModuleEnum.GL))
				{
					if (!item.MIsExpenseItem)
					{
						if (!string.IsNullOrWhiteSpace(item.MInventoryAccountCode))
						{
							BDAccountListModel bDAccountListModel = accountListIncludeBalance.FirstOrDefault((BDAccountListModel f) => f.MCode == item.MInventoryAccountCode);
							if (bDAccountListModel != null)
							{
								item.MInventoryAccountCode = bDAccountListModel.MFullName;
							}
						}
					}
					else
					{
						item.MInventoryAccountCode = string.Empty;
					}
					if (!string.IsNullOrWhiteSpace(item.MIncomeAccountCode))
					{
						BDAccountListModel bDAccountListModel2 = accountListIncludeBalance.FirstOrDefault((BDAccountListModel f) => f.MCode == item.MIncomeAccountCode);
						if (bDAccountListModel2 != null)
						{
							item.MIncomeAccountCode = bDAccountListModel2.MFullName;
						}
					}
					if (!string.IsNullOrWhiteSpace(item.MCostAccountCode))
					{
						BDAccountListModel bDAccountListModel3 = accountListIncludeBalance.FirstOrDefault((BDAccountListModel f) => f.MCode == item.MCostAccountCode);
						if (bDAccountListModel3 != null)
						{
							item.MCostAccountCode = bDAccountListModel3.MFullName;
						}
					}
				}
				IEnumerable<REGTaxRateModel> source = from f in list
				where f.MItemID == item.MPurTaxTypeID || f.MItemID == item.MSalTaxTypeID
				select f;
				if (source.Any())
				{
					REGTaxRateModel rEGTaxRateModel = source.FirstOrDefault((REGTaxRateModel f) => f.MItemID == item.MPurTaxTypeID);
					item.MPurTaxTypeID = ((rEGTaxRateModel == null) ? string.Empty : $"{rEGTaxRateModel.MName}({rEGTaxRateModel.MEffectiveTaxRate.ToOrgDigitalFormat(ctx)}%)");
					rEGTaxRateModel = source.FirstOrDefault((REGTaxRateModel f) => f.MItemID == item.MSalTaxTypeID);
					item.MSalTaxTypeID = ((rEGTaxRateModel == null) ? string.Empty : $"{rEGTaxRateModel.MName}({rEGTaxRateModel.MEffectiveTaxRate.ToOrgDigitalFormat(ctx)}%)");
				}
				else
				{
					item.MPurTaxTypeID = string.Empty;
					item.MSalTaxTypeID = string.Empty;
				}
			}
			return reportList;
		}

		public OperationResult ArchiveItem(MContext ctx, string itemIds, bool isRestore = false)
		{
			int num = dal.ArchiveItem(ctx, itemIds, isRestore);
			OperationResult operationResult = new OperationResult();
			operationResult.Success = (num > 0);
			return operationResult;
		}

		public void CheckItemExist<T>(MContext ctx, List<T> modelList, List<IOValidationResultModel> validationResult, string fieldName = "MItemID", string idFieldName = "MItemID")
		{
			List<BDItemModel> itemListIgnoreLocale = dal.GetItemListIgnoreLocale(ctx, true);
			int num = 0;
			foreach (T model in modelList)
			{
				CheckItemExist(ctx, model, ref itemListIgnoreLocale, validationResult, fieldName, idFieldName, num);
				num++;
			}
		}

		public void CheckItemExist<T>(MContext ctx, T model, ref List<BDItemModel> itemList, List<IOValidationResultModel> validationResult, string fieldName = "MItemID", string idFieldName = "MItemID", int rowIndex = -1)
		{
			if (itemList == null)
			{
				itemList = dal.GetItemListIgnoreLocale(ctx, true);
			}
			string text = string.Empty;
			string itemId = ModelHelper.GetModelValue(model, fieldName);
			if (!string.IsNullOrWhiteSpace(itemId))
			{
				BDItemModel bDItemModel = itemList.FirstOrDefault(delegate(BDItemModel f)
				{
					int result;
					if ((string.IsNullOrWhiteSpace(f.MNumber) || !(HttpUtility.HtmlDecode(f.MNumber.Trim()) == itemId)) && !itemId.StartsWith(HttpUtility.HtmlDecode(f.MNumber.Trim()) + ":"))
					{
						result = ((itemId == f.MItemID) ? 1 : 0);
						goto IL_0065;
					}
					result = 1;
					goto IL_0065;
					IL_0065:
					return (byte)result != 0;
				});
				if (bDItemModel != null)
				{
					if (!bDItemModel.MIsActive)
					{
						text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "ItemHasDisabled", "商品：{0}已禁用！");
					}
					else
					{
						ModelHelper.SetModelValue(model, fieldName, bDItemModel.MItemID, null);
					}
				}
				else
				{
					text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "ItemNotFound", "The Item:{0} can't be found!");
				}
				if (!string.IsNullOrWhiteSpace(text))
				{
					int rowIndex2 = 0;
					if (rowIndex != -1)
					{
						rowIndex2 = rowIndex;
					}
					else
					{
						int.TryParse(ModelHelper.TryGetModelValue(model, "MRowIndex"), out rowIndex2);
					}
					validationResult.Add(new IOValidationResultModel
					{
						Id = ModelHelper.GetModelValue(model, idFieldName),
						FieldType = IOValidationTypeEnum.InventoryItem,
						FieldValue = itemId,
						Message = text,
						RowIndex = rowIndex2
					});
				}
			}
		}

		public BDItemModel GetInsertItemModel(MContext ctx, string code, string desc, List<BASLangModel> languageList)
		{
			BDItemModel bDItemModel = new BDItemModel();
			bDItemModel.MItemID = UUIDHelper.GetGuid();
			bDItemModel.MDesc = desc;
			bDItemModel.MNumber = code;
			bDItemModel.IsNew = true;
			if (languageList == null)
			{
				languageList = new BASLangBusiness().GetOrgLangList(ctx);
			}
			MultiLanguageFieldList multiLanguageFieldList = new MultiLanguageFieldList();
			multiLanguageFieldList.MMultiLanguageField = new List<MultiLanguageField>();
			multiLanguageFieldList.MFieldName = "MDesc";
			foreach (BASLangModel language in languageList)
			{
				MultiLanguageField multiLanguageField = new MultiLanguageField();
				multiLanguageField.MLocaleID = language.LangID;
				multiLanguageField.MOrgID = ctx.MOrgID;
				multiLanguageField.MValue = desc;
				multiLanguageFieldList.MMultiLanguageField.Add(multiLanguageField);
			}
			bDItemModel.MultiLanguage = new List<MultiLanguageFieldList>();
			bDItemModel.MultiLanguage.Add(multiLanguageFieldList);
			return bDItemModel;
		}

		public bool Exists(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.Exists(ctx, pkID, includeDelete);
		}

		public bool ExistsByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.ExistsByFilter(ctx, filter);
		}

		public OperationResult InsertOrUpdate(MContext ctx, BDItemModel modelData, string fields = null)
		{
			return dal.InsertOrUpdate(ctx, modelData, fields);
		}

		public OperationResult InsertOrUpdateModels(MContext ctx, List<BDItemModel> modelData, string fields = null)
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

		public BDItemModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.GetDataModel(ctx, pkID, includeDelete);
		}

		public BDItemModel GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.GetDataModelByFilter(ctx, filter);
		}

		public List<BDItemModel> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelList(ctx, filter, includeDelete);
		}

		public DataGridJson<BDItemModel> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelPageList(ctx, filter, includeDelete);
		}
	}
}
